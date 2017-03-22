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
            int[] moistureZones = new int[] { 1, 3, 5 };
            

            // Print out the state of the state machine at the start of the program
            Console.WriteLine($"Temperature State: {StateMachineContainer.Instance.Temperature.CurrentState.ToString()}");
            for(int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i++)
            {
                Console.WriteLine($"Lighting Zone {StateMachineContainer.Instance.LightStateMachines[i].Zone}"
                    + $"State: {StateMachineContainer.Instance.LightStateMachines[i].CurrentState.ToString()}");
            }
            for(int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i++)
            {
                Console.WriteLine($"Watering Zone {StateMachineContainer.Instance.WateringStateMachines[i].Zone}"
                    + $"State: {StateMachineContainer.Instance.WateringStateMachines[i].CurrentState.ToString()}");
            }

            // Event handlers for printing state changes
            StateMachineContainer.Instance.Temperature.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            for (int j = 0; j < StateMachineContainer.Instance.LightStateMachines.Count; j++)
            {
                StateMachineContainer.Instance.LightStateMachines[j].StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            }
            for (int j = 0; j < StateMachineContainer.Instance.WateringStateMachines.Count; j++)
            {
                StateMachineContainer.Instance.WateringStateMachines[j].StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            }

            // Event handlers for when blocking collections get data
            NetworkListener.Instance.ItemInQueue += (o, i) => { Task.Run(() => PacketConsumer.Instance.ReceiveGreenhouseData(i.Buffer)); };

            // Timer for requesting sensor data
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
