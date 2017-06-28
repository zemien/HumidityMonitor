using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HumidityMonitorApp.DAL;
using Xamarin.Forms;

namespace HumidityMonitorApp.ViewModels
{
    public class HumidityMonitorAppViewModel : ViewModelBase
    {

        string _location = "TODO - Location";
        private const string TIMEZONE = "TODO - IANA time zone code";
        HumidityViewModel _latestHumidity;
        ObservableCollection<HumidityViewModel> _detailedHumidityCollection;
        ObservableCollection<HumiditySummaryViewModel> _humiditySummaryCollection;
        DateTime _detailedHumidityDate;
        DateTime _humiditySummaryStartDate;
        DateTime _humiditySummaryEndDate;
        bool _canGetLatestHumidity;
        bool _isLoading;
        bool _viewSwitch;
        ViewEnum _currentView;
        readonly HumidityMonitorDAL _dal = new HumidityMonitorDAL("TODO - Backend API base URI");
        readonly IftttDehumidifierSwitchDAL _switchDal = new IftttDehumidifierSwitchDAL("TODO - IFTTT Maker Webhook Key");
        CancellationTokenSource _detailedLoadingCancellationTokenSource;
        CancellationTokenSource _summaryLoadingCancellationTokenSource;

        public HumidityMonitorAppViewModel()
        {
            _canGetLatestHumidity = true;
            _detailedHumidityCollection = new ObservableCollection<HumidityViewModel>();
            _humiditySummaryCollection = new ObservableCollection<HumiditySummaryViewModel>();
            _detailedHumidityDate = DateTime.MinValue;
            GetLatestHumidityCommand = new Command(async () => await GetLatestHumidity(), () => _canGetLatestHumidity);
            MoveToNextDateCommand = new Command(() => MoveToNextDate());
            MoveToPreviousDateCommand = new Command(() => MoveToPreviousDate());
            SwitchOnDehumidifierCommand = new Command(() => _switchDal.SwitchOnDehumidifier());
            SwitchOffDehumidifierCommand = new Command(() => _switchDal.SwitchOffDehumidifier());
        }

        public ICommand GetLatestHumidityCommand { get; private set; }

        public ICommand MoveToPreviousDateCommand { get; private set; }

        public ICommand MoveToNextDateCommand { get; private set; }

        public ICommand SwitchOnDehumidifierCommand { get; private set; }

        public ICommand SwitchOffDehumidifierCommand { get; private set; }

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

