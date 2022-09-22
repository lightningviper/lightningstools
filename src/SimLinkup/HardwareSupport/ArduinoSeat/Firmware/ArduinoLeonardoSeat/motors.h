#pragma once
#ifndef MOTORS_H
#define MOTORS_H

#include "arduino.h"
//#include "libraries/Adafruit-Motor-Shield/AFMotor.h"
#include <AFMotor.h>
#include "hostMessageProcessing.h"
#include "pinAssignments.h"

void setupMotorOutputs();

void updateMotorOutputs();

void setMotorSpeeds();
void setMotorStates();

void setupBuiltinLED();
void turnBuiltInLEDOff();
void turnBuiltInLEDOn();
void toggleBuiltInLED();
void updateBuiltInLED();

#endif
