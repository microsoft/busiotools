// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#define I2C_PULLUP 1
#define SDA_PORT PORTC
#define SDA_PIN 7 // digital pin 30
#define SCL_PORT PORTA
#define SCL_PIN 6 // digital pin 28

#include <SoftWire.h>
SoftWire Wire = SoftWire();

#include <SerialCommand.h>
#include <SPI.h>
#include "malterror.h"

#define UNREFERENCED_PARAMETER(v) (void) (v)

// Version information
#define VERSION "0001"

const uint8_t muxAddr = 0x70; //P14MSD5V9540B device address
const uint8_t lightSensorAddr = 0x44; // OPT3001 device address
const uint8_t colorSensorAddr = 0x29; // TCS3272 device address

const int ambientSensorId = 1;
const int screenSensorId = 2;
const int channelSelectAmbient = 0x04;  // channel 0 enabled
const int channelSelectScreen = 0x05; // channel 1 enabled

// Light sensor OPT3001 Configuration
typedef struct _SENSOR_CONFIG_REGISTER
{
   union
   {
      uint8_t msbAsUInt8;
      struct
      {        
        uint8_t rangeNumber : 4;
        uint8_t conversionTime : 1;
        uint8_t conversionMode : 2;
        uint8_t overflowFlag : 1;
      };
   };
   union
   {
      uint8_t lsbAsUInt8;
      struct
      {
        uint8_t conversionReady : 1;
        uint8_t flagHigh : 1;
        uint8_t flagLow : 1;
        uint8_t latch : 1;
        uint8_t polarity : 1;
        uint8_t maskExponent : 1;
        uint8_t faultCount : 2;
      };
   };
} SENSOR_CONFIG_REGISTER, *PSENSOR_CONFIG_REGISTER;

SENSOR_CONFIG_REGISTER currentSensorConfig;

typedef enum _CONVERSION_TIME
{  
  conversionTime100ms = 0,
  conversionTime800ms = 1
} CONVERSION_TIME;

// Light Sensor registers
const uint8_t resultRegister = 0x00;
const uint8_t configRegister = 0x01;

// Color Sensor registers
// command bits = b10100000, auto-increment protocol transaction (read successive bytes)
// register address + command bits
const uint8_t enableAddr = 0xa0;  
const uint8_t ATimeAddr = 0xa1;
const uint8_t controlAddr = 0xaf;
const uint8_t configAddr = 0xad;
const uint8_t colorAddr = 0xb4;

// DAC MCP4821 information:
const uint8_t chipSelectPin = 53; // DAC chip select (CS) attached to pin 53

//     DAC MCP4821 write command register: 
//        +----+----+----+------+-----+-----+----+----+----+----+----+----+----+----+----+----+
//   Bit  | 15 | 14 | 13 |  12  | 11  | 10  | 9  | 8  | 7  | 6  | 5  | 4  | 3  | 2  | 1  | 0  |
//        +----+----+----+------+-----+-----+----+----+----+----+----+----+----+----+----+----+
//   Data |  * | -- | GA | SHDN | D11 | D10 | D9 | D8 | D7 | D6 | D5 | D4 | D3 | D2 | D1 | D0 |
//        +----+----+----+------+-----+-----+----+----+----+----+----+----+----+----+----+----+

typedef enum _SHDN
{
  shdnShutdown = 0,
  shdnActive = 1
} SHDN;

typedef enum _GA
{
  ga1x = 1, // 1x (Vout = Vref * D/4096)
  ga2x = 0  // 2x (Vout = 2*Vref * D/4096) where internal Vref = 2.048V
} GA;

typedef enum _DAC_WRITE_COMMAND
{
  dacWrite = 0,
  dacIgnore = 1
} DAC_WRITE_COMMAND;

typedef union _DAC_WRITE_REGISTER
{
  uint16_t asUInt16;
  struct
  {
    uint16_t data : 12;
    SHDN shdn : 1;
    GA ga : 1;
    uint8_t : 1;
    uint8_t writeToDAC : 1;
  };
} DAC_WRITE_REGISTER, *PDAC_WRITE_REGISTER;

