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
                        StateMachineContainer.Instance.LightingZone1.Begin = kvp.Value;
                        break;
                    case 3:
                        StateMachineContainer.Instance.LightingZone3.Begin = kvp.Value;
                        break;
                    case 5:
                        StateMachineContainer.Instance.LightingZone5.Begin = kvp.Value;
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
                        StateMachineContainer.Instance.LightingZone1.End = kvp.Value;
                        break;
                    case 3:
                        StateMachineContainer.Instance.LightingZone3.End = kvp.Value;
                        break;
                    case 5:
                        StateMachineContainer.Instance.LightingZone5.End = kvp.Value;
                        break;
                    default:
                        break;
                }
            }
            foreach(KeyValuePair<int, DateTime> kvp in limits.WaterStarts)
            {
                switch (kvp.Key)
                {
                    case 1:
                        StateMachineContainer.Instance.WateringZone1.Begin = kvp.Value;
                        break;
                    case 2:
                        StateMachineContainer.Instance.WateringZone2.Begin = kvp.Value;
                        break;
                    case 3:
                        StateMachineContainer.Instance.WateringZone3.Begin = kvp.Value;
                        break;
                    case 4:
                        StateMachineContainer.Instance.WateringZone4.Begin = kvp.Value;
                        break;
                    case 5:
                        StateMachineContainer.Instance.WateringZone5.Begin = kvp.Value;
                        break;
                    case 6:
                        StateMachineContainer.Instance.WateringZone6.Begin = kvp.Value;
                        break;
                    default:
                        break;
                }
            }
            foreach(KeyValuePair<int, DateTime> kvp in limits.WaterEnds)
            {
                switch (kvp.Key)
                {
                    case 1:
                        StateMachineContainer.Instance.WateringZone1.End = kvp.Value;
                        break;
                    case 2:
                        StateMachineContainer.Instance.WateringZone2.End = kvp.Value;
                        break;
                    case 3:
                        StateMachineContainer.Instance.WateringZone3.End = kvp.Value;
                        break;
                    case 4:
                        StateMachineContainer.Instance.WateringZone4.End = kvp.Value;
                        break;
                    case 5:
                        StateMachineContainer.Instance.WateringZone5.End = kvp.Value;
                        break;
                    case 6:
                        StateMachineContainer.Instance.WateringZone6.End = kvp.Value;
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine($"Temperature High Limit: {StateMachineContainer.Instance.Temperature.HighLimit}");
            Console.WriteLine($"Temperature Low Limit: {StateMachineContainer.Instance.Temperature.LowLimit}");
            Console.WriteLine($"LZone 1 Start: {StateMachineContainer.Instance.LightingZone1.Begin}\nLZone 1 End: {StateMachineContainer.Instance.LightingZone1.End}");
            Console.WriteLine($"LZone 3 Start: {StateMachineContainer.Instance.LightingZone3.Begin}\nLZone 3 End: {StateMachineContainer.Instance.LightingZone3.End}");
            Console.WriteLine($"LZone 5 Start: {StateMachineContainer.Instance.LightingZone3.Begin}\nLZone 5 End: {StateMachineContainer.Instance.LightingZone5.End}");
            Console.WriteLine($"WZone 1 Start: {StateMachineContainer.Instance.WateringZone1.Begin}\nWZone 1 End: {StateMachineContainer.Instance.WateringZone1.End}");
            Console.WriteLine($"WZone 2 Start: {StateMachineContainer.Instance.WateringZone2.Begin}\nWZone 2 End: {StateMachineContainer.Instance.WateringZone2.End}");
            Console.WriteLine($"WZone 3 Start: {StateMachineContainer.Instance.WateringZone3.Begin}\nWZone 3 End: {StateMachineContainer.Instance.WateringZone3.End}");
            Console.WriteLine($"WZone 4 Start: {StateMachineContainer.Instance.WateringZone4.Begin}\nWZone 4 End: {StateMachineContainer.Instance.WateringZone4.End}");
            Console.WriteLine($"WZone 5 Start: {StateMachineContainer.Instance.WateringZone5.Begin}\nWZone 5 End: {StateMachineContainer.Instance.WateringZone5.End}");
            Console.WriteLine($"WZone 6 Start: {StateMachineContainer.Instance.WateringZone6.Begin}\nWZone 6 End: {StateMachineContainer.Instance.WateringZone6.End}");
        }
    }
}
