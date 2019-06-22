#include "PacketSerial-1.2.0/PacketSerial.h"

//pin assignments
const int X_PIN = A21;
const int Y_PIN = A22;
const int Z_PIN = A20;
const int BUILTIN_LED_PIN = 13;

//DAC precision & scale
const unsigned int DAC_PRECISION_BITS = 12;
const uint16_t MAX_DAC_VALUE = 0xFFF;

//communication settings
const unsigned int BAUD_RATE = 115200;
const size_t RECEIVE_BUFFER_SIZE= 64*1024;

//beam timings
const float MAX_LINEAR_DISTANCE = sqrtf((MAX_DAC_VALUE * MAX_DAC_VALUE) *2.0);
const float FULL_RANGE_BEAM_MOVEMENT_MIN_TIME_MICROSECONDS=500;  
const float DAC_FULL_SCALE_SETTLING_TIME_MICROSECONDS=32; 
const float DAC_CODE_TO_CODE_SETTLING_TIME_MICROSECONDS=15; 
const float BLANKING_DELAY_MICROSECONDS=9; 

//draw points buffer setup
const size_t DRAW_POINTS_BUFFER_SIZE = 21 * 1024;
uint32_t _drawPointsBuffer[DRAW_POINTS_BUFFER_SIZE];
size_t _drawPointsBufferLength = 0;

//beam location and state
uint16_t _currentBeamLocationXDAC = 0;
uint16_t _currentBeamLocationYDAC = 0;
bool _beamOn = true; //set to true at initialization time but will be turned off in setup()

//LED state
bool _ledOn = false;

PacketSerial_<COBS, 0, RECEIVE_BUFFER_SIZE> packetSerial;

void setup()
{
  pinMode(Z_PIN, OUTPUT);
  pinMode(BUILTIN_LED_PIN, OUTPUT);
  analogWriteResolution(DAC_PRECISION_BITS);
  LEDOn();
  beamOff();
  packetSerial.begin(BAUD_RATE);
  packetSerial.setPacketHandler(&onPacketReceived);
  beamOff();
}

void loop()
{
  packetSerial.update();
  draw();
}
void onPacketReceived(const uint8_t* buffer, size_t size)
{
  toggleLED();
  size_t bytesToCopy = min(DRAW_POINTS_BUFFER_SIZE * sizeof(uint32_t), size);
  memcpy (_drawPointsBuffer, buffer, bytesToCopy);
  _drawPointsBufferLength=bytesToCopy/sizeof(uint32_t);
}

void draw()
{
      if (_drawPointsBufferLength <= 0)
      {
        return;
      }
      for (size_t i = 0; i < _drawPointsBufferLength; i++)
      {
        uint32_t xyzCombined = _drawPointsBuffer[i];
    
        bool beamOnFlag = ((((uint32_t)xyzCombined) & ((uint32_t)0x1000000)) == (uint32_t)0x1000000); //bit 24
        uint16_t xDAC = ((((uint32_t)xyzCombined) & ((uint32_t)0xFFF000)) >> 12); //bits 12-23 [12 bits]
        uint16_t yDAC = (((uint32_t)MAX_DAC_VALUE)-(((uint32_t)xyzCombined) & ((uint32_t)0xFFF))); //bits 0-11 [12 bits]
        
        beamOnFlag ? beamOn() : beamOff();
        beamTo(xDAC, yDAC);
      }
      beamOff();
      beamTo(0, MAX_DAC_VALUE);
}

void LEDOff()
{
  _ledOn = false;
  updateLED();
}

void LEDOn()
{
  _ledOn = true;
  updateLED();
}

void toggleLED()
{
  _ledOn = !_ledOn;
  updateLED();
}

void updateLED()
{
  digitalWrite(BUILTIN_LED_PIN, _ledOn ? HIGH : LOW);
}

void beamTo(uint16_t x, uint16_t y)
{
  //update the DAC outputs
  if (_currentBeamLocationXDAC == x && _currentBeamLocationYDAC == y)
  {
    return;
  }
  analogWrite(X_PIN, x);
  analogWrite(Y_PIN, y);

  //calculate expected DAC settling time
  float dx = _currentBeamLocationXDAC - x;
  float dy = _currentBeamLocationYDAC - y;
  float dacSettlingTime= fmax(DAC_CODE_TO_CODE_SETTLING_TIME_MICROSECONDS, ((fmax(dx, dy)/ MAX_DAC_VALUE) * DAC_FULL_SCALE_SETTLING_TIME_MICROSECONDS));

  //calculate expected beam travel time
  float beamMovementTimeMicroseconds =  (fabs(sqrtf(dx * dx + dy * dy)) / MAX_LINEAR_DISTANCE) * FULL_RANGE_BEAM_MOVEMENT_MIN_TIME_MICROSECONDS;

  //wait for (the longer of) expected DAC settling time or expected beam movement time
  delayMicroseconds(fmax(beamMovementTimeMicroseconds, dacSettlingTime)); 

  _currentBeamLocationXDAC = x;
  _currentBeamLocationYDAC = y;
}

void beamOff()
{
  if (!_beamOn)
  {
    return;
  }
  digitalWrite(Z_PIN, HIGH);
  delayMicroseconds(BLANKING_DELAY_MICROSECONDS);
  _beamOn = false;
}

void beamOn()
{
  if (_beamOn)
  {
    return;
  }
  digitalWrite(Z_PIN, LOW);
  delayMicroseconds(BLANKING_DELAY_MICROSECONDS);
  _beamOn = true;
}
