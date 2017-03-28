using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.Data
{
    public class ManualPacketAnalyzer
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public ManualPacketAnalyzer()
        { }

        /// <summary>
        /// Set the values for each manual command that we received, in the appropriate state machine(s)
        /// </summary>
        /// <param name="packet"></param>
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
            if (packet.ManualShade != StateMachineContainer.Instance.Shading.ManualShade)
            {
                StateMachineContainer.Instance.Shading.ManualShade = packet.ManualShade;
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
