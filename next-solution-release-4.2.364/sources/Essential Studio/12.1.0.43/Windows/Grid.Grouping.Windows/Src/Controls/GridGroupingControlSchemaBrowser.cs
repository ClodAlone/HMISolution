//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupingControlSchemaBrowser.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using Syncfusion.Grouping;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Grid.Grouping;
using Syncfusion.Styles;
using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// A form that hosts a PropertyGrid and lets you browse schema information
    /// of a <see cref="GridGroupingControl"/>.
    /// </summary>
    /// <para/>
    /// <example>Displaying the GridGroupingControlSchemaBrowser
    /// <code lang="C#">
    ///     private void Form1_Load(object sender, System.EventArgs e)
    ///     {
    ///         GridGroupingControlSchemaBrowser frm = new GridGroupingControlSchemaBrowser(this.gridGoupingControl1);
    ///         frm.Show();
    ///     }
    /// </code>
    /// <code lang="VB">
    ///   Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
    ///         Dim frm As New GridGroupingControlSchemaBrowser(Me.gridGoupingControl1)
    ///         frm.Show()
    ///     End Sub 'Form1_Load
    /// </code>
    /// </example>
    public class GridGroupingControlSchemaBrowser : System.Windows.Forms.Form
    {
        internal System.Windows.Forms.PropertyGrid propertyGrid;
        private System.ComponentModel.Container components = null;
        private PropertyGridContextMenu pgMenu;
        private System.Windows.Forms.Button btnLoadXmlSchema;
        private System.Windows.Forms.Button btnSaveXmlSchema;

        GridGroupingControl grid;

        /// <summary>
        /// Initializes the browser and attaches it to a GridGroupingControl
        /// </summary>
        /// <param name="grid">The grid grouping control.</param>
        public GridGroupingControlSchemaBrowser(GridGroupingControl grid)
        {
            this.InitializeComponent();

            this.Text = grid.Name;
            this.grid = grid;
            this.grid.FilterRuntimeProperties = true;
            this.propertyGrid.SelectedObject = grid;

            if (this.propertyGrid != null)
            {
                this.pgMenu = new PropertyGridContextMenu(this.propertyGrid);
            }

            this.grid.PropertyChanged += new DescriptorPropertyChangedEventHandler(this.grid_PropertyChanged);
            this.grid.TableDescriptor.PropertyChanged += new DescriptorPropertyChangedEventHandler(this.TableDescriptor_PropertyChanged);
            this.grid.Engine.SourceListChanged += new EventHandler(this.Engine_SourceListChanged);
            Application.Idle += new EventHandler(this.Application_Idle);
        }

        bool allowAutoLocation = true;

        /// <summary>
        /// Enables or disables automatic positioning of schema browser next to the parent form
        /// of the grouping control when the schema browser is shown the first time.
        /// </summary>
        public bool AllowAutoLocation
        {
            get
            {
                return this.allowAutoLocation;
            }

            set
            {
                this.allowAutoLocation = value;
            }
        }
        /// <summary>
        /// Determine the location when visible changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.allowAutoLocation)
            {
                Form f = this.grid.FindForm();
                if (f != null)
                {
                    this.Location = new Point(f.Right, f.Top);
                }

                this.allowAutoLocation = false;
            }

            base.OnVisibleChanged(e);
        }

        bool needRefresh = false;

        private void grid_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            this.needRefresh = true;
        }

        private void TableDescriptor_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            this.needRefresh = true;
        }

        private void Engine_SourceListChanged(object sender, EventArgs e)
        {
            this.needRefresh = true;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (this.needRefresh && this.propertyGrid != null && this.propertyGrid.Parent != null)
            {
                this.needRefresh = false;
                this.propertyGrid.Refresh();
            }
        }

        /// <override/>
        protected override void OnClosed(EventArgs e)
        {
            Application.Idle -= new EventHandler(this.Application_Idle);
            base.OnClosed(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) 
        {
            if (disposing) 
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <override/>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.btnLoadXmlSchema = new System.Windows.Forms.Button();
            this.btnSaveXmlSchema = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // propertyGrid
            //
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.CommandsVisibleIfAvailable = true;
            this.propertyGrid.LargeButtons = false;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(416, 384);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.Text = "propertyGrid";
            this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
            this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
            //
            // btnLoadXmlSchema
            //
            this.btnLoadXmlSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadXmlSchema.Location = new System.Drawing.Point(216, 400);
            this.btnLoadXmlSchema.Name = "btnLoadXmlSchema";
            this.btnLoadXmlSchema.Size = new System.Drawing.Size(168, 24);
            this.btnLoadXmlSchema.TabIndex = 4;
            this.btnLoadXmlSchema.Text = "Load Xml Schema";
            this.btnLoadXmlSchema.Click += new System.EventHandler(this.btnLoadXmlSchema_Click);
            //
            // btnSaveXmlSchema
            //
            this.btnSaveXmlSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveXmlSchema.Location = new System.Drawing.Point(24, 400);
            this.btnSaveXmlSchema.Name = "btnSaveXmlSchema";
            this.btnSaveXmlSchema.Size = new System.Drawing.Size(168, 24);
            this.btnSaveXmlSchema.TabIndex = 3;
            this.btnSaveXmlSchema.Text = "Save Xml Schema";
            this.btnSaveXmlSchema.Click += new System.EventHandler(this.btnSaveXmlSchema_Click);
            //
            // GridGroupingControlSchemaBrowser
            //
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(416, 438);
            this.Controls.Add(this.btnLoadXmlSchema);
            this.Controls.Add(this.btnSaveXmlSchema);
            this.Controls.Add(this.propertyGrid);
            this.Name = "GridGroupingControlSchemaBrowser";
            this.Text = "GridGroupingControlSchemaBrowser";
            this.ResumeLayout(false);

        }
        #endregion

        private void btnSaveXmlSchema_Click(object sender, System.EventArgs e)
        {
            FileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                XmlTextWriter xw = new XmlTextWriter(dlg.FileName, System.Text.Encoding.UTF8);
                xw.Formatting = Formatting.Indented;
                this.grid.WriteXmlSchema(xw);
                xw.Close();
            }
        }

        private void btnLoadXmlSchema_Click(object sender, System.EventArgs e)
        {
            FileDialog dlg = new OpenFileDialog();
            dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                XmlReader xr = new XmlTextReader(dlg.FileName);
                this.ApplySchema(xr, Path.GetFileName(dlg.FileName), "Engine Schema (" + Path.GetFileName(dlg.FileName) + ")");
                xr.Close();
            }
        }

        void ApplySchema(XmlReader xr, string info, string undoDescription)
        {
            GridEngine engine = GridEngine.CreateFromXml(xr);
            if (engine != null)
            {
                this.grid.Engine.InitializeFrom(engine);
            }

            this.grid.Refresh();
        }
    }
}
