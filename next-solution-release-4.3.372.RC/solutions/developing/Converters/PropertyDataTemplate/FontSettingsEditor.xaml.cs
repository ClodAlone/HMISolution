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
using DocumentManager.ComponentService;
using UFInterfaces;
using UFUserEditor.ComponentService;
using Utilities;
using Utilities.WPF;

namespace Converters.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for FontSettingsEditor.xaml
    /// </summary>
    public partial class FontSettingsEditor : UserControl
    {

#region Declaration
        public FontSettings UIFontSettings;
#endregion

#region Constructors
        public FontSettingsEditor()
        {
            InitializeComponent();

            FontStyleHelper.InitializeFontComboBox(fontNameBox);
            FontStyleHelper.InitializeFontSizeComboBox(fontSizeBox);
            FontStyleHelper.InitializeFontStyleComboBox(fontStyleBox);
            FontStyleHelper.InitializeFontWeightComboBox(fontWeightBox);

            UIFontSettings = new FontSettings();
            DataContext = UIFontSettings;
        }

        public FontSettingsEditor(FontSettings fontsettings)
        {
            InitializeComponent();
            FontStyleHelper.InitializeFontComboBox(fontNameBox);
            FontStyleHelper.InitializeFontSizeComboBox(fontSizeBox);
            FontStyleHelper.InitializeFontStyleComboBox(fontStyleBox);
            FontStyleHelper.InitializeFontWeightComboBox(fontWeightBox);

            UIFontSettings = new FontSettings(fontsettings); 
            DataContext = UIFontSettings;
        }
#endregion
    }
}
