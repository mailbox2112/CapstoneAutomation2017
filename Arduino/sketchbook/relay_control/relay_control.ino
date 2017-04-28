/* ===============================================================
      Project: 8 Channel 5V Relay Module
       Author: David Fodel
      Created: 22nd Jan 2017
  Arduino IDE: 1.8.1
  Description: Control the Arduino relay module.
  ================================================================== */

/*
  Connect 5V on Arduino to VCC on Relay Module
  Connect 5V from external supply to JD-VCC
  Connect GND from external supply to GND */
#include <AccelStepper.h>
#include <avr/wdt.h>

#define ON 0
#define OFF 1
#define OPEN 0
#define CLOSED 1

#define ACK 0x20
#define NACK 0x21

#define DEBUG_LIGHT 13

#define EX_FAN1    53   // D53 controls exhaust fan 1
#define EX_FAN2    52   // D52 controls exhaust fan 2
#define C_FAN1     51   // ""
#define C_FAN2     50
#define SHUT1      49
#define SHUT2      48
#define SOL1       47
#define SOL2       46
#define SOL3       45
#define SOL4       44
#define SOL5       43
#define SOL6       42
#define LIGHT1     41
#define LIGHT2     40
#define LIGHT3     39
#define HEAT1      38
#define HEAT2      37
#define SHADE_CLK  36
#define SHADE_DIR  35

byte command;
const int pul = 8;
const int dir = 9;
AccelStepper stepper(1, pul, dir);

// TODO: Fix shade motor bug
// TODO: Add timer that counts between each message received; if no message received within certain amount of time, reset the arduino
void setup() {
  // Reset then enable watchdog timer
  wdt_disable();
  
  //Setup all the Arduino Pins
  pinMode(EX_FAN1, OUTPUT);
  pinMode(EX_FAN2, OUTPUT);
  pinMode(C_FAN1, OUTPUT);
  pinMode(C_FAN2, OUTPUT);
  pinMode(SHUT1, OUTPUT);
  pinMode(SHUT2, OUTPUT);
  pinMode(SOL1, OUTPUT);
  pinMode(SOL2, OUTPUT);
  pinMode(SOL3, OUTPUT);
  pinMode(SOL4, OUTPUT);
  pinMode(SOL5, OUTPUT);
  pinMode(SOL6, OUTPUT);
  pinMode(LIGHT1, OUTPUT);
  pinMode(LIGHT2, OUTPUT);
  pinMode(LIGHT3, OUTPUT);
  pinMode(HEAT1, OUTPUT);
  pinMode(HEAT2, OUTPUT);
  pinMode(SHADE_CLK, OUTPUT);
  pinMode(SHADE_DIR, OUTPUT);

  // Turn OFF any power to the Relay channels
  // writing HIGH turns the relay off???
  digitalWrite(EX_FAN1, HIGH);
  digitalWrite(EX_FAN2, HIGH);
  digitalWrite(C_FAN1, HIGH);
  digitalWrite(C_FAN2, HIGH);
  digitalWrite(SHUT1, HIGH);
  digitalWrite(SHUT2, HIGH);
  digitalWrite(SOL1, HIGH);
  digitalWrite(SOL2, HIGH);
  digitalWrite(SOL3, HIGH);
  digitalWrite(SOL4, HIGH);
  digitalWrite(SOL5, HIGH);
  digitalWrite(SOL6, HIGH);
  digitalWrite(LIGHT1, HIGH);
  digitalWrite(LIGHT2, HIGH);
  digitalWrite(LIGHT3, HIGH);
  digitalWrite(HEAT1, HIGH);
  digitalWrite(HEAT2, HIGH);
  digitalWrite(SHADE_CLK, HIGH);
  digitalWrite(SHADE_DIR, HIGH);

  stepper.setMaxSpeed(1000);
  stepper.setSpeed(700);
  stepper.setAcceleration(10);

  // wait to connect to Serial
  Serial.begin(9600);
  
  // Enable Watchdog timer
  wdt_enable(WDTO_8S);
  while (!Serial) {
    ;
  }
}

void loop() {
  if (Serial.available()) {
    process_command();
  }
}

