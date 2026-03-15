//-------------------------------------------------------------------------------------------------
// <copyright file="GridControlDesigner.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Diagnostics;
using System.Drawing.Text;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;
using Microsoft.VisualBasic;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridControlDesigner : ControlDesigner
    {
        // we use load mode to deal with serialization versioning issues
        // refer to OnLoadComplete for implementation
        private enum LoadMode
        {
            /// <summary>
            /// initial load may have serialization errors. add assembly resolver and redo
            /// </summary>
            Initial,  

            /// <summary>
            /// reload is done. remove assembly resolver and set to LoadMode.Done mode
            /// </summary>
            Clearing, 

            /// <summary>
            /// Represent Done.
            /// </summary>
            Done,
        }

        private DesignerVerbCollection verbs = null;
        private int formatRowCount = 10;
        private int formatColCount = 10;
        private bool persistRowStyles = false;

        private static string suiteRegistryKey = @"Software\Syncfusion\Essential Suite\Grid";
        private static string refreshModeRegistryValue = "RefreshMode";
        private static LoadMode loadMode = LoadMode.Initial;

        // designer verbs
        DesignerVerb editDesigner;
        DesignerVerb touchAndSave;
        DesignerVerb displayBaseStyles;

        static GridControlDesigner()
        {
            GridControlDesigner.loadMode = ShouldSetRefreshMode() ? LoadMode.Initial : LoadMode.Done;
        }

#if SyncfusionFramework2_0
        IDesignerHost host;

        IDesignerHost Host
        {
            get
            {
                if (host == null)
                {
                    host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    host.Activated += new EventHandler(this.OnDesignerActivate);
                    host.Deactivated += new EventHandler(this.OnDesignerDeactivate);
                }

                return this.host;
            }
        }

        ////With Whidbey the default UndoEngine provided by the designer
        ////is way to slow for a GridControl because of its many
        ////properties. You will notice it especially when you try
        ////to resize the control.

        ////The problem is that when you start resizing the control undo information 
        ////is generated in the Visual Studio Designer. At that time the IDE uses 
        ////reflection to loop through each and every property in the grouping grid 
        ////and checks whether it needs to be serialized to code or not (just like 
        ////when the InitializeComponent code is created). 

        ////What you would need to do is when you start resizing the control do this: 
        ////Press the mouse button, resize it slightly but keep holding down the button. 
        ////You will notice undo information being generated. After you see the window 
        ////react to the resize command you will now be able to quickly resize the 
        ////Control. Key is to keep holding the mouse button down and wait until the 
        ////designer reacts.

        ////Since this experience is not pleasant at all for customers we therefore
        ////decided to disable undo generation altogether for the form a GridControl
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

            object h = Host;

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

        private static bool ShouldSetRefreshMode()
        {
            RegistryKey gridKey = null;
            bool returnValue = true;
            try
            {
                gridKey = GridControlDesigner.GetSuiteRootKey();
                if (gridKey != null)
                {
                    object o = gridKey.GetValue(refreshModeRegistryValue);

                    if (o == null)
                    {
                        return true;
                    }

                    int dw = (int)o;
                    returnValue = (dw == 0) ? false : true;
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (ExceptionManager.RaiseExceptionCatched(null, ex))
                {
                    throw;
                }
                ////e.ToString();
            }
            finally
            {
                if (gridKey != null)
                {
                    gridKey.Close();
                }
            }

            return returnValue;
        }

        private static RegistryKey GetSuiteRootKey()
        {
            // read the registry and get the path
            RegistryKey rootKey = Registry.CurrentUser;

            string path = suiteRegistryKey;

            RegistryKey subKey = rootKey.OpenSubKey(path);

            if (subKey == null)
            {
                rootKey = Registry.LocalMachine;
                subKey = rootKey.OpenSubKey(path);
            }

            return subKey;
        }

        /// <internalonly/>
        public GridControlDesigner()
        { 
        }

#if !SyncfusionFramework2_0
           public override void OnSetComponentDefaults()
        {
            GridControl grid = this.Control as GridControl;
            if (grid != null)
            {
                grid.SerializeCellsBehavior = GridSerializeCellsBehavior.SerializeAsRangeStylesIntoCode;
                grid.UseRightToLeftCompatibleTextBox = true;
            }
            base.OnSetComponentDefaults ();
        }
#else
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            GridControl grid = this.Control as GridControl;
            if (grid != null)
            {
                grid.SerializeCellsBehavior = GridSerializeCellsBehavior.SerializeAsRangeStylesIntoCode;
                grid.UseRightToLeftCompatibleTextBox = true;
            }

            base.InitializeNewComponent(defaultValues);
        }
#endif

        [DesignOnly(true), Browsable(false), DefaultValue(10)]
        [Description("This property is obsolete and is provided for compatibility only.")]
        public int FormatRowCount
        {
            get
            {
                return this.formatRowCount;
            }

            set
            {
                this.formatRowCount = value;
            }
        }

        [DesignOnly(true), Browsable(false), DefaultValue(10)]
        [Description("This property is obsolete and is provided for compatibility only.")]
        public int FormatColCount
        {
            get
            {
                return this.formatColCount;
            }

            set
            {
                this.formatColCount = value;
            }
        }

        [DesignOnly(true), Browsable(false), DefaultValue(false)]
        [Description("This property is obsolete and is provided for compatibility only.")]
        public bool PersistRowStyles
        {
            get
            {
                return this.persistRowStyles;
            }

            set
            {
                this.persistRowStyles = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GridControl grid = this.Control as GridControl;

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
            }

            base.Dispose(disposing);
        }

        private void OnLoadComplete(object sender, EventArgs args)
        {
            if (GridControlDesigner.loadMode == LoadMode.Initial)
            {
                GridControlDesigner.loadMode = LoadMode.Clearing;
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(GridWindowsAssembly.AssemblyResolver);
                IDesignerLoaderService dsl = (IDesignerLoaderService)
                    this.GetService(typeof(IDesignerLoaderService));
                if (dsl != null)
                {
                    dsl.Reload();
                }
            }
            else if (GridControlDesigner.loadMode == LoadMode.Clearing)
            {
                GridControlDesigner.loadMode = LoadMode.Done;
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(GridWindowsAssembly.AssemblyResolver);
            }
        }

#if SyncfusionFramework2_0
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection actionLists = new DesignerActionListCollection();
                DesignerVerb[] verbs = new DesignerVerb[Verbs.Count];
                Verbs.CopyTo(verbs, 0);
                actionLists.Add(new DesignerActionSupportList(Component, verbs));
                return actionLists;
            }
        }
