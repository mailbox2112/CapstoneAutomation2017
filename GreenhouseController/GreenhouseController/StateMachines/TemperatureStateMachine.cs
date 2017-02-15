using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class TemperatureStateMachine : IStateMachine
    {
        private const int _emergencyTemp = 120;

        public GreenhouseState CurrentState { get; set; }

        public TemperatureStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }


        public GreenhouseState DetermineState(double value, int hiLimit, int? loLimit = default(int?))
        {
            CurrentState = GreenhouseState.PROCESSING_DATA;
            if (value < loLimit)
            {
                return GreenhouseState.HEATING;
            }
            else if (value > hiLimit)
            {
                if (value >= _emergencyTemp)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else
                {
                    return GreenhouseState.COOLING;
                }
            }
            else
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                return GreenhouseState.WAITING_FOR_DATA;
            }
        }
    }
}
