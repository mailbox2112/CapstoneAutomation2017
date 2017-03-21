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
            Assert.IsNotNull(StateMachineContainer.Instance.LightingZone1);
            Assert.IsNotNull(StateMachineContainer.Instance.LightingZone3);
            Assert.IsNotNull(StateMachineContainer.Instance.LightingZone5);
            Assert.IsNotNull(StateMachineContainer.Instance.WateringZone1);
            Assert.IsNotNull(StateMachineContainer.Instance.WateringZone2);
            Assert.IsNotNull(StateMachineContainer.Instance.WateringZone3);
            Assert.IsNotNull(StateMachineContainer.Instance.WateringZone4);
            Assert.IsNotNull(StateMachineContainer.Instance.WateringZone5);
            Assert.IsNotNull(StateMachineContainer.Instance.WateringZone6);
            Assert.IsNotNull(StateMachineContainer.Instance.Temperature);

            // Test that it returns the right type of object. Maybe this should be its own seperate test though?
            Assert.IsInstanceOfType(StateMachineContainer.Instance.LightingZone1, typeof(LightingStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.LightingZone3, typeof(LightingStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.LightingZone5, typeof(LightingStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.WateringZone1, typeof(WateringStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.WateringZone2, typeof(WateringStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.WateringZone3, typeof(WateringStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.WateringZone4, typeof(WateringStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.WateringZone5, typeof(WateringStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.WateringZone6, typeof(WateringStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.Temperature, typeof(TemperatureStateMachine));
        }
    }
}
