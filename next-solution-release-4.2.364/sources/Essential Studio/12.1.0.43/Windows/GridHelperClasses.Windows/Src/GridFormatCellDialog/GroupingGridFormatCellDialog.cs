//-------------------------------------------------------------------------------------------------
// <copyright file="GroupingGridFormatCellDialog.cs" company="Syncfusion">
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
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using System.Collections;

    /// <summary>
    /// Defines excel-like Format Cells Dialog that allows the user to format the cells dynamically.
    /// There are options to customized cell font, font color, font size, font style, font effects,
    /// background, alignment, cell merge, text formats.
    /// </summary>
    public partial class GroupingGridFormatCellDialog : Form
    {
        GridGroupingControl grid;
        string prevFormat = string.Empty;
        bool firstCall = true;
        GridRangeInfo range;
        bool changeBackColor = false, changeFont = false, changeFontColor = false;
        Hashtable styleCollection = null;

        /// <summary>
        /// Instantiates GroupingGridFormatCell dialog.
        /// </summary>
        /// <param name="grid">GroupingGrid to be formatted.</param>
        public GroupingGridFormatCellDialog(GridGroupingControl grid)
        {
            this.InitializeComponent();
            this.styleCollection = new Hashtable();
            this.grid = grid;

            if ((grid.TableControl.CurrentCell.RowIndex == -1 && grid.TableControl.CurrentCell.ColIndex == -1)
                || (grid.TableControl.CurrentCell.RowIndex == 0 && grid.TableControl.CurrentCell.ColIndex == 0))
            {
                grid.TableControl.CurrentCell.Activate(1, 1);
            }
            this.grid.QueryCellStyleInfo += new GridTableCellStyleInfoEventHandler(grid_QueryCellStyleInfo);
            
        }

        /// <summary>
        /// Applies styles to cells based on stylecollection hashtable
        /// </summary>
        void grid_QueryCellStyleInfo(object sender, GridTableCellStyleInfoEventArgs e)
        {
            if (e.TableCellIdentity.DisplayElement.GetRecord() != null && e.TableCellIdentity.Column != null)
            {
                int id = e.TableCellIdentity.DisplayElement.GetRecord().Id;
                string selstring = id + e.TableCellIdentity.Column.Name;
                if (styleCollection.Contains(selstring))
                {
                    object value = e.Style.FormattedText;
                    e.Style.CopyFrom((GridStyleInfo)styleCollection[selstring]);
                    e.Style.FormattedText = value.ToString();
                }
            }
        }
        /// <summary>
        /// Loads the GroupingGridFormatCellDialog
        /// </summary>
        private void GridFormatCellDialog_Load(object sender, EventArgs e)
        {
            GridRangeInfo range = this.grid.TableModel.SelectedRanges.ActiveRange, range2;
            GridStyleInfo style2 = this.grid.TableModel.GetCombinedStyle(range);

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
                style2 = this.grid.TableControl.GetViewStyleInfo(this.grid.TableControl.CurrentCell.RowIndex, this.grid.TableControl.CurrentCell.ColIndex);
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
            if (this.grid.TableModel.CoveredRanges.Find(this.grid.TableControl.CurrentCell.RowIndex, this.grid.TableControl.CurrentCell.ColIndex, out range2))
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

        /// <summary>
        /// Applies selected Font Color to the cells.
        /// </summary>
        void colorPickerButton1_ColorSelected(object sender, EventArgs e)
        {
            this.changeFontColor = true;
        }

        /// <summary>
        /// Applies selected FontType to the cells.
        /// </summary>
        void lstFontType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.changeFont = true;
            this.txtFType.Text = this.lstFontType.SelectedItem.ToString();
        }

        /// <summary>
        /// Applies selected FontSize to the cells.
        /// </summary>
        void lstFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.changeFont = true;
            this.txtFSize.Text = this.lstFontSize.SelectedItem.ToString();
        }

        /// <summary>
        /// Applies selected FontFace to the cells.
        /// </summary>
        void lstFontFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.changeFont = true;
            this.txtFFace.Text = this.lstFontFace.SelectedItem.ToString();
        }

        /// <summary>
        /// Displays selected Pattern-style background in preview panel
        /// </summary>
        void cmbPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowSampleBg();
        }

        /// <summary>
        /// Displays selected Gradient-style background in preview panel
        /// </summary>
        void cmbGradient_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShowSampleBg();
        }
        
        /// <summary>
        /// Selects different background Styles.
        /// </summary>
        /// <Styles>Pattern, Gradient</Styles>
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

        /// <summary>
        /// Applies selected Font Color
        /// </summary>
        void clrFore_ColorSelected(object sender, EventArgs e)
        {
            this.changeFontColor = true;
            this.ShowSampleBg();
        }
        /// <summary>
        /// Displays the selected background color in the Panel as preview.
        /// </summary>
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

        /// <summary>
        /// Applies selected back color to the cells
        /// </summary>
        void clrBack_ColorSelected(object sender, EventArgs e)
        {
            this.changeBackColor = true;
            this.ShowSampleBg();
        }
        
        /// <summary>
        /// Applies styles to the Grid if the ok button is clicked in the dialog.
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.grid.Table.CurrentRecord != null)
            {
                this.grid.BeginUpdate();

                int Id = this.grid.Table.CurrentRecord.Id;
                string colName = string.Empty;
                if (((GridTableCellStyleInfo)this.grid.TableControl.CurrentCell.Renderer.StyleInfo).TableCellIdentity.Column != null)
                    colName = ((GridTableCellStyleInfo)this.grid.TableControl.CurrentCell.Renderer.StyleInfo).TableCellIdentity.Column.Name;
                else
                    colName = this.grid.TableDescriptor.Columns[0].Name;
                int firstRecordRowIndex = this.grid.Table.Records[0].GetRowIndex();
                this.range = this.grid.TableModel.SelectedRanges.ActiveRange;
                int row = this.grid.TableControl.CurrentCell.RowIndex, col = this.grid.TableControl.CurrentCell.ColIndex;
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

                while (isCell && row <= this.grid.TableModel.RowCount && col <= this.grid.TableModel.ColCount)
                {

                    style = this.grid.TableModel[row, col];

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
                            this.grid.TableModel.CoveredRanges.Add(this.range);
                        }
                        else
                        {
                            GridRangeInfo r = new GridRangeInfo();
                            if (this.grid.TableModel.CoveredRanges.Find(row, col, out r))
                            {
                                this.grid.TableModel.CoveredRanges.Remove(r);
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
                                style.FormattedText = style.GetFormattedText(style.CellValue);
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

                        #region Store style to the Collection
                        string key = Id + colName;
                        if (this.styleCollection.ContainsKey(key))
                        {
                            this.styleCollection[key] = style;
                        }
                        else
                        {
                            this.styleCollection.Add(key, style);
                        }
                        #endregion Store style to the Collection
                    }

                    isCell = this.range.GetNextCell(ref row, ref col, isCol);
                    if(row - firstRecordRowIndex >-1)
                        Id = this.grid.Table.Records[row - firstRecordRowIndex].GetRecord().Id;
                    int ii = this.grid.TableDescriptor.GetLastColumnIndex();
                        colName = this.grid.TableDescriptor.Columns[this.grid.TableDescriptor.ColIndexToField(col)].Name;
                }

                this.grid.EndUpdate(true);
                this.grid.TableModel.InvalidateRange(this.range, GridRangeOptions.None);
            }
        }

        /// <summary>
        /// Applies specific format to the item based on the selected index.
        /// </summary>
        private void lstbxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lstbxType.Show();
            this.lblIfText.Text = string.Empty;
            GridRangeInfo range = this.grid.TableModel.SelectedRanges.ActiveRange;
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
                    style = this.grid.TableModel.GetCombinedStyle(range);
                    if (style != null && style.Format != null)
                    {
                        this.lstbxType.SelectedItem = style.Format;
                    }
                }
            }
        }
    } 
}
