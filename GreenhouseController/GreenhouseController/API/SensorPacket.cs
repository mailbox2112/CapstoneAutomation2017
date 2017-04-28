using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.API
{
    public class SensorPacket
    {
        public DateTime SampleTime;
        public int Zone;
        public double Light;
        public double Temperature;
        public double Humidity;
        public double Probe1;
        public double Probe2;
    }
}
