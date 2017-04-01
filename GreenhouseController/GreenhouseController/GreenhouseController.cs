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
            var dataBuffer = new BlockingCollection<byte[]>();
            PacketRequester packetListener = new PacketRequester(dataBuffer);
            PacketConsumer packetConsumer = new PacketConsumer();

            // Connect to the server
            packetListener.TryConnect();

            // Print out the state of the state machine at the start of the program
            Console.WriteLine($"Temperature State: {StateMachineContainer.Instance.Temperature.CurrentState.ToString()}");
            Console.WriteLine($"Shading State: {StateMachineContainer.Instance.Shading.CurrentState.ToString()}");
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
            StateMachineContainer.Instance.Shading.StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            for (int j = 0; j < StateMachineContainer.Instance.LightStateMachines.Count; j++)
            {
                StateMachineContainer.Instance.LightStateMachines[j].StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            }
            for (int j = 0; j < StateMachineContainer.Instance.WateringStateMachines.Count; j++)
            {
                StateMachineContainer.Instance.WateringStateMachines[j].StateChanged += (o, i) => { Console.WriteLine($"{o}: {i.State}"); };
            }

            // Event handlers for when blocking collections get data
            packetListener.ItemInQueue += (o, i) => { packetConsumer.ReceiveGreenhouseData(i.Buffer, i.Type); };
            
            // Timer for requesting sensor data
            var time = new System.Timers.Timer();
            time.Interval = 10000;
            time.Elapsed += (o, i) => { packetListener.RequestData(); };
            time.AutoReset = true;
            time.Enabled = true;
            GC.KeepAlive(time);

            // Listens for any data that comes in, be it sensor data or control data
            Console.ReadLine();
        }
    }
}
