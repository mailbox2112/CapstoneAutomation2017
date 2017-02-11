using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class TemperatureStateMachineTests
    {
        TemperatureStateMachine testMachine;
        [TestMethod]
        public void TestTemperatureStateMachineCreation()
        {
            testMachine = new TemperatureStateMachine();
            Assert.IsNotNull(testMachine);
        }

        [TestMethod]
        public void TestTemperatureStateDecisions()
        {
            testMachine = new TemperatureStateMachine();
            testMachine.DetermineGreenhouseState(89.6, 50, 0);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(testMachine.EndState == GreenhouseState.COOLING);

            testMachine.DetermineGreenhouseState(0, 100, 50);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(testMachine.EndState == GreenhouseState.HEATING);

            testMachine.DetermineGreenhouseState(50, 100, 0);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            testMachine.DetermineGreenhouseState(50, 80, 70);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(testMachine);
            }
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.HEATING);

            testMachine.DetermineGreenhouseState(100, 80, 50);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(testMachine);
            }
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.COOLING);

            testMachine.DetermineGreenhouseState(150, 100, 0);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.EMERGENCY);
        }
    }
}
