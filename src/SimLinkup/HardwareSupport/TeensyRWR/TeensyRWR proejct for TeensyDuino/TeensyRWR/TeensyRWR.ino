#include <PacketSerial.h>

const int X_PIN = A21;
const int Y_PIN = A22;
const int Z_PIN = 39;
const int BUILTIN_LED_PIN = 13;

//DAC precision & scale
const unsigned int DAC_PRECISION_BITS = 12;
const unsigned int MAX_DAC_VALUE = 0xFFF;

//communications settings
const unsigned int BAUD_RATE = 115200;
const unsigned int VECTOR_BUFFER_SIZE = 14 * 1000;
const unsigned int RECEIVE_BUFFER_SIZE = 3 * VECTOR_BUFFER_SIZE;

//timings
const unsigned int SERIAL_WRITE_DELAY_MILLIS = 5;
const unsigned int BEAM_TURNON_SETTLING_TIME_MICROSECONDS = 0;
const unsigned int BEAM_TURNOFF_SETTLING_TIME_MICROSECONDS = 0;

//firmware version
const unsigned int FIRMWARE_MAJOR_VERSION = 0;
const unsigned int FIRMWARE_MINOR_VERSION = 1;
const String FIRMWARE_IDENTIFICATION = "Teensy RWR v" + String(FIRMWARE_MAJOR_VERSION) + "." + String(FIRMWARE_MINOR_VERSION);

#define CPU_RESTART_ADDR (uint32_t *)0xE000ED0C
#define CPU_RESTART_VAL 0x5FA0004
#define CPU_RESTART (*CPU_RESTART_ADDR = CPU_RESTART_VAL);

typedef struct Point
{
  unsigned short X;
  unsigned short Y;
} Point;

typedef struct Vector
{
  Point point;
  bool BeamOn;
} Vector;



Vector _vectorFrontBuffer[VECTOR_BUFFER_SIZE];
unsigned short _vectorFrontBufferDataLength = 0;

Vector _vectorBackBuffer[VECTOR_BUFFER_SIZE];
unsigned short _vectorBackBufferDataLength = 0;

bool _ledOn = false;
bool _beamOn = false;
Point _beamLocation;

PacketSerial_<COBS, 0, RECEIVE_BUFFER_SIZE> _packetSerial ;
IntervalTimer _drawTimer;
void setup() {
  _packetSerial.begin(BAUD_RATE);
  _packetSerial.setStream(&Serial);
  _packetSerial.setPacketHandler(&onPacketReceived);
  _drawTimer.begin(draw, 16000);
  analogWriteResolution(DAC_PRECISION_BITS);
  pinMode(Z_PIN, OUTPUT);
  pinMode(BUILTIN_LED_PIN, OUTPUT);
  LEDOff();
}

void loop() {
  _packetSerial.update();
}
void onPacketReceived(const uint8_t* buffer, size_t size) {
  if (size <= 2) return;
  toggleLED();
  size_t bufferIndex = 0;
  uint16_t numVectors = (uint16_t)(((uint32_t)buffer[bufferIndex] << 8) | (uint32_t)buffer[bufferIndex + 1]);
  bufferIndex += 2;

  if (numVectors >= VECTOR_BUFFER_SIZE) {
    numVectors = VECTOR_BUFFER_SIZE - 1;
  }

  _vectorBackBufferDataLength = 0;
  for (uint16_t i = 0; i < numVectors; i++)
  {
    uint32_t xyzCombined = (unsigned int)((unsigned int)buffer[bufferIndex] << 16 | (unsigned int)buffer[bufferIndex + 1] << 8 | (unsigned int)buffer[bufferIndex + 2]);
    bufferIndex += 3;
    Vector v;
    v.point.X = (xyzCombined & 0xFFF000) >> 12;
    v.point.Y = (xyzCombined & 0xFFE);
    v.BeamOn = ((xyzCombined & 0x001)==0x001);
    _vectorBackBuffer[i] = v;
    _vectorBackBufferDataLength++;
  }
  swap();
}
void swap() {
  memcpy(_vectorFrontBuffer, _vectorBackBuffer, sizeof(Vector)*_vectorBackBufferDataLength);
  _vectorFrontBufferDataLength = _vectorBackBufferDataLength;
  _vectorBackBufferDataLength = 0;

}
void draw() {
  noInterrupts();
  if (_vectorFrontBufferDataLength > 0)
  {
    for (int i = 0; i < _vectorFrontBufferDataLength; i++)
    {
      if (_vectorFrontBuffer[i].BeamOn)
      {
        lineTo(_vectorFrontBuffer[i].point);
      }
      else {
        moveTo(_vectorFrontBuffer[i].point);
      }
    }
  }
  beamOff();
  moveTo(Point{0,0});
  interrupts();

}
void LEDOff() {
  _ledOn = false;
  updateLED();
}
void LEDOn() {
  _ledOn = true;
  updateLED();
}
void toggleLED() {
  _ledOn = !_ledOn;
  updateLED();
}
void updateLED() {
  digitalWrite(BUILTIN_LED_PIN, _ledOn ? HIGH : LOW);
}


void moveTo(Point point) {
  beamOff();
  beamTo(point);
}

void lineTo(Point point) {
  beamOn();
  beamToWithInterpolation(point);
}

void beamTo(Point point) {
  if (!(_beamLocation.X == point.X && _beamLocation.Y == point.Y))
  {
    writePointToDACs(point);
  }
}

void beamToWithInterpolation(Point point) {
  if (_beamOn && (!(_beamLocation.X == point.X && _beamLocation.Y == point.Y)))
  {
    Point beamStartLocation=_beamLocation;
    float dx = point.X - beamStartLocation.X;
    float dy = point.Y - beamStartLocation.Y;
    float distance = abs(sqrtf((dx * dx) + (dy * dy)));
    for (float i = 0; i <=distance; i++) {
      Point nextPoint;
      nextPoint.X = ((float)beamStartLocation.X + (dx / distance) * i);
      nextPoint.Y = ((float)beamStartLocation.Y + (dy / distance) * i);
      writePointToDACs(nextPoint);
    }
  }
  if (!(_beamLocation.X == point.X && _beamLocation.Y == point.Y)) 
  {
    writePointToDACs(point);
  }
}

void writePointToDACs(Point point) {
  if (_beamLocation.X != point.X || _beamLocation.Y != point.Y) {
    analogWrite(X_PIN, point.X);
    analogWrite(Y_PIN, point.Y);
    _beamLocation = point;
  }
}

void beamOff() {
  if (_beamOn) {
    digitalWrite(Z_PIN, HIGH);
    _beamOn = false;
    if (BEAM_TURNOFF_SETTLING_TIME_MICROSECONDS > 0) {
      delayMicroseconds(BEAM_TURNOFF_SETTLING_TIME_MICROSECONDS);
    }
  }
}

void beamOn() {
  if (!_beamOn) {
    digitalWrite(Z_PIN, LOW);
    _beamOn = true;
    if (BEAM_TURNON_SETTLING_TIME_MICROSECONDS) {
      delayMicroseconds(BEAM_TURNON_SETTLING_TIME_MICROSECONDS);
    }
  }
}
