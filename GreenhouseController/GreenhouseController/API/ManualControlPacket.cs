using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.API
{
    public class ManualControlPacket
    {
        public bool WaterOverride;
        public bool Shades;
        public bool LightOverride;
        public bool Water;
        public bool Lights;
        public bool Fans;
    }
}
