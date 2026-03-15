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

namespace OpcClientDriver.UI.SettingsControls
{
    /// <summary>
    /// Interaction logic for ItemName.xaml
    /// </summary>
    public partial class ItemName : UserControl
    {
        OPCUAEntityReference item;
        public ItemName()
        {
            InitializeComponent();
        }
        #region Commands

        private void ResetItem_Click(object sender, RoutedEventArgs e)
        {
            var baseDyn = DataContext as OpcClientDriverDynTagSettings;
            baseDyn.ItemName = null;
            baseDyn.AppName = null;
            baseDyn.RelativePath = null;
            baseDyn.EndpointUrl = null;
            baseDyn.ResolvedNodeId = null;
            DataContext = null;
            DataContext = baseDyn;
        }

        private void btnItem_Click(object sender, RoutedEventArgs e)
        {
            if (item == null)
                item = new OPCUAEntityReference(null);

            if (item.Edit(sync: true, noDataSinks: true, noLocalServer: true) == true)
            {
                var a = DataContext as OpcClientDriverDynTagSettings;
                if (a != null)
                {
                    a.ItemName = item.HumanReadable;
                    a.AppName = item.AppName;
                    a.RelativePath = item.RelativePath;
                    a.EndpointUrl = item.EndpointUrl;
                    a.ResolvedNodeId = item.ResolvedNodeId;

                    DataContext = null;
                    DataContext = a;
                }
            }
        }
        #endregion
    }
}
