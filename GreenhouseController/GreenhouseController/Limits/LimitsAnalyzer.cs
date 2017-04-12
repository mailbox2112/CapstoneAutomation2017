using GreenhouseController.Limits;
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
            if (StateMachineContainer.Instance.Shading.HighLimit != limits.ShadeLim)
            {
                StateMachineContainer.Instance.Shading.HighLimit = limits.ShadeLim;
            }
            foreach(ZoneSchedule schedule in limits.Light)
            {
                switch(schedule.Zone)
                {
                    case 1:
                        StateMachineContainer.Instance.LightStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.LightStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    case 2:
                        StateMachineContainer.Instance.LightStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.LightStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    case 3:
                        StateMachineContainer.Instance.LightStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.LightStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    default:
                        break;
                }
            }
            foreach(ZoneSchedule schedule in limits.Water)
            {
                switch (schedule.Zone)
                {
                    case 1:
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    case 2:
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    case 3:
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    case 4:
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    case 5:
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    case 6:
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].Begin = schedule.Start;
                        StateMachineContainer.Instance.WateringStateMachines[schedule.Zone - 1].End = schedule.End;
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine($"Temperature High Limit: {StateMachineContainer.Instance.Temperature.HighLimit}");
            Console.WriteLine($"Temperature Low Limit: {StateMachineContainer.Instance.Temperature.LowLimit}");
            Console.WriteLine($"Shading Limit: {StateMachineContainer.Instance.Shading.HighLimit}");

            for(int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i++)
            {
                Console.WriteLine($"LZone {StateMachineContainer.Instance.LightStateMachines[i].Zone}"
                    + $"Start: {StateMachineContainer.Instance.LightStateMachines[i].Begin}"
                    + $"\nLZone {StateMachineContainer.Instance.LightStateMachines[i].Zone}"
                    + $"End: {StateMachineContainer.Instance.LightStateMachines[i].End}"
                    );
            }
            
            for(int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i++)
            {
                Console.WriteLine($"WZone {StateMachineContainer.Instance.WateringStateMachines[i].Zone}"
                    + $"Start: {StateMachineContainer.Instance.WateringStateMachines[i].Begin}"
                    + $"\nWZone {StateMachineContainer.Instance.WateringStateMachines[i].Zone}"
                    + $"End: {StateMachineContainer.Instance.WateringStateMachines[i].End}");
            }
            
        }
    }
}
