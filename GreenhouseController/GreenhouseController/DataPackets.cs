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
        
        public int tempHi;
        public int tempLo;
        public int humidHi;
        public int humidLo;
        public int lightHi;
        public int lightlo;
    }
}
