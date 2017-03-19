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
            // Connect to the server
            NetworkListener.Instance.TryConnect();
            // Create the blocking collection
            var dataBuffer = new BlockingCollection<byte[]>();

            // Print out the state of the state machine at the start of the program
            Console.WriteLine($"Temperature State: {StateMachineContainer.Instance.Temperature.CurrentState.ToString()}");
            Console.WriteLine($"Lighting State: {StateMachineContainer.Instance.Lighting.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.Watering.CurrentState.ToString()}");

            // Event handlers for printing state changes
            StateMachineContainer.Instance.Lighting.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.Temperature.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.Watering.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            
            // Event handlers for when blocking collections get data
            NetworkListener.Instance.ItemInQueue += (o, i) => { Task.Run(() => PacketConsumer.Instance.ReceiveGreenhouseData(i.Buffer)); };

            var time = new System.Timers.Timer();
            time.Interval = 15000;
            time.Elapsed += (o, i) => { NetworkListener.Instance.RequestData(); };
            time.AutoReset = true;
            time.Enabled = true;
            GC.KeepAlive(time);
            
            // Listens for any data that comes in, be it sensor data or control data
            Task.WaitAll(Task.Run(new Action(() => NetworkListener.Instance.ReadGreenhouseData(dataBuffer))));
        }
    }
}
