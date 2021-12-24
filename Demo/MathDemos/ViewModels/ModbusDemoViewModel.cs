using Catel.MVVM;
using System.Threading.Tasks;

using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using NModbus;
using NModbus.Serial;
using NModbus.Utility;
using System.Linq;
using NModbus.Logging;
using System.Windows.Input;

using NModbus.Extensions;
using NModbus.Message;
using NModbus.Device;
using Catel.Data;
using MathDemos.Models;

namespace MathDemos.ViewModels
{
    public class ModbusDemoViewModel : ViewModelBase
    {
        public ModbusDemoViewModel(/* dependency injection here */)
        {
            StartModbusSlaveCommand = new Command(StartModbusTcpSlave);
            StartModbusMasterCommand = new Command(StartModbusMaster);
        }

        public override string Title { get { return "Modbus Demo"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
     
        public ICommand StartModbusSlaveCommand { set; get; }
        public ICommand StartModbusMasterCommand { set; get; }

        public string ModbusSlaveDebugString
        {
            get { return GetValue<string>(ModbusSlaveDebugStringProperty); }
            set { SetValue(ModbusSlaveDebugStringProperty, value); }
        }

        public static readonly PropertyData ModbusSlaveDebugStringProperty = RegisterProperty(nameof(ModbusSlaveDebugString), typeof(string), () => "");

        public string ModbusMasterDebugString
        {
            get { return GetValue<string>(ModbusMasterDebugStringProperty); }
            set { SetValue(ModbusMasterDebugStringProperty, value); }
        }

        public static readonly PropertyData ModbusMasterDebugStringProperty = RegisterProperty(nameof(ModbusMasterDebugString), typeof(string), () => "");

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

        public void StartModbusMaster()
        {
            ModbusMasterDebugString = "Master started: \n";
        }

        // 4 bits => 0-F, 8 bits => 256 ASCII character.  // 1 register: 2 Byte=> 2 ASCII;
        // Test: 0100 0001, 0100 0010 => A, B => arg.points[0] = 16706 short.

        public void StartModbusTcpSlave() // static
        {
            ModbusSlaveDebugString = "Modbus started: \n";
            try
            {
                int port = 502;
                IPAddress address = new IPAddress(new byte[] {127, 0, 0, 1});  
                // TODO get EthernetIP or Ethernet2 Ip
                // 127, 0, 0, 1  // 192, 168, 0, 206

                // create and start the TCP slave
                TcpListener slaveTcpListener = new TcpListener(address, port);
                slaveTcpListener.Start();


                IModbusFactory factory = new ModbusFactory(functionServices: null,
                                                           includeBuiltIn: true, 
                                                           logger: new ConsoleModbusLogger(LoggingLevel.Debug));
                var dataStore = new SlaveStorage();
                // FC01
                dataStore.CoilDiscretes.StorageOperationOccurred += (sender, args) =>
                {
                    ModbusSlaveDebugString += $"Coil discretes: {args.Operation}  starting at { args.StartingAddress }" + 
                                              $"\n" + "Points: ";
                    foreach(bool _point in args.Points)
                    {
                        ModbusSlaveDebugString += _point.ToString() + ", ";
                    }
                    ModbusSlaveDebugString += "\n";
                };
                // FC02
                dataStore.CoilInputs.StorageOperationOccurred += (sender, args) =>
                {
                    ModbusSlaveDebugString += $"Coil inputs: {args.Operation} starting at {args.StartingAddress}" + 
                                              $"\n" + "Points: ";
                    foreach(bool _point in args.Points)
                    {
                        ModbusSlaveDebugString += _point.ToString() + ", ";
                    }
                    ModbusSlaveDebugString += "\n";
                };
                // FC04
                dataStore.InputRegisters.StorageOperationOccurred += (sender, args) =>
                {
                    ModbusSlaveDebugString += $"Input registers: {args.Operation} starting at {args.StartingAddress}" + "\n" + "Points: ";
                    foreach(ushort _point in args.Points)
                    {
                        ModbusSlaveDebugString += _point.ToString() + ", ";
                    }
                    ModbusSlaveDebugString += "\n";

                    char[] charsResult = handleModbusRegPointsToChars(args.Points);
                    foreach(char _c in charsResult)
                    {
                        ModbusSlaveDebugString += $"{_c}";
                    }

                    if (args.Points.Count() % 2 == 0)
                    {
                        float[] floatsResult = HandleModbusRegPointsToFloats(args.Points);
                        ModbusSlaveDebugString += "Converted floats: ";
                        foreach(float _singleFloat in floatsResult)
                        {
                            ModbusSlaveDebugString += $"{_singleFloat}, ";
                        }
                        ModbusSlaveDebugString += "\n";

                        // TEST convert back
                        ModbusSlaveDebugString += "Test ushorts: ";
                        foreach(Single _singleFloat in floatsResult)
                        {
                            ushort[] _ushorts = ConvertSingleFloatToUshorts(_singleFloat);
                            ModbusSlaveDebugString += $"{_ushorts[0]}, {_ushorts[1]} ";
                        }
                    }
                };
                // FC03
                dataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) =>
                {
                    ModbusSlaveDebugString += $"Holding registers: {args.Operation} starting at {args.StartingAddress}" + "\n" + "Points: ";
                    foreach (ushort _point in args.Points)
                    {
                        ModbusSlaveDebugString += _point.ToString() + ", ";
                    }
                    ModbusSlaveDebugString += "\n";

                    // Convert to char:
                    char[] charsResult = handleModbusRegPointsToChars(args.Points);
                    foreach (char _c in charsResult)
                    {
                        ModbusSlaveDebugString += $"{_c}";
                    }
                    // convert to float.
                    float[] floatsResult = HandleModbusRegPointsToFloats(args.Points);
                    ModbusSlaveDebugString += "Converted floats: ";
                    foreach (float _singleFloat in floatsResult)
                    {
                        ModbusSlaveDebugString += $"{_singleFloat}, ";
                    }
                    ModbusSlaveDebugString += "\n";

                    // TEST convert back
                    ModbusSlaveDebugString += "Test ushorts: ";
                    foreach (Single _singleFloat in floatsResult)
                    {
                        ushort[] _ushorts = ConvertSingleFloatToUshorts(_singleFloat);
                        ModbusSlaveDebugString += $"{_ushorts[0]}, {_ushorts[1]} ";
                    }
                };

                IModbusSlave slave1 = factory.CreateSlave(unitId: 1, dataStore: dataStore);
                IModbusSlave slave2 = factory.CreateSlave(unitId: 2, dataStore: null);

                IModbusSlaveNetwork network = factory.CreateSlaveNetwork(slaveTcpListener);
                network.AddSlave(slave1);
                network.AddSlave(slave2);

                Thread startModbusListen = new Thread(() =>
                { 
                    network.ListenAsync().GetAwaiter().GetResult();
                    // prevent the main thread from exiting
                    Thread.Sleep(Timeout.Infinite);
                });
                startModbusListen.Start();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error while starting slave: {ex.Message}");
            }
        }

        // Convert to char/ float/ ...
        private char[] handleModbusRegPointsToChars(ushort[] argsPoints)
        {
            char[] chars = new char[argsPoints.Length * 2];
            for(int i = 0; i < argsPoints.Length; i++)     // length == counts: the total number of elements. 
            {
                byte[] _byteArray = BitConverter.GetBytes(argsPoints[i]);
                for(int j = 0; j < _byteArray.Length; j++)
                {
                    var _char = System.Convert.ToChar(value: _byteArray[j]);
                    chars[i * 2 + j] = _char;
                    // ModbusSlaveDebugString += $"Char: {_char} \n";
                }
            }
            return chars;
        }

        private Single[] HandleModbusRegPointsToFloats(ushort[] argsPoints)
        {
            Single[] singleFloats = new Single[argsPoints.Length / 2];
            if (argsPoints.Length  % 2 == 0)
            {
                for (int i = 0; i < argsPoints.Length - 1; i+=2)
                {
                    Single singleFloat = ModbusUtility.GetSingle(lowOrderValue: argsPoints[i], highOrderValue: argsPoints[i + 1]);
                    singleFloats[i / 2] = singleFloat;
                } 
            }
            return singleFloats;
        }

        // Convert back to ushorts for Modbus transfer data.
        private ushort[] ConvertSingleFloatToUshorts(Single _singleFloat)
        {
            byte[] bytes = BitConverter.GetBytes(_singleFloat);
           
            ushort lowerOrderUshort = BitConverter.ToUInt16(value: bytes, startIndex: 0);
            ushort upperOrderUshort = BitConverter.ToUInt16(value: bytes, startIndex: 2);
            ushort[] ushorts = { lowerOrderUshort, upperOrderUshort };
            return ushorts;
        }
    }
}
