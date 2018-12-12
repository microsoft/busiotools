// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Media;

namespace MALTUtil
{
    class Program
    {
        private static SerialPort port = new SerialPort();
        const int c_LightOff = 0;
        const int c_LightMax = 2600;
        static int m_ConversionTime = 800;

        static void Main(string[] args)
        {
            Console.WriteLine("Microsoft Ambient Light Tool");

            if (args.Length == 0)
            {
                // Expect the user to have provided a command.
                PrintHelp();
                goto Exit;
            }

            string portName = GetComPort();
            if (portName == null)
            {
                Console.WriteLine("No Arduino devices connected to the system.");
                return;
            }

            port.PortName = portName;
            port.BaudRate = 9600;
            port.Open();

            // Check the first argument for the command.
            string firstArg = args[0].ToLower();

            if (firstArg.Contains("firmwareversion"))
            {
                string version = GetVersion();
                Console.Out.WriteLine("MALT Version: %s.", version);
                goto Exit;
            }
            else if (firstArg.Contains("light"))
            {
                UInt32 lightVal = 0;
                try
                {
                    lightVal = UInt32.Parse(args[1]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to parse light value.");
                    goto Exit;
                }

                SetLight(lightVal);
                goto Exit;
            }
            else if (firstArg.Contains("conversiontime"))
            {
                UInt32 ct = 0;
                try
                {
                    ct = UInt32.Parse(args[1]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to parse conversion time value.");
                    goto Exit;
                }

                SetConversionTime(ct);
                goto Exit;
            }
            else if (firstArg.Contains("screenlux"))
            {
                double screenLux = GetScreenLux();
                Console.Out.WriteLine("Screen Lux: {0}.", screenLux);
                goto Exit;
            }
            else if (firstArg.Contains("ambientlux"))
            {
                double ambientLux = GetAmbientLux();
                Console.Out.WriteLine("Ambient Lux: {0}.", ambientLux);
                goto Exit;
            }
            else if (firstArg.Contains("screencolor"))
            {
                uint[] screenColor = GetScreenColor();
                Console.Out.WriteLine("Screen Color: [Clear: {0}, Red: {1}, Green: {2}, Blue: {3}].", screenColor[0], screenColor[1], screenColor[2], screenColor[3]);
                goto Exit;
            }
            else if (firstArg.Contains("ambientcolor"))
            {
                uint[] ambientColor = GetAmbientColor();
                Console.Out.WriteLine("Ambient Color: [Clear: {0}, Red: {1}, Green: {2}, Blue: {3}].", ambientColor[0], ambientColor[1], ambientColor[2], ambientColor[3]);
                goto Exit;
            }
            else if (firstArg.Contains("autocurve"))
            {
                TakeAutobrightnessCurve();
                goto Exit;
            }
            else if (firstArg.Contains("manualcurve"))
            {
                TakeManualCurve();
                goto Exit;
            }
            else if (firstArg.Contains("stress"))
            {
                // TODO
                //Stress(); 
                goto Exit;
            }
            else
            {
                // Unrecognized command.
                PrintHelp();
            }

            Exit:
            if (port.IsOpen)
            {
                port.Close();
            }
        }
        static void PrintHelp()
        {
            Console.WriteLine("************************************************");
            Console.WriteLine("\tU S A G E");
            Console.WriteLine("************************************************\n");
            Console.WriteLine("MALT Commands:");
            Console.WriteLine("\t/firmwareversion\t\tGet version of the MALT firmware");
            Console.WriteLine("\t/light n\t\tSet light to value 'n' between ({0} - {1})", c_LightOff, c_LightMax);
            Console.WriteLine("\t/conversionTime\t\tSet conversion time in milliseconds on both of the ambient light sensors. Valid values for OPT3001 light sensor are 800 and 100.\n");
            Console.WriteLine("\t/screenLux\t\tRead the screen light sensor value in lux\n");
            Console.WriteLine("\t/ambientLux\t\tRead the ambient light sensor value in lux\n");
            Console.WriteLine("\t/screenColor\t\tRead the screen color sensor value\n");
            Console.WriteLine("\t/ambientColor\t\tRead the ambient color sensor value\n");
            Console.WriteLine("\nTest Sequences:");
            Console.WriteLine("\t/autoCurve n\t\tSteps the light up from 0 to the max value and prints the screen and ambient lux to maltALROutput.csv. Wait for 'n' seconds before beginning the test to allow for time to set up (default value is 0)");
            Console.WriteLine("\t/manualCurve n\t\t Adjusts the manual brightness level on the PC and prints the screen lux at each percentage to maltManualOutput.csv. Wait for 'n' seconds before beginning the test to allow for time to set up (default value is 0)");
            Console.WriteLine("\t/stress \t\t Puts the system to sleep while adjusting brightness & measures the accuracy coming out of sleep.");
        }

        static string GetComPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine("ERROR: Could not get the list of attached Arduino devices. Exception text: {0}", e.Message);
            }

            // If we reach here, there is nothing attached.
            return null;
        }

