using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Utilities;
using System.ComponentModel;
using Opc.Ua;
using System.Threading;
using ViewModelLib;

namespace OPCUAViewModel.UserControls
{
    /// <summary>
    /// Interaction logic for MonitoredItemViewModel.xaml
    /// </summary>
    public partial class MonitoredItemStatusViewModel : UserControl
    {
        public MonitoredItemStatusViewModel()
        {
            InitializeComponent();
        }
    }
}
