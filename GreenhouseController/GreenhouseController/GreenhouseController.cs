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
            StateMachineContainer.Instance.Lighting.StateChanged += StateChanged;
            StateMachineContainer.Instance.Temperature.StateChanged += StateChanged;
            StateMachineContainer.Instance.Watering.StateChanged += StateChanged;

            // Event handler for when blocking collection gets data
            DataProducer.Instance.ItemInQueue += (o, i) => { Task.Run(() => DataConsumer.Instance.ReceiveGreenhouseData(i.Buffer)); };

            // Start the data producer task
            Task.WaitAll(Task.Run(new Action(() => DataProducer.Instance.ReadGreenhouseData(buffer))));
        }

        //static void ItemInQueue(object sender, DataEventArgs e)
        //{
        //    Task.Run(() => DataConsumer.Instance.ReceiveGreenhouseData(e.Buffer));
        //}
        
        static void StateChanged(object sender, StateEventArgs e)
        {
            Console.WriteLine($"{sender}: {e.State}");
        }
    }
}
