/*
  Serial Event example

  When new serial data arrives, this sketch adds it to a String.
  When a newline is received, the loop prints the string and
  clears it.

  A good test for this is to try it with a GPS receiver
  that sends out NMEA 0183 sentences.

  Created 9 May 2011
  by Tom Igoe

  This example code is in the public domain.

  http://www.arduino.cc/en/Tutorial/SerialEvent

*/
boolean commandSent = false;
String command = "";
#define ACK 0b10101100
#define NACK 0b01010110
#define HEAT 30
#define COOL 31
#define FANS 32
#define VENT 33
#define SHADE 34
#define LIGHT 35
#define WATER 36

void setup() {
  // initialize serial:
  Serial.begin(9600);
  // initialize pins
  pinMode(HEAT, OUTPUT);
  digitalWrite(HEAT, HIGH);
  pinMode(COOL, OUTPUT);
  digitalWrite(COOL, HIGH);
  pinMode(FANS, OUTPUT);
  digitalWrite(FANS, HIGH);
  pinMode(VENT, OUTPUT);
  digitalWrite(VENT, HIGH);
  pinMode(SHADE, OUTPUT);
  digitalWrite(SHADE, HIGH);
  pinMode(LIGHT, OUTPUT);
  digitalWrite(LIGHT, HIGH);
  pinMode(WATER, OUTPUT);
  digitalWrite(WATER, HIGH);
}

void loop() {
  // reset the flag when a command is set
  if (Serial.available() > 0) {
    command = Serial.readString();
    processData(command);
  }
}

/*
  SerialEvent occurs whenever a new data comes in the
  hardware serial RX.  This routine is run between each
  time loop() runs, so using delay inside loop can delay
  response.  Multiple bytes of data may be available.
*/
void processData(String command) {
  // Boolean to keep track of if command received was good or bad
  bool ack = false;
  // Check what the command is
  if (command == "HEAT_ON")
  {
    digitalWrite(HEAT, LOW);
    ack = true;
  }
  else if (command == "HEAT_OFF")
  {
    digitalWrite(HEAT, HIGH);
    ack = true;
  }
  else if (command == "FANS_ON")
  {
    digitalWrite(COOL, LOW);
    ack = true;
  }
  else if (command == "FANS_OFF")
  {
    digitalWrite(COOL, HIGH);
    ack = true;
  }
  else if (command == "VENT_OPEN")
  {
    digitalWrite(VENT, LOW);
    ack = true;
  }
  else if (command == "VENT_CLOSE")
  {
    digitalWrite(VENT, HIGH);
    ack = true;
  }
  else if (command == "SHADE_EXTEND")
  {
    digitalWrite(SHADE, LOW);
    ack = true;
  }
  else if (command == "SHADE_RETRACT")
  {
    digitalWrite(SHADE, HIGH);
    ack = true;
  }
  else if (command == "WATER_ON")
  {
    digitalWrite(WATER, LOW);
    ack = true;
  }
  else if (command == "WATER_OFF")
  {
    digitalWrite(WATER, HIGH);
    ack = true;
  }
  else if (command == "LIGHTS_ON")
  {
    digitalWrite(LIGHT, LOW);
    ack = true;
  }
  else if (command == "LIGHTS_OFF")
  {
    digitalWrite(LIGHT, HIGH);
    ack = true;
  }

  // If the command was good, send an acknowledgement
  if (ack == true)
  {
    Serial.write(ACK);
    Serial.flush();
    //Serial.println("COMMAND ACKNOWLEDGED");
    commandSent = true;
  }
  // Otherwise, tell them the command was bad
  else
  {
    Serial.write(NACK);
    Serial.flush();
    //Serial.println("COMMAND FAILURE");
  }
}
