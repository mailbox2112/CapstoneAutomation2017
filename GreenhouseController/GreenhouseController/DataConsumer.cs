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
    public class DataConsumer
    {
        private static volatile DataConsumer _instance;
        private static object _syncRoot = new object();
        private byte[] _data;
        private List<DataPacket> _zoneInformation;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private DataConsumer()
        {
            Console.WriteLine("Constructing greenhouse data analyzer...");
            _zoneInformation = new List<DataPacket>(5);
            Console.WriteLine("Greenhouse data analyzer constructed.\n");
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static DataConsumer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new DataConsumer();
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
                    var deserializedData = JsonConvert.DeserializeObject<DataPacket>(Encoding.ASCII.GetString(_data));

                    if (deserializedData.Zone == 0)
                    {
                        SendLimitsToAnalyzer(deserializedData);
                    }
                    else
                    {
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

        /// <summary>
        /// If we receive a packet of just limits, use this to find them
        /// </summary>
        /// <param name="limits"></param>
        public void SendLimitsToAnalyzer(DataPacket limits)
        {
            ActionAnalyzer analyzeLimits = new ActionAnalyzer();
            analyzeLimits.GetGreenhouseLimits(limits);
        }
    }
}
