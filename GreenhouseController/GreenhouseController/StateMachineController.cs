using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class StateMachineController
    {
        private WateringStateMachine Watering;
        private LightingStateMachine Lighting;
        private TemperatureStateMachine Temperature;

        private static volatile StateMachineController _instance;
        private static object _syncRoot = new object();

        private StateMachineController()
        {
            Watering = new WateringStateMachine();
            Lighting = new LightingStateMachine();
            Temperature = new TemperatureStateMachine();
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
            Lighting.DetermineGreenhouseState(value, limit);
        }

        /// <summary>
        /// Gets the current state of the lighting state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetLightingCurrentState()
        {
            return Lighting.CurrentState;
        }

        /// <summary>
        /// Gets the goal state of the lighting state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetLightingEndState()
        {
            return Lighting.EndState;
        }

        public LightingStateMachine GetLightingMachine()
        {
            return Lighting;
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
            Watering.DetermineGreenhouseState(value, limit);
        }

        /// <summary>
        /// Gets the current state of the watering state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetWateringCurrentState()
        {
            return Watering.CurrentState;
        }

        /// <summary>
        /// Gets the goal state of the watering state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetWateringEndState()
        {
            return Watering.EndState;
        }

        public WateringStateMachine GetWateringMachine()
        {
            return Watering;
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
            Temperature.DetermineGreenhouseState(value, hiLimit, loLimit);
        }

        /// <summary>
        /// Gets the current state of the temperature state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetTemperatureCurrentState()
        {
            return Temperature.CurrentState;
        }

        /// <summary>
        /// Gets the goal state of the temperature state machine
        /// </summary>
        /// <returns></returns>
        public GreenhouseState GetTemperatureEndState()
        {
            return Temperature.EndState;
        }

        public TemperatureStateMachine GetTemperatureMachine()
        {
            return Temperature;
        }
        #endregion
    }

}
