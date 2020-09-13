#include "font.h"

const byte NUM_CHARACTERS_TO_DISPLAY = 32;
const byte COLUMN_DRIVER_PIN[] = { 1, 2, 3, 4, 5 };
const byte BRIGHTNESS_POT_PIN = 14; 
const byte BLANKING_PIN = 8; 
const byte CLOCK_PIN = 10; 
const byte DATA_PIN = 12; 
const unsigned int MAX_STROBE_TIME_MICROSECONDS = 600; 
const unsigned int MAX_BRIGHTNESS=4096;

char _charArray[NUM_CHARACTERS_TO_DISPLAY + 1];
unsigned int _brightness=MAX_BRIGHTNESS;

void setup() {
  pinMode(BLANKING_PIN, OUTPUT);
  digitalWriteFast(BLANKING_PIN, LOW);

  pinMode(CLOCK_PIN, OUTPUT);
  digitalWriteFast(CLOCK_PIN, HIGH);

  pinMode(DATA_PIN, OUTPUT);
  digitalWriteFast(DATA_PIN, LOW);

  analogReadResolution(12);
  pinMode(BRIGHTNESS_POT_PIN, INPUT);

  readBrightness();
  
  for (int i = 0; i < 5; i++) {
    pinMode(COLUMN_DRIVER_PIN[i], OUTPUT);
    digitalWriteFast(COLUMN_DRIVER_PIN[i], HIGH);
  }

  prepString("TRN     LOW  S  FILE AUDALT SRCH");
}


void loop() {
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
