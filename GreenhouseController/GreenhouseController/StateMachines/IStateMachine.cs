using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public interface IStateMachine
    {
        GreenhouseState DetermineState(double value);

        List<Commands> ConvertStateToCommands(GreenhouseState state);

        GreenhouseState CurrentState { get; set; }

        int? HighLimit { get; set; }

        int? LowLimit { get; set; }

        void OnStateChange(StateEventArgs e);
    }
}
