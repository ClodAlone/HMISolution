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

namespace UnitConverterManager.Controls
{
    /// <summary>
    /// Interaction logic for CultureSelection.xaml
    /// </summary>
    public partial class ConverterSelection : UserControl
    {
        public ConverterSelection()
        {
            InitializeComponent();

            listbox.MouseDoubleClick += (o, e) =>
            {
                this.FindParent<Window>().DialogResult = true;
            };
        }
    }
}
