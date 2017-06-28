////////////////////////////////////////////////////////////////////////////
//
//  This file is part of HumidityMonitor
//  Copyright (c) 2017, Shawn Lam
//
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

using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace HumidityLoggerActions
{
    public class AverageHumidityAction : ILoggerAction<double>
    {
        private const byte ReadingsBeforeAveraging = 12;
        private ConcurrentBag<double> _humidityReadings;
        private readonly ILoggerAction<double>[] _childLoggerActions;

        public AverageHumidityAction(params ILoggerAction<double>[] childLoggerActions)
        {
            _childLoggerActions = childLoggerActions;
            _humidityReadings = new ConcurrentBag<double>();
        }

        public async Task LogAction(double obj)
        {
            _humidityReadings.Add(obj);

            if (_humidityReadings.Count >= 12)
            {
                var average = _humidityReadings.Average();
                _humidityReadings = new ConcurrentBag<double>();

                foreach (var action in _childLoggerActions)
                {
                    await action.LogAction(average);
                }
            }
        }
        
    }
}
