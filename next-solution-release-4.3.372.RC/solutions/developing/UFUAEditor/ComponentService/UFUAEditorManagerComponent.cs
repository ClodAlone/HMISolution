using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Controls;
using UFUAEditor.Controls;
using OPCUABrowser.ComponentService;
using PropertyControl.ComponentService;
using UFUAEditor.PropertyDataTemplate;
using System.Windows.Threading;
using CommandManager.PropertyDataTemplate;
using UFUAEditor.Alarms;
using NewDriverWizard.ComponentService;
using WPFUtilities.Extensions;
using WPFUtilities.PropertyDataTemplate;
using DataReaderEditor.PropertyDataTemplate;
using CommonControls.PropertyDataTemplate;
using CommandExplorer.ComponentService;
using SelectionMode = UFInterfaces.Editors.SelectionMode;
using Utilities.WPF;
using UFUAEditor.TabHelper;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UriResolver.ComponentService;
using Utilities;
using UFUAEditor.Document;
using System.Collections.ObjectModel;
using StringManager.ComponentService;
using UFInterfaces.Editors;
using DevExpress.Xpo;
using XpoHelpers;
using UFUserEditor.ComponentService;
using WPFUtilities;
using UFUAModel;
using System.Text.RegularExpressions;
using OPCUAViewModel;
using log4net;
using DocumentManager.ComponentService.Helpers;
using System.Windows.Input;
using System.Threading.Tasks;
using Opc.Ua;
using UFUAAlarm;
#if !NET_STANDARD
using OPCUAViewModel.PropertyDataTemplate;
using HelpProvider.ComponentService;
using DataReaderEditor.Converters;
using DevExpress.Xpf.Core.Native;
using Mindscape.WpfElements.PropertyEditing;
using System.Diagnostics;
#endif
namespace UFUAEditor.ComponentService
{
    public class UFUAEditorManagerComponent : ComponentBase<IUFUAEditorManager>, IUFUAEditorManager, IDocumentManager, IDisposable
#if !NET_STANDARD
        , ICrossReference
#endif
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, UFUAServerDocument> mapActiveDocuments = new Dictionary<String, UFUAServerDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<String, UFUAServerDocument> mapActiveHiddenDocuments = new Dictionary<String, UFUAServerDocument>(StringComparer.OrdinalIgnoreCase);
        readonly List<UFUAServerDocument> listActiveHiddenDocumentsToRefresh = new List<UFUAServerDocument>();

        readonly Dictionary<UFUAServerDocument, String> mapActiveDocumentUris = new Dictionary<UFUAServerDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        readonly Dictionary<String, UFUAServerDocument> mapRunningDocuments = new Dictionary<String, UFUAServerDocument>(StringComparer.OrdinalIgnoreCase);
#if !NET_STANDARD
        readonly Dictionary<String, AlarmCommandsEngine> mapRunningAlarmCommandsEngine = new Dictionary<String, AlarmCommandsEngine>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<UFUAServerDocument, UFUAEditorControl> mapActiveDocumentView = new Dictionary<UFUAServerDocument, UFUAEditorControl>();

        readonly Dictionary<String, ObservableCollection<IDocumentManager>> mapChildDocumentManagers = new Dictionary<String, ObservableCollection<IDocumentManager>>();
        MenuControl menuControl;
        List<CommandBinding> globalCbs;
        bool bToolbarInitialized;
        static readonly internal Dictionary<ControlTabEnum, string> TabsScheme = new Dictionary<ControlTabEnum, string>()
        {
            { ControlTabEnum.Prototypes, Properties.Settings.Default.TreePrototypeTypeScheme },
            { ControlTabEnum.AddressSpace, Properties.Settings.Default.AddressSpaceTypeScheme },
            { ControlTabEnum.GeneralSettings, Properties.Settings.Default.GeneralSettingsTypeScheme },
            { ControlTabEnum.Drivers, Properties.Settings.Default.DriversTypeScheme },
            { ControlTabEnum.Alarms, Properties.Settings.Default.AlarmsTypeScheme },
            { ControlTabEnum.Historicals, Properties.Settings.Default.HistoricalTypeScheme },
            { ControlTabEnum.DataLoggers, Properties.Settings.Default.DataLoggerTypeScheme },
            { ControlTabEnum.EngineeringUnits, Properties.Settings.Default.EngineeringUnitsTypeScheme },
            { ControlTabEnum.Views, Properties.Settings.Default.ViewsTypeScheme },
            { ControlTabEnum.Redundancy, Properties.Settings.Default.RedundancyTypeScheme }
        };
#else
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif

        public static UFUAEditorManagerComponent ufuaEditorManagerComponent { get; protected set; }

        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (ufuaEditorManagerComponent == null)
                ufuaEditorManagerComponent = this;

            GetComponentInterfaces();
        }

        #endregion IUFInterfaceBase Members
#if !NET_STANDARD
        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, image, bShared);
            return bm;
        }

        internal static BitmapImage GetTagBitmapImage(UFUAModel.UFUATag tag)
        {
            BitmapImage bmpImage = null;
            string bitmapName = GetTagBitmapImageName(tag);
            if (!string.IsNullOrEmpty(bitmapName))
                bmpImage = UFUAEditorManagerComponent.GetBitmapImage(bitmapName);
            return bmpImage;
        }

        internal static string GetTagBitmapImageName(UFUAModel.UFUATag tag)
        {
            string bmpImageName = null;
            switch (tag.ModelType)
            {
                case UFUAModel.ModelType.Method:
                    bmpImageName = "UFUASMethodSmall";
                    break;
                case UFUAModel.ModelType.ObjectType:
                    bmpImageName = "UFUASVariableTypeSmall";
                    break;
            }

            if (string.IsNullOrEmpty(bmpImageName))
            {
                switch (tag.DataType)
                {
                    case UFUAModel.DataType.Boolean:
                        bmpImageName = "UFUASVariableBooleanSmall";
                        break;
                    case UFUAModel.DataType.SByte:
                        bmpImageName = "UFUASVariableSByteSmall";
                        break;
                    case UFUAModel.DataType.Byte:
                        bmpImageName = "UFUASVariableByteSmall";
                        break;
                    case UFUAModel.DataType.Int16:
                        bmpImageName = "UFUASVariableInt16Small";
                        break;
                    case UFUAModel.DataType.UInt16:
                        bmpImageName = "UFUASVariableUInt16Small";
                        break;
                    case UFUAModel.DataType.Int32:
                        bmpImageName = "UFUASVariableInt32Small";
                        break;
                    case UFUAModel.DataType.UInt32:
                        bmpImageName = "UFUASVariableUInt32Small";
                        break;
                    case UFUAModel.DataType.Int64:
                        bmpImageName = "UFUASVariableInt64Small";
                        break;
                    case UFUAModel.DataType.UInt64:
                        bmpImageName = "UFUASVariableUInt64Small";
                        break;
                    case UFUAModel.DataType.Float:
                        bmpImageName = "UFUASVariableFloatSmall";
                        break;
                    case UFUAModel.DataType.Double:
                        bmpImageName = "UFUASVariableDoubleSmall";
                        break;
                    case UFUAModel.DataType.String:
                        bmpImageName = "UFUASVariableStringSmall";
                        break;
                    default:
                        bmpImageName = "UFUASVariableSmall";
                        break;
                }
            }

            return bmpImageName;
        }

        public bool TempVarLoaded { get; protected set; }
