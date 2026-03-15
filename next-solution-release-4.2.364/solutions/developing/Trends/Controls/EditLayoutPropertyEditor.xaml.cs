using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFInterfaces;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;

namespace Trends.Controls
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
                if (contextObject is ServerHistoryTrend)
                {
                    var obj = contextObject as ServerHistoryTrend;
                    //ScrollViewer view = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
                    using (var lcontrol = new ServerHistoryTrend()
                    {
                        Width = obj.Width,
                        Height = obj.Height,
                        ControlViewMode = obj.ControlViewMode,
                        UseEndTime = obj.UseEndTime,
                        UseStartTime = obj.UseStartTime,
                        StartTime = obj.StartTime,
                        EndTime = obj.EndTime,
                        FilterType = obj.FilterType,
                        UseMaxReturnValues = obj.UseMaxReturnValues,
                        MaxReturnValues = obj.MaxReturnValues,
                        ReadTypeDefinition = obj.ReadTypeDefinition,
                        Aggregate = obj.Aggregate
                    })
                    {
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            lcontrol.DockLayout = obj.DockLayout;
                            lcontrol.GridLayout = obj.GridLayout;
                        }

                        lcontrol.bSmartSettingsEditing = true;

                        //view.Content = control;
                        var Dialog = new GeneralDialogContent(lcontrol)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.EditLayoutPopupTitle,
                            HelpLink = "ServerHistoryTrendEditLayout"
                        };

                        Dialog.Closing += (o, ea) =>
                        {
                            if ((o as GeneralDialogContent).DialogResult == true)
                            {
                                lcontrol.SaveDesignGridLayout();
                                lcontrol.SaveDesignDockLayout();
                                obj.GridLayout = lcontrol.GridLayout;
                                obj.DockLayout = lcontrol.DockLayout;
                                obj.UseEndTime = lcontrol.UseEndTime;
                                obj.UseStartTime = lcontrol.UseStartTime;
                                obj.StartTime = lcontrol.StartTime;
                                obj.EndTime = lcontrol.EndTime;
                                obj.FilterType = lcontrol.FilterType;
                                obj.UseMaxReturnValues = lcontrol.UseMaxReturnValues;
                                obj.MaxReturnValues = lcontrol.MaxReturnValues;
                                obj.ReadTypeDefinition = lcontrol.ReadTypeDefinition;
                                obj.Aggregate = lcontrol.Aggregate;
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
                if (contextObject is ServerHistoryTrend)
                {
                    if (UIInterface == null ||
                        UIInterface.ShowYesNo(Properties.Resources.ResetLayoutAskConfirm, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    {
                        var obj = contextObject as ServerHistoryTrend;
                        obj.ResetGridLayout();
                        obj.ResetDockLayout();
                    }
                }
            }
        }
        #endregion
    }
}
