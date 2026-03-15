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
using UnitConverterManager.ComponentService;
using UnitConverterManager.Document;
using WPFUtilities.PropertyDataTemplate;

namespace UnitConverterManager.Controls
{
    /// <summary>
    /// Interaction logic for NewConverterItem.xaml
    /// </summary>
    public partial class NewConverter : UserControl
    {

        public NewConverter(string selected = null)
        {
            InitializeComponent();
            txtConverter.Text = selected;
        }

        public string GetSelected()
        {
            return txtConverter.Text;
        }
        public void SetSelected(string selected)
        {
            txtConverter.Text = selected;
        }
    }
}
