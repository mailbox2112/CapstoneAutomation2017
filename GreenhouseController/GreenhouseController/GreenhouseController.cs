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
            Console.WriteLine($"Temperature State: {StateMachineContainer.Instance.Temperature.CurrentState.ToString()}");
            Console.WriteLine($"Lighting State: {StateMachineContainer.Instance.Lighting.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.Watering.CurrentState.ToString()}");
            StateMachineContainer.Instance.Lighting.StateChanged += StateChanged;
            StateMachineContainer.Instance.Temperature.StateChanged += StateChanged;
            StateMachineContainer.Instance.Watering.StateChanged += StateChanged;
            DataProducer.Instance.ItemInQueue += ItemInQueue;
            Task.WaitAll(Task.Run(new Action(() => DataProducer.Instance.ReadGreenhouseData(buffer))));
        }

        static void ItemInQueue(object sender, DataEventArgs e)
        {
            Task.Run(() => DataConsumer.Instance.ReceiveGreenhouseData(e.Buffer));
        }
        
        static void StateChanged(object sender, StateEventArgs e)
        {
            Console.WriteLine($"{sender}: {e.State}");
        }
    }
}
