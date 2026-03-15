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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the dialog to change the legend properties.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ChartLegendPropertiesDialog : System.Windows.Forms.Form
    {
        #region Initialize commponents and constructor
        private System.Windows.Forms.Button bttnCancel;
        private System.Windows.Forms.Button bttnOK;
        private System.Windows.Forms.PropertyGrid prptgrPropertys;
        private System.Windows.Forms.Button bttnApply;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegendPropertiesDialog"/> class.
        /// </summary>
        public ChartLegendPropertiesDialog()
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
        /// Overridden. Cleans up any resources being used.
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
            this.prptgrPropertys = new System.Windows.Forms.PropertyGrid();
            this.bttnCancel = new System.Windows.Forms.Button();
            this.bttnOK = new System.Windows.Forms.Button();
            this.bttnApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // prptgrPropertys
            // 
            this.prptgrPropertys.CommandsVisibleIfAvailable = true;
            this.prptgrPropertys.LargeButtons = false;
            this.prptgrPropertys.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.prptgrPropertys.Location = new System.Drawing.Point(8, 8);
            this.prptgrPropertys.Name = "prptgrPropertys";
            this.prptgrPropertys.Size = new System.Drawing.Size(312, 288);
            this.prptgrPropertys.TabIndex = 0;
            this.prptgrPropertys.Text = "propertyGrid1";
            this.prptgrPropertys.ToolbarVisible = false;
            this.prptgrPropertys.ViewBackColor = System.Drawing.SystemColors.Window;
            this.prptgrPropertys.ViewForeColor = System.Drawing.SystemColors.WindowText;
            // 
            // bttnCancel
            // 
            this.bttnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bttnCancel.Location = new System.Drawing.Point(165, 304);
            this.bttnCancel.Name = "bttnCancel";
            this.bttnCancel.Size = new System.Drawing.Size(75, 24);
            this.bttnCancel.TabIndex = 7;
            this.bttnCancel.Text = "Cancel";
            // 
            // bttnOK
            // 
            this.bttnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bttnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bttnOK.Location = new System.Drawing.Point(85, 304);
            this.bttnOK.Name = "bttnOK";
            this.bttnOK.Size = new System.Drawing.Size(75, 24);
            this.bttnOK.TabIndex = 6;
            this.bttnOK.Text = "OK";
            this.bttnOK.Click += new System.EventHandler(this.bttnOK_Click);
            // 
            // bttnApply
            // 
            this.bttnApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bttnApply.Location = new System.Drawing.Point(245, 304);
            this.bttnApply.Name = "bttnApply";
            this.bttnApply.TabIndex = 8;
            this.bttnApply.Text = "Apply";
            this.bttnApply.Click += new System.EventHandler(this.bttnApply_Click);
            // 
            // LegendPropertyForm
            // 
            this.AcceptButton = this.bttnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.bttnCancel;
            this.ClientSize = new System.Drawing.Size(328, 336);
            this.Controls.Add(this.bttnApply);
            this.Controls.Add(this.bttnCancel);
            this.Controls.Add(this.bttnOK);
            this.Controls.Add(this.prptgrPropertys);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LegendPropertyForm";
            this.Text = "Legend properties ";
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region LegendProperty

        /// <summary>
        /// The LegendProperties Class.
        /// </summary>
        private class LegendProperties
        {
            #region Members
            private Size itemsSize;
            private LineInfo border = new LineInfo();
            private Color backColor;
            private bool floatingAutoSize;
            private bool showSymbol;
            private bool showBorder;
            private ChartDock position;
            private ChartOrientation orientation;
            private ChartAlignment legendAlignment;
            private StringAlignment itemsAlignment;
            private VerticalAlignment itemsTextAlignment;
            private int rows;
            private int columns;
            private bool visibleCheckBox;
            private bool onlyColumnsForFloating;
            private bool setDefSizeForCustom;
            private bool showItemsShadow;
            private Size itemsShadowOffset;
            private Color itemsShadowColor;
            private string m_text;
            private StringAlignment m_textAlignment;
            private Color m_textColor;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets a value indicating whether the visibility of the legend border.
            /// </summary>
            [Category("Border"), Description("Determines the visibility of the legend's border.")]
            public bool ShowBorder
            {
                get
                {
                    return showBorder;
                }

                set
                {
                    showBorder = value;
                }
            }

            /// <summary>
            /// Gets the line style of the border used by the legend.
            /// </summary>
            [Category("Border"), Description("The line style of border of legend.")]
            public LineInfo Border
            {
                get
                {
                    return border;
                }
            }

            /// <summary>
            /// Gets or sets the background color of the legend.
            /// </summary>
            [Description("The background color of legend.")]
            public Color BackColor
            {
                get
                {
                    return backColor;
                }

                set
                {
                    backColor = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the legend will automatically position and size itself when set as floating.
            /// </summary>
            [Description("Indicates whether the legend will automatically position and size itself when floating.")]
            public bool FloatingAutoSize
            {
                get
                {
                    return floatingAutoSize;
                }

                set
                {
                    floatingAutoSize = value;
                }
            }

            /// <summary>
            /// Gets or sets a value that indicates the visibility of the series symbol.
            /// </summary>
            [Description("Indicates if the series symbol is visible.")]
            public bool ShowSymbol
            {
                get
                {
                    return showSymbol;
                }

                set
                {
                    showSymbol = value;
                }
            }

            /// <summary>
            /// Gets or sets the position of the legend.
            /// </summary>
            [Description("Indicates the position of the legend.")]
            public ChartDock Position
            {
                get
                {
                    return position;
                }

                set
                {
                    position = value;
                }
            }

            /// <summary>
            /// Gets or sets the orientation of items on the legend.
            /// </summary>
            [Description("Indicates the orientation of items on the legend.")]
            public ChartOrientation Orientation
            {
                get
                {
                    return orientation;
                }

                set
                {
                    orientation = value;
                }
            }

            /// <summary>
            /// Gets or sets the alignment of legend.
            /// </summary>
            [Description("Indicates the Alignment of legend.")]
            public ChartAlignment LegendAlignment
            {
                get
                {
                    return legendAlignment;
                }

                set
                {
                    legendAlignment = value;
                }
            }

            /// <summary>
            /// Gets or sets the alignment of items.
            /// </summary>
            [Description("Indicates the Alignment of items.")]
            public StringAlignment ItemsAlignment
            {
                get
                {
                    return itemsAlignment;
                }

                set
                {
                    itemsAlignment = value;
                }
            }

            /// <summary>
            /// Gets or sets the default size of items.
            /// </summary>
            [Description("Indicates the default size of items.")]
            public Size ItemsSize
            {
                get
                {
                    return itemsSize;
                }

                set
                {
                    itemsSize = value;
                }
            }

            /// <summary>
            /// Gets or sets the number of rows in a legend. 
            /// </summary>
            [Description("Indicates the rows of Legend."), DefaultValue(1)]
            public int RowsCount
            {
                get
                {
                    return rows;
                }

                set
                {
                    rows = value;
                }
            }

            /// <summary>
            /// Gets or sets the number of columns in a legend.
            /// </summary>
            [Description("Indicates the columns of Legend."), DefaultValue(1)]
            public int ColumnsCount
            {
                get
                {
                    return columns;
                }

                set
                {
                    columns = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [visible check box].
            /// </summary>
            /// <value><c>true</c> if [visible check box]; otherwise, <c>false</c>.</value>
            public bool VisibleCheckBox
            {
                get
                {
                    return visibleCheckBox;
                }

                set
                {
                    visibleCheckBox = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [only columns for floating].
            /// </summary>
            /// <value>
            ///     <c>true</c> if [only columns for floating]; otherwise, <c>false</c>.
            /// </value>
            public bool OnlyColumnsForFloating
            {
                get
                {
                    return onlyColumnsForFloating;
                }

                set
                {
                    onlyColumnsForFloating = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [set def size for custom].
            /// </summary>
            /// <value>
            ///    <c>true</c> if [set def size for custom]; otherwise, <c>false</c>.
            /// </value>
            public bool SetDefSizeForCustom
            {
                get
                {
                    return setDefSizeForCustom;
                }

                set
                {
                    setDefSizeForCustom = value;
                }
            }

            /// <summary>
            /// Gets or sets the items text aligment.
            /// </summary>
            /// <value>The items text aligment.</value>
            public VerticalAlignment ItemsTextAligment
            {
                get
                {
                    return itemsTextAlignment;
                }

                set
                {
                    itemsTextAlignment = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [show items shadow].
            /// </summary>
            /// <value><c>true</c> if [show items shadow]; otherwise, <c>false</c>.</value>
            public bool ShowItemsShadow
            {
                get
                {
                    return showItemsShadow;
                }

                set
                {
                    showItemsShadow = value;
                }
            }

            /// <summary>
            /// Gets or sets the items shadow offset.
            /// </summary>
            /// <value>The items shadow offset.</value>
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
            public Size ItemsShadowOffset
            {
                get
                {
                    return itemsShadowOffset;
                }

                set
                {
                    itemsShadowOffset = value;
                }
            }

            /// <summary>
            /// Gets or sets the color of the items shadow.
            /// </summary>
            /// <value>The color of the items shadow.</value>
            public Color ItemsShadowColor
            {
                get
                {
                    return itemsShadowColor;
                }

                set
                {
                    itemsShadowColor = value;
                }
            }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            /// <value>The text.</value>
            [Category("Title"), Description("Gets or sets the text of title.")]
            public string Text
            {
                get
                {
                    return m_text;
                }

                set
                {
                    m_text = value;
                }
            }

            /// <summary>
            /// Gets or sets the text alignment.
            /// </summary>
            /// <value>The text alignment.</value>
            [Category("Title"), Description("Determines the alignment of the legend's title.")]
            public StringAlignment TextAlignment
            {
                get
                {
                    return m_textAlignment;
                }

                set
                {
                    m_textAlignment = value;
                }
            }

            /// <summary>
            /// Gets or sets the color of the text.
            /// </summary>
            /// <value>The color of the text.</value>
            [Category("Title"), Description("Gets or sets the color of title.")]
            public Color TextColor
            {
                get
                {
                    return m_textColor;
                }

                set
                {
                    m_textColor = value;
                }
            }
            #endregion

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="LegendProperties"/> class.
            /// </summary>
            public LegendProperties()
            {
            }
            #endregion

            #region Methods

            /// <summary>
            /// Sets the info.
            /// </summary>
            /// <param name="info">The info.</param>
            public void SetInfo(ChartLegend info)
            {
                border.BackColor = info.Border.BackColor;
                border.DashStyle = info.Border.DashStyle;
                border.ForeColor = info.Border.ForeColor;
                border.PenType = info.Border.PenType;
                border.Width = info.Border.Width;
                backColor = info.BackColor;
                floatingAutoSize = info.FloatingAutoSize;
                orientation = info.Orientation;
                position = info.Position;
                showBorder = info.ShowBorder;
                showSymbol = info.ShowSymbol;
                legendAlignment = info.Alignment;
                itemsAlignment = info.ItemsAlignment;
                rows = info.RowsCount;
                columns = info.ColumnsCount;
                visibleCheckBox = info.VisibleCheckBox;
                itemsSize = info.ItemsSize;
                onlyColumnsForFloating = info.OnlyColumnsForFloating;
                itemsTextAlignment = info.ItemsTextAligment;
                itemsShadowColor = info.ItemsShadowColor;
                itemsShadowOffset = info.ItemsShadowOffset;
                m_textAlignment = info.TextAlignment;
                m_textColor = info.ForeColor;
                m_text = info.Text;
            }

            /// <summary>
            /// Outs the info.
            /// </summary>
            /// <param name="info">The info.</param>
            public void OutInfo(ChartLegend info)
            {
                info.BackColor = BackColor;
                info.FloatingAutoSize = FloatingAutoSize;
                info.Orientation = Orientation;
                info.Position = Position;
                info.ShowBorder = ShowBorder;
                info.ShowSymbol = ShowSymbol;
                info.Border.BackColor = Border.BackColor;
                info.Border.DashStyle = Border.DashStyle;
                info.Border.ForeColor = Border.ForeColor;
                info.Border.PenType = Border.PenType;
                info.Border.Width = Border.Width;
                info.Alignment = legendAlignment;
                info.ItemsAlignment = itemsAlignment;
                info.RowsCount = rows;
                info.ColumnsCount = columns;
                info.VisibleCheckBox = visibleCheckBox;
                info.ItemsSize = itemsSize;
                info.OnlyColumnsForFloating = onlyColumnsForFloating;
                info.ItemsTextAligment = itemsTextAlignment;
                info.ShowItemsShadow = showItemsShadow;
                info.ItemsShadowColor = itemsShadowColor;
                info.ItemsShadowOffset = itemsShadowOffset;
                info.TextAlignment = m_textAlignment;
                info.ForeColor = m_textColor;
                info.Text = m_text;
            }
            #endregion
        }
        #endregion

        #region Members
        private ChartLegend currentInfo = null;
        private LegendProperties info = new LegendProperties();
        private LegendProperties undoInfo = new LegendProperties();
        #endregion

        #region Methods
        /// <summary>
        /// Sets the legend to editing.
        /// </summary>
        /// <param name="legendInfo">The legend.</param>
        public void SetLegendInfo(ChartLegend legendInfo)
        {
            currentInfo = legendInfo;
            info.SetInfo(legendInfo);
            undoInfo.SetInfo(legendInfo);

            prptgrPropertys.SelectedObject = this.info;
        }

        /// <summary>
        /// Handles the Click event of the bttnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bttnOK_Click(object sender, System.EventArgs e)
        {
            info.OutInfo(currentInfo);
            Hide();
        }

        /// <summary>
        /// Handles the Click event of the bttnApply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bttnApply_Click(object sender, System.EventArgs e)
        {
            info.OutInfo(currentInfo);
        }
        #endregion
    }
}
