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
        private SerialPort _output;
        public ArduinoControlSender()
        {
            // TODO: construct!
            _output = new SerialPort("/dev/ttyAMA0", 115200, Parity.None, 8, StopBits.One);
        }

        public void Dispose()
        {
            // TODO: close sockets and stuff here!
            _output.Close();
        }

        public void SendCommands(List<GreenhouseCommands> _commandsToSend)
        {
            byte[] buffer = new byte[8];
            try
            {
                // TODO: send specified commands to arduino
                _output.Open();
                foreach(GreenhouseCommands command in _commandsToSend)
                {
                    try
                    {
                        Console.WriteLine($"Attempting to send command {command}");
                        _output.Write(command.ToString());
                        Console.WriteLine($"Commmand {command} sent successfully");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    try
                    {
                        // TODO: read response from arduino
                        int response = _output.Read(buffer, 0, 8);
                        Console.WriteLine($"Command {command} executed successfully");
                        Console.WriteLine($"{response}");
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
