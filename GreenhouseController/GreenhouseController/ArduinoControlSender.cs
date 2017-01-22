using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class ArduinoControlSender : IDisposable
    {
        //private SerialPort _output;
        public ArduinoControlSender()
        {
            // TODO: construct!
            //_output = new SerialPort("/dev/ttyAM0", 9600, Parity.None, 32, StopBits.One);
        }

        public void Dispose()
        {
            // TODO: close sockets and stuff here!
            //_output.Close();
        }

        public void SendCommands(List<GreenhouseCommands> _commandsToSend)
        {
            try
            {
                // TODO: send specified commands to arduino
                //_output.Open();
                foreach(GreenhouseCommands command in _commandsToSend)
                {
                    try
                    {
                        Console.WriteLine($"Attempting to send command {command}");
                        //_output.Write(command.ToString());
                        Console.WriteLine($"Commmand {command} sent successfully");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    try
                    {
                        // TODO: read response from arduino
                        //_output.Read()
                        Console.WriteLine($"Command {command} executed successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex + $"\n Command {command} unsuccessful");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Dispose();
        }
    }
}
