using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class GreenhouseActionSolver: IDisposable
    {

        public GreenhouseActionSolver()
        {
            // TODO: construct!
        }

        public List<GreenhouseCommands> DetermineGreenhouseAction(GreenhouseState[] state)
        {
            // TODO: add decision logic here!
            List<GreenhouseCommands> fakeActions = new List<GreenhouseCommands>() { GreenhouseCommands.COOL_ON, GreenhouseCommands.OPEN_VENTS };
            return fakeActions;
        }

        public void Dispose()
        {
            // TODO: implement!
        }
    }
}
