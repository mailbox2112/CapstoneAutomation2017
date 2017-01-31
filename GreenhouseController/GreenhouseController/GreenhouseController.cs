using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class GreenhouseController
    {
        // TODO: RESET BUTTON?
        static void Main(string[] args)
        {
            var buffer = new BlockingCollection<byte[]>();
            GreenhouseStateMachine.Instance.Initialize();
            Console.WriteLine($"State: {GreenhouseStateMachine.Instance.CurrentState.ToString()}");
            GreenhouseDataProducer.Instance.ItemInQueue += ItemInQueue;
            Task.WaitAll(Task.Run(new Action(() => GreenhouseDataProducer.Instance.RequestAndReceiveGreenhouseData(buffer))));
        }

        static void ItemInQueue(object sender, DataEventArgs e)
        {
            Task.Run(() => GreenhouseDataConsumer.Instance.ReceiveGreenhouseData(e.Buffer));
        }
    }
}
