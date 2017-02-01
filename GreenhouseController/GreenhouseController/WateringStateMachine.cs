using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class WateringStateMachine : IStateMachine
    {
        public GreenhouseState CurrentState { get; set; }
        public GreenhouseState EndState { get; set; }

        private static volatile WateringStateMachine _instance;
        private static volatile object _syncRoot = new object();

        private WateringStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }

        public static WateringStateMachine Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new WateringStateMachine();
                        }
                    }
                }
                return _instance;
            }
        }

        public void DetermineGreenhouseState(double value, int hiLimit, int? loLimit = default(int?))
        {
            CurrentState = GreenhouseState.PROCESSING_DATA;

            if(value < hiLimit)
            {
                EndState = GreenhouseState.WATERING;
            }
            else
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                EndState = GreenhouseState.WAITING_FOR_DATA;
            }
        }
    }
}
