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
            // Create the blocking collection
            var buffer = new BlockingCollection<byte[]>();

            // Print out the state of the state machine at the start of the program
            Console.WriteLine($"Temperature State: {StateMachineContainer.Instance.Temperature.CurrentState.ToString()}");
            Console.WriteLine($"Lighting State: {StateMachineContainer.Instance.Lighting.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.Watering.CurrentState.ToString()}");

            // Event handlers for printing state changes
            StateMachineContainer.Instance.Lighting.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.Temperature.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.Watering.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };

            // Event handler for when blocking collection gets data
            DataProducer.Instance.ItemInQueue += (o, i) => { Task.Run(() => DataConsumer.Instance.ReceiveGreenhouseData(i.Buffer)); };

            // Start the data producer task
            //DataRequestTimer dataRequester = new DataRequestTimer(buffer);
            
            // TODO: replace this with a timer task
            Task.WaitAll(Task.Run(new Action(() => DataProducer.Instance.ReadGreenhouseData(buffer))));
        }
    }
}
