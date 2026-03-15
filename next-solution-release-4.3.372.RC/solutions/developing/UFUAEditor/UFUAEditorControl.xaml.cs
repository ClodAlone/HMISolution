using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using UFUAEditor.Controls;
using DriverSettingsInterfaces;
using System.Reflection;
using UFUAModel;
using System.Windows.Threading;
using UFInterfaces;
using NewDriverWizard.ComponentService;
// FOGBUGZ 11407
using UIMsgBoxAlertService.ComponentService;
using UFUAEditor.TabHelper;
using WizardSettings;
using UFInterfaces.Editors;
using DocumentManager.ComponentService;
using TempVarriables.ComponentService;
using UFUAEditor.ComponentService;
using DataReader.Extensions;
using System.Threading.Tasks;
using DataReader.Helpers;
using System.Threading;
using System.Data;
using log4net;
using DataLoggerModel.Helpers;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using UFUAEditor.Extensions;
using DevExpress.Xpo;
using DevExpress.Xpf.Core;
using Utilities.Xpo.UndoRedo;
using System.IO;
using CsvHelper;
using UFUAEditor.ViewModels;
using System.Diagnostics;
using System.Text;

namespace UFUAEditor
{
    /// <summary>
    /// Interaction logic for UFUAEditor.xaml
    /// </summary>
    [System.Runtime.InteropServices.GuidAttribute("885A31B7-D6BB-421B-9841-2C70ECB0E767")]
    public partial class UFUAEditorControl : UserControl, IEditableObject/*, IDisposable (keep this comment)*/
    {
        #region Declarations
        RedundancySettings redundancySettings;
        DriverProjectList driverProjectList;
        AddressSpace addressSpaceControl;
        ViewList viewList;
        PrototypeList prototypeList;
        HistoricalPrototypeList historicalPrototypeList;
        DataLoggerSettingsList dataLoggerSettingsList;
        AlarmPrototypeList alarmPrototypeList;
        EngineeringUnitPrototypeList engineeringUnitPrototypeList;
        //UserControl tempVariablesControl;
        //UserControl emptyTempVariablesControl;
        Window parentWindow;

        bool? bCanExecuteImportTagsDriver;

        Dictionary<AggregationTypes.CommandType, List<String>> pendingAggregateDataLoggers = new Dictionary<AggregationTypes.CommandType, List<String>>();
        DispatcherOperation dpAggregateTables;
        CancellationTokenSource ctsAggregateTables;

        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);
        internal bool isActive;
        public ControlTabEnum TabEnum { get; set; }
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public UFUAEditorControl(UFUAServerDocument doc)
        {
            InitializeComponent();
            Document = doc;

            Loaded += (o, e) =>
            {
                bLoaded = true;

                if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.IsInEasyMode)
                    HideComponents();
                else
                    RestoreComponents();

                if (parentWindow == null)
                {
                    parentWindow = this.FindParent<Window>();
                    if (parentWindow != null)
                        parentWindow.Activated += parentWindow_Activated;
                }
            };
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            Document.GetSession().ObjectChanged += (s, e) =>
            {
                if (e.Object is DataLoggerModel.DataLoggerSettings &&
                    e.PropertyName == "UseAggregatedTables" && e.OldValue != e.NewValue)
                {
                    var datalogger = e.Object as DataLoggerModel.DataLoggerSettings;
                    AddPendingAggregateTables(datalogger.Name, datalogger.UseAggregatedTables ? AggregationTypes.CommandType.Add : AggregationTypes.CommandType.Remove);
                }
                else if (e.Object is UFUAModel.UFUACommunicationDriver)
                    bCanExecuteImportTagsDriver = null;
            };

            Document.CreateUndoRedoHelper();
            addressSpaceControl = new AddressSpace(Document);
            addressSpaceControl.ClearValue(FrameworkElement.WidthProperty);
            addressSpaceControl.ClearValue(FrameworkElement.HeightProperty);
            addressSpaceGrid.Children.Add(addressSpaceControl);

            configurationSettings.DataContext = Document.GetConfiguration();

            var listBaseAddresses = Document.GetConfiguration().BaseAddresses;
            TransportGrid.ItemsSource = listBaseAddresses;

            tabControlExt.SelectionChanging += (o, e) =>
            {
                if (bDisposed || Document == null)
                    return;

                EndEdit();
            };

            tabControlExt.SelectionChanged += (o, e) =>
                {
                    if (bDisposed || Document == null)
                        return;

                    if (e.OldSelectedIndex != e.NewSelectedIndex)
                        Document.CopyWinClipboardToInMemoryData();

                    if (e.OldSelectedItem == tabAddressSpace)
                    {
                        if (addressSpaceControl != null)
                            addressSpaceControl.OnDeactivate();
                    }

                    if (e.NewSelectedItem == tabGeneralSettings)
                    // if (e.AddedItems.Contains(tabGeneralSettings))
                    {
                        // e.Handled = true;
                        Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(configurationSettings.DataContext);
                    }
                    else if (e.NewSelectedItem == tabDrivers)
                    // else if (e.AddedItems.Contains(tabDrivers))
                    {
                        // e.Handled = true;
                        if (driverProjectList == null)
                        {
                            using (new WaitCursor())
                            {
                                driverProjectList = new DriverProjectList(Document);
                                driverProjectList.ClearValue(FrameworkElement.WidthProperty);
                                driverProjectList.ClearValue(FrameworkElement.HeightProperty);
                                driverGrid.Children.Add(driverProjectList);
                            }
                        }
                        driverProjectList.OnActivate();
                    }
                    else if (e.NewSelectedItem == tabRedundancy)
                    // else if (e.AddedItems.Contains(tabRedundancy))
                    {
                        // e.Handled = true;
                        if (redundancySettings == null)
                        {
                            using (new WaitCursor())
                            {
                                redundancySettings = new RedundancySettings(Document);
                                redundancySettings.ClearValue(FrameworkElement.WidthProperty);
                                redundancySettings.ClearValue(FrameworkElement.HeightProperty);
                                redundancyGrid.Children.Add(redundancySettings);
                            }
                        }
                        redundancySettings.OnActivate();
                    }
                    else if (e.NewSelectedItem == tabAddressSpace)
                    // else if (e.AddedItems.Contains(tabAddressSpace))
                    {
                        // e.Handled = true;
                        if (addressSpaceControl == null)
                        {
                            using (new WaitCursor())
                            {
                                addressSpaceControl = new AddressSpace(Document);
                                addressSpaceControl.ClearValue(FrameworkElement.WidthProperty);
                                addressSpaceControl.ClearValue(FrameworkElement.HeightProperty);
                                addressSpaceGrid.Children.Add(addressSpaceControl);
                            }
                        }
                        addressSpaceControl.OnActivate();
                    }
                    else if (e.NewSelectedItem == tabPrototypes)
                    // else if (e.AddedItems.Contains(tabPrototypes))
                    {
                        // e.Handled = true;
                        if (prototypeList == null)
                        {
                            using (new WaitCursor())
                            {
                                prototypeList = new PrototypeList(Document);
                                prototypeList.ClearValue(FrameworkElement.WidthProperty);
                                prototypeList.ClearValue(FrameworkElement.HeightProperty);
                                prototypeGrid.Children.Add(prototypeList);
                            }
                        }
                        prototypeList.OnActivate();
                    }
                    else if (e.NewSelectedItem == tabViews)
                    // else if (e.AddedItems.Contains(tabViews))
                    {
                        // e.Handled = true;
                        if (viewList == null)
                        {
                            using (new WaitCursor())
                            {
                                viewList = new ViewList(Document);
                                viewList.ClearValue(FrameworkElement.WidthProperty);
                                viewList.ClearValue(FrameworkElement.HeightProperty);
                                viewGrid.Children.Add(viewList);
                            }
                        }
                        viewList.OnActivate();
                    }
                    else if (e.NewSelectedItem == tabAlarmPrototypes)
                    // else if (e.AddedItems.Contains(tabAlarmPrototypes))
                    {
                        // e.Handled = true;
                        if (alarmPrototypeList == null)
                        {
                            using (new WaitCursor())
                            {
                                alarmPrototypeList = new AlarmPrototypeList(Document);
                                alarmPrototypeList.ClearValue(FrameworkElement.WidthProperty);
                                alarmPrototypeList.ClearValue(FrameworkElement.HeightProperty);
                                alarmPrototypeGrid.Children.Add(alarmPrototypeList);
                            }
                        }
                        alarmPrototypeList.OnActivate();
                    }
                    else if (e.NewSelectedItem == tabHistoricalPrototypes)
                    // else if (e.AddedItems.Contains(tabHistoricalPrototypes))
                    {
                        // e.Handled = true;
                        if (historicalPrototypeList == null)
                        {
                            using (new WaitCursor())
                            {
                                historicalPrototypeList = new HistoricalPrototypeList(Document);
                                historicalPrototypeList.ClearValue(FrameworkElement.WidthProperty);
                                historicalPrototypeList.ClearValue(FrameworkElement.HeightProperty);
                                historicalPrototypeGrid.Children.Add(historicalPrototypeList);
                            }
                        }
                        historicalPrototypeList.OnActivate();
                    }
                    else if (e.NewSelectedItem == tabDataLoggerSettings)
                    // else if (e.AddedItems.Contains(tabDataLoggerSettings))
                    {
                        // e.Handled = true;
                        if (dataLoggerSettingsList == null)
                        {
                            using (new WaitCursor())
                            {
                                dataLoggerSettingsList = new DataLoggerSettingsList(Document);
                                dataLoggerSettingsList.ClearValue(FrameworkElement.WidthProperty);
                                dataLoggerSettingsList.ClearValue(FrameworkElement.HeightProperty);
                                dataLoggerSettingsGrid.Children.Add(dataLoggerSettingsList);
                            }
                        }
                        dataLoggerSettingsList.OnActivate();
                    }
                    else if (e.NewSelectedItem == tabEngineeringUnitPrototypes)
                    // else if (e.AddedItems.Contains(tabEngineeringUnitPrototypes))
                    {
                        // e.Handled = true;
                        if (engineeringUnitPrototypeList == null)
                        {
                            using (new WaitCursor())
                            {
                                engineeringUnitPrototypeList = new EngineeringUnitPrototypeList(Document);
                                engineeringUnitPrototypeList.ClearValue(FrameworkElement.WidthProperty);
                                engineeringUnitPrototypeList.ClearValue(FrameworkElement.HeightProperty);
                                engineeringUnitPrototypeGrid.Children.Add(engineeringUnitPrototypeList);
                            }
                        }
                        engineeringUnitPrototypeList.OnActivate();
                    }

                    /*
                    if (e.RemovedItems.Count > 0 || e.AddedItems.Count > 0)
                    {
                        TabItem oldItem = null;
                        TabItem newItem = null;
                        if (e.RemovedItems.Count > 0 && e.RemovedItems[0] is TabItem)
                            oldItem = e.RemovedItems[0] as TabItem;
                        if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem)
                            newItem = e.AddedItems[0] as TabItem;
                        OnSelectionTabChanged(oldItem, newItem);
                    }
                     * */
                    OnSelectionTabChanged(e.OldSelectedItem, e.NewSelectedItem);
                };
        }

        //internal void ActivateTempVariableTab()
        //{
        //    tabTempVariables.IsSelected = true;
        //    WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
        //    if (tempVariablesControl is ITempVarControl)
        //        (tempVariablesControl as ITempVarControl).ClearSelection();
        //}

        internal void AddPrototypeList(List<IXPSimpleObject> list)
        {
            SetSelectedTab(DocManagerType.PrototypeScriptCode);
            WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
            prototypeList.AddPrototypeList(list);
        }

        internal void RemoveImportedPrototypes()
        {
            SetSelectedTab(DocManagerType.PrototypeScriptCode);
            WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
            prototypeList.UndoAction();            
        }

        internal void UpdateUserControl()
        {
            prototypeList.RefreshTreeControl();
        }

        internal void RemovePrototype(IXPSimpleObject obj)
        {
            SetSelectedTab(DocManagerType.PrototypeScriptCode);
            WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
            prototypeList.RemovePrototype(obj);
        }

        internal void AddTagFolderList(List<IXPSimpleObject> list)
        {
            SetSelectedTab(DocManagerType.AddressSpaceScriptCode);
            WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
            addressSpaceControl.AddTagFolderList(list);
        }

        internal void SetSelectedTab(DocManagerType e)
        {
            switch (e)
            {
                case DocManagerType.PrototypeScriptCode:
                    tabPrototypes.IsSelected = true;
                    break;
                case DocManagerType.AddressSpaceScriptCode:
                    tabAddressSpace.IsSelected = true;
                    break;
                case DocManagerType.HistoricalPrototypes:
                    tabHistoricalPrototypes.IsSelected = true;
                    break;
                case DocManagerType.AlarmPrototype:
                    tabAlarmPrototypes.IsSelected = true;
                    break;
                case DocManagerType.AlarmThresholds:
                    tabAddressSpace.IsSelected = true;
                    break;
                case DocManagerType.MessagePrototype:
                    tabAlarmPrototypes.IsSelected = true;
                    break;
                case DocManagerType.DataLoggerSettings:
                    tabDataLoggerSettings.IsSelected = true;
                    break;
                case DocManagerType.AuditingConn:
                case DocManagerType.EventConn:
                case DocManagerType.HistorianConn:
                    tabGeneralSettings.IsSelected = true;
                    break;
                case DocManagerType.Driver:
                    tabDrivers.IsSelected = true;
                    break;
                case DocManagerType.EUnits:
                    tabEngineeringUnitPrototypes.IsSelected = true;
                    break;
                default:
                    break;
            }
        }

        internal void RestoreComponents()
        {
            foreach (var item in tabControlExt.Items)
            {
                //if (item == tabTempVariables && !ComponentService.UFUAEditorManagerComponent.ufuaEditorManagerComponent.TempVarLoaded)
                //    tabTempVariables.Visibility = Visibility.Collapsed;
                //else
                    ((DXTabItem)item).Visibility = Visibility.Visible;
            }
        }

        internal void HideComponents()
        {
            foreach (var item in tabControlExt.Items)
            {
                //if (item == tabTempVariables && !ComponentService.UFUAEditorManagerComponent.ufuaEditorManagerComponent.TempVarLoaded)
                //    tabTempVariables.Visibility = Visibility.Collapsed;
                //else
                {
                    var tab = (DXTabItem)item;
                    if (tab.Tag as ControlTabEnum? == null || !UFUAEditorManagerComponent.TabsScheme.ContainsKey((ControlTabEnum)tab.Tag))
                        continue;

                    var bIsTabHidden = Document.EditorManagerComponent.Workspace.IsComponentHidden(UFUAEditorManagerComponent.TabsScheme[(ControlTabEnum)tab.Tag]);
                    tab.Visibility = bIsTabHidden ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        internal void OnDeactivate()
        {
            isActive = false;

            if (tabAddressSpace.IsSelected)
                addressSpaceControl.OnDeactivate();
            //else if (tabPrototypes.IsSelected)
            //    prototypeList.OnDeactivate();
            //else if (tabViews.IsSelected)
            //    viewList.OnDeactivate();
            //else if (tabHistoricalPrototypes.IsSelected)
            //    historicalPrototypeList.OnDeactivate();
            //else if (tabDataLoggerSettings.IsSelected)
            //    dataLoggerSettingsList.OnDeactivate();
            //else if (tabEngineeringUnitPrototypes.IsSelected)
            //    engineeringUnitPrototypeList.OnDeactivate();
            //else if (tabAlarmPrototypes.IsSelected)
            //    alarmPrototypeList.OnDeactivate();
            //else if (tabDrivers.IsSelected)
            //    driverProjectList.OnDeactivate();
            //else if (tabRedundancy.IsSelected)
            //    redundancySettings.OnDeactivate();
            //else if (tabTempVariables.IsSelected)
            //{
            //    if (tempVariablesControl is ITempVarControl)
            //        (tempVariablesControl as ITempVarControl).OnDeactivate();
            //}
        }
        
        internal void OnActivate()
        {
            isActive = true;
            Document.CopyWinClipboardToInMemoryData();

            if (tabAddressSpace.IsSelected)
                addressSpaceControl.OnActivate();
            else if (tabPrototypes.IsSelected)
                prototypeList.OnActivate();
            else if (tabViews.IsSelected)
                viewList.OnActivate();
            else if (tabHistoricalPrototypes.IsSelected)
                historicalPrototypeList.OnActivate();
            else if (tabDataLoggerSettings.IsSelected)
                dataLoggerSettingsList.OnActivate();
            else if (tabEngineeringUnitPrototypes.IsSelected)
                engineeringUnitPrototypeList.OnActivate();
            else if (tabAlarmPrototypes.IsSelected)
                alarmPrototypeList.OnActivate();
            else if (tabDrivers.IsSelected)
                driverProjectList.OnActivate();
            else if (tabRedundancy.IsSelected)
                redundancySettings.OnActivate();
            //else if (tabTempVariables.IsSelected)
            //{
            //    if (tempVariablesControl is ITempVarControl)
            //        (tempVariablesControl as ITempVarControl).OnActivate();
            //}
            else
                Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(configurationSettings.DataContext);
        }

        private void parentWindow_Activated(object sender, EventArgs e)
        {
            if (isActive)
            {
                Document.CopyWinClipboardToInMemoryData();
            }
        }

        #region IEditableObject Members
        bool bEditingUow;
        public void BeginEdit()
        {
            var currentTabEditor = GetSelectedTabEditor();
            if (currentTabEditor is IEditableObject)
                (currentTabEditor as IEditableObject).BeginEdit();
            else if (!bEditingUow && Document.UowContext != null)
            {
                bEditingUow = true;
                Document.UowContext.BeforeFlushChanges += uowContext_BeforeFlushChanges;
            }
        }

        public void CancelEdit()
        {
            var currentTabEditor = GetSelectedTabEditor();
            if (currentTabEditor is IEditableObject)
                (currentTabEditor as IEditableObject).CancelEdit();
            else if (bEditingUow && Document.UowContext != null)
            {
                bEditingUow = false;
                Document.UowContext.RollbackTransaction();
                Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
            }
        }

        public void EndEdit()
        {
            bool bResult = true;
            var currentTabEditor = GetSelectedTabEditor();
            if (currentTabEditor is IEditableObject)
                (currentTabEditor as IEditableObject).EndEdit();
            else if (bEditingUow && Document.UowContext != null)
            {
                bEditingUow = false;
                bResult = Document.UowContext.TryCommitChanges(UFUAServerDocument.logGeneral);
                Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
                if (bResult)
                    Document.NeedsSave = true;
            }

            if (!bResult)
            {
                var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(Properties.Resources.ErrorOnApplyingChanges);
            }
        }

        private void uowContext_BeforeFlushChanges(object sender, DevExpress.Xpo.SessionManipulationEventArgs e)
        {
            var currentTabEditor = GetSelectedTabEditor();
            if (currentTabEditor != null)
            {
                var objects = Document.EditorManagerComponent.Workspace.ContextObjects;
                if (objects == null && Document.EditorManagerComponent.Workspace.ContextObject != null)
                    objects = new List<object>() { Document.EditorManagerComponent.Workspace.ContextObject };
                if (objects != null)
                {
                    var list = new List<DevExpress.Xpo.IXPSimpleObject>();
                    foreach (var obj in objects)
                    {
                        var parent = Document.UowContext.GetParentObject(obj);
                        if (parent != null)
                            list.Add(parent as DevExpress.Xpo.IXPSimpleObject);

                        if (parent is UFUAModel.UFUAEngineeringUnit &&
                            obj is UFUAModel.UFUAEngineeringUnit)
                        {
                            var oldObject = parent as UFUAModel.UFUAEngineeringUnit;
                            var newObject = obj as UFUAModel.UFUAEngineeringUnit;
                            if (oldObject.Name != newObject.Name)
                                Document.EngineeringUnitsNameReplace(oldObject.Name, newObject.Name);
                        }
                        else if (parent is UFUAModel.UFUAHistorianSettings &&
                            obj is UFUAModel.UFUAHistorianSettings)
                        {
                            var oldObject = parent as UFUAModel.UFUAHistorianSettings;
                            var newObject = obj as UFUAModel.UFUAHistorianSettings;
                            if (oldObject.Name != newObject.Name)
                                Document.HistoricalSettingsNameReplace(oldObject.Name, newObject.Name);
                        }
                    }

                    if (list.Count > 0)
                        Document.AddUndoAction(currentTabEditor, list, UndoRedoAction.Changed);
                }
            }
        }

        #endregion IEditableObject Members

        #region Aggregated Tables

        internal void AddPendingAggregateTables(String datalogger, AggregationTypes.CommandType commandType)
        {
            lock (pendingAggregateDataLoggers)
            {
                if (!pendingAggregateDataLoggers.ContainsKey(commandType))
                    pendingAggregateDataLoggers.Add(commandType, new List<String>());
                if (!pendingAggregateDataLoggers[commandType].Contains(datalogger))
                    pendingAggregateDataLoggers[commandType].Add(datalogger);
                if (dpAggregateTables == null ||
                    dpAggregateTables.Status == DispatcherOperationStatus.Completed ||
                    dpAggregateTables.Status == DispatcherOperationStatus.Aborted)
                {
                    dpAggregateTables = Dispatcher.BeginInvokeAsynchronouslyInBackground(() => AskForAggregationTables());
                }
            }
        }

        void AskForAggregationTables()
        {
            foreach (AggregationTypes.CommandType commandType in Enum.GetValues(typeof(AggregationTypes.CommandType)))
            {
                List<String> dataloggers = null;
                lock (pendingAggregateDataLoggers)
                {
                    if (!pendingAggregateDataLoggers.ContainsKey(commandType))
                        continue;

                    dataloggers = pendingAggregateDataLoggers[commandType].ToList();
                    pendingAggregateDataLoggers.Remove(commandType);
                }

                if (dataloggers.Count == 0)
                    return;

                UIMsgBoxAlertService.ComponentService.CustomDialogResults dialogResult = UIMsgBoxAlertService.ComponentService.CustomDialogResults.No;
                var uiinterface = UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface;
                if (uiinterface != null)
                {
                    if (commandType == AggregationTypes.CommandType.Add)
                        dialogResult = uiinterface.ShowYesNo(Properties.Resources.AskAddAggregatedTables.Replace("-newline-", Environment.NewLine), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                    else if (commandType == AggregationTypes.CommandType.Remove)
                        dialogResult = uiinterface.ShowYesNo(Properties.Resources.AskRemoveAggregatedTables.Replace("-newline-", Environment.NewLine), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                    else if (commandType == AggregationTypes.CommandType.Update)
                        dialogResult = uiinterface.ShowYesNo(Properties.Resources.AskUpdateAggregatedTables.Replace("-newline-", Environment.NewLine), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                }

                if (dialogResult == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Yes)
                {
                    StartAggregateTables(dataloggers, commandType);
                }
            }
        }

        void StartAggregateTables(IList<String> dataloggers, AggregationTypes.CommandType commandType)
        {
            Document.EditorManagerComponent.Workspace.IsBusy = true;
            if (ctsAggregateTables == null)
                ctsAggregateTables = new CancellationTokenSource();
            var token = ctsAggregateTables.Token;
            var task = Task.Factory.StartNew(() =>
            {
                foreach (var datalogger in dataloggers)
                {
                    token.ThrowIfCancellationRequested();

                    AggregateTables(datalogger, commandType, silent: true);
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

            task.ContinueWith((result) =>
            {
                Document.EditorManagerComponent.Workspace.IsBusy = false;
                var uiinterface = UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface;
                if (result.Exception != null && uiinterface != null)
                    uiinterface.ShowError(String.Format(Properties.Resources.ErrorOnAggregatedTables.Replace("-newline-", Environment.NewLine), result.Exception.InnerException.Message));
            }, token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void AggregateTables(String dataloggerName, AggregationTypes.CommandType commandType, bool silent = false)
        {
            var datalogger = Document.GetDataLogger(dataloggerName);
            var helper = new DataLoggerModel.Helpers.DataLoggerSettingsHelper(datalogger, Document.rootBase);

            foreach (var col in helper.DataLoggerSettings.Columns)
            {
                if (col.ColumnTag != null && !col.ColumnTag.IsEmpty())
                    col.UFUATagReference = Document.FindTagByEntityReference(col.ColumnTag);
            }

            DataReader.DataReaderModel model = helper.DataLoggerSettings.ConnectionSettings;
            if (model == null || model.IsEmpty())
            {
                String connectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(Document.GetConfiguration().HistorianDefaultConnection, Document.rootBase);
                model = new DataReader.DataReaderModel()
                {
                    DataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(connectionString),
                    Connection = XpoConversionHelper.GetConnectionStringFromXpoConnection(connectionString)
                };
            }

            var columns = (from c in helper.DataLoggerSettings.Columns.AsParallel()
                           where c.UFUATagReference != null
                           select c.Name).ToArray();

            CreateDataLogger(model, helper);

            string arguments = string.Format(UFUAServerInfo.Properties.Settings.Default.SQLDatabaseConfigurationArgs, /*"/O{0}" "/P{1}" ""/D"{2}" "/T{3}" "/U{4}" "/L{5}" "/C{6}" "/H{7}"*/
                    (int)AggregationTypes.OperationType.AggregatesTables,
                    (int)commandType,
                    model.GetFullConnection(),
                    helper.TableName,
                    helper.UtcTimeColumnName,
                    helper.LocalTimeColumnName,
                    String.Join(",", columns),
                    helper.MillisecondsColumnName);

            if (silent)
                arguments = string.Format("/S {0}", arguments);
            else
                arguments = string.Format("/I{1} {0}", arguments, System.Diagnostics.Process.GetCurrentProcess().Id);

            string path = UFUAServerInfo.Properties.Settings.Default.SQLDatabaseConfigurationTool; /*"SQLDatabaseConfiguration.exe"*/
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

            var startInfo = new System.Diagnostics.ProcessStartInfo(path, arguments)
            {
                RedirectStandardError = silent,
                RedirectStandardOutput = silent,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (silent)
            {
                using (var process = new System.Diagnostics.Process())
                {
                    List<Exception> exceptions = null;
                    process.StartInfo = startInfo;
                    process.ErrorDataReceived += (o, e) =>
                    {
                        if (!String.IsNullOrEmpty(e.Data))
                        {
                            var message = e.Data.Replace("-newline-", Environment.NewLine);
                            if (exceptions == null)
                                exceptions = new List<Exception>();
                            exceptions.Add(new Exception(message));
                            logGeneral.Error(message);
                        }
                    };
                    process.OutputDataReceived += (o, e) =>
                    {
                        if (!String.IsNullOrEmpty(e.Data))
                        {
                            var message = e.Data.Replace("-newline-", Environment.NewLine);
                            logGeneral.Info(message);
                        }
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();

                    if (exceptions != null)
                    {
                        if (exceptions.Count == 1)
                            throw exceptions[0];
                        else if (exceptions.Count > 1)
                            throw new AggregateException(exceptions);
                    }
                }
            }
            else
                System.Diagnostics.Process.Start(startInfo);
        }

        static void CreateDataLogger(DataReader.DataReaderModel model, DataLoggerModel.Helpers.DataLoggerSettingsHelper settingsHelper)
        {
            using (var writer = new DataWriter.DataSetWriter(model.DataProvider, model.Connection))
            {
                if (!writer.TryOpenConnection())
                {
                    writer.CreateDataBase();
                    writer.OpenConnection();
                }

                var dataSet = new DataSet();
                dataSet.Tables.Add(settingsHelper.CreateDataTable());
                writer.CheckTables(dataSet, settingsHelper.DataLoggerSettings.SkipCheckColumnsType);
            }
        }
        #endregion

        internal UserControl GetSelectedTabEditor()
        {
            UserControl currentTabEditor = null;
            if (tabAddressSpace.IsSelected)
                currentTabEditor = addressSpaceControl;
            else if (tabPrototypes.IsSelected)
                currentTabEditor = prototypeList;
            else if (tabViews.IsSelected)
                currentTabEditor = viewList;
            else if (tabHistoricalPrototypes.IsSelected)
                currentTabEditor = historicalPrototypeList;
            else if (tabDataLoggerSettings.IsSelected)
                currentTabEditor = dataLoggerSettingsList;
            else if (tabEngineeringUnitPrototypes.IsSelected)
                currentTabEditor = engineeringUnitPrototypeList;
            else if (tabAlarmPrototypes.IsSelected)
                currentTabEditor = alarmPrototypeList;
            else if (tabDrivers.IsSelected)
                currentTabEditor = driverProjectList;
            //else if (tabTempVariables.IsSelected)
            //    currentTabEditor = tempVariablesControl;

            return currentTabEditor;
        }

        public event EventHandler<ControlTabChangedEventArgs> SelectionTabChanged;
        #region OnSelectionTabChanged
        /// <summary>
        /// Triggers the SelectionTabChanged event.
        /// </summary>
        void OnSelectionTabChanged(Object oldItem, Object newItem)
        {
            var e = SelectionTabChanged;
            if (e != null)
            {
                var args = new ControlTabChangedEventArgs() 
                { 
                    OldTabId = ControlTabEnum.None,
                    NewTabId = ControlTabEnum.None
                };

                if (oldItem != null)
                {
                    if (oldItem == tabAddressSpace)
                        args.OldTabId = ControlTabEnum.AddressSpace;
                    else if (oldItem == tabGeneralSettings)
                        args.OldTabId = ControlTabEnum.GeneralSettings;
                    else if (oldItem == tabDrivers)
                        args.OldTabId = ControlTabEnum.Drivers;
                    else if (oldItem == tabPrototypes)
                        args.OldTabId = ControlTabEnum.Prototypes;
                    else if (oldItem == tabViews)
                        args.OldTabId = ControlTabEnum.Views;
                    else if (oldItem == tabHistoricalPrototypes)
                        args.OldTabId = ControlTabEnum.Historicals;
                    else if (oldItem == tabDataLoggerSettings)
                        args.OldTabId = ControlTabEnum.DataLoggers;
                    else if (oldItem == tabAlarmPrototypes)
                        args.OldTabId = ControlTabEnum.Alarms;
                    else if (oldItem == tabEngineeringUnitPrototypes)
                        args.OldTabId = ControlTabEnum.EngineeringUnits;
                    //else if (oldItem == tabTempVariables)
                    //    args.OldTabId = ControlTabEnum.TempVariables;
                    else if (oldItem == tabRedundancy)
                        args.OldTabId = ControlTabEnum.Redundancy;
                }

                if (newItem != null)
                {
                    if (newItem == tabAddressSpace)
                        args.NewTabId = ControlTabEnum.AddressSpace;
                    else if (newItem == tabGeneralSettings)
                        args.NewTabId = ControlTabEnum.GeneralSettings;
                    else if (newItem == tabDrivers)
                        args.NewTabId = ControlTabEnum.Drivers;
                    else if (newItem == tabPrototypes)
                        args.NewTabId = ControlTabEnum.Prototypes;
                    else if (newItem == tabViews)
                        args.NewTabId = ControlTabEnum.Views;
                    else if (newItem == tabHistoricalPrototypes)
                        args.NewTabId = ControlTabEnum.Historicals;
                    else if (newItem == tabDataLoggerSettings)
                        args.NewTabId = ControlTabEnum.DataLoggers;
                    else if (newItem == tabAlarmPrototypes)
                        args.NewTabId = ControlTabEnum.Alarms;
                    else if (newItem == tabEngineeringUnitPrototypes)
                        args.NewTabId = ControlTabEnum.EngineeringUnits;
                    //else if (newItem == tabTempVariables)
                    //    args.NewTabId = ControlTabEnum.TempVariables;
                    else if (newItem == tabRedundancy)
                        args.NewTabId = ControlTabEnum.Redundancy;
                }

                e(this, args);
            }
        }
        #endregion

        #region Properties

        UFUAServerDocument _Document;
        [Browsable(false)]
        public UFUAServerDocument Document
        {
            get
            {
                return _Document;
            }
            private set
            {
                _Document = value;
            }
        }

        #endregion

        #region Command Handlers
        //private void OnAddNewTempFolder(object sender, ExecutedRoutedEventArgs e)
        //{
        //    e.Handled = true;
        //    if (tempVariablesControl is ITempVarControl)
        //        (tempVariablesControl as ITempVarControl).AddNewFolder();
        //}
        //private void CanAddNewTempFolder(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute =  ComponentService.UFUAEditorManagerComponent.ufuaEditorManagerComponent.TempVarLoaded && tabTempVariables.IsSelected;
        //}
        //private void OnAddNewTempTag(object sender, ExecutedRoutedEventArgs e)
        //{
        //    e.Handled = true;
        //    if (!tabTempVariables.IsSelected)
        //    {
        //        tabTempVariables.IsSelected = true;
        //        WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
        //    }
        //    else
        //        tabTempVariables.IsSelected = true;
        //    if (tempVariablesControl is ITempVarControl)
        //    {
        //        (tempVariablesControl as ITempVarControl).AddNewTag();
        //    }
        //}

        //private void CanAddNewTempTag(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = ComponentService.UFUAEditorManagerComponent.ufuaEditorManagerComponent.TempVarLoaded /*&& tabTempVariables.IsSelected*/;
        //}
        private void OnAddNewTag(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!tabAddressSpace.IsSelected && !tabPrototypes.IsSelected)
                tabAddressSpace.IsSelected = true;
            UFUAModel.UFUATag tag = null;
            if (tabAddressSpace.IsSelected)
            {
                tag = Document.AddNewTag(addressSpaceControl.GetSelectedFolder());
            }
            else
            {
                var prototypeListGetSelectedFolder = prototypeList.GetSelectedFolder();
                if (prototypeListGetSelectedFolder != null)
                {
                    var prototypeListGetSelectedPrototype = prototypeList.GetSelectedPrototype();
                    tag = Document.AddNewTag(prototypeListGetSelectedFolder, true, /*prototypeListGetSelectedPrototype != null ?*/ Document.NewMemberOrderId(prototypeListGetSelectedFolder)/*prototypeListGetSelectedPrototype) : -1*/);
                }
                else
                {
                    var prototypeListGetSelectedPrototype = prototypeList.GetSelectedPrototype();
                    if (prototypeListGetSelectedPrototype != null)
                        tag = Document.AddNewTag(prototypeListGetSelectedPrototype, Document.NewMemberOrderId(prototypeListGetSelectedPrototype));
                }
            }

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                if (tabAddressSpace.IsSelected)
                {
                    addressSpaceControl.AddTag(tag);
                }
                else
                {
                    prototypeList.AddTag(tag);
                }
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            //using (var t = new TemporaryContextObject(Document.EditorManagerComponent.Workspace, tag))
            {

                var newTagControl = new NewTag(Document, tabPrototypes.IsSelected)
                {
                    DataContext = tag
                };

                GeneralDialogContent Dialog = new GeneralDialogContent(newTagControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewTag"
                };

                if (Dialog.ShowDialog() == true)
                {
                    if (tabAddressSpace.IsSelected)
                    {
                        addressSpaceControl.AddTag(tag);
                    }
                    else
                    {
                        prototypeList.AddTag(tag);
                    }
                }
                else
                {
                    tag.NodeId = Guid.Empty;
                    tag.Delete();
                };
            }
        }

        private void CanImportExportAddressSpace(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnImportExportAddressSpace(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var ret = Document.ServerImportExport<UFUAModel.ConfigurationBase>();
            DataReadyToUpdate<UFUAModel.ConfigurationBase>(ret);
        }

        private void CanAddNewTag(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabPrototypes.IsSelected && prototypeList.IsAnyItemSelected() ||
                !tabPrototypes.IsSelected;
        }

        private void CanImportExportPrototypes(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabPrototypes.IsSelected;
        }

        private void OnImportExportPrototypes(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var ret = Document.ServerImportExport<UFUAModel.UFUATagPrototype>();
            DataReadyToUpdate<UFUAModel.UFUATagPrototype>(ret);
        }

        private void CanImportExportAlarms(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabAlarmPrototypes.IsSelected;
        }

        private void OnImportExportAlarms(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var ret = Document.ServerImportExport<UFUAModel.UFUAAlarmDefinition>();
            DataReadyToUpdate<UFUAModel.UFUAAlarmDefinition>(ret);
        }

        private void CanImportExportHistoricals(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabHistoricalPrototypes.IsSelected;
        }

        private void OnImportExportHistoricals(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var ret = Document.ServerImportExport<UFUAModel.UFUAHistorianSettings>();
            DataReadyToUpdate<UFUAModel.UFUAHistorianSettings>(ret);
        }
        private void CanImportExportDataloggers(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabDataLoggerSettings.IsSelected;
        }

        private void OnImportExportDataloggers(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var ret = Document.ServerImportExport<DataLoggerModel.DataLoggerSettings>();
            DataReadyToUpdate<DataLoggerModel.DataLoggerSettings>(ret);
        }
        private void CanImportExportEUnits(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabEngineeringUnitPrototypes.IsSelected; 
        }

        private void OnImportExportEUnits(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var ret = Document.ServerImportExport<UFUAModel.UFUAEngineeringUnit>();
            DataReadyToUpdate<UFUAModel.UFUAEngineeringUnit>(ret);
        }

        private void CanImportExportTags(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnImportExportTags(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var ret = Document.ServerImportExport<UFUAModel.UFUATag>();
            DataReadyToUpdate<UFUAModel.UFUATag>(ret);
        }

        private void DataReadyToUpdate<T>(WPFUtilities.ImportExportHelpers.ImportExportResult result)
        {
            Document.EditorManagerComponent.Workspace.IsBusy = true;
            try
            {
                if (typeof(T) == typeof(UFUAModel.UFUATagPrototype))
                {
                    tabPrototypes.IsSelected = true;
                    prototypeList.treeListControl.ClearSelection();
                }
                else if (typeof(T) == typeof(DataLoggerModel.DataLoggerSettings))
                {
                    tabDataLoggerSettings.IsSelected = true;
                    dataLoggerSettingsList.treeListControl.ClearSelection();
                }
                else if (typeof(T) == typeof(UFUAModel.UFUAAlarmDefinition))
                {
                    tabAlarmPrototypes.IsSelected = true;
                    alarmPrototypeList.treeListControl.ClearSelection();
                }
                else if (typeof(T) == typeof(UFUAModel.ConfigurationBase))
                {
                    tabAddressSpace.IsSelected = true;
                    addressSpaceControl.treeListControl.ClearSelection();
                }
                else if (typeof(T) == typeof(UFUAModel.UFUATag))
                {
                    tabAddressSpace.IsSelected = true;
                    addressSpaceControl.treeListControl.ClearSelection();
                }
                else if (typeof(T) == typeof(UFUAModel.UFUAEngineeringUnit))
                {
                    tabEngineeringUnitPrototypes.IsSelected = true;
                    engineeringUnitPrototypeList.treeListControl.ClearSelection();
                }
                else if (typeof(T) == typeof(UFUAModel.UFUAHistorianSettings))
                {
                    tabHistoricalPrototypes.IsSelected = true;
                    historicalPrototypeList.treeListControl.ClearSelection();
                }
                else if (typeof(T) == typeof(UFUAModel.UFUAView))
                {
                    tabViews.IsSelected = true;
                    viewList.treeListControl.ClearSelection();
                }

                if (result.AddedObjects != null && result.AddedObjects.Count > 0)
                    Document.OnChangedDocument(this, ChangedType.added, result.AddedObjects);
                if (result.ChangedObjects != null && result.ChangedObjects.Count > 0)
                    Document.OnChangedDocument(this, ChangedType.changed, result.ChangedObjects);
                if (result.RemovedObjects != null && result.RemovedObjects.Count > 0)
                    Document.OnChangedDocument(this, ChangedType.removed, result.RemovedObjects);
            }
            finally
            {
                Document.EditorManagerComponent.Workspace.IsBusy = false;
            }

            if (result.WasError)
                ShowSystemLog();
            else if (result.WasImporting)
            {
                var uiMsgBox = Document.EditorManagerComponent.UIInterface;
                if (uiMsgBox != null)
                {
                    uiMsgBox.ShowInformation(Properties.Resources.OperationCompleted);
                }
            }
        }

        private void ShowSystemLog()
        {
            var uiMsgBox = Document.EditorManagerComponent.UIInterface;
            if (uiMsgBox != null)
            {
                if (uiMsgBox.ShowYesNo(Properties.Resources.ImportExportGenericError, CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                {
                    Dispatcher.BeginInvokeInBackgroundIfRequired(() => Document.EditorManagerComponent?.Workspace.ShowSystemLog());
                }
            }
        }

        private void OnAddNewPrototype(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabPrototypes.IsSelected = true;

            var prototype = Document.AddNewPrototype();
            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                prototypeList.AddPrototype(prototype);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newPrototypeControl = new NewPrototype()
                {
                    DataContext = prototype
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newPrototypeControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewPrototype"
                };
                if (Dialog.ShowDialog() == true)
                {
                    prototypeList.AddPrototype(prototype);
                }
                else
                {
                    prototype.NodeId = Guid.Empty;
                    prototype.Delete();
                }
            }
        }

        private void CanAddNewPrototype(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewFolder(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!tabAddressSpace.IsSelected && !tabPrototypes.IsSelected)
                tabAddressSpace.IsSelected = true;

            UFUAModel.UFUAFolder folder = null;
            if (tabAddressSpace.IsSelected)
            {
                folder = Document.AddNewFolder(addressSpaceControl.GetSelectedFolder());
            }
            else
            {
                var prototypeListGetSelectedFolder = prototypeList.GetSelectedFolder();
                if (prototypeListGetSelectedFolder != null)
                    folder = Document.AddNewFolder(prototypeListGetSelectedFolder, Document.NewFolderOrderId(prototypeListGetSelectedFolder));
                else
                {
                    var prototypeListGetSelectedPrototype = prototypeList.GetSelectedPrototype();
                    if (prototypeListGetSelectedPrototype != null)
                        folder = Document.AddNewFolder(prototypeListGetSelectedPrototype, Document.NewFolderOrderId(prototypeListGetSelectedPrototype));
                }
            }

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                if (tabAddressSpace.IsSelected)
                {
                    addressSpaceControl.AddFolder(folder);
                }
                else
                {
                    prototypeList.AddFolder(folder);
                }
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newFolderControl = new NewFolder()
                {
                    DataContext = folder
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newFolderControl, GeneralDialogButtons.OkCancelButtons)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = ""
                };
                if (Dialog.ShowDialog() == true)
                {
                    if (tabAddressSpace.IsSelected)
                    {
                        addressSpaceControl.AddFolder(folder);
                    }
                    else
                    {
                        prototypeList.AddFolder(folder);
                    }
                }
                else
                {
                    folder.Delete();
                }
            }
        }

        private void CanAddNewFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabAddressSpace.IsSelected)
            {
                e.CanExecute = addressSpaceControl.GetSelectedFolder() != null;
            }
            else if (prototypeList != null)
            {
                var prototypeListGetSelectedFolder = prototypeList.GetSelectedFolder();
                if (prototypeListGetSelectedFolder != null)
                    e.CanExecute = true;
                else
                {
                    var prototypeListGetSelectedPrototype = prototypeList.GetSelectedPrototype();
                    if (prototypeListGetSelectedPrototype != null)
                        e.CanExecute = true;
                }
            }
        }

        private void OnAddNewGeneralFolder(object sender, ExecutedRoutedEventArgs e)
        {
            //if (!tabTempVariables.IsSelected)
                OnAddNewFolder(sender, e);
            //else
            //{
            //    e.Handled = true;
            //    if (tempVariablesControl is ITempVarControl)
            //        (tempVariablesControl as ITempVarControl).AddNewFolder();
            //}
        }

        private void CanAddNewGeneralFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabAddressSpace.IsSelected)
            {
                e.CanExecute = addressSpaceControl.IsRootSelected() || addressSpaceControl.GetSelectedFolder() != null;
            }
            else if (tabPrototypes.IsSelected && prototypeList != null)
            {
                var prototypeListGetSelectedFolder = prototypeList.GetSelectedFolder();
                if (prototypeListGetSelectedFolder != null)
                    e.CanExecute = true;
                else
                {
                    var prototypeListGetSelectedPrototype = prototypeList.GetSelectedPrototype();
                    if (prototypeListGetSelectedPrototype != null)
                        e.CanExecute = true;
                }
            }
            //else if (tabTempVariables.IsSelected)
            //{
            //    e.CanExecute = ComponentService.UFUAEditorManagerComponent.ufuaEditorManagerComponent.TempVarLoaded;
            //}
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Document.SaveToFile();
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.NeedsSave;
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabAddressSpace.IsSelected)
                    addressSpaceControl.DeleteSelectedItems();
                else if (tabPrototypes.IsSelected)
                    prototypeList.DeleteSelectedItems();
                else if (tabViews.IsSelected)
                    viewList.DeleteSelectedItems();
                else if (tabHistoricalPrototypes.IsSelected)
                    historicalPrototypeList.DeleteSelectedItems();
                else if (tabDataLoggerSettings.IsSelected)
                    dataLoggerSettingsList.DeleteSelectedItems();
                else if (tabEngineeringUnitPrototypes.IsSelected)
                    engineeringUnitPrototypeList.DeleteSelectedItems();
                else if (tabAlarmPrototypes.IsSelected)
                    alarmPrototypeList.DeleteSelectedItems();
                else if (tabDrivers.IsSelected)
                    driverProjectList.DeleteSelectedItems();
                else if (tabRedundancy.IsSelected)
                    redundancySettings.DeleteSelectedItems();
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabAddressSpace.IsSelected && addressSpaceControl.IsAnyItemSelected() ||
                           tabPrototypes.IsSelected && prototypeList.IsAnyItemSelected() ||
                           tabViews.IsSelected && viewList.IsAnyItemSelected() ||
                           tabHistoricalPrototypes.IsSelected && historicalPrototypeList.IsAnyItemSelected() ||
                           tabDataLoggerSettings.IsSelected && dataLoggerSettingsList.IsAnyItemSelected() ||
                           tabAlarmPrototypes.IsSelected && alarmPrototypeList.IsAnyItemSelected() ||
                           tabEngineeringUnitPrototypes.IsSelected && engineeringUnitPrototypeList.IsAnyItemSelected() ||
                           tabDrivers.IsSelected && driverProjectList.IsAnyItemSelected() ||
                           tabRedundancy.IsSelected && redundancySettings.IsAnyItemSelected();
        }


        private void OnCertificateChecker(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabGeneralSettings.IsSelected = true;
            Document.CertificateChecker();
        }

        private void CanCertificateChecker(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void OnStartServer(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabGeneralSettings.IsSelected = true;
            try
            {
                if (Document.StartServer(txtevent, Scroll, bSave: true, manually: true))
                {
                    bool running = false;
#if !DEBUG
                    while (!running)
#endif
                    {
                        Document.EditorManagerComponent.Workspace.IsBusy = true;
                        running = await Document.ServerCMSHelperAsync.WaitServerStarted(10000);
                        Document.EditorManagerComponent.Workspace.IsBusy = false;
#if !DEBUG

                        if (running || 
                            Document.EditorManagerComponent.UIInterface == null ||
                            Document.EditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.WaitingStartServer, CustomDialogIcons.Question) == CustomDialogResults.No)
                            break;
#endif
                    }

                    // if (Keyboard.IsKeyDown(Key.LeftShift))
#if !DEBUG
                    if (running)
#endif
                    {
                        var now = DateTime.Now.AddSeconds(5);
                        while (now > DateTime.Now)
                        {
                            if (Document.EditorManagerComponent.opcUABrowser == null)
                                break;

                            var ret = Document.EditorManagerComponent.opcUABrowser.BrowseEndpoint(Document.GetDefaultLocalEndpoint());
                            if (ret == true)
                                break;
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Document.EditorManagerComponent.UIInterface != null)
                {
                    if (ex is System.ComponentModel.Win32Exception && Document.NeedToRunAsCFR21UserIndentity())
                    {
                        Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StartServerFailedByCFR21UserIdentity.Replace("'newline'", Environment.NewLine),
                            UFUAServerInfo.UFUAServerInfo.GetServerName(), UFUAServerInfo.UFUAServerInfo.GetCFR21UserNameSetting()));
                    }
                    else
                    {
                        Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StartServerFailed.Replace("'newline'", Environment.NewLine),
                            UFUAServerInfo.UFUAServerInfo.GetServerName(), ex.Message));
                    }
                }
            }
            finally
            {
                Document.EditorManagerComponent.Workspace.IsBusy = false;
            }
        }

        private void CanStartServer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Document.ServerCMSHelperAsync.IsServerRunning;
        }

        private async void OnStopServer(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabGeneralSettings.IsSelected = true;
            //viewStatus.IsMaximized = true;
            try
            {
                if (Document.ServerCMSHelperAsync.StopServer())
                {
                    bool stopped = false;
#if !DEBUG
                    while (!stopped)
#endif
                    {
                        Document.EditorManagerComponent.Workspace.IsBusy = true;
                        stopped = await Document.ServerCMSHelperAsync.WaitServerStopped(10000);
                        Document.EditorManagerComponent.Workspace.IsBusy = false;
#if !DEBUG
                        if (stopped ||
                            Document.EditorManagerComponent.UIInterface == null ||
                            Document.EditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.WaitingStopServer, CustomDialogIcons.Question) == CustomDialogResults.No)
                            break;
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                if (Document.EditorManagerComponent.UIInterface != null)
                {
                    Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StopServerFailed.Replace("'newline'", Environment.NewLine),
                        UFUAServerInfo.UFUAServerInfo.GetServerName(), ex.Message));
                }
            }
            finally
            {
                Document.EditorManagerComponent.Workspace.IsBusy = false;
            }
        }

        private void CanStopServer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = /*Document.ServerCMSHelperAsync.IsServerRunning || */Document.ServerCMSHelperAsync.IsServerStarted;
        }

        private void OnAddDriver(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabDrivers.IsSelected = true;

            //**********
            // NewDriverWizard

            ProjectViewModel NewProject
             = new ProjectViewModel
             {
                 //projectManagerService = projectManagerService,
                 //screenManagerService = screenManagerService,
                 UFUAEditorManager = Document.EditorManagerComponent,
                 //UIInterface = UIInterface,
                 //UriRisolver = UriRisolver
                 helpProvider = Document.EditorManagerComponent.HelpProvider
             };

            string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            var list = FindAndLoadDLL.LoadDLLs<IUFNewDriverWizard>(rootPath,
                                                        string.Format("{0}{1}", "NewDriverWizard", ".dll"), false);
            if (list.Count > 0)
            {
                list[0].Initialize();
                DriverXmlInfo driverInfo = list[0].ConfigureNewDriver(Document, NewProject, Document.ConnectionString) as DriverXmlInfo;
                if (driverInfo != null)
                {
                    var ld = (from c in Document.GetConfiguration().ComunicationDrivers where c.AssemblyName == driverInfo.AssemblyName select c).ToList();
                    if (ld.Count == 0)
                    {
                        var driver = Document.AddNewDriver();
                        driver.Factory = driverInfo.Factory;
                        driver.FriendlyName = driverInfo.FriendlyName;
                        driver.Name = driverInfo.AssemblyName.Substring(0, driverInfo.AssemblyName.Length - 4);
                        driver.AssemblyName = driverInfo.AssemblyName;

                        Document.GetConfiguration().ComunicationDrivers.Add(driver);
                        Document.AddUndoAction(driverProjectList, driver, UndoRedoAction.Added);

                        //var listDrivers = Document.GetConfiguration().ComunicationDrivers;
                        //gridDriverList.ItemsSource = listDrivers.ToList();

                        //gridDriverList.SelectedItem = driver;
                        driverProjectList.FlatGridRefresh(new List<UFUACommunicationDriver> { driver });
                    }
                }
            }

            //*********
            //var driverList = new DriverList();
            //GeneralDialogContent Dialog = new GeneralDialogContent(driverList)
            //{
            //    DialogKeepContent = true,
            //    Owner = this.FindParent<Window>()
            //};
            //if (Dialog.ShowDialog() == true)
            //{
            //    var driverInfo = driverList.GetSelectedDriverInfo();
            //    if (driverInfo != null)
            //    {
            //        var driver = Document.AddNewDriver();
            //        driver.Factory = driverInfo.Factory;
            //        driver.FriendlyName = driverInfo.FriendlyName;
            //        driver.Name = driverInfo.AssemblyName.Substring(0, driverInfo.AssemblyName.Length - 4);

            //        driver.Path = driverInfo.Path;
            //        driver.AssemblyName = driverInfo.AssemblyName;
                    
            //        Document.GetConfiguration().ComunicationDrivers.Add(driver);

            //        //var listDrivers = Document.GetConfiguration().ComunicationDrivers;
            //        //gridDriverList.ItemsSource = listDrivers.ToList();

            //        //gridDriverList.SelectedItem = driver;
            //        driverProjectList.gridDataControl.SelectedItem = driver;
            //    }
            //}
        }

        private void CanAddNewDriver(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnOpenDriverSettings(object sender, ExecutedRoutedEventArgs e)
        {
            driverProjectList.EditSelectedItem();
        }

        private void CanOpenDriverSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabDrivers.IsSelected && driverProjectList.IsAnyItemSelected();
        }

        private void OnImportTagsDriver(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            UFUAModel.UFUACommunicationDriver selected = null;
            if (tabDrivers.IsSelected)
                selected = driverProjectList.GetSelectedItem();
            if (selected == null || !selected.CanImportTags)
            {
                var drivers = (from c in Document.GetDrivers() where c.CanImportTags select c).ToList();
                if (drivers.Count == 1)
                    selected = drivers[0];
                else
                {
                    var controlDrivers = new DriverProjectList(Document, bPopup: true, bFilterCanImportTags: true);
                    GeneralDialogContent dialog = new GeneralDialogContent(controlDrivers)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.BrowseDriversTitle,
                        HelpLink = "SelectDriverForImportTags"
                    };
                    if (dialog.ShowDialog() == true)
                        selected = controlDrivers.GetSelectedItem();
                }
            }
            if (selected != null && selected.CanImportTags)
            {
                addressSpaceControl.ImportDriverTags(selected);
            }
        }

        private void CanImportTagsDriver(object sender, CanExecuteRoutedEventArgs e)
        {
            UFUAModel.UFUACommunicationDriver selected = null;
            if (tabDrivers.IsSelected)
            {
                selected = driverProjectList.GetSelectedItem();
                e.CanExecute = selected != null && selected.CanImportTags;
            }

            if (selected == null)
            {
                if (!bCanExecuteImportTagsDriver.HasValue)
                {
                    bCanExecuteImportTagsDriver = (from drv in Document.GetDrivers().ToList().AsParallel()
                                                   where drv.CanImportTags
                                                   select drv.CanImportTags).FirstOrDefault();
                    e.CanExecute = bCanExecuteImportTagsDriver.Value;
                }
                else
                    e.CanExecute = bCanExecuteImportTagsDriver.Value;
            }
        }
        
        private void OnAddNewHistoricalSettings(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabHistoricalPrototypes.IsSelected = true;
            var hs = Document.AddNewHistoricalSettings();

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                historicalPrototypeList.AddHistorianModel(hs);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newHistoricalControl = new NewHistoricalSettings(Document)
                {
                    DataContext = hs
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newHistoricalControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewHistoricalSettings"
                };
                if (Dialog.ShowDialog() == true)
                {
                    historicalPrototypeList.AddHistorianModel(hs);
                }
                else
                {
                    hs.Delete();
                }
            }
        }

        private void CanAddNewHistoricalSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAssignHistoricalSettings(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var historicalList = new HistoricalPrototypeList(Document, true);
            GeneralDialogContent Dialog = new GeneralDialogContent(historicalList)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "AssignHistoricalSettings"
            };
            if (Dialog.ShowDialog() == true)
            {
                var hs = historicalList.GetSelectedItem();
                if (hs != null)
                {
                    if (tabAddressSpace.IsSelected)
                        addressSpaceControl.AssignItemToSelect(hs);
                    else if (tabPrototypes.IsSelected)
                        prototypeList.AssignItemToSelect(hs);
                }
            }
        }

        private void CanAssignHistoricalSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabAddressSpace.IsSelected && addressSpaceControl.IsAnyItemSelected() &&
                            addressSpaceControl.CanAssignItem() ||
                           tabPrototypes.IsSelected && prototypeList.IsAnyItemSelected() &&
                            prototypeList.CanAssignItem();
        }

        private void OnAddNewDataLoggerSettings(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabDataLoggerSettings.IsSelected = true;
            var dataloggerSettings = Document.AddNewDataLoggerSettings();

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                dataLoggerSettingsList.AddDataLoggerSettings(dataloggerSettings);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newDataLoggerSettings = new NewDataLoggerSettings(Document)
                {
                    DataContext = dataloggerSettings
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newDataLoggerSettings)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewDataLoggerSettings"
                };
                if (Dialog.ShowDialog() == true)
                {
                    dataLoggerSettingsList.AddDataLoggerSettings(dataloggerSettings);
                }
                else
                {
                    dataloggerSettings.Delete();
                }
            }
        }

        private void CanAddNewDataLoggerSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAggregateTables(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            tabDataLoggerSettings.IsSelected = true;
            List<String> dataloggers = null;
            var uiinterface = UFUAEditorManagerComponent.ufuaEditorManagerComponent.UIInterface;
            var selecteds = dataLoggerSettingsList.GetSelectedDataLoggers();
            if (selecteds != null && selecteds.Count > 0)
            {
                dataloggers = (from c in selecteds where c.UseAggregatedTables select c.Name).ToList();
                if (dataloggers.Count > 0)
                    StartAggregateTables(dataloggers, AggregationTypes.CommandType.Update);
            }

            if (uiinterface != null && (dataloggers == null || dataloggers.Count == 0))
                uiinterface.ShowInformation(Properties.Resources.NotAnyAggregatedTablesSelected);
        }

        private void CanAggregateTables(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabDataLoggerSettings.IsSelected && dataLoggerSettingsList.IsAnyItemSelected();
        }

        private void OnAddNewAlarmArea(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabAlarmPrototypes.IsSelected = true;
            var aa = Document.AddNewAlarmArea(alarmPrototypeList.GetSelectedAreaParent());

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                alarmPrototypeList.AddAlarmArea(aa);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newAlarmArea = new NewAlarmArea()
                {
                    DataContext = aa
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newAlarmArea)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewAlarmArea"
                };
                if (Dialog.ShowDialog() == true)
                {
                    alarmPrototypeList.AddAlarmArea(aa);
                }
                else
                {
                    aa.Delete();
                }
            }
        }

        private void CanAddNewAlarmArea(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewAlarmSource(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabAlarmPrototypes.IsSelected = true;
            var aSource = Document.AddNewAlarmSource(alarmPrototypeList.GetSelectedAreaParent());

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                alarmPrototypeList.AddAlarmSource(aSource);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newAlarmSource = new NewAlarmSource()
                {
                    DataContext = aSource
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newAlarmSource)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewAlarmSource"
                };
                if (Dialog.ShowDialog() == true)
                {
                    alarmPrototypeList.AddAlarmSource(aSource);
                }
                else
                {
                    aSource.Delete();
                }
            }
        }

        private void CanAddNewAlarmSource(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = alarmPrototypeList != null && alarmPrototypeList.GetSelectedAreaParent() != null;
        }

        private void OnAddNewAlarmDefinition(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabAlarmPrototypes.IsSelected = true;
            var aDefinition = Document.AddNewAlarmPrototype(alarmPrototypeList.GetSelectedSourceParent());

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                alarmPrototypeList.AddAlarmDefinition(aDefinition);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newAlarmDefinition = new NewAlarmDefinition(Document)
                {
                    DataContext = aDefinition
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newAlarmDefinition)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewAlarmDefinition"
                };
                if (Dialog.ShowDialog() == true)
                {
                    alarmPrototypeList.AddAlarmDefinition(aDefinition);
                }
                else
                {
                    aDefinition.Delete();
                }
            }
        }

        private void CanAddNewAlarmDefinition(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = alarmPrototypeList != null && alarmPrototypeList.GetSelectedSourceParent() != null;
        }

        private void OnAddNewMessageDefinition(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabAlarmPrototypes.IsSelected = true;
            var aDefinition = Document.AddNewAlarmPrototype(alarmPrototypeList.GetSelectedSourceParent());

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                aDefinition.Severity = 0;
                alarmPrototypeList.AddAlarmDefinition(aDefinition);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newAlarmDefinition = new NewAlarmDefinition(Document, false)
                {
                    DataContext = aDefinition
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newAlarmDefinition)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewMessageDefinition"
                };
                if (Dialog.ShowDialog() == true)
                {
                    alarmPrototypeList.AddAlarmDefinition(aDefinition);
                }
                else
                {
                    aDefinition.Delete();
                }
            }
        }

        private void CanAddNewMessageDefinition(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = alarmPrototypeList != null && alarmPrototypeList.GetSelectedSourceParent() != null;
        }

        private void OnAssignAlarmDefinition(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (addressSpaceControl.alarmList == null)
            {
                var prototypeList = new AlarmPrototypeList(Document, true);
                addressSpaceControl.alarmList = new AssignAlarmDefinition() { DataContext = new AssignAlarmViewModel(prototypeList) };
            }
            GeneralDialogContent Dialog = new GeneralDialogContent(addressSpaceControl.alarmList)
            {
                DialogKeepContent = true,
                Owner = this.FindParent<Window>(),
                HelpLink = "AssignAlarmDefinition"
            };
            if (Dialog.ShowDialog() == true)
            {
                var model = addressSpaceControl.alarmList.DataContext as AssignAlarmViewModel;
                var hs = model.SelectedAlarmDefinitions;
                if (hs != null && hs.Count() > 0)
                {
                    if (tabAddressSpace.IsSelected)
                        addressSpaceControl.AssignItemsToSelect(hs, model);
                    else if (tabPrototypes.IsSelected)
                        prototypeList.AssignItemsToSelect(hs, model);
                }
            }
        }

        private void CanAssignTag(object sender, CanExecuteRoutedEventArgs e)
        {
            if (tabAlarmPrototypes.IsSelected)
                e.CanExecute = alarmPrototypeList != null && alarmPrototypeList.GetSelectedDefinitionParent(true) != null;
            else if (tabHistoricalPrototypes.IsSelected)
                e.CanExecute = historicalPrototypeList != null && historicalPrototypeList.GetSelectedHistorianParent() != null;
            else if (tabDataLoggerSettings.IsSelected)
                e.CanExecute = dataLoggerSettingsList != null && dataLoggerSettingsList.GetSelectedDataLoggerSettingsParent() != null;
            else if (tabViews.IsSelected)
                e.CanExecute = viewList != null && viewList.GetSelectedViewParent() != null;
            else if (tabEngineeringUnitPrototypes.IsSelected)
                e.CanExecute = engineeringUnitPrototypeList != null && engineeringUnitPrototypeList.GetSelectedEUParent() != null;
        }

        private void OnAssignTag(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TagEntityReference value = TagEntityReference.Empty;

            var targetType = TargetType.None;
            if (tabEngineeringUnitPrototypes.IsSelected)
                targetType = TargetType.EngineeringUnit;
            else if (tabHistoricalPrototypes.IsSelected)
                targetType = TargetType.Historian;
            else if (tabAlarmPrototypes.IsSelected)
                targetType = TargetType.AlarmThreshold;
            else if (tabDataLoggerSettings.IsSelected)
                targetType = TargetType.DataloggerColumn;
            else if (tabViews.IsSelected)
                targetType = TargetType.View;
            bool showPrototypeTab = tabHistoricalPrototypes.IsSelected || tabAlarmPrototypes.IsSelected || tabEngineeringUnitPrototypes.IsSelected;
            var tags = value.Edit(this.FindParent<Window>(), UFInterfaces.Editors.SelectionMode.MultipleRow, targetType: targetType, bPrototypesTab: showPrototypeTab);
            if (tags != null)
                if (tabAlarmPrototypes.IsSelected)
                    alarmPrototypeList.AddAlarmToTags(tags);
                else if (tabHistoricalPrototypes.IsSelected)
                    historicalPrototypeList.AddHistorianToTags(tags);
                else if (tabDataLoggerSettings.IsSelected)
                    AddNewDataLoggerColumns(tags);
                else if (tabViews.IsSelected)
                    viewList.AddViewToTags(tags);
                else if (tabEngineeringUnitPrototypes.IsSelected)
                    engineeringUnitPrototypeList.AddEUToTags(tags);
        }

        private void AddNewDataLoggerColumns(List<TagIdentifier> tags)
        {
            var datalogger = dataLoggerSettingsList.GetSelectedDataLoggerSettingsParent();
            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                var dataloggerColumns = dataLoggerSettingsList.AddDataloggerToTags(datalogger, tags);
                foreach (var dataloggerColumn in dataloggerColumns)
                {
                    if (datalogger.UseAggregatedTables && dataloggerColumn.ColumnTag != null && !dataloggerColumn.ColumnTag.IsEmpty())
                    {
                        UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                        if (view != null)
                            view.AddPendingAggregateTables(datalogger.Name, AggregationTypes.CommandType.Update);
                    }
                }
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var dataloggerColumn = Document.AddNewDataLoggerColumn(datalogger);
                var newDataLoggerColumn = new NewDataLoggerColumn(Document)
                {
                    DataContext = dataloggerColumn
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newDataLoggerColumn)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewDataLoggerColumn"
                };
                if (Dialog.ShowDialog() == true)
                {
                    dataLoggerSettingsList.AddDataLoggerColumn(dataloggerColumn);
                    if (datalogger.UseAggregatedTables && dataloggerColumn.ColumnTag != null && !dataloggerColumn.ColumnTag.IsEmpty())
                    {
                        UFUAEditorControl view = Document.ActiveView as UFUAEditorControl;
                        if (view != null)
                            view.AddPendingAggregateTables(datalogger.Name, AggregationTypes.CommandType.Update);
                    }
                }
                else
                {
                    dataloggerColumn.Delete();
                }
            }
        }

        private void CanAssignAlarmDefinition(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabAddressSpace.IsSelected && addressSpaceControl.IsAnyItemSelected() &&
                            addressSpaceControl.CanAssignItem() ||
                           tabPrototypes.IsSelected && prototypeList.IsAnyItemSelected() &&
                            prototypeList.CanAssignItem();
        }

        private void OnAddWholeStringId(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (Document.EditorManagerComponent.StringEditor == null)
                return;

            var listTexts = new List<string>();

            if (tabAddressSpace.IsSelected)
            {
                if (addressSpaceControl.IsAnyItemSelected())
                    listTexts = GetStringIds(addressSpaceControl.treeListControl.GetSelectedNodes());
                else
                {
                    var tags = new List<UFUAModel.UFUATag>();
                    tags.AddRange(Document.GetFlatTagCollection());
                    tags.ForEach((item) => listTexts.AddRange(Document.GetWholeStrings(item)));
                }
            }
            else if (tabPrototypes.IsSelected)
            {
                if (prototypeList.IsAnyItemSelected())
                    listTexts = GetStringIds(prototypeList.treeListControl.GetSelectedNodes());
                else
                {
                    var prototypes = new List<UFUAModel.UFUATagPrototype>();
                    prototypes.AddRange(Document.GetPrototypes());
                    prototypes.ForEach((item) => listTexts.AddRange(Document.GetWholeStrings(item)));

                    if (listTexts.Count > 0)
                        Document.EditorManagerComponent.StringEditor.AddListStringId(Document, listTexts);
                }
            }
            else
                listTexts = Document.GetWholeStrings();

            if (listTexts != null && listTexts.Count > 0)
                Document.EditorManagerComponent.StringEditor.AddListStringId(Document, listTexts);
        }

        private void CanAddWholeStringId(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isActive && Document.EditorManagerComponent.StringEditor != null;
        }

        private List<string> GetStringIds(TreeListNode[] selectedItems = null)
        {
            var listTexts = new List<String>();
            if (selectedItems != null)
            {
                var prototypes = (from p in selectedItems
                                  where p.Tag is UFUAModel.UFUATagPrototype
                                  select p.Tag as UFUAModel.UFUATagPrototype).ToList();

                prototypes.ForEach((item) => listTexts.AddRange(Document.GetWholeStrings(item)));

                var tags = (from p in selectedItems
                            where p.Tag is UFUAModel.UFUATag
                            select p.Tag as UFUAModel.UFUATag).ToList();

                tags.ForEach((item) => listTexts.AddRange(Document.GetWholeStrings(item)));

                var folders = (from p in selectedItems
                               where p.Tag is UFUAModel.UFUAFolder
                               select p.Tag as UFUAModel.UFUAFolder).ToList();

                folders.ForEach((item) => listTexts.AddRange(Document.GetWholeStrings(item)));

                var thresholds = (from p in selectedItems
                                  where p.Tag is UFUAModel.UFUAAlarmThreshold
                                  select p.Tag as UFUAModel.UFUAAlarmThreshold).ToList();

                thresholds.ForEach((item) => listTexts.AddRange(Document.GetWholeStrings(item)));
            }
            else
                listTexts.AddRange(Document.GetWholeStrings());

            return listTexts;
        }

        private void OnServiceManager(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                Document.ServiceManager();
            }
            catch (Exception ex)
            {
                if (Document.EditorManagerComponent.UIInterface != null)
                {
                    Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ServiceManagerStartFailed.Replace("'newline'", Environment.NewLine), ex.Message));
                }
            }
        }

        private void CanServiceManager(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnManteinanceHistoricalSettings(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var hs = historicalPrototypeList.GetSelectedItem();
            var msControl = new ManteinanceHistoricalSettings(Document)
            {
                DataContext = hs
            };
            GeneralDialogContent Dialog = new GeneralDialogContent(msControl)
            {
                Owner = this.FindParent<Window>(),
                Title = hs.Name,
                HelpLink = "HistoricalSettings"
            };
            if (Dialog.ShowDialog() == true)
            {
            }
        }

        private void CanManteinanceHistoricalSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////
            // https://support.progea.com/Products/default.asp?11454
            e.CanExecute = false;
            //e.CanExecute = historicalPrototypeList != null && tabHistoricalPrototypes.IsSelected &&
            //                historicalPrototypeList.GetSelectedItem() != null;
            ////////////////////////////////////////////////////////////////////////////////////////////
        }

        private void OnMoveMemberUp(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            prototypeList.MoveMemberUp();
        }
        private void CanMoveMemberUp(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabPrototypes.IsSelected && prototypeList.IsAnyItemSelected();
        }
        private void OnMoveMemberDown(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            prototypeList.MoveMemberDown();
        }
        private void CanMoveMemberDown(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabPrototypes.IsSelected && prototypeList.IsAnyItemSelected();
        }
        private void OnSetMemberPosition(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            UFUAModel.UFUATag tag = prototypeList.GetSelectedTag();
            UFUAModel.UFUAFolder folder = prototypeList.GetSelectedFolder();
            if (tag != null || folder != null)
            {
                using (var uow = Document.BeginNestedUnitOfWork())
                {
                    DevExpress.Xpo.XPObject selected = tag;
                    if (selected == null)
                        selected = folder;
                    var obj = uow.GetNestedObject(selected);
                    var control = new SetMemberPosition()
                    {
                        DataContext = obj
                    };
                    GeneralDialogContent Dialog = new GeneralDialogContent(control)
                    {
                        Owner = this.FindParent<Window>(),
                        HelpLink = "SetMemberPosition"
                    };
                    if (Dialog.ShowDialog() == true)
                    {
                        if (obj is UFUAModel.UFUATag)
                            prototypeList.SetMemberPosition((obj as UFUAModel.UFUATag).MemberOrderId.Value);
                        else
                            prototypeList.SetMemberPosition((obj as UFUAModel.UFUAFolder).MemberOrderId.Value);
                    }
                }
            }
        }
        private void CanSetMemberPosition(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabPrototypes.IsSelected && prototypeList.IsAnyItemSelected();
        }

        private void OnAddNewEngineeringUnits(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabEngineeringUnitPrototypes.IsSelected = true;
            var engineeringUnit = Document.AddNewEngineeringUnits();

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                engineeringUnitPrototypeList.AddEU(engineeringUnit);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newEngineeringUnit = new NewEngineeringUnit()
                {
                    DataContext = engineeringUnit
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newEngineeringUnit)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewEngineeringUnits"
                };
                if (Dialog.ShowDialog() == true)
                {
                    engineeringUnitPrototypeList.AddEU(engineeringUnit);
                }
                else
                {
                    engineeringUnit.Delete();
                }
            }
        }

        private void CanAddNewEngineeringUnits(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        private void OnImportEngineeringUnits(object sender, ExecutedRoutedEventArgs e)
        {
            EUImportViewModel euvm = new EUImportViewModel();
            BaseImportView view = new BaseImportView();
            view.DataContext = euvm;
            GeneralDialogContent gdc = new GeneralDialogContent(view)
            {
                Title = Properties.Resources.ImportEngineeringUnitsTitle
            };
            if(gdc.ShowDialog() == true)
            {
                var items = euvm.GetSelectedItems();

                mapImportedEUToUFUAEU(items);
            }
            euvm.Dispose();
        }

        private void mapImportedEUToUFUAEU(List<ImportEngineeringUnit> items)
        {
            using (var v = new WaitCursor())
            {
                var importedEU = Document.AddImportedEngineeringUnits(items);
                
                if(importedEU.Count > 0)
                    engineeringUnitPrototypeList.AddImportedEUs(importedEU);
            }
        }        

        private void CanImportEngineeringUnits(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewView(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabViews.IsSelected = true;
            var view = Document.AddNewView();

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                viewList.AddView(view);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newView = new NewView()
                {
                    DataContext = view
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newView)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewView"
                };
                if (Dialog.ShowDialog() == true)
                {
                    viewList.AddView(view);
                }
                else
                {
                    view.Delete();
                }
            }
        }

        private void CanAddNewView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAssociateView(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var viewList = new ViewList(Document, true);
            GeneralDialogContent Dialog = new GeneralDialogContent(viewList)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "AssociateView"
            };
            if (Dialog.ShowDialog() == true)
            {
                var hs = viewList.GetSelectedItem();
                if (hs != null)
                    addressSpaceControl.AssignItemToSelect(hs);
            }
        }

        private void CanAssociateView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabAddressSpace.IsSelected && addressSpaceControl.IsAnyItemSelected() &&
                            addressSpaceControl.CanAssignView();
        }


        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabAddressSpace.IsSelected)
                {
                    addressSpaceControl.CopySelectedToClipboard();
                    addressSpaceControl.DeleteSelectedItems();
                }
                else if (tabPrototypes.IsSelected)
                {
                    prototypeList.CopySelectedToClipboard();
                    prototypeList.DeleteSelectedItems();
                }
                else if (tabViews.IsSelected)
                {
                    viewList.CopySelectedToClipboard();
                    viewList.DeleteSelectedItems();
                }
                else if (tabHistoricalPrototypes.IsSelected)
                {
                    historicalPrototypeList.CopySelectedToClipboard();
                    historicalPrototypeList.DeleteSelectedItems();
                }
                else if (tabDataLoggerSettings.IsSelected)
                {
                    dataLoggerSettingsList.CopySelectedToClipboard();
                    dataLoggerSettingsList.DeleteSelectedItems();
                }
                else if (tabEngineeringUnitPrototypes.IsSelected)
                {
                    engineeringUnitPrototypeList.CopySelectedToClipboard();
                    engineeringUnitPrototypeList.DeleteSelectedItems();
                }
                else if (tabAlarmPrototypes.IsSelected)
                {
                    alarmPrototypeList.CopySelectedToClipboard();
                    alarmPrototypeList.DeleteSelectedItems();
                }
                else if (tabDrivers.IsSelected)
                {
                    driverProjectList.CopySelectedToClipboard();
                    driverProjectList.DeleteSelectedItems();
                }
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabAddressSpace.IsSelected && addressSpaceControl.IsAnyItemSelected() ||
               tabPrototypes.IsSelected && prototypeList.IsAnyItemSelected() ||
               tabViews.IsSelected && viewList.IsAnyItemSelected() ||
               tabHistoricalPrototypes.IsSelected && historicalPrototypeList.IsAnyItemSelected() ||
               tabDataLoggerSettings.IsSelected && dataLoggerSettingsList.IsAnyItemSelected() ||
               tabAlarmPrototypes.IsSelected && alarmPrototypeList.IsAnyItemSelected() ||
               tabEngineeringUnitPrototypes.IsSelected && engineeringUnitPrototypeList.IsAnyItemSelected() ||
               tabDrivers.IsSelected && driverProjectList.IsAnyItemSelected();
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabAddressSpace.IsSelected)
                    addressSpaceControl.CopySelectedToClipboard();
                else if (tabPrototypes.IsSelected)
                    prototypeList.CopySelectedToClipboard();
                else if (tabViews.IsSelected)
                    viewList.CopySelectedToClipboard();
                else if (tabHistoricalPrototypes.IsSelected)
                    historicalPrototypeList.CopySelectedToClipboard();
                else if (tabDataLoggerSettings.IsSelected)
                    dataLoggerSettingsList.CopySelectedToClipboard();
                else if (tabEngineeringUnitPrototypes.IsSelected)
                    engineeringUnitPrototypeList.CopySelectedToClipboard();
                else if (tabAlarmPrototypes.IsSelected)
                    alarmPrototypeList.CopySelectedToClipboard();
                else if (tabDrivers.IsSelected)
                    driverProjectList.CopySelectedToClipboard();
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabAddressSpace.IsSelected && addressSpaceControl != null)
                e.CanExecute = addressSpaceControl.IsAnyItemSelected();
            else if (tabPrototypes.IsSelected && prototypeList != null)
                e.CanExecute = prototypeList.IsAnyItemSelected();
            else if (tabViews.IsSelected && viewList != null)
                e.CanExecute = viewList.IsAnyItemSelected();
            else if (tabHistoricalPrototypes.IsSelected && historicalPrototypeList != null)
                e.CanExecute = historicalPrototypeList.IsAnyItemSelected();
            else if (tabDataLoggerSettings.IsSelected && dataLoggerSettingsList != null)
                e.CanExecute = dataLoggerSettingsList.IsAnyItemSelected();
            else if (tabEngineeringUnitPrototypes.IsSelected && engineeringUnitPrototypeList != null)
                e.CanExecute = engineeringUnitPrototypeList.IsAnyItemSelected();
            else if (tabAlarmPrototypes.IsSelected && alarmPrototypeList != null)
                e.CanExecute = alarmPrototypeList.IsAnyItemSelected();
            else if (tabDrivers.IsSelected && driverProjectList != null)
                e.CanExecute = driverProjectList.IsAnyItemSelected();
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabAddressSpace.IsSelected)
                    addressSpaceControl.PasteFromClipboard();
                else if (tabPrototypes.IsSelected)
                    prototypeList.PasteFromClipboard();
                else if (tabViews.IsSelected)
                    viewList.PasteFromClipboard();
                else if (tabHistoricalPrototypes.IsSelected)
                    historicalPrototypeList.PasteFromClipboard();
                else if (tabDataLoggerSettings.IsSelected)
                    dataLoggerSettingsList.PasteFromClipboard();
                else if (tabEngineeringUnitPrototypes.IsSelected)
                    engineeringUnitPrototypeList.PasteFromClipboard();
                else if (tabAlarmPrototypes.IsSelected)
                    alarmPrototypeList.PasteFromClipboard();
                else if (tabDrivers.IsSelected)
                    driverProjectList.PasteFromClipboard();
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            // Document.CopyWinClipboardToInMemoryData();
            if (tabAddressSpace.IsSelected)
                e.CanExecute = Document.ClipboardContainsTags() || Document.ClipboardContainsFolders() || 
                    (addressSpaceControl.GetSelectedParentTag() != null && Document.ClipboardContainsAlarmThresholds());
            else if (tabPrototypes.IsSelected)
                e.CanExecute = Document.ClipboardContainsPrototypes() || 
                    (prototypeList.IsAnyItemSelected() && Document.ClipboardContainsFolders()) ||
                    (prototypeList.IsAnyItemSelected() && Document.ClipboardContainsTags()) ||
                    (prototypeList.GetSelectedParentTag() != null && Document.ClipboardContainsAlarmThresholds());
            else if (tabViews.IsSelected)
                e.CanExecute = Document.ClipboardContainsViews();
            else if (tabHistoricalPrototypes.IsSelected)
                e.CanExecute = Document.ClipboardContainsHistorians();
            else if (tabDataLoggerSettings.IsSelected)
                e.CanExecute = Document.ClipboardContainsDataLoggerSettings() || 
                    (dataLoggerSettingsList.IsAnyItemSelected() && Document.ClipboardContainsDataLoggerColumn());
            else if (tabEngineeringUnitPrototypes.IsSelected)
                e.CanExecute = Document.ClipboardContainsEngineeringUnits();
            else if (tabAlarmPrototypes.IsSelected)
                e.CanExecute = Document.ClipboardContainsAlarmAreas() ||
                    (alarmPrototypeList.IsAnyItemSelected() && Document.ClipboardContainsAlarmSources()) ||
                    (alarmPrototypeList.IsAnyItemSelected() && Document.ClipboardContainsAlarmDefinitions());
            else if (tabDrivers.IsSelected)
                e.CanExecute = Document.ClipboardContainsDrivers();
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var ownerTypeName = Document.GetNextUndoOwner();
                if (ownerTypeName == typeof(AddressSpace).Name)
                {
                    tabAddressSpace.IsSelected = true;
                    addressSpaceControl.UndoAction();
                }
                else if (ownerTypeName == typeof(PrototypeList).Name)
                {
                    tabPrototypes.IsSelected = true;
                    prototypeList.UndoAction();
                }
                else if (ownerTypeName == typeof(ViewList).Name)
                {
                    tabViews.IsSelected = true;
                    viewList.UndoAction();
                }
                else if (ownerTypeName == typeof(HistoricalPrototypeList).Name)
                {
                    tabHistoricalPrototypes.IsSelected = true;
                    historicalPrototypeList.UndoAction();
                }
                else if (ownerTypeName == typeof(DataLoggerSettingsList).Name)
                {
                    tabDataLoggerSettings.IsSelected = true;
                    dataLoggerSettingsList.UndoAction();
                }
                else if (ownerTypeName == typeof(EngineeringUnitPrototypeList).Name)
                {
                    tabEngineeringUnitPrototypes.IsSelected = true;
                    engineeringUnitPrototypeList.UndoAction();
                }
                else if (ownerTypeName == typeof(AlarmPrototypeList).Name)
                {
                    tabAlarmPrototypes.IsSelected = true;
                    alarmPrototypeList.UndoAction();
                }
                else if (ownerTypeName == typeof(DriverProjectList).Name)
                {
                    tabDrivers.IsSelected = true;
                    driverProjectList.UndoAction();
                }
                else if (ownerTypeName == typeof(RedundancySettings).Name)
                {
                    tabRedundancy.IsSelected = true;
                    redundancySettings.UndoAction();
                }
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.UndoContainsSomething();
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var ownerTypeName = Document.GetNextRedoOwner();
                if (ownerTypeName == typeof(AddressSpace).Name)
                {
                    tabAddressSpace.IsSelected = true;
                    addressSpaceControl.RedoAction();
                }
                else if (ownerTypeName == typeof(PrototypeList).Name)
                {
                    tabPrototypes.IsSelected = true;
                    prototypeList.RedoAction();
                }
                else if (ownerTypeName == typeof(ViewList).Name)
                {
                    tabViews.IsSelected = true;
                    viewList.RedoAction();
                }
                else if (ownerTypeName == typeof(HistoricalPrototypeList).Name)
                {
                    tabHistoricalPrototypes.IsSelected = true;
                    historicalPrototypeList.RedoAction();
                }
                else if (ownerTypeName == typeof(DataLoggerSettingsList).Name)
                {
                    tabDataLoggerSettings.IsSelected = true;
                    dataLoggerSettingsList.RedoAction();
                }
                else if (ownerTypeName == typeof(EngineeringUnitPrototypeList).Name)
                {
                    tabEngineeringUnitPrototypes.IsSelected = true;
                    engineeringUnitPrototypeList.RedoAction();
                }
                else if (ownerTypeName == typeof(AlarmPrototypeList).Name)
                {
                    tabAlarmPrototypes.IsSelected = true;
                    alarmPrototypeList.RedoAction();
                }
                else if (ownerTypeName == typeof(DriverProjectList).Name)
                {
                    tabDrivers.IsSelected = true;
                    driverProjectList.RedoAction();
                }
                else if (ownerTypeName == typeof(RedundancySettings).Name)
                {
                    tabRedundancy.IsSelected = true;
                    redundancySettings.RedoAction();
                }
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.RedoContainsSomething();
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (tabDrivers.IsSelected)
                driverProjectList.EditSelectedItem();
            else if(Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else if (tabAddressSpace.IsSelected)
                addressSpaceControl.EditSelectedItem();
            else if (tabPrototypes.IsSelected)
                prototypeList.EditSelectedItem();
            else if (tabViews.IsSelected)
                viewList.EditSelectedItem();
            else if (tabHistoricalPrototypes.IsSelected)
                historicalPrototypeList.EditSelectedItem();
            else if (tabDataLoggerSettings.IsSelected)
                dataLoggerSettingsList.EditSelectedItem();
            else if (tabEngineeringUnitPrototypes.IsSelected)
                engineeringUnitPrototypeList.EditSelectedItem();
            else if (tabAlarmPrototypes.IsSelected)
                alarmPrototypeList.EditSelectedItem();
             
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabAddressSpace.IsSelected && addressSpaceControl != null)
                e.CanExecute = addressSpaceControl.IsAnyItemSelected() && addressSpaceControl.CanEdit();
            else if (tabPrototypes.IsSelected && prototypeList != null)
                e.CanExecute = prototypeList.IsAnyItemSelected();
            else if (tabViews.IsSelected && viewList != null)
                e.CanExecute = viewList.IsAnyItemSelected();
            else if (tabHistoricalPrototypes.IsSelected && historicalPrototypeList != null)
                e.CanExecute = historicalPrototypeList.IsAnyItemSelected();
            else if (tabDataLoggerSettings.IsSelected && dataLoggerSettingsList != null)
                e.CanExecute = dataLoggerSettingsList.IsAnyItemSelected();
            else if (tabEngineeringUnitPrototypes.IsSelected && engineeringUnitPrototypeList != null)
                e.CanExecute = engineeringUnitPrototypeList.IsAnyItemSelected();
            else if (tabAlarmPrototypes.IsSelected && alarmPrototypeList != null)
                e.CanExecute = alarmPrototypeList.IsAnyItemSelected();
            else if (tabDrivers.IsSelected && driverProjectList != null)
                e.CanExecute = driverProjectList.IsAnyItemSelected();
        }

        #endregion
        
        #region Event Handlers
        private void TransportAdd_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var bas = UFUAServerInfo.UFUAServerInfo.GetCurrentApplicationBaseAddresses();
            if (bas.Count == 0)
                return;            
            var ba = Document.AddNewBaseAddress(Opc.Ua.Utils.ParseUri(bas.FirstOrDefault()).Scheme);
            var n = new UFUACommonControls.NewTransport(bas)
            {
                DataContext = ba
            };
            GeneralDialogContent newBaseAddressDialog = new GeneralDialogContent(n)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "NewTransport"
            };

            if (newBaseAddressDialog.ShowDialog() == true)
            {
                //Document.AddUndoAction(this, ba, UndoRedoAction.Added);
                TransportGrid.ItemsSource = null;
                TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                TransportGrid.SelectedItem = ba;
            }
            else
                ba.Delete();

            TransportGrid.ItemsSource = null;
            TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
        }

        private void TransportDelete_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (TransportGrid.SelectedItem != null)
            {
                var ba = TransportGrid.SelectedItem as UFUAModel.UFUABaseAddress;
                if (ba != null && (MessageBox.Show(string.Format(Properties.Resources.AskDeleteStation, ba.Path),
                    Properties.Resources.CaptionDeleteBA, MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    ba.Delete();

                    //Document.AddUndoAction(this, ba, UndoRedoAction.Removed);
                    TransportGrid.ItemsSource = null;
                    TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                }
            }
        }

        private void TransportEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (TransportGrid.SelectedItem != null)
            {
                if (TransportGrid.SelectedItem is UFUAModel.UFUABaseAddress)
                {
                    var si = TransportGrid.SelectedItem as UFUAModel.UFUABaseAddress;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var bas = UFUAServerInfo.UFUAServerInfo.GetCurrentApplicationBaseAddresses();
                        var n = new UFUACommonControls.NewTransport(bas)
                        {
                            DataContext = uow.GetNestedObject(si)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(n)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditTransport"
                        };

                        if (Dialog.ShowDialog() == true)
                        {
                            //Document.AddUndoAction(this, si, UndoRedoAction.Changed);
                            uow.CommitChanges();
                            Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(configurationSettings.DataContext);
                            TransportGrid.ItemsSource = null;
                            TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                        }
                    }
                }
            }
        }

        private void OnAttachDebugger(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            addressSpaceControl.AttachDebuggerOnSelected();
        }

        private void CanAttachDebugger(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabAddressSpace.IsSelected)
            {
                e.CanExecute = addressSpaceControl.CanAttachDebuggerOnSelected();
            }
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            Document.EditorManagerComponent.Workspace.ContextObject = null;

            foreach (DevExpress.Xpf.Core.DXTabItem item in tabControlExt.Items)
            {
                if (item.Content is FrameworkElement)
                {
                    var fe = item.Content as FrameworkElement;
                    var list = (from p in fe.GetChildrenOfType<FrameworkElement>()
                                where p is IDisposable select p as IDisposable).ToList();
                    
                    list.ForEach((o) => o.Dispose() );
                }
            }

            if (parentWindow != null)
                parentWindow.Activated -= parentWindow_Activated;

            lock (pendingAggregateDataLoggers)
            {
                if (dpAggregateTables != null &&
                    dpAggregateTables.Status != DispatcherOperationStatus.Aborted &&
                    dpAggregateTables.Status != DispatcherOperationStatus.Completed)
                    dpAggregateTables.Abort();
            }

            if (ctsAggregateTables != null)
            {
                ctsAggregateTables.Cancel();
                ctsAggregateTables.Dispose();
            }

            //tabControlExt.Items.Clear();
            //tabControlExt.Dispose();
        }
        #endregion
    }
}
