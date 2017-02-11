using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class WateringStateMachineTests
    {
        WateringStateMachine testMachine;

        [TestMethod]
        public void TestWateringStateMachineCreation()
        {
            testMachine = new WateringStateMachine();
            Assert.IsNotNull(testMachine);
        }

        [TestMethod]
        public void TestWateringStateDecisions()
        {
            testMachine = new WateringStateMachine();
            testMachine.DetermineGreenhouseState(30, 50);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(testMachine.EndState == GreenhouseState.WATERING);

            testMachine.DetermineGreenhouseState(50, 30);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            testMachine.DetermineGreenhouseState(30, 50);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(testMachine);
            }
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WATERING);

            testMachine.DetermineGreenhouseState(0, 100);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.EMERGENCY);
        }
    }
}
