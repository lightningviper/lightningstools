#include "timing.h"
/* -------------- TIMING CODE ---------------------------------------- */
void conditionalDelayMicroseconds(uint32_t delayTimeMicroseconds) {
  if (delayTimeMicroseconds > 0) delayMicroseconds(delayTimeMicroseconds );
}

void conditionalDelayNanoseconds(uint32_t delayTimeNanoseconds) {
  if (delayTimeNanoseconds > 0) delayNanos(delayTimeNanoseconds);
}
/* ----------------------------------------------------------------------------- */
