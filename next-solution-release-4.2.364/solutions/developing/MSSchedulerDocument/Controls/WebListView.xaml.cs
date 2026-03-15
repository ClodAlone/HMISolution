using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSSchedulerSettings.Controls
{
    /// <summary>
    /// Interaction logic for WebListView.xaml
    /// </summary>
    public partial class WebListView : ListView
    {
        private AutomationPeer peer;

        public WebListView()
        {
            InitializeComponent();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            peer = base.OnCreateAutomationPeer();
            return peer;
        }

        public void ResetPeerCache()
        {
            UpdateLayout();
            peer?.ResetChildrenCache();
            //Items.Refresh();
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            ResetPeerCache();
        }
    }
}
