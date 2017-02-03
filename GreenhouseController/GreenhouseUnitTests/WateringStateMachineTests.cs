using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class WateringStateMachineTests
    {
        [TestMethod]
        public void TestWateringStateMachineCreation()
        {
            object obj = WateringStateMachine.Instance;
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TestWateringStateDecisions()
        {
            WateringStateMachine.Instance.DetermineGreenhouseState(30, 50);
            Assert.IsTrue(WateringStateMachine.Instance.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(WateringStateMachine.Instance.EndState == GreenhouseState.WATERING);

            WateringStateMachine.Instance.DetermineGreenhouseState(50, 30);
            Assert.IsTrue(WateringStateMachine.Instance.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            WateringStateMachine.Instance.DetermineGreenhouseState(30, 50);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(WateringStateMachine.Instance);
            }
            Assert.IsTrue(WateringStateMachine.Instance.CurrentState == GreenhouseState.WATERING);
        }
    }
}
