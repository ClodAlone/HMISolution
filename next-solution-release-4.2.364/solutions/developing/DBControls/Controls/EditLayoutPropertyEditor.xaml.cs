using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFInterfaces;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;

namespace DBControls.Controls
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
                if (contextObject is GridControl)
                {
                    var obj = contextObject as GridControl;
                    //ScrollViewer view = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
                    using (var lcontrol = new GridControl()
                    {
                        bDesignmode = true,
                        Width = obj.Width,
                        Height = obj.Height,
                    })
                    {
                        lcontrol.Document = obj.Document;
                        lcontrol.ControlDataSource = obj.ControlDataSource;
                        lcontrol.defaultDataProvider = obj.defaultDataProvider;
                        lcontrol.defaultConnectionString = obj.defaultConnectionString;
                        lcontrol.RowAreaFontSettings = obj.RowAreaFontSettings;
                        lcontrol.HeaderFontSettings = obj.HeaderFontSettings;
                        lcontrol.bSmartSettingsEditing = true;
                        lcontrol.commands.IsEnabled = false;

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            lcontrol.GridLayout = obj.GridLayout;
                        }

                        lcontrol.ShowCommandButtons = obj.ShowCommandButtons;
                        lcontrol.ShowBestFitButton = obj.ShowBestFitButton;
                        lcontrol.ShowFilterPanel = obj.ShowFilterPanel;
                        lcontrol.ShowSearchPanel = obj.ShowSearchPanel;
                        lcontrol.ShowGroupPanel = obj.ShowGroupPanel;

                        //view.Content = control;
                        var Dialog = new GeneralDialogContent(lcontrol)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.EditLayoutPopupTitle,
                            HelpLink = "GridControlEditLayout"
                        };

                        lcontrol.AllowEdit = obj.AllowEdit;
                        lcontrol.AllowAddNew = false;
                        lcontrol.AllowRemove = false;

                        Dialog.Closing += (o, ea) =>
                        {
                            if ((o as GeneralDialogContent).DialogResult == true)
                            {
                                lcontrol.SaveDesignGridLayout();
                                obj.GridLayout = lcontrol.GridLayout;
                            }
                        };

                        Dialog.ShowDialog();
                    }
                }
                else if (contextObject is PivotGrid)
                {
                    var obj = contextObject as PivotGrid;
                    //ScrollViewer view = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
                    using (var lcontrol = new PivotGrid()
                    {
                        bDesignmode = true,
                        Width = obj.Width,
                        Height = obj.Height
                    })
                    {
                        lcontrol.Document = obj.Document;
                        lcontrol.NeedsUpdate = true;
                        lcontrol.ControlDataSource = obj.ControlDataSource;
                        lcontrol.bSmartSettingsEditing = true;
                        lcontrol.commandGrid.IsEnabled = false;

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            lcontrol.GridLayout = obj.GridLayout;
                        }

                        lcontrol.ShowColumnHeaders = true;
                        lcontrol.ShowRowHeaders = true;
                        lcontrol.ShowDataHeaders = true;
                        lcontrol.ShowFilterHeaders = true;
                        lcontrol.ShowRowGrandTotals = obj.ShowRowGrandTotals;
                        lcontrol.ShowRowTotals = obj.ShowRowTotals;
                        lcontrol.ShowColumnGrandTotals = obj.ShowColumnGrandTotals;
                        lcontrol.ShowColumnGrandTotalHeader = obj.ShowColumnGrandTotalHeader;
                        lcontrol.ShowColumnTotals = obj.ShowColumnTotals;
                        lcontrol.ShowTotalsForSingleValues = obj.ShowTotalsForSingleValues;

                        //view.Content = control;
                        var Dialog = new GeneralDialogContent(lcontrol)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.EditLayoutPopupTitle,
                            HelpLink = "GridControlEditLayout"
                        };

                        Dialog.Closing += (o, ea) =>
                        {
                            if ((o as GeneralDialogContent).DialogResult == true)
                            {
                                lcontrol.SaveDesignGridLayout();
                                obj.GridLayout = lcontrol.GridLayout;
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
                if (contextObject is GridControl)
                {
                    if (UIInterface == null ||
                        UIInterface.ShowYesNo(Properties.Resources.ResetLayoutAskConfirm, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    {
                        var obj = contextObject as GridControl;
                        obj.ResetGridLayout();
                    }
                }
                else if (contextObject is PivotGrid)
                {
                    if (UIInterface == null ||
                        UIInterface.ShowYesNo(Properties.Resources.ResetLayoutAskConfirm, CustomDialogIcons.Question) == CustomDialogResults.Yes)
                    {
                        var obj = contextObject as PivotGrid;
                        obj.ResetGridLayout();
                    }
                }
            }
        }
        #endregion
    }
}
