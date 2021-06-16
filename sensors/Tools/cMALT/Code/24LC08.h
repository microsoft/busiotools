#include "helpers.h"

union cvt {
double val;
byte b[8];
} x1, x2, x3, y1, y2, y3, z1, z2, z3;


void writeToEeprom(double calnum[]) 
{
    x1.val = calnum[0];
    x2.val = calnum[1];
    x3.val = calnum[2];
    y1.val = calnum[3];
    y2.val = calnum[4];
    y3.val = calnum[5];
    z1.val = calnum[6];
    z2.val = calnum[7];
    z3.val = calnum[8];
    
    Wire.beginTransmission(eepromAddr);
		Wire.write(0x00);
		Wire.write(x1.b[0]);
		Wire.write(x1.b[1]);
		Wire.write(x1.b[2]);
		Wire.write(x1.b[3]);
    Wire.write(x2.b[0]);
    Wire.write(x2.b[1]);
    Wire.write(x2.b[2]);
    Wire.write(x2.b[3]);
    Wire.write(x3.b[0]);
    Wire.write(x3.b[1]);
    Wire.write(x3.b[2]);
    Wire.write(x3.b[3]);
    Wire.write(y1.b[0]);
    Wire.write(y1.b[1]);
    Wire.write(y1.b[2]);
    Wire.write(y1.b[3]);
    Wire.endTransmission();
    delay(10);
    Wire.beginTransmission(eepromAddr2);
    Wire.write(0x00);
    Wire.write(y2.b[0]);
    Wire.write(y2.b[1]);
    Wire.write(y2.b[2]);
    Wire.write(y2.b[3]);
    Wire.write(y3.b[0]);
    Wire.write(y3.b[1]);
    Wire.write(y3.b[2]);
    Wire.write(y3.b[3]);
    Wire.write(z1.b[0]);
    Wire.write(z1.b[1]);
    Wire.write(z1.b[2]);
    Wire.write(z1.b[3]);
    Wire.write(z2.b[0]);
    Wire.write(z2.b[1]);
    Wire.write(z2.b[2]);
    Wire.write(z2.b[3]);
    Wire.endTransmission();
    delay(10);
    Wire.beginTransmission(eepromAddr3);
    Wire.write(0x00);
    Wire.write(z3.b[0]);
    Wire.write(z3.b[1]);
    Wire.write(z3.b[2]);
    Wire.write(z3.b[3]);
    Wire.endTransmission();
    delay(10);
}

void readEeprom() 
{
	Wire.beginTransmission(eepromAddr);
	Wire.write(0x00);
	Wire.requestFrom(eepromAddr, 16);
  for (int i = 0; i < 4; i++) {
    x1.b[i] = Wire.read();
  }

  for (int i = 0; i < 4; i++) {
    x2.b[i] = Wire.read();
  }

  for (int i = 0; i < 4; i++) {
    x3.b[i] = Wire.read();
  }

  for (int i = 0; i < 4; i++) {
    y1.b[i] = Wire.read();
  }
 Wire.endTransmission();

 Wire.beginTransmission(eepromAddr2);
  Wire.write(0x00);
  Wire.requestFrom(eepromAddr2, 16);
  for (int i = 0; i < 4; i++) {
    y2.b[i] = Wire.read();
  }

  for (int i = 0; i < 4; i++) {
    y3.b[i] = Wire.read();
  }

  for (int i = 0; i < 4; i++) {
    z1.b[i] = Wire.read();
  }

  for (int i = 0; i < 4; i++) {
    z2.b[i] = Wire.read();
  }
  Wire.endTransmission();

  Wire.beginTransmission(eepromAddr3);
  Wire.write(0x00);
  Wire.requestFrom(eepromAddr3, 4);
  for (int i = 0; i < 4; i++) {
    z3.b[i] = Wire.read();
  }
  Wire.endTransmission();

  calMatrix[0][0] = x1.val;
  calMatrix[1][0] = x2.val;
  calMatrix[2][0] = x3.val;
  calMatrix[0][1] = y1.val;
  calMatrix[1][1] = y2.val;
  calMatrix[2][1] = y3.val;
  calMatrix[0][2] = z1.val;
  calMatrix[1][2] = z2.val;
  calMatrix[2][2] = z3.val;  
}
