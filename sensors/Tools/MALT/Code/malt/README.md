# MALT microcontroller code overview

## Arduino Sketch
Found in malt.ino

## Dependencies
The Arduino sketch malt.ino depends on the SerialCommand library, available for download [here](https://github.com/kroimon/Arduino-SerialCommand). 

## Arduino Sketch
Download the reference Arduino sketch (malt.ino) from this repo. If you have used different parts for your version, the sketch will need to be modified to match your design.

## Troubleshooting
To validate that your MALT is working, you can either
- Manually send commands via a serial monitor to the MALT. MALT command reference is located [here](../../docs/Commands.md).
- Use MALTTestApp.exe to interface with the MALT.
- Additional arduino troubleshooting is located [here](https://www.arduino.cc/en/Guide/Troubleshooting). 

### Error handling
MALT errors are defined in `malterror.h`. 
You may need to copy the file into your Arduino sketch directory. Include this file from your Arduino sketch using
```
#include "malterror.h"
```

Include the same file in any test software you write which interfaces with the Arduino.
