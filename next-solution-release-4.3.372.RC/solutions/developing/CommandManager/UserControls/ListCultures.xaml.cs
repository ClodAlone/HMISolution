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

namespace CommandManager.UserControls
{
    /// <summary>
    /// Interaction logic for ListCultures.xaml
    /// </summary>
    public partial class ListCultures : UserControl
    {
        public ListCultures()
        {
            InitializeComponent();

            listbox.MouseDoubleClick += (o, e) =>
            {
                this.FindParent<Window>().DialogResult = true;
            };
        }
    }
}
