using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    /// <summary>
    /// Enum of possible states that could be combined in our NFA
    /// </summary>
    enum GreenhouseState
    {
        WAITING,
        HEATING,
        COOLING,
        LIGHTING,
        WATERING,
        VENTILATING,
        SHADING
    }
}
