#include "lights.h"

bool _builtInLEDOn = false;
uint8_t _CMDSLightsState = 0;
uint8_t _EWPILightsState = 0;
extern uint16_t _EWMUAndCMDSBrightness;
extern uint16_t _EWPIBrightness;

/* -------------- OUTPUT PIN INITIALIZATION ---------------------------------------- */
void setupLightOutputs() {
  analogWriteResolution(ANALOG_WRITE_RESOLUTION_BITS);
  setupBuiltinLED();
  if (IS_CMDS) setupLightOutputsForCMDS();
  if (IS_EWPI) setupLightOutputsForEWPI();
}

void setupBuiltinLED() {
  pinMode(TEENSY_BUILTIN_LED_PIN, OUTPUT); turnBuiltInLEDOn();
}
void setupLightOutputsForCMDS() {
  pinMode(CMDS_NOGO_ANNUNCIATOR_PIN, OUTPUT);
  analogWriteFrequency(CMDS_NOGO_ANNUNCIATOR_PIN, CMDS_PWM_LIGHTING_FREQUENCY_HZ);   
  pinMode(CMDS_GO_ANNUNCIATOR_PIN, OUTPUT); 
  analogWriteFrequency(CMDS_GO_ANNUNCIATOR_PIN, CMDS_PWM_LIGHTING_FREQUENCY_HZ);
  pinMode(CMDS_DISPENSE_RDY_ANNUNCIATOR_PIN, OUTPUT); 
  analogWriteFrequency(CMDS_DISPENSE_RDY_ANNUNCIATOR_PIN, CMDS_PWM_LIGHTING_FREQUENCY_HZ);
  _CMDSLightsState = DISPLAY_TEST_PATTERN_AT_STARTUP ? CMDSLightbits::ALL_CMDS_LIGHTS_ON : CMDSLightbits::ALL_CMDS_LIGHTS_OFF;
  updateLightOutputs();
}
void setupLightOutputsForEWPI() {
  pinMode(PRI_LED_PIN, OUTPUT); 
  analogWriteFrequency(PRI_LED_PIN, EWPI_PWM_LIGHTING_FREQUENCY_HZ);   
  pinMode(UNK_LED_PIN, OUTPUT); 
  analogWriteFrequency(UNK_LED_PIN, EWPI_PWM_LIGHTING_FREQUENCY_HZ);   
  pinMode(ML_LED_PIN, OUTPUT); 
  analogWriteFrequency(ML_LED_PIN, EWPI_PWM_LIGHTING_FREQUENCY_HZ);   
  _EWPILightsState = DISPLAY_TEST_PATTERN_AT_STARTUP ? EWPILightbits::ALL_EWPI_LIGHTS_ON : EWPILightbits::ALL_EWPI_LIGHTS_OFF;
  updateLightOutputs();
}
/* ----------------------------------------------------------------------------- */

/* -------------- LAMP/LED FUNCTIONS  ---------------------------------------- */
void updateLightOutputs()
{
  if (IS_CMDS) updateCMDSLightOutputs();
  if (IS_EWPI) updateEWPILightOutputs();
}

void updateEWPILightOutputs()
{
  uint16_t brightness = _EWPIBrightness;
  _EWPILightsState & EWPILightbits::PRI ? ledOn(PRI_LED_PIN, brightness) : ledOff(PRI_LED_PIN);
  _EWPILightsState & EWPILightbits::UNK ? ledOn(UNK_LED_PIN, brightness) : ledOff(UNK_LED_PIN);
  _EWPILightsState & EWPILightbits::ML ? ledOn(ML_LED_PIN, brightness) : ledOff(ML_LED_PIN);
}

void updateCMDSLightOutputs()
{
  uint16_t brightness = _EWMUAndCMDSBrightness;
  _CMDSLightsState & CMDSLightbits::GO ? lampOn(CMDS_GO_ANNUNCIATOR_PIN, brightness) : lampOff(CMDS_GO_ANNUNCIATOR_PIN);
  _CMDSLightsState & CMDSLightbits::NOGO ? lampOn(CMDS_NOGO_ANNUNCIATOR_PIN, brightness) : lampOff(CMDS_NOGO_ANNUNCIATOR_PIN);
  _CMDSLightsState & CMDSLightbits::DISPENSE_READY ? lampOn(CMDS_DISPENSE_RDY_ANNUNCIATOR_PIN, brightness) : lampOff(CMDS_DISPENSE_RDY_ANNUNCIATOR_PIN);
}

void ledOff(uint8_t outputPin) {
  analogWrite (outputPin, 0);
}
void ledOn(uint8_t outputPin, uint16_t brightness)  {
  analogWrite(outputPin, displayIntensityPct(brightness)*MAX_INTENSITY);
}

void lampOff(uint8_t outputPin) {
  analogWrite(outputPin, 0);
}

void lampOn(uint8_t outputPin, uint16_t brightness) {
  analogWrite (outputPin, brightness);
}

/* ----------------------------------------------------------------------------- */

/* -------------- BUILT-IN LED HANDLING ---------------------------------------- */
void turnBuiltInLEDOff() {
  _builtInLEDOn = false;
  updateBuiltInLED();
}
void turnBuiltInLEDOn() {
  _builtInLEDOn = true;
  updateBuiltInLED();
}
void toggleBuiltInLED() {
  _builtInLEDOn = !_builtInLEDOn;
  updateBuiltInLED();
}
void updateBuiltInLED() {
  digitalWriteFast(TEENSY_BUILTIN_LED_PIN, _builtInLEDOn ? HIGH : LOW);
}
/* ----------------------------------------------------------------------------- */
