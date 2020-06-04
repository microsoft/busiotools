

#include "helpers.h"

// Light Sensor registers
const uint8_t resultRegister = 0x00;
const uint8_t configRegister = 0x01;


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

  // Write the result to serial out.
  // In the failure case, the exponent and result will be 0. Software must check the error before using these values.
  result = MSB & 0x0F; 
  result = (result << 8) | LSB;
  exponent = MSB >> 4;
  
  Serial.print("Exponent: ");
  Serial.println(exponent, DEC);
  Serial.print("Result: ");
  Serial.println(result, DEC);

Exit:
  Serial.print("Error Code: ");
  Serial.println(err);
}
