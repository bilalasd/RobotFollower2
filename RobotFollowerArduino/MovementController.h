// MovementController.h
#include "MotorController.h"
#ifndef _MOVEMENTCONTROLLER_h
#define _MOVEMENTCONTROLLER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

class MovementControllerClass
{
protected:
private:
	MotorController motorFL;
	MotorController motorFR;
	MotorController motorBL;
	MotorController motorBR;

public:
	void Stop();
	void MoveInDirecion(double magnitude, double direction, double rotation);
	double Limit(double number);
	void TurnForwardLeft(float speed);
	void TurnForwardRight(float speed);
	void TurnBackwardLeft(float speed);
	void TurnBackwardRight(float speed);
	void MoveForward(float speed);
	void MoveLeft(float speed);
	void MoveRight(float speed);
	void MoveBack(float speed);
	long GetSpeed();
	MovementControllerClass() :motorFL(8, 9, 2, 4, false) , motorFR(10, 11, 3, 5, true) , motorBL(12, 13, 18, 22, true) , motorBR(7, 6, 19, 24, false) {

	};

};

extern MovementControllerClass MovementController;

#endif

