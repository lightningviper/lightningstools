#pragma once
#ifndef BRIGHTNESS_H
#define BRIGHTNESS_H

#include "arduino.h"
#include "basicConfig.h"
#include "pinAssignments.h"

const uint8_t ANALOG_READ_RESOLUTION_BITS = 8;
const uint16_t MAX_INTENSITY = (pow(2, ANALOG_READ_RESOLUTION_BITS)) - 1;

void setupBrightnessPotInputs();
void updateBrightnessPotInputs();
void readEWMUAndCMDSBrightness();
void readEWPIBrightness();
uint16_t smoothAnalogRead(uint8_t inputPin, uint16_t previousValue, uint16_t maxDelta);
float displayIntensityPct(uint16_t brightnessLevel);


#endif
