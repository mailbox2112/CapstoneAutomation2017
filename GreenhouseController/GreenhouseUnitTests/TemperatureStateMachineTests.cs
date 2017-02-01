using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class TemperatureStateMachineTests
    {
        [TestMethod]
        public void TestTemperatureStateMachineCreation()
        {
            object obj = TemperatureStateMachine.Instance;
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestTemperatureStateDecisions()
        {
            TemperatureStateMachine.Instance.DetermineGreenhouseState(89.6, 50, 0);
            Assert.IsTrue(TemperatureStateMachine.Instance.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(TemperatureStateMachine.Instance.EndState == GreenhouseState.COOLING);

            TemperatureStateMachine.Instance.DetermineGreenhouseState(0, 100, 50);
            Assert.IsTrue(TemperatureStateMachine.Instance.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(TemperatureStateMachine.Instance.EndState == GreenhouseState.HEATING);

            TemperatureStateMachine.Instance.DetermineGreenhouseState(50, 100, 0);
            Assert.IsTrue(TemperatureStateMachine.Instance.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            TemperatureStateMachine.Instance.DetermineGreenhouseState(50, 80, 70);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(TemperatureStateMachine.Instance);
            }
            Assert.IsTrue(TemperatureStateMachine.Instance.CurrentState == GreenhouseState.HEATING);

            TemperatureStateMachine.Instance.DetermineGreenhouseState(100, 80, 50);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(TemperatureStateMachine.Instance);
            }
            Assert.IsTrue(TemperatureStateMachine.Instance.CurrentState == GreenhouseState.COOLING);
        }
    }
}
