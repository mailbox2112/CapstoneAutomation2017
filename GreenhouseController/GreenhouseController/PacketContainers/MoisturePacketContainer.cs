using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.Packets
{
    public class MoisturePacketContainer
    {
        public int Type = 1;
        public List<MoisturePacket> Packets;

        public MoisturePacketContainer() { }
    }
}
