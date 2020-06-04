# MALT microcontroller code overview

## Dependencies
The Arduino sketch malt.ino depends on the [SerialCommand](https://github.com/kroimon/Arduino-SerialCommand) library, [SoftI2C](https://github.com/felias-fogg/SoftI2CMaster) library, and the [MatrixMath](https://github.com/eecharlie/MatrixMath) library. Click each library name to download from GitHub.

## Arduino Sketch
Download the reference Arduino sketch (malt.ino) and all header files from this repo. If you have used different parts for your version, the sketch and header files will need to be modified to match your design.

## Troubleshooting
To validate that your MALT is working, you can either
- Manually send commands via a serial monitor to the MALT.
    - READALSSENSOR 1 reads the Ambient Light Sensor on the ambient facing side of the board
    - READALSSENSOR 2 reads the Ambient Light Sensor on the screen facing side of the board
    - READCOLORSENSOR 1 reads the Color Sensor on the ambient facing side of the board
    - READCOLORSENSOR 2 reads the Color Sensor on the screen facing side of the board
    - READIRSENSOR 1 reads the Color+IR Sensor on the ambient facing side of the board
- Find additional arduino troubleshooting [here](https://www.arduino.cc/en/Guide/Troubleshooting). 

### Error handling
MALT errors are defined in `helper.h`. 
You may need to copy the file into your Arduino sketch directory. Include this file from your Arduino sketch using
```
#include "helper.h"
```

Include the same file in any test software you write which interfaces with the Arduino.
