using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class TLHPacket
    { 
        public int ID;
        public double Temperature;
        public double Light;
        public double Humidity;
        public DateTime TimeOfSend = DateTime.Now;
    }
}
