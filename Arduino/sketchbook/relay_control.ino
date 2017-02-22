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
 #include <digitalWriteFast.h>
 
 #define ON 0
 #define OFF 1

 #define ACK 0b01011100
 #define NACK 0b01010110

 #define DEBUG_LIGHT 13
 #define HEAT 12   // Connect Digital Pin 12 on Arduino to CH1 on Relay Module
 #define FANS 11   // Connect Digital Pin 11 on Arduino to CH2 on Relay Module
 #define VENTS 10  // ""
 #define WATER 9   // ""
 #define LIGHTS 8   // ""

 String command;
 const String heat_commands[] = {"HEAT_ON","HEAT_OFF"};
 const String fan_commands[] = {"FANS_ON","FANS_OFF"};
 const String vent_commands[] = {"VENTS_ON","VENTS_OFF"};
 const String water_commands[] = {"WATER_ON","WATER_OFF"};
 const String light_commands[] = {"LIGHTS_ON", "LIGHTS_OFF"};
 int count = 0;
 
 void setup(){
   //Setup all the Arduino Pins
   pinModeFast(HEAT, OUTPUT);
   pinMode(FANS, OUTPUT);
   pinModeFast(VENTS, OUTPUT);
   pinModeFast(WATER, OUTPUT);
   pinModeFast(LIGHTS, OUTPUT);
   pinModeFast(DEBUG_LIGHT, OUTPUT);
    
   Serial.begin(9600);
   
   // Turn OFF any power to the Relay channels
   // writing HIGH turns the relay off???
   digitalWriteFast(HEAT, HIGH);
   digitalWrite(FANS, HIGH);
   digitalWriteFast(VENTS, HIGH);
   digitalWriteFast(WATER, HIGH);
   digitalWriteFast(LIGHTS, HIGH);
   digitalWriteFast(DEBUG_LIGHT, HIGH);

   // wait to connect to Serial
   while(!Serial) { ; }
 }
 
 void loop() {
    if(Serial.available()) {
      process_command();
    }
 }

 void process_command() {
      // turn on heat relay, fan relay, vent relay, etc...
      command = Serial.readString(); // read command string
      if(command.equals(heat_commands[ON])) { digitalWriteFast(HEAT, LOW); }
      else if(command.equals(heat_commands[OFF])) { digitalWriteFast(HEAT, HIGH); }
      else if(command.equals(fan_commands[ON])) { digitalWrite(FANS, LOW); }
      else if(command.equals(fan_commands[OFF])) { digitalWrite(FANS, HIGH); }
      else if(command.equals(vent_commands[ON])) { digitalWriteFast(VENTS, LOW); } 
      else if(command.equals(vent_commands[OFF])) { digitalWriteFast(VENTS, HIGH); }
      else if(command.equals(light_commands[ON])) { digitalWriteFast(LIGHTS, LOW); }
      else if(command.equals(light_commands[OFF])) { digitalWriteFast(LIGHTS, HIGH); }
      else if(command.equals(water_commands[ON])) { digitalWriteFast(WATER, LOW); }
      else if(command.equals(water_commands[OFF])) { digitalWriteFast(WATER, HIGH); }
      // received command does not match possible commands
      else Serial.write(NACK); // respond to RPi that that command didn't work
      Serial.write(ACK); // respond to RPi that write successful
  }

