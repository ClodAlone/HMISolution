using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFInterfaces;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;

namespace SpreadSheet.Controls
{
    /// <summary>
    /// Interaction logic for EditLayoutPropertyEditor.xaml
    /// </summary>
    public partial class EditLayoutPropertyEditor : UserControl
    {
        #region Dependency Properties
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(EditLayoutPropertyEditor), new UIPropertyMetadata(null));
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }
        #endregion
        #endregion

        #region Constructors
        public EditLayoutPropertyEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        IWorkspace workspace;
        IWorkspace Workspace
        {
            get
            {
                if (workspace == null && Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;
                return workspace;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null && Document != null)
                    uiInterface = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }
        #endregion

        #region Methods
        object GetContextObject()
        {
            var contextObject = Workspace.ContextObject;
            if (contextObject == null && Workspace.ContextObjects != null && Workspace.ContextObjects.Count > 0)
                contextObject = Workspace.ContextObjects[0];
            if (contextObject is IEntityReference)
                contextObject = (contextObject as IEntityReference).ContainedObject;
            return contextObject;
        }
        #endregion

        #region Commands
        private void EditLayout_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (Workspace != null)
            {
                var contextObject = GetContextObject();
                if (contextObject is SpreadSheet)
                {
                    var obj = contextObject as SpreadSheet;
                    //ScrollViewer view = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
                    using (var lcontrol = new SpreadSheet()
                    {
                        bSettingMode = true,
                        bDesignerMode = true,
                        ShowRibbons = true,
                        DocumentPath = obj.DocumentPath
                    })
                    { 
                        //if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        //{
                        //    lcontrol.GridLayout = obj.GridLayout;
                        //}

                        //view.Content = control;
                        var Dialog = new GeneralDialogContent(lcontrol)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.EditLayoutPopupTitle,
                            HelpLink = "SpreadSheetEditLayout"
                        };

                        //Dialog.Closing += (o, ea) =>
                        //{
                        //    if ((o as GeneralDialogContent).DialogResult == true)
                        //    {
                        //        lcontrol.SaveDesignGridLayout();
                        //        obj.GridLayout = lcontrol.GridLayout;
                        //    }
                        //};

                        Dialog.ShowDialog();
                    }
                }
            }
        }

        //private void ResetLayout_Click(object sender, RoutedEventArgs e)
        //{
        //    e.Handled = true;
        //    if (Workspace != null)
        //    {
        //        var contextObject = GetContextObject();
        //        if (contextObject is SpreadSheet)
        //        {
        //            if (UIInterface == null ||
        //                UIInterface.ShowYesNo(Properties.Resources.ResetLayoutAskConfirm, CustomDialogIcons.Question) == CustomDialogResults.Yes)
        //            {
        //                var obj = contextObject as SpreadSheet;
        //                obj.ResetGridLayout();
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
