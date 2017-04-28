using GreenhouseController.API;
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
        /// Changes the greenhouse limits based on the limit packet data
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
                switch(schedule.zone)
                {
                    case 1:
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].Begin = schedule.start;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].End = schedule.end;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].OverrideThreshold = schedule.threshold;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].ScheduleType = schedule.type;
                        break;
                    case 2:
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].Begin = schedule.start;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].End = schedule.end;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].OverrideThreshold = schedule.threshold;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].ScheduleType = schedule.type;
                        break;
                    case 3:
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].Begin = schedule.start;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].End = schedule.end;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].OverrideThreshold = schedule.threshold;
                        StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].ScheduleType = schedule.type;
                        break;
                }
            }
            foreach(ZoneSchedule schedule in limits.Water)
            {
                StateMachineContainer.Instance.WateringStateMachines[schedule.zone - 1].Begin = schedule.start;
                StateMachineContainer.Instance.WateringStateMachines[schedule.zone - 1].End = schedule.end;
                StateMachineContainer.Instance.WateringStateMachines[schedule.zone - 1].OverrideThreshold = schedule.threshold;
                StateMachineContainer.Instance.WateringStateMachines[schedule.zone - 1].ScheduleType = schedule.type;
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

        /// <summary>
        /// Change the greenhouse limits based on the API automation data
        /// </summary>
        /// <param name="limits"></param>
        public void ChangeGreenhouseLimits(AutomationPacket limits)
        {
            if (StateMachineContainer.Instance.Temperature.HighLimit != limits.TempHigh)
            {
                StateMachineContainer.Instance.Temperature.HighLimit = limits.TempHigh;
            }
            if (StateMachineContainer.Instance.Temperature.LowLimit != limits.TempLow)
            {
                StateMachineContainer.Instance.Temperature.LowLimit = limits.TempLow;
            }
            if (StateMachineContainer.Instance.Shading.HighLimit != limits.ShadeLim)
            {
                StateMachineContainer.Instance.Shading.HighLimit = limits.ShadeLim;
            }
            if (limits.LightSchedules != null)
            {
                foreach (ZoneSchedule schedule in limits.LightSchedules)
                {
                    switch (schedule.zone)
                    {
                        case 1:
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].Begin = schedule.start;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].End = schedule.end;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].OverrideThreshold = schedule.threshold;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].ScheduleType = schedule.type;
                            break;
                        case 2:
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].Begin = schedule.start;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].End = schedule.end;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].OverrideThreshold = schedule.threshold;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].ScheduleType = schedule.type;
                            break;
                        case 3:
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].Begin = schedule.start;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].End = schedule.end;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].OverrideThreshold = schedule.threshold;
                            StateMachineContainer.Instance.LightStateMachines[schedule.zone - 1].ScheduleType = schedule.type;
                            break;
                    }
                }
            }
                
            if (limits.WaterSchedules != null)
            {
                foreach (ZoneSchedule schedule in limits.WaterSchedules)
                {
                    StateMachineContainer.Instance.WateringStateMachines[schedule.zone - 1].Begin = schedule.start;
                    StateMachineContainer.Instance.WateringStateMachines[schedule.zone - 1].End = schedule.end;
                    StateMachineContainer.Instance.WateringStateMachines[schedule.zone - 1].OverrideThreshold = schedule.threshold;
                    StateMachineContainer.Instance.WateringStateMachines[schedule.zone - 1].ScheduleType = schedule.type;
                }
            } 

            Console.WriteLine($"Temperature High Limit: {StateMachineContainer.Instance.Temperature.HighLimit}");
            Console.WriteLine($"Temperature Low Limit: {StateMachineContainer.Instance.Temperature.LowLimit}");
            Console.WriteLine($"Shading Limit: {StateMachineContainer.Instance.Shading.HighLimit}");

            for (int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i++)
            {
                Console.WriteLine($"LZone {StateMachineContainer.Instance.LightStateMachines[i].Zone}"
                    + $"Start: {StateMachineContainer.Instance.LightStateMachines[i].Begin}"
                    + $"\nLZone {StateMachineContainer.Instance.LightStateMachines[i].Zone}"
                    + $"End: {StateMachineContainer.Instance.LightStateMachines[i].End}"
                    );
            }

            for (int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i++)
            {
                Console.WriteLine($"WZone {StateMachineContainer.Instance.WateringStateMachines[i].Zone}"
                    + $"Start: {StateMachineContainer.Instance.WateringStateMachines[i].Begin}"
                    + $"\nWZone {StateMachineContainer.Instance.WateringStateMachines[i].Zone}"
                    + $"End: {StateMachineContainer.Instance.WateringStateMachines[i].End}");
            }
        }
    }
}
