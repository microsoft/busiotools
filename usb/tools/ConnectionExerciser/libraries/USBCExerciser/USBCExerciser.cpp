/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

#include <SoftwareSerial.h>
#include <SerialCommand.h>
#include <LiquidCrystal.h>
#include <Event.h>
#include <Timer.h>
#include <Wire.h>
#include "DualRoleConnectionExerciser.h"

void AddSerCommand(const char* cmd, void(*function)(SerialCommand*));
extern Timer timer;
extern LiquidCrystal *lcd;
extern bool dbg;
extern uint32_t cmdDelay;
extern uint32_t disconnectTimeout;

byte switchToPortNum = 0;
bool enableSuperSpeed = true;

#define DBGPRINT(A)    if (dbg) {Serial.print(A);}
#define DBGPRINTLN(A)  if (dbg) {Serial.println(A);}

                            // PK[0]
#define kOEN          A9    // PK[1]
#define kSEL1         A10   // PK[2]
#define kSEL0         A11   // PK[3]
#define kALERT        A12   // PK[4]
                            // PK[5]
#define kSS_OEN       A14   // PK[6]
#define kSS_SEL       A15   // PK[7]

/*
#define kCCSEL0       A8    // PK[0]
#define kCCSEL1       A9    // PK[1]
#define kUSB1SEL0     A11   // PK[2]
#define kUSB1SEL1     A10   // PK[3]
#define kOEN          A11   // PK[4]
#define kSEL          A12   // PK[5]
#define kUSB2SEL1     A13   // PK[6]
#define kUSB2SEL0     A14   // PK[7]
*/

#define kPOWER_SELA   7     // PH[4]
#define kPOWER_SELB   8     // PH[5]
#define kPOWER_EN     9     // PH[6]

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
extern void (*SuperSpeed)(bool);

void USBC_PrintVoltsCB(SerialCommand* cmd);
void USBC_PrintAmpsCB(SerialCommand* cmd);
void USBC_ChangePortCB(SerialCommand* cmd);
void USBC_SetPort(byte port);
char USBC_GetPort(void);
void USBC_UpdateDisplayCB(void);
void USBC_Usage();
float USBC_ReadVoltage(void);
float USBC_ReadCurrent(void);
void USBC_DisconnectUSB(void);
void USBC_CyclePorts(void);
void USBC_UpdateVoltageString(float volts, char *strVolts );
void USBC_UpdateCurrentString(float amps, char *strAmps );
void USBC_SuperSpeedCB(SerialCommand* cmd);
void USBC_SuperSpeed( bool enable );
void USBC_SetCmdDelayCB(SerialCommand* cmd);
void USBC_SetDisconnectTimeoutCB(SerialCommand* cmd);
void USBC_SetPort_Internal(void);

void USBC_setup()
{
    //
    // Hook up function pointers
    //
    ReadVoltage = &USBC_ReadVoltage;
    ReadCurrent = &USBC_ReadCurrent;
    SetPort = &USBC_SetPort;
    GetPort = &USBC_GetPort;
    SuperSpeed = &USBC_SuperSpeed;

    //
    // Register serial command callbacks
    //
    AddSerCommand("volts", USBC_PrintVoltsCB);
    AddSerCommand("amps", USBC_PrintAmpsCB);
    AddSerCommand("port", USBC_ChangePortCB);
    AddSerCommand("delay", USBC_SetCmdDelayCB);
    AddSerCommand("timeout", USBC_SetDisconnectTimeoutCB);
    AddSerCommand("superspeed", USBC_SuperSpeedCB);

    // 1602A LCD module
    pinMode(kRS, OUTPUT);
    pinMode(kRW, OUTPUT);
    pinMode(kEN, OUTPUT);

    // ADC
    pinMode(A0, INPUT);
    pinMode(A1, INPUT);
    pinMode(A2, INPUT);

    // Port K
    pinMode(   kOEN, OUTPUT);
    pinMode(  kSEL0, OUTPUT);
    pinMode(  kSEL1, OUTPUT);
    pinMode( kALERT, INPUT);
    pinMode(kSS_OEN, OUTPUT);
    pinMode(kSS_SEL, OUTPUT);
    DDRK = 0xFF;
    PORTK = 0x10;

    pinMode(kPOWER_SELA, OUTPUT);
    pinMode(kPOWER_SELB, OUTPUT);
    pinMode(kPOWER_EN, OUTPUT);


    // Initialize
    digitalWrite(kRS, LOW);
    digitalWrite(kRW, LOW);
    digitalWrite(kEN, LOW);
    digitalWrite(kOEN, HIGH);
    digitalWrite(kSS_OEN, HIGH);
    digitalWrite(kPOWER_SELA, LOW);
    digitalWrite(kPOWER_SELB, LOW);
    digitalWrite(kPOWER_EN, LOW);
    analogReference(INTERNAL2V56);

    Wire.begin();
    Wire.setClock( 400000 );

    Wire.beginTransmission( 0x40 );
    Wire.write( 0x05 );
    Wire.write( 0x02 );
    Wire.write( 0x00 );
    Wire.endTransmission( true );

    Wire.beginTransmission( 0x40 );
    Wire.write( 0x00 );
    Wire.write( 0x48 );
    Wire.write( 0x07 );
    Wire.endTransmission( true );


    lcd = new LiquidCrystal(kRS, kRW, kEN, 37, 36, 35, 34, 33, 32, 31, 30);
    lcd->begin(kLCDCOLS, kLCDROWS);
    timer.every( 500, USBC_UpdateDisplayCB);

    if ( PINL & 0x01 )
        timer.every( 3000, USBC_CyclePorts);

    SetPort(3);
}

