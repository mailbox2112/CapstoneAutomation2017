/* ===============================================================
      Project: 8 Channel 5V Relay Module
       Author: David Fodel
      Created: 22nd Jan 2017
  Arduino IDE: 1.8.1
  Description: Control the Arduino relay module.
================================================================== */

 /* 
  Connect 5V on Arduino to VCC on Relay Module
  Connect GND on Arduino to GND on Relay Module 
  Connect GND on Arduino to the Common Terminal (middle terminal) on Relay Module. */

 #define ON 0
 #define OFF 1

 #define ACK 0b01011100

 #define HEAT 12   // Connect Digital Pin 12 on Arduino to CH1 on Relay Module
 #define FANS 11   // Connect Digital Pin 11 on Arduino to CH2 on Relay Module
 #define VENTS 10  // ""
 #define WATER 9   // ""
 #define LIGHTS 8   // ""

 String command;
 const String heat_commands[] = {"HEAT_ON","HEAT_OFF"};
 const String fan_commands[] = {"FANS_ON","FANS_OFF"};
 const String vent_commands[] = {"VENTS_ON","VENTS_OFF"};
 const String water_command = {"WATER_ON"};
 const String light_commands[] = {"LIGHTS_ON", "LIGHTS_OFF"};
 
 void setup(){
   //Setup all the Arduino Pins
   pinMode(HEAT, OUTPUT);
   pinMode(FANS, OUTPUT);
   pinMode(VENTS, OUTPUT);
   pinMode(WATER, OUTPUT);
   pinMode(LIGHTS, OUTPUT);

   Serial.begin(9600);
   
   // Turn OFF any power to the Relay channels
   // writing HIGH turns the relay off???
   digitalWrite(HEAT, HIGH);
   digitalWrite(FANS, HIGH);
   digitalWrite(VENTS, HIGH);
   digitalWrite(WATER, HIGH);
   digitalWrite(LIGHTS, HIGH);
   delay(2000); //Wait 2 seconds before starting sequence
 }
 
 void loop() {
    // read characters into command string
    // TODO: this might not read all the characters of the message string, re-think this
    if(Serial.available()) {
      command = Serial.readString(); // read command string
      Serial.write(ACK); // respond with ack packet
      Serial.flush();
      // turn on the relay associated with the heater
      if(Serial.availableForWrite() == 0) Serial.println(action(command));
    }
 }

 String action(String command) {
      // turn on heat relay, fan relay, vent relay, etc...
      if(command.equals(heat_commands[ON])) digitalWrite(HEAT, LOW);
      else if(command.equals(heat_commands[OFF])) digitalWrite(HEAT, HIGH);
      else if(command.equals(fan_commands[ON])) digitalWrite(FANS, LOW);
      else if(command.equals(fan_commands[OFF])) digitalWrite(FANS, HIGH);
      else if(command.equals(vent_commands[ON])) digitalWrite(VENTS, LOW);
      else if(command.equals(vent_commands[OFF])) digitalWrite(VENTS, HIGH);
      else if(command.equals(light_commands[ON])) digitalWrite(LIGHTS, LOW);
      else if(command.equals(light_commands[OFF])) digitalWrite(LIGHTS, HIGH);
      // received command does not match possible commands
      else return "Cannot fulfill that command.";
      return command + " SUCCESS.";
  }
