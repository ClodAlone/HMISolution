using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ScreenManager.Controls
{
    /// <summary>
    /// Interaction logic for CulturesSelector.xaml
    /// </summary>
    public partial class CulturesSelector : UserControl
    {
        public CulturesSelector(List<string> cultures)
        {
            InitializeComponent();
            selector.ItemsSource = cultures;
        }
        public List<string> GetSelectedCultures()
        {
            List<string> list = new List<string>();
            if(selector.SelectedItems.Count > 0)
                foreach(var item in selector.SelectedItems)
                {
                    list.Add(item as string);
                }
            return list;
        }
        bool bDobuleClick;
        private void GridControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            bDobuleClick = true;
        }

        private void GridControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            if (!bDobuleClick)
                return;
            bDobuleClick = false;

            e.Handled = true;

            if(selector.SelectedItems != null)
            {
                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    e.Handled = true;
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            }
        }
    }
}
