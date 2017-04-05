using GreenhouseController.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class StateMachineContainer
    {
        public TemperatureStateMachine Temperature { get; set; }
        public ShadingStateMachine Shading { get; set; }

        public List<LightingStateMachine> LightStateMachines = new List<LightingStateMachine>(3);
        public List<WateringStateMachine> WateringStateMachines = new List<WateringStateMachine>(6);

        private static volatile StateMachineContainer _instance;
        private static object _syncRoot = new object();
        private int[] _wateringZones = new int[6] { 1, 2, 3, 4, 5, 6 };
        private int[] _lightZones = new int[3] { 1, 2, 3 };

        private StateMachineContainer()
        {
            foreach(int zone in _wateringZones)
            {
                WateringStateMachines.Add(new WateringStateMachine(zone));
            }

            foreach(int zone in _lightZones)
            {
                LightStateMachines.Add(new LightingStateMachine(zone));
            }

            Temperature = new TemperatureStateMachine();
            Shading = new ShadingStateMachine();
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
