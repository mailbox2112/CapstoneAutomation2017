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
    public enum GreenhouseState
    {
        WAITING,
        HEATING,
        COOLING,
        LIGHTING,
        WATERING,
        LIGHTING_WATERING,
        HEATING_LIGHTING,
        HEATING_WATERING,
        HEATING_LIGHTING_WATERING,
        COOLING_LIGHTING,
        COOLING_WATERING,
        COOLING_LIGHTING_WATERING
    }
}
