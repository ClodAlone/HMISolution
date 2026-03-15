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

namespace PropertyControl.Localization.Helpers
{
    /// <summary>
    /// Interaction logic for InvariantStrings.xaml
    /// </summary>
    public partial class InvariantStrings : UserControl
    {
        public InvariantStrings()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender,
        System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }
    }
}
