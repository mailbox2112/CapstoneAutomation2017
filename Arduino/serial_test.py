#!/usr/bin/env python

import serial

running = True
ser = serial.Serial('/dev/ttyACM0',9600)

while(running):
	command = raw_input('Enter command: ')
	ser.write(command)
