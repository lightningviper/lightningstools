//pin assignments
const int X_PIN = A21;
const int Y_PIN = A22;
const int Z_PIN = 39;
const int BUILTIN_LED_PIN = 13;

//DAC precision & scale
const unsigned int DAC_PRECISION_BITS = 12;
const unsigned int MAX_DAC_VALUE = 0xFFF;
const unsigned int CENTER_DAC_VALUE = 0x800;
const unsigned int MAX_STEPS_LONGEST_DIAGONAL = 195;

//communications settings
const unsigned int BAUD_RATE = 115200;
const unsigned int RECEIVE_BUFFER_SIZE = 20 * 1024;

//timings
const unsigned int SERIAL_READ_TIMEOUT_MILLIS = 200;
const unsigned int SERIAL_WRITE_DELAY_MILLIS = 10;
const unsigned int BEAM_TURNON_SETTLING_TIME_MICROSECONDS = 0;
const unsigned int BEAM_TURNOFF_SETTLING_TIME_MICROSECONDS = 0;
const unsigned int BEAM_MOVEMENT_SETTLING_TIME_MICROSECONDS = 0;

//firmware version
const unsigned int FIRMWARE_MAJOR_VERSION = 0;
const unsigned int FIRMWARE_MINOR_VERSION = 1;
const String FIRMWARE_IDENTIFICATION = "Teensy RWR v" + String(FIRMWARE_MAJOR_VERSION) + "." + String(FIRMWARE_MINOR_VERSION);

#define CPU_RESTART_ADDR (uint32_t *)0xE000ED0C
#define CPU_RESTART_VAL 0x5FA0004
#define CPU_RESTART (*CPU_RESTART_ADDR = CPU_RESTART_VAL);

typedef struct {
  unsigned short X = 0;
  unsigned short Y = 0;
}
Point;

typedef enum {
  TRACE = 0,
  INFO,
  WARNING,
  ERROR
}
DebugLevel;

const Point CENTER;

const DebugLevel _debugLevel = TRACE;

Point _beamLocation;
bool _beamOn;
String _inputString = "";
String _drawCommands = "";
bool _ledOn = false;
bool _echoBackEnabled = false;
bool _debugLoggingEnabled = false;
bool _beamAutoCenteringEnabled = false;

void setup() {
  Serial.setTimeout(SERIAL_READ_TIMEOUT_MILLIS);
  Serial.begin(BAUD_RATE);
  debugLog("setup() entered");
  _inputString.reserve(RECEIVE_BUFFER_SIZE);
  _drawCommands.reserve(RECEIVE_BUFFER_SIZE);
  analogWriteResolution(DAC_PRECISION_BITS);
  pinMode(Z_PIN, OUTPUT);
  pinMode(BUILTIN_LED_PIN, OUTPUT);
  beamOff();
  beamTo(CENTER);
  debugLog("setup() exited");
}

void loop() {
  debugLog("loop() entered", TRACE);
  processSerialData();
  draw();
  debugLog("loop() exited", TRACE);
}
void processSerialData() {
  debugLog("serialEvent() entered", TRACE);
  unsigned int bytesRead = 0;

  while (Serial.available() > 0 && bytesRead < 256) {
    int inChar = Serial.read();
    bytesRead++;
    if (inChar != -1) {
      debugLog("serial character entered: ", TRACE);

      if (_echoBackEnabled) {
        serialPrint((char) inChar);
      }

      switch ((char) inChar) {
        case 'c':
        case 'C': //toggle beam auto-centering enable/disable
          {
            toggleBeamAutoCenteringEnabled();
            break;
          }

        case 'd':
        case 'D': //toggle debug logging enable/disable
          {
            toggleDebugLoggingEnabled();
            break;
          }

        case 'e':
        case 'E': //toggle echo-back enable/disable
          {
            toggleEchobackEnabled();
            break;
          }

        case 'i':
        case 'I': //identify firmware
          {
            identify();
            break;
          }

        case 'r':
        case 'R': //reboot the device and restart the program
          {
            CPU_RESTART;
            break;
          }

        case 'm':
        case 'M':
        case 'l':
        case 'L':
        case ',':
        case ' ':
        case '0'...
            '9':
          _inputString += (char) inChar;
          debugLog("appending char to _inputString:" + String((char) inChar), TRACE);
          break;

        case 'z':
        case 'Z': //end of command-list, render
          {
            toggleLED();
            _drawCommands = _inputString;
            _inputString = "";
            _inputString.reserve(RECEIVE_BUFFER_SIZE);
            break;
          }
      }
    }
  }
  debugLog("serialEvent() exited", TRACE);
}

