namespace HumidityMonitorApi
{
    public class HumiditySummaryViewModel
    {
        public string Location { get; set; }

        public string Date { get; set; }
     
        public int? Min { get; set; }

        public int? Max { get; set; }
    }
}
