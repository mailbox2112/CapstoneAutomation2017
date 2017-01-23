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
            Task produce = new Task(() => GreenhouseDataProducer.Instance.RequestAndReceiveGreenhouseData(buffer));
            Task consume = new Task(() => GreenhouseDataConsumer.Instance.ReceiveGreenhouseDataAsync(buffer));
            produce.Start();
            consume.Start();
            Task.WaitAll(produce, consume);
        }
    }
}
