#include "joystick.h"
extern uint16_t _EWMUAndCMDSBrightness;
extern uint16_t _EWPIBrightness;

/*  --------    INPUT DEBOUNCING ----------------------*/
EasyButton _O1(O1_PIN);
EasyButton _O2(O2_PIN);
EasyButton _CH(CH_PIN);
EasyButton _FL(FL_PIN);

EasyButton _JETTISON(JETTISON_PIN);

EasyButton _PRGM_BIT(PRGM_BIT_PIN);
EasyButton _PRGM_1(PRGM_1_PIN);
EasyButton _PRGM_2(PRGM_2_PIN);
EasyButton _PRGM_3(PRGM_3_PIN);
EasyButton _PRGM_4(PRGM_4_PIN);

EasyButton _MWS(MWS_PIN);
EasyButton _JMR(JMR_PIN);
EasyButton _RWR(RWR_PIN);
EasyButton _DISP(DISP_PIN);

EasyButton _MODE_OFF(MODE_OFF_PIN);
EasyButton _MODE_STBY(MODE_STBY_PIN);
EasyButton _MODE_MAN(MODE_MAN_PIN);
EasyButton _MODE_SEMI(MODE_SEMI_PIN);
EasyButton _MODE_AUTO(MODE_AUTO_PIN);
EasyButton _MODE_BYP(MODE_BYP_PIN);

EasyButton _MWS_MENU(MWS_MENU_PIN);
EasyButton _JMR_MENU(JMR_MENU_PIN);
EasyButton _RWR_MENU(RWR_MENU_PIN);
EasyButton _DISP_MENU(DISP_MENU_PIN);

EasyButton _PRI(PRI_BUTTON_PIN);
EasyButton _SEP(SEP_BUTTON_PIN);
EasyButton _UNK(UNK_BUTTON_PIN);
EasyButton _EWPI_MD(MD_BUTTON_PIN);
/* ----------------------------------------------------*/

uint32_t _invertBits = 0;
elapsedMillis _timeSinceLastJoystickUpdate = 0;
uint8_t DX_Button_Assignments[NUM_LOGICAL_SWITCHES_AND_BUTTONS];

/* -------------- GAME DEVICE HANDLING ---------------------------------------- */
void setupButtonInputs() {
#ifdef JOYSTICK_INTERFACE

  if (IS_CMDS || IS_EWMU) beginEWMUAndCMDSCommonInputDebouncing();
  if (IS_CMDS) beginCMDSpecificInputDebouncing();
  if (IS_EWMU) 
  {
    setupEWMUButtonMatrix();
    beginEWMUSpecificInputDebouncing();
  }
  if (IS_EWPI) beginEWPIInputDebouncing();
  Joystick.useManualSend(true);

#endif
}

void setupEWMUButtonMatrix() {
#ifdef JOYSTICK_INTERFACE
  if (IS_EWMU)
  {
    pinMode(EWMU_PUSHBUTTON_MATRIX_ROW1_PIN, INPUT_PULLUP);
    pinMode(EWMU_PUSHBUTTON_MATRIX_ROW2_PIN, INPUT_PULLUP);
    pinMode(EWMU_PUSHBUTTON_MATRIX_COL1_PIN, INPUT_PULLUP);
    pinMode(EWMU_PUSHBUTTON_MATRIX_COL2_PIN, INPUT_PULLUP);
    pinMode(EWMU_PUSHBUTTON_MATRIX_COL3_PIN, INPUT_PULLUP);
    pinMode(EWMU_PUSHBUTTON_MATRIX_COL4_PIN, INPUT_PULLUP);
  }
#endif
}

void updateJoystickOutputs()
{
#ifdef JOYSTICK_INTERFACE
  if (!SEND_DX_JOYSTICK_REPORTS || _timeSinceLastJoystickUpdate < DX_JOYSTICK_REPORTING_FREQUENCY_MILLIS) return;
  _timeSinceLastJoystickUpdate = 0;

  for (uint8_t i = 0; i < NUM_JOYSTICK_AXES; i++) {
    SetJoystickAxis(i, 0);
  }

  if (IS_CMDS) updateCMDSJoystickOutputs();
  if (IS_EWMU) updateEWMUJoystickOutputs();
  if (IS_EWPI) updateEWPIJoystickOutputs();

  Joystick.send_now();
#endif
}
void SetJoystickAxis(uint8_t axisNum, uint16_t value)
{
#ifdef JOYSTICK_INTERFACE
  switch (axisNum)
  {
    case 0: Joystick.X (value); break;
    case 1: Joystick.Y (value); break;
    case 2: Joystick.Z (value); break;
    case 3: Joystick.Zrotate (value); break;
    case 4: Joystick.sliderLeft (value); break;
    case 5: Joystick.sliderRight (value); break;
    default: break;
  }
#endif
}

void updateCMDSJoystickOutputs() {
#ifdef JOYSTICK_INTERFACE
  Joystick.button(JETTISON_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_JETTISON ? !_JETTISON.read() : _JETTISON.read());
  Joystick.button(JETTISON_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_JETTISON ? _JETTISON.read() : !_JETTISON.read());

  Joystick.button(MWS_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MWS ? !_MWS.read() : _MWS.read());
  Joystick.button(MWS_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MWS ? _MWS.read() : !_MWS.read());
  Joystick.button(JMR_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_JMR ? !_JMR.read() : _JMR.read());
  Joystick.button(JMR_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_JMR ? _JMR.read() : !_JMR.read());
  Joystick.button(RWR_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_RWR ? !_RWR.read() : _RWR.read());
  Joystick.button(RWR_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_RWR ? _RWR.read() : !_RWR.read());
  
  Joystick.button(MODE_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_OFF ? !_MODE_OFF.read() : _MODE_OFF.read());
  Joystick.button(MODE_STBY_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_STBY ? !_MODE_STBY.read() : _MODE_STBY.read());
  Joystick.button(MODE_MAN_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_MAN ? !_MODE_MAN.read() : _MODE_MAN.read());
  Joystick.button(MODE_SEMI_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_SEMI ? !_MODE_SEMI.read() : _MODE_SEMI.read());
  Joystick.button(MODE_AUTO_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_AUTO ? !_MODE_AUTO.read() : _MODE_AUTO.read());
  Joystick.button(MODE_BYP_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_MODE_BYP ? !_MODE_BYP.read() : _MODE_BYP.read());;
  
  Joystick.button(O1_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_O1 ? !_O1.read() : _O1.read());
  Joystick.button(O1_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_O1 ? _O1.read() : !_O1.read());
  Joystick.button(O2_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_O2 ? !_O2.read() : _O2.read());
  Joystick.button(O2_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_O2 ? _O2.read() : !_O2.read());
  Joystick.button(CH_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_CH ? !_CH.read() : _CH.read());
  Joystick.button(CH_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_CH ? _CH.read() : !_CH.read());
  Joystick.button(FL_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_FL ? !_FL.read() : _FL.read());
  Joystick.button(FL_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_FL ? _FL.read() : !_FL.read());

  Joystick.button(PRGM_BIT_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_PRGM_BIT ? !_PRGM_BIT.read() : _PRGM_BIT.read());
  Joystick.button(PRGM_1_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_PRGM_1 ? !_PRGM_1.read() : _PRGM_1.read());
  Joystick.button(PRGM_2_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_PRGM_2 ? !_PRGM_2.read() : _PRGM_2.read());
  Joystick.button(PRGM_3_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_PRGM_3 ? !_PRGM_3.read() : _PRGM_3.read());
  Joystick.button(PRGM_4_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_PRGM_4 ? !_PRGM_4.read() : _PRGM_4.read());
#endif
}

