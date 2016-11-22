#include <NewPing.h>
#include "MovementController.h"
//#include <Encoder.h>
//#include <ADXL345.h>
//#include <I2Cdev.h>
//#include <Wire.h>
//#include <SPI.h>


MovementControllerClass motorCont;

//first param trigger pin, seccond param echo pin
NewPing leftSonar(50, 48);
NewPing rightSonar(46, 44);

//double lastGyroReadTime = 0;
//double yawDriftValuePerSec = 0;
//double pitchDriftValuePerSec = 0;
//double rollDriftValuePerSec = 0;
//double yaw = 0;
//double pitch = 0;
//double roll = 0;

void setup()
{
  Serial.begin(115200);
  Serial.println("Robot Follower Spring 2016 - Fall 2016");
  Serial.println("");
  Serial.println("Serial Started, waiting for commands..");
  Serial.setTimeout(50);
}

void loop()
{
  SerialHandler();
  //GetSonarValues();
}

void GetSonarValues() {
  Serial.print("leftSonar ");
  Serial.print(leftSonar.ping_cm());
  Serial.print("rightSonar ");
  Serial.println(rightSonar.ping_cm());
}

void SerialHandler() {
  float speed = 0;
  if (Serial.available())
  {
    String cmd = Serial.readString();
    cmd.replace("\r", "");
    cmd.replace("\n", "");

    if (cmd == "STOP")
    {
      motorCont.Stop();
      Serial.println("Stopped");
    }
    else if (cmd.startsWith("MOVE"))
    {
      cmd.replace("MOVE ", "");
      int spaceIndex = cmd.indexOf(' ');
      int secondSpaceIndex = cmd.indexOf(' ', spaceIndex + 1);
      int angle = cmd.substring(0, spaceIndex).toInt();
      float magnitude = cmd.substring(spaceIndex + 1, secondSpaceIndex).toFloat();
      float rotation = cmd.substring(secondSpaceIndex).toFloat();
      if (magnitude == 0) {
        motorCont.Stop();
      }
      else {
        motorCont.MoveInDirecion(magnitude, angle, rotation);
      }

      Serial.print("Moving at Angle: ");
      Serial.print(angle);
      Serial.print(" Magnitude: ");
      Serial.print(magnitude);
      Serial.print(" Rotation: ");
      Serial.println(rotation);
    }
    else if (cmd.startsWith("RUALIVE"))
    {
      Serial.println("ALIVE");
    }
    else if (cmd.startsWith("sonar"))
    {
      Serial.print("leftSonar ");
      Serial.print(leftSonar.ping_cm());
      Serial.print(",rightSonar ");
      Serial.println(rightSonar.ping_cm());
    }
    else if (cmd.startsWith("rightSonar"))
    {
      Serial.print("rightSonar ");
      Serial.println(rightSonar.ping_cm());
    }
  }

  delay(1);
}

//void GetGyroValues() {
//  float ypr[3];
//  if (yaw == 0 && pitch == 0 && roll == 0) //first time this is run
//  {
//    lastGyroReadTime = millis();
//    //gyro.getYawPitchRoll(ypr);
//    yaw = ypr[0];
//    pitch = ypr[1];
//    roll = ypr[2];
//  }
//  else
//  {
//    //gyro.getYawPitchRoll(ypr);
//    yaw = ypr[0] - yawDriftValuePerSec*(float)(lastGyroReadTime / 1000);
//    pitch = ypr[1] - pitchDriftValuePerSec*(float)(lastGyroReadTime / 1000);
//    roll = ypr[2] - rollDriftValuePerSec*(float)(lastGyroReadTime / 1000);
//  }
//
//}
//
//void CalibrateGyro() {
//  float initialYPR[3];
//  float finalYPR[3];
//
//
//  gyro.getYawPitchRoll(initialYPR);
//  delay(1000);
//  gyro.getYawPitchRoll(finalYPR);
//
//  yawDriftValuePerSec = finalYPR[0] - initialYPR[0];
//  pitchDriftValuePerSec = finalYPR[1] - initialYPR[1];
//  rollDriftValuePerSec = finalYPR[2] - initialYPR[2];
//}
