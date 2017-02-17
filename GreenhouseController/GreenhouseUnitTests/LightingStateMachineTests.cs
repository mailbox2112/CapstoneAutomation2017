using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

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
            Assert.IsInstanceOfType(testMachine, typeof(LightingStateMachine));
        }

        [TestMethod]
        public void TestLightingStateDecisions()
        {
            testMachine = new LightingStateMachine();
            GreenhouseState result = testMachine.DetermineState(30, 100);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(result == GreenhouseState.LIGHTING);

            testMachine.DetermineState(100, 30);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            result = testMachine.DetermineState(30, 100);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(result, testMachine);
            }
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.LIGHTING);
        }

        [TestMethod]
        public void TestConvertLightingStateToCommands()
        {
            testMachine = new LightingStateMachine();
            GreenhouseState state = GreenhouseState.LIGHTING;
            List<Commands> results = testMachine.ConvertStateToCommands(state);
            Assert.IsTrue(results[0] == Commands.LIGHTS_ON);

            state = GreenhouseState.WAITING_FOR_DATA;
            results = testMachine.ConvertStateToCommands(state);
            Assert.IsTrue(results[0] == Commands.LIGHTS_OFF);
        }
    }
}
