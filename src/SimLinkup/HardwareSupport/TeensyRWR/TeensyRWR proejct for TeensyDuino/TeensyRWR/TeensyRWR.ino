#include <Scheduler.h>
#include <Scheduler/Semaphore.h>

//pin assignments
const int X_PIN=A21;
const int Y_PIN=A22;
const int Z_PIN=39;
const int BUILTIN_LED_PIN=13;

//DAC precision & scale
const unsigned int DAC_PRECISION_BITS=12;
const unsigned int MAX_DAC_VALUE= 0xFFF;
const unsigned int CENTER_DAC_VALUE=0x800;

//communications settings
const unsigned int BAUD_RATE=9600;
const unsigned int RECEIVE_BUFFER_SIZE = 20*1024;

//timings
const unsigned int SERIAL_WRITE_DELAY_MILLIS=1;
const unsigned int BEAM_TURNON_SETTLING_TIME_MICROSECONDS=15;
const unsigned int BEAM_TURNOFF_SETTLING_TIME_MICROSECONDS=15;
const unsigned int BEAM_MOVEMENT_SETTLING_TIME_MICROSECONDS=50;

//firmware version
const unsigned int FIRMWARE_MAJOR_VERSION =0;
const unsigned int FIRMWARE_MINOR_VERSION =1;
const String FIRMWARE_IDENTIFICATION="Teensy RWR v" + String(FIRMWARE_MAJOR_VERSION) + "." + String(FIRMWARE_MINOR_VERSION);

#define CPU_RESTART_ADDR (uint32_t *)0xE000ED0C
#define CPU_RESTART_VAL 0x5FA0004
#define CPU_RESTART (*CPU_RESTART_ADDR = CPU_RESTART_VAL);

typedef struct 
{
  unsigned int X=CENTER_DAC_VALUE;
  unsigned int Y=CENTER_DAC_VALUE;
} Point;

const Point CENTER;


Point _beamLocation;
bool _beamOn;
String _inputString="";
String _drawCommands="";
bool _ledOn=false;
bool _emitArduinoSerialPlotterInfo=false;
bool _echoBackEnabled=false;
bool _debugLoggingEnabled=false;
bool _beamAutoCenteringEnabled=false;

void setup() 
{
  Serial.begin(BAUD_RATE); 
  debugLog("setup() entered");
  _inputString.reserve(RECEIVE_BUFFER_SIZE );
  analogWriteResolution(DAC_PRECISION_BITS); 
  pinMode(Z_PIN, OUTPUT);
  pinMode(BUILTIN_LED_PIN, OUTPUT);
  beamOff();
  beamTo(CENTER);
  Scheduler.startLoop(drawLoop);
  Scheduler.startLoop(serialLoop);
  debugLog("setup() exited");
}

void loop() 
{
  yield();
}
void serialLoop() 
{
  debugLog("serialLoop() entered");
  while(Serial.available() > 0) 
  {
    int inChar=Serial.read();
    if (inChar != -1) 
    {
      toggleLED();
      if (_echoBackEnabled) 
      {
        serialPrint((char)inChar);
      }
      switch ((char)inChar) 
      {
        case 'c':case 'C': //toggle beam auto-centering enable/disable 
        {
          toggleBeamAutoCenteringEnabled();
          break;
        }
  
        case 'd':case 'D': //toggle debug logging enable/disable
        {
          toggleDebugLoggingEnabled();
          break;
        }
  
        case 'e':case 'E': //toggle echo-back enable/disable
        {
          toggleEchobackEnabled();
          break;
        }
  
        case 'i': case 'I': //identify firmware
        {
          identify();
          break;
        }
  
        case 'p': case 'P': //toggle emitting of Arduino serial plotter data 
        {
          toggleEmitArduinoSerialPlotterInfo();
          break;
        }
  
        case 'r': case 'R': //reboot the device and restart the program 
        {
          CPU_RESTART;
          break;
        }
  
        case 'm': case 'M':
        case 'l': case 'L':
        case ',':
        case ' ':
        case '-':
        case '.':
        case '0' ... '9':
          _inputString+=(char)inChar;
          break;
  
        case 'z': case 'Z': //end of command-list, render
        {
          _drawCommands=_inputString;
          _inputString="";
          _inputString.reserve(RECEIVE_BUFFER_SIZE);
          break;
        }
      }
    }
    yield();
  }
  debugLog("serialLoop() exited");
}

void toggleBeamAutoCenteringEnabled() 
{
  debugLog("toggleBeamAutoCenteringEnabled() entered");
  _beamAutoCenteringEnabled= !_beamAutoCenteringEnabled;
  debugLog("toggleBeamAutoCenteringEnabled() exited");
}

void toggleDebugLoggingEnabled() 
{
  debugLog("toggleDebugLoggingEnabled() entered");
  _debugLoggingEnabled= !_debugLoggingEnabled;
  debugLog("toggleDebugLoggingEnabled() exited");
}

void toggleEmitArduinoSerialPlotterInfo() {
  debugLog("toggleEmitArduinoSerialPlotterInfo() entered");
  _emitArduinoSerialPlotterInfo = !_emitArduinoSerialPlotterInfo;
  debugLog("toggleEmitArduinoSerialPlotterInfo() exited");
}

void toggleEchobackEnabled() {
  debugLog("toggleEchobackEnabled() entered");
  _echoBackEnabled=!_echoBackEnabled;
  debugLog("toggleEchobackEnabled() exited");
}

void toggleLED() 
{
  debugLog("toggleLED() entered");
  _ledOn = !_ledOn;
  digitalWrite(BUILTIN_LED_PIN, _ledOn ? HIGH : LOW);
  debugLog("toggleLED() exited");
}

