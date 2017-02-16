using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class WateringStateMachine : IStateMachine
    {
        private const int _emergencyMoist = 0;

        public GreenhouseState CurrentState { get; set; }
        
        public WateringStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }

        public GreenhouseState DetermineState(double value, int hiLimit, int? loLimit = default(int?))
        {
            if (CurrentState == GreenhouseState.WATERING)
            {
                CurrentState = GreenhouseState.PROCESSING_WATER;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // Check the states based on data, and if we were already watering take that into account
            if (value < hiLimit && CurrentState != GreenhouseState.PROCESSING_WATER)
            {
                if (value == _emergencyMoist)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else 
                {
                    return GreenhouseState.WATERING;
                }
            }
            else if (value < hiLimit && CurrentState == GreenhouseState.PROCESSING_WATER)
            {
                if (value == _emergencyMoist)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else
                {
                    CurrentState = GreenhouseState.WATERING;
                    return GreenhouseState.NO_CHANGE;
                }
            }
            else
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                return GreenhouseState.WAITING_FOR_DATA;
            }
        }

        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            List<Commands> commandsToSend = new List<Commands>();
            
            if (state == GreenhouseState.WATERING)
            {
                commandsToSend.Add(Commands.WATER_ON);
            }
            else if (state == GreenhouseState.WAITING_FOR_DATA)
            {
                commandsToSend.Add(Commands.WATER_OFF);
            }

            return commandsToSend;
        }
    }
}
