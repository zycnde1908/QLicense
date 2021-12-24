using Catel;
using Catel.MVVM;
using System.Threading.Tasks;
using Catel.Services;
using System.Windows.Input;
using Catel.IoC;
using MathDemos.Models;
using MathDemos.Views;
using Catel.Data;
using Catel.Messaging;
using System.Collections.ObjectModel;

namespace MathDemos.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICommandManager _commandManager;
        private readonly IUIVisualizerService _uiVisualizerService;
        public MainViewModel(ICommandManager commandManager,
                             IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => commandManager);
            this._commandManager = commandManager;

            Argument.IsNotNull(() => uiVisualizerService);
            this._uiVisualizerService = uiVisualizerService;

            commandManager.CreateCommand("AddOutlierDetect");
            AddOutlierDetectWindowCommand = new TaskCommand(OpenOutlierDetect);
            commandManager.RegisterCommand("AddOutlierDetect", AddOutlierDetectWindowCommand, this);

            var mediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
            mediator.Register<string>(this, AddOutlierDetectorInMainView, "OutlierDetectorAdded");


            commandManager.CreateCommand("AddModbusDemoWindow");
            AddModbusDemoWindowCommand = new TaskCommand(OpenModbusDemo);
            commandManager.RegisterCommand("AddModbusDemoWindow", AddModbusDemoWindowCommand, this);

            // var mediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
            mediator.Register<string>(this, AddModbusDemoInMainView, "ModbusDemoAdded");
        }


        public ICommand AddOutlierDetectWindowCommand { set; get; }
        public ICommand AddModbusDemoWindowCommand { set; get; }
        public override string Title { get { return "Welcome to MathDemos"; } }


        // TODO: Register models with the vmpropmodel codesnippet



        public ObservableCollection<OutlierDetector> OutlierDetectors
        {
            get { return GetValue<ObservableCollection<OutlierDetector>>(OutlierDetectorsProperty); }
            set { SetValue(OutlierDetectorsProperty, value); }
        }

        public static readonly PropertyData OutlierDetectorsProperty = RegisterProperty(nameof(OutlierDetectors), typeof(ObservableCollection<OutlierDetector>), () => new ObservableCollection<OutlierDetector>());


        public ObservableCollection<ModbusDemo> ModbusDemos
        {
            get { return GetValue<ObservableCollection<ModbusDemo>>(ModbusDemosProperty); }
            set { SetValue(ModbusDemosProperty, value); }
        }

        public static readonly PropertyData ModbusDemosProperty = RegisterProperty(nameof(ModbusDemos), typeof(ObservableCollection<ModbusDemo>), () => new ObservableCollection<ModbusDemo>());

        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        public async Task OpenOutlierDetect()
        {
            var typeFactory = this.GetTypeFactory();
            var openOutlierDetectViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<AddOutlierDetectViewModel>(OutlierDetectors);
            await this._uiVisualizerService.ShowDialogAsync(openOutlierDetectViewModel);
        }

        public async Task OpenModbusDemo()
        {
            var typeFactory = this.GetTypeFactory();
            var openModbusDemoViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<AddModbusViewModel>(ModbusDemos);
            await this._uiVisualizerService.ShowDialogAsync(openModbusDemoViewModel);
        }

        private async void AddOutlierDetectorInMainView(string obj)
        {
            OutlierDetectors.Add(new OutlierDetector(varianceDecayTime: 3,
                                                     meanDecayTime: 3,
                                                     outlierCutoff: 1,
                                                     deltaT: 1));
        }

        private async void AddModbusDemoInMainView(string obj)
        {
            ModbusDemos.Add(new ModbusDemo());
        }
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }
    }
}