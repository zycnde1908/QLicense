using Catel.MVVM;
using System.Threading.Tasks;
// using MathDemos.Helpers;
using MathDemos.Models;
using Catel.Data;
using OxyPlot;
using OxyPlot.Wpf;
using System.Windows.Input;

namespace MathDemos.ViewModels
{
    public class OutlierDetectionViewModel : ViewModelBase
    {
        public OutlierDetectionViewModel(/* dependency injection here */)
        {
            // TODO: 
            // DataGeration
            DataGeneratorViewModel = new DataGenerator(sampleRate: 30, drift: 20, varianceWaviness: 2, timeSum: 30, outlierCutoff: 3);
            OutlierDetectorViewModel = new OutlierDetector(varianceDecayTime:3, meanDecayTime: 3, outlierCutoff: 3, deltaT: 0.0333);
            // OutlierDetection
            // Visual show
            RawDataPlotModel = new OxyPlot.PlotModel();
            RawDataPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Title = "Time (s)",
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Key = "TimeAxis"
            });

            RawDataPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Title = "Value",
                Position = OxyPlot.Axes.AxisPosition.Left,
                Key = "ValueAxis"
            });

            RawDataPlotModel.Series.Add(new OxyPlot.Series.LineSeries
            {
                XAxisKey = "TimeAxis",
                YAxisKey = "ValueAxis",
                MarkerType = MarkerType.Circle,
                MarkerStrokeThickness = 1,
                MarkerSize = 2,
                MarkerFill = OxyColors.Blue,
                LineStyle = LineStyle.None,
                
            });
            this.RawDataPlotModel.InvalidatePlot(true);

            GenerateRawDataCommand = new Command(GenerateRawData);
        }
        
        public override string Title { get { return "View model title"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        public OxyPlot.PlotModel RawDataPlotModel { set; get; }


        public OutlierDetector OutlierDetectorViewModel
        {
            get { return GetValue<OutlierDetector>(OutlierDetectorViewModelProperty); }
            set { SetValue(OutlierDetectorViewModelProperty, value); }
        }

        public static readonly PropertyData OutlierDetectorViewModelProperty = RegisterProperty(nameof(OutlierDetectorViewModel), typeof(OutlierDetector), null);


        public DataGenerator DataGeneratorViewModel
        {
            get { return GetValue<DataGenerator>(DataGeneratorViewModelProperty); }
            set { SetValue(DataGeneratorViewModelProperty, value); }
        }

        public static readonly PropertyData DataGeneratorViewModelProperty = RegisterProperty(nameof(DataGeneratorViewModel), typeof(DataGenerator), null);

        // private System.Drawing.Graphics graphics { set; get; }


        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        public ICommand GenerateRawDataCommand { set; get; }

        private void GenerateRawData()
        {
            this.DataGeneratorViewModel.DataGeneration();
            for(int i = 0; i < DataGeneratorViewModel.RawData.GetLength(0); i++)
            {
                (RawDataPlotModel.Series[0] as OxyPlot.Series.LineSeries).Points.Add(new OxyPlot.DataPoint(x: DataGeneratorViewModel.RawData[i,0],
                                                                                                           y: DataGeneratorViewModel.RawData[i,1]));
            }
            this.RawDataPlotModel.InvalidatePlot(true);
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
           /* if(graphics == null)
            {
                graphics = new System.Drawing.
            }

            graphics.DrawLine(
                pen: new System.Drawing.Pen(System.Drawing.Color.Green),
                x1: 1,
                y1: 1,
                x2: 2,
                y2: 2);*/
            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }
    }
}