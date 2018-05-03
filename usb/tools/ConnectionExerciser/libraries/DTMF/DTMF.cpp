/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

#include <SoftwareSerial.h>
#include <SerialCommand.h>
#include <Event.h>
#include <Timer.h>
#include "DTMF.h"

void AddSerCommand(const char* cmd, void(*function)(SerialCommand*));
extern Timer timer;
extern bool dbg;
extern uint32_t cmdDelay;
extern uint32_t disconnectTimeout;
extern void Usage();

#define DBGPRINT(A)    if (dbg) {Serial.print(A);}
#define DBGPRINTLN(A)  if (dbg) {Serial.println(A);}

#define kD0           22 // PA[0]
#define kD1           23 // PA[1]
#define kD2           24 // PA[2]
#define kD3           25 // PA[3]
#define kRDN          26 // PA[4]
#define kIRQN         27 // PA[5]
#define kRS0          28 // PA[6]
#define kWRN          29 // PA[7]

extern float (*ReadVoltage)();
extern float (*ReadCurrent)();
extern void (*SetPort)(byte);
extern char (*GetPort)();
extern void (*SuperSpeed)(bool);
extern byte VERSION;
extern byte shield;


void DTMF_loop();
void DTMF_SoftwareReset();
byte DTMF_ReadStatusRegister();
void DTMF_WriteControlRegister( byte value );
void DTMF_TransmitTone( byte data );
char DTMF_ReadData();
void DTMF_GenerateTone(SerialCommand* cmd);
byte DTMF_ToneToByte( char tone );
void DTMF_BuildCommandString( char c );
bool DTMF_HandleCommand( void );

bool TxReady = false;
char TxBuffer[8];
byte TxIndex = 0;
bool TxSendBuffer = false;
bool TxLastCharacter = false;
char RxBuffer[8];
byte RxIndex = 0;
extern uint8_t Data[10];

void DTMF_setup()
{
    //
    // Enable pull ups
    //
    PORTA = 0xD0;

    //
    // RS0 is pulled low on the DTMF board
    //
    if ( PINA & 0x40 == 0x40 )
    {
        //
        // No DTMF board
        //
        DBGPRINTLN( F("No DTMF present") );
        return;
    }

    AddSerCommand("tone", DTMF_GenerateTone);

    //
    // Configure outputs
    //
    DDRA = 0xD0;


    // Initialize
    DTMF_SoftwareReset();
    DTMF_WriteControlRegister( 0x0D );
    DTMF_WriteControlRegister( 0x00 );

    timer.every( 5, DTMF_loop );
    TxReady = true;
}

void DTMF_loop()
{
    byte status;

    if ( digitalRead( kIRQN ) == 0 )
    {
        status = DTMF_ReadStatusRegister();
        // DBGPRINT( F("// DTMF Status: ") );
        // DBGPRINTLN( status );

        if ((status & 0x02) == 0x02) {
            TxReady = true;
            if ( TxLastCharacter == true ) {
                //
                // Turn off tone output at end of last character
                //
                DTMF_WriteControlRegister( 0x04 );
                TxLastCharacter = false;
            }
        }
        if ((status & 0x04) == 0x04) {
            DTMF_BuildCommandString( DTMF_ReadData() );
        }
    }
    if ( TxSendBuffer && TxReady ) {
        if ( TxIndex == 0 ) {
            //
            // Turn on tone output at start of first character
            //
            DTMF_WriteControlRegister( 0x05 );
        }
        DTMF_TransmitTone( TxBuffer[TxIndex] );
        if ( TxBuffer[TxIndex] == '#' ) {
            DBGPRINTLN( TxBuffer[TxIndex] );
            TxLastCharacter = true;
            TxSendBuffer = false;
            TxIndex = 0;
        } else {
            DBGPRINT( TxBuffer[TxIndex] );
            TxIndex++;
        }
    }
}

void DTMF_BuildCommandString( char c )
{
    RxBuffer[RxIndex] = c;
    if ( c == '#' )
    {
        if ( DTMF_HandleCommand() )
        {
            delay(100);
            TxIndex = 0;
            TxSendBuffer = true;
        }
    } else {
        RxIndex++;
    }
}

