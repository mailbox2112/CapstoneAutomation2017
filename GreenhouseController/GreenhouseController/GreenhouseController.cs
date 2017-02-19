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
        public static CancellationToken cancel = new CancellationToken();
        // TODO: RESET BUTTON?
        static void Main(string[] args)
        {
            BlockingCollection<byte[]> buffer = new BlockingCollection<byte[]>();
            
            Console.WriteLine($"Temperature State: {StateMachineContainer.Instance.Temperature.CurrentState.ToString()}");
            Console.WriteLine($"Lighting State: {StateMachineContainer.Instance.Lighting.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.Watering.CurrentState.ToString()}");
            DataProducer.Instance.ItemInQueue += ItemInQueue;
            Task.WaitAll(Task.Run(new Action(() => DataProducer.Instance.ReadGreenhouseData(buffer)), cancellationToken: cancel));
        }

        static void ItemInQueue(object sender, DataEventArgs e)
        {
            Task.Run(() => DataConsumer.Instance.ReceiveGreenhouseData(e.Buffer), cancellationToken: cancel);
        }
    }
}
