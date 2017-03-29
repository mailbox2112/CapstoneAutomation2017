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
        public void ReceiveGreenhouseData(BlockingCollection<byte[]> source, string type)
        {
            try
            {
                source.TryTake(out _data);
                switch(type)
                {
                    case "TLH":
                        _tlhInformation = JsonConvert.DeserializeObject<List<TLHPacket>>(Encoding.ASCII.GetString(_data));
                        break;
                    case "MOISTURE":
                        _moistureInformation = JsonConvert.DeserializeObject<List<MoisturePacket>>(Encoding.ASCII.GetString(_data));
                        break;
                    case "MANUAL":
                        _manual = JsonConvert.DeserializeObject<ManualPacket>(Encoding.ASCII.GetString(_data));
                        break;
                    case "LIMITS":
                        _limits = JsonConvert.DeserializeObject<LimitPacket>(Encoding.ASCII.GetString(_data));
                        break;
                    default:
                        break;
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
