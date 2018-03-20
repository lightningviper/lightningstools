#include "SVGCurveLib.h"
#include <PacketSerial.h>

const int X_PIN = A21;
const int Y_PIN = A22;
const int Z_PIN = A14;
const int BUILTIN_LED_PIN = 13;

//DAC precision & scale
const unsigned int DAC_PRECISION_BITS = 12;
const uint16_t MAX_DAC_VALUE = 0xFFF;

//communication settings
const unsigned int BAUD_RATE = 115200;
const size_t RECEIVE_BUFFER_SIZE=64 * 1024;

//drawing settings
const size_t DRAW_POINTS_BUFFER_SIZE = 20 * 1024 * 3;
const unsigned int INTERPOLATION_STEPS = 50;
const float MIN_DISTANCE = 0.001f;

//timings
const unsigned int SERIAL_WRITE_DELAY_MILLIS = 5;

//firmware version
const unsigned int FIRMWARE_MAJOR_VERSION = 0;
const unsigned int FIRMWARE_MINOR_VERSION = 1;
const String FIRMWARE_IDENTIFICATION = "Teensy RWR v" + String(FIRMWARE_MAJOR_VERSION) + "." + String(FIRMWARE_MINOR_VERSION);

#define CPU_RESTART_ADDR (uint32_t *)0xE000ED0C
#define CPU_RESTART_VAL 0x5FA0004
#define CPU_RESTART (*CPU_RESTART_ADDR = CPU_RESTART_VAL);

bool _ledOn = false;
bool _beamOn = false;

uint8_t _drawPointsBackBuffer[DRAW_POINTS_BUFFER_SIZE];
volatile size_t _drawPointsBackBufferLength = 0;

uint8_t _drawPointsFrontBuffer[DRAW_POINTS_BUFFER_SIZE];
volatile size_t _drawPointsFrontBufferLength = 0;

IntervalTimer _drawTimer;

float _currentX = 0;
float _currentY = 0;
float _figureStartX = 0;
float _figureStartY = 0;
char _previousCommand = 'z';
float _previousCommandControlPointX = 0;
float _previousCommandControlPointY = 0;

float _viewBoxLeft = 0;
float _viewBoxTop = -300;
float _viewBoxWidth = 300;
float _viewBoxHeight = 300;

float _xDAC = 0;
float _yDAC = 0;

bool _invertX = false;
bool _invertY = true;

volatile bool _drawing = false;
volatile bool _drawReady = false;

PacketSerial_<COBS, 0, RECEIVE_BUFFER_SIZE> _packetSerial;


void setup() {
  pinMode(Z_PIN, OUTPUT);
  pinMode(BUILTIN_LED_PIN, OUTPUT);
  analogWriteResolution(DAC_PRECISION_BITS);
  LEDOff();
  _packetSerial.begin(9600);
  _packetSerial.setStream(&Serial);
  _packetSerial.setPacketHandler(&processFrame);
  _drawTimer.begin(draw, 16000);
}

void loop() {
  _packetSerial.update();
}
void processFrame(const uint8_t* buffer, size_t size) {
  discardBackBuffer();
  parse(buffer, size);
  present();
}
void present() {
  if (_drawPointsBackBufferLength > 0) {
    noInterrupts();
    _drawReady = false;
    while (_drawing);
    memcpy(_drawPointsFrontBuffer, _drawPointsBackBuffer, _drawPointsBackBufferLength);
    _drawPointsFrontBufferLength = _drawPointsBackBufferLength;
    _drawReady = true;
    interrupts();
    discardBackBuffer();
  }
}
void discardBackBuffer() {
  if (_drawPointsBackBufferLength == 0) return;
  _drawPointsBackBufferLength = 0;
  memset(_drawPointsBackBuffer, 0, DRAW_POINTS_BUFFER_SIZE);
}
void draw() {
  if (!_drawReady || _drawPointsFrontBufferLength < 3) return;
  noInterrupts();
  _drawing = true;
  for (size_t i = 0; i <= (_drawPointsFrontBufferLength - 3); i += 3)
  {
    uint32_t xyzCombined = (_drawPointsFrontBuffer[i] << 16 | _drawPointsFrontBuffer[i + 1] << 8 | _drawPointsFrontBuffer[i + 2]);
    uint16_t xDAC = ((xyzCombined & 0x7FF000) >> 12) * 2;
    uint16_t yDAC = ((xyzCombined & 0xFFe) >> 1) * 2;
    bool beamIsOn = (xyzCombined & 0x01);
    if (beamIsOn)
    {
      beamOn();
      beamToWithInterpolation(xDAC, yDAC);
    }
    else {
      beamOff();
      beamTo(xDAC, yDAC);
    }
  }
  beamOff();
  beamTo(0, MAX_DAC_VALUE);
  interrupts();
  _drawing = false;
}
void parse(const uint8_t* buffer, size_t size) {
  if (size == 0) return;
  size_t offset = 0;
  while (offset < size - 1) {
    skipSeparators(buffer, offset, size);
    if (offset > size - 1) break;
    char nextChar = buffer[offset++];
    if (nextChar > 0) {
      processPathChar(nextChar, buffer, offset, size);
    }
  }
}