SerialCommand SerCmd;

void setup()
{  
  SENSOR_CONFIG_REGISTER configReg;
  
  // Prepare Serial
  Serial.begin(9600);
  
  // Prepare I2C (connected to light/color sensors)
  Wire.begin();
  Wire.setClock(100000);

  // Prepare SPI (connected to light panel)
  pinMode(chipSelectPin, OUTPUT);
  digitalWrite(chipSelectPin, HIGH);
  SPI.begin();
  SPI.beginTransaction(SPISettings(1000000, MSBFIRST, SPI_MODE0));

  // Setup sensor config register  
  configReg.msbAsUInt8 = 0;
  configReg.lsbAsUInt8 = 0;
  configReg.latch = 0; // TODO: We will turn on interrupts later; for now, leave it.
  configReg.conversionMode = 0b11; // Continuous conversions
  configReg.conversionTime = conversionTime800ms; // Default to the 800ms conversion time - the upper edge may ask to change it later
  configReg.rangeNumber = 0b1100; // Automatic

  // Turn light sensors on and set them to read continuously
  initLightSensor(channelSelectAmbient, configRegister, configReg.msbAsUInt8, configReg.lsbAsUInt8);
  initLightSensor(channelSelectScreen, configRegister, configReg.msbAsUInt8, configReg.lsbAsUInt8);

  // Turn color sensors on
  initColorSensor(channelSelectAmbient);
  initColorSensor(channelSelectScreen);

  // Save the current configuration.
  currentSensorConfig = configReg;
  
  // Add serial commands
  SerCmd.addCommand("LIGHT", adjustLight);
  SerCmd.addCommand("READALSSENSOR", readLightSensor);
  SerCmd.addCommand("READCOLORSENSOR", readColorSensor);
  SerCmd.addCommand("CONVERSIONTIME",  setConversionTime);
  SerCmd.addCommand("MALTVERSION", getVersion);
  SerCmd.setDefaultHandler(unrecognized);
}

void loop()
{
  // Process incoming serial commands
  SerCmd.readSerial();
}

// AdjustLight serial command takes one argument - the level to set the light to. This can be calculated using the info from the light and DAC MCP4821 datasheets.
// The light panel's dimming voltage range is from .25 to 1.3 V (but it can handle up to 8V).
void adjustLight()
{
  char* arg;
  uint16_t lightLevel;
  DAC_WRITE_REGISTER writeReg;
  MALTERROR err = E_SUCCESS;
  
  arg = SerCmd.next();
  if (arg == NULL)
  {
    // Error: Provide a decimal light value to use to set the light panel intensity
    err = E_INVALID_PARAM;
    goto Exit;
  }

  // It's up to the software to provide a reasonable level which the panel/voltage can support.
  lightLevel = atoi(arg);
  
  // Write to the DAC
  writeReg.asUInt16 = 0;
  writeReg.data = lightLevel;
  writeReg.shdn = shdnActive;
  writeReg.ga = ga1x; // TODO - in the future, the software should be able to set this if it wants a higher voltage.
  writeReg.writeToDAC = dacWrite;
  
  // Write to the DAC
  digitalWrite(chipSelectPin, LOW);
  SPI.transfer16(writeReg.asUInt16);
  digitalWrite(chipSelectPin, HIGH);

Exit:
  Serial.println(err);
}

