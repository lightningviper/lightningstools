#include <util/atomic.h> 

const int X_PIN = A21;
const int Y_PIN = A22;
const int Z_PIN = A14;
const int BUILTIN_LED_PIN = 13;

//DAC precision & scale
const unsigned int DAC_PRECISION_BITS = 12;
const uint16_t MAX_DAC_VALUE = 0xFFF;

//communication settings
const unsigned int BAUD_RATE = 115200;
const unsigned int SERIAL_READ_TIMEOUT_MILLIS = 0;
const size_t RECEIVE_BUFFER_SIZE = 32 * 1024;

//drawing settings
const size_t DRAW_POINTS_BUFFER_SIZE = 15 * 1024;
const unsigned int INTERPOLATION_STEPS = 25;
const unsigned int REFRESH_RATE_HZ = 60;

volatile uint32_t _drawPointsBuffer1[DRAW_POINTS_BUFFER_SIZE];
volatile uint32_t _drawPointsBuffer2[DRAW_POINTS_BUFFER_SIZE];
volatile uint32_t *_pDrawPointsBackBuffer=_drawPointsBuffer1;
volatile uint32_t *_pDrawPointsFrontBuffer=_drawPointsBuffer2;
volatile size_t _drawPointsBackBufferLength = 0;
volatile size_t _drawPointsFrontBufferLength = 0;

uint8_t _receiveBuffer[RECEIVE_BUFFER_SIZE];
size_t _receiveBufferLength = 0;

IntervalTimer _drawTimer;

//parser state
float _currentX = 0;
float _currentY = 0;
float _figureStartX = 0;
float _figureStartY = 0;
char _previousCommand = 'z';
float _previousCommandControlPointX = 0;
float _previousCommandControlPointY = 0;

//viewBox and inversion settings
float _viewBoxLeft = 0.00;
float _viewBoxTop = 0.00;
float _viewBoxWidth = MAX_DAC_VALUE;
float _viewBoxHeight = MAX_DAC_VALUE;
bool _invertX = false;
bool _invertY = true;

//draw-point interpolation state
int16_t _previousDrawPointXDAC = 0;
int16_t _previousDrawPointYDAC = 0;
bool _prevBeamOn = false;
uint16_t _currentBeamLocationXDAC = 0;
uint16_t _currentBeamLocationYDAC = 0;
bool _beamOn = false;
bool _ledOn = false;

//firmware version
const unsigned int FIRMWARE_MAJOR_VERSION = 0;
const unsigned int FIRMWARE_MINOR_VERSION = 1;
const String FIRMWARE_IDENTIFICATION = "Teensy RWR v" + String(FIRMWARE_MAJOR_VERSION) + "." + String(FIRMWARE_MINOR_VERSION);

#define CPU_RESTART_ADDR (uint32_t *)0xE000ED0C
#define CPU_RESTART_VAL 0x5FA0004
#define CPU_RESTART (*CPU_RESTART_ADDR = CPU_RESTART_VAL);


void setup()
{
  pinMode(Z_PIN, OUTPUT);
  pinMode(BUILTIN_LED_PIN, OUTPUT);
  analogWriteResolution(DAC_PRECISION_BITS);
  LEDOn();
  _drawTimer.priority(0);
  _drawTimer.begin(draw, 1 * 1000 * 1000 / REFRESH_RATE_HZ);
}

void loop()
{
  int result = readIncomingSerialData();
  if (result == 0)
  {
    processFrame(_receiveBuffer, _receiveBufferLength);
    discardReceiveBuffer();
  }
}

int readIncomingSerialData()
{
  while (Serial.available()>0) 
  {
    int bytesAvailable = Serial.available();
    for (int i=0;i<bytesAvailable;i++)
    {
      if (_receiveBufferLength +1 >= RECEIVE_BUFFER_SIZE)
      {
        return -1;
      }
      int thisByte = Serial.read();
      if (thisByte <= 0)
      {
        return thisByte;
      }
      _receiveBuffer[_receiveBufferLength++] = thisByte;
    }
  }
  return -1;
}

