// MotorController.h

#ifndef _MOTORCONTROLLER_h
#define _MOTORCONTROLLER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif


#endif
#include <Encoder.h>

class MotorController
{
	
public:
	
	MotorController(int directionPin, int pwmPin, uint8_t encoderChannelAPin, uint8_t encoderChannelBPin,bool inverseDirection) : encoder(encoderChannelAPin, encoderChannelBPin) {
		this->directionPin = directionPin;
		this->pwmPin = pwmPin;

		pinMode(directionPin, OUTPUT);
		pinMode(pwmPin, OUTPUT);
		inverseDir = inverseDirection;
	};
	long GetPosition();
	long GetSpeed();
	void Stop();
	void SetSpeed(float speed);
	
private:
	Encoder encoder;
	
	void MoveCW(int angle);

	int directionPin;
	int pwmPin;
	int encoderChannelAPin;
	int encoderChannelBPin;
	bool inverseDir;
	


};