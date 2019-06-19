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
const unsigned int SERIAL_READ_TIMEOUT_MILLIS = 0;
const size_t RECEIVE_BUFFER_SIZE=42 * 1024;

//beam timings
const unsigned int REFRESH_RATE_HZ = 60;
const uint16_t BEAM_MOVEMENT_WHILE_BEAM_OFF_DELAY_MICROSECONDS = 50;
const uint16_t BEAM_MOVEMENT_MAX_DISTANCE_DELAY_MICROSECONDS = 50;
const uint16_t BEAM_DELAY_BETWEEN_DRAW_POINTS_MICROSECONDS=8;
const uint16_t BLANKING_SIGNAL_SETTLING_TIME_MICROSECONDS=15;
const uint16_t BLANKING_SIGNAL_RISE_TIME_MICROSECONDS=15;

//draw points buffer setup
const size_t DRAW_POINTS_BUFFER_SIZE = 10 * 1024;
uint32_t _drawPointsBuffer[DRAW_POINTS_BUFFER_SIZE];
size_t _drawPointsBufferLength = 0;


//draw timer setup 
IntervalTimer _drawTimer;

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
//  _drawTimer.priority(255);
//  _drawTimer.begin(draw, 1 * 1000 * 1000 / REFRESH_RATE_HZ);

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
    
        bool beamOnFlag = ((xyzCombined & (uint32_t)0x1000000) == (uint32_t)0x1000000); //bit 24
        uint16_t xDAC = ((xyzCombined & (uint32_t)0xFFF000) >> 12); //bits 12-23 [12 bits]
        uint16_t yDAC = (xyzCombined & (uint32_t)0xFFF); //bits 0-11 [12 bits]

        if (beamOnFlag) //we should DRAW A LINE to this point WITH THE BEAM TURNED ON
        {
          beamOn();
          beamTo(xDAC, yDAC);
        }
        else //we should MOVE to this point WITH THE BEAM TURNED OFF
        {
          beamOff();
          beamTo(xDAC, yDAC);
        }
        delayMicroseconds(BEAM_DELAY_BETWEEN_DRAW_POINTS_MICROSECONDS);
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
  float maxDistance = distance(0,0, MAX_DAC_VALUE, MAX_DAC_VALUE);
  float dist = distance(x, y, _currentBeamLocationXDAC, _currentBeamLocationYDAC);
  writePointToDACs(x, y);
  delayMicroseconds((uint16_t) BEAM_MOVEMENT_MAX_DISTANCE_DELAY_MICROSECONDS * (dist / maxDistance) ); //wait for beam to get there
  if (!_beamOn) delayMicroseconds(BEAM_MOVEMENT_WHILE_BEAM_OFF_DELAY_MICROSECONDS);
}
float distance(float x1, float y1, float x2, float y2)
{
  float dx = x2 - x1;
  float dy = y2 - y1;
  return fabs(sqrtf(dx * dx + dy * dy));
}

void writePointToDACs(uint16_t xDAC, uint16_t yDAC)
{
  if (_currentBeamLocationXDAC == xDAC && _currentBeamLocationYDAC == yDAC)
  {
    return;
  }
  analogWrite(X_PIN, xDAC);
  analogWrite(Y_PIN, yDAC);
  _currentBeamLocationXDAC = xDAC;
  _currentBeamLocationYDAC = yDAC;
}

void beamOff()
{
  if (!_beamOn)
  {
    return;
  }
  digitalWrite(Z_PIN, HIGH);
  delayMicroseconds(BLANKING_SIGNAL_SETTLING_TIME_MICROSECONDS);
  _beamOn = false;
}

void beamOn()
{
  if (_beamOn)
  {
    return;
  }
  digitalWrite(Z_PIN, LOW);
  delayMicroseconds(BLANKING_SIGNAL_RISE_TIME_MICROSECONDS);
  _beamOn = true;
}
