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
using CommonControls;
using Utilities;
using Utilities.WPF;
using Microsoft.Data.ConnectionUI;
using UFRecipeEditor.ComponentService;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using UFInterfaces.Editors;
using UFRecipeSettings.Documents;
using UFRecipeSettings.UFRecipeModel;
using OPCUAViewModel;

namespace UFRecipeEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewRecipeDefinition.xaml
    /// </summary>
    public partial class NewRecipeDefinition : UserControl
    {
        #region Declarations
        readonly UFRecipeDocument Document;
        readonly RecipeEditorManagerComponent EditorManager;
        #endregion

        public NewRecipeDefinition(UFRecipeDocument doc, RecipeEditorManagerComponent editorManager)
        {
            InitializeComponent();
            Document = doc;
            EditorManager = editorManager;

            Loaded += (o, e) =>
            {
                UFRecipeEntity a = DataContext as UFRecipeEntity;
                if (a != null)
                {
                    OPCUAEntityReferenceModel TagRecipeList = new OPCUAEntityReferenceModel() { Value = a.TagRecipeList };
                    OPCUAEntityReferenceModel TagRecipeIndex = new OPCUAEntityReferenceModel() { Value = a.TagRecipeIndex };
                    OPCUAEntityReferenceModel TagRecipeState = new OPCUAEntityReferenceModel() { Value = a.TagRecipeState };
                    OPCUAEntityReferenceModel TagRecipeLoad = new OPCUAEntityReferenceModel() { Value = a.TagRecipeLoad };
                    OPCUAEntityReferenceModel TagRecipeSave = new OPCUAEntityReferenceModel() { Value = a.TagRecipeSave };
                    OPCUAEntityReferenceModel TagRecipeDelete = new OPCUAEntityReferenceModel() { Value = a.TagRecipeDelete };
                    OPCUAEntityReferenceModel TagRecipeWrite = new OPCUAEntityReferenceModel() { Value = a.TagRecipeWrite };
                    OPCUAEntityReferenceModel TagRecipeRead = new OPCUAEntityReferenceModel() { Value = a.TagRecipeRead };

                    textColumnTagRecipeList.DataContext = TagRecipeList;
                    textColumnTagRecipeIndex.DataContext = TagRecipeIndex;
                    textColumnTagRecipeState.DataContext = TagRecipeState;
                    textColumnTagRecipeLoad.DataContext = TagRecipeLoad;
                    textColumnTagRecipeSave.DataContext = TagRecipeSave;
                    textColumnTagRecipeDelete.DataContext = TagRecipeDelete;
                    textColumnTagRecipeWrite.DataContext = TagRecipeWrite;
                    textColumnTagRecipeRead.DataContext = TagRecipeRead;

                    TagRecipeList.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagRecipeList = TagRecipeList.Value;
                        }
                    };
                    TagRecipeIndex.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagRecipeIndex = TagRecipeIndex.Value;
                        }
                    };

                    TagRecipeState.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagRecipeState = TagRecipeState.Value;
                        }
                    };

                    TagRecipeLoad.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagRecipeLoad = TagRecipeLoad.Value;
                        }
                    };

                    TagRecipeSave.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagRecipeSave = TagRecipeSave.Value;
                        }
                    };

                    TagRecipeDelete.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagRecipeDelete = TagRecipeDelete.Value;
                        }
                    };

                    TagRecipeWrite.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagRecipeWrite = TagRecipeWrite.Value;
                        }
                    };

                    TagRecipeRead.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.TagRecipeRead = TagRecipeRead.Value;
                        }
                    };


                }

            };
        }

        private void textEditConnection_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            var recipe = DataContext as UFRecipeEntity;
            var dataReaderModel = new DataReader.DataReaderModel()
            {
                Connection = recipe.ConnectionString,
                DataProvider = recipe.DataProvider,
                DataProviderDisplayName = recipe.DataProviderDisplayName,
                DataProviderDescription = recipe.DataProviderDescription,
                DataProviderShortDisplayName = recipe.DataProviderShortDisplayName,
                DataSourceName = recipe.DataSourceName,
                DataSourceDisplayName = recipe.DataSourceDisplayName
            };

            var viewModel = new DataReaderEditor.DataReaderModelView(dataReaderModel, recipe.Document?.rootBase);
            viewModel.EditConnection.Execute(null);

            recipe.ConnectionString = dataReaderModel.Connection;
            recipe.DataProvider = dataReaderModel.DataProvider;
            recipe.DataProviderDisplayName = dataReaderModel.DataProviderDisplayName;
            recipe.DataProviderDescription = dataReaderModel.DataProviderDescription;
            recipe.DataProviderShortDisplayName = dataReaderModel.DataProviderShortDisplayName;
            recipe.DataSourceName = dataReaderModel.DataSourceName;
            recipe.DataSourceDisplayName = dataReaderModel.DataSourceDisplayName;

            textEditConnection.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

            /*
            var wizard = new ConnectionWizard()
            {
                ConnectionString = textEditConnection.Text
            };
            GeneralDialogContent Dialog = new GeneralDialogContent(wizard)
            {
                Owner = this.FindParent<Window>()
            };
            if (Dialog.ShowDialog() == true)
            {
                var settings = DataContext as UFRecipeModel.UFRecipe;
                settings.Connection = wizard.ConnectionString;
            }
            */
        }

        private void textEditConnection_ClearButtonClick(object sender, RoutedEventArgs e)
        {
            var recipe = DataContext as UFRecipeEntity;
            recipe.ConnectionString = 
            recipe.DataProvider = 
            recipe.DataProviderDisplayName = 
            recipe.DataProviderDescription = 
            recipe.DataProviderShortDisplayName = 
            recipe.DataSourceName = 
            recipe.DataSourceDisplayName = String.Empty;

            textEditConnection.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tag = DataContext as IDynamicSettingsEditing;
            if (EditorManager.UfuaEditorService != null && tag != null)
            {
                /* The logic above has been moved to recipe model.
                var writabledatavalues = (from c in Document.RecipeEntity.GetFlatDataValuesCollection()
                                          where c.UseInCommunication && (c.UFGroupAss == null || String.IsNullOrEmpty(c.UFGroupAss.StartingAddress))
                                          select c).ToList();
                if (writabledatavalues.Count > 0)
                    (DataContext as UFRecipeEntity).DataType = (int)writabledatavalues[0].DataType;
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
