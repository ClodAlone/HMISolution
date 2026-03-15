using DataLoggerColumnListControl;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using Converters;
using OPCUAViewModelService.ComponentService;
using UFInterfaces.Editors;
using OPCUAViewModel.Converters;

namespace SmartControlUtilities.Controls
{
    /// <summary>
    /// Interaction logic for TagSelector.xaml
    /// </summary>
    public partial class TagSelector : UserControl
    {
        #region DP

        #region UpdateRelatedProperties
        public static readonly DependencyProperty UpdateRelatedPropertiesProperty = DependencyProperty.Register("UpdateRelatedProperties", typeof(bool), typeof(TagSelector), new UIPropertyMetadata(true));
        public bool UpdateRelatedProperties
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UpdateRelatedPropertiesProperty);
            }
            set
            {
                SetValue(UpdateRelatedPropertiesProperty, value);
            }
        }

        #endregion


        #region HitTestVisiblePropName
        public static readonly DependencyProperty HitTestVisiblePropNameProperty = DependencyProperty.Register("HitTestVisiblePropName", typeof(string), typeof(TagSelector), new UIPropertyMetadata(null));

        public string HitTestVisiblePropName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(HitTestVisiblePropNameProperty);
            }
            set
            {
                SetValue(HitTestVisiblePropNameProperty, value);
            }
        }

        #endregion


        #region TagName
        public static readonly DependencyProperty TagNameProperty = DependencyProperty.Register("TagName", typeof(string), typeof(TagSelector), new UIPropertyMetadata(null));
        public string TagName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TagNameProperty);
            }
            set
            {
                SetValue(TagNameProperty, value);
            }
        }
        #endregion

        #region NodeID
        public static readonly DependencyProperty NodeIDProperty = DependencyProperty.Register("NodeID", typeof(string), typeof(TagSelector), new UIPropertyMetadata(null));
        public string NodeID
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(NodeIDProperty);
            }
            set
            {
                SetValue(NodeIDProperty, value);
            }
        }
        #endregion

        #region SourceTimeStamp
        public static readonly DependencyProperty SourceTimeStampProperty = DependencyProperty.Register("SourceTimeStamp", typeof(string), typeof(TagSelector), new UIPropertyMetadata(null));
        public string SourceTimeStamp
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(SourceTimeStampProperty);
            }
            set
            {
                SetValue(SourceTimeStampProperty, value);
            }
        }
        #endregion

        #region ReferencedTag
        public static readonly DependencyProperty ReferencedTagProperty = DependencyProperty.Register("ReferencedTag", typeof(string), typeof(TagSelector), new UIPropertyMetadata(null, new PropertyChangedCallback(OnReferencedTagChanged)));
        private static void OnReferencedTagChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TagSelector control = o as TagSelector;
            if (control != null)
                control.OnReferencedTagChanged((string)e.OldValue, (string)e.NewValue);
        }
        protected virtual void OnReferencedTagChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlBinding();
        }
        public string ReferencedTag
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ReferencedTagProperty);
            }
            set
            {
                SetValue(ReferencedTagProperty, value);
            }
        }
        #endregion


        #region HistoricalPropName
        public static readonly DependencyProperty HistoricalPropNameProperty = DependencyProperty.Register("HistoricalPropName", typeof(string), typeof(TagSelector), new UIPropertyMetadata(null, new PropertyChangedCallback(OnHistoricalPropNameChanged)));
        private static void OnHistoricalPropNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TagSelector control = o as TagSelector;
            if (control != null)
                control.OnHistoricalPropNameChanged((string)e.OldValue, (string)e.NewValue);
        }
        protected virtual void OnHistoricalPropNameChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdatehistoricalControlBinding();
        }
        public string HistoricalPropName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(HistoricalPropNameProperty);
            }
            set
            {
                SetValue(HistoricalPropNameProperty, value);
            }
        }
        #endregion


        #region ColumnPropName
        public static readonly DependencyProperty ColumnPropNameProperty = DependencyProperty.Register("ColumnPropName", typeof(string), typeof(TagSelector), new UIPropertyMetadata(null));
        public string ColumnPropName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ColumnPropNameProperty);
            }
            set
            {
                SetValue(ColumnPropNameProperty, value);
            }
        }
        #endregion


        #region DlrsourcePropName
        public static readonly DependencyProperty DlrsourcePropNameProperty = DependencyProperty.Register("DlrsourcePropName", typeof(string), typeof(TagSelector), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDlrsourcePropNameChanged)));
        private static void OnDlrsourcePropNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TagSelector control = o as TagSelector;
            if (control != null)
                control.OnDlrsourcePropNameChanged((string)e.OldValue, (string)e.NewValue);
        }
        protected virtual void OnDlrsourcePropNameChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateDlrsourceControlBinding();
        }
        public string DlrsourcePropName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DlrsourcePropNameProperty);
            }
            set
            {
                SetValue(DlrsourcePropNameProperty, value);
            }
        }
        #endregion


        #region RuntimeVersion
        public static readonly DependencyProperty RuntimeVersionProperty = DependencyProperty.Register("RuntimeVersion", typeof(bool), typeof(TagSelector), new UIPropertyMetadata(false));
        public bool RuntimeVersion
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(RuntimeVersionProperty);
            }
            set
            {
                SetValue(RuntimeVersionProperty, value);
            }
        }
        #endregion

        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(TagSelector), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDocumentChanged)));
        private static void OnDocumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TagSelector control = o as TagSelector;
            if (control != null)
                control.OnDocumentChanged((IDocument)e.OldValue, (IDocument)e.NewValue);
        }
        protected virtual void OnDocumentChanged(IDocument oldValue, IDocument newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != null)
            {
                UFUAEditor = newValue.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            }
        }
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

        #region OnlyHistoricalTags
        public static readonly DependencyProperty OnlyHistoricalTagsProperty = DependencyProperty.Register("OnlyHistoricalTags", typeof(bool), typeof(TagSelector), new UIPropertyMetadata(false, new PropertyChangedCallback(OnOnlyHistoricalTagsChanged)));
        private static void OnOnlyHistoricalTagsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TagSelector control = o as TagSelector;
            if (control != null)
                control.OnOnlyHistoricalTagsChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        protected virtual void OnOnlyHistoricalTagsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue)
            {
                tagDialog_localserver = newValue;
                tagDialog_noDataSinks = newValue;
                tagDialog_type = newValue ? UFInterfaces.Editors.FilterType.Historians : UFInterfaces.Editors.FilterType.None;
            }
        }
        public bool OnlyHistoricalTags
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(OnlyHistoricalTagsProperty);
            }
            set
            {
                SetValue(OnlyHistoricalTagsProperty, value);
            }
        }
        #endregion

        #endregion

        #region declarations
        IUFUAEditorManager UFUAEditor;
        bool bLoaded;
        bool bEditing;
        String oldText;
        public event EventHandler OnApplyChanges;
        TextEdit uriText;
        ProgressBar progressBar;
        ComboBoxEdit uriLabel;
        bool bInit;
        UFInterfaces.Editors.FilterType tagDialog_type = UFInterfaces.Editors.FilterType.None;
        bool tagDialog_localserver;
        bool tagDialog_noDataSinks;
        #endregion

        #region Events
        virtual public void OnApplyChangesEvent(object sender, EventArgs e = null)
        {
            OnApplyChanges?.Invoke(sender, e ?? EventArgs.Empty);
        }
        #endregion

        #region ctor
        public TagSelector()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    InitControl();
                }
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                    bFilled = false;

                    controlGrid.Children.Clear();

                    if (uriLabel != null)
                    {
                        uriLabel.PopupOpening -= uriLabel_PopupOpening;
                        uriLabel.LostFocus -= uriLabel_LostFocus;
                        uriLabel.GotFocus -= uriLabel_GotFocus;
                        uriLabel.PreviewKeyDown -= uriLabel_KeyDown;
                        uriLabel.SelectedIndexChanged -= uriLabel_SelectedIndexChanged;
                        BindingOperations.ClearAllBindings(uriLabel);
                    }
                    uriLabel = null;
                    if (uriText != null)
                    {
                        BindingOperations.ClearAllBindings(uriText);
                    }
                    uriText = null;
                    BindingOperations.ClearAllBindings(dockPanel);

                    visConverter = null;
                    invVisconverter = null;
                    converter = null;
                }
            };
        }

        #endregion

        #region methods
        private void InitControl()
        {
            uriText = new TextEdit() { IsHitTestVisible = false };
            var uriLabel_panel = new DockPanel();
            progressBar = new ProgressBar()
            {
                IsIndeterminate = true,
                Width = 20,
                Height = 20,
                Visibility = Visibility.Collapsed,
                VerticalAlignment = VerticalAlignment.Top
            };
            uriLabel = new ComboBoxEdit()
            {
                ShowSizeGrip = false,
                AutoComplete = true,
                ImmediatePopup = true,
                FindButtonPlacement = EditorPlacement.Popup
            };

            UpdateControlBinding();
            if(UpdateRelatedProperties)
            {
                UpdateDlrsourceControlBinding();
                UpdatehistoricalControlBinding();
            }
            else
            {
                uriText.Visibility = Visibility.Collapsed;
                uriLabel.Visibility = Visibility.Visible; 
            }

            uriLabel.PopupOpening += uriLabel_PopupOpening;
            uriLabel.LostFocus += uriLabel_LostFocus;
            uriLabel.GotFocus += uriLabel_GotFocus;
            uriLabel.PreviewKeyDown += uriLabel_KeyDown;
            uriLabel.SelectedIndexChanged += uriLabel_SelectedIndexChanged;

            controlGrid.Children.Add(uriText);
            controlGrid.Children.Add(uriLabel_panel);
            DockPanel.SetDock(progressBar, Dock.Left);
            uriLabel_panel.Children.Add(progressBar);
            uriLabel_panel.Children.Add(uriLabel);

            bInit = true;
        }
        UriConverter converter;
        Converters.BooleanToVisibilityConverter visConverter;
        BooleanToInvertionVisibilityConverter invVisconverter;
        private void UpdateControlBinding()
        {
            if (HitTestVisiblePropName != null)
            {
                var bindingEnable = new Binding()
                {
                    Path = new PropertyPath(HitTestVisiblePropName),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay
                };
                dockPanel.SetBinding(DockPanel.IsHitTestVisibleProperty, bindingEnable);
            }

            if (ReferencedTag != null)
            {
                if (converter == null)
                    converter = new UriConverter();
                var bindingValue = new Binding()
                {
                    Path = new PropertyPath(ReferencedTag),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    Converter = converter
                };
                uriText.SetBinding(DevExpress.Xpf.Editors.TextEdit.EditValueProperty, bindingValue);
                uriLabel.SetBinding(DevExpress.Xpf.Editors.TextEdit.EditValueProperty, bindingValue);
            }
        }
        private void UpdatehistoricalControlBinding()
        {
            if (HistoricalPropName != null)
            {
                var bindingHistoricalValue = new Binding()
                {
                    Path = new PropertyPath(HistoricalPropName),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay
                };
                dockPanel.SetBinding(DockPanel.ToolTipProperty, bindingHistoricalValue);
                uriText?.SetBinding(DevExpress.Xpf.Editors.TextEdit.ToolTipProperty, HistoricalPropName);
                uriLabel?.SetBinding(DevExpress.Xpf.Editors.TextEdit.ToolTipProperty, HistoricalPropName);
            }
        }
        
        private void UpdateDlrsourceControlBinding()
        {
            if (DlrsourcePropName != null)
            {
                if (visConverter == null)
                    visConverter = new Converters.BooleanToVisibilityConverter();
                if (invVisconverter == null)
                    invVisconverter = new BooleanToInvertionVisibilityConverter();

                var bindingVisibility = new Binding()
                {
                    Path = new PropertyPath(DlrsourcePropName),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    Converter = visConverter
                };
                var bindingInvVisibility = new Binding()
                {
                    Path = new PropertyPath(DlrsourcePropName),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    Converter = invVisconverter
                };
                uriText.SetBinding(DevExpress.Xpf.Editors.TextEdit.VisibilityProperty, bindingVisibility);
                uriLabel.SetBinding(DevExpress.Xpf.Editors.TextEdit.VisibilityProperty, bindingInvVisibility);
            }
        }
        
        private void BtnClear(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;
            Helpers.SetProperty(null, ReferencedTag, DataContext);
            if(UpdateRelatedProperties)
            {
                Helpers.SetProperty(string.Empty, TagName, DataContext);
                Helpers.SetProperty(string.Empty, HistoricalPropName, DataContext);
                Helpers.SetProperty(string.Empty, ColumnPropName, DataContext);
                Helpers.SetProperty(string.Empty, SourceTimeStamp, DataContext);
                Helpers.SetProperty(string.Empty, NodeID, DataContext);
            }
            uriLabel.GetBindingExpression(ComboBoxEdit.EditValueProperty).UpdateTarget();
            uriText.GetBindingExpression(TextEdit.EditValueProperty).UpdateTarget();

            OnApplyChangesEvent(this);
        }

        private void BtnEdit(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;
            if (UpdateRelatedProperties && DlrsourcePropName != null && (bool)Helpers.GetPropertyValue(DlrsourcePropName, DataContext))
            {
                #region DlrSource
                if (UFUAEditor == null)
                    return;

                var lcontrol = new DataLoggerColumnList(Document);
                var dialog = new GeneralDialogContent(lcontrol, GeneralDialogButtons.OkCancelHelpButtons)
                {
                    Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                    Title = Properties.Resources.ColumnListSelection,
                    HelpLink = "DataLoggerColumnListSelector"
                };

                if (dialog.ShowDialog() == true)
                {
                    var currvalue = lcontrol.GetSelectedColumnReference();

                    if (currvalue != null)
                    {
                        var column = lcontrol.GetSelectedDataLoggerColumn();
                        Helpers.SetProperty(currvalue, ReferencedTag, DataContext);
                        Helpers.SetProperty(lcontrol.GetSelectedDLRName(), HistoricalPropName, DataContext);
                        Helpers.SetProperty(column.Name, ColumnPropName, DataContext);
                        Helpers.SetProperty($"{column.Name}_{column.SourceTimeStampColumnName}", SourceTimeStamp, DataContext);
                        Helpers.SetProperty(string.Empty, NodeID, DataContext);
                    }
                    else
                    {
                        Helpers.SetProperty(null, ReferencedTag, DataContext);
                        Helpers.SetProperty(string.Empty, TagName, DataContext);
                        Helpers.SetProperty(string.Empty, HistoricalPropName, DataContext);
                        Helpers.SetProperty(string.Empty, ColumnPropName, DataContext);
                        Helpers.SetProperty(string.Empty, SourceTimeStamp, DataContext);
                        Helpers.SetProperty(string.Empty, NodeID, DataContext);
                    }
                }
                #endregion
                uriLabel.GetBindingExpression(ComboBoxEdit.EditValueProperty).UpdateTarget();
                uriText.GetBindingExpression(TextEdit.EditValueProperty).UpdateTarget();
                OnApplyChangesEvent(this);
            }
            else
            {
                #region HistoricalSource || UpdateRelatedProperties
                if (UFUAEditor == null)
                    return;

                if(!RuntimeVersion)
                {
                    OPCUAEntityReference value = (OPCUAEntityReference)Helpers.GetPropertyValue(ReferencedTag, DataContext);
                    if (value == null)
                        value = new OPCUAEntityReference();

                    value.Editor = UFUAEditor;
                    value.Document = Document;
                    
                    if (value.Edit(sync: true, filter: tagDialog_type, localserver: tagDialog_localserver, noDataSinks: tagDialog_noDataSinks))
                    {
                        Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            if ((value as OPCUAEntityReference) != null && (value as OPCUAEntityReference).HasValidValue)
                            {
                                Helpers.SetProperty(value, ReferencedTag, DataContext);
                                if(UpdateRelatedProperties)
                                {
                                    var document = UFUAEditor.GetProjectDocument(Document, value.AppName, true);
                                    if (document != null)
                                    {
                                        Helpers.SetProperty(UFUAEditor.GetHistorianName(document, value.ResolvedNodeId), HistoricalPropName, DataContext);
                                    }
                                    Helpers.SetProperty(string.Empty, ColumnPropName, DataContext);
                                    Helpers.SetProperty(GetRelativePath((value as OPCUAEntityReference).RelativePath), TagName, DataContext);
                                    Helpers.SetProperty(string.Empty, SourceTimeStamp, DataContext);
                                    Helpers.SetProperty((value as OPCUAEntityReference).ResolvedNodeId?.ToString(), NodeID, DataContext);
                                }
                            }
                            else
                            {
                                Helpers.SetProperty(null, ReferencedTag, DataContext);
                                if (UpdateRelatedProperties)
                                {
                                    Helpers.SetProperty(string.Empty, TagName, DataContext);
                                    Helpers.SetProperty(string.Empty, HistoricalPropName, DataContext);
                                    Helpers.SetProperty(string.Empty, ColumnPropName, DataContext);
                                    Helpers.SetProperty(string.Empty, SourceTimeStamp, DataContext);
                                    Helpers.SetProperty(string.Empty, NodeID, DataContext);
                                }
                            }
                            uriLabel.GetBindingExpression(ComboBoxEdit.EditValueProperty).UpdateTarget();
                            uriText.GetBindingExpression(TextEdit.EditValueProperty).UpdateTarget();
                            OnApplyChangesEvent(this);
                        });
                    }
                }
                else
                {
                    var runtimeAddressSpaceControl = UFUAEditor.GetRuntimeAddressSpaceControl(Document);
                    if (runtimeAddressSpaceControl == null)
                        return;
                    if (runtimeAddressSpaceControl is IAddressSpaceControl)
                        (runtimeAddressSpaceControl as IAddressSpaceControl).FilterType = tagDialog_type;

                    runtimeAddressSpaceControl.DataContext = (OPCUAEntityReference)Helpers.GetPropertyValue(ReferencedTag, DataContext);
                    var dialog = new GeneralDialog(runtimeAddressSpaceControl)
                    {
                        Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                        Title = Properties.Resources.TagListSelection,
                        bShowOk = true,
                        bShowCancel = true,
                        bShowClose = false,
                        bShowHelp = false
                    };

                    dialog.Loaded += (o, ea) => { ThemeHelper.SetTheme(dialog); };

                    if (dialog.ShowDialog() == true)
                    {
                        Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            var value = runtimeAddressSpaceControl.DataContext;
                            if ((value as OPCUAEntityReference) != null && (value as OPCUAEntityReference).HasValidValue)
                            {
                                Helpers.SetProperty(value, ReferencedTag, DataContext);
                                if (UpdateRelatedProperties)
                                {
                                    var document = UFUAEditor.GetProjectDocument(Document, (value as OPCUAEntityReference).AppName, true);
                                    if (document != null)
                                    {
                                        Helpers.SetProperty(UFUAEditor.GetHistorianName(document, (value as OPCUAEntityReference).ResolvedNodeId), HistoricalPropName, DataContext);
                                    }
                                    Helpers.SetProperty(string.Empty, ColumnPropName, DataContext);
                                    Helpers.SetProperty(GetRelativePath((value as OPCUAEntityReference).RelativePath), TagName, DataContext);
                                    Helpers.SetProperty(string.Empty, SourceTimeStamp, DataContext);
                                    Helpers.SetProperty((value as OPCUAEntityReference).ResolvedNodeId?.ToString(), NodeID, DataContext);
                                }
                            }
                            else
                            {
                                Helpers.SetProperty(null, ReferencedTag, DataContext);
                                if (UpdateRelatedProperties)
                                {
                                    Helpers.SetProperty(string.Empty, TagName, DataContext);
                                    Helpers.SetProperty(string.Empty, HistoricalPropName, DataContext);
                                    Helpers.SetProperty(string.Empty, ColumnPropName, DataContext);
                                    Helpers.SetProperty(string.Empty, SourceTimeStamp, DataContext);
                                    Helpers.SetProperty(string.Empty, NodeID, DataContext);
                                }
                            }
                            uriLabel.GetBindingExpression(ComboBoxEdit.EditValueProperty).UpdateTarget();
                            uriText.GetBindingExpression(TextEdit.EditValueProperty).UpdateTarget();
                            OnApplyChangesEvent(this);
                        });
                    }
                }
                #endregion
            }
        }

        string GetRelativePath(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                string oldChars = string.Format("{0}:", ns);
                string relative = string.Format("{0}", (value).Replace(oldChars, ""));
                return relative; 
            }

            return String.Empty;
        }

        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
        }

        bool bFilled;
        void FillComboBox(bool bForceRefresh = false)
        {
            if ((bFilled && !bForceRefresh) || !bLoaded || !OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            bFilled = true;
            progressBar.Visibility = Visibility.Visible;
            bool onlyHistorical = OnlyHistoricalTags;
            var task = Task.Factory.StartNew(() =>
            {
                return editor.GetFlatFullTagNameCollectionOrderByName(doc, onlyHistorical, bForceRefresh);
            });
            task.ContinueWith(ret =>
            {
                if (bLoaded && uriLabel != null && progressBar != null)
                {
                    uriLabel.ItemsSource = ret.Result;
                    progressBar.Visibility = Visibility.Collapsed;
                    uriLabel.IsPopupOpen = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;
            if (!UpdateRelatedProperties && DlrsourcePropName != null && (bool)Helpers.GetPropertyValue(DlrsourcePropName, DataContext))
                return;

            if (sender as ComboBoxEdit == null /*|| (sender as ComboBoxEdit).Tag == null*/)
                return;

            if (!bEditing || Document == null || UFUAEditor == null)
            {
                bEditing = false;
                var tag = Helpers.GetPropertyValue(ReferencedTag, DataContext) as OPCUAEntityReference;
                if (tag != null && tag.StringRepresentation == uriLabel.Text)
                {
                    uriLabel.Text = tag.StringRepresentationWithProject;
                    oldText = null;
                }
                return;
            }

            bEditing = false;
            UpdateData();
        }

        OPCUAEntityReference original;
        private async void UpdateData(/*bool fromUriLostFocus*/)
        {
            var uriLabel = this.uriLabel;
            if (!bInit || UFUAEditor == null || uriLabel == null)
                return;

            string instance;
            string name;

            SmartControlHelper.GetInstanceName(uriLabel.Text, out instance, out name);

            if (original == null)
                original = Helpers.GetPropertyValue(ReferencedTag, DataContext) as OPCUAEntityReference;
            var xml = UFUAEditor.GetTagEntityReference(Document, name, instance);
            if (xml == null)
            {
                var datasync = OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                if (datasync != null)
                {
                    var relPath = name.Replace('\\', '&').Replace('/', '&');
                    datasync.GetVariables();
                    var tagRef = datasync.GetReference(relPath);
                    if (!String.IsNullOrEmpty(tagRef.ReadablePath))
                        xml = tagRef.ToXml();
                }
            }
            if (String.IsNullOrEmpty(xml))
            {
                original = new OPCUAEntityReference(null);
                original.HumanReadable = original.RelativePath = original.ReadablePath = uriLabel.Text;
                original.ResolvedNodeId = null;
                Helpers.SetProperty(original, ReferencedTag, DataContext);

                var oldColor = uriLabel.Foreground;
                uriLabel.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = oldColor;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = oldColor;
            }
            else
            {
                oldText = null;
                if (DataContext == null)
                    return;
                var tag = xml.FromXml<OPCUAEntityReference>();
                uriLabel.Text = tag.StringRepresentationWithProject;
                Helpers.SetProperty(tag, ReferencedTag, DataContext);
                if(UpdateRelatedProperties)
                {
                    var document = UFUAEditor.GetProjectDocument(Document, tag.AppName, true);
                    if (document != null)
                    {
                        Helpers.SetProperty(UFUAEditor.GetHistorianName(document, tag.ResolvedNodeId), HistoricalPropName, DataContext);
                    }
                    Helpers.SetProperty(string.Empty, ColumnPropName, DataContext);
                    Helpers.SetProperty(GetRelativePath((tag as OPCUAEntityReference).RelativePath), TagName, DataContext);
                    Helpers.SetProperty(string.Empty, SourceTimeStamp, DataContext);
                    Helpers.SetProperty((tag as OPCUAEntityReference).ResolvedNodeId?.ToString(), NodeID, DataContext);
                }
            }
            OnApplyChangesEvent(this);//new PenNameSelector.ApplyChangeCallerEventArgs { dataContextResetRequired = fromUriLostFocus });
        }

        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;
            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            var reference = Helpers.GetPropertyValue(ReferencedTag, DataContext) as OPCUAEntityReference;
            if (reference != null)
            {
                if (String.IsNullOrEmpty(oldText) &&
                   (uriLabel.Text.Contains(reference.HumanReadable) ||
                    uriLabel.Text.Contains(reference.HumanReadable.Replace("\\", "/").Replace("&", "/")) ||
                    uriLabel.Text.Contains(reference.HumanReadable.Replace('/', '\\').Replace('&', '\\'))))
                {
                    oldText = uriLabel.Text;
                    var s = reference.StringRepresentation;
                    if (!String.IsNullOrEmpty(s))
                        uriLabel.Text = s;
                }
            }

            uriLabel.SelectAll();
        }

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            bEditing = false;
            if (DataContext == null || (sender as ComboBoxEdit).SelectedIndex == -1)
                return;
            UpdateData();
        }
        #endregion
    }
}

