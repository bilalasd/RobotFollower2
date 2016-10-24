#include "MovementController.h"
#include "Arduino.h"

bool invertDirection = false;

void MovementControllerClass::Stop() {
	motorBL.Stop();
	motorBR.Stop();
	motorFL.Stop();
	motorFR.Stop();
}

void MovementControllerClass::MoveInDirecion(double magnitude, double direction, double rotation) {
	double speedFL, speedBL, speedFR, speedBR;
	double directionInRad = 0;
	double cosOfDirection = 0;
	double sinOfDirection = 0;
	double mag;

	mag = Limit(magnitude)*sqrt(2.0);
	directionInRad = (direction + 45)*PI / 180.0;
	Serial.print("Direction in Rad: ");
	Serial.println(directionInRad);

	cosOfDirection = cos(directionInRad);
	sinOfDirection = sin(directionInRad);

	speedFL = sinOfDirection*mag + rotation;
	speedFR = cosOfDirection*mag - rotation;
	speedBL = cosOfDirection*mag + rotation;
	speedBR = sinOfDirection*mag - rotation;

	//Find the max
	double max = speedFL;
	if (speedFR > max)
	{
		max = speedFR;
	}
	if (speedBL > max)
	{
		max = speedBL;
	}
	if (speedBR > max)
	{
		max = speedBR;
	}

	Serial.print("FL ");
	Serial.print(speedFL);
	Serial.print(" FR ");
	Serial.print(speedFR);
	Serial.print(" BL ");
	Serial.print(speedBL);
	Serial.print(" BR ");
	Serial.println(speedBL);

	if (max > 1.0)
	{
		//Normalize speeds;
		speedBL = speedBL / max;
		speedBR = speedBR / max;
		speedFL = speedFL / max;
		speedFR = speedFR / max;
	}

	Serial.print("FL ");
	Serial.print(speedFL);
	Serial.print(" FR ");
	Serial.print(speedFR);
	Serial.print(" BL ");
	Serial.print(speedBL);
	Serial.print(" BR ");
	Serial.println(speedBL);
	//Serial.print(" Max ");
	//Serial.println(max);

	motorBL.SetSpeed(speedBL * 100.0);
	motorBR.SetSpeed(speedBR * 100.0);
	motorFL.SetSpeed(speedFL * 100.0);
	motorFR.SetSpeed(speedFR * 100.0);
}

double MovementControllerClass::Limit(double number) {
	if (number > 1.0)
	{
		return 1.0;
	}
	else if (number < -1.0) {
		return -1.0;
	}
	else
	{
		return number;
	}
}

void MovementControllerClass::TurnForwardLeft(float speed) {
	motorBL.SetSpeed(speed*(float)0.5);
	motorFL.SetSpeed(speed*(float)0.5);
	motorBR.SetSpeed(speed*(float)1.5);
	motorFR.SetSpeed(speed*(float)1.5);
}

void MovementControllerClass::TurnForwardRight(float speed) {
	motorBL.SetSpeed(speed*(float)1.5);
	motorFL.SetSpeed(speed*(float)1.5);
	motorBR.SetSpeed(speed*(float)0.5);
	motorFR.SetSpeed(speed*(float)0.5);
}

void MovementControllerClass::TurnBackwardLeft(float speed) {
	motorBL.SetSpeed(-speed*(float)0.5);
	motorFL.SetSpeed(-speed*(float)0.5);
	motorBR.SetSpeed(-speed*(float)1.5);
	motorFR.SetSpeed(-speed*(float)1.5);
}

void MovementControllerClass::TurnBackwardRight(float speed) {
	motorBL.SetSpeed(-speed*(float)1.5);
	motorFL.SetSpeed(-speed*(float)1.5);
	motorBR.SetSpeed(-speed*(float)0.5);
	motorFR.SetSpeed(-speed*(float)0.5);
}

void MovementControllerClass::MoveForward(float speed) {
	motorBL.SetSpeed(speed);
	motorBR.SetSpeed(speed);
	motorFL.SetSpeed(speed);
	motorFR.SetSpeed(speed);
}

void MovementControllerClass::MoveLeft(float speed) {
	motorBL.SetSpeed(speed);
	motorBR.SetSpeed(-speed);
	motorFL.SetSpeed(-speed);
	motorFR.SetSpeed(speed);
}

void MovementControllerClass::MoveRight(float speed) {
	motorBL.SetSpeed(-speed);
	motorBR.SetSpeed(speed);
	motorFL.SetSpeed(speed);
	motorFR.SetSpeed(-speed);
}
void MovementControllerClass::MoveBack(float speed) {
	motorBL.SetSpeed(-speed);
	motorBR.SetSpeed(-speed);
	motorFL.SetSpeed(-speed);
	motorFR.SetSpeed(-speed);
}

long MovementControllerClass::GetSpeed() {
	long averageSpeed = (motorBL.GetSpeed() + motorBR.GetSpeed() + motorFL.GetSpeed() + motorFR.GetSpeed()) / (long)4;
	return averageSpeed;
}