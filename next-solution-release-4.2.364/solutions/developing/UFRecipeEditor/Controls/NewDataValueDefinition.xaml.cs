using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Data;
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
using UFInterfaces.Editors;
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.Documents;
using UFRecipeSettings.UFRecipeModel;
using UFUAModel.Extensions;
using Utilities;
using Utilities.WPF;

namespace UFRecipeEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewDataValueDefinition.xaml
    /// </summary>
    public partial class NewDataValueDefinition : UserControl
    {
        #region Declarations
        readonly UFRecipeDocument Document;
        readonly RecipeEditorManagerComponent EditorComponent;
        #endregion

        public NewDataValueDefinition(UFRecipeDocument doc, RecipeEditorManagerComponent editorcomponent)
        {
            InitializeComponent();
            Document = doc;
            EditorComponent = editorcomponent;
            comboDataType.ItemsSource = Enum.GetValues(typeof(UFUAModel.DataType));
            Loaded += (o, e) =>
            {
                UFDataValueEntity a = DataContext as UFDataValueEntity;
                if (a != null)
                {
                    OPCUAEntityReferenceModel TagDataValue = new OPCUAEntityReferenceModel() { Value = a.TagDataValue };
                    textColumnTag.DataContext = TagDataValue;
                    TagDataValue.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagDataValue = TagDataValue.Value;
                        }
                    };
                    OPCUAEntityReferenceModel TagIODataValue = new OPCUAEntityReferenceModel() { Value = a.TagIODataValue };
                    textColumnTagIO.DataContext = TagIODataValue;
                    TagIODataValue.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagIODataValue = TagIODataValue.Value;
                        }
                    };
                }
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tag = DataContext as IDynamicSettingsEditing;
            if (EditorComponent.UfuaEditorService != null && tag != null)
            {
                var gruppo = (DataContext as UFRecipeSettings.UFRecipeModel.UFGroupEntity);
                if (gruppo != null)
                {
                    var writabledatavalues = (from c in gruppo.DataValues
                                              where c.UseInCommunication
                                              orderby c.OID ascending
                                              select c).ToList();
                    if (writabledatavalues.Count > 0)
                        gruppo.DataType = (int)writabledatavalues[0].DataType;
                }

                var dynamicSettings = EditorComponent.UfuaEditorService.GetDynamicSettingsControl(Document, DataContext);
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
            var stringEditor = EditorComponent.StringEditor.GetStringEditor(Document.Parent);
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
