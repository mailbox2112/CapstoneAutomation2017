
/* 2/10/20117 
 * YCP Greenhouse Capstone Project for Goode Elementary school
 * Shawn O'Brien & Benjamin Newlin - Sensing team
*/

/* Code for sensors in greenhouse
 * Temperature/Light/Humidity and Moisture 
 * TSL2591 Digital Light Sensor
 * AM2302 Temperature and Humidity sensor
 * 
*/


/*Changes/Updates
 *1/20/2017 - STO - Added Ethernet sheaild functionality
 *2/3/2017  - STO - Replaced ethernet shield with ENC28J60-H whcih is what we are going to use
 *2/20/2017 - STO - Cleaned up code for milestone 1
 *2/28/2017 - STO - Added DIP switch for we can set the user can set the sensors easily
 *3/12/2017 - STO - Added timer to send data automatically on a timer to Matt (visualization team) 
 *3/15/2017 - STO - Added hopeful fix to JSON packet structure. Did not have backslash infront of quoutes 
 *3/19/2017 - STO - Cleaned upp code alot. To many global varibales. Running at 93% full dynamic memory. After moving ethernet stuff to setup 70%
 *3/24/2017 - STO - Added Unique IP and MAC adresse with DIP switch. 
 *3/24/2017 - STO - Added ip_multiplier to multiple the dip_value for IP
 *3/26/2017 - STO - IP range TLH: 192.168.1.171-178  Moisture: 192.168.1.181-188
 *4/9/2017 -  STO - Changed auto timing to 5sec, chabged arrangment of moisture sensors, and light sensativity
 */
 
 
 //////////////////////////////////////////////////////////////////
 //Libraries////
 //////////////////////////////////////////////////////////////////
 
//For Ethernet
#include <EtherCard.h>
#include <IPAddress.h>
#define STATIC 1  // Uses static mode   1=static, 0=dhcp

//For Light sensor
#include <Wire.h>
#include <Adafruit_Sensor.h>
#include "Adafruit_TSL2591.h"

//For Temp/Hum Sensor
#include "DHT.h"

//for Timer
#include <Event.h>
#include <Timer.h>
Timer timer;

//For JSON
#include <ArduinoJson.h>


////////////////////////////////////////////////
//Pins// 
#define DHTPIN 2
int MOISTUREPIN = A0;

//DIP switch pins
int dipPins[] = {7, 6, 5, 4};


//Global varibles//
#define DHTTYPE DHT22 //For library define the model of the sensor chip

String command;

int MAX_BUFFER = 20;
byte Ethernet::buffer[200]; // tcp/ip send and receive buffer


/////Declare sensors/////
Adafruit_TSL2591 tsl = Adafruit_TSL2591(2591); //Need unique identifier. Can have mutiple sensors on same I2C buss. Used model number
DHT dht(DHTPIN, DHTTYPE);


//Address of visulization team 
int my_port = 1337;
int dest_port = 6000;
uint8_t ip_dest_add[4];

//JSON

StaticJsonBuffer<200> jsonBuffer;
JsonObject& json_packet_tlh = jsonBuffer.createObject();
JsonObject& json_packet_mois = jsonBuffer.createObject();

/////////////////////////////////////////////////////////


void setup() {
  Serial.begin(9600);
  dht.begin();
  
  //Setup dip switch pins
  for(int i=0; i<=3; i++){
   pinMode(dipPins[i], INPUT);
  }
  //Configure TSL2591 Light sensor
  configure_Light_Sensor();
  
  //Check DIP. TLH or Moisture? for IP adre.
  //TLH Range: 171-178
  //Moisture Range: 181-188
  int ip_multiplier; 
  if(digitalRead(dipPins[0]) == HIGH){ 
    ip_multiplier=1; //TLH
  }
  if(digitalRead(dipPins[0]) == LOW){
    ip_multiplier=11; //Moisture
  }
  
  
  
  // start the Ethernet
  byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, ip_multiplier+dip_value()}; //Unique mac between boards in the greenhouse
  int ung_id = 170 + (ip_multiplier+dip_value());
  byte ip[] = {192,168,1,ung_id};
  byte dns_ip[] = {192,168,1,1 };
  byte gateway_ip[] = {192,168,1,1};
  byte subnet_ip[] = {255,255,255,0}; //Direct plug in
  
  ether.begin(sizeof Ethernet::buffer, mac);
  ether.staticSetup(ip, gateway_ip, dns_ip, subnet_ip);
  
  //IP of Vis. Team Host
  ether.parseIp(ip_dest_add, "192.168.1.103");
  
 
  
  //Setup timer to auto send sensor data
   timer.every(20000, auto_send_sensor); //20 Sec


}

