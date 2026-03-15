//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupingControlDesigner.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using Syncfusion.Grouping;
using Syncfusion.Diagnostics;
using Syncfusion.Collections;
using Syncfusion.Windows.Forms;

using Table = Syncfusion.Grouping.Table;

////using Microsoft.VisualStudio.Designer;

namespace Syncfusion.Windows.Forms.Grid.Grouping.Design
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridGroupingControlDesigner : ControlDesigner
    {
        GridGroupingControl groupingControl;
        private DesignerVerbCollection verbs = null;
        IDesignerHost host;
        IComponentChangeService componentChangeService;
        bool modified = false;
        GridGroupingControlPreviewDialog groupingControlPreviewDialog;
        bool showing = false;
        bool propertyChanged = false;
        bool propertyReset = false;
        internal bool inUndo = false;
        GridGroupingControlUndoInit undoSnapshot = null;

#if SyncfusionFramework2_0
        DesignerActionListCollection actionLists;

        private void BuildActionLists()
        {
            this.actionLists = new DesignerActionListCollection();
            this.actionLists.Add(new GridGroupingControlChooseDataSourceActionList(this));
            DesignerVerb[] verbs = new DesignerVerb[Verbs.Count];
            Verbs.CopyTo(verbs, 0);
            this.actionLists.Add(new DesignerActionVerbList(verbs));
            this.actionLists.Add(new DesignerActionSupportList(Component));
            this.actionLists[0].AutoShow = true;
        }

        // Properties
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (this.actionLists == null)
                {
                    this.BuildActionLists();
                }

                return this.actionLists;
            }
        }

        ////With Whidbey the default UndoEngine provided by the designer
        ////is way to slow for a GridGroupingControl because of its many
        ////properties. You will notice it especially when you try
        ////to resize the GridGroupingControl.

        ////The problem is that when you start resizing the control undo information 
        ////is generated in the Visual Studio Designer. At that time the IDE uses 
        ////reflection to loop through each and every property in the grouping grid 
        ////and checks whether it needs to be serialized to code or not (just like 
        ////when the InitializeComponent code is created). That whole process takes 
        ////some time because of the many properties in the grouping grid.

        ////What you would need to do is when you start resizing the control do this: 
        ////Press the mouse button, resize it slightly but keep holding down the button. 
        ////You will notice undo information being generated. After you see the window 
        ////react to the resize command you will now be able to quickly resize the 
        ////Control. Key is to keep holding the mouse button down and wait until the 
        ////designer reacts.

        ////Since this experience is not pleasant at all for customers we therefore
        ////decided to disable undo generation altogether for the form a GridGroupingControl
        ////is dropped on.

        ////There might be a way to hook into the undo generation at design-time
        ////but there is no documented way of doing this and therefore probably quite
        ////some effort to get right.

        UndoEngine undoEngine;

        private void OnDesignerActivate(object source, EventArgs evevent)
        {
            this.selSvc = (ISelectionService)this.GetService(typeof(ISelectionService));
            if (this.selSvc != null)
            {
                this.selSvc.SelectionChanging += new EventHandler(this.OnSelectionChanging);
                this.selSvc.SelectionChanged += new EventHandler(this.OnSelectionChanged);
            }
        }

        /// <override/>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            if (this.undoEngine == null)
            {
                this.undoEngine = this.GetService(typeof(UndoEngine)) as UndoEngine;
            }

            if (this.undoEngine != null)
            {
                this.undoEngine.Enabled = false;
            }
        }

        private void OnDesignerDeactivate(object sender, EventArgs e)
        {
        }

        ISelectionService selSvc;

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (base.Component != null)
            {
                ISelectionService service1 = (ISelectionService)sender;
                if (service1.GetComponentSelected(this.Component))
                {
                    if (this.undoEngine == null)
                    {
                        this.undoEngine = this.GetService(typeof(UndoEngine)) as UndoEngine;
                    }

                    if (this.undoEngine != null)
                    {
                        this.undoEngine.Enabled = false;
                    }
                }
            }
        }

        private void OnSelectionChanging(object sender, EventArgs e)
        {
            if (this.undoEngine == null)
            {
                this.undoEngine = this.GetService(typeof(UndoEngine)) as UndoEngine;
            }

            if (this.undoEngine != null)
            {
                this.undoEngine.Enabled = true;
            }
        }
