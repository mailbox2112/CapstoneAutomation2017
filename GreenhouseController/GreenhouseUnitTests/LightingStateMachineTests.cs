using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class LightingStateMachineTests
    {
        LightingStateMachine testMachine;
        [TestMethod]
        public void TestLightingStateMachineCreation()
        {
            testMachine = new LightingStateMachine();
            Assert.IsNotNull(testMachine);
        }

        [TestMethod]
        public void TestLightingStateDecisions()
        {
            testMachine = new LightingStateMachine();
            testMachine.DetermineState(30, 100);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.PROCESSING_DATA);

            testMachine.DetermineState(100, 30);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            testMachine.DetermineState(30, 100);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(testMachine);
            }
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.LIGHTING);
        }
    }
}
