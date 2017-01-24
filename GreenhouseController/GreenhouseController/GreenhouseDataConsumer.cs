using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class GreenhouseDataConsumer
    {
        private static volatile GreenhouseDataConsumer instance;
        private static object _syncRoot = new Object();
        private object _lock = new object();
        private byte[] _data;
        private List<Packet> _zoneInformation;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint"></param>
        /// <param name="hostAddress"></param>
        private GreenhouseDataConsumer()
        {
            Console.WriteLine("Constructing greenhouse data analyzer...");
            Console.WriteLine("Greenhouse data analyzer constructed.");
            _zoneInformation = new List<Packet>();
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static GreenhouseDataConsumer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (instance == null)
                        {
                            // TODO: add whatever parameters get passed into construction!
                            instance = new GreenhouseDataConsumer();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Takes in a BlockingCollection and removes data. Sends data to be assessed elsewhere
        /// </summary>
        /// <param name="source"></param>
        public void ReceiveGreenhouseData(BlockingCollection<byte[]> source)
        {
            while (true)
            {
                if (source.Count != 0)
                {
                    try
                    {
                        source.TryTake(out _data);
                        var _deserializedData = JsonConvert.DeserializeObject<Packet>(Encoding.ASCII.GetString(_data));
                        SendDataToAnalyzer(_deserializedData);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the data off to be analyzed
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void SendDataToAnalyzer(Packet data)
        {
            // TODO: Add error control! What if we get a packet from a zone we already have, and the values are different?!
            if(data != null)
            {
                Console.Write($"Greenhouse Zone: {data.zone}\nTemperature: {data.temperature}\nHumidity: {data.humidity} \nLight Intensity: {data.light}\n");
                GreenhouseDataAnalyzer analyze = new GreenhouseDataAnalyzer();
                if (_zoneInformation.Count != 5)
                {
                    _zoneInformation.Add(data);
                }
                else
                {
                    Task.Run(() => analyze.InterpretStateData(_zoneInformation));
                    _zoneInformation.Clear();
                }
            }
        }
    }
}
