# HMD Validation Kit Libraries

## Managed Libraries

###HmdKit.cs

###Usage:


- Include HmdKit.cs in your automation app.
- Create an instance of the HmdKit class and call findHmdKitDevice to scan COM ports for HmdKits. 
- Check the return value of findHmdKitDevice or the IsPresent property.
- Use the methods to perform actions like connecting USB/HDMI or checking display brightness. 
- If a method returns false, the device may be in a bad state or the COM port may be in use by another application.
    
### Classes:

        /// This class provides managed access to the functionality of the HMD Validation Kit.
        public class HmdKit : IDisposable
### Private Members:

        /// The default baud rate for the HMD Kit's Serial over USB communication is 9600 baud.
        private const int HmdKitBaudRate = 9600;
        
        /// This constant controls the number of retries for sending a command.
        private const int RetryCount = 3;
       
        /// This is the string hardcoded in the HMD Validation Kit for the version. This constant should match the expected version from the HMD Validation Kit.
        private const string HmdKitVersion = "01";
        
        /// This specifies the type of shield detected by the HMD Validation Kit. The current revision should always detect shield type "8".
        private const string HmdKitShieldType = "08";
        
        /// This is the default response time that should provide enough time for a simple command to complete. Use this for commands that don't involve waiting for something to happen on the HMD Validation Kit like taking audio samples or waiting for sensor data.
        private const int ResponseTimeDefault = 500;
        
        /// This time should provide enough window for the TCS color sensor's "integration time" to elapse and for the HMD Validation Kit to return a value. See the TCS34725 documentation for more information.
        private const int ResponseTimeColorSensor = 2000;

        /// This is a non-constant value that should provide enough time for a "GetVolume*" command to run for the specified number of samples. It will change every time the SetVolumeSampleCount method is used.
        private int responseTimeGetVolume = 1000;

        /// This is the internal instance of the serial port attached to the HMD Validation Kit.
        private SerialPort serialPort;

        /// This dictionary translates the friendly names of the commands to their real command strings that will be sent over the cable to the HMD Validation Kit.
        private Dictionary<string, string> hmdKitCommand = new Dictionary<string, string>();

        /// This lock is set when the HMD Validation Kit's serial port is being accessed.
        private object hmdKitLock = new object();

        /// This private member tracks the enumeration state of the HMD Validation Kit.
        private bool isPresent;
        #endregion

### Constructors:
        
        /// Initializes a new instance of the HmdKit class. This maps all of the appropriate friendly names to their serial commands along with initializing the enumeration state to false.
        public HmdKit()

### Properties:
        
        /// Gets a value indicating whether the HMD Validation Kit is enumerated.
        public bool IsPresent

