using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.Data
{
    public class ManualPacket
    {
        public int Type = 3;
        public bool? ManualHeat;
        public bool? ManualCool;
        public bool? ManualLight;
        public bool? ManualWater;
        public bool? ManualShade;
    }
}