#endif

        internal object DataSource
        {
            get
            {
                return groupingControl.DataSource;
            }

            set
            {
                groupingControl.DataSource = value;
            }
        }

        internal string DataMember
        {
            get
            {
                return groupingControl.DataMember;
            }

            set
            {
                groupingControl.DataMember = value;
            }
        }

        #region Initialize

        IDesignerHost Host
        {
            get
            {
                if (host == null)
                {
                    host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                    componentChangeService.ComponentRemoving += new ComponentEventHandler(componentChangeService_ComponentRemoving);
                    componentChangeService.ComponentChanging += new ComponentChangingEventHandler(componentChangeService_ComponentChanging);
                    componentChangeService.ComponentAdded += new ComponentEventHandler(componentChangeService_ComponentAdded);
                    componentChangeService.ComponentAdding += new ComponentEventHandler(componentChangeService_ComponentAdding);
                    componentChangeService.ComponentChanged += new ComponentChangedEventHandler(componentChangeService_ComponentChanged);
                    componentChangeService.ComponentRemoved += new ComponentEventHandler(componentChangeService_ComponentRemoved);
                    componentChangeService.ComponentRename += new ComponentRenameEventHandler(componentChangeService_ComponentRename);

#if SyncfusionFramework2_0
                    host.Activated += new EventHandler(this.OnDesignerActivate);
                    host.Deactivated += new EventHandler(this.OnDesignerDeactivate);
#endif
                }

                return this.host;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (componentChangeService != null)
            {
                componentChangeService.ComponentRemoving -= new ComponentEventHandler(componentChangeService_ComponentRemoving);
                componentChangeService.ComponentChanging -= new ComponentChangingEventHandler(componentChangeService_ComponentChanging);
                componentChangeService.ComponentAdded -= new ComponentEventHandler(componentChangeService_ComponentAdded);
                componentChangeService.ComponentAdding -= new ComponentEventHandler(componentChangeService_ComponentAdding);
                componentChangeService.ComponentChanged -= new ComponentChangedEventHandler(componentChangeService_ComponentChanged);
                componentChangeService.ComponentRemoved -= new ComponentEventHandler(componentChangeService_ComponentRemoved);
                componentChangeService.ComponentRename -= new ComponentRenameEventHandler(componentChangeService_ComponentRename);
            }

#if SyncfusionFramework2_0
            if (host != null)
            {
                host.Activated -= new EventHandler(this.OnDesignerActivate);
                host.Deactivated -= new EventHandler(this.OnDesignerDeactivate);
            }

            if (this.selSvc != null)
            {
                this.selSvc.SelectionChanging -= new EventHandler(this.OnSelectionChanging);
                this.selSvc.SelectionChanged -= new EventHandler(this.OnSelectionChanged);
            }
#endif

            if (groupingControl != null)
            {
                groupingControl.Engine.DataSourceChanged -= new EventHandler(Engine_DataSourceChanged);
                groupingControl.HandleDestroyed -= new EventHandler(Control_HandleDestroyed);
                groupingControl.PropertyChanging -= new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanging);
                groupingControl.PropertyChanged -= new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanged);
            }

            base.Dispose(disposing);
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            groupingControl = (GridGroupingControl)Control;
            groupingControl.InDesigner = true;

            base.PreFilterProperties(properties);
        }

#if SyncfusionFramework2_0
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            groupingControl = (GridGroupingControl)Control;
            groupingControl.InDesigner = true;

            groupingControl.VersionInfo = groupingControl.ProductVersion;

            base.InitializeNewComponent(defaultValues);
        }
#else
        public override void OnSetComponentDefaults()
        {
            groupingControl = (GridGroupingControl) Control;
            groupingControl.InDesigner = true;

            groupingControl.VersionInfo = groupingControl.ProductVersion;

            base.OnSetComponentDefaults ();
        }