### Methods:
        
        /// Implementation of the Dispose interface to dispose of the managed resources used by the HmdKit class.
        public void Dispose()
        
        /// Checks all open serial ports for HMD Exercisers
        /// Returns true if a valid HMD Kit is detected, otherwise returns false.
        public bool FindHmdKitDevice()

        
        /// Sends a command to the HMD Exerciser. The command should return within responseTime milliseconds.
        
        /// Parameter "command"
        ///     The command to send to the HMD Kit. The command should be a part of the hmdKitCommand Dictionary.
        /// Parameter "responseTime"
        ///     The time that the SendCommand method should wait for a response. This can be longer for commands that will take more time.
        /// Parameter "parameter"
        ///     An parameter for commands that set USB ports, HDMI ports, etc. This will be appended to the command and sent to the HMD Kit.
        /// Parameter "output"
        ///     Returns with the response for commands that need to get information from the HMD Kit
        public void SendCommand(string command, int responseTime, string parameter, out string output)
        
        /// Sends a command to the HMD Exerciser. The command should return within responseTime milliseconds. This overload doesn't have an output and the parameter is optional.
        /// Parameter "command"
        /// The command to send to the HMD Kit. The command should be a part of the hmdKitCommand Dictionary.
        /// Parameter "responseTime"
        /// The time that the SendCommand method should wait for a response. This can be longer for commands that will take more time.
        /// Parameter "parameter"
        /// An optional parameter for commands that set USB ports, HDMI ports, etc. This will be appended to the command and sent to the HMD Kit.
        public void SendCommand(string command, int responseTime, string parameter = null)
        
        /// Sets the USB port that is connected to J1.
        /// Parameter "port"
        ///     Ports can be either 0,1,2,3,4 or go by the silkscreen on the PCB: J2,J3,J4, or J6. 
        ///     The mapping is J2=1 J3=2 J4=4 J6=3
        public void SetUsbPort(string port)
        
        /// Gets the currently connected USB port on the HMD Exerciser.
        /// Returns the currently connected USB port. '0' designates no port is connected.
        public string GetUsbPort()
        
        /// Sets the currently connected HDMI port.
        /// Parameter "port"
        ///     The port to connect. Valid values are J2/J3 or 1/2. J2 and 1 are equivalent and J3 and 2 are equivalent.
        public void SetHdmiPort(string port)
        
        /// Gets the index of the currently connected HDMI port from the HMD Exerciser.
        /// Returns the currently connected HDMI port. '0' means no port is connected. Otherwise, the port will be 2(J2) or 1(J3)
        public string GetHdmiPort()
        
        /// Sets the angle of the specified servo in degrees.
        /// Parameter "servo"
        ///     The servo to set the angle on. See the silkscreen on the servo connectors of the HMD Exerciser.
        ///         1 Selects "Servo 1"
        ///         2 Selects "Servo 2"
        /// Parameter "degrees"
        ///     The degree position to move to in the servo's sweep. The degrees should be between 45* and 135* for most servos.
        public void SetServoAngle(string servo, string degrees)
        
        /// Sets the presence spoofing LED to either drown out the HMD's presence sensor with light (no user presence) or spoof user presence.
        /// Parameter "userPresent"
        ///     The desired user presence state
        ///         true User presence is spoofed by imitating the IR presence sensor response pattern.
        ///         false User presence is "removed" by flooding the IR presence sensor with IR light, removing the ability for it to see the reflection of the response pattern.
        public void SetPresence(bool userPresent)

        
        /// Gets the brightness of the requested display using the TCS34725 color sensors on the HMD Board
        /// Note that this is also affected by the HMD that is set. See SetHmd for more info.
        /// Parameter "display"
        ///     The display to read color from.
        ///         0 The left display from the perspective of wearing the HMD
        ///         1 The right display from the perspective of wearing the HMD
        /// Returns the raw display brightness from the indicated display. The brightness is a magnitude between 0 and 65535. See the TCS34725 color sensor docs for more information. 
        public string GetDisplayBrightness(int display)
        
        /// Gets the RGB color values for the specified display. The colors are a magnitude between 0 and 65535. See the TCS34725 color sensor docs for more information. 
        /// Parameter "display"
        ///     The display to read color from.
        ///         0 The left display from the perspective of wearing the HMD
        ///         1 The right display from the perspective of wearing the HMD
        /// Parameter "red"
        ///     Out param that returns the magnitude of the red component of the received light.
        /// Parameter "green"
        ///     Out param that returns the magnitude of the green component of the received light.
        /// Parameter "blue"
        ///     Out param that returns the magnitude of the blue component of the received light.
        public void GetDisplayColor(int display, out string red, out string green, out string blue)

        /// Sets the number of volume samples to take before returning a result from any of the getVolume* methods. The default value is 2048 and the max is ULONG_MAX. THe sample rate of the Arduino is set to 38.4kHz.
        /// Parameter "samples"
        ///     The number of samples to collect before returning a volume level. One second is approximately 40,960 samples.
        public void SetVolumeSampleCount(string samples)
        
        /// Gets the RMS volume level on the 1/8" jack of the HMD exerciser over a number of samples. The result will be between 0 and 100*sqrt(2). The number of samples can be configured using setVolumeSampleCount.
        /// Volume levels are amplified on the PCB to give a full range response (0-5V) over headphone level signals. The center level is 2.5V.
        /// Returns the RMS volume over the given sample period on a scale of 0-100*sqrt(2), since the volume is not centered on zero.
        public string GetVolumeRms()

        /// Gets the peak volume level on the 1/8" jack of the HMD exerciser over a number of samples. The number of samples can be configured using setVolumeSampleCount.
        /// Volume levels are amplified on the PCB to give a full range response (0-5V) over headphone level signals. The center level is 2.5V.
        /// Returns the highest sample over the given sample period on a scale of 0-100.
        public string GetVolumePeak()
        
        /// Gets the average volume level on the 1/8" jack of the HMD exerciser over a number of samples. The number of samples can be configured using setVolumeSampleCount.
        /// Volume levels are amplified on the PCB to give a full range response (0-5V) over headphone level signals. The center level is 2.5V.
        /// Returns the average volume over the given sample period on a scale of 0-100.
        public string GetVolumeAvg()

        /// Sets the HMD port that the other methods will act on. If the HMD of interest is plugged into P1, set the HMD to 1. if the HMD of interest is lugged into P2, set the HMD to 2.
        /// Parameter "hmd"
        ///     The HMD you would like brightness, presence, and color commands to apply to.
        ///         1 The HMD attached to port P1
        ///         2 The HMD attached to port P2
        public void SetHmd(int hmd)
        
        /// Gets the VBus voltage from the USB port
        /// Returns the USB voltage as displayed on the LCD
        public string Volts()
        
        /// Gets the current drawn by the currently selected USB port.
        /// Returns the current as displayed on the LCD.
        public string Amps()
        
        /// Sets the superspeed ports on or off. Pass true to enable superspeed lines and false to disable superspeed lines.
        /// Parameter "status"
        ///     The desired status of the SuperSpeed lines. 
        ///         True for connected
        ///         False for disconnected
        public void SuperSpeed(bool status)

        /// Sets a delay in seconds before the next issued command will execute.
        /// Parameter "delay"
        ///     The delay in seconds before the next command sent to the HMD kit will execute.
        public void CommandDelay(int delay)
        
        /// Sets a timeout for the port to disconnect after the next connect command is sent
        /// Parameter "timeout"
        /// The time in milliseconds the port will wait to disconnect after the next connect command is sent.
        public void DisconnectTimeout(int timeout)
        
        /// Implementation of the Dispose interface to dispose of the managed resources used by the HmdKit class.
        /// Parameter "disposing"
        ///     If true, the managed resources are disposed.
        protected virtual void Dispose(bool disposing)

Click here to download the BUS IO Tools package as a zip file: https://github.com/Microsoft/busiotools/archive/master.zip
