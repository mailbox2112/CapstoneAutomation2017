using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class ArduinoControlSender : IDisposable
    {
        private bool _success = false;
        private int _retryCount = 0;
        private byte[] _ACK = new byte[2] { 10, 12 };
        private byte[] _NACK = new byte[2] { 5, 6};

        private SerialPort _output;
        public ArduinoControlSender()
        {
            // TODO: construct!
             _output = new SerialPort("/dev/ttyACM0", 9600, Parity.None, 8, StopBits.One);
        }

        public void Dispose()
        {
            // TODO: close sockets and stuff here!
             _output.Close();
        }

        /// <summary>
        /// Takes a list of commands to be sent to the arduino and sends them over the pi's serial port
        /// </summary>
        /// <param name="_commandsToSend">State to convert to commands and send to Arduino</param>
        public void SendCommand(KeyValuePair<IStateMachine, GreenhouseState> statePair)
        {
            byte[] buffer = new byte[8];
            List<Commands> commandsToSend = new List<Commands>();

            // TODO: Fix state machine here. Should enter the sending state just as we send to the serial port.
            // Also, what happens if one of the commands fails but others are fine? retry, and make state machine 
            // behave properly in that case.  How do we tell if one of the commands failed and we need to retry?

             _output.Open();
            Thread.Sleep(1000);

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
            Console.WriteLine($"Attempting to send state {statePair.Value}");

            foreach (var command in commandsToSend)
            {
                // Send commands
                try
                {
                    //_output.Write(command.ToString());
                    if (command == Commands.FANS_ON)
                    {
                        _output.WriteLine(command.ToString());
                    }
                    
                    Console.WriteLine($"Command {command} sent");

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
                    Console.WriteLine("Reading response.");
                    if (command == Commands.FANS_ON)
                    {
                        _output.Read(buffer, 0, 0);
                    }
                    Console.WriteLine($"Response {buffer} received.");
                    //buffer = NACK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                
                if (buffer == _ACK)
                {
                    Console.WriteLine($"Command {command} sent successfully");

                    _success = true;
                }
                else if (buffer == _NACK || buffer == null)
                {
                    Console.WriteLine($"Command {command} sent unsuccessfully, attempting to resend.");

                    // Attempt to resend the command 5 more times
                    while(_retryCount != 5 && _success == false)
                    {
                        // Try-catch so we don't explode if it fails to send/receive
                        try
                        {
                            _output.WriteLine(command.ToString());

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
                    if (statePair.Value == GreenhouseState.COOLING || statePair.Value == GreenhouseState.HEATING)
                    {
                        StateMachineContainer.Instance.Temperature.CurrentState = statePair.Value;
                    }
                    else if (statePair.Value == GreenhouseState.LIGHTING)
                    {
                        StateMachineContainer.Instance.Lighting.CurrentState = statePair.Value;
                    }
                    else if (statePair.Value == GreenhouseState.WATERING)
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

            byte[] buffer = new byte[8];
            List<Commands> commandsToSend = new List<Commands>();

            // Get the commands we need to send to turn the manual control off
            if (stateMachine is TemperatureStateMachine)
            {
                commandsToSend.Add(Commands.HEAT_OFF);
                commandsToSend.Add(Commands.VENT_CLOSED);
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
                    _output.WriteLine(command.ToString());
                    
                    // Wait for response
                    _output.Read(buffer, 0, 0);
                    buffer = _NACK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }


                if (buffer == _ACK)
                {
                    Console.WriteLine($"Command {command} sent successfully");

                    _success = true;
                }
                else if (buffer == _NACK || buffer == null)
                {
                    Console.WriteLine($"Command {command} sent unsuccessfully, attempting to resend.");

                    // Attempt to resend the command 5 more times
                    while (_retryCount != 5 && _success == false)
                    {
                        // Try-catch so we don't explode if it fails to send/receive
                        try
                        {
                            _output.Write(command.ToString());

                            _output.Read(buffer, 0, 0);
                            buffer = _ACK;
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
