using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class StateMachineController
    {
        private WateringStateMachine _watering;
        private LightingStateMachine _lighting;
        private TemperatureStateMachine _temperature;

        private static volatile StateMachineController _instance;
        private static object _syncRoot = new object();

        private StateMachineController()
        {
            _watering = new WateringStateMachine();
            _lighting = new LightingStateMachine();
            _temperature = new TemperatureStateMachine();
        }

        public static StateMachineController Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_syncRoot)
                    {
                        if(_instance == null)
                        {
                            _instance = new StateMachineController();
                        }
                    }
                }
                return _instance;
            }
        }

        #region Lighting Methods
        /// <summary>
        /// Determines the state of the lighting state machine from the input data
        /// </summary>
        /// <param name="value"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public void DetermineLightingState(double value, int limit)
        {
            _lighting.DetermineGreenhouseState(value, limit);
        }

        /// <summary>
        /// Gets the current state of the lighting state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetLightingCurrentState()
        {
            return _lighting.CurrentState;
        }

        /// <summary>
        /// Gets the goal state of the lighting state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetLightingEndState()
        {
            return _lighting.EndState;
        }

        public LightingStateMachine GetLightingMachine()
        {
            return _lighting;
        }
        #endregion

        #region Watering Methods
        /// <summary>
        /// Determines the state of the watering state machine
        /// </summary>
        /// <param name="value"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public void DetermineWateringState(double value, int limit)
        {
            _watering.DetermineGreenhouseState(value, limit);
        }

        /// <summary>
        /// Gets the current state of the watering state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetWateringCurrentState()
        {
            return _watering.CurrentState;
        }

        /// <summary>
        /// Gets the goal state of the watering state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetWateringEndState()
        {
            return _watering.EndState;
        }

        public WateringStateMachine GetWateringMachine()
        {
            return _watering;
        }
        #endregion

        #region Temperature Methods
        /// <summary>
        /// Decides the state of the temperature state machine based on input values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hiLimit"></param>
        /// <param name="loLimit"></param>
        /// <returns></returns>
        public void DetermineTemperatureState(double value, int hiLimit, int loLimit)
        {
            _temperature.DetermineGreenhouseState(value, hiLimit, loLimit);
        }

        /// <summary>
        /// Gets the current state of the temperature state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetTemperatureCurrentState()
        {
            return _temperature.CurrentState;
        }

        /// <summary>
        /// Gets the goal state of the temperature state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetTemperatureEndState()
        {
            return _temperature.EndState;
        }

        public TemperatureStateMachine GetTemperatureMachine()
        {
            return _temperature;
        }
        #endregion
    }

}
