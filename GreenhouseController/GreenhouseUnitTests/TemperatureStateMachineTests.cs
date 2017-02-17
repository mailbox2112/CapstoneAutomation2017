using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

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
            Assert.IsInstanceOfType(testMachine, typeof(TemperatureStateMachine));
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

        [TestMethod]
        public void TestConvertTemperatureStateToCommands()
        {
            testMachine = new TemperatureStateMachine();
            List<Commands> result = new List<Commands>();
            result = testMachine.ConvertStateToCommands(GreenhouseState.COOLING);
            Assert.IsTrue(result.Contains(Commands.FANS_ON));
            Assert.IsTrue(result.Contains(Commands.SHADE_EXTEND));
            Assert.IsTrue(result.Contains(Commands.VENT_OPEN));

            result = testMachine.ConvertStateToCommands(GreenhouseState.HEATING);
            Assert.IsTrue(result.Contains(Commands.HEAT_ON));
            Assert.IsTrue(result.Contains(Commands.SHADE_RETRACT));
            Assert.IsTrue(result.Contains(Commands.VENT_CLOSED));

            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.HEAT_OFF));
            Assert.IsTrue(result.Contains(Commands.FANS_OFF));
            Assert.IsTrue(result.Contains(Commands.VENT_CLOSED));
            Assert.IsTrue(result.Contains(Commands.SHADE_RETRACT));
        }
    }
}
