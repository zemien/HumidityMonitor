using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace HumidityMonitorModels
{
    public class HumidityEntity: TableEntity
    {
        public HumidityEntity()
        {

        }

        public HumidityEntity(string location, int relativeHumidity)
        {
            PartitionKey = location;
            RowKey = DateTime.UtcNow.ToReverseTicks();
            RelativeHumidity = relativeHumidity;
        }

        public int RelativeHumidity { get; set; }
    }
}