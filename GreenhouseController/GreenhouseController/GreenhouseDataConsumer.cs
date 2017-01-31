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
    public class GreenhouseDataConsumer
    {
        private static volatile GreenhouseDataConsumer _instance;
        private static object _syncRoot = new object();
        private byte[] _data;
        private List<DataPacket> _zoneInformation;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private GreenhouseDataConsumer()
        {
            Console.WriteLine("Constructing greenhouse data analyzer...");
            _zoneInformation = new List<DataPacket>();
            Console.WriteLine("Greenhouse data analyzer constructed.\n");
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static GreenhouseDataConsumer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new GreenhouseDataConsumer();
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
            try
            {
                source.TryTake(out _data);
                var deserializedData = JsonConvert.DeserializeObject<DataPacket>(Encoding.ASCII.GetString(_data));
                
                // Check for repeat zones, and if we have any, throw out the old zone data
                if(_zoneInformation.Where(p => p.zone == deserializedData.zone) != null)
                {
                    _zoneInformation.RemoveAll(p => p.zone == deserializedData.zone);
                }

                _zoneInformation.Add(deserializedData);

                if (_zoneInformation.Count == 5)
                {
                    SendDataToAnalyzer(_zoneInformation);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Sends the data off to be analyzed
        /// </summary>
        /// <param name="data">Packet object representing the data contained in JSON sent over TCP</param>
        /// <returns></returns>
        public void SendDataToAnalyzer(List<DataPacket> data)
        {
            DataPacket[] tempZoneInfo = new DataPacket[5];
            data.CopyTo(tempZoneInfo);
            data.Clear();
            // TODO: Add error control! What if we get a packet from a zone we already have, and the values are different?!
            
            GreenhouseActionAnalyzer analyze = new GreenhouseActionAnalyzer();
            Task.Run(() => analyze.AnalyzeData(tempZoneInfo));
        }
    }
}
