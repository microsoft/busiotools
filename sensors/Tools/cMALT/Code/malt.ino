#define I2C_PULLUP 1
#define SDA_PORT PORTC
#define SDA_PIN 7 // digital pin 30
#define SCL_PORT PORTA
#define SCL_PIN 6 // digital pin 28

#include <SoftWire.h>
SoftWire Wire = SoftWire();

#include <SerialCommand.h>
#include <SPI.h>
#include "AS73211.h"
#include "lightpanel.h"
#include "mcp4821.h"
#include "opt3001.h"
#include "TC34007.h"

#define UNREFERENCED_PARAMETER(v) (void) (v)

// Version information
#define VERSION "0001"

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

  // Turn IR sensors on
  initIRSensor(channelSelectAmbient);
  initIRSensor(channelSelectScreen);

  // Save the current configuration.
  currentSensorConfig = configReg;
  
  // Add serial commands
  SerCmd.addCommand("LIGHT", adjustLight);
  SerCmd.addCommand("READALSSENSOR", readLightSensor);
  SerCmd.addCommand("READCOLORSENSOR", readColorSensor);
  SerCmd.addCommand("READIRSENSOR", readIRSensor);
  SerCmd.addCommand("CONVERSIONTIME",  setConversionTime);
  SerCmd.addCommand("MALTVERSION", getVersion);
  SerCmd.setDefaultHandler(unrecognized);
}

void loop()
{
  // Process incoming serial commands
  SerCmd.readSerial();
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
  Serial.print("Error Code: ");
  Serial.println(E_UNRECOGNIZED_COMMAND);
}
