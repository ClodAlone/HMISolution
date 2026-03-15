#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// LayOutDialog form.
    /// </summary>
    public class LayoutDialog
        : Form
    {
        #region Constants
        private const int c_nCOLUMNS_MAX_DIGITS = 5;
        private const int c_nFLOAT_MAX_DIGITS = 10;
        private const int c_nNODE_SIZE = 45;
        private readonly char c_cDECIMAL_SEPARATOR = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
        private readonly string c_srtDECIMAL_SEPARATOR = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        #endregion

        #region Fields
        /// <summary>
        /// Indicates whether BackSpace or Delete key was pressed.
        /// </summary>
        private bool m_bBackDelete;

        /// <summary>
        /// Helper flag.
        /// </summary>
        private bool m_bLock;

        /// <summary>
        /// Updating document.
        /// </summary>
        private Model m_model;

        /// <summary>
        /// Updating document selection list.
        /// </summary>
        private NodeCollection m_lstSelection;

        /// <summary>
        /// RadioButtons with directions for GraphLayoutManagers RotationAngle.
        /// </summary>
        private RadioButton[] m_directions;

        private GraphLMSettings m_directedLMSettings;
        private GraphLMSettings m_radialLMSettings;
        private GraphLMSettings m_hierarchicLMSettings;
        private SymmetricLMSettings m_symmetricLMSettings;
        private TableLMSettings m_tableLMSettings;
        private Diagram m_diagram;
        #endregion

        #region Form controls
        private Button btnCancel;
        private Button btnOk;
        private Button btnApply;
        private GroupBox grpPlacement;
        private ComboBox comboStyle;
        private System.Windows.Forms.Label lblStylePlacement;
        private GroupBox grpSettings;
        private RadioButton rdbSelection;
        private RadioButton rdbCurrentPage;
        private GroupBox grpPreview;
        private CheckBox chkEnlargePage;
        private PictureBox pcPreview;
        private GroupBox grpTableLayout;
        private GroupBox grpTreeLayout;
        private GroupBox grpSymmetricLayout;
        private System.Windows.Forms.Label lblMaxColumnCount;
        private System.Windows.Forms.Label lblMaxRowsCount;
        private System.Windows.Forms.Label lblTableMaxWidth;
        private System.Windows.Forms.Label lblTableMaxHeight;
        private System.Windows.Forms.Label lblTableCellSizeMode;
		private System.Windows.Forms.Label lblTableTopMargin;
        private System.Windows.Forms.Label lblTableLeftMargin;
        private System.Windows.Forms.Label lblTableMeasureUnit;
        private ComboBox comboTableCellSizeMode;
		private ComboBox comboTableMeasureUnits;
        private System.Windows.Forms.Label lblTableExpandMode;
        private ComboBox comboTableExpandMode;
        private System.Windows.Forms.Label lblDirectedDirection;
        private System.Windows.Forms.Label lblForceSpringFactor;
        private System.Windows.Forms.Label lblMaxSpringLength;
        private System.Windows.Forms.Label lblMaxIteraction;
        private System.Windows.Forms.Label lblDirectedSpacing;
        private System.Windows.Forms.Label lblDirectedHorizontalSpacing;
        private TextBox txtDirectedHorizontalSpacing;
        private TextBox txtTableMaxWidth;
        private TextBox txtTableMaxHeight;
        private TextBox txtSymmetricForceSpringFactor;
        private TextBox txtSymmetricMaxSpringLength;
        private TextBox txtSymmetricMaxIteraction;
        private TextBox txtTableMaxColumnCount;
        private TextBox txtTableMaxRowsCount;
        private TextBox txtTableHorizontalSpacing;
        private TextBox txtTableVerticalSpacing;
		private TextBox txtTableLeftMargin;
        private TextBox txtTableTopMargin;
        private TextBox txtDirectedVerticalSpacing;
        private TextBox txtRotationAngle;
        private System.Windows.Forms.Label lblTableVerticalSpacing;
        private System.Windows.Forms.Label lblTableHorizontalSpacing;
        private RadioButton rbtnTopLeft;
        private RadioButton rbtnTop;
        private RadioButton rbtnTopRight;
        private RadioButton rbtnLeft;
        private RadioButton rbtnRight;
        private RadioButton rbtnBottomRight;
        private RadioButton rbtnBottom;
        private System.Windows.Forms.Label lblRotationAngle;
        private RadioButton rbtnBottomLeft;
        private System.Windows.Forms.Label lblDirectedVerticalSpacing;
        private IContainer components = null;
        #endregion

        #region Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutDialog"/> class.
        /// </summary>
        /// <param name="diagram">The diagram.</param>
        public LayoutDialog(Diagram diagram)
        {
            if (diagram == null)
                throw new ArgumentNullException("diagram");

            // Required for Windows Form Designer support
            InitializeComponent();

            m_model = diagram.Model;
            m_lstSelection = diagram.View.SelectionList;
            m_diagram = diagram;

            InitializeDialog();
            UpdateCurrentLayoutSettings();

            if (m_lstSelection.Count == 0)
            {
                rdbSelection.Enabled = false;
                rdbCurrentPage.Checked = true;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LayoutDialog));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.grpPlacement = new System.Windows.Forms.GroupBox();
            this.comboStyle = new System.Windows.Forms.ComboBox();
            this.lblStylePlacement = new System.Windows.Forms.Label();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.rdbSelection = new System.Windows.Forms.RadioButton();
            this.rdbCurrentPage = new System.Windows.Forms.RadioButton();
            this.grpPreview = new System.Windows.Forms.GroupBox();
            this.pcPreview = new System.Windows.Forms.PictureBox();
            this.chkEnlargePage = new System.Windows.Forms.CheckBox();
            this.grpTableLayout = new System.Windows.Forms.GroupBox();
            this.txtTableHorizontalSpacing = new System.Windows.Forms.TextBox();
            this.txtTableVerticalSpacing = new System.Windows.Forms.TextBox();
            this.lblTableVerticalSpacing = new System.Windows.Forms.Label();
            this.lblTableHorizontalSpacing = new System.Windows.Forms.Label();
            this.comboTableCellSizeMode = new System.Windows.Forms.ComboBox();
            this.txtTableMaxWidth = new System.Windows.Forms.TextBox();
            this.lblTableMaxWidth = new System.Windows.Forms.Label();
            this.lblTableMaxHeight = new System.Windows.Forms.Label();
			this.lblTableTopMargin = new System.Windows.Forms.Label();
            this.lblTableLeftMargin = new System.Windows.Forms.Label();
			this.lblTableMeasureUnit = new System.Windows.Forms.Label();
			this.txtTableTopMargin = new System.Windows.Forms.TextBox();
            this.txtTableLeftMargin = new System.Windows.Forms.TextBox();
            this.txtTableMaxHeight = new System.Windows.Forms.TextBox();
            this.lblMaxColumnCount = new System.Windows.Forms.Label();
            this.txtTableMaxColumnCount = new System.Windows.Forms.TextBox();
            this.lblMaxRowsCount = new System.Windows.Forms.Label();
            this.txtTableMaxRowsCount = new System.Windows.Forms.TextBox();
            this.lblTableCellSizeMode = new System.Windows.Forms.Label();
            this.lblTableExpandMode = new System.Windows.Forms.Label();
            this.comboTableExpandMode = new System.Windows.Forms.ComboBox();
			this.comboTableMeasureUnits = new System.Windows.Forms.ComboBox();
            this.grpTreeLayout = new System.Windows.Forms.GroupBox();
            this.rbtnTopLeft = new System.Windows.Forms.RadioButton();
            this.lblDirectedDirection = new System.Windows.Forms.Label();
            this.txtDirectedHorizontalSpacing = new System.Windows.Forms.TextBox();
            this.txtDirectedVerticalSpacing = new System.Windows.Forms.TextBox();
            this.lblDirectedSpacing = new System.Windows.Forms.Label();
            this.lblDirectedVerticalSpacing = new System.Windows.Forms.Label();
            this.lblDirectedHorizontalSpacing = new System.Windows.Forms.Label();
            this.rbtnTop = new System.Windows.Forms.RadioButton();
            this.rbtnTopRight = new System.Windows.Forms.RadioButton();
            this.rbtnLeft = new System.Windows.Forms.RadioButton();
            this.rbtnRight = new System.Windows.Forms.RadioButton();
            this.rbtnBottomRight = new System.Windows.Forms.RadioButton();
            this.rbtnBottomLeft = new System.Windows.Forms.RadioButton();
            this.rbtnBottom = new System.Windows.Forms.RadioButton();
            this.txtRotationAngle = new System.Windows.Forms.TextBox();
            this.lblRotationAngle = new System.Windows.Forms.Label();
            this.grpSymmetricLayout = new System.Windows.Forms.GroupBox();
            this.lblForceSpringFactor = new System.Windows.Forms.Label();
            this.txtSymmetricForceSpringFactor = new System.Windows.Forms.TextBox();
            this.lblMaxSpringLength = new System.Windows.Forms.Label();
            this.txtSymmetricMaxSpringLength = new System.Windows.Forms.TextBox();
            this.lblMaxIteraction = new System.Windows.Forms.Label();
            this.txtSymmetricMaxIteraction = new System.Windows.Forms.TextBox();
            this.grpPlacement.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.grpPreview.SuspendLayout();
            this.grpTableLayout.SuspendLayout();
            this.grpTreeLayout.SuspendLayout();
            this.grpSymmetricLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleDescription = resources.GetString("btnCancel.AccessibleDescription");
            this.btnCancel.AccessibleName = resources.GetString("btnCancel.AccessibleName");
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnCancel.Anchor")));
            this.btnCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.BackgroundImage")));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnCancel.Dock")));
            this.btnCancel.Enabled = ((bool)(resources.GetObject("btnCancel.Enabled")));
            this.btnCancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnCancel.FlatStyle")));
            this.btnCancel.Font = ((System.Drawing.Font)(resources.GetObject("btnCancel.Font")));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnCancel.ImageAlign")));
            this.btnCancel.ImageIndex = ((int)(resources.GetObject("btnCancel.ImageIndex")));
            this.btnCancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnCancel.ImeMode")));
            this.btnCancel.Location = ((System.Drawing.Point)(resources.GetObject("btnCancel.Location")));
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnCancel.RightToLeft")));
            this.btnCancel.Size = ((System.Drawing.Size)(resources.GetObject("btnCancel.Size")));
            this.btnCancel.TabIndex = ((int)(resources.GetObject("btnCancel.TabIndex")));
            this.btnCancel.Text = resources.GetString("btnCancel.Text");
            this.btnCancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnCancel.TextAlign")));
            this.btnCancel.Visible = ((bool)(resources.GetObject("btnCancel.Visible")));
            this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.AccessibleDescription = resources.GetString("btnOk.AccessibleDescription");
            this.btnOk.AccessibleName = resources.GetString("btnOk.AccessibleName");
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnOk.Anchor")));
            this.btnOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnOk.BackgroundImage")));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnOk.Dock")));
            this.btnOk.Enabled = ((bool)(resources.GetObject("btnOk.Enabled")));
            this.btnOk.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnOk.FlatStyle")));
            this.btnOk.Font = ((System.Drawing.Font)(resources.GetObject("btnOk.Font")));
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnOk.ImageAlign")));
            this.btnOk.ImageIndex = ((int)(resources.GetObject("btnOk.ImageIndex")));
            this.btnOk.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnOk.ImeMode")));
            this.btnOk.Location = ((System.Drawing.Point)(resources.GetObject("btnOk.Location")));
            this.btnOk.Name = "btnOk";
            this.btnOk.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnOk.RightToLeft")));
            this.btnOk.Size = ((System.Drawing.Size)(resources.GetObject("btnOk.Size")));
            this.btnOk.TabIndex = ((int)(resources.GetObject("btnOk.TabIndex")));
            this.btnOk.Text = resources.GetString("btnOk.Text");
            this.btnOk.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnOk.TextAlign")));
            this.btnOk.Visible = ((bool)(resources.GetObject("btnOk.Visible")));
            this.btnOk.Click += new System.EventHandler(this.Ok_Click);
            // 
            // btnApply
            // 
            this.btnApply.AccessibleDescription = resources.GetString("btnApply.AccessibleDescription");
            this.btnApply.AccessibleName = resources.GetString("btnApply.AccessibleName");
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnApply.Anchor")));
            this.btnApply.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnApply.BackgroundImage")));
            this.btnApply.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnApply.Dock")));
            this.btnApply.Enabled = ((bool)(resources.GetObject("btnApply.Enabled")));
            this.btnApply.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnApply.FlatStyle")));
            this.btnApply.Font = ((System.Drawing.Font)(resources.GetObject("btnApply.Font")));
            this.btnApply.Image = ((System.Drawing.Image)(resources.GetObject("btnApply.Image")));
            this.btnApply.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnApply.ImageAlign")));
            this.btnApply.ImageIndex = ((int)(resources.GetObject("btnApply.ImageIndex")));
            this.btnApply.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnApply.ImeMode")));
            this.btnApply.Location = ((System.Drawing.Point)(resources.GetObject("btnApply.Location")));
            this.btnApply.Name = "btnApply";
            this.btnApply.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnApply.RightToLeft")));
            this.btnApply.Size = ((System.Drawing.Size)(resources.GetObject("btnApply.Size")));
            this.btnApply.TabIndex = ((int)(resources.GetObject("btnApply.TabIndex")));
            this.btnApply.Text = resources.GetString("btnApply.Text");
            this.btnApply.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnApply.TextAlign")));
            this.btnApply.Visible = ((bool)(resources.GetObject("btnApply.Visible")));
            this.btnApply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // grpPlacement
            // 
            this.grpPlacement.AccessibleDescription = resources.GetString("grpPlacement.AccessibleDescription");
            this.grpPlacement.AccessibleName = resources.GetString("grpPlacement.AccessibleName");
            this.grpPlacement.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("grpPlacement.Anchor")));
            this.grpPlacement.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("grpPlacement.BackgroundImage")));
            this.grpPlacement.Controls.Add(this.comboStyle);
            this.grpPlacement.Controls.Add(this.lblStylePlacement);
            this.grpPlacement.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("grpPlacement.Dock")));
            this.grpPlacement.Enabled = ((bool)(resources.GetObject("grpPlacement.Enabled")));
            this.grpPlacement.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpPlacement.Font = ((System.Drawing.Font)(resources.GetObject("grpPlacement.Font")));
            this.grpPlacement.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("grpPlacement.ImeMode")));
            this.grpPlacement.Location = ((System.Drawing.Point)(resources.GetObject("grpPlacement.Location")));
            this.grpPlacement.Name = "grpPlacement";
            this.grpPlacement.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("grpPlacement.RightToLeft")));
            this.grpPlacement.Size = ((System.Drawing.Size)(resources.GetObject("grpPlacement.Size")));
            this.grpPlacement.TabIndex = ((int)(resources.GetObject("grpPlacement.TabIndex")));
            this.grpPlacement.TabStop = false;
            this.grpPlacement.Text = resources.GetString("grpPlacement.Text");
            this.grpPlacement.Visible = ((bool)(resources.GetObject("grpPlacement.Visible")));
            // 
            // comboStyle
            // 
            this.comboStyle.AccessibleDescription = resources.GetString("comboStyle.AccessibleDescription");
            this.comboStyle.AccessibleName = resources.GetString("comboStyle.AccessibleName");
            this.comboStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("comboStyle.Anchor")));
            this.comboStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("comboStyle.BackgroundImage")));
            this.comboStyle.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("comboStyle.Dock")));
            this.comboStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStyle.Enabled = ((bool)(resources.GetObject("comboStyle.Enabled")));
            this.comboStyle.Font = ((System.Drawing.Font)(resources.GetObject("comboStyle.Font")));
            this.comboStyle.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("comboStyle.ImeMode")));
            this.comboStyle.IntegralHeight = ((bool)(resources.GetObject("comboStyle.IntegralHeight")));
            this.comboStyle.ItemHeight = ((int)(resources.GetObject("comboStyle.ItemHeight")));
            this.comboStyle.Location = ((System.Drawing.Point)(resources.GetObject("comboStyle.Location")));
            this.comboStyle.MaxDropDownItems = ((int)(resources.GetObject("comboStyle.MaxDropDownItems")));
            this.comboStyle.MaxLength = ((int)(resources.GetObject("comboStyle.MaxLength")));
            this.comboStyle.Name = "comboStyle";
            this.comboStyle.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("comboStyle.RightToLeft")));
            this.comboStyle.Size = ((System.Drawing.Size)(resources.GetObject("comboStyle.Size")));
            this.comboStyle.TabIndex = ((int)(resources.GetObject("comboStyle.TabIndex")));
            this.comboStyle.Text = resources.GetString("comboStyle.Text");
            this.comboStyle.Visible = ((bool)(resources.GetObject("comboStyle.Visible")));
            this.comboStyle.SelectedIndexChanged += new System.EventHandler(this.ComboStyle_SelectedIndexChanged);
            // 
            // lblStylePlacement
            // 
            this.lblStylePlacement.AccessibleDescription = resources.GetString("lblStylePlacement.AccessibleDescription");
            this.lblStylePlacement.AccessibleName = resources.GetString("lblStylePlacement.AccessibleName");
            this.lblStylePlacement.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblStylePlacement.Anchor")));
            this.lblStylePlacement.AutoSize = ((bool)(resources.GetObject("lblStylePlacement.AutoSize")));
            this.lblStylePlacement.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblStylePlacement.Dock")));
            this.lblStylePlacement.Enabled = ((bool)(resources.GetObject("lblStylePlacement.Enabled")));
            this.lblStylePlacement.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblStylePlacement.Font = ((System.Drawing.Font)(resources.GetObject("lblStylePlacement.Font")));
            this.lblStylePlacement.Image = ((System.Drawing.Image)(resources.GetObject("lblStylePlacement.Image")));
            this.lblStylePlacement.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblStylePlacement.ImageAlign")));
            this.lblStylePlacement.ImageIndex = ((int)(resources.GetObject("lblStylePlacement.ImageIndex")));
            this.lblStylePlacement.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblStylePlacement.ImeMode")));
            this.lblStylePlacement.Location = ((System.Drawing.Point)(resources.GetObject("lblStylePlacement.Location")));
            this.lblStylePlacement.Name = "lblStylePlacement";
            this.lblStylePlacement.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblStylePlacement.RightToLeft")));
            this.lblStylePlacement.Size = ((System.Drawing.Size)(resources.GetObject("lblStylePlacement.Size")));
            this.lblStylePlacement.TabIndex = ((int)(resources.GetObject("lblStylePlacement.TabIndex")));
            this.lblStylePlacement.Text = resources.GetString("lblStylePlacement.Text");
            this.lblStylePlacement.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblStylePlacement.TextAlign")));
            this.lblStylePlacement.Visible = ((bool)(resources.GetObject("lblStylePlacement.Visible")));
            // 
            // grpSettings
            // 
            this.grpSettings.AccessibleDescription = resources.GetString("grpSettings.AccessibleDescription");
            this.grpSettings.AccessibleName = resources.GetString("grpSettings.AccessibleName");
            this.grpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("grpSettings.Anchor")));
            this.grpSettings.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("grpSettings.BackgroundImage")));
            this.grpSettings.Controls.Add(this.rdbSelection);
            this.grpSettings.Controls.Add(this.rdbCurrentPage);
            this.grpSettings.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("grpSettings.Dock")));
            this.grpSettings.Enabled = ((bool)(resources.GetObject("grpSettings.Enabled")));
            this.grpSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpSettings.Font = ((System.Drawing.Font)(resources.GetObject("grpSettings.Font")));
            this.grpSettings.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("grpSettings.ImeMode")));
            this.grpSettings.Location = ((System.Drawing.Point)(resources.GetObject("grpSettings.Location")));
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("grpSettings.RightToLeft")));
            this.grpSettings.Size = ((System.Drawing.Size)(resources.GetObject("grpSettings.Size")));
            this.grpSettings.TabIndex = ((int)(resources.GetObject("grpSettings.TabIndex")));
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = resources.GetString("grpSettings.Text");
            this.grpSettings.Visible = ((bool)(resources.GetObject("grpSettings.Visible")));
            // 
            // rdbSelection
            // 
            this.rdbSelection.AccessibleDescription = resources.GetString("rdbSelection.AccessibleDescription");
            this.rdbSelection.AccessibleName = resources.GetString("rdbSelection.AccessibleName");
            this.rdbSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rdbSelection.Anchor")));
            this.rdbSelection.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rdbSelection.Appearance")));
            this.rdbSelection.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rdbSelection.BackgroundImage")));
            this.rdbSelection.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbSelection.CheckAlign")));
            this.rdbSelection.Checked = true;
            this.rdbSelection.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rdbSelection.Dock")));
            this.rdbSelection.Enabled = ((bool)(resources.GetObject("rdbSelection.Enabled")));
            this.rdbSelection.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rdbSelection.FlatStyle")));
            this.rdbSelection.Font = ((System.Drawing.Font)(resources.GetObject("rdbSelection.Font")));
            this.rdbSelection.Image = ((System.Drawing.Image)(resources.GetObject("rdbSelection.Image")));
            this.rdbSelection.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbSelection.ImageAlign")));
            this.rdbSelection.ImageIndex = ((int)(resources.GetObject("rdbSelection.ImageIndex")));
            this.rdbSelection.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rdbSelection.ImeMode")));
            this.rdbSelection.Location = ((System.Drawing.Point)(resources.GetObject("rdbSelection.Location")));
            this.rdbSelection.Name = "rdbSelection";
            this.rdbSelection.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rdbSelection.RightToLeft")));
            this.rdbSelection.Size = ((System.Drawing.Size)(resources.GetObject("rdbSelection.Size")));
            this.rdbSelection.TabIndex = ((int)(resources.GetObject("rdbSelection.TabIndex")));
            this.rdbSelection.TabStop = true;
            this.rdbSelection.Text = resources.GetString("rdbSelection.Text");
            this.rdbSelection.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbSelection.TextAlign")));
            this.rdbSelection.Visible = ((bool)(resources.GetObject("rdbSelection.Visible")));
            this.rdbSelection.CheckedChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // rdbCurrentPage
            // 
            this.rdbCurrentPage.AccessibleDescription = resources.GetString("rdbCurrentPage.AccessibleDescription");
            this.rdbCurrentPage.AccessibleName = resources.GetString("rdbCurrentPage.AccessibleName");
            this.rdbCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rdbCurrentPage.Anchor")));
            this.rdbCurrentPage.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rdbCurrentPage.Appearance")));
            this.rdbCurrentPage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rdbCurrentPage.BackgroundImage")));
            this.rdbCurrentPage.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbCurrentPage.CheckAlign")));
            this.rdbCurrentPage.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rdbCurrentPage.Dock")));
            this.rdbCurrentPage.Enabled = ((bool)(resources.GetObject("rdbCurrentPage.Enabled")));
            this.rdbCurrentPage.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rdbCurrentPage.FlatStyle")));
            this.rdbCurrentPage.Font = ((System.Drawing.Font)(resources.GetObject("rdbCurrentPage.Font")));
            this.rdbCurrentPage.Image = ((System.Drawing.Image)(resources.GetObject("rdbCurrentPage.Image")));
            this.rdbCurrentPage.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbCurrentPage.ImageAlign")));
            this.rdbCurrentPage.ImageIndex = ((int)(resources.GetObject("rdbCurrentPage.ImageIndex")));
            this.rdbCurrentPage.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rdbCurrentPage.ImeMode")));
            this.rdbCurrentPage.Location = ((System.Drawing.Point)(resources.GetObject("rdbCurrentPage.Location")));
            this.rdbCurrentPage.Name = "rdbCurrentPage";
            this.rdbCurrentPage.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rdbCurrentPage.RightToLeft")));
            this.rdbCurrentPage.Size = ((System.Drawing.Size)(resources.GetObject("rdbCurrentPage.Size")));
            this.rdbCurrentPage.TabIndex = ((int)(resources.GetObject("rdbCurrentPage.TabIndex")));
            this.rdbCurrentPage.Text = resources.GetString("rdbCurrentPage.Text");
            this.rdbCurrentPage.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbCurrentPage.TextAlign")));
            this.rdbCurrentPage.Visible = ((bool)(resources.GetObject("rdbCurrentPage.Visible")));
            this.rdbCurrentPage.CheckedChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // grpPreview
            // 
            this.grpPreview.AccessibleDescription = resources.GetString("grpPreview.AccessibleDescription");
            this.grpPreview.AccessibleName = resources.GetString("grpPreview.AccessibleName");
            this.grpPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("grpPreview.Anchor")));
            this.grpPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("grpPreview.BackgroundImage")));
            this.grpPreview.Controls.Add(this.pcPreview);
            this.grpPreview.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("grpPreview.Dock")));
            this.grpPreview.Enabled = ((bool)(resources.GetObject("grpPreview.Enabled")));
            this.grpPreview.Font = ((System.Drawing.Font)(resources.GetObject("grpPreview.Font")));
            this.grpPreview.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("grpPreview.ImeMode")));
            this.grpPreview.Location = ((System.Drawing.Point)(resources.GetObject("grpPreview.Location")));
            this.grpPreview.Name = "grpPreview";
            this.grpPreview.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("grpPreview.RightToLeft")));
            this.grpPreview.Size = ((System.Drawing.Size)(resources.GetObject("grpPreview.Size")));
            this.grpPreview.TabIndex = ((int)(resources.GetObject("grpPreview.TabIndex")));
            this.grpPreview.TabStop = false;
            this.grpPreview.Text = resources.GetString("grpPreview.Text");
            this.grpPreview.Visible = ((bool)(resources.GetObject("grpPreview.Visible")));
            // 
            // pcPreview
            // 
            this.pcPreview.AccessibleDescription = resources.GetString("pcPreview.AccessibleDescription");
            this.pcPreview.AccessibleName = resources.GetString("pcPreview.AccessibleName");
            this.pcPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pcPreview.Anchor")));
            this.pcPreview.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pcPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pcPreview.BackgroundImage")));
            this.pcPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcPreview.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pcPreview.Dock")));
            this.pcPreview.Enabled = ((bool)(resources.GetObject("pcPreview.Enabled")));
            this.pcPreview.Font = ((System.Drawing.Font)(resources.GetObject("pcPreview.Font")));
            this.pcPreview.Image = ((System.Drawing.Image)(resources.GetObject("pcPreview.Image")));
            this.pcPreview.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pcPreview.ImeMode")));
            this.pcPreview.Location = ((System.Drawing.Point)(resources.GetObject("pcPreview.Location")));
            this.pcPreview.Name = "pcPreview";
            this.pcPreview.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pcPreview.RightToLeft")));
            this.pcPreview.Size = ((System.Drawing.Size)(resources.GetObject("pcPreview.Size")));
            this.pcPreview.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("pcPreview.SizeMode")));
            this.pcPreview.TabIndex = ((int)(resources.GetObject("pcPreview.TabIndex")));
            this.pcPreview.TabStop = false;
            this.pcPreview.Text = resources.GetString("pcPreview.Text");
            this.pcPreview.Visible = ((bool)(resources.GetObject("pcPreview.Visible")));
            // 
            // chkEnlargePage
            // 
            this.chkEnlargePage.AccessibleDescription = resources.GetString("chkEnlargePage.AccessibleDescription");
            this.chkEnlargePage.AccessibleName = resources.GetString("chkEnlargePage.AccessibleName");
            this.chkEnlargePage.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkEnlargePage.Anchor")));
            this.chkEnlargePage.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkEnlargePage.Appearance")));
            this.chkEnlargePage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkEnlargePage.BackgroundImage")));
            this.chkEnlargePage.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkEnlargePage.CheckAlign")));
            this.chkEnlargePage.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkEnlargePage.Dock")));
            this.chkEnlargePage.Enabled = ((bool)(resources.GetObject("chkEnlargePage.Enabled")));
            this.chkEnlargePage.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkEnlargePage.FlatStyle")));
            this.chkEnlargePage.Font = ((System.Drawing.Font)(resources.GetObject("chkEnlargePage.Font")));
            this.chkEnlargePage.Image = ((System.Drawing.Image)(resources.GetObject("chkEnlargePage.Image")));
            this.chkEnlargePage.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkEnlargePage.ImageAlign")));
            this.chkEnlargePage.ImageIndex = ((int)(resources.GetObject("chkEnlargePage.ImageIndex")));
            this.chkEnlargePage.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkEnlargePage.ImeMode")));
            this.chkEnlargePage.Location = ((System.Drawing.Point)(resources.GetObject("chkEnlargePage.Location")));
            this.chkEnlargePage.Name = "chkEnlargePage";
            this.chkEnlargePage.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkEnlargePage.RightToLeft")));
            this.chkEnlargePage.Size = ((System.Drawing.Size)(resources.GetObject("chkEnlargePage.Size")));
            this.chkEnlargePage.TabIndex = ((int)(resources.GetObject("chkEnlargePage.TabIndex")));
            this.chkEnlargePage.Text = resources.GetString("chkEnlargePage.Text");
            this.chkEnlargePage.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkEnlargePage.TextAlign")));
            this.chkEnlargePage.Visible = ((bool)(resources.GetObject("chkEnlargePage.Visible")));
            this.chkEnlargePage.CheckedChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // grpTableLayout
            // 
            this.grpTableLayout.AccessibleDescription = resources.GetString("grpTableLayout.AccessibleDescription");
            this.grpTableLayout.AccessibleName = resources.GetString("grpTableLayout.AccessibleName");
            this.grpTableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("grpTableLayout.Anchor")));
            this.grpTableLayout.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("grpTableLayout.BackgroundImage")));
			this.grpTableLayout.Controls.Add(this.comboTableMeasureUnits);
            this.grpTableLayout.Controls.Add(this.lblTableMeasureUnit);
            this.grpTableLayout.Controls.Add(this.lblTableTopMargin);
            this.grpTableLayout.Controls.Add(this.lblTableLeftMargin);
            this.grpTableLayout.Controls.Add(this.txtTableTopMargin);
            this.grpTableLayout.Controls.Add(this.txtTableLeftMargin);
            this.grpTableLayout.Controls.Add(this.txtTableHorizontalSpacing);
            this.grpTableLayout.Controls.Add(this.txtTableVerticalSpacing);
            this.grpTableLayout.Controls.Add(this.lblTableVerticalSpacing);
            this.grpTableLayout.Controls.Add(this.lblTableHorizontalSpacing);
            this.grpTableLayout.Controls.Add(this.comboTableCellSizeMode);
            this.grpTableLayout.Controls.Add(this.txtTableMaxWidth);
            this.grpTableLayout.Controls.Add(this.lblTableMaxWidth);
            this.grpTableLayout.Controls.Add(this.lblTableMaxHeight);
            this.grpTableLayout.Controls.Add(this.txtTableMaxHeight);
            this.grpTableLayout.Controls.Add(this.lblMaxColumnCount);
            this.grpTableLayout.Controls.Add(this.txtTableMaxColumnCount);
            this.grpTableLayout.Controls.Add(this.lblMaxRowsCount);
            this.grpTableLayout.Controls.Add(this.txtTableMaxRowsCount);
            this.grpTableLayout.Controls.Add(this.lblTableCellSizeMode);
            this.grpTableLayout.Controls.Add(this.lblTableExpandMode);
            this.grpTableLayout.Controls.Add(this.comboTableExpandMode);
            this.grpTableLayout.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("grpTableLayout.Dock")));
            this.grpTableLayout.Enabled = ((bool)(resources.GetObject("grpTableLayout.Enabled")));
            this.grpTableLayout.Font = ((System.Drawing.Font)(resources.GetObject("grpTableLayout.Font")));
            this.grpTableLayout.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("grpTableLayout.ImeMode")));
            this.grpTableLayout.Location = ((System.Drawing.Point)(resources.GetObject("grpTableLayout.Location")));
            this.grpTableLayout.Name = "grpTableLayout";
            this.grpTableLayout.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("grpTableLayout.RightToLeft")));
            this.grpTableLayout.Size = ((System.Drawing.Size)(resources.GetObject("grpTableLayout.Size")));
            this.grpTableLayout.TabIndex = ((int)(resources.GetObject("grpTableLayout.TabIndex")));
            this.grpTableLayout.TabStop = false;
            this.grpTableLayout.Text = resources.GetString("grpTableLayout.Text");
            this.grpTableLayout.Visible = ((bool)(resources.GetObject("grpTableLayout.Visible")));
			//
			//txtTab;eLeftMargin
			//
			this.txtTableLeftMargin.AccessibleDescription = resources.GetString("txtTableLeftMargin.AccessibleDescription");
            this.txtTableLeftMargin.AccessibleName = resources.GetString("txtTableLeftMargin.AccessibleName");
            this.txtTableLeftMargin.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTableLeftMargin.Anchor")));
            this.txtTableLeftMargin.AutoSize = ((bool)(resources.GetObject("txtTableLeftMargin.AutoSize")));
            this.txtTableLeftMargin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTableLeftMargin.BackgroundImage")));
            this.txtTableLeftMargin.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTableLeftMargin.Dock")));
            this.txtTableLeftMargin.Enabled = ((bool)(resources.GetObject("txtTableLeftMargin.Enabled")));
            this.txtTableLeftMargin.Font = ((System.Drawing.Font)(resources.GetObject("txtTableLeftMargin.Font")));
            this.txtTableLeftMargin.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTableLeftMargin.ImeMode")));
            this.txtTableLeftMargin.Location = ((System.Drawing.Point)(resources.GetObject("txtTableLeftMargin.Location")));
            this.txtTableLeftMargin.MaxLength = ((int)(resources.GetObject("txtTableLeftMargin.MaxLength")));
            this.txtTableLeftMargin.Multiline = ((bool)(resources.GetObject("txtTableLeftMargin.Multiline")));
            this.txtTableLeftMargin.Name = "txtTableLeftMargin";
            this.txtTableLeftMargin.PasswordChar = ((char)(resources.GetObject("txtTableLeftMargin.PasswordChar")));
            this.txtTableLeftMargin.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTableLeftMargin.RightToLeft")));
            this.txtTableLeftMargin.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTableLeftMargin.ScrollBars")));
            this.txtTableLeftMargin.Size = ((System.Drawing.Size)(resources.GetObject("txtTableLeftMargin.Size")));
            this.txtTableLeftMargin.TabIndex = ((int)(resources.GetObject("txtTableLeftMargin.TabIndex")));
            this.txtTableLeftMargin.Text = resources.GetString("txtTableLeftMargin.Text");
            this.txtTableLeftMargin.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTableLeftMargin.TextAlign")));
            this.txtTableLeftMargin.Visible = ((bool)(resources.GetObject("txtTableLeftMargin.Visible")));
            this.txtTableLeftMargin.WordWrap = ((bool)(resources.GetObject("txtTableLeftMargin.WordWrap")));
            this.txtTableLeftMargin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtTableLeftMargin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtTableLeftMargin.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
			//			
			//txtTableTopMargin
			//
			this.txtTableTopMargin.AccessibleDescription = resources.GetString("txtTableTopMargin.AccessibleDescription");
            this.txtTableTopMargin.AccessibleName = resources.GetString("txtTableTopMargin.AccessibleName");
            this.txtTableTopMargin.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTableTopMargin.Anchor")));
            this.txtTableTopMargin.AutoSize = ((bool)(resources.GetObject("txtTableTopMargin.AutoSize")));
            this.txtTableTopMargin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTableTopMargin.BackgroundImage")));
            this.txtTableTopMargin.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTableTopMargin.Dock")));
            this.txtTableTopMargin.Enabled = ((bool)(resources.GetObject("txtTableTopMargin.Enabled")));
            this.txtTableTopMargin.Font = ((System.Drawing.Font)(resources.GetObject("txtTableTopMargin.Font")));
            this.txtTableTopMargin.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTableTopMargin.ImeMode")));
            this.txtTableTopMargin.Location = ((System.Drawing.Point)(resources.GetObject("txtTableTopMargin.Location")));
            this.txtTableTopMargin.MaxLength = ((int)(resources.GetObject("txtTableTopMargin.MaxLength")));
            this.txtTableTopMargin.Multiline = ((bool)(resources.GetObject("txtTableTopMargin.Multiline")));
            this.txtTableTopMargin.Name = "txtTableTopMargin";
            this.txtTableTopMargin.PasswordChar = ((char)(resources.GetObject("txtTableTopMargin.PasswordChar")));
            this.txtTableTopMargin.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTableTopMargin.RightToLeft")));
            this.txtTableTopMargin.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTableTopMargin.ScrollBars")));
            this.txtTableTopMargin.Size = ((System.Drawing.Size)(resources.GetObject("txtTableTopMargin.Size")));
            this.txtTableTopMargin.TabIndex = ((int)(resources.GetObject("txtTableTopMargin.TabIndex")));
            this.txtTableTopMargin.Text = resources.GetString("txtTableTopMargin.Text");
            this.txtTableTopMargin.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTableTopMargin.TextAlign")));
            this.txtTableTopMargin.Visible = ((bool)(resources.GetObject("txtTableTopMargin.Visible")));
            this.txtTableTopMargin.WordWrap = ((bool)(resources.GetObject("txtTableTopMargin.WordWrap")));
            this.txtTableTopMargin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtTableTopMargin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtTableTopMargin.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // txtTableHorizontalSpacing
            // 
            this.txtTableHorizontalSpacing.AccessibleDescription = resources.GetString("txtTableHorizontalSpacing.AccessibleDescription");
            this.txtTableHorizontalSpacing.AccessibleName = resources.GetString("txtTableHorizontalSpacing.AccessibleName");
            this.txtTableHorizontalSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTableHorizontalSpacing.Anchor")));
            this.txtTableHorizontalSpacing.AutoSize = ((bool)(resources.GetObject("txtTableHorizontalSpacing.AutoSize")));
            this.txtTableHorizontalSpacing.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTableHorizontalSpacing.BackgroundImage")));
            this.txtTableHorizontalSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTableHorizontalSpacing.Dock")));
            this.txtTableHorizontalSpacing.Enabled = ((bool)(resources.GetObject("txtTableHorizontalSpacing.Enabled")));
            this.txtTableHorizontalSpacing.Font = ((System.Drawing.Font)(resources.GetObject("txtTableHorizontalSpacing.Font")));
            this.txtTableHorizontalSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTableHorizontalSpacing.ImeMode")));
            this.txtTableHorizontalSpacing.Location = ((System.Drawing.Point)(resources.GetObject("txtTableHorizontalSpacing.Location")));
            this.txtTableHorizontalSpacing.MaxLength = ((int)(resources.GetObject("txtTableHorizontalSpacing.MaxLength")));
            this.txtTableHorizontalSpacing.Multiline = ((bool)(resources.GetObject("txtTableHorizontalSpacing.Multiline")));
            this.txtTableHorizontalSpacing.Name = "txtTableHorizontalSpacing";
            this.txtTableHorizontalSpacing.PasswordChar = ((char)(resources.GetObject("txtTableHorizontalSpacing.PasswordChar")));
            this.txtTableHorizontalSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTableHorizontalSpacing.RightToLeft")));
            this.txtTableHorizontalSpacing.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTableHorizontalSpacing.ScrollBars")));
            this.txtTableHorizontalSpacing.Size = ((System.Drawing.Size)(resources.GetObject("txtTableHorizontalSpacing.Size")));
            this.txtTableHorizontalSpacing.TabIndex = ((int)(resources.GetObject("txtTableHorizontalSpacing.TabIndex")));
            this.txtTableHorizontalSpacing.Text = resources.GetString("txtTableHorizontalSpacing.Text");
            this.txtTableHorizontalSpacing.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTableHorizontalSpacing.TextAlign")));
            this.txtTableHorizontalSpacing.Visible = ((bool)(resources.GetObject("txtTableHorizontalSpacing.Visible")));
            this.txtTableHorizontalSpacing.WordWrap = ((bool)(resources.GetObject("txtTableHorizontalSpacing.WordWrap")));
            this.txtTableHorizontalSpacing.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtTableHorizontalSpacing.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtTableHorizontalSpacing.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // txtTableVerticalSpacing
            // 
            this.txtTableVerticalSpacing.AccessibleDescription = resources.GetString("txtTableVerticalSpacing.AccessibleDescription");
            this.txtTableVerticalSpacing.AccessibleName = resources.GetString("txtTableVerticalSpacing.AccessibleName");
            this.txtTableVerticalSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTableVerticalSpacing.Anchor")));
            this.txtTableVerticalSpacing.AutoSize = ((bool)(resources.GetObject("txtTableVerticalSpacing.AutoSize")));
            this.txtTableVerticalSpacing.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTableVerticalSpacing.BackgroundImage")));
            this.txtTableVerticalSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTableVerticalSpacing.Dock")));
            this.txtTableVerticalSpacing.Enabled = ((bool)(resources.GetObject("txtTableVerticalSpacing.Enabled")));
            this.txtTableVerticalSpacing.Font = ((System.Drawing.Font)(resources.GetObject("txtTableVerticalSpacing.Font")));
            this.txtTableVerticalSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTableVerticalSpacing.ImeMode")));
            this.txtTableVerticalSpacing.Location = ((System.Drawing.Point)(resources.GetObject("txtTableVerticalSpacing.Location")));
            this.txtTableVerticalSpacing.MaxLength = ((int)(resources.GetObject("txtTableVerticalSpacing.MaxLength")));
            this.txtTableVerticalSpacing.Multiline = ((bool)(resources.GetObject("txtTableVerticalSpacing.Multiline")));
            this.txtTableVerticalSpacing.Name = "txtTableVerticalSpacing";
            this.txtTableVerticalSpacing.PasswordChar = ((char)(resources.GetObject("txtTableVerticalSpacing.PasswordChar")));
            this.txtTableVerticalSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTableVerticalSpacing.RightToLeft")));
            this.txtTableVerticalSpacing.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTableVerticalSpacing.ScrollBars")));
            this.txtTableVerticalSpacing.Size = ((System.Drawing.Size)(resources.GetObject("txtTableVerticalSpacing.Size")));
            this.txtTableVerticalSpacing.TabIndex = ((int)(resources.GetObject("txtTableVerticalSpacing.TabIndex")));
            this.txtTableVerticalSpacing.Text = resources.GetString("txtTableVerticalSpacing.Text");
            this.txtTableVerticalSpacing.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTableVerticalSpacing.TextAlign")));
            this.txtTableVerticalSpacing.Visible = ((bool)(resources.GetObject("txtTableVerticalSpacing.Visible")));
            this.txtTableVerticalSpacing.WordWrap = ((bool)(resources.GetObject("txtTableVerticalSpacing.WordWrap")));
            this.txtTableVerticalSpacing.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtTableVerticalSpacing.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtTableVerticalSpacing.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
			//
			//lblTableLeftMargin	
			//
			this.lblTableLeftMargin.AccessibleDescription = resources.GetString("lblTableLeftMargin.AccessibleDescription");
            this.lblTableLeftMargin.AccessibleName = resources.GetString("lblTableLeftMargin.AccessibleName");
            this.lblTableLeftMargin.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableLeftMargin.Anchor")));
            this.lblTableLeftMargin.AutoSize = ((bool)(resources.GetObject("lblTableLeftMargin.AutoSize")));
            this.lblTableLeftMargin.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableLeftMargin.Dock")));
            this.lblTableLeftMargin.Enabled = ((bool)(resources.GetObject("lblTableLeftMargin.Enabled")));
            this.lblTableLeftMargin.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableLeftMargin.Font = ((System.Drawing.Font)(resources.GetObject("lblTableLeftMargin.Font")));
            this.lblTableLeftMargin.Image = ((System.Drawing.Image)(resources.GetObject("lblTableLeftMargin.Image")));
            this.lblTableLeftMargin.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableLeftMargin.ImageAlign")));
            this.lblTableLeftMargin.ImageIndex = ((int)(resources.GetObject("lblTableLeftMargin.ImageIndex")));
            this.lblTableLeftMargin.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableLeftMargin.ImeMode")));
            this.lblTableLeftMargin.Location = ((System.Drawing.Point)(resources.GetObject("lblTableLeftMargin.Location")));
            this.lblTableLeftMargin.Name = "lblTableLeftMargin";
            this.lblTableLeftMargin.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableLeftMargin.RightToLeft")));
            this.lblTableLeftMargin.Size = ((System.Drawing.Size)(resources.GetObject("lblTableLeftMargin.Size")));
            this.lblTableLeftMargin.TabIndex = ((int)(resources.GetObject("lblTableLeftMargin.TabIndex")));
            this.lblTableLeftMargin.Text = resources.GetString("lblTableLeftMargin.Text");
            this.lblTableLeftMargin.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableLeftMargin.TextAlign")));
            this.lblTableLeftMargin.Visible = ((bool)(resources.GetObject("lblTableLeftMargin.Visible")));						
			//
			//lblTableTopMargin
			//
			this.lblTableTopMargin.AccessibleDescription = resources.GetString("lblTableTopMargin.AccessibleDescription");
            this.lblTableTopMargin.AccessibleName = resources.GetString("lblTableTopMargin.AccessibleName");
            this.lblTableTopMargin.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableTopMargin.Anchor")));
            this.lblTableTopMargin.AutoSize = ((bool)(resources.GetObject("lblTableTopMargin.AutoSize")));
            this.lblTableTopMargin.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableTopMargin.Dock")));
            this.lblTableTopMargin.Enabled = ((bool)(resources.GetObject("lblTableTopMargin.Enabled")));
            this.lblTableTopMargin.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableTopMargin.Font = ((System.Drawing.Font)(resources.GetObject("lblTableTopMargin.Font")));
            this.lblTableTopMargin.Image = ((System.Drawing.Image)(resources.GetObject("lblTableTopMargin.Image")));
            this.lblTableTopMargin.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableTopMargin.ImageAlign")));
            this.lblTableTopMargin.ImageIndex = ((int)(resources.GetObject("lblTableTopMargin.ImageIndex")));
            this.lblTableTopMargin.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableTopMargin.ImeMode")));
            this.lblTableTopMargin.Location = ((System.Drawing.Point)(resources.GetObject("lblTableTopMargin.Location")));
            this.lblTableTopMargin.Name = "lblTableTopMargin";
            this.lblTableTopMargin.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableTopMargin.RightToLeft")));
            this.lblTableTopMargin.Size = ((System.Drawing.Size)(resources.GetObject("lblTableTopMargin.Size")));
            this.lblTableTopMargin.TabIndex = ((int)(resources.GetObject("lblTableTopMargin.TabIndex")));
            this.lblTableTopMargin.Text = resources.GetString("lblTableTopMargin.Text");
            this.lblTableTopMargin.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableTopMargin.TextAlign")));
            this.lblTableTopMargin.Visible = ((bool)(resources.GetObject("lblTableTopMargin.Visible")));
			//
			//lblTableMeasureUnit
			//
			this.lblTableMeasureUnit.AccessibleDescription = resources.GetString("lblTableMeasureUnit.AccessibleDescription");
            this.lblTableMeasureUnit.AccessibleName = resources.GetString("lblTableMeasureUnit.AccessibleName");
            this.lblTableMeasureUnit.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableMeasureUnit.Anchor")));
            this.lblTableMeasureUnit.AutoSize = ((bool)(resources.GetObject("lblTableMeasureUnit.AutoSize")));
            this.lblTableMeasureUnit.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableMeasureUnit.Dock")));
            this.lblTableMeasureUnit.Enabled = ((bool)(resources.GetObject("lblTableMeasureUnit.Enabled")));
            this.lblTableMeasureUnit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableMeasureUnit.Font = ((System.Drawing.Font)(resources.GetObject("lblTableMeasureUnit.Font")));
            this.lblTableMeasureUnit.Image = ((System.Drawing.Image)(resources.GetObject("lblTableMeasureUnit.Image")));
            this.lblTableMeasureUnit.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableMeasureUnit.ImageAlign")));
            this.lblTableMeasureUnit.ImageIndex = ((int)(resources.GetObject("lblTableMeasureUnit.ImageIndex")));
            this.lblTableMeasureUnit.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableMeasureUnit.ImeMode")));
            this.lblTableMeasureUnit.Location = ((System.Drawing.Point)(resources.GetObject("lblTableMeasureUnit.Location")));
            this.lblTableMeasureUnit.Name = "lblTableMeasureUnit";
            this.lblTableMeasureUnit.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableMeasureUnit.RightToLeft")));
            this.lblTableMeasureUnit.Size = ((System.Drawing.Size)(resources.GetObject("lblTableMeasureUnit.Size")));
            this.lblTableMeasureUnit.TabIndex = ((int)(resources.GetObject("lblTableMeasureUnit.TabIndex")));
            this.lblTableMeasureUnit.Text = resources.GetString("lblTableMeasureUnit.Text");
            this.lblTableMeasureUnit.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableMeasureUnit.TextAlign")));
            this.lblTableMeasureUnit.Visible = ((bool)(resources.GetObject("lblTableMeasureUnit.Visible")));
            // 
            // lblTableVerticalSpacing
            // 
            this.lblTableVerticalSpacing.AccessibleDescription = resources.GetString("lblTableVerticalSpacing.AccessibleDescription");
            this.lblTableVerticalSpacing.AccessibleName = resources.GetString("lblTableVerticalSpacing.AccessibleName");
            this.lblTableVerticalSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableVerticalSpacing.Anchor")));
            this.lblTableVerticalSpacing.AutoSize = ((bool)(resources.GetObject("lblTableVerticalSpacing.AutoSize")));
            this.lblTableVerticalSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableVerticalSpacing.Dock")));
            this.lblTableVerticalSpacing.Enabled = ((bool)(resources.GetObject("lblTableVerticalSpacing.Enabled")));
            this.lblTableVerticalSpacing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableVerticalSpacing.Font = ((System.Drawing.Font)(resources.GetObject("lblTableVerticalSpacing.Font")));
            this.lblTableVerticalSpacing.Image = ((System.Drawing.Image)(resources.GetObject("lblTableVerticalSpacing.Image")));
            this.lblTableVerticalSpacing.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableVerticalSpacing.ImageAlign")));
            this.lblTableVerticalSpacing.ImageIndex = ((int)(resources.GetObject("lblTableVerticalSpacing.ImageIndex")));
            this.lblTableVerticalSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableVerticalSpacing.ImeMode")));
            this.lblTableVerticalSpacing.Location = ((System.Drawing.Point)(resources.GetObject("lblTableVerticalSpacing.Location")));
            this.lblTableVerticalSpacing.Name = "lblTableVerticalSpacing";
            this.lblTableVerticalSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableVerticalSpacing.RightToLeft")));
            this.lblTableVerticalSpacing.Size = ((System.Drawing.Size)(resources.GetObject("lblTableVerticalSpacing.Size")));
            this.lblTableVerticalSpacing.TabIndex = ((int)(resources.GetObject("lblTableVerticalSpacing.TabIndex")));
            this.lblTableVerticalSpacing.Text = resources.GetString("lblTableVerticalSpacing.Text");
            this.lblTableVerticalSpacing.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableVerticalSpacing.TextAlign")));
            this.lblTableVerticalSpacing.Visible = ((bool)(resources.GetObject("lblTableVerticalSpacing.Visible")));
            // 
            // lblTableHorizontalSpacing
            // 
            this.lblTableHorizontalSpacing.AccessibleDescription = resources.GetString("lblTableHorizontalSpacing.AccessibleDescription");
            this.lblTableHorizontalSpacing.AccessibleName = resources.GetString("lblTableHorizontalSpacing.AccessibleName");
            this.lblTableHorizontalSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableHorizontalSpacing.Anchor")));
            this.lblTableHorizontalSpacing.AutoSize = ((bool)(resources.GetObject("lblTableHorizontalSpacing.AutoSize")));
            this.lblTableHorizontalSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableHorizontalSpacing.Dock")));
            this.lblTableHorizontalSpacing.Enabled = ((bool)(resources.GetObject("lblTableHorizontalSpacing.Enabled")));
            this.lblTableHorizontalSpacing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableHorizontalSpacing.Font = ((System.Drawing.Font)(resources.GetObject("lblTableHorizontalSpacing.Font")));
            this.lblTableHorizontalSpacing.Image = ((System.Drawing.Image)(resources.GetObject("lblTableHorizontalSpacing.Image")));
            this.lblTableHorizontalSpacing.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableHorizontalSpacing.ImageAlign")));
            this.lblTableHorizontalSpacing.ImageIndex = ((int)(resources.GetObject("lblTableHorizontalSpacing.ImageIndex")));
            this.lblTableHorizontalSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableHorizontalSpacing.ImeMode")));
            this.lblTableHorizontalSpacing.Location = ((System.Drawing.Point)(resources.GetObject("lblTableHorizontalSpacing.Location")));
            this.lblTableHorizontalSpacing.Name = "lblTableHorizontalSpacing";
            this.lblTableHorizontalSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableHorizontalSpacing.RightToLeft")));
            this.lblTableHorizontalSpacing.Size = ((System.Drawing.Size)(resources.GetObject("lblTableHorizontalSpacing.Size")));
            this.lblTableHorizontalSpacing.TabIndex = ((int)(resources.GetObject("lblTableHorizontalSpacing.TabIndex")));
            this.lblTableHorizontalSpacing.Text = resources.GetString("lblTableHorizontalSpacing.Text");
            this.lblTableHorizontalSpacing.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableHorizontalSpacing.TextAlign")));
            this.lblTableHorizontalSpacing.Visible = ((bool)(resources.GetObject("lblTableHorizontalSpacing.Visible")));
			// 
            // comboTableMeasureUnits
            // 
            this.comboTableMeasureUnits.AccessibleDescription = resources.GetString("comboTableMeasureUnits.AccessibleDescription");
            this.comboTableMeasureUnits.AccessibleName = resources.GetString("comboTableMeasureUnits.AccessibleName");
            this.comboTableMeasureUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("comboTableMeasureUnits.Anchor")));
            this.comboTableMeasureUnits.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("comboTableMeasureUnits.BackgroundImage")));
            this.comboTableMeasureUnits.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("comboTableMeasureUnits.Dock")));
            this.comboTableMeasureUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTableMeasureUnits.Enabled = ((bool)(resources.GetObject("comboTableMeasureUnits.Enabled")));
            this.comboTableMeasureUnits.Font = ((System.Drawing.Font)(resources.GetObject("comboTableMeasureUnits.Font")));
            this.comboTableMeasureUnits.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("comboTableMeasureUnits.ImeMode")));
            this.comboTableMeasureUnits.IntegralHeight = ((bool)(resources.GetObject("comboTableMeasureUnits.IntegralHeight")));
            this.comboTableMeasureUnits.ItemHeight = ((int)(resources.GetObject("comboTableMeasureUnits.ItemHeight")));
            this.comboTableMeasureUnits.Location = ((System.Drawing.Point)(resources.GetObject("comboTableMeasureUnits.Location")));
            this.comboTableMeasureUnits.MaxDropDownItems = ((int)(resources.GetObject("comboTableMeasureUnits.MaxDropDownItems")));
            this.comboTableMeasureUnits.MaxLength = ((int)(resources.GetObject("comboTableMeasureUnits.MaxLength")));
            this.comboTableMeasureUnits.Name = "comboTableMeasureUnits";
            this.comboTableMeasureUnits.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("comboTableMeasureUnits.RightToLeft")));
            this.comboTableMeasureUnits.Size = ((System.Drawing.Size)(resources.GetObject("comboTableMeasureUnits.Size")));
            this.comboTableMeasureUnits.TabIndex = ((int)(resources.GetObject("comboTableMeasureUnits.TabIndex")));
            this.comboTableMeasureUnits.Text = resources.GetString("comboTableMeasureUnits.Text");
            this.comboTableMeasureUnits.Visible = ((bool)(resources.GetObject("comboTableMeasureUnits.Visible")));
            this.comboTableMeasureUnits.SelectedIndexChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // comboTableCellSizeMode
            // 
            this.comboTableCellSizeMode.AccessibleDescription = resources.GetString("comboTableCellSizeMode.AccessibleDescription");
            this.comboTableCellSizeMode.AccessibleName = resources.GetString("comboTableCellSizeMode.AccessibleName");
            this.comboTableCellSizeMode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("comboTableCellSizeMode.Anchor")));
            this.comboTableCellSizeMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("comboTableCellSizeMode.BackgroundImage")));
            this.comboTableCellSizeMode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("comboTableCellSizeMode.Dock")));
            this.comboTableCellSizeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTableCellSizeMode.Enabled = ((bool)(resources.GetObject("comboTableCellSizeMode.Enabled")));
            this.comboTableCellSizeMode.Font = ((System.Drawing.Font)(resources.GetObject("comboTableCellSizeMode.Font")));
            this.comboTableCellSizeMode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("comboTableCellSizeMode.ImeMode")));
            this.comboTableCellSizeMode.IntegralHeight = ((bool)(resources.GetObject("comboTableCellSizeMode.IntegralHeight")));
            this.comboTableCellSizeMode.ItemHeight = ((int)(resources.GetObject("comboTableCellSizeMode.ItemHeight")));
            this.comboTableCellSizeMode.Location = ((System.Drawing.Point)(resources.GetObject("comboTableCellSizeMode.Location")));
            this.comboTableCellSizeMode.MaxDropDownItems = ((int)(resources.GetObject("comboTableCellSizeMode.MaxDropDownItems")));
            this.comboTableCellSizeMode.MaxLength = ((int)(resources.GetObject("comboTableCellSizeMode.MaxLength")));
            this.comboTableCellSizeMode.Name = "comboTableCellSizeMode";
            this.comboTableCellSizeMode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("comboTableCellSizeMode.RightToLeft")));
            this.comboTableCellSizeMode.Size = ((System.Drawing.Size)(resources.GetObject("comboTableCellSizeMode.Size")));
            this.comboTableCellSizeMode.TabIndex = ((int)(resources.GetObject("comboTableCellSizeMode.TabIndex")));
            this.comboTableCellSizeMode.Text = resources.GetString("comboTableCellSizeMode.Text");
            this.comboTableCellSizeMode.Visible = ((bool)(resources.GetObject("comboTableCellSizeMode.Visible")));
            this.comboTableCellSizeMode.SelectedIndexChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // txtTableMaxWidth
            // 
            this.txtTableMaxWidth.AccessibleDescription = resources.GetString("txtTableMaxWidth.AccessibleDescription");
            this.txtTableMaxWidth.AccessibleName = resources.GetString("txtTableMaxWidth.AccessibleName");
            this.txtTableMaxWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTableMaxWidth.Anchor")));
            this.txtTableMaxWidth.AutoSize = ((bool)(resources.GetObject("txtTableMaxWidth.AutoSize")));
            this.txtTableMaxWidth.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTableMaxWidth.BackgroundImage")));
            this.txtTableMaxWidth.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTableMaxWidth.Dock")));
            this.txtTableMaxWidth.Enabled = ((bool)(resources.GetObject("txtTableMaxWidth.Enabled")));
            this.txtTableMaxWidth.Font = ((System.Drawing.Font)(resources.GetObject("txtTableMaxWidth.Font")));
            this.txtTableMaxWidth.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTableMaxWidth.ImeMode")));
            this.txtTableMaxWidth.Location = ((System.Drawing.Point)(resources.GetObject("txtTableMaxWidth.Location")));
            this.txtTableMaxWidth.MaxLength = ((int)(resources.GetObject("txtTableMaxWidth.MaxLength")));
            this.txtTableMaxWidth.Multiline = ((bool)(resources.GetObject("txtTableMaxWidth.Multiline")));
            this.txtTableMaxWidth.Name = "txtTableMaxWidth";
            this.txtTableMaxWidth.PasswordChar = ((char)(resources.GetObject("txtTableMaxWidth.PasswordChar")));
            this.txtTableMaxWidth.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTableMaxWidth.RightToLeft")));
            this.txtTableMaxWidth.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTableMaxWidth.ScrollBars")));
            this.txtTableMaxWidth.Size = ((System.Drawing.Size)(resources.GetObject("txtTableMaxWidth.Size")));
            this.txtTableMaxWidth.TabIndex = ((int)(resources.GetObject("txtTableMaxWidth.TabIndex")));
            this.txtTableMaxWidth.Text = resources.GetString("txtTableMaxWidth.Text");
            this.txtTableMaxWidth.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTableMaxWidth.TextAlign")));
            this.txtTableMaxWidth.Visible = ((bool)(resources.GetObject("txtTableMaxWidth.Visible")));
            this.txtTableMaxWidth.WordWrap = ((bool)(resources.GetObject("txtTableMaxWidth.WordWrap")));
            this.txtTableMaxWidth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtTableMaxWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtTableMaxWidth.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // lblTableMaxWidth
            // 
            this.lblTableMaxWidth.AccessibleDescription = resources.GetString("lblTableMaxWidth.AccessibleDescription");
            this.lblTableMaxWidth.AccessibleName = resources.GetString("lblTableMaxWidth.AccessibleName");
            this.lblTableMaxWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableMaxWidth.Anchor")));
            this.lblTableMaxWidth.AutoSize = ((bool)(resources.GetObject("lblTableMaxWidth.AutoSize")));
            this.lblTableMaxWidth.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableMaxWidth.Dock")));
            this.lblTableMaxWidth.Enabled = ((bool)(resources.GetObject("lblTableMaxWidth.Enabled")));
            this.lblTableMaxWidth.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableMaxWidth.Font = ((System.Drawing.Font)(resources.GetObject("lblTableMaxWidth.Font")));
            this.lblTableMaxWidth.Image = ((System.Drawing.Image)(resources.GetObject("lblTableMaxWidth.Image")));
            this.lblTableMaxWidth.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableMaxWidth.ImageAlign")));
            this.lblTableMaxWidth.ImageIndex = ((int)(resources.GetObject("lblTableMaxWidth.ImageIndex")));
            this.lblTableMaxWidth.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableMaxWidth.ImeMode")));
            this.lblTableMaxWidth.Location = ((System.Drawing.Point)(resources.GetObject("lblTableMaxWidth.Location")));
            this.lblTableMaxWidth.Name = "lblTableMaxWidth";
            this.lblTableMaxWidth.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableMaxWidth.RightToLeft")));
            this.lblTableMaxWidth.Size = ((System.Drawing.Size)(resources.GetObject("lblTableMaxWidth.Size")));
            this.lblTableMaxWidth.TabIndex = ((int)(resources.GetObject("lblTableMaxWidth.TabIndex")));
            this.lblTableMaxWidth.Text = resources.GetString("lblTableMaxWidth.Text");
            this.lblTableMaxWidth.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableMaxWidth.TextAlign")));
            this.lblTableMaxWidth.Visible = ((bool)(resources.GetObject("lblTableMaxWidth.Visible")));
            // 
            // lblTableMaxHeight
            // 
            this.lblTableMaxHeight.AccessibleDescription = resources.GetString("lblTableMaxHeight.AccessibleDescription");
            this.lblTableMaxHeight.AccessibleName = resources.GetString("lblTableMaxHeight.AccessibleName");
            this.lblTableMaxHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableMaxHeight.Anchor")));
            this.lblTableMaxHeight.AutoSize = ((bool)(resources.GetObject("lblTableMaxHeight.AutoSize")));
            this.lblTableMaxHeight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableMaxHeight.Dock")));
            this.lblTableMaxHeight.Enabled = ((bool)(resources.GetObject("lblTableMaxHeight.Enabled")));
            this.lblTableMaxHeight.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableMaxHeight.Font = ((System.Drawing.Font)(resources.GetObject("lblTableMaxHeight.Font")));
            this.lblTableMaxHeight.Image = ((System.Drawing.Image)(resources.GetObject("lblTableMaxHeight.Image")));
            this.lblTableMaxHeight.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableMaxHeight.ImageAlign")));
            this.lblTableMaxHeight.ImageIndex = ((int)(resources.GetObject("lblTableMaxHeight.ImageIndex")));
            this.lblTableMaxHeight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableMaxHeight.ImeMode")));
            this.lblTableMaxHeight.Location = ((System.Drawing.Point)(resources.GetObject("lblTableMaxHeight.Location")));
            this.lblTableMaxHeight.Name = "lblTableMaxHeight";
            this.lblTableMaxHeight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableMaxHeight.RightToLeft")));
            this.lblTableMaxHeight.Size = ((System.Drawing.Size)(resources.GetObject("lblTableMaxHeight.Size")));
            this.lblTableMaxHeight.TabIndex = ((int)(resources.GetObject("lblTableMaxHeight.TabIndex")));
            this.lblTableMaxHeight.Text = resources.GetString("lblTableMaxHeight.Text");
            this.lblTableMaxHeight.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableMaxHeight.TextAlign")));
            this.lblTableMaxHeight.Visible = ((bool)(resources.GetObject("lblTableMaxHeight.Visible")));
            // 
            // txtTableMaxHeight
            // 
            this.txtTableMaxHeight.AccessibleDescription = resources.GetString("txtTableMaxHeight.AccessibleDescription");
            this.txtTableMaxHeight.AccessibleName = resources.GetString("txtTableMaxHeight.AccessibleName");
            this.txtTableMaxHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTableMaxHeight.Anchor")));
            this.txtTableMaxHeight.AutoSize = ((bool)(resources.GetObject("txtTableMaxHeight.AutoSize")));
            this.txtTableMaxHeight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTableMaxHeight.BackgroundImage")));
            this.txtTableMaxHeight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTableMaxHeight.Dock")));
            this.txtTableMaxHeight.Enabled = ((bool)(resources.GetObject("txtTableMaxHeight.Enabled")));
            this.txtTableMaxHeight.Font = ((System.Drawing.Font)(resources.GetObject("txtTableMaxHeight.Font")));
            this.txtTableMaxHeight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTableMaxHeight.ImeMode")));
            this.txtTableMaxHeight.Location = ((System.Drawing.Point)(resources.GetObject("txtTableMaxHeight.Location")));
            this.txtTableMaxHeight.MaxLength = ((int)(resources.GetObject("txtTableMaxHeight.MaxLength")));
            this.txtTableMaxHeight.Multiline = ((bool)(resources.GetObject("txtTableMaxHeight.Multiline")));
            this.txtTableMaxHeight.Name = "txtTableMaxHeight";
            this.txtTableMaxHeight.PasswordChar = ((char)(resources.GetObject("txtTableMaxHeight.PasswordChar")));
            this.txtTableMaxHeight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTableMaxHeight.RightToLeft")));
            this.txtTableMaxHeight.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTableMaxHeight.ScrollBars")));
            this.txtTableMaxHeight.Size = ((System.Drawing.Size)(resources.GetObject("txtTableMaxHeight.Size")));
            this.txtTableMaxHeight.TabIndex = ((int)(resources.GetObject("txtTableMaxHeight.TabIndex")));
            this.txtTableMaxHeight.Text = resources.GetString("txtTableMaxHeight.Text");
            this.txtTableMaxHeight.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTableMaxHeight.TextAlign")));
            this.txtTableMaxHeight.Visible = ((bool)(resources.GetObject("txtTableMaxHeight.Visible")));
            this.txtTableMaxHeight.WordWrap = ((bool)(resources.GetObject("txtTableMaxHeight.WordWrap")));
            this.txtTableMaxHeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtTableMaxHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtTableMaxHeight.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // lblMaxColumnCount
            // 
            this.lblMaxColumnCount.AccessibleDescription = resources.GetString("lblMaxColumnCount.AccessibleDescription");
            this.lblMaxColumnCount.AccessibleName = resources.GetString("lblMaxColumnCount.AccessibleName");
            this.lblMaxColumnCount.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblMaxColumnCount.Anchor")));
            this.lblMaxColumnCount.AutoSize = ((bool)(resources.GetObject("lblMaxColumnCount.AutoSize")));
            this.lblMaxColumnCount.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblMaxColumnCount.Dock")));
            this.lblMaxColumnCount.Enabled = ((bool)(resources.GetObject("lblMaxColumnCount.Enabled")));
            this.lblMaxColumnCount.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblMaxColumnCount.Font = ((System.Drawing.Font)(resources.GetObject("lblMaxColumnCount.Font")));
            this.lblMaxColumnCount.Image = ((System.Drawing.Image)(resources.GetObject("lblMaxColumnCount.Image")));
            this.lblMaxColumnCount.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblMaxColumnCount.ImageAlign")));
            this.lblMaxColumnCount.ImageIndex = ((int)(resources.GetObject("lblMaxColumnCount.ImageIndex")));
            this.lblMaxColumnCount.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblMaxColumnCount.ImeMode")));
            this.lblMaxColumnCount.Location = ((System.Drawing.Point)(resources.GetObject("lblMaxColumnCount.Location")));
            this.lblMaxColumnCount.Name = "lblMaxColumnCount";
            this.lblMaxColumnCount.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblMaxColumnCount.RightToLeft")));
            this.lblMaxColumnCount.Size = ((System.Drawing.Size)(resources.GetObject("lblMaxColumnCount.Size")));
            this.lblMaxColumnCount.TabIndex = ((int)(resources.GetObject("lblMaxColumnCount.TabIndex")));
            this.lblMaxColumnCount.Text = resources.GetString("lblMaxColumnCount.Text");
            this.lblMaxColumnCount.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblMaxColumnCount.TextAlign")));
            this.lblMaxColumnCount.Visible = ((bool)(resources.GetObject("lblMaxColumnCount.Visible")));
            this.lblMaxColumnCount.TextChanged += new System.EventHandler(this.TableColumnCount_TextChanged);
            // 
            // txtTableMaxColumnCount
            // 
            this.txtTableMaxColumnCount.AccessibleDescription = resources.GetString("txtTableMaxColumnCount.AccessibleDescription");
            this.txtTableMaxColumnCount.AccessibleName = resources.GetString("txtTableMaxColumnCount.AccessibleName");
            this.txtTableMaxColumnCount.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTableMaxColumnCount.Anchor")));
            this.txtTableMaxColumnCount.AutoSize = ((bool)(resources.GetObject("txtTableMaxColumnCount.AutoSize")));
            this.txtTableMaxColumnCount.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTableMaxColumnCount.BackgroundImage")));
            this.txtTableMaxColumnCount.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTableMaxColumnCount.Dock")));
            this.txtTableMaxColumnCount.Enabled = ((bool)(resources.GetObject("txtTableMaxColumnCount.Enabled")));
            this.txtTableMaxColumnCount.Font = ((System.Drawing.Font)(resources.GetObject("txtTableMaxColumnCount.Font")));
            this.txtTableMaxColumnCount.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTableMaxColumnCount.ImeMode")));
            this.txtTableMaxColumnCount.Location = ((System.Drawing.Point)(resources.GetObject("txtTableMaxColumnCount.Location")));
            this.txtTableMaxColumnCount.MaxLength = ((int)(resources.GetObject("txtTableMaxColumnCount.MaxLength")));
            this.txtTableMaxColumnCount.Multiline = ((bool)(resources.GetObject("txtTableMaxColumnCount.Multiline")));
            this.txtTableMaxColumnCount.Name = "txtTableMaxColumnCount";
            this.txtTableMaxColumnCount.PasswordChar = ((char)(resources.GetObject("txtTableMaxColumnCount.PasswordChar")));
            this.txtTableMaxColumnCount.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTableMaxColumnCount.RightToLeft")));
            this.txtTableMaxColumnCount.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTableMaxColumnCount.ScrollBars")));
            this.txtTableMaxColumnCount.Size = ((System.Drawing.Size)(resources.GetObject("txtTableMaxColumnCount.Size")));
            this.txtTableMaxColumnCount.TabIndex = ((int)(resources.GetObject("txtTableMaxColumnCount.TabIndex")));
            this.txtTableMaxColumnCount.Text = resources.GetString("txtTableMaxColumnCount.Text");
            this.txtTableMaxColumnCount.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTableMaxColumnCount.TextAlign")));
            this.txtTableMaxColumnCount.Visible = ((bool)(resources.GetObject("txtTableMaxColumnCount.Visible")));
            this.txtTableMaxColumnCount.WordWrap = ((bool)(resources.GetObject("txtTableMaxColumnCount.WordWrap")));
            this.txtTableMaxColumnCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtTableMaxColumnCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextColumnBoxKeyPress);
            this.txtTableMaxColumnCount.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // lblMaxRowsCount
            // 
            this.lblMaxRowsCount.AccessibleDescription = resources.GetString("lblMaxRowsCount.AccessibleDescription");
            this.lblMaxRowsCount.AccessibleName = resources.GetString("lblMaxRowsCount.AccessibleName");
            this.lblMaxRowsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblMaxRowsCount.Anchor")));
            this.lblMaxRowsCount.AutoSize = ((bool)(resources.GetObject("lblMaxRowsCount.AutoSize")));
            this.lblMaxRowsCount.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblMaxRowsCount.Dock")));
            this.lblMaxRowsCount.Enabled = ((bool)(resources.GetObject("lblMaxRowsCount.Enabled")));
            this.lblMaxRowsCount.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblMaxRowsCount.Font = ((System.Drawing.Font)(resources.GetObject("lblMaxRowsCount.Font")));
            this.lblMaxRowsCount.Image = ((System.Drawing.Image)(resources.GetObject("lblMaxRowsCount.Image")));
            this.lblMaxRowsCount.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblMaxRowsCount.ImageAlign")));
            this.lblMaxRowsCount.ImageIndex = ((int)(resources.GetObject("lblMaxRowsCount.ImageIndex")));
            this.lblMaxRowsCount.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblMaxRowsCount.ImeMode")));
            this.lblMaxRowsCount.Location = ((System.Drawing.Point)(resources.GetObject("lblMaxRowsCount.Location")));
            this.lblMaxRowsCount.Name = "lblMaxRowsCount";
            this.lblMaxRowsCount.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblMaxRowsCount.RightToLeft")));
            this.lblMaxRowsCount.Size = ((System.Drawing.Size)(resources.GetObject("lblMaxRowsCount.Size")));
            this.lblMaxRowsCount.TabIndex = ((int)(resources.GetObject("lblMaxRowsCount.TabIndex")));
            this.lblMaxRowsCount.Text = resources.GetString("lblMaxRowsCount.Text");
            this.lblMaxRowsCount.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblMaxRowsCount.TextAlign")));
            this.lblMaxRowsCount.Visible = ((bool)(resources.GetObject("lblMaxRowsCount.Visible")));
            this.lblMaxRowsCount.TextChanged += new System.EventHandler(this.TableColumnCount_TextChanged);
            // 
            // txtTableMaxRowsCount
            // 
            this.txtTableMaxRowsCount.AccessibleDescription = resources.GetString("txtTableMaxRowsCount.AccessibleDescription");
            this.txtTableMaxRowsCount.AccessibleName = resources.GetString("txtTableMaxRowsCount.AccessibleName");
            this.txtTableMaxRowsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTableMaxRowsCount.Anchor")));
            this.txtTableMaxRowsCount.AutoSize = ((bool)(resources.GetObject("txtTableMaxRowsCount.AutoSize")));
            this.txtTableMaxRowsCount.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTableMaxRowsCount.BackgroundImage")));
            this.txtTableMaxRowsCount.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTableMaxRowsCount.Dock")));
            this.txtTableMaxRowsCount.Enabled = ((bool)(resources.GetObject("txtTableMaxRowsCount.Enabled")));
            this.txtTableMaxRowsCount.Font = ((System.Drawing.Font)(resources.GetObject("txtTableMaxRowsCount.Font")));
            this.txtTableMaxRowsCount.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTableMaxRowsCount.ImeMode")));
            this.txtTableMaxRowsCount.Location = ((System.Drawing.Point)(resources.GetObject("txtTableMaxRowsCount.Location")));
            this.txtTableMaxRowsCount.MaxLength = ((int)(resources.GetObject("txtTableMaxRowsCount.MaxLength")));
            this.txtTableMaxRowsCount.Multiline = ((bool)(resources.GetObject("txtTableMaxRowsCount.Multiline")));
            this.txtTableMaxRowsCount.Name = "txtTableMaxRowsCount";
            this.txtTableMaxRowsCount.PasswordChar = ((char)(resources.GetObject("txtTableMaxRowsCount.PasswordChar")));
            this.txtTableMaxRowsCount.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTableMaxRowsCount.RightToLeft")));
            this.txtTableMaxRowsCount.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTableMaxRowsCount.ScrollBars")));
            this.txtTableMaxRowsCount.Size = ((System.Drawing.Size)(resources.GetObject("txtTableMaxRowsCount.Size")));
            this.txtTableMaxRowsCount.TabIndex = ((int)(resources.GetObject("txtTableMaxRowsCount.TabIndex")));
            this.txtTableMaxRowsCount.Text = resources.GetString("txtTableMaxRowsCount.Text");
            this.txtTableMaxRowsCount.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTableMaxRowsCount.TextAlign")));
            this.txtTableMaxRowsCount.Visible = ((bool)(resources.GetObject("txtTableMaxRowsCount.Visible")));
            this.txtTableMaxRowsCount.WordWrap = ((bool)(resources.GetObject("txtTableMaxRowsCount.WordWrap")));
            this.txtTableMaxRowsCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtTableMaxRowsCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextColumnBoxKeyPress);
            this.txtTableMaxRowsCount.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // lblTableCellSizeMode
            // 
            this.lblTableCellSizeMode.AccessibleDescription = resources.GetString("lblTableCellSizeMode.AccessibleDescription");
            this.lblTableCellSizeMode.AccessibleName = resources.GetString("lblTableCellSizeMode.AccessibleName");
            this.lblTableCellSizeMode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableCellSizeMode.Anchor")));
            this.lblTableCellSizeMode.AutoSize = ((bool)(resources.GetObject("lblTableCellSizeMode.AutoSize")));
            this.lblTableCellSizeMode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableCellSizeMode.Dock")));
            this.lblTableCellSizeMode.Enabled = ((bool)(resources.GetObject("lblTableCellSizeMode.Enabled")));
            this.lblTableCellSizeMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableCellSizeMode.Font = ((System.Drawing.Font)(resources.GetObject("lblTableCellSizeMode.Font")));
            this.lblTableCellSizeMode.Image = ((System.Drawing.Image)(resources.GetObject("lblTableCellSizeMode.Image")));
            this.lblTableCellSizeMode.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableCellSizeMode.ImageAlign")));
            this.lblTableCellSizeMode.ImageIndex = ((int)(resources.GetObject("lblTableCellSizeMode.ImageIndex")));
            this.lblTableCellSizeMode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableCellSizeMode.ImeMode")));
            this.lblTableCellSizeMode.Location = ((System.Drawing.Point)(resources.GetObject("lblTableCellSizeMode.Location")));
            this.lblTableCellSizeMode.Name = "lblTableCellSizeMode";
            this.lblTableCellSizeMode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableCellSizeMode.RightToLeft")));
            this.lblTableCellSizeMode.Size = ((System.Drawing.Size)(resources.GetObject("lblTableCellSizeMode.Size")));
            this.lblTableCellSizeMode.TabIndex = ((int)(resources.GetObject("lblTableCellSizeMode.TabIndex")));
            this.lblTableCellSizeMode.Text = resources.GetString("lblTableCellSizeMode.Text");
            this.lblTableCellSizeMode.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableCellSizeMode.TextAlign")));
            this.lblTableCellSizeMode.Visible = ((bool)(resources.GetObject("lblTableCellSizeMode.Visible")));
            // 
            // lblTableExpandMode
            // 
            this.lblTableExpandMode.AccessibleDescription = resources.GetString("lblTableExpandMode.AccessibleDescription");
            this.lblTableExpandMode.AccessibleName = resources.GetString("lblTableExpandMode.AccessibleName");
            this.lblTableExpandMode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTableExpandMode.Anchor")));
            this.lblTableExpandMode.AutoSize = ((bool)(resources.GetObject("lblTableExpandMode.AutoSize")));
            this.lblTableExpandMode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTableExpandMode.Dock")));
            this.lblTableExpandMode.Enabled = ((bool)(resources.GetObject("lblTableExpandMode.Enabled")));
            this.lblTableExpandMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTableExpandMode.Font = ((System.Drawing.Font)(resources.GetObject("lblTableExpandMode.Font")));
            this.lblTableExpandMode.Image = ((System.Drawing.Image)(resources.GetObject("lblTableExpandMode.Image")));
            this.lblTableExpandMode.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableExpandMode.ImageAlign")));
            this.lblTableExpandMode.ImageIndex = ((int)(resources.GetObject("lblTableExpandMode.ImageIndex")));
            this.lblTableExpandMode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTableExpandMode.ImeMode")));
            this.lblTableExpandMode.Location = ((System.Drawing.Point)(resources.GetObject("lblTableExpandMode.Location")));
            this.lblTableExpandMode.Name = "lblTableExpandMode";
            this.lblTableExpandMode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTableExpandMode.RightToLeft")));
            this.lblTableExpandMode.Size = ((System.Drawing.Size)(resources.GetObject("lblTableExpandMode.Size")));
            this.lblTableExpandMode.TabIndex = ((int)(resources.GetObject("lblTableExpandMode.TabIndex")));
            this.lblTableExpandMode.Text = resources.GetString("lblTableExpandMode.Text");
            this.lblTableExpandMode.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTableExpandMode.TextAlign")));
            this.lblTableExpandMode.Visible = ((bool)(resources.GetObject("lblTableExpandMode.Visible")));
            // 
            // comboTableExpandMode
            // 
            this.comboTableExpandMode.AccessibleDescription = resources.GetString("comboTableExpandMode.AccessibleDescription");
            this.comboTableExpandMode.AccessibleName = resources.GetString("comboTableExpandMode.AccessibleName");
            this.comboTableExpandMode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("comboTableExpandMode.Anchor")));
            this.comboTableExpandMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("comboTableExpandMode.BackgroundImage")));
            this.comboTableExpandMode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("comboTableExpandMode.Dock")));
            this.comboTableExpandMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTableExpandMode.Enabled = ((bool)(resources.GetObject("comboTableExpandMode.Enabled")));
            this.comboTableExpandMode.Font = ((System.Drawing.Font)(resources.GetObject("comboTableExpandMode.Font")));
            this.comboTableExpandMode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("comboTableExpandMode.ImeMode")));
            this.comboTableExpandMode.IntegralHeight = ((bool)(resources.GetObject("comboTableExpandMode.IntegralHeight")));
            this.comboTableExpandMode.ItemHeight = ((int)(resources.GetObject("comboTableExpandMode.ItemHeight")));
            this.comboTableExpandMode.Location = ((System.Drawing.Point)(resources.GetObject("comboTableExpandMode.Location")));
            this.comboTableExpandMode.MaxDropDownItems = ((int)(resources.GetObject("comboTableExpandMode.MaxDropDownItems")));
            this.comboTableExpandMode.MaxLength = ((int)(resources.GetObject("comboTableExpandMode.MaxLength")));
            this.comboTableExpandMode.Name = "comboTableExpandMode";
            this.comboTableExpandMode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("comboTableExpandMode.RightToLeft")));
            this.comboTableExpandMode.Size = ((System.Drawing.Size)(resources.GetObject("comboTableExpandMode.Size")));
            this.comboTableExpandMode.TabIndex = ((int)(resources.GetObject("comboTableExpandMode.TabIndex")));
            this.comboTableExpandMode.Text = resources.GetString("comboTableExpandMode.Text");
            this.comboTableExpandMode.Visible = ((bool)(resources.GetObject("comboTableExpandMode.Visible")));
            this.comboTableExpandMode.SelectedIndexChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // grpTreeLayout
            // 
            this.grpTreeLayout.AccessibleDescription = resources.GetString("grpTreeLayout.AccessibleDescription");
            this.grpTreeLayout.AccessibleName = resources.GetString("grpTreeLayout.AccessibleName");
            this.grpTreeLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("grpTreeLayout.Anchor")));
            this.grpTreeLayout.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("grpTreeLayout.BackgroundImage")));
            this.grpTreeLayout.Controls.Add(this.rbtnTopLeft);
            this.grpTreeLayout.Controls.Add(this.lblDirectedDirection);
            this.grpTreeLayout.Controls.Add(this.txtDirectedHorizontalSpacing);
            this.grpTreeLayout.Controls.Add(this.txtDirectedVerticalSpacing);
            this.grpTreeLayout.Controls.Add(this.lblDirectedSpacing);
            this.grpTreeLayout.Controls.Add(this.lblDirectedVerticalSpacing);
            this.grpTreeLayout.Controls.Add(this.lblDirectedHorizontalSpacing);
            this.grpTreeLayout.Controls.Add(this.rbtnTop);
            this.grpTreeLayout.Controls.Add(this.rbtnTopRight);
            this.grpTreeLayout.Controls.Add(this.rbtnLeft);
            this.grpTreeLayout.Controls.Add(this.rbtnRight);
            this.grpTreeLayout.Controls.Add(this.rbtnBottomRight);
            this.grpTreeLayout.Controls.Add(this.rbtnBottomLeft);
            this.grpTreeLayout.Controls.Add(this.rbtnBottom);
            this.grpTreeLayout.Controls.Add(this.txtRotationAngle);
            this.grpTreeLayout.Controls.Add(this.lblRotationAngle);
            this.grpTreeLayout.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("grpTreeLayout.Dock")));
            this.grpTreeLayout.Enabled = ((bool)(resources.GetObject("grpTreeLayout.Enabled")));
            this.grpTreeLayout.Font = ((System.Drawing.Font)(resources.GetObject("grpTreeLayout.Font")));
            this.grpTreeLayout.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("grpTreeLayout.ImeMode")));
            this.grpTreeLayout.Location = ((System.Drawing.Point)(resources.GetObject("grpTreeLayout.Location")));
            this.grpTreeLayout.Name = "grpTreeLayout";
            this.grpTreeLayout.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("grpTreeLayout.RightToLeft")));
            this.grpTreeLayout.Size = ((System.Drawing.Size)(resources.GetObject("grpTreeLayout.Size")));
            this.grpTreeLayout.TabIndex = ((int)(resources.GetObject("grpTreeLayout.TabIndex")));
            this.grpTreeLayout.TabStop = false;
            this.grpTreeLayout.Text = resources.GetString("grpTreeLayout.Text");
            this.grpTreeLayout.Visible = ((bool)(resources.GetObject("grpTreeLayout.Visible")));
            // 
            // rbtnTopLeft
            // 
            this.rbtnTopLeft.AccessibleDescription = resources.GetString("rbtnTopLeft.AccessibleDescription");
            this.rbtnTopLeft.AccessibleName = resources.GetString("rbtnTopLeft.AccessibleName");
            this.rbtnTopLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rbtnTopLeft.Anchor")));
            this.rbtnTopLeft.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rbtnTopLeft.Appearance")));
            this.rbtnTopLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbtnTopLeft.BackgroundImage")));
            this.rbtnTopLeft.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTopLeft.CheckAlign")));
            this.rbtnTopLeft.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rbtnTopLeft.Dock")));
            this.rbtnTopLeft.Enabled = ((bool)(resources.GetObject("rbtnTopLeft.Enabled")));
            this.rbtnTopLeft.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rbtnTopLeft.FlatStyle")));
            this.rbtnTopLeft.Font = ((System.Drawing.Font)(resources.GetObject("rbtnTopLeft.Font")));
            this.rbtnTopLeft.Image = ((System.Drawing.Image)(resources.GetObject("rbtnTopLeft.Image")));
            this.rbtnTopLeft.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTopLeft.ImageAlign")));
            this.rbtnTopLeft.ImageIndex = ((int)(resources.GetObject("rbtnTopLeft.ImageIndex")));
            this.rbtnTopLeft.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rbtnTopLeft.ImeMode")));
            this.rbtnTopLeft.Location = ((System.Drawing.Point)(resources.GetObject("rbtnTopLeft.Location")));
            this.rbtnTopLeft.Name = "rbtnTopLeft";
            this.rbtnTopLeft.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rbtnTopLeft.RightToLeft")));
            this.rbtnTopLeft.Size = ((System.Drawing.Size)(resources.GetObject("rbtnTopLeft.Size")));
            this.rbtnTopLeft.TabIndex = ((int)(resources.GetObject("rbtnTopLeft.TabIndex")));
            this.rbtnTopLeft.Text = resources.GetString("rbtnTopLeft.Text");
            this.rbtnTopLeft.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTopLeft.TextAlign")));
            this.rbtnTopLeft.Visible = ((bool)(resources.GetObject("rbtnTopLeft.Visible")));
            this.rbtnTopLeft.CheckedChanged += new System.EventHandler(this.DirectionChange);
            // 
            // lblDirectedDirection
            // 
            this.lblDirectedDirection.AccessibleDescription = resources.GetString("lblDirectedDirection.AccessibleDescription");
            this.lblDirectedDirection.AccessibleName = resources.GetString("lblDirectedDirection.AccessibleName");
            this.lblDirectedDirection.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblDirectedDirection.Anchor")));
            this.lblDirectedDirection.AutoSize = ((bool)(resources.GetObject("lblDirectedDirection.AutoSize")));
            this.lblDirectedDirection.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblDirectedDirection.Dock")));
            this.lblDirectedDirection.Enabled = ((bool)(resources.GetObject("lblDirectedDirection.Enabled")));
            this.lblDirectedDirection.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDirectedDirection.Font = ((System.Drawing.Font)(resources.GetObject("lblDirectedDirection.Font")));
            this.lblDirectedDirection.Image = ((System.Drawing.Image)(resources.GetObject("lblDirectedDirection.Image")));
            this.lblDirectedDirection.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDirectedDirection.ImageAlign")));
            this.lblDirectedDirection.ImageIndex = ((int)(resources.GetObject("lblDirectedDirection.ImageIndex")));
            this.lblDirectedDirection.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblDirectedDirection.ImeMode")));
            this.lblDirectedDirection.Location = ((System.Drawing.Point)(resources.GetObject("lblDirectedDirection.Location")));
            this.lblDirectedDirection.Name = "lblDirectedDirection";
            this.lblDirectedDirection.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblDirectedDirection.RightToLeft")));
            this.lblDirectedDirection.Size = ((System.Drawing.Size)(resources.GetObject("lblDirectedDirection.Size")));
            this.lblDirectedDirection.TabIndex = ((int)(resources.GetObject("lblDirectedDirection.TabIndex")));
            this.lblDirectedDirection.Text = resources.GetString("lblDirectedDirection.Text");
            this.lblDirectedDirection.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDirectedDirection.TextAlign")));
            this.lblDirectedDirection.Visible = ((bool)(resources.GetObject("lblDirectedDirection.Visible")));
            // 
            // txtDirectedHorizontalSpacing
            // 
            this.txtDirectedHorizontalSpacing.AccessibleDescription = resources.GetString("txtDirectedHorizontalSpacing.AccessibleDescription");
            this.txtDirectedHorizontalSpacing.AccessibleName = resources.GetString("txtDirectedHorizontalSpacing.AccessibleName");
            this.txtDirectedHorizontalSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtDirectedHorizontalSpacing.Anchor")));
            this.txtDirectedHorizontalSpacing.AutoSize = ((bool)(resources.GetObject("txtDirectedHorizontalSpacing.AutoSize")));
            this.txtDirectedHorizontalSpacing.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtDirectedHorizontalSpacing.BackgroundImage")));
            this.txtDirectedHorizontalSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtDirectedHorizontalSpacing.Dock")));
            this.txtDirectedHorizontalSpacing.Enabled = ((bool)(resources.GetObject("txtDirectedHorizontalSpacing.Enabled")));
            this.txtDirectedHorizontalSpacing.Font = ((System.Drawing.Font)(resources.GetObject("txtDirectedHorizontalSpacing.Font")));
            this.txtDirectedHorizontalSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtDirectedHorizontalSpacing.ImeMode")));
            this.txtDirectedHorizontalSpacing.Location = ((System.Drawing.Point)(resources.GetObject("txtDirectedHorizontalSpacing.Location")));
            this.txtDirectedHorizontalSpacing.MaxLength = ((int)(resources.GetObject("txtDirectedHorizontalSpacing.MaxLength")));
            this.txtDirectedHorizontalSpacing.Multiline = ((bool)(resources.GetObject("txtDirectedHorizontalSpacing.Multiline")));
            this.txtDirectedHorizontalSpacing.Name = "txtDirectedHorizontalSpacing";
            this.txtDirectedHorizontalSpacing.PasswordChar = ((char)(resources.GetObject("txtDirectedHorizontalSpacing.PasswordChar")));
            this.txtDirectedHorizontalSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtDirectedHorizontalSpacing.RightToLeft")));
            this.txtDirectedHorizontalSpacing.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtDirectedHorizontalSpacing.ScrollBars")));
            this.txtDirectedHorizontalSpacing.Size = ((System.Drawing.Size)(resources.GetObject("txtDirectedHorizontalSpacing.Size")));
            this.txtDirectedHorizontalSpacing.TabIndex = ((int)(resources.GetObject("txtDirectedHorizontalSpacing.TabIndex")));
            this.txtDirectedHorizontalSpacing.Text = resources.GetString("txtDirectedHorizontalSpacing.Text");
            this.txtDirectedHorizontalSpacing.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtDirectedHorizontalSpacing.TextAlign")));
            this.txtDirectedHorizontalSpacing.Visible = ((bool)(resources.GetObject("txtDirectedHorizontalSpacing.Visible")));
            this.txtDirectedHorizontalSpacing.WordWrap = ((bool)(resources.GetObject("txtDirectedHorizontalSpacing.WordWrap")));
            this.txtDirectedHorizontalSpacing.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtDirectedHorizontalSpacing.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtDirectedHorizontalSpacing.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // txtDirectedVerticalSpacing
            // 
            this.txtDirectedVerticalSpacing.AccessibleDescription = resources.GetString("txtDirectedVerticalSpacing.AccessibleDescription");
            this.txtDirectedVerticalSpacing.AccessibleName = resources.GetString("txtDirectedVerticalSpacing.AccessibleName");
            this.txtDirectedVerticalSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtDirectedVerticalSpacing.Anchor")));
            this.txtDirectedVerticalSpacing.AutoSize = ((bool)(resources.GetObject("txtDirectedVerticalSpacing.AutoSize")));
            this.txtDirectedVerticalSpacing.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtDirectedVerticalSpacing.BackgroundImage")));
            this.txtDirectedVerticalSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtDirectedVerticalSpacing.Dock")));
            this.txtDirectedVerticalSpacing.Enabled = ((bool)(resources.GetObject("txtDirectedVerticalSpacing.Enabled")));
            this.txtDirectedVerticalSpacing.Font = ((System.Drawing.Font)(resources.GetObject("txtDirectedVerticalSpacing.Font")));
            this.txtDirectedVerticalSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtDirectedVerticalSpacing.ImeMode")));
            this.txtDirectedVerticalSpacing.Location = ((System.Drawing.Point)(resources.GetObject("txtDirectedVerticalSpacing.Location")));
            this.txtDirectedVerticalSpacing.MaxLength = ((int)(resources.GetObject("txtDirectedVerticalSpacing.MaxLength")));
            this.txtDirectedVerticalSpacing.Multiline = ((bool)(resources.GetObject("txtDirectedVerticalSpacing.Multiline")));
            this.txtDirectedVerticalSpacing.Name = "txtDirectedVerticalSpacing";
            this.txtDirectedVerticalSpacing.PasswordChar = ((char)(resources.GetObject("txtDirectedVerticalSpacing.PasswordChar")));
            this.txtDirectedVerticalSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtDirectedVerticalSpacing.RightToLeft")));
            this.txtDirectedVerticalSpacing.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtDirectedVerticalSpacing.ScrollBars")));
            this.txtDirectedVerticalSpacing.Size = ((System.Drawing.Size)(resources.GetObject("txtDirectedVerticalSpacing.Size")));
            this.txtDirectedVerticalSpacing.TabIndex = ((int)(resources.GetObject("txtDirectedVerticalSpacing.TabIndex")));
            this.txtDirectedVerticalSpacing.Text = resources.GetString("txtDirectedVerticalSpacing.Text");
            this.txtDirectedVerticalSpacing.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtDirectedVerticalSpacing.TextAlign")));
            this.txtDirectedVerticalSpacing.Visible = ((bool)(resources.GetObject("txtDirectedVerticalSpacing.Visible")));
            this.txtDirectedVerticalSpacing.WordWrap = ((bool)(resources.GetObject("txtDirectedVerticalSpacing.WordWrap")));
            this.txtDirectedVerticalSpacing.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // lblDirectedSpacing
            // 
            this.lblDirectedSpacing.AccessibleDescription = resources.GetString("lblDirectedSpacing.AccessibleDescription");
            this.lblDirectedSpacing.AccessibleName = resources.GetString("lblDirectedSpacing.AccessibleName");
            this.lblDirectedSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblDirectedSpacing.Anchor")));
            this.lblDirectedSpacing.AutoSize = ((bool)(resources.GetObject("lblDirectedSpacing.AutoSize")));
            this.lblDirectedSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblDirectedSpacing.Dock")));
            this.lblDirectedSpacing.Enabled = ((bool)(resources.GetObject("lblDirectedSpacing.Enabled")));
            this.lblDirectedSpacing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDirectedSpacing.Font = ((System.Drawing.Font)(resources.GetObject("lblDirectedSpacing.Font")));
            this.lblDirectedSpacing.Image = ((System.Drawing.Image)(resources.GetObject("lblDirectedSpacing.Image")));
            this.lblDirectedSpacing.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDirectedSpacing.ImageAlign")));
            this.lblDirectedSpacing.ImageIndex = ((int)(resources.GetObject("lblDirectedSpacing.ImageIndex")));
            this.lblDirectedSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblDirectedSpacing.ImeMode")));
            this.lblDirectedSpacing.Location = ((System.Drawing.Point)(resources.GetObject("lblDirectedSpacing.Location")));
            this.lblDirectedSpacing.Name = "lblDirectedSpacing";
            this.lblDirectedSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblDirectedSpacing.RightToLeft")));
            this.lblDirectedSpacing.Size = ((System.Drawing.Size)(resources.GetObject("lblDirectedSpacing.Size")));
            this.lblDirectedSpacing.TabIndex = ((int)(resources.GetObject("lblDirectedSpacing.TabIndex")));
            this.lblDirectedSpacing.Text = resources.GetString("lblDirectedSpacing.Text");
            this.lblDirectedSpacing.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDirectedSpacing.TextAlign")));
            this.lblDirectedSpacing.Visible = ((bool)(resources.GetObject("lblDirectedSpacing.Visible")));
            // 
            // lblDirectedVerticalSpacing
            // 
            this.lblDirectedVerticalSpacing.AccessibleDescription = resources.GetString("lblDirectedVerticalSpacing.AccessibleDescription");
            this.lblDirectedVerticalSpacing.AccessibleName = resources.GetString("lblDirectedVerticalSpacing.AccessibleName");
            this.lblDirectedVerticalSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblDirectedVerticalSpacing.Anchor")));
            this.lblDirectedVerticalSpacing.AutoSize = ((bool)(resources.GetObject("lblDirectedVerticalSpacing.AutoSize")));
            this.lblDirectedVerticalSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblDirectedVerticalSpacing.Dock")));
            this.lblDirectedVerticalSpacing.Enabled = ((bool)(resources.GetObject("lblDirectedVerticalSpacing.Enabled")));
            this.lblDirectedVerticalSpacing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDirectedVerticalSpacing.Font = ((System.Drawing.Font)(resources.GetObject("lblDirectedVerticalSpacing.Font")));
            this.lblDirectedVerticalSpacing.Image = ((System.Drawing.Image)(resources.GetObject("lblDirectedVerticalSpacing.Image")));
            this.lblDirectedVerticalSpacing.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDirectedVerticalSpacing.ImageAlign")));
            this.lblDirectedVerticalSpacing.ImageIndex = ((int)(resources.GetObject("lblDirectedVerticalSpacing.ImageIndex")));
            this.lblDirectedVerticalSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblDirectedVerticalSpacing.ImeMode")));
            this.lblDirectedVerticalSpacing.Location = ((System.Drawing.Point)(resources.GetObject("lblDirectedVerticalSpacing.Location")));
            this.lblDirectedVerticalSpacing.Name = "lblDirectedVerticalSpacing";
            this.lblDirectedVerticalSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblDirectedVerticalSpacing.RightToLeft")));
            this.lblDirectedVerticalSpacing.Size = ((System.Drawing.Size)(resources.GetObject("lblDirectedVerticalSpacing.Size")));
            this.lblDirectedVerticalSpacing.TabIndex = ((int)(resources.GetObject("lblDirectedVerticalSpacing.TabIndex")));
            this.lblDirectedVerticalSpacing.Text = resources.GetString("lblDirectedVerticalSpacing.Text");
            this.lblDirectedVerticalSpacing.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDirectedVerticalSpacing.TextAlign")));
            this.lblDirectedVerticalSpacing.Visible = ((bool)(resources.GetObject("lblDirectedVerticalSpacing.Visible")));
            // 
            // lblDirectedHorizontalSpacing
            // 
            this.lblDirectedHorizontalSpacing.AccessibleDescription = resources.GetString("lblDirectedHorizontalSpacing.AccessibleDescription");
            this.lblDirectedHorizontalSpacing.AccessibleName = resources.GetString("lblDirectedHorizontalSpacing.AccessibleName");
            this.lblDirectedHorizontalSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblDirectedHorizontalSpacing.Anchor")));
            this.lblDirectedHorizontalSpacing.AutoSize = ((bool)(resources.GetObject("lblDirectedHorizontalSpacing.AutoSize")));
            this.lblDirectedHorizontalSpacing.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblDirectedHorizontalSpacing.Dock")));
            this.lblDirectedHorizontalSpacing.Enabled = ((bool)(resources.GetObject("lblDirectedHorizontalSpacing.Enabled")));
            this.lblDirectedHorizontalSpacing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDirectedHorizontalSpacing.Font = ((System.Drawing.Font)(resources.GetObject("lblDirectedHorizontalSpacing.Font")));
            this.lblDirectedHorizontalSpacing.Image = ((System.Drawing.Image)(resources.GetObject("lblDirectedHorizontalSpacing.Image")));
            this.lblDirectedHorizontalSpacing.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDirectedHorizontalSpacing.ImageAlign")));
            this.lblDirectedHorizontalSpacing.ImageIndex = ((int)(resources.GetObject("lblDirectedHorizontalSpacing.ImageIndex")));
            this.lblDirectedHorizontalSpacing.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblDirectedHorizontalSpacing.ImeMode")));
            this.lblDirectedHorizontalSpacing.Location = ((System.Drawing.Point)(resources.GetObject("lblDirectedHorizontalSpacing.Location")));
            this.lblDirectedHorizontalSpacing.Name = "lblDirectedHorizontalSpacing";
            this.lblDirectedHorizontalSpacing.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblDirectedHorizontalSpacing.RightToLeft")));
            this.lblDirectedHorizontalSpacing.Size = ((System.Drawing.Size)(resources.GetObject("lblDirectedHorizontalSpacing.Size")));
            this.lblDirectedHorizontalSpacing.TabIndex = ((int)(resources.GetObject("lblDirectedHorizontalSpacing.TabIndex")));
            this.lblDirectedHorizontalSpacing.Text = resources.GetString("lblDirectedHorizontalSpacing.Text");
            this.lblDirectedHorizontalSpacing.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDirectedHorizontalSpacing.TextAlign")));
            this.lblDirectedHorizontalSpacing.Visible = ((bool)(resources.GetObject("lblDirectedHorizontalSpacing.Visible")));
            // 
            // rbtnTop
            // 
            this.rbtnTop.AccessibleDescription = resources.GetString("rbtnTop.AccessibleDescription");
            this.rbtnTop.AccessibleName = resources.GetString("rbtnTop.AccessibleName");
            this.rbtnTop.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rbtnTop.Anchor")));
            this.rbtnTop.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rbtnTop.Appearance")));
            this.rbtnTop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbtnTop.BackgroundImage")));
            this.rbtnTop.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTop.CheckAlign")));
            this.rbtnTop.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rbtnTop.Dock")));
            this.rbtnTop.Enabled = ((bool)(resources.GetObject("rbtnTop.Enabled")));
            this.rbtnTop.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rbtnTop.FlatStyle")));
            this.rbtnTop.Font = ((System.Drawing.Font)(resources.GetObject("rbtnTop.Font")));
            this.rbtnTop.Image = ((System.Drawing.Image)(resources.GetObject("rbtnTop.Image")));
            this.rbtnTop.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTop.ImageAlign")));
            this.rbtnTop.ImageIndex = ((int)(resources.GetObject("rbtnTop.ImageIndex")));
            this.rbtnTop.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rbtnTop.ImeMode")));
            this.rbtnTop.Location = ((System.Drawing.Point)(resources.GetObject("rbtnTop.Location")));
            this.rbtnTop.Name = "rbtnTop";
            this.rbtnTop.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rbtnTop.RightToLeft")));
            this.rbtnTop.Size = ((System.Drawing.Size)(resources.GetObject("rbtnTop.Size")));
            this.rbtnTop.TabIndex = ((int)(resources.GetObject("rbtnTop.TabIndex")));
            this.rbtnTop.Text = resources.GetString("rbtnTop.Text");
            this.rbtnTop.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTop.TextAlign")));
            this.rbtnTop.Visible = ((bool)(resources.GetObject("rbtnTop.Visible")));
            this.rbtnTop.CheckedChanged += new System.EventHandler(this.DirectionChange);
            // 
            // rbtnTopRight
            // 
            this.rbtnTopRight.AccessibleDescription = resources.GetString("rbtnTopRight.AccessibleDescription");
            this.rbtnTopRight.AccessibleName = resources.GetString("rbtnTopRight.AccessibleName");
            this.rbtnTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rbtnTopRight.Anchor")));
            this.rbtnTopRight.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rbtnTopRight.Appearance")));
            this.rbtnTopRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbtnTopRight.BackgroundImage")));
            this.rbtnTopRight.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTopRight.CheckAlign")));
            this.rbtnTopRight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rbtnTopRight.Dock")));
            this.rbtnTopRight.Enabled = ((bool)(resources.GetObject("rbtnTopRight.Enabled")));
            this.rbtnTopRight.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rbtnTopRight.FlatStyle")));
            this.rbtnTopRight.Font = ((System.Drawing.Font)(resources.GetObject("rbtnTopRight.Font")));
            this.rbtnTopRight.Image = ((System.Drawing.Image)(resources.GetObject("rbtnTopRight.Image")));
            this.rbtnTopRight.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTopRight.ImageAlign")));
            this.rbtnTopRight.ImageIndex = ((int)(resources.GetObject("rbtnTopRight.ImageIndex")));
            this.rbtnTopRight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rbtnTopRight.ImeMode")));
            this.rbtnTopRight.Location = ((System.Drawing.Point)(resources.GetObject("rbtnTopRight.Location")));
            this.rbtnTopRight.Name = "rbtnTopRight";
            this.rbtnTopRight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rbtnTopRight.RightToLeft")));
            this.rbtnTopRight.Size = ((System.Drawing.Size)(resources.GetObject("rbtnTopRight.Size")));
            this.rbtnTopRight.TabIndex = ((int)(resources.GetObject("rbtnTopRight.TabIndex")));
            this.rbtnTopRight.Text = resources.GetString("rbtnTopRight.Text");
            this.rbtnTopRight.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnTopRight.TextAlign")));
            this.rbtnTopRight.Visible = ((bool)(resources.GetObject("rbtnTopRight.Visible")));
            this.rbtnTopRight.CheckedChanged += new System.EventHandler(this.DirectionChange);
            // 
            // rbtnLeft
            // 
            this.rbtnLeft.AccessibleDescription = resources.GetString("rbtnLeft.AccessibleDescription");
            this.rbtnLeft.AccessibleName = resources.GetString("rbtnLeft.AccessibleName");
            this.rbtnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rbtnLeft.Anchor")));
            this.rbtnLeft.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rbtnLeft.Appearance")));
            this.rbtnLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbtnLeft.BackgroundImage")));
            this.rbtnLeft.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnLeft.CheckAlign")));
            this.rbtnLeft.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rbtnLeft.Dock")));
            this.rbtnLeft.Enabled = ((bool)(resources.GetObject("rbtnLeft.Enabled")));
            this.rbtnLeft.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rbtnLeft.FlatStyle")));
            this.rbtnLeft.Font = ((System.Drawing.Font)(resources.GetObject("rbtnLeft.Font")));
            this.rbtnLeft.Image = ((System.Drawing.Image)(resources.GetObject("rbtnLeft.Image")));
            this.rbtnLeft.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnLeft.ImageAlign")));
            this.rbtnLeft.ImageIndex = ((int)(resources.GetObject("rbtnLeft.ImageIndex")));
            this.rbtnLeft.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rbtnLeft.ImeMode")));
            this.rbtnLeft.Location = ((System.Drawing.Point)(resources.GetObject("rbtnLeft.Location")));
            this.rbtnLeft.Name = "rbtnLeft";
            this.rbtnLeft.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rbtnLeft.RightToLeft")));
            this.rbtnLeft.Size = ((System.Drawing.Size)(resources.GetObject("rbtnLeft.Size")));
            this.rbtnLeft.TabIndex = ((int)(resources.GetObject("rbtnLeft.TabIndex")));
            this.rbtnLeft.Text = resources.GetString("rbtnLeft.Text");
            this.rbtnLeft.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnLeft.TextAlign")));
            this.rbtnLeft.Visible = ((bool)(resources.GetObject("rbtnLeft.Visible")));
            this.rbtnLeft.CheckedChanged += new System.EventHandler(this.DirectionChange);
            // 
            // rbtnRight
            // 
            this.rbtnRight.AccessibleDescription = resources.GetString("rbtnRight.AccessibleDescription");
            this.rbtnRight.AccessibleName = resources.GetString("rbtnRight.AccessibleName");
            this.rbtnRight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rbtnRight.Anchor")));
            this.rbtnRight.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rbtnRight.Appearance")));
            this.rbtnRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbtnRight.BackgroundImage")));
            this.rbtnRight.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnRight.CheckAlign")));
            this.rbtnRight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rbtnRight.Dock")));
            this.rbtnRight.Enabled = ((bool)(resources.GetObject("rbtnRight.Enabled")));
            this.rbtnRight.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rbtnRight.FlatStyle")));
            this.rbtnRight.Font = ((System.Drawing.Font)(resources.GetObject("rbtnRight.Font")));
            this.rbtnRight.Image = ((System.Drawing.Image)(resources.GetObject("rbtnRight.Image")));
            this.rbtnRight.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnRight.ImageAlign")));
            this.rbtnRight.ImageIndex = ((int)(resources.GetObject("rbtnRight.ImageIndex")));
            this.rbtnRight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rbtnRight.ImeMode")));
            this.rbtnRight.Location = ((System.Drawing.Point)(resources.GetObject("rbtnRight.Location")));
            this.rbtnRight.Name = "rbtnRight";
            this.rbtnRight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rbtnRight.RightToLeft")));
            this.rbtnRight.Size = ((System.Drawing.Size)(resources.GetObject("rbtnRight.Size")));
            this.rbtnRight.TabIndex = ((int)(resources.GetObject("rbtnRight.TabIndex")));
            this.rbtnRight.Text = resources.GetString("rbtnRight.Text");
            this.rbtnRight.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnRight.TextAlign")));
            this.rbtnRight.Visible = ((bool)(resources.GetObject("rbtnRight.Visible")));
            this.rbtnRight.CheckedChanged += new System.EventHandler(this.DirectionChange);
            // 
            // rbtnBottomRight
            // 
            this.rbtnBottomRight.AccessibleDescription = resources.GetString("rbtnBottomRight.AccessibleDescription");
            this.rbtnBottomRight.AccessibleName = resources.GetString("rbtnBottomRight.AccessibleName");
            this.rbtnBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rbtnBottomRight.Anchor")));
            this.rbtnBottomRight.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rbtnBottomRight.Appearance")));
            this.rbtnBottomRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbtnBottomRight.BackgroundImage")));
            this.rbtnBottomRight.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottomRight.CheckAlign")));
            this.rbtnBottomRight.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rbtnBottomRight.Dock")));
            this.rbtnBottomRight.Enabled = ((bool)(resources.GetObject("rbtnBottomRight.Enabled")));
            this.rbtnBottomRight.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rbtnBottomRight.FlatStyle")));
            this.rbtnBottomRight.Font = ((System.Drawing.Font)(resources.GetObject("rbtnBottomRight.Font")));
            this.rbtnBottomRight.Image = ((System.Drawing.Image)(resources.GetObject("rbtnBottomRight.Image")));
            this.rbtnBottomRight.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottomRight.ImageAlign")));
            this.rbtnBottomRight.ImageIndex = ((int)(resources.GetObject("rbtnBottomRight.ImageIndex")));
            this.rbtnBottomRight.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rbtnBottomRight.ImeMode")));
            this.rbtnBottomRight.Location = ((System.Drawing.Point)(resources.GetObject("rbtnBottomRight.Location")));
            this.rbtnBottomRight.Name = "rbtnBottomRight";
            this.rbtnBottomRight.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rbtnBottomRight.RightToLeft")));
            this.rbtnBottomRight.Size = ((System.Drawing.Size)(resources.GetObject("rbtnBottomRight.Size")));
            this.rbtnBottomRight.TabIndex = ((int)(resources.GetObject("rbtnBottomRight.TabIndex")));
            this.rbtnBottomRight.Text = resources.GetString("rbtnBottomRight.Text");
            this.rbtnBottomRight.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottomRight.TextAlign")));
            this.rbtnBottomRight.Visible = ((bool)(resources.GetObject("rbtnBottomRight.Visible")));
            this.rbtnBottomRight.CheckedChanged += new System.EventHandler(this.DirectionChange);
            // 
            // rbtnBottomLeft
            // 
            this.rbtnBottomLeft.AccessibleDescription = resources.GetString("rbtnBottomLeft.AccessibleDescription");
            this.rbtnBottomLeft.AccessibleName = resources.GetString("rbtnBottomLeft.AccessibleName");
            this.rbtnBottomLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rbtnBottomLeft.Anchor")));
            this.rbtnBottomLeft.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rbtnBottomLeft.Appearance")));
            this.rbtnBottomLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbtnBottomLeft.BackgroundImage")));
            this.rbtnBottomLeft.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottomLeft.CheckAlign")));
            this.rbtnBottomLeft.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rbtnBottomLeft.Dock")));
            this.rbtnBottomLeft.Enabled = ((bool)(resources.GetObject("rbtnBottomLeft.Enabled")));
            this.rbtnBottomLeft.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rbtnBottomLeft.FlatStyle")));
            this.rbtnBottomLeft.Font = ((System.Drawing.Font)(resources.GetObject("rbtnBottomLeft.Font")));
            this.rbtnBottomLeft.Image = ((System.Drawing.Image)(resources.GetObject("rbtnBottomLeft.Image")));
            this.rbtnBottomLeft.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottomLeft.ImageAlign")));
            this.rbtnBottomLeft.ImageIndex = ((int)(resources.GetObject("rbtnBottomLeft.ImageIndex")));
            this.rbtnBottomLeft.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rbtnBottomLeft.ImeMode")));
            this.rbtnBottomLeft.Location = ((System.Drawing.Point)(resources.GetObject("rbtnBottomLeft.Location")));
            this.rbtnBottomLeft.Name = "rbtnBottomLeft";
            this.rbtnBottomLeft.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rbtnBottomLeft.RightToLeft")));
            this.rbtnBottomLeft.Size = ((System.Drawing.Size)(resources.GetObject("rbtnBottomLeft.Size")));
            this.rbtnBottomLeft.TabIndex = ((int)(resources.GetObject("rbtnBottomLeft.TabIndex")));
            this.rbtnBottomLeft.Text = resources.GetString("rbtnBottomLeft.Text");
            this.rbtnBottomLeft.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottomLeft.TextAlign")));
            this.rbtnBottomLeft.Visible = ((bool)(resources.GetObject("rbtnBottomLeft.Visible")));
            this.rbtnBottomLeft.CheckedChanged += new System.EventHandler(this.DirectionChange);
            // 
            // rbtnBottom
            // 
            this.rbtnBottom.AccessibleDescription = resources.GetString("rbtnBottom.AccessibleDescription");
            this.rbtnBottom.AccessibleName = resources.GetString("rbtnBottom.AccessibleName");
            this.rbtnBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rbtnBottom.Anchor")));
            this.rbtnBottom.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rbtnBottom.Appearance")));
            this.rbtnBottom.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbtnBottom.BackgroundImage")));
            this.rbtnBottom.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottom.CheckAlign")));
            this.rbtnBottom.Checked = true;
            this.rbtnBottom.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rbtnBottom.Dock")));
            this.rbtnBottom.Enabled = ((bool)(resources.GetObject("rbtnBottom.Enabled")));
            this.rbtnBottom.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rbtnBottom.FlatStyle")));
            this.rbtnBottom.Font = ((System.Drawing.Font)(resources.GetObject("rbtnBottom.Font")));
            this.rbtnBottom.Image = ((System.Drawing.Image)(resources.GetObject("rbtnBottom.Image")));
            this.rbtnBottom.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottom.ImageAlign")));
            this.rbtnBottom.ImageIndex = ((int)(resources.GetObject("rbtnBottom.ImageIndex")));
            this.rbtnBottom.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rbtnBottom.ImeMode")));
            this.rbtnBottom.Location = ((System.Drawing.Point)(resources.GetObject("rbtnBottom.Location")));
            this.rbtnBottom.Name = "rbtnBottom";
            this.rbtnBottom.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rbtnBottom.RightToLeft")));
            this.rbtnBottom.Size = ((System.Drawing.Size)(resources.GetObject("rbtnBottom.Size")));
            this.rbtnBottom.TabIndex = ((int)(resources.GetObject("rbtnBottom.TabIndex")));
            this.rbtnBottom.TabStop = true;
            this.rbtnBottom.Text = resources.GetString("rbtnBottom.Text");
            this.rbtnBottom.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rbtnBottom.TextAlign")));
            this.rbtnBottom.Visible = ((bool)(resources.GetObject("rbtnBottom.Visible")));
            this.rbtnBottom.CheckedChanged += new System.EventHandler(this.DirectionChange);
            // 
            // txtRotationAngle
            // 
            this.txtRotationAngle.AccessibleDescription = resources.GetString("txtRotationAngle.AccessibleDescription");
            this.txtRotationAngle.AccessibleName = resources.GetString("txtRotationAngle.AccessibleName");
            this.txtRotationAngle.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtRotationAngle.Anchor")));
            this.txtRotationAngle.AutoSize = ((bool)(resources.GetObject("txtRotationAngle.AutoSize")));
            this.txtRotationAngle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtRotationAngle.BackgroundImage")));
            this.txtRotationAngle.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtRotationAngle.Dock")));
            this.txtRotationAngle.Enabled = ((bool)(resources.GetObject("txtRotationAngle.Enabled")));
            this.txtRotationAngle.Font = ((System.Drawing.Font)(resources.GetObject("txtRotationAngle.Font")));
            this.txtRotationAngle.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtRotationAngle.ImeMode")));
            this.txtRotationAngle.Location = ((System.Drawing.Point)(resources.GetObject("txtRotationAngle.Location")));
            this.txtRotationAngle.MaxLength = ((int)(resources.GetObject("txtRotationAngle.MaxLength")));
            this.txtRotationAngle.Multiline = ((bool)(resources.GetObject("txtRotationAngle.Multiline")));
            this.txtRotationAngle.Name = "txtRotationAngle";
            this.txtRotationAngle.PasswordChar = ((char)(resources.GetObject("txtRotationAngle.PasswordChar")));
            this.txtRotationAngle.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtRotationAngle.RightToLeft")));
            this.txtRotationAngle.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtRotationAngle.ScrollBars")));
            this.txtRotationAngle.Size = ((System.Drawing.Size)(resources.GetObject("txtRotationAngle.Size")));
            this.txtRotationAngle.TabIndex = ((int)(resources.GetObject("txtRotationAngle.TabIndex")));
            this.txtRotationAngle.Text = resources.GetString("txtRotationAngle.Text");
            this.txtRotationAngle.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtRotationAngle.TextAlign")));
            this.txtRotationAngle.Visible = ((bool)(resources.GetObject("txtRotationAngle.Visible")));
            this.txtRotationAngle.WordWrap = ((bool)(resources.GetObject("txtRotationAngle.WordWrap")));
            this.txtRotationAngle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtRotationAngle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtRotationAngle.TextChanged += new System.EventHandler(this.RotationAngle_TextChanged);
            // 
            // lblRotationAngle
            // 
            this.lblRotationAngle.AccessibleDescription = resources.GetString("lblRotationAngle.AccessibleDescription");
            this.lblRotationAngle.AccessibleName = resources.GetString("lblRotationAngle.AccessibleName");
            this.lblRotationAngle.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblRotationAngle.Anchor")));
            this.lblRotationAngle.AutoSize = ((bool)(resources.GetObject("lblRotationAngle.AutoSize")));
            this.lblRotationAngle.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblRotationAngle.Dock")));
            this.lblRotationAngle.Enabled = ((bool)(resources.GetObject("lblRotationAngle.Enabled")));
            this.lblRotationAngle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblRotationAngle.Font = ((System.Drawing.Font)(resources.GetObject("lblRotationAngle.Font")));
            this.lblRotationAngle.Image = ((System.Drawing.Image)(resources.GetObject("lblRotationAngle.Image")));
            this.lblRotationAngle.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblRotationAngle.ImageAlign")));
            this.lblRotationAngle.ImageIndex = ((int)(resources.GetObject("lblRotationAngle.ImageIndex")));
            this.lblRotationAngle.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblRotationAngle.ImeMode")));
            this.lblRotationAngle.Location = ((System.Drawing.Point)(resources.GetObject("lblRotationAngle.Location")));
            this.lblRotationAngle.Name = "lblRotationAngle";
            this.lblRotationAngle.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblRotationAngle.RightToLeft")));
            this.lblRotationAngle.Size = ((System.Drawing.Size)(resources.GetObject("lblRotationAngle.Size")));
            this.lblRotationAngle.TabIndex = ((int)(resources.GetObject("lblRotationAngle.TabIndex")));
            this.lblRotationAngle.Text = resources.GetString("lblRotationAngle.Text");
            this.lblRotationAngle.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblRotationAngle.TextAlign")));
            this.lblRotationAngle.Visible = ((bool)(resources.GetObject("lblRotationAngle.Visible")));
            // 
            // grpSymmetricLayout
            // 
            this.grpSymmetricLayout.AccessibleDescription = resources.GetString("grpSymmetricLayout.AccessibleDescription");
            this.grpSymmetricLayout.AccessibleName = resources.GetString("grpSymmetricLayout.AccessibleName");
            this.grpSymmetricLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("grpSymmetricLayout.Anchor")));
            this.grpSymmetricLayout.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("grpSymmetricLayout.BackgroundImage")));
            this.grpSymmetricLayout.Controls.Add(this.lblForceSpringFactor);
            this.grpSymmetricLayout.Controls.Add(this.txtSymmetricForceSpringFactor);
            this.grpSymmetricLayout.Controls.Add(this.lblMaxSpringLength);
            this.grpSymmetricLayout.Controls.Add(this.txtSymmetricMaxSpringLength);
            this.grpSymmetricLayout.Controls.Add(this.lblMaxIteraction);
            this.grpSymmetricLayout.Controls.Add(this.txtSymmetricMaxIteraction);
            this.grpSymmetricLayout.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("grpSymmetricLayout.Dock")));
            this.grpSymmetricLayout.Enabled = ((bool)(resources.GetObject("grpSymmetricLayout.Enabled")));
            this.grpSymmetricLayout.Font = ((System.Drawing.Font)(resources.GetObject("grpSymmetricLayout.Font")));
            this.grpSymmetricLayout.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("grpSymmetricLayout.ImeMode")));
            this.grpSymmetricLayout.Location = ((System.Drawing.Point)(resources.GetObject("grpSymmetricLayout.Location")));
            this.grpSymmetricLayout.Name = "grpSymmetricLayout";
            this.grpSymmetricLayout.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("grpSymmetricLayout.RightToLeft")));
            this.grpSymmetricLayout.Size = ((System.Drawing.Size)(resources.GetObject("grpSymmetricLayout.Size")));
            this.grpSymmetricLayout.TabIndex = ((int)(resources.GetObject("grpSymmetricLayout.TabIndex")));
            this.grpSymmetricLayout.TabStop = false;
            this.grpSymmetricLayout.Text = resources.GetString("grpSymmetricLayout.Text");
            this.grpSymmetricLayout.Visible = ((bool)(resources.GetObject("grpSymmetricLayout.Visible")));
            // 
            // lblForceSpringFactor
            // 
            this.lblForceSpringFactor.AccessibleDescription = resources.GetString("lblForceSpringFactor.AccessibleDescription");
            this.lblForceSpringFactor.AccessibleName = resources.GetString("lblForceSpringFactor.AccessibleName");
            this.lblForceSpringFactor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblForceSpringFactor.Anchor")));
            this.lblForceSpringFactor.AutoSize = ((bool)(resources.GetObject("lblForceSpringFactor.AutoSize")));
            this.lblForceSpringFactor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblForceSpringFactor.Dock")));
            this.lblForceSpringFactor.Enabled = ((bool)(resources.GetObject("lblForceSpringFactor.Enabled")));
            this.lblForceSpringFactor.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblForceSpringFactor.Font = ((System.Drawing.Font)(resources.GetObject("lblForceSpringFactor.Font")));
            this.lblForceSpringFactor.Image = ((System.Drawing.Image)(resources.GetObject("lblForceSpringFactor.Image")));
            this.lblForceSpringFactor.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblForceSpringFactor.ImageAlign")));
            this.lblForceSpringFactor.ImageIndex = ((int)(resources.GetObject("lblForceSpringFactor.ImageIndex")));
            this.lblForceSpringFactor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblForceSpringFactor.ImeMode")));
            this.lblForceSpringFactor.Location = ((System.Drawing.Point)(resources.GetObject("lblForceSpringFactor.Location")));
            this.lblForceSpringFactor.Name = "lblForceSpringFactor";
            this.lblForceSpringFactor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblForceSpringFactor.RightToLeft")));
            this.lblForceSpringFactor.Size = ((System.Drawing.Size)(resources.GetObject("lblForceSpringFactor.Size")));
            this.lblForceSpringFactor.TabIndex = ((int)(resources.GetObject("lblForceSpringFactor.TabIndex")));
            this.lblForceSpringFactor.Text = resources.GetString("lblForceSpringFactor.Text");
            this.lblForceSpringFactor.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblForceSpringFactor.TextAlign")));
            this.lblForceSpringFactor.Visible = ((bool)(resources.GetObject("lblForceSpringFactor.Visible")));
            // 
            // txtSymmetricForceSpringFactor
            // 
            this.txtSymmetricForceSpringFactor.AccessibleDescription = resources.GetString("txtSymmetricForceSpringFactor.AccessibleDescription");
            this.txtSymmetricForceSpringFactor.AccessibleName = resources.GetString("txtSymmetricForceSpringFactor.AccessibleName");
            this.txtSymmetricForceSpringFactor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtSymmetricForceSpringFactor.Anchor")));
            this.txtSymmetricForceSpringFactor.AutoSize = ((bool)(resources.GetObject("txtSymmetricForceSpringFactor.AutoSize")));
            this.txtSymmetricForceSpringFactor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtSymmetricForceSpringFactor.BackgroundImage")));
            this.txtSymmetricForceSpringFactor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtSymmetricForceSpringFactor.Dock")));
            this.txtSymmetricForceSpringFactor.Enabled = ((bool)(resources.GetObject("txtSymmetricForceSpringFactor.Enabled")));
            this.txtSymmetricForceSpringFactor.Font = ((System.Drawing.Font)(resources.GetObject("txtSymmetricForceSpringFactor.Font")));
            this.txtSymmetricForceSpringFactor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtSymmetricForceSpringFactor.ImeMode")));
            this.txtSymmetricForceSpringFactor.Location = ((System.Drawing.Point)(resources.GetObject("txtSymmetricForceSpringFactor.Location")));
            this.txtSymmetricForceSpringFactor.MaxLength = ((int)(resources.GetObject("txtSymmetricForceSpringFactor.MaxLength")));
            this.txtSymmetricForceSpringFactor.Multiline = ((bool)(resources.GetObject("txtSymmetricForceSpringFactor.Multiline")));
            this.txtSymmetricForceSpringFactor.Name = "txtSymmetricForceSpringFactor";
            this.txtSymmetricForceSpringFactor.PasswordChar = ((char)(resources.GetObject("txtSymmetricForceSpringFactor.PasswordChar")));
            this.txtSymmetricForceSpringFactor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtSymmetricForceSpringFactor.RightToLeft")));
            this.txtSymmetricForceSpringFactor.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtSymmetricForceSpringFactor.ScrollBars")));
            this.txtSymmetricForceSpringFactor.Size = ((System.Drawing.Size)(resources.GetObject("txtSymmetricForceSpringFactor.Size")));
            this.txtSymmetricForceSpringFactor.TabIndex = ((int)(resources.GetObject("txtSymmetricForceSpringFactor.TabIndex")));
            this.txtSymmetricForceSpringFactor.Text = resources.GetString("txtSymmetricForceSpringFactor.Text");
            this.txtSymmetricForceSpringFactor.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtSymmetricForceSpringFactor.TextAlign")));
            this.txtSymmetricForceSpringFactor.Visible = ((bool)(resources.GetObject("txtSymmetricForceSpringFactor.Visible")));
            this.txtSymmetricForceSpringFactor.WordWrap = ((bool)(resources.GetObject("txtSymmetricForceSpringFactor.WordWrap")));
            this.txtSymmetricForceSpringFactor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtSymmetricForceSpringFactor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtSymmetricForceSpringFactor.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // lblMaxSpringLength
            // 
            this.lblMaxSpringLength.AccessibleDescription = resources.GetString("lblMaxSpringLength.AccessibleDescription");
            this.lblMaxSpringLength.AccessibleName = resources.GetString("lblMaxSpringLength.AccessibleName");
            this.lblMaxSpringLength.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblMaxSpringLength.Anchor")));
            this.lblMaxSpringLength.AutoSize = ((bool)(resources.GetObject("lblMaxSpringLength.AutoSize")));
            this.lblMaxSpringLength.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblMaxSpringLength.Dock")));
            this.lblMaxSpringLength.Enabled = ((bool)(resources.GetObject("lblMaxSpringLength.Enabled")));
            this.lblMaxSpringLength.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblMaxSpringLength.Font = ((System.Drawing.Font)(resources.GetObject("lblMaxSpringLength.Font")));
            this.lblMaxSpringLength.Image = ((System.Drawing.Image)(resources.GetObject("lblMaxSpringLength.Image")));
            this.lblMaxSpringLength.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblMaxSpringLength.ImageAlign")));
            this.lblMaxSpringLength.ImageIndex = ((int)(resources.GetObject("lblMaxSpringLength.ImageIndex")));
            this.lblMaxSpringLength.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblMaxSpringLength.ImeMode")));
            this.lblMaxSpringLength.Location = ((System.Drawing.Point)(resources.GetObject("lblMaxSpringLength.Location")));
            this.lblMaxSpringLength.Name = "lblMaxSpringLength";
            this.lblMaxSpringLength.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblMaxSpringLength.RightToLeft")));
            this.lblMaxSpringLength.Size = ((System.Drawing.Size)(resources.GetObject("lblMaxSpringLength.Size")));
            this.lblMaxSpringLength.TabIndex = ((int)(resources.GetObject("lblMaxSpringLength.TabIndex")));
            this.lblMaxSpringLength.Text = resources.GetString("lblMaxSpringLength.Text");
            this.lblMaxSpringLength.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblMaxSpringLength.TextAlign")));
            this.lblMaxSpringLength.Visible = ((bool)(resources.GetObject("lblMaxSpringLength.Visible")));
            // 
            // txtSymmetricMaxSpringLength
            // 
            this.txtSymmetricMaxSpringLength.AccessibleDescription = resources.GetString("txtSymmetricMaxSpringLength.AccessibleDescription");
            this.txtSymmetricMaxSpringLength.AccessibleName = resources.GetString("txtSymmetricMaxSpringLength.AccessibleName");
            this.txtSymmetricMaxSpringLength.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtSymmetricMaxSpringLength.Anchor")));
            this.txtSymmetricMaxSpringLength.AutoSize = ((bool)(resources.GetObject("txtSymmetricMaxSpringLength.AutoSize")));
            this.txtSymmetricMaxSpringLength.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtSymmetricMaxSpringLength.BackgroundImage")));
            this.txtSymmetricMaxSpringLength.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtSymmetricMaxSpringLength.Dock")));
            this.txtSymmetricMaxSpringLength.Enabled = ((bool)(resources.GetObject("txtSymmetricMaxSpringLength.Enabled")));
            this.txtSymmetricMaxSpringLength.Font = ((System.Drawing.Font)(resources.GetObject("txtSymmetricMaxSpringLength.Font")));
            this.txtSymmetricMaxSpringLength.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtSymmetricMaxSpringLength.ImeMode")));
            this.txtSymmetricMaxSpringLength.Location = ((System.Drawing.Point)(resources.GetObject("txtSymmetricMaxSpringLength.Location")));
            this.txtSymmetricMaxSpringLength.MaxLength = ((int)(resources.GetObject("txtSymmetricMaxSpringLength.MaxLength")));
            this.txtSymmetricMaxSpringLength.Multiline = ((bool)(resources.GetObject("txtSymmetricMaxSpringLength.Multiline")));
            this.txtSymmetricMaxSpringLength.Name = "txtSymmetricMaxSpringLength";
            this.txtSymmetricMaxSpringLength.PasswordChar = ((char)(resources.GetObject("txtSymmetricMaxSpringLength.PasswordChar")));
            this.txtSymmetricMaxSpringLength.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtSymmetricMaxSpringLength.RightToLeft")));
            this.txtSymmetricMaxSpringLength.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtSymmetricMaxSpringLength.ScrollBars")));
            this.txtSymmetricMaxSpringLength.Size = ((System.Drawing.Size)(resources.GetObject("txtSymmetricMaxSpringLength.Size")));
            this.txtSymmetricMaxSpringLength.TabIndex = ((int)(resources.GetObject("txtSymmetricMaxSpringLength.TabIndex")));
            this.txtSymmetricMaxSpringLength.Text = resources.GetString("txtSymmetricMaxSpringLength.Text");
            this.txtSymmetricMaxSpringLength.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtSymmetricMaxSpringLength.TextAlign")));
            this.txtSymmetricMaxSpringLength.Visible = ((bool)(resources.GetObject("txtSymmetricMaxSpringLength.Visible")));
            this.txtSymmetricMaxSpringLength.WordWrap = ((bool)(resources.GetObject("txtSymmetricMaxSpringLength.WordWrap")));
            this.txtSymmetricMaxSpringLength.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtSymmetricMaxSpringLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtSymmetricMaxSpringLength.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // lblMaxIteraction
            // 
            this.lblMaxIteraction.AccessibleDescription = resources.GetString("lblMaxIteraction.AccessibleDescription");
            this.lblMaxIteraction.AccessibleName = resources.GetString("lblMaxIteraction.AccessibleName");
            this.lblMaxIteraction.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblMaxIteraction.Anchor")));
            this.lblMaxIteraction.AutoSize = ((bool)(resources.GetObject("lblMaxIteraction.AutoSize")));
            this.lblMaxIteraction.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblMaxIteraction.Dock")));
            this.lblMaxIteraction.Enabled = ((bool)(resources.GetObject("lblMaxIteraction.Enabled")));
            this.lblMaxIteraction.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblMaxIteraction.Font = ((System.Drawing.Font)(resources.GetObject("lblMaxIteraction.Font")));
            this.lblMaxIteraction.Image = ((System.Drawing.Image)(resources.GetObject("lblMaxIteraction.Image")));
            this.lblMaxIteraction.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblMaxIteraction.ImageAlign")));
            this.lblMaxIteraction.ImageIndex = ((int)(resources.GetObject("lblMaxIteraction.ImageIndex")));
            this.lblMaxIteraction.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblMaxIteraction.ImeMode")));
            this.lblMaxIteraction.Location = ((System.Drawing.Point)(resources.GetObject("lblMaxIteraction.Location")));
            this.lblMaxIteraction.Name = "lblMaxIteraction";
            this.lblMaxIteraction.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblMaxIteraction.RightToLeft")));
            this.lblMaxIteraction.Size = ((System.Drawing.Size)(resources.GetObject("lblMaxIteraction.Size")));
            this.lblMaxIteraction.TabIndex = ((int)(resources.GetObject("lblMaxIteraction.TabIndex")));
            this.lblMaxIteraction.Text = resources.GetString("lblMaxIteraction.Text");
            this.lblMaxIteraction.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblMaxIteraction.TextAlign")));
            this.lblMaxIteraction.Visible = ((bool)(resources.GetObject("lblMaxIteraction.Visible")));
            // 
            // txtSymmetricMaxIteraction
            // 
            this.txtSymmetricMaxIteraction.AccessibleDescription = resources.GetString("txtSymmetricMaxIteraction.AccessibleDescription");
            this.txtSymmetricMaxIteraction.AccessibleName = resources.GetString("txtSymmetricMaxIteraction.AccessibleName");
            this.txtSymmetricMaxIteraction.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtSymmetricMaxIteraction.Anchor")));
            this.txtSymmetricMaxIteraction.AutoSize = ((bool)(resources.GetObject("txtSymmetricMaxIteraction.AutoSize")));
            this.txtSymmetricMaxIteraction.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtSymmetricMaxIteraction.BackgroundImage")));
            this.txtSymmetricMaxIteraction.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtSymmetricMaxIteraction.Dock")));
            this.txtSymmetricMaxIteraction.Enabled = ((bool)(resources.GetObject("txtSymmetricMaxIteraction.Enabled")));
            this.txtSymmetricMaxIteraction.Font = ((System.Drawing.Font)(resources.GetObject("txtSymmetricMaxIteraction.Font")));
            this.txtSymmetricMaxIteraction.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtSymmetricMaxIteraction.ImeMode")));
            this.txtSymmetricMaxIteraction.Location = ((System.Drawing.Point)(resources.GetObject("txtSymmetricMaxIteraction.Location")));
            this.txtSymmetricMaxIteraction.MaxLength = ((int)(resources.GetObject("txtSymmetricMaxIteraction.MaxLength")));
            this.txtSymmetricMaxIteraction.Multiline = ((bool)(resources.GetObject("txtSymmetricMaxIteraction.Multiline")));
            this.txtSymmetricMaxIteraction.Name = "txtSymmetricMaxIteraction";
            this.txtSymmetricMaxIteraction.PasswordChar = ((char)(resources.GetObject("txtSymmetricMaxIteraction.PasswordChar")));
            this.txtSymmetricMaxIteraction.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtSymmetricMaxIteraction.RightToLeft")));
            this.txtSymmetricMaxIteraction.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtSymmetricMaxIteraction.ScrollBars")));
            this.txtSymmetricMaxIteraction.Size = ((System.Drawing.Size)(resources.GetObject("txtSymmetricMaxIteraction.Size")));
            this.txtSymmetricMaxIteraction.TabIndex = ((int)(resources.GetObject("txtSymmetricMaxIteraction.TabIndex")));
            this.txtSymmetricMaxIteraction.Text = resources.GetString("txtSymmetricMaxIteraction.Text");
            this.txtSymmetricMaxIteraction.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtSymmetricMaxIteraction.TextAlign")));
            this.txtSymmetricMaxIteraction.Visible = ((bool)(resources.GetObject("txtSymmetricMaxIteraction.Visible")));
            this.txtSymmetricMaxIteraction.WordWrap = ((bool)(resources.GetObject("txtSymmetricMaxIteraction.WordWrap")));
            this.txtSymmetricMaxIteraction.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
            this.txtSymmetricMaxIteraction.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.txtSymmetricMaxIteraction.TextChanged += new System.EventHandler(this.UpdateApplyBtnState);
            // 
            // LayoutDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.CancelButton = this.btnCancel;
            this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.grpTableLayout);
            this.Controls.Add(this.grpPreview);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.grpPlacement);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.chkEnlargePage);
            this.Controls.Add(this.grpSymmetricLayout);
            this.Controls.Add(this.grpTreeLayout);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.MaximizeBox = false;
            this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
            this.MinimizeBox = false;
            this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
            this.Name = "LayoutDialog";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
            this.ShowInTaskbar = false;
            this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.grpPlacement.ResumeLayout(false);
            this.grpSettings.ResumeLayout(false);
            this.grpPreview.ResumeLayout(false);
            this.grpTableLayout.ResumeLayout(false);
            this.grpTreeLayout.ResumeLayout(false);
            this.grpSymmetricLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Gets the selected layout manager.
        /// </summary>
        /// <value>The layout manager.</value>
        protected LayoutManager LayoutManager
        {
            get
            {
                LayoutStyle style = (LayoutStyle)comboStyle.SelectedItem;
                LayoutManager managerToReturn = null;

                switch (style)
                {
                    case LayoutStyle.DirectedTree:
                        DirectedTreeLayoutManager lmDirTmp = this.DirectedTreeLM;

                        lmDirTmp.VerticalSpacing = m_directedLMSettings.VerticalSpacing;
                        lmDirTmp.HorizontalSpacing = m_directedLMSettings.HorizontalSpacing;
                        lmDirTmp.RotationAngle = m_directedLMSettings.RotationAngle;

                        managerToReturn = lmDirTmp;
                        break;
                    case LayoutStyle.Hierarchic:
                        HierarchicLayoutManager lmHrTmp = this.HierarchicLM;

                        lmHrTmp.VerticalSpacing = m_hierarchicLMSettings.VerticalSpacing;
                        lmHrTmp.HorizontalSpacing = m_hierarchicLMSettings.HorizontalSpacing;
                        lmHrTmp.RotationAngle = m_hierarchicLMSettings.RotationAngle;

                        managerToReturn = lmHrTmp;
                        break;
                    case LayoutStyle.Symmetric:
                        SymmetricLayoutManager lmSymTmp = this.SymmetricLM;

                        lmSymTmp.SpringLength = m_symmetricLMSettings.Distance;
                        lmSymTmp.MaxIteraction = m_symmetricLMSettings.MaxIteraction;
                        lmSymTmp.SpringFactor = m_symmetricLMSettings.SpringFactor;

                        managerToReturn = lmSymTmp;
                        break;
                    case LayoutStyle.Table:
                        TableLayoutManager lmTblTmp = this.TableLM;
						lmTblTmp.MeasurementUnits = m_tableLMSettings.MeasurementUnits;
						lmTblTmp.LeftMargin = m_tableLMSettings.LeftMargin;
                        lmTblTmp.TopMargin = m_tableLMSettings.TopMargin;
                        lmTblTmp.VerticalSpacing = m_tableLMSettings.VerticalSpacing;
                        lmTblTmp.HorizontalSpacing = m_tableLMSettings.HorizontalSpacing;
                        lmTblTmp.MaxSize = new SizeF(m_tableLMSettings.MaxWidth, m_tableLMSettings.MaxHeight);
                        lmTblTmp.MaxColummnCount = m_tableLMSettings.MaxColumnCount;
                        lmTblTmp.MaxRowsCount = m_tableLMSettings.MaxRowsCount;
                        lmTblTmp.Orientation = m_tableLMSettings.Orientation;
                        lmTblTmp.CellSizeMode = m_tableLMSettings.CellSizeMode;						
                        managerToReturn = lmTblTmp;
                        break;
                    case LayoutStyle.Radial:
                        RadialTreeLayoutManager lmRadTmp = this.RadialLM;

                        lmRadTmp.VerticalSpacing = m_radialLMSettings.VerticalSpacing;
                        lmRadTmp.HorizontalSpacing = m_radialLMSettings.HorizontalSpacing;
                        lmRadTmp.RotationAngle = m_radialLMSettings.RotationAngle;

                        managerToReturn = lmRadTmp;
                        break;
                }

                return managerToReturn;
            }
        }
        private DirectedTreeLayoutManager DirectedTreeLM
        {
            get { return new DirectedTreeLayoutManager(null, 0, 0, 0); }
        }
        private HierarchicLayoutManager HierarchicLM
        {
            get { return new HierarchicLayoutManager(null, 0, 0, 0); }
        }
        private SymmetricLayoutManager SymmetricLM
        {
            get { return new SymmetricLayoutManager(null, 0); }
        }
        private TableLayoutManager TableLM
        {
            get { return new TableLayoutManager(null, 0, 0); }
        }
        private RadialTreeLayoutManager RadialLM
        {
            get { return new RadialTreeLayoutManager(null, 0, 0, 0); }
        }
        #endregion

        #region Events handlers
        private void ComboStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideSettingsGroups();
            RebindLMSettings();
            UpdatePreview();
            UpdateApplyBtnState(sender, e);
        }
        private void Apply_Click(object sender, EventArgs e)
        {
            UpdateLayout();
            btnApply.Enabled = false;
        }
        private void Ok_Click(object sender, EventArgs e)
        {
			if(btnApply.Enabled)
				UpdateLayout();
            Close();
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void UpdateApplyBtnState(object sender, EventArgs e)
        {
            if (!btnApply.Enabled)
                btnApply.Enabled = true;
        }
        private void DirectionChange(object sender, EventArgs e)
        {
            if (!m_bLock)
            {
                int index = Array.IndexOf(m_directions, sender);

                if (index != -1)
                {
                    int fAngle = ((8 - index) * 45) % 360;

                    m_bLock = true;
                    txtRotationAngle.Text = fAngle.ToString();
                    txtRotationAngle.Focus();
                    m_bLock = false;

                    if (!btnApply.Enabled)
                        btnApply.Enabled = true;
                }
            }
        }
        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            m_bBackDelete = IsBackOrDelete(e);
        }
        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, false, c_nFLOAT_MAX_DIGITS);
        }
        private void TextColumnBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, true, c_nCOLUMNS_MAX_DIGITS);
        }
        private void TableColumnCount_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            int nTextBoxValue = int.Parse(txtBox.Text);

            if (nTextBoxValue > 9999)
            {
                txtBox.Text = 9999.ToString();
            }

            UpdateApplyBtnState(sender, e);
        }
        private void RotationAngle_TextChanged(object sender, EventArgs e)
        {
            if (!m_bLock)
            {
                m_bLock = true;

                float fRotationAngle = GetValue(txtRotationAngle);

                // check whether max value is not exeeded
                if (fRotationAngle > CommonUsedValues.CIRCLE)
                {
                    txtRotationAngle.Text = CommonUsedValues.CIRCLE.ToString();
                    fRotationAngle = CommonUsedValues.CIRCLE;
                }

                // define radio button to select
                int nTemp = (int)((CommonUsedValues.CIRCLE - 45 / 2) - fRotationAngle);
                int index;

                if (nTemp < 0)
                {
                    index = 0;
                }
                else
                {
                    index = ((nTemp / 45) + 1) % 8;
                }

                m_directions[index].Checked = true;

                m_bLock = false;
            }

            UpdateApplyBtnState(sender, e);
        }
        #endregion

        #region Helper methods
        private void UpdateCurrentLayoutSettings()
        {
            bool bStop = false;
            TableLayoutManager tlm = m_diagram.LayoutManager as TableLayoutManager;

            if (tlm != null)
            {
				m_tableLMSettings.MeasurementUnits = tlm.MeasurementUnits;
				m_tableLMSettings.LeftMargin = tlm.LeftMargin;
                m_tableLMSettings.TopMargin = tlm.TopMargin;
                m_tableLMSettings.VerticalSpacing = tlm.VerticalSpacing;
                m_tableLMSettings.HorizontalSpacing = tlm.HorizontalSpacing;
                m_tableLMSettings.MaxWidth = tlm.MaxSize.Width;
                m_tableLMSettings.MaxHeight = tlm.MaxSize.Height;
                m_tableLMSettings.MaxColumnCount = tlm.MaxColummnCount;
                m_tableLMSettings.MaxRowsCount = tlm.MaxRowsCount;
                m_tableLMSettings.Orientation = tlm.Orientation;
                m_tableLMSettings.CellSizeMode = tlm.CellSizeMode;
                bStop = true;
            }

            if (!bStop)
            {
                DirectedTreeLayoutManager dtlm = m_diagram.LayoutManager as DirectedTreeLayoutManager;

                if (dtlm != null)
                {
                    m_directedLMSettings.HorizontalSpacing = dtlm.HorizontalSpacing;
                    m_directedLMSettings.VerticalSpacing = dtlm.VerticalSpacing;
                    m_directedLMSettings.RotationAngle = (int)dtlm.RotationAngle;
                    comboStyle.SelectedIndex = 1;
                    bStop = true;
                }
            }
            if (!bStop)
            {
                HierarchicLayoutManager hlm = m_diagram.LayoutManager as HierarchicLayoutManager;

                if (hlm != null)
                {
                    m_hierarchicLMSettings.HorizontalSpacing = hlm.HorizontalSpacing;
                    m_hierarchicLMSettings.VerticalSpacing = hlm.VerticalSpacing;
                    m_hierarchicLMSettings.RotationAngle = (int)hlm.RotationAngle;
                    comboStyle.SelectedIndex = 2;
                    bStop = true;
                }
            }

            if (!bStop)
            {
                RadialTreeLayoutManager rtlm = m_diagram.LayoutManager as RadialTreeLayoutManager;

                if (rtlm != null)
                {
                    m_radialLMSettings.HorizontalSpacing = rtlm.HorizontalSpacing;
                    m_radialLMSettings.VerticalSpacing = rtlm.VerticalSpacing;
                    m_radialLMSettings.RotationAngle = (int)rtlm.RotationAngle;
                    comboStyle.SelectedIndex = 3;
                    bStop = true;
                }
            }

            if (!bStop)
            {
                SymmetricLayoutManager slm = m_diagram.LayoutManager as SymmetricLayoutManager;

                if (slm != null)
                {
                    m_symmetricLMSettings.Distance = slm.SpringLength;
                    m_symmetricLMSettings.MaxIteraction = slm.MaxIteraction;
                    m_symmetricLMSettings.SpringFactor = slm.SpringFactor;
                    comboStyle.SelectedIndex = 4;
                }
            }
        }

        /// <summary>
        /// Gets the value from text box control.
        /// </summary>
        /// <param name="txtBox">The text box.</param>
        /// <returns>The value from the textbox</returns>
        protected float GetValue(TextBox txtBox)
        {
            string strValue = txtBox.Text;
            NumberFormatInfo nfi = Thread.CurrentThread.CurrentCulture.NumberFormat;
            RegexOptions regOpt = RegexOptions.IgnoreCase;
            Match match = Regex.Match(strValue, "[" + c_srtDECIMAL_SEPARATOR + "]", regOpt);

            if (match.Success)
            {
                if (strValue.Length == 1)
                    strValue = "0";
            }
            else if (strValue.Length == 0)
            {
                strValue = "0";
            }

            // parse value
            float fRetValue = 0;

            try
            {
                fRetValue = (float)double.Parse(strValue, nfi);
            }
            catch (FormatException)
            {
                txtBox.Text = "0";
            }

            return fRetValue;
        }

        /// <summary>
        /// Initializes the comboBox layout style.
        /// </summary>
        private void InitializeDialog()
        {
            m_directions = new RadioButton[]
            {
               rbtnBottom,
               rbtnBottomRight,
               rbtnRight,
               rbtnTopRight,
               rbtnTop,
               rbtnTopLeft,
               rbtnLeft,
               rbtnBottomLeft
            };

            DrawDirections();

            m_radialLMSettings = new GraphLMSettings();
            m_hierarchicLMSettings = new GraphLMSettings();
            m_directedLMSettings = new GraphLMSettings();
            m_symmetricLMSettings = new SymmetricLMSettings();
            m_tableLMSettings = new TableLMSettings();

            FillCombo(this.comboStyle, typeof(LayoutStyle));
            FillCombo(comboTableCellSizeMode, typeof(CellSizeMode));
            FillCombo(comboTableExpandMode, typeof(Orientation));
			FillCombo(comboTableMeasureUnits, typeof(MeasureUnits));
        }

        /// <summary>
        /// Updates the layout parameters.
        /// </summary>
        private void RebindLMSettings()
        {
            LayoutStyle style = (LayoutStyle)comboStyle.SelectedItem;

            switch (style)
            {
                case LayoutStyle.DirectedTree:
                    RebindDirectdTreeLMSettings();
                    break;
                case LayoutStyle.Hierarchic:
                    RebindHierarchicLMSettings();
                    break;
                case LayoutStyle.Symmetric:
                    RebindSymmetricLMSettings();
                    break;
                case LayoutStyle.Table:
                    RebindTableLMSettings();
                    break;
                case LayoutStyle.Radial:
                    RebindRadialLMSettings();
                    break;
            }
        }

        /// <summary>
        /// Hides the parameter groups.
        /// </summary>
        private void HideSettingsGroups()
        {
            grpSymmetricLayout.Visible = false;
            grpTableLayout.Visible = false;
            grpTreeLayout.Visible = false;
        }

        /// <summary>
        /// Fills the combo items list with enum names.
        /// </summary>
        /// <param name="combo">The combo.</param>
        /// <param name="enumType">Type of the enum.</param>
        private void FillCombo(ComboBox combo, Type enumType)
        {
            Array enums = Enum.GetValues(enumType);

            if (enums != null && enums.Length > 0)
            {
                foreach (object obj in enums)
                {
                    combo.Items.Add(obj);
                }

                combo.SelectedItem = combo.Items[0];
            }
        }

        /// <summary>
        /// Binds specified DataSource property to given control property.
        /// </summary>
        /// <param name="ctrlToBindTo">Control to bind to.</param>
        /// <param name="strControlPropertyName">Binding Control property name.</param>
        /// <param name="objDataSource">Binding DataSourse.</param>
        /// <param name="strDataSourcePropertyName">Binding DataSourse property name.</param>
        private void BindTo(Control ctrlToBindTo, string strControlPropertyName, object objDataSource, string strDataSourcePropertyName)
        {
            if (ctrlToBindTo.DataBindings[strControlPropertyName] != null)
                ctrlToBindTo.DataBindings.Remove(ctrlToBindTo.DataBindings[strControlPropertyName]);

            ctrlToBindTo.DataBindings.Add(strControlPropertyName, objDataSource, strDataSourcePropertyName);
        }

        /// <summary>
        /// Rebinds directed tree layout settings.
        /// </summary>
        private void RebindDirectdTreeLMSettings()
        {
            grpTreeLayout.Visible = true;
            grpTreeLayout.Text = "Directed Tree Layout";

            // rebind settings
            BindTo(txtDirectedHorizontalSpacing, "Text", m_directedLMSettings, "HorizontalSpacing");
            BindTo(txtDirectedVerticalSpacing, "Text", m_directedLMSettings, "VerticalSpacing");
            BindTo(txtRotationAngle, "Text", m_directedLMSettings, "RotationAngle");
        }

        /// <summary>
        /// Rebinds symmetric layout settings.
        /// </summary>
        private void RebindSymmetricLMSettings()
        {
            grpSymmetricLayout.Visible = true;

            // rebind settings
            BindTo(txtSymmetricForceSpringFactor, "Text", m_symmetricLMSettings, "SpringFactor");
            BindTo(txtSymmetricMaxIteraction, "Text", m_symmetricLMSettings, "MaxIteraction");
            BindTo(txtSymmetricMaxSpringLength, "Text", m_symmetricLMSettings, "Distance");
        }

        /// <summary>
        /// Rebinds table layout settings.
        /// </summary>
        private void RebindTableLMSettings()
        {
            grpTableLayout.Visible = true;

            // rebind settings
            BindTo(txtTableHorizontalSpacing, "Text", m_tableLMSettings, "HorizontalSpacing");
            BindTo(txtTableVerticalSpacing, "Text", m_tableLMSettings, "VerticalSpacing");
            BindTo(txtTableMaxColumnCount, "Text", m_tableLMSettings, "MaxColumnCount");
            BindTo(txtTableMaxRowsCount, "Text", m_tableLMSettings, "MaxRowsCount");
            BindTo(txtTableMaxWidth, "Text", m_tableLMSettings, "MaxWidth");
            BindTo(txtTableMaxHeight, "Text", m_tableLMSettings, "MaxHeight");

            BindTo(comboTableCellSizeMode, "SelectedItem", m_tableLMSettings, "CellSizeMode");
            comboTableCellSizeMode.SelectedItem = m_tableLMSettings.CellSizeMode;

            BindTo(comboTableExpandMode, "SelectedItem", m_tableLMSettings, "Orientation");
            comboTableExpandMode.SelectedItem = m_tableLMSettings.Orientation;
			BindTo(comboTableMeasureUnits, "SelectedItem", m_tableLMSettings, "MeasurementUnits");
            comboTableMeasureUnits.SelectedItem = m_tableLMSettings.MeasurementUnits;
			BindTo(txtTableLeftMargin, "Text", m_tableLMSettings, "LeftMargin");
            BindTo(txtTableTopMargin, "Text", m_tableLMSettings, "TopMargin");
        }

        /// <summary>
        /// Rebinds hierarchic layout settings.
        /// </summary>
        private void RebindHierarchicLMSettings()
        {
            grpTreeLayout.Visible = true;
            grpTreeLayout.Text = "Hierarchic Layout";

            // rebind settings
            BindTo(txtDirectedHorizontalSpacing, "Text", m_hierarchicLMSettings, "HorizontalSpacing");
            BindTo(txtDirectedVerticalSpacing, "Text", m_hierarchicLMSettings, "VerticalSpacing");
            BindTo(txtRotationAngle, "Text", m_hierarchicLMSettings, "RotationAngle");
        }

        /// <summary>
        /// Rebinds radial layout settings.
        /// </summary>
        private void RebindRadialLMSettings()
        {
            grpTreeLayout.Visible = true;
            grpTreeLayout.Text = "Radal layout";

            // rebind settings
            BindTo(txtDirectedHorizontalSpacing, "Text", m_radialLMSettings, "HorizontalSpacing");
            BindTo(txtDirectedVerticalSpacing, "Text", m_radialLMSettings, "VerticalSpacing");
            BindTo(txtRotationAngle, "Text", m_radialLMSettings, "RotationAngle");
        }

        /// <summary>
        /// Layouts the nodes.
        /// </summary>
        private void UpdateLayout()
        {
            LayoutManager lm = this.LayoutManager;
            GraphLayoutManager lmGraph = lm as GraphLayoutManager;

            if (lm != null)
            {
                m_model.HistoryManager.StartAtomicAction("LayoutNodes");

                // eneable size to content
                bool bSizeToContent = m_model.SizeToContent;
                m_model.SizeToContent = chkEnlargePage.Checked;

                // assing model
                lm.Model = m_model;

                // update nodes to lay out
                NodeCollection nodes = rdbSelection.Checked ? m_lstSelection : m_model.Nodes;
                lm.Nodes.Clear();
                lm.Nodes.AddRange(nodes);

                m_diagram.LayoutManager = lm;

                lm.UpdateLayout(m_lstSelection);

                // update selection list
                m_diagram.Controller.SelectionList.Clear();
                m_diagram.Controller.SelectionList.AddRange(lm.Nodes);

                // restore last state
                m_model.SizeToContent = bSizeToContent;
                m_model.HistoryManager.EndAtomicAction();
            }
        }

        /// <summary>
        /// Draws the directions arrows.
        /// </summary>
        private void DrawDirections()
        {
            int length = rbtnBottomLeft.Width;
            int width = (int)(length * 0.2);
            int height = (int)(length * 0.3);
            int arrowCount = 8;
            int angle = CommonUsedValues.CIRCLE;
            Image image;

            // save pathPoints
            PointF[] pts = new PointF[]
                {
                    new PointF( length / 2, length - height ),
                    new PointF( length / 2 - width, height ),
                    new PointF( length / 2 + width, height ),
                    new PointF( length / 2, length - height ),
                };

            // create arrow graphics path
            GraphicsPath gph = new GraphicsPath();
            gph.AddLines(pts);

            for (int i = 0; i < arrowCount; i++)
            {
                image = new Bitmap(length, length);

                using (Graphics gfx = Graphics.FromImage(image))
                {
                    Matrix mtxRotate = new Matrix();
                    mtxRotate.RotateAt(angle, new PointF(length / 2, length / 2));
                    gfx.MultiplyTransform(mtxRotate);
                    gfx.FillPath(Brushes.Black, gph);
                }

                m_directions[i].Image = image;
                angle -= 45;
            }
        }

        private bool IsBackOrDelete(KeyEventArgs e)
        {
            bool bBackDelete = false;

            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                bBackDelete = true;

            return bBackDelete;
        }

        /// <summary>
        /// Determines whether the specified textBox is handled.
        /// </summary>
        /// <param name="txtBox">The textBox.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
        /// <param name="bOnlyNumbers">if set to <c>true</c>check only for numbers.</param>
        /// <param name="nMaxAllowedDigits">The max allowed digits.</param>
        /// <returns>
        /// <c>true</c> if the specified textBox is handled; otherwise, <c>false</c>.
        /// </returns>
        private bool IsHandled(TextBox txtBox, KeyPressEventArgs e, bool bOnlyNumbers, int nMaxAllowedDigits)
        {
            bool bHandled = false;

            if (m_bBackDelete)
            {
                bHandled = true;
            }
            else
            {
                if (txtBox.Text.Length == nMaxAllowedDigits)
                {
                    // if there is selected text -- allow enter numbers
                    // consider comma also
                    if (txtBox.SelectionLength > 0)
                    {
                        if (!bOnlyNumbers)
                        {
                            Match match = Regex.Match(txtBox.Text, c_srtDECIMAL_SEPARATOR);

                            if (match.Success)
                            {
                                Match matchCommaInSelected = Regex.Match(txtBox.SelectedText, c_srtDECIMAL_SEPARATOR);

                                if (matchCommaInSelected.Success)
                                {
                                    bHandled = true;
                                }
                                else
                                {
                                    if (Char.IsDigit(e.KeyChar))
                                        bHandled = true;
                                    else
                                        bHandled = false;
                                }
                            }
                            else
                                bHandled = true;
                        }
                        else
                        {
                            if (Char.IsDigit(e.KeyChar))
                                bHandled = true;
                        }
                    }
                    else
                        bHandled = false;
                }
                else
                {
                    if (Char.IsDigit(e.KeyChar))
                    {
                        bHandled = true;
                    }
                    else if (!bOnlyNumbers)
                    {
                        // If comma is already present -- no more needed
                        Match match = Regex.Match(txtBox.Text, "[" + c_srtDECIMAL_SEPARATOR + "]");

                        if (match.Success)
                        {
                            if (txtBox.SelectionLength > 0)
                            {
                                Match matchCommaInSelected = Regex.Match(txtBox.SelectedText, "[" + c_srtDECIMAL_SEPARATOR + "]");

                                if (matchCommaInSelected.Success)
                                    bHandled = true;
                                else
                                    bHandled = false;
                            }
                            else
                                bHandled = false;
                        }
                        else
                        {
                            if (e.KeyChar == c_cDECIMAL_SEPARATOR)
                                bHandled = true;
                            else
                                bHandled = false;
                        }
                    }
                }
            }

            return bHandled;
        }

        #region Preview
        /// <summary>
        /// Updates the preview.
        /// </summary>
        private void UpdatePreview()
        {
            Image img = pcPreview.Image;

            if (img == null)
            {
                img = new Bitmap(pcPreview.Width, pcPreview.Height);
            }

            using (Graphics gfx = Graphics.FromImage(img))
            {
                gfx.Clear(Color.White);

                // draw preview diagram
                LayoutStyle style = (LayoutStyle)comboStyle.SelectedItem;

                switch (style)
                {
                    case LayoutStyle.DirectedTree:
                        DrawDirectedTreeLayout(gfx, pcPreview.Size);
                        break;
                    case LayoutStyle.Hierarchic:
                        DrawHierarchicLayout(gfx, pcPreview.Size);
                        break;
                    case LayoutStyle.Symmetric:
                        DrawSymmetricLayout(gfx, pcPreview.Size);
                        break;
                    case LayoutStyle.Table:
                        DrawTableLayout(gfx, pcPreview.Size);
                        break;
                    case LayoutStyle.Radial:
                        DrawRadialLayout(gfx, pcPreview.Size);
                        break;
                }
            }

            pcPreview.Image = img;
        }

        private void DrawDirectedTreeLayout(Graphics gfx, Size szSize)
        {
            int ndWidth = c_nNODE_SIZE;
            int frWidth = (int)(ndWidth * 1.2);
            int scWidth = (int)(ndWidth * 0.8);
            int height = (int)(ndWidth * 1.6);
            float leftMargin = (szSize.Width - (scWidth + 2 * frWidth + ndWidth)) / 2;
            float upMargin = (szSize.Height - (2 * height + ndWidth)) / 2;

            PointF[] pts = new PointF[]
                {
                    new PointF( scWidth + frWidth, 0 ),
                    new PointF( scWidth, height ),
                    new PointF( scWidth + 2 * frWidth, height ),
                    new PointF( 0, 2 * height ),
                    new PointF( 2 * scWidth, 2 * height ),
                    new PointF( scWidth + 2 * frWidth, 2 * height )
                 };

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                pts[i] = new PointF(pts[i].X + leftMargin, pts[i].Y + upMargin);
            }

            DrawConnector(gfx, pts[0], pts[1]);
            DrawConnector(gfx, pts[0], pts[2]);
            DrawConnector(gfx, pts[1], pts[3]);
            DrawConnector(gfx, pts[1], pts[4]);
            DrawConnector(gfx, pts[2], pts[5]);

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                DrawNode(gfx, pts[i]);
            }
        }
        private void DrawHierarchicLayout(Graphics gfx, Size szSize)
        {
            int ndWidth = c_nNODE_SIZE;
            int frWidth = (int)(ndWidth * 0.8);
            int height = (int)(ndWidth * 1.8);
            float leftMargin = (szSize.Width - (3 * frWidth + 2 * ndWidth)) / 2;
            float upMargin = (szSize.Height - (2 * height + ndWidth)) / 2;

            PointF[] pts = new PointF[]
                {
                    new PointF( frWidth, 0 ),
                    new PointF( 0, height ),
                    new PointF( 2 * frWidth, height ),
                    new PointF( 3 * frWidth + ndWidth, height ),
                    new PointF( frWidth, 2 * height ),
                    new PointF( 3 * frWidth + ndWidth, 2 * height ),
                };

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                pts[i] = new PointF(pts[i].X + leftMargin, pts[i].Y + upMargin);
            }

            DrawConnector(gfx, pts[0], pts[1]);
            DrawConnector(gfx, pts[0], pts[2]);
            DrawConnector(gfx, pts[1], pts[4]);
            DrawConnector(gfx, pts[2], pts[4]);
            DrawConnector(gfx, pts[2], pts[5]);
            DrawConnector(gfx, pts[3], pts[5]);

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                DrawNode(gfx, pts[i]);
            }
        }
        private void DrawSymmetricLayout(Graphics gfx, Size szSize)
        {
            int ndWidth = c_nNODE_SIZE;
            int frWidth = (int)(ndWidth * 1.1);
            int height = frWidth;
            float leftMargin = (szSize.Width - (3 * frWidth + ndWidth)) / 2;
            float upMargin = (szSize.Height - (3 * height + ndWidth)) / 2;

            PointF[] pts = new PointF[]
                {
                    new PointF( 3 * frWidth, 0 ),
                    new PointF( frWidth, height ),
                    new PointF( 2 * frWidth, height ),
                    new PointF( frWidth, 2 * height ),
                    new PointF( 2 * frWidth, 2 * height ),
                    new PointF( 0, 3 * height )
                };

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                pts[i] = new PointF(pts[i].X + leftMargin, pts[i].Y + upMargin);
            }

            DrawConnector(gfx, pts[0], pts[1]);
            DrawConnector(gfx, pts[2], pts[3]);
            DrawConnector(gfx, pts[1], pts[3]);
            DrawConnector(gfx, pts[5], pts[3]);

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                DrawNode(gfx, pts[i]);
            }
        }
        private void DrawTableLayout(Graphics gfx, Size szSize)
        {
            int ndWidth = c_nNODE_SIZE;
            int frWidth = (int)(ndWidth * 0.3);
            int height = (int)(ndWidth * 0.2);
            int widthCount = 3;
            int heightCount = 4;
            int count = 10;
            float leftMargin = (szSize.Width - (widthCount * (ndWidth + frWidth) - frWidth)) / 2;
            float upMargin = (szSize.Height - (heightCount * (ndWidth + height) - height)) / 2;
            PointF ptLocation = new PointF(leftMargin, upMargin);

            for (int j = 0; j < heightCount; j++)
            {
                if (count <= 0)
                    break;

                ptLocation.X = leftMargin;

                for (int i = 0; i < widthCount; i++)
                {
                    count--;

                    DrawNode(gfx, ptLocation);
                    ptLocation.X += ndWidth + frWidth;

                    if (count <= 0)
                        break;
                }

                ptLocation.Y += ndWidth + height;
            }
        }
        private void DrawRadialLayout(Graphics gfx, Size szSize)
        {
            int ndWidth = c_nNODE_SIZE;
            int frWidth = (int)(ndWidth * 1.6);
            int height = frWidth;
            float leftMargin = (szSize.Width - (2 * frWidth + ndWidth)) / 2;
            float upMargin = (szSize.Height - (2 * height + ndWidth)) / 2;

            PointF[] pts = new PointF[]
                {
                    new PointF( frWidth, 0 ),
                    new PointF( 0, height ),
                    new PointF( frWidth, height ),
                    new PointF( 2 * frWidth, height ),
                    new PointF( frWidth, 2 * height ),
                };

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                pts[i] = new PointF(pts[i].X + leftMargin, pts[i].Y + upMargin);
            }

            DrawConnector(gfx, pts[0], pts[2]);
            DrawConnector(gfx, pts[1], pts[0]);
            DrawConnector(gfx, pts[2], pts[1]);
            DrawConnector(gfx, pts[2], pts[3]);
            DrawConnector(gfx, pts[2], pts[4]);

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                DrawNode(gfx, pts[i]);
            }
        }
        private void DrawConnector(Graphics gfx, PointF ptStart, PointF ptEnd)
        {
            ptStart.X += c_nNODE_SIZE / 2;
            ptStart.Y += c_nNODE_SIZE / 2;
            ptEnd.X += c_nNODE_SIZE / 2;
            ptEnd.Y += c_nNODE_SIZE / 2;

            PointF[] pts = new PointF[]
                {
                    new PointF( ptStart.X, ptStart.Y ),
                    new PointF( ptStart.X, ptEnd.Y ),
                    new PointF( ptEnd.X, ptEnd.Y )
                };

            using (Pen line = new Pen(Color.Black))
            {
                gfx.DrawLines(line, pts);
            }
        }
        private void DrawNode(Graphics gfx, PointF ptLocation)
        {
            SizeF szSize = new SizeF(c_nNODE_SIZE, c_nNODE_SIZE);

            using (Brush brush = new SolidBrush(Color.White))
            using (Pen outline = new Pen(Color.Black))
            {
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(
                    Geometry.ConvertPoint(ptLocation), Geometry.ConvertSize(szSize));
                gfx.FillRectangle(brush, rc);
                gfx.DrawRectangle(outline, rc);
            }
        }
        #endregion

        #endregion
    }

    #region Parameter structures
    /// <summary>
    /// Symmetric Layout Manager settings.
    /// </summary>
    internal class SymmetricLMSettings
    {
        #region Fields
        private float m_fDistance = 100;
        private double m_dSpringFactor = 0.442;
        private int m_nMaxIteraction = 500;
        #endregion

        #region Properties
        public float Distance
        {
            get { return m_fDistance; }
            set { m_fDistance = value; }
        }
        public double SpringFactor
        {
            get { return m_dSpringFactor; }
            set { m_dSpringFactor = value; }
        }
        public int MaxIteraction
        {
            get { return m_nMaxIteraction; }
            set { m_nMaxIteraction = value; }
        }
        #endregion
    }

    /// <summary>
    /// Graph Layout Manager settings.
    /// </summary>
    internal class GraphLMSettings
    {
        #region Fields
        private int m_fRotationAnge;
        private float m_fHorizontalSpacing = 20;
        private float m_fVerticalSpacing = 20;
        #endregion

        #region Proeprties
        public int RotationAngle
        {
            get { return m_fRotationAnge; }
            set { m_fRotationAnge = value; }
        }
        public float HorizontalSpacing
        {
            get { return m_fHorizontalSpacing; }
            set { m_fHorizontalSpacing = value; }
        }
        public float VerticalSpacing
        {
            get { return m_fVerticalSpacing; }
            set { m_fVerticalSpacing = value; }
        }
        #endregion
    }

    /// <summary>
    /// Table Layout Manager settings.
    /// </summary>
    internal class TableLMSettings
    {
        #region Fields
        private int m_nMaxColumnCount = 4;
        private int m_nMaxRowsCount = 4;
        private float m_fHorizontalSpacing = 20;
        private float m_fVerticalSpacing = 20;
        private float m_fMaxWidth = 400;
        private float m_fMaxHeight = 400;
		private float m_fLeftMargin = 10;
        private float m_fTopMargin = 10;
        private MeasureUnits m_measureUnit = MeasureUnits.Pixel;
        private Orientation m_orientation;
        private CellSizeMode m_cellSizeMode;
        #endregion

        #region Properties
		public MeasureUnits MeasurementUnits
        {
            get { return m_measureUnit; }
            set { m_measureUnit = value; }
        }
		public float LeftMargin
        {
            get { return m_fLeftMargin; }
            set { m_fLeftMargin = value; }
        }
        public float TopMargin
        {
            get { return m_fTopMargin;}
            set { m_fTopMargin = value; }
        }
        public int MaxColumnCount
        {
            get { return m_nMaxColumnCount; }
            set { m_nMaxColumnCount = value; }
        }
        public int MaxRowsCount
        {
            get { return m_nMaxRowsCount; }
            set { m_nMaxRowsCount = value; }
        }
        public float HorizontalSpacing
        {
            get { return m_fHorizontalSpacing; }
            set { m_fHorizontalSpacing = value; }
        }
        public float VerticalSpacing
        {
            get { return m_fVerticalSpacing; }
            set { m_fVerticalSpacing = value; }
        }
        public float MaxWidth
        {
            get { return m_fMaxWidth; }
            set { m_fMaxWidth = value; }
        }
        public float MaxHeight
        {
            get { return m_fMaxHeight; }
            set { m_fMaxHeight = value; }
        }
        public Orientation Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }
        public CellSizeMode CellSizeMode
        {
            get { return m_cellSizeMode; }
            set { m_cellSizeMode = value; }
        }
        #endregion
    }
    #endregion
}