#endif

        protected override void OnCreateHandle()
        {
            TraceUtil.TraceCurrentMethodInfo(Control);
            if (Control != null)
            {
                groupingControl = (GridGroupingControl)Control;
                groupingControl.HandleDestroyed += new EventHandler(Control_HandleDestroyed);
                groupingControl.Engine.DataSourceChanged += new EventHandler(Engine_DataSourceChanged);
                groupingControl.PropertyChanging += new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanging);
                ////groupingControl.TableDescriptor.PropertyChanging += new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanging);
                ////groupingControl.Engine.PropertyChanging += new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanging);
                groupingControl.PropertyChanged += new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanged);
                ////groupingControl.TableDescriptor.PropertyChanged += new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanged);
                ////groupingControl.Engine.PropertyChanged += new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanged);
                groupingControl.InDesigner = true;
                ////groupingControl.TableDescriptor.Name = "Schema";

                object h = Host;
            }

            base.OnCreateHandle();
        }

        private void Control_HandleDestroyed(object sender, EventArgs e)
        {
            if (groupingControl != null)
            {
                groupingControl.HandleDestroyed -= new EventHandler(Control_HandleDestroyed);
                groupingControl.PropertyChanging -= new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanging);
                groupingControl.PropertyChanged -= new DescriptorPropertyChangedEventHandler(groupingControl_PropertyChanged);
            }

            groupingControl = null;
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if ((propertyChanged || propertyReset) && !inUndo)
            {
                OnWndProcPropertyChanged();
                this.RefreshPropertyBrowser();
                propertyChanged = false;
                propertyReset = false;
            }

            undoSnapshot = null;
        }

        bool inRefreshPropertyBrowser = false;

        internal void RefreshPropertyBrowser()
        {
            if (inRefreshPropertyBrowser)
            {
                return;
            }

            inRefreshPropertyBrowser = true;
            try
            {
                TypeDescriptor.Refresh(this.groupingControl);
                ////            object propertyBrowser = this.GetService(typeof(IVSMDPropertyBrowser));
                ////            if (propertyBrowser != null){
                ////                propertyBrowser.GetType().InvokeMember("Microsoft.VisualStudio.Designer.Interfaces.IVSMDPropertyBrowser.Refresh", BindingFlags.DeclaredOnly |
                ////                    BindingFlags.Public | BindingFlags.NonPublic |
                ////                    BindingFlags.Instance | BindingFlags.InvokeMethod, null, propertyBrowser, null);
                ////            }
            }
            catch (Exception ex)
            {
                IUIService UIservice = (IUIService)GetService(typeof(System.Windows.Forms.Design.IUIService));
                if (UIservice != null)
                {
                    UIservice.ShowError(ex);
                }
                else
                {
                    MessageBoxAdv.Show(ex.ToString());
                }
            }

            inRefreshPropertyBrowser = false;
        }

        #region Undo

        bool inOnWndProcPropertyChanged = false;

        void OnWndProcPropertyChanged()
        {
            ////            if (inOnWndProcPropertyChanged || inBroadcastComponentChanged)
            ////                return;
            ////
            ////            inOnWndProcPropertyChanged = true;
            ////            try
            ////            {
            ////                GridGroupingControlUndoInit undoInit = undoSnapshot;
            ////               propertyReset = false;
            ////                propertyChanged = false;
            ////                if (undoInit != null)
            ////                {
            ////                    IOleUndoManager oleUndoManager = (IOleUndoManager) Host.GetService(typeof(IOleUndoManager));
            ////                   if (oleUndoManager != null)
            ////                    {
            ////                        string assName = Host.GetType().Assembly.FullName;
            ////
            ////                        if (assName.IndexOf("1.0.5000") != -1) // && !firstTime)
            ////                        {
            ////                            // 1.1 Workaround: DesignerHost.OnComponentChanged adds a "Set Property '###'" UndoInit
            ////                            // that is empty. This is not wanted since we provide our own undo init. To get rid
            ////                            // of it we call UndoTo do undo it. This call will have no effect on the property
            ////                            // settings of the grouping control since the UndoInit itself is empty.
            ////                            // With .NET 1.0 things work perfect and no workaround is needed.
            ////                            string s = null;
            ////                            try
            ////                            {
            ////                                s = oleUndoManager.GetLastUndoDescription();
            ////                            }
            ////                            catch {}
            ////                            try
            ////                            {
            ////                                if (s == null || s.StartsWith("Set Property"))
            ////                                {
            ////                                    oleUndoManager.UndoTo(null);
            ////                                }
            ////                            }
            ////                            catch {}
            ////                        }
            ////                        try
            ////                        {
            ////                            oleUndoManager.Add(undoInit);
            ////                            //firstTime = false;
            ////                        }
            ////                        catch {}
            ////                   }
            ////                }
            ////            }
            ////            finally
            ////            {
            ////                inOnWndProcPropertyChanged = false;
            ////            }
        }

        private void groupingControl_PropertyChanging(object sender, DescriptorPropertyChangedEventArgs e)
        {
            if (inBroadcastComponentChanged)
            {
                return;
            }

            if (undoSnapshot == null && !inUndo)
            {
                undoSnapshot = new GridGroupingControlUndoInit(this, this.groupingControl);
            }
        }

        private void groupingControl_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            this.isModified = true;
            EventArgs inner = e.Inner;

            if (!propertyReset || inBroadcastComponentChanged)
            {
                // "Reset###" of collections don't trigger ComponentChanged event ...
                while (inner != null)
                {
                    if (inner is DescriptorPropertyChangedEventArgs)
                    {
                        e = (DescriptorPropertyChangedEventArgs)inner;
                        inner = e.Inner;
                    }
                    else if (inner is ListPropertyChangedEventArgs)
                    {
                        if (((ListPropertyChangedEventArgs)inner).Action == ListPropertyChangedType.Refresh)
                        {
                            if (undoSnapshot != null)
                            {
                                propertyReset = true;
                                propertyChanged = true;
                                undoSnapshot.Name = "Change " + e.PropertyName;
                                ////this.BroadcastComponentChanged();
                            }
                        }

                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void componentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            ForceRefill(e.Component);
            if (!propertyReset)
            {
                if (undoSnapshot != null && !inUndo)
                {
                    if (e.Component is IComponent)
                    {
                        undoSnapshot = null;
                    }
                    else if (e.Member != null)
                    {
                        undoSnapshot.Name = "Set Property '" + e.Member.Name + "'";
                    }

                    this.propertyChanged = true;
                }
                else
                {
                    undoSnapshot = null;
                }
            }
        }

        #endregion

        #region Fill DataAdapter

        DataSet GetDataset(object list)
        {
            if (list is DataTable)
            {
                return ((DataTable)list).DataSet;
            }
            else if (list is DataView)
            {
                return ((DataView)list).DataViewManager.DataSet;
            }
            else if (list is DataViewManager)
            {
                return ((DataViewManager)list).DataSet;
            }
            else if (list is DataSet)
            {
                return (DataSet)list;
            }
#if SyncfusionFramework2_0
            else if (list is BindingSource)
            {
                return GetDataset(((BindingSource)list).List);
            }
#endif
            else
            {
                return null;
            }
        }

        public void ForceRefill()
        {
            if (groupingControlPreviewDialog != null)
            {
                groupingControlPreviewDialog.Dispose();
                groupingControlPreviewDialog = null;
            }

            TypeDescriptor.Refresh(this.groupingControl);
        }

        bool ContainerHasAdapter
        {
            get
            {
                foreach (IComponent component in Host.Container.Components)
                {
                    if (component is IDataAdapter)
                    {
                        return true;
                    }
#if SyncfusionFramework2_0
                    else
                    {
                        IDataAdapter addapter = GetIAdapterFromDataTableAdapter(component);
                        if (addapter != null)
                        {
                            return true;
                        }
                    }
#endif
                }

                return false;
            }
        }

#if SyncfusionFramework2_0
        object GetData(object component)
        {
            object data = null;
            Type cmpType = component.GetType();
            if (cmpType.Name.EndsWith("Adapter"))
            {
                MethodInfo mi = cmpType.GetMethod(
                    "GetData",
                    BindingFlags.Instance | BindingFlags.Public | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                if (mi != null)
                {
                    data = mi.Invoke(component, new object[0]);
                }
            }

            return data;
        }

        IDataAdapter GetIAdapterFromDataTableAdapter(object cmp)
        {
            IDataAdapter adapter = null;
            Type cmpType = cmp.GetType();
            if (cmpType.Name.EndsWith("Adapter"))
            {
                PropertyInfo pi = cmpType.GetProperty(
                    "Adapter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                if (pi != null)
                {
                    MethodInfo mi = pi.GetGetMethod(true);

                    if (mi != null)
                    {
                        adapter = (IDataAdapter)mi.Invoke(cmp, new object[0]);
                    }
                }
            }

            return adapter;
        }
#endif

        public IDataAdapter[] Adapters
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach (IComponent component in Host.Container.Components)
                {
                    if (component is IDataAdapter)
                    {
                        list.Add(component);
                    }
#if SyncfusionFramework2_0
                    else
                    {
                        IDataAdapter addapter = GetIAdapterFromDataTableAdapter(component);
                        if (addapter != null)
                        {
                            list.Add(addapter);
                        }
                    }
#endif
                }

                return (IDataAdapter[])list.ToArray(typeof(IDataAdapter));
            }
        }

        bool ContainerHasTypedDataset
        {
            get
            {
                foreach (IComponent component in Host.Container.Components)
                {
                    if (component.GetType().IsSubclassOf(typeof(DataSet)))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        void ForceRefill(object component)
        {
            if (component == null)
            {
                return;
            }

            if (component is IDataAdapter || component.GetType().IsSubclassOf(typeof(DataSet)))
            {
                ForceRefill();
            }

            if (this.groupingControl != null)
            {
                this.groupingControl.pdcCache = null;
                this.groupingControl.tdPropertiesCache.Clear();
            }

            TypeDescriptor.Refresh(this.groupingControl);
        }

        private void componentChangeService_ComponentAdded(object sender, ComponentEventArgs e)
        {
            ForceRefill(e.Component);
        }

        private void componentChangeService_ComponentRemoving(object sender, ComponentEventArgs e)
        {
            ForceRefill(e.Component);
        }

        private void componentChangeService_ComponentChanging(object sender, ComponentChangingEventArgs e)
        {
            if (inOnWndProcPropertyChanged)
            {
                return;
            }

            /* 
             * Possible idea to add back undo support for Whidbey ... but it results in InvalidCastExeption
             * 
            IOleUndoManager oleUndoManager = (IOleUndoManager) Host.GetService(typeof(IOleUndoManager));
            GridGroupingControlUndoInit undoInit = null;
            if (oleUndoManager != null)
            {
                undoInit = new GridGroupingControlUndoInit(this, this.groupingControl);
                oleUndoManager.Add(undoInit);
            }
            */

            ForceRefill(e.Component);
        }

        private void componentChangeService_ComponentAdding(object sender, ComponentEventArgs e)
        {
            ForceRefill(e.Component);
        }

        private void componentChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            ForceRefill(e.Component);
        }

        private void componentChangeService_ComponentRename(object sender, ComponentRenameEventArgs e)
        {
            ForceRefill(e.Component);
        }

        private void Engine_DataSourceChanged(object sender, EventArgs e)
        {
            if (this.groupingControl != null)
            {
                this.groupingControl.pdcCache = null;
                this.groupingControl.tdPropertiesCache.Clear();
                this.groupingControl.TableDescriptor.Relations.Reset();
            }

            ForceRefill();
        }

        #endregion

        #region Preview Dialog

        void ShowPreviewDialog()
        {
            IOleUndoManager oleUndoManager = (IOleUndoManager)Host.GetService(typeof(IOleUndoManager));

            IUIService UIservice = (IUIService)GetService(typeof(System.Windows.Forms.Design.IUIService));
            showing = true;

            if (groupingControlPreviewDialog == null)
            {
                groupingControlPreviewDialog = new GridGroupingControlPreviewDialog();
                groupingControlPreviewDialog.RightToLeft = this.groupingControl.RightToLeft;
            }

            object ds = this.groupingControl.DataSource;
            DataSet dataSet = GetDataset(ds);
            DialogResult action = DialogResult.None;

#if SyncfusionFramework2_0
            if (ContainerHasAdapter && dataSet != null)
            {
                dataSet.EnforceConstraints = false;
                foreach (IComponent component in Host.Container.Components)
                {
                    GetData(component);
                }

                foreach (IDataAdapter adapter in this.Adapters)
                {
                retry:
                    try
                    {
                        adapter.Fill(dataSet);
                    }
                    catch (Exception ex)
                    {
                        action = ShowFillException(adapter, dataSet, UIservice, action, ex);
                        if (action == DialogResult.Retry)
                        {
                            goto retry;
                        }
                        else if (action == DialogResult.Abort)
                        {
                            this.ForceRefill();
                            return;
                        }
                    }
                }

                dataSet.EnforceConstraints = true;
            }
#else
            if (ContainerHasAdapter && dataSet != null )
            {
                DataSet clone = dataSet;//.Clone();
                foreach (IDataAdapter adapter in this.Adapters)
                {
                retry:
                    try
                    {
                        adapter.Fill(clone);
                    }
                    catch (Exception ex)
                    {
                        action = ShowFillException(adapter, dataSet, UIservice, action, ex);
                        if (action == DialogResult.Retry)
                            goto retry;
                        else if (action == DialogResult.Abort)
                        {
                            this.ForceRefill();
                            return;
                        }
                    }
                }
//                if (ds is DataTable)
//                    ds = clone.Tables[((DataTable) ds).TableName];
//                else if (ds is DataSet)
//                    ds = clone;
            }
#endif

            groupingControlPreviewDialog.GroupingControl.DataSource = ds;

            groupingControlPreviewDialog.GroupingControl.DataMember = this.groupingControl.DataMember;
            groupingControlPreviewDialog.GroupingControl.InitializeFrom(this.groupingControl);

            groupingControlPreviewDialog.Text = this.groupingControl.Name;
            groupingControlPreviewDialog.GroupingControl.Update();

            //// Wire events
            groupingControlPreviewDialog.GroupingControl.PropertyChanged += new DescriptorPropertyChangedEventHandler(previewControl_PropertyChanged);
            groupingControlPreviewDialog.Load += new EventHandler(previewDialog_Load);
            groupingControlPreviewDialog.GroupingControl.QueryCellStyleInfo += new GridTableCellStyleInfoEventHandler(previewDialog_QueryCellStyleInfo);

            if (UIservice != null)
            {
                UIservice.ShowDialog(groupingControlPreviewDialog);
            }
            else
            {
                groupingControlPreviewDialog.ShowDialog();
            }

            //// Unwire events
            groupingControlPreviewDialog.GroupingControl.PropertyChanged -= new DescriptorPropertyChangedEventHandler(previewControl_PropertyChanged);
            groupingControlPreviewDialog.GroupingControl.QueryCellStyleInfo -= new GridTableCellStyleInfoEventHandler(previewDialog_QueryCellStyleInfo);
            groupingControlPreviewDialog.Load -= new EventHandler(previewDialog_Load);

            ////#if SyncfusionFramework2_0
            //// Clear tables so that contents are not shown in designer form.
            if (dataSet != null)
            {
                dataSet.EnforceConstraints = false;
                foreach (DataTable dt in dataSet.Tables)
                {
                    dt.Clear();
                }

                dataSet.EnforceConstraints = true;
            }
            ////#endif

            DialogResult dialogResult = DialogResult.No;

            if (modified && UIservice != null)
            {
                dialogResult = UIservice.ShowMessage("Apply Changes you made in Preview dialog to " + groupingControl.Name + " ?\r\n", "Preview and Edit", MessageBoxButtons.YesNo);
            }

            //// Changes are not saved into .cs file at this time, only to design time state.
            if (dialogResult == DialogResult.Yes)
            {
                GridGroupingControlUndoInit undoInit = null;
                if (oleUndoManager != null)
                {
                    undoInit = new GridGroupingControlUndoInit(this, this.groupingControl);
                }

                Form f = this.groupingControl.FindForm();

                this.isModified = false;
                this.groupingControl.InitializeFrom(groupingControlPreviewDialog.GroupingControl);
                if (isModified && !this.inUndo)
                {
                    BroadcastComponentChanged();
                }

                groupingControl.Refresh();
                RefreshPropertyBrowser();

                if (oleUndoManager != null)
                {
                    oleUndoManager.Add(undoInit);
                }
            }
        }

        DialogResult ShowFillException(IDataAdapter adapter, DataSet dataSet, IUIService UIservice, DialogResult action, Exception ex)
        {
            if (action == DialogResult.None || action == DialogResult.Retry)
            {
                string adapterName = ((IComponent)adapter).Site != null ? ((IComponent)adapter).Site.Name : adapter.ToString();
                string dataSetName = ((IComponent)dataSet).Site != null ? ((IComponent)dataSet).Site.Name : dataSet.DataSetName;
                string msg = "Failed to execute line " + adapterName + ".Fill(" + dataSetName + ")\r\n"
                    + "Exception: " + ex.Message.ToString() + "\r\n";
                if (ex.InnerException != null)
                {
                    msg += ex.InnerException.Message;
                }

                msg += "\r\n";
                msg += "Is the datasource service running? (e.g. Sql Server or MSDE)";
                msg += "\r\n";
                msg += "\r\n";
                msg += "If you ignore this problem, the grid will display the empty dataset without records.";
                msg += "\r\n";
                msg += "\r\n";
                if (UIservice != null)
                {
                    action = UIservice.ShowMessage(msg, Localization.SR.GetString(Localization.SR.FailedToFillDataset), MessageBoxButtons.AbortRetryIgnore);
                }
                else
                {
                    action = MessageBoxAdv.Show(msg, Localization.SR.GetString(Localization.SR.FailedToFillDataset), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                }
            }

            return action;
        }

        private void previewControl_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            if (showing)
            {
                return;
            }

            if (groupingControlPreviewDialog != null && !this.modified)
            {
                groupingControlPreviewDialog.Text = groupingControlPreviewDialog.Text + "*";
            }

            modified = true;
        }

        private void previewDialog_Load(object sender, EventArgs e)
        {
            groupingControlPreviewDialog.Load -= new EventHandler(previewDialog_Load);
            modified = false;
            groupingControlPreviewDialog.Text = this.groupingControl.Name;
        }

        private void previewDialog_QueryCellStyleInfo(object sender, GridTableCellStyleInfoEventArgs e)
        {
            showing = false;
        }

        #endregion

        #region Xml Serialization Helpers

        void ApplyLookAndFeel(XmlReader xr, string info, string undoDescription)
        {
            IUIService UIservice = (IUIService)GetService(typeof(System.Windows.Forms.Design.IUIService));
            try
            {
                GridGroupingLookAndFeel lookAndFeel = GridGroupingLookAndFeel.CreateFromXml(xr);

                if (lookAndFeel != null)
                {
                    IOleUndoManager oleUndoManager = (IOleUndoManager)Host.GetService(typeof(IOleUndoManager));
                    Form f = this.groupingControl.FindForm();

                    GridGroupingControlUndoInit undoInit = null;
                    if (oleUndoManager != null)
                    {
                        undoInit = new GridGroupingControlUndoInit(this, this.groupingControl, undoDescription);
                    }

                    this.isModified = false;
                    lookAndFeel.ApplyTo(this.groupingControl);
                    if (isModified && !this.inUndo)
                    {
                        BroadcastComponentChanged();
                    }

                    groupingControl.Refresh();
                    RefreshPropertyBrowser();

                    if (oleUndoManager != null)
                    {
                        oleUndoManager.Add(undoInit);
                    }
                }
            }
            catch (Exception ex)
            {
                if (UIservice != null)
                {
                    UIservice.ShowError(ex);
                }
            }
        }

        void ApplySchema(XmlReader xr, string info, string undoDescription)
        {
            IUIService UIservice = (IUIService)GetService(typeof(System.Windows.Forms.Design.IUIService));
            try
            {
                GridEngine engine = GridEngine.CreateFromXml(xr);

                if (engine != null)
                {
                    DialogResult action = DialogResult.None;

                    string msg = "Do you want to inititialize the grouping control with the definition from " + info + "?" +
                        "\r\nYour existing settings will all be replaced.";

                    if (UIservice != null)
                    {
                        action = UIservice.ShowMessage(msg, Localization.SR.GetString(Localization.SR.PasteEngineSchema), MessageBoxButtons.YesNo);
                    }
                    else
                    {
                        action = MessageBoxAdv.Show(msg, Localization.SR.GetString(Localization.SR.PasteEngineSchema), MessageBoxButtons.YesNo);
                    }

                    if (action == DialogResult.Yes)
                    {
                        IOleUndoManager oleUndoManager = (IOleUndoManager)Host.GetService(typeof(IOleUndoManager));
                        Form f = this.groupingControl.FindForm();

                        GridGroupingControlUndoInit undoInit = null;
                        if (oleUndoManager != null)
                        {
                            undoInit = new GridGroupingControlUndoInit(this, this.groupingControl, undoDescription);
                        }

                        this.isModified = false;
                        this.groupingControl.Engine.InitializeFrom(engine);
                        if (isModified && !this.inUndo)
                        {
                            BroadcastComponentChanged();
                        }

                        groupingControl.Refresh();
                        RefreshPropertyBrowser();

                        if (oleUndoManager != null)
                        {
                            oleUndoManager.Add(undoInit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (UIservice != null)
                {
                    UIservice.ShowError(ex);
                }
            }
        }
        #endregion

        ////bool firstTime = true;
        bool isModified = false;

        #region Verbs

        /// <override/>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (this.verbs == null)
                {
                    this.verbs = new DesignerVerbCollection();
                    this.verbs.Add(new DesignerVerb("Preview and Edit ...", new EventHandler(this.verbs_Preview)));
                    this.verbs.Add(new DesignerVerb("Copy Schema", new EventHandler(this.verbs_CopySchema)));
                    this.verbs.Add(new DesignerVerb("Paste Schema", new EventHandler(this.verbs_PasteSchema)));
                    this.verbs.Add(new DesignerVerb("Save Schema ...", new EventHandler(this.verbs_SaveSchema)));
                    this.verbs.Add(new DesignerVerb("Choose Schema ...", new EventHandler(this.verbs_ChooseSchema)));
                    this.verbs.Add(new DesignerVerb("Copy Look and Feel", new EventHandler(this.verbs_CopyLookAndFeel)));
                    this.verbs.Add(new DesignerVerb("Paste Look and Feel", new EventHandler(this.verbs_PasteLookAndFeel)));
                    this.verbs.Add(new DesignerVerb("Save Look and Feel ...", new EventHandler(this.verbs_SaveLookAndFeel)));
                    this.verbs.Add(new DesignerVerb("Choose Look and Feel ...", new EventHandler(this.verbs_ChooseLookAndFeel)));
                }

                return verbs;
            }
        }

        void verbs_Preview(object sender, EventArgs e)
        {
            ShowPreviewDialog();
        }

        void verbs_CopySchema(object sender, EventArgs e)
        {
            StringWriter sw = new StringWriter();
            this.groupingControl.Engine.WriteXml(sw);
            Clipboard.SetDataObject(sw.ToString());
        }

        void verbs_SaveSchema(object sender, EventArgs e)
        {
            FileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                XmlTextWriter xw = new XmlTextWriter(dlg.FileName, System.Text.Encoding.UTF8);
                xw.Formatting = Formatting.Indented;
                this.groupingControl.WriteXmlSchema(xw);
                xw.Close();
            }
        }

        void verbs_ChooseSchema(object sender, EventArgs e)
        {
            FileDialog dlg = new OpenFileDialog();
            dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                XmlReader xr = new XmlTextReader(dlg.FileName);
                ApplySchema(xr, Path.GetFileName(dlg.FileName), "Engine Schema (" + Path.GetFileName(dlg.FileName) + ")");
                xr.Close();
            }
        }

        void verbs_PasteSchema(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            IUIService UIservice = (IUIService)GetService(typeof(System.Windows.Forms.Design.IUIService));

            if (iData.GetDataPresent(DataFormats.Text))
            {
                string xmlSchema = (string)iData.GetData(DataFormats.Text);
                XmlReader xr = new XmlTextReader(new StringReader(xmlSchema));
                ApplySchema(xr, "the clipboard", "Engine Schema " + this.groupingControl.Name);
            }
            else
            {
                string error = "No clipboard data found";
                if (UIservice != null)
                {
                    UIservice.ShowError(error);
                }
            }
        }

        void verbs_CopyLookAndFeel(object sender, EventArgs e)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            this.groupingControl.WriteXmlLookAndFeel(xw);
            Clipboard.SetDataObject(sw.ToString());
            xw.Close();
        }

        void verbs_SaveLookAndFeel(object sender, EventArgs e)
        {
            FileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                XmlTextWriter xw = new XmlTextWriter(dlg.FileName, System.Text.Encoding.UTF8);
                xw.Formatting = Formatting.Indented;
                groupingControl.WriteXmlLookAndFeel(xw);
                xw.Close();
            }
        }

        void verbs_ChooseLookAndFeel(object sender, EventArgs e)
        {
            FileDialog dlg = new OpenFileDialog();
            dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                XmlReader xr = new XmlTextReader(dlg.FileName);
                ApplyLookAndFeel(xr, Path.GetFileName(dlg.FileName), "Look and Feel  (" + Path.GetFileName(dlg.FileName) + ")");
                xr.Close();
            }
        }

        void verbs_PasteLookAndFeel(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            IUIService UIservice = (IUIService)GetService(typeof(System.Windows.Forms.Design.IUIService));

            if (iData.GetDataPresent(DataFormats.Text))
            {
                string xmlLookAndFeel = (string)iData.GetData(DataFormats.Text);
                XmlReader xr = new XmlTextReader(new StringReader(xmlLookAndFeel));
                ApplyLookAndFeel(xr, "the clipboard", "Look and Feel " + this.groupingControl.Name);
            }
            else
            {
                string error = "No clipboard data found";
                if (UIservice != null)
                {
                    UIservice.ShowError(error);
                }
            }
        }

        #endregion

        bool inBroadcastComponentChanged = false;

        private void BroadcastComponentChanged()
        {
            IComponentChangeService changeService = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            if (changeService != null)
            {
                inBroadcastComponentChanged = true;

                this.undoSnapshot = null;
                this.propertyChanged = false;
                this.propertyReset = false;

                changeService.OnComponentChanging(Control, null);

                this.undoSnapshot = null;
                this.propertyChanged = false;
                this.propertyReset = false;

                changeService.OnComponentChanged(Control, null, null, null);

                this.undoSnapshot = null;
                this.propertyChanged = false;
                this.propertyReset = false;

                inBroadcastComponentChanged = false;
            }
        }
    }

