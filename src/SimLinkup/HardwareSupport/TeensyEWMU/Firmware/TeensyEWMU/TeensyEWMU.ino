#include "font.h"
#include "PacketSerial-1.2.0/PacketSerial.h"

const byte NUM_CHARACTERS_TO_DISPLAY = 32;
const byte COLUMN_DRIVER_PIN[] = { 1, 2, 3, 4, 5 };
const byte BRIGHTNESS_POT_PIN = 14; 
const byte BLANKING_PIN = 8; 
const byte CLOCK_PIN = 10; 
const byte DATA_PIN = 12; 
const byte BUILTIN_LED_PIN=13;
const unsigned int MAX_BRIGHTNESS=4096;
const unsigned int MAX_COLUMN_STROBE_TIME_MICROSECONDS=2000;

const byte ADC_PRECISION_BITS=12;

char _charArray[NUM_CHARACTERS_TO_DISPLAY];

unsigned int _brightness=MAX_BRIGHTNESS;
float _brightnessPct =(float)1.0;
bool _ledOn = false;

const unsigned int BAUD_RATE = 115200;
const size_t RECEIVE_BUFFER_SIZE= 128;
PacketSerial_<COBS, 0, RECEIVE_BUFFER_SIZE> _packetSerial;

void setup() {
  memset(_charArray, 0, sizeof(_charArray));
  
  pinMode(BUILTIN_LED_PIN, OUTPUT);
  LEDOn();

  pinMode(BLANKING_PIN, OUTPUT);
  digitalWriteFast(BLANKING_PIN, LOW);

  pinMode(CLOCK_PIN, OUTPUT);
  digitalWriteFast(CLOCK_PIN, HIGH);

  pinMode(DATA_PIN, OUTPUT);
  digitalWriteFast(DATA_PIN, LOW);

  analogReadResolution(ADC_PRECISION_BITS);
  pinMode(BRIGHTNESS_POT_PIN, INPUT);
  readBrightness();
  
  for (int i = 0; i < 5; i++) {
    columnOff(i);
  }

  _packetSerial.begin(BAUD_RATE);
  _packetSerial.setPacketHandler(&onPacketReceived);

}

void loop() {
  _packetSerial.update();
  readBrightness();
  showChars();
}

void readBrightness() {
  _brightness= analogRead(BRIGHTNESS_POT_PIN);
  _brightnessPct=((float)_brightness /(float)MAX_BRIGHTNESS);
}

void showChars() {
    unsigned int pixelCounter[5]={0,0,0,0,0};
    for (byte column = 0; column < 5; column++) {
      pixelCounter[column]=0;
      for (byte thisChar = NUM_CHARACTERS_TO_DISPLAY; thisChar--; ) {
        for (byte row = 0; row < 7; row++) { 
          digitalWriteFast(CLOCK_PIN, HIGH); 
          delayMicroseconds(2); 
          if (bitRead((font[(byte)_charArray[thisChar]][column]), (6- row))) {
            digitalWriteFast(DATA_PIN, HIGH);
            pixelCounter[column]++; 
          } else {
            digitalWriteFast(DATA_PIN, LOW); 
          }
          delayMicroseconds(1); 
          digitalWriteFast(CLOCK_PIN, LOW); 
          delayMicroseconds(5);
        }
      }
      if (pixelCounter[column] >0) 
      {
        unsigned int strobeTimeMicroseconds = MAX_COLUMN_STROBE_TIME_MICROSECONDS * _brightnessPct;
        //((float)1-((float)pixelCounter[column]/(float)(7.0 *NUM_CHARACTERS_TO_DISPLAY))) *_brightnessPct * (float)MAX_COLUMN_STROBE_TIME_MICROSECONDS;
        strobeColumn(column, strobeTimeMicroseconds);
      }
    }
}

void onPacketReceived(const uint8_t* buffer, size_t size)
{
  toggleLED();
  size_t numBytesToCopy = (size_t) (min(size, NUM_CHARACTERS_TO_DISPLAY));
  memset(_charArray, 0, sizeof(_charArray));
  memcpy(_charArray, buffer, numBytesToCopy);
}

void columnOff(byte column) {
  pinMode(COLUMN_DRIVER_PIN[column], INPUT);
}

void columnOn(byte column) {
  pinMode(COLUMN_DRIVER_PIN[column], OUTPUT); 
  digitalWriteFast(COLUMN_DRIVER_PIN[column], LOW);
}

void strobeColumn(byte column, unsigned int strobeTimeMicroseconds) {
  columnOn(column);
  unblankDisplay();
  delayMicroseconds(strobeTimeMicroseconds);
  columnOff(column);
  blankDisplay();
}

void unblankDisplay() {
  digitalWriteFast(BLANKING_PIN, HIGH);
  delayMicroseconds(2);
}

void blankDisplay() {
  digitalWriteFast(BLANKING_PIN, LOW); 
  delayMicroseconds(5);
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
