using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class DataPacket
    {
        public int zone;
        public double temperature;
        public double humidity;
        public double light;
        public double moisture;
        
        public int tempHi;
        public int tempLo;
        public int lightLim;
        public int moistLim;

        public bool? manualHeat;
        public bool? manualCool;
        public bool? manualLight;
        public bool? manualWater;
    }
}
