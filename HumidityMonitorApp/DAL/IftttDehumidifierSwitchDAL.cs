using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HumidityMonitorApp.DAL
{
    public class IftttDehumidifierSwitchDAL
    {
        private readonly HttpClient _client = new HttpClient();
        private Uri _switchOnUri;
        private Uri _switchOffUri;

        public IftttDehumidifierSwitchDAL(string iftttWebhookKey)
        {
            _switchOnUri = new Uri($"https://maker.ifttt.com/trigger/switch_on_dehumidifier/with/key/{iftttWebhookKey}");
            _switchOffUri = new Uri($"https://maker.ifttt.com/trigger/switch_off_dehumidifier/with/key/{iftttWebhookKey}");
        }

        public void SwitchOnDehumidifier(){
            Task.Run(async () => await _client.GetAsync(_switchOnUri));
        }

        public void SwitchOffDehumidifier()
        {
            Task.Run(async () => await _client.GetAsync(_switchOffUri));
        }
    }
}