void updateEWMUJoystickOutputs() {
#ifdef JOYSTICK_INTERFACE
  SetJoystickAxis (EWMU_BRIGHTNESS_DX_AXIS, (uint16_t)(((float)_EWMUAndCMDSBrightness / (float)MAX_INTENSITY) * JOYSTICK_AXIS_MAX_VAL));

  Joystick.button(JETTISON_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_JETTISON ? !_JETTISON.read() : _JETTISON.read());
  Joystick.button(JETTISON_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_JETTISON ? _JETTISON.read() : !_JETTISON.read());

  Joystick.button(MWS_MENU_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_MWS_MENU ? !_MWS_MENU.read() : _MWS_MENU.read());
  Joystick.button(MWS_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MWS ? !_MWS.read() : _MWS.read());
  Joystick.button(MWS_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MWS ? _MWS.read() : !_MWS.read());

  Joystick.button(JMR_MENU_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_JMR_MENU ? !_JMR_MENU.read() : _JMR_MENU.read());
  Joystick.button(JMR_ON_DX_BUTTON,  _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_JMR ? !_JMR.read() : _JMR.read());
  Joystick.button(JMR_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_JMR ? _JMR.read() : !_JMR.read());

  Joystick.button(RWR_MENU_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_RWR_MENU ? !_RWR_MENU.read() : _RWR_MENU.read());
  Joystick.button(RWR_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_RWR ? !_RWR.read() : _RWR.read());
  Joystick.button(RWR_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_RWR ? _RWR.read() : !_RWR.read());

  Joystick.button(DISP_MENU_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_DISP_MENU  ? !_DISP_MENU.read() : _DISP_MENU.read());
  Joystick.button(DISP_ON_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_DISP ? !_DISP.read() : _DISP.read());
  Joystick.button(DISP_OFF_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_DISP ? _DISP.read() : !_DISP.read());

  bool modeStandby = _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_STBY ? !_MODE_STBY.read() : _MODE_STBY.read();
  bool modeMan = _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_MAN ? !_MODE_MAN.read() : _MODE_MAN.read();
  bool modeSemi = _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_SEMI ? !_MODE_SEMI.read() : _MODE_SEMI.read();
  bool modeAuto = _invertBits & SwitchAndButtonIDs::CMDS_AND_EWMU_MODE_AUTO ? !_MODE_AUTO.read() : _MODE_AUTO.read();
  bool modeOff = !(modeStandby || modeMan || modeSemi || modeAuto);
  Joystick.button(MODE_OFF_DX_BUTTON, modeOff);
  Joystick.button(MODE_STBY_DX_BUTTON, modeStandby);
  Joystick.button(MODE_MAN_DX_BUTTON, modeMan);
  Joystick.button(MODE_SEMI_DX_BUTTON, modeSemi);
  Joystick.button(MODE_AUTO_DX_BUTTON, modeAuto);

  pinMode(EWMU_PUSHBUTTON_MATRIX_ROW1_PIN, OUTPUT);
  digitalWriteFast(EWMU_PUSHBUTTON_MATRIX_ROW1_PIN, LOW);
  conditionalDelayMicroseconds(PIN_MODE_CHANGE_DELAY_MICROSECONDS);
  Joystick.button(EWMU_SET1_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_SET1 ? digitalRead(EWMU_PUSHBUTTON_MATRIX_COL1_PIN) : !digitalRead(EWMU_PUSHBUTTON_MATRIX_COL1_PIN));
  Joystick.button(EWMU_SET2_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_SET2  ? digitalRead(EWMU_PUSHBUTTON_MATRIX_COL2_PIN) : !digitalRead(EWMU_PUSHBUTTON_MATRIX_COL2_PIN));
  Joystick.button(EWMU_SET3_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_SET3  ? digitalRead(EWMU_PUSHBUTTON_MATRIX_COL3_PIN) : !digitalRead(EWMU_PUSHBUTTON_MATRIX_COL3_PIN));
  Joystick.button(EWMU_SET4_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_SET4  ? digitalRead(EWMU_PUSHBUTTON_MATRIX_COL4_PIN) : !digitalRead(EWMU_PUSHBUTTON_MATRIX_COL4_PIN));
  digitalWriteFast(EWMU_PUSHBUTTON_MATRIX_ROW1_PIN, HIGH);
  pinMode(EWMU_PUSHBUTTON_MATRIX_ROW1_PIN, INPUT_PULLUP);
  conditionalDelayMicroseconds(PIN_MODE_CHANGE_DELAY_MICROSECONDS);

  pinMode(EWMU_PUSHBUTTON_MATRIX_ROW2_PIN, OUTPUT);
  digitalWriteFast(EWMU_PUSHBUTTON_MATRIX_ROW2_PIN, LOW);
  conditionalDelayMicroseconds(PIN_MODE_CHANGE_DELAY_MICROSECONDS);
  Joystick.button(NXT_UP_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_NXT_UP ? digitalRead(EWMU_PUSHBUTTON_MATRIX_COL1_PIN) : !digitalRead(EWMU_PUSHBUTTON_MATRIX_COL1_PIN));
  Joystick.button(NXT_DOWN_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_NXT_DOWN ? digitalRead(EWMU_PUSHBUTTON_MATRIX_COL2_PIN) : !digitalRead(EWMU_PUSHBUTTON_MATRIX_COL2_PIN));
  Joystick.button(RTN_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWMU_RTN ? digitalRead(EWMU_PUSHBUTTON_MATRIX_COL3_PIN) : !digitalRead(EWMU_PUSHBUTTON_MATRIX_COL3_PIN));
  digitalWriteFast(EWMU_PUSHBUTTON_MATRIX_ROW2_PIN, HIGH);
  pinMode(EWMU_PUSHBUTTON_MATRIX_ROW2_PIN, INPUT_PULLUP);
  conditionalDelayMicroseconds(PIN_MODE_CHANGE_DELAY_MICROSECONDS);
#endif
}