        static string GetVersion()
        {
            // Send GetVersion command
            port.WriteLine("MALTVERSION");

            // First bit is error
            int errVal = Int32.Parse(port.ReadLine());
            if (errVal != 0)
            {
                Console.WriteLine("Failed to read version: {0}", errVal);
                throw new ApplicationException(errVal.ToString());
            }
            else
            {
                // Next line is the actual version.
                return port.ReadLine();
            }
        }

        static void SetConversionTime(UInt32 ConversionTime)
        {
            if (ConversionTime != 800 && ConversionTime != 100)
            {
                Console.WriteLine("OPT3001 supports 800 and 100 millesecond conversion times. Please enter either 800 or 100.");
                return;
            }
            else
            {
                // Write CONVERSIONTIME ConversionTime to the arduino
                String toWrite = "CONVERSIONTIME " + ConversionTime.ToString();
                port.WriteLine(toWrite);

                // Check for any error code coming back.
                int errVal = Int32.Parse(port.ReadLine());
                if (errVal != 0)
                {
                    Console.WriteLine("Failed to set conversion time: {0}", errVal);
                    throw new ApplicationException(errVal.ToString());
                }

                // Save the new conversion time.
                m_ConversionTime = (int)ConversionTime;
            }
        }

        static double GetScreenLux()
        {
            // Send command
            port.WriteLine("READALSSENSOR 2");

            // First bit is error
            int errVal = Int32.Parse(port.ReadLine());
            if (errVal != 0)
            {
                Console.WriteLine("Failed to read ALS ambient-facing sensor: {0}", errVal);
                throw new ApplicationException(errVal.ToString());
            }

            // Next line is the exponent.
            UInt32 exponent = UInt32.Parse(port.ReadLine());

            // Next line is the fractional result.
            UInt32 result = UInt32.Parse(port.ReadLine());

            return RawToLux(result, exponent);
        }

        static double GetAmbientLux()
        {
            // Send command
            port.WriteLine("READALSSENSOR 1");

            // First bit is error
            int errVal = Int32.Parse(port.ReadLine());
            if (errVal != 0)
            {
                Console.WriteLine("Failed to read ALS ambient-facing sensor: {0}", errVal);
                throw new ApplicationException(errVal.ToString());
            }

            // Next line is the exponent.
            UInt32 exponent = UInt32.Parse(port.ReadLine());

            // Next line is the fractional result.
            UInt32 result = UInt32.Parse(port.ReadLine());

            return RawToLux(result, exponent);
        }

        static uint[] GetScreenColor()
        {
            // Send command
            port.WriteLine("READCOLORSENSOR 2");

            // First bit is error
            int errVal = Int32.Parse(port.ReadLine());
            if (errVal != 0)
            {
                Console.WriteLine("Failed to read screen-facing color sensor: {0}", errVal);
                throw new ApplicationException(errVal.ToString());
            }

            uint[] result = new uint[4];

            // Next line is the clear value.
            result[0] = UInt32.Parse(port.ReadLine());

            // Next line is the red value.
            result[1] = UInt32.Parse(port.ReadLine());

            // Next line is the green value.
            result[2] = UInt32.Parse(port.ReadLine());

            // Next line is the blue value.
            result[3] = UInt32.Parse(port.ReadLine());

            return result;
        }

        static uint[] GetAmbientColor()
        {
            // Send command
            port.WriteLine("READCOLORSENSOR 1");

            // First bit is error
            int errVal = Int32.Parse(port.ReadLine());
            if (errVal != 0)
            {
                Console.WriteLine("Failed to read ambient-facing color sensor: {0}", errVal);
                throw new ApplicationException(errVal.ToString());
            }

            uint[] result = new uint[4];

            // Next line is the clear value.
            result[0] = UInt32.Parse(port.ReadLine());

            // Next line is the red value.
            result[1] = UInt32.Parse(port.ReadLine());

            // Next line is the green value.
            result[2] = UInt32.Parse(port.ReadLine());

            // Next line is the blue value.
            result[3] = UInt32.Parse(port.ReadLine());

            return result;
        }