#endif

        /// <override/>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (this.verbs == null)
                {
                    this.editDesigner = new DesignerVerb("Edit", new EventHandler(this.OnEditDesigner));
                    this.touchAndSave = new DesignerVerb("Touch It", new EventHandler(this.OnTouchAndSave));

                    this.displayBaseStyles = new DesignerVerb("Edit base styles", new EventHandler(this.OnDisplayBaseStyles));

                    this.verbs = new DesignerVerbCollection();

                    this.verbs.Add(this.editDesigner);
                    this.verbs.Add(this.touchAndSave);
                    this.verbs.Add(this.displayBaseStyles);
                }

                return this.verbs;
            }
        }

        private void BroadcastComponentChanged()
        {
            IComponentChangeService changeService = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            if (changeService != null)
            {
                GridControlBase grid = this.Control as GridControlBase;
                changeService.OnComponentChanging(grid, null);
                changeService.OnComponentChanged(grid, null, null, null);
            }
        }

        #region verb handlers and helpers

        private void OnDisplayBaseStyles(object sender, EventArgs args)
        {
            GridControlBase grid = this.Control as GridControlBase;
            if (grid != null)
            {
                IDesignerHost host = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
                DialogResult result = GridBaseStylesMap.ShowGridBaseStylesMapDialog(grid.Model, "BaseStylesMap", host);
                grid.Invalidate();
                ////                _ShowGridBaseStylesMapDialog will do refresh broadcast Changed notification
                ////                Obsolete: --> grid.Model.Refresh();
                ////
                ////                if(result == DialogResult.OK)
                ////                    this.BroadcastComponentChanged();
            }
        }

        private void OnEditDesigner(object sender, EventArgs args)
        {
            GridControl grid = this.Control as GridControl;
            if (grid != null)
            {
                Syncfusion.Windows.Forms.Grid.Design.GridDesignerMain.Design(grid);
                grid.Invalidate();
                this.BroadcastComponentChanged();
            }
        }

        private void OnTouchAndSave(object sender, EventArgs args)
        {
            GridControlBase grid = this.Control as GridControlBase;
            if (grid != null)
            {
                IDesignerHost host = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
                this.BroadcastComponentChanged();
                grid.Invalidate();
            }
        }
        
        internal static void DisplayErrorMessage(string resName)
        {
            GridControlDesigner.DisplayMessage(resName, MessageBoxIcon.Error);
        }

        internal static void DisplayWarningMessage(string resName)
        {
            GridControlDesigner.DisplayMessage(resName, MessageBoxIcon.Warning);
        }

        internal static void DisplayMessage(string resName, MessageBoxIcon icon)
        {
            string caption = "GridControl"; ////SR.GetString(SR.GridControlName);
            string message = resName; ////SR.GetString(resName);
            MessageBox.Show(null, message, caption, MessageBoxButtons.OK, icon);
        }

        #endregion verb handlers and helpers
    }

    sealed class GridControlDesignerBinder : SerializationBinder
    {
        public override Type BindToType(
            string assemblyName, string typeName)
        {
            Type t = Type.GetType(typeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                return null;
            }
        }
    }
}