void processFrame(const uint8_t* buffer, size_t size)
{
  toggleLED();
  discardBackBuffer();
  parse(buffer, size);
  swap();
}

void swap() 
{
   ATOMIC_BLOCK(ATOMIC_RESTORESTATE)
   {
     volatile uint32_t* pTemp=_pDrawPointsFrontBuffer;
    _pDrawPointsFrontBuffer = _pDrawPointsBackBuffer;
    _pDrawPointsBackBuffer=pTemp;
    _drawPointsFrontBufferLength = _drawPointsBackBufferLength;
    discardBackBuffer();
   }
}
void discardBackBuffer()
{
  _drawPointsBackBufferLength = 0;
}

void discardReceiveBuffer()
{
  _receiveBufferLength = 0;
}

void draw()
{
   ATOMIC_BLOCK(ATOMIC_RESTORESTATE)
   {
      if (_drawPointsFrontBufferLength < 1)
      {
        return;
      }
      for (size_t i = 0; i < _drawPointsFrontBufferLength; i++)
      {
        uint32_t xyzCombined = _pDrawPointsFrontBuffer[i];
    
        uint16_t xDAC = ((xyzCombined & (uint32_t)0xFFF000) >> 12);
        uint16_t yDAC = (xyzCombined & (uint32_t)0xFFF);
        bool beamOnFlag = (xyzCombined & (uint32_t)0x1000000) == (uint32_t)0x1000000;
        beamOnFlag ? beamOn() : beamOff();
        beamTo(xDAC, yDAC);
      }
      beamOff();
      beamTo(0, MAX_DAC_VALUE);
   }
}

void parse(const uint8_t* buffer, size_t size)
{
  if (size == 0)
  {
    return;
  }
  size_t offset = 0;
  while (offset < size - 1)
  {
    skipSeparators(buffer, offset, size);
    if (offset > size - 1)
    {
      break;
    }
    char nextChar = buffer[offset++];
    if (nextChar > 0) {
      processPathChar(nextChar, buffer, offset, size);
    }
  }
}

void echo(char charToEcho)
{
  Serial.write(charToEcho);
}

void processPathChar(char pathChar, const uint8_t* buffer, size_t &offset, size_t bufferLength)
{
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
      else
      {
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
      else
      {
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
      else
      {
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
      else
      {
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
  if (nextCharIsNumeric(buffer, offset, bufferLength))
  {
    processPathChar(_previousCommand, buffer, offset, bufferLength);
  }
}

bool nextCharIsNumeric(const uint8_t* buffer, size_t &offset, size_t bufferLength)
{
  skipSeparators(buffer, offset, bufferLength);
  if (offset > bufferLength - 1)
  {
    return false;
  }
  char nextChar = (char)buffer[offset];
  switch (nextChar)
  {
    case '0' ... '9':
    case '.':
    case '-':
      return true;
  }
  return false;
}

void readFloats(const uint8_t* buffer, size_t &offset, size_t bufferLength, uint8_t howMany, float * floats)
{
  for (size_t i = 0; i < howMany; i++)
  {
    skipSeparators(buffer, offset, bufferLength);
    floats[i] = parseFloat(buffer, offset, bufferLength);
  }
}

float parseFloat(const uint8_t* buffer, size_t &offset, size_t bufferLength)
{
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
  return toParse.toFloat();
}

void skipSeparators(const uint8_t* buffer, size_t &offset, size_t bufferLength)
{
  while (offset <= bufferLength - 1)
  {
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

void moveTo(float x, float y)
{
  _figureStartX = x;
  _figureStartY = y;
  insertInterpolatedDrawPoints(x, y, false);
  _currentX = x;
  _currentY = y;
}

void lineTo(float x, float y)
{
  insertInterpolatedDrawPoints(x, y, true);
  _currentX = x;
  _currentY = y;
}

void horizontalLineTo(float x)
{
  lineTo(x, _currentY);
}

void verticalLineTo(float y)
{
  lineTo(_currentX, y);
}

float distance(float x1, float y1, float x2, float y2)
{
  float dx = x2 - x1;
  float dy = y2 - y1;
  return fabs(sqrtf(dx * dx + dy * dy));
}

void cubicBezierCurveTo(float x1, float y1, float x2, float y2, float x, float y)
{
  float startX = _currentX;
  float startY = _currentY;
  for (unsigned int i = 0; i < INTERPOLATION_STEPS; i++)
  {
    float nextPointX = 0;
    float nextPointY = 0;
    pointOnCubicBezierCurve(startX, startY, x1, y1, x2, y2, x, y, (float)i / (float)(INTERPOLATION_STEPS - 1), &nextPointX, &nextPointY);
    lineTo(nextPointX, nextPointY);
  }
  lineTo(x, y);
  _previousCommandControlPointX = x2;
  _previousCommandControlPointY = y2;
}

void quadraticBezierCurveTo(float x1, float y1, float x, float y)
{
  float startX = _currentX;
  float startY = _currentY;
  for (unsigned int i = 0; i < INTERPOLATION_STEPS; i++)
  {
    float nextPointX = 0;
    float nextPointY = 0;
    pointOnQuadraticBezierCurve(startX, startY, x1, y1, x, y, (float)i / (float)(INTERPOLATION_STEPS - 1), &nextPointX, &nextPointY);
    lineTo(nextPointX, nextPointY);
  }
  lineTo(x, y);
  _previousCommandControlPointX = x1;
  _previousCommandControlPointY = y1;
}

void ellipticalArc(float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepDirectionFlag, float x, float y)
{
  float startX = _currentX;
  float startY = _currentY;
  for (unsigned int i = 0; i < INTERPOLATION_STEPS; i++)
  {
    float nextPointX = 0;
    float nextPointY = 0;
    pointOnEllipticalArc(startX, startY, rx, ry, xAxisRotation, largeArcFlag, sweepDirectionFlag, x, y, (float)i / (float)(INTERPOLATION_STEPS - 1), &nextPointX, &nextPointY);
    lineTo(nextPointX, nextPointY);
  }
  lineTo(x, y);
}

void closePath()
{
  lineTo(_figureStartX, _figureStartY);
  _figureStartX = 0;
  _figureStartY = 0;
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
  writePointToDACs(x, y);
}

void insertInterpolatedDrawPoints(float x, float y, bool beamOn)
{
  if (_drawPointsBackBufferLength + 1 > DRAW_POINTS_BUFFER_SIZE)
  {
    return;
  }

  float pctX = (x - _viewBoxLeft) / _viewBoxWidth;
  float pctY = (y - _viewBoxTop) / _viewBoxHeight;

  if (pctX < 0.0 || pctX > 1.0 || pctY < 0.0 || pctY > 1.0)
  {
    beamOn = false;
  }
  int16_t finalXDAC = roundf((_invertX ? MAX_DAC_VALUE - (pctX * MAX_DAC_VALUE) : pctX * MAX_DAC_VALUE));
  int16_t finalYDAC = roundf((_invertY ? MAX_DAC_VALUE - (pctY * MAX_DAC_VALUE) : pctY * MAX_DAC_VALUE));
  if (finalXDAC < 0)
  {
    finalXDAC = 0;
  }
  else if (finalXDAC > MAX_DAC_VALUE)
  {
    finalXDAC = MAX_DAC_VALUE;
  }

  if (finalYDAC < 0)
  {
    finalYDAC = 0;
  }
  else if (finalYDAC > MAX_DAC_VALUE)
  {
    finalYDAC = MAX_DAC_VALUE;
  }

  uint16_t startXDAC = _previousDrawPointXDAC;
  uint16_t startYDAC = _previousDrawPointYDAC;
  
  float xDistance = finalXDAC - startXDAC;
  float yDistance = finalYDAC - startYDAC;
  float euclideanDistance = fabs(sqrtf((xDistance * xDistance) + (yDistance * yDistance)));
  int32_t numSteps= beamOn? ceilf(euclideanDistance) : 1;
  float dx = xDistance / numSteps;
  float dy = yDistance / numSteps; 

  if (numSteps ==0) 
  {
    return;
  }
  for (int32_t i=1;i<=numSteps;i++)
  {
    if (_drawPointsBackBufferLength + 1 > DRAW_POINTS_BUFFER_SIZE)
    {
      return;
    }
    uint16_t xDAC = (uint16_t)(startXDAC + (dx * i));
    uint16_t yDAC = (uint16_t)(startYDAC + (dy * i));
    uint32_t zBit = (((uint32_t) (beamOn ? 1 : 0)) << 24);
    uint32_t xBits = ((xDAC & (uint32_t)0xFFF) << 12);
    uint32_t yBits = (yDAC & (uint32_t)0xFFF);
    uint32_t xyzCombined = xBits | yBits | zBit;


    if (_drawPointsBackBufferLength == 0 || (_drawPointsBackBufferLength > 0 && ((abs(_previousDrawPointXDAC - xDAC) >= 1) || (abs(_previousDrawPointYDAC - yDAC) >= 1))))
    {
      _pDrawPointsBackBuffer[_drawPointsBackBufferLength++] = xyzCombined;
      _previousDrawPointXDAC = xDAC;
      _previousDrawPointYDAC = yDAC;
    }
  }
  if ((_previousDrawPointXDAC != finalXDAC) || (_previousDrawPointYDAC != finalYDAC))
  {
      uint32_t zBit = (((uint32_t) (beamOn ? 1 : 0)) << 24);
      uint32_t xBits = ((finalXDAC & (uint32_t)0xFFF) << 12);
      uint32_t yBits = (finalYDAC & (uint32_t)0xFFF);
      uint32_t xyzCombined = xBits | yBits | zBit;
      _pDrawPointsBackBuffer[_drawPointsBackBufferLength++] = xyzCombined;
      _previousDrawPointXDAC = finalXDAC;
      _previousDrawPointYDAC = finalXDAC;
  }
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
  _beamOn = false;
}

void beamOn()
{
  if (_beamOn)
  {
    return;
  }
  digitalWrite(Z_PIN, LOW);
  _beamOn = true;
}






const float PI_OVER_180 = (PI / 180.0);

void pointOnLine(float p0x, float p0y, float p1x, float p1y, float t, float *outX, float *outY)
{
  *outX = calculateLinearLineParameter(p0x, p1x, t);
  *outY = calculateLinearLineParameter(p0y, p1y, t);
}

float calculateLinearLineParameter (float x0, float x1, float t)
{
  return x0 + (x1 - x0) * t;
};

void pointOnQuadraticBezierCurve(float p0x, float p0y, float p1x, float p1y, float p2x, float p2y, float t, float *outX, float *outY)
{
  *outX = calculateQuadraticBezierParameter(p0x, p1x, p2x, t);
  *outY = calculateQuadraticBezierParameter(p0y, p1y, p2y, t);
}

float calculateQuadraticBezierParameter (float x0, float x1, float x2, float t)
{
  return powf(1.0 - t, 2.0) * x0 + 2.0 * t * (1.0 - t) * x1 + powf(t, 2.0) * x2;
};

void pointOnCubicBezierCurve(float p0x, float p0y, float p1x, float p1y, float p2x, float p2y, float p3x, float p3y, float t, float *outX, float *outY)
{
  *outX = calculateCubicBezierParameter(p0x, p1x, p2x, p3x, t);
  *outY = calculateCubicBezierParameter(p0y, p1y, p2y, p3y, t);
}

float calculateCubicBezierParameter (float x0, float x1, float x2, float x3, float t)
{
  return (powf(1.0 - t, 3.0) * x0) + (3.0 * t * powf(1.0 - t, 2.0) * x1) + (3.0 * (1.0 - t) * powf(t, 2.0) * x2) + (powf(t, 3.0) * x3);
};

float toRadians(float angle)
{
  return angle * PI_OVER_180;
};

float angleBetween(float v0x, float v0y, float v1x, float v1y)
{
  float p = v0x * v1x + v0y * v1y;
  float n = sqrtf((powf(v0x, 2.0) + powf(v0y, 2.0)) * (powf(v1x, 2.0) + powf(v1y, 2.0)));
  float sign = v0x * v1y - v0y * v1x < 0 ? -1.0f : 1.0f;
  float angle = sign * acosf(p / n);
  return angle;
};

void pointOnEllipticalArc(float p0x, float p0y, float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepFlag, float p1x, float p1y, float t, float *outX, float *outY)
{
  rx = fabs(rx);
  ry = fabs(ry);
  xAxisRotation = fmod(xAxisRotation, 360.0f);
  float xAxisRotationRadians = toRadians(xAxisRotation);
  if (p0x == p1x && p0y == p1y)
  {
    *outX = p0x;
    *outY = p0y;
    return;
  }

  if (rx == 0 || ry == 0)
  {
    pointOnLine(p0x, p0y, p1x, p1y, t, outX, outY);
    return;
  }

  float dx = (p0x - p1x) / 2.0;
  float dy = (p0y - p1y) / 2.0;
  float transformedPointX = cosf(xAxisRotationRadians) * dx + sinf(xAxisRotationRadians) * dy;
  float transformedPointY = -sinf(xAxisRotationRadians) * dx + cosf(xAxisRotationRadians) * dy;

  float radiiCheck = powf(transformedPointX, 2.0) / powf(rx, 2.0) + powf(transformedPointY, 2.0) / powf(ry, 2.0);
  if (radiiCheck > 1)
  {
    rx = sqrtf(radiiCheck) * rx;
    ry = sqrtf(radiiCheck) * ry;
  }

  float cSquareNumerator = powf(rx, 2.0) * powf(ry, 2.0) - powf(rx, 2.0) * powf(transformedPointY, 2.0) - powf(ry, 2.0) * powf(transformedPointX, 2.0);
  float cSquareRootDenom = powf(rx, 2.0) * powf(transformedPointY, 2.0) + powf(ry, 2.0) * powf(transformedPointY, 2.0);
  float cRadicand = cSquareNumerator / cSquareRootDenom;

  cRadicand = cRadicand < 0 ? 0 : cRadicand;
  float cCoef = (largeArcFlag != sweepFlag ? 1 : -1) * sqrtf(cRadicand);
  float transformedCenterX = cCoef * ((rx * transformedPointY) / ry);
  float transformedCenterY = cCoef * (-(ry * transformedPointY) / rx);
  float centerX = cosf(xAxisRotationRadians) * transformedCenterX - sinf(xAxisRotationRadians) * transformedCenterY + ((p0x + p1x) / 2.0);
  float centerY = sinf(xAxisRotationRadians) * transformedCenterX + cosf(xAxisRotationRadians) * transformedCenterY + ((p0y + p1y) / 2.0);
  float startVectorX = (transformedPointX - transformedCenterY) / rx;
  float startVectorY = (transformedPointY - transformedCenterY) / ry;
  float startAngle = angleBetween(1.0f, 0.0f, startVectorX, startVectorY);
  float endVectorX = (-transformedPointX - transformedCenterX) / rx;
  float endVectorY = (-transformedPointY - transformedCenterY) / ry;
  float sweepAngle = angleBetween(startVectorX, startVectorY, endVectorX, endVectorY);

  if (!sweepFlag && sweepAngle > 0)
  {
    sweepAngle -= TWO_PI;
  }
  else if (sweepFlag && sweepAngle < 0)
  {
    sweepAngle += TWO_PI;
  }

  sweepAngle = fmod(sweepAngle, TWO_PI);

  float angle = startAngle + (sweepAngle * t);
  float ellipseComponentX = rx * cosf(angle);
  float ellipseComponentY = ry * sinf(angle);
  *outX = cosf(xAxisRotationRadians) * ellipseComponentX - sinf(xAxisRotationRadians) * ellipseComponentY + centerX;
  *outY = sinf(xAxisRotationRadians) * ellipseComponentX + cosf(xAxisRotationRadians) * ellipseComponentY + centerY;
}
