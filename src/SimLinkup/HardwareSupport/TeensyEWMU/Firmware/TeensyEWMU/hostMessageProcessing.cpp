#include "hostMessageProcessing.h"

extern uint8_t _CMDSLightsState;
extern uint8_t _EWPILightsState;
extern char _CMDSChars[CMDS_NUM_CHARACTERS_TO_DISPLAY];
extern char _EWMUChars[EWMU_NUM_CHARACTERS_TO_DISPLAY];
extern char _EWPIChars[EWPI_NUM_CHARACTERS_TO_DISPLAY];
extern uint32_t _invertBits;
extern uint8_t _CMDSConditionalBlankingBits;

PacketSerial_<COBS, 0, RECEIVE_BUFFER_SIZE> _packetSerial;

void startAcceptingSerialDataPackets() {
  _packetSerial.begin(BAUD_RATE);
  _packetSerial.setPacketHandler(&onPacketReceived);
}

void processIncomingSerialData() {
  _packetSerial.update();
}

void onPacketReceived(const uint8_t* buffer, size_t size) {
  toggleBuiltInLED();

  size_t bufferOffset = 0;
  while (bufferOffset < size)
  {
    uint8_t fieldType = buffer[bufferOffset];
    bufferOffset++;

    if (fieldType & TeensyEWMUPacketFields::EWMU_DISPLAY_STRING)
    {
      memset(_EWMUChars, 0, sizeof(_EWMUChars));
      memcpy(_EWMUChars, buffer + bufferOffset, EWMU_NUM_CHARACTERS_TO_DISPLAY);
      bufferOffset += EWMU_NUM_CHARACTERS_TO_DISPLAY;
    }
    else if (fieldType & TeensyEWMUPacketFields::CMDS_DISPLAY_STRING)
    {
      memset(_CMDSChars, 0, sizeof(_CMDSChars));
      memcpy(_CMDSChars, buffer + bufferOffset, CMDS_NUM_CHARACTERS_TO_DISPLAY);
      bufferOffset += CMDS_NUM_CHARACTERS_TO_DISPLAY;
    }
    else if (fieldType & TeensyEWMUPacketFields::EWPI_DISPLAY_STRING)
    {
      memset(_EWPIChars, 0, sizeof(_EWPIChars));
      memcpy(_EWPIChars, buffer + bufferOffset, EWPI_NUM_CHARACTERS_TO_DISPLAY);
      bufferOffset += EWPI_NUM_CHARACTERS_TO_DISPLAY;
    }
    else if (fieldType & TeensyEWMUPacketFields::EWPI_LIGHTBITS)
    {
      memset(&_EWPILightsState, 0, sizeof(_EWPILightsState));
      memcpy(&_EWPILightsState, buffer + bufferOffset, sizeof(_EWPILightsState));
      bufferOffset += sizeof(_EWPILightsState);
    }
    else if (fieldType & TeensyEWMUPacketFields::CMDS_LIGHTBITS)
    {
      memset(&_CMDSLightsState, 0, sizeof(_CMDSLightsState));
      memcpy(&_CMDSLightsState, buffer + bufferOffset, sizeof(_CMDSLightsState));
      bufferOffset += sizeof(_CMDSLightsState);
    }
    else if (fieldType & TeensyEWMUPacketFields::INVERT_STATES)
    {
      memset(&_invertBits, 0, sizeof(_invertBits));
      memcpy(&_invertBits, buffer + bufferOffset, sizeof(_invertBits));
      bufferOffset += sizeof(_invertBits);      
    }
    else if (fieldType & TeensyEWMUPacketFields::CMDS_CONDITIONAL_BLANKING_BITS)
    {
      memset(&_CMDSConditionalBlankingBits, 0, sizeof(_CMDSConditionalBlankingBits));
      memcpy(&_CMDSConditionalBlankingBits, buffer + bufferOffset, sizeof(_CMDSConditionalBlankingBits));
      bufferOffset += sizeof(_CMDSConditionalBlankingBits);      
    }
  }
}
