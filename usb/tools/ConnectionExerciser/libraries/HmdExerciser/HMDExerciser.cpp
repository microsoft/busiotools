/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

#include <SerialCommand.h>
#include "HMDExerciser.h"
#include "Adafruit_TCS34725.h"
#include "ServoShield.h"
#include "limits.h"

#define DBGPRINT(A)    if (dbg) {Serial.print(A);}
#define DBGPRINTLN(A)  if (dbg) {Serial.println(A);}
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))

// Delays
#define RetryCount 4
#define StartupDelay 200
#define ReadingDelay 750

// Audio
#define PeakAmplitude (1024 / 2)
#define MaxSampleCount ULONG_MAX

//Pins
#define PresenceSpoof 23
#define ColorSensorInt 24
#define PresenceIrSensorHmd1 2
#define ColorSensorPower0Hmd1 22
#define ColorSensorPower1Hmd1 25
#define PresenceIrSensorHmd2 3
#define ColorSensorPower0Hmd2 26
#define ColorSensorPower1Hmd2 27


struct _HmdPins
{
    int Hmd = 1;
    int PresenceSpoofer = PresenceSpoof;
    int PresenceIrSensor = PresenceIrSensorHmd1;
    int ColorSensorPower0 = ColorSensorPower0Hmd1;
    int ColorSensorPower1 = ColorSensorPower1Hmd1;
    int ColorSensorInterrupt = ColorSensorInt;
}HmdPins;


Adafruit_TCS34725 tcs = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_700MS, TCS34725_GAIN_60X);

void HMD_SetHmdCB(SerialCommand* cmd);
void HMD_GetDisplayBrightnessCB(SerialCommand* cmd);
void HMD_GetDisplayColorCB(SerialCommand* cmd);
void HMD_SetPresenceCB(SerialCommand* cmd);
void HMD_GetVolumeRmsCB(SerialCommand* cmd);
void HMD_GetVolumeAvgCB(SerialCommand* cmd);
void HMD_GetVolumePeakCB(SerialCommand* cmd);
void HMD_SetVolumeSampleCountCB(SerialCommand* cmd);
bool HMD_SetHmd(int hmd);
void HMD_PresenceOn();
void HMD_PresenceOff();
void HMD_IrIsr();
void HMD_GetVolume();
void HMD_Usage();
bool HMD_UpdateColorSensorValues(int displayIndex);
void AddSerCommand(const char* cmd, void(*function)(SerialCommand*));
extern bool dbg;

bool gHmdPresenceTrigger = false;
unsigned long gSampleCount = 2048;
uint64_t gVolumeAvg = 0, gVolumeRms = 0, gVolumeMax = 0;
uint16_t gRed, gGreen, gBlue, gClear = 0;

void HMD_setup()
{
    DBGPRINTLN(F("// HMD Exerciser Initialized "));
    AddSerCommand("presence", HMD_SetPresenceCB);
    AddSerCommand("brightness", HMD_GetDisplayBrightnessCB);
    AddSerCommand("color", HMD_GetDisplayColorCB);
    AddSerCommand("getvolumerms", HMD_GetVolumeRmsCB);
    AddSerCommand("getvolumeavg", HMD_GetVolumeAvgCB);
    AddSerCommand("getvolumepeak", HMD_GetVolumePeakCB);
    AddSerCommand("svsc", HMD_SetVolumeSampleCountCB);
    AddSerCommand("sethmd", HMD_SetHmdCB);

    for (int i = 2; i > 0; i--)
    {
        HMD_SetHmd(i);
        // Color Sensor
        pinMode(HmdPins.ColorSensorPower0, OUTPUT);
        pinMode(HmdPins.ColorSensorPower1, OUTPUT);
        pinMode(HmdPins.ColorSensorInterrupt, INPUT);
        digitalWrite(HmdPins.ColorSensorPower0, LOW);
        digitalWrite(HmdPins.ColorSensorPower1, LOW);

        // Presence Spoofing
        pinMode(HmdPins.PresenceSpoofer, OUTPUT);
        digitalWrite(HmdPins.PresenceSpoofer, HIGH);
        pinMode(HmdPins.PresenceIrSensor, INPUT);
    }
    
    // Audio
    DIDR0 = 0x01; // Disable digital in for ADC 0
    ADCSRA = 0xE7; // ADC Enabled, Start Conversion, Auto Trigger, Clear Interrupt Flag, Interrupt Disable, Prescaler 32 (38.4khz sample rate)
    ADMUX = 0x40;

    SERVO_setup();
}

