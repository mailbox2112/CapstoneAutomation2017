using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class DataPacket
    {
        public int Zone;
        public double Temperature;
        public double Humidity;
        public double Light;
        public double Moisture;
        
        public bool? ManualHeat;
        public bool? ManualCool;
        public bool? ManualLight;
        public bool? ManualWater;
    }
}