void echo(char charToEcho) {
  Serial.write(charToEcho);
}

void processPathChar(char pathChar, const uint8_t* buffer, size_t &offset, size_t bufferLength) {
  float args[7] = {0, 0, 0, 0, 0, 0, 0};
  switch (pathChar)
  {
    case 'M': //move to, absolute
      readFloats(buffer, offset, bufferLength, 2, args);
      moveTo(args[0], args[1]);
      _previousCommand = 'L'; //subsequent coordinates will be treated as implicit absolute LineTo commands
      _previousCommandControlPointX = _currentX;
      _previousCommandControlPointY = _currentY;
      break;
    case 'm': //move to, relative
      readFloats(buffer, offset, bufferLength, 2, args);
      moveTo(args[0] + _currentX, args[1] + _currentY);
      _previousCommand = 'l'; //subsequent coordinates will be treated as implicit relative lineTo commands
      break;
    case 'L': //line to, absolute
      readFloats(buffer, offset, bufferLength, 2, args);
      lineTo(args[0], args[1]);
      _previousCommand = 'L';
      break;
    case 'l': //line to, relative
      readFloats(buffer, offset, bufferLength, 2, args);
      lineTo(args[0] + _currentX, args[1] + _currentY);
      _previousCommand = 'l';
      break;
    case 'H': //horizontal line to, absolute
      readFloats(buffer, offset, bufferLength, 1, args);
      horizontalLineTo(args[0]);
      _previousCommand = 'H';
      break;
    case 'h': //horizontal line to, relative
      readFloats(buffer, offset, bufferLength, 1, args);
      horizontalLineTo(args[0] + _currentX);
      _previousCommand = 'h';
      break;
    case 'V': //vertical line to, absolute
      readFloats(buffer, offset, bufferLength, 1, args);
      verticalLineTo(args[0]);
      _previousCommand = 'V';
      break;
    case 'v': //vertical line to, relative
      readFloats(buffer, offset, bufferLength, 1, args);
      verticalLineTo(args[0] + _currentY);
      _previousCommand = 'v';
      break;
    case 'C': //cubic bezier curve to, absolute
      readFloats(buffer, offset, bufferLength, 6, args);
      cubicBezierCurveTo(args[0], args[1], args[2], args[3], args[4], args[5]);
      _previousCommand = 'C';
      break;
    case 'c': //cubic bezier curve to, relative
      readFloats(buffer, offset, bufferLength, 6, args);
      cubicBezierCurveTo(args[0] + _currentX, args[1] + _currentY, args[2] + _currentX, args[3] + _currentY, args[4] + _currentX, args[5] + _currentY);
      _previousCommand = 'c';
      break;
    case 'S': //smooth cubic bezier curve to, absolute
      readFloats(buffer, offset, bufferLength, 4, args);
      if (_previousCommand == 'S' || _previousCommand == 's' || _previousCommand == 'C' || _previousCommand == 'c')
      {
        cubicBezierCurveTo(_currentX - (_previousCommandControlPointX - _currentX), _currentY - (_previousCommandControlPointY - _currentY), args[0], args[1], args[2], args[3]);
      }
      else {
        cubicBezierCurveTo(_currentX, _currentY, args[0], args[1], args[2], args[3]);
      }
      _previousCommand = 'S';
      break;
    case 's': //smooth cubic bezier curve to, relative
      readFloats(buffer, offset, bufferLength, 4, args);
      if (_previousCommand == 'S' || _previousCommand == 's' || _previousCommand == 'C' || _previousCommand == 'c')
      {
        cubicBezierCurveTo(_currentX - (_previousCommandControlPointX - _currentX), _currentY - (_previousCommandControlPointY - _currentY), args[0] + _currentX, args[1] + _currentY, args[2] + _currentX, args[3] + _currentY);
      }
      else {
        cubicBezierCurveTo(_currentX, _currentY, args[0] + _currentX, args[1] + _currentY, args[2] + _currentX, args[3] + _currentY);
      }
      _previousCommand = 's';
      break;
    case 'Q': //quadratic bezier curve to, absolute
      readFloats(buffer, offset, bufferLength, 4, args);
      quadraticBezierCurveTo(args[0], args[1], args[2], args[3]);
      _previousCommand = 'Q';
      break;
    case 'q': //quadratic bezier curve to, relative
      readFloats(buffer, offset, bufferLength, 4, args);
      quadraticBezierCurveTo(args[0] + _currentX, args[1] + _currentY, args[2] + _currentX, args[3] + _currentY);
      _previousCommand = 'q';
      break;
    case 'T': //smooth quadratic bezier curve to, absolute
      readFloats(buffer, offset, bufferLength, 2, args);
      if (_previousCommand == 'Q' || _previousCommand == 'q' || _previousCommand == 'T' || _previousCommand == 't')
      {
        quadraticBezierCurveTo(_currentX - (_previousCommandControlPointX - _currentX), _currentY - (_previousCommandControlPointY - _currentY), args[0], args[1]);
      }
      else {
        quadraticBezierCurveTo(_currentX, _currentY, args[0], args[1]);
      }
      _previousCommand = 'T';
      break;
    case 't': //smooth quadratic bezier curve to, relative
      readFloats(buffer, offset, bufferLength, 2, args);
      if (_previousCommand == 'Q' || _previousCommand == 'q' || _previousCommand == 'T' || _previousCommand == 't')
      {
        quadraticBezierCurveTo(_currentX - (_previousCommandControlPointX - _currentX), _currentY - (_previousCommandControlPointY - _currentY), args[0] + _currentX, args[1] + _currentY);
      }
      else {
        quadraticBezierCurveTo(_currentX, _currentY, args[0] + _currentX, args[1] + _currentY);
      }
      _previousCommand = 't';
      break;
    case 'A': //elliptical arc, absolute
      readFloats(buffer, offset, bufferLength, 7, args);
      ellipticalArc(args[0], args[1], args[2], args[3] > 0.5, args[4] > 0.5, args[5], args[6]);
      _previousCommand = 'A';
      break;
    case 'a': //elliptical arc, relative
      readFloats(buffer, offset, bufferLength, 7, args);
      ellipticalArc(args[0], args[1], args[2], args[3] > 0.5, args[4] > 0.5, args[5] + _currentX, args[6] + _currentY);
      _previousCommand = 'a';
      break;
    case 'Z': //close path
      closePath();
      _previousCommand = 'Z';
      break;
    case 'z': //close path
      closePath();
      _previousCommand = 'z';
      break;
  }
  if (nextCharsAreNumeric(buffer, offset, bufferLength)) {
    processPathChar(_previousCommand, buffer, offset, bufferLength);
  }
}

