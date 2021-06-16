#pragma once

#include <SerialCommand.h>
#include <MatrixMath.h>
#include <SoftWire.h>
#include <SPI.h>
SoftWire Wire = SoftWire();
SerialCommand SerCmd;

const int screenSensorId = 2;
const int ambientSensorId = 1;
const int channelSelectScreen = 0x05; // channel 1 enabled
const int channelSelectAmbient = 0x04;  // channel 0 enabled
const uint8_t muxAddr = 0x70; //P14MSD5V9540B device address [6:0]=1110000
const uint8_t colorSensorAddr = 0x74; //AS73211 device address [6:0]=1110100
const uint8_t IRSensorAddr = 0x29; //TC34007 device address [6:0]=0101001
const uint8_t lightSensorAddr = 0x44; //OPT3001 device address [6:0]=1000100
const uint8_t eepromAddr = 0x50; //24LC08 block 0 address [6:0]=1010000
const uint8_t eepromAddr2 = 0x51; //24LC08 block 1 address [6:0]=1010001
const uint8_t eepromAddr3 = 0x52; //24LC08 block 2 address [6:0]=1010010

//Calibration Matrix
mtx_type calMatrix[3][3];

void writeToDevice(uint8_t deviceAddr, char writeData)
{
  Wire.beginTransmission(deviceAddr);
  Wire.write(writeData);
  Wire.endTransmission();
}

typedef enum _MALTERROR
{
    E_SUCCESS = 0,
    E_INVALID_PARAM = 1,
    E_UNRECOGNIZED_COMMAND = 2
} MALTERROR;
