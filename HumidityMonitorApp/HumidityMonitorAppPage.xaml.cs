using System;
using HumidityMonitorApp.ViewModels;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

namespace HumidityMonitorApp
{
    public partial class HumidityMonitorAppPage : ContentPage
    {
        private readonly HumidityMonitorAppViewModel _viewModel;
        private double _width = 0;
        private double _height = 0;
        private const double EPSILON = 0.1;

        public HumidityMonitorAppPage()
        {
            InitializeComponent();
            _viewModel = new HumidityMonitorAppViewModel();
            BindingContext = _viewModel;

            //Set default options and invoke default commands
            _viewModel.ViewSwitch = true;
            _viewModel.DetailedHumidityDate = DateTime.Today;
            _viewModel.HumiditySummaryStartDate = DateTime.Today.AddDays((int)DateTime.Today.DayOfWeek * -1);
            _viewModel.HumiditySummaryEndDate = _viewModel.HumiditySummaryStartDate.AddDays(6);
            _viewModel.GetLatestHumidityCommand.Execute(null);

            Device.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                _viewModel.GetLatestHumidityCommand.Execute(null);
                return true;
            });
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (Math.Abs(width - this._width) > EPSILON || Math.Abs(height - this._height) > EPSILON)
            {
                this._width = width;
                this._height = height;
                //Respond to orientation changes here
            }
        }

        void PrimaryAxis_LabelCreated(object sender, ChartAxisLabelEventArgs e)
        {
            DateTime date = DateTime.Parse(e.LabelContent);

            e.LabelContent = date.ToString("ddd");
        }
    }
}
