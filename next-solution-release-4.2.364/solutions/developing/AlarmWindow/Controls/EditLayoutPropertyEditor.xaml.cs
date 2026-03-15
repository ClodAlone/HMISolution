using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFInterfaces;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using OPCUAViewModel;
using System.Collections.Generic;

namespace AlarmWindow.Controls
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
                if (contextObject is Auditing)
                {
                    var obj = contextObject as Auditing;
                    //ScrollViewer view = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
                    using (var control = new Auditing()
                    {
                        Width = obj.Width,
                        Height = obj.Height,
                    })
                    {

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            control.GridLayout = obj.GridLayout;
                        }

                        control.AllowDropColumns = true;
                        control.bSmartSettingsEditing = true;

                        //view.Content = control;
                        var Dialog = new GeneralDialogContent(control)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.EditLayoutPopupTitle,
                            HelpLink = "AuditingEditLayout"
                        };

                        Dialog.Closing += (o, ea) =>
                        {
                            if ((o as GeneralDialogContent).DialogResult == true)
                            {
                                control.SaveDesignGridLayout();
                                obj.GridLayout = control.GridLayout;
                            }
                        };

                        Dialog.ShowDialog();
                    }
                }
                else if (contextObject is GridAlarmWindow)
                {
                    var obj = contextObject as GridAlarmWindow;
                    //ScrollViewer view = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
                    using (var control = new GridAlarmWindow()
                    {
                        Width = obj.Width,
                        Height = obj.Height
                    })
                    {
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            control.GridLayout = obj.GridLayout;
                        }

                        control.gridControl.ItemsSource = new List<ConditionStateViewModel>();
                        control.AllowDropColumns = true;
                        control.ShowGroupPanel = false;
                        control.AllowResizing = true;
                        control.AuthomaticColumnLayout = true;
                        control.IsEnabled = true;
                        control.ConnectChildAlarms = obj.ConnectChildAlarms;

                        control.bSmartSettingsEditing = true;

                        //view.Content = control;
                        var Dialog = new GeneralDialogContent(control)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.EditLayoutPopupTitle,
                            HelpLink = "GridAlarmWindowEditLayout"
                        };

                        control.ShowUshelvedAlaramsButton = false;
                        control.ShowExpandCollapseButtons = false;
                        control.ShowFilterPanel = true;

                        Dialog.Closing += (o, ea) =>
                        {
                            if ((o as GeneralDialogContent).DialogResult == true)
                            {
                                obj.ConnectChildAlarms = control.Col115.Visible;
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
                if (contextObject is Auditing)
                {
                    if (UIInterface == null ||
                        UIInterface.ShowYesNo(Properties.Resources.ResetLayoutAskConfirm, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    {
                        var obj = contextObject as Auditing;
                        obj.ResetGridLayout();
                    }
                }
                else if (contextObject is GridAlarmWindow)
                {
                    if (UIInterface == null ||
                        UIInterface.ShowYesNo(Properties.Resources.ResetLayoutAskConfirm, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    {
                        var obj = contextObject as GridAlarmWindow;
                        obj.ResetGridLayout();
                    }
                }
            }
        }
        #endregion
    }
}