// We choose 800ms as our initial conversion time for the OPT3001. If the upper layer (user or software) wishes to change it at any point, it may.
// If a measurement conversion is in progress when the configuration register is written, the active measurement conversion immediately aborts (per OPT3001 datasheet)
void setConversionTime()
{
  char* arg;
  uint16_t conversionTime;
  SENSOR_CONFIG_REGISTER newSensorConfig = currentSensorConfig;
  MALTERROR err = E_SUCCESS;
  
  arg = SerCmd.next();
  if (arg == NULL)
  {
    // Error: Provide a conversion time in ms. Example: CONVERSIONTIME 100 will set the conversion time to 100ms. OPT3001 supports 800 or 100 ms conversion time
    err = E_INVALID_PARAM;
    goto Exit;
  }

  // For OPT3001, the conversion time may either be 100ms or 800ms.
  conversionTime = atoi(arg);

  if (conversionTime != 800 && conversionTime != 100)
  {
    // Error: OPT3001 supports 800 or 100 ms conversion time. Example CONVERSIONTIME 100 will set the conversion time to 100ms.
    err = E_INVALID_PARAM;
    goto Exit;    
  }

  // Change only the conversion time in the config register.
  newSensorConfig.conversionTime = (conversionTime == 800) ? conversionTime800ms : conversionTime100ms;
  
  initLightSensor(channelSelectAmbient, configRegister, newSensorConfig.msbAsUInt8, newSensorConfig.lsbAsUInt8);
  initLightSensor(channelSelectScreen, configRegister, newSensorConfig.msbAsUInt8, newSensorConfig.lsbAsUInt8);

Exit:
  Serial.println(err);
}

// ReadLightSensor serial command takes one argument - which sensor to read from
// 1 for the ambient light sensor facing away from the screen, 2 for the screen-facing sensor
void readLightSensor()
{
  char* arg;
  int sensorId;
  uint8_t channelSelect;
  uint8_t MSB;
  uint8_t LSB;
  uint16_t result;
  uint16_t exponent;
  MALTERROR err = E_SUCCESS;

  arg = SerCmd.next();
  if (arg == NULL)
  {
    // Error: Provide which sensor to read from. 1 for sensor 1 (should be facing away from the screen to measure ambient light), 2 for Sensor 2 (should be placed on the screen)
    err = E_INVALID_PARAM;
    goto Exit;
  }
  
  sensorId = atoi(arg);
  if(sensorId != screenSensorId && sensorId != ambientSensorId)
  {
    // Invalid sensor ID provided. Use 1 for sensor 1 (should be facing away from the screen to measure ambient light), 2 for Sensor 2 (should be placed on the screen)
    err = E_INVALID_PARAM;
    goto Exit;
  }

  channelSelect = (sensorId == screenSensorId) ? channelSelectScreen : channelSelectAmbient;

  // Read the value of that sensor
  readFromLightSensor(channelSelect, &MSB, &LSB);

Exit:
  // Always write the error code first
  Serial.println(err);
  
  // Write the result to serial out.
  // In the failure case, the exponent and result will be 0. Software must check the error before using these values.
  result = MSB & 0x0F; 
  result = (result << 8) | LSB;
  exponent = MSB >> 4;
  
  Serial.println(exponent, DEC);
  Serial.println(result, DEC);
}

// ReadColorSensor serial command takes one argument - which sensor to read from
// 1 for the ambient color sensor facing away from the screen, 2 for the screen-facing sensor
void readColorSensor()
{
  char* arg;
  int sensorId;
  uint8_t channelSelect;
  uint8_t clearLow;
  uint8_t clearHigh;
  uint8_t redLow;
  uint8_t redHigh;
  uint8_t greenLow;
  uint8_t greenHigh;
  uint8_t blueLow;
  uint8_t blueHigh;
  uint16_t clear;
  uint16_t red;
  uint16_t green;
  uint16_t blue;
  MALTERROR err = E_SUCCESS;

  arg = SerCmd.next();
  if (arg == NULL)
  {
    // Error: Provide which sensor to read from. 1 for sensor 1 (should be facing away from the screen to measure ambient color), 2 for Sensor 2 (should be placed on the screen)
    err = E_INVALID_PARAM;
    goto Exit;
  }
  
  sensorId = atoi(arg);
  if(sensorId != screenSensorId && sensorId != ambientSensorId)
  {
    // Invalid sensor ID provided. Use 1 for sensor 1 (should be facing away from the screen to measure ambient light), 2 for Sensor 2 (should be placed on the screen)
    err = E_INVALID_PARAM;
    goto Exit;
  }

  channelSelect = (sensorId == screenSensorId) ? channelSelectScreen : channelSelectAmbient;

  // Read the value of that sensor
  readFromColorSensor(channelSelect, &clearLow, &clearHigh, &redLow, &redHigh, &greenLow, &greenHigh, &blueLow, &blueHigh);

Exit:
  // Always write the error code first
  Serial.println(err);
  
  // Write the result to serial out.
  clear = clearHigh & 0x0f;
  clear = (clear << 8) | clearLow;
  red = redHigh & 0x0f;
  red = (red << 8) | redLow;
  green = greenHigh & 0x0f;
  green = (green << 8) | greenLow;
  blue = blueHigh & 0x0f;
  blue = (blue << 8) | blueLow;

  Serial.println(clear);
  Serial.println(red);
  Serial.println(green);
  Serial.println(blue);
}

