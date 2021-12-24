using Catel.MVVM;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MathDemos.Models;
using MathDemos.Services;
using System.Collections.ObjectModel;
using Catel;
using Catel.Data;
using Catel.Services;
using Catel.IoC;
using Catel.Messaging;

namespace MathDemos.ViewModels
{
    public class AddOutlierDetectViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        public AddOutlierDetectViewModel(ObservableCollection<OutlierDetector> outlierDetectors, IMessageService messageService)
        {
            this.OutlierDetectors = outlierDetectors;
            this._messageService = messageService;
            AddOutlierDetectorCommand = new Command(AddOutlierDetector);
        }

        public override string Title { get { return "View model title"; } }

        // TODO: Register models with the vmpropmodel codesnippet

        public ObservableCollection<OutlierDetector> OutlierDetectors
        {
            get { return GetValue<ObservableCollection<OutlierDetector>>(OutlierDetectorsProperty); }
            set { SetValue(OutlierDetectorsProperty, value); }
        }

        public static readonly PropertyData OutlierDetectorsProperty = RegisterProperty(nameof(OutlierDetectors), typeof(ObservableCollection<OutlierDetector>), () => new ObservableCollection<OutlierDetector>());
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        public ICommand AddOutlierDetectorCommand { set; get; }

        private void AddOutlierDetector()
        {
            if(OutlierDetectors.Count == 0)
            {
                var mediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
                mediator.SendMessage<string>(this.OutlierDetectors.Count.ToString(), "OutlierDetectorAdded");
            }
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
