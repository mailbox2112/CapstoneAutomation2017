using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class TemperatureStateMachine : IStateMachine
    {
        public GreenhouseState EndState { get; set; }
        public GreenhouseState CurrentState { get; set; }

        private static volatile TemperatureStateMachine _instance;
        private static object _syncRoot = new object();

        private TemperatureStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }

        public static TemperatureStateMachine Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new TemperatureStateMachine();
                        }
                    }
                }
                return _instance;
            }
        }


        public GreenhouseState DetermineGreenhouseState(double value, int hiLimit, int? loLimit = default(int?))
        {
            CurrentState = GreenhouseState.PROCESSING_DATA;
            if (value < loLimit)
            {
                EndState = GreenhouseState.HEATING;
            }
            else if (value > hiLimit)
            {
                EndState = GreenhouseState.COOLING;
            }
            else
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                EndState = GreenhouseState.WAITING_FOR_DATA;
            }
            return CurrentState;
        }
    }
}
