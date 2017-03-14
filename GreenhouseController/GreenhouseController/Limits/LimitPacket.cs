using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LimitPacket
    {
        public int PacketType = 1;
        public int TempHi;
        public int TempLo;
        public int MoistLim;
        public int LightHi;
        public int LightLo;
    }
}
