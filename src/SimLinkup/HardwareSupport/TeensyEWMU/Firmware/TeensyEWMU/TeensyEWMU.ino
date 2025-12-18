#include "basicConfig.h"
#include "pinAssignments.h"
#include "brightness.h"
#include "timing.h"
#include "hostMessageProcessing.h"
#include "characterDisplay.h"
#include "lights.h"
#include "joystick.h"

void setup()
{
  setupButtonInputs();
  setupBrightnessPotInputs();
  setupCharacterDisplays();
  setupLightOutputs();
  startAcceptingSerialDataPackets();
}

void loop()
{
  processIncomingSerialData();
  updateBrightnessPotInputs();
  updateCharacterDisplays();
  updateLightOutputs();
  updateJoystickOutputs();
}
