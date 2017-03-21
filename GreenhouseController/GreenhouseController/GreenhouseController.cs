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
            Console.WriteLine($"Lighting Zone 1 State: {StateMachineContainer.Instance.LightingZone1.CurrentState.ToString()}");
            Console.WriteLine($"Lighting Zone 3 State: {StateMachineContainer.Instance.LightingZone3.CurrentState.ToString()}");
            Console.WriteLine($"Lighting Zone 5 State: {StateMachineContainer.Instance.LightingZone5.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.WateringZone1.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.WateringZone2.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.WateringZone3.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.WateringZone4.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.WateringZone5.CurrentState.ToString()}");
            Console.WriteLine($"Watering State: {StateMachineContainer.Instance.WateringZone6.CurrentState.ToString()}");

            // Event handlers for printing state changes
            StateMachineContainer.Instance.LightingZone1.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.LightingZone3.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.LightingZone5.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.Temperature.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.WateringZone1.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.WateringZone2.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.WateringZone3.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.WateringZone4.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.WateringZone5.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            StateMachineContainer.Instance.WateringZone6.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };

            // Event handlers for when blocking collections get data
            NetworkListener.Instance.ItemInQueue += (o, i) => { Task.Run(() => PacketConsumer.Instance.ReceiveGreenhouseData(i.Buffer)); };

            var time = new System.Timers.Timer();
            time.Interval = 30000;
            time.Elapsed += (o, i) => { NetworkListener.Instance.RequestData(); };
            time.AutoReset = true;
            time.Enabled = true;
            GC.KeepAlive(time);
            
            // Listens for any data that comes in, be it sensor data or control data
            Task.WaitAll(Task.Run(new Action(() => NetworkListener.Instance.ReadGreenhouseData(dataBuffer))));
        }
    }
}
