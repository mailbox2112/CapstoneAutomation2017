using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class LightingStateMachineTests
    {
        [TestMethod]
        public void TestLightingStateMachineCreation()
        {
            object obj = LightingStateMachine.Instance;
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestLightingStateDecisions()
        {
            LightingStateMachine.Instance.DetermineGreenhouseState(30, 100);
            Assert.IsTrue(LightingStateMachine.Instance.CurrentState == GreenhouseState.PROCESSING_DATA);

            LightingStateMachine.Instance.DetermineGreenhouseState(100, 30);
            Assert.IsTrue(LightingStateMachine.Instance.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            LightingStateMachine.Instance.DetermineGreenhouseState(30, 100);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(LightingStateMachine.Instance);
            }
            Assert.IsTrue(LightingStateMachine.Instance.CurrentState == GreenhouseState.LIGHTING);
        }
    }
}