        public HumidityViewModel LatestHumidity
        {
            get
            {
                return _latestHumidity;
            }

            set
            {
                if (_latestHumidity != value)
                {
                    _latestHumidity = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<HumidityViewModel> DetailedHumidityCollection
        {
            get
            {
                return _detailedHumidityCollection;
            }
            set
            {
                if (_detailedHumidityCollection != value)
                {
                    _detailedHumidityCollection = value;
                    OnPropertyChanged();
                }
            }
        }


        public ObservableCollection<HumiditySummaryViewModel> HumiditySummaryCollection
        {
            get
            {
                return _humiditySummaryCollection;
            }
            set
            {
                if (_humiditySummaryCollection != value)
                {
                    _humiditySummaryCollection = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime DetailedHumidityDate
        {
            get
            {
                return _detailedHumidityDate;
            }
            set
            {
                if (_detailedHumidityDate != value)
                {
                    _detailedHumidityDate = value;
                    OnPropertyChanged();

                    if (_detailedLoadingCancellationTokenSource != null)
                    {
                        _detailedLoadingCancellationTokenSource.Cancel();
                    }

                    _detailedLoadingCancellationTokenSource = new CancellationTokenSource();

                    Device.BeginInvokeOnMainThread(async () => await GetDetailedHumidity(value, _detailedLoadingCancellationTokenSource.Token));
                }
            }
        }

        public DateTime HumiditySummaryStartDate
        {
            get
            {
                return _humiditySummaryStartDate;
            }
            set
            {
                if (_humiditySummaryStartDate != value)
                {
                    _humiditySummaryStartDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime HumiditySummaryEndDate
        {
            get
            {
                return _humiditySummaryEndDate;
            }
            set
            {
                if (_humiditySummaryEndDate != value)
                {
                    _humiditySummaryEndDate = value;
                    OnPropertyChanged();

                    if (_summaryLoadingCancellationTokenSource != null)
                    {
                        _summaryLoadingCancellationTokenSource.Cancel();
                    }

                    _summaryLoadingCancellationTokenSource = new CancellationTokenSource();

                    Device.BeginInvokeOnMainThread(async () => await GetHumiditySummary(_humiditySummaryStartDate, value, _summaryLoadingCancellationTokenSource.Token));
                }
            }
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ViewSwitch
        {
            get
            {
                return _viewSwitch;
            }
            set
            {
                if (_viewSwitch != value)
                {
                    _viewSwitch = value;
                    CurrentView = value ? ViewEnum.Day : ViewEnum.Week;
                    OnPropertyChanged();
                }
            }
        }

        public ViewEnum CurrentView
        {
            get
            {
                return _currentView;
            }
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    ViewSwitch = value == ViewEnum.Day;
                    OnPropertyChanged();
                }
            }
        }

        async Task GetLatestHumidity()
        {
            CanGetLatestHumidity(false);

            var model = await _dal.GetLatestReadingAsync(_location, TIMEZONE);

            if (model != null)
            {
                LatestHumidity = HumidityViewModel.MapFromModel(model);
            }

            CanGetLatestHumidity(true);
        }

        async Task GetDetailedHumidity(DateTime date, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                IsLoading = true;
                var model = await _dal.GetDateHistory(_location, TIMEZONE, date);

                ct.ThrowIfCancellationRequested();

                if (model != null)
                {
                    var results = model.OrderBy(m => m.Date).Select(m => HumidityViewModel.MapFromModel(m)).ToList();

                    ct.ThrowIfCancellationRequested();

                    DetailedHumidityCollection.Clear();

                    foreach (var result in results)
                    {
                        DetailedHumidityCollection.Add(result);
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                IsLoading = false;
            }
        }

        async Task GetHumiditySummary(DateTime startDate, DateTime endDate, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                IsLoading = true;
                var model = await _dal.GetDateSummary(_location, TIMEZONE, startDate, endDate);

                ct.ThrowIfCancellationRequested();

                if (model != null)
                {
                    var results = model.OrderBy(m => m.Date).Select(m => HumiditySummaryViewModel.MapFromModel(m)).ToList();

                    ct.ThrowIfCancellationRequested();

                    HumiditySummaryCollection.Clear();

                    foreach (var result in results)
                    {
                        HumiditySummaryCollection.Add(result);
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                IsLoading = false;
            }
        }

        void CanGetLatestHumidity(bool value)
        {
            _canGetLatestHumidity = value;
            ((Command)GetLatestHumidityCommand).ChangeCanExecute();
        }

        void MoveToPreviousDate()
        {
            if (CurrentView == ViewEnum.Day)
            {
                if ((DetailedHumidityDate - DateTime.MinValue).Days >= 1)
                {
                    DetailedHumidityDate = DetailedHumidityDate.AddDays(-1);
                }
            }
            else
            {
                if ((HumiditySummaryStartDate - DateTime.MinValue).Days >= 7)
                {
                    HumiditySummaryStartDate = HumiditySummaryStartDate.AddDays(-7);
                    HumiditySummaryEndDate = HumiditySummaryEndDate.AddDays(-7);
                }
            }
        }

        void MoveToNextDate()
        {
            if (CurrentView == ViewEnum.Day)
            {
                if ((DateTime.MaxValue - DetailedHumidityDate).Days >= 1)
                {
                    DetailedHumidityDate = DetailedHumidityDate.AddDays(1);
                }
            }
            else
            {
                if ((DateTime.MaxValue - HumiditySummaryEndDate).Days >= 7)
                {
                    HumiditySummaryStartDate = HumiditySummaryStartDate.AddDays(7);
                    HumiditySummaryEndDate = HumiditySummaryEndDate.AddDays(7);
                }
            }
        }
    }
}
