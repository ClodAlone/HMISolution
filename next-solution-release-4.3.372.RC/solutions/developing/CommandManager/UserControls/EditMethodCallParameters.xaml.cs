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
using Opc.Ua;
using OPCUAViewModel;

namespace CommandManager.UserControls
{
    /// <summary>
    /// Interaction logic for EditMethodCallParameters.xaml
    /// </summary>
    public partial class EditMethodCallParameters : UserControl
    {
        bool bLoaded;

        public EditMethodCallParameters()
        {
            InitializeComponent();

            Loaded += (ob, ev) =>
                {
                    if (bLoaded)
                        return;
                    bLoaded = true;
                };
        }
    }
}