bool nextCharsAreNumeric(const uint8_t* buffer, size_t &offset, size_t bufferLength) {
  skipSeparators(buffer, offset, bufferLength);
  if (offset > bufferLength - 1) return false;
  char nextChar = (char)buffer[offset];
  switch (nextChar) {
    case '0' ... '9':
    case '.':
    case '-':
      return true;
  }
  return false;
}
void readFloats(const uint8_t* buffer, size_t &offset, size_t bufferLength, uint8_t howMany, float *floats) {
  for (size_t i = 0; i < howMany; i++)
  {
    skipSeparators(buffer, offset, bufferLength);
    floats[i] = parseFloat(buffer, offset, bufferLength);
  }
}
float parseFloat(const uint8_t* buffer, size_t &offset, size_t bufferLength) {
  String toParse = "";
  while (offset <= bufferLength - 1)
  {
    char thisChar = (char)buffer[offset];
    switch (thisChar)
    {
      case '0' ... '9':
      case '.':
      case '-':
        toParse += thisChar;
        offset++;
        break;
      default:
        return toParse.toFloat();
    }
  }
  return 0;
}

void skipSeparators(const uint8_t* buffer, size_t &offset, size_t bufferLength) {
  while (offset <= bufferLength - 1) {
    switch ((char)buffer[offset])
    {
      case '0' ... '9':
      case '.':
      case '-':
      case 'M':
      case 'm':
      case 'L':
      case 'l':
      case 'H':
      case 'h':
      case 'V':
      case 'v':
      case 'C':
      case 'c':
      case 'Q':
      case 'q':
      case 'S':
      case 's':
      case 'T':
      case 't':
      case 'A':
      case 'a':
      case 'Z':
      case 'z':
        return;
      default:
        offset++;
        break;
    }
  }
}

void moveTo(float x, float y) {
  _figureStartX = x;
  _figureStartY = y;
  if (distance(_currentX, _currentY, x, y) >= MIN_DISTANCE)
  {
    insertDrawPoint(x, y, false);
  }
  _currentX = x;
  _currentY = y;
}

void lineTo(float x, float y) {
  if (distance(_currentX, _currentY, x, y) >= MIN_DISTANCE)
  {
    insertDrawPoint(x, y, true);
  }
  _currentX = x;
  _currentY = y;
}

void serialPrint(String string) {
  Serial.print(string);
  delay(SERIAL_WRITE_DELAY_MILLIS);
}
void serialPrintln(String string) {
  Serial.println(string);
  delay(SERIAL_WRITE_DELAY_MILLIS);
}

void horizontalLineTo(float x)
{
  lineTo(x, _currentY);
}

void verticalLineTo(float y) {
  lineTo(_currentX, y);
}

float distance(float x1, float y1, float x2, float y2)
{
  float dx = x2 - x1;
  float dy = y2 - y1;
  return fabs(sqrtf(dx * dx + dy * dy));
}
void cubicBezierCurveTo(float x1, float y1, float x2, float y2, float x, float y) {

  SVGCurveLib::PointGeneric<> curPoint = {_currentX, _currentY};
  for (unsigned int i = 0; i <= INTERPOLATION_STEPS; i++)
  {
    SVGCurveLib::PointGeneric<> nextPoint  = SVGCurveLib::PointOnCubicBezierCurve(curPoint, {x1, y1}, {x2, y2}, {x, y}, (float)i / (float)INTERPOLATION_STEPS);
    lineTo(nextPoint.x, nextPoint.y);
  }
  lineTo(x, y);
  _previousCommandControlPointX = x2;
  _previousCommandControlPointY = y2;
}

void quadraticBezierCurveTo(float x1, float y1, float x, float y) {
  SVGCurveLib::PointGeneric<> curPoint = {_currentX, _currentY};
  for (unsigned int i = 0; i <= INTERPOLATION_STEPS; i++)
  {
    SVGCurveLib::PointGeneric<> nextPoint  = SVGCurveLib::PointOnQuadraticBezierCurve(curPoint, {x1, y1}, {x, y}, (float)i / (float)INTERPOLATION_STEPS);
    lineTo(nextPoint.x, nextPoint.y);
  }
  lineTo(x, y);
  _previousCommandControlPointX = x1;
  _previousCommandControlPointY = y1;
}

void ellipticalArc(float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepDirectionFlag, float x, float y) {
  SVGCurveLib::PointGeneric<> curPoint = {_currentX, _currentY};
  for (unsigned int i = 0; i <= INTERPOLATION_STEPS; i++)
  {
    SVGCurveLib::PointGeneric<> nextPoint  = SVGCurveLib::PointOnEllipticalArc(curPoint, rx, ry, xAxisRotation, largeArcFlag, sweepDirectionFlag, {x, y}, (float)i / (float)INTERPOLATION_STEPS);
    lineTo(nextPoint.x, nextPoint.y);
  }
  lineTo(x, y);
}

