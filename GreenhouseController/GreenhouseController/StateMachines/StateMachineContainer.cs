using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class StateMachineContainer
    {
        public WateringStateMachine Watering { get; set; }
        public LightingStateMachine Lighting { get; set; }
        public TemperatureStateMachine Temperature { get; set; }

        private static volatile StateMachineContainer _instance;
        private static object _syncRoot = new object();

        private StateMachineContainer()
        {
            Watering = new WateringStateMachine();
            Lighting = new LightingStateMachine();
            Temperature = new TemperatureStateMachine();
        }

        public static StateMachineContainer Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_syncRoot)
                    {
                        if(_instance == null)
                        {
                            _instance = new StateMachineContainer();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
