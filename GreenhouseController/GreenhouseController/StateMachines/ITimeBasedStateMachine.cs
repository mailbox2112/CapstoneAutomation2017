using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public interface ITimeBasedStateMachine
    {
        int Zone { get; set; }

        GreenhouseState DetermineState(DateTime currentTime, double value);

        List<Commands> ConvertStateToCommands(GreenhouseState state);

        GreenhouseState CurrentState { get; set; }

        DateTime Begin { get; set; }

        DateTime End { get; set; }

        double? OverrideThreshold { get; set; }

        void OnStateChange(StateEventArgs e);
    }
}