#if SyncfusionFramework2_0
    // Nested Types
    [ComplexBindingProperties("DataSource", "DataMember")]
    internal class GridGroupingControlChooseDataSourceActionList : DesignerActionList
    {
        // Methods
        public GridGroupingControlChooseDataSourceActionList(GridGroupingControlDesigner owner)
            : base(owner.Component)
        {
            this.owner = owner;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection actionItemCollection = new DesignerActionItemCollection();

            actionItemCollection.Add(new DesignerActionHeaderItem("Data", "Data"));

            DesignerActionPropertyItem dataSourceItem = new DesignerActionPropertyItem("DataSource", "Choose DataSource", "Data");
            dataSourceItem.RelatedComponent = this.owner.Component;
            actionItemCollection.Add(dataSourceItem);

            DesignerActionPropertyItem dataMemberItem = new DesignerActionPropertyItem("DataMember", "Choose DataMember", "Data");
            dataMemberItem.RelatedComponent = this.owner.Component;
            actionItemCollection.Add(dataMemberItem);

            return actionItemCollection;
        }

        // Properties
        [AttributeProvider(typeof(IListSource))]
        public object DataSource
        {
            get
            {
                return this.owner.DataSource;
            }
           
            set
            {
                GridGroupingControl view1 = (GridGroupingControl)this.owner.Component;
                IDesignerHost host1 = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(view1)["DataSource"];
                IComponentChangeService service1 = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                object[] objArray1 = new object[1] { view1.Name };
                DesignerTransaction transaction1 = host1.CreateTransaction("GridGroupingControlChooseDataSourceTransactionString " + objArray1.ToString());
                try
                {
                    service1.OnComponentChanging(this.owner.Component, descriptor1);
                    this.owner.DataSource = value;
                    service1.OnComponentChanged(this.owner.Component, descriptor1, null, null);
                    transaction1.Commit();
                    transaction1 = null;
                }
                finally
                {
                    if (transaction1 != null)
                    {
                        transaction1.Cancel();
                    }
                }
            }
        }

        [DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public string DataMember
        {
            get
            {
                return this.owner.DataMember;
            }

            set
            {
                GridGroupingControl view1 = (GridGroupingControl)this.owner.Component;
                IDesignerHost host1 = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(view1)["DataMember"];
                IComponentChangeService service1 = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                object[] objArray1 = new object[1] { view1.Name };
                DesignerTransaction transaction1 = host1.CreateTransaction("GridGroupingControlChooseDataMemberTransactionString " + objArray1.ToString());
                try
                {
                    service1.OnComponentChanging(this.owner.Component, descriptor1);
                    this.owner.DataMember = value;
                    service1.OnComponentChanged(this.owner.Component, descriptor1, null, null);
                    transaction1.Commit();
                    transaction1 = null;
                }
                finally
                {
                    if (transaction1 != null)
                    {
                        transaction1.Cancel();
                    }
                }
            }
        }

        //// Fields
        private GridGroupingControlDesigner owner;
    }

    internal class DesignerActionVerbList : DesignerActionList
    {
        //// Methods
        public DesignerActionVerbList(DesignerVerb[] verbs)
            : base(null)
        {
            this._verbs = verbs;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection coll = new DesignerActionItemCollection();
            coll.Add(new DesignerActionHeaderItem("Appearance, Look and feel", "Grid"));
            for (int i = 0; i < this._verbs.Length; i++)
            {
                if ((this._verbs[i].Visible && this._verbs[i].Enabled) && this._verbs[i].Supported)
                {
                    coll.Add(new DesignerActionVerbItem(this._verbs[i]));
                }
            }

            return coll;
        }

        //// Properties
        public override bool AutoShow
        {
            get
            {
                return false;
            }
        }

        //// Fields
        private DesignerVerb[] _verbs;
    }

    internal class DesignerActionVerbItem : DesignerActionMethodItem
    {
        //// Methods
        public DesignerActionVerbItem(DesignerVerb verb)
            : base(null, string.Empty, string.Empty)
        {
            if (verb == null)
            {
                throw new ArgumentNullException();
            }

            this._targetVerb = verb;
        }

        public override void Invoke()
        {
            this._targetVerb.Invoke();
        }

        //// Properties
        public override string Category
        {
            get
            {
                return "Grid"; ////"Verbs";
            }
        }

        public override string Description
        {
            get
            {
                return this._targetVerb.Description;
            }
        }

        public override string DisplayName
        {
            get
            {
                return this._targetVerb.Text;
            }
        }

        public override bool IncludeAsDesignerVerb
        {
            get
            {
                return false;
            }
        }

        public override string MemberName
        {
            get
            {
                return null;
            }
        }

        //// Fields
        private DesignerVerb _targetVerb;
    }

    internal class DesignerActionSupportList : DesignerActionList
    {
        public DesignerActionSupportList(IComponent component)
            : base(component)
        {
            ////this.AutoShow = true;
        }

        /// <summary>
        /// Returns the collection of <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> objects contained in the list.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> array that contains the items in this list.
        /// </returns>
        /// Override
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection actionItems = new DesignerActionItemCollection();

            actionItems.Add(new DesignerActionPropertyItem("GridVisualStyles", "Skins", "Grid"));

            actionItems.Add(new DesignerActionHeaderItem("Documentation and Support Resource", "Assistance"));

            actionItems.Add(
                new DesignerActionMethodItem(
                this,
                "OnViewOnLineDocumentation",
                "Online Documentation", 
                "Assistance"));

            actionItems.Add(
                new DesignerActionMethodItem(
                this,
                "OnGoToForums",
                "Forums Support", 
                "Assistance"));

            actionItems.Add(
              new DesignerActionMethodItem(
                this,
                "OnGoToDirectTrac",
                "Direct-Trac Support",
                "Assistance"));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "Search",
                    "Search: ", 
                    "Assistance"));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "KeyWord",
                    string.Empty, 
                    "Assistance"));
            
            actionItems.Add(
                new DesignerActionMethodItem(
                    this,
                    "OnSearch",
                    "Search...", 
                    "Assistance"));

            return actionItems;
        }
        
        #region ActionItem Members and Helpers

        protected void OnSearch()
        {
            Cursor current = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                switch (this.search)
                {
                    case SearchOptions.Forums:
                        System.Diagnostics.Process.Start(string.Format("http://www.syncfusion.com/support/forums/grid-windows/search/{0}", keyword));
                        break;
                    case SearchOptions.KnowledgeBase:
                        System.Diagnostics.Process.Start(string.Format("http://www.syncfusion.com/support/kb/tag/{0}", keyword));
                        break;
                    case SearchOptions.Syncfusion:
                        System.Diagnostics.Process.Start(string.Format("http://www.syncfusion.com/search/{0}", keyword));
                        break;
                }
            }
            finally
            {
                Cursor.Current = current;
            }
        }

        private void OnViewOnLineDocumentation()
        {
            OnView("http://help.syncfusion.com/resources/User%20Interface/Windows%20Forms/Grid");
        }

        private void OnGoToForums()
        {
            OnView("http://www.syncfusion.com/support/forums/grid-windows");
        }

        private void OnGoToDirectTrac()
        {
            OnView("http://www.syncfusion.com/Account/Logon?ReturnUrl=%2fsupport%2fdirecttrac");
        }

        private void OnView(string link)
        {
            Cursor current = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                System.Diagnostics.Process.Start(link);
            }
            finally
            {
                Cursor.Current = current;
            }
        }

        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return Grid.TableOptions.GridVisualStyles;
            }

            set
            {
                IDesignerHost host = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                PropertyDescriptor pd = TypeDescriptor.GetProperties(Grid.TableOptions)["GridVisualStyles"];
                IComponentChangeService service = Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                object[] objArray = new object[1] { Grid.Name };
                DesignerTransaction transaction = host.CreateTransaction("GridVisualStylesTransactionString " + objArray.ToString());
                try
                {
                    service.OnComponentChanging(Grid.TableOptions, pd);
                    Grid.TableOptions.GridVisualStyles = value;
                    service.OnComponentChanged(Grid.TableOptions, pd, null, null);
                    transaction.Commit();
                    transaction = null;
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Cancel();
                    }
                }
            }
        }
       
        public string KeyWord
        {
            get
            {
                return keyword;
            }

            set
            {
                keyword = value;
            }
        }

        public SearchOptions Search
        {
            get { return this.search; }
            set { this.search = value; }
        }

        private GridGroupingControl Grid
        {
            get { return (GridGroupingControl)this.Component; }
        }

        #endregion ////ActionItem Members and Helpers

        //// Fields
        string keyword = string.Empty;
        SearchOptions search = SearchOptions.KnowledgeBase;
    }

    internal enum SearchOptions
    {
        /// <summary>
        /// Represents Syncfusion
        /// </summary>
        Syncfusion,

        /// <summary>
        /// Represents Forums
        /// </summary>
        Forums,

        /// <summary>
        /// Represents KnowledgeBase
        /// </summary>
        KnowledgeBase
    }
