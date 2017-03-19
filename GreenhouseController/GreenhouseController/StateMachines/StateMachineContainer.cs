using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class StateMachineContainer
    {
        // TODO: create and add a shading state machine
        public WateringStateMachine Watering { get; set; }
        public LightingStateMachine LightingZone1 { get; set; }
        public LightingStateMachine LightingZone3 { get; set; }
        public LightingStateMachine LightingZone5 { get; set; }
        public TemperatureStateMachine Temperature { get; set; }

        private static volatile StateMachineContainer _instance;
        private static object _syncRoot = new object();

        private StateMachineContainer()
        {
            Watering = new WateringStateMachine();
            LightingZone1 = new LightingStateMachine(1);
            LightingZone3 = new LightingStateMachine(3);
            LightingZone5 = new LightingStateMachine(5);
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
