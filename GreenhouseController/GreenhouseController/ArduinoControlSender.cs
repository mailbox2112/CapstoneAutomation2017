using GreenhouseController.StateMachines;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class ArduinoControlSender
    {
        // TODO: make this use a queue and event-based!
        // Constants for setting up serial ports
        private const int _BAUD = 9600;
        private const Parity _PARITY = Parity.None;
        private const int _DATABITS = 8;
        private const StopBits _STOPBITS = StopBits.One;

        // Communications elements
        private SerialPort _output;
        private byte[] _ACK = new byte[] { 0x20 };
        private byte[] _NACK = new byte[] { 0x21 };
        private byte[] _AWAKE = new byte[] { 0x22 };
        private bool _success = false;
        private int _retryCount = 0;

        // Singleton pattern items
        private static volatile ArduinoControlSender _instance;
        private static object _syncRoot = new object();
        
        /// <summary>
        /// Empty constructor
        /// </summary>
        private ArduinoControlSender()
        { }

        /// <summary>
        /// Singleton pattern field
        /// </summary>
        public static ArduinoControlSender Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new ArduinoControlSender();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Tries to open a serial port on the ports available to the device.
        /// If the port is already open, it checks to make sure that the's still communications
        /// available on the port.
        /// </summary>
        public void TryConnect()
        {
            // TODO: loop through to find serial ports, and establish the fact we're connected
            // We might need to start an external script to do this properly in linux,
            // SerialPort.GetPortNames() only ever returns ttyS0
            // Find ports
            string[] ports = SerialPort.GetPortNames();
            foreach(string port in ports)
            {
                Console.WriteLine($"{port} available.");
            }

            // Create the serial port
            if (_output == null)
            {
                //_output = new SerialPort("/dev/ttyACM0", _BAUD, _PARITY, _DATABITS, _STOPBITS);
                _output = new SerialPort("COM4", _BAUD, _PARITY, _DATABITS, _STOPBITS);
            }

            // Open the serial port
            if (_output.IsOpen != true)
            {
                _output.Open();
                Thread.Sleep(2000);
                _output.ReadTimeout = 500;
                _output.RtsEnable = true;
            }
        }

        /// <summary>
        /// Sends the Arduino a check-in message. No response means that the Arduino is no longer connected/functioning
        /// </summary>
        public bool CheckArduinoStatus()
        {
            bool response = false;
            byte[] buffer = new byte[1];
            try
            {
                // Send the "Are you there?" command
                _output.Write(_AWAKE, 0, _AWAKE.Length);

                // Read the response;
                _output.Read(buffer, 0, buffer.Length);

                // Set the return value to true
                if (buffer.SequenceEqual(_ACK))
                {
                    response = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response = false;
            }
            
            // Return the result
            return response;
        }

        /// <summary>
        /// Takes a Key-Value pair of TemperatureStateMachine and the state we're going to, then sends the required commands
        /// </summary>
        /// <param name="statePair">KeyValuePair of temperature state machine and the state it needs to go to</param>
        public void SendCommand(KeyValuePair<IStateMachine, GreenhouseState> statePair)
        {
            // TODO: add correct sequencing of commands
            byte[] buffer = new byte[1];
            List<Commands> commandsToSend = new List<Commands>();
            
            commandsToSend = statePair.Key.ConvertStateToCommands(statePair.Value);
            
            
            foreach (var command in commandsToSend)
            {
                byte[] convertedCommandBytes = ConvertCommandToBytes(command);
                // Send commands
                try
                {
                    Console.WriteLine($"Attempting to send command {command}");
                    _output.Write(convertedCommandBytes, 0, convertedCommandBytes.Length);
                    Console.WriteLine("Send finished.");

                    // Change states based on the key/value pair we passed in
                    statePair.Key.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;

                    if (command == Commands.SHADE_EXTEND || command == Commands.SHADE_RETRACT)
                    {
                        _output.ReadTimeout = 15000;
                    }
                    else
                    {
                        _output.ReadTimeout = 500;
                    }
                    // Wait for response
                    Console.WriteLine($"Waiting for response...");
                    _output.Read(buffer, 0, buffer.Length);
                    Console.WriteLine($"{buffer.GetValue(0)} received.");
                    
                    buffer = _ACK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Check the response from the Arduino
                // ACK = success
                if (buffer.SequenceEqual(_ACK))
                {
                    Console.WriteLine($"Command {command} sent successfully");
                    _success = true;
                }
                // NACK = command wasn't acknowledged
                else if (buffer.SequenceEqual(_NACK) || buffer == null)
                {
                    Console.WriteLine($"Command {command} returned unsuccessful response, attempting to resend.");

                    // Attempt to resend the command 5 more times
                    while(_retryCount != 5 && _success == false)
                    {
                        // Try-catch so thread doesn't explode if it fails to send/receive
                        try
                        {
                            Console.WriteLine("Retrying send...");
                            _output.Write(convertedCommandBytes, 0, convertedCommandBytes.Length);
                            Console.WriteLine("Awaiting response...");
                            
                            _output.Read(buffer, 0, buffer.Length);
                            Console.WriteLine($"{buffer.GetValue(0)} received");
                            //buffer = ACK;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message, "Retrying again...");
                        }
                        
                        // If we succeeded this time, break out of the loop!
                        if (buffer.SequenceEqual(_ACK))
                        {
                            Console.WriteLine($"Command {command} sent successfully.");
                            _success = true;
                        }
                        // If not, we keep going!
                        else if (buffer.SequenceEqual(_NACK) || buffer.SequenceEqual(null) && _retryCount != 5)
                        {
                            Console.WriteLine("Retrying again...");
                            _retryCount++;
                        }
                    }
                }

                // Change state based on results of sending commands
                // If we never successfully sent the command on retries, we go to the error state
                if (_success == false)
                {
                    statePair.Key.CurrentState = GreenhouseState.ERROR;
                }
                // If the command WAS sent successfully, we set the state accordingly and proceed as normal.
                else if (_success == true)
                {
                    statePair.Key.CurrentState = statePair.Value;
                    Console.WriteLine($"State change {statePair.Value} executed successfully\n");
                }
                _retryCount = 0;
                _success = false;
            }
        }

        /// <summary>
        /// Takes a Key-Value pair of WateringStateMachine and the state we're going to, then sends the required commands
        /// </summary>
        /// <param name="statePair">KeyValuePair of temperature state machine and the state it needs to go to</param>
        public void SendCommand(KeyValuePair<ITimeBasedStateMachine, GreenhouseState> statePair)
        {
            // TODO: add correct sequencing of commands
            byte[] buffer = new byte[1];
            List<Commands> commandsToSend = new List<Commands>();
            commandsToSend = statePair.Key.ConvertStateToCommands(statePair.Value);

            foreach (var command in commandsToSend)
            {
                byte[] convertedCommandBytes = ConvertCommandToBytes(command);
                // Send commands
                try
                {
                    Console.WriteLine($"Attempting to send command {command}");
                    _output.Write(convertedCommandBytes, 0, convertedCommandBytes.Length);
                    Console.WriteLine("Send finished.");

                    // Change states based on the key/value pair we passed in
                    statePair.Key.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;

                    // Wait for response
                    Console.WriteLine($"Waiting for response...");
                    _output.Read(buffer, 0, buffer.Length);
                    Console.WriteLine($"{buffer.GetValue(0)} received.");

                    buffer = _ACK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Check the response from the Arduino
                // ACK = success
                if (buffer.SequenceEqual(_ACK))
                {
                    Console.WriteLine($"Command {command} sent successfully");
                    _success = true;
                }
                // NACK = command wasn't acknowledged
                else if (buffer.SequenceEqual(_NACK) || buffer == null)
                {
                    Console.WriteLine($"Command {command} returned unsuccessful response, attempting to resend.");

                    // Attempt to resend the command 5 more times
                    while (_retryCount != 5 && _success == false)
                    {
                        // Try-catch so thread doesn't explode if it fails to send/receive
                        try
                        {
                            Console.WriteLine("Retrying send...");
                            _output.Write(convertedCommandBytes, 0, convertedCommandBytes.Length);
                            Console.WriteLine("Awaiting response...");

                            _output.Read(buffer, 0, buffer.Length);
                            Console.WriteLine($"{buffer.GetValue(0)} received");
                            //buffer = ACK;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message, "Retrying again...");
                        }

                        // If we succeeded this time, break out of the loop!
                        if (buffer.SequenceEqual(_ACK))
                        {
                            Console.WriteLine($"Command {command} sent successfully.");
                            _success = true;
                        }
                        // If not, we keep going!
                        else if (buffer.SequenceEqual(_NACK) || buffer.SequenceEqual(null) && _retryCount != 5)
                        {
                            Console.WriteLine("Retrying again...");
                            _retryCount++;
                        }
                    }
                }

                // Change state based on results of sending commands
                // If we never successfully sent the command on retries, we go to the error state
                if (_success == false)
                {
                    statePair.Key.CurrentState = GreenhouseState.ERROR;
                }
                // If the command WAS sent successfully, we set the state accordingly and proceed as normal.
                else if (_success == true)
                {
                    statePair.Key.CurrentState = statePair.Value;
                    Console.WriteLine($"State change {statePair.Value} executed successfully\n");
                }
                _retryCount = 0;
                _success = false;
            }
        }

        private byte[] ConvertCommandToBytes(Commands command)
        {
            byte[] byteValue = new byte[1];
            switch (command)
            {
                case Commands.HEAT_ON:
                    byteValue[0] = 0x00;
                    break;
                case Commands.HEAT_OFF:
                    byteValue[0] = 0x01;
                    break;
                case Commands.FANS_ON:
                    byteValue[0] = 0x02;
                    break;
                case Commands.FANS_OFF:
                    byteValue[0] = 0x03;
                    break;
                case Commands.LIGHT1_ON:
                    byteValue[0] = 0x04;
                    break;
                case Commands.LIGHT1_OFF:
                    byteValue[0] = 0x05;
                    break;
                case Commands.LIGHT2_ON:
                    byteValue[0] = 0x06;
                    break;
                case Commands.LIGHT2_OFF:
                    byteValue[0] = 0x07;
                    break;
                case Commands.LIGHT3_ON:
                    byteValue[0] = 0x08;
                    break;
                case Commands.LIGHT3_OFF:
                    byteValue[0] = 0x09;
                    break;
                case Commands.WATER1_ON:
                    byteValue[0] = 0x0A;
                    break;
                case Commands.WATER1_OFF:
                    byteValue[0] = 0x0B;
                    break;
                case Commands.WATER2_ON:
                    byteValue[0] = 0x0C;
                    break;
                case Commands.WATER2_OFF:
                    byteValue[0] = 0x0D;
                    break;
                case Commands.WATER3_ON:
                    byteValue[0] = 0x0E;
                    break;
                case Commands.WATER3_OFF:
                    byteValue[0] = 0x0F;
                    break;
                case Commands.WATER4_ON:
                    byteValue[0] = 0x10;
                    break;
                case Commands.WATER4_OFF:
                    byteValue[0] = 0x11;
                    break;
                case Commands.WATER5_ON:
                    byteValue[0] = 0x12;
                    break;
                case Commands.WATER5_OFF:
                    byteValue[0] = 0x13;
                    break;
                case Commands.WATER6_ON:
                    byteValue[0] = 0x14;
                    break;
                case Commands.WATER6_OFF:
                    byteValue[0] = 0x15;
                    break;
                case Commands.SHADE_EXTEND:
                    byteValue[0] = 0x16;
                    break;
                case Commands.SHADE_RETRACT:
                    byteValue[0] = 0x17;
                    break;
                case Commands.VENTS_OPEN:
                    byteValue[0] = 0x18;
                    break;
                case Commands.VENTS_CLOSED:
                    byteValue[0] = 0x19;
                    break;
                default:
                    break;
            }
            return byteValue;
        }
    }
}