#endif

    class GridGroupingControlUndoInit : IOleUndoUnit
    {
        GridGroupingControlDesigner designer;
        GridGroupingControl savedState = new GridGroupingControl(true);
        GridGroupingControl groupingControl;
        private string name;

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public GridGroupingControlUndoInit(GridGroupingControlDesigner gd, GridGroupingControl control)
            : this(gd, control, null)
        {
        }

        public GridGroupingControlUndoInit(GridGroupingControlDesigner gd, GridGroupingControl control, string name)
        {
            designer = gd;
            groupingControl = control;
            savedState.InitializeFrom(groupingControl);
            this.name = name;
        }

        public int Do(IOleUndoManager pUndoManager)
        {
            if (pUndoManager != null)
            {
                pUndoManager.Add(new GridGroupingControlUndoInit(designer, groupingControl, name));
            }

            designer.inUndo = true;
            this.groupingControl.InitializeFrom(savedState);
            this.groupingControl.Refresh();
            designer.RefreshPropertyBrowser();
            designer.inUndo = false;

            ////savedState.Dispose();
            return 0;
        }

        public string GetDescription()
        {
            if (name == null)
            {
                return "Preview and Edit";
            }

            return name;
        }

        public int GetUnitType(ref Guid pClsid, out int plID)
        {
            pClsid = Guid.Empty;
            plID = 0;
            return -2147467263/*0x80004001*/;
        }

        public void OnNextAdd()
        {
            TraceUtil.TraceCurrentMethodInfo();
        }
    }

    #region COM Interface Definitions

    [Guid(@"74946810-37A0-11D2-A273-00C04F8EF4FF"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IVSMDPropertyBrowser
    {
    }

    [Guid(@"D001F200-EF97-11CE-9BC9-00AA00608E01"),
    ComVisible(true),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IOleUndoManager
    {
        void Open(object pPUU);

        void Close(object pPUU, bool fCommit);

        void Add(IOleUndoUnit pUU);

        [PreserveSig]
        int GetOpenParentState(out int pdwState);

        [PreserveSig]
        int DiscardFrom(IOleUndoUnit pUU);

        void UndoTo(IOleUndoUnit pUU);

        void RedoTo(IOleUndoUnit pUU);

        [return: MarshalAs(UnmanagedType.Interface)]
        object EnumUndoable();

        [return: MarshalAs(UnmanagedType.Interface)]
        object EnumRedoable();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetLastUndoDescription();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetLastRedoDescription();

        void Enable(int fEnable);
    }

    [ComVisible(true),
    Guid(@"894AD3B0-EF97-11CE-9BC9-00AA00608E01"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IOleUndoUnit
    {
        [PreserveSig]
        int Do(IOleUndoManager pUndoManager);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDescription();

        [PreserveSig]
        int GetUnitType(ref Guid pClsid, out int plID);

        void OnNextAdd();
    }
    #endregion
}