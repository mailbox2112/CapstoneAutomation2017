using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

namespace GreenhouseUnitTests
{
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
            DateTime currentTime = new DateTime(2017, 7, 4, 17, 30, 0);
            double moisture = 30;
            GreenhouseState result;

            #region Automation Tests
            #region Processing_Data Tests
            // Test case: automated mode,
            // coming from the wait state,
            // WITHIN scheduled time, WITHOUT overrides
            // Result: watering
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualWater = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(currentTime, moisture);
            Assert.IsTrue(result == GreenhouseState.WATERING);

            // Test case: automated mode,
            // coming from wait state,
            // within scheduled time,
            // with overrides
            // value below threshold
            // Result: watering
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualWater = null;
            testMachine.OverrideThreshold = 70;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;

            result = testMachine.DetermineState(currentTime, moisture);
            Assert.IsTrue(result == GreenhouseState.WATERING);

            // Test case: automated mode,
            // coming from wait state,
            // within scheduled time,
            // with overrides
            // value above threshold
            // Result: waiting
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualWater = null;
            testMachine.OverrideThreshold = 70;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;

            result = testMachine.DetermineState(currentTime, 90);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: automated mode,
            // coming from wait state,
            // outside scheduled time,
            // Result: waiting
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualWater = null;

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 20, 0, 0), moisture);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case:
            // coming from wait state
            // scheduletype BLOCKED
            // Result: waiting/no change
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.BLOCKED;

            result = testMachine.DetermineState(currentTime, 55);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);
            #endregion
            #region Processing_Water Tests
            // Test case: coming from watering state,
            // within scheduled time
            // without overrides
            // Result: keep water on
            testMachine.CurrentState = GreenhouseState.WATERING;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(currentTime, moisture);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WATERING);

            // Test case: coming from watering state,
            // within scheduled time
            // with overrides
            // value below threshold
            // Result: keep water on
            testMachine.CurrentState = GreenhouseState.WATERING;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;
            testMachine.OverrideThreshold = 70;

            result = testMachine.DetermineState(currentTime, moisture);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WATERING);

            // Test case: coming from watering state
            // within scheduled time
            // with overrides
            // value above threshold
            // Result: turn water off
            testMachine.CurrentState = GreenhouseState.WATERING;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;
            testMachine.OverrideThreshold = 70;

            result = testMachine.DetermineState(currentTime, 90);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: coming from watering state
            // outside scheduled time
            // Result: turn water off
            testMachine.CurrentState = GreenhouseState.WATERING;

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 20, 0, 0), moisture);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: coming from watering state
            // scheduletype BLOCKED
            // Result: turn water off
            testMachine.CurrentState = GreenhouseState.WATERING;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.BLOCKED;

            result = testMachine.DetermineState(currentTime, 45);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);
            #endregion
            #endregion

            #region Manual Tests
            // Test case: coming from wait state
            // manual water on command
            // Result: watering
            testMachine.ManualWater = true;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(currentTime, moisture);
            Assert.IsTrue(result == GreenhouseState.WATERING);

            // Test case: coming from watering state
            // manual water on command
            // Result: watering/no change
            testMachine.ManualWater = true;
            testMachine.CurrentState = GreenhouseState.WATERING;

            result = testMachine.DetermineState(currentTime, moisture);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WATERING);

            // Test case: coming from waiting state
            // manual water off command
            // Result: waiting/no change
            testMachine.ManualWater = false;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(currentTime, moisture);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: coming from watering state
            // manual water off command
            // Result: waiting
            testMachine.ManualWater = false;
            testMachine.CurrentState = GreenhouseState.WATERING;

            result = testMachine.DetermineState(currentTime, moisture);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);
            #endregion
        }

        [TestMethod]
        public void TestConvertWateringStateToCommands()
        {
            testMachine = new WateringStateMachine(1);
            List<Commands> result = new List<Commands>();

            // Test zone 1 water on
            result = testMachine.ConvertStateToCommands(GreenhouseState.WATERING);
            Assert.IsTrue(result.Contains(Commands.WATER1_ON));

            // Test zone 2 water on
            testMachine.Zone = 2;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WATERING);
            Assert.IsTrue(result.Contains(Commands.WATER2_ON));

            // Test zone 3 water on
            testMachine.Zone = 3;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WATERING);
            Assert.IsTrue(result.Contains(Commands.WATER3_ON));

            // Test zone 4 water on
            testMachine.Zone = 4;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WATERING);
            Assert.IsTrue(result.Contains(Commands.WATER4_ON));

            // Test zone 5 water on
            testMachine.Zone = 5;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WATERING);
            Assert.IsTrue(result.Contains(Commands.WATER5_ON));

            // Test zone 6 water on
            testMachine.Zone = 6;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WATERING);
            Assert.IsTrue(result.Contains(Commands.WATER6_ON));

            // Test zone 1 water off
            testMachine.Zone = 1;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.WATER1_OFF));

            // Test zone 2 water off
            testMachine.Zone = 2;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.WATER2_OFF));

            // Test zone 3 water off
            testMachine.Zone = 3;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.WATER3_OFF));

            // Test zone 4 water off
            testMachine.Zone = 4;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.WATER4_OFF));

            // Test zone 5 water off
            testMachine.Zone = 5;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.WATER5_OFF));

            // Test zone 6 water off
            testMachine.Zone = 6;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.WATER6_OFF));
        }
    }
}
