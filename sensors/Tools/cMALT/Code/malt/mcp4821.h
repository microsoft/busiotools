#pragma once

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
