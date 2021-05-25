#include "brightness.h"

const uint8_t _LEDIntensityLookup [256] =  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 10, 10, 11, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 23, 23, 24, 24, 25, 26, 26, 27, 28, 28, 29, 30, 30, 31, 32, 32, 33, 34, 35, 35, 36, 37, 38, 38, 39, 40, 41, 42, 42, 43, 44, 45, 46, 47, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 84, 85, 86, 87, 88, 89, 91, 92, 93, 94, 95, 97, 98, 99, 100, 102, 103, 104, 105, 107, 108, 109, 111, 112, 113, 115, 116, 117, 119, 120, 121, 123, 124, 126, 127, 128, 130, 131, 133, 134, 136, 137, 139, 140, 142, 143, 145, 146, 148, 149, 151, 152, 154, 155, 157, 158, 160, 162, 163, 165, 166, 168, 170, 171, 173, 175, 176, 178, 180, 181, 183, 185, 186, 188, 190, 192, 193, 195, 197, 199, 200, 202, 204, 206, 207, 209, 211, 213, 215, 217, 218, 220, 222, 224, 226, 228, 230, 232, 233, 235, 237, 239, 241, 243, 245, 247, 249, 251, 253, 255};
uint16_t _EWMUAndCMDSBrightness = MAX_INTENSITY;
uint16_t _EWPIBrightness = MAX_INTENSITY;

/* -------------- BRIGHTNESS HANDLING ---------------------------------------- */
void setupBrightnessPotInputs() {
  analogReadResolution(ANALOG_READ_RESOLUTION_BITS);
  if (IS_EWMU) pinMode(EWMU_BRIGHTNESS_POT_PIN, INPUT);
  if (IS_EWPI) pinMode(EWPI_BRIGHTNESS_POT_PIN, INPUT);
  updateBrightnessPotInputs();
}
void updateBrightnessPotInputs() {
  if (IS_EWMU) readEWMUBrightness();
  if (IS_EWPI) readEWPIBrightness();
}
uint16_t smoothAnalogRead(uint8_t inputPin, uint16_t previousValue, uint16_t maxDelta)
{
  float newValue = analogRead(inputPin);
  float delta = newValue - previousValue;
  float toReturn = previousValue + (abs(delta) > abs(maxDelta) ? (maxDelta * (delta < 0 ? -1.0f : 1.0f)) : delta);
  if (toReturn < 0) toReturn = 0;
  return toReturn;
}
void readEWMUBrightness()
{
  _EWMUAndCMDSBrightness = smoothAnalogRead(EWMU_BRIGHTNESS_POT_PIN, _EWMUAndCMDSBrightness, (float)MAX_INTENSITY * 0.05f);
}

void readEWPIBrightness() 
{
  _EWPIBrightness = smoothAnalogRead(EWPI_BRIGHTNESS_POT_PIN, _EWPIBrightness, (float)MAX_INTENSITY * 0.05f);
}

float displayIntensityPct(uint16_t brightnessLevel)
{
  uint8_t intensity = _LEDIntensityLookup[brightnessLevel];
  return (float)intensity / (float)MAX_INTENSITY;
}


/* ----------------------------------------------------------------------------- */
