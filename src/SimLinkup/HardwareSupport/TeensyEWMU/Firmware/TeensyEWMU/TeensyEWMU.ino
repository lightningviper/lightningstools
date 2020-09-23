#include "font.h"
#include "PacketSerial-1.2.0/PacketSerial.h"

const byte NUM_CHARACTERS_TO_DISPLAY = 32;
const byte COLUMN_DRIVER_PIN[] = { 1, 2, 3, 4, 5 };
const byte BRIGHTNESS_POT_PIN = 14; 
const byte BLANKING_PIN = 8; 
const byte CLOCK_PIN = 10; 
const byte DATA_PIN = 12; 
const byte BUILTIN_LED_PIN=13;
const unsigned int MAX_STROBE_TIME_MICROSECONDS = 2000; 
const unsigned int MAX_BRIGHTNESS=4096;

const byte ADC_PRECISION_BITS=12;

char _charArray[NUM_CHARACTERS_TO_DISPLAY + 1];
unsigned int _brightness=MAX_BRIGHTNESS;
bool _ledOn = false;

const unsigned int BAUD_RATE = 115200;
const size_t RECEIVE_BUFFER_SIZE= 64 * 1024;
PacketSerial_<COBS, 0, RECEIVE_BUFFER_SIZE> _packetSerial;

void setup() {
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
    pinMode(COLUMN_DRIVER_PIN[i], OUTPUT);
    digitalWriteFast(COLUMN_DRIVER_PIN[i], HIGH);
  }

  _packetSerial.begin(BAUD_RATE);
  _packetSerial.setPacketHandler(&onPacketReceived);

  prepString("Hello World Hello World Hello World");
}

void loop() {
  _packetSerial.update();
  readBrightness();
  showChars();
}

void readBrightness() {
  _brightness= analogRead(BRIGHTNESS_POT_PIN);
}

void prepString(String inString) {
  if (inString.length() < NUM_CHARACTERS_TO_DISPLAY) {
    while (inString.length() < NUM_CHARACTERS_TO_DISPLAY) {
      inString = inString + " ";
    }
  } else {
    inString.remove(NUM_CHARACTERS_TO_DISPLAY);
  }

  inString.toCharArray(_charArray, NUM_CHARACTERS_TO_DISPLAY + 1); // +1 to leave room for the terminating char
}

void showChars() {
    for (byte column = 0; column < 5; column++) { 
      int pixelCounter = 0;
      for (int thisChar = NUM_CHARACTERS_TO_DISPLAY; thisChar > 0; --thisChar) {
        for (byte row = 0; row < 7; row++) { 
          digitalWriteFast(CLOCK_PIN, HIGH); 
          delayMicroseconds(1);
          if (bitRead((font[_charArray[thisChar - 1]][column]), (6- row))) {
            digitalWriteFast(DATA_PIN, HIGH);
            pixelCounter++; 
          } else {
            digitalWriteFast(DATA_PIN, LOW); 
          }
          delayMicroseconds(2); 
          digitalWriteFast(CLOCK_PIN, LOW); 
          delayMicroseconds(2);
        }
      }
      if(pixelCounter>0){
        digitalWriteFast(BLANKING_PIN, HIGH);
        digitalWriteFast(COLUMN_DRIVER_PIN[column], LOW); 
        float brightnessPct = ((float)_brightness /(float)MAX_BRIGHTNESS);
        unsigned int strobeTimeMicroseconds = (unsigned int) (MAX_STROBE_TIME_MICROSECONDS * brightnessPct);
        delayMicroseconds(strobeTimeMicroseconds);
        digitalWriteFast(COLUMN_DRIVER_PIN[column], HIGH); 
      }
      digitalWriteFast(BLANKING_PIN, LOW); 

    }
}

void onPacketReceived(const uint8_t* buffer, size_t size)
{
  toggleLED();
  size_t numBytesToCopy = (size_t) (min(size, NUM_CHARACTERS_TO_DISPLAY));
  memset(_charArray, 0, sizeof(_charArray));
  memcpy(_charArray, buffer, numBytesToCopy);
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
