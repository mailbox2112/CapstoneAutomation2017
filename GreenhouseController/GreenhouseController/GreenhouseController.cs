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
            Console.WriteLine($"Temperature State: {StateMachineController.Instance.GetTemperatureCurrentState().ToString()}");
            Console.WriteLine($"Lighting State: {StateMachineController.Instance.GetLightingCurrentState().ToString()}");
            Console.WriteLine($"Watering State: {StateMachineController.Instance.GetWateringCurrentState().ToString()}");
            DataProducer.Instance.ItemInQueue += ItemInQueue;
            Task.WaitAll(Task.Run(new Action(() => DataProducer.Instance.RequestAndReceiveGreenhouseData(buffer))));
        }

        static void ItemInQueue(object sender, DataEventArgs e)
        {
            Task.Run(() => DataConsumer.Instance.ReceiveGreenhouseData(e.Buffer));
        }
    }
}