void USBC_PrintVoltsCB(SerialCommand* cmd)
{
    char Buffer[8];
    float f;
    int i;

    f = USBC_ReadVoltage();
    i = (int)f;

    sprintf_P( Buffer, PSTR("%02d%02d"), i, abs((int)((f - (float)i) * 100.00)) );
    cmd->GetHardwareSerial()->println(Buffer);
}

void USBC_PrintAmpsCB(SerialCommand* cmd)
{
    char Buffer[8];
    float f;
    int i;

    f = ReadCurrent();
    i = (int)f;
    if ( f < 0.0 )
        sprintf_P( Buffer, PSTR("1%d%02d"), -1 * i, abs((int)((f - (float)i) * 100.00)) );
    else
        sprintf_P( Buffer, PSTR("%02d%02d"), i, abs((int)((f - (float)i) * 100.00)) );
    cmd->GetHardwareSerial()->println(Buffer);
}

void USBC_CyclePorts()
{
}

void USBC_SuperSpeedCB(SerialCommand* cmd)
{
    char *arg = cmd->next();

    if ( arg != NULL )
    {
        cmd->GetHardwareSerial()->print(F("superspeed "));
        cmd->GetHardwareSerial()->println(arg);

        USBC_SuperSpeed( atoi(arg) );
    }
}

void USBC_SuperSpeed( bool enable )
{
    if (enable)
    {
        DBGPRINTLN(F("// Enable superspeed switches"));
    }
    else
    {
        DBGPRINTLN(F("// Disable superspeed switches"));
    }

    if (enable != enableSuperSpeed)
    {
        // Set the enable state no matter what port is current connected
        enableSuperSpeed = enable;

        byte port = (((PINH & 0x30) >> 4) + 1);

        if (1 == port || 2 == port)
        {
            // Re-connect to the same port only if on SuperSpeed port
            USBC_SetPort(port);
        }
    }
}

void USBC_SetCmdDelayCB(SerialCommand* cmd)
{
    char* arg = cmd->next();

    cmdDelay = atoi(arg);

    cmd->GetHardwareSerial()->print(F("delay "));
    cmd->GetHardwareSerial()->println(cmdDelay);

    cmdDelay *= 1000; // Convert to ms
}

void USBC_SetDisconnectTimeoutCB(SerialCommand* cmd)
{
    char* arg = cmd->next();

    disconnectTimeout = atoi(arg);

    cmd->GetHardwareSerial()->print(F("timeout "));
    cmd->GetHardwareSerial()->println(disconnectTimeout);
}

void USBC_ChangePortCB(SerialCommand* cmd)
{
    char *arg = cmd->next();

    if ( arg == NULL ) {
        cmd->GetHardwareSerial()->println(USBC_GetPort());
        return;
    }

    cmd->GetHardwareSerial()->print(F("port "));
    cmd->GetHardwareSerial()->println(atoi(arg));

    USBC_SetPort( atoi(arg) );
}

void USBC_SetPort( byte port )
{
    switchToPortNum = port;

    timer.after(cmdDelay, USBC_SetPort_Internal);
    cmdDelay = 0;
}

