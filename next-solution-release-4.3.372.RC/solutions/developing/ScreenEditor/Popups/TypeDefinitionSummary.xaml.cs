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
using Utilities;
using Utilities.WPF;
using System.Windows.Controls.Primitives;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for TypeDefinitionSummary.xaml
    /// </summary>
    public partial class TypeDefinitionSummary : UserControl, IDisposable
    {
        public TypeDefinitionSummary(FrameworkElement fe)
        {
            InitializeComponent();

            name.Text = fe.Name;

            //gridDataControl.Model.Options.ListBoxSelectionMode = GridSelectionMode.MultiExtended;
            //gridDataControl.Model.Options.AllowSelection = GridSelectionFlags.Any & ~GridSelectionFlags.Cell & ~GridSelectionFlags.Column & ~GridSelectionFlags.Table;
            gridDataControl.MouseDoubleClick += gridDataControl_MouseDoubleClick;
        }

        void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridDataControl.SelectedItem != null)
            {
                //AnimationManager.AnimationManager am = gridDataControl.SelectedItem as AnimationManager.AnimationManager;
                //var userControl = am.Editor;
                //userControl.DataContext = am;
                //userControl.ClearValue(FrameworkElement.WidthProperty);
                //userControl.ClearValue(FrameworkElement.HeightProperty);
                //propertyControl.contentAnimation.Content = userControl;

                //Container._transContainer.control = propertyControl;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as GeneralDialog)?.Close();
        }

        public void Dispose()
        {
            // gridDataControl.Model.Dispose();
            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
