//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelColStylesIndexerCollectionEditor.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    internal abstract class GridStylesUITypeEditor : UITypeEditor
    {
        internal abstract string Format(object o, int index);
        internal abstract void OnApplyChanges(object value);

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }

            return base.GetEditStyle(context);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                IWindowsFormsEditorService editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;

                if (editorService != null)
                {
                    DesignerTransaction trans = null;
                    if (designerHost != null)
                    {
                        trans = designerHost.CreateTransaction("Editing style collection");
                    }

                    GridCollectionEditor collectionEditor = new GridCollectionEditor(value as ICollection, this);
                    DialogResult result = editorService.ShowDialog(collectionEditor);

                    if (collectionEditor.Dirty)
                    {
                        IComponentChangeService changeService = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                        if (changeService != null)
                        {
                            changeService.OnComponentChanging(context.Instance, null);
                        }

                        if (changeService != null)
                        {
                            changeService.OnComponentChanged(context.Instance, null, null, null);
                        }
                    }

                    if (result == DialogResult.OK)
                    {
                        if (trans != null)
                        {
                            trans.Commit();
                        }

                        return value;
                    }

                    if (trans != null)
                    {
                        trans.Cancel();
                    }
                }
            }

            return base.EditValue(context, provider, value);
        }
    }
    
    internal class GridColStylesUITypeEditor : GridStylesUITypeEditor
    {
        internal override string Format(object o, int index)
        {
            return string.Format("Column {0}", index + 1);
        }

        internal override void OnApplyChanges(object value)
        {
            // no implementation
        }
    }

    internal class GridRowsStylesUITypeEditor : GridStylesUITypeEditor
    {
        internal override string Format(object o, int index)
        {
            return string.Format("Row {0}", index + 1);
        }

        internal override void OnApplyChanges(object value)
        {
            // no implementation    
        }
    }

    /// <internalonly/>
    internal class GridCollectionEditor : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

        private System.Windows.Forms.ListBox lstItems;
        private System.Windows.Forms.PropertyGrid pgEditor;
        private GridStylesUITypeEditor uiTypeEditor;
        private ICollection editCollection = null;
        private bool dirty = false;
        private PropertyGridContextMenu pgMenu = null;

        protected GridCollectionEditor()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        internal GridCollectionEditor(ICollection collection, GridStylesUITypeEditor uiTypeEditor)
        {
            this.editCollection = collection;
            this.uiTypeEditor = uiTypeEditor;

            this.InitializeComponent();

            this.pgMenu = new PropertyGridContextMenu(this.pgEditor);

            int i = 0;
            foreach (object o in this.editCollection)
            {
                string s = this.uiTypeEditor.Format(o, i);
                this.lstItems.Items.Add(new GridCollectionEditorNode(s, o));
                i++;
            }

            if (this.editCollection.Count > 0)
            {
                this.lstItems.SelectedIndex = 0;
                this.UpdateSelection();
            }

            this.HookListeners();
        }

        internal bool Dirty
        {
            get
            {
                return this.dirty;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.pgMenu != null)
                {
                    this.pgMenu.Dispose();
                    this.pgMenu = null;
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstItems = new System.Windows.Forms.ListBox();
            this.pgEditor = new System.Windows.Forms.PropertyGrid();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstItems
            // 
            this.lstItems.Location = new System.Drawing.Point(8, 8);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(176, 342);
            this.lstItems.TabIndex = 0;
            this.lstItems.SelectedIndexChanged += new System.EventHandler(this.lstItems_SelectedIndexChanged);
            // 
            // pgEditor
            // 
            this.pgEditor.CommandsVisibleIfAvailable = true;
            this.pgEditor.LargeButtons = false;
            this.pgEditor.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.pgEditor.Location = new System.Drawing.Point(200, 8);
            this.pgEditor.Name = "pgEditor";
            this.pgEditor.Size = new System.Drawing.Size(224, 304);
            this.pgEditor.TabIndex = 1;
            this.pgEditor.Text = "PropertyGrid";
            this.pgEditor.ViewBackColor = System.Drawing.SystemColors.Window;
            this.pgEditor.ViewForeColor = System.Drawing.SystemColors.WindowText;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(240, 328);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 24);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(336, 328);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            // 
            // GridCollectionEditor
            // 
#if !SyncfusionFramework2_0            
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
#endif
            this.ClientSize = new System.Drawing.Size(432, 358);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.btnCancel,
                                                                          this.btnOk,
                                                                          this.pgEditor,
                                                                          this.lstItems});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GridCollectionEditor";
            this.Text = "Collection Editor";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.GridCollectionEditor_Closing);
            this.ResumeLayout(false);

        }
        #endregion

        private void lstItems_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.UpdateSelection();
        }

        private void UpdateSelection()
        {
            GridCollectionEditorNode node = this.lstItems.SelectedItem as GridCollectionEditorNode;
            this.pgEditor.SelectedObject = node.Value;
        }

        private void OnStyleChanged(object sender, StyleChangedEventArgs args)
        {
            this.dirty = true;
        }

        private void HookListeners()
        {
            foreach (object o in this.editCollection)
            {
                GridStyleInfo styleInfo = o as GridStyleInfo;
                if (styleInfo != null)
                {
                    styleInfo.Changed += new StyleChangedEventHandler(this.OnStyleChanged);
                }
            }
        }

        private void UnHookListeners()
        {
            foreach (object o in this.editCollection)
            {
                GridStyleInfo styleInfo = o as GridStyleInfo;
                if (styleInfo != null)
                {
                    styleInfo.Changed -= new StyleChangedEventHandler(this.OnStyleChanged);
                }
            }
        }

        private void GridCollectionEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.UnHookListeners();
        }
    }

    internal class GridCollectionEditorNode
    {
        private string text;
        private object o;

        internal GridCollectionEditorNode(string text, object o)
        {
            this.text = text;
            this.o = o;
        }

        public override string ToString()
        {
            return this.text;
        }

        internal object Value
        {
            get
            {
                return this.o;
            }
        }
    }
}

