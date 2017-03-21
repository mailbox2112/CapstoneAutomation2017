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
        public WateringStateMachine WateringZone1 { get; set; }
        public WateringStateMachine WateringZone2 { get; set; }
        public WateringStateMachine WateringZone3 { get; set; }
        public WateringStateMachine WateringZone4 { get; set; }
        public WateringStateMachine WateringZone5 { get; set; }
        public WateringStateMachine WateringZone6 { get; set; }

        public LightingStateMachine LightingZone1 { get; set; }
        public LightingStateMachine LightingZone3 { get; set; }
        public LightingStateMachine LightingZone5 { get; set; }
        public TemperatureStateMachine Temperature { get; set; }

        private static volatile StateMachineContainer _instance;
        private static object _syncRoot = new object();

        private StateMachineContainer()
        {
            WateringZone1 = new WateringStateMachine(1);
            WateringZone2 = new WateringStateMachine(2);
            WateringZone3 = new WateringStateMachine(3);
            WateringZone4 = new WateringStateMachine(4);
            WateringZone5 = new WateringStateMachine(5);
            WateringZone6 = new WateringStateMachine(6);

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
