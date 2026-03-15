#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Forms.Tools.Navigation.Design
{
    /// <summary>
    ///  Bar CollectionEditor Form
    /// </summary>
   public partial class BarCollectionEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                _provider = null;
                _navigationView = null;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BarCollectionEditorForm));
            this._tvBars = new System.Windows.Forms.TreeView();
            this._btnClose = new System.Windows.Forms.Button();
            this._lblTree = new System.Windows.Forms.Label();
            this._lblProperties = new System.Windows.Forms.Label();
            this._propertyGrid = new System.Windows.Forms.PropertyGrid();
            this._btnAddRoot = new System.Windows.Forms.Button();
            this._btnAddChild = new System.Windows.Forms.Button();
            this._btnUp = new System.Windows.Forms.Button();
            this._btnRemove = new System.Windows.Forms.Button();
            this._btnDown = new System.Windows.Forms.Button();
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this._btnSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _tvBars
            // 
            resources.ApplyResources(this._tvBars, "_tvBars");
            this._tvBars.HideSelection = false;
            this._tvBars.Name = "_tvBars";
            this._tvBars.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnNodeMouseDoubleClick);
            this._tvBars.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnAfterTreeNodeSelect);
            this._tvBars.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnBeforeTreeNodeSelect);
            // 
            // _btnClose
            // 
            resources.ApplyResources(this._btnClose, "_btnClose");
            this._btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnClose.Name = "_btnClose";
            this._btnClose.UseVisualStyleBackColor = true;
            // 
            // _lblTree
            // 
            resources.ApplyResources(this._lblTree, "_lblTree");
            this._lblTree.Name = "_lblTree";
            // 
            // _lblProperties
            // 
            resources.ApplyResources(this._lblProperties, "_lblProperties");
            this._lblProperties.Name = "_lblProperties";
            // 
            // _propertyGrid
            // 
            resources.ApplyResources(this._propertyGrid, "_propertyGrid");
            this._propertyGrid.Name = "_propertyGrid";
            // 
            // _btnAddRoot
            // 
            resources.ApplyResources(this._btnAddRoot, "_btnAddRoot");
            this._btnAddRoot.Name = "_btnAddRoot";
            this._btnAddRoot.UseVisualStyleBackColor = true;
            this._btnAddRoot.Click += new System.EventHandler(this.OnAddRootClick);
            // 
            // _btnAddChild
            // 
            resources.ApplyResources(this._btnAddChild, "_btnAddChild");
            this._btnAddChild.Name = "_btnAddChild";
            this._btnAddChild.UseVisualStyleBackColor = true;
            this._btnAddChild.Click += new System.EventHandler(this.OnAddChildClick);
            // 
            // _btnUp
            // 
            resources.ApplyResources(this._btnUp, "_btnUp");
            this._btnUp.Name = "_btnUp";
            this._btnUp.UseVisualStyleBackColor = true;
            this._btnUp.Click += new System.EventHandler(this.OnUpClick);
            // 
            // _btnRemove
            // 
            resources.ApplyResources(this._btnRemove, "_btnRemove");
            this._btnRemove.Name = "_btnRemove";
            this._btnRemove.UseVisualStyleBackColor = true;
            this._btnRemove.Click += new System.EventHandler(this.OnRemoveClick);
            // 
            // _btnDown
            // 
            resources.ApplyResources(this._btnDown, "_btnDown");
            this._btnDown.Name = "_btnDown";
            this._btnDown.UseVisualStyleBackColor = true;
            this._btnDown.Click += new System.EventHandler(this.OnDownClick);
            // 
            // _imageList
            // 
            this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this._imageList, "_imageList");
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // _btnSelect
            // 
            resources.ApplyResources(this._btnSelect, "_btnSelect");
            this._btnSelect.Name = "_btnSelect";
            this._btnSelect.UseVisualStyleBackColor = true;
            this._btnSelect.Click += new System.EventHandler(this.OnSelectClick);
            // 
            // BarCollectionEditorForm
            // 
            this.AcceptButton = this._btnClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._btnSelect);
            this.Controls.Add(this._btnRemove);
            this.Controls.Add(this._btnDown);
            this.Controls.Add(this._btnUp);
            this.Controls.Add(this._btnAddChild);
            this.Controls.Add(this._btnAddRoot);
            this.Controls.Add(this._propertyGrid);
            this.Controls.Add(this._lblProperties);
            this.Controls.Add(this._lblTree);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._tvBars);
            this.DoubleBuffered = true;
            this.MinimizeBox = false;
            this.Name = "BarCollectionEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _tvBars;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Label _lblTree;
        private System.Windows.Forms.Label _lblProperties;
        private System.Windows.Forms.PropertyGrid _propertyGrid;
        private System.Windows.Forms.Button _btnAddRoot;
        private System.Windows.Forms.Button _btnAddChild;
        private System.Windows.Forms.Button _btnUp;
        private System.Windows.Forms.Button _btnRemove;
        private System.Windows.Forms.Button _btnDown;
        private System.Windows.Forms.ImageList _imageList;
        private System.Windows.Forms.Button _btnSelect;
    }
}