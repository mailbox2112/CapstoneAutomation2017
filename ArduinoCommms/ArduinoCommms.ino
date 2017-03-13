boolean commandSent = false;
String command = "";
#define ACK 0b10101100
#define NACK 0b01010110
#define EX_FAN1 53
#define EX_FAN2 52
#define C_FAN1 51
#define C_FAN2 50
#define SHUT1 49
#define SHUT2 48
#define SOL1 47
#define SOL2 46
#define SOL3 45
#define SOL4 44
#define SOL5 43
#define SOL6 42
#define LIGHT1 41
#define LIGHT2 40
#define LIGHT3 39
#define HEAT1 38
#define HEAT2 37
#define SHADE_CLK 36
#define SHADE_DIR 35

void setup() {
  // initialize serial:
  Serial.begin(9600);
  // initialize pins
  // FANS
  pinMode(EX_FAN1, OUTPUT);
  digitalWrite(EX_FAN1, HIGH);
  pinMode(EX_FAN2, OUTPUT);
  digitalWrite(EX_FAN2, HIGH);
  pinMode(C_FAN1, OUTPUT);
  digitalWrite(C_FAN1, HIGH);
  pinMode(C_FAN2, OUTPUT);
  digitalWrite(C_FAN2, HIGH);
  //SHUTTERS
  pinMode(SHUT1, OUTPUT);
  digitalWrite(SHUT1, HIGH);
  pinMode(SHUT2, OUTPUT);
  digitalWrite(SHUT2, HIGH);
  // SOLENOIDS
  pinMode(SOL1, OUTPUT);
  digitalWrite(SOL1, HIGH);
  pinMode(SOL2, OUTPUT);
  digitalWrite(SOL2, HIGH);
  pinMode(SOL3, OUTPUT);
  digitalWrite(SOL3, HIGH);
  pinMode(SOL4, OUTPUT);
  digitalWrite(SOL4, HIGH);
  pinMode(SOL5, OUTPUT);
  digitalWrite(SOL5, HIGH);
  pinMode(SOL6, OUTPUT);
  digitalWrite(SOL6, HIGH);
  // LIGHT
  pinMode(LIGHT1, OUTPUT);
  digitalWrite(LIGHT1, HIGH);
  pinMode(LIGHT2, OUTPUT);
  digitalWrite(LIGHT2, HIGH);
  pinMode(LIGHT3, OUTPUT);
  digitalWrite(LIGHT3, HIGH);
  // HEAT
  pinMode(HEAT1, OUTPUT);
  digitalWrite(HEAT1, HIGH);
  pinMode(HEAT2, OUTPUT);
  digitalWrite(HEAT2, HIGH);
  // SHADE MOTOR
  pinMode(SHADE_CLK, OUTPUT);
  digitalWrite(SHADE_CLK, HIGH);
  pinMode(SHADE_DIR, OUTPUT);
  digitalWrite(SHADE_DIR, HIGH);
}

void loop() {
  // reset the flag when a command is set
  if (Serial.available() > 0) {
    command = Serial.readString();
    processData(command);
  }
}

void processData(String command) {
  // Boolean to keep track of if command received was good or bad
  bool ack = false;
  // Check what the command is
  if (command == "HEAT_ON")
  {
    digitalWrite(HEAT1, LOW);
    digitalWrite(HEAT2, LOW);
    ack = true;
  }
  else if (command == "HEAT_OFF")
  {
    digitalWrite(HEAT1, HIGH);
    digitalWrite(HEAT2, HIGH);
    ack = true;
  }
  else if (command == "FANS_ON")
  {
    digitalWrite(EX_FAN1, LOW);
    digitalWrite(EX_FAN2, LOW);
    digitalWrite(C_FAN1, LOW);
    digitalWrite(C_FAN2, LOW);
    ack = true;
  }
  else if (command == "FANS_OFF")
  {
    digitalWrite(EX_FAN1, HIGH);
    digitalWrite(EX_FAN2, HIGH);
    digitalWrite(C_FAN1, HIGH);
    digitalWrite(C_FAN2, HIGH);
    ack = true;
  }
  else if (command == "VENT_OPEN")
  {
    digitalWrite(SHUT1, LOW);
    digitalWrite(SHUT2, LOW);
    ack = true;
  }
  else if (command == "VENT_CLOSE")
  {
    digitalWrite(SHUT1, HIGH);
    digitalWrite(SHUT2, HIGH);
    ack = true;
  }
  else if (command == "SHADE_EXTEND")
  {
    // TODO: shade motor stuff
    digitalWrite(SHADE_CLK, LOW);
    ack = true;
  }
  else if (command == "SHADE_RETRACT")
  {
    // TODO: shade motor stuff
    digitalWrite(SHADE_CLK, HIGH);
    ack = true;
  }
  else if (command == "WATER_ON")
  {
    digitalWrite(SOL1, LOW);
    digitalWrite(SOL2, LOW);
    digitalWrite(SOL3, LOW);
    digitalWrite(SOL4, LOW);
    digitalWrite(SOL5, LOW);
    digitalWrite(SOL6, LOW);
    ack = true;
  }
  else if (command == "WATER_OFF")
  {
    digitalWrite(SOL1, HIGH);
    digitalWrite(SOL2, HIGH);
    digitalWrite(SOL3, HIGH);
    digitalWrite(SOL4, HIGH);
    digitalWrite(SOL5, HIGH);
    digitalWrite(SOL6, HIGH);
    ack = true;
  }
  else if (command == "LIGHTS_ON")
  {
    digitalWrite(LIGHT1, LOW);
    digitalWrite(LIGHT2, LOW);
    digitalWrite(LIGHT3, LOW);
    ack = true;
  }
  else if (command == "LIGHTS_OFF")
  {
    digitalWrite(LIGHT1, HIGH);
    digitalWrite(LIGHT2, HIGH);
    digitalWrite(LIGHT3, HIGH);
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
