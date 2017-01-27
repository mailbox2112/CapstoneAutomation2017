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
 */

 #define ON 0
 #define OFF 1
 #define OPEN 0
 #define CLOSED 1

 #define HEAT 12        // Connect Digital Pin 12 on Arduino to CH1 on Relay Module (heater relay)
 #define FANS 11        // Connect Digital Pin 11 on Arduino to CH2 on Relay Module (fan relay)
 #define VENTS 10       // Connect Digital Pin 10 on Arduino to CH3 on Relay Module (vent relay)
 #define IRRIGATION 9   // Connect Digital Pin 9 on Arduino to CH4 on Relay Module (irrigation relay)
 #define SHADE 8        // Connect Digital Pin 8 on Arduino to CH5 on Relay Module (shade motor relay)

 String command, ex;
 String heat_commands[] = {"HEAT_ON","HEAT_OFF"};
 String fan_commands[] = {"FANS_ON","FANS_OFF"};
 String vent_commands[] = {"VENTS_ON","VENTS_OFF"};
 String irrigation_command = "WATER_PLANTS";
 String shade_commands[] = {"SHADE_OPEN","SHADE_CLOSED"};
 
 void setup(){
   //Setup all the Arduino Pins
   pinMode(HEAT, OUTPUT);
   pinMode(FANS, OUTPUT);
   pinMode(VENTS, OUTPUT);

   Serial.begin(9600);
   
   // Turn OFF any power to the Relay channels
   // writing HIGH turns the relay off???
   digitalWrite(HEAT, HIGH);
   digitalWrite(FANS, HIGH);
   digitalWrite(VENTS, HIGH);
   delay(2000); //Wait 2 seconds before starting sequence
 }
 
 void loop() {
   // if communications are open...
   while(Serial.available()) {
    // read characters into command string
    // TODO: this might not read all the characters of the message string, re-think this
    char c = Serial.read();
    command += c;
   }
   // turn on the relay associated with the heater
   if(command.equals(heat_commands[ON])) digitalWrite(HEAT, LOW);
   else if(command.equals(heat_commands[OFF])) digitalWrite(HEAT, HIGH);
   // turn on the fan relay
   else if(command.equals(fan_commands[ON])) digitalWrite(FANS, LOW);
   // turn off the fan relay
   else if(command.equals(fan_commands[OFF])) digitalWrite(FANS, HIGH);
   // turn on the vent relay
   else if(command.equals(vent_commands[ON])) digitalWrite(VENTS, LOW);
   // turn off the vent relay
   else if(command.equals(vent_commands[OFF])) digitalWrite(VENTS, HIGH);
   // turn on the shade open relay
   else if(command.equals(shade_commands[OPEN])) digitalWrite(SHADE, LOW);
   // turn off the shade open relay
   else if(command.equals(shade_commands[CLOSED])) digitalWrite(SHADE, HIGH);
   // turn on the irrigation
   else if(command.equals(irrigation_command)) digitalWrite(IRRIGATION, LOW);
   // received command does not match possible commands
   else ex = "Cannot fulfill that command.";
 }
