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
using OPCUAViewModel;

namespace OPCUABrowser
{
    /// <summary>
    /// Interaction logic for ConnectControl.xaml
    /// </summary>
    public partial class ConnectControl : UserControl
    {
        OPCUABrowserUI parent;
        String _appDescTitle;
        public String AppDescTitle 
        { 
            get
            {
                return _appDescTitle;
            }
            set 
            { 
                if (_appDescTitle == value)
                    return;
                _appDescTitle = value;
                appDescTitle.Text = _appDescTitle;
            } 
        }

        public ConnectControl(OPCUABrowserUI p)
        {
            parent = p;
            InitializeComponent();
        }

        private void Button_Browse(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            parent.Browse(DataContext as EndpointDescriptionViewModel);
        }

        private void Button_AreaBrowse(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            parent.BrowseArea(DataContext as EndpointDescriptionViewModel);
        }

        private void Button_Alarm(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            parent.ViewAlarms(DataContext as EndpointDescriptionViewModel);
        }

        private void Button_Audit(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            parent.ViewAuditEvents(DataContext as EndpointDescriptionViewModel);
        }
    }
}