#endif

        private void GetComponentInterfaces()
        {
#if !NET_STANDARD
            Assembly assembly = Assembly.GetExecutingAssembly();
            var ext = Properties.Settings.Default.DefaultFileExt.ToLower().Replace(".", "");
            ApplicationPropertiesHelper.SetProperty(ext, Path.GetFileNameWithoutExtension(assembly.Location));
            var list = FindAndLoadDLL.LoadDLLs<DataSinkInterface>(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetAssembly(typeof(Utilities.XmlHelper)).Location), "DataSinks"),
                                                        "TempVariables.dll", false);
            if (list.Count > 0)
            {
                TempVarLoaded = true;
            }

            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            workspace.Closing += workspace_Closing;
            workspace.Closed += workspace_Closed;
            workspace.CloseButtonClick += workspace_CloseButtonClick;
            workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
            workspace.PromptFriendObjects += workspace_PromptFriendObjects;
            workspace.PromptDocumentEditorObject += workspace_PromptDocumentEditorObject;
            workspace.PromptSmartTagsEditorObject += workspace_PromptSmartTagsEditorObject;
            workspace.EasyModeChanged += workspace_EasyModeChanged;

            if (PropertyControl != null)
            {
                // Configuration Data Templates
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
                factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, UIInterface);
                factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, HelpProvider);
                factory.SetValue(ConnectionSourcePropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("HistorianDefaultConnection", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);
                PropertyControl.AddPropertyEditor("EventDefaultConnection", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);
                PropertyControl.AddPropertyEditor("AuditTraceDefaultConnection", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);
                PropertyControl.AddPropertyEditor("ConnectionSettings", typeof(String), typeof(UFUAModel.UFUAHistorianSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.BytesModeProperty, true);
                dt.DataType = typeof(Int64);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("MaxHistoryTotalSafelyFilesSize", typeof(Nullable<Int64>), typeof(UFUAModel.ConfigurationBase), dt);

                // Address Space Data Templates
                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TagEntityReferencePropertyEditor));
                dt.DataType = typeof(UFUAModel.TagEntityReference);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor(typeof(UFUAModel.TagEntityReference), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(ListTagEntityReferencePropertyEditor));
                dt.DataType = typeof(XPCollection<UFUAModel.XPTagEntityReference>);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor(typeof(XPCollection<UFUAModel.XPTagEntityReference>), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PrototypeModelPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("PrototypeName", typeof(string), typeof(UFUAModel.UFUATag), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(EngineeringUnitPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("UFUAEngineeringUnit", typeof(string), typeof(UFUAModel.UFUATag), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(HistorianSettingsPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("HistorianSettings", typeof(string), typeof(UFUAModel.UFUATag), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(EnumStringsPropertyEditor));
                factory.SetValue(EnumStringsPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(UFUAModel.Helpers.CustomXPCollection<UFUAModel.UFUAEnumString>);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("EnumStrings", typeof(UFUAModel.Helpers.CustomXPCollection<UFUAModel.UFUAEnumString>), typeof(UFUAModel.UFUATag), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(DynamicSettingsPropertyEditor));
                factory.SetValue(DynamicSettingsPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("DynamicSettings", typeof(string), typeof(UFUAModel.UFUATag), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(DriverDynamicSettingsPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor(typeof(UFUAModel.DriverDynamicSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("UserReadAccessMask", typeof(int), typeof(UFUAModel.UFUATag), dt);
                PropertyControl.AddPropertyEditor("UserWriteAccessMask", typeof(int), typeof(UFUAModel.UFUATag), dt);
                PropertyControl.AddPropertyEditor("UserReadAccessMask", typeof(int), typeof(UFUAModel.UFUAAlarmDefinition), dt);
                PropertyControl.AddPropertyEditor("UserWriteAccessMask", typeof(int), typeof(UFUAModel.UFUAAlarmDefinition), dt);

                // Alarms Data Templates
                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TimeSpanPropertyEditor));
                dt.DataType = typeof(TimeSpan);
                factory.SetValue(TimeSpanPropertyEditor.TimeSpanFormatProperty, String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart));
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("TimeUnit", typeof(TimeSpan), typeof(UFUAModel.UFUAAlarmDefinition), dt);
                PropertyControl.AddPropertyEditor("DelayTimeOn", typeof(TimeSpan), typeof(UFUAModel.UFUAAlarmDefinition), dt);
                PropertyControl.AddPropertyEditor("DelayTimeOff", typeof(TimeSpan), typeof(UFUAModel.UFUAAlarmDefinition), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, Double.MinValue);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 0.5);
                factory.SetValue(NumericUpDownPropertyEditor.WorkspaceProperty, workspace);
                factory.SetValue(NumericUpDownPropertyEditor.ValidationNamesProperty, new String[] { "ActivationValue" });
                dt.DataType = typeof(Double);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ActivationLowValue", typeof(Nullable<Double>), typeof(UFUAModel.UFUAAlarmDefinition), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, Double.MinValue);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 0.5);
                factory.SetValue(NumericUpDownPropertyEditor.WorkspaceProperty, workspace);
                factory.SetValue(NumericUpDownPropertyEditor.ValidationNamesProperty, new String[] { "ActivationLowValue" });
                dt.DataType = typeof(Double);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ActivationValue", typeof(Nullable<Double>), typeof(UFUAModel.UFUAAlarmDefinition), dt);

                // Data Loggers and Historian Templates
                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TimeSpanPropertyEditor));
                factory.SetValue(TimeSpanPropertyEditor.TimeSpanFormatProperty, String.Format("d '({0})' hh:mm:ss.fff", Properties.Resources.TimeSpanFormatDaysPart));
                dt.DataType = typeof(TimeSpan);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("RecordingTimeInterval", typeof(TimeSpan), typeof(DataLoggerModel.DataLoggerSettings), dt);
                PropertyControl.AddPropertyEditor("MinTimeInterval", typeof(TimeSpan), typeof(UFUAModel.UFUAHistorianSettings), dt);
                PropertyControl.AddPropertyEditor("MaxTimeInterval", typeof(TimeSpan), typeof(UFUAModel.UFUAHistorianSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TimeSpanPropertyEditor));
                factory.SetValue(TimeSpanPropertyEditor.TimeSpanFormatProperty, "hh:mm:ss");
                dt.DataType = typeof(TimeSpan);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("HysteresisTimeInterval", typeof(TimeSpan), typeof(DataLoggerModel.DataLoggerSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
                dt.DataType = typeof(Double);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ExceptionDeviation", typeof(Double), typeof(UFUAModel.UFUAHistorianSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(DataReaderModelPropertyEditor));
                factory.SetValue(DataReaderModelPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(DataReader.DataReaderModel);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ConnectionSettings", typeof(DataReader.DataReaderModel), typeof(DataLoggerModel.DataLoggerSettings), dt);

                //dt = new DataTemplate();
                //factory = new FrameworkElementFactory(typeof(SelectPathPropertyEditor));
                //dt.DataType = typeof(String);
                //dt.VisualTree = factory;
                //PropertyControl.AddPropertyEditor("FlushDataSafelyPath", typeof(String), typeof(DataLoggerModel.DataLoggerSettings), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TimePropertyEditor));
                dt.DataType = typeof(DateTime);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("RedundancyFullSynchronizationStartTime", typeof(DateTime), typeof(UFUAModel.ConfigurationBase), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TimeSpanPropertyEditor));
                factory.SetValue(TimeSpanPropertyEditor.TimeSpanFormatProperty, String.Format("d '({0})' hh:mm:ss.fff", Properties.Resources.TimeSpanFormatDaysPart));
                dt.DataType = typeof(TimeSpan);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("RedundancyFullSynchronizationTimeSpan", typeof(TimeSpan), typeof(UFUAModel.ConfigurationBase), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TimeSpanPropertyEditor));
                factory.SetValue(TimeSpanPropertyEditor.TimeSpanFormatProperty, String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart));
                dt.DataType = typeof(TimeSpan);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("RedundancyStartupTimeout", typeof(TimeSpan), typeof(UFUAModel.ConfigurationBase), dt);
                PropertyControl.AddPropertyEditor("RedundancySynchronizeTimeout", typeof(TimeSpan), typeof(UFUAModel.ConfigurationBase), dt);
                PropertyControl.AddPropertyEditor("RedundancyTimeout", typeof(TimeSpan), typeof(UFUAModel.ConfigurationBase), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(MaskedTextBoxPropertyEditor));
                factory.SetValue(MaskedTextBoxPropertyEditor.MaskProperty, MaskedTextBoxPropertyEditor.DefaultIpMask);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("RedundancyUdpServiceIpAddress", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);
                PropertyControl.AddPropertyEditor("RedundancyUdpClientIpAddress", typeof(string), typeof(UFUAModel.ConfigurationBase), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("AlarmText", typeof(string), typeof(UFUAAlarmThreshold), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.CommandExplorerPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("CommandsOn", typeof(string), typeof(UFUAAlarmThreshold), dt);
                PropertyControl.AddPropertyEditor("CommandsOff", typeof(string), typeof(UFUAAlarmThreshold), dt);
                PropertyControl.AddPropertyEditor("CommandsAck", typeof(string), typeof(UFUAAlarmThreshold), dt);
                PropertyControl.AddPropertyEditor("CommandsReset", typeof(string), typeof(UFUAAlarmThreshold), dt);
                PropertyControl.AddPropertyEditor("CommandsDbClick", typeof(string), typeof(UFUAAlarmThreshold), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(ExpressionPropertyEditor));
                factory.SetValue(ExpressionPropertyEditor.WorkspaceProperty, workspace);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Expression", typeof(String), typeof(UFUAAlarmThreshold), dt);
                PropertyControl.AddPropertyEditor("Expression", typeof(String), typeof(DataLoggerModel.DataLoggerColumn), dt);
                PropertyControl.AddPropertyEditor("SeverityExpression", typeof(String), typeof(UFUAAlarmThreshold), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(SourceFilePropertyEditor));
                factory.SetValue(SourceFilePropertyEditor.WorkspaceProperty, Workspace);
                factory.SetValue(SourceFilePropertyEditor.FilterProperty, Properties.Resources.SoundFileFilterOption);
                factory.SetValue(SourceFilePropertyEditor.CopyOptionProperty, SourceFileCopyOption.Ask);
                factory.SetValue(SourceFilePropertyEditor.DefaultFolderProperty, SpecialFolders.Documents);
                factory.SetValue(SourceFilePropertyEditor.DefaultExtProperty, "wav");
                factory.SetValue(SourceFilePropertyEditor.UseUriProperty, false);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("SoundFile", typeof(String), typeof(UFUAAlarmDefinition), dt);
            }
#endif
        }

#if !NET_STANDARD
        internal void RefreshHiddenDocuments(IDocument parent)
        {
            var l = (from c in mapActiveHiddenDocuments/*.AsParallel()*/
                     where c.Value.Parent == parent
                     select c).ToList();
            l.ForEach(document =>
            {
                //DisposeDocument(document.Value);
                //mapActiveHiddenDocuments.Remove(document.Key);
                if (!listActiveHiddenDocumentsToRefresh.Contains(document.Value))
                    listActiveHiddenDocumentsToRefresh.Add(document.Value);

                GetChildDocumentManagers(document.Value, document.Key, false);
            });
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is UFUAEditorControl))
                return;

            UFUAEditorControl view = e.TargetItem as UFUAEditorControl;
            if (!CloseView(view, bCloseDoc: false, bSave: true))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(UFUAEditorControl view, bool bSave = true)
        {
            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (!view.Document.CanClose())
                    return false;

                if (bSave && view.Document.NeedsSave)
                {
                    if (UIInterface != null)
                    {
                        var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                            String.Format("{0} ({1})", TypeTitle, view.Document.Parent.Title)), CustomDialogIcons.Question);
                        if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                            return false;
                        bSave = res == CustomDialogResults.Yes;
                    }

                    if (bSave)
                    {
                        if (view.Document.NeedsSave && !view.Document.SaveToFile())
                            return false;
                    }
                }
            }

            return true;
        }

        private bool CloseView(UFUAEditorControl view, bool bSave = true, bool bCloseDoc = true)
        {
            if (view == null)
                return true;

            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (!CanClose(view, bSave))
                {
                    return false;
                }

                if (!bCloseDoc)
                {
                    bCloseDoc = view.Document.NeedsSave;
                }

                if (bCloseDoc)
                {
                    CloseAllChild(view.Document, bParentClosing: bCloseDoc);

                    mapActiveDocumentUris.Remove(view.Document);
                    mapActiveDocumentView[view.Document].Dispose();
                    mapActiveDocumentView.Remove(view.Document);
                    mapAddressSpaceControls.Remove(view.Document);
                    mapPrototypesControls.Remove(view.Document);
                    mapActiveDocuments.Remove(uri);
                    mapActiveDocumentTitles.Remove(uri);
                }
                else
                {
                    view.Document.ActiveView = null;
                    workspace.ContextObject = null;
                }
            }

            if (workspace.ContextDocument == view.Document)
                workspace.ContextDocument = null;

            view.Document.PropertyChanged -= Document_PropertyChanged;
            view.OnDeactivate();

            workspace.RemoveDockingChildren(view);

            if (bCloseDoc)
            {
                view.Document.SaveToFile(true);
                view.Document.DisposeInApplicationIdle();
            }

            return true;
        }
#endif

        void DisposeDocument(UFUAServerDocument doc)
        {
            RemoveActiveDocumentUri(doc);
            if (doc is IDisposable)
                (doc as IDisposable).Dispose();
        }

#if !NET_STANDARD
        void DisposeDocumentInApplicationIdle(UFUAServerDocument doc)
        {
            RemoveActiveDocumentUri(doc);
            doc.DisposeInApplicationIdle();
        }
#endif

        void RemoveActiveDocumentUri(UFUAServerDocument doc)
        {
            String uri;
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                mapActiveDocumentUris.Remove(doc);
#if !NET_STANDARD
                mapAddressSpaceControls.Remove(doc);
                mapPrototypesControls.Remove(doc);
                mapActiveDocumentView[doc].Dispose();
                mapActiveDocumentView.Remove(doc);
                mapChildDocumentManagers.Remove(uri);
#endif
                mapActiveDocuments.Remove(uri);
                mapActiveDocumentTitles.Remove(uri);
            }
            else
            {
#if !NET_STANDARD
                mapAddressSpaceControls.Remove(doc);
                mapPrototypesControls.Remove(doc);
#endif
                var listuri = (from c in mapActiveDocuments where c.Value == doc select c.Key).ToList();
                listuri.ForEach(u =>
                {
#if !NET_STANDARD
                    mapChildDocumentManagers.Remove(u);
#endif
                    mapActiveDocuments.Remove(u);
                    mapActiveDocumentTitles.Remove(u);
                });
            }
        }

