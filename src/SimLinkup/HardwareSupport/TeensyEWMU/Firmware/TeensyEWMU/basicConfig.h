#pragma once
#ifndef BASIC_CONFIG_H
#define BASIC_CONFIG_H

#include "arduino.h"

/* -------------- BASIC CONFIGURATION --------------- */
const bool IS_CMDS =false; //set to TRUE if you are using this firmware with a CMDS panel
const bool IS_EWMU =true; //set to TRUE if you are using this firmware with an EWMU panel
const bool IS_EWPI =true; //set to TRUE if you are using this firmware with an EWPI panel
const bool IS_EWMU_AND_EWPI_DAISY_CHAINED = true;
const bool DISPLAY_TEST_PATTERN_AT_STARTUP=true;
#define JOYSTICK_INTERFACE //comment out to not process joystick button returns
/* -------------------------------------------------- */

#endif