bool DTMF_HandleCommand( void )
{
    bool SendData = false;
    float f;
    int i;
    uint8_t id;
    uint16_t data;

    //
    // A well formed command is terminated with a #
    //
    if ( RxIndex >= sizeof( RxBuffer) || RxBuffer[RxIndex] != '#' )
    {
        goto Exit;
    }

    //
    // Just a # was recieved
    //
    if ( RxIndex == 0 )
    {
        TxBuffer[0] = RxBuffer[0];
        SendData = true;
        goto Exit;
    }

    //
    // Handle single character commands
    //
    if ( RxIndex == 1 )
    {
        switch (RxBuffer[0])
        {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
                if ( SetPort )
                {
                    SetPort( RxBuffer[0] - '0' );
                    TxBuffer[0] = RxBuffer[0];
                    TxBuffer[1] = RxBuffer[1];
                    if ( RxBuffer[0] == '0' )
                    {
                        DBGPRINTLN( F("// Turn off ports") );
                    } else {
                        DBGPRINT( F("// Set port " ) );
                        DBGPRINTLN( RxBuffer[0] );
                    }
                    SendData = true;
                }
                break;
            case '5':
            case '6':
                if ( SuperSpeed )
                {
                    if ( RxBuffer[0] == '5' )
                    {
                        SuperSpeed(false);
                    } else {
                        SuperSpeed(true);
                    }
                    TxBuffer[0] = RxBuffer[0];
                    TxBuffer[1] = RxBuffer[1];
                    SendData = true;
                }
                break;
            case 'A':
                if ( GetPort )
                {
                    TxBuffer[0] = GetPort();
                    TxBuffer[1] = RxBuffer[1];
                    DBGPRINT( F("// Get Port : ") );
                    DBGPRINTLN( TxBuffer[0] );
                    SendData = true;
                }
                break;
            case 'B':
                if ( ReadVoltage )
                {
                    f = ReadVoltage();
                    i = (int)f;
                    sprintf_P( TxBuffer, PSTR("%02d%02d#"), i, abs((int)((f - (float)i) * 100.00)) );
                    DBGPRINT( F("// Volts: "));
                    DBGPRINTLN( TxBuffer );
                    SendData = true;
                }
                break;
            case 'C':
                if ( ReadCurrent )
                {
                    f = ReadCurrent();
                    i = (int)f;
                    if ( f < 0.0 )
                        sprintf_P( TxBuffer, PSTR("1%d%02d#"), -1 * i, abs((int)((f - (float)i) * 100.00)) );
                    else
                        sprintf_P( TxBuffer, PSTR("%02d%02d#"), i, abs((int)((f - (float)i) * 100.00)) );
                    DBGPRINT( F("// Current: "));
                    DBGPRINTLN( TxBuffer );
                    SendData = true;
                }
                break;
            case 'D':
                sprintf_P( TxBuffer, PSTR("%02d%02d#"), VERSION, shield );
                DBGPRINT( F("// Version : ") );
                DBGPRINTLN( TxBuffer );
                SendData = true;
                break;
        }
        goto Exit;
    }

    if ( RxBuffer[1] == '*' )
    {
        switch ( RxBuffer[0] )
        {
            case '0':
                id = RxBuffer[2] - '0';
                data = (RxBuffer[3] - '0') * 10 + (RxBuffer[4] - '0');
                if ( id <= 9 )
                {
                    DBGPRINT( F("// Put Data:") );
                    DBGPRINT( id );
                    DBGPRINT( F(",") );
                    DBGPRINTLN( data );
                    Data[id] = data;
                    for ( TxIndex = 0; TxIndex <= RxIndex; TxIndex++ )
                    {
                        TxBuffer[TxIndex] = RxBuffer[TxIndex];
                    }
                    SendData = true;
                }
                break;
            case '1':
                id = RxBuffer[2] - '0';
                if ( id <= 9 )
                {
                    DBGPRINT( F("// Get data:") );
                    DBGPRINT( id );
                    DBGPRINT( F(",") );
                    DBGPRINTLN( Data[id] );
                    TxBuffer[0] = (Data[id] / 10) + '0';
                    TxBuffer[1] = (Data[id] % 10) + '0';
                    TxBuffer[2] = '#';
                    SendData = true;
                }
                break;
            case '2':
                // Command Delay in seconds.  Must transmit two digits: 1 second = 01
                cmdDelay = (RxBuffer[2] - '0') * 10 + \
                           (RxBuffer[3] - '0');
                cmdDelay *= 1000;

                DBGPRINT(F("// Command Delay: "));
                DBGPRINTLN(cmdDelay);
                for (TxIndex = 0; TxIndex <= RxIndex; TxIndex++)
                {
                    TxBuffer[TxIndex] = RxBuffer[TxIndex];
                }
                SendData = true;
                break;
            case '3':
                // Disconnect Timeout in miliseconds.  Must transmit 4 digits: 1 ms = 0001
                disconnectTimeout = (RxBuffer[2] - '0') * 1000 + \
                                    (RxBuffer[3] - '0') * 100 + \
                                    (RxBuffer[4] - '0') * 10 + \
                                    (RxBuffer[5] - '0');

                DBGPRINT(F("// Disconnect Timeout: "));
                DBGPRINTLN(disconnectTimeout);
                for (TxIndex = 0; TxIndex <= RxIndex; TxIndex++)
                {
                    TxBuffer[TxIndex] = RxBuffer[TxIndex];
                }
                SendData = true;
                break;
        }
        goto Exit;
    }

Exit:
    RxIndex = 0;
    for ( i = 0; i < sizeof(RxBuffer); i++ )
        RxBuffer[i] = 0;
    return SendData;
}


