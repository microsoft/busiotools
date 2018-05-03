/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

#include <SerialCommand.h>
#include <Servo.h>
#include <ServoEaser.h>
#include "ServoShield.h"

#define DBGPRINT(A)    if (dbg) {Serial.print(A);}
#define DBGPRINTLN(A)  if (dbg) {Serial.println(A);}

// Pins
#define ServoPin1 11
#define ServoPin2 12

ServoEaser Servo1Easer;
ServoEaser Servo2Easer;

int servoFrameMillis = 20;
int msPerDegree = 5;
int easingMultipler = 3;
int defaultEaseTime = msPerDegree * easingMultipler;

struct Servos
{
    Servo servo;
    ServoEaser servoEaser;
    int angle;
    int pin;
};
Servos ServoArray[2];

void SERVO_SetAngleCB(SerialCommand* cmd);
void SERVO_Usage();
void AddSerCommand(const char* cmd, void(*function)(SerialCommand*));
bool SERVO_Overshoot(int angle, int servo);
void SERVO_Loop();
extern bool dbg;

void SERVO_setup()
{
    DBGPRINTLN(F("// Servo Shield Initialized "));
    AddSerCommand("setangle", SERVO_SetAngleCB);
    ServoArray[0].pin = ServoPin1;
    ServoArray[1].pin = ServoPin2;
    ServoArray[0].servo.attach(ServoArray[0].pin, 800, 2200);
    ServoArray[1].servo.attach(ServoArray[1].pin, 800, 2200);
    ServoArray[0].angle = 90;
    ServoArray[1].angle = 90;
    ServoArray[0].servoEaser.begin( ServoArray[0].servo, servoFrameMillis );
    ServoArray[1].servoEaser.begin( ServoArray[1].servo, servoFrameMillis );
}

void SERVO_SetAngleCB(SerialCommand* cmd)
{
    char buffer[2];
    char *arg = cmd->next();
    int servoIndex = 0;
    int angle = 0;

    DBGPRINTLN(F("// Set Servo Angle "));

    if (arg == NULL)
    {
        return;
    }

    servoIndex = atoi(arg);
    arg = cmd->next();

    if (arg == NULL)
    {
        return;
    }

    angle = atoi(arg);
    
    if ((servoIndex > 0) && (servoIndex <= (sizeof(ServoArray) / sizeof(ServoArray[0]))))
    {
        servoIndex = servoIndex - 1;
    }
    else
    {
        DBGPRINT(F("// Servo \""));
        DBGPRINT(servoIndex);
        DBGPRINTLN(F("\" not valid"));
        sprintf_P(buffer, PSTR("%u"), 0);
        cmd->GetHardwareSerial()->println(buffer);
        return false;
    }

    if (angle < 0 || angle > 180)
    {
        DBGPRINT(F("// Angle \""));
        DBGPRINT(angle);
        DBGPRINTLN(F("\" not valid"));
        sprintf_P(buffer, PSTR("%u"), 0);
        cmd->GetHardwareSerial()->println(buffer);
        return false;
    }

    
    int angleChange = (angle < ServoArray[servoIndex].angle) ? (ServoArray[servoIndex].angle - angle) : (angle - ServoArray[servoIndex].angle);
    int moveTime = 125 + (angleChange * defaultEaseTime);
    if(moveTime < 200)
    {
        moveTime = 200;
    }
    else
    {
        ServoArray[servoIndex].servoEaser.easeTo(angle, moveTime);
        
        DBGPRINT(F("// Moving "));
        DBGPRINT(angle);
        DBGPRINT(F(" degrees over "));
        DBGPRINT(moveTime);
        DBGPRINTLN(F(" ms "));
    }
    ServoArray[servoIndex].angle = angle;
    
    sprintf_P(buffer, PSTR("%u"), servoIndex+1);
    cmd->GetHardwareSerial()->println(buffer);
}

void SERVO_Usage()
{
    DBGPRINTLN(F("setAngle [servo] [angle]"));
    DBGPRINTLN(F("    sets specified servo to specified angle"));
}

void SERVO_Loop()
{
    for(int i = 0; i < 2; i++)
    {
        ServoArray[i].servoEaser.update();
    }
}
