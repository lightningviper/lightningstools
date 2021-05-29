#include "characterDisplay.h"

/* ------------  CHARACTER DISPLAYS COMMON INITIALIZATION -------------------------- */
extern uint16_t _EWMUAndCMDSBrightness;
extern uint16_t _EWPIBrightness;
extern bool _CMDS_O1SwitchState;
extern bool _CMDS_O2SwitchState;
extern bool _CMDS_CHSwitchState;
extern bool _CMDS_FLSwitchState;

bool _displayIsBlanked = false;
char _CMDSChars[CMDS_NUM_CHARACTERS_TO_DISPLAY] = {'\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3'};
char _EWMUChars[EWMU_NUM_CHARACTERS_TO_DISPLAY] = {'\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3'};
char _EWPIChars[EWPI_NUM_CHARACTERS_TO_DISPLAY] = {'\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3','\3'};
uint8_t _CMDSConditionalBlankingBits;


void setupCharacterDisplays()
{
  pinMode(CHARACTER_DISPLAYS_CLOCK_PIN, OUTPUT); digitalWriteFast(CHARACTER_DISPLAYS_CLOCK_PIN, HIGH);
  pinMode(EWMU_AND_CMDS_CHARACTER_DISPLAYS_BLANKING_PIN, OUTPUT); blankDisplay(EWMU_AND_CMDS_CHARACTER_DISPLAYS_BLANKING_PIN);
  pinMode(EWPI_CHARACTER_DISPLAYS_BLANKING_PIN, OUTPUT); blankDisplay(EWPI_CHARACTER_DISPLAYS_BLANKING_PIN);
  pinMode(EWMU_AND_CMDS_CHARACTER_DISPLAYS_DATA_PIN, OUTPUT); digitalWriteFast(EWMU_AND_CMDS_CHARACTER_DISPLAYS_DATA_PIN, LOW);
  pinMode(EWPI_CHARACTER_DISPLAYS_DATA_PIN, OUTPUT); digitalWriteFast(EWPI_CHARACTER_DISPLAYS_DATA_PIN, LOW);
  for (int i = 0; i < CHARACTER_DISPLAY_NUM_COLS; i++) {
    pinMode(CHARACTER_DISPLAYS_COLUMN_DRIVER_PINS[i], OUTPUT);
    turnCharacterDisplayColumnOff(CHARACTER_DISPLAYS_COLUMN_DRIVER_PINS[i]);
  }
  if (!DISPLAY_TEST_PATTERN_AT_STARTUP) {
    memset(_CMDSChars, 0, sizeof(_CMDSChars));
    memset(_EWMUChars, 0, sizeof(_EWMUChars));
    memset(_EWPIChars, 0, sizeof(_EWPIChars));
  }
}
/* ------------------------------------------------------------ */

/* -------------- CHARACTER DISPLAY DATA LOADING ---------------------------------------- */
void updateCharacterDisplays()
{
  if (IS_CMDS) displayCMDSString();
  if (!IS_EWMU_AND_EWPI_DAISY_CHAINED)
  {
    if (IS_EWMU) displayEWMUString();
    if (IS_EWPI) displayEWPIString();
  }
  else 
  {
    displayCombinedEWMUAndEWPIString();
  }
}
void displayCMDSString() {
  displayString(_CMDSChars, CMDS_NUM_CHARACTERS_TO_DISPLAY, CHARACTER_DISPLAYS_CLOCK_PIN, EWMU_AND_CMDS_CHARACTER_DISPLAYS_DATA_PIN, EWMU_AND_CMDS_CHARACTER_DISPLAYS_BLANKING_PIN, _EWMUAndCMDSBrightness);
}
void displayEWMUString() {
  displayString(_EWMUChars, EWMU_NUM_CHARACTERS_TO_DISPLAY, CHARACTER_DISPLAYS_CLOCK_PIN, EWMU_AND_CMDS_CHARACTER_DISPLAYS_DATA_PIN, EWMU_AND_CMDS_CHARACTER_DISPLAYS_BLANKING_PIN, _EWMUAndCMDSBrightness);
}
void displayEWPIString() {
  displayString(_EWPIChars, EWPI_NUM_CHARACTERS_TO_DISPLAY, CHARACTER_DISPLAYS_CLOCK_PIN, EWPI_CHARACTER_DISPLAYS_DATA_PIN, EWPI_CHARACTER_DISPLAYS_BLANKING_PIN, _EWPIBrightness);
}
void displayCombinedEWMUAndEWPIString() {
  char combinedEWMUAndEWPIChars[EWMU_NUM_CHARACTERS_TO_DISPLAY + EWPI_NUM_CHARACTERS_TO_DISPLAY];
  memcpy (combinedEWMUAndEWPIChars, _EWMUChars, EWMU_NUM_CHARACTERS_TO_DISPLAY);
  memcpy (combinedEWMUAndEWPIChars + EWMU_NUM_CHARACTERS_TO_DISPLAY, _EWPIChars, EWPI_NUM_CHARACTERS_TO_DISPLAY);
  displayString(combinedEWMUAndEWPIChars, EWMU_NUM_CHARACTERS_TO_DISPLAY + EWPI_NUM_CHARACTERS_TO_DISPLAY, CHARACTER_DISPLAYS_CLOCK_PIN, EWMU_AND_CMDS_CHARACTER_DISPLAYS_DATA_PIN, EWMU_AND_CMDS_CHARACTER_DISPLAYS_BLANKING_PIN, _EWMUAndCMDSBrightness);
}

