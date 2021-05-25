#pragma once
#ifndef HOST_MESSAGE_PROCESSING_H
#define HOST_MESSAGE_PROCESSING_H

#include "arduino.h"
#include "src/libraries/PacketSerial-1.2.0/PacketSerial.h"
#include "characterDisplay.h"
#include "lights.h"
#include "joystickAssignments.h"
 
typedef enum TeensyEWMUPacketFields {
  EWMU_DISPLAY_STRING = 0x01,
  CMDS_DISPLAY_STRING = 0x02,
  EWPI_DISPLAY_STRING = 0x04,
  EWPI_LIGHTBITS = 0x08,
  CMDS_LIGHTBITS = 0x10,
  INVERT_STATES = 0x20
} TeensyEWMUPacketFields;

typedef enum EWPILightbits {
  ALL_EWPI_LIGHTS_OFF  = 0x00,
  PRI = 0x01,
  UNK = 0x02,
  ML = 0x04,
  ALL_EWPI_LIGHTS_ON = 0x07,
} EWPILightbits;

typedef enum CMDSLightbits {
  ALL_CMDS_LIGHTS_OFF  = 0x00,
  NOGO = 0x01,
  GO = 0x02,
  DISPENSE_READY = 0x04,
  ALL_CMDS_LIGHTS_ON = 0x07,
} CMDSLightbits;

typedef enum SwitchAndButtonIDs {
  CMDS_O1=0x1UL,
  CMDS_O2=0x2UL,
  CMDS_CH=0x4UL,
  CMDS_FL=0x8UL,
  CMDS_AND_EWMU_JETTISON=0x10UL,
  CMDS_PRGM_BIT=0x20UL,
  CMDS_PRGM_1=0x40UL,
  CMDS_PRGM_2=0x80UL,
  CMDS_PRGM_3=0x100UL,
  CMDS_PRGM_4=0x200UL,
  CMDS_AND_EWMU_MWS=0x400UL,
  CMDS_AND_EWMU_JMR=0x800UL,
  CMDS_AND_EWMU_RWR=0x1000UL,
  EWMU_DISP=0x2000UL,
  CMDS_AND_EWMU_MODE_OFF=0x4000UL,
  CMDS_AND_EWMU_MODE_STBY=0x8000UL,
  CMDS_AND_EWMU_MODE_MAN=0x10000UL,
  CMDS_AND_EWMU_MODE_SEMI=0x20000UL,
  CMDS_AND_EWMU_MODE_AUTO=0x40000UL,
  CMDS_MODE_BYP=0x80000UL,
  EWMU_MWS_MENU=0x100000UL,
  EWMU_JMR_MENU=0x200000UL,
  EWMU_RWR_MENU=0x400000UL,
  EWMU_DISP_MENU=0x800000UL,
  EWPI_PRI=0x100000UL,
  EWPI_SEP=0x200000UL,
  EWPI_UNK=0x400000UL,
  EWPI_MD=0x800000UL,
  EWMU_SET1=0x1000000UL,
  EWMU_SET2=0x2000000UL,
  EWMU_SET3=0x4000000UL,
  EWMU_SET4=0x8000000UL,
  EWMU_NXT_UP=0x10000000UL,
  EWMU_NXT_DOWN=0x20000000UL,
  EWMU_RTN=0x40000000UL,
} SwitchAndButtonIDs;

/* -------------- COMMUNICATIONS SETUP --------------- */
const uint32_t BAUD_RATE = 115200;
const size_t RECEIVE_BUFFER_SIZE = 64 * 1024;
/* --------------------------------------------------- */

void startAcceptingSerialDataPackets();
void processIncomingSerialData();
void onPacketReceived(const uint8_t* buffer, size_t size);

#endif
