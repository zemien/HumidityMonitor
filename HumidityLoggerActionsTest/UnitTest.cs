using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HumidityMonitorModels;

namespace HumidityLoggerActionsTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReverseTick_Convert_Test()
        {
            var expected = new DateTime(2017, 1, 15, 15, 5, 23, DateTimeKind.Utc);
            Assert.AreEqual(expected, ReverseTicks.ToDateTime(expected.ToReverseTicks()));
        }

        [TestMethod]
        public void ReverseTick_ToReverseTicks_Test()
        {
            var inputDate = DateTime.MinValue;
            var actualString = inputDate.ToReverseTicks();
            Assert.AreEqual(19, actualString.Length);
        }
    }
}