void HMD_SetPresenceCB(SerialCommand* cmd)
{
    char buffer[2];
    DBGPRINTLN(F("// SetPresence "));
    char *arg = cmd->next();
    int presence = 0;

    if (arg == NULL)
    {
        return;
    }

    presence = atoi(arg);

    switch (presence)
    {
    case 0:
        HMD_PresenceOff();
        DBGPRINTLN(F("// Presence Spoofer Off "));
        break;
    case 1:
        HMD_PresenceOn();
        DBGPRINTLN(F("// Presence Spoofer On "));
        break;
    default:
        return;
    }

    sprintf_P(buffer, PSTR("%u"), presence);
    cmd->GetHardwareSerial()->println(buffer);
}

void HMD_SetHmdCB(SerialCommand* cmd)
{
    char buffer[2];
    DBGPRINTLN(F("// SetHmd "));
    char *arg = cmd->next();
    int hmd = 0;

    if (arg == NULL)
    {
        return;
    }

    hmd = atoi(arg);
    if(!HMD_SetHmd(hmd))
    {
        DBGPRINTLN(F("// Invalid HMD index. Valid values are 1 or 2 "));
        cmd->GetHardwareSerial()->println('0');
        return;
    }

    sprintf_P(buffer, PSTR("%u"), hmd);
    cmd->GetHardwareSerial()->println(buffer);
}

bool HMD_SetHmd(int hmd)
{
    HMD_PresenceOff();
    if (hmd == 1)
    {
        HmdPins.Hmd = 1;
		HmdPins.PresenceIrSensor = PresenceIrSensorHmd1;
		HmdPins.ColorSensorPower0 = ColorSensorPower0Hmd1;
		HmdPins.ColorSensorPower1 = ColorSensorPower1Hmd1;
    }
    else if (hmd == 2)
    {
        HmdPins.Hmd = 2;
		HmdPins.PresenceIrSensor = PresenceIrSensorHmd2;
		HmdPins.ColorSensorPower0 = ColorSensorPower0Hmd2;
		HmdPins.ColorSensorPower1 = ColorSensorPower1Hmd2;
    }
    else
    {
        return false;
    }

    return true;
}

void HMD_GetDisplayBrightnessCB(SerialCommand* cmd)
{
    char *arg = cmd->next();
    char buffer[8];
    int display;
    DBGPRINTLN(F("// GetDisplayBrightness "));

    if (arg == NULL)
    {
        display = 0;
    }
    else
    {
        display = atoi(arg);
    }

    display = atoi(arg);

    DBGPRINT(F("// Display: "));
    DBGPRINTLN(display);

    if (HMD_UpdateColorSensorValues(display))
    {
        // Send gClear value response to serial
        sprintf_P(buffer, PSTR("%7u"), gClear);
        cmd->GetHardwareSerial()->println(buffer);
    }
}

void HMD_GetDisplayColorCB(SerialCommand* cmd)
{
    char *arg = cmd->next();
    char buffer[24];
    int display;
    DBGPRINTLN(F("// GetDisplayColor "));

    if (arg == NULL)
    {
        display = 0;
    }
    else
    {
        display = atoi(arg);
    }

    if (HMD_UpdateColorSensorValues(display))
    {
        // Send RGB value response to serial
        sprintf_P(buffer, PSTR("%7u %7u %7u"), gRed, gGreen, gBlue);
        cmd->GetHardwareSerial()->println(buffer);
    }
}

void HMD_GetVolumeRmsCB(SerialCommand* cmd)
{
    DBGPRINTLN(F("// GetVolumeRms "));
    char buffer[5] = "";
    
    HMD_GetVolume();
    
    // Send volume response to serial
    sprintf_P(buffer, PSTR("%3u"), gVolumeRms);
    cmd->GetHardwareSerial()->println(buffer);
}

void HMD_GetVolumePeakCB(SerialCommand* cmd)
{
    DBGPRINTLN(F("// GetVolumePeak "));
    char buffer[5] = "";

    HMD_GetVolume();

    // Send volume response to serial
    sprintf_P(buffer, PSTR("%3u"), gVolumeMax);
    cmd->GetHardwareSerial()->println(buffer);
}

void HMD_GetVolumeAvgCB(SerialCommand* cmd)
{
    DBGPRINTLN(F("// GetVolumeAvg "));
    char buffer[5] = "";

    HMD_GetVolume();

    // Send volume response to serial
    sprintf_P(buffer, PSTR("%3u"), gVolumeAvg);
    cmd->GetHardwareSerial()->println(buffer);
}

void HMD_SetVolumeSampleCountCB(SerialCommand* cmd)
{
    char *arg = cmd->next();
    char buffer[12];
    unsigned long sampleCount;
    DBGPRINTLN(F("// SetVolumeSampleCount "));

    if (arg == NULL)
    {
        return;
    }
    else
    {
        sampleCount = strtoul(arg, NULL, 10);
        DBGPRINT(F("// Received Sample Count: "));
        DBGPRINTLN(arg);
        if (sampleCount < 1)
        {
            cmd->GetHardwareSerial()->println("-1");
        }
        else if(sampleCount > MaxSampleCount)
        {
            DBGPRINT(F("// Sample count "));
            DBGPRINTLN(arg);
            DBGPRINT(F("is greater than max sample count "));
            DBGPRINT(MaxSampleCount);
            cmd->GetHardwareSerial()->println("-1");
        }
        else
        {
            gSampleCount = sampleCount;

            // Respond with sample count
            sprintf_P(buffer, PSTR("%10lu"), gSampleCount);
            cmd->GetHardwareSerial()->println(buffer);
        }
    }
}

bool HMD_UpdateColorSensorValues(int displayIndex)
{
    char buffer[8];
    bool sensorRead = false;

    // Set default readings to zero
    gRed = 0;
    gGreen = 0;
    gBlue = 0;
    gClear = 0;

    DBGPRINTLN(F("// Turning on display sensor "));

    // Turn on appropriate color sensor
    switch (displayIndex)
    {
    case 0:
        digitalWrite(HmdPins.ColorSensorPower0, HIGH);
        break;
    case 1:
        digitalWrite(HmdPins.ColorSensorPower1, HIGH);
        break;
    default:
        DBGPRINT(F("// No display for that display index "));
        return false;
    }

    DBGPRINTLN(F("// Waiting for sensor to power up "));

    // Wait for sensor to power up
    delay(StartupDelay);

    DBGPRINTLN(F("// Reading sensor value "));

    // Attempt to talk to the sensor
    for (int i = 0; i < RetryCount; i++)
    {
        // Read value if sensor is present
        if (tcs.begin())
        {
            tcs.setIntegrationTime(TCS34725_INTEGRATIONTIME_700MS);
            tcs.setGain(TCS34725_GAIN_60X);

            //Since we've just powered up, we need to wait for a reading to complete
            delay(ReadingDelay);

            DBGPRINTLN(F("// Getting raw data from sensor "));

            // Grab the RGBC values
            tcs.getRawData(&gRed, &gGreen, &gBlue, &gClear);
            sensorRead = true;

            DBGPRINT(F("// R: "));
            DBGPRINTLN(utoa(gRed, buffer, 10));
            DBGPRINT(F("// G: "));
            DBGPRINTLN(utoa(gGreen, buffer, 10));
            DBGPRINT(F("// B: "));
            DBGPRINTLN(utoa(gBlue, buffer, 10));
            DBGPRINT(F("// gClear: "));
            DBGPRINTLN(utoa(gClear, buffer, 10));
            break;
        }
    }

    // Turn off color sensor
    switch (displayIndex)
    {
    case 0:
        digitalWrite(HmdPins.ColorSensorPower0, LOW);
        break;
    case 1:
        digitalWrite(HmdPins.ColorSensorPower1, LOW);
        break;
    }
    
    return sensorRead;
}

void HMD_PresenceOn()
{
    // Attach an interrupt to the IR sensor to control presence spoofing time synchronization
    attachInterrupt(digitalPinToInterrupt(HmdPins.PresenceIrSensor), HMD_IrIsr, RISING);
    digitalWrite(HmdPins.PresenceSpoofer, LOW);
}

void HMD_PresenceOff()
{
    // Disable the interrupt and set the presence LED full on to drown out the presence sensor and fake non-presence
    detachInterrupt(digitalPinToInterrupt(HmdPins.PresenceIrSensor));
    digitalWrite(HmdPins.PresenceSpoofer, HIGH);
}

void HMD_IrIsr()
{
    gHmdPresenceTrigger = true;
}

void HMD_PulseLed()
{
    // Fake the blinking sequence of the HMD presence sensor
    PORTA |= B00000010;
    delayMicroseconds(40);
    PORTA &= ~B00000010;
    delayMicroseconds(4686);
    PORTA |= B00000010;
    delayMicroseconds(40);
    PORTA &= ~B00000010;
}

void HMD_GetVolume()
{
    char buffer[8];
    gVolumeMax = 0;
    gVolumeAvg = 0;
    gVolumeRms = 0;

    for (uint64_t i = 0; i < gSampleCount; i++)
    {
        while (!(ADCSRA & /*0x10*/_BV(ADIF))); // Wait for the ADIF bit of the ADC status register to be signaled
        sbi(ADCSRA, ADIF); // Restart the ADC
        
        // Grab the low and high bytes and concatenate them
        byte l = ADCL;
        byte h = ADCH;
        int reading = ((int)h << 8) | l;
        unsigned int amplitude = abs(reading - PeakAmplitude);

        // Compare with previous entry for max
        gVolumeMax = max(gVolumeMax, amplitude);
        // Sum total for avg
        gVolumeAvg += amplitude;
        // Sum squares for RMS
        gVolumeRms += ((unsigned long)amplitude * amplitude);
    }
    gVolumeMax = 100 * gVolumeMax / PeakAmplitude;
    gVolumeAvg = ((gVolumeAvg / gSampleCount) * 100) / PeakAmplitude;
    gVolumeRms = (sqrt(gVolumeRms / gSampleCount) * 100) / PeakAmplitude;
    gVolumeRms = gVolumeRms * 0.7; // Approximate peak using .7 for sine wave
    DBGPRINT(F("Volume Rms: "));
    DBGPRINTLN(utoa(gVolumeRms, buffer, 10));
    DBGPRINT(F("Volume Avg: "));
    DBGPRINTLN(utoa(gVolumeAvg, buffer, 10));
    DBGPRINT(F("Volume Peak: "));
    DBGPRINTLN(utoa(gVolumeMax, buffer, 10));
}

void HMD_Usage()
{
    DBGPRINTLN(F("sethmd [hmd]"));
    DBGPRINTLN(F("    selects the HMD to target for brightness, color, and presence commands"));
    DBGPRINTLN(F("brightness [display]"));
    DBGPRINTLN(F("    returns the brightness of the specified display"));
    DBGPRINTLN(F("color [display]"));
    DBGPRINTLN(F("    returns the RGB color of the specified display"));
    DBGPRINTLN(F("presence [option]"));
    DBGPRINTLN(F("    0, turns the presence sensor off"));
    DBGPRINTLN(F("    1, turns the presence sensor on"));
    DBGPRINTLN(F("getvolumerms"));
    DBGPRINTLN(F("    returns the rms volume of the audio waveform"));
    DBGPRINTLN(F("getvolumepeak"));
    DBGPRINTLN(F("    returns the absolute value peak value of the waveform"));
    DBGPRINTLN(F("getvolumeavg"));
    DBGPRINTLN(F("    returns the avg volume of the absolute value of the audio waveform"));
    DBGPRINTLN(F("svsc [sample count]"));
    DBGPRINT(F("    sets the number of samples to take from the waveform. Default is 2048, max is "));
    DBGPRINTLN(MaxSampleCount);

    SERVO_Usage();
}
