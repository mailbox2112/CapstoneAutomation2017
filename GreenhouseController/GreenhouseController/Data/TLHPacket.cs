using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class TLHPacket
    {
        public int Type = 0;
        public int ID;
        public int Temperature;
        public int Light;
        public int Humidity;
        public DateTime TimeOfSend = DateTime.Now;
    }
}
