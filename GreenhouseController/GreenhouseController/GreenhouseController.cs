using System;
using System.Collections.Concurrent;
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
            var buffer = new BlockingCollection<byte[]>();
            GreenhouseDataProducer.Instance.ItemInQueue += ItemInQueue;
            Task.WaitAll(Task.Run(new Action(() => GreenhouseDataProducer.Instance.RequestAndReceiveGreenhouseData(buffer))));
        }

        static void ItemInQueue(object sender, DataEventArgs e)
        {
            Task.Run(() => GreenhouseDataConsumer.Instance.ReceiveGreenhouseData(e.buffer));
        }
    }
}