void USBC_SetPort_Internal()
{
    byte mux[4] = {0x3, 0x2, 0x0, 0x1};

    USBC_DisconnectUSB();

    // NOTE: see PK[] defines above to interpret PORTK
    if (switchToPortNum >= 1 && switchToPortNum <= 4) {
        switch (switchToPortNum){
        case 1:
            digitalWrite( kPOWER_SELA, LOW );
            digitalWrite( kPOWER_SELB, LOW );
            digitalWrite( kPOWER_EN, HIGH );

            PORTK = (enableSuperSpeed) ? 0x0C : 0x4C;
            break;
        case 2:
            digitalWrite( kPOWER_SELA, HIGH );
            digitalWrite( kPOWER_SELB, LOW );
            digitalWrite( kPOWER_EN, HIGH );

            PORTK = (enableSuperSpeed) ? 0x84 : 0xC4;
            break;
        case 3:
            digitalWrite( kPOWER_SELA, HIGH );
            digitalWrite( kPOWER_SELB, HIGH );
            digitalWrite( kPOWER_EN, HIGH );

            PORTK = 0x08;
            break;
        case 4:
            digitalWrite( kPOWER_SELA, LOW );
            digitalWrite( kPOWER_SELB, HIGH );
            digitalWrite( kPOWER_EN, HIGH );

            PORTK = 0x00;
        }

        if (0 != disconnectTimeout)
        {
            timer.after(disconnectTimeout, USBC_DisconnectUSB);
            disconnectTimeout = 0;
        }
    }
}

char USBC_GetPort()
{
    char port = '0';
    uint8_t tmp;

    if ( digitalRead( kPOWER_EN ) )
    {
        // port = ((PINH & 0x30) >> 4) + '1';
        tmp = (PINH & 0x30) >> 4;
        switch (tmp)
        {
        case 0: port = '1'; break;
        case 1: port = '2'; break;
        case 2: port = '4'; break;
        case 3: port = '3'; break;
        }
    }

    return port;
}

void USBC_Usage()
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
    DBGPRINTLN(F("delay [seconds]"));
    DBGPRINTLN(F("    seconds to delay the next port change."));
    DBGPRINTLN(F("timeout [miliseconds]"));
    DBGPRINTLN(F("    next port change will disconnect after specified number of ms."));
}

void USBC_UpdateDisplayCB()
{
    char tmpStr[10];
  
    lcd->home();
    USBC_UpdateVoltageString( USBC_ReadVoltage(), tmpStr );
    lcd->print( tmpStr );
    lcd->setCursor(1,1);
    USBC_UpdateCurrentString( USBC_ReadCurrent(), tmpStr );
    lcd->print( tmpStr );
}


float USBC_ReadVoltage()
{
    int v;
    float volts;

    Wire.beginTransmission( 0x40 );
    Wire.write( 0x02 );
    Wire.endTransmission( true );
    Wire.requestFrom( 0x40, 2 );
    if ( Wire.available() == 2 )
    {
        v = Wire.read();
        v = (v << 8) | Wire.read();
        volts = v * 0.00125;
    }
    return volts;
}

float USBC_ReadCurrent()
{
    uint16_t raw;
    int i;
    float amps;

    Wire.beginTransmission( 0x40 );
    Wire.write( 0x04 );
    Wire.endTransmission( true );
    Wire.requestFrom( 0x40, 2 );
    if ( Wire.available() == 2 )
    {
        raw = Wire.read();
        raw = raw << 8 | Wire.read();
        i = (int16_t)raw;
        amps = i * 0.001;
    }
    return amps;
}

void USBC_DisconnectUSB()
{
    // Disconnect data signals
    digitalWrite(kOEN, HIGH);
    digitalWrite(kSS_OEN, HIGH);

    // Disconnect power
    digitalWrite(kPOWER_EN, LOW);

    // Stay in the disconnected state for at least 200 ms to allow
    // any high voltage contract to be disconnected by the supply
    delay(200);
}

void USBC_UpdateVoltageString(float volts, char *strVolts)
{
    int i, f;
  
    i = (int)volts;
    f = abs((int)((volts - (float)i) * 100.00));  
    sprintf_P( strVolts, PSTR(" %2d.%02d V"), i, f );
}

void USBC_UpdateCurrentString(float amps, char *strAmps)
{
    int i, f;
  
    i = abs((int)amps);
    f = (int)((abs(amps) - (float)i) * 100.00);  

    if ( amps < 0.0 )
    {
        sprintf_P( strAmps, PSTR("-%d.%02d A"), i, f );
    } else {
        sprintf_P( strAmps, PSTR(" %d.%02d A"), i, f );
    }
}

