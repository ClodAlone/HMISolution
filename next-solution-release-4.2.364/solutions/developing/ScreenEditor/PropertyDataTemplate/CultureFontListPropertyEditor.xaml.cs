using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Converters;
using PropertyControl.ComponentService;
using ScreenManager.ComponentService;
using ScreenSettings;
using ScreenSettings.Entities;
using StringManager.ComponentService;
using UFInterfaces;
using UFProjectManager.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities;

namespace ScreenManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for StartupLogicsPropertyEditor.xaml
    /// </summary>
    public partial class CultureFontListPropertyEditor : UserControl
    {
        ScreenDocument doc;
        IStringEditorManager stringEditor;
        List<string> cultures = new List<string>();
        Dictionary<string,FontSettings> mapSettings = new Dictionary<string, FontSettings>();
        public CultureFontListPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (sender as Button);
            Dictionary<string, FontSettings> listSettings = button.Tag as Dictionary<string, FontSettings>;
            if (listSettings == null)
                listSettings = new Dictionary<string, FontSettings>();
            doc = ScreenManagerComponent.screenManagerComponent.Workspace.ContextDocument as ScreenDocument;
            stringEditor = doc?.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            if (doc != null && stringEditor != null)
            {
                var control = new ListFontEditor(listSettings);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.CultureFontSettingListDialogTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "CultureFontSettingList"
                };
                if (Dialog.ShowDialog() == true)
                {
                    (sender as Button).Tag = new Dictionary<string, FontSettings>(control.CurrentList);
                    uriLabel.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriCommand.Tag = null;
            uriLabel.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
