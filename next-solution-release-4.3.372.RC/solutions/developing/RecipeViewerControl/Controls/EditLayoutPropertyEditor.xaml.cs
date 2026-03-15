using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFInterfaces;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using System.Collections.Generic;

namespace RecipeViewerControl.Controls
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
                if (contextObject is RecipeGrid)
                {
                    var obj = contextObject as RecipeGrid;
                    //ScrollViewer view = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
                    using (var control = new RecipeGrid()
                    {
                        Width = obj.Width,
                        Height = obj.Height,
                    })
                    {
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            control.GridLayout = obj.GridLayout;
                        }

                        control.bSmartSettingsEditing = true;

                        var bShowGroupPanel = control.tableView.ShowGroupPanel;
                        var bAllowGrouping = control.tableView.AllowGrouping;
                        var bAllowDrop = control.tableView.AllowDrop;
                        var bAllowColumnMoving = control.tableView.AllowColumnMoving;
                        control.tableView.ShowGroupPanel = false;
                        control.tableView.AllowGrouping = false;
                        control.tableView.AllowDrop = true;
                        control.tableView.AllowColumnMoving = true;
                        control.tableView.ShowColumnChooser();

                        //view.Content = control;
                        var Dialog = new GeneralDialogContent(control)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.EditLayoutPopupTitle,
                            HelpLink = "RecipeGridEditLayout"
                        };

                        Dialog.Closing += (o, ea) =>
                        {
                            if ((o as GeneralDialogContent).DialogResult == true)
                            {
                                control.tableView.ShowGroupPanel = bShowGroupPanel;
                                control.tableView.AllowGrouping = bAllowGrouping;
                                control.tableView.AllowDrop = bAllowDrop;
                                control.tableView.AllowColumnMoving = bAllowColumnMoving;
                                control.tableView.HideColumnChooser();
                                control.SaveDesignGridLayout();
                                obj.GridLayout = control.GridLayout;
                            }
                        };

                        Dialog.ShowDialog();
                    }
                }
            }
        }

        private void ResetLayout_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (Workspace != null)
            {
                var contextObject = GetContextObject();
                if (contextObject is RecipeGrid)
                {
                    if (UIInterface == null ||
                        UIInterface.ShowYesNo(Properties.Resources.ResetLayoutAskConfirm, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    {
                        var obj = contextObject as RecipeGrid;
                        obj.ResetGridLayout();
                    }
                }
            }
        }
        #endregion
    }
}
