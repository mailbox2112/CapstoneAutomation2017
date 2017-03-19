﻿using Newtonsoft.Json;
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
        private List<DataPacket> _zoneInformation;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private PacketConsumer()
        {
            Console.WriteLine("Constructing greenhouse data analyzer...");
            _zoneInformation = new List<DataPacket>(5);
            Console.WriteLine("Greenhouse data analyzer constructed.\n");
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
            if (source.Count != 0)
            {
                try
                {
                    source.TryTake(out _data);
                    var data = JObject.Parse(Encoding.ASCII.GetString(_data));
                    if (data["PacketType"].Value<int>() == 0)
                    {
                        var deserializedData = JsonConvert.DeserializeObject<DataPacket>(Encoding.ASCII.GetString(_data));

                        // Check for repeat zones, and if we have any, throw out the old zone data
                        if (_zoneInformation.Where(p => p.Zone == deserializedData.Zone) != null)
                        {
                            _zoneInformation.RemoveAll(p => p.Zone == deserializedData.Zone);
                        }

                        _zoneInformation.Add(deserializedData);

                        if (_zoneInformation.Count == 5)
                        {
                            SendDataToAnalyzer(_zoneInformation);
                        }
                    }
                    else if (data["PacketType"].Value<int>() == 1)
                    {
                        var deserializedData = JsonConvert.DeserializeObject<LimitPacket>(Encoding.ASCII.GetString(_data));

                        LimitsAnalyzer analyzeLimits = new LimitsAnalyzer();
                        analyzeLimits.ChangeGreenhouseLimits(deserializedData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Sends the data off to be analyzed
        /// </summary>
        /// <param name="data">Packet object representing the data contained in JSON sent over TCP</param>
        /// <returns></returns>
        public void SendDataToAnalyzer(List<DataPacket> data)
        {
            // Copy info to an array so we don't modify list as we use it in another thread
            DataPacket[] tempZoneInfo = new DataPacket[data.Count];
            data.CopyTo(tempZoneInfo);
            data.Clear();
            
            // Send array
            ActionAnalyzer analyze = new ActionAnalyzer();
            Task.Run(() => analyze.AnalyzeData(tempZoneInfo));
        }
    }
}