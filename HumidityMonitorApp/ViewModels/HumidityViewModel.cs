using System;
using HumidityMonitorApp.DAL;

namespace HumidityMonitorApp.ViewModels
{
    public class HumidityViewModel: ViewModelBase
    {
        string _location;
        DateTime? _date;
        int? _humidity;

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

        public DateTime? Date
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

        public int? Humidity
        {
            get
            {
                return _humidity;
            }

            set
            {
                if (_humidity != value)
                {
                    _humidity = value;
                    OnPropertyChanged();
                }
            }
        }

        public static HumidityViewModel MapFromModel (HumidityModel model){
            return new HumidityViewModel
            {
                Humidity = model.Humidity,
                Location = model.Location,
                Date = DateTime.Parse(model.Date, null, System.Globalization.DateTimeStyles.RoundtripKind)
            };
        }
    }
}
