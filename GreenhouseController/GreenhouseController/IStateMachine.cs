using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public interface IStateMachine
    {
        GreenhouseState DetermineGreenhouseState(double value, int hiLimit, int? loLimit = null);

        GreenhouseState CurrentState { get; set; }

        GreenhouseState EndState { get; set; }
    }
}
