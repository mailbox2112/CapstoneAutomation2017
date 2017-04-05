using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class LimitsAnalyzerTests
    {
        [TestMethod]
        public void TestSettingGreenhouseLimits()
        {
            // Set up all the values we need to create the limits
            int[] lightZones = new int[] { 1, 2, 3, 4, 5 };
            int[] waterZones = new int[] { 1, 2, 3, 4, 5, 6 };
            int tempHi = 90, tempLo = 60, shadeLim = 55000;
            Dictionary<int, DateTime> lightStart = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> lightEnd = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> waterStart = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> waterEnd = new Dictionary<int, DateTime>();
            DateTime start = new DateTime(1995, 7, 4, 0, 0, 0);
            DateTime end = new DateTime(1995, 7, 4, 12, 0, 0);

            // Populate dictionaries
            foreach (int zone in lightZones)
            {
                lightStart.Add(zone, start);
                lightEnd.Add(zone, end);
            }
            foreach (int zone in waterZones)
            {
                waterStart.Add(zone, start);
                waterEnd.Add(zone, end);
            }
            
            // Create the packet
            LimitPacket packet = new LimitPacket()
            {
                TempHi = tempHi,
                TempLo = tempLo,
                ShadeLim = shadeLim,
                LightStarts = lightStart,
                LightEnds = lightEnd,
                WaterStarts = waterStart,
                WaterEnds = waterEnd
            };

            // Create the analyzer and change the limits
            LimitsAnalyzer analyzer = new LimitsAnalyzer();
            analyzer.ChangeGreenhouseLimits(packet);

            // Check the limits
            Assert.IsTrue(StateMachineContainer.Instance.Temperature.HighLimit == tempHi);
            Assert.IsTrue(StateMachineContainer.Instance.Temperature.LowLimit == tempLo);
            Assert.IsTrue(StateMachineContainer.Instance.Shading.HighLimit == shadeLim);
            foreach (LightingStateMachine stateMachine in StateMachineContainer.Instance.LightStateMachines)
            {

            }
        }
    }
}
