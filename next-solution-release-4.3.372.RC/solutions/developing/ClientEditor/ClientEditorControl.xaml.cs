using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using ClientEditor.Controls;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpf.Core;
using UIMsgBoxAlertService.ComponentService;
using log4net;
using ClientEditor.Document;
using DevExpress.Xpo;
using Utilities.UndoRedo;
using ClientEditor.ComponentService;
using OPCUAViewModel;
using DocumentManager.ComponentService.Helpers;
using UFProjectManager;
using TempVarriables.ComponentService;
using System.Windows.Threading;
using UFProjectManager.ComponentService;
using AppNameSettingService;

namespace ClientEditor
{
    /// <summary>
    /// Interaction logic for DocumentEditorControl.xaml
    /// </summary>
    public partial class ClientEditorControl : UserControl, IEditableObject, IDisposable
    {
        #region Declarations
        readonly ClientEditorManagerComponent clientEditorManagerComponent;
        ClientDocument _Document;
        [Browsable(false)]
        public ClientDocument Document
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
        internal bool isActive;
        bool bLoaded;
        string appName;
        UserControl tempVariablesControl;
        UserControl emptyTempVariablesControl;
        #endregion

        #region ctor
        public ClientEditorControl(ClientEditorManagerComponent c, ClientDocument doc)
        {
            InitializeComponent();
            Document = doc;
            clientEditorManagerComponent = c;

            Loaded += (o, e) =>
            {
                bLoaded = true;
                var ufuaeditormanager = clientEditorManagerComponent.UFUAEditor;
                appName = ufuaeditormanager?.GetAplicationName(Document, true);
                if(!string.IsNullOrEmpty(appName))
                {
                    AppNameSettings newsettings = null;
                    Dictionary<string, AppNameSettings> map = new Dictionary<string, AppNameSettings>(Document.MapAppNameSettings);
                    AppNameSettings defaultAppNameSettings = (AppNameSettings)clientEditorManagerComponent.UFProjectManager.GetDefaultAppNameSettings(Document);
                    if (defaultAppNameSettings != null)
                    {
                        if (!string.IsNullOrEmpty(appName) && (map.Count == 0 || !map.ContainsKey(appName)))
                            map.Add(appName, defaultAppNameSettings);
                        else
                            defaultAppNameSettings = map[appName];
                    }

                    var editAppNameSettings = new AppNameSettingsEditor(map, defaultAppNameSettings, appName);
                    editAppNameSettings.ClearValue(AppNameSettingsEditor.WidthProperty);
                    editAppNameSettings.ClearValue(AppNameSettingsEditor.HeightProperty);

                    editAppNameSettings.AppNameSettingsChanged += (obj, ea) =>
                    {
                        var forceUpdateAll = defaultAppNameSettings.HasOverriddenString() && !appName.Equals(defaultAppNameSettings.ToString());
                        if (!string.IsNullOrEmpty(defaultAppNameSettings.HostNameRenamed) ||
                           !string.IsNullOrEmpty(defaultAppNameSettings.BackupHostName) ||
                           !string.IsNullOrEmpty(defaultAppNameSettings.EndpointRenamed) ||
                           defaultAppNameSettings.AlwaysDiscoverEndpoint ||
                           !string.IsNullOrEmpty(defaultAppNameSettings.AppNameRenamed) ||
                           !string.IsNullOrEmpty(defaultAppNameSettings.BackupAppName) ||
                           defaultAppNameSettings.UseSecurityWhenNotLocal ||
                           defaultAppNameSettings.UsePollingRead ||
                           forceUpdateAll)
                            Document.MapAppNameSettings = editAppNameSettings.MapAppNameSettings.ToDictionary(s => s.Key, s => s.Value);
                        else
                            Document.MapAppNameSettings = (from s in editAppNameSettings.MapAppNameSettings where s.Key != appName select s).ToDictionary(s => s.Key, s => s.Value);

                        clientEditorManagerComponent.UFProjectManager.UpdateDefaultAppNameSettings(Document, defaultAppNameSettings);
                    };

                    tabGeneralSettings.Content = editAppNameSettings;
                }

                tabControlExt.SelectionChanged += (obj, ea) =>
                {
                    if (bDisposed || Document == null)
                        return;

                    if (ea.NewSelectedItem == tabTempVariables)
                    {
                        if (tempVariablesControl == null)
                        {
                            using (new WaitCursor())
                            {
                                var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                                if (dsInterface != null)
                                {
                                    dsInterface.SetDocumentParent(Document);
                                    tempVariablesControl = dsInterface.Editor(false);
                                    if (tempVariablesControl != null)
                                    {
                                        tempVariablesControl.ClearValue(FrameworkElement.WidthProperty);
                                        tempVariablesControl.ClearValue(FrameworkElement.HeightProperty);
                                        tabTempVariables.Content = tempVariablesControl;
                                    }
                                    else if (emptyTempVariablesControl == null)
                                    {
                                        emptyTempVariablesControl = new WPFUtilities.Controls.EmptyTab();
                                        (emptyTempVariablesControl as WPFUtilities.Controls.EmptyTab).InfoMessage = String.Format(Properties.Resources.NullEditorMessage, dsInterface.HumanReadableName);

                                        emptyTempVariablesControl.ClearValue(FrameworkElement.WidthProperty);
                                        emptyTempVariablesControl.ClearValue(FrameworkElement.HeightProperty);
                                        tabTempVariables.Content = tempVariablesControl;
                                    }
                                }
                            }
                        }
                        if (tempVariablesControl is ITempVarControl)
                            (tempVariablesControl as ITempVarControl).OnActivate();
                    }
                };

                tabTempVariables.IsSelected = true;

            };
        }
        #endregion

