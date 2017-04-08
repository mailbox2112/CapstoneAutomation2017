using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

namespace GreenhouseUnitTests
{
    // TODO: New unit tests!
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
            testMachine.HighLimit = 85;
            testMachine.LowLimit = 55;
            GreenhouseState result;

            // Test processing_heating decisions
            testMachine.CurrentState = GreenhouseState.HEATING;
            result = testMachine.DetermineState(45);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE && testMachine.CurrentState == GreenhouseState.HEATING);

            result = testMachine.DetermineState((double)testMachine.LowLimit + 5);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test processing_cooling decisions
            testMachine.CurrentState = GreenhouseState.COOLING;
            result = testMachine.DetermineState(90);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE && testMachine.CurrentState == GreenhouseState.COOLING);

            result = testMachine.DetermineState((double)testMachine.HighLimit - 5);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);
            
            // Test processing_data decisions
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            result = testMachine.DetermineState(45);
            Assert.IsTrue(result == GreenhouseState.HEATING);

            result = testMachine.DetermineState(95);
            Assert.IsTrue(result == GreenhouseState.COOLING);

            result = testMachine.DetermineState(73);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE && testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test for error conditions
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            result = testMachine.DetermineState(150);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            result = testMachine.DetermineState(0);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            testMachine.CurrentState = GreenhouseState.HEATING;
            result = testMachine.DetermineState(150);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            result = testMachine.DetermineState(0);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            testMachine.CurrentState = GreenhouseState.COOLING;
            result = testMachine.DetermineState(150);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            result = testMachine.DetermineState(0);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            // Test manual control
            // Test manual cooling
            testMachine.ManualCool = true;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            result = testMachine.DetermineState(80);
            Assert.IsTrue(result == GreenhouseState.COOLING);

            testMachine.CurrentState = GreenhouseState.COOLING;
            result = testMachine.DetermineState(80);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);

            // Test when both manual heat and cooling are true
            testMachine.ManualHeat = true;
            result = testMachine.DetermineState(75);
            Assert.IsTrue(result == GreenhouseState.ERROR);

            testMachine.ManualCool = false;

            // Test manual heating
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            result = testMachine.DetermineState(75);
            Assert.IsTrue(result == GreenhouseState.HEATING);

            testMachine.CurrentState = GreenhouseState.HEATING;
            result = testMachine.DetermineState(75);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
        }

        [TestMethod]
        public void TestConvertTemperatureStateToCommands()
        {
            throw new NotImplementedException();
        }
    }
}
