#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the dialog to change the toolbar properties.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    internal class ToolBarPropertyForm : System.Windows.Forms.Form
    {
        #region Initialize commponents and constructor
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Button bttnCancel;
        private System.Windows.Forms.Button bttnOK;
        private System.Windows.Forms.Button bttnApply;
        private System.Windows.Forms.PropertyGrid prptgrPropertys;

        public ToolBarPropertyForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.bttnCancel = new System.Windows.Forms.Button();
            this.bttnOK = new System.Windows.Forms.Button();
            this.prptgrPropertys = new System.Windows.Forms.PropertyGrid();
            this.bttnApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bttnCancel
            // 
            this.bttnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bttnCancel.Location = new System.Drawing.Point(168, 304);
            this.bttnCancel.Name = "bttnCancel";
            this.bttnCancel.Size = new System.Drawing.Size(75, 24);
            this.bttnCancel.TabIndex = 10;
            this.bttnCancel.Text = "Cancel";
            this.bttnCancel.Click += new System.EventHandler(this.bttnCancel_Click);
            // 
            // bttnOK
            // 
            this.bttnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bttnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bttnOK.Location = new System.Drawing.Point(88, 304);
            this.bttnOK.Name = "bttnOK";
            this.bttnOK.Size = new System.Drawing.Size(75, 24);
            this.bttnOK.TabIndex = 9;
            this.bttnOK.Text = "OK";
            this.bttnOK.Click += new System.EventHandler(this.bttnOK_Click);
            // 
            // prptgrPropertys
            // 
            this.prptgrPropertys.CommandsVisibleIfAvailable = true;
            this.prptgrPropertys.LargeButtons = false;
            this.prptgrPropertys.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.prptgrPropertys.Location = new System.Drawing.Point(8, 7);
            this.prptgrPropertys.Name = "prptgrPropertys";
            this.prptgrPropertys.Size = new System.Drawing.Size(312, 288);
            this.prptgrPropertys.TabIndex = 8;
            this.prptgrPropertys.Text = "propertyGrid1";
            this.prptgrPropertys.ToolbarVisible = false;
            this.prptgrPropertys.ViewBackColor = System.Drawing.SystemColors.Window;
            this.prptgrPropertys.ViewForeColor = System.Drawing.SystemColors.WindowText;
            // 
            // bttnApply
            // 
            this.bttnApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bttnApply.Location = new System.Drawing.Point(248, 304);
            this.bttnApply.Name = "bttnApply";
            this.bttnApply.TabIndex = 11;
            this.bttnApply.Text = "Apply";
            this.bttnApply.Click += new System.EventHandler(this.bttnApply_Click);
            // 
            // ToolBarPropertyForm
            // 
            this.AcceptButton = this.bttnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.bttnCancel;
            this.ClientSize = new System.Drawing.Size(328, 334);
            this.Controls.Add(this.bttnApply);
            this.Controls.Add(this.bttnCancel);
            this.Controls.Add(this.bttnOK);
            this.Controls.Add(this.prptgrPropertys);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolBarPropertyForm";
            this.Text = "ToolBar Properties";
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region ToolBar properties
        /// <summary>
        /// Represents the toolbar properties to edit.
        /// </summary>
        private class ToolBarProperty
        {
            #region Members
            private Point m_location;
            private Size m_buttonSize;
            private ChartOrientation m_orientation;
            private int m_spacing;
            private int m_padding;
            private bool m_bAutoSize;
            private Size m_size;
            private Color m_backColor;
            private Color m_buttonBackColor;
            private Color m_buttonForeColor;
            private int m_header;
            private LineInfo m_border = new LineInfo();
            private bool m_bShowBorder;
            private ChartDock m_position;
            private ChartAlignment m_alignment;
            private bool m_dockingFree;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets a Location property.
            /// </summary>
            [Description("Indicates the location of toolbar.")]
            public Point Location
            {
                get
                {
                    return m_location;
                }

                set
                {
                    m_location = value;
                }
            }

            /// <summary>
            /// Gets or sets a ButtonSize property.
            /// </summary>
            [Category("Buttons"), Description("Indicates the size of buttons.")]
            public Size ButtonSize
            {
                get
                {
                    return m_buttonSize;
                }

                set
                {
                    m_buttonSize = value;
                }
            }

            /// <summary>
            /// Gets or sets a Orientation property.
            /// </summary>
            [DefaultValue(ChartOrientation.Horizontal), Description("Indicates the orientation of toolbar.")]
            public ChartOrientation Orientation
            {
                get
                {
                    return m_orientation;
                }

                set
                {
                    m_orientation = value;
                }
            }

            /// <summary>
            /// Gets or sets a Spacing property.
            /// </summary>
            [DefaultValue(0),
      Description("Indicates the spasing between buttons.")]
            public int Spacing
            {
                get
                {
                    return m_spacing;
                }

                set
                {
                    m_spacing = value;
                }
            }

            /// <summary>
            /// Gets or sets a Padding property.
            /// </summary>
            [DefaultValue(2),
            Description("Indicates the spasing between buttons and border.")]
            public int Padding
            {
                get
                {
                    return m_padding;
                }

                set
                {
                    m_padding = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether auto size is enable or not.
            /// </summary>
            /// <value><c>true</c> if [auto size]; otherwise, <c>false</c>.</value>
            [DefaultValue(true), Description("Indicates whether the toolbar will automatically size itself.")]
            public bool AutoSize
            {
                get
                {
                    return m_bAutoSize;
                }

                set
                {
                    m_bAutoSize = value;
                }
            }

            /// <summary>
            /// Gets or sets a Size property.
            /// </summary>
            [Description("Indicates the size of toolbar")]
            public Size Size
            {
                get
                {
                    return m_size;
                }

                set
                {
                    if (!m_bAutoSize)
                    {
                        m_size = value;
                    }
                }
            }

            /// <summary>
            /// Gets or sets a BackColor property.
            /// </summary>
            [Description("Indicates the back color of toolbar")]
            public Color BackColor
            {
                get
                {
                    return m_backColor;
                }

                set
                {
                    m_backColor = value;
                }
            }

            /// <summary>
            /// Gets or sets a ButtonBackColor property.
            /// </summary>
            [Category("Buttons")]
            public Color ButtonBackColor
            {
                get
                {
                    return m_buttonBackColor;
                }

                set
                {
                    m_buttonBackColor = value;
                }
            }

            /// <summary>
            /// Gets or sets a ButtonForeColor property.
            /// </summary>
            [Category("Buttons")]
            public Color ButtonForeColor
            {
                get
                {
                    return m_buttonForeColor;
                }

                set
                {
                    m_buttonForeColor = value;
                }
            }

            /// <summary>
            /// Gets or sets a Header property.
            /// </summary>
            [DefaultValue(0),
      Description("Indicates the height of header on toolbar.")]
            public int Header
            {
                get
                {
                    return m_header;
                }

                set
                {
                    m_header = value;
                }
            }

            /// <summary>
            /// Gets the Border property.
            /// </summary>
            [Description("Indicates the style of border.")]
            public LineInfo Border
            {
                get
                {
                    return m_border;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether show the border or not.
            /// </summary>
            /// <value><c>true</c> if [show border]; otherwise, <c>false</c>.</value>
            [DefaultValue(true), Description("Indicates the visible of border.")]
            public bool ShowBorder
            {
                get
                {
                    return m_bShowBorder;
                }

                set
                {
                    m_bShowBorder = value;
                }
            }

            /// <summary>
            /// Gets or sets a Position property.
            /// </summary>
            [DefaultValue(ChartDock.Top)]
            [Description("Indicates the dock position of toolbar.")]
            public ChartDock Position
            {
                get
                {
                    return m_position;
                }

                set
                {
                    m_position = value;
                }
            }

            /// <summary>
            /// Gets or sets a Alignment property.
            /// </summary>
            [DefaultValue(ChartAlignment.Center)]
            [Description("Indicates the alignment of toolbar.")]
            public ChartAlignment Alignment
            {
                get
                {
                    return m_alignment;
                }

                set
                {
                    m_alignment = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether docking free is enable or not.
            /// </summary>
            /// <value><c>true</c> if [docking free]; otherwise, <c>false</c>.</value>
            [DefaultValue(false),
            Description("Indicates if the control should be docked inside the Chart")]
            public bool DockingFree
            {
                get
                {
                    return m_dockingFree;
                }

                set
                {
                    m_dockingFree = value;
                }
            }
            #endregion

            #region Public methods
            /// <summary>
            /// Sets the toolbar properties.
            /// </summary>
            /// <param name="info">The <see cref="ChartToolBarInfo"/>.</param>
            public void SetInfo(ChartToolBarInfo info)
            {
                m_bAutoSize = info.AutoSize;
                m_bShowBorder = info.ShowBorder;
                m_buttonSize = info.ButtonSize;
                m_backColor = info.BackColor;
                m_header = info.Header;
                m_location = info.Location;
                m_orientation = info.Orientation;
                m_size = info.Size;
                m_spacing = info.Spacing;
                m_position = info.Position;
                m_alignment = info.Alignment;
                m_dockingFree = info.DockingFree;
                m_buttonBackColor = info.ButtonBackColor;
                m_padding = info.Padding;
                m_buttonForeColor = info.ButtonForeColor;
                m_border.BackColor = info.Border.BackColor;
                m_border.DashStyle = info.Border.DashStyle;
                m_border.ForeColor = info.Border.ForeColor;
                m_border.PenType = info.Border.PenType;
                m_border.Width = info.Border.Width;
            }

            /// <summary>
            /// Gets the toolbar properites.
            /// </summary>
            /// <param name="info">The <see cref="ChartToolBarInfo"/>.</param>
            public void GetInfo(ChartToolBarInfo info)
            {
                info.AutoSize = m_bAutoSize;
                info.ShowBorder = m_bShowBorder;
                info.ButtonSize = m_buttonSize;
                info.BackColor = m_backColor;
                info.Header = m_header;
                info.Location = m_location;
                info.Orientation = m_orientation;
                info.Size = m_size;
                info.Spacing = m_spacing;
                info.ButtonBackColor = m_buttonBackColor;
                info.Padding = m_padding;
                info.ButtonForeColor = m_buttonForeColor;
                info.Border.BackColor = m_border.BackColor;
                info.Border.DashStyle = m_border.DashStyle;
                info.Border.ForeColor = m_border.ForeColor;
                info.Border.PenType = m_border.PenType;
                info.Border.Width = m_border.Width;
                info.Position = m_position;
                info.Alignment = m_alignment;
                info.DockingFree = m_dockingFree;

                SetInfo(info);
            }
            #endregion
        }
        #endregion

        #region Members
        private ChartToolBarInfo m_Info;
        private ToolBarProperty m_UndoProperty = new ToolBarProperty();
        private ToolBarProperty m_property = new ToolBarProperty();
        private bool m_applyIsClick = false;
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the toolbar properties.
        /// </summary>
        /// <param name="info">The info.</param>
        public void SetInfo(ChartToolBarInfo info)
        {
            m_Info = info;
            m_property.SetInfo(info);
            m_UndoProperty.SetInfo(info);

            prptgrPropertys.SelectedObject = m_property;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Handles the Click event of the bttnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bttnOK_Click(object sender, System.EventArgs e)
        {
            m_property.GetInfo(m_Info);
            Hide();
        }

        /// <summary>
        /// Handles the Click event of the bttnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bttnCancel_Click(object sender, System.EventArgs e)
        {
            if (m_applyIsClick)
            {
                m_UndoProperty.GetInfo(m_Info);
            }

            Hide();
        }

        /// <summary>
        /// Handles the Click event of the bttnApply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bttnApply_Click(object sender, System.EventArgs e)
        {
            m_property.GetInfo(m_Info);
            m_applyIsClick = true;
        }
        #endregion
    }
}
