using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HumidityMonitorApp.DAL
{
    public class HumidityMonitorDAL
    {
        private HttpClient _client;
        private readonly Uri _baseAddress;

        public HumidityMonitorDAL(string apiUrl)
        {
            _baseAddress = new Uri(apiUrl);
        }

        private HttpClient Client
        {
            get
            {
                if (_client == null) {
                    _client = new HttpClient{BaseAddress = _baseAddress};
                }

                return _client;
            }
        }

        public async Task<HumidityModel> GetLatestReadingAsync(string location, string timezone){
            var result = await Client.GetAsync($"/api/humidity?location={location}&ianaTimeZone={timezone}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK){
                var content = await result.Content.ReadAsStringAsync();
                var humidity = JsonConvert.DeserializeObject<HumidityModel>(content);

                if (humidity != null){
                    return humidity;
                }
            }

            return null;
        }

        public async Task<List<HumiditySummaryModel>> GetDateSummary(string location, string timezone, DateTime startDate, DateTime endDate){
            var result = await Client.GetAsync($"/api/humidity/summary?location={location}&ianaTimeZone={timezone}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await result.Content.ReadAsStringAsync();
                var humiditySummary = JsonConvert.DeserializeObject<List<HumiditySummaryModel>>(content);

                if (humiditySummary != null)
                {
                    return humiditySummary;
                }
            }

            return null;
        }


        public async Task<List<HumidityModel>> GetDateHistory(string location, string timezone, DateTime date){
            var result = await Client.GetAsync($"/api/humidity/history?location={location}&ianaTimeZone={timezone}&date={date:yyyy-MM-dd}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await result.Content.ReadAsStringAsync();
                var humidityHistory = JsonConvert.DeserializeObject<List<HumidityModel>>(content);

                if (humidityHistory != null)
                {
                    return humidityHistory;
                }
            }

            return null;
        }
    }
}
