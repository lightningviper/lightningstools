#pragma once
#ifndef JOYSTICK_H
#define JOYSTICK_H

#include "arduino.h"
#include <EasyButton.h>
#include <EasyButtonBase.h>
#include <EasyButtonTouch.h>
#include <EasyButtonVirtual.h>
#include <Sequence.h>
#include "pinAssignments.h"
#include "joystickAssignments.h"

/* -------------- JOYSTICK CONFIGURATION --------------- */
const bool SEND_DX_JOYSTICK_REPORTS = true;
const uint8_t NUM_JOYSTICK_AXES = 8;
const uint32_t DX_JOYSTICK_REPORTING_FREQUENCY_MILLIS = 100;
const float JOYSTICK_AXIS_MAX_VAL = 1023.0f;
/* -------------------------------------------------- */

void setupButtonInputs();
void setupEWMUButtonMatrix();
void beginEWMUAndCMDSCommonInputDebouncing();
void beginCMDSpecificInputDebouncing();
void beginEWMUSpecificInputDebouncing();
void beginEWPIInputDebouncing();
void updateJoystickOutputs();
void SetJoystickAxis(uint8_t axisNum, uint16_t value);
void updateCMDSJoystickOutputs();
void updateEWMUJoystickOutputs();
void updateEWPIJoystickOutputs();

#endif
