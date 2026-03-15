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
using ScreenParametersEditor.ComponentService;
using ScreenParameterSettings;
using System.Windows.Threading;
using Utilities;

namespace ScreenParametersEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ParameterItemPropertyEditor.xaml
    /// </summary>
    public partial class ParameterItemPropertyEditor : UserControl
    {
        #region Constructors
        public ParameterItemPropertyEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Commands
        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            using (var item = new OPCUAEntityReference(null))
            {
                if (item.Edit(sync: true, noDataSinks: true, localserver: true) && item.RelativePath != null)
                    text.Text = item.RelativePath;
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            text.Text = null;
        }
        #endregion
    }
}
