/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

#include <SoftwareSerial.h>
#include <SerialCommand.h>
#include <LiquidCrystal.h>
#include <Event.h>
#include <Timer.h>
#include <Wire.h>
#include <DualRoleConnectionExerciser.h>
#include <USBCExerciser.h>
#include <HMDExerciser.h>
#include <HDMIExerciser.h>
#include <DTMF.h>
#include <ServoShield.h>
#include <Model_3201.h>

#define DBGPRINT(A)    if (dbg) {Serial.print(A);}
#define DBGPRINTLN(A)  if (dbg) {Serial.println(A);}

byte VERSION = 1;
byte shield = 0;
bool hmdShield = false;

void AddSerCommand(const char* cmd, void (*function)(SerialCommand*));
void AddSerDefaultHandler(void (*function)(SerialCommand*));

SerialCommand sCmd(Serial);
SerialCommand s1Cmd(Serial1);
Timer timer;
LiquidCrystal *lcd;
bool dbg = false;
uint8_t Data[10];
uint32_t cmdDelay = 0; // Seconds
uint32_t disconnectTimeout = 0; // miliseconds
float (*ReadVoltage)() = NULL;
float (*ReadCurrent)() = NULL;
void (*SetPort)(byte) = NULL;
char (*GetPort)() = NULL;
void (*ShieldLoop)() = NULL;
void (*SuperSpeed)(bool) = NULL;
extern bool gHmdPresenceTrigger;

void setup() {
  Serial1.begin(115200);
  
  //
  // PORTL[7:4] and PORTB[6] are used to identify the shield.
  // Pull-ups are enable to detect the case where
  // no shield is attached.
  //

  // HMD Shield and DTMF share the same pins and shouldn't be used together
  // Check if HMD Shield is present and run the appropriate setup

  // Enable pullups on PORTB[6] and PORTL[7:4]
  PORTB = 0x40;
  PORTL = 0xF0;

  //
  // Look for stacked shields -- but not on the 3201
  //
  if ( (PINL >> 4) != MODEL_3201 )
  {     
      Serial.begin(9600);
      shield = (PINB >> 6) & 1;
      
      DBGPRINT( F("// Shield type: ") );
      DBGPRINTLN( shield );
      if(!shield)
      {
        HMD_setup();
        hmdShield = true;
      }
      else
      {
        DTMF_setup();
      }
  }
  else
  {
      //
      // The 3201 communicates at 115200 to handle programming the
      // FPGA image into flash.
      //
      Serial.begin(115200);
  }

  shield = PINL >> 4;

  DBGPRINT( F("// Shield type: ") );
  DBGPRINTLN( shield );
  switch (shield)
  {
    case DRCE_SHIELD:
      DRCE_setup();
      break;
    case USBC_SHIELD:
      USBC_setup();
      HDMI_setup();
      break;
    case MODEL_3201:
      DTMF_setup();
      USBC_setup();
      Model_3201_setup();
      break;
  }

  AddSerCommand("debug", SetDebugCB);
  AddSerCommand("version", GetVersionCB);
  AddSerCommand("put", PutDataCB);
  AddSerCommand("get", GetDataCB);
  AddSerDefaultHandler(Usage);
}

void AddSerCommand( const char* cmd, void (*function)(SerialCommand*))
{
  sCmd.addCommand(cmd, function);
  s1Cmd.addCommand(cmd, function);
}

void AddSerDefaultHandler(void (*function)(SerialCommand*))
{
  sCmd.addDefaultHandler(function);
  s1Cmd.addDefaultHandler(function);
}

void loop() {
  sCmd.readSerial();
  s1Cmd.readSerial();
  timer.update();
  if ( ShieldLoop )
  {
    ShieldLoop();
  }
  if(gHmdPresenceTrigger)
  {
    HMD_PulseLed();
    delay(20);
    gHmdPresenceTrigger = false;
  }
  if(hmdShield)
  {
    SERVO_Loop();
  }
}

void Usage(SerialCommand*)
{
  DBGPRINTLN(F("debug [options]"));
  DBGPRINTLN(F("    on, enable debug output"));
  DBGPRINTLN(F("    off (or other), disable debug output"));
  DBGPRINTLN(F("version"));
  DBGPRINTLN(F("    ABTT:<firmare version>:<shield type>"));
  if(USBC_SHIELD)
  {
    USBC_Usage();
    HDMI_Usage();
    HMD_Usage();
  }
  if(DRCE_SHIELD)
  {
    DRCE_Usage();
  }
}

void PutDataCB(SerialCommand* cmd)
{
  uint8_t index;
  char *arg = cmd->next();

  if ( arg )
  {
    index = atoi(arg);
    arg = cmd->next();
    if ( index <= 9 && arg )
    {
      Data[index] = atoi(arg);
      cmd->GetHardwareSerial()->print( F("put ") );
      cmd->GetHardwareSerial()->print( index );
      cmd->GetHardwareSerial()->print( F(" ") );
      cmd->GetHardwareSerial()->println( arg );
    }
  }
}

void GetDataCB(SerialCommand* cmd)
{
  char Buffer[3];
  uint8_t index;
  char *arg = cmd->next();

  if ( arg )
  {
    index = atoi(arg);
    if ( index <= 9 )
    {
      Buffer[0] = (Data[index] / 10) + '0';
      Buffer[1] = (Data[index] % 10) + '0';
      Buffer[2] = '\0';
      cmd->GetHardwareSerial()->println(Buffer);
    }
  }
}

void GetVersionCB(SerialCommand* cmd)
{
  char Buffer[8];

  sprintf_P( Buffer, PSTR("%02d%02d"), VERSION, shield );
  cmd->GetHardwareSerial()->println( Buffer );
}

void SetDebugCB(SerialCommand* cmd)
{
  char *arg = cmd->next();

  if (arg != NULL) {
    if ( strcmp(arg, "on" ) == 0) {
      dbg = true;
      DBGPRINTLN(F("Turn on debug"));
    }
    if ( strcmp(arg, "off" ) == 0) {
      DBGPRINTLN(F("Turn off debug"));
      dbg = false;
    }
  }
}