void displayString(const char *chars, uint8_t numCharacters, uint8_t clockPin, uint8_t dataPin, uint8_t blankingPin, uint16_t brightness)
{
  blankDisplay(blankingPin);
  for (uint8_t column = 0; column < CHARACTER_DISPLAY_NUM_COLS; column++)
  {
    for (uint8_t thisChar = numCharacters; thisChar--; )
    {
      for (uint8_t row = 0; row < CHARACTER_DISPLAY_NUM_ROWS; row++)
      {
        digitalWriteFast(clockPin, HIGH); //raise CLOCK pin
        conditionalDelayNanoseconds(DIGITAL_OUTPUT_PIN_RISE_TIME_NANOSECONDS); //wait for CLOCK pin to rise

        uint8_t thisDisplayChip = ((uint8_t)((thisChar) / 4));
        bool blankAllCharactersOnThisDisplayChip = false;
        if (IS_CMDS && _CMDSConditionalBlankingBits & CMDSConditionalDisplayBlankingBits::ENABLE_CONDITIONAL_BLANKING) 
        {
          switch (thisDisplayChip)
          {
            case 0: blankAllCharactersOnThisDisplayChip = !_CMDS_O1SwitchState; break;
            case 1: blankAllCharactersOnThisDisplayChip = !_CMDS_O2SwitchState; break;
            case 2: blankAllCharactersOnThisDisplayChip = !_CMDS_CHSwitchState; break;
            case 3: blankAllCharactersOnThisDisplayChip = !_CMDS_FLSwitchState; break;
          }
        }

        bool dataVal = blankAllCharactersOnThisDisplayChip ? LOW : bitRead((font[(byte)chars[thisChar]][column]), ((CHARACTER_DISPLAY_NUM_ROWS - 1) - row)) ? HIGH : LOW;
        
        digitalWriteFast(dataPin, dataVal); //set DATA pin value
        conditionalDelayNanoseconds(CHARACTER_DISPLAY_CLOCK_HIGH_HOLD_TIME_TIME_NANOSECONDS); //wait for CLOCK pin HIGH pulse width time

        digitalWriteFast(clockPin, LOW); //lower CLOCK pin
        conditionalDelayNanoseconds(CHARACTER_DISPLAY_CLOCK_HIGH_TO_LOW_TRANSITION_TIME_NANOSECONDS); //wait for CLOCK pin to fall
        conditionalDelayNanoseconds(CHARACTER_DISPLAY_CLOCK_LOW_HOLD_TIME_TIME_NANOSECONDS); //wait for CLOCK pin LOW pulse width time
      }
    }
    float displayIntensityPctToSet = displayIntensityPct(brightness);
    if (displayIntensityPctToSet > 0)
    {
      unblankDisplay(blankingPin);
      strobeCharacterDisplayColumn(CHARACTER_DISPLAYS_COLUMN_DRIVER_PINS[column], displayIntensityPctToSet);
    }
  }
  blankDisplay(blankingPin);
}
/* ----------------------------------------------------------------------------- */


/* -------------- COLUMN STROBING ---------------------------------------- */
void turnCharacterDisplayColumnOff(uint8_t columnPin) {
  digitalWriteFast(columnPin, LOW);
  conditionalDelayMicroseconds(DIGITAL_OUTPUT_PIN_FALL_TIME_NANOSECONDS);
}

void turnCharacterDisplayColumnOn(uint8_t columnPin) {
  digitalWriteFast(columnPin, HIGH);
  conditionalDelayMicroseconds(DIGITAL_OUTPUT_PIN_RISE_TIME_NANOSECONDS);
}

void strobeCharacterDisplayColumn(uint8_t columnPin, float displayIntensityPct)
{
  float columnOnTimeMicroseconds=MAX_COLUMN_STROBE_TIME_MICROSECONDS * displayIntensityPct;
  float columnOffTimeMicroseconds=MAX_COLUMN_STROBE_TIME_MICROSECONDS-columnOnTimeMicroseconds;

  turnCharacterDisplayColumnOn(columnPin);
  conditionalDelayMicroseconds(columnOnTimeMicroseconds);
  turnCharacterDisplayColumnOff(columnPin);
  conditionalDelayMicroseconds(columnOffTimeMicroseconds);
}
/* ----------------------------------------------------------------------------- */

/* -------------- DISPLAY BLANKING/UNBLANKING ---------------------------------------- */
void unblankDisplay(uint8_t blankingPin) {
  if (_displayIsBlanked)
  {
    digitalWriteFast(blankingPin, HIGH);
    conditionalDelayMicroseconds(UNBLANKING_DELAY_MICROSECONDS); 
    _displayIsBlanked = false;
  }
}

void blankDisplay(uint8_t blankingPin) {
  if (!_displayIsBlanked)
  {
    digitalWriteFast(blankingPin, LOW);
    conditionalDelayMicroseconds(BLANKING_DELAY_MICROSECONDS);
    _displayIsBlanked = true;
  }
}
/* ----------------------------------------------------------------------------- */
