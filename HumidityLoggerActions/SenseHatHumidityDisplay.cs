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
using Emmellsoft.IoT.Rpi.SenseHat.Fonts.SingleColor;
using System;
using System.Threading.Tasks;
using Windows.UI;

namespace HumidityLoggerActions
{
    public class SenseHatHumidityDisplay : ILoggerAction<double>
    {
        private readonly ISenseHat _senseHat;

        public SenseHatHumidityDisplay(ISenseHat senseHat)
        {
            _senseHat = senseHat;
        }

        public async Task LogAction(double obj)
        {
            await Task.Run(() =>
            {
                var tinyFont = new TinyFont();

                ISenseHatDisplay display = _senseHat.Display;

                double humidityValue = obj;

                int humidity = (int)Math.Round(humidityValue);

                string text = humidity.ToString();

                if (text.Length > 2)
                {
                    // Too long to fit the display!
                    text = "**";
                }

                display.Clear();
                Color color;
                color = GetTextColor(humidity);

                tinyFont.Write(display, text, color);
                display.Update();
            });
        }

        private static Color GetTextColor(int humidity)
        {
            Color color;
            switch (humidity)
            {
                case int n when (n < 50):
                    color = Colors.Red;
                    break;

                case int n when (n >= 50 && n < 80):
                    color = Colors.Green;
                    break;

                default:
                    color = Colors.Blue;
                    break;
            }

            return color;
        }
    }
}