#if !NET_STANDARD
        void workspace_Closed(object sender, EventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            UFUAServerDocument[] array = new UFUAServerDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                UFUAEditorControl view = doc.ActiveView as UFUAEditorControl;
                if (view != null)
                    CloseView(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            UFUAServerDocument[] array = new UFUAServerDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                UFUAEditorControl view = doc.ActiveView as UFUAEditorControl;
                if (view != null && !CanClose(view))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        void GetGlobalCbs(UFUAEditorControl view)
        {
            var globalCmds = (from DevExpress.Xpf.Bars.BarItem bi in menuControl.menuGeneralItems.GetChildrenOfType<DevExpress.Xpf.Bars.BarItem>() where bi.Command != null select bi.Command).ToList();
            globalCbs = (from CommandBinding cb in view.CommandBindings where globalCmds.Contains(cb.Command) select cb).ToList();
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewOld = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var viewNew = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is UFUAEditorControl && viewOld != null)
            {
                var view = e.OldValue as UFUAEditorControl;
                //workspace.RemoveBarManagerCommands(view.CommandBindings);
                //if (workspace.ContextDocument == view.Document)
                //    workspace.ContextDocument = null;

                if (globalCbs == null)
                    GetGlobalCbs(view);
                Workspace.RemoveBarManagerCommands(new CommandBindingCollection(globalCbs));

                view.OnDeactivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue != null && e.NewValue is UFUAEditorControl && viewNew != null)
            {
                var view = e.NewValue as UFUAEditorControl;
                workspace.ContextDocument = view.Document;

                if (globalCbs == null)
                    GetGlobalCbs(view);
                Workspace.AddBarManagerGlobalCommands(new CommandBindingCollection(globalCbs));

                view.OnActivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && view.IsLoaded)
                {
                    view.Visibility = Visibility.Visible;
                }
            }
        }

        void workspace_PromptFriendObjects(object sender, GetFriendObjectsEventArgs e)
        {
            if (!(sender is UFUAModel.UFUAAlarmThreshold))
                return;
            var source = sender as UFUAAlarmThreshold;
            if (source.UFUATagAss != null && source.UFUATagAss.IsSharedMember)
                return;

            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.ActiveView == null || Doc.UowContext == null || !XpoHelper.IsSessionObject(source, Doc.UowContext))
                    continue;

                if (Doc.GetFriendObjects(source, e))
                    break;
            }
        }

        void UFUAEditorManagerComponent_SelectionTabChanged(object sender, UFUAEditor.TabHelper.ControlTabChangedEventArgs e)
        {

        }

        void workspace_PromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            if (!(sender is IXPSimpleObject))
                return;

            IXPSimpleObject source = sender as IXPSimpleObject;
            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.ActiveView == null)
                    continue;

                if (Doc.UowContext != null && XpoHelper.IsSessionObject(source, Doc.UowContext))
                {
                    e.documentEditor = Doc.ActiveView;
                    break;
                }
            }
        }

        void workspace_PromptSmartTagsEditorObject(object sender, GetSmartTagsEditorObjectEventArgs e)
        {
            if (!(sender is IXPSimpleObject))
                return;

            IXPSimpleObject source = sender as IXPSimpleObject;
            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (!XpoHelper.IsSessionObject(source, Doc.GetSession()))
                    continue;

                e.smartTagsEditor = Doc.GetSmartTagsEditorObject(source);
                break;
            }
        }

        void workspace_EasyModeChanged(object sender, EventArgs e)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.ActiveView != null
                        select c).ToList();

            foreach (var document in list)
            {
                var ui = document.ActiveView as UFUAEditorControl;
                if (ui != null)
                {
                    if (workspace.IsInEasyMode)
                        ui.HideComponents();
                    else
                        ui.RestoreComponents();
                }
            }
        }

        String GetDocumentTitle(Uri uri)
        {
            lock (lockObject)
            {
                String ret = Path.GetFileNameWithoutExtension(uri.GetPathString());
                String sourcefmt = ret;
                int i = 1;
                while (mapActiveDocumentTitles.ContainsValue(ret))
                    ret = String.Format("{0}{1}", sourcefmt, i++);

                mapActiveDocumentTitles.Add(uri.GetPathString(), ret);

                return ret;
            }
        }

        internal UFUAEditorControl GetViewFromUri(Uri uri)
        {
            UFUAServerDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                return doc.ActiveView as UFUAEditorControl;
            }

            return null;
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, UFUAServerDocument> keyvaluepair in mapActiveDocuments)
                {
                    if (keyvaluepair.Value != sender || keyvaluepair.Value.ActiveView == null)
                        continue;

                    workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                    break;
                }
            }
        }

        internal String GetDocumentTitle(String uri)
        {
            return Path.GetFileNameWithoutExtension(uri);
        }

        internal void CreateDefaultDocument(Uri uri, bool encryptFile = false)
        {
            //UFUAServerDocument newProject = new UFUAServerDocument(this);
            //newProject.ProjectPath = uri.GetPathString();
            //if (newProject.SaveToFile())
            //    newProject.CreateProjectFolders();
        }

        internal UFUAServerDocument GetContextServerDocument()
        {
            if (Workspace.ContextDocument is UFUAServerDocument)
                return Workspace.ContextDocument as UFUAServerDocument;

            var source = Workspace.ContextObject as IXPSimpleObject;
            if (source == null && Workspace.ContextObjects != null && Workspace.ContextObjects.Count > 0)
                source = Workspace.ContextObjects[0] as IXPSimpleObject;
            if (source != null)
            {
                foreach (var doc in mapActiveDocuments.Values)
                {
                    if (doc.UowContext != null && XpoHelper.IsSessionObject(source, doc.UowContext) ||
                        doc.GetSession() != null && XpoHelper.IsSessionObject(source, doc.GetSession()))
                    {
                        return doc;
                    }
                }
            }

            return null;
        }

        internal IList<T> GetSelectedObjects<T>()
        {
            var ret = new List<T>();
            if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.PropertyControl != null)
            {
                if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.PropertyControl.SelectObject != null &&
                                UFUAEditorManagerComponent.ufuaEditorManagerComponent.PropertyControl.SelectObject is T)
                {
                    ret.Add((T)UFUAEditorManagerComponent.ufuaEditorManagerComponent.PropertyControl.SelectObject);
                }
                else if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.PropertyControl.SelectObjects != null)
                {
                    ret.AddRange(UFUAEditorManagerComponent.ufuaEditorManagerComponent.PropertyControl.SelectObjects.OfType<T>());
                }
            }

            return ret;
        }
#endif

        #region Properties
#if !NET_STANDARD
        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                return workspace;
            }
        }

        IHelpProvider helpProvider;
        public IHelpProvider HelpProvider
        {
            get
            {
                if (helpProvider == null)
                    helpProvider = GetService(typeof(IHelpProvider)) as IHelpProvider;
                return helpProvider;
            }
        }

        IPropertyControl propertyControl;
        public IPropertyControl PropertyControl
        {
            get
            {
                if (propertyControl == null)
                    propertyControl = GetService(typeof(IPropertyControl)) as IPropertyControl;
                return propertyControl;
            }
        }
#endif

        IUriRisolver uriRisolver;
        public IUriRisolver UriRisolver
        {
            get
            {
                if (uriRisolver == null)
                    uriRisolver = GetService(typeof(IUriRisolver)) as IUriRisolver;
                return uriRisolver;
            }
        }

#if !NET_STANDARD
        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        IStringEditorManager stringEditor;
        public IStringEditorManager StringEditor
        {
            get
            {
                if (stringEditor == null)
                    stringEditor = GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return stringEditor;
            }
        }

        IUFUserEditorManager userEditor;
        public IUFUserEditorManager UserEditor
        {
            get
            {
                if (userEditor == null)
                    userEditor = GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                return userEditor;
            }
        }

        IOPCUABrowser opcuaBrowser;
        public IOPCUABrowser opcUABrowser
        {
            get
            {
                if (opcuaBrowser == null)
                    opcuaBrowser = GetService(typeof(IOPCUABrowser)) as IOPCUABrowser;
                return opcuaBrowser;
            }
        }

        ICommandExplorer commandExplorer;
        public ICommandExplorer CommandExplorer
        {
            get
            {
                if (commandExplorer == null)
                    commandExplorer = GetService(typeof(ICommandExplorer)) as ICommandExplorer;
                return commandExplorer;
            }
        }

        IUFNewDriverWizard newDriverWizard;
        public IUFNewDriverWizard NewDriverWizard
        {
            get
            {
                if (newDriverWizard == null)
                    newDriverWizard = GetService(typeof(IUFNewDriverWizard)) as IUFNewDriverWizard;
                return newDriverWizard;
            }
        }
#endif
        #endregion Properties

        #region IDocumentManager Members
#if !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UFUAServerDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseView(doc.ActiveView as UFUAEditorControl))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = UFUAServerDocument.FromFile(uri.GetPathString(), this, parent);

                    if (doc != null && doc.ActiveView == null)
                    {
                        UFUAEditorControl ufuaserverEditor = null;
                        if (mapActiveDocumentView.ContainsKey(doc))
                            ufuaserverEditor = mapActiveDocumentView[doc] as UFUAEditorControl;
                        if (ufuaserverEditor == null)
                            ufuaserverEditor = new UFUAEditorControl(doc);
                        doc.Parent = parent;
                        doc.ActiveView = ufuaserverEditor;
                        //if (doc.NeedsSave)
                        //    doc.SaveToFile();

                        workspace.SetDesiredHeightAndWidthInDockedMode(ufuaserverEditor, ufuaserverEditor.Height, ufuaserverEditor.Width);
                        ufuaserverEditor.ClearValue(FrameworkElement.WidthProperty);
                        ufuaserverEditor.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = GetBitmapImage("UFUASEditorSmall");
                        //workspace.SetDockedElementIcon(ufuaserverEditor, new ImageBrush(bm));

                        if (!mapActiveDocuments.ContainsKey(uri.GetPathString()))
                            mapActiveDocuments.Add(uri.GetPathString(), doc);

                        if (!mapActiveDocumentUris.ContainsKey(doc))
                            mapActiveDocumentUris.Add(doc, uri.GetPathString());

                        if (!mapActiveDocumentView.ContainsKey(doc))
                            mapActiveDocumentView.Add(doc, ufuaserverEditor);

                        workspace.AddDockingChildren(ufuaserverEditor,
                            String.Format("{0} ({1})", TypeTitle, parent.Title), UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                        workspace.SetDockedElementIcon(ufuaserverEditor, new ImageBrush(bm));
                        workspace.FlashDockedElement(ufuaserverEditor);

                        ufuaserverEditor.Document.PropertyChanged += Document_PropertyChanged;

                        if (doc.NeedsSave)
                            workspace.SetChangedDocumentTitle(doc.ActiveView, true);
                    }
                }
            }
        }
#endif

        public void PreTerminate(Uri uri, IDocument parent)
        {
#if !NET_STANDARD
            AlarmCommandsEngine engine = null;
            if (mapRunningAlarmCommandsEngine.TryGetValue(uri.GetPathString(), out engine))
            {
                mapRunningAlarmCommandsEngine.Remove(uri.GetPathString());
                engine.Dispose();
            }
#endif
        }

        public void Terminate(Uri uri, IDocument parent)
        {
            UFUAServerDocument doc = null;
            if (mapRunningDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                if (listStartedDocuments.Contains(doc))
                {
                    listStartedDocuments.Remove(doc);
#if !NET_STANDARD
                    if (!doc.ServerCMSHelperSync.IsServerRunningAsService)
                        doc.ServerCMSHelperSync.StopServer();
#endif
                }
                mapRunningDocuments.Remove(uri.GetPathString());
                doc.Dispose();
            }

            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            lock (mapCacheAlarmCommandsOnDbClick)
            {
                if (mapCacheAlarmCommandsOnDbClick.ContainsKey(p))
                    mapCacheAlarmCommandsOnDbClick.Remove(p);
            }

            lock (mapCacheEntityReference)
            {
                if (mapCacheEntityReference.ContainsKey(p))
                    mapCacheEntityReference.Remove(p);
            }

#if!NET_STANDARD
            lock (mapCacheAllProjectDocuments)
            {
                var appname = GetAplicationName(p);
                if (mapCacheAllProjectDocuments.ContainsKey(appname))
                    mapCacheAllProjectDocuments.Remove(appname);
            }
#endif
        }

#if !NET_STANDARD
        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values/*.AsParallel()*/
                        where c.Parent == parent/* && c.ActiveView != null */
                        select c).ToList();

            list.ForEach(document =>
            {
                document.SaveToFile();
            });
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values/*.AsParallel()*/
                        where c.Parent == parent && c.ActiveView != null
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseView(document.ActiveView as UFUAEditorControl, bCloseDoc: bParentClosing))
                    return false;
            }

            if (bParentClosing)
            {
                list = (from c in mapActiveDocuments.Values/*.AsParallel()*/
                        where c.Parent == parent && c.ActiveView == null
                        select c).ToList();
                foreach (var document in list)
                {
                    if (document.NeedsSave)
                    {
                        bool bSave = true;
                        if (UIInterface != null)
                        {
                            var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                                String.Format("{0} ({1})", TypeTitle, document.Parent.Title)), CustomDialogIcons.Question);
                            if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                                return false;
                            bSave = res == CustomDialogResults.Yes;
                        }

                        if (bSave)
                        {
                            if (document.NeedsSave && !document.SaveToFile())
                                return false;
                        }
                    }

                    document.SaveToFile(true);
                    DisposeDocumentInApplicationIdle(document);
                }

                var l = (from c in mapActiveHiddenDocuments/*.AsParallel()*/
                         where c.Value.Parent == parent
                         select c).ToList();
                l.ForEach(document =>
                {
                    DisposeDocumentInApplicationIdle(document.Value);
                    mapActiveHiddenDocuments.Remove(document.Key);
                    if (listActiveHiddenDocumentsToRefresh.Contains(document.Value))
                        listActiveHiddenDocumentsToRefresh.Remove(document.Value);
                });
            }

            return true;
        }

        public bool IsAnyChildNeedsSave(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent/* && c.ActiveView != null*/
                        select c).ToList();
            foreach (var document in list)
            {
                if (document.NeedsSave)
                    return true;
            }

            return false;
        }
#endif

        readonly List<UFUAServerDocument> listStartedDocuments = new List<UFUAServerDocument>();
        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            UFUAServerDocument doc = null;
            if (!mapRunningDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = UFUAServerDocument.FromFile(uri.GetPathString(), this, parent, bCreateNew: false);
                if (doc == null)
                    return;
                doc.Parent = parent;
                mapRunningDocuments.Add(uri.GetPathString(), doc);
                //mapActiveDocumentUris.Add(doc, uri.GetPathString());
            }
            else if (doc.IsEmpty)
                return;

