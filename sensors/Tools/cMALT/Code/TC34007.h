#include "helpers.h"


//Color Sensor registers
const uint8_t IRAddr = 0xC0; //Selecting IR values instead of clear values
const uint8_t EnableAddr = 0x80; //Responsible for powering on the device and setting sleep times
const uint8_t TimeAddr = 0x81; //Controls the internal integration time of the RGBC channel ADCs
const uint8_t ControlAddr = 0x8F; //Contols magnitude of gain

void initIRSensor(char channelSelect)
{
  // Select the corresponding channel
  writeToDevice(muxAddr, channelSelect);

  //default set in config state when turned on
  //do configs and then change to measurement state and start measurement

  // Config Reg 1
  Wire.beginTransmission(IRSensorAddr);
  Wire.write(EnableAddr);
  Wire.write(0x03); //Powers chip and ADC on
  Wire.endTransmission();

  // Config Reg 2
  Wire.beginTransmission(IRSensorAddr);
  Wire.write(IRAddr);
  Wire.write(0x00); //IR and Clear color data share a register. Sending this value selects IR. (0x00 selects clear)
  Wire.endTransmission();

   // Config Reg 3
  Wire.beginTransmission(IRSensorAddr);
  Wire.write(TimeAddr);
  Wire.write(0xF6); //Sets integration time to 27.8ms (ATIME register)
  Wire.endTransmission();

  // Config Reg 4
  Wire.beginTransmission(IRSensorAddr);
  Wire.write(ControlAddr);
  Wire.write(0x02); //Sets gain to 16x (AGAIN register)
  Wire.endTransmission();
}



// Reads 8 bytes of data from the color sensor
void readFromIRSensor(char channelSelect, uint8_t* ClearLow, uint8_t* ClearHigh, uint8_t* BlueLow, uint8_t* BlueHigh, uint8_t* RedLow, uint8_t* RedHigh, uint8_t* GreenLow, uint8_t* GreenHigh)
{
  // Select the corresponding channel
  writeToDevice(muxAddr, channelSelect);

  // Write register to begin read from
  writeToDevice(IRSensorAddr, 0x94);  

  // Enable read
  Wire.requestFrom(IRSensorAddr, 8);
  
  *ClearLow = Wire.read();
  *ClearHigh = Wire.read();
  *RedLow = Wire.read();
  *RedHigh = Wire.read();
  *GreenLow = Wire.read();
  *GreenHigh = Wire.read();
  *BlueLow = Wire.read();
  *BlueHigh = Wire.read();

}



// READIRSENSOR serial command takes one argument - which sensor to read from
// 1 for the ambient color sensor facing away from the screen, 2 for the screen-facing sensor
void readIRSensor()
{
  char* arg;
  int sensorId;
  uint8_t channelSelect;
  uint8_t ClearHigh;
  uint8_t ClearLow;
  uint8_t BlueHigh;
  uint8_t BlueLow;
  uint8_t GreenLow;
  uint8_t GreenHigh;
  uint8_t RedHigh;
  uint8_t RedLow;
  int Clear;
  int Blue;
  int Green;
  int Red;
  float ratio;
  float temp;
  float IRx;
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

  readFromIRSensor(channelSelect, &ClearLow, &ClearHigh, &BlueLow, &BlueHigh, &RedLow, &RedHigh, &GreenLow, &GreenHigh);

  Clear = ClearHigh & 0x0f;
  Clear = (Clear << 8) | ClearLow;
  Blue = BlueHigh & 0x0f;
  Blue = (Blue << 8) | BlueLow;
  Red = RedHigh & 0x0f;
  Red = (Red << 8) | RedLow;
  Green = GreenHigh & 0x0f;
  Green = (Green << 8) | GreenLow;

  IRx = (Red+Green+Blue-Clear)/2;
  Red = Red - IRx;
  Blue = Blue - IRx;
  Green = Green - IRx;
  temp = (3852*(float(Blue)/float(Red))) + 1855;
  Serial.print("Color Temperature: ");
  Serial.print(temp);
  Serial.println("K");


  
Exit:  
  Serial.print("Error Code: ");
  Serial.println(err);
}