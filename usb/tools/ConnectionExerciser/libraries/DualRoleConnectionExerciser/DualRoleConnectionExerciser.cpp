/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

#include <SoftwareSerial.h>
#include <SerialCommand.h>
#include <LiquidCrystal.h>
#include <Event.h>
#include <Timer.h>
#include "DualRoleConnectionExerciser.h"

void AddSerCommand(const char* cmd, void(*function)(SerialCommand*));
extern Timer timer;
extern LiquidCrystal *lcd;
extern bool dbg;

#define DBGPRINT(A)    if (dbg) {Serial.print(A);}
#define DBGPRINTLN(A)  if (dbg) {Serial.println(A);}

#define kIdEnable     5 // DUT USB port kIdEnable select (0 = floating, 1 = ground)
#define kUsbEnableN   6 // USB mux output enable (0 = connect, 1 = disconnect)
#define kSELA         7 // USB mux/PMIC select line A
#define kSELB         8 // USB mux/PMIC select line B
#define kPowerEnable  9 // Output enable for VBUS mux
#define kGroundIdPin 10 // Switch to ground the ID pin (0 = floating, 1 = ground)
#define USBMUX(A) PORTH= (PINH & 0xCF) | (A) << 4

#define kLCDROWS      2 // Number of rows on the LCD module
#define kLCDCOLS      8 // Number of columns on the LCD module
#define kRS          41 // LCD module register select line (0 = command, 1 = data input)
#define kRW          38 // Read not write (0 = write, 1 = read)
#define kEN          39 // LCD module enable

#define kVREF 2.56       // Internal reference voltage
#define kCURROFFSET 0.05 // Voltage offset of differential ADC for current measurement
#define kVOLTAGESCALEFACTOR 5.5 // see description of PrintSummary()
#define kCURRENTSCALEFACTOR 10 // see description of PrintSummary()
#define kADCSCALAR (kVREF * ((10.0 + 5.0)/5.0) / 1024.0) // see description of PrintSummary()
#define kDIFFADCSCALAR (kVREF * kCURRENTSCALEFACTOR / 512.0) // see description of PrintSummary()

//
// Function pointers used by DTMF routines.  These are populated
// in the setup routine.
//
extern float (*ReadVoltage)();
extern float (*ReadCurrent)();
extern void (*SetPort)(byte);
extern char (*GetPort)();
extern void (*ShieldLoop)();

void DRCE_PrintVoltsCB(SerialCommand* cmd);
void DRCE_PrintAmpsCB(SerialCommand* cmd);
void DRCE_ChangePortCB(SerialCommand* cmd);
void DRCE_SetPort(byte port);
char DRCE_GetPort(void);
void DRCE_UpdateDisplayCB(void);
void DRCE_Usage();
float DRCE_ReadVoltage(void);
float DRCE_ReadCurrent(void);
void DRCE_DisconnectUSB(void);
void DRCE_CyclePorts(void);
void DRCE_UpdateVoltageString(float volts, char *strVolts );
void DRCE_UpdateCurrentString(float amps, char *strAmps );
void DRCE_Loop(void);

uint16_t *VoltageReadings;
int16_t *CurrentReadings;
uint8_t  ReadingsIndex;


void DRCE_setup()
{
    //
    // Hook up function pointers
    //
    ReadVoltage = &DRCE_ReadVoltage;
    ReadCurrent = &DRCE_ReadCurrent;
    SetPort = &DRCE_SetPort;
    GetPort = &DRCE_GetPort;
    ShieldLoop = &DRCE_Loop;

    VoltageReadings = new uint16_t[128];
    CurrentReadings = new int16_t[128];
    for ( ReadingsIndex = 0; ReadingsIndex < 128; ReadingsIndex++ )
    {
        VoltageReadings[ReadingsIndex] = 0;
        CurrentReadings[ReadingsIndex] = 0;
    }
    ReadingsIndex = 0;

    //
    // Register serial command callbacks
    //
    AddSerCommand("volts", DRCE_PrintVoltsCB);
    AddSerCommand("amps", DRCE_PrintAmpsCB);
    AddSerCommand("port", DRCE_ChangePortCB);

    // 1602A LCD module
    pinMode(kRS, OUTPUT);
    pinMode(kRW, OUTPUT);
    pinMode(kEN, OUTPUT);

    // ADC
    pinMode(A0, INPUT);
    pinMode(A1, INPUT);
    pinMode(A2, INPUT);

    // PMIC and USB MUX
    pinMode(kIdEnable, OUTPUT);
    pinMode(kGroundIdPin, OUTPUT);
    pinMode(kUsbEnableN, OUTPUT);
    pinMode(kSELA, OUTPUT);
    pinMode(kSELB, OUTPUT);
    pinMode(kPowerEnable, OUTPUT);

    //
    // Initialize pins
    //
    digitalWrite(kRS, LOW);
    digitalWrite(kRW, LOW);
    digitalWrite(kEN, LOW);
    digitalWrite(A3, LOW);
    digitalWrite(A4, HIGH);
    digitalWrite(kIdEnable, LOW);   // Initialize ID pin floating, DUT is peripheral.
    digitalWrite(kGroundIdPin, LOW );
    digitalWrite(kUsbEnableN, HIGH);  // Initialize USB mux disabled, no ports connected.
    digitalWrite(kSELA, LOW);
    digitalWrite(kSELB, LOW);
    digitalWrite(kPowerEnable, LOW);
    analogReference(INTERNAL2V56);

    lcd = new LiquidCrystal(kRS, kRW, kEN, 37, 36, 35, 34, 33, 32, 31, 30);
    lcd->begin(kLCDCOLS, kLCDROWS);
    timer.every( 500, DRCE_UpdateDisplayCB);

    if ( PINL & 0x01 )
        timer.every( 3000, DRCE_CyclePorts);
}

void DRCE_Loop()
{
    VoltageReadings[ReadingsIndex] = analogRead(A2);
    CurrentReadings[ReadingsIndex] = analogRead(A0) - analogRead(A1);
    ReadingsIndex = (ReadingsIndex + 1) % 128;
}

void DRCE_PrintVoltsCB(SerialCommand* cmd)
{
    char Buffer[8];
    float f;
    int i;

    f = DRCE_ReadVoltage();
    i = (int)f;

    sprintf_P(Buffer, PSTR("%02d%02d"), i, abs((int)((f - (float)i) * 100.00)));
    cmd->GetHardwareSerial()->println(Buffer);
}

void DRCE_PrintAmpsCB(SerialCommand* cmd)
{
    char Buffer[8];
    float f;
    int i;

    f = DRCE_ReadCurrent();
    i = (int)f;
    if (f < 0.0)
        sprintf_P(Buffer, PSTR("1%d%02d"), -1 * i, abs((int)((f - (float)i) * 100.00)));
    else
        sprintf_P(Buffer, PSTR("%02d%02d"), i, abs((int)((f - (float)i) * 100.00)));
    cmd->GetHardwareSerial()->println(Buffer);
}

void DRCE_CyclePorts()
{
    byte mux[4] = {0x3, 0x2, 0x0, 0x1};

    DRCE_DisconnectUSB();
    delay(100);


    switch ((PINH & 0x30) >> 4)
    {
        case 0x3: USBMUX( 0x2 ); break;
        case 0x2: USBMUX( 0x0 ); break;
        case 0x0: USBMUX( 0x1 ); break;
        case 0x1: USBMUX( 0x3 ); break;
    }
    digitalWrite(kPowerEnable, HIGH );
    delay(5);

    if ( ((PINH & 0x30) >> 4) >= 0x2 ) {
        digitalWrite( kGroundIdPin, HIGH );
    } else {
        digitalWrite( kGroundIdPin, LOW );
    }
    digitalWrite(kIdEnable, LOW);
    digitalWrite(kUsbEnableN, LOW);

    switch ((PINH & 0x30) >> 4)
    {
        case 0x0: DBGPRINTLN( F("PORT 3") ); break;
        case 0x1: DBGPRINTLN( F("PORT 4") ); break;
        case 0x2: DBGPRINTLN( F("PORT 2") ); break;
        case 0x3: DBGPRINTLN( F("PORT 1") ); break;
    }
}

void DRCE_ChangePortCB(SerialCommand* cmd)
{
    char *arg = cmd->next();

    if ( arg == NULL ) {
        cmd->GetHardwareSerial()->println(DRCE_GetPort());
        return;
    }

    cmd->GetHardwareSerial()->print(F("port "));
    cmd->GetHardwareSerial()->println(atoi(arg));

    DRCE_SetPort( atoi(arg) );
}

void DRCE_SetPort( byte port )
{
    byte mux[4] = {0x3, 0x2, 0x0, 0x1};

    DRCE_DisconnectUSB();
    delay(100);
    
    if ( port >= 1 && port <= 4 ) {
        digitalWrite(kPowerEnable, HIGH);
    
        DBGPRINT(F("DRCE_SetPort() : "));
        DBGPRINTLN(port);
      
        USBMUX( mux[port-1] );

        delay(5);
        if ( port == 1 || port == 2 ) {
            digitalWrite( kGroundIdPin, HIGH );
        } else {
            digitalWrite( kGroundIdPin, LOW );
        }
        digitalWrite(kIdEnable, LOW);
        digitalWrite(kUsbEnableN, LOW);
    } else {
        DBGPRINTLN(F("USB ports are all off"));
    }
    
}

char DRCE_GetPort()
{
    char table[4] = {'3', '4', '2', '1'};

    DBGPRINTLN( F("DRCE_GetPort()") );

    if ( digitalRead( kUsbEnableN ) == HIGH ) {
        return '0';
    }

    return table[ (PINH & 0x30) >> 4 ];
}

void DRCE_Usage()
{
    DBGPRINTLN(F("volts"));
    DBGPRINTLN(F("    shows voltage. no parameters."));
    DBGPRINTLN(F("amps"));
    DBGPRINTLN(F("    shows amperage. no parameters."));
    DBGPRINTLN(F("port [options]"));
    DBGPRINTLN(F("    <empty>, shows current USB port."));
    DBGPRINTLN(F("    1, connects to USB port 1"));
    DBGPRINTLN(F("    2, connects to USB port 2"));
    DBGPRINTLN(F("    3, connects to USB port 3"));
    DBGPRINTLN(F("    0 (or other), disconnect all ports"));
}

void DRCE_UpdateDisplayCB()
{
    char tmpStr[10];
  
    lcd->home();
    DRCE_UpdateVoltageString( DRCE_ReadVoltage(), tmpStr );
    lcd->print( tmpStr );
    lcd->setCursor(1,1);
    DRCE_UpdateCurrentString( DRCE_ReadCurrent(), tmpStr );
    lcd->print( tmpStr );
}


float DRCE_ReadVoltage()
{
    uint32_t v;
    uint8_t i;
    float volts;

    v = 0;
    for ( i = 0; i < 128; i++ )
    {
        v += VoltageReadings[i];
    }
    v = v / 128;
  
    // v = analogRead(A2);

    volts = v * 2.56 / 1024.0 * (100.0 + 49.9)/49.9;
    //
    // The circuit will cause VBUS to be less than 0.5 volts when nothing is connected.
    // Return zero when this condition is detected.
    //
    if ( volts < 0.75 )
        volts = 0.0;
    return volts;
}

float DRCE_ReadCurrent()
{
    int32_t i;
    uint8_t index;
    float amps;
  
    i = 0;
    for ( index = 0; index < 128; index++ )
    {
        i += CurrentReadings[index];
    }
    i = i / 128;

    // i = analogRead(A0) - analogRead(A1);
    amps =  -1.0 * i * 2.56 / 1024.0;

    return amps;
}

void DRCE_DisconnectUSB()
{
    digitalWrite(kUsbEnableN, HIGH);
    digitalWrite(kIdEnable, LOW);
    digitalWrite(kGroundIdPin, LOW );
    delay(5);
    digitalWrite(kPowerEnable, LOW);
    delay(50);
}

void DRCE_UpdateVoltageString(float volts, char *strVolts)
{
    int i, f;
  
    i = (int)volts;
    f = abs((int)((volts - (float)i) * 100.00));  
    sprintf_P( strVolts, PSTR("  %d.%02d V"), i, f );
}

void DRCE_UpdateCurrentString(float amps, char *strAmps)
{
    int i, f;

    i = (int)amps;
    f = abs((int)((amps - (float)i) * 1000.00)); 
    if ( amps < 0.0 )
        *strAmps = '-';
    else if ( amps > 0.0 )
        *strAmps = '+';
    else
        *strAmps = ' ';

    sprintf_P( strAmps+1, PSTR("%d.%03dA"), i, f );
}

