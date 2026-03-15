using DevExpress.Xpf.Core;
using MSZUtilsServiceHelper;
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

namespace MSZUtils.Controls
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class NewCustomer : DXWindow
    {
        List<AreaGeoID> areaGeoId;
        public NewCustomer(Customer customer, List<AreaGeoID> areaGeoIds)
        {
            InitializeComponent();
            DataContext = customer;
            areaGeoId = areaGeoIds;
            List<string> list = (from a in areaGeoId select a.Area).ToList();
            areaCombo.ItemsSource = list;
        }
        #region Methods
        public void ShowDetailsList()
        {
            ShowDialog();
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
