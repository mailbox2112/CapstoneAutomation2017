using GreenhouseController.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class PacketRequester
    {
        private const string IP = "192.168.1.103";
        private const int PORT = 8888;
        
        // Big byte arrays because there's the potential for the limit packet to go past a kilobyte :O
        private byte[] _buffer = new byte[10024];
        private byte[] _tempBuffer = new byte[10024];
        private NetworkStream _dataStream;
        private TcpClient _client = new TcpClient();
        private List<string> _requests = new List<string>() { "TLH", "MOISTURE", "LIMITS", "MANUAL" };
        private BlockingCollection<byte[]> _queue;
        private System.Timers.Timer _timer;

        public event EventHandler<DataEventArgs> ItemInQueue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostEndpoint">Endpoint to be reached</param>
        /// <param name="hostAddress">IP address we're trying to connect to</param>
        public PacketRequester(BlockingCollection<byte[]> queue, System.Timers.Timer timer)
        {
            Console.WriteLine("Constructing packet requester...");
            _queue = queue;
            _timer = timer;
            Console.WriteLine("Packet requester constructed.\n");
        }

        /// <summary>
        /// Attempt to connect to the server
        /// </summary>
        public void TryConnect()
        {
            while (!_client.Connected)
            {
                try
                {
                    _client = new TcpClient(IP, PORT);
                    //_client = new TcpClient("127.0.0.1", PORT);
                    Console.WriteLine("Connected to data server.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not connect to data server, retrying. {ex.Message}");
                    Thread.Sleep(1000);
                }
            }
            _dataStream = _client.GetStream();
        }

        /// <summary>
        /// Request data and read the response from the server
        /// </summary>
        public void RequestData()
        {
            // Stop the timer, gauranteeing that we won't get multiple requests in if the program hangs
            _timer.Stop();
            foreach(string request in _requests)
            {
                // Try connecting to the socket
                TryConnect();
                ArduinoControlSender.Instance.CheckArduinoStatus();
                Console.WriteLine($"\nRequesting {request}...");

                // Get the bytes from the JSON to send, then write the data to the socket
                byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request));
                _dataStream.Write(data, 0, data.Length);
                _dataStream.Flush();
                Console.WriteLine("Request sent!\n");

                // Read the incoming data, try 5 times if need be, then close the socket
                //while (retryCount != 5 && success == false)
                //{
                ReadGreenhouseData(request);
                //}
                
                // Make sure we're still connected to the socket before trying to close it
                if (_client.Connected)
                {
                    _client.Close();
                }
            }

            // Send the hardware state after all the data is processed
            TryConnect();
            byte[] hardwareBytes = Encoding.ASCII.GetBytes(ListToJson(GetGreenhouseStates()));
            _dataStream.Write(hardwareBytes, 0, hardwareBytes.Length);
            _dataStream.Flush();
            _client.Close();

            // Restart the timer now
            _timer.Start();
        }

        /// <summary>
        /// Read greenhouse data from the server
        /// </summary>
        public void ReadGreenhouseData(string type)
        {
            // Read the data in response the request for data
            try
            {
                if (_client.Connected)
                {
                    // Read data and copy it to a temporary array/buffer
                    _dataStream.Read(_buffer, 0, _buffer.Length);
                    Console.WriteLine("Data received.");
                    Array.Copy(sourceArray: _buffer, destinationArray: _tempBuffer, length: _buffer.Length);

                    // Try to add it to the blocking collection
                    _queue.TryAdd(_tempBuffer);

                    // Hook up a temporary event handler and trigger the event
                    EventHandler<DataEventArgs> handler = ItemInQueue;
                    handler(this, new DataEventArgs() { Buffer = _queue});

                    // Clear the buffer
                    Array.Clear(_buffer, 0, _buffer.Length);
                }
            }
            // Should we lose the connection, we get rid of the socket, try to start a new one,
            // and try to connect to it.
            catch (Exception ex)
            {
                // Write the exception for debugging purposes
                Console.WriteLine(ex.Message);

                // Close the connection and create a new client
                _client.Close();
                TryConnect();
            }
        }

        /// <summary>
        /// Private method to get a list of anything that's on
        /// </summary>
        /// <returns></returns>
        private List<GreenhouseState> GetGreenhouseStates()
        {
            List<GreenhouseState> states = new List<GreenhouseState>();
            
            // Add the temperature state if it's heating
            if (StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.HEATING 
                || StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.COOLING)
            {
                states.Add(StateMachineContainer.Instance.Temperature.CurrentState);
            }

            if (StateMachineContainer.Instance.Shading.CurrentState == GreenhouseState.SHADING)
            {
                states.Add(StateMachineContainer.Instance.Shading.CurrentState);
            }

            // Add a watering state if it's watering
            foreach(WateringStateMachine stateMachine in StateMachineContainer.Instance.WateringStateMachines)
            {
                if (stateMachine.CurrentState == GreenhouseState.WATERING && !states.Contains(GreenhouseState.WATERING))
                {
                    states.Add(GreenhouseState.WATERING);
                }
            }

            // Add a lighting state if it's lighting
            foreach(LightingStateMachine stateMachine in StateMachineContainer.Instance.LightStateMachines)
            {
                if (stateMachine.CurrentState == GreenhouseState.LIGHTING && !states.Contains(GreenhouseState.LIGHTING))
                {
                    states.Add(GreenhouseState.LIGHTING);
                }
            }

            return states;
        }

        private string ListToJson(List<GreenhouseState> states)
        {
            HardwareStatePacket packet = new HardwareStatePacket()
            {
                heater = false.ToString().ToLower(),
                lights = false.ToString().ToLower(),
                vents = false.ToString().ToLower(),
                pump = false.ToString().ToLower(),
                fans = false.ToString().ToLower(),
                shades = false.ToString().ToLower()
            };

            foreach(GreenhouseState state in states)
            {
                switch(state)
                {
                    case GreenhouseState.WATERING:
                        packet.pump = true.ToString().ToLower();
                        break;
                    case GreenhouseState.LIGHTING:
                        packet.lights = true.ToString().ToLower();
                        break;
                    case GreenhouseState.HEATING:
                        packet.heater = true.ToString().ToLower();
                        packet.fans = true.ToString().ToLower();
                        break;
                    case GreenhouseState.COOLING:
                        packet.fans = true.ToString().ToLower();
                        packet.vents = true.ToString().ToLower();
                        break;
                    case GreenhouseState.SHADING:
                        packet.shades = true.ToString().ToLower();
                        break;
                    default:
                        break;
                }
            }

            string json = JsonConvert.SerializeObject(packet);
            return json;
        }
    }
}
