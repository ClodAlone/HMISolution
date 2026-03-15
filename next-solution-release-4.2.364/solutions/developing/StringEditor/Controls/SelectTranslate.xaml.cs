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

namespace StringManager.Controls
{
    /// <summary>
    /// Interaction logic for SelectTranslate.xaml
    /// </summary>
    public partial class SelectTranslate : UserControl
    {
        public SelectTranslate(List<String> list)
        {
            InitializeComponent();

            lstboxFrom.ItemsSource = list;
            lstboxTo.ItemsSource = list;
        }

        public String SelectedFrom
        {
            get
            {
                return lstboxFrom.SelectedValue as String;
            }
        }

        public List<String> SelectedTo
        {
            get
            {
                var ret = new List<String>();
                foreach (var item in lstboxTo.SelectedItems)
                    ret.Add(item as String);
                return ret;
            }
        }
    }
}
