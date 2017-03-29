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
        WAITING_FOR_DATA,
        WAITING_FOR_RESPONSE,
        PROCESSING_DATA,
        SENDING_DATA,
        HEATING,
        COOLING,
        LIGHTING,
        SHADING,
        WATERING,
        PROCESSING_HEATING,
        PROCESSING_COOLING,
        PROCESSING_LIGHTING,
        PROCESSING_SHADING,
        PROCESSING_WATER,
        EMERGENCY,
        ERROR,
        NO_CHANGE
    }
}
