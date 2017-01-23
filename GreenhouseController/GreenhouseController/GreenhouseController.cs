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
            var buffer = new Queue<byte[]>();
            while (greenhouseOperational)
            {
                // TODO: implement some way to know when action is done
                GreenhouseDataConsumer.Instance.ReceiveGreenhouseDataAsync(buffer);
                GreenhouseDataProducer.Instance.RequestAndReceiveGreenhouseData(buffer);
            }
        }
    }
}
