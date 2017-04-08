using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

namespace GreenhouseUnitTests
{
    // TODO: Write new unit tests!
    [TestClass]
    public class WateringStateMachineTests
    {
        WateringStateMachine testMachine;

        [TestMethod]
        public void TestWateringStateMachineCreation()
        {
            testMachine = new WateringStateMachine(1);
            Assert.IsNotNull(testMachine);
            Assert.IsInstanceOfType(testMachine, typeof(WateringStateMachine));
        }

        [TestMethod]
        public void TestWateringStateDecisions()
        {
            testMachine = new WateringStateMachine(1);
            testMachine.Begin = new DateTime(2017, 7, 4, 17, 0, 0);
            testMachine.End = new DateTime(2017, 7, 4, 18, 0, 0);
            testMachine.ManualWater = false;

            // Test processing_data decisions
            GreenhouseState result = testMachine.DetermineState(new DateTime(2017, 7, 4, 17, 34, 0), 0);
            Assert.IsTrue(result == GreenhouseState.WATERING);

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 17, 34, 0), 75);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 14, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 20, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);

            // Test processing_watering decisions
            testMachine.CurrentState = GreenhouseState.WATERING;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 17, 35, 0), 0);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE && testMachine.CurrentState == GreenhouseState.WATERING);

            testMachine.CurrentState = GreenhouseState.WATERING;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 17, 35, 0), 80);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            testMachine.CurrentState = GreenhouseState.WATERING;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 20, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Time is before scheduled watering, moisture not above limit, 
            testMachine.CurrentState = GreenhouseState.WATERING;
            result = testMachine.DetermineState(new DateTime(2017, 7, 3, 0, 0, 0), 0);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA, result.ToString());

            // Test manual controls
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualWater = true;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 17, 35, 0), 0);
            Assert.IsTrue(result == GreenhouseState.WATERING);

            testMachine.CurrentState = GreenhouseState.WATERING;
            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 17, 35, 0), 0);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE && testMachine.CurrentState == GreenhouseState.WATERING);
        }

        [TestMethod]
        public void TestConvertWateringStateToCommands()
        {
            throw new NotImplementedException();
        }
    }
}
