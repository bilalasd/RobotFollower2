#include <NewPing.h>

NewPing leftSonar(50, 48);
NewPing rightSonar(46, 44);

int left = 0;
int right = 0;



void setup() {
  Serial.begin(115200);
}

void loop() {
  left = leftSonar.ping_cm();
  right = rightSonar.ping_cm();
  Serial.print("Left Sonar: ");
  Serial.print(left);
  Serial.print(" Right Sonar: ");
  //delay(100);
  Serial.println(right);
  delay(100);

}