#if !NET_STANDARD
            var documentPath = doc.Parent.GetSpecialFolder(SpecialFolders.Documents);

            if (documentPath.IsAbsoluteUri)
                SoundState.SetSoundFilePath(documentPath.AbsolutePath);
            else
                SoundState.SetSoundFilePath(doc.ConnectionString, documentPath.OriginalString);

            var thresholds = doc.GetAlarmThresholds()
                .Where(c => c.Enabled.HasValue && c.Enabled.Value && c.HasCommands)
                .ToList();

            var fileSoundThresholds = doc.GetAlarmThresholds()
                .Where(x => x.Enabled.HasValue && x.Enabled.Value && !string.IsNullOrWhiteSpace(x.SoundFile))
                .ToList();


            if (thresholds.Any() || fileSoundThresholds.Any())
            {
                var alarmCommandsEngine = new AlarmCommandsEngine(Dispatcher.CurrentDispatcher, this, parent);
                mapRunningAlarmCommandsEngine.Add(uri.GetPathString(), alarmCommandsEngine);

                foreach (var threshold in thresholds)
                {
                    var conditionName = threshold.GetUniqueConditionName();
                    if (conditionName != null)
                        alarmCommandsEngine.Add(conditionName, threshold);
                }

                foreach (var fileSoundThreshold in fileSoundThresholds)
                {
                    var conditionName = fileSoundThreshold.GetUniqueConditionName();
                    if (conditionName == null) continue;

                    var alarmStatus = new AlarmStatus
                    {
                        Name = fileSoundThreshold.Name,
                        SoundFile = fileSoundThreshold.SoundFile,
                        RepeatSoundContinuously = fileSoundThreshold.RepeatSoundContinuously,
                        Severity = (EventSeverity)fileSoundThreshold.Severity
                    };
                    alarmCommandsEngine.AddPlaySoundCommand(fileSoundThreshold.UniqueIdentifier, alarmStatus);
                }

                if (!alarmCommandsEngine.IsEmpty)
                    alarmCommandsEngine.Init(parent.Title);
            }


            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(Environment.GetCommandLineArgs());
            if (commandArgs.ArgPairs.ContainsKey("client"))
                return;

            //var startupControl = new Controls.StartupControl();
            //var wnd = new DevExpress.Xpf.Core.DXWindow()
            //{
            //    BorderEffect = DevExpress.Xpf.Core.BorderEffect.Default,
            //    ShowInTaskbar = false,
            //    ResizeMode = System.Windows.ResizeMode.NoResize,
            //    WindowStyle = WindowStyle.None,
            //    WindowState = WindowState.Normal,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    Content = startupControl,
            //    SizeToContent = SizeToContent.WidthAndHeight
            //};

            //ThemeHelper.SetTheme(wnd);

            //var canClose = false;
            //wnd.Closing += (o, e) =>
            //{
            //    e.Cancel = !canClose;
            //};

            //wnd.Show();
            if (!doc.ServerCMSHelperAsync.IsServerRunning)
            {
                try
                {
                    if (doc.StartServer(/*startupControl.txtevent, startupControl.Scroll, */bSave: false))
                    {
                        var dateTime = DateTime.Now.AddSeconds(60);
                        while (!doc.ServerCMSHelperAsync.IsServerRunning && dateTime > DateTime.Now)
                            WaitForPriority.DoEventsSync();
                        while (!doc.ServerCMSHelperAsync.IsServerStarted && dateTime > DateTime.Now && doc.ServerCMSHelperAsync.IsServerStartedManually)
                            WaitForPriority.DoEventsSync();

                        if (!listStartedDocuments.Contains(doc))
                            listStartedDocuments.Add(doc);
                    }
                }
                catch (Exception ex)
                {
                    if (UIInterface != null)
                    {
                        UIInterface.ShowError(String.Format(Properties.Resources.StartServerFailed.Replace("'newline'", Environment.NewLine),
                            UFUAServerInfo.UFUAServerInfo.GetServerName(), ex.Message));
                    }
                }
            }
            //canClose = true;
            //wnd.Close();
#endif
        }

#if !NET_STANDARD
        public void Copy(Uri uri, string newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    // if (!bCopy)
                    {
                        UFUAServerDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                            CloseView(doc.ActiveView as UFUAEditorControl, false);
                    }

                    UFUAServerDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, null, this);
                }
            }
        }

        public void Rename(Uri uri, string oldName, string newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UFUAServerDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as UFUAEditorControl, false);

                    UFUAServerDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);
                }
            }
        }

        public void Delete(Uri uri, IDocument parent)
        {
            //if (UIInterface != null)
            //{
            //    if (UIInterface.ShowOkCancel(String.Format(Properties.Resources.ConfirmRemove,
            //        GetDocumentTitle(uri)), CustomDialogIcons.Exclamation) == CustomDialogResults.Cancel)
            //        return;
            //}

            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    UFUAServerDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                        CloseView(doc.ActiveView as UFUAEditorControl, false);

                    UFUAServerDocument.RemoveFile(uri.GetPathString(), parent, this);
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(bForceSave: true, forceEncryption: encryptFile);
            var doc = UFUAServerDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc != null)
            {
                doc.Parent = parent;
                using (doc)
                {
                    return doc.SaveToFile(bForceSave: true, forceEncryption: encryptFile);
                }
            }
            return false;
        }

        public void CleanCoreFiles(String projectPath)
        {
            var fileCore = String.Format("{0}\\{1}\\{2}{3}{4}",
                                projectPath,
                                TypeLabel,
                                FileName,
                                FileType,
                                UFUAServerInfo.UFUAServerInfo.GetServerCoreExtension());

            if (System.IO.File.Exists(fileCore))
                System.IO.File.Delete(fileCore);

            //I delete all driver "* .core" files
            Directory.GetFiles(UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), "*" + UFUAServerInfo.UFUAServerInfo.GetServerCoreExtension()).ToList().ForEach(f => File.Delete(f));

        }
#endif

        public IDocument GetDocument(Uri uri)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()];
            return null;
        }

        public IDocument GetChildDocument(Uri uri)
        {
            return null;
        }

#if !NET_STANDARD
        internal void OnChangedDocument(Object sender, ChangedType type, System.Collections.ICollection changedObjects)
        {
            UFUAServerDocument doc = sender as UFUAServerDocument;
            String uri;
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                ObservableCollection<IDocumentManager> list = new ObservableCollection<IDocumentManager>();
                if (mapChildDocumentManagers.ContainsKey(uri))
                {
                    var founds = (from c in mapChildDocumentManagers[uri] where c is TreeDocumentManagers.TreeChangedDocument select c as TreeDocumentManagers.TreeChangedDocument).ToList();
                    foreach (var found in founds)
                        found.OnChangedDocument(sender, type, changedObjects);
                }
            }
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: true);

            return GetChildDocumentManagers(doc, uri.GetPathString());
        }

        ObservableCollection<IDocumentManager> GetChildDocumentManagers(UFUAServerDocument doc, String uri, bool bCreate = true)
        {
            if (!bCreate && !mapChildDocumentManagers.ContainsKey(uri))
                return null;

            ObservableCollection<IDocumentManager> list = new ObservableCollection<IDocumentManager>();
            if (mapChildDocumentManagers.ContainsKey(uri))
            {
                list = mapChildDocumentManagers[uri];
                //while (list.Count > 0)
                //    list.RemoveAt(0);
                //doc = null;
            }
            else
            {
                mapChildDocumentManagers.Add(uri, list);
                list.Add(new TreeDocumentManagers.TreePrototypeDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeAddressSpaceDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeGeneralSettingsDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeDriversDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeAlarmPrototypesDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeHistoricalPrototypesDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeDataLoggerSettingsDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeEngineeringUnitPrototypesDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeViewsDocManager(this, doc));
                list.Add(new TreeDocumentManagers.TreeRedundancyDocManager(this, doc));
            }
            return list;
        }

        public UFInterfaces.Service.IServiceControl GetServiceControl(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent, true, true);
            if (doc == null || doc.IsEmpty)
                return null;

            try
            {
                string serverConn = String.Empty;
                string stringConn = String.Empty;
                string userConn = String.Empty;
                if (doc.FilePath != null)
                {
                    serverConn = DevExpress.Xpo.DB.InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", doc.FilePath));
                    if (StringEditor != null)
                        stringConn = StringEditor.GetConnectionStringFromFile(doc.rootBase);
                    if (UserEditor != null)
                        userConn = UserEditor.GetConnectionStringFromFile(doc.rootBase);
                }
                else
                    serverConn = userConn = stringConn = doc.ConnectionString;

                var docPath = doc.GetSpecialFolder(SpecialFolders.Documents).GetPathString();
                docPath = docPath.Trim('\\', '/');

                var applicationName = doc.GetAplicationName();
                return new Service.ServiceControl(doc.GetAplicationName(), serverConn, stringConn, userConn, docPath, doc.NeedToRunAsCFR21UserIndentity());
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }

        public IDictionary<string, string> GetOptionsLicenseRequired(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent, true, true);
            if (doc == null || doc.IsEmpty)
                return null;

            try
            {
                return new Dictionary<String, String>()
                {
                    {
                        Properties.Resources.LicenseTagsRequired, doc.TotalDynamicTags.ToString()
                    }
                    //{ 
                    //    Properties.Resources.ADBeep, Boolean.TrueString
                    //},
                    //{
                    //    Properties.Resources.ADDelayOn, Boolean.FalseString
                    //},
                    //{
                    //    Properties.Resources.ADCondition, "Hello"
                    //}
                };
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }
#endif

        public String TypeTitle
        {
            get
            {
                return Properties.Resources.TypeTitle;
            }
        }

        public String TypeLabel
        {
            get
            {
                return Properties.Settings.Default.TypeLabel;
            }
        }

#if !NET_STANDARD
        public BitmapImage TypeIcon
        {
            get
            {
                return GetBitmapImage("UFUASEditorSmall");
            }
        }

        public BitmapImage TypeIconOpen
        {
            get
            {
                return TypeIcon;
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetBitmapImage("UFUASEditor");
            }
        }

        public System.Windows.Controls.Primitives.Popup TypeContextMenu
        {
            get
            {
                return null;
            }
        }
#endif

        public String TypeScheme
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return Path.GetFileNameWithoutExtension(assembly.Location);
            }
        }

        public String FileType
        {
            get
            {
                return Properties.Settings.Default.DefaultFileExt;
            }
        }

        public String FileName
        {
            get
            {
                return Properties.Settings.Default.DefaultProjectName;
            }
        }

        public String[] SaveAsFileExtensions
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.SaveAsFileExtensions))
                    return Properties.Settings.Default.SaveAsFileExtensions.ToLower().Split(';');
                return null;
            }
        }

#if !NET_STANDARD
        public bool RegisterFileType
        {
            get { return true; }
        }

        public bool CanBeDragged
        {
            get
            {
                return false;
            }
        }

        public Object DragContent
        {
            get
            {
                return null;
            }
        }

        public Object BrowsableContent
        {
            get
            {
                return null;
            }
        }
#endif

        public bool isMultipleResource
        {
            get
            {
                return false;
            }
        }

        public bool isServiceResource
        {
            get { return false; }
        }

#if !NET_STANDARD
        public bool IsResourceExpandable(IDocument document = null)
        {
            return true;
        }
#endif

        public bool IsStartupControllerAware
        {
            get
            {
                return false;
            }
        }

#if !NET_STANDARD
        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(uri, encryptFile);
                return uri;
            }

            String path, newProjectName;
            int i = 0;
            do
            {
                newProjectName = String.Format("{0}{1}", Properties.Settings.Default.DefaultProjectName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newProjectName, Properties.Settings.Default.DefaultFileExt);
            } while (File.Exists(path));

            Uri url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url, encryptFile);
            return url;
        }
#endif

        public Type DocumentType
        {
            get
            {
                return typeof(UFUAServerDocument);
            }
        }

        #endregion IDocumentManager Members

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();
#if !NET_STANDARD
            mapChildDocumentManagers.Clear();
            mapAddressSpaceControls.Clear();
            mapPrototypesControls.Clear();
#endif

            foreach (IDisposable document in mapRunningDocuments.Values)
                document.Dispose();
            mapRunningDocuments.Clear();

#if !NET_STANDARD
            foreach (IDisposable engine in mapRunningAlarmCommandsEngine.Values)
                engine.Dispose();
            mapRunningAlarmCommandsEngine.Clear();

            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.Closed -= workspace_Closed;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.PromptFriendObjects -= workspace_PromptFriendObjects;
                workspace.PromptDocumentEditorObject -= workspace_PromptDocumentEditorObject;
                workspace.PromptSmartTagsEditorObject -= workspace_PromptSmartTagsEditorObject;
                workspace.EasyModeChanged -= workspace_EasyModeChanged;
            }
