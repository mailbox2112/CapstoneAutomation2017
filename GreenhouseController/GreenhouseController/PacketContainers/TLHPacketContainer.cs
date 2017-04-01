using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.Packets
{
    public class TLHPacketContainer
    {
        public int Type = 0;
        public List<TLHPacket> Packets;

        public TLHPacketContainer() { }
    }
}
