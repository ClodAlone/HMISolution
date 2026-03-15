//-------------------------------------------------------------------------------------------------
// <copyright file="GridFormatCellDialog.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Drawing;
    using Syncfusion.Styles;

    /// <summary>
    /// Defines excel-like Format Cells Dialog that allows the user to format the cells dynamically.
    /// There are options to customized cell font, font color, font size, font style, font effects,
    /// background, alignment, cell merge, text formats.
    /// </summary>
    public partial class GridFormatCellDialog : Form
    {
        GridControl grid;
        string prevFormat = string.Empty;
        bool firstCall = true;
        GridRangeInfo range;
        bool changeBackColor = false, changeFont = false, changeFontColor = false;
        private bool applyStyleInHeaders = false;
           
        /// <summary>
        /// Instantiates GridFormatCell dialog.
        /// </summary>
        /// <param name="grid">grid to be formatted.</param>
        public GridFormatCellDialog(GridControl grid)
        {
            this.InitializeComponent();

            this.grid = grid;

            if ((grid.CurrentCell.RowIndex == -1 && grid.CurrentCell.ColIndex == -1)
                || (grid.CurrentCell.RowIndex == 0 && grid.CurrentCell.ColIndex == 0))
            {
                grid.CurrentCell.Activate(1, 1);
            }

            grid.Model.Options.SelectCellsMouseButtonsMask = MouseButtons.Left;
        }
        /// <summary>
        /// Gets a value indicating whether the GridFormatCellDialog sytle is apply for Header also.
        /// </summary>
        [DefaultValue(false),
        Category(@"Appearance")]
        [Description("Defines whether the GridFormatCellDialog should be used for header cells.")]
        public bool ApplyStyleInHeaders
        {
            get { return applyStyleInHeaders; }
            set { applyStyleInHeaders = value; }
        }

        private void GridFormatCellDialog_Load(object sender, EventArgs e)
        {
            GridRangeInfo range = this.grid.Model.SelectedRanges.ActiveRange, range2;
            GridStyleInfo style2 = this.grid.GetCombinedStyle(range);

            foreach (FontFamily f in FontFamily.Families)
            {
                this.lstFontFace.Items.Add(f.Name);
                this.txtFFace.AutoCompleteCustomSource.Add(f.Name);
            }

            foreach (object o in this.lstFontSize.Items)
            {
                this.txtFSize.AutoCompleteCustomSource.Add(o.ToString());
            }

            if (style2 == null)
            {
                style2 = this.grid.GetViewStyleInfo(this.grid.CurrentCell.RowIndex, this.grid.CurrentCell.ColIndex);
            }

            this.txtFFace.Text = style2.Font.Facename;
            this.txtFSize.Text = style2.Font.Size.ToString();
            if (style2.Font.Bold && style2.Font.Italic)
            {
                this.txtFType.Text = "Bold Italic";
            }
            else
            {
                this.txtFType.Text = style2.Font.FontStyle.ToString();
            }

            this.chkUnderline.Checked = style2.Font.Underline;
            this.chkStrikeout.Checked = style2.Font.Strikeout;
            this.colorPickerButton1.SelectedColor = style2.TextColor;

            this.cmbHzntl.SelectedItem = style2.HorizontalAlignment.ToString();
            this.cmbVrtcl.SelectedItem = style2.VerticalAlignment.ToString();
            this.upDownIndent.Value = style2.TextMargins.Left;
            this.upDownOrient.Value = style2.Font.Orientation;
            this.chkWrapText.Checked = style2.WrapText;
            if (this.grid.Model.CoveredRanges.Find(this.grid.CurrentCell.RowIndex, this.grid.CurrentCell.ColIndex, out range2))
            {
                this.chkMerge.Checked = true;
            }
            else
            {
                this.chkMerge.Checked = false;
            }

            this.chkRightToLeft.Checked = style2.RightToLeft == RightToLeft.Yes ? true : false;

            string[] gradients = Enum.GetNames(typeof(GradientStyle));
            foreach (string g in gradients)
            {
                this.cmbGradient.Items.Add(g);
            }

            string[] patterns = Enum.GetNames(typeof(PatternStyle));
            foreach (string p in patterns)
            {
                this.cmbPattern.Items.Add(p);
            }

            this.cmbBgStyle.SelectedItem = style2.Interior.Style.ToString();
            this.cmbGradient.SelectedItem = style2.Interior.GradientStyle.ToString();
            this.clrBack.SelectedColor = style2.BackColor;
            this.clrFore.SelectedColor = style2.Interior.ForeColor;
            this.clrPanel.BackColor = style2.BackColor;
            this.clrPanel.BackgroundColor = style2.Interior;
            if (this.cmbBgStyle.SelectedItem.ToString() == "Gradient")
            {
                this.cmbGradient.Enabled = true;
                this.lblGradient.Enabled = true;
            }

            this.cmbPattern.SelectedItem = style2.Interior.PatternStyle.ToString();

            if (this.cmbBgStyle.SelectedItem.ToString() == "Pattern")
            {
                this.cmbPattern.Enabled = true;
                this.lblPattern.Enabled = true;
            }

            if (style2.Format.StartsWith("0") || style2.Format.StartsWith("1"))
            {
                this.lstbxFormat.SelectedItem = "Number";
            }
            else if (style2.Format.Equals("C"))
            {
                this.lstbxFormat.SelectedItem = "Currency";
            }
            else if (style2.Format.Contains("E"))
            {
                this.lstbxFormat.SelectedItem = "Scientific";
            }
            else if (style2.Format.Contains("%"))
            {
                this.lstbxFormat.SelectedItem = "Percentage";
            }
            else if (style2.Format.Equals("t"))
            {
                this.lstbxFormat.SelectedItem = "Time";
            }
            else if (style2.Format.Contains("d") || style2.Format.Contains("s") || style2.Format.Contains("f"))
            {
                this.lstbxFormat.SelectedItem = "Date";
            }
            else
            {
                this.lstbxFormat.SelectedItem = "Text";
            }

            this.prevFormat = this.lstbxFormat.SelectedItem.ToString();

            this.lstFontFace.SelectedIndexChanged += new EventHandler(this.lstFontFace_SelectedIndexChanged);
            this.lstFontSize.SelectedIndexChanged += new EventHandler(this.lstFontSize_SelectedIndexChanged);
            this.lstFontType.SelectedIndexChanged += new EventHandler(this.lstFontType_SelectedIndexChanged);
            this.colorPickerButton1.ColorSelected += new EventHandler(colorPickerButton1_ColorSelected);
            this.clrBack.ColorSelected += new EventHandler(this.clrBack_ColorSelected);
            this.clrFore.ColorSelected += new EventHandler(this.clrFore_ColorSelected);
            this.cmbBgStyle.SelectedIndexChanged += new EventHandler(this.cmbBgStyle_SelectedIndexChanged);
            this.cmbGradient.SelectedIndexChanged += new EventHandler(this.cmbGradient_SelectedIndexChanged);
            this.cmbPattern.SelectedIndexChanged += new EventHandler(this.cmbPattern_SelectedIndexChanged);

            this.changeFontColor = false;
            this.changeFont = false;
            this.changeBackColor = false;
        }

        void colorPickerButton1_ColorSelected(object sender, EventArgs e)
        {
            this.changeFontColor = true;
        }

        void lstFontType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.changeFont = true;
            this.txtFType.Text = this.lstFontType.SelectedItem.ToString();
        }

        void lstFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.changeFont = true;
            this.txtFSize.Text = this.lstFontSize.SelectedItem.ToString();
        }

        void lstFontFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.changeFont = true;
            this.txtFFace.Text = this.lstFontFace.SelectedItem.ToString();
        }

        void cmbPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowSampleBg();
        }

        void cmbGradient_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowSampleBg();
        }

        void cmbBgStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.cmbBgStyle.SelectedIndex)
            {
                case 1:
                    this.clrFore.Enabled = true;
                    this.lblGradient.Enabled = this.cmbGradient.Enabled = true;
                    this.lblPattern.Enabled = this.cmbPattern.Enabled = false;
                    break;
                case 2:
                    this.clrFore.Enabled = true;
                    this.lblPattern.Enabled = this.cmbPattern.Enabled = true;
                    this.lblGradient.Enabled = this.cmbGradient.Enabled = false;
                    break;
                default:
                    this.clrFore.Enabled = false;
                    this.lblGradient.Enabled = this.cmbGradient.Enabled = this.lblPattern.Enabled = this.cmbPattern.Enabled = false;
                    break;
            }

            this.ShowSampleBg();
        }

        void clrFore_ColorSelected(object sender, EventArgs e)
        {
            this.changeFontColor = true;
            this.ShowSampleBg();
        }

        private void ShowSampleBg()
        {
            if (this.cmbBgStyle.SelectedItem.ToString() == "Gradient")
            {
                GradientStyle g = (GradientStyle)Enum.Parse(typeof(GradientStyle), this.cmbGradient.SelectedItem.ToString());
                this.clrPanel.BackgroundColor = new BrushInfo(g, this.clrFore.SelectedColor, this.clrBack.SelectedColor);
            }
            else if (this.cmbBgStyle.SelectedItem.ToString() == "Pattern")
            {
                PatternStyle p = (PatternStyle)Enum.Parse(typeof(PatternStyle), this.cmbPattern.SelectedItem.ToString());
                this.clrPanel.BackgroundColor = new BrushInfo(p, this.clrFore.SelectedColor, this.clrBack.SelectedColor);
            }
            else
            {
                this.clrPanel.BackgroundColor = new BrushInfo(this.clrBack.SelectedColor);
            }
        }

        void clrBack_ColorSelected(object sender, EventArgs e)
        {
            this.changeBackColor = true;
            this.ShowSampleBg();
        }
               
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.grid.BeginUpdate();
            this.range = this.grid.Model.SelectedRanges.ActiveRange;
            if (!ApplyStyleInHeaders)
                AdjustHeaderRange();
            int row = this.grid.CurrentCell.RowIndex, col = this.grid.CurrentCell.ColIndex;
            if (this.range.Contains(GridRangeInfo.Cell(row, col)))
            {
                this.range.GetFirstCell(out row, out col);
            }
            else
            {
                this.range = GridRangeInfo.Cell(row, col);
            }

            GridStyleInfo style;
            bool isCell = true;
            bool isCol = true;
            if (this.range.IsRows)
                isCol = false;

            while (isCell && row <= this.grid.RowCount && col <= this.grid.ColCount)
            {
                style = this.grid[row, col];

                if (style != null)
                {
                    #region Font settings
                    if (this.changeFont)
                    {
                        style.Font.Facename = this.txtFFace.Text;
                        if (this.txtFType.Text == "Bold Italic")
                        {
                            style.Font.Bold = true;
                            style.Font.Italic = true;
                        }
                        else
                        {
                            style.Font.FontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), this.txtFType.Text);
                        }

                        style.Font.Size = float.Parse(this.txtFSize.Text);
                    }
                    style.Font.Underline = this.chkUnderline.Checked;
                    style.Font.Strikeout = this.chkStrikeout.Checked;
                    if (this.changeFontColor)
                        style.TextColor = this.colorPickerButton1.SelectedColor;

                    #endregion

                    #region Background Settings
                    
                    switch (this.cmbBgStyle.SelectedItem.ToString())
                    {
                        case "Solid":
                            if (this.changeBackColor)
                            style.BackColor = this.clrBack.SelectedColor;
                            break;
                        case "Gradient":
                            GradientStyle gradient = (GradientStyle)Enum.Parse(typeof(GradientStyle), this.cmbGradient.SelectedItem.ToString());
                            style.Interior = this.clrPanel.BackgroundColor;
                            break;
                        case "Pattern":
                            PatternStyle pattern = (PatternStyle)Enum.Parse(typeof(PatternStyle), this.cmbPattern.SelectedItem.ToString());
                            style.Interior = this.clrPanel.BackgroundColor;
                            break;
                    }

                    #endregion

                    #region Alignment settings

                    style.HorizontalAlignment = (GridHorizontalAlignment)Enum.Parse(typeof(GridHorizontalAlignment), this.cmbHzntl.SelectedItem.ToString());
                    style.VerticalAlignment = (GridVerticalAlignment)Enum.Parse(typeof(GridVerticalAlignment), this.cmbVrtcl.SelectedItem.ToString());
                    style.TextMargins.Left = (int)this.upDownIndent.Value;
                    style.Font.Orientation = (int)this.upDownOrient.Value;
                    style.WrapText = this.chkWrapText.Checked;

                    if (this.chkMerge.Checked)
                    {
                        this.grid.Model.CoveredRanges.Add(this.range);
                    }
                    else
                    {
                        GridRangeInfo r = new GridRangeInfo();
                        if (this.grid.Model.CoveredRanges.Find(row, col, out r))
                        {
                            this.grid.Model.CoveredRanges.Remove(r);
                        }
                    }

                    if (this.chkRightToLeft.Checked)
                    {
                        style.RightToLeft = RightToLeft.Yes;
                    }
                    else
                    {
                        style.RightToLeft = RightToLeft.No;
                    }

                    #endregion

                    #region Number Settings
                    if (this.lstbxType.SelectedItem == null && this.lstbxType.Items.Count > 0)
                    {
                        this.lstbxType.SelectedIndex = 0;
                    }

                    switch (this.lstbxFormat.SelectedItem.ToString())
                    {
                        case "Number":
                        case "Currency":
                        case "Percentage":
                        case "Scientific":
                            if (style.CellValueType == typeof(DateTime))
                            {
                                style.Format = string.Empty;
                                style.CellValueType = typeof(string);
                                if (style.Tag != null)
                                {
                                    style.CellValue = style.Tag.ToString();
                                }
                            }

                            style.CellValueType = typeof(double);
                            style.Format = this.lstbxType.SelectedItem.ToString();
                            break;
                        case "Date":
                            if (style.CellValueType != typeof(DateTime))
                            {
                                style.Format = string.Empty;
                                style.Tag = style.CellValue;
                                style.CellValue = DateTime.Now.ToString();
                            }

                            style.CellValueType = typeof(DateTime);
                            style.Format = this.lstbxType.SelectedItem.ToString();
                            break;
                        case "Time":
                            if (style.CellValueType != typeof(DateTime))
                            {
                                style.Format = string.Empty;
                                style.Tag = style.CellValue;
                                style.CellValue = DateTime.Now.ToString();
                            }

                            style.CellValueType = typeof(DateTime);
                            style.Format = this.lstbxType.SelectedItem.ToString();
                            break;
                        default:
                            style.CellValueType = typeof(string);
                            if (style.Tag != null)
                            {
                                style.CellValue = style.Tag;
                            }

                            style.Format = string.Empty;
                            break;
                    }
                    #endregion
                }

                isCell = this.range.GetNextCell(ref row, ref col, isCol);
            }

            this.grid.EndUpdate(true);
            this.grid.InvalidateRange(this.range);
        }
        /// <summary>
        /// Reset the Grid Range when the boolean value of ApplyStyleInHeaders is false.
        /// </summary>
        private void AdjustHeaderRange()
        {
            if (this.range.RangeType == GridRangeInfoType.Cols)
            {
                this.range = GridRangeInfo.Cells(1, this.range.Left, this.grid.RowCount, this.range.Right);
                this.grid.CurrentCell.MoveTo(1, this.range.Left);
            }
            else if (this.range.RangeType == GridRangeInfoType.Rows)
            {
                this.range = GridRangeInfo.Cells(this.range.Top, 1, this.range.Bottom, this.grid.ColCount);
                this.grid.CurrentCell.MoveTo(this.range.Top, 1);
            }
            else if (this.range.RangeType == GridRangeInfoType.Table)
            {
                this.range = GridRangeInfo.Cells(1, 1, this.grid.RowCount, this.grid.ColCount);
                this.grid.CurrentCell.MoveTo(1, 1);
            }
        }
        private void lstbxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lstbxType.Show();
            this.lblIfText.Text = string.Empty;
            GridRangeInfo range = this.grid.Model.SelectedRanges.ActiveRange;
            GridStyleInfo style;

            switch (this.lstbxFormat.SelectedItem.ToString())
            {
                case "Number":
                    this.lstbxType.Items.Clear();
                    this.lstbxType.Items.Add("0.00");
                    this.lstbxType.Items.Add("0.00;(0.00)");
                    this.lstbxType.Items.Add("10:##,##0.#");
                    break;
                case "Currency":
                    this.lstbxType.Items.Clear();
                    this.lstbxType.Items.Add("C");
                    break;
                case "Percentage":
                    this.lstbxType.Items.Clear();
                    this.lstbxType.Items.Add("###0.##%");
                    break;
                case "Scientific":
                    this.lstbxType.Items.Clear();
                    this.lstbxType.Items.Add("#0.#E+00");
                    break;
                case "Date":
                    this.lstbxType.Items.Clear();
                    this.lstbxType.Items.Add("d");
                    this.lstbxType.Items.Add("D");
                    this.lstbxType.Items.Add("f");
                    this.lstbxType.Items.Add("dddd, dd MMMM yyyy");
                    this.lstbxType.Items.Add("s");
                    break;
                case "Time":
                    this.lstbxType.Items.Clear();
                    this.lstbxType.Items.Add("t");
                    break;
                default:
                    this.lstbxType.Hide();
                    this.lblIfText.Text = "Text format cells are treated as text even when\na number is in the cell. The cell is displayed\nexactly as entered";
                    break;
            }

            if (this.lstbxType.Items.Count > 0)
            {
                this.lstbxType.SelectedIndex = 0;
                if (this.firstCall)
                {
                    style = this.grid.GetCombinedStyle(range);
                    if (style != null && style.Format != null)
                    {
                        this.lstbxType.SelectedItem = style.Format;
                    }
                }
            }
        }
    } 
}
