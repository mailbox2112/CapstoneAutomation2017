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
            // Make a time object to send into the state machine
            DateTime currentTime = new DateTime(2017, 7, 4, 17, 30, 0);
            double lightAvg = 45000;

            // Set up state machine
            testMachine = new LightingStateMachine(1);
            testMachine.Begin = new DateTime(2017, 7, 4, 17, 0, 0);
            testMachine.End = new DateTime(2017, 7, 4, 18, 0, 0);
            testMachine.OverrideThreshold = 55000;
            GreenhouseState result;

            #region Automation Mode Tests
            #region Processing_Data Tests
            // Test case: lighting is in automation mode,
            // coming from wait state, WITHOUT sensor override
            // WITHIN scheduled time
            // Result: LIGHTING
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(currentTime, lightAvg);
            Assert.IsTrue(result == GreenhouseState.LIGHTING);

            // Test case: lighting is in automation mode,
            // coming from wait state, WITH sensor override.
            // Light values are BELOW threshold
            // WITHIN scheduled time
            // Result: LIGHTING
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;

            result = testMachine.DetermineState(currentTime, lightAvg);
            Assert.IsTrue(result == GreenhouseState.LIGHTING);

            // Test case: lighting is in automation mode,
            // coming from wait state, WITH sensor override.
            // Light valuees are ABOVE threshold
            // WITHIN scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;

            result = testMachine.DetermineState(currentTime, 65000);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting is in automation mode,
            // coming from wait state, WITHOUT sensor override.
            // Light values are BELOW threshold
            // OUTSIDE scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 19, 0, 0), lightAvg);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting is in automation mode,
            // coming from wait state, WITHOUT sensor override.
            // Light values are ABOVE threshold
            // OUTSIDE scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 19, 0, 0), 65000);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting in automation mode,
            // coming from wait state, BLOCKED scheduletype
            // Light values should be irrelevant
            // Result: WAITING/NO CHANGE
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.BLOCKED;

            result = testMachine.DetermineState(currentTime, 45000);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);
            #endregion
            #region Processing_Lighting Tests
            // Test case: lighting is in automation mode
            // coming from lighting state, WITHOUT sensor override
            // Light values are BELOW threshold
            // WITHIN scheduled time
            // Result: LIGHTING/NO CHANGE
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(currentTime, lightAvg);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.LIGHTING);

            // Test case: lighting is in automation mode
            // coming from lighting state, WITHOUT sensor override
            // Light values are ABOVE threshold
            // WITHIN scheduled time
            // Result: LIGHTING/NO CHANGE
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(currentTime, 65000);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.LIGHTING);

            // Test case: lighting is in automation mode
            // coming from lighting state, WITHOUT sensor override
            // Light values are BELOW threshold
            // OUTSIDE scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 20, 0, 0), lightAvg);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting is in automation mode
            // coming from lighting state, WITHOUT sensor override
            // Light values are ABOVE threshold
            // OUTSIDE scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.CONSTANT;

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 20, 0, 0), 65000);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting is in automation mode
            // coming from lighting state, WITH sensor override
            // Light values are BELOW threshold
            // WITHIN scheduled time
            // Result: LIGHTING/NO CHANGE
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;

            result = testMachine.DetermineState(currentTime, lightAvg);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.LIGHTING);

            // Test case: lighting is in automation mode
            // coming from lighting state, WITH sensor override
            // Light values are ABOVE threshold
            // WITHIN scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;

            result = testMachine.DetermineState(currentTime, 65000);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting is in automated mode
            // coming from lighting state, WITH sensor override
            // Light values are BELOW threshold
            // OUTSIDE scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 20, 0, 0), lightAvg);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting is in automated mode
            // coming from lighting state, WITH sensor override
            // Light values are ABOVE threshold
            // OUTSIDE scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ManualLight = null;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.SENSORS;

            result = testMachine.DetermineState(new DateTime(2017, 7, 4, 20, 0, 0), 65000);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting is in automated mode
            // coming from lighting state, BLOCKED scheduletype
            // Light values are irrelevant
            // WITHIN scheduled time
            // Result: WAITING
            testMachine.CurrentState = GreenhouseState.LIGHTING;
            testMachine.ScheduleType = GreenhouseController.Limits.ScheduleTypes.BLOCKED;

            result = testMachine.DetermineState(currentTime, 60000);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);
            #endregion
            #endregion

            #region Manual Mode Tests
            // Test case: lighting is manually on
            // and we're coming from the waiting state
            testMachine.ManualLight = true;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(currentTime, lightAvg);
            Assert.IsTrue(result == GreenhouseState.LIGHTING);

            // Test case: lighting is manually on
            // and we're coming from the lighting state
            testMachine.ManualLight = true;
            testMachine.CurrentState = GreenhouseState.LIGHTING;

            result = testMachine.DetermineState(currentTime, lightAvg);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.LIGHTING);

            // Test case: lighting is manually off
            // and we're coming from the waiting state
            testMachine.ManualLight = false;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(currentTime, lightAvg);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: lighting is manually off
            // and we're coming from the lighting state
            testMachine.ManualLight = false;
            testMachine.CurrentState = GreenhouseState.LIGHTING;

            result = testMachine.DetermineState(currentTime, lightAvg);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);
            #endregion
        }

        [TestMethod]
        public void TestConvertLightingStateToCommands()
        {
            List<Commands> result = new List<Commands>();
            testMachine = new LightingStateMachine(1);

            // Test case: light on, zone 1
            result = testMachine.ConvertStateToCommands(GreenhouseState.LIGHTING);
            Assert.IsTrue(result.Contains(Commands.LIGHT1_ON));

            // Test case: light on, zone 2
            testMachine.Zone = 2;
            result = testMachine.ConvertStateToCommands(GreenhouseState.LIGHTING);
            Assert.IsTrue(result.Contains(Commands.LIGHT2_ON));

            // Test case: light on, zone 3
            testMachine.Zone = 3;
            result = testMachine.ConvertStateToCommands(GreenhouseState.LIGHTING);
            Assert.IsTrue(result.Contains(Commands.LIGHT3_ON));

            // Test case: light off, zone 1
            testMachine.Zone = 1;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.LIGHT1_OFF));

            // Test case: light off, zone 2
            testMachine.Zone = 2;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.LIGHT2_OFF));

            // Test case: light off, zone 3
            testMachine.Zone = 3;
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.LIGHT3_OFF));
        }
    }
}
