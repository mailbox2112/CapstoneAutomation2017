using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class ArduinoControlSenderSimulator : IDisposable
    {
        public ArduinoControlSenderSimulator() { }

        public void Dispose() { }

        /// <summary>
        /// Takes a list of commands to be sent to the arduino and sends them over the pi's serial port
        /// </summary>
        /// <param name="_commandsToSend">List of GreenhouseCommands from the enum</param>
        public void SendCommand(IStateMachine stateMachine)
        {
            try
            {
                stateMachine.CurrentState = GreenhouseState.SENDING_DATA;
                Console.WriteLine($"Attempting to send state {stateMachine.EndState}");
                Console.WriteLine($"State {stateMachine.EndState} sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            try
            {
                stateMachine.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;
                Console.WriteLine($"State {stateMachine.EndState} executed successfully\n");
                stateMachine.CurrentState = stateMachine.EndState;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + $"\n State {stateMachine.EndState} unsuccessful\n");
            }
        }
    }
}
