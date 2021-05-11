#pragma once
#ifndef LIGHTS_H
#define LIGHTS_H

#include "arduino.h"
#include "brightness.h"
#include "hostMessageProcessing.h"
#include "pinAssignments.h"

const uint8_t ANALOG_WRITE_RESOLUTION_BITS = 8;
const uint16_t PWM_LIGHTING_FREQUENCY_HZ = 1*1000*1000;

void setupLightOutputs();
void setupBuiltinLED();
void setupLightOutputsForCMDS();
void setupLightOutputsForEWPI();

void updateLightOutputs();
void updateEWPILightOutputs();
void updateCMDSLightOutputs();

void ledOff(uint8_t outputPin);
void ledOn(uint8_t outputPin, uint16_t brightness);

void lampOff(uint8_t outputPin);
void lampOn(uint8_t outputPin, uint16_t brightness);


void turnBuiltInLEDOff();
void turnBuiltInLEDOn();
void toggleBuiltInLED();
void updateBuiltInLED();

#endif
