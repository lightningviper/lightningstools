#include "hostMessageProcessing.h"

extern uint8_t _MotorStates;

extern uint8_t _Motor1Speed;
extern uint8_t _Motor2Speed;
extern uint8_t _Motor3Speed;
extern uint8_t _Motor4Speed;

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

    if (fieldType & ArduinoSeatPacketFields::MOTOR_STATES)
    {
      memset(&_MotorStates, 0, sizeof(_MotorStates));
      memcpy(&_MotorStates, buffer + bufferOffset, sizeof(_MotorStates));
      bufferOffset += sizeof(_MotorStates);
    }
    else if (fieldType & ArduinoSeatPacketFields::MOTOR_1_SPEED)
    {
      memset(&_Motor1Speed, 0, sizeof(_Motor1Speed));
      memcpy(&_Motor1Speed, buffer + bufferOffset, sizeof(_Motor1Speed));
      bufferOffset += sizeof(_Motor1Speed);
    }
    else if (fieldType & ArduinoSeatPacketFields::MOTOR_2_SPEED)
    {
      memset(&_Motor2Speed, 0, sizeof(_Motor2Speed));
      memcpy(&_Motor2Speed, buffer + bufferOffset, sizeof(_Motor2Speed));
      bufferOffset += sizeof(_Motor2Speed);
    }
    else if (fieldType & ArduinoSeatPacketFields::MOTOR_3_SPEED)
    {
      memset(&_Motor3Speed, 0, sizeof(_Motor3Speed));
      memcpy(&_Motor3Speed, buffer + bufferOffset, sizeof(_Motor3Speed));
      bufferOffset += sizeof(_Motor3Speed);
    }
    else if (fieldType & ArduinoSeatPacketFields::MOTOR_4_SPEED)
    {
      memset(&_Motor4Speed, 0, sizeof(_Motor4Speed));
      memcpy(&_Motor4Speed, buffer + bufferOffset, sizeof(_Motor4Speed));
      bufferOffset += sizeof(_Motor4Speed);
    }
  }
}
