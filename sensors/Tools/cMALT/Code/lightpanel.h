

#include "mcp4821.h"
#include "helpers.h"


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
