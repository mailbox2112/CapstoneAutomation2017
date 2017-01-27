using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    enum GreenhouseState
    {
        WAITING,
        HEATING,
        COOLING,
        LIGHTING,
        WATERING,
        HEATING_LIGHTING,
        COOLING_LIGHTING,
        HEATING_WATERING,
        COOLING_WATERING,
        LIGHTING_WATERING,
        HEATING_LIGHTING_WATERING,
        COOLING_LIGHTING_WATERING,
    }
}