//
// Get the current version and writes it to serial out
//
void getVersion()
{
  // Always succeed.
  Serial.println(E_SUCCESS);
  Serial.println(F(VERSION));
}

//
// Default command handler which is called for any unrecognized serial command.
//
void unrecognized (const char* command) 
{
  UNREFERENCED_PARAMETER(command);
  Serial.println(E_UNRECOGNIZED_COMMAND);
}

//
// HELPERS
//
void initLightSensor(char channelSelect, char reg, char MSByte, char LSByte)
{
  // Select the corresponding channel
  writeToDevice(muxAddr, channelSelect);

  Wire.beginTransmission(lightSensorAddr);
  Wire.write(reg);
  Wire.write(MSByte);
  Wire.write(LSByte);
  Wire.endTransmission();
}

void initColorSensor(char channelSelect)
{
  // Select the corresponding channel
  writeToDevice(muxAddr, channelSelect);

  // ATimeAddress
  Wire.beginTransmission(colorSensorAddr);
  Wire.write(ATimeAddr);
  Wire.write(0X10);
  Wire.endTransmission();

  // ConfigAddress
  Wire.beginTransmission(colorSensorAddr);
  Wire.write(configAddr);
  Wire.write(0X00);
  Wire.endTransmission();

  // ControlAddress
  Wire.beginTransmission(colorSensorAddr);
  Wire.write(controlAddr);
  Wire.write(0X00);
  Wire.endTransmission();
  
  // EnableAddress
  Wire.beginTransmission(colorSensorAddr);
  Wire.write(enableAddr);
  Wire.write(0X03);
  Wire.endTransmission();
}

// Reads 8 bytes of data from the color sensor
void readFromColorSensor(char channelSelect, uint8_t* clearLow, uint8_t* clearHigh, uint8_t* redLow, uint8_t* redHigh, uint8_t* greenLow, uint8_t* greenHigh, uint8_t* blueLow, uint8_t* blueHigh)
{
  // Select the corresponding channel
  writeToDevice(muxAddr, channelSelect);

  // Enable the register for read
  writeToDevice(colorSensorAddr, colorAddr);  

  uint8_t numBytes = 8;
  Wire.requestFrom(colorSensorAddr, numBytes); 
  *clearLow = Wire.read();
  *clearHigh = Wire.read();
  *redLow = Wire.read();
  *redHigh = Wire.read();
  *greenLow = Wire.read();
  *greenHigh = Wire.read();    
  *blueLow = Wire.read();
  *blueHigh = Wire.read();
}

// Reads 2 bytes of data from the light sensor
void readFromLightSensor(char channelSelect, uint8_t* MSB, uint8_t* LSB)
{
  // Select the corresponding channel
  writeToDevice(muxAddr, channelSelect);

  // Enable the register for read
  writeToDevice(lightSensorAddr, resultRegister);

  // Request 2 bytes from the register, OPT3001 gives us the MSB first
  uint8_t numBytes = 2;
  Wire.requestFrom(lightSensorAddr, numBytes); 
  *MSB = Wire.read();
  *LSB = Wire.read();
}

void writeToDevice(char deviceAddr, char writeData)
{
  Wire.beginTransmission(deviceAddr);
  Wire.write(writeData);
  Wire.endTransmission();
}
