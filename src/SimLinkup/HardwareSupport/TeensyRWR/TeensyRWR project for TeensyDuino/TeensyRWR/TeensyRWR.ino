#include "PacketSerial-1.2.0/PacketSerial.h"

//pin assignments
const int X_PIN = A21;
const int Y_PIN = A22;
const int Z_PIN = A20;
const int BUILTIN_LED_PIN = 13;

//inversion settings
const bool INVERT_X = false;
const bool INVERT_Y = false;
const bool INVERT_Z = true;

//DAC precision & scale
const unsigned int DAC_PRECISION_BITS = 12;
const uint16_t MAX_DAC_VALUE = 0xFFF;

//draw points buffer setup
const size_t DRAW_POINTS_BUFFER_SIZE = 21 * 1024;
uint32_t _drawPointsBuffer[DRAW_POINTS_BUFFER_SIZE];
size_t _drawPointsBufferLength = 0;

//communication settings
const unsigned int BAUD_RATE = 115200;
const size_t RECEIVE_BUFFER_SIZE= 64*1024;
PacketSerial_<COBS, 0, RECEIVE_BUFFER_SIZE> packetSerial;

//beam timings
const float BEAM_INCREMENT_MOVEMENT_DELAY_MICROSECONDS=0.3;
const float BLANKING_DELAY_MICROSECONDS=7;

//beam location and state
uint16_t _currentBeamLocationXDAC = 0;
uint16_t _currentBeamLocationYDAC = 0;
bool _beamOn = true; //set to true at initialization time but will be turned off in setup()

//LED state
bool _ledOn = false;


void setup()
{
  pinMode(Z_PIN, OUTPUT);
  beamOff();

  pinMode(BUILTIN_LED_PIN, OUTPUT);
  analogWriteResolution(DAC_PRECISION_BITS);
  LEDOn();
  
  packetSerial.begin(BAUD_RATE);
  packetSerial.setPacketHandler(&onPacketReceived);

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
      if (_drawPointsBufferLength <= 0) return;

      for (size_t i = 0; i < _drawPointsBufferLength; i++)
      {
        uint32_t point = _drawPointsBuffer[i];
        bool beamOnFlag = ((((uint32_t)point) & ((uint32_t)0x1000000)) == (uint32_t)0x1000000); //bit 24
        uint16_t xDAC = ((((uint32_t)point) & ((uint32_t)0xFFF000)) >> 12); //bits 12-23 [12 bits]
        uint16_t yDAC = (((((uint32_t)point) & ((uint32_t)0xFFF)))); //bits 0-11 [12 bits]
        beamOnFlag ? beamOn() : beamOff();
        drawPoint(xDAC, yDAC, beamOnFlag);
      }
      beamOff();
      drawPoint(0, MAX_DAC_VALUE, false);
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

void drawPoint(uint16_t x, uint16_t y, bool beamOnFlag)
{
  if (x <0) x = 0;
  else if (x > MAX_DAC_VALUE) x = MAX_DAC_VALUE;

  if (y <0) y = 0;
  else if (y > MAX_DAC_VALUE) y = MAX_DAC_VALUE;

  if (_currentBeamLocationXDAC == x && _currentBeamLocationYDAC == y) return;

  uint16_t startX = _currentBeamLocationXDAC;
  uint16_t startY = _currentBeamLocationYDAC;
  
  //calculate the distance the beam will move
  float xDist = x-startX;
  float yDist = y-startY;
  float euclideanDist=fabs(sqrtf((xDist * xDist) + (yDist * yDist)));
  uint32_t numSteps=fmax(1, (ceilf(euclideanDist)));

 if (beamOnFlag) 
  {
    float dx = (xDist/numSteps);
    float dy = (yDist/numSteps);
    for (uint32_t i=1;i<numSteps;i++) 
    {
      updateDACOutputs((startX + (dx * i)), (startY + (dy * i)));
      delayMicroseconds(BEAM_INCREMENT_MOVEMENT_DELAY_MICROSECONDS);
    }
    updateDACOutputs(x,y);
    delayMicroseconds(BEAM_INCREMENT_MOVEMENT_DELAY_MICROSECONDS);
  }
  else 
  {
    updateDACOutputs (x,y);
    delayMicroseconds(euclideanDist * BEAM_INCREMENT_MOVEMENT_DELAY_MICROSECONDS);
  }
  
  _currentBeamLocationXDAC = x;
  _currentBeamLocationYDAC = y;
}
void updateDACOutputs(uint16_t xDAC, uint16_t yDAC) 
{
    analogWrite(X_PIN, INVERT_X ? MAX_DAC_VALUE - xDAC : xDAC);
    analogWrite(Y_PIN, INVERT_Y ? MAX_DAC_VALUE - yDAC : yDAC);
}

void beamOff()
{
  if (!_beamOn) return;
  digitalWrite(Z_PIN, INVERT_Z ? HIGH : LOW);
  delayMicroseconds(BLANKING_DELAY_MICROSECONDS);
  _beamOn=false;
}

void beamOn()
{
  if (_beamOn) return;
  digitalWrite(Z_PIN, INVERT_Z ? LOW: HIGH);
  delayMicroseconds(BLANKING_DELAY_MICROSECONDS);
  _beamOn=true;
}
