////////////////////////////////////////////////////////////////////////////
//
//  This file is part of HumidityMonitor
//  Copyright (c) 2017, Shawn Lam
//
//  Portions of this file is part of Rpi.SenseHat.Demo
//  Copyright (c) 2016-2017, Mattias Larsson
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy of 
//  this software and associated documentation files (the "Software"), to deal in 
//  the Software without restriction, including without limitation the rights to use, 
//  copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
//  Software, and to permit persons to whom the Software is furnished to do so, 
//  subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all 
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//  INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//  PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
//  SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Emmellsoft.IoT.Rpi.SenseHat;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using Windows.System.Threading;
using HumidityLoggerActions;
using System.Collections.Generic;

namespace HumidityLogger
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, ILoggerAction<double>
    {
        public readonly ObservableCollection<string> Logs = new ObservableCollection<string>();

        public MainPage()
        {
            this.InitializeComponent();
            this.LogList.ItemsSource = Logs;
            SetUpAndLaunchTimer();
        }

        private void SetUpAndLaunchTimer()
        {
            Task.Run(async () =>
            {
                var senseHat = await SenseHatFactory.GetSenseHat().ConfigureAwait(false);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    if (senseHat == null)
                    {
                        WaitingTextBlock.Text = "No Sense Hat found. Please restart the app to try again.";
                    }
                    else
                    {
                        WaitingTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        LogList.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                });

                var actions = new List<ILoggerAction<double>>();
                actions.Add(new SenseHatHumidityDisplay(senseHat));


                actions.Add(new AverageHumidityAction(this, new TableStorageHumidityLogger("TODO - Location code",
                    "TODO - Azure table storage connection string", "HumidityLog")));

                TimeSpan period = TimeSpan.FromSeconds(5);

                ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    senseHat.Sensors.HumiditySensor.Update();

                    if (senseHat.Sensors.Humidity.HasValue)
                    {
                        double humidityValue = senseHat.Sensors.Humidity.Value;

                        foreach (var action in actions)
                        {
                            await action.LogAction(humidityValue).ConfigureAwait(false);
                        }
                    }
                }, period);
            }).ConfigureAwait(false);
        }

        public async Task LogAction(double value)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    Logs.Add($"{DateTime.UtcNow:yyyy-MM-ddThh:mm:ssZ} {value:0.0}% RH");
                    LogList.SelectedIndex = LogList.Items.Count - 1;
                    LogList.ScrollIntoView(LogList.SelectedItem);
                });
        }
    }
}