void drawLoop() 
{
  debugLog("drawLoop() entered");
  Point point;  
  unsigned int index;
  for (unsigned int i=0;i<_drawCommands.length();i++)
  {
    index=i;
    char curChar=_drawCommands.charAt(i);
    switch (curChar)
    {
      case 'm': case 'M': //move to point
        index++;
        point = readPoint(_drawCommands, index);
        moveTo(point);
        break;

      case 'l': case 'L': //draw line to point
        index++;
        point = readPoint(_drawCommands, index);
        lineTo(point);
        break;
    }
    yield();
  }
  beamOff();
  if (_beamAutoCenteringEnabled) 
  {
    autoCenterBeam();
  }
  yield();
  debugLog("drawLoop() exited");
}

Point readPoint(String string, unsigned int &index) 
{
  debugLog("readPoint(string, unsigned int &index) entered");
  Point point;
  point.X = readClampedUnsignedInt(string, index, MAX_DAC_VALUE, CENTER_DAC_VALUE);
  point.Y = readClampedUnsignedInt(string, index, MAX_DAC_VALUE, CENTER_DAC_VALUE);
  
  debugLog("readPoint(string, unsigned int &index) exited");
  return point;
}

unsigned int readClampedUnsignedInt(String string, unsigned int &index, unsigned int maxValue, unsigned int defaultValue) 
{
  debugLog("readClampedUnsignedInt(string, unsigned int &index, unsigned int maxValue, unsigned int defaultValue) entered");
  String toParse="";
  for (unsigned int i = index;i<string.length();i++) 
  {
    index++;
    char thisChar=string.charAt(i);
    if (thisChar =='0' || thisChar =='1' || thisChar =='2' || thisChar =='3' || thisChar =='4' || thisChar =='5' || thisChar =='6' || thisChar =='7' || thisChar =='8' || thisChar =='9' || thisChar =='-' || thisChar =='.')
    {
      toParse+=thisChar;
    }
    else if (toParse.length() >0) 
    {
      break;
    }
  }

  unsigned int parsed = defaultValue;
  if (toParse.length() > 0) 
  {
    int asInt = toParse.toInt();;
    parsed = asInt >=0 ? (unsigned int) asInt : 0;
    parsed = parsed <= maxValue ? parsed : maxValue;
  }
  debugLog("readClampedUnsignedInt(string, unsigned int &index, unsigned int maxValue, unsigned int defaultValue) exited with return value:" + String(parsed));
  return parsed;
}


void moveTo(Point point) 
{
  debugLog("moveTo(point) entered - Point.X=" + String(point.X) + "; Point.Y=" + String(point.Y));
  beamOff();
  beamTo(point);
  debugLog("moveTo(point) exited");
}

void lineTo(Point point)
{
  debugLog("lineTo(point) entered - Point.X=" + String(point.X) + "; Point.Y=" + String(point.Y));
  beamOn();
  beamTo(point);
  debugLog("lineTo(point) exited");
}

void beamTo(Point point) 
{
  debugLog("beamTo(point) entered - Point.X=" + String(point.X) + "; Point.Y=" + String(point.Y));
  analogWrite(X_PIN, point.X);
  analogWrite(Y_PIN, point.Y);
  yieldMicroseconds(BEAM_MOVEMENT_SETTLING_TIME_MICROSECONDS);
  _beamLocation=point;
  emitArduinoSerialPlotterInfo();
  debugLog("beamTo(point) exited");
}

void beamOff()
{
  debugLog("beamOff() entered");
  if (_beamOn) 
  {
    digitalWrite(Z_PIN, HIGH);
    _beamOn=false;
    yieldMicroseconds(BEAM_TURNOFF_SETTLING_TIME_MICROSECONDS);
  }
  debugLog("beamOff() exited");
}

void beamOn()
{
  debugLog("beamOn() entered");
  if (!_beamOn) 
  {
    digitalWrite(Z_PIN, LOW);
    _beamOn=true;
    yieldMicroseconds(BEAM_TURNON_SETTLING_TIME_MICROSECONDS);
  }
  debugLog("beamOn() exited");
}

void autoCenterBeam() 
{
  debugLog("autoCenterBeam() entered");
  if (_beamAutoCenteringEnabled)
  {
    beamTo(CENTER);
  }
  debugLog("autoCenterBeam() exited");
}

void emitArduinoSerialPlotterInfo() 
{
  debugLog("emitArduinoSerialPlotterInfo() entered");
  if (_emitArduinoSerialPlotterInfo) 
  {
    serialPrint(_beamLocation.X);
    serialPrint('\t');
    serialPrint(_beamLocation.Y);
    serialPrint('\t');
    serialPrintln(_beamOn ? 0 : 1);    
  }
  debugLog("emitArduinoSerialPlotterInfo() exited");
}

void identify() 
{
  debugLog("identify() entered");
  serialPrintln(FIRMWARE_IDENTIFICATION);
  debugLog("identify() entered");
}
void serialPrint(String message) 
{
  Serial.print(message);
  yieldMilliseconds(SERIAL_WRITE_DELAY_MILLIS);
}
void yieldMilliseconds(int delay) {
  unsigned long startMillis=millis();
  while (startMillis + delay < millis())
  {
    yield(); 
  }
}
void yieldMicroseconds(int delay) {
  unsigned long startMicros=micros();
  while (startMicros + delay < micros())
  {
    yield(); 
  }
}
void serialPrintln(String message) {
   serialPrint(message + '\r' + '\n');
}
void debugLog(String message) 
{
  if(_debugLoggingEnabled) 
  {
    serialPrintln(String(micros()) + ":" + message);
  }
}

