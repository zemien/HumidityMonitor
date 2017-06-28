using Microsoft.WindowsAzure.Storage.Table;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace HumidityMonitorModels
{
    public class HumiditySummaryEntity: TableEntity
    {
        public HumiditySummaryEntity()
        {

        }

        public HumiditySummaryEntity(string location, LocalDateTime date, List<HumidityEntity> dataPoints)
        {
            PartitionKey = location;
            RowKey = date.ToDateTimeUnspecified().ToReverseTicks();

            if (dataPoints != null && dataPoints.Any())
            {
                Min = dataPoints.Min(he => he.RelativeHumidity);
                Max = dataPoints.Max(he => he.RelativeHumidity);
            }
        }

        public int? Min { get; set; }

        public int? Max { get; set; }

    }
}
