using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class ArduinoControlSender : IDisposable
    {
        // TODO: ERROR CONTROL!
        private bool _success = false;
        private int _retryCount = 0;
        private byte[] ACK = new byte[] { 1 };
        private byte[] NACK = new byte[] { 0 };

        private SerialPort _output;
        public ArduinoControlSender()
        {
            // TODO: construct!
            // _output = new SerialPort("/dev/ttyAMA0", 115200, Parity.None, 8, StopBits.One);
        }

        public void Dispose()
        {
            // TODO: close sockets and stuff here!
            // _output.Close();
        }

        /// <summary>
        /// Takes a list of commands to be sent to the arduino and sends them over the pi's serial port
        /// </summary>
        /// <param name="_commandsToSend">List of GreenhouseCommands from the enum</param>
        public void SendCommand(List<GreenhouseState> statesToSend)
        {
            byte[] buffer = new byte[8];
            List<Commands> commandsToSend = new List<Commands>();

            // TODO: Fix state machine here. Should enter the sending state just as we send to the serial port.
            // Also, what happens if one of the commands fails but others are fine? retry, and make state machine 
            // behave properly in that case.  How do we tell if one of the commands failed and we need to retry?

            // _output.Open();
            foreach (var state in statesToSend)
            {
                if (state == GreenhouseState.COOLING || state == GreenhouseState.HEATING)
                {
                    StateMachineContainer.Instance.Temperature.CurrentState = GreenhouseState.SENDING_DATA;
                    commandsToSend = StateMachineContainer.Instance.Temperature.ConvertStateToCommands(state);
                }
                else if (state == GreenhouseState.LIGHTING)
                {
                    StateMachineContainer.Instance.Lighting.CurrentState = GreenhouseState.SENDING_DATA;
                    commandsToSend = StateMachineContainer.Instance.Lighting.ConvertStateToCommands(state);
                }
                else if (state == GreenhouseState.WATERING)
                {
                    StateMachineContainer.Instance.Watering.CurrentState = GreenhouseState.SENDING_DATA;
                    commandsToSend = StateMachineContainer.Instance.Watering.ConvertStateToCommands(state);
                }
                Console.WriteLine($"Attempting to send state {state}");

                foreach (var command in commandsToSend)
                {
                    // Send commands
                    try
                    {
                        //_output.Write(command.ToString());

                        // TODO: Move this somewhere that makes sense. Do we change state for each command sent? Probably
                        if (state == GreenhouseState.COOLING || state == GreenhouseState.HEATING)
                        {
                            StateMachineContainer.Instance.Temperature.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;
                        }
                        else if (state == GreenhouseState.LIGHTING)
                        {
                            StateMachineContainer.Instance.Lighting.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;
                        }
                        else if (state == GreenhouseState.WATERING)
                        {
                            StateMachineContainer.Instance.Watering.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    // Wait for response
                    try
                    {
                        //_output.Read(buffer, 0, 0);
                        buffer = ACK;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }


                    if (buffer == ACK)
                    {
                        Console.WriteLine($"Command {command} sent successfully");

                        if (state == GreenhouseState.COOLING || state == GreenhouseState.HEATING)
                        {
                            StateMachineContainer.Instance.Temperature.CurrentState = state;
                        }
                        else if (state == GreenhouseState.LIGHTING)
                        {
                            StateMachineContainer.Instance.Lighting.CurrentState = state;
                        }
                        else if (state == GreenhouseState.WATERING)
                        {
                            StateMachineContainer.Instance.Watering.CurrentState = state;
                        }
                    }
                    else if (buffer == NACK || buffer == null)
                    {
                        Console.WriteLine($"Command {command} sent unsuccessfully.");
                    }
                }

                Console.WriteLine($"State change {state} executed successfully\n");
                _success = true;
            }
        }
    }
}