void DTMF_SoftwareReset()
{
    DTMF_ReadStatusRegister();
    DTMF_WriteControlRegister( 0x00 );
    DTMF_WriteControlRegister( 0x00 );
    DTMF_WriteControlRegister( 0x08 );
    DTMF_WriteControlRegister( 0x00 );
    DTMF_ReadStatusRegister();
}

byte DTMF_ReadStatusRegister()
{
    byte status;

    noInterrupts();
    DDRA  = 0xD0;           // configure the port
    PORTA = 0xD0;           // {WRN, RSO, 0, RDN}
    PORTA = 0xC0;           // {WRN, RS0, 0, RDN#}
    PORTA = 0xC0;           // {WRN, RS0, 0, RDN#}
    PORTA = 0xC0;           // {WRN, RS0, 0, RDN#}
    status = PINA & 0x0F;   // Read the status
    PORTA = 0xD0;           // {WRN, RS0, 0, RDN}
    interrupts();
    return status;
}

void DTMF_WriteControlRegister( byte data )
{
    noInterrupts();
    data  = data & 0x0F;
    DDRA  = 0xDF;           // configure the port
    PORTA = 0xD0 | data;    // { WRN, RS0, 0, RDN}
    PORTA = 0x50 | data;    // {WRN#, RS0, 0, RDN}
    PORTA = 0x50 | data;    // {WRN#, RS0, 0, RDN}
    PORTA = 0x50 | data;    // {WRN#, RS0, 0, RDN}
    PORTA = 0xD0 | data;    // { WRN, RS0, 0, RDN}
    PORTA = 0xD0;
    DDRA  = 0xD0;
    interrupts();
}

void DTMF_TransmitTone( byte tone )
{
    byte data = DTMF_ToneToByte( tone );
    noInterrupts();
    data = data & 0x0F;
    DDRA  = 0xDF;           // configure the port
    PORTA = 0x90 | data;    // { WRN, RS0#, 0, RDN}
    PORTA = 0x10 | data;    // {WRN#, RS0#, 0, RDN}
    PORTA = 0x10 | data;    // {WRN#, RS0#, 0, RDN}
    PORTA = 0x10 | data;    // {WRN#, RS0#, 0, RDN}
    PORTA = 0x90 | data;    // { WRN, RS0#, 0, RDN}
    PORTA = 0xD0 | data;    // { WRN, RS0,  0, RDN}
    PORTA = 0xD0;
    DDRA  = 0xD0;
    TxReady = false;
    interrupts();
}

char DTMF_ReadData()
{
    byte data;
    byte value[16] = { 'D', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '*', '#', 'A', 'B', 'C' };

    noInterrupts();
    DDRA  = 0xD0;           // configure the port
    PORTA = 0x90;           // {WRN, RS0#, 0, RDN}
    PORTA = 0x80;           // {WRN, RS0#, 0, RDN#}
    PORTA = 0x80;           // {WRN, RS0#, 0, RDN#}
    PORTA = 0x80;           // {WRN, RS0#, 0, RDN#}
    data  = PINA & 0x0F;    // Read the data
    PORTA = 0x80;           // {WRN, RS0#, 0, RDN}
    PORTA = 0xD0;           // {WRN, RS0, 0, RDN}
    DDRA  = 0xD0;
    interrupts();

    return value[data];
}

void DTMF_GenerateTone(SerialCommand* cmd)
{
    char *arg = cmd->next();
    byte index = 0;

    DBGPRINTLN( F("Generate Tone" ) );

    if ( arg == NULL ) {
        DBGPRINTLN( F("Tone not specified") );
    } else {
        while ( arg[index] ) {
            TxBuffer[TxIndex] = arg[index];
            TxIndex++;
            if ( arg[index] == '#' ) {
                DBGPRINTLN( F("EOT character found") );
                TxIndex = 0;
                TxSendBuffer = true;
                return;
            }
            index++;
        }
    }
}

byte DTMF_ToneToByte( char tone )
{
    byte retVal = 0xFF;

    switch (tone)
    {
        case '0': retVal = 0x0A; break;
        case '1': retVal = 0x01; break;
        case '2': retVal = 0x02; break;
        case '3': retVal = 0x03; break;
        case '4': retVal = 0x04; break;
        case '5': retVal = 0x05; break;
        case '6': retVal = 0x06; break;
        case '7': retVal = 0x07; break;
        case '8': retVal = 0x08; break;
        case '9': retVal = 0x09; break;
        case '*': retVal = 0x0B; break;
        case '#': retVal = 0x0C; break;
        case 'a': retVal = 0x0D; break;
        case 'b': retVal = 0x0E; break;
        case 'c': retVal = 0x0F; break;
        case 'd': retVal = 0x00; break;
    }
    return retVal;
}

