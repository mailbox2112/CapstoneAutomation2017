using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class StateMachineControllerTests
    {
        [TestMethod]
        public void TestStateMachineControllerInitialization()
        {
            // Test that whatever it returns isn't null
            for(int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i++)
            {
                Assert.IsNotNull(StateMachineContainer.Instance.LightStateMachines[i]);
            }

            for (int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i++)
            {
                Assert.IsNotNull(StateMachineContainer.Instance.WateringStateMachines[i]);
            }
            Assert.IsNotNull(StateMachineContainer.Instance.Temperature);

            // Test that it returns the right type of object. Maybe this should be its own seperate test though?
            for(int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i++)
            {
                Assert.IsInstanceOfType(StateMachineContainer.Instance.LightStateMachines[i], typeof(LightingStateMachine));
            }

            for(int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i++)
            {
                Assert.IsInstanceOfType(StateMachineContainer.Instance.WateringStateMachines[i], typeof(WateringStateMachine));
            }
            Assert.IsInstanceOfType(StateMachineContainer.Instance.Temperature, typeof(TemperatureStateMachine));
        }
    }
}
