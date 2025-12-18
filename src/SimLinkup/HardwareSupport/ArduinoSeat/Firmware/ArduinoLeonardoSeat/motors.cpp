#include "motors.h"

bool _builtInLEDOn = false;
uint8_t _MotorStates;

uint8_t _Motor1Speed;
uint8_t _Motor2Speed;
uint8_t _Motor3Speed;
uint8_t _Motor4Speed;

AF_DCMotor _Motor1(1);
AF_DCMotor _Motor2(2);
AF_DCMotor _Motor3(3);
AF_DCMotor _Motor4(4);

void setupMotorOutputs() {
	_Motor1.setSpeed(0);
	_Motor2.setSpeed(0);
	_Motor3.setSpeed(0);
	_Motor4.setSpeed(0);
	
	_Motor1.run(RELEASE);
	_Motor2.run(RELEASE);
	_Motor3.run(RELEASE);
	_Motor4.run(RELEASE);
}

void updateMotorOutputs()
{
	setMotorSpeeds();
	setMotorStates();
}

void setMotorSpeeds()
{
	_Motor1.setSpeed(_Motor1Speed);
	_Motor2.setSpeed(_Motor2Speed);
	_Motor3.setSpeed(_Motor3Speed);
	_Motor4.setSpeed(_Motor4Speed);
}

void setMotorStates()
{
  if (_MotorStates & MotorBits::ALL_OFF)
  {
    _Motor1.run(RELEASE);
    _Motor2.run(RELEASE);
    _Motor3.run(RELEASE);
    _Motor4.run(RELEASE);
    return;      
  }
  
	if (_Motor1Speed < 10)
		_Motor1.run(RELEASE);
	else 
    _MotorStates & MotorBits::MOTOR_1_STATE ? _Motor1.run(FORWARD) : _Motor1.run(RELEASE);
  
	if (_Motor2Speed < 10)
		_Motor2.run(RELEASE);
	else
    _MotorStates & MotorBits::MOTOR_2_STATE ? _Motor2.run(FORWARD) : _Motor2.run(RELEASE);
	
	if (_Motor3Speed < 10)
		_Motor3.run(RELEASE);
	else
    _MotorStates & MotorBits::MOTOR_3_STATE ? _Motor3.run(FORWARD) : _Motor3.run(RELEASE);
	
	if (_Motor4Speed < 10)
		_Motor4.run(RELEASE);
	else
    _MotorStates & MotorBits::MOTOR_4_STATE ? _Motor4.run(FORWARD) : _Motor4.run(RELEASE);
}

/* ----------------------------------------------------------------------------- */

/* -------------- BUILT-IN LED HANDLING ---------------------------------------- */
void setupBuiltinLED() {
  pinMode(ARDUINO_BUILTIN_LED_PIN, OUTPUT); turnBuiltInLEDOn();
}

void turnBuiltInLEDOff() {
  _builtInLEDOn = false;
  updateBuiltInLED();
}
void turnBuiltInLEDOn() {
  _builtInLEDOn = true;
  updateBuiltInLED();
}
void toggleBuiltInLED() {
  _builtInLEDOn = !_builtInLEDOn;
  updateBuiltInLED();
}
void updateBuiltInLED() {
  digitalWrite(ARDUINO_BUILTIN_LED_PIN, _builtInLEDOn ? HIGH : LOW);
}
/* ----------------------------------------------------------------------------- */
