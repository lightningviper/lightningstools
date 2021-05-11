#pragma once
#ifndef PIN_ASSIGNMENTS_H
#define PIN_ASSIGNMENTS_H

#include "arduino.h"
#include "basicConfig.h"

/* -------------- PIN ASSIGNMENTS --------------- */
const uint8_t TEENSY_BUILTIN_LED_PIN = 13;

const uint8_t CHARACTER_DISPLAYS_COLUMN_DRIVER_PINS[] = { 1, 2, 3, 4, 5 };
const uint8_t CHARACTER_DISPLAYS_CLOCK_PIN = 10;
const uint8_t EWMU_AND_CMDS_CHARACTER_DISPLAYS_DATA_PIN = 12;
const uint8_t EWMU_AND_CMDS_CHARACTER_DISPLAYS_BLANKING_PIN =  8;

const uint8_t MWS_PIN = 6;
const uint8_t JMR_PIN = 7;
const uint8_t RWR_PIN = 9;
const uint8_t DISP_PIN = 11;

const uint8_t MODE_OFF_PIN = 28;
const uint8_t MODE_STBY_PIN = 29;
const uint8_t MODE_MAN_PIN = 30;
const uint8_t MODE_SEMI_PIN = 31;
const uint8_t MODE_AUTO_PIN = 32;
const uint8_t MODE_BYP_PIN = 33;

const uint8_t JETTISON_PIN = 0;

const uint8_t O1_PIN = 24;
const uint8_t O2_PIN = 25;
const uint8_t CH_PIN = 26;
const uint8_t FL_PIN = 27;

const uint8_t PRGM_BIT_PIN = 34;
const uint8_t PRGM_1_PIN = 36;
const uint8_t PRGM_2_PIN = 37;
const uint8_t PRGM_3_PIN = 38;
const uint8_t PRGM_4_PIN = 39;

const uint8_t CMDS_NOGO_ANNUNCIATOR_PIN = 23;
const uint8_t CMDS_GO_ANNUNCIATOR_PIN = 22;
const uint8_t CMDS_DISPENSE_RDY_ANNUNCIATOR_PIN = 35;

const uint8_t MWS_MENU_PIN = 24;
const uint8_t JMR_MENU_PIN = 25;
const uint8_t RWR_MENU_PIN = 26;
const uint8_t DISP_MENU_PIN = 27;

const uint8_t EWMU_PUSHBUTTON_MATRIX_COL1_PIN = 36;
const uint8_t EWMU_PUSHBUTTON_MATRIX_COL2_PIN = 37;
const uint8_t EWMU_PUSHBUTTON_MATRIX_COL3_PIN = 38;
const uint8_t EWMU_PUSHBUTTON_MATRIX_COL4_PIN = 39;
const uint8_t EWMU_PUSHBUTTON_MATRIX_ROW1_PIN = 33;
const uint8_t EWMU_PUSHBUTTON_MATRIX_ROW2_PIN = 34;

const uint8_t EWMU_AND_CMDS_BRIGHTNESS_POT_PIN = 14;

const uint8_t EWPI_CHARACTER_DISPLAYS_BLANKING_PIN = IS_EWMU_AND_EWPI_DAISY_CHAINED ? EWMU_AND_CMDS_CHARACTER_DISPLAYS_BLANKING_PIN : 17;
const uint8_t EWPI_CHARACTER_DISPLAYS_DATA_PIN = IS_EWMU_AND_EWPI_DAISY_CHAINED ? EWMU_AND_CMDS_CHARACTER_DISPLAYS_DATA_PIN : 15;
const uint8_t EWPI_BRIGHTNESS_POT_PIN = IS_EWMU_AND_EWPI_DAISY_CHAINED ? EWMU_AND_CMDS_BRIGHTNESS_POT_PIN : 16;

const uint8_t PRI_LED_PIN = 23;
const uint8_t UNK_LED_PIN = 22;
const uint8_t ML_LED_PIN = 35;

const uint8_t PRI_BUTTON_PIN = 19;
const uint8_t SEP_BUTTON_PIN = 21;
const uint8_t UNK_BUTTON_PIN = 20;
const uint8_t MD_BUTTON_PIN = 18;
/* ---------------------------------------------- */

#endif
