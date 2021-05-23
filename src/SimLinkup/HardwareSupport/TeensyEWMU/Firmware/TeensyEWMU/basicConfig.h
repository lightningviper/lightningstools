#pragma once
#ifndef BASIC_CONFIG_H
#define BASIC_CONFIG_H

#include "arduino.h"

/* -------------- BASIC CONFIGURATION --------------- */
#define IS_CMDS true //set to TRUE if you are using this firmware with a CMDS panel
#define IS_EWMU false //set to TRUE if you are using this firmware with an EWMU panel
#define IS_EWPI false //set to TRUE if you are using this firmware with an EWPI panel
#define IS_EWMU_AND_EWPI_DAISY_CHAINED false //set to TRUE if you are daisy-chaining the EWMU outputs to the EWPI inputs together

#define DISPLAY_TEST_PATTERN_AT_STARTUP false //set to TRUE to turn all pixels on on all displays and light all lamps/LEDs at startup until a command is received from the PC host
#define JOYSTICK_INTERFACE //comment out to not process joystick button returns
/* -------------------------------------------------- */

#endif