#endif
        }

        #endregion IDisposable Members

        internal UFUAServerDocument CreateDocument(IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = new Uri(p.rootBase, UriKind.RelativeOrAbsolute);

            UFUAServerDocument doc = null;
            doc = UFUAServerDocument.FromFile(uri.GetPathString(), this, parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }

        internal UFUAServerDocument GetOrCreateDocument(IDocument parent, bool bRefresh = false, bool bUseHiddens = true)
        {
            lock (lockObject)
            {
                bUseHiddens = false;
                bRefresh = false;

                var p = DocumentHelper.GetRootParent(parent, traverse: false);
                // var p = parent.Parent ?? parent;
                var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);

                Dictionary<String, UFUAServerDocument> map = bUseHiddens ? mapActiveHiddenDocuments : mapActiveDocuments;
                var list = (from c in map/*.AsParallel()*/
                            where c.Key == uri.GetPathString()//  && c.Value.Parent == p && c.Value.ActiveView == null
                            select c.Value).ToList();

                //ObservableCollection<IDocumentManager> listManagers = null;
                if (bUseHiddens && list.Count > 0)
                {
                    if (listActiveHiddenDocumentsToRefresh.Contains(list[0]))
                    {
                        listActiveHiddenDocumentsToRefresh.Remove(list[0]);
                        mapActiveHiddenDocuments.Remove(uri.GetPathString());
                        //#if !NET_STANDARD
                        //                        if (mapChildDocumentManagers.ContainsKey(list[0]))
                        //                            listManagers = mapChildDocumentManagers[list[0]];
                        //#endif

                        DisposeDocument(list[0]);
                        list.Clear();
                    }
                }
                UFUAServerDocument doc = null;
                // if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                if (list.Count == 0 || bRefresh)
                {
                    if (bUseHiddens && list.Count > 0)
                    {
                        list.ForEach(d =>
                        {
                            DisposeDocument(d);
                        });
                        mapActiveHiddenDocuments.Remove(uri.GetPathString());
                    }

                    doc = UFUAServerDocument.FromFile(uri.GetPathString(), this, parent);
                    if (doc == null)
                    {
                        return null;
                    }
                    doc.Parent = p;
#if !NET_STANDARD
                    doc.NeedsSave = false;
#endif
                    if (!map.ContainsKey(uri.GetPathString()))
                        map.Add(uri.GetPathString(), doc);
                    //#if !NET_STANDARD
                    //                    if (listManagers != null && !mapChildDocumentManagers.ContainsKey(doc))
                    //                        mapChildDocumentManagers.Add(doc, listManagers);
                    //#endif
                }
                else
                    doc = list[0];
                return doc;
            }
        }

        #region IUFUAEditorManager
#if !NET_STANDARD
        public UserControl GetAlarmListControl(IDocument parent, bool singleselection = true)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            return new Controls.AlarmBrowser(doc, true, singleselection);
        }

        public UserControl GetAddressSpaceControl(IDocument parent, bool bRefreshAS = false, bool bNewControl = false)
        {
            return GetAddressSpaceControl(parent, SelectionType.OPCUAEntityReference, bRefreshAS, bNewControl, FilterType.None, TargetType.None, SelectionMode.SingleRow, false);
        }

        readonly Dictionary<UFUAServerDocument, Controls.AddressSpace> mapAddressSpaceControls = new Dictionary<UFUAServerDocument, Controls.AddressSpace>();
        readonly Dictionary<UFUAServerDocument, Controls.PrototypeList> mapPrototypesControls = new Dictionary<UFUAServerDocument, Controls.PrototypeList>();
        internal UserControl GetAddressSpaceControl(IDocument parent, SelectionType selectionType, bool bRefreshAS, bool bNewControl, FilterType filterType, TargetType targetType, SelectionMode selectionMode, bool bPrototypesTab)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: bRefreshAS, bUseHiddens: false);
            if (doc == null)
                return null;

            AddressSpace addressSpaceControl;
            PrototypeList prototypesControl;

            if (bNewControl)
                addressSpaceControl = new AddressSpace(doc, true);
            else
            {
                if (!mapAddressSpaceControls.ContainsKey(doc))
                    mapAddressSpaceControls.Add(doc, new AddressSpace(doc, true));
                else
                    mapAddressSpaceControls[doc].SelectionType = selectionType;
                addressSpaceControl = mapAddressSpaceControls[doc];
            }

            addressSpaceControl.SelectionType = selectionType;
            addressSpaceControl.SelectionMode = selectionMode;
            addressSpaceControl.FilterType = filterType;
            addressSpaceControl.TargetType = targetType;

            if (!bPrototypesTab)
                return addressSpaceControl;
            else
            {
                if (bNewControl)
                    prototypesControl = new PrototypeList(doc, true);
                else
                {
                    if (!mapPrototypesControls.ContainsKey(doc))
                        mapPrototypesControls.Add(doc, new PrototypeList(doc, true));
                    prototypesControl = mapPrototypesControls[doc];
                }

                prototypesControl.SelectionMode = selectionMode;
                prototypesControl.TargetType = targetType;

                var tabControl = new DevExpress.Xpf.Core.DXTabControl();
                tabControl.Items.Add(new DevExpress.Xpf.Core.DXTabItem() { Content = addressSpaceControl, Header = Properties.Resources.AddressSpaceHeader });
                tabControl.Items.Add(new DevExpress.Xpf.Core.DXTabItem() { Content = prototypesControl, Header = Properties.Resources.Prototypes });
                return new UserControl() { Content = tabControl };
            }
        }

        public UserControl GetRuntimeAddressSpaceControl(IDocument parent, bool inExecution = false, bool multiSelectionAllowed = false)
        {
            UFUAServerDocument doc = null;
            if (!inExecution)
                doc = GetOrCreateDocument(parent, bRefresh: false, bUseHiddens: false);
            else
                doc = CreateDocument(parent);
            if (doc == null)
                return null;
            var childs = parent.Childs;
            var listChilds = new List<UFUAServerDocument>();
            if (childs != null && childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    UFUAServerDocument docChild = null;
                    if (!inExecution)
                        docChild = GetOrCreateDocument(child, bRefresh: false, bUseHiddens: false);
                    else
                        docChild = CreateDocument(child);
                    if (docChild != null)
                        listChilds.Add(docChild);
                }
            }
            return new Controls.RuntimeAddressSpaceControl(doc, listChilds, multiSelectionAllowed);
        }
        public UserControl GetDriverListControl(bool popup = true)
        {
            return new Controls.DriverList(popup/*true*/);
        }

        public UserControl GetDynamicSettingsControl(IDocument parent, object dynamicTag)
        {
            var isServerDocument = parent is UFUAServerDocument;
            var doc = GetOrCreateDocument(parent, bRefresh: !isServerDocument, bUseHiddens: !isServerDocument);
            if (doc == null)
                return null;
            return new Controls.DynamicSettings(doc, dynamicTag as IDynamicSettingsEditing, false);
        }

        public UserControl GetHistoricalControl(IDocument parent)
        {
            return null;
        }
        public UserControl GetEngineeringUnitsControl(IDocument parent, bool isPopup, string itemToSelect)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: false, bUseHiddens: false);
            if (doc == null)
                return null;

            return new EngineeringUnitPrototypeList(doc, isPopup, itemToSelect);
        }
#endif

        public String GetServerEntityReference(IDocument parent, bool bCheckEmpty = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null || (bCheckEmpty && doc.IsEmpty))
                return null;
            try
            {
                return doc.GetServerOPCUAEntityReference().ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public String GetAlarmsSourceEntityReference(IDocument parent, string sourcePath, bool inExecution = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                var entity = doc.GetAlarmsSourceOPCUAEntityReference(sourcePath, inExecution);
                if (entity == null)
                    return null;

                return entity.ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public bool IsEventDataProtectionEnabled(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            try
            {
                return doc.GetConfiguration(inExecution: true).EnableEventDataProtection;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return false;
            }
        }

        public bool IsHistorianDataProtectionEnabled(IDocument parent, string historian)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            try
            {
                var settings = doc.GetHistoricalSettings(historian, inExecution: true);
                return settings != null && settings.EnableDataProtection;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return false;
            }
        }

        public bool IsAtLeastOneAuditTraceEnabled(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            try
            {
                return doc.IsAtLeastOneAuditTraceEnabled();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return false;
            }
        }

        public String GetHistorianDefaultConnection(IDocument parent, bool createTable = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                var connection = doc.GetHistorianDefaultConnection();
                if (createTable && !String.IsNullOrEmpty(connection))
                    UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditDataLog>(connection, typeof(UFUAHistorianModel.UFUAAuditDataItem));
                return connection;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public String GetHistorianConnection(IDocument parent, String historian)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                return doc.GetHistorianConnection(historian);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public String GetDataLoggerConnection(IDocument parent, String datalogger)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                return doc.GetDataLoggerConnection(datalogger);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public String GetEventDefaultConnection(IDocument parent, bool createTable = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                var connection = doc.GetEventDefaultConnection();
                if (createTable && !String.IsNullOrEmpty(connection))
                    UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditDataLog>(connection);
                return connection;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public String GetAuditTraceDefaultConnection(IDocument parent, bool createTable = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                var connection = doc.GetAuditTraceDefaultConnection();
                if (createTable && !String.IsNullOrEmpty(connection))
                    UFUAHistorianModel.Helpers.HistorianHelper.TryUpdateSchema<UFUAHistorianModel.UFUAAuditDataLog>(connection);
                return connection;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public IDictionary<String, String> GetHistorianConnections(IDocument parent, bool usedefault = true)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetHistorianConnections(usedefault);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public String[] GetServerUriArray(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetServerUriArray();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IDictionary<String, String> GetDataLoggerConnections(IDocument parent, bool usedefault = true)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetDataLoggerConnections(usedefault);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public String GetDataLoggerTableName(IDocument parent, String name)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                return doc.GetDataLoggerTableName(name);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public IDictionary<String, IDictionary<String, String>> GetFlatListChildsPrototypeInstances(IDocument parent, bool inExecution = false)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            if (p.Childs.Count > 0)
            {
                Dictionary<String, IDictionary<String, String>> map = new Dictionary<String, IDictionary<String, String>>();
                foreach (var c in p.Childs)
                {
                    var doc = GetOrCreateDocument(c);
                    if (doc == null)
                        return null;
                    try
                    {
                        //return doc.GetFlatListPrototypeInstances();
                        var flatPrototypeList = doc.GetFlatListPrototypeInstances();

                        map.Add(c.Title, flatPrototypeList);
                    }
                    catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                    {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                        log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                        return null;
                    }
                }
                return map;
            }
            return null;
        }

        public IDictionary<String, IList<string>> GetFlatListChildTags(IDocument parent, bool inExecution = false)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);

            if (p.Childs.Count > 0)
            {
                Dictionary<String, IList<string>> map = new Dictionary<String, IList<string>>();

                foreach (var c in p.Childs)
                {
                    var doc = GetOrCreateDocument(c);
                    if (doc == null)
                        return null;
                    try
                    {
                        var flatTagsList = doc.GetFlatTagNameCollection();
                        map.Add(c.Title, flatTagsList.ToList());
                    }
                    catch (Exception e)
                    {

                    }
                }
                return map;
            }
            return null;
        }

        public IEnumerable<String> GetFlatListTags(IDocument parent, bool inExecution = false)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            if (inExecution)
            {
                lock (mapCacheTagsList)
                {
                    if (mapCacheTagsList.ContainsKey(p))
                        return mapCacheTagsList[p];
                }
            }

            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                var flatTagsList = doc.GetFlatTagNameCollection();

                if (inExecution)
                {
                    lock (mapCacheTagsList)
                    {
                        if (!mapCacheTagsList.ContainsKey(p))
                            mapCacheTagsList.Add(p, flatTagsList);
                    }
                }

                return flatTagsList;

            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IDictionary<String, IDictionary<String, IList<string>>> GetListChildsPrototypesDesc(IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);

            if (p.Childs.Count > 0)
            {
                Dictionary<String, IDictionary<String, IList<string>>> map = new Dictionary<String, IDictionary<String, IList<string>>>();

                foreach (var c in p.Childs)
                {
                    var doc = GetOrCreateDocument(c);
                    if (doc == null)
                        return null;
                    try
                    {
                        var flatProtoDescList = doc.GetListPrototypesDesc();
                        map.Add(c.Title, flatProtoDescList);
                    }
                    catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
                    {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                        log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                        return null;
                    }
                }
                return map;
            }
            return null;
        }
        public IDictionary<String, IList<string>> GetListPrototypesDesc(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetListPrototypesDesc();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IDictionary<String, IList<String>> GetFlatListPrototypes(IDocument parent, bool inExecution = false)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            if (inExecution)
            {
                lock (mapCachePrototypesList)
                {
                    if (mapCachePrototypesList.ContainsKey(p))
                        return mapCachePrototypesList[p];
                }
            }

            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                //return doc.GetFlatListPrototypes();
                var prototypesList = doc.GetFlatListPrototypes();

                if (inExecution)
                {
                    lock (mapCachePrototypesList)
                    {
                        if (!mapCachePrototypesList.ContainsKey(p))
                            mapCachePrototypesList.Add(p, prototypesList);
                    }                    
                }

                return prototypesList;

            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        Dictionary<IDocument, IDictionary<String, String>> mapCachePrototypeInstances = new Dictionary<IDocument, IDictionary<String, String>>();
        Dictionary<IDocument, IDictionary<String, IList<String>>> mapCachePrototypesList = new Dictionary<IDocument, IDictionary<String, IList<String>>>();
        Dictionary<IDocument, IList<String>> mapCacheTagsList = new Dictionary<IDocument, IList<String>>();
        Dictionary<string, IDocument> mapCacheAllProjectDocuments = new Dictionary<string, IDocument>();


        public IDocument GetProjectDocument(IDocument parent, string appName, bool inExecution = false)
        {
            try
            {
                string parentName = GetAplicationName(parent);

                if (parentName == appName)
                    return parent;

                if (inExecution)
                {
                    lock (mapCacheAllProjectDocuments)
                    {
                        if (mapCacheAllProjectDocuments.ContainsKey(appName))
                            return mapCacheAllProjectDocuments[appName];

                        if (!mapCacheAllProjectDocuments.ContainsKey(parentName))
                            mapCacheAllProjectDocuments.Add(parentName, parent);
                    }
                }

                var p = DocumentHelper.GetRootParent(parent, traverse: false);                    

                if (p != null)
                {
                    if (p.Childs != null && p.Childs.Count > 0)
                    {
                        foreach (var child in p.Childs)
                        {
                            string name = GetAplicationName(child);

                            if (inExecution)
                            {
                                lock (mapCacheAllProjectDocuments)
                                {
                                    if (!mapCacheAllProjectDocuments.ContainsKey(name))
                                        mapCacheAllProjectDocuments.Add(name, child);
                                }
                            }
                            
                            var found = GetProjectDocument(child, appName, inExecution);
                            if (found != null)
                            {
                                return found;
                            }
                            
                        }
                    }
                }
                return null;
            }
            catch(Exception ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#endif

                return null;
            }
        }

        public IDictionary<String, String> GetFlatListPrototypeInstances(IDocument parent, bool inExecution = false)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            if (inExecution)
            {
                lock (mapCachePrototypeInstances)
                {
                    if (mapCachePrototypeInstances.ContainsKey(p))
                        return mapCachePrototypeInstances[p];
                }
            }

            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                //return doc.GetFlatListPrototypeInstances();
                var flatPrototypeList = doc.GetFlatListPrototypeInstances();

                if (inExecution)
                {
                    lock (mapCachePrototypeInstances)
                    {
                        if (!mapCachePrototypeInstances.ContainsKey(p))
                            mapCachePrototypeInstances.Add(p, flatPrototypeList);
                    }
                }

                return flatPrototypeList;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }
        public String GetAplicationName(IDocument parent, bool refresh = false)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: true);
            if (doc == null)
                return null;
            return doc.GetAplicationName(refresh);
        }

