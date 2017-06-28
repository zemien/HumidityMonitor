using HumidityMonitorModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using System.Collections.Generic;

namespace HumidityLoggerActionsTest
{
    [TestClass]
    public class HumiditySummaryTest
    {
        [TestMethod]
        public void HumiditySummary_NullDataPoints_NoSummary()
        {
            var summary = new HumiditySummaryEntity("Lounge", new LocalDate(2017, 06, 10), null);
            Assert.IsNull(summary.Min);
            Assert.IsNull(summary.Max);
        }

        [TestMethod]
        public void HumiditySummary_NoDataPoints_NoSummary()
        {
            var summary = new HumiditySummaryEntity("Lounge", new LocalDate(2017, 06, 10), new List<HumidityEntity>());
            Assert.IsNull(summary.Min);
            Assert.IsNull(summary.Max);
        }

        [TestMethod]
        public void HumiditySummary_Location_IsPartitionKey()
        {
            var date = new LocalDate(2017, 06, 10);
            var summary = new HumiditySummaryEntity("Lounge", date, new List<HumidityEntity>());
            Assert.AreEqual("Lounge", summary.PartitionKey);
        }

        [TestMethod]
        public void HumiditySummary_Date_IsRowKey()
        {
            var date = new LocalDate(2017, 06, 10);
            var summary = new HumiditySummaryEntity("Lounge", date, new List<HumidityEntity>());
            Assert.AreEqual(date.ToString("yyyy-MM-dd", null), summary.RowKey);
        }

        [TestMethod]
        public void HumiditySummary_Min_IsSmallest()
        {
            var location = "Lounge";
            var date = new LocalDate(2017, 06, 10);
            var dataPoints = new List<HumidityEntity>
            {
                new HumidityEntity(location, 70),
                new HumidityEntity(location, 50),
                new HumidityEntity(location, 12),
                new HumidityEntity(location, 100),
                new HumidityEntity(location, 15)
            };
            var summary = new HumiditySummaryEntity(location, date, dataPoints);

            Assert.AreEqual(12, summary.Min);
        }

        [TestMethod]
        public void HumiditySummary_Max_IsLargest()
        {
            var location = "Lounge";
            var date = new LocalDate(2017, 06, 10);
            var dataPoints = new List<HumidityEntity>
            {
                new HumidityEntity(location, 70),
                new HumidityEntity(location, 50),
                new HumidityEntity(location, 12),
                new HumidityEntity(location, 100),
                new HumidityEntity(location, 15)
            };
            var summary = new HumiditySummaryEntity(location, date, dataPoints);

            Assert.AreEqual(100, summary.Max);
        }
    }
}
