using Catel.MVVM;
using System.Threading.Tasks;
using System.Windows.Input;
using MathDemos.Models;
using Catel.Data;
using Catel.Messaging;
using Catel.IoC;
using Catel.Services;
using System.Collections.ObjectModel;

namespace MathDemos.ViewModels
{
    public class AddModbusViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        public AddModbusViewModel(ObservableCollection<ModbusDemo> modbusDemos, IMessageService messageService)
        {
            this.ModbusDemos = modbusDemos;
            this._messageService = messageService;
            AddModbusDemoCommand = new Command(AddModbus);
        }

        public override string Title { get { return "View model title"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets


        public ObservableCollection<ModbusDemo> ModbusDemos
        {
            get { return GetValue<ObservableCollection<ModbusDemo>>(ModbusDemosProperty); }
            set { SetValue(ModbusDemosProperty, value); }
        }

        public static readonly PropertyData ModbusDemosProperty = RegisterProperty(nameof(ModbusDemos), typeof(ObservableCollection<ModbusDemo>), () => new ObservableCollection<ModbusDemo>());
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        public ICommand AddModbusDemoCommand { set; get; }

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

        private void AddModbus()
        {
            if(ModbusDemos.Count == 0)
            {
                var mediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
                mediator.SendMessage<string>(this.ModbusDemos.Count.ToString(), "ModbusDemoAdded");

            }
        }
    }
}
