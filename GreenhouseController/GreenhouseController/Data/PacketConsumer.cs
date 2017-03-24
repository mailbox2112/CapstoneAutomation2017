using GreenhouseController.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class PacketConsumer
    {
        private static volatile PacketConsumer _instance;
        private static object _syncRoot = new object();
        private byte[] _data;
        private List<TLHPacket> _tlhInformation;
        private List<MoisturePacket> _moistureInformation;
        private DateTime _currentTime;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private PacketConsumer()
        {
            Console.WriteLine("Constructing greenhouse data analyzer...");
            Console.WriteLine("Greenhouse data analyzer constructed.\n");
            _tlhInformation = new List<TLHPacket>();
            _moistureInformation = new List<MoisturePacket>();
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static PacketConsumer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new PacketConsumer();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Takes in a BlockingCollection and removes data. Sends data to be assessed elsewhere
        /// </summary>
        /// <param name="source">Blocking collection used to hold data for producer consumer pattern</param>
        public void ReceiveGreenhouseData(BlockingCollection<byte[]> source)
        {
            // TODO: Fix this up so that it doesn't mess with things already happening within state machines, etc.
            if (source.Count != 0)
            {
                try
                {
                    source.TryTake(out _data);
                    var data = JObject.Parse(Encoding.ASCII.GetString(_data));

                    //Console.WriteLine(data.ToString());
                    

                    if (data["Type"].Value<int>() == 0)
                    {
                        _currentTime = data["TimeOfSend"].Value<DateTime>();
                        var deserializedData = JsonConvert.DeserializeObject<TLHPacket>(Encoding.ASCII.GetString(_data));

                        // Check for repeat zones, and if we have any, throw out the old zone data
                        if (_tlhInformation.Where(p => p.ID == deserializedData.ID) != null)
                        {
                            _tlhInformation.RemoveAll(p => p.ID == deserializedData.ID);
                        }

                        _tlhInformation.Add(deserializedData);
                    }
                    // if it's a moisture packet
                    else if (data["Type"].Value<int>() == 1)
                    {
                        var deserializedData = JsonConvert.DeserializeObject<MoisturePacket>(Encoding.ASCII.GetString(_data));

                        // Check for repeat zones, and if we have any, throw out the old zone data
                        if (_moistureInformation.Where(p => p.ID == deserializedData.ID) != null)
                        {
                            _moistureInformation.RemoveAll(p => p.ID == deserializedData.ID);
                        }

                        _moistureInformation.Add(deserializedData);
                    }
                    else if (data["Type"].Value<int>() == 2)
                    {
                        var deserializedData = JsonConvert.DeserializeObject<LimitPacket>(Encoding.ASCII.GetString(_data));

                        LimitsAnalyzer analyzeLimits = new LimitsAnalyzer();
                        analyzeLimits.ChangeGreenhouseLimits(deserializedData);
                    }
                    else if (data["Type"].Value<int>() == 3)
                    {
                        var deserializedData = JsonConvert.DeserializeObject<ManualPacket>(Encoding.ASCII.GetString(_data));

                        ManualControlAnalyzer analyzePacket = new ManualControlAnalyzer();
                        analyzePacket.SetManualValues(deserializedData);
                        ActionAnalyzer analyzer = new ActionAnalyzer();
                        analyzer.ActivateManualControl();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                if (_tlhInformation.Count == 5 && _moistureInformation.Count == 6)
                {
                    SendDataToAnalyzer(_tlhInformation, _moistureInformation);
                }
            }
        }

        /// <summary>
        /// Sends the data off to be analyzed
        /// </summary>
        /// <param name="data">Packet object representing the data contained in JSON sent over TCP</param>
        /// <returns></returns>
        public void SendDataToAnalyzer(List<TLHPacket> tlhData, List<MoisturePacket> moistureData)
        {
            // Copy info to an array so we don't modify list as we use it in another thread
            TLHPacket[] tempZoneInfo = new TLHPacket[tlhData.Count];
            tlhData.CopyTo(tempZoneInfo);
            tlhData.Clear();

            MoisturePacket[] moistZoneInfo = new MoisturePacket[moistureData.Count];
            moistureData.CopyTo(moistZoneInfo);
            moistureData.Clear();

            // Send array
            ActionAnalyzer analyze = new ActionAnalyzer();
            analyze.AnalyzeData(tempZoneInfo, moistZoneInfo, _currentTime);
        }
    }
}
