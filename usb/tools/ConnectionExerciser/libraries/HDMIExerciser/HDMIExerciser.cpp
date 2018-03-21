/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

#include <SerialCommand.h>
#include "HDMIExerciser.h"

#define DBGPRINT(A)    if (dbg) {Serial.print(A);}
#define DBGPRINTLN(A)  if (dbg) {Serial.println(A);}

// Pins
#define HdmiEnable 52
#define HdmiSelect 53
#define HdmiHpd 40

void HDMI_SetPortCB(SerialCommand* cmd);
void HDMI_GetPortCB(SerialCommand* cmd);
void HDMI_Usage();
void AddSerCommand(const char* cmd, void(*function)(SerialCommand*));
extern bool dbg;

unsigned int gPort;

void HDMI_setup()
{
	DBGPRINT(F("// HDMI Exerciser Initialized "));
	AddSerCommand("sethdmiport", HDMI_SetPortCB);
	AddSerCommand("gethdmiport", HDMI_GetPortCB);

	pinMode(HdmiEnable, OUTPUT);
	pinMode(HdmiSelect, OUTPUT);
    pinMode(HdmiHpd, INPUT);

    // Initialize with Port 2 (J3)
	digitalWrite(HdmiEnable, HIGH);
	digitalWrite(HdmiSelect, HIGH);
    gPort = 2;
}

void HDMI_SetPortCB(SerialCommand* cmd)
{
	DBGPRINTLN(F("// SetHdmiPort "));
	char *arg = cmd->next();

	if (arg == NULL)
	{
		return;
	}

	gPort = atoi(arg);

	switch (gPort)
	{
    case 0:
        digitalWrite(HdmiEnable, LOW);
        DBGPRINTLN(F("// Hdmi Disconnected "));
        break;
	case 1:
        digitalWrite(HdmiEnable, HIGH);
		digitalWrite(HdmiSelect, LOW);
		DBGPRINTLN(F("// J2 Connected "));
		break;
	case 2:
        digitalWrite(HdmiEnable, HIGH);
		digitalWrite(HdmiSelect, HIGH);
		DBGPRINTLN(F("// J3 Connected "));
		break;
	default:
		return;
	}

    char port = '0' + gPort;
	cmd->GetHardwareSerial()->println(port);
}

void HDMI_GetPortCB(SerialCommand* cmd)
{
	DBGPRINTLN(F("// GetHdmiPort "));
	//DBGPRINTLN(F('0' + gPort));
    char port = '0' + gPort;

	cmd->GetHardwareSerial()->println(port);
}

void HDMI_Usage()
{
    DBGPRINTLN(F("sethdmiport [options]"));
    DBGPRINTLN(F("    1, connects to HDMI port J2"));
    DBGPRINTLN(F("    2, connects to HDMI port J3"));
    DBGPRINTLN(F("    0, disconnect all ports"));
    DBGPRINTLN(F("gethdmiport"));
    DBGPRINTLN(F("    returns the currently connected HDMI port"));
}
