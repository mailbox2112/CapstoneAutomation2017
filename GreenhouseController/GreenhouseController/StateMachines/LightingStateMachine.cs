using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LightingStateMachine : IStateMachine
    {
        private GreenhouseState _currentState;
        public GreenhouseState CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                OnStateChange(new StateEventArgs() { State = CurrentState });
            }
        }
        public EventHandler<StateEventArgs> StateChanged;
        
        public LightingStateMachine()
        { }

        public GreenhouseState DetermineState(double value, int hiLimit, int? loLimit = default(int?))
        {
            if (CurrentState == GreenhouseState.LIGHTING)
            {
                CurrentState = GreenhouseState.PROCESSING_LIGHTING;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // Process data and take into account if we were already lighting when we received the data
            if (value < hiLimit && CurrentState != GreenhouseState.PROCESSING_LIGHTING)
            {
                return GreenhouseState.LIGHTING;
            }
            else if (value < hiLimit && CurrentState == GreenhouseState.PROCESSING_LIGHTING)
            {
                CurrentState = GreenhouseState.LIGHTING;
                return GreenhouseState.NO_CHANGE;
            }
            else if (value > hiLimit && CurrentState == GreenhouseState.PROCESSING_DATA)
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                return GreenhouseState.NO_CHANGE;
            }
            else
            {
                return GreenhouseState.WAITING_FOR_DATA;
            }
        }

        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            List<Commands> commandsToSend = new List<Commands>();
            if (state == GreenhouseState.LIGHTING)
            {
                commandsToSend.Add(Commands.LIGHTS_ON);
            }
            else if (state == GreenhouseState.WAITING_FOR_DATA)
            {
                commandsToSend.Add(Commands.LIGHTS_OFF);
            }

            return commandsToSend;
        }

        public void OnStateChange(StateEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}
