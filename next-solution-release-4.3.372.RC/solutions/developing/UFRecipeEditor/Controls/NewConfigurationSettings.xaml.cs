using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities;
using Utilities.WPF;
using UFRecipeEditor.ComponentService;

namespace UFRecipeEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewConfigurationSettings.xaml
    /// </summary>
    public partial class NewConfigurationSettings : UserControl
    {
        public NewConfigurationSettings()
        {
            InitializeComponent();

            Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;
            textEditMaxAge.Mask = String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart);
        }

        private void textEditEventConnection_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            var doc = RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextDocument;
            string connection = XpoHelpers.XpoHelper.NormalizeConnectionString(textEditEventConnection.Text, doc?.rootBase);
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connection, "ConnectionWizard", this.FindParent<Window>(), RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface, RecipeEditorManagerComponent.recipeEditorManagerComponent.HelpProvider))
            {
                using (var cursor = new WaitCursor())
                {
                    var settings = DataContext as UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration;
                    settings.EventDefaultConnection = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(connection, doc?.rootBase);
                }
            }
        }

        private void textEditEventConnection_DlgButton_Clear(object sender, RoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                textEditEventConnection.Text = string.Empty;
                var settings = DataContext as UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration;
                settings.EventDefaultConnection = string.Empty;
            }
        }
    }
}