#if !NET_STANDARD
        public String GetDefaultLocalEndpoint(IDocument parent, bool refresh = false)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: true);
            if (doc == null)
                return null;
            return doc.GetDefaultLocalEndpoint(refresh);
        }
        public List<String> GetEndpoints(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: true);
            if (doc == null)
                return null;
            return doc.GetEndpoints();
        }
        public IDictionary<String, String> GetListNodeNames(IDocument parent, IList<String> nodes)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: true);
            if (doc == null)
                return null;
            try
            {
                return doc.GetListNodeNames(nodes);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }
#endif

        Dictionary<IDocument, IDictionary<String, String>> mapCacheAlarmCommandsOnDbClick = new Dictionary<IDocument, IDictionary<String, String>>();

        public IDictionary<String, String> GetAlarmCommandsOnDbClick(IDocument parent)
        {
            var root = DocumentHelper.GetRootParent(parent, traverse: false);
            lock (mapCacheAlarmCommandsOnDbClick)
            {
                if (mapCacheAlarmCommandsOnDbClick.ContainsKey(root))
                    return mapCacheAlarmCommandsOnDbClick[root];
            }

            var doc = GetOrCreateDocument(root);
            if (doc == null)
                return null;
            try
            {
                var alarmCommandsOnDbClick = doc.GetAlarmCommandsOnDbClick();

                lock (mapCacheAlarmCommandsOnDbClick)
                {
                    if (!mapCacheAlarmCommandsOnDbClick.ContainsKey(root))
                        mapCacheAlarmCommandsOnDbClick.Add(root, alarmCommandsOnDbClick);
                }

                return alarmCommandsOnDbClick;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public bool IsTagPrototype(IDocument parent, String tagName, String instance, bool useCachedUow = false, string childProject = null)
        {
            UFUAServerDocument doc = null;
            if (!string.IsNullOrEmpty(childProject))
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: false);
                if (p.Childs != null && p.Childs.Count > 0)
                {
                    var list = (from c in p.Childs where c.Title == childProject select c).ToList();
                    if (list.Count > 0)
                    {
                        doc = GetOrCreateDocument(list[0]);
                    }
                }
                else
                    return false;
            }
            else
                doc = GetOrCreateDocument(parent);
            if (doc != null)
            {
                var tag = doc.GetUFUATag(instance, tagName, false, inExecution: useCachedUow);
                return (tag != null && !string.IsNullOrEmpty(tag.PrototypeName));
            }
            return false;
        }


        Dictionary<IDocument, Dictionary<String, String>> mapCacheEntityReference = new Dictionary<IDocument, Dictionary<String, String>>();

        public String GetTagEntityReference(IDocument parent, String tagName, String instance, bool inExecution = false, bool useCachedUow = false, string Project = null)
        {
            UFUAServerDocument doc = null;
            IDocument p = DocumentHelper.GetRootParent(parent, traverse: false);
            int idxParent = tagName.IndexOf(@"..\");
            var key = String.Format("{0}-{1}", tagName, instance);
            int nBackSteps = 0;
            if (idxParent != -1)
            {
                string testname = tagName;
                do
                {
                    nBackSteps++;
                    
                    testname = testname.Substring(idxParent + 3);
                    idxParent = testname.IndexOf(@"..\");
                } while (idxParent != -1);

                p = DocumentHelper.GetRootParent(parent, nBackSteps);
                if (p == null)
                    return null;

                doc = GetOrCreateDocument(p);
                tagName = testname;
                idxParent = testname.IndexOf('.');
                if(idxParent != -1)
                {
                    instance = testname.Substring(0, idxParent);
                    tagName = testname.Substring(idxParent + 1).Replace(".", "\\");
                }
            }
            string childInstance = string.Empty;
            string childTagName = string.Empty;
            if (!string.IsNullOrEmpty(Project) && p.Childs != null && p.Childs.Count > 0)
            {
                p = DocumentHelper.GetChild(parent, Project);
                if (p != null)
                {
                    //other possible childs, look for others...
                    string chPrj = tagName;
                    string chName = tagName;
                    int chIdx = tagName.IndexOf('\\');
                    if(chIdx != -1)
                    {
                        do
                        {
                            chPrj = tagName.Substring(0, chIdx);
                            chName = tagName.Substring(chIdx + 1);

                            var pTemp = DocumentHelper.GetChild(p, chPrj);
                            if (pTemp == null)
                                break;
                            p = pTemp;
                            tagName = chName;
                            chIdx = chName.IndexOf('\\');
                        } while (chIdx != -1);
                        chIdx = tagName.IndexOf("\\");
                        if(chIdx != -1)
                        {
                            childInstance = tagName.Substring(0, chIdx);
                            childTagName = tagName.Substring(chIdx + 1);
                        }
                    }
                    doc = GetOrCreateDocument(p);
                }
                else
                    return null;
            }
            
            if (inExecution)
            {
                lock (mapCacheEntityReference)
                {
                    if (mapCacheEntityReference.ContainsKey(p) &&
                        mapCacheEntityReference[p].ContainsKey(key))
                        return mapCacheEntityReference[p][key];
                }
            }
            if (doc == null)
                doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                OPCUAEntityReference tag = null;
                if (!string.IsNullOrEmpty(childInstance) && !string.IsNullOrEmpty(childTagName)) 
                    tag = doc.GetTagOPCUAEntityReference(childTagName, childInstance, inExecution: inExecution || useCachedUow);
                if(tag == null)
                    tag = doc.GetTagOPCUAEntityReference(tagName, instance, inExecution: inExecution || useCachedUow);
                if (tag == null)
                    return null;
                var ret = tag.ToXml();

                if (inExecution)
                {
                    lock (mapCacheEntityReference)
                    {
                        if (!mapCacheEntityReference.ContainsKey(p))
                            mapCacheEntityReference.Add(p, new Dictionary<string, string>());
                        if (mapCacheEntityReference[p].ContainsKey(key))
                            mapCacheEntityReference[p].Remove(key);
                        mapCacheEntityReference[p].Add(key, ret);
                    }
                }
                return ret;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

#if !NET_STANDARD
        internal UFUAModel.TagEntityReference GetObjectTagEntityReference(IDocument parent, String tagName, String member)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                string instance = null;
                string name = tagName;
                if (!string.IsNullOrEmpty(member))
                {
                    name = member;
                    instance = tagName;
                }

                var entityReference = doc.GetTagOPCUAEntityReference(name, instance, inExecution: false);
                if (entityReference == null)
                    return null;

                Guid tagGuid = Guid.Empty;
                if (entityReference.ResolvedNodeId.IdType == Opc.Ua.IdType.Guid)
                    tagGuid = (Guid)entityReference.ResolvedNodeId.Identifier;
                else if (entityReference.ResolvedNodeId.IdType == Opc.Ua.IdType.String)
                {
                    var identifier = entityReference.ResolvedNodeId.Identifier.ToString();
                    var index = identifier.LastIndexOf('?');
                    if (index != -1)
                        identifier = identifier.Substring(index + 1);
                    Guid.TryParse(identifier, out tagGuid);
                }

                if (tagGuid == Guid.Empty)
                    return null;
                return new UFUAModel.TagEntityReference(tagGuid, entityReference.RelativePath, entityReference.ResolvedNodeId, entityReference.HumanReadable);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }

        public Tuple<String, String> GetVariableListSettingsFlat(IDocument parent, List<object> list)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            return doc.GetVariableListSettingsFlat(list);
        }

        public Dictionary<String, String> CheckAndUpdateVariableListSettingsFlat(IDocument parent, String flat)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            var active = Workspace.ActiveWindow;

            Edit(uri, p);

            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;

            if (doc.ActiveView != null)
                WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, doc.ActiveView);

            doc.VariableCreated += Doc_VariableCreated;
            doc.CreatingVariable += Doc_CreatingVariable;
            try
            {
                return doc.CheckAndUpdateVariableListSettingsFlat(flat);
            }
            finally
            {
                doc.VariableCreated -= Doc_VariableCreated;
                doc.CreatingVariable -= Doc_CreatingVariable;

                Workspace.ActiveWindow = active;
            }
        }

        private void Doc_CreatingVariable(object sender, VariableEventArgs e)
        {
            OnCreatingVariable(sender, e);
        }

        private void Doc_VariableCreated(object sender, VariableEventArgs e)
        {
            OnVariableCreated(sender, e);
        }

        public event EventHandler<VariableEventArgs> CreatingVariable;
        virtual public void OnCreatingVariable(Object sender, VariableEventArgs args)
        {
            var t = CreatingVariable;
            if (t != null)
                t(sender, args);
        }

        public event EventHandler<VariableEventArgs> VariableCreated;
        virtual public void OnVariableCreated(Object sender, VariableEventArgs args)
        {
            var t = VariableCreated;
            if (t != null)
                t(sender, args);
        }
