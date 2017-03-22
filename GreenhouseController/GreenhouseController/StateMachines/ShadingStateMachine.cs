using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.StateMachines
{
    public class ShadingStateMachine : IStateMachine
    {
        public GreenhouseState CurrentState
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int? HighLimit
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int? LowLimit
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool? ManualShde
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            throw new NotImplementedException();
        }

        public GreenhouseState DetermineState(double value)
        {
            throw new NotImplementedException();
        }

        public void OnStateChange(StateEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
