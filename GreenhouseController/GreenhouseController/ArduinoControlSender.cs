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

            // TODO: send specified commands to arduino
            // _output.Open();
            foreach (var state in statesToSend)
            { 
                try
                {
                    if (state == GreenhouseState.COOLING || state == GreenhouseState.HEATING)
                    {
                        StateMachineContainer.Instance.Temperature.CurrentState = GreenhouseState.SENDING_DATA;
                    }
                    else if (state == GreenhouseState.LIGHTING)
                    {
                        StateMachineContainer.Instance.Lighting.CurrentState = GreenhouseState.SENDING_DATA;
                    }
                    else if (state == GreenhouseState.WATERING)
                    {
                        StateMachineContainer.Instance.Watering.CurrentState = GreenhouseState.SENDING_DATA;
                    }
                    Console.WriteLine($"Attempting to send state {state}");

                    // _output.Write(state.ToString());

                    Console.WriteLine($"State {state} sent successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                try
                {
                    // TODO: read response from arduino
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

                    // Error control
                    // int response = _output.Read(buffer, 0, 8);
                    // if (response is bad) 
                    // {
                    //      _retryCount++;
                    //      retry;
                    //      state == ERROR
                    // }

                    Console.WriteLine($"State {state} executed successfully\n");

                    // Console.WriteLine($"{response}");

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

                    _success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + $"\n State {state} unsuccessful\n");
                }
            }
        }
    }
}
