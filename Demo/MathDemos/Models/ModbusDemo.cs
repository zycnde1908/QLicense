using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel;
using Catel.Data;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NModbus;
using NModbus.Serial;
using NModbus.Utility;

namespace MathDemos.Models
{
    using System.Linq;
    using System.Runtime.CompilerServices;
    using NModbus.Logging;


    public class ModbusDemo : ModelBase
    {
        private const string PrimarySerialPortName = "COM12";
        private const string SecondarySerialPortName = "COM2";


        public ModbusDemo()
        {
            this.Active = true;
        }
        public bool Active { set; get; }
        private class HmiBufferRequestmessage : IModbusMessage
        {
            public byte FunctionCode { get; set; }

            public byte SlaveAddress { get; set; }

            public byte[] MessageFrame { get; private set; }

            public byte[] ProtocolDataUnit { get; private set; }

            public ushort TransactionId { get; set; }

            public void Initialize(byte[] frame)
            {
                SlaveAddress = frame[0];
                FunctionCode = frame[1];

                MessageFrame = frame
                    .Take(frame.Length - 2)
                    .ToArray();

                ProtocolDataUnit = frame
                    .Skip(1)
                    .ToArray();
            }
        }

        private class HmiBufferResponseMessage : IModbusMessage
        {
            public byte FunctionCode { get; set; }

            public byte SlaveAddress { get; set; }

            public byte[] MessageFrame { get; private set; }

            public byte[] ProtocolDataUnit { get; private set; }

            public ushort TransactionId { get; set; }

            public void Initialize(byte[] frame)
            {
                SlaveAddress = frame[0];
                FunctionCode = frame[1];

                MessageFrame = frame
                    .Take(frame.Length - 2)
                    .ToArray();

                ProtocolDataUnit = frame
                    .Skip(1)
                    .ToArray();
            }
        }

        private class HmiBufferFunctionService : IModbusFunctionService
        {
            public byte FunctionCode => 45;

            public IModbusMessage CreateRequest(byte[] frame)
            {
                Console.WriteLine($"HMI Buffer Message Receieved - {frame.Length} bytes");

                var request = new HmiBufferRequestmessage();

                request.Initialize(frame);

                return request;
            }

            public IModbusMessage HandleSlaveRequest(IModbusMessage request, ISlaveDataStore dataStore)
            {
                Console.WriteLine("HMI Buffer Message Receieved");

                throw new NotImplementedException();
            }

            public int GetRtuRequestBytesToRead(byte[] frameStart)
            {
                byte registerCountMSB = frameStart[4];
                byte registerCountLSB = frameStart[5];

                int numberOfRegisters = (registerCountMSB << 8) + registerCountLSB;

                Console.WriteLine($"Got Hmi Buffer Request for {numberOfRegisters} registers.");

                return (numberOfRegisters * 2) + 1;
            }

            public int GetRtuResponseBytesToRead(byte[] frameStart)
            {
                return 4;
            }
        }
        public void StartModbusTcpSlaveDemo()
        {
            StartModbusTcpSlave();
        }

        /// <summary>
        ///     Simple Modbus TCP slave example.
        /// </summary>
        public static void StartModbusTcpSlave()
        {
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);
            slaveTcpListener.Start();


            var functionServices = new IModbusFunctionService[]
                {
                    new HmiBufferFunctionService()
                };

            IModbusFactory factory = new ModbusFactory(functionServices, true, new ConsoleModbusLogger(LoggingLevel.Debug));

            var acTechDataStore = new SlaveStorage();
            acTechDataStore.InputRegisters.StorageOperationOccurred += (sender, args) => System.Diagnostics.Debug.WriteLine($"ACTECH Input registers: {args.Operation} starting at {args.StartingAddress}");
            acTechDataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => System.Diagnostics.Debug.WriteLine($"ACTECH Holding registers: {args.Operation} starting at {args.StartingAddress}");

            var danfossStore = new SlaveStorage();
            danfossStore.InputRegisters.StorageOperationOccurred += (sender, args) => System.Diagnostics.Debug.WriteLine($"DANFOSS Input registers: {args.Operation} starting at {args.StartingAddress}");
            danfossStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => System.Diagnostics.Debug.WriteLine($"DANFOSS Holding registers: {args.Operation} starting at {args.StartingAddress}");

            IModbusSlaveNetwork network = factory.CreateSlaveNetwork(slaveTcpListener);

            IModbusSlave slave1 = factory.CreateSlave(1, acTechDataStore);
            IModbusSlave slave2 = factory.CreateSlave(2, danfossStore);

            network.AddSlave(slave1);
            network.AddSlave(slave2);

            network.ListenAsync().GetAwaiter().GetResult();

            // prevent the main thread from exiting
            Thread.Sleep(Timeout.Infinite);
        }
    }
}