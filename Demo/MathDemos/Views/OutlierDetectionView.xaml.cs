using MathDemos.ViewModels;
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Catel;
using Catel.IoC;
using Catel.Windows.Controls;

namespace MathDemos.Views
{
    public partial class OutlierDetectionView : Catel.Windows.Controls.UserControl // , System.Windows.Forms.Form
    {
        public OutlierDetectionView()
        {
            InitializeComponent();
            // this.GenerateRawDataButton. TODO: how to set anchor, or docking property...
        }
    }
}
