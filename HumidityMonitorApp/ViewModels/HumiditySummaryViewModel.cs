using System;
using HumidityMonitorApp.DAL;

namespace HumidityMonitorApp.ViewModels
{
    public class HumiditySummaryViewModel : ViewModelBase
    {
        string _location;
        DateTime _date;
        int? _min;
        int? _max;

        public string Location
        {
            get
            {
                return _location;
            }

            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }

            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? Min
        {
            get
            {
                return _min;
            }

            set
            {
                if (_min != value)
                {
                    _min = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? Max
        {
            get
            {
                return _max;
            }

            set
            {
                if (_max != value){
                    _max = value;
                    OnPropertyChanged();
                }
            }
        }

        public static HumiditySummaryViewModel MapFromModel(HumiditySummaryModel model)
        {
            return new HumiditySummaryViewModel
            {
                Location = model.Location,
                Min = model.Min,
                Max = model.Max,
                Date = DateTime.Parse(model.Date, null, System.Globalization.DateTimeStyles.RoundtripKind)
            };
        }
    }
}