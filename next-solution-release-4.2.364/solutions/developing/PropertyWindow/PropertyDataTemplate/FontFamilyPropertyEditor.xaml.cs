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
using Utilities;

namespace PropertyControl.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for IntegerUpDownPropertyEditor.xaml
    /// </summary>
    public partial class FontFamilyPropertyEditor : UserControl
    {
        public FontFamilyPropertyEditor()
        {
            InitializeComponent();
        }

        bool bInit;
        private void combo_DropDownOpened(object sender, EventArgs e)
        {
            try 
	        {	   
                if(!bInit)
                {
                    if((sender as ComboBox).Items.Count == 0)
                    {
                        FontStyleHelper.InitializeFontComboBox((sender as ComboBox));
                        bInit = true;
                    }
                }
	        }
	        catch (Exception)
	        {
	        }
        }
    }
}