void toggleBeamAutoCenteringEnabled() {
  debugLog("toggleBeamAutoCenteringEnabled() entered");
  _beamAutoCenteringEnabled = !_beamAutoCenteringEnabled;
  debugLog("toggleBeamAutoCenteringEnabled() exited");
}

void toggleDebugLoggingEnabled() {
  debugLog("toggleDebugLoggingEnabled() entered");
  _debugLoggingEnabled = !_debugLoggingEnabled;
  debugLog("toggleDebugLoggingEnabled() exited");
}

void toggleEchobackEnabled() {
  debugLog("toggleEchobackEnabled() entered");
  _echoBackEnabled = !_echoBackEnabled;
  debugLog("toggleEchobackEnabled() exited");
}

void toggleLED() {
  debugLog("toggleLED() entered");
  _ledOn = !_ledOn;
  digitalWrite(BUILTIN_LED_PIN, _ledOn ? HIGH : LOW);
  debugLog("toggleLED() exited");
}

void draw() {
  debugLog("draw() entered", TRACE);
  Point point;
  unsigned int numCommandsProcessed = 0;

  debugLog("draw commands:", TRACE);
  debugLog("-------------------------------------------", TRACE);
  debugLog(_drawCommands, TRACE);
  debugLog("-------------------------------------------", TRACE);

  unsigned int index = 0;
  for (unsigned int i = 0; i < _drawCommands.length(); i++) {
    char curChar = _drawCommands.charAt(i);
    debugLog("draw() processing command char at index " + String(i) + "(" + String(curChar) + ")", TRACE);

    switch (curChar) {

      case 'm':
      case 'M': //move to point
        index = i;
        point = readPoint(_drawCommands, index);
        i = --index;
        moveTo(point);
        numCommandsProcessed++;
        break;

      case 'l':
      case 'L': //draw line to point
        index = i;
        point = readPoint(_drawCommands, index);
        lineTo(point);
        numCommandsProcessed++;
        i = --index;
        break;
    }
    processSerialData();
  }

  beamOff();

  if (_beamAutoCenteringEnabled) {
    autoCenterBeam();
  }

  debugLog("# commands processed: " + String(numCommandsProcessed), TRACE);
  debugLog("draw() exited", TRACE);
}

Point readPoint(String string, unsigned int & index) {
  debugLog("readPoint(string, unsigned int &index) entered", TRACE);
  Point point;
  point.X = readClampedUnsignedInt(string, index, MAX_DAC_VALUE, CENTER_DAC_VALUE);
  point.Y = readClampedUnsignedInt(string, index, MAX_DAC_VALUE, CENTER_DAC_VALUE);

  debugLog("readPoint(string, unsigned int &index) exited", TRACE);
  return point;
}

unsigned int readClampedUnsignedInt(String string, unsigned int &index, unsigned int maxValue, unsigned int defaultValue)
{
  debugLog("readClampedUnsignedInt(string, unsigned int &index, unsigned int maxValue, unsigned int defaultValue) entered");
  String toParse = "";
  for (unsigned int i = index; i < string.length(); i++)
  {
    index++;
    char thisChar = string.charAt(i);
    if (thisChar == '0' || thisChar == '1' || thisChar == '2' || thisChar == '3' || thisChar == '4' || thisChar == '5' || thisChar == '6' || thisChar == '7' || thisChar == '8' || thisChar == '9' || thisChar == '-' || thisChar == '.')
    {
      toParse += thisChar;
    }
    else if (toParse.length() > 0)
    {
      break;
    }
  }

  unsigned int parsed = defaultValue;
  if (toParse.length() > 0)
  {
    int asInt = toParse.toInt();;
    parsed = asInt >= 0 ? (unsigned int) asInt : 0;
    parsed = parsed <= maxValue ? parsed : maxValue;
  }
  debugLog("readClampedUnsignedInt(string, unsigned int &index, unsigned int maxValue, unsigned int defaultValue) exited with return value:" + String(parsed));
  return parsed;
}


