using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class ArduinoControlSenderSimulator
    {
        public ArduinoControlSenderSimulator() { }

        /// <summary>
        /// Takes a list of commands to be sent to the arduino and sends them over the pi's serial port
        /// </summary>
        /// <param name="_commandsToSend">List of GreenhouseCommands from the enum</param>
        public void SendCommand(GreenhouseState state, IStateMachine stateMachine)
        {
            try
            {
                stateMachine.CurrentState = GreenhouseState.SENDING_DATA;
                Console.WriteLine($"Attempting to send state {stateMachine.CurrentState}");
                Console.WriteLine($"State {stateMachine.CurrentState} sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            try
            {
                stateMachine.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;
                Console.WriteLine($"State {stateMachine.CurrentState} executed successfully\n");
                stateMachine.CurrentState = state;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + $"\n State {stateMachine.CurrentState} unsuccessful\n");
            }
        }
    }
}
