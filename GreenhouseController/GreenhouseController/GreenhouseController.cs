using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class GreenhouseController
    {
        static void Main(string[] args)
        {
            // TODO: get real flag that greenhouse is running!
            bool greenhouseOperational = true;
            bool actionInProgress = false;
            GreenhouseState[] currentAction;
            while (greenhouseOperational)
            {
                byte[] data = DataReceiver.Instance.RequestAndReceiveGreenhouseData();
                if(actionInProgress == false)
                {
                    currentAction = GreenhouseStateAnalyzer.Instance.AssessGreenhouseState(data);
                }
                else
                {
                    currentAction = null;
                }
                if(currentAction != null)
                {
                    actionInProgress = true;
                    using (ArduinoControlSender sender = new ArduinoControlSender())
                    {
                        try
                        {
                            sender.SendCommands(currentAction);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(300000);
                }
                // TODO: implement some way to know when action is done
            }
        }
    }
}