void process_command() {
  // turn on heat relay, fan relay, vent relay, etc...
  command = Serial.read(); // read command string
  ///////////////////////////////////////////////////////
  // ERROR CONTROL
  // RESET WDT WHEN WE GET "ARE YOU THERE?" COMMANDS
  ///////////////////////////////////////////////////////
  if (command == 0x22) {
    wdt_reset();
  }
  ///////////////////////////////////////////////////////
  // HEATING
  ///////////////////////////////////////////////////////
  else if (command == 0x00) {
    digitalWrite(HEAT1, LOW);
    digitalWrite(HEAT2, LOW);
  }

  else if (command == 0x01) {
    digitalWrite(HEAT1, HIGH);
    digitalWrite(HEAT2, HIGH);
  }

  ///////////////////////////////////////////////////////
  // FANS
  ///////////////////////////////////////////////////////
  else if (command == 0x02) {
    digitalWrite(EX_FAN1, LOW);
    digitalWrite(EX_FAN2, LOW);
    digitalWrite(C_FAN1, LOW);
    digitalWrite(C_FAN2, LOW);
  }

  else if (command == 0x03) {
    digitalWrite(EX_FAN1, HIGH);
    digitalWrite(EX_FAN2, HIGH);
    digitalWrite(C_FAN1, HIGH);
    digitalWrite(C_FAN2, HIGH);
  }

  ////////////////////////////////////////////////////////
  // VENTS
  ////////////////////////////////////////////////////////
  else if (command == 0x18) {
    digitalWrite(SHUT1, LOW);
    digitalWrite(SHUT2, LOW);
  }

  else if (command == 0x19) {
    digitalWrite(SHUT1, HIGH);
    digitalWrite(SHUT2, HIGH);
  }

  /////////////////////////////////////////////////////////
  // LIGHTS
  /////////////////////////////////////////////////////////
  else if (command == 0x04) {
    digitalWrite(LIGHT1, LOW);
  }
  else if (command == 0x06) {
    digitalWrite(LIGHT2, LOW);
  }
  else if (command == 0x08) {
    digitalWrite(LIGHT3, LOW);
  }

  else if (command == 0x05) {
    digitalWrite(LIGHT1, HIGH);
  }
  else if (command == 0x07) {
    digitalWrite(LIGHT2, HIGH);
  }
  else if (command == 0x09) {
    digitalWrite(LIGHT3, HIGH);
  }

  ////////////////////////////////////////////////////////
  // WATERING
  ////////////////////////////////////////////////////////
  else if (command == 0x0A) {
    digitalWrite(SOL1, LOW);
  }
  else if (command == 0x0C) {
    digitalWrite(SOL2, LOW);
  }
  else if (command == 0x0E) {
    digitalWrite(SOL3, LOW);
  }
  else if (command == 0x10) {
    digitalWrite(SOL4, LOW);
  }
  else if (command == 0x12) {
    digitalWrite(SOL5, LOW);
  }
  else if (command == 0x14) {
    digitalWrite(SOL6, LOW);
  }

  else if (command == 0x0B) {
    digitalWrite(SOL1, HIGH);
  }
  else if (command == 0x0D) {
    digitalWrite(SOL2, HIGH);
  }
  else if (command == 0x0F) {
    digitalWrite(SOL3, HIGH);
  }
  else if (command == 0x11) {
    digitalWrite(SOL4, HIGH);
  }
  else if (command == 0x13) {
    digitalWrite(SOL5, HIGH);
  }
  else if (command == 0x15) {
    digitalWrite(SOL6, HIGH);
  }

  /////////////////////////////////////////////////////
  // SHADES
  /////////////////////////////////////////////////////
  else if (command == 0x16) {
    wdt_reset();
    stepper.moveTo(500);
    while (stepper.currentPosition() != 300){
      wdt_reset();
      stepper.run(); // Full speed up to position 300
    }
    stepper.stop(); // Stop as fast as possible
    //stepper.runToPosition();
  }

  else if (command == 0x17) {
    wdt_reset();
    stepper.moveTo(-500);
    while (stepper.currentPosition() != -300){
      wdt_reset();
      stepper.run(); // Full speed up to position 0
    }
    stepper.stop(); // Stop as fast as possible
    //stepper.runToPosition();
  }

  // received command does not match possible commands
  else Serial.write(NACK); // respond to RPi that that command didn't work
  Serial.write(ACK); // respond to RPi that write successful
}
