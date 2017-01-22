using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class ArduinoControlSender : IDisposable
    {
        private List<GreenhouseCommands> _commandsToSend;
        public ArduinoControlSender()
        {
            // TODO: construct!
        }

        public void Dispose()
        {
            // TODO: close sockets and stuff here!
        }

        public async void SendCommands(GreenhouseState[] state)
        {
            // Determine what commands to send
            foreach (GreenhouseState s in state)
            {
                // Decision logic!
            }
            try
            {
                // TODO: send packets of commands to arduino
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Dispose();
        }
    }
}
