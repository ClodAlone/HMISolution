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
    /// Interaction logic for BitMaskPropertyEditor.xaml
    /// </summary>
    public partial class FontSettingsPropertyEditor : UserControl
    {
        public FontSettingsPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var fontsettings = button.Tag as FontSettings;
            if (fontsettings != null)
            {
                FontSettingsEditor fonteditor = new FontSettingsEditor(fontsettings);
                GeneralDialogContent Dialog = new GeneralDialogContent(fonteditor)
                {
                    Title = Properties.Resources.FontEditor,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "FontEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    button.Tag = new FontSettings(fonteditor.UIFontSettings); 
                } 
            }
        }
    }
}
