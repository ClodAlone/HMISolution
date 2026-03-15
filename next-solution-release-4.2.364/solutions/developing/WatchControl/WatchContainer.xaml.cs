using DevExpress.Xpf.Core;
using DocumentManager.ComponentService;
using OPCUAViewModel;
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
using Utilities.WPF;

namespace WatchControl
{
    /// <summary>
    /// Interaction logic for WatchContainer.xaml
    /// </summary>
    public partial class WatchContainer : UserControl, IDisposable
    {
        public WatchContainer(IDocument parent)
        {
            InitializeComponent();
            tabControl.SelectionChanging += (o, e) =>
            {
                var item = e.NewSelectedItem as DXTabItem;
                if (item != null && item.Content == null)
                {
                    if (item.Tag != null)
                        item.Content = new WatchControl(parent, item.Tag as String, item.Tag as String);
                    else
                        item.Content = new WatchControl(parent, item.Header as String);
                }
            };
            AddTabWatch(parent, Properties.Resources.WatchTitle1);
            AddTabWatch(parent, Properties.Resources.WatchTitle2);
            AddTabWatch(parent, Properties.Resources.WatchTitle3);
            AddTabWatch(parent, Properties.Resources.WatchTitle4);

            OPCUAEntityReference.GetDataSinkInterfaces().ForEach(datasink =>
            {
                if (OPCUAEntityReference.GetDataSinkInterface(datasink).IsProjectTypeAware(parent.ProjectType))
                    AddTabWatch(parent, OPCUAEntityReference.GetDataSinkInterface(datasink).HumanReadableName, datasink);
            });
        }

        void AddTabWatch(IDocument parent, String title, String datasink = null)
        {
            var tabItem = new DXTabItem() { Header = title, Tag = datasink};
            tabItem.InitItemTemplate();
            tabControl.Items.Add(tabItem);
        }

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            var disposable = (from c in tabControl.Items.OfType<DXTabItem>() where c.Content is IDisposable select c.Content as IDisposable).ToList();
            disposable.ForEach(c => c.Dispose());

            tabControl.Items.Clear();
        }
    }
}
