using Catel;
using Catel.IoC;
using Catel.Windows.Controls;


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
using MathDemos.Models;

namespace MathDemos.Views
{
    public partial class ModbusDemoView : Catel.Windows.Controls.UserControl
    {
        public ModbusDemoView()
        {
            InitializeComponent();
        }
    }
}
