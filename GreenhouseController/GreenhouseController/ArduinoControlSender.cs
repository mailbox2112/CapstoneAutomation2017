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
        private SerialPort _output;
        public ArduinoControlSender()
        {
            // TODO: construct!
            //_output = new SerialPort("/dev/ttyAMA0", 115200, Parity.None, 8, StopBits.One);
        }

        public void Dispose()
        {
            // TODO: close sockets and stuff here!
            //_output.Close();
        }

        /// <summary>
        /// Takes a list of commands to be sent to the arduino and sends them over the pi's serial port
        /// </summary>
        /// <param name="_commandsToSend">List of GreenhouseCommands from the enum</param>
        public void SendCommands(GreenhouseState stateToSend)
        {
            byte[] buffer = new byte[8];
            
            // TODO: send specified commands to arduino
            //_output.Open();
           
            try
            {
                Console.WriteLine($"Attempting to send state {stateToSend}");
                //_output.Write(command.ToString());
                Console.WriteLine($"State {stateToSend} sent successfully");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            try
            {
                // TODO: read response from arduino
                //int response = _output.Read(buffer, 0, 8);
                Console.WriteLine($"State {stateToSend} executed successfully\n");
                //Console.WriteLine($"{response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + $"\n State {stateToSend} unsuccessful\n");
            }

            Dispose();
        }
    }
}
