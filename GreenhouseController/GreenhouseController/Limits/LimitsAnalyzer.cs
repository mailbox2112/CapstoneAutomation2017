using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LimitsAnalyzer
    {
        public LimitsAnalyzer() { }

        /// <summary>
        /// Changes the  greenhouse limits based on the limit packet data
        /// </summary>
        /// <param name="limits"></param>
        public void ChangeGreenhouseLimits(LimitPacket limits)
        {
            // TODO: Make these thread-safe somehow!
            if (StateMachineContainer.Instance.Temperature.HighLimit != limits.TempHi)
            {
                StateMachineContainer.Instance.Temperature.HighLimit = limits.TempHi;
            }
            if (StateMachineContainer.Instance.Temperature.LowLimit != limits.TempLo)
            {
                StateMachineContainer.Instance.Temperature.LowLimit = limits.TempLo;
            }
            foreach(KeyValuePair<int, DateTime> kvp in limits.LightStarts)
            {
                switch(kvp.Key)
                {
                    case 1:
                        StateMachineContainer.Instance.LightingZone1.BeginLighting = kvp.Value;
                        break;
                    case 3:
                        StateMachineContainer.Instance.LightingZone3.BeginLighting = kvp.Value;
                        break;
                    case 5:
                        StateMachineContainer.Instance.LightingZone5.BeginLighting = kvp.Value;
                        break;
                    default:
                        break;
                }
            }
            foreach(KeyValuePair<int, DateTime> kvp in limits.LightEnds)
            {
                switch (kvp.Key)
                {
                    case 1:
                        StateMachineContainer.Instance.LightingZone1.EndLighting = kvp.Value;
                        break;
                    case 3:
                        StateMachineContainer.Instance.LightingZone3.EndLighting = kvp.Value;
                        break;
                    case 5:
                        StateMachineContainer.Instance.LightingZone5.EndLighting = kvp.Value;
                        break;
                    default:
                        break;
                }
            }
            foreach(KeyValuePair<int, DateTime> kvp in limits.WaterStarts)
            {

            }
            foreach(KeyValuePair<int, DateTime> kvp in limits.WaterEnds)
            {

            }

            Console.WriteLine($"Temperature High Limit: {StateMachineContainer.Instance.Temperature.HighLimit}");
            Console.WriteLine($"Temperature Low Limit: {StateMachineContainer.Instance.Temperature.LowLimit}");
            //Console.WriteLine($"Lighting High Limit: {StateMachineContainer.Instance.Lighting.HighLimit}");
            //Console.WriteLine($"Lighting Low Limit: {StateMachineContainer.Instance.Lighting.LowLimit}");
            //Console.WriteLine($"Watering Low Limit: {StateMachineContainer.Instance.Watering.LowLimit}");
        }
    }
}
