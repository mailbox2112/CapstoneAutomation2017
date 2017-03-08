using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LimitConsumer
    {
        // Singleton fields
        private static volatile LimitConsumer _instance;
        private static object _syncRoot = new object();

        private byte[] _data;

        /// <summary>
        /// Private singleton constructor
        /// </summary>
        private LimitConsumer()
        {

        }

        /// <summary>
        /// Singleton propety
        /// </summary>
        public static LimitConsumer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new LimitConsumer();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Change the greenhouse limits based on what we receive
        /// </summary>
        /// <param name="target"></param>
        public void ChangeLimits(BlockingCollection<byte[]> source)
        {
            source.TryTake(out _data);
            var deserializedData = JsonConvert.DeserializeObject<LimitPacket>(Encoding.ASCII.GetString(_data));
            Console.WriteLine($"{Encoding.ASCII.GetString(_data)}");

            if (StateMachineContainer.Instance.Temperature.HighLimit != deserializedData.TempHi)
            {
                StateMachineContainer.Instance.Temperature.HighLimit = deserializedData.TempHi;
            }
            if (StateMachineContainer.Instance.Temperature.LowLimit != deserializedData.TempLo)
            {
                StateMachineContainer.Instance.Temperature.LowLimit = deserializedData.TempLo;
            }
            if (StateMachineContainer.Instance.Lighting.LowLimit != deserializedData.LightLo)
            {
                StateMachineContainer.Instance.Lighting.LowLimit = deserializedData.LightLo;
            }
            if (StateMachineContainer.Instance.Watering.LowLimit != deserializedData.MoistLim)
            {
                StateMachineContainer.Instance.Watering.LowLimit = deserializedData.MoistLim;
            }

            Console.WriteLine($"Temperature High Limit: {StateMachineContainer.Instance.Temperature.HighLimit}");
            Console.WriteLine($"Temperature Low Limit: {StateMachineContainer.Instance.Temperature.LowLimit}");
            Console.WriteLine($"Lighting Low Limit: {StateMachineContainer.Instance.Lighting.LowLimit}");
            Console.WriteLine($"Watering High Limit: {StateMachineContainer.Instance.Watering.LowLimit}");
        }
    }
}
