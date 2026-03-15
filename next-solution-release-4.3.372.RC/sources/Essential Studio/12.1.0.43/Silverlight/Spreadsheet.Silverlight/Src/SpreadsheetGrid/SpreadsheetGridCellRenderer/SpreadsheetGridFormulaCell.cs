#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public class SpreadsheetGridFormulaModel : GridCellFormulaModel
    {
        public SpreadsheetGridFormulaModel(GridModel grid): base(grid)
        {

        }

        public override IGridCellRenderer CreateRenderer()
        {
            SpreadsheetGridCellTextBoxRenderer r = new SpreadsheetGridCellTextBoxRenderer();
            r.RaiseCreated(this);
            return r;
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            SpreadsheetGridModel SpreadsheetModel = this.Grid as SpreadsheetGridModel;
            if (SpreadsheetModel != null && SpreadsheetModel.IsInFormulabarEditing)
            {
                if (style.RowIndex == SpreadsheetModel.CurrentCellState.RowIndex && style.ColumnIndex == SpreadsheetModel.CurrentCellState.ColumnIndex)
                {
                    return style.Text;
                }
            }
            return base.GetFormattedText(style, value, textInfo);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

#if SILVERLIGHT
    public class SpreadsheetGridCellTextBoxRenderer : GridCellTextBoxCellRenderer
#else
    public class SpreadsheetGridCellTextBoxRenderer : GridCellTextBoxRenderer
#endif
    {
        public SpreadsheetGridCellTextBoxRenderer()
        {
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
            this.SupportsRenderOptimization = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        public override void OnInitializeContent(TextBox textBox, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(textBox, style);
            if (style.Format.ToString().EndsWith("%") && style.FormulaTag == null)
            {
                if (!textBox.Text.EndsWith("%"))
                {
                    Regex regex = new Regex(@"[\d]");
                    if (regex.IsMatch(textBox.Text))
                    {
                        var val = Int32.Parse(textBox.Text) * 100;
                        textBox.Text = val.ToString() + "%";
                        textBox.Select(textBox.Text.Length - 1, 0);
                    }
                }
            }
        }

        protected override string GetControlTextCore(GridRenderStyleInfo style, object cellValue)
        {
            switch (GridControl.Model.Options.FormulaDisplayBehavior)
            {
                case GridShowFormulaBehavior.Always:
                    return style.GetText(cellValue);

                case GridShowFormulaBehavior.Never:
                    goto default;

                case GridShowFormulaBehavior.WhenCurrent:
                    if (CurrentCell.HasCurrentCellAt(style.CellRowColumnIndex))
                        return style.GetText(cellValue);
                    goto default;

                case GridShowFormulaBehavior.WhenEditing:
                    if (CurrentCell.HasCurrentCellAt(style.CellRowColumnIndex) && CurrentCell.IsEditing)
                        return style.GetText(cellValue);
                    goto default;

                default:
                    String text = style.GetFormattedText(cellValue);

                    return text;
            }
        }

        /// <summary>
        /// Overridden to make sure the formula is recomputed as the changes are saved.
        /// </summary>
        /// <returns>True if the changes were saved.</returns>
        protected override bool OnSaveChanges()
        {
            bool b = base.OnSaveChanges();
            if (b)
            {
                //reset the formula info to force recalculation
                CurrentStyle.ModelStyle.FormulaTag = null;
            }
            return b;
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing || (e.Text.Length > 0 && (int)((char)e.Text[0]) == 13))//13 is the ASCII for Enter key
                return;
            if (string.IsNullOrEmpty(e.Text))
                return;
            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            TextBox tb = CurrentCellUIElement;
            if (tb != null && !string.IsNullOrEmpty(e.Text))
            {

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[\b]");
                string str = regex.Replace(e.Text, "");
#if SILVERLIGHT
                if (!str.Equals("\r") || GridControl.Model.EnableMultiline)
#else
                if (!str.Equals("\r"))
#endif
                {
                    tb.Text = str;
                    tb.Select(tb.Text.Length, 1);
                }
                int ascii = (int)((char)e.Text[0]);
                if (!(ascii > 57) && !(ascii < 48))
                {
                    if (this.CurrentStyle.Format.ToString().EndsWith("%") && tb.Text != ".")
                    {
                        tb.Text = tb.Text + "%";
                        tb.Select(1, 0);
                    }
                }

            }

            e.Handled = true;
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                return false;
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


    }

}
