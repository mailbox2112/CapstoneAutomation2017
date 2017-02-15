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
            GreenhouseState result = testMachine.DetermineState(89.6, 50, 0);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(result == GreenhouseState.COOLING);

            result = testMachine.DetermineState(0, 100, 50);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(result == GreenhouseState.HEATING);

            result = testMachine.DetermineState(50, 100, 0);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            result = testMachine.DetermineState(50, 80, 70);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(result, testMachine);
            }
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.HEATING);

            result = testMachine.DetermineState(100, 80, 50);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(result, testMachine);
            }
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.COOLING);

            result = testMachine.DetermineState(150, 100, 0);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);
        }
    }
}
