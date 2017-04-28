using GreenhouseController.Data;
using GreenhouseController.Packets;
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
            // Create a container for the TLH and moisture packets so we can deserialize them easily
            TLHPacketContainer tlhContainer = new TLHPacketContainer();
            MoisturePacketContainer moistureContainer = new MoisturePacketContainer();

            // Take the bytes out of the queue and turn them back into a string
            source.TryTake(out _data);
            string json = Encoding.ASCII.GetString(_data);

            // Take the string to a JObject and deserialize according to the appropriate Type value
            JObject received = JObject.Parse(json);
            Console.WriteLine(received.ToString());
            switch (received["Type"].Value<int>())
            {
                case 0:
                    tlhContainer = JsonConvert.DeserializeObject<TLHPacketContainer>(json);
                    _tlhInformation = tlhContainer.Packets;
                    break;
                case 1:
                    moistureContainer = JsonConvert.DeserializeObject<MoisturePacketContainer>(json);
                    _moistureInformation = moistureContainer.Packets;
                    break;
                case 2:
                    _limits = JsonConvert.DeserializeObject<LimitPacket>(json);
                    break;
                case 3:
                    _manual = JsonConvert.DeserializeObject<ManualPacket>(json);
                    break;
            }

            // If we have all the TLH information, moisture information, limit and manual information we need...
            if (_tlhInformation != null && _moistureInformation != null && _limits != null && _manual != null)
            {
                Console.WriteLine("Sending to analyzers");

                // Put everything into temporary variables and clear their values afterwards
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

                // Send the temporary variables off to be analyzed
                DataAnalyzer data = new DataAnalyzer();
                data.ExecuteActions(tlhToSend, moistureToSend, tempManual, tempLimits);
            }
        }
    }
}
