using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UFMenuEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using MenuSettings.MenuModel;
using MenuSettings.Documents;
using OPCUAViewModel;
using UFInterfaces;

namespace UFMenuEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewMenuItemDefinition.xaml
    /// </summary>
    public partial class NewMenuItemDefinition : UserControl
    {
        readonly MenuEditorManagerComponent EditorComponent;
        readonly UFMenuDocument Document;
        OPCUAEntityReference markitem;
        OPCUAEntityReference enableitem;
        bool bLoaded;
        #region Constructor
        public NewMenuItemDefinition(MenuEditorManagerComponent editorComponent, UFMenuDocument doc)
        {
            EditorComponent = editorComponent;
            Document = doc;
            InitializeComponent();
            List<MenuType> atemp = new List<MenuType>();
            atemp.Add(MenuType.Item);
            atemp.Add(MenuType.Separator);
            comboMenuType.ItemsSource = atemp;

            Loaded += (o, e) =>
            {
                UFMenuItemEntity uFMenuItemEntity = DataContext as UFMenuItemEntity;
                if (uFMenuItemEntity != null)
                {
                    OPCUAEntityReferenceModel MarkTag = new OPCUAEntityReferenceModel() { Value = uFMenuItemEntity.MarkTag};
                    OPCUAEntityReferenceModel EnableTag = new OPCUAEntityReferenceModel() { Value = uFMenuItemEntity.EnableTag };

                    textEditMark.DataContext = MarkTag;
                    textEditEnable.DataContext = EnableTag;

                    MarkTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            uFMenuItemEntity.MarkTag = MarkTag.Value;
                        }
                    };
                    EnableTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            uFMenuItemEntity.EnableTag = EnableTag.Value;
                        }
                    };
                }

                if (CommandCtrl.Content == null)
                {
                    var commandor = EditorComponent.CommandExplorer.control;
                    EditorComponent.CommandExplorer.SetSync(commandor, true);
                    commandor.ClearValue(FrameworkElement.WidthProperty);
                    commandor.ClearValue(FrameworkElement.HeightProperty);

                    commandor.DataContext = uFMenuItemEntity;
                    CommandCtrl.Content = commandor;
                }
                else
                    (CommandCtrl.Content as FrameworkElement).DataContext = uFMenuItemEntity;

                if(!bLoaded)
                {
                    bLoaded = true;
                    ImageViewModel entityimagemodel = new ImageViewModel() { Value = uFMenuItemEntity.MenuItemImage };
                    textEditImage.Workspace = Document?.GetService(typeof(IWorkspace)) as IWorkspace;
                    textEditImage.DataContext = entityimagemodel;
                    //lblEditDesc.Background = textEditName.Background = comboMenuType.Background = ApplicationPropertiesHelper.GetProperty("CurrentSkinBackColor") as Brush;
                }


                try
                {
                    SetControlVisibility(uFMenuItemEntity);
                }
                catch (Exception ex)
                { }
            };
        }
        #endregion
        
        #region Objects events
        private void comboMenuType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (e.AddedItems.Count > 0)
            {
                try
                {
                    SetControlVisibility(DataContext as UFMenuItemEntity);
                }
                catch (Exception ex)
                { }
            }
        }
        #endregion

        #region Methods
        void SetControlVisibility(UFMenuItemEntity m)
        {
            if (m == null)
                return;
            if(m.IsPopup())
            {
                lblEditName.Visibility = System.Windows.Visibility.Visible;
                textEditName.Visibility = System.Windows.Visibility.Visible;

                lblEditDesc.Visibility = System.Windows.Visibility.Visible;
                textEditDesc.Visibility = System.Windows.Visibility.Visible;

                lblMenuType.Visibility = System.Windows.Visibility.Hidden;
                comboMenuType.Visibility = System.Windows.Visibility.Hidden;

                lblEditEnable.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;
                TagEnableGrid.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;

                lblEditMark.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;
                TagMarkGrid.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;

                lblEditImage.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;
                gridEditImage.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;

                //lblCommandCtrl.Visibility = System.Windows.Visibility.Hidden;
                gridCommandCtrl.Visibility = System.Windows.Visibility.Hidden;
            }
            else if(m.MenuItemType == MenuType.Item)
            {
                lblEditName.Visibility = System.Windows.Visibility.Visible;
                textEditName.Visibility = System.Windows.Visibility.Visible;

                lblEditDesc.Visibility = System.Windows.Visibility.Visible;
                textEditDesc.Visibility = System.Windows.Visibility.Visible;

                lblMenuType.Visibility = System.Windows.Visibility.Visible;
                comboMenuType.Visibility = System.Windows.Visibility.Visible;

                lblEditEnable.Visibility = System.Windows.Visibility.Visible;
                TagEnableGrid.Visibility = System.Windows.Visibility.Visible;

                lblEditMark.Visibility = System.Windows.Visibility.Visible;
                TagMarkGrid.Visibility = System.Windows.Visibility.Visible;

                lblEditImage.Visibility = System.Windows.Visibility.Visible;
                gridEditImage.Visibility = System.Windows.Visibility.Visible;

                //lblCommandCtrl.Visibility = System.Windows.Visibility.Visible;
                gridCommandCtrl.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                lblEditName.Visibility = System.Windows.Visibility.Hidden;
                textEditName.Visibility = System.Windows.Visibility.Hidden;

                lblEditDesc.Visibility = System.Windows.Visibility.Hidden;
                textEditDesc.Visibility = System.Windows.Visibility.Hidden;

                lblMenuType.Visibility = System.Windows.Visibility.Visible;
                comboMenuType.Visibility = System.Windows.Visibility.Visible;

                lblEditEnable.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;
                TagEnableGrid.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;

                lblEditMark.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;
                TagMarkGrid.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;

                lblEditImage.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;
                gridEditImage.Visibility = System.Windows.Visibility.Collapsed;//System.Windows.Visibility.Hidden;

                //lblCommandCtrl.Visibility = System.Windows.Visibility.Hidden;
                gridCommandCtrl.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        #endregion

        private void textEditName_Click(object sender, RoutedEventArgs e)
        {
            var stringEditor = EditorComponent.StringEditor.GetStringEditor(Document.Parent);
            stringEditor.DataContext = textEditName.Text;

            var Dialog = new GeneralDialogContent(stringEditor)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.SelectStringEditor,
                HelpLink = "StringEditor"
            };
            if (Dialog.ShowDialog() != true)
                return;

            textEditName.Text = stringEditor.DataContext as String;
            textEditName.Focus();
            textEditName.SelectAll();
        }

        private void textEditName_Clear(object sender, RoutedEventArgs e)
        {
            textEditName.Text = String.Empty;
        }

        //private void btnDelEnable_Click(object sender, RoutedEventArgs e)
        //{
        //    var a = DataContext as UFMenuItemEntity;
        //    if (a != null)
        //    {
        //        a.EnableTagName = string.Empty;
        //        if (a.EnableTag != null)
        //            a.EnableTag.Dispose();
        //        a.EnableTag = null;
        //    }
        //}

        //private void btnDelMark_Click(object sender, RoutedEventArgs e)
        //{
        //    var a = DataContext as UFMenuItemEntity;
        //    if (a != null)
        //    {
        //        a.MarkTagName = string.Empty;
        //        if (a.MarkTag != null)
        //            a.MarkTag.Dispose();
        //        a.MarkTag = null;
        //    }
        //}

    }
}
