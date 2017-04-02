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
        private byte[] _ACK = new byte[] { 0x5C };
        private byte[] _NACK = new byte[] { 0x56 };
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
                _output = new SerialPort("/dev/ttyACM0", _BAUD, _PARITY, _DATABITS, _STOPBITS);
                //_output = new SerialPort("COM3", _BAUD, _PARITY, _DATABITS, _STOPBITS);
            }

            // Open the serial port
            if (_output.IsOpen != true)
            {
                _output.Open();
                Thread.Sleep(2000);
                _output.ReadTimeout = 500;
                _output.RtsEnable = true;
            }
            // TODO: add task down here to periodically poll for the arduino to make sure everything is okay
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
                // Send commands
                try
                {
                    Console.WriteLine($"Attempting to send command {command}");
                    _output.Write(command.ToString());
                    Thread.Sleep(1250);
                    Console.WriteLine("Send finished.");

                    // Change states based on the key/value pair we passed in
                    statePair.Key.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;

                    if (command == Commands.SHADE_EXTEND || command == Commands.SHADE_RETRACT)
                    {
                        _output.ReadTimeout = 10000;
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
                            _output.Write(command.ToString());
                            Thread.Sleep(1250);
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
                // Send commands
                try
                {
                    Console.WriteLine($"Attempting to send command {command}");
                    _output.Write(command.ToString());
                    Thread.Sleep(1250);
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
                            _output.Write(command.ToString());
                            Thread.Sleep(1250);
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
    }
}