void loop() {
  
 //Check to see if any packet has bee received. If so calls ether.udpServerListenOnPort(); Interrupt.
  ether.packetLoop(ether.packetReceive());
  
  //update timer
  timer.update();
  

}


/////////////////////
/////Functions//////
///////////////////

/////////////////////////////////////////////
/*Timer Functions to auto send sensor data*/
///////////////////////////////////////////

void auto_send_sensor(){
  
  
  
 //Temp/Ligh/Hum
 if(digitalRead(dipPins[0]) == HIGH){  
   
   ////Sensor Data////
   //Temp
   float tempF = dht.readTemperature(true); //true=Fahrenheit
   //Humidity
   float hum = dht.readHumidity();
   //Light
   uint32_t lum = tsl.getFullLuminosity(); //Read Luminosity
   uint16_t ir, full, visible, lux;
   //Convert lum
   ir = lum >> 16;
   full = lum & 0xFFFF;
   visible = full-ir;
   lux = tsl.calculateLux(full, ir);
   
   //Json Packet
   json_packet_tlh["TYPE"] = 0;
   json_packet_tlh["ID"] = dip_value();
   json_packet_tlh["TEMP"] = tempF;
   json_packet_tlh["LIGHT"] = lux;
   json_packet_tlh["HUM"] = hum;
  
   int dipValue = dip_value(); 
   String temp;
   json_packet_tlh.printTo(temp);
   int temp_length = temp.length()+1;
   char Send_Buffer[temp_length];
   temp.toCharArray(Send_Buffer, temp_length);      
  
   ether.sendUdp(Send_Buffer, temp_length, my_port, ip_dest_add, dest_port);
 }
 
 //Moisture
 if(digitalRead(dipPins[0]) == LOW){  
   //Json Packet
   
   float calibrationValue = 240; //Not going to be 1024!
   int moistureRead = analogRead(MOISTUREPIN);
   //moisture percentage
   float moistureValue = (moistureRead/calibrationValue) *100; 
   json_packet_mois["TYPE"] = 1;
   json_packet_mois["ID"] = dip_value();
   json_packet_mois["1.1"] = (analogRead(16)/calibrationValue) *100;
   json_packet_mois["1.2"] = (analogRead(17)/calibrationValue) *100;
   json_packet_mois["2.1"] = (analogRead(15)/calibrationValue) *100;
   json_packet_mois["2.2"] = (analogRead(14)/calibrationValue) *100;
  
  
   String temp;
   json_packet_mois.printTo(temp);
   int temp_length = temp.length()+1;
   char Send_Buffer[temp_length];
   temp.toCharArray(Send_Buffer, temp_length);     

  
   ether.sendUdp(Send_Buffer, temp_length, my_port, ip_dest_add, dest_port);
 }
 
 
}



///////////////////////////////////////
/*Functions for TSL2591 Light Sensor*/
/////////////////////////////////////

void configure_Light_Sensor()
{
  /*Set gain of the sesnor
   *LOW = 1x gain (bright light)
   *MED = 25x gain (Normal Light) Suggested in manual 
   *HIGH = 428x gain (Very very low light conditions) Pretty much a dark room
   */
   tsl.setGain(TSL2591_GAIN_MED); //(Normal light)
   
   /*Set integration time
    *Lower the time the best for dim light
    *Higher the number the best for High light
    */
    tsl.setTiming(TSL2591_INTEGRATIONTIME_100MS); //Picked suggested lowest time option
  
}//End of configure_Light_Sensor()

/////////////////////////////
//     dip value       /////
////////////////////////////
int dip_value(){
   int i,j=0;
 
 //Get the switches state
 for(i=1; i<=3; i++){
 j = (j << 1) | digitalRead(dipPins[i])==LOW;   // read the input pin
 }
 return j; //return value 
  
}//End of dip_value()
