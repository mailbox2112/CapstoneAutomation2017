using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

namespace GreenhouseUnitTests
{
    // TODO: Write new unit tests!
    [TestClass]
    public class LightingStateMachineTests
    {
        LightingStateMachine testMachine;
        [TestMethod]
        public void TestLightingStateMachineCreation()
        {
            testMachine = new LightingStateMachine(1);
            Assert.IsNotNull(testMachine);
            Assert.IsInstanceOfType(testMachine, typeof(LightingStateMachine));
        }

        [TestMethod]
        public void TestLightingStateDecisions()
        {
            testMachine = new LightingStateMachine(1);
            testMachine.Begin = new DateTime(2017, 7, 4, 17, 0, 0);
            testMachine.End = new DateTime(2017, 7, 4, 18, 0, 0);
            GreenhouseState result;

            // Test processing_data decisions
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 17, 1, 0), 0);
            Assert.IsTrue(result == GreenhouseState.LIGHTING);

            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 19, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);

            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            result = testMachine.DetermineState(new DateTime(2017, 7, 3, 0, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);

            // Test processing_lighting decisions
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 17, 34, 0), 0);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE && testMachine.CurrentState == GreenhouseState.LIGHTING);

            testMachine.CurrentState = GreenhouseState.LIGHTING;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 19, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test manual lighting controls
            testMachine.ManualLight = true;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            result = testMachine.DetermineState(new DateTime(2017, 4, 4, 8, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.LIGHTING);

            testMachine.CurrentState = GreenhouseState.LIGHTING;
            result = testMachine.DetermineState(new DateTime(2017, 4, 4, 8, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE && testMachine.CurrentState == GreenhouseState.LIGHTING);
        }

        [TestMethod]
        public void TestConvertLightingStateToCommands()
        {
            throw new NotImplementedException();
        }
    }
}
