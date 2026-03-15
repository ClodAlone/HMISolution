using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBaseEx.Enumerators;
using EtherNetIP;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using System.ComponentModel;

namespace EtherNetIP.UI.SettingsControls
{
    /// <summary>
    /// Interaction logic for AddressType.xaml
    /// </summary>
    public partial class AddressType : UserControl
    {
        public AddressType()
        {
            InitializeComponent();
        }
    }
}
