using GreenhouseController.Limits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.API
{
    public class AutomationPacket
    {
        public int ShadeLim;
        public int TempLow;
        public int TempHigh;
        public int humiditiy;
        public int moisture;
        public List<ZoneSchedule> LightSchedules;
        public List<ZoneSchedule> WaterSchedules;
    }
}
