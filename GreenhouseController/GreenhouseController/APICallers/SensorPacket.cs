using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.APICallers
{
    public class SensorPacket
    {
        public DateTime sampletime;
        public int zone;
        double light;
        double temperature;
        double humidity;
        double probe1;
        double probe2;
    }
}
