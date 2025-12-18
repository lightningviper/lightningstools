#pragma once
#ifndef HOST_MESSAGE_PROCESSING_H
#define HOST_MESSAGE_PROCESSING_H

#include "arduino.h"
#include "libraries/PacketSerial-1.4.0/src/PacketSerial.h"
#include "motors.h"
 
typedef enum ArduinoSeatPacketFields {
  MOTOR_STATES = 0x01, 	// MOTOR STATES
  MOTOR_1_SPEED = 0x10, // MOTOR 1 SPEED VALUE
  MOTOR_2_SPEED = 0x20, // MOTOR 2 SPEED VALUE
  MOTOR_3_SPEED = 0x40, // MOTOR 3 SPEED VALUE
  MOTOR_4_SPEED = 0x80, // MOTOR 4 SPEED VALUE
} ArduinoSeatPacketFields;

typedef enum MotorBits {
  ALL_OFF  = 0x00,
  MOTOR_1_STATE = 0x01,
  MOTOR_2_STATE = 0x02,
  MOTOR_3_STATE = 0x04,
  MOTOR_4_STATE = 0x08,
} MotorBits;

/* -------------- COMMUNICATIONS SETUP --------------- */
const uint32_t BAUD_RATE = 115200;
const size_t RECEIVE_BUFFER_SIZE = 1024;
/* --------------------------------------------------- */

void startAcceptingSerialDataPackets();
void processIncomingSerialData();
void onPacketReceived(const uint8_t* buffer, size_t size);

#endif
