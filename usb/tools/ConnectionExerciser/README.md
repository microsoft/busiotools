Building the code:

1) Clone the code for the ConnectionExerciser project.
2) Clone the following projects into the libraries directory:

    https://github.com/PFroese/ArduinoSerialCommand
    https://github.com/mattnichols/Timer
    https://github.com/adafruit/Adafruit_TCS34725
    https://github.com/todbot/ServoEaser
    
3) Make the following minor modification to the ArduinoSerialCommand library:
    - Open SerialCommand.h
    - Locate the #define for MAXSERIALCOMMANDS
    - Change the value from 16 to 32
4) Install the Arduino IDE.
5) Under "File | Preferences" change the "Sketchbook location" to the path to the ConnectionExerciser project.
6) Select "File | Sketchbook | Shield" to open the project.
7) Select "Tools | Board | Mega 2560"
8) Select "Sketch | Compile" to build the device image, and "Sketch | Upload" to deploy it to an attached Connection Exerciser device.

    
