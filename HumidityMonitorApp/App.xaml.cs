using Xamarin.Forms;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace HumidityMonitorApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new HumidityMonitorAppPage();
        }

        protected override void OnStart()
        {
            MobileCenter.Start("android=0fad6de0-e35c-40e7-b22e-7cac385e8de8;" +
                   "ios=8b9ef20b-933f-4717-bbdf-747d34af1ff7",
                   typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
