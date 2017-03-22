using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.Data
{
    public class ManualControlAnalyzer
    {
        public ManualControlAnalyzer()
        {

        }

        public void SetManualValues(ManualPacket packet)
        {
            if (packet.ManualHeat != StateMachineContainer.Instance.Temperature.ManualHeat)
            {
                StateMachineContainer.Instance.Temperature.ManualHeat = packet.ManualHeat;
            }
            if (packet.ManualCool != StateMachineContainer.Instance.Temperature.ManualCool)
            {
                StateMachineContainer.Instance.Temperature.ManualCool = packet.ManualCool;
            }
            for (int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i++)
            {
                if (packet.ManualLight != StateMachineContainer.Instance.LightStateMachines[i].ManualLight)
                {
                    StateMachineContainer.Instance.LightStateMachines[i].ManualLight = packet.ManualLight;
                }
            }
            for (int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i++)
            {
                if (packet.ManualWater != StateMachineContainer.Instance.WateringStateMachines[i].ManualWater)
                {
                    StateMachineContainer.Instance.WateringStateMachines[i].ManualWater = packet.ManualWater;
                }
            }
        }
    }
}
