using DevExpress.Spreadsheet;
using OPCUAViewModel;
using ScreenSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using UFInterfaces;
using Utilities;
using Utilities.WPF;
using ViewModelLib;
using System.Windows.Media.Effects;
using DevExpress.Xpf.Spreadsheet;
using DevExpress.Xpf.Printing;
using System.Windows.Markup;
using System.Printing;
using System.IO;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using System.Windows.Controls.Primitives;
using WPFUtilities;
using WPFUtilities.Extensions;
using DevExpress.XtraSpreadsheet;
using System.Windows.Automation.Peers;
using SpreadSheet.Automations;
using WPFUtilities.PropertyDataTemplate;
using DocumentManager.ComponentService;
using WPFUtilities.Converters;
using System.Windows.Media.Animation;
using UIMsgBoxAlertService.ComponentService;
using SpreadSheet.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Ribbon;
using System.Xml.Serialization;

namespace SpreadSheet
{
    /// <summary>
    /// Interaction logic for SpreadSheet.xaml
    /// </summary>
    public partial class SpreadSheet : UserControl, IContainPropertyEditors, IDataErrorInfo, IDisposable, IEntityReference
    {
        #region DP
        #region Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(SpreadSheet));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(SpreadSheet));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(SpreadSheet));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(SpreadSheet));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }


        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as SpreadSheet;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && !bDisposed)
                UpdateColors();
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as SpreadSheet;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && !bDisposed && !IsManipulationEnabled)
                UpdateColors();
        }
        #endregion
        public static readonly DependencyProperty ShowRibbonsProperty = DependencyProperty.Register("ShowRibbons", typeof(bool), typeof(SpreadSheet), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowRibbonsChanged), new CoerceValueCallback(OnCoerceShowRibbons)));

        private static object OnCoerceShowRibbons(DependencyObject o, object value)
        {
            SpreadSheet spreadSheet = o as SpreadSheet;
            if (spreadSheet != null)
                return spreadSheet.OnCoerceShowRibbons((bool)value);
            else
                return value;
        }

        private static void OnShowRibbonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpreadSheet spreadSheet = o as SpreadSheet;
            if (spreadSheet != null)
                spreadSheet.OnShowRibbonsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowRibbons(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowRibbonsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            ribbonControl1.Visibility = ShowRibbons ? Visibility.Visible : Visibility.Collapsed;
        }
        [Category("SpreadSheetSettings")]
        public bool ShowRibbons
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowRibbonsProperty); 
            }
            set
            {
                SetValue(ShowRibbonsProperty, value);
            }
        }

        public static readonly DependencyProperty DocumentPathProperty = DependencyProperty.Register("DocumentPath", typeof(Uri), typeof(SpreadSheet), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDocumentPathChanged), new CoerceValueCallback(OnCoerceDocumentPath)));

        private static object OnCoerceDocumentPath(DependencyObject o, object value)
        {
            SpreadSheet spreadSheet = o as SpreadSheet;
            if (spreadSheet != null)
                return spreadSheet.OnCoerceDocumentPath((Uri)value);
            else
                return value;
        }

        private static void OnDocumentPathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpreadSheet spreadSheet = o as SpreadSheet;
            if (spreadSheet != null)
                spreadSheet.OnDocumentPathChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceDocumentPath(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDocumentPathChanged(Uri oldValue, Uri newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (!String.IsNullOrEmpty(DocumentPath) && !bDesignerMode)
            //    spreadsheetControl1.LoadDocument(DocumentPath);
            if (oldValue != newValue)
            {
                string pathCheck = CheckFilePath(newValue);
                if (!bPathError)
                {
                    spreadsheetControl1.LoadDocument(pathCheck);
                }
                else
                {
                    spreadsheetControl1.CreateNewDocument();//Per evitare che rimanga visibile sullo sfondo l'ultimo file valido caricato. Vedi https://www.devexpress.com/Support/Center/Question/Details/T149767/unload-or-close-document-from-spreadsheet
                    pathErrorOverlay.Visibility = Visibility.Visible;
                }
            }
        }
        internal string CheckFilePath(Uri fileUri)
        {
            string filepath = null;
            var document = ScreenDocument.GetScreenDocument(this);
            if (document != null && DocumentPath != null)
            {
                Uri uri = UriToAbsoluteUriConverter.Convert(fileUri, document, SpecialFolders.Documents);
                if (uri != null)
                    filepath = uri.GetPathString();
            }
            bPathError = false;
            pathErrorOverlay.Visibility = Visibility.Collapsed;
            if (DocumentPath == null)
            {
                if (pathErrorOverlay.Text != Properties.Resources.FilePathNotSpecified)
                {
                    pathErrorOverlay.Style = (System.Windows.Style)FindResource("PathErrorOverlayStyle_notSpecified");
                    pathErrorOverlay.Text = Properties.Resources.FilePathNotSpecified;
                }
                bPathError = true;
            }
            else if (filepath == null || !System.IO.File.Exists((filepath)))
            {
                if (pathErrorOverlay.Text != Properties.Resources.InvalidFilePath)
                {
                    pathErrorOverlay.Style = (System.Windows.Style)FindResource("PathErrorOverlayStyle_notFound");
                    pathErrorOverlay.Text = Properties.Resources.InvalidFilePath;
                }
                bPathError = true;
            }
            return filepath;
        }
        [Category("SpreadSheetSettings")]
        public Uri DocumentPath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                //return MakeAbsolute((String)GetValue(DocumentPathProperty));
                return (Uri)GetValue(DocumentPathProperty);
            }
            set
            {
                //SetValue(DocumentPathProperty, MakeRelative(value));
                SetValue(DocumentPathProperty, value);
            }
        }

        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SpreadSheet), new UIPropertyMetadata(false, new PropertyChangedCallback(OnReadOnlyChanged), new CoerceValueCallback(OnCoerceReadOnly)));

        private static object OnCoerceReadOnly(DependencyObject o, object value)
        {
            SpreadSheet spreadSheet = o as SpreadSheet;
            if (spreadSheet != null)
                return spreadSheet.OnCoerceReadOnly((bool)value);
            else
                return value;
        }

        private static void OnReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpreadSheet spreadSheet = o as SpreadSheet;
            if (spreadSheet != null)
                spreadSheet.OnReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceReadOnly(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnReadOnlyChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            spreadsheetControl1.ReadOnly = ReadOnly;
        }
        [Category("SpreadSheetSettings")]
        public bool ReadOnly
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ReadOnlyProperty);
            }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.SmartControl(this);
            }
        }
        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(SpreadSheet), new UIPropertyMetadata(false));

        [XmlIgnore]
        [MergablePropertyAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SmartProperties
        {
            get
            {
                return (bool)GetValue(SmartPropertiesProperty);
            }
        }

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(SpreadSheet), new UIPropertyMetadata(false));

        [XmlIgnore]
        [MergablePropertyAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditLayout
        {
            get
            {
                return (bool)GetValue(EditLayoutProperty);
            }
        }

        public static readonly DependencyProperty SpreadSheetDataListProperty = DependencyProperty.Register("SpreadSheetDataList", typeof(SpreadSheetDataList), typeof(SpreadSheet), new UIPropertyMetadata(new SpreadSheetDataList(), new PropertyChangedCallback(OnSpreadSheetDataListChanged), new CoerceValueCallback(OnCoerceSpreadSheetDataList)));

        private static object OnCoerceSpreadSheetDataList(DependencyObject o, object value)
        {
            SpreadSheet spreadSheet = o as SpreadSheet;
            if (spreadSheet != null)
                return spreadSheet.OnCoerceSpreadSheetDataList((SpreadSheetDataList)value);
            else
                return value;
        }

        private static void OnSpreadSheetDataListChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpreadSheet spreadSheet = o as SpreadSheet;
            if (spreadSheet != null)
                spreadSheet.OnSpreadSheetDataListChanged((SpreadSheetDataList)e.OldValue, (SpreadSheetDataList)e.NewValue);
        }

        protected virtual SpreadSheetDataList OnCoerceSpreadSheetDataList(SpreadSheetDataList value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpreadSheetDataListChanged(SpreadSheetDataList oldValue, SpreadSheetDataList newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("SpreadSheetSettings")]
        [Browsable(false)]
        public SpreadSheetDataList SpreadSheetDataList
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SpreadSheetDataList)GetValue(SpreadSheetDataListProperty);
            }
            set
            {
                SetValue(SpreadSheetDataListProperty, value);
            }
        }

        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }

        [Browsable(false)]
        public bool NotRunningOnServer
        {
            get
            {
                return !RunningOnServer;
            }
        }

        #endregion

        #region Declarations
        bool bLoaded;
        bool bPathError=true;
        Dictionary<int, PropertyObserver<OPCUAEntityReference>> mapObserver = new Dictionary<int, PropertyObserver<OPCUAEntityReference>>();
        Dictionary<int, PropertyObserver<MonitoredItemViewModel>> observerMonitoredModel = new Dictionary<int, PropertyObserver<MonitoredItemViewModel>>();
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        IDocument Document;
        Dictionary<int, OPCUAEntityReference> opcuaEntityReference;
        private Dictionary<int, OPCUAEntityReference> OpcuaEntityReference
        {
            get
            {
                if (opcuaEntityReference == null)
                {
                    opcuaEntityReference = new Dictionary<int, OPCUAEntityReference>();
                    foreach (SpreadSheetItem item in SpreadSheetDataList)
                    {
                        opcuaEntityReference[SpreadSheetDataList.IndexOf(item)] = item.TagReference;
                    }
                }

                return opcuaEntityReference;
            }
        }
        Storyboard sb;

        DispatcherOperation dpUpdateValue;
        Dictionary<int, Opc.Ua.DataValue> queuedValues = new Dictionary<int, Opc.Ua.DataValue>();
        #endregion

        #region ctor
        internal bool bDesignerMode;
        internal bool bSettingMode;
        bool bInit;
        public SpreadSheet()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
                {
                    if (!bLoaded && !bDisposed)
                    {
                        bLoaded = true;

                        grpHomeFont.CaptionButtonClick += ExecuteSpreadSheetCommand;
                        grpHomeFont.ShowCaptionButton = true;

                        grpHomeAlignment.CaptionButtonClick += ExecuteSpreadSheetCommand;
                        grpHomeAlignment.ShowCaptionButton = true;

                        if (bPathError)
                            pathErrorOverlay.Visibility = Visibility.Visible;

                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                        if (Document != null)
                            UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                        OverrideBaseProperties();
                        UpdateColors();

                        if (bDesignerMode || DesignerProperties.GetIsInDesignMode(this))
                        {
                            DesignerProperties.SetIsInDesignMode(this, false); // this line is needed otherwise disposing docking throws an exception
                            bDesignerMode = true;

                            if (!bSettingMode)
                            {
                                spreadsheetControl1.ReadOnly = true;
                                ribbonControl1.IsEnabled = false;
                            }
                        }
                        else
                        {
                            if (RunningOnServer)
                            {
                                ShowRibbons = false;
                                spreadsheetControl1.ReadOnly = true;
                                DelayedSingleActionInvoker SizeChangedInvoker = new DelayedSingleActionInvoker(() =>
                                {
                                    spreadsheetControl1.GetVisualChildrenOfType<RepeatButton>().ToList().ForEach(x => x.IsEnabled = true);
                                }); 

                                if (SizeChangedInvoker != null)
                                    SizeChangedInvoker.BeginInvoke();
                            }

                            spreadsheetControl1.ReplaceService<DevExpress.XtraSpreadsheet.Services.IMessageBoxService>(new CustomMessageBoxService(spreadsheetControl1 as FrameworkElement, UIMsgBoxAlertService));

                            if (sb == null)
                            {
                                sb = this.FindResource("PathErrorOverlayFadeOutSB") as Storyboard;
                                sb.Completed += SbCompleted;
                                sb.Begin();
                            }
                            SubscribeItems();
                        }
                        bInit = true;
                    }
                };
        }

        private void ExecuteSpreadSheetCommand(object sender, RibbonCaptionButtonClickEventArgs e)
        {
            SpreadsheetUICommand cmd = null;
            if (sender == grpHomeFont)
                cmd = SpreadsheetUICommand.FormatCellsFont;
            else if (sender == grpHomeAlignment)
                cmd = SpreadsheetUICommand.FormatCellsAlignment;

            if (cmd != null && cmd.CanExecute(null))
                cmd.Execute(spreadsheetControl1);
        }

        private void UpdateColors()
        {
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue && Background != null)
            {
                BackgroundBorder.Background = Background;
            }
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
            {
                spreadsheetControl1.Foreground = Foreground;
            }
        }
        #endregion

        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InvokeAutomationPeer(this);
        }
        internal void SelectWorkSheet()
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            var _list = spreadsheetControl1.GetVisualChildrenOfType<DevExpress.Xpf.Spreadsheet.Internal.TabSelectorItem>().ToList();
            bool bToSelected = false;
            bool bSelected = false;
            foreach (DevExpress.Xpf.Spreadsheet.Internal.TabSelectorItem t in _list)
            {
                if (t.State == DevExpress.Xpf.Spreadsheet.Internal.TabSelectorItemState.Selected)
                {
                    bToSelected = true;
                    continue;
                }

                if (bToSelected)
                {
                    bSelected = true;
                    workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[t.Text];
                    break;
                }
            }
            if (!bSelected && _list.Count > 0)
                workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[_list[0].Text];
            workbook.Worksheets.ActiveWorksheet.SetSelectedRanges(new List<CellRange>());
        }
        #endregion

        void SbCompleted(object sender, EventArgs ea)
        {
            pathErrorOverlay.Visibility = Visibility.Collapsed;
        }

        void SubscribeItems()
        {
            if (SpreadSheetDataList == null || SpreadSheetDataList.Count == 0)
                return;
      
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            var workbook = spreadsheetControl1.Document;

            SetBusy(true);

            try 
            { 
                //SpreadSheetDataList.ForEach(item =>
                foreach (var key in OpcuaEntityReference.Keys)
                {
                    if (SpreadSheetDataList[key].Sheet < workbook.Worksheets.Count &&
                        OpcuaEntityReference[key] != null &&
                        OpcuaEntityReference[key].IsValid)
                    {
                        var sheet = workbook.Worksheets[SpreadSheetDataList[key].Sheet];
                        var cell = sheet.Cells[SpreadSheetDataList[key].CellName];

                        if (cell != null)
                        {
                            if (mapObserver == null)
                                mapObserver = new Dictionary<int, PropertyObserver<OPCUAEntityReference>>();

                            mapObserver[key] = new PropertyObserver<OPCUAEntityReference>(OpcuaEntityReference[key])
                            .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                            {
                                mapObserver[key].UnregisterHandler(p => p.MonitoredItemViewModel);

                                if (n.MonitoredItemViewModel != null)
                                {
                                    observerMonitoredModel[key] = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel)
                                    .RegisterHandler(m => m.LastMessage, m =>
                                    {
                                        Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                                        {
                                            SetEntityError(cell, m.LastMessage);
                                        });
                                    });

                                    observerMonitoredModel[key].RegisterHandler(m => m.DataValue, m =>
                                    {
                                        if (m.DataValue != null)
                                        {
                                            bool bExecute = false;
                                            lock (queuedValues)
                                            {
                                                bExecute = queuedValues.Count() == 0;
                                                queuedValues[key] = m.DataValue;

                                                if (bExecute)
                                                {
                                                    dpUpdateValue = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                                    {
                                                        Dictionary<int, Opc.Ua.DataValue> temporary;
                                                        lock (queuedValues)
                                                        {
                                                            temporary = new Dictionary<int, Opc.Ua.DataValue>(queuedValues);
                                                            queuedValues.Clear();
                                                        }

                                                        foreach (var k in temporary.Keys)
                                                        {
                                                            var currentCell = sheet.Cells[SpreadSheetDataList[k].CellName];
                                                            if (currentCell != null)
                                                                UpdateCellValue(currentCell, temporary[k]);
                                                        }
                                                    });
                                                }
                                            }
                                        }
                                    });

                                    var dataValue = n.MonitoredItemViewModel.DataValue;
                                    if (dataValue != null)
                                    {
                                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                        {
                                            UpdateCellValue(cell, dataValue);
                                        });
                                    }
                                }
                            });

                            if (OpcuaEntityReference[key].MonitoredItemViewModel != null)
                            {
                                var dataValue = OpcuaEntityReference[key].MonitoredItemViewModel.DataValue;
                                if (dataValue != null)
                                {
                                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                    {
                                        UpdateCellValue(cell, dataValue);
                                    });
                                }
                            }

                            mapObserver[key].RegisterHandler(n => n.NodeIdViewModel, n =>
                            {
                                // mapObserver[key].UnregisterHandler(p => p.NodeIdViewModel);
                                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                                {
                                    EvaluateAccessLevel(OpcuaEntityReference[key]);
                                });
                            });
                            //FogBugz 11581
                            if (doc != null && !string.IsNullOrEmpty(doc.SessionString))
                                OpcuaEntityReference[key].Resolve(doc.SessionString, doc);
                            else
                                OpcuaEntityReference[key].Resolve(Properties.Resources.SessionName, doc);

                            OpcuaEntityReference[key].SetInUse(this, true);
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
              SetBusy(false);
            }
        }

        void UpdateCellValue(Cell cell, Opc.Ua.DataValue dataValue)
        {
            if (Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) ||
                dataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
            {
                cell.Value = CellValue.TryCreateFromObject(dataValue.Value);
                SetEntityError(cell, null);
            }
            else
            {
                SetEntityError(cell, dataValue.StatusCode.ToString());
            }
        }

        public void EvaluateAccessLevel(OPCUAEntityReference opcuaEntityReference)
        {
            var fe = this as FrameworkElement;
            if (fe == null || opcuaEntityReference == null || opcuaEntityReference.NodeIdViewModel == null)
                return;

            if ((!opcuaEntityReference.NodeIdViewModel.IsUserWritable || !opcuaEntityReference.NodeIdViewModel.IsWritable)/* && fe.IsHitTestVisible*/)
            {
                // fe.IsHitTestVisible = false;
                fe.ToolTip = "Item not writeable";
            }
            if ((!opcuaEntityReference.NodeIdViewModel.IsUserReadable || !opcuaEntityReference.NodeIdViewModel.IsReadable) && fe.IsEnabled)
            {
                fe.IsEnabled = false;
                fe.ToolTip = "Item not readable";
            }
        }

        void SetBusy(bool bBusy)
        {
            if (bBusy)
            {
                spreadsheetControl1.IsEnabled = false;
                //busyContent.Text = Properties.Resources.WaitText;
                busyControl.Visibility = Visibility.Visible;
                //busyContent.Visibility = Visibility.Visible;
            }
            else
            {
                spreadsheetControl1.IsEnabled = true;
                busyControl.Visibility = Visibility.Collapsed;
                //busyContent.Visibility = Visibility.Collapsed;
            }
        }

        bool errorEffectOn;

        System.Windows.Media.Effects.Effect previousEffect;
        bool previousClipToBounds;
        private void SetEntityError(Cell cell, String error)
        {
            if (String.IsNullOrEmpty(error))
            {
                if (errorEffectOn)
                {
                    (spreadsheetControl1 as UIElement).Effect = previousEffect;
                    (spreadsheetControl1 as UIElement).ClipToBounds = previousClipToBounds;
                    //(this as UIElement).Opacity = 1;
                    previousEffect = null;
                    errorEffectOn = false;
                }
            }
            else
            {
                if (!errorEffectOn)
                {
                    errorEffectOn = true;
                    previousEffect = (spreadsheetControl1 as UIElement).Effect;
                    previousClipToBounds = (spreadsheetControl1 as UIElement).ClipToBounds;

                    var effect = new DropShadowEffect
                    {
                        ShadowDepth = 0,
                        BlurRadius = 10,
                        Color = Colors.Red
                    };
                    (spreadsheetControl1 as UIElement).Effect = effect;
                    (spreadsheetControl1 as UIElement).ClipToBounds = false;
                }
            }
        }


        private void spreadsheetControl1_CellEndEdit(object sender, DevExpress.XtraSpreadsheet.SpreadsheetCellValidatingEventArgs e)
        {
            var workbook = spreadsheetControl1.Document;
            var sheet = workbook.Worksheets[e.SheetName];
            Cell cell = (sender as SpreadsheetControl).ActiveCell;
            string _name = cell.GetReferenceA1();

            try
            {
                (from c in SpreadSheetDataList where c.Sheet == sheet.Index && c.CellName.Equals(_name) select SpreadSheetDataList.IndexOf(c)).ToList().ForEach(key =>
                    {
                       OpcuaEntityReference[key].MonitoredItemViewModel.WriteValue(e.EditorText);
                    });
            }
            catch
            { 
            
            }
        }

        void UnsubscribeItems()
        {
            if (observerMonitoredModel != null)
            {
                foreach (var observer in observerMonitoredModel.Values)
                {
                    observer.UnregisterHandler(p => p.LastMessage);
                    observer.UnregisterHandler(p => p.DataValue);
                    observer.Dispose();
                }
                observerMonitoredModel.Clear();
            }

            if (mapObserver != null)
            {
                foreach (var observer in mapObserver.Values)
                    observer.Dispose();
                mapObserver.Clear();
            }

            foreach (var item in OpcuaEntityReference.Values)
            {
                if (item != null && item.IsValid)
                    item.SetInUse(this, false);
            }

            OpcuaEntityReference.Clear();
        }

        #region IContainPropertyEditors Members

        [Browsable(false)]
        public Type ObjectType
        {
            get
            {
                return this.GetType();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                IDocument Document;
                IWorkspace Workspace;
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                Workspace = Document?.GetService(typeof(IWorkspace)) as IWorkspace;

                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();
                // Defines Data Template for 'DocumentPathProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(SourceFilePropertyEditor));
                factory.SetValue(SourceFilePropertyEditor.WorkspaceProperty, Workspace);
                factory.SetValue(SourceFilePropertyEditor.FilterProperty, Properties.Resources.FilterOption);
                factory.SetValue(SourceFilePropertyEditor.CopyOptionProperty, SourceFileCopyOption.Ask);
                factory.SetValue(SourceFilePropertyEditor.DefaultFolderProperty, SpecialFolders.Documents);
                factory.SetValue(SourceFilePropertyEditor.DefaultExtProperty, "xlsx");
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                mapDataTemplates.Add(DocumentPathProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.SmartPropertiesEditor));
                factory.SetValue(Controls.SmartPropertiesEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SmartPropertiesProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.EditLayoutPropertyEditor));
                factory.SetValue(Controls.EditLayoutPropertyEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditLayoutProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "DocumentPath")
                {
                    if (DocumentPath != null)
                    {
                        object pathCheck = CheckFilePath(DocumentPath);
                        if (bPathError)
                            pathErrorOverlay.Visibility = Visibility.Visible;
                    }
                }

                return null;
            }
        }

        #endregion

        #region IDisposable Members
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;

            bDisposed = true;
            DetachOverrideBaseProperties();

            if (sb != null)
            {
                sb.Completed -= SbCompleted;
                sb = null;
            }

            grpHomeFont.CaptionButtonClick -= ExecuteSpreadSheetCommand;
            grpHomeAlignment.CaptionButtonClick -= ExecuteSpreadSheetCommand;

            UnsubscribeItems();

            lock (queuedValues)
            {
                if (dpUpdateValue != null &&
                    dpUpdateValue.Status != DispatcherOperationStatus.Aborted &&
                    dpUpdateValue.Status != DispatcherOperationStatus.Completed)
                    dpUpdateValue.Abort();
                queuedValues.Clear();
            }
            barManager1.Items.Clear();
            barManager1.Dispose();
        }

        #endregion

        #region IEntityReference Members

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}
