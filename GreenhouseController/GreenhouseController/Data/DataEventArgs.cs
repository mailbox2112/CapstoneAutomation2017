using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class DataEventArgs : EventArgs
    {
        public BlockingCollection<byte[]> Buffer { get; set; }
    }
}
