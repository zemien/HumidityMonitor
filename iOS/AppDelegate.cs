using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace HumidityMonitorApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());
            new Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer();
            return base.FinishedLaunching(app, options);
        }
    }
}