        #region Command Handlers

        private void OnAddNewTempFolder(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!tabTempVariables.IsSelected)
            {
                tabTempVariables.IsSelected = true;
                WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
            }

            if (tempVariablesControl is ITempVarControl)
                (tempVariablesControl as ITempVarControl).AddNewFolder();
        }
        private void CanAddNewTempFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tempVariablesControl is ITempVarControl;
        }
        private void OnAddNewTempTag(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!tabTempVariables.IsSelected)
            {
                tabTempVariables.IsSelected = true;
                WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
            }

            if (tempVariablesControl is ITempVarControl)
                (tempVariablesControl as ITempVarControl).AddNewTag();
        }

        private void CanAddNewTempTag(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tempVariablesControl is ITempVarControl;
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tempVariablesControl is ITempVarControl && (tempVariablesControl as ITempVarControl).IsAnyItemSelected();
        }

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Document.SaveToFile();
            var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
            if (dsInterface != null)
                dsInterface.Save(Document);
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                if (dsInterface != null)
                    e.CanExecute = Document.NeedsSave || dsInterface.NeedsSave(Document);
                else
                    e.CanExecute = Document.NeedsSave;
            }
            catch (Exception)
            {
                e.CanExecute = Document.NeedsSave;
            }
        }

        #endregion

        #region methods
        internal void ActivateTempVariableTab()
        {
            tabTempVariables.IsSelected = true;
            WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);
            if (tempVariablesControl is ITempVarControl)
                (tempVariablesControl as ITempVarControl).ClearSelection();
        }
        internal UserControl GetSelectedTabEditor()
        {
            UserControl currentTabEditor = null;
            if (tabTempVariables.IsSelected)
                currentTabEditor = tempVariablesControl;

            return currentTabEditor;
        }

        internal void OnActivate()
        {
            isActive = true;

            if (tabTempVariables.IsSelected && tempVariablesControl is ITempVarControl)
                    (tempVariablesControl as ITempVarControl).OnActivate();
        }

        internal void OnDeactivate()
        {
            isActive = false;
        }
        #endregion

        #region IEditableObject Members
        public void BeginEdit()
        {
            var currentTabEditor = GetSelectedTabEditor();
            if (currentTabEditor is IEditableObject)
                (currentTabEditor as IEditableObject).BeginEdit();
        }

        public void CancelEdit()
        {
            var currentTabEditor = GetSelectedTabEditor();
            if (currentTabEditor is IEditableObject)
                (currentTabEditor as IEditableObject).CancelEdit();
        }

        public void EndEdit()
        {
            bool bResult = true;
            var currentTabEditor = GetSelectedTabEditor();
            if (currentTabEditor is IEditableObject)
                (currentTabEditor as IEditableObject).EndEdit();
        }
        #endregion IEditableObject Members
        
        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            
            foreach (DevExpress.Xpf.Core.DXTabItem item in tabControlExt.Items)
            {
                if (item.Content is FrameworkElement)
                {
                    var fe = item.Content as FrameworkElement;
                    var list = (from p in fe.GetChildrenOfType<FrameworkElement>()
                                where p is IDisposable
                                select p as IDisposable).ToList();
                    if (fe is IDisposable)
                        fe.Dispose();
                    list.ForEach((o) => o.Dispose());
                }
            }
        }
        #endregion
    }
}
