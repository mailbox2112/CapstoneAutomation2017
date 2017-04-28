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
            testMachine.LowLimit = 65;
            GreenhouseState result;

            #region Automated Decision Tests
            // Test setup
            testMachine.ManualCool = null;
            testMachine.ManualHeat = null;

            #region Processing_data Tests
            // Test case: automated mode
            // coming from waiting state
            // value within high and low limits
            // Result: waiting
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(75);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: automated mode
            // coming from waiting state
            // value above high limit but below emergency high
            // Result: cooling
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(100);
            Assert.IsTrue(result == GreenhouseState.COOLING);

            // Test case: automated mode
            // coming from waiting state
            // value above emergency high
            // Result: emergency
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(150);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            // Test case: automated mode
            // coming from waiting state
            // value below low but above emergency low
            // Result: heating
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(55);
            Assert.IsTrue(result == GreenhouseState.HEATING);

            // Test case: automated mode
            // coming from waiting state
            // value below emergency low
            // Result: emergency
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(0);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);
            #endregion
            #region Processing_cooling Tests
            // Test case: automated mode
            // coming from cooling state
            // value high but not above emergency high
            // Result: no change/cooling
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(100);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.COOLING);

            // Test case: automated mode
            // coming from cooling state
            // value not below hysteresis
            // Result: cooling/no change
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(83);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.COOLING);

            // Test case: automated mode
            // coming from cooling state
            // value above emergency high
            // Result: emergency
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(150);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            // Test case: automated mode
            // coming form cooling state
            // value within limits w/ hysteresis
            // Result : waiting
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(75);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: automated mode
            // coming from cooling state
            // value below low limit but not emergency low
            // Result: heating
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(45);
            Assert.IsTrue(result == GreenhouseState.HEATING);

            // Test case: automated mode
            // coming from cooling state
            // value below emergency low
            // Result: emergency
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(0);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);
            #endregion
            #region Processing_heating Tests
            // Test case: automated mode
            // coming from heating state
            // value still below low but above emergency
            // Result: heating/no change
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(45);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.HEATING);
            
            // Test case: automated mode
            // coming from heating state
            // value below emergency low
            // Result: emergency
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(0);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);

            // Test case: automated mode
            // coming from heating state
            // value between high lim and low lim w/ hysteresis
            // Result: waiting
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(75);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: automated mode
            // coming from heating state
            // value above low lim, below hysteresis value
            // Result: heating
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(68);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.HEATING);

            // Test case: automated mode
            // coming from heating state
            // value above high lim but below emergency high
            // Result: cooling
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(100);
            Assert.IsTrue(result == GreenhouseState.COOLING);

            // Test case: automated mode
            // coming from heating state
            // value above emergency high
            // Result: emergency
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(160);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);
            #endregion
            #endregion
            #region Manual Mode Tests
            // Setup
            testMachine.ManualHeat = true;

            // Test case: manual heat on
            // coming from heat state
            // Result: heating/no change
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.HEATING);

            // Test case: manual heat on
            // coming from wait state
            // Result: heating
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.HEATING);

            // Test case: manual heat on
            // coming from cool state
            // Result: heating
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.HEATING);

            // Test case: manual heat off
            // coming from heat state
            // Result: waiting
            testMachine.ManualHeat = false;
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: manual heat off
            // coming from wait state
            // Result: waiting
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: manual heat off
            // coming from cooling state
            // Result: waiting
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.COOLING);

            // Test case: manual cool on
            // coming from wait state
            // Result: cooling
            testMachine.ManualCool = true;
            testMachine.ManualHeat = null;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.COOLING);

            // Test case: manual cool on
            // coming from heat state
            // Result: cooling
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.COOLING);

            // Test case: manual cool on
            // coming from cool state
            // Result: cooling/no change
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.COOLING);

            // Test case: manual cool off
            // coming from cool state
            // Result: waiting
            testMachine.ManualCool = false;
            testMachine.CurrentState = GreenhouseState.COOLING;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: manual cool off
            // coming from heat state
            // Result: heating/no change
            testMachine.CurrentState = GreenhouseState.HEATING;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.HEATING);

            // Test case: manual cool off
            // coming from wait state
            // Result: waiting/no change
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: both manual cool and heat on
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;
            testMachine.ManualCool = true;
            testMachine.ManualHeat = true;

            result = testMachine.DetermineState(70);
            Assert.IsTrue(result == GreenhouseState.ERROR);
            #endregion
        }

        [TestMethod]
        public void TestConvertTemperatureStateToCommands()
        {
            // TODO: implement this!
            throw new NotImplementedException();
        }
    }
}
