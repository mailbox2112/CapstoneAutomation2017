using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LimitPacket
    {
        public int PacketType = 2;
        public int TempHi;
        public int TempLo;
        public Dictionary<int, DateTime> WaterStarts;
        public Dictionary<int, DateTime> WaterEnds;
        public Dictionary<int, DateTime> LightStarts;
        public Dictionary<int, DateTime> LightEnds;
        public int ShadeLim;
    }
}
