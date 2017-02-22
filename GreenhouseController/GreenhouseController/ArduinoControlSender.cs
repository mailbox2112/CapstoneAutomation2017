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
        // Constants for setting up serial ports
        private const int _BAUD = 9600;
        private const Parity _PARITY = Parity.None;
        private const int _DATABITS = 8;
        private const StopBits _STOPBITS = StopBits.One;

        // Communications elements
        private SerialPort _output;
        private byte[] _ACK = new byte[] { 0xAC };
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
            //TODO: loop through to find serial ports, and establish the fact we're connected
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
        /// Takes a list of commands to be sent to the arduino and sends them over the pi's serial port
        /// </summary>
        /// <param name="_commandsToSend">State to convert to commands and send to Arduino</param>
        public void SendCommand(KeyValuePair<IStateMachine, GreenhouseState> statePair)
        {
            byte[] buffer = new byte[1];
            List<Commands> commandsToSend = new List<Commands>();

            // TODO: Fix state machine here. Should enter the sending state just as we send to the serial port.
            // Also, what happens if one of the commands fails but others are fine? retry, and make state machine 
            // behave properly in that case.  How do we tell if one of the commands failed and we need to retry?

            
            if (statePair.Key is TemperatureStateMachine)
            {
                commandsToSend = StateMachineContainer.Instance.Temperature.ConvertStateToCommands(statePair.Value);
            }
            else if (statePair.Key is LightingStateMachine)
            {
                commandsToSend = StateMachineContainer.Instance.Lighting.ConvertStateToCommands(statePair.Value);
            }
            else if (statePair.Key is WateringStateMachine)
            {
                commandsToSend = StateMachineContainer.Instance.Watering.ConvertStateToCommands(statePair.Value);
            }
            
            foreach (var command in commandsToSend)
            {
                // Send commands
                try
                {
                    Console.WriteLine($"Attempting to send command {command}");
                    _output.Write(command.ToString());
                    Thread.Sleep(1250);
                    Console.WriteLine("Send finished.");
                    // TODO: Move this somewhere that makes sense. Do we change state for each command sent? Probably
                    if (statePair.Key is TemperatureStateMachine)
                    {
                        StateMachineContainer.Instance.Temperature.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;
                    }
                    else if (statePair.Key is LightingStateMachine)
                    {
                        StateMachineContainer.Instance.Lighting.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;
                    }
                    else if (statePair.Key is WateringStateMachine)
                    {
                        StateMachineContainer.Instance.Watering.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;
                    }

                    // Wait for response
                    Console.WriteLine($"Waiting for response...");
                    _output.Read(buffer, 0, buffer.Length);
                    Console.WriteLine($"{buffer.GetValue(0)} received.");
                    //buffer = NACK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                

                if (buffer.SequenceEqual(_ACK))
                {
                    Console.WriteLine($"Command {command} sent successfully");
                    _success = true;
                }
                else if (buffer.SequenceEqual(_NACK) || buffer == null)
                {
                    Console.WriteLine($"Command {command} returned unsuccessful response, attempting to resend.");

                    // Attempt to resend the command 5 more times
                    while(_retryCount != 5 && _success == false)
                    {
                        // Try-catch so we don't explode if it fails to send/receive
                        try
                        {
                            Console.WriteLine("Retrying send...");
                            _output.Write(command.ToString());
                            Console.WriteLine("Awaiting response...");

                            Console.WriteLine("Awaiting response...");
                            _output.Read(buffer, 0, buffer.Length);
                            Console.WriteLine($"{buffer.GetValue(0)} received");
                            //buffer = ACK;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message, "Retrying again....");
                        }
                        
                        // If we succeeded this time, break out of the loop!
                        if (buffer == _ACK)
                        {
                            Console.WriteLine($"Command {command} sent successfully.");
                            _success = true;
                        }
                        else if (buffer == _NACK || buffer == null && _retryCount != 5)
                        {
                            Console.WriteLine("Retrying again....");
                            _retryCount++;
                        }
                    }
                }

                // Change state based on results of sending commands
                if (_success == false)
                {
                    if (statePair.Key is TemperatureStateMachine)
                    {
                        StateMachineContainer.Instance.Temperature.CurrentState = GreenhouseState.ERROR;
                    }
                    else if (statePair.Key is LightingStateMachine)
                    {
                        StateMachineContainer.Instance.Lighting.CurrentState = GreenhouseState.ERROR;
                    }
                    else if (statePair.Key is WateringStateMachine)
                    {
                        StateMachineContainer.Instance.Watering.CurrentState = GreenhouseState.ERROR;
                    }
                }
                else if (_success == true)
                {
                    if (statePair.Key is TemperatureStateMachine)
                    {
                        StateMachineContainer.Instance.Temperature.CurrentState = statePair.Value;
                    }
                    else if (statePair.Key is LightingStateMachine)
                    {
                        StateMachineContainer.Instance.Lighting.CurrentState = statePair.Value;
                    }
                    else if (statePair.Key is WateringStateMachine)
                    {
                        StateMachineContainer.Instance.Watering.CurrentState = statePair.Value;
                    }
                    Console.WriteLine($"State change {statePair.Value} executed successfully\n");
                }
                _retryCount = 0;
                _success = false;
            }
        }

        /// <summary>
        /// Send command to turn off manual control of a statemachine
        /// </summary>
        /// <param name="stateMachine">State machine to set back on automated control</param>
        public void SendManualOffCommand(IStateMachine stateMachine)
        {

            byte[] buffer = new byte[1];
            List<Commands> commandsToSend = new List<Commands>();

            // Get the commands we need to send to turn the manual control off
            if (stateMachine is TemperatureStateMachine)
            {
                commandsToSend.Add(Commands.HEAT_OFF);
                commandsToSend.Add(Commands.VENT_CLOSE);
                commandsToSend.Add(Commands.SHADE_RETRACT);
            }
            else if (stateMachine is LightingStateMachine)
            {
                commandsToSend.Add(Commands.LIGHTS_OFF);
            }
            else if (stateMachine is WateringStateMachine)
            {
                commandsToSend.Add(Commands.WATER_OFF);
            }

            foreach (var command in commandsToSend)
            {
                // Try to send command to turn off heater, watering, lighting etc.
                try
                {
                    Console.WriteLine("Retrying send...");
                    _output.Write(command.ToString());
                    Console.WriteLine("Awaiting response...");

                    Console.WriteLine("Awaiting response...");
                    _output.Read(buffer, 0, buffer.Length);
                    Console.WriteLine($"{buffer.GetValue(0)} received");
                    //buffer = NACK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }


                if (buffer.SequenceEqual(_ACK))
                {
                    Console.WriteLine($"Command {command} sent successfully");

                    _success = true;
                }
                else if (buffer.SequenceEqual(_NACK) || buffer == null)
                {
                    Console.WriteLine($"Command {command} sent unsuccessfully, attempting to resend.");

                    // Attempt to resend the command 5 more times
                    while (_retryCount != 5 && _success == false)
                    {
                        // Try-catch so we don't explode if it fails to send/receive
                        try
                        {
                            _output.Write(command.ToString());
                            Thread.Sleep(1250);
                            _output.Read(buffer, 0, 0);
                            //buffer = ACK;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message, "Retrying again....");
                        }

                        // If we succeeded this time, break out of the loop!
                        if (buffer == _ACK)
                        {
                            Console.WriteLine($"Command {command} sent successfully.");
                            _success = true;
                        }
                        else if (buffer == _NACK || buffer == null && _retryCount != 5)
                        {
                            Console.WriteLine("Retrying again....");
                            _retryCount++;
                        }
                    }
                }

                // Change state based on results of sending commands
                if (_success == false)
                {
                    if (stateMachine is TemperatureStateMachine)
                    {
                        StateMachineContainer.Instance.Temperature.CurrentState = GreenhouseState.ERROR;
                    }
                    else if (stateMachine is LightingStateMachine)
                    {
                        StateMachineContainer.Instance.Lighting.CurrentState = GreenhouseState.ERROR;
                    }
                    else if (stateMachine is WateringStateMachine)
                    {
                        StateMachineContainer.Instance.Watering.CurrentState = GreenhouseState.ERROR;
                    }
                }
                else if (_success == true)
                {
                    if (stateMachine is TemperatureStateMachine)
                    {
                        StateMachineContainer.Instance.Temperature.CurrentState = GreenhouseState.WAITING_FOR_DATA;
                    }
                    else if (stateMachine is LightingStateMachine)
                    {
                        StateMachineContainer.Instance.Lighting.CurrentState = GreenhouseState.WAITING_FOR_DATA;
                    }
                    else if (stateMachine is WateringStateMachine)
                    {
                        StateMachineContainer.Instance.Watering.CurrentState = GreenhouseState.WAITING_FOR_DATA;
                    }
                    Console.WriteLine($"State change {GreenhouseState.WAITING_FOR_DATA} executed successfully\n");
                }
                _retryCount = 0;
                _success = false;
            }
        }
    }
}
