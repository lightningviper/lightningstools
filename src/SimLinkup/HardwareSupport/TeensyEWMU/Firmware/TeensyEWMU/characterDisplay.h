#pragma once
#ifndef CHARACTER_DISPLAY_H
#define CHARACTER_DISPLAY_H

#include "arduino.h"
#include "basicConfig.h"
#include "pinAssignments.h"
#include "brightness.h"
#include "timing.h"
#include "lights.h"
#include "font.h"

/* ------  CHARACTER DISPLAY CONFIGURATION ------ */
const uint8_t CHARACTER_DISPLAY_NUM_COLS = 5;
const uint8_t CHARACTER_DISPLAY_NUM_ROWS = 7;
const uint8_t CMDS_NUM_CHARACTERS_TO_DISPLAY = 16;
const uint8_t EWMU_NUM_CHARACTERS_TO_DISPLAY = 32;
const uint8_t EWPI_NUM_CHARACTERS_TO_DISPLAY = 16;
const uint8_t TOTAL_CHARACTERS_TO_DISPLAY = (IS_CMDS ? CMDS_NUM_CHARACTERS_TO_DISPLAY : 0) + (IS_EWMU ? EWMU_NUM_CHARACTERS_TO_DISPLAY : 0) + (IS_EWPI ? EWPI_NUM_CHARACTERS_TO_DISPLAY : 0);

const uint32_t BLANKING_DELAY_MICROSECONDS = 1;
const uint32_t UNBLANKING_DELAY_MICROSECONDS = 4;
const uint32_t CHARACTER_DISPLAY_CLOCK_HIGH_HOLD_TIME_TIME_NANOSECONDS = 200;
const uint32_t CHARACTER_DISPLAY_CLOCK_LOW_HOLD_TIME_TIME_NANOSECONDS = 200;
const uint32_t CHARACTER_DISPLAY_CLOCK_HIGH_TO_LOW_TRANSITION_TIME_NANOSECONDS = 200;
const uint32_t CHARACTER_DISPLAY_CLOCK_LOW_TO_HIGH_TRANSITION_TIME_NANOSECONDS = 200;
const uint32_t MICROSECONDS_PER_SECOND = 1000 * 1000;
const uint32_t REFRESH_RATE_HZ = 100 / (((IS_CMDS || IS_EWMU ? 1: 0) + (IS_EWPI ? 1 : 0)) > 1 ? 2 : 1);
const uint32_t MICROSECONDS_PER_REFRESH_CYCLE = (MICROSECONDS_PER_SECOND / REFRESH_RATE_HZ);
const uint32_t TOTAL_COLUMNS_PER_REFRESH_CYCLE = ((IS_EWMU || IS_CMDS ? CHARACTER_DISPLAY_NUM_COLS : 0) + (IS_EWPI ? CHARACTER_DISPLAY_NUM_COLS : 0));
const uint32_t MAX_COLUMN_STROBE_TIME_MICROSECONDS = MICROSECONDS_PER_REFRESH_CYCLE / TOTAL_COLUMNS_PER_REFRESH_CYCLE;

/* --------------------------------------------------- */

void setupCharacterDisplays();
void updateCharacterDisplays();

void displayString(const char *chars, uint8_t numCharacters, uint8_t clockPin, uint8_t dataPin, uint8_t blankingPin, uint16_t brightness);
void displayCMDSString();
void displayEWMUString();
void displayEWPIString();
void displayCombinedEWMUAndEWPIString();

void strobeCharacterDisplayColumn(uint8_t columnPin, float displayIntensityPct);
void turnCharacterDisplayColumnOff(uint8_t columnPin);
void turnCharacterDisplayColumnOn(uint8_t columnPin);
void strobeCharacterDisplayColumn(uint8_t columnPin, float displayIntensityPct);
void unblankDisplay(uint8_t blankingPin);
void blankDisplay(uint8_t blankingPin);


#endif
