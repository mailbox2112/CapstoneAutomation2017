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
        private byte[] _data;
        private List<TLHPacket> _tlhInformation;
        private List<MoisturePacket> _moistureInformation;
        private ManualPacket _manual;
        private LimitPacket _limits;
        private DateTime _currentTime;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        public PacketConsumer()
        {
            Console.WriteLine("Constructing greenhouse data analyzer...");
            Console.WriteLine("Greenhouse data analyzer constructed.\n");
            _tlhInformation = new List<TLHPacket>();
            _moistureInformation = new List<MoisturePacket>();
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
                Console.WriteLine(Encoding.ASCII.GetString(_data));
                var data = JObject.Parse(Encoding.ASCII.GetString(_data));

                Console.WriteLine(data.ToString());
                    

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

                    _limits = deserializedData;
                }
                else if (data["Type"].Value<int>() == 3)
                {
                    var deserializedData = JsonConvert.DeserializeObject<ManualPacket>(Encoding.ASCII.GetString(_data));

                    _manual = deserializedData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (_tlhInformation.Count == 5 && _moistureInformation.Count == 6 && _limits != null && _manual != null)
            {
                TLHPacket[] tlhToSend = new TLHPacket[_tlhInformation.Count];
                _tlhInformation.CopyTo(tlhToSend);
                _tlhInformation.Clear();

                MoisturePacket[] moistureToSend = new MoisturePacket[_moistureInformation.Count];
                _moistureInformation.CopyTo(moistureToSend);
                _moistureInformation.Clear();

                ManualPacket tempManual = _manual;
                _manual = null;
                LimitPacket tempLimits = _limits;
                _limits = null;

                DataAnalyzer data = new DataAnalyzer();
                Task.Run(() => data.ExecuteActions(tlhToSend, moistureToSend, tempManual, tempLimits));
            }
        }
    }
}
