using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class DataRequestTimer
    {
        public BlockingCollection<byte[]> BytesToSend;

        public DataRequestTimer(BlockingCollection<byte[]> target)
        {
            BytesToSend = target;
        }

        public void RequestData(Object state)
        {
            DataProducer.Instance.ReadGreenhouseData(BytesToSend);
        }

        public void RequestLimits(Object state) { }
    }
}