void updateEWPIJoystickOutputs() {
#ifdef JOYSTICK_INTERFACE
  SetJoystickAxis (EWPI_BRIGHTNESS_DX_AXIS, (uint16_t)(((float)_EWPIBrightness / (float)MAX_INTENSITY) * 1023.0f));
  Joystick.button(PRI_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWPI_PRI ? !_PRI.read() : _PRI.read());
  Joystick.button(SEP_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWPI_SEP ? !_SEP.read() : _SEP.read());
  Joystick.button(UNK_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWPI_UNK ? !_UNK.read() : _UNK.read());
  Joystick.button(EWPI_MD_DX_BUTTON, _invertBits & SwitchAndButtonIDs::EWPI_MD ? !_EWPI_MD.read() : _EWPI_MD.read());
#endif
}
void beginEWMUAndCMDSCommonInputDebouncing()
{
#ifdef JOYSTICK_INTERFACE
  _JETTISON.begin();
  _MWS.begin();
  _JMR.begin();
  _RWR.begin();
  _MODE_OFF.begin();
  _MODE_STBY.begin();
  _MODE_MAN.begin();
  _MODE_SEMI.begin();
  _MODE_AUTO.begin();
#endif
}

void beginCMDSpecificInputDebouncing()
{
#ifdef JOYSTICK_INTERFACE
  _O1.begin();
  _O2.begin();
  _CH.begin();
  _FL.begin();
  _PRGM_BIT.begin();
  _PRGM_1.begin();
  _PRGM_2.begin();
  _PRGM_3.begin();
  _PRGM_4.begin();
  _MODE_BYP.begin();
#endif
}

void beginEWMUSpecificInputDebouncing() {
#ifdef JOYSTICK_INTERFACE
  _MWS_MENU.begin();
  _JMR_MENU.begin();
  _RWR_MENU.begin();
  _DISP_MENU.begin();
  _DISP.begin();
#endif
}

void beginEWPIInputDebouncing()
{
#ifdef JOYSTICK_INTERFACE
  _PRI.begin();
  _SEP.begin();
  _UNK.begin();
  _EWPI_MD.begin();
#endif
}
/* ----------------------------------------------------------------------------- */
