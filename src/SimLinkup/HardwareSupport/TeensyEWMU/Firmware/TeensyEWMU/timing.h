#pragma once
#ifndef TIMING_H
#define TIMING_H

#include "arduino.h"

/* -------------- BASIC PIN TIMINGS -----------------------*/
const uint32_t DIGITAL_OUTPUT_PIN_RISE_TIME_NANOSECONDS = 20;
const uint32_t DIGITAL_OUTPUT_PIN_FALL_TIME_NANOSECONDS = 20;
const uint32_t PIN_MODE_CHANGE_DELAY_MICROSECONDS = 10;
/* --------------------------------------------------*/

void conditionalDelayMicroseconds(uint32_t delayTimeMicroseconds);
void conditionalDelayNanoseconds(uint32_t delayTimeNanoseconds);
static inline void delayNanos(uint32_t) __attribute__((always_inline, unused));
static inline void delayNanos(uint32_t nsec)
{
  if (__builtin_constant_p(nsec)) {
    // use NOPs for the common usage of a constexpr input and short delay
    if (nsec == 0) return;
    if (nsec <= 1000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop");
      return;
    }
    if (nsec <= 2000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop");
      return;
    }
    if (nsec <= 3000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop");
      return;
    }
    if (nsec <= 4000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop");
      return;
    }
    if (nsec <= 5000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      return;
    }
    if (nsec <= 6000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop");
      return;
    }
    if (nsec <= 7000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop");
      return;
    }
    if (nsec <= 8000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop");
      return;
    }
    if (nsec <= 9000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop");
      return;
    }
    if (nsec <= 10000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      return;
    }
    if (nsec <= 11000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop");
      return;
    }
    if (nsec <= 12000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop");
      return;
    }
    if (nsec <= 13000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop");
      return;
    }
    if (nsec <= 14000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop");
      return;
    }
    if (nsec <= 15000 / (F_CPU / 1000000)) {
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      __asm__ volatile("nop\n nop\n nop\n nop\n nop");
      return;
    }
  }
  uint32_t n = nsec * (F_CPU / 45776) >> 16;
  if (n == 0) return;
  __asm__ volatile(
    "L_%=_delayNanoseconds:"    "\n\t"
#ifdef KINETISL
    "sub    %0, #1"       "\n\t"
    "bne    L_%=_delayNanoseconds"    "\n"
    : "+l" (n) :
#else
    "subs   %0, #1"       "\n\t"
    "bne    L_%=_delayNanoseconds"    "\n"
    : "+r" (n) :
#endif
  );
}

#endif
