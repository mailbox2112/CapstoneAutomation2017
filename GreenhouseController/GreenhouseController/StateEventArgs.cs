using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class StateEventArgs : EventArgs
    {
        public GreenhouseState State { get; set; }
    }
}
