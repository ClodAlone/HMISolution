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
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.Documents;
using Utilities;
using Utilities.WPF;
using UFRecipeSettings.UFRecipeModel;
using UFInterfaces.Editors;

namespace UFRecipeEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewGroupDefinition.xaml
    /// </summary>
    public partial class NewGroupDefinition : UserControl
    {
        #region Declarations
        readonly UFRecipeDocument Document;
        readonly RecipeEditorManagerComponent EditorManager;
        #endregion

        public NewGroupDefinition(UFRecipeDocument doc, RecipeEditorManagerComponent editorManager)
        {
            InitializeComponent();
            Document = doc;
            EditorManager = editorManager;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tag = DataContext as IDynamicSettingsEditing;
            if (EditorManager.UfuaEditorService != null && tag != null)
            {
                /* The logic above has been moved to recipe model.
                var gruppo = (DataContext as UFRecipeSettings.UFRecipeModel.UFGroupEntity);
                if (gruppo != null )
                {
                    var writabledatavalues = (from c in gruppo.DataValues
                                          where c.UseInCommunication
                                          orderby c.OID ascending
                                          select c).ToList();
                    if(writabledatavalues.Count > 0)
                        gruppo.DataType = (int) writabledatavalues[0].DataType;
                }
                */

                var dynamicSettings = EditorManager.UfuaEditorService.GetDynamicSettingsControl(Document, DataContext);
                if (dynamicSettings == null)
                    return;

                GeneralDialogContent Dialog = new GeneralDialogContent(dynamicSettings)
                {
                    Title = Properties.Resources.NewRecipeDefinitionDynamicEditorTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "DynamicSettingEditor"
                };

                var oldDynSettings = tag.DynamicSettingsForEditing;
                var ret = (Dialog.ShowDialog() == true);
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (!ret)
                        tag.DynamicSettingsForEditing = oldDynSettings;

                    if (dynamicSettings is IDisposable)
                        (dynamicSettings as IDisposable).Dispose();
                });
            }
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            var tag = DataContext as IDynamicSettingsEditing;
            if (tag != null)
                tag.DynamicSettingsForEditing = String.Empty;
        }

        private void textEditDesc_Click(object sender, RoutedEventArgs e)
        {
            var stringEditor = EditorManager.StringEditor.GetStringEditor(Document.Parent);
            stringEditor.DataContext = textEditDesc.Text;

            var Dialog = new GeneralDialogContent(stringEditor)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.SelectStringEditor,
                HelpLink = "StringEditor"
            };
            if (Dialog.ShowDialog() != true)
                return;

            textEditDesc.Text = stringEditor.DataContext as String;
            textEditDesc.Focus();
            textEditDesc.SelectAll();
        }

        private void textEditDesc_Clear(object sender, RoutedEventArgs e)
        {
            textEditDesc.Text = String.Empty;
        }
    }
}
