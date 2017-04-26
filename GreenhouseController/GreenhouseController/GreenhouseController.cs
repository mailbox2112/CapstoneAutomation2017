using GreenhouseController.API;
using GreenhouseController.APICallers;
using Newtonsoft.Json;
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
            // Connect to the Arduino. Connecting here prevents a 2 second delay before the first commands are sent
            ArduinoControlSender.Instance.TryConnect(createNewPort: true);
            ArduinoControlSender.Instance.CheckArduinoStatus();

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

            // Sync object for event
            object syncRoot = new object();

            // Timer for requesting sensor data
            var time = new System.Timers.Timer();

            // Create the blocking collection
            //var dataBuffer = new BlockingCollection<byte[]>();
            //PacketRequester packetListener = new PacketRequester(dataBuffer, time);
            //PacketConsumer packetConsumer = new PacketConsumer();
            APICaller apiCaller = new APICaller();
            APIDataHandler dataHandler = new APIDataHandler(apiCaller);

            // We lock the data analyzer object, because the timer gets put on a separate thread by default
            // So by locking the data analyzer, we can avoid simultaneous operations that would cause B A D things to happen
            // Also, since there's no longer multithreading in the operations, the lock gaurantees there's no unwanted simultaneous operations
            time.Interval = 5000;
            //time.Elapsed += (o, i) => { lock (syncRoot) { packetListener.RequestData(); } };
            time.AutoReset = true;
            time.Elapsed += (o, i) =>
            {
                lock (syncRoot)
                {
                    dataHandler.RequestDataFromAPI();
                }
            };
            
            // Event handler for when blocking collections get data
            //packetListener.ItemInQueue += (o, i) => { packetConsumer.ReceiveGreenhouseData(i.Buffer); };
            
            // Start the timer and keep the OS from garbage collecting it since it runs indefinitely
            time.Start();
            GC.KeepAlive(time);

            // Just keeps the program from ending. Really not the best way, but hey, it works
            Console.ReadLine();
        }
    }
}