void moveTo(Point point) {
  debugLog("moveTo(point) entered - Point.X=" + String(point.X, 4) + "; Point.Y=" + String(point.Y, 4));
  beamOff();
  beamTo(point);
  debugLog("moveTo(point) exited");
}

void lineTo(Point point) {
  debugLog("lineTo(point) entered - Point.X=" + String(point.X, 4) + "; Point.Y=" + String(point.Y, 4));
  beamOn();
  beamTo(point);
  debugLog("lineTo(point) exited");
}

void beamTo(Point point) {
  debugLog("beamTo(point) entered - Point.X=" + String(point.X, 4) + "; Point.Y=" + String(point.Y, 4));
  if (_beamOn) {
    const double maxDistance = sqrt((unsigned long)2 * (unsigned long)MAX_DAC_VALUE * (unsigned long)MAX_DAC_VALUE);
    double dx = point.X - _beamLocation.X;
    double dy = point.Y - _beamLocation.Y;
    double distance = abs(sqrt((dx * dx) + (dy * dy)));
    unsigned int numSteps = (unsigned int)((distance / maxDistance) * MAX_STEPS_LONGEST_DIAGONAL);
    debugLog("numSteps=" + String(numSteps));

    for (unsigned int i = 0; i < numSteps; i++) {
      Point nextPoint;
      nextPoint.X = _beamLocation.X + (((double) dx / (double) numSteps));
      nextPoint.Y = _beamLocation.Y + (((double) dy / (double) numSteps));
      debugLog("next Point X=" + String(nextPoint.X, 4) + "; Y=" + String(nextPoint.Y, 4));
      writePointToDACs(nextPoint);
      processSerialData();
    }
  }
  writePointToDACs(point);
  debugLog("beamTo(point) exited");
}

void writePointToDACs(Point point) {
  debugLog("writePointToDACs(point) entered - Point.X=" + String(point.X, 4) + "; Point.Y=" + String(point.Y, 4), TRACE);

  if (_beamLocation.X != point.X || _beamLocation.Y != point.Y) {
    debugLog("Writing to DACs: point.X=" + String(point.X) + "; point.Y=" + String(point.Y) + "");
    analogWrite(X_PIN, point.X);
    analogWrite(Y_PIN, point.Y);
    if (BEAM_MOVEMENT_SETTLING_TIME_MICROSECONDS > 0) {
      delayMicroseconds(BEAM_MOVEMENT_SETTLING_TIME_MICROSECONDS);
    }
    _beamLocation = point;
  } else {
    debugLog("Skipped writing to DACs: point.X=" + String(point.X) + "; point.Y=" + String(point.Y) + "; beam is already at that location");
  }

  debugLog("writePointToDACs(point) exited", TRACE);
}

void beamOff() {
  debugLog("beamOff() entered");
  if (_beamOn) {
    digitalWrite(Z_PIN, HIGH);
    _beamOn = false;
    if (BEAM_TURNOFF_SETTLING_TIME_MICROSECONDS > 0) {
      delayMicroseconds(BEAM_TURNOFF_SETTLING_TIME_MICROSECONDS);
    }
  }
  debugLog("beamOff() exited");
}

void beamOn() {
  debugLog("beamOn() entered");
  if (!_beamOn) {
    digitalWrite(Z_PIN, LOW);
    _beamOn = true;
    if (BEAM_TURNON_SETTLING_TIME_MICROSECONDS) {
      delayMicroseconds(BEAM_TURNON_SETTLING_TIME_MICROSECONDS);
    }
  }
  debugLog("beamOn() exited");
}

void autoCenterBeam() {
  debugLog("autoCenterBeam() entered");
  if (_beamAutoCenteringEnabled) {
    beamTo(CENTER);
  }
  debugLog("autoCenterBeam() exited");
}

void identify() {
  debugLog("identify() entered");
  serialPrintln(FIRMWARE_IDENTIFICATION);
  debugLog("identify() entered");
}

void serialPrint(String message) {
  Serial.print(message);
  delay(SERIAL_WRITE_DELAY_MILLIS);
}

void serialPrintln(String message) {
  serialPrint(message + '\r' + '\n');
}

void debugLog(String message) {
  debugLog(message, INFO);
}

void debugLog(String message, DebugLevel level) {
  if (_debugLoggingEnabled && level >= _debugLevel) {
    serialPrintln(String(micros()) + ":" + message);
  }
}