void closePath() {
  lineTo(_figureStartX, _figureStartY);
  _figureStartX = 0;
  _figureStartY = 0;
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

void beamTo(uint16_t x, uint16_t y) {
  writePointToDACs(x, y);
}

void beamToWithInterpolation(uint16_t x, uint16_t y) {
  if (_beamOn);
  {
    uint16_t startX = _xDAC;
    uint16_t startY = _yDAC;
    int16_t dx = x - startX;
    int16_t dy = y - startY;
    float distance = fabs(sqrtf((dx * dx) + (dy * dy)));
    //    for (float i = 0; i <= distance; i += 0.1) {
    for (float i = 0; i <= distance; i ++) {
      uint16_t nextX = startX + (dx / distance) * i;
      uint16_t nextY = startY + (dy / distance) * i;
      writePointToDACs(nextX, nextY);
    }
  }
  writePointToDACs(x, y);
}
void insertDrawPoint(float x, float y, bool beamOn) {
  if (_drawPointsBackBufferLength + 3 >= DRAW_POINTS_BUFFER_SIZE)
  {
    return;
  }
  float pctX = (x - _viewBoxLeft) / _viewBoxWidth;
  float pctY = (y - _viewBoxTop) / _viewBoxHeight;

  if (pctX < 0.0 || pctX > 1.0 || pctY < 0.0 || pctY > 1.0)
  {
    beamOn = false;
  }
  int16_t xDAC = (int16_t)(_invertX ? MAX_DAC_VALUE - (pctX * MAX_DAC_VALUE) : pctX * MAX_DAC_VALUE);
  int16_t yDAC = (int16_t)(_invertY ? MAX_DAC_VALUE - (pctY * MAX_DAC_VALUE) : pctY * MAX_DAC_VALUE);
  if (xDAC < 0) {
    xDAC = 0;
  }
  else if (xDAC > MAX_DAC_VALUE) {
    xDAC = MAX_DAC_VALUE;
  }

  if (yDAC < 0) {
    yDAC = 0;
  }
  else if (yDAC > MAX_DAC_VALUE) {
    yDAC = MAX_DAC_VALUE;
  }

  uint32_t xBits = ((((uint32_t)xDAC / (uint32_t)2) & (uint32_t)0x7FF) << 12);
  uint32_t yBits = ((((uint32_t)yDAC / (uint32_t)2) & (uint32_t)0x7FF) << 1);
  uint32_t zBits = (uint32_t)(beamOn ? 1 : 0);
  uint32_t xyzCombined =  xBits | yBits | zBits;
  uint32_t prevXyzCombined = 0;
  if (_drawPointsBackBufferLength >= 3)
  {
    prevXyzCombined =
      ((uint32_t)_drawPointsBackBuffer[_drawPointsBackBufferLength - 3] << 16) |
      ((uint32_t)_drawPointsBackBuffer[_drawPointsBackBufferLength - 2] << 8) |
      ((uint32_t)_drawPointsBackBuffer[_drawPointsBackBufferLength - 1]);
  }
  if ((_drawPointsBackBufferLength < 3) || (_drawPointsBackBufferLength >= 3 && xyzCombined != prevXyzCombined))
  {
    _drawPointsBackBuffer[_drawPointsBackBufferLength++] = ((xyzCombined & 0xFF0000) >> 16);
    _drawPointsBackBuffer[_drawPointsBackBufferLength++] = ((xyzCombined & 0xFF00) >> 8);
    _drawPointsBackBuffer[_drawPointsBackBufferLength++] = (xyzCombined & 0xFF);
  }
}
void writePointToDACs(uint16_t xDAC, uint16_t yDAC) {
  if (_xDAC == xDAC && _yDAC == yDAC) return;
  analogWrite(X_PIN, xDAC);
  analogWrite(Y_PIN, yDAC);
  _xDAC = xDAC;
  _yDAC = yDAC;
}

void beamOff() {
  if (!_beamOn) return;
  digitalWrite(Z_PIN, HIGH);
  _beamOn = false;
}

void beamOn() {
  if (_beamOn) return;
  digitalWrite(Z_PIN, LOW);
  _beamOn = true;
}