        static void TakeAutobrightnessCurve()
        {
            Console.WriteLine("Setting conversion time to 100ms to make the test faster.");
            SetConversionTime(100);

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Light Level,Ambient Lux,Screen Lux");

            double ambientLux1, ambientLux2, ambientLuxCurrent;
            double screenLux1, screenLux2, screenLuxCurrent;

            // Move the light up in increments of 5 to speed the test up further.
            for (uint i = 500; i <= 2600; i += 5)
            {
                SetLight(i);

                System.Threading.Thread.Sleep(150);
                ambientLux1 = GetAmbientLux();
                screenLux1 = GetScreenLux();

                System.Threading.Thread.Sleep(150);
                ambientLux2 = GetAmbientLux();
                screenLux2 = GetScreenLux();

                System.Threading.Thread.Sleep(300);
                ambientLuxCurrent = GetAmbientLux();
                screenLuxCurrent = GetScreenLux();

                while (screenLux1 != screenLux2 || screenLux2 != screenLuxCurrent)
                {
                    ambientLux1 = ambientLux2;
                    screenLux1 = screenLux2;

                    ambientLux2 = ambientLuxCurrent;
                    screenLux2 = screenLuxCurrent;

                    System.Threading.Thread.Sleep(150);
                    ambientLuxCurrent = GetAmbientLux();
                    screenLuxCurrent = GetScreenLux();
                }

                Console.WriteLine("Light Level: {0}. Ambient Lux: {1}. Screen Lux {2}.", i, ambientLuxCurrent, screenLuxCurrent);
                csv.AppendLine(string.Format("{0},{1},{2}", i, ambientLuxCurrent, screenLuxCurrent));
            }

            try
            {
                string filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "autoBrightness.csv");
                File.WriteAllText(filePath, csv.ToString());
                Console.WriteLine("Output successfully written to {0}.", filePath);

                SoundPlayer finish = new SoundPlayer("Alarm05.wav");
                finish.Play();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void TakeManualCurve()
        {
            Console.WriteLine("Setting conversion time to 100ms to make the manual brightness test faster.");
            SetConversionTime(100);

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Brightness Percentage,Screen Lux");

            double screenLux1, screenLux2, screenLuxCurrent;

            try
            {
                ManagementScope scope = new ManagementScope("root\\WMI");
                SelectQuery query = new SelectQuery("WmiMonitorBrightnessMethods");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection objectCollection = searcher.Get();
                foreach (ManagementObject mObj in objectCollection)
                {
                    // Expect the system to have 0 to 100 brightness levels.
                    for (int i = 0; i <= 100; i++)
                    {
                        Object[] args = { 5, i };
                        mObj.InvokeMethod("WmiSetBrightness", args);

                        System.Threading.Thread.Sleep(150);
                        screenLux1 = GetScreenLux();

                        System.Threading.Thread.Sleep(150);
                        screenLux2 = GetScreenLux();

                        System.Threading.Thread.Sleep(150);
                        screenLuxCurrent = GetScreenLux();

                        while (screenLux1 != screenLux2 || screenLux2 != screenLuxCurrent)
                        {
                            screenLux1 = screenLux2;
                            screenLux2 = screenLuxCurrent;

                            System.Threading.Thread.Sleep(150);
                            screenLuxCurrent = GetScreenLux();
                        }

                        Console.WriteLine("{0}% brightness. Screen is measured at {1} lux", i, screenLuxCurrent);
                        csv.AppendLine(string.Format("{0},{1}", i + "%", screenLuxCurrent));
                    }

                    try
                    {
                        string filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "manualBrightness.csv");
                        File.WriteAllText(filePath, csv.ToString());
                        Console.WriteLine("Output successfully written to {0}.", filePath);

                        SoundPlayer finish = new SoundPlayer("Alarm05.wav");
                        finish.Play();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine("ERROR: Your device does not support brightness changes. Exception text: {0}", e.Message);
            }
        }

        static void SetLight(UInt32 LightLevel)
        {
            if (LightLevel > 2600)
            {
                Console.WriteLine("Light panel supports values from 0 to 2600.");
                return;
            }
            else
            {
                // Write LIGHT LightLevel to the arduino
                String toWrite = "LIGHT " + LightLevel.ToString();
                port.WriteLine(toWrite);

                // Check for any error code coming back.
                int errVal = Int32.Parse(port.ReadLine());
                if (errVal != 0)
                {
                    Console.WriteLine("Failed to set light: {0}", errVal);
                    throw new ApplicationException(errVal.ToString());
                }
            }
        }

        private static double RawToLux(UInt32 Result, UInt32 Exponent)
        {
            double lux = 0;

            // Formula to convert raw sensor output to lux is defined in the OPT 3001 spec.
            // If you are using a different part, this calculation will be different.

            double lsbSize = .01 * (Math.Pow(2, Exponent));
            lux = lsbSize * Result;

            return lux;
        }
    }
}
