// 
// 
// 
#include "Arduino.h"
#include "MotorController.h"
#include <Encoder.h>



const long pi = 3.1415926535897;
const long oneStepInDeg = 360 / 140;	//one step of encoder in degrees
const long oneStepInM = (pi * (long)6 * (long)0.0254) / (long)140;	//(2 * pi * radius) <- circumference, convert to meters(*0.0254), divide the the number of pulses per turn

int directionPin = 0;
int pwnPin = 0;
uint8_t encoderChannelAPin = 0;
uint8_t encoderChannelBPin = 0;
bool inverseDir;


void MotorController::Stop() {
	digitalWrite(directionPin, LOW);
	digitalWrite(pwmPin, LOW);
}
void MotorController::SetSpeed(float speed) {
	float newSpeed = speed;


	if (newSpeed > 100)
	{
		newSpeed = 100;
	}
	if (newSpeed < -100)
	{
		newSpeed = -100;
	}
	newSpeed = map(newSpeed, -100, 100, 0, 255);

	if (newSpeed != 0)
	{
		digitalWrite(directionPin, HIGH);

		if (inverseDir)
		{
			analogWrite(pwmPin, (255 - newSpeed));
		}
		else
		{
			analogWrite(pwmPin, newSpeed);
		}
	}
	else
	{
		Stop();
	}
}

long MotorController::GetSpeed() {
	long oldPosition = encoder.read();
	long time = (long)millis();

	while (oldPosition == encoder.read())
	{
		if (millis() - time > 1000)//1 second timeout
		{
			return 0;
		}
	}
	time = (long)millis() - time;
	time *= (long)1000; //convert to seconds

	return ((long)(oneStepInM) / time);
}

long MotorController::GetPosition() {
	return encoder.read();
}


