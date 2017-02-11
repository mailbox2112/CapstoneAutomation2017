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

        public GreenhouseState EndState { get; set; }
        public GreenhouseState CurrentState { get; set; }

        public TemperatureStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
            EndState = GreenhouseState.WAITING_FOR_DATA;
        }


        public void DetermineGreenhouseState(double value, int hiLimit, int? loLimit = default(int?))
        {
            CurrentState = GreenhouseState.PROCESSING_DATA;
            if (value < loLimit)
            {
                EndState = GreenhouseState.HEATING;
            }
            else if (value > hiLimit)
            {
                if (value >= _emergencyTemp)
                {
                    CurrentState = GreenhouseState.EMERGENCY;
                }
                else
                {
                    EndState = GreenhouseState.COOLING;
                }
            }
            else
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                EndState = GreenhouseState.WAITING_FOR_DATA;
            }
        }
    }
}