#endif

        public String GetNodeIdEntityReference(IDocument parent, String tagName, String nodeID)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetNodeIdOPCUAEntityReference(tagName, nodeID).ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }
        public String GetWriteVarValuesOPCUAEntityReference(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetWriteVarValuesOPCUAEntityReference().ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public String GetWriteValuesEntityReference(IDocument parent, String driverName)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetWriteValuesOPCUAEntityReference(driverName).ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public String GetReadValuesEntityReference(IDocument parent, String driverName)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetReadValuesOPCUAEntityReference(driverName).ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IList<String> GetDataLoggerSettingsNameList(IDocument parent, bool bReloadDocument = true, bool inExecution = false)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: bReloadDocument);
            if (doc == null)
                return null;
            try
            {
                return doc.GetDataLoggerSettingsNameList(inExecution);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IList<String> GetHistoricalSettingsNameList(IDocument parent, bool bReloadDocument = true, bool inExecution = false)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: bReloadDocument);
            if (doc == null)
                return null;
            try
            {
                return doc.GetHistoricalSettingsNameList(inExecution);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IList<String> GetAuditTraceTagNameList(IDocument parent, bool bReloadDocument = true, bool inExecution = false)
        {
            var doc = GetOrCreateDocument(parent, bRefresh: bReloadDocument);
            if (doc == null)
                return null;
            try
            {
                return doc.GetAuditTraceTagNameList(inExecution);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IList<List<String>> GetDataLoggerSettings(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetDataLoggerSettingsList();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }
        public String GetDataLoggerDataTable(IDocument parent, String DataLoggerName, bool inExecution = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                var p = doc.GetDataLogger(DataLoggerName, inExecution);
                if (p == null)
                    return null;
                DataLoggerModel.Helpers.DataLoggerTable dlt = new DataLoggerModel.Helpers.DataLoggerTable(p, doc.rootBase);
                return dlt.ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IList<String> GetDataLoggerColumnList(IDocument parent, String rootName)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetDataLoggerColumnList(rootName);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }
        public bool UsesAggreagatedTables(IDocument parent, String rootName)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            try
            {
                return doc.UsesAggreagatedTables(rootName);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return false;
            }
        }
        public bool RemoveListTags(IDocument parent, List<string> resolvedNodeIds)
        {
#if !NET_STANDARD
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            try
            {
                doc.RemoveTagList(resolvedNodeIds);
                doc.SaveToFile(bForceSave: true);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorRemovingTags, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return false;
            }
            return true;
#else
            return false;
#endif
        }
        public String GetTagEngineeringUnit(IDocument parent, String nodeid)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                return doc.GetTagEngineeringUnit(nodeid);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public Dictionary<string, string> GetTagsEngineeringUnit(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return new Dictionary<string, string>();
            try
            {
                return doc.GetTagsEngineeringUnit();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return new Dictionary<string, string>();
            }
        }
        public IList<List<String>> GetDataLoggerColumnSettingList(IDocument parent, String rootName)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetDataLoggerColumnSettingList(rootName);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public String GetEngineeringUnit(IDocument parent, String engineeringUnitName, bool inExecution = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                var eu = doc.GetEngineeringUnit(engineeringUnitName, inExecution);
                if (eu == null)
                    return null;
                return new UFUAModel.Helpers.EngineeringUnitData(eu).ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

#if !NET_STANDARD
        public IEnumerable<String> GetEngineeringUnitNames(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetEngineeringUnitNames();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }

        public BitmapImage GetBitmapImage(IDocument parent, String image)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return GetBitmapImage(image);
            }
            catch
            {
                return null;
            }
        }
#endif

        public String GetDefApplicationName(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent, true, true);
            if (doc == null)
                return String.Empty;
            try
            {
                return doc.GetAplicationName();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

        public String GetServiceName(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent, true, true);
            if (doc == null)
                return String.Empty;
            try
            {
                return doc.GetServiceName();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

#if !NET_STANDARD
        public String GetDataLoggerColumnReference(IDocument parent, string rootName, string columnName)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            try
            {
                return doc.GetDataLoggerColumnReference(rootName, columnName).ToXml();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }
#endif

        public String GetHistorianName(IDocument parent, object resolvednodeid)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                return doc.GetHistorianName(resolvednodeid);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

#if !NET_STANDARD
        public IEnumerable<String> GetFlatListAlarmSources(IDocument parent, bool bForceRefresh = false)
        {
            var doc = GetOrCreateDocument(parent, bForceRefresh);
            if (doc == null)
                return null;
            try
            {
                return doc.GetFlatListAlarmSources();
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }

        public IEnumerable<string> GetFlatFullTagNameCollectionOrderByName(IDocument parent, bool onlyHistorical = false, bool bForceRefresh = false)
        {
            var doc = GetOrCreateDocument(parent, bForceRefresh);
            if (doc == null)
                return null;
            try
            {
                return doc.GetFlatFullTagNameCollectionOrderByName(onlyHistorical, bForceRefresh);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

        public IEnumerable<string> GetFlatFullFolderNameCollectionOrderByName(IDocument parent, bool bForceRefresh = false)
        {
            var doc = GetOrCreateDocument(parent, bForceRefresh);
            if (doc == null)
                return null;
            try
            {
                return doc.GetFlatFullFolderNameCollectionOrderByName(bForceRefresh);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return null;
            }
        }

#endif

        public String GetServerConfigurationId(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return String.Empty;
            try
            {
                Guid confId = doc.GetConfiguration().ConfigurationId;
                if (confId != null)
                    return confId.ToString();
                return String.Empty;
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#else
                log.ErrorFormat(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
#endif
                return String.Empty;
            }
        }

#if !NET_STANDARD
        public IToolbar GetToolbar()
        {
            if (menuControl == null)
                menuControl = new MenuControl(this, workspace);
            if (!menuControl.IsToolbarShown && workspace.IsWorkspaceLoaded && !bToolbarInitialized)
            {
                bToolbarInitialized = true;
                menuControl.Show();
            }

            return menuControl;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            var list = new List<System.Windows.Input.ICommand>();
            list.Add(UIGeneralCommands.AddNewTag);
            list.Add(UIGeneralCommands.ImportExportTags);
            list.Add(UIGeneralCommands.ImportExportAddressSpace);
            list.Add(UIGeneralCommands.AddNewPrototype);
            list.Add(UIGeneralCommands.StartServer);
            return list;
        }
#endif
        #endregion

        #region ICrossReference
#if !NET_STANDARD
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.QuitEvent.IsCancellationRequested)
                return result;
            bool getScreens = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Resources);
            bool getTexts = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            if (!getScreens && !getTags && !getConnections && !getScreens && !getTexts)
                return result;

            using (UFUAServerDocument doc = CreateDocument(model.Parent))
            {
                if (doc != null)
                {
                    if (getTags || getTexts || getConnections)
                        result.AddRange(doc.GetCRFlatTagCollection(model));
                    string applicationName = doc.GetAplicationName(true);
                    if (model.QuitEvent.IsCancellationRequested)
                        return result;

                    //if (!string.IsNullOrEmpty(doc.ConnectionString))
                    //    AddEntityConnection(result, Properties.Resources.UFUAEditorConnectionString, doc.Title, doc.rootBase, doc.ConnectionString, applicationName, DocManagerType.UFUAEditor);

                    string histDefConnection = null;
                    string eventDefConnection = null;
                    string auditDefConnection = null;
                    DataReaderModelConverter dataReaderModelConverter = null;
                    if (getConnections)
                    {
                        histDefConnection = doc.GetHistorianDefaultConnection();
                        if (!string.IsNullOrEmpty(histDefConnection))
                            AddEntityConnection(result, doc.Title, Properties.Resources.HistDefConnectionString, doc.rootBase, histDefConnection, applicationName, DocManagerType.HistorianConn.ToString());

                        eventDefConnection = doc.GetEventDefaultConnection();
                        if (!string.IsNullOrEmpty(eventDefConnection))
                            AddEntityConnection(result, doc.Title, Properties.Resources.EventDefConnectionString, doc.rootBase, eventDefConnection, applicationName, DocManagerType.EventConn.ToString());

                        auditDefConnection = doc.GetAuditTraceDefaultConnection();
                        if (!string.IsNullOrEmpty(auditDefConnection))
                            AddEntityConnection(result, doc.Title, Properties.Resources.AuditDefConnectionString, doc.rootBase, auditDefConnection, applicationName, DocManagerType.AuditingConn.ToString());
                    }

                    string endpointUrl = doc.GetDefaultLocalEndpoint(true);
                    if (getConnections || getTags)
                    {
                        var dlrsettings = doc.GetDataLoggerSettings();
                        string dlrSettingsDocType = DocManagerType.DataLoggerSettings.ToString();
                        for (int i = 0; i < dlrsettings.Count(); i++)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return result;

                            var dlr = dlrsettings[i];
                            if (getTags)
                            {
                                if (dlr.EnableRecordingTag != null && !dlr.EnableRecordingTag.IsEmpty())
                                    AddEntityTag(endpointUrl, Properties.Resources.NewDataLoggerSettingsEnableRecordingTag, result, dlr.EnableRecordingTag, dlr.Name, doc.rootBase, applicationName, dlrSettingsDocType);

                                if (dlr.RecordingTag != null && !dlr.RecordingTag.IsEmpty())
                                    AddEntityTag(endpointUrl, Properties.Resources.NewDataLoggerSettingsRecordingTag, result, dlr.RecordingTag, dlr.Name, doc.rootBase, applicationName, dlrSettingsDocType);

                                if (dlr.ResettingTag != null && !dlr.ResettingTag.IsEmpty())
                                    AddEntityTag(endpointUrl, Properties.Resources.NewDataLoggerSettingsResettingTag, result, dlr.ResettingTag, dlr.Name, doc.rootBase, applicationName, dlrSettingsDocType);

                                List<DataLoggerModel.DataLoggerColumn> dlrcolumns = (from c in dlr.Columns where c.IsValid select c).ToList();
                                dlrcolumns.ToList().ForEach(column =>
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        return;

                                    if (column.ColumnTag != null)
                                        AddEntityTag(endpointUrl, Properties.Resources.NewDataLoggerColumnTag, result, column.ColumnTag, string.Format("{0}\\{1}", dlr.Name, column.Name), doc.rootBase, applicationName, DocManagerType.DataLoggerSettings.ToString());
                                });

                            }

                            if (getConnections)
                            {
                                string connectionString = null;
                                if (dlr.ConnectionSettings != null && !string.IsNullOrEmpty(dlr.ConnectionSettings.Connection))
                                {
                                    if (dataReaderModelConverter == null)
                                        dataReaderModelConverter = new DataReaderModelConverter();
                                    connectionString = dataReaderModelConverter.Convert(dlr.ConnectionSettings, typeof(String), null, System.Globalization.CultureInfo.CurrentCulture)?.ToString();
                                }
                                else if (!string.IsNullOrEmpty(histDefConnection))
                                    connectionString = histDefConnection;
                                if (!string.IsNullOrEmpty(connectionString))
                                    AddEntityConnection(result, dlr.Name, Properties.Resources.RefrencedByDlr, doc.rootBase, connectionString, applicationName, DocManagerType.DataLoggerSettings.ToString());
                            }
                        };

                        var historicalSettings = doc.GetHistoricalSettings();
                        string historicalSettingsDocType = DocManagerType.HistoricalPrototypes.ToString();
                        for (int i = 0; i < historicalSettings.Count(); i++)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return result;

                            var historian = historicalSettings[i];
                            if (getTags)
                            {
                                if (historian.EnableTag != null && !historian.EnableTag.IsEmpty())
                                    AddEntityTag(endpointUrl, Properties.Resources.HisEnabledTag, result, historian.EnableTag, historian.Name, doc.rootBase, applicationName, historicalSettingsDocType);
                                if (historian.ResettingTag != null && !historian.ResettingTag.IsEmpty())
                                    AddEntityTag(endpointUrl, Properties.Resources.NewHistorianSettingsResettingTag, result, historian.ResettingTag, historian.Name, doc.rootBase, applicationName, historicalSettingsDocType);
                            }

                            if (getConnections)
                            {
                                if (!string.IsNullOrEmpty(historian.ConnectionSettings))
                                    AddEntityConnection(result, historian.Name, Properties.Resources.RefrencedByHistorian, doc.rootBase, historian.ConnectionSettings, applicationName, historicalSettingsDocType);
                                else if (!string.IsNullOrEmpty(histDefConnection))
                                    AddEntityConnection(result, historian.Name, Properties.Resources.RefrencedByHistorian, doc.rootBase, histDefConnection, applicationName, historicalSettingsDocType);
                            }
                        }
                    }

                    if (getTags || getScreens || getTexts)
                    {
                        if (getTexts)
                        {
                            var engineerings = doc.GetEngineeringUnits().ToList();
                            engineerings.ForEach(e =>
                            {
                                if (!string.IsNullOrEmpty(e.UnitName))
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = e.UnitName,
                                        Name = e.UnitName,
                                        AppName = applicationName,
                                        CReferenceType = CrossReferenceType.Strings,
                                        Description = string.Format("{0} ({1})", e.Name, Properties.Resources.CREUName),
                                        Settings = string.Format("{0}|{1}", DocManagerType.EUnits, doc.rootBase),
                                        ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                        IconType = DocManagerType.EUnits.ToString()
                                    });
                            });
                        }
                        var alarmThresholds = doc.GetAlarmThresholds(false);
                        for (int i = 0; i < alarmThresholds.Count(); i++)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return result;

                            var alarmThreshold = alarmThresholds[i];
                            string sharedDescr = string.Empty;
                            string docType = DocManagerType.AlarmThresholds.ToString();
                            if (alarmThreshold.UFUATagAss != null)
                                if (alarmThreshold.UFUATagAss.IsSubPrototypeMember)
                                {
                                    if (alarmThreshold.UFUATagAss.UseShared.Value)
                                    {
                                        sharedDescr = Properties.Resources.CRNewAlarmThresholdPrototype;
                                        docType = DocManagerType.AlarmPrototype.ToString();
                                    }
                                    else
                                        sharedDescr = Properties.Resources.CRNewAlarmThresholdTagRference;
                                }
                                else if (alarmThreshold.UFUATagAss.IsPrototypeMember)
                                {
                                    sharedDescr = Properties.Resources.CRNewAlarmThresholdPrototype;
                                    docType = DocManagerType.AlarmPrototype.ToString();
                                }
                                else
                                    sharedDescr = Properties.Resources.CRNewAlarmThresholdTagRference;
                            else
                            {
                                sharedDescr = Properties.Resources.CRNewAlarmThresholdPrototype;
                                docType = DocManagerType.AlarmPrototype.ToString();
                            }
                            if (getTags)
                            {
                                AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CREnabledThresholdTag}", result, alarmThreshold.EnableTag, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CRActivationValueThresholdTagLow}", result, alarmThreshold.ActivationLowValueTag, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CRActivationValueThresholdTag}", result, alarmThreshold.ActivationValueTag, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CRLowLimitThresholdTag}", result, alarmThreshold.LowLimitTag, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CRLowLowLimitThresholdTag}", result, alarmThreshold.LowLowLimitTag, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CRHighLimitThresholdTag}", result, alarmThreshold.HighLimitTag, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CRHighHighLimitThresholdTag}", result, alarmThreshold.HighHighLimitTag, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CRSeverityTag}", result, alarmThreshold.SeverityTag, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                for (int j = 0; j < alarmThreshold.AliasTags.Count(); j++)
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        return result;

                                    var aliasTag = alarmThreshold.AliasTags[j];
                                    AddEntityTag(endpointUrl, $"{sharedDescr} {Properties.Resources.CRAlarmThresholdAliasTag}", result, aliasTag.TagEntity, alarmThreshold.Name, doc.rootBase, applicationName, docType);
                                }
                            }
                            if (alarmThreshold.HasCRCommands)
                            {
                                AddCommandsTag(endpointUrl, model, applicationName, doc, $"{sharedDescr} {Properties.Resources.CRNewAlarmThresholdCommandsAck}", alarmThreshold.CommandsAck, result, alarmThreshold.Name, doc.rootBase, docType);
                                AddCommandsTag(endpointUrl, model, applicationName, doc, $"{sharedDescr} {Properties.Resources.CRNewAlarmThresholdCommandsDbClick}", alarmThreshold.CommandsDbClick, result, alarmThreshold.Name, doc.rootBase, docType);
                                AddCommandsTag(endpointUrl, model, applicationName, doc, $"{sharedDescr} {Properties.Resources.CRNewAlarmThresholdCommandsOff}", alarmThreshold.CommandsOff, result, alarmThreshold.Name, doc.rootBase, docType);
                                AddCommandsTag(endpointUrl, model, applicationName, doc, $"{sharedDescr} {Properties.Resources.CRNewAlarmThresholdCommandsOn}", alarmThreshold.CommandsOn, result, alarmThreshold.Name, doc.rootBase, docType);
                                AddCommandsTag(endpointUrl, model, applicationName, doc, $"{sharedDescr} {Properties.Resources.CRNewAlarmThresholdCommandsReset}", alarmThreshold.CommandsReset, result, alarmThreshold.Name, doc.rootBase, docType);
                            }
                            if (getTexts)
                            {
                                string text = alarmThreshold.AddTagDescription || string.IsNullOrEmpty(alarmThreshold.AlarmText) ? alarmThreshold.ComposedAlarmText : alarmThreshold.AlarmText;
                                if (!string.IsNullOrEmpty(text))
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = text,
                                        Name = text,
                                        AppName = applicationName,
                                        CReferenceType = CrossReferenceType.Strings,
                                        Description = string.Format("{0} ({1})", alarmThreshold.Name, Properties.Resources.CRThresholdText),
                                        Settings = string.Format("{0}|{1}", DocManagerType.UFUAServer, doc.rootBase),
                                        ContainerDoc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.TypeScheme,
                                        IconType = docType
                                    });
                            }
                        }
                        if (getTags)
                        {
                            var driversettings = doc.GetDrivers();
                            for (int i = 0; i < driversettings.Count(); i++)
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return result;

                                string docType = DocManagerType.Driver.ToString();
                                var driver = driversettings[i];

                                try
                                {
                                    Dictionary<string, TagEntityReference> res = doc.GetDriverSettingsTagList(driver, doc);
                                    foreach (var ktag in res.Keys)
                                    {
                                        if (res[ktag] != null && res[ktag] is TagEntityReference && !(res[ktag] as TagEntityReference).IsEmpty())
                                            AddEntityTag(endpointUrl, Properties.Resources.DriverTag, result, (res[ktag] as TagEntityReference), ktag, doc.rootBase, applicationName, docType);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    model.ErrorMessages.Add(ex.Message);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private void AddEntityConnection(List<UFInterfaces.Editors.CrossReferenceResultModel> result, string type, string name, string rootBase, string connection, string applicationName, string docType)
        {
            var model = new CrossReferenceResultModel()
            {
                RelativePath = connection, //$"{name}\\{type}",
                Name = name, //type,
                AppName = applicationName,
                CReferenceType = CrossReferenceType.Connections,
                Description = $"{type}", // string.Format("{0}", connection),
                Settings = string.Format("{0}|{1}|{2}", docType, rootBase, type),
                ContainerDoc = TypeScheme,
                IconType = docType
            };
            result.Add(model);
        }

        private void AddCommandsTag(string endpointUrl, UFInterfaces.Editors.CrossReferenceModel model, string applicationName, IDocument doc, string descr, string commands, List<UFInterfaces.Editors.CrossReferenceResultModel> result, string name, string pathString, string tagType)
        {

            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getScreens = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Resources);
            if (!getTags && !getScreens)
                return;
            var parent = DocumentHelper.GetRootParent(doc.Parent, traverse: false);
            if (!string.IsNullOrEmpty(commands))
            {
                var commandList = commands.FromXml<CommandManager.CommandManagerList>();
                for (int i = 0; i < commandList.Count(); i++)
                {
                    var command = commandList[i];
                    if (getTags)
                    {
                        if (!string.IsNullOrEmpty(command.Expression))
                        {
                            List<string> expressionTags = Utilities.Converters.ExpressionValueConverterHelper.GetListVarInExpression(command.Expression);
                            Parallel.ForEach(expressionTags, expression => //for (int j = 0; j < expressionTags.Count; j++)
                            {
                                OPCUAEntityReference tag = GetReferenceTag(doc, expression);
                                if (tag == null)
                                    tag = new OPCUAEntityReference() { RelativePath = expression, AppName = applicationName };
                                lock (result)
                                    AddEntityTag(descr, result, tag, $"{name} - {Properties.Resources.ItemsExpressionTagHeader} {expressionTags.IndexOf(expression)}", pathString, tagType);
                            });
                        }
                        command.ListTags.ForEach(tag =>
                        {
                            AddEntityTag(descr, result, tag, name, pathString, tagType);
                        });
                    }

                    if (getScreens)
                    {
                        var properties = CommandManager.Extensions.CommandManagerExtensions.GetBrowsablePropertiesOfType<Uri>(command);
                        foreach (PropertyInfo prop in properties)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                break;

                            var uri = prop.GetValue(command) as Uri;
                            if (uri != null && uri.GetPathString() != null)
                            {
                                string relativePath = uri.GetPathString();
                                var screenname = System.IO.Path.GetFileNameWithoutExtension(relativePath);
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = relativePath,
                                    Name = screenname,
                                    AppName = parent.Title,
                                    CReferenceType = CrossReferenceType.Resources,
                                    Description = string.Format("{0} ({1})", descr, $"{name} - {screenname}"),
                                    Settings = string.Format("{0}|{1}", tagType, pathString),
                                    ContainerDoc = TypeScheme,
                                    IconType = tagType
                                });
                            }
                        }
                    }
                }
            }
        }

        private OPCUAEntityReference GetReferenceTag(IDocument doc, string reference)
        {
            OPCUAEntityReference tag = null;
            var split = reference.Split('-');
            var instance = split[0];
            var name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;
            var xml = GetTagEntityReference(doc, name, instance, inExecution: true);
            if (!String.IsNullOrEmpty(xml))
                tag = xml.FromXml<OPCUAEntityReference>();
            return tag;
        }

        private void AddEntityTag(string descr, List<UFInterfaces.Editors.CrossReferenceResultModel> result, OPCUAEntityReference tag, string name, string pathString, string tagType)
        {
            if (IsTagReferenceValid(tag))
            {
                UFInterfaces.Editors.CrossReferenceResultModel resultModel = GetCREntityTagModel(tag, name, pathString, tagType, descr);
                resultModel.EndpointUrl = tag.EndpointUrl;
                result.Add(resultModel);
            }
        }

        private void AddEntityTag(string endpointUrl, string descr, List<UFInterfaces.Editors.CrossReferenceResultModel> result, TagEntityReference tag, string name, string pathString, string title, string tagType)
        {
            if (IsTagReferenceValid(tag))
            {
                UFInterfaces.Editors.CrossReferenceResultModel resultModel = GetCREntityTagModel(tag, name, pathString, title, tagType, descr);
                resultModel.EndpointUrl = endpointUrl;
                result.Add(resultModel);
            }
        }

        private CrossReferenceResultModel GetCREntityTagModel(OPCUAEntityReference tag, string name, string pathString, string tagType, string descr)
        {
            return new CrossReferenceResultModel()
            {
                RelativePath = tag.RelativePath,
                Name = tag.Name,
                AppName = tag.AppName,
                ReferencedNodeId = tag.ResolvedNodeId?.Identifier.ToString(),
                EndpointUrl = tag.EndpointUrl,
                CReferenceType = CrossReferenceType.Tags,
                Description = string.Format("{0}: {1}", descr, name),
                Settings = string.Format("{0}|{1}", tagType, pathString),
                ContainerDoc = TypeScheme,
                IconType = tagType
            };
        }

        private CrossReferenceResultModel GetCREntityTagModel(TagEntityReference tag, string name, string pathString, string applicationName, string tagType, string descr)
        {
            return new CrossReferenceResultModel()
            {
                RelativePath = tag.Name,
                Name = tag.Name.Split('/').LastOrDefault(),
                AppName = applicationName,
                ReferencedNodeId = tag.NodeId.Identifier.ToString(),
                CReferenceType = CrossReferenceType.Tags,
                Description = string.Format("{0}: {1}", descr, name),
                Settings = string.Format("{0}|{1}", tagType, pathString),
                ContainerDoc = TypeScheme,
                IconType = tagType
            };
        }
        private bool IsTagReferenceValid(TagEntityReference tagReference)
        {
            return tagReference != null && !tagReference.IsEmpty();
        }
        private bool IsTagReferenceValid(OPCUAEntityReference tagReference)
        {
            return tagReference != null && tagReference.IsValid;
        }
        public void EditCRObject(IDocument parent, string settings)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            string[] path = settings.Split('|');
            if (path.Length >= 2)
            {
                var uri = new Uri(path[1], UriKind.RelativeOrAbsolute);
                Edit(uri, p);
                Dispatcher.CurrentDispatcher.BeginInvokeIfRequired(() =>
                {
                    DocManagerType e;
                    Enum.TryParse(path[0], out e);
                    GetActiveView(uri).SetSelectedTab(e);
                });
            }
        }
        public UFUAEditorControl GetActiveView(Uri uri)
        {
            UFUAServerDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as UFUAEditorControl;
            return null;
        }

        public void RenameCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            if (model.QuitEvent.IsCancellationRequested)
                return;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            Dictionary<string, string> nodeIdMap = new Dictionary<string, string>();

            if (model.QuitEvent.IsCancellationRequested)
                return;

            UFUAServerDocument doc = GetOrCreateDocument(model.Parent);
            if (doc != null)
                doc.RenameReferences(nodeIdMap, model);
        }

        public bool NeedToCloseCRDocuments()
        {
            return mapActiveDocuments.Count > 0 && (from d in mapActiveDocuments.Values where d.ActiveView != null select d).FirstOrDefault() != null;
        }

        public bool NeedSingleThreadedApartment
        {
            get
            {
                return false;
            }
        }

        public bool CheckVariable(IDocument parent, string name, string endpointurl, Dictionary<String, List<String>> prototypelist, out string tagFound, bool getTag = false, bool clearCache = false)
        {
            tagFound = null;
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            try
            {
                if (!string.IsNullOrEmpty(endpointurl))
                {
                    var endpointslist = doc.GetEndpoints();
                    if (!endpointslist.Contains(endpointurl))
                        return true;
                }

                return doc.CheckVariable(name, prototypelist, out tagFound, getTag, clearCache);
            }
            catch (DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException ex)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, TypeTitle, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return false;
            }
        }
#endif
        #endregion
    }
}