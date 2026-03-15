using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OPCUAViewModel;
using System.Windows.Threading;
using Utilities;
using ScreenParameterSettings;
using WPFUtilities;

namespace ScreenParametersEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewParameter.xaml
    /// </summary>
    public partial class NewParameter : UserControl
    {
        public NewParameter(ParameterItem par)
        {
            InitializeComponent();
            ID_editor.DataContext = text_editor.DataContext = par;
        }
    }
}
