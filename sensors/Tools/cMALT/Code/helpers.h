#pragma once

#include <SerialCommand.h>
#include <MatrixMath.h>
SerialCommand SerCmd;

const int screenSensorId = 2;
const int ambientSensorId = 1;
const int channelSelectScreen = 0x05; // channel 1 enabled
const int channelSelectAmbient = 0x04;  // channel 0 enabled
const uint8_t muxAddr = 0x70; //P14MSD5V9540B device address
const uint8_t colorSensorAddr = 0x74; //AS73211 device address [6:0]=1110100 where last bit is R/W
const uint8_t IRSensorAddr = 0x29; //TC34007 device address [6:0]=
const uint8_t lightSensorAddr = 0x44; // OPT3001 device address

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
