#include "basicConfig.h"
#include "pinAssignments.h"
#include "hostMessageProcessing.h"
#include "motors.h"

void setup() 
{
  Serial.print("Initialize");
  setupMotorOutputs();
  Serial.print("Start Serial");
  startAcceptingSerialDataPackets();
}

void loop() 
{
  //Serial.print("Process Data\n");
  processIncomingSerialData();
  //Serial.print("Update Motors\n");
  updateMotorOutputs();
}
