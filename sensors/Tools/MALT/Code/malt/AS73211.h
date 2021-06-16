#include "helpers.h"
#include "24LC08.h"

//Color Sensor registers
const uint8_t creg1Addr = 0x06; //Gain & Time
const uint8_t creg2Addr = 0x07; //EN_TM, EN_DIV, and DIV
const uint8_t creg3Addr = 0x08; //MMODE, SB, RDYOD, CCLK
const uint8_t breakAddr = 0x09; //Break time between measurements
const uint8_t edgesAddr = 0x0A; //Edge count value in SYND mode
const uint8_t osrAddr = 0x00; //Operational state register
double calnum[9];

void calibrateTopX() {
  calnum[0] = atof(SerCmd.next());
  calnum[1] = atof(SerCmd.next());
  calnum[2] = atof(SerCmd.next());
}

void calibrateTopY() {
  calnum[3] = atof(SerCmd.next());
  calnum[4] = atof(SerCmd.next());
  calnum[5] = atof(SerCmd.next());
}

void calibrateTopZ() {
  calnum[6] = atof(SerCmd.next());
  calnum[7] = atof(SerCmd.next());
  calnum[8] = atof(SerCmd.next());
}

void calibrateBotX() {
  calnum[0] = atof(SerCmd.next());
  calnum[1] = atof(SerCmd.next());
  calnum[2] = atof(SerCmd.next());
}

void calibrateBotY() {
  calnum[3] = atof(SerCmd.next());
  calnum[4] = atof(SerCmd.next());
  calnum[5] = atof(SerCmd.next());
}

void calibrateBotZ() {
  calnum[6] = atof(SerCmd.next());
  calnum[7] = atof(SerCmd.next());
  calnum[8] = atof(SerCmd.next());
}


// Pushes calibration matrix into the eeprom on ambient side
void calibrateTop()
{  
  writeToDevice(muxAddr, channelSelectAmbient);
  
  //take in calibration matrix from helpers (for now)
  writeToEeprom(calnum);
}

// Pushes calibration matrix into the eeprom on screen side
void calibrateBottom()
{
  writeToDevice(muxAddr, channelSelectScreen);
  
  //take in calibration matrix from helpers (for now)
  writeToEeprom(calnum);
}


// Reads 8 bytes of data from the color sensor
void readFromColorSensor(char channelSelect, uint8_t* zLow, uint8_t* zHigh, uint8_t* yLow, uint8_t* yHigh, uint8_t* xLow, uint8_t* xHigh)
{
  // Select the corresponding channel
  writeToDevice(muxAddr, channelSelect);

  // Write register to begin read from
  writeToDevice(colorSensorAddr, 0x04);  

  // Enable read
  Wire.requestFrom(colorSensorAddr, 6);
  
  *zLow = Wire.read();
  *zHigh = Wire.read();
  *yLow = Wire.read();
  *yHigh = Wire.read();
  *xLow = Wire.read();
  *xHigh = Wire.read();
  
}




// ReadColorSensor serial command takes one argument - which sensor to read from
// 1 for the ambient color sensor facing away from the screen, 2 for the screen-facing sensor
void readColorSensor()
{
  char* arg;
  int sensorId;
  uint8_t channelSelect;
  uint8_t zLow;
  uint8_t zHigh;
  uint8_t yLow;
  uint8_t yHigh;
  uint8_t xLow;
  uint8_t xHigh;
  uint16_t z;
  uint16_t y;
  uint16_t x;
  float zz;
  float yy;
  float xx;
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
  readFromColorSensor(channelSelect, &zLow, &zHigh, &yLow, &yHigh, &xLow, &xHigh);

  // Write the result to serial out.
  z = zHigh & 0x0f;
  z = (z << 8) | zLow;
  y = yHigh & 0x0f;
  y = (y << 8) | yLow;
  x = xHigh & 0x0f;
  x = (x << 8) | xLow;

  xx = (x*(886.628/65536));
  yy = (y*(954.830/65536));
  zz = (z*(512.774/65536));

  mtx_type valMatrix[3][1] = {{xx},{yy},{zz}};
  mtx_type finalValues[3][1];

  // Populate the calibration matrix from the eeprom into helpers
  readEeprom();

  Matrix.Multiply((mtx_type*)calMatrix, (mtx_type*)valMatrix, 3, 3, 1, (mtx_type*)finalValues);  

  Serial.println(finalValues[0][0]);
  Serial.println(finalValues[0][1]);
  Serial.println(finalValues[0][2]);
  
Exit:  
  Serial.println(err);
}

// ReadColorSensor serial command takes one argument - which sensor to read from
// 1 for the ambient color sensor facing away from the screen, 2 for the screen-facing sensor
void readColorSensorRAW()
{
  char* arg;
  int sensorId;
  uint8_t channelSelect;
  uint8_t zLow;
  uint8_t zHigh;
  uint8_t yLow;
  uint8_t yHigh;
  uint8_t xLow;
  uint8_t xHigh;
  uint16_t z;
  uint16_t y;
  uint16_t x;
  float zz;
  float yy;
  float xx;
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
  readFromColorSensor(channelSelect, &zLow, &zHigh, &yLow, &yHigh, &xLow, &xHigh);

  // Write the result to serial out.
  z = zHigh & 0x0f;
  z = (z << 8) | zLow;
  y = yHigh & 0x0f;
  y = (y << 8) | yLow;
  x = xHigh & 0x0f;
  x = (x << 8) | xLow;

  xx = (x*(886.628/65536));
  yy = (y*(954.830/65536));
  zz = (z*(512.774/65536));  

  Serial.println(xx);
  Serial.println(yy);
  Serial.println(zz);
  
Exit:  
  Serial.println(err);
}



void initColorSensor(char channelSelect)
{
  // Select the corresponding channel
  writeToDevice(muxAddr, channelSelect);

  //default set in config state when turned on
  //do configs and then change to measurement state and start measurement

  // Config Reg 3
  Wire.beginTransmission(colorSensorAddr);
  Wire.write(creg3Addr);
  Wire.write(0X00); //sets continuous measurement, turns standby switch to off, and sets clock to 1.024 MHz
  Wire.endTransmission();

  // Config Reg 2
  Wire.beginTransmission(colorSensorAddr);
  Wire.write(creg2Addr);
  Wire.write(0x40); //Internal measurement of the externally defined conversion time via SYN pulse in SYND mode is enabled, disables digital divider result register, sets value of divider to 2
  Wire.endTransmission();

  // Config Reg 1
  Wire.beginTransmission(colorSensorAddr);
  Wire.write(creg1Addr);
  Wire.write(0xa6); // sets GAIN for xyz to 2x with tconv at 64 ms with 2^16 clocks
  Wire.endTransmission();
  
  // Enable Measurement
  Wire.beginTransmission(colorSensorAddr);
  Wire.write(osrAddr);
  Wire.write(0x83); // Switches into measurement state, starts measurement, Power Down state switched off
  Wire.endTransmission();  
}
