using GreenhouseController.Limits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LimitPacket
    {
        public int Type = 2;
        public int TempHi;
        public int TempLo;
        public List<ZoneSchedule> Water;
        public List<ZoneSchedule> Light;
        public int ShadeLim;
    }
}
