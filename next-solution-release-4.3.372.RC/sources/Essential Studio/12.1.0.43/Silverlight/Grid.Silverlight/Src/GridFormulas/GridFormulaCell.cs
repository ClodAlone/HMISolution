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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
#if !WinRT
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using System.Windows.Interop;
using System.Collections.Generic;
#if SILVERLIGHT
using ArrayList = System.Collections.Generic.List<object>;
using Stack = System.Collections.Generic.Stack<object>;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
#endif
namespace Syncfusion.Windows.Controls.Grid
{
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.GridCommon;
using Syncfusion.WinRT.Styles;

namespace Syncfusion.WinRT.Controls.Grid
{
#endif

    #region GridCellFormulaModel/GridFormulaCellRenderer

    #region GridCellFormulaModel
    /// <summary>
    /// This GridCellModel supports entering formulas in a grid cell. This support is provided
    /// through the <see cref="GridCellFormulaModel.Engine"/> member of this class. Engine is
    /// a <see cref="GridFormulaEngine"/>.
    /// </summary>
    /// <remarks>
    /// The default behavior is that any cell whose CellType is GridCellFormulaModel will
    /// be interpreted as a formula cell provided the text in the cell starts with '='. 
    /// So, for such cells you would enter formulas such as
    /// =A1+A2+A3 or =Sum(A1:A3), an an attempt would be made to parse and compute the entry.
    /// <para/>
    /// Alternatively, you can use the <see cref="GridCellFormulaModel.formulaChar"/> character
    /// to specify which cells of CellType GridCellFormulaModel are to be used as formulas.
    /// You set this formulaChar private property through the class constructor which passes
    /// this parameter. If you set this value to '\0', every cell of CellType GridCellFormulaModel
    /// will be treated as a formula.
    /// <para/>
    /// The <see cref="RefreshCells"/> method redraws all the formula cells that depend upon
    /// a particular cell. This method is called when the user changes a value that affects
    /// other formula cells.
    /// <para/>
    /// The <see cref="GetFormattedText"/> method is where the actual calculations are done,
    /// if necessary, through calls to the Engine methods. This method also conditionally
    /// parses the formula only if it has been modified since the previous parse.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellFormulaModel : GridCellTextBoxModel
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellFormulaModel"/> object.
        /// </summary>
        /// <param name="grid"></param>
        public GridCellFormulaModel(GridModel grid)
            : base()
        {
            this.grid = grid;
            engine = new GridFormulaEngine(grid);
            engine.formulaChar = FormulaChar;

            GridSheetFamilyItem family = GridFormulaEngine.GetSheetFamilyItem(grid);
            if (!family.isSheeted)
            {
                grid.CommittedCellInfo += new GridCommitCellInfoEventHandler(grid_CommittedCellInfo);
            }

            if (grid.isLoaded)
            {
                grid.RowsInserted += new GridRangeInsertedEventHandler(grid_RowsInserted);
                grid.RowsRemoved += new GridRangeRemovedEventHandler(grid_RowsRemoved);
                grid.ColumnsInserted += new GridRangeInsertedEventHandler(grid_ColumnsInserted);
                grid.ColumnsRemoved += new GridRangeRemovedEventHandler(grid_ColumnsRemoved);
            }
        }

        /// <summary>
        /// Creates formula cell renderer.
        /// </summary>
        /// <returns>The cell renderer that this method creates.</returns>
        public override IGridCellRenderer CreateRenderer()
        {
            GridCellFormulaRenderer r = new GridCellFormulaRenderer();
            r.RaiseCreated(this);
            return r;
        }


        GridModel grid;

        /// <summary>
        /// Gets the GridModel associated with this cell model.
        /// </summary>
        public GridModel Grid
        {
            get { return grid; }
            // set { grid = value; }
        }

        private char formulaChar = '=';

        /// <summary>
        /// Gets or sets the formula character.
        /// </summary>
        ///<remarks> 
        /// Set this field to a particular character such as '=' to conditionally flag a cell
        /// whose CellType is GridCellFormulaModel as holding a formula. If you do not 
        /// set this field, then all cells of CellType GridCellFormulaModel that begin with '='
        /// will be treated as a formula.
        /// </remarks>
        public char FormulaChar
        {
            get
            {
                return formulaChar;
            }
            set { formulaChar = value; }
        }

        #region model events

        void grid_RowsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            this.Engine.AdjustReferencesForRangeChange(sender as GridModel, false, true, e.InsertAt, e.InsertAt + e.Count - 1, false);
        }
        void grid_ColumnsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            this.Engine.AdjustReferencesForRangeChange(sender as GridModel, true, false, e.RemoveAt, e.RemoveAt + e.Count - 1, true);
        }

        void grid_ColumnsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            this.Engine.AdjustReferencesForRangeChange(sender as GridModel, true, true, e.InsertAt, e.InsertAt + e.Count - 1, false);
        }

        void grid_RowsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            this.Engine.AdjustReferencesForRangeChange(sender as GridModel, false, false, e.RemoveAt, e.RemoveAt + e.Count - 1, true);
        }



        //Used to redraw cells changed in CommitCellInfo.
        void grid_CommittedCellInfo(object sender, GridCommitCellInfoEventArgs e)
        {
            //Console.WriteLine("  DependentCells{0}  DependentFormulaCells{1}   refreshedCells{2}", engine.DependentCells.Count, engine.DependentFormulaCells.Count, engine.refreshedCells.Count);

            if (e.Sip == null || e.Sip == GridStyleInfoStore.CellValueProperty || e.Sip == GridStyleInfoStore.FormulaTagProperty)
            {
                if (this.grid[e.Cell.RowIndex, e.Cell.ColumnIndex].CellValue != e.Style.CellValue)
                    this.grid[e.Cell.RowIndex, e.Cell.ColumnIndex].CellValue = e.Style.CellValue;
                RefreshCells(e.Cell.RowIndex, e.Cell.ColumnIndex);
            }
            // Console.WriteLine("  DependentCells{0}  DependentFormulaCells{1}   refreshedCells{2}", engine.DependentCells.Count, engine.DependentFormulaCells.Count, engine.refreshedCells.Count);

        }

        #endregion

        /// <summary>
        /// This method forces any formula cell dependent upon the passed-in cell in the row
        /// and column to be recomputed. This method is called by the model whenever
        /// the contents of the passed-in cell is changed. For example, the user changing a
        /// single non-formula cell may require the recomputing of several additional formula cells. 
        /// This method triggers the redrawing of these dependent cells.
        /// </summary>
        /// <param name="row">Row index of the cell that was modifed by the user.</param>
        /// <param name="col">Column index of the cell that was modified by the user.</param>
        public void RefreshCells(int row, int col)
        {
            if (CalculatingSuspended)
                return;

            GridSheetFamilyItem family = GridFormulaEngine.GetSheetFamilyItem(this.Grid);

            string sheet = family.GridModelToToken != null
                        ? family.GridModelToToken[this.Grid] as string : "";
            string s = sheet + GridRangeInfo.GetAlphaLabel(col) + row.ToString();

            Engine.Refresh(s);
        }

        /// <summary>
        /// Gets or sets whether calculations should be done as values change in the underlying GridControl.
        /// </summary>
        public bool CalculatingSuspended
        {
            get { return Engine.CalculatingSuspended; }
            set { Engine.CalculatingSuspended = value; }
        }

        /// <summary>
        /// Triggers the parsing and computing of formulas.
        /// </summary>
        /// <remarks>
        /// Overridden to trigger the parsing and computing of formulas. The 
        /// computed value is returned for non-editing cells. The raw formula
        /// text is returned when the cell is actively beng edited.
        /// </remarks>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>Either the computed value or the raw formula text.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            string s = style.Text;
            Engine.GetFormulaText(ref s);

            // if (this.copyFormulasToClipboard)
            //     return s;

            bool usesFormulaChar = this.formulaChar != (char)0 && s.Length > 0 && s[0] == this.formulaChar;
            if (this.formulaChar == (char)0 || usesFormulaChar)
            {
                if (Grid.SuspendFormulaParsingAndCalculation)
                    return s;

                if (usesFormulaChar)
                    s = s.Substring(1); //strip it off

                if (s.Trim().Length == 0)
                    return "";

                GridStyleInfoIdentity id = style.Identity as GridStyleInfoIdentity;

                bool saveTag = false;
                if (style.FormulaTag == null)
                {
                    Engine.cell = GridRangeInfo.GetAlphaLabel(id.ColumnIndex) + id.RowIndex.ToString();
                    style.FormulaTag = new GridFormulaTag(Engine.Parse(s), null, id.RowIndex, id.ColumnIndex);
                    saveTag = true;
                }

                GridFormulaTag tag = style.FormulaTag;
                if (tag != null)
                {
                    //Check to see if parameter references need to be updated.
                    bool needAdjustment = NeedToAdjustReferences(id, tag);
                    if (needAdjustment)
                    {
                        tag.ParsedRow = id.RowIndex;
                        tag.ParsedCol = id.ColumnIndex;
                        saveTag = true;
                        Engine.SetDirty(tag);
                    }

                    if (Engine.IsDirty(tag)) //if(IsEmpty(tag.Text))
                    {
                        Engine.cell = GridRangeInfo.GetAlphaLabel(id.ColumnIndex) + id.RowIndex.ToString();

                        try
                        {
                            if (grid.EnableFormulaCalculations)
                            {
                                string text = Engine.ComputedValue(tag.Formula);
                                text = text.Trim('\"');
                                tag.Text = text;
                                saveTag = true;
                            }
                            else if (!string.IsNullOrEmpty(style.Format) && style.Format == ";;;")
                                return string.Empty;
                            else
                                return " ";
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }

                    if (saveTag)
                    {
                        bool b = this.Grid.IgnoreReadOnly;
                        this.Grid.IgnoreReadOnly = true;
                        this.Grid[id.RowIndex, id.ColumnIndex].FormulaTag = tag;
                        this.Grid.IgnoreReadOnly = b;
                    }

                    //FIXED() func will return text and it should not be converted to value type.
                    char UNIQUESTRINGMARKER = (char)127;
                    if (tag.Text != null && tag.Text.IndexOf(UNIQUESTRINGMARKER) == 0)
                        return tag.Text.Remove(0, 1);

                    CultureInfo ci = style.GetCulture(true);
                    DateTime dt;
                    double d;
                    if (tag.Text != null && double.TryParse(tag.Text, NumberStyles.Number | NumberStyles.AllowExponent, ci.NumberFormat, out d))
                    {
                        if (!IsEmpty(style.Format))
                        {
                            return ValueConvert.FormatValue(d, typeof(double), style.Format, ci, ci.NumberFormat, null);
                        }
                        return base.GetFormattedText(style, d, textInfo);
                    }
                    //if the tag.Text contains /n then the text converted into Date Time
                    else if (tag.Text != null && DateTime.TryParse(tag.Text, ci.NumberFormat, DateTimeStyles.None, out dt) && !string.IsNullOrEmpty(style.Format))
                    {
                        if (!IsEmpty(style.Format))
                        {
                            return ValueConvert.FormatValue(dt, typeof(DateTime), style.Format, ci, ci.NumberFormat, null);
                        }
                        return base.GetFormattedText(style, dt, textInfo);
                    }
                    else
                    {
                        if (tag.Text == null)
                            return string.Empty;
                        else
                        {
                            if (style.Format == ";;;")
                                return string.Empty;
                            return tag.Text.Trim('\"'); ;
                        }
                    }
                }
                else
                    return "";
            }
            else
            {
                //do not have a formula in the cell

                //check for possible format and try to apply it
                if (!IsEmpty(style.Format))
                {
                    CultureInfo ci = style.GetCulture(true);

                    double d;
                    if (style.Text.EndsWith("%"))
                    {
                        var txt = style.Text.Remove(style.Text.Length - 1);
                        if (double.TryParse(txt, NumberStyles.Number | NumberStyles.AllowExponent, ci.NumberFormat, out d))
                        {

                            return ValueConvert.FormatValue(d / 100, typeof(double), style.Format, ci, ci.NumberFormat,null);
                        }
                    }
                    if (double.TryParse(style.Text, NumberStyles.Number | NumberStyles.AllowExponent, ci.NumberFormat, out d))
                    {
                        return ValueConvert.FormatValue(d, typeof(double), style.Format, ci, ci.NumberFormat,null);
                    }
                }
                return base.GetFormattedText(style, value, textInfo);
            }
        }

        internal override string GetFormulaValue(GridStyleInfo style, object value, GridFormulaTag formulaTag)
        {
            string s = string.Empty;
            if (value != null)
                s = value.ToString();

            Engine.GetFormulaText(ref s);

            // if (this.copyFormulasToClipboard)
            //     return s;

            bool usesFormulaChar = this.formulaChar != (char)0 && s.Length > 0 && s[0] == this.formulaChar;
            if (this.formulaChar == (char)0 || usesFormulaChar)
            {
                if (usesFormulaChar)
                    s = s.Substring(1); //strip it off

                if (s.Trim().Length == 0)
                    return "";

                GridStyleInfoIdentity id = style.Identity as GridStyleInfoIdentity;

                bool saveTag = false;
                if (formulaTag == null)
                {
                    Engine.cell = GridRangeInfo.GetAlphaLabel(id.ColumnIndex) + id.RowIndex.ToString();
                    formulaTag = new GridFormulaTag(Engine.Parse(s), null, id.RowIndex, id.ColumnIndex);
                    formulaTag.Formula = AdjustRowIndex(formulaTag.Formula, id.RowIndex);
                    if (style.ConditionalFormat.Cell != null && !style.ConditionalFormat.Cell.IsEmpty)
                    {
                        RowColumnIndex AdjustTo = (RowColumnIndex)style.ConditionalFormat.Cell;
                        if (id.RowIndex == AdjustTo.RowIndex && id.ColumnIndex != AdjustTo.ColumnIndex)
                            formulaTag.Formula = AdjustColumnIndex(formulaTag.Formula, GridRangeInfo.GetAlphaLabel(AdjustTo.ColumnIndex), id.ColumnIndex);
                    }
                    saveTag = true;
                }

                GridFormulaTag tag = formulaTag;
                if (tag != null)
                {
                    //Check to see if parameter references need to be updated.
                    bool needAdjustment = NeedToAdjustReferences(id, tag);
                    if (needAdjustment)
                    {
                        tag.ParsedRow = id.RowIndex;
                        tag.ParsedCol = id.ColumnIndex;
                        saveTag = true;
                        Engine.SetDirty(tag);
                    }

                    if (Engine.IsDirty(tag)) //if(IsEmpty(tag.Text))
                    {
                        Engine.cell = GridRangeInfo.GetAlphaLabel(id.ColumnIndex) + id.RowIndex.ToString();

                        try
                        {
                            Engine.isFormulainConditionalFormat = true;
                            string text = Engine.ComputedValue(tag.Formula);
                            text = text.Trim('\"');
                            tag.Text = text;
                            saveTag = true;
                            Engine.isFormulainConditionalFormat = false;
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }

                    if (saveTag)
                    {
                        bool b = this.Grid.IgnoreReadOnly;
                        this.Grid.IgnoreReadOnly = true;
                        formulaTag = tag;
                        if (style.HasConditionalFormat)
                            style.ConditionalFormat.FormulaTag = tag;
                        this.Grid.IgnoreReadOnly = b;
                    }

                    //FIXED() func will return text and it should not be converted to value type.
                    char UNIQUESTRINGMARKER = (char)127;
                    if (tag.Text != null && tag.Text.IndexOf(UNIQUESTRINGMARKER) == 0)
                        return tag.Text.Remove(0, 1);

                    CultureInfo ci = style.GetCulture(true);
                    DateTime dt;
                    double d;
                    if (tag.Text != null && double.TryParse(tag.Text, NumberStyles.Number | NumberStyles.AllowExponent, ci.NumberFormat, out d))
                    {
                        if (!IsEmpty(style.Format))
                        {
                            return ValueConvert.FormatValue(d, typeof(double), style.Format, ci, ci.NumberFormat,null);
                        }
                        return base.GetFormulaValue(style, d, formulaTag);
                    }
                    else if (tag.Text != null && DateTime.TryParse(tag.Text, ci.NumberFormat, DateTimeStyles.None, out dt))
                    {
                        if (!IsEmpty(style.Format))
                        {
                            return ValueConvert.FormatValue(dt, typeof(DateTime), style.Format, ci, ci.NumberFormat,null);
                        }
                        return base.GetFormulaValue(style, dt, formulaTag);
                    }
                    else
                    {
                        if (tag.Text == null)
                            return string.Empty;
                        return tag.Text;
                    }
                }
                else
                    return "";
            }
            else
            {
                //do not have a formula in the cell

                //check for possible format and try to apply it
                if (!IsEmpty(style.Format))
                {
                    CultureInfo ci = style.GetCulture(true);

                    double d;
                    if (double.TryParse(style.Text, NumberStyles.Number | NumberStyles.AllowExponent, ci.NumberFormat, out d))
                    {
                        return ValueConvert.FormatValue(d, typeof(double), style.Format, ci, ci.NumberFormat,null);
                    }
                }
                return base.GetFormulaValue(style, value, formulaTag);
            }
        }

        internal string AdjustRowIndex(string formula, int newIndex)
        {
            StringBuilder newFormula = new StringBuilder(formula);
            StringBuilder sb = new StringBuilder();
            string[] numbers = Regex.Split(formula, @"\D+");
            int index = 0;
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int temp = formula.IndexOf(value);
                    if (temp > 0 && formula[temp - 1] != 'n')
                    {
                        index += temp;
                        newFormula = newFormula.Replace(value, newIndex.ToString(), index, value.Length);
                        formula = formula.Substring(temp + value.Length);
                        index += newIndex.ToString().Length;
                    }
                }
            }
            return newFormula.ToString();
        }

        internal string AdjustColumnIndex(string formula, string oldColumn, int newIndex)
        {
            string newColumn = GridRangeInfo.GetAlphaLabel(newIndex);
            StringBuilder newFormula = new StringBuilder(formula);
            StringBuilder sb = new StringBuilder();
            string[] numbers = Regex.Split(formula, @"\D+");
            int index = 0;
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int temp = formula.IndexOf(value);
                    if (temp > 0 && formula[temp - 1] != 'n')
                    {
                        index += temp;
                        newFormula = newFormula.Replace(oldColumn, newColumn, index - oldColumn.Length, newColumn.Length);
                        formula = formula.Substring(temp + value.Length);
                        index += newIndex.ToString().Length;
                    }
                }
            }
            return newFormula.ToString();
        }

        /// <summary>
        /// Returns whether the input string is null or has zero length.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <returns>True if null or zero length, false otherwise.</returns>
        public static bool IsEmpty(string s)
        {
            return s == null || s.Length == 0;
        }

        private bool NeedToAdjustReferences(GridStyleInfoIdentity id, GridFormulaTag tag)
        {
            return (id.RowIndex != tag.ParsedRow && tag.ParsedRow != -1)
                || (id.ColumnIndex != tag.ParsedCol && tag.ParsedCol != -1);
        }

        GridFormulaEngine engine;

        /// <summary>
        /// Gets the GridFormulaEngine associated with the CellModel. <see cref="GridFormulaEngine"/> encapsulates all 
        /// formula features. Use this object to add / remove library functions.
        /// </summary>
        public GridFormulaEngine Engine
        {
            get { return engine; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                grid.CommittedCellInfo -= new GridCommitCellInfoEventHandler(grid_CommittedCellInfo);
                grid.RowsInserted -= new GridRangeInsertedEventHandler(grid_RowsInserted);
                grid.RowsRemoved -= new GridRangeRemovedEventHandler(grid_RowsRemoved);
                grid.ColumnsInserted -= new GridRangeInsertedEventHandler(grid_ColumnsInserted);
                grid.ColumnsRemoved -= new GridRangeRemovedEventHandler(grid_ColumnsRemoved);
            }
            base.Dispose(disposing);
        }

    }
    #endregion

    #region GridFormulaCellRenderer

    /// <summary>
    /// Implements a GridCellRender that supports formulas.
    /// </summary>
#if WPF
    public class GridCellFormulaRenderer : GridCellTextBoxRenderer
#else
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellFormulaRenderer : GridCellTextBoxCellRenderer
#endif
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellFormulaRenderer"/>.
        /// </summary>
        public GridCellFormulaRenderer()
            : base()
        {
            SupportsRenderOptimization = true;
            AllowRecycle = true;
            IsControlTextShown = true;
            IsFocusable = true;
            model = CellModel as GridCellFormulaModel;
        }

        private GridCellFormulaModel model = null;

        /// <summary>
        /// Overridden to conditionally return either the style.Text property which holds the formula,
        /// or the style.FormattedText property which holds the formatted computed value.
        /// </summary>
        /// <param name="style">The style object.</param>
        /// <returns>Either the formula text or the computed text depending upon the context of the call.</returns>
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
                    //if (style.FormulaTag != null && style.FormulaTag.Formula.Contains("DATE"))
                    //{
                    //    DateTime dt = new DateTime();
                    //    dt = DateTime.FromOADate(double.Parse(text));
                    //    text = dt.ToLongDateString();
                    //}
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
    }
    #endregion

    #endregion


    #region GridFormulaEngine Class


    /// <summary>
    /// Encapsulates the code required to parse and compute formulas. Hashtable
    /// properties maintain a Formula Library of functions as well as a list
    /// of dependent cells.
    /// <para/>
    /// You can add and remove library functions.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridFormulaEngine : IDisposable
    {

        //the grid
        private GridModel grid;

        /// <summary>
        /// The character that indicates a formula.
        /// </summary>
        /// <remarks>
        /// If this character is not 0, the text of a cell
        /// must begin with this character if the cell contains
        /// a formula. Common usage would be to use '=' as the
        /// character to begin a formula in a cell.
        /// 
        /// This value is normally set through the cell model constructor.
        /// </remarks>
        internal char formulaChar;

        //Holds the cell being calculated.. set in CellModel.GetFormattedText.
        internal string cell;

        /// <summary>
        /// Gets or sets the cell (in the column-row notation of A1 or E21) whose 
        /// formula is being parsed or computed.
        /// </summary>
        /// <remarks>
        /// This property should be set only if you are directly parsing and 
        /// computing formulas by calling the Parse or ComputedValue methods. In this 
        /// case, you should set the property before calling either method. FormulaContextCell
        /// is used to provide support for CurrentRowNotation, circular calculation checks,
        /// and reference updating.
        /// </remarks>
        public string FormulaContextCell
        {
            get { return cell; }
            set { cell = value; }
        }

        //Used to determine if this GridFormulaEngine instance is a member of
        //several sheets. If so, dependent cells are tracked through a static member
        //so that they are known across instances.
        internal bool IsSheeted
        {
            get
            {
                GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                return (family == null)
                    ? false : family.isSheeted;
            }
        }


        private bool supportBlanksInSheetNames = true;

        /// <summary>
        /// Gets or sets whether blanks are treated as significant in sheet names.
        /// </summary>
        /// <remarks>
        /// Earlier versions of GridFormulaEngine did not support the use of blanks
        /// in sheet names. (They were treated as insignificant.) The current version
        /// does treat blanks as significant. If you want to revert to the prior
        /// treatment of blanks in sheet names, then set this property to false.
        /// </remarks>
        public bool SupportBlanksInSheetNames
        {
            get { return supportBlanksInSheetNames; }
            set { supportBlanksInSheetNames = value; }
        }



        private bool fixedReferenceOnlyOnPaste = false;

        /// <summary>
        /// Determines whether fixed references should be adjusted on other than paste.
        /// </summary>
        public bool FixedReferenceOnlyOnPaste
        {
            get { return fixedReferenceOnlyOnPaste; }
            set { fixedReferenceOnlyOnPaste = value; }
        }

        [ThreadStaticAttribute]
        private static Hashtable sheetFamiliesList = null;

        [ThreadStaticAttribute]
        private static int sheetFamilyID = 0;

        [ThreadStaticAttribute]
        private static GridSheetFamilyItem defaultFamilyItem = null;

        /// <summary>
        /// Returns the GridSheetFamilyItem for the specified model. If there was no item registered for 
        /// the model, a new item is created and cached.
        /// </summary>
        /// <param name="model">The grid model.</param>
        /// <returns>The GridSheetFamilyItem for the specified model.</returns>
        public static GridSheetFamilyItem GetSheetFamilyItem(GridModel model)
        {
            if (sheetFamilyID == 0)
            {
                if (defaultFamilyItem == null)
                    defaultFamilyItem = new GridSheetFamilyItem();
                return defaultFamilyItem;
            }

            if (sheetFamiliesList == null)
                sheetFamiliesList = new Hashtable();

            int i = (int)modelToSheetID[model];

            if (sheetFamiliesList[i] == null)
            {
                sheetFamiliesList.Add(i, new GridSheetFamilyItem());
            }
            return sheetFamiliesList[i] as GridSheetFamilyItem;
        }


        private Hashtable dependentCells = null;

        /// <summary>
        /// Contains a mapping between a cell and a list of formula cells that depend on it.
        /// </summary>
        /// <remarks>
        /// The key is the given cell, and the value is a Hashtable of cells containing
        /// formulas that reference this cell.
        /// </remarks>
        /// <example> Here is code that will list formula cells affected by changing the given cell.
        /// <code lang="C#">
        ///	public void DisplayAllAffectedCells()
        ///	{
        ///		GridFormulaEngine engine = ((GridCellFormulaModel)this.gridControl1.Model.CellModels["FormulaCell"]).Engine;
        ///	
        ///		foreach(object o in engine.DependentCells.Keys)
        ///		{
        ///			string s1 = o as string;
        ///			Console.Write(s1 + " affects ");
        ///			Hashtable ht = (Hashtable) engine.DependentCells[s1];
        ///			foreach(object o1 in ht.Keys)
        ///			{
        ///				string s2 = o1 as string;
        ///				Console.Write(s2 + " ");
        ///			}
        ///			Console.WriteLine("");
        ///		}
        ///	}
        /// </code>
        /// <code lang="VB">
        ///		Public Sub DisplayAllAffectedCells()
        ///			Dim engine As GridFormulaEngine = CType(Me.gridControl1.Model.CellModels("FormulaCell"), GridCellFormulaModel).Engine
        ///			Dim o As Object
        ///			For Each o In  engine.DependentCells.Keys
        ///				Dim s1 As String = CStr(o)
        ///				Console.Write((s1 + " affects "))
        ///				Dim ht As Hashtable = CType(engine.DependentCells(s1), Hashtable)
        ///				Dim o1 As Object
        ///				For Each o1 In  ht.Keys
        ///					Dim s2 As String = CStr(o1)
        ///					Console.Write((s2 + " "))
        ///				Next o1
        ///				Console.WriteLine("")
        ///			Next o
        ///		End Sub 'DisplayAllAffectedCells
        /// </code>
        /// </example>
        public Hashtable DependentCells
        {
            get
            {
                if (IsSheeted)
                {
                    GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                    if (family.sheetDependentCells == null)
                        family.sheetDependentCells = new Hashtable();
                    return family.sheetDependentCells;
                }
                else
                {
                    if (dependentCells == null)
                        dependentCells = new Hashtable();
                    return dependentCells;
                }
            }
        }

        private Hashtable dependentFormulaCells = null;

        /// <summary>
        /// Contains a mapping between a formula cell and a list of cells upon which it depends.
        /// </summary>
        /// <remarks>
        /// The key is the given formula cell, and the value is a Hashtable of cells that this 
        /// formula cell references.
        /// </remarks>
        /// <example> Here is code that lists formula cells affected by changing a given cell:
        /// <code lang="C#">
        ///		public void DisplayAllFormulaDependencies()
        ///		{
        ///			GridFormulaEngine engine = ((GridCellFormulaModel)this.gridControl1.Model.CellModels["FormulaCell"]).Engine;
        ///			
        ///			foreach(object o in engine.DependentFormulaCells.Keys)
        ///			{
        ///				string s1 = o as string;
        ///				Console.Write(s1 + " depends upon ");
        ///				Hashtable ht = (Hashtable) engine.DependentFormulaCells[s1];
        ///				foreach(object o1 in ht.Keys)
        ///				{
        ///					string s2 = o1 as string;
        ///					Console.Write(s2 + " ");
        ///				}
        ///				Console.WriteLine("");
        ///			}
        ///		}
        /// </code>
        /// <code lang="VB">
        ///		Public Sub DisplayAllFormulaDependencies()
        ///			Dim engine As GridFormulaEngine = CType(Me.gridControl1.CellModels("FormulaCell"), GridCellFormulaModel).Engine
        ///   
        ///			Dim o As Object
        ///			For Each o In  engine.DependentFormulaCells.Keys
        ///				Dim s1 As String = CStr(o)
        ///				Console.Write((s1 + " depends upon "))
        ///				Dim ht As Hashtable = CType(engine.DependentFormulaCells(s1), Hashtable)
        ///				Dim o1 As Object
        ///				For Each o1 In  ht.Keys
        ///					Dim s2 As String = CStr(o1) 
        ///					Console.Write((s2 + " "))
        ///				Next o1
        ///				Console.WriteLine("")
        ///			Next o
        ///		End Sub 'DisplayAllFormulaDependencies
        /// </code>
        /// </example>
        public Hashtable DependentFormulaCells
        {
            get
            {
                if (IsSheeted)
                {
                    GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                    if (family.sheetDependentFormulaCells == null)
                        family.sheetDependentFormulaCells = new Hashtable();
                    return family.sheetDependentFormulaCells;
                }
                else
                {
                    if (dependentFormulaCells == null)
                        dependentFormulaCells = new Hashtable();
                    return dependentFormulaCells;
                }
            }
        }

        /// <summary>
        /// Gets / sets the number of recursive checks done
        /// for circular references.
        /// </summary>
        /// <remarks>
        /// When you edit a cell, the engine parsing will attempt
        /// to flag circular references as an error. This value sets the
        /// number of dependent cells it will check before assuming there
        /// is no circular reference. Checking every reference can be 
        /// time consuming depending upon the formulas being used. This
        /// property lets you decide how many recursions the engine will 
        /// allow when looking for circular references. The default value
        /// is -1 meaning no checks are done as you type formula into the 
        /// cell. Setting this property to int.MaxValue will make the 
        /// engine check all dependent cells for a circular reference.
        /// </remarks>
        public int MaximumCircularChecks
        {
            get { return maxCircularChecks; }
            set { maxCircularChecks = value; }
        }

        private bool doCircularCheckInValidating = false;

        /// <summary>
        /// Gets or sets whether circular references should be checked in 
        /// the CurrentCell.Validating event.
        /// </summary>
        public bool DoCircularCheckInValidating
        {
            get { return doCircularCheckInValidating; }
            set { doCircularCheckInValidating = value; }
        }


        //Uses recursion to check for a circular dependence.
        private int maxCircularChecks = -1;
        private int circularCheckCount = 0;
        internal bool IsCircularReference(string cell)
        {
            if (maxCircularChecks == -1)
                return false;

            circularCheckCount = 0;
            Hashtable ht = (Hashtable)DependentCells[cell];
            return DoesHashtableOrChildContain(ht, cell);
        }

        private bool DoesHashtableOrChildContain(Hashtable ht, string cell)
        {
            if (ht == null)
                return false;
            if (ht.ContainsKey(cell))
                return true;

            if (circularCheckCount > maxCircularChecks)
                return false;
            circularCheckCount++;

            //recursivecall
            foreach (string key in ht.Keys)
            {
                Hashtable ht1 = (Hashtable)DependentCells[key];
                if (ht1 != null && DoesHashtableOrChildContain(ht1, cell))
                    return true;
            }
            return false;
        }




        /// <summary>
        /// Displays information on the cell currently being calculated.
        /// </summary>
        /// <returns>String with information on the cell currently being calculated.</returns>
        public override string ToString()
        {
            return "GridFormulaEngine { Cell: " + (cell != null ? cell.ToString() : "null") + " " + (DependentCells != null ? DependentCells.Count : -1).ToString() + "}";
        }

        #region Sheet Support

        [ThreadStaticAttribute]
        internal static int TokenCount = 0;

        [ThreadStaticAttribute]
        internal static Hashtable modelToSheetID = null;

        /// <summary>
        /// Used by the <see cref="ChangeGridSheetName"/> method to decide
        /// what characters can precede a SheetName in a formula. The default
        /// value is "+-/*<>=(,:".
        /// </summary>
        [ThreadStaticAttribute]
        public static string ValidSheetChars = "+-/*<>=(,:";

        internal const char sheetToken = '!';

        /// <summary>
        /// Changes a sheetname that was previously registered using <see cref="RegisterGridAsSheet"/>. 
        /// This method iterates through all the cells in all the sheets in the sheet family, swapping all 
        /// occurrences of the oldName in any formula with the newName.
        /// </summary>
        /// <param name="oldName">The old sheet name.</param>
        /// <param name="newName">The new sheet name.</param>
        /// <param name="grid">The grid model.</param>
        /// <returns>True if the sheet was successfully renamed. If false returns,
        /// check whether the sheet family already contains the new name.</returns>
        public static bool ChangeGridSheetName(string oldName, string newName, GridModel grid)
        {
            bool returnValue = false;
            try
            {
                GridFormulaEngine engine = ((GridCellFormulaModel)grid.CellModels["FormulaCell"]).Engine;
                GridSheetFamilyItem family = GetSheetFamilyItem(grid);
                string oldNameUpper = oldName.ToUpper();
                string newNameUpper = newName.ToUpper();
                string validChars = ValidSheetChars;
                if (engine != null && family != null
                    && family.SheetNameToGridModel.ContainsKey(oldNameUpper)
                    && !family.SheetNameToGridModel.ContainsKey(newNameUpper))
                {
                    //swap out the name mapped to the gridmodel
                    object gridModel = family.SheetNameToGridModel[oldNameUpper];
                    family.SheetNameToGridModel.Remove(oldNameUpper);
                    family.SheetNameToGridModel.Add(newNameUpper, gridModel);

                    //swap out the name mapped to the sheet Token
                    object token = family.SheetNameToToken[oldNameUpper];
                    family.SheetNameToToken.Remove(oldNameUpper);

                    family.TokenToSheetName.Remove(token);

                    family.SheetNameToToken.Add(newNameUpper, token);
                    family.TokenToSheetName.Add(token, newName);
                    //swap out the name in the sized sheet name list
                    family.sheetNamesSized.Remove(oldNameUpper);
                    family.sheetNamesSized.Add(newNameUpper);
                    family.sheetNamesSized.Sort(new LenComparer());

                    oldNameUpper += GridFormulaEngine.sheetToken;
                    newName += GridFormulaEngine.sheetToken;

                    //now loop thru all the sheets and all the cells replacing
                    //the name in the formulas. Note that formulaTags should not
                    //be affected. Only the formula text stored in the cell's GridStyleInfo.
                    foreach (GridModel model in family.SheetNameToGridModel.Values)
                    {
                        GridCellData data = model.Data;
                        for (int row = 1; row <= model.RowCount; ++row)
                        {
                            for (int col = 1; col <= model.ColumnCount; ++col)
                            {
                                if (data[row, col] != null)
                                {
                                    GridStyleInfo style = new GridStyleInfo(data[row, col]);
                                    string text = style.Text;
                                    string textUpper = text.ToUpper();
                                    if (text.Length > 0 // && style.CellType == "FormulaCell"
                                        && text[0] == engine.formulaChar)
                                    {
                                        int i = textUpper.Length;
                                        bool changed = false;
                                        while (i > -1 && (i = textUpper.Substring(0, i).LastIndexOf(oldNameUpper)) > -1)
                                        {
                                            int startI = i;
                                            int j = i + oldNameUpper.Length;
                                            i--;
                                            int saveI = i;
                                            while (i > -1 && textUpper[i] == ' ')
                                                i--;
                                            if (i > -1 && (i == 0 || validChars.IndexOf(textUpper[i]) > -1))
                                            {
                                                text = text.Substring(0, saveI + 1) + newName + text.Substring(j);
                                                changed = true;
                                                // i = j;
                                            }
                                            else if (i > -1)
                                                i = startI - 1;
                                        }
                                        if (changed)
                                        {
                                            style.Text = text;
                                            data[row, col] = style.Store;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    returnValue = true;
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        /// <summary>
        /// Registers a grid so it can be referenced in a formula from another grid.
        /// </summary>
        /// <param name="refName">The reference name used to refer to this grid from formulas in other grids.
        /// </param>
        /// <param name="model">The GridModel from the grid being registered.
        /// </param>
        /// /// <param name="sheetFamilyID">An integer previously created with a call 
        /// to GridFormulaEngine.CreateSheetFamilyID. This number is used to identify
        /// the grids as belonging to a particular family of grids. You can only reference
        /// grids from within the same family.
        /// </param>
        /// <remarks>
        /// Essential Grid supports multisheet references with its formulas. For example, if you
        /// have two tabpages with a GridControl on each, you can reference cells from the first
        /// in the second grid. For this to work, both grids need to be registered using this method.
        /// 
        /// The syntax for using a sheet reference as part of a formula is to prefix a cell reference 
        /// with the sheet reference name followed by an exclamation point.
        /// 
        /// The formula "= sheet1!A1 + sheet2!C3" would add the value of cell A1 for 
        /// the grid whose reference name is sheet1 to the value from cell C3 in the grid 
        /// whose reference name is sheet2.
        ///  
        /// </remarks>
        /// <example> Use this code to use cross sheet references.
        /// <code lang="C#">
        ///		//Register 3 grids so cell can be referenced across grids.
        ///		int sheetfamilyID = GridFormulaEngine.CreateSheetFamilyID();
        ///		GridFormulaEngine.RegisterGridAsSheet("summary", this.gridControl1.Model, sheetfamilyID);
        ///		GridFormulaEngine.RegisterGridAsSheet("income", this.gridControl2.Model, sheetfamilyID);
        ///		GridFormulaEngine.RegisterGridAsSheet("expenses", this.gridControl3.Model, sheetfamilyID);
        ///		....
        ///		//Sample formula usage for cells in gridControl1, the 'summary' grid.
        ///		//This code sums up some cells from gridControl3, the 'expenses' grid, 
        ///		//and gridControl2, the 'income' grid.
        ///		
        ///		//Sum the range B2:B8 from the expenses grid.
        ///		this.gridControl1[3,4].Text = "= Sum(expenses!B2:expenses!B8)";
        ///
        ///		//Sum the range B2:B4 from the income grid.
        ///		this.gridControl1[4,4].Text = "= Sum(income!B2:income!B4)";
        ///		
        /// </code>
        /// <code lang="VB">
        ///		'Register 3 grids so cells can be referenced across grids.
        ///		Dim sheetfamilyID As Integer = GridFormulaEngine.CreateSheetFamilyID();
        ///		GridFormulaEngine.RegisterGridAsSheet("summary", Me.gridControl1.Model, sheetfamilyID)
        ///		GridFormulaEngine.RegisterGridAsSheet("income", Me.gridControl2.Model, sheetfamilyID) 
        ///		GridFormulaEngine.RegisterGridAsSheet("expenses", Me.gridControl3.Model, sheetfamilyID) 
        ///		....
        ///		'Sample formula usage for cells in gridControl1, the 'summary' grid.
        ///		'This code sums ups some cells from gridControl3, the 'expenses' grid, 
        ///		'and gridControl2, the 'income' grid.
        ///		
        ///		'Sum the range B2:B8 from the expenses grid.
        ///		Me.gridControl1(3,4).Text = "= Sum(expenses!B2:expenses!B8)"
        ///
        ///		'Sum the range B2:B4 from the income grid.
        ///		Me.gridControl1(4,4).Text = "= Sum(income!B2:income!B4)"
        /// </code>
        /// </example>
        public static void RegisterGridAsSheet(string refName, GridModel model, int sheetFamilyID)
        {
            if (modelToSheetID == null)
            {
                modelToSheetID = new Hashtable();
            }
            if (modelToSheetID[model] == null)
            {
                modelToSheetID.Add(model, sheetFamilyID);
            }

            GridSheetFamilyItem family = GetSheetFamilyItem(model);
            family.isSheeted = true;

            string refName1 = refName.ToUpper();
            if (family.SheetNameToGridModel == null)
            {
                family.SheetNameToGridModel = new Hashtable();
            }

            if (family.TokenToGridModel == null)
            {
                family.TokenToGridModel = new Hashtable();
            }

            if (family.SheetNameToToken == null)
            {
                family.SheetNameToToken = new Hashtable();
            }
            if (family.TokenToSheetName == null)
            {
                family.TokenToSheetName = new Hashtable();
            }

            if (family.GridModelToToken == null)
            {
                family.GridModelToToken = new Hashtable();
            }

            if (family.SheetNameToGridModel.ContainsKey(refName1))
            {
                string token = (string)family.SheetNameToToken[refName1];
                family.TokenToGridModel[token] = model;
                family.GridModelToToken[model] = token;
                family.TokenToSheetName[token] = refName;
            }
            else
            {
                string token = sheetToken + TokenCount.ToString() + sheetToken;
                TokenCount++;
                family.TokenToGridModel.Add(token, model);
                family.SheetNameToToken.Add(refName1, token);
                family.SheetNameToGridModel.Add(refName1, model);
                family.GridModelToToken.Add(model, token);
                family.TokenToSheetName[token] = refName;

                //model.CellsChanged += new GridCellsChangedEventHandler(GridCellsChanged);
                model.CommittedCellInfo += new GridCommitCellInfoEventHandler(grid_CommittedCellInfo);
            }
            //need a list of names sorted by string length to simplify searches
            if (family.sheetNamesSized == null)
            {
                family.sheetNamesSized = new List<string>();
            }
            family.sheetNamesSized.Add(refName1);
            family.sheetNamesSized.Sort(new LenComparer());
        }



        /// <summary>
        /// Returns an integer that is used to identify a family of grids.
        /// </summary>
        /// <remarks>
        /// Essential Grid supports multisheet references within a family of grids. To use
        /// this functionality, you employ the method to get a unique identifier for
        /// the family. Then in the RegisterGridAsSheet method that you call to add grids
        /// to this family, you pass this unique identifier to mark the grids as belonging
        /// to this family. You can only cross reference grids within the same family.
        /// </remarks>
        /// <returns>Sheet family id.</returns>
        public static int CreateSheetFamilyID()
        {
            if (sheetFamilyID == int.MaxValue)
                sheetFamilyID = int.MinValue;

            return sheetFamilyID++;
        }

        private static void grid_CommittedCellInfo(object sender, GridCommitCellInfoEventArgs e)
        {
            // Console.WriteLine("{0}  {1}", e.Cell, grid[1, 1].CellValue);
            //RefreshCells(e.Cell.RowIndex, e.Cell.ColumnIndex);
            //Each item when we commit the Grid Style info property, it will invalidate the dependent cells
            if (e.Sip != null && e.Sip.PropertyName != "CellValue")
            {
                return;
            }
            GridModel model = sender as GridModel;
            GridSheetFamilyItem family = GetSheetFamilyItem(model);
            if (model != null)
            {
                string token = family.GridModelToToken[sender] as string;
                GridFormulaEngine engine = ((GridCellFormulaModel)model.CellModels["FormulaCell"]).Engine;
                //    Console.WriteLine("  DependentCells{0}  DependentFormulaCells{1}   refreshedCells{2}", engine.DependentCells.Count, engine.DependentFormulaCells.Count, engine.refreshedCells.Count);
                int row = e.Cell.RowIndex;
                int col = e.Cell.ColumnIndex;
                string s = token + GridRangeInfo.GetAlphaLabel(col) + row.ToString();
                engine.Refresh(s);
                //   Console.WriteLine("  DependentCells{0}  DependentFormulaCells{1}   refreshedCells{2}", engine.DependentCells.Count, engine.DependentFormulaCells.Count, engine.refreshedCells.Count);

            }
        }


        /// <summary>
        /// Unregisters a grid so it can no longer be referenced in a formula from another grid.
        /// </summary>
        /// <param name="refName">The reference name used to refer to this grid from formulas in other grids.
        /// </param>
        /// <param name="model">The grid model.</param>
        public static void UnregisterGridAsSheet(string refName, GridModel model)
        {
            GridSheetFamilyItem family = GetSheetFamilyItem(model);
            string refName1 = refName.ToUpper();
            if (family.SheetNameToGridModel != null && family.SheetNameToGridModel.ContainsKey(refName1))
            {
                family.SheetNameToGridModel.Remove(refName1);
                string token = (string)family.SheetNameToToken[refName1];

                if (model != null)
                    model.CommittedCellInfo -= new GridCommitCellInfoEventHandler(grid_CommittedCellInfo);

                family.SheetNameToToken.Remove(refName1);
                family.TokenToGridModel.Remove(token);
                family.TokenToSheetName.Remove(refName);
                modelToSheetID.Remove(model);
                family.GridModelToToken.Remove(model);

            }
        }

        private void PutTokensForSheets(ref string text)
        {
            GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
            if (family.sheetNamesSized != null)
            {
                if (!text.Contains(":"))
                {

                    foreach (string name in family.sheetNamesSized)
                    {
                        string token = (string)family.SheetNameToToken[name];
                        string s = name.ToUpper() + sheetToken;
                        text = text.Replace(s, token);
                    }
                }
                else
                {
                    foreach (string name in family.sheetNamesSized)
                    {
                        string token = (string)family.SheetNameToToken[name];
                        string s = name.ToUpper();
                        text = text.Replace(s, token);
                    }
                    text = text.Replace("!!", "!");
                }
            }
        }

        #endregion

        [ThreadStaticAttribute]
        private static bool yesToCloning = false;

        /// <summary>
        /// Gets / sets whether FormulaTags are cloned when
        /// setting one FormulaTag object equal to another. 
        /// </summary>
        /// <remarks>
        /// This is by default set to False as normally the FormulaEngine
        /// expects to be working with referenced objects, and not clones 
        /// of referenced objects.
        /// </remarks>
        public static bool CloneableFormulaTags
        {
            get { return yesToCloning; }
            set
            {
                if (value != yesToCloning)
                {
                    yesToCloning = value;
                    GridStyleInfoStore.FormulaTagProperty.IsCloneable = yesToCloning;
                    GridStyleInfoStore.FormulaTagProperty.IsDisposable = yesToCloning;
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gridModel">The GridModel from the underlying grid.</param>
        public GridFormulaEngine(GridModel gridModel)
        {
            GridStyleInfoStore.FormulaTagProperty.IsCloneable = GridFormulaEngine.yesToCloning;
            GridStyleInfoStore.FormulaTagProperty.IsDisposable = GridFormulaEngine.yesToCloning;

            grid = gridModel;

            this.InitLibraryFunctions();

            formulaChar = (char)0;

            tokens = new char[]{TOKEN_add,
                                   TOKEN_subtract,
                                   TOKEN_multiply,
                                   TOKEN_divide, 
                                   TOKEN_less,
                                   TOKEN_greater,
                                   TOKEN_equal,
                                   TOKEN_lesseq,
                                   TOKEN_greatereq,
                                   TOKEN_noequal};

            refreshedCells = new Hashtable();
        }

        private bool useVirtualDataSource = true;

        /// <summary>
        /// Indicates whether the underlying data is virtually bound to the grid.
        /// </summary>
        /// <remarks>If the formula engine knows the data is stored within a GridControl.Data
        /// object, it can optimize data access. If the grid is being populated through
        /// virtual techniques (handling QueryCellInfo), the formula engine has to get the data 
        /// through that means. The default value is to assume the data is coming from a virtual
        /// datasource. There are some other situations, like formulas in headers or frozen cells, when
        /// this property should be set to True.
        /// </remarks>
        public bool UsesVirtualDataSource
        {
            get { return useVirtualDataSource; }
            set { useVirtualDataSource = value; }
        }


        private bool calculationsSuspended = false;

        /// <summary>
        /// Indicates whether formulas are immediately calculated as dependent cells are changed.
        /// </summary>
        /// <remarks>Use this property to suspend calculations while a series of changes 
        /// are made to dependent cells either by the user or programmatically. When the changes are 
        /// complete, set this property to False, and then call Engine.RecalculateRange to recalculate
        /// the affected range. See the sample in GridCellFormulaModel.CalculatingSuspended.
        /// </remarks>
        public bool CalculatingSuspended
        {
            get { return calculationsSuspended; }
            set { calculationsSuspended = value; }
        }

        private int maximumRecursiveCalls = 100;

        /// <summary>
        /// Specifies the maximum number of recursive calls that can be used to compute a cellvalue.
        /// </summary>
        /// <remarks>This property comes into play when you have a calculated formula cell that depends on  
        /// another calculated formula that depends on another calculated formula and so on. If the 
        /// 'depends on another formula' number exceeds MaximumRecursiveCalls, you will see a Too Complex message
        /// displayed in the cell. The default value is 100, but you can set it higher or lower depending upon 
        /// your expected needs. The purpose of the limit is to avoid a circular reference locking up your
        /// application.
        /// </remarks>
        public int MaximumRecursiveCalls
        {
            get { return maximumRecursiveCalls; }
            set { maximumRecursiveCalls = value; }
        }

        private bool currentRowNotationEnabled = true;
        /// <summary>
        /// Enables / disables using row = 0 in formulas to represent the current row.
        /// </summary>
        /// <remarks> 
        /// When this property is set True, entering zero as a row in a formula is
        /// interpreted to be the current row. Using the current row notation allows
        /// you to sort a column in the grid and maintain the relative formula. 
        /// After sorting, you do have to call engine.RecalculateRange to allow the
        /// relative formulas to reset themselves. 
        /// </remarks>
        public bool CurrentRowNotationEnabled
        {
            get { return currentRowNotationEnabled; }
            set { currentRowNotationEnabled = value; }
        }

        #region Refreshing/Recalculating cells

        //Used to prevent infinite refreshes on circular references.
        /* private */
        internal Hashtable refreshedCells = null;
        internal bool isFormulainConditionalFormat = false;
        private int dependencyLevel = 0;

        //////		a debug routine to display recursively display dependent cells
        //////		sample usage at start of next routine...
        //////		private int indent = 0;
        //////		private void DisplayAffectedCells(string s)
        //////		{
        //////			string spc = new String('_', 4 * indent);
        //////			indent++;
        //////			Console.Write(spc + s + " affects ");
        //////			Hashtable ht = (Hashtable) this.DependentCells[s];
        //////			if(ht != null)
        //////			{
        //////				foreach(object o1 in ht.Keys)
        //////				{
        //////					string s2 = o1 as string;
        //////					Console.Write(s2 + " ");
        //////				}
        //////				Console.WriteLine("");
        //////				foreach(object o1 in ht.Keys)
        //////				{
        //////					string s2 = o1 as string;
        //////					DisplayAffectedCells(s2);
        //////				}
        //////			}
        //////			else
        //////				Console.WriteLine("");
        //////
        //////			indent--;
        //////		}


        /// <summary>
        /// Recalculates any cell that depends upon the passed in cell.
        /// </summary>
        /// <param name="s">A cell such as A21 or EE31.</param>
        public void Refresh(string s)
        {
            //Console.WriteLine("  dependencyLevel{0}", dependencyLevel);

            // 			if(s=="D2")
            // 			{
            // 				DisplayAffectedCells(s);
            // 			}

            if (CalculatingSuspended) //don't refresh any cells
                return;

            if (dependencyLevel == 0)
            {
                refreshedCells.Clear();
            }
            if (dependencyLevel > MaximumRecursiveCalls)
            {
                dependencyLevel--;
                return; // Possible circular dependency or simply too complex.
            }

            if (DependentCells[s] != null)
            {
                //Track the cells that have been refreshed since initial call to Refresh.

                if (!refreshedCells.ContainsKey(s))
                {
                    refreshedCells.Add(s, "");
                }

                dependencyLevel++;

                try
                {
                    GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                    Hashtable ht = (Hashtable)DependentCells[s];
                    foreach (object o in ht.Keys)
                    {
                        string s1 = o as string;
                        if (s1 != null)
                        {
                            GridModel grd = grid;
                            string sheet = SheetToken(s1);
                            if (sheet.Length > 0)
                                grid = family.TokenToGridModel[sheet] as GridModel;
                            int row = RowIndex(s1);
                            int col = ColIndex(s1);

                            GridStyleInfo style = UsesVirtualDataSource
                                ? grid[row, col]
                                : new GridStyleInfo(grid.Data[row, col]);

                            GridFormulaTag tag;
                            GridFormulaTag tag2 = null;
                            try
                            {
                                tag = style.FormulaTag;
                                if (style.HasConditionalFormat)
                                    tag2 = style.ConditionalFormat.FormulaTag;
                            }
                            catch
                            {
                                this.grid = grd;
                                throw new NullReferenceException(FormulaErrorStrings[virtual_mode_required]);
                            }
                            if (tag != null)
                            {
                                SetDirty(tag);// tag.Text = "";
                                if (!refreshedCells.ContainsKey(s1))
                                {
                                    Refresh(s1); //recursive call
                                }
                            }
                            if (tag2 != null)
                            {
                                SetDirty(tag2);
                            }
                            grid.InvalidateCell(GridRangeInfo.Cell(row, col));
                            foreach (GridControlBase g in grid.Views)
                            {
#if !SILVERLIGHT
                                g.InvalidateVisual(false); //clay
#else
                                g.InvalidateVisual(true);
#endif
                            }

                            this.grid = grd;
                        }
                    }
                }
                finally
                {
                    dependencyLevel--;
                    if (dependencyLevel == 0)
                    {
                        refreshedCells.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Recalculates every cell that depends upon any cell in the passed-in range.
        /// </summary>
        /// <remarks>For example, if range is GridRangeInfo(1,1,2,2), and cells (5,6) and
        /// (12,17) hold formulas that reference the cells in the range, then cells (5,6) 
        /// and (12,17) will be re-computed as the result of this call.</remarks>
        /// <param name="range">GridRangeInfo object to be refreshed.</param>
        public void RefreshRange(GridRangeInfo range)
        {
            range = range.ExpandRange(1, 1, this.grid.RowCount, this.grid.ColumnCount);

            //Loop through the range refreshing the cells...
            for (int r = range.Top; r <= range.Bottom; r++)
            {
                for (int c = range.Left; c <= range.Right; c++)
                {
                    string s = GridRangeInfo.GetAlphaLabel(c) + GridRangeInfo.GetNumericLabel(r);
                    this.Refresh(s);
                }
            }
        }

        /// <summary>
        /// Recalculates any formula cells in the specified range.
        /// </summary>
        /// <remarks>The calculations for non-visible formula cells are performed the next time 
        ///  cell are actually displayed. If you want the calculation performed immediately 
        ///  on cells (visible or not), call the two argument overload of RecalculateRange, 
        ///  passing the forceCalculations argument as True.</remarks>
        /// <param name="range">GridRangeInfo object that specifies the cells to be recalculated.</param>
        public void RecalculateRange(GridRangeInfo range)
        {
            this.RecalculateRange(range, false);
        }


        /// <summary>
        /// Recalculates any formula cells in the specified range.
        /// </summary>
        /// <param name="range">GridRangeInfo object that specifies the cells to be recalculated.</param>
        /// <param name="forceCalculations">Determines whether the calculations on non-visible
        /// cells are performed immediately or delayed until the next time the cell is drawn.
        /// For visible cells, the calculations are done immediately.</param>
        public void RecalculateRange(GridRangeInfo range, bool forceCalculations)
        {
            this.RecalculateRange(range, forceCalculations, false);
        }

        /// <summary>
        /// Recalculates any formula cells in the specified range.
        /// </summary>
        /// <param name="range">GridRangeInfo object that specifies the cells to be recalculated.</param>
        /// <param name="forceCalculations">Determines whether the calculations on non-visible
        /// cells are performed immediately or delayed until the next time the cell is drawn.
        /// For visible cells, the calculations are done immediately.</param>
        /// <param name="forceParsing">When forceParsing is False, a formula is only 
        /// re-parsed if FormulaTag is NULL, or FormulaTag.Formula is empty. Otherwise, the existing
        /// parsed formula in FormulaTag.Formula is used to perform the calculation. The value
        /// of forceParsing only affects the Engine if forceCalculations is True.</param>
        /// <remarks> This method does not do anything if <see cref="CalculatingSuspended"/> is true.
        /// </remarks>
        public void RecalculateRange(GridRangeInfo range, bool forceCalculations, bool forceParsing)
        {
            RecalculateRange(range, this.grid, forceCalculations, forceParsing);
        }

        /// <summary>
        /// Recalculates any formula cells in the specified range.
        /// </summary>
        /// <param name="range">GridRangInfo object that specifies the cells to be recalculated.</param>
        /// <param name="grd">The GridModel object where the range to be updated is located.</param>
        /// <param name="forceCalculations">Determines whether the calculations on non-visible
        /// cells are performed immediately or delayed until the next time the cell is drawn.
        /// For visible cells, the calculations are done immediately.</param>
        /// <param name="forceParsing">When forceParsing is False, a formula is only 
        /// re-parsed if FormulaTag is NULL, or FormulaTag.Formula is empty. Otherwise, the existing
        /// parsed formula in FormulaTag.Formula is used to perform the calculation. The value
        /// of forceParsing only affects the Engine if forceCalculations is True.</param>
        /// <remarks> This method does not do anything if <see cref="CalculatingSuspended"/> is True.
        /// </remarks>
        public void RecalculateRange(GridRangeInfo range, GridModel grd, bool forceCalculations, bool forceParsing)
        {
            if (CalculatingSuspended) //Don't refresh any cells.
                return;

            GridModel saveGrid = this.grid;
            this.grid = grd;

            bool b = this.grid.IgnoreReadOnly;
            this.grid.IgnoreReadOnly = true;
            range = range.ExpandRange(1, 1, this.grid.RowCount, this.grid.ColumnCount);

            GridCellData data = this.grid.Data;
            for (int r = range.Top; r <= range.Bottom; r++)
            {
                for (int c = range.Left; c <= range.Right; c++)
                {
                    if (data[r, c] == null && !UsesVirtualDataSource)
                        continue;

                    GridStyleInfo style = UsesVirtualDataSource
                        ? this.grid[r, c]
                        : new GridStyleInfo(new GridStyleInfo(data[r, c]));

                    if (style.Store != null && style.CellModel is GridCellFormulaModel)
                    {
                        string s = style.Text;
                        GetFormulaText(ref s);

                        bool usesFormulaChar = this.formulaChar != (char)0 && s.Length > 0 && s[0] == this.formulaChar;
                        if (this.formulaChar == (char)0 || usesFormulaChar)
                        {
                            if (usesFormulaChar)
                                s = s.Substring(1); //strip it off

                            //Only force a parse if Formula not in Tag.
                            string formula = (!forceParsing && style.FormulaTag != null) ? style.FormulaTag.Formula : "";

                            if (forceCalculations)
                            {
                                style.FormulaTag = null;

                                this.cell = GridRangeInfo.GetAlphaLabel(c) + r.ToString();

                                if (formula.Length == 0 && s.Length > 0)
                                    formula = this.Parse(s);

                                string text = this.ComputedValue(formula);
                                style.FormulaTag = new GridFormulaTag(formula, text, r, c);
                            }
                            if (!UsesVirtualDataSource)
                                data[r, c] = (GridStyleInfoStore)style.Store;
                        }
                    }

                }
            }
            this.grid.IgnoreReadOnly = b;

            foreach (GridControlBase g in grid.Views)
            {
                g.InvalidateCell(new CellSpanInfoBase(0, 0, grid.RowCount, grid.ColumnCount));
                g.InvalidateVisual();
            }
            this.grid = saveGrid;
        }

        #endregion

        #region adjustments for inserting/deleting rows / columns

        //Adjust references for inserting or removing rows or columns.

        internal bool AdjustReferencesForRangeChange(GridModel grd, bool isCols, bool isInsert, int start, int end, bool isDelete)
        {

            GridModel saveGrid = this.grid;

            this.grid = grd;

            //Turn off undo while we possibly update references...


            string gridToken = (IsSheeted) ? GetSheetFamilyItem(this.grid).GridModelToToken[this.grid].ToString() : "";
            string sheetName = (IsSheeted) ? GetSheetFamilyItem(this.grid).TokenToSheetName[gridToken].ToString().ToUpper() : "";

            //Generate a list of formula cells from dependent cells.
            Hashtable formulaList = new Hashtable();
            foreach (string s in DependentCells.Keys)
            {
                //Check if row / col needs checking - i.e. past insertion or deletion.
                // Continue if no check necessary.
                string sheet = SheetToken(s);

                if ((isCols && this.ColIndex(s) < start)
                    || (!isCols && this.RowIndex(s) < start)
                    || gridToken != sheet
                    )
                {
                    continue;
                }

                //ht holds the formula cells that depend upon cell s
                //Check formulaList to make sure it holds each formula cell.
                //formulaList is dynamically created hashtable.
                Hashtable ht = (Hashtable)DependentCells[s];
                foreach (object o in ht.Keys)
                {
                    string s1 = o as string;
                    if (!formulaList.ContainsKey(s1))
                        formulaList.Add(s1, "");
                }
            }

            foreach (string s in formulaList.Keys)
            {

                GridModel grd1 = this.grid;
                string sheet = SheetToken(s);
                if (sheet.Length > 0)
                {
                    grd1 = GetSheetFamilyItem(this.grid).TokenToGridModel[sheet] as GridModel;
                }


                string currentSheetName = (IsSheeted) ? GetSheetFamilyItem(this.grid).TokenToSheetName[sheet].ToString().ToUpper() : "";

                int row = this.RowIndex(s);
                int col = this.ColIndex(s);
                int offSet = isInsert ? end - start + 1
                    : start - end - 1;
                //if adjustment is in the same grid, need to adjust for inserted rows/columns
                if (sheet == gridToken || gridToken == "")
                {
                    if (isCols)
                    {
                        col += (col < start) ? 0 : offSet;
                    }
                    else
                    {
                        row += (row < start) ? 0 : offSet;
                    }
                }
                string oldFormula = grd1[row, col].Text;
                string newFormula = isCols ? this.AdjustReferences(oldFormula, 0, offSet, -1, start, sheetName, currentSheetName, isDelete)
                    : this.AdjustReferences(oldFormula, offSet, 0, start, -1, sheetName, currentSheetName, isDelete);
                if (oldFormula != newFormula)
                {
                    bool b = grd1.IgnoreReadOnly;
                    grd1.IgnoreReadOnly = true;
                    grd1[row, col].Text = newFormula;
                    grd1[row, col].FormulaTag = null;

                    grd1.InvalidateCell(new RowColumnIndex(row, col));
                    foreach (GridControlBase g in grd1.Views)
                        g.InvalidateVisual(true);
                    //need to refresh the cell here
                    //GridRangeInfo range = GridRangeInfo.Auto(row, col);
                    //if (!grd1.ActiveGridView.ViewLayout.VisibleCellsRange.Contains(range) || grd1.Updating)
                    //{
                    //    string save = this.cell;
                    //    this.cell = GridRangeInfo.GetAlphaLabel(col) + row.ToString();
                    //    string s2 = this.Parse(newFormula);
                    //    grd1[row, col].FormulaTag = new GridFormulaTag(s2, this.ComputedValue(s2), row, col);
                    //    if (this.ForceSaveCellInfo)
                    //    {
                    //        grd1.RaiseSaveCellInfo(new GridSaveCellInfoEventArgs(row, col, grd1[row, col], StyleModifyType.Override));
                    //    }
                    //    this.cell = save;
                    //}
                    grd1.IgnoreReadOnly = b;

                }
            }

            //if (0 != (this.FormulaCopyFlags & GridFormulaCopyFlags.NamedRangeReferencesUpdated))
            //{
            //    //The idea is to put the named range into this formula, "=sum(namedrange)"
            //    //and then adjust this formula for the inserted / deleted row / col. Finally,
            //    //the changed named range is put back into the collections.

            //    Hashtable changes = new Hashtable();
            //    foreach (string s in this.NamedRanges.Keys)
            //    {
            //        string val = this.NamedRanges[s].ToString();
            //        string oldFormula = string.Format("=sum({0})", val);
            //        int offSet = isInsert ? end - start + 1
            //            : start - end - 1;
            //        string newFormula = isCols ? this.AdjustReferences(oldFormula, 0, offSet, -1, start, sheetName, "")
            //            : this.AdjustReferences(oldFormula, offSet, 0, start, -1);
            //        if (oldFormula != newFormula)
            //        {
            //            newFormula = newFormula.Substring(5, newFormula.Length - 6);
            //            changes.Add(s, newFormula);
            //        }
            //    }
            //    foreach (string s in changes.Keys)
            //    {
            //        this.NamedRanges[s] = changes[s];
            //        Hashtable ht = (Hashtable)DependentNamedRangeCells[s];
            //        if (ht != null)
            //        {
            //            foreach (string s1 in ht.Keys)
            //            {
            //                string cell1 = s1;
            //                int i = s1.LastIndexOf(sheetToken);
            //                int row, col;
            //                GridModel grd1 = grid;
            //                if (i > -1)
            //                {
            //                    GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);

            //                    this.grid = (GridModel)family.TokenToGridModel[cell1.Substring(0, i + 1)];
            //                    row = RowIndex(cell1);
            //                    col = ColIndex(cell1);
            //                }
            //                else
            //                {
            //                    row = RowIndex(cell1);
            //                    col = ColIndex(cell1);
            //                }
            //                bool b = grd1.Model.IgnoreReadOnly;
            //                grd1.Model.IgnoreReadOnly = true;
            //                this.grid[row, col].FormulaTag = null;
            //                if (this.ForceSaveCellInfo)
            //                {
            //                    GridRangeInfo range = GridRangeInfo.Auto(row, col);
            //                    if (!grid.ActiveGridView.ViewLayout.VisibleCellsRange.Contains(range))
            //                    {
            //                        this.grid.RaiseSaveCellInfo(new GridSaveCellInfoEventArgs(row, col, grid[row, col], StyleModifyType.Override));
            //                    }
            //                }
            //                grd1.Model.IgnoreReadOnly = b;

            //                this.grid = grd1;
            //            }
            //        }
            //    }
            //}

            this.grid = saveGrid;

            return true;
        }

        /// <summary>
        /// Helper method for adjusting formulas.
        /// </summary>
        /// <param name="origText">Valid unparsed formula string.</param>
        /// <param name="rowOffset">Row offset adjusmtent to be made to the origText.</param>
        /// <param name="colOffset">Column offset adjustment to be made to the origText</param>
        /// <returns>Unparsed formula string derived from origText by adjusting the row 
        /// and column references.
        /// </returns>
        /// <remarks>
        /// For example, calling AdjustReferences("=A4+C4", 2, 1) returns the string "=B6+D6".
        /// This is a helper method that is used to adjust formula references for inserted and
        /// deleted rows and columns. Normally, you would not need this method unless you 
        /// are managing formulas outside the GridControl as in a virtual GridControl or a 
        /// GridDataBoundGrid. This AdjustReferences implementation does not support 
        /// updating references with sheet names within the formulas.</remarks>
        public string AdjustReferences(string origText, int rowOffset, int colOffset)
        {
            return AdjustReferences(origText, rowOffset, colOffset, -1, -1);
        }

        /// <summary>
        /// Helper method for adjusting formulas.
        /// </summary>
        /// <param name="origText">Valid unparsed formula string.</param>
        /// <param name="rowOffset">Row offset adjusmtent to be made to the origText.</param>
        /// <param name="colOffset">Column offset adjustment to be made to the origText</param>
        /// <param name="sheetNameWhereCopied">The name of the sheet where the copy took place.</param>
        /// <param name="currentSheetName">The name of the sheet where the cell holding origText is located.</param>
        /// <returns>Unparsed formula string derived from origText by adjusting the row 
        /// and column references.
        /// </returns>
        /// <remarks>
        /// This AdjustReferences implementation does support 
        /// updating references with sheet names within the formulas.</remarks>
        public string AdjustReferences(string origText, int rowOffset, int colOffset, string sheetNameWhereCopied, string currentSheetName)
        {
            return AdjustReferences(origText, rowOffset, colOffset, -1, -1, sheetNameWhereCopied, currentSheetName);
        }

        private string AdjustReferences(string origText, int rowOffset, int colOffset, int rowCut, int colCut)
        {
            return AdjustReferences(origText, rowOffset, colOffset, rowCut, colCut, "", "");
        }

        internal bool inPaste = false;
        private bool CheckNeedToAdjust(string sheetName, string requiredSheetName, string currentSheetName)
        {
            return inPaste || requiredSheetName == sheetName || (currentSheetName == requiredSheetName && sheetName == "");
        }
        private string AdjustReferences(string origText, int rowOffset, int colOffset, int rowCut, int colCut, string requiredSheetName, string currentSheetName)
        {
            return AdjustReferences(origText, rowOffset, colOffset, rowCut, colCut, requiredSheetName, currentSheetName, false);
        }
        private string AdjustReferences(string origText, int rowOffset, int colOffset, int rowCut, int colCut, string requiredSheetName, string currentSheetName, bool isDelete)
        {
            string s = "";
            string upperText = origText.ToUpper();
            string sheetName = "";

            requiredSheetName = requiredSheetName.ToUpper();
            currentSheetName = currentSheetName.ToUpper();

            bool needToAdjust = CheckNeedToAdjust(sheetName, requiredSheetName, currentSheetName);

            int pos = 0;
            int len = origText.Length;

            while (pos < len)
            {
                if (upperText[pos] == STRING_fixedreference[0] && (inPaste || !FixedReferenceOnlyOnPaste))
                {//skip $'s on cols
                    s += origText[pos];
                    pos++;
                    while (pos < len && char.IsLetter(upperText, pos))
                    {
                        s += origText[pos];
                        pos++;
                    }
                    if (pos < len && upperText[pos] == STRING_fixedreference[0])
                    {//skip $'s on rows
                        s += origText[pos];
                        pos++;
                        while (pos < len && char.IsDigit(upperText, pos))
                        {
                            s += origText[pos];
                            pos++;
                        }
                    }
                    else if (pos < len)
                    { //no $ on row
                        int row = 0;
                        while (pos < len && char.IsDigit(upperText, pos))
                        {
                            row = 10 * row + int.Parse(upperText[pos].ToString());
                            pos++;
                        }
                        if (row >= rowCut && needToAdjust)
                            row = Math.Max(row + rowOffset, this.grid.HeaderRows);
                        s += row.ToString();
                    }

                }
                else if (char.IsLetter(upperText, pos))
                {
                    int loopStart = pos;
                    pos++;
                    while (pos < len && (char.IsLetter(upperText, pos) || upperText[pos] == ' ' || upperText[pos] == '_'))
                    {
                        pos++;
                    }

                    if (pos < len && upperText[pos] == STRING_fixedreference[0])
                    { //handle no $ on col but $ on row
                        string cellRef = upperText.Substring(loopStart, pos - loopStart) + "1";
                        int col = this.ColIndex(cellRef);

                        if (col >= colCut && needToAdjust)
                            col = Math.Max(col + colOffset, this.grid.HeaderColumns);

                        s += GridRangeInfo.GetAlphaLabel(col);

                        s += origText[pos];
                        pos++;
                        while (pos < len && char.IsDigit(upperText, pos))
                        {
                            s += origText[pos];
                            pos++;
                        }

                    }
                    else if (pos < len && char.IsDigit(upperText, pos))
                    {
                        while (pos < len && char.IsDigit(upperText, pos))
                        {
                            pos++;
                        }

                        string cellRef = upperText.Substring(loopStart, pos - loopStart);
                        int row = this.RowIndex(cellRef);
                        int col = this.ColIndex(cellRef);

                        if ((row >= rowCut && row <= (-rowOffset + rowCut - 1)) || (col >= colCut && col <= (-colOffset + colCut - 1)))
                            return "=#REF!";

                        if (needToAdjust && (row > rowCut || (row == rowCut && !isDelete)))
                            row = Math.Max(row + rowOffset, this.grid.HeaderRows);
                        if (needToAdjust && (col > colCut || (col == colCut && !isDelete)))
                            col = Math.Max(col + colOffset, this.grid.HeaderColumns);

                        s += GridRangeInfo.GetAlphaLabel(col) + row.ToString();
                    }
                    else //Not a cell reference.
                    {
                        sheetName = origText.Substring(loopStart, pos - loopStart);
                        s += sheetName;
                        sheetName = (pos < len && origText[pos] == sheetToken)
#if !WinRT
                                      ? sheetName.ToUpper(CultureInfo.InvariantCulture)
#else
                                      ?sheetName.ToUpper()
#endif
                                      : "";
                        needToAdjust = CheckNeedToAdjust(sheetName, requiredSheetName, currentSheetName);
                    }
                }
                else
                {
                    s += origText[pos];
                    pos++;
                }
            }
            //	Console.WriteLine("out[" + s + "]");
            return s;
        }
        #endregion

        #region Error Messages

        /// <summary>
        /// String array that holds the strings used in error messages within the Formula Engine.
        /// </summary>
        /// <remarks>If you want to change the error messages displayed within the Formula Engine,
        /// you can set the new strings into the appropriate position in the FormulaErrorStrings 
        /// array. You should assign your new
        /// strings to the corresponding positions. 
        /// </remarks>
        /// <example>Here is the code that shows position of each string in FormulaErrorStrings:
        /// <code lang="C#">
        ///		public string[] FormulaErrorStrings = new string[]
        ///		{
        ///			"binary operators cannot start an expression",	//0
        ///			"cannot parse",									//1
        ///			"bad library",									//2
        ///			"invalid char in front of",						//3
        ///			"number contains 2 decimal points",				//4
        ///			"expression cannot end with an operator",		//5
        ///			"invalid characters following an operator",		//6
        ///			"invalid character in number",					//7
        ///			"mismatched parentheses",						//8
        ///			"unknown formula name",							//9
        ///			"requires a single argument",					//10
        ///			"requires 3 arguments",							//11
        ///			"invalid Math argument",						//12
        ///			"requires 2 arguments",							//13
        ///			"bad index",									//14
        ///			"too complex",									//15
        ///			"circular reference: ",							//16
        ///			"missing formula",								//17
        ///			"improper formula",								//18
        ///			"invalid expression",							//19
        ///			"cell empty"									//20
        ///			"bad formula",									//21
        ///			"empty expression",								//22
        ///			"Virtual Mode required - set UsesVirtualDataSource", //23
        ///			"mismatched string quotes",                     //24
        ///			"wrong number of arguments",                    //25
        ///			"invalid arguments",							//26
        ///			"iterations do not converge",                   //27
        ///			"Control named '{0}' is already registered",    //28
        ///         "Calculation overflow",							//29
        ///         "missing operand"							    //30
        ///		};
        /// </code>
        /// </example>
        public string[] FormulaErrorStrings = new string[]
            {
                "binary operators cannot start an expression",	//0
                "cannot parse",									//1
                "bad library",									//2
                "invalid char in front of",						//3
                "number contains 2 decimal points",				//4
                "expression cannot end with an operator",		//5
                "invalid characters following an operator",		//6
                "invalid character in number",					//7
                "mismatched parentheses",						//8
                "unknown formula name",							//9
                "requires a single argument",					//10
                "requires 3 arguments",							//11
                "invalid Math argument",						//12
                "requires 2 arguments",							//13
                "bad index",									//14
                "too complex",									//15
                "circular reference: ",							//16
                "missing formula",								//17
                "improper formula",								//18
                "invalid expression",							//19
                "cell empty",									//20
                "bad formula",									//21
                "empty expression",								//22
                "Virtual Mode required - set UsesVirtualDataSource", //23
                "mismatched string quotes",                             //24
                "wrong number of arguments",                    //25
                "invalid arguments",								//26
                "iterations do not converge",                        //27
                "Control named '{0}' is already registered",          //28
                "Calculation overflow",								//29
                "missing operand",								//30
            };

        internal int operators_cannot_start_an_expression = 0;
        internal int cannot_parse = 1;
        internal int bad_library = 2;
        internal int invalid_char_in_front_of = 3;
        internal int number_contains_2_decimal_points = 4;
        internal int expression_cannot_end_with_an_operator = 5;
        internal int invalid_characters_following_an_operator = 6;
        internal int invalid_char_in_number = 7;
        internal int mismatched_parentheses = 8;
        internal int unknown_formula_name = 9;
        internal int requires_a_single_argument = 10;
        internal int requires_3_args = 11;
        internal int invalid_Math_argument = 12;
        internal int requires_2_args = 13;
        internal int bad_index = 14;
        internal int too_complex = 15;
        internal int circular_reference_ = 16;
        internal int missing_formula = 17;
        internal int improper_formula = 18;
        internal int invalid_expression = 19;
        internal int cell_empty = 20;
        internal int bad_formula = 21;
        internal int empty_expression = 22;
        internal int virtual_mode_required = 23;
        internal int mismatched_tics = 24;
        internal int wrong_number_arguments = 25;
        internal int invalid_arguments = 26;
        internal int iterations_dont_converge = 27;
        internal int already_registered = 28;
        internal int calculation_overflow = 29;
        internal int missing_operand = 30;

        #endregion

        #region Parsing Code

        #region String Support

        //handle special case #N/A
        private char UNIQUESLASHMARKER = (char)129;

        private char UNIQUESTRINGMARKER = (char)127;
        private Hashtable SaveStrings(ref string text)
        {
            //handle #N/A problem by replacing it for parsing...
            text = text.Replace("#N/A", "#N" + UNIQUESLASHMARKER + "A");

            Hashtable strings = null;
            string TICs2 = TIC + TIC;
            int id = 0;
            int i = -1;
            if ((i = text.IndexOf(TIC)) > -1)
            {
                while (i > -1 && i < text.Length)
                {
                    if (strings == null)
                        strings = new Hashtable();

                    int j = (i + 1) < text.Length ? text.IndexOf(TIC, i + 1) : -1;
                    if (j > -1)
                    {
                        string key = TIC + UNIQUESTRINGMARKER + id.ToString() + TIC;
                        if (j < text.Length - 2 && text[j + 1] == TIC[0])
                        {
                            j = text.IndexOf(TIC, j + 2);
                            if (j == -1)
                                throw new ArgumentException(FormulaErrorStrings[mismatched_tics]);
                        }

                        string s = text.Substring(i, j - i + 1);
                        strings.Add(key, s);
                        s = s.Replace(TICs2, TIC);
                        id++;
                        text = text.Substring(0, i) + key + text.Substring(j + 1);
                        i = i + key.Length;
                        if (i < text.Length)
                            i = text.IndexOf(TIC, i);
                    }
                    else
                    {
                        throw new ArgumentException(FormulaErrorStrings[mismatched_tics]);
                    }
                }
            }

            return strings;
        }

        private void SetStrings(ref string retValue, Hashtable strings)
        {
            //handle #N/A problem by replacing it for parsing...
            retValue = retValue.Replace("#N" + UNIQUESLASHMARKER + "A", "#N/A");


            foreach (string s in strings.Keys)
            {
                retValue = retValue.Replace(s, (string)strings[s]);
            }
        }
        #endregion


        static GridFormulaEngine()
        {
            ParseDecimalSeparator = '.';
            ParseArgumentSeparator = ',';
        }

        /// <summary>
        /// Character recognized by the parsing engine as decimal separator for numbers.
        /// </summary>
        [ThreadStaticAttribute]
        public static char ParseDecimalSeparator = '.';

        /// <summary>
        /// Character recognized by the parsing code as the delimiter for arguments in a named formula's argument list.
        /// </summary>
        [ThreadStaticAttribute]
        public static char ParseArgumentSeparator = ',';




        private List<string> errorStrings;

        /// <summary>
        /// Checks if given string holds a valid formula.
        /// </summary>
        /// <param name="text">String to be tested.</param>
        /// <param name="parsedFormula">Tokenized string holding holding valid parsed formula.</param>
        /// <param name="errorMessage">Error message from the grid if string is invalid.</param>
        /// <param name="computedValue">Computed value from parsed formula.</param>
        /// <returns>True if string holds a valid formula, false otherwise.</returns>
        public bool IsFormulaValid(string text, out string parsedFormula, out string errorMessage, out string computedValue)
        {
            string saveCell = this.cell;
            this.cell = "";

            errorMessage = "";
            parsedFormula = "";
            computedValue = "";
            bool save = this.lockDependencies;
            try
            {
                this.lockDependencies = true;
                parsedFormula = Parse(text);
                computedValue = this.ComputedValue(parsedFormula);
                if (errorStrings == null)
                {
                    errorStrings = new List<string>();
                    this.errorStrings.AddRange(this.FormulaErrorStrings);
                }

                int index = errorStrings.IndexOf(computedValue);
                if (index > -1)
                {
                    errorMessage = errorStrings[index].ToString();
                    computedValue = "";
                }
                return index == -1;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            finally
            {
                this.lockDependencies = save;
                this.cell = saveCell;
            }
        }

        /// <summary>
        /// Checks if the given string holds a valid formula.
        /// </summary>
        /// <param name="text">String to be tested.</param>
        /// <returns>True if string holds a valid formula, false otherwise.</returns>
        public bool IsFormulaValid(string text)
        {
            string s, p, c;
            return IsFormulaValid(text, out p, out s, out c);
        }

        /// <summary>
        /// Parses a formula string into a tokenized string.
        /// </summary>
        /// <param name="text">The string to be parsed.</param>
        /// <returns>The parsed string.</returns>
        /// <remarks> 
        ///  This method accepts a string that holds a formula, like =Sum(A1:B5), 
        ///  and translates this string into a tokenized expression that can
        ///  be computed using the ComputedValue method. Before 
        ///  calling the method, you should set FormulaContextCell
        ///  to properly reflect which cell owns this formula.
        ///  The return value, which is 
        ///  the tokenized string, is referred to as a parsed formula string.
        /// </remarks>
        public string Parse(string text)
        {
            //Console.WriteLine("Parse->[{0}]", text);
            //#if DEBUG
            //            if (Switches.FormulaCell.TraceVerbose)
            //                TraceUtil.TraceCurrentMethodInfo(text, this);
            //#else
            //            ;
            //#endif

            GetFormulaText(ref text);

            bool usesFormulaChar = this.formulaChar != (char)0 && text.Length > 0 && text[0] == this.formulaChar;
            if (usesFormulaChar)
                text = text.Substring(1); //strip it off

            if (GridCellFormulaModel.IsEmpty(text))
                return text;


            //Need to handle % sign as in =1%*F1
            //if(text.IndexOf("%") > 0)
            //	Console.WriteLine("IndexOf");
            //text = text.Replace("%", "");

            //Make braces strings...
            text = text.Replace(BRACELEFT, TIC);
            text = text.Replace(BRACERIGHT, TIC);

            //Save strings...
            Hashtable formulaStrings = SaveStrings(ref text);

            if (formulaChar != (char)0 && formulaChar == text[0])
            {
                text = text.Substring(1);
            }


            int i = 0;

            if (!SupportBlanksInSheetNames)
            {
                text = text.Replace(" ", "");
            }

            //Special check for single namedrange.
            if (NamedRanges.ContainsKey(text))
            {
                SetNamedRangeDependency(text, cell);
                text = ((string)NamedRanges[text]).ToUpper();
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(text);

                //Search in order of size, match must not have a letter or digit immediately
                //preceding or following.
                foreach (string key in this.NamedRangesSized)
                {
                    int newLoc = -1;
                    int oldLoc = 0;
                    string s = "";
                    while (oldLoc < sb.Length && (newLoc = sb.ToString().IndexOf(key, oldLoc)) > -1)
                    {
                        int len = key.Length;
                        if ((newLoc == 0 || !char.IsLetterOrDigit(sb[newLoc - 1]))
                            && (newLoc + len == sb.Length || !char.IsLetterOrDigit(sb[newLoc + len])))
                        {
                            //only do this on first replacement
                            if (s.Length == 0)
                            {
                                SetNamedRangeDependency(key, cell);
                                //upper and enclose in parens
                                s = "(" + this.NamedRanges[key].ToString().ToUpper() + ")";
                                PutTokensForSheets(ref s);
                            }
                            sb.Replace(key, s, newLoc, len);
                            len = s.Length;
                        }
                        oldLoc = newLoc + len;
                    }
                }
                text = sb.ToString();
            }
#if !WinRT
            text = text.ToUpper(CultureInfo.InvariantCulture);
#else
            text = text.ToUpper();
#endif
            //Replace sheet references with tokens.
            GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
            if (family.SheetNameToGridModel != null && family.SheetNameToGridModel.Count > 0)
            {
                try
                {
                    PutTokensForSheets(ref text);
                }
                catch (Exception ex)
                {
#if WPF
                    TraceUtil.TraceExceptionCatched(ex);
#endif
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        throw ex;
                    throw ex; //rethrow so the caller can handle the bad parse on formula name
                    //return ex.Message;
                }
            }

            if (SupportBlanksInSheetNames)
            {
                text = text.Replace(" ", "");
            }

            try
            {
                MarkLibraryFormulas(ref text);
            }
            catch (Exception ex)
            {
#if WPF
                TraceUtil.TraceExceptionCatched(ex);
#endif
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw ex;
                throw ex; //rethrow so the caller can handle the bad parse on formula name
                //return ex.Message;
            }

            //Special case parsing done in function definition.

            //Look for inner matching & parse pieces without parens with ParseSimple.
            while ((i = text.IndexOf(')')) > -1)
            {
                int k = text.Substring(0, i).LastIndexOf('(');
                if (k == -1)
                    throw new ArgumentException(FormulaErrorStrings[mismatched_parentheses]);
                if (k == i - 1)
                    throw new ArgumentException(FormulaErrorStrings[empty_expression]);

                string s = text.Substring(k + 1, i - k - 1);
                text = text.Substring(0, k) + ParseSimple(s) + text.Substring(i + 1);
            }

            //All parens should be removed.
            if (text.IndexOf('(') > -1)
            {
                throw new ArgumentException(FormulaErrorStrings[mismatched_parentheses]);
            }

            string retValue = ParseSimple(text);
            if (formulaStrings != null && formulaStrings.Count > 0)
            {
                SetStrings(ref retValue, formulaStrings);
            }


            return retValue;
        }


        //Operator Parsing
        //CHAR_xxxx used in formulas; swapped for TOKEN_xxxx in parsed formula.
        //TOKEN_xxxx is lowercase alpha char.
        //STRING_xxxx identifies the operators that require 2 characters; get mapped to CHAR_xxxx to fit into single char algorithm.
        //Lowercase letters used: abdefghjklmnopqrstuvwx.
        // z is used as a temp swap character...
        private const char TOKEN_multiply = 'm';
        private const char CHAR_multiply = '*';
        private const char TOKEN_divide = 'd';
        private const char CHAR_divide = '/';
        private const char TOKEN_add = 'a';
        private const char CHAR_add = '+';
        private const char TOKEN_subtract = 's';
        private const char CHAR_subtract = '-';

        private const char TOKEN_less = 'l';
        private const char CHAR_less = '<';
        private const char TOKEN_greater = 'g';
        private const char CHAR_greater = '>';
        private const char TOKEN_equal = 'e';
        private const char CHAR_equal = '=';

        private const char TOKEN_lesseq = 'k';
        private const char CHAR_lesseq = 'f';
        private const string STRING_lesseq = "<=";
        private const char TOKEN_greatereq = 'j';
        private const char CHAR_greatereq = 'h';
        private const string STRING_greatereq = ">=";
        private const char TOKEN_noequal = 'o';
        private const char CHAR_noequal = 'p';
        private const string STRING_noequal = "<>";

        private const string STRING_fixedreference = "$";
        private const string STRING_empty = "";

        private const char TOKEN_and = 'c';
        private char CHAR_and = 'i';
        private string STRING_and = "&";//"AND";
        private const char TOKEN_pow = (char)126;//'k'; //power
        private char CHAR_pow = 'w';
        private string STRING_pow = "^";//"OR";

        private char CHAR_EP = 'x';
        private char CHAR_EM = 'r';
        private const char TOKEN_EP = 't';
        private const char TOKEN_EM = 'v';
        private string STRING_EP = "E+";
        private string STRING_E = "E";
        private string STRING_EM = "E-";

        private string TIC = "\"";//"'"; //used to mark strings
        private string BRACELEFT = "{";
        private string BRACERIGHT = "}";
        private char[] BRACEDELIMETER = new char[] { ',' };

        private char LEFTBRACKET = (char)130;//'[';
        private char RIGHTBRACKET = (char)131;//']';

        private const char LEADMINUS = (char)132;
        private const char ZMARKER = 'z';
        private const char BMARKER = 'b';

        private string IFMarker = "qIF" + (char)130; //'[';//(char)130;

        private char[] tokens;

        private string ParseSimple(string text)
        {
            //strip leading plus
            if (text.Length > 0 && text[0] == '+')
                text = text.Substring(1);

            if (text.Trim().Length == 0 || text == "#N/A")
                return text;

            //TraceUtil.TraceCurrentMethodInfoIf(Switches.FormulaCell.TraceVerbose, text, this);

            text = HandleEmbeddedEs(text);

            System.Text.StringBuilder sb = new System.Text.StringBuilder(text);
            bool process = true;
            while (process)
            {
                sb.Replace("--", "+");
                sb.Replace("++", "+");
                //Mark unary minus with u-token.
                sb.Replace(",-", ",u").Replace(LEFTBRACKET + "-", LEFTBRACKET + "u").Replace("=-", "=u").Replace(">-", ">u").Replace("<-", "<u").Replace("/-", "/u").Replace("*-", "*u").Replace("+-", "+u").Replace("^-", "^u");
                //Get rid of leading pluses.
                sb.Replace(",+", ",").Replace(LEFTBRACKET + "+", LEFTBRACKET.ToString()).Replace("=+", "=").Replace(">+", ">").Replace("<+", "<").Replace("/+", "/").Replace("*+", "*").Replace("^+", "^");
                if (sb.Length > 0 && sb[0] == '+')
                    sb.Remove(0, 1);
                process = text != sb.ToString();
                text = sb.ToString();
            }


            text = sb.Replace(STRING_lesseq, CHAR_lesseq.ToString())
                .Replace(STRING_greatereq, CHAR_greatereq.ToString())
                .Replace(STRING_noequal, CHAR_noequal.ToString())
                .Replace(STRING_fixedreference, STRING_empty)
                .Replace(STRING_pow, CHAR_pow.ToString())
                .Replace(STRING_and, CHAR_and.ToString())
                .ToString();

            bool needToContinue = true;
            //Doing things sequentially imposes computation hierarchy for 6 levels.
            // 0. E+ E- //handles exponential notation like 1.2e-1 or 1.2e+1 to 1.2e1
            // 1. ^ exponentiation
            // 2. * /
            // 3. + -
            // 4. < > = <= >= <>
            // 5. & concatenation
            // in each level parsing done left to right
            text = ParseSimple(text, new char[] { TOKEN_EP, TOKEN_EM }, new char[] { CHAR_EP, CHAR_EM }, ref needToContinue);
            if (needToContinue)
                text = ParseSimple(text, new char[] { TOKEN_pow }
                    , new char[] { CHAR_pow }, ref needToContinue);
            if (needToContinue)
                text = ParseSimple(text, new char[] { TOKEN_multiply, TOKEN_divide }, new char[] { CHAR_multiply, CHAR_divide }, ref needToContinue);
            if (needToContinue)
                text = ParseSimple(text, new char[] { TOKEN_add, TOKEN_subtract }, new char[] { CHAR_add, CHAR_subtract }, ref needToContinue);
            if (needToContinue)
                text = ParseSimple(text, new char[] { TOKEN_less, TOKEN_greater, TOKEN_equal, TOKEN_lesseq, TOKEN_greatereq, TOKEN_noequal }
               , new char[] { CHAR_less, CHAR_greater, CHAR_equal, CHAR_lesseq, CHAR_greatereq, CHAR_noequal }, ref needToContinue);

            if (needToContinue)
                text = ParseSimple(text, new char[] { TOKEN_and }
                    , new char[] { CHAR_and }, ref needToContinue);

            return text;
        }

        private bool EInsideBMARKERs(int j, string text)
        {
            int left = text.Substring(0, j).LastIndexOf(BMARKER);
            int right = text.IndexOf(BMARKER, j);
            return left > -1 && j > left && j < right;

        }

        private string HandleEmbeddedEs(string text)
        {
            int j = 0;
            while (j > -1 && ((j = text.IndexOf(STRING_EP, j)) > -1) && !EInsideBMARKERs(j, text))
            {
                int left = j;
                while (left > 0 && (char.IsDigit(text[left - 1]) || text[left - 1] == ParseDecimalSeparator))
                    left--;
                if (left != j && (left == 0 || !char.IsUpper(text[left - 1])))
                {
                    int right = j + STRING_EP.Length;
                    while (right < text.Length && char.IsDigit(text[right]))
                    {
                        right++;
                    }
                    if (right != j + STRING_EP.Length)
                    {
                        text = text.Substring(0, j) + CHAR_EP + text.Substring(j + STRING_EP.Length);
                    }
                }
                j += 1;
            }
            j = 0;
            while (j > -1 && ((j = text.IndexOf(STRING_EM, j)) > -1) && !EInsideBMARKERs(j, text))
            {
                int left = j;
                while (left > 0 && (char.IsDigit(text[left - 1]) || text[left - 1] == ParseDecimalSeparator))
                    left--;
                if (left != j && (left == 0 || !char.IsUpper(text[left - 1])))
                {
                    int right = j + STRING_EM.Length;
                    while (right < text.Length && char.IsDigit(text[right]))
                    {
                        right++;
                    }
                    if (right != j + STRING_EM.Length)
                    {
                        text = text.Substring(0, j) + CHAR_EM + text.Substring(j + STRING_EM.Length);
                    }
                }
                j += 1;
            }

            j = 0;
            //only look for single e's if expression has not been parsed
            while (j > -1 && ((j = text.IndexOf(STRING_E, j)) > -1)
                && !EInsideBMARKERs(j, text))//text[0] != BMARKER
            {
                int left = j;
                while (left > 0 && (char.IsDigit(text[left - 1]) || text[left - 1] == ParseDecimalSeparator))
                    left--;
                if (left != j && (left == 0 || !char.IsUpper(text[left - 1])))
                {
                    int right = j + STRING_E.Length;
                    while (right < text.Length && char.IsDigit(text[right]))
                    {
                        right++;
                    }
                    if (right != j + STRING_E.Length && (left == -1 || !char.IsUpper(text[left])))
                    {
                        text = text.Substring(0, j) + CHAR_EP + text.Substring(j + STRING_E.Length);
                    }
                }
                j += 1;
            }

            return text;
        }

        private string ZapBlocks(string text)
        {
            if (text.IndexOf(BMARKER) > -1)
            {
                int bracketLevel = 0;
                for (int i = text.Length - 1; i >= 0; --i)
                {
                    if (text[i] == RIGHTBRACKET)
                        bracketLevel--;
                    else if (text[i] == LEFTBRACKET)
                        bracketLevel++;
                    else if (text[i] == BMARKER && bracketLevel == 0)
                        text = text.Remove(i, 1);
                }

            }
            return text;
        }

        private int FindNonQB(string text)
        {
            int ret = -1;
            if (text.IndexOf(BMARKER) > -1)
            {
                int bracketLevel = 0;
                for (int i = 0; i < text.Length; ++i)
                {
                    if (text[i] == RIGHTBRACKET)
                        bracketLevel--;
                    else if (text[i] == LEFTBRACKET)
                        bracketLevel++;
                    else if (text[i] == BMARKER && bracketLevel == 0)
                    {
                        ret = i;
                        break;
                    }
                }

            }
            return ret;
        }



        private string ParseSimple(string text, char[] markers, char[] operators, ref bool needToContinue)
        {
            //#if DEBUG
            //            if (Switches.FormulaCell.TraceVerbose)
            //                TraceUtil.TraceCurrentMethodInfo(text, markers, operators);
            //#else
            //            ;
            //#endif

            int i;

            //Op is string containing CHAR_xxxx's for each operator in this level.
            string op = "";
            foreach (char c in operators)
                op = op + c;


            if (text.Length > 0 && text[0] == '-')
            {
                //Leading unary minus.

                text = text.Substring(1).Replace('-', LEADMINUS);
                text = "0-" + text;
                text = ParseSimple(text, new char[] { TOKEN_subtract }, new char[] { CHAR_subtract }, ref needToContinue);
                text = text.Replace(LEADMINUS, '-');

            }

            //Check for simple namedrange.
            if (NamedRanges.ContainsKey(text))
            {
                SetNamedRangeDependency(text, cell);
                text = this.NamedRanges[text].ToString().ToUpper();
            }

            if (text.IndexOfAny(operators) > -1)
            {
                while ((i = text.IndexOfAny(operators)) > -1)
                {
                    string left = "";
                    string right = "";

                    int leftIndex = 0;
                    int rightIndex = 0;

                    if (i < 1 && text[i] != '-')
                    {
                        throw new ArgumentException(FormulaErrorStrings[operators_cannot_start_an_expression]);
                    }

                    //Process left argument.
                    int j = i - 1;

                    if (i == 0 && text[i] == '-')
                    {
                        //Unary minus - block and continue.
                        text = "bnu" + text.Substring(1) + BMARKER;
                        continue;
                    }
                    else if (text[j] == TIC[0]) //string
                    {
                        int k = text.Substring(0, j - 1).LastIndexOf(TIC);
                        if (k < 0)
                            throw new ArgumentException(FormulaErrorStrings[cannot_parse]);

                        //left = text.Substring(k+1, j-k-1);
                        //leftIndex = k + 1;
                        left = text.Substring(k, j - k + 1); //keep the tics
                        leftIndex = k;
                    }
                    else if (text[j] == BMARKER) //block of already parsed code
                    {
                        //int k = text.Substring(0, j-1).LastIndexOf(BMARKER);
                        int k = FindLastNonQB(text.Substring(0, j - 1));
                        if (k < 0)
                            throw new ArgumentException(FormulaErrorStrings[cannot_parse]);

                        left = text.Substring(k + 1, j - k - 1);
                        leftIndex = k + 1;
                    }
                    else if (text[j] == RIGHTBRACKET) //library member
                    {
                        int bracketCount = 0;
                        int k = j - 1;
                        while (k > 0 && (text[k] != 'q' || bracketCount != 0))
                        {
                            if (text[k] == 'q')
                                bracketCount--;
                            else if (text[k] == RIGHTBRACKET)
                                bracketCount++;
                            k--;
                        }

                        //int k = text.Substring(0, j-1).LastIndexOf('q');
                        if (k < 0)
                            throw new ArgumentException(FormulaErrorStrings[bad_library]);

                        left = text.Substring(k, j - k + 1);
                        left = left.Replace(BMARKER, ZMARKER); //marked computed formulas with z instead of b
                        leftIndex = k;
                    }
                    else if (!char.IsDigit(text[j]) && text[j] != '%') //number
                    {
                        //Check for namedrange.
                        while (j >= 0 && char.IsUpper(text[j]))
                            j--;

                        left = text.Substring(j + 1, i - j - 1);//'n' for number
                        leftIndex = j + 1;
                        if (this.NamedRanges.ContainsKey(left))
                        {
                            left = Parse((string)this.NamedRanges[left]);
                        }
                        else if (left == TRUEVALUESTR)
                        {
                            left = 'n' + TRUEVALUESTR;
                            //left = 'n' + TRUEVALUE.ToString();
                        }
                        else if (left == FALSEVALUESTR)
                        {
                            left = 'n' + FALSEVALUESTR;
                            //left = 'n' + FALSEVALUE.ToString();
                        }
                        else
                            throw new ArgumentException(FormulaErrorStrings[invalid_char_in_front_of] + " " + text[i]);
                    }
                    else
                    {
                        bool period = false;
                        bool percent = false;

                        while (j > -1 && (char.IsDigit(text[j]) || (!period && text[j] == ParseDecimalSeparator)
                            || (!percent && text[j] == '%')))
                        {
                            if (text[j] == ParseDecimalSeparator)
                                period = true;
                            else if (text[j] == '%')
                                percent = true;
                            j = j - 1;
                        }
                        if (j > -1 && period && text[j] == ParseDecimalSeparator)
                            throw new ArgumentException(FormulaErrorStrings[number_contains_2_decimal_points]);

                        if (j > -1 && text[j] == 'u')
                            j--;
                        j = j + 1;

                        if (j == 0 || (j > 0 && !char.IsUpper(text[j - 1]))
                            || (j == 1 && text[0] == 'u'))
                        {
                            if (j == 1 && text[0] == 'u')
                                j--;

                            left = 'n' + text.Substring(j, i - j);//'n' for number
                            leftIndex = j;
                        }
                        else
                        {
                            //We have a cell reference.
                            j = j - 1;
                            while (j > -1 && char.IsUpper(text[j]))
                                j = j - 1;
                            if (j > -1 && text[j] == sheetToken)
                            {
                                j = j - 1;
                                while (j > -1 && text[j] != sheetToken)
                                    j = j - 1;
                                if (j > -1 && text[j] == sheetToken)
                                    j = j - 1;
                            }

                            j = j + 1;
                            left = text.Substring(j, i - j);

                            //Handle 0 as relative row reference.
                            if (currentRowNotationEnabled && left.Length > 1 && left[left.Length - 1] == '0' &&
                                !char.IsDigit(left[left.Length - 2]))
                            {
                                left = left.Substring(0, left.Length - 1) + RowIndex(cell);
                            }
                            UpdateDependencies(left);
                            leftIndex = j;
                        }
                    }

                    //Process right argument.
                    if (i == text.Length - 1)
                        throw new ArgumentException(FormulaErrorStrings[expression_cannot_end_with_an_operator]);
                    else
                    {
                        j = i + 1;
                        if (text[j] == TIC[0]) //string
                        {
                            int k = text.Substring(j + 1).IndexOf(TIC);
                            if (k < 0)
                                throw new ArgumentException(FormulaErrorStrings[cannot_parse]);

                            right = text.Substring(j, k + 2);
                            rightIndex = k + j + 2;
                        }
                        else if (text[j] == BMARKER) //block of already parsed code
                        {
                            //int k = text.Substring(j+1).IndexOf(BMARKER);
                            int k = FindNonQB(text.Substring(j + 1));
                            if (k < 0)
                                throw new ArgumentException(FormulaErrorStrings[cannot_parse]);

                            right = text.Substring(j + 1, k);
                            rightIndex = k + j + 2;
                        }
                        //						else if(text[j] == TIC[0]) //string
                        //						{
                        //							int k = text.Substring(j+1).IndexOf(TIC[0]);
                        //							if(k<0)
                        //								throw new ArgumentException(FormulaErrorStrings[cannot_parse]);
                        //
                        //							right = text.Substring(j , k + 2);
                        //							rightIndex = k + j + 2;
                        //						}
                        else if (text[j] == 'q') //library
                        {
                            int bracketCount = 0;
                            int k = j + 1;
                            while (k < text.Length && (text[k] != RIGHTBRACKET || bracketCount != 0))
                            {
                                if (text[k] == RIGHTBRACKET)
                                    bracketCount++;
                                else if (text[k] == 'q')
                                    bracketCount--;
                                k++;
                            }
                            if (k == text.Length)
                                throw new ArgumentException(FormulaErrorStrings[cannot_parse]);

                            right = text.Substring(j, k - j + 1);
                            right = right.Replace(BMARKER, ZMARKER); //marked computed formulas with z instead of b
                            rightIndex = k + 1;
                        }
                        else if (char.IsDigit(text[j]) || text[j] == ParseDecimalSeparator)
                        {
                            bool period = text[j] == ParseDecimalSeparator;
                            j = j + 1;
                            while (j < text.Length && (char.IsDigit(text[j]) || (!period && text[j] == ParseDecimalSeparator)))
                            {
                                if (text[j] == ParseDecimalSeparator)
                                    period = true;
                                j = j + 1;
                            }
                            if (j < text.Length && text[j] == '%')
                                j += 1;
                            if (period && j < text.Length && text[j] == ParseDecimalSeparator)
                                throw new ArgumentException(FormulaErrorStrings[number_contains_2_decimal_points]);
                            right = 'n' + text.Substring(i + 1, j - i - 1);
                            rightIndex = j;
                        }
                        else if (char.IsUpper(text[j]) || text[j] == sheetToken || text[j] == 'u')
                        {
                            if (text[j] == sheetToken)
                            {
                                j++;
                                while (j < text.Length && text[j] != sheetToken)
                                    j++;
                            }

                            j = j + 1;

                            int j0 = 0;
                            while (j < text.Length && char.IsUpper(text[j]))
                            {
                                j++;
                                j0++;
                            }
                            bool noCellReference = (j == text.Length) || !char.IsDigit(text[j]);
                            if (j0 > 4)
                            {
                                while (j < text.Length && (char.IsUpper(text[j]) || char.IsDigit(text[j])))
                                {
                                    j++;
                                }
                                noCellReference = true;
                            }


                            while (j < text.Length && char.IsDigit(text[j]))
                            {
                                j = j + 1;
                            }

                            j = j - 1;

                            right = text.Substring(i + 1, j - i);
                            if (!noCellReference)
                            {
                                //Handle 0 as relative row reference.
                                if (currentRowNotationEnabled && right.Length > 1 && right[right.Length - 1] == '0' &&
                                    !char.IsDigit(right[right.Length - 2]))
                                {
                                    right = right.Substring(0, right.Length - 1) + RowIndex(cell);
                                }
                                UpdateDependencies(right); //cb
                            }
                            else
                            {
                                if (NamedRanges.ContainsKey(right))
                                {
                                    SetNamedRangeDependency(right, cell);
                                    right = Parse((string)NamedRanges[right]);
                                }
                                else if (right == TRUEVALUESTR)
                                {
                                    //right = 'n' + TRUEVALUE.ToString();
                                    right = 'n' + TRUEVALUESTR;
                                }
                                else if (right == FALSEVALUESTR)
                                {
                                    //right = 'n' + FALSEVALUE.ToString();
                                    right = 'n' + FALSEVALUESTR;
                                }
                                else
                                    throw new ArgumentException(FormulaErrorStrings[invalid_characters_following_an_operator]);
                            }
                            rightIndex = j + 1;
                        }
                        else
                        {
                            throw new ArgumentException(FormulaErrorStrings[invalid_characters_following_an_operator]);
                        }
                    }

                    int p = op.IndexOf(text[i]);
                    //string s = BMARKER + left.Replace("b", "") + right.Replace("b", "") + markers[p] + BMARKER;
                    string s = BMARKER + ZapBlocks(left) + ZapBlocks(right) + markers[p] + BMARKER;
                    s = s.Replace(ZMARKER, BMARKER); //swap b markers back
                    if (leftIndex > 0)
                        s = text.Substring(0, leftIndex) + s;
                    if (rightIndex < text.Length)
                        s = s + text.Substring(rightIndex);
                    s = s.Replace("bb", "b");
                    //s = s.Replace(TIC+TIC, TIC);
                    text = s;
                }
            }
            else
            {
                //No operators  ..must be number, reference, or library method.

                //Process left argument.
                int j = text.Length - 1;

                if (text[j] == BMARKER) //Block of already parsed code.
                {
                    //int k = text.Substring(0, j-1).LastIndexOf(BMARKER);
                    int k = FindLastNonQB(text.Substring(0, j - 1));
                    if (k < 0)
                        throw new ArgumentException(FormulaErrorStrings[cannot_parse]);

                }
                else if (text[j] == RIGHTBRACKET) //library member
                {
                    int bracketCount = 0;
                    int k = j - 1;
                    while (k > 0 && (text[k] != 'q' || bracketCount != 0))
                    {
                        if (text[k] == 'q')
                            bracketCount--;
                        else if (text[k] == RIGHTBRACKET)
                            bracketCount++;
                        k--;
                    }

                    //int k = text.Substring(0, j-1).LastIndexOf('q');
                    if (k < 0)
                        throw new ArgumentException(FormulaErrorStrings[bad_library]);
                }
                //				else if(!char.IsDigit(text[j])) //number
                //				{
                //					throw new ArgumentException(FormulaErrorStrings[invalid_char_in_number]);
                //				}
                else
                {
                    bool period = false;
                    bool percent = false;

                    while (j > -1 && (char.IsDigit(text[j]) || (!period && text[j] == ParseDecimalSeparator)
                        || (!percent && text[j] == '%')))
                    {
                        if (text[j] == ParseDecimalSeparator)
                            period = true;
                        else if (text[j] == '%')
                            percent = true;
                        j = j - 1;
                    }
                    if (j > -1 && period && text[j] == ParseDecimalSeparator)
                        throw new ArgumentException(FormulaErrorStrings[number_contains_2_decimal_points]);

                }
                if (text.Length > 0 && (char.IsUpper(text, 0) || text[0] == sheetToken))
                {
                    //Check if cell reference.
                    bool ok = true;
                    bool checkLetter = true;
                    for (int k = 0; k < text.Length; ++k)
                    {
                        if (text[k] == sheetToken)
                        {
                            k++;
                            while (k < text.Length && char.IsDigit(text[k]))
                                k++;
                            if (k == text.Length || text[k] != sheetToken)
                            {
                                ok = false;
                                break;
                            }
                        }
                        else
                        {
                            if (!checkLetter && char.IsUpper(text, k))
                            {
                                ok = false;
                                break;
                            }
                            //20040709 - if(char.IsLetterOrDigit(text, k) || text[k] == sheetToken)
                            if (char.IsDigit(text, k) || char.IsUpper(text, k) || text[k] == sheetToken)
                            {
                                checkLetter = char.IsUpper(text, k);
                            }
                            else
                            {
                                ok = false;
                                break;
                            }
                        }
                    }
                    if (ok)
                    {
                        //Handle 0 as relative row reference.
                        if (currentRowNotationEnabled && text.Length > 1 && text[text.Length - 1] == '0' &&
                            !char.IsDigit(text[text.Length - 2]))
                        {
                            text = text.Substring(0, text.Length - 1) + RowIndex(cell);
                        }
                        UpdateDependencies(text); //cb
                        needToContinue = false;
                    }
                }

            }
            return text;
        }


        private int FindLastNonQB(string text)
        {
            int ret = -1;
            if (text.IndexOf(BMARKER) > -1)
            {
                int bracketLevel = 0;
                for (int i = text.Length - 1; i >= 0; --i)
                {
                    if (text[i] == RIGHTBRACKET)
                        bracketLevel--;
                    else if (text[i] == LEFTBRACKET)
                        bracketLevel++;
                    else if (text[i] == BMARKER && bracketLevel == 0)
                    {
                        ret = i;
                        break;
                    }
                }

            }
            return ret;
        }

        /// <summary>
        /// Replaces namedranges with their values.
        /// </summary>
        /// <remarks>
        /// Accepts a string such as Sin(SumRange), and tokenizes it into
        /// bqSIN[A1A4a]b which serves as input to ComputedValue.
        /// </remarks>
        /// <param name="argList">argList containing named ranges.</param>
        private void MarkNamedRanges(ref string argList)
        {
            int rightParens = argList.IndexOf(')');
            char[] markers = new char[] { ')', ParseArgumentSeparator, '}', '+', '-', '*', '/', '<', '>', '=' };

            int leftParens = 0;

            int i = leftParens + 1; //start of args
            int end = argList.Substring(i).IndexOfAny(markers);

            while (end > -1 && end + i < argList.Length)
            {
                string s = (string)NamedRanges[argList.Substring(i, end)];
                if (s != null)
                {
                    SetNamedRangeDependency(argList.Substring(i, end), cell);
                    s = s.ToUpper();
                    MarkLibraryFormulas(ref s);
                    PutTokensForSheets(ref s);
                }
                if (s != null)
                {
                    argList = argList.Substring(0, i) + s.ToString() + argList.Substring(i + end);
                    i += s.ToString().Length + 1;
                }
                else
                {
                    i += end + 1;
                    while (i < argList.Length && !char.IsUpper(argList[i]))
                    {
                        i++;
                    }
                }
                end = argList.Substring(i).IndexOfAny(markers);
            }
        }

        /// <summary>
        /// Tokenizes all library references.
        /// </summary>
        /// <remarks>
        /// Accepts a string such as Sin(A1+A2), and tokenizes it into
        /// bqSIN[A1A2a]b which serves as input to ComputedValue.
        /// </remarks>
        /// <param name="formula">Formula to be tokenized.</param>
        private void MarkLibraryFormulas(ref string formula)
        {
            int rightParens = formula.IndexOf(')');

            while (rightParens > -1)
            {
                int parenCount = 0;
                int leftParens = rightParens - 1;
                while (leftParens > -1 && (formula[leftParens] != '(' || parenCount != 0))
                {
                    if (formula[leftParens] == ')')
                        parenCount++;
                    else if (formula[leftParens] == ')')
                        parenCount--;
                    leftParens--;
                }

                if (leftParens == -1)
                {
                    throw new ArgumentException(FormulaErrorStrings[mismatched_parentheses]);
                }

                int i = leftParens - 1;
                while (i > -1 && char.IsLetterOrDigit(formula[i]))
                    i--;
                int len = leftParens - i - 1;
                if (len > 0 && this.LibraryFunctions[formula.Substring(i + 1, len)] != null)
                {
                    string s = formula.Substring(leftParens, rightParens - leftParens + 1);
                    MarkNamedRanges(ref s);
                    if (forceParsingOfLibraryFunctionArguments)
                    {
                        SwapInnerParens(ref s);
                        AddParensToArgs(ref s);
                    }
                    formula = formula.Substring(0, i + 1) + 'q' + formula.Substring(i + 1, len) + s.Replace('(', LEFTBRACKET).Replace(')', RIGHTBRACKET) + formula.Substring(rightParens + 1);
                }
                else if (len > 0)
                {
                    throw new ArgumentException(FormulaErrorStrings[unknown_formula_name]);
                }
                else
                {
                    string s = "";
                    if (leftParens > 0)
                        s = formula.Substring(0, leftParens);
                    s = s + '{' +
                        formula.Substring(leftParens + 1, rightParens - leftParens - 1) + '}';
                    if (rightParens < formula.Length)
                        s = s + formula.Substring(rightParens + 1);

                    formula = s;
                }
                rightParens = formula.IndexOf(')');
            }

            formula = formula.Replace('{', '(').Replace('}', ')');
        }

        private void SwapInnerParens(ref string s)
        {
            if (s.Length > 2)
            {
                s = s[0] + s.Substring(1, s.Length - 2).Replace("(", "{").Replace(")", "}") + s[s.Length - 1];
            }
        }


        private void AddParensToArgs(ref string formula)
        {
            if (formula.Length == 0)
                return;

            char[] rightSides = new char[] { ParseArgumentSeparator, RIGHTBRACKET };
            //char[] rightSides = new char[]{ParseArgumentSeparator, RIGHTBRACKET,')'};
            int i = formula.LastIndexOf(ParseArgumentSeparator);
            if (i == -1)
            {
                if (formula.Length > 2 && formula[0] == '(' && formula[formula.Length - 1] == ')')
                {
                    if (formula[1] != '{' && formula[1] != '(')
                    {
                        formula = formula.Substring(0, formula.Length - 1) + "}" + formula.Substring(formula.Length - 1);
                        formula = formula[0] + "{" + formula.Substring(1);
                    }
                }
            }
            else
            {
                bool oneTimeOnly = true;
                while (i > -1)
                {
                    int j = formula.IndexOfAny(rightSides, i + 1);
                    if (j == -1 && formula[formula.Length - 1] == ')')
                        j = formula.Length - 1;
                    if (j > 0)
                    {
                        if (formula[i + 1] != '{' && formula[j - 1] != '}')
                        {
                            formula = formula.Substring(0, j) + "}" + formula.Substring(j);
                            formula = formula.Substring(0, i + 1) + "{" + formula.Substring(i + 1);
                        }
                    }
                    i = formula.Substring(0, i).LastIndexOf(ParseArgumentSeparator);
                    if (oneTimeOnly && i == -1 && formula[0] == '(')
                    {
                        i = 0;
                        oneTimeOnly = false;
                    }
                }
            }
        }

        private bool forceParsingOfLibraryFunctionArguments = false;

        /// <summary>
        /// Gets or sets whether all function arguments are parsed using GridFormulaEngine.Parse.
        /// </summary>
        /// <remarks>
        /// If you are using NamedRanges or CurrentRowNotationEnabled inside function arguments, 
        /// you should set this property 
        /// to true to make sure the proper substitutions are done on the arguments.
        /// </remarks>
        public bool ForceParsingOfLibraryFunctionArguments
        {
            get { return forceParsingOfLibraryFunctionArguments; }
            set { forceParsingOfLibraryFunctionArguments = value; }
        }

        #endregion

        #region Named Ranges support

        /// <summary>
        /// Gets the named range Hashtable.
        /// </summary>
        /// <remarks>
        /// The key is the uppercase name and the value is the range for this name.
        /// </remarks>
        public Hashtable NamedRanges
        {
            get
            {
                if (IsSheeted)
                {
                    GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                    if (family.sheetNamedRanges == null)
                        family.sheetNamedRanges = new Hashtable();
                    return family.sheetNamedRanges;
                }
                else
                {
                    if (namedRanges == null)
                        namedRanges = new Hashtable();
                    return namedRanges;
                }
            }
        }

        /// <summary>
        /// Gets the named range Hashtable that holds the original case name as the value.
        /// </summary>
        /// <remarks>
        /// The key is the uppercase name and the value is the original name.
        /// </remarks>
        public Hashtable NamedRangesOriginalNames
        {
            get
            {
                if (IsSheeted)
                {
                    GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                    if (family.sheetNamedRangesOriginalNames == null)
                        family.sheetNamedRangesOriginalNames = new Hashtable();
                    return family.sheetNamedRangesOriginalNames;
                }
                else
                {
                    if (namedRangesOriginalNames == null)
                        namedRangesOriginalNames = new Hashtable();
                    return namedRangesOriginalNames;
                }
            }
        }

        /// <summary>
        /// Updates all cells that depend upon the given named range.
        /// </summary>
        /// <param name="key">The named range whose cells should be updated.</param>
        public void UpdateDependentNamedRangeCell(string key)
        {
            if (DependentNamedRangeCells.ContainsKey(key))
            {
                Hashtable ht = (Hashtable)((Hashtable)DependentNamedRangeCells[key]).Clone();
                foreach (string s in ht.Keys)
                {
                    string cell1 = s;
                    int i = s.LastIndexOf(sheetToken);
                    int row, col;
                    GridModel grd = grid;
                    if (i > -1)
                    {
                        GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);

                        this.grid = (GridModel)family.TokenToGridModel[cell1.Substring(0, i + 1)];
                        row = RowIndex(cell1);
                        col = ColIndex(cell1);
                    }
                    else
                    {
                        row = RowIndex(cell1);
                        col = ColIndex(cell1);
                    }
                    this.RecalculateRange(GridRangeInfo.Cell(row, col), true, true);

                    this.grid = grd;
                }
            }
        }

        /// <summary>
        /// Removes entries in the DependentNamedRangeCells collection for the given named range.
        /// </summary>
        /// <param name="key">The named range.</param>
        public void RemoveNamedRangeDependency(string key)
        {
            if (DependentNamedRangeCells.ContainsKey(key))
            {
                Hashtable ht = (Hashtable)DependentNamedRangeCells[key];
                ht.Clear();
                DependentNamedRangeCells.Remove(key);
            }
        }

        /// <summary>
        /// Adds a cell to the DependentNamedRangeCells list.
        /// </summary>
        /// <param name="key">The named range.</param>
        /// <param name="cell1">The cell (such as C11 or AJ232).</param>
        public void SetNamedRangeDependency(string key, string cell1)
        {
            GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
            if (family.SheetNameToGridModel != null && cell1.IndexOf(sheetToken) == -1)
            {
                string token = family.GridModelToToken[this.grid] as string;
                cell1 = token + cell1;
            }

            if (DependentNamedRangeCells.ContainsKey(key))
            {
                Hashtable ht = (Hashtable)DependentNamedRangeCells[key];
                if (!ht.ContainsKey(cell1))
                    ht.Add(cell1, "");
            }
            else
            {
                Hashtable ht = new Hashtable();
                ht.Add(cell1, "");
                DependentNamedRangeCells.Add(key, ht);
            }

        }

        private Hashtable namedRangesOriginalNames = null;
        private Hashtable namedRanges = null;
        private Hashtable dependentNamedRangeCells = null;
        private List<string> namedRangesSized = null;

        /// <summary>
        /// Holds namedranges in order of string length.
        /// </summary>
        protected List<string> NamedRangesSized
        {
            get
            {
                if (namedRangesSized == null)
                {
                    namedRangesSized = new List<string>();
                    foreach (string s in NamedRanges.Keys)
                    {
                        namedRangesSized.Add(s);
                    }
                    AdjustNameRangesForSize();
                }

                return namedRangesSized;
            }
        }

        /// <summary>
        /// Orders a namedrange collection according to string length.
        /// </summary>
        /// <remarks>The GridFormulaEngine needs an ordered list of named ranges 
        /// to be able to properly parse named ranges. If you manually add named ranges
        /// to namedranges, then you should call this method afterwards.
        /// </remarks>
        public void AdjustNameRangesForSize()
        {
            NamedRangesSized.Sort(new LenComparer());
        }

        /// <summary>
        /// Used by AdjustNameRangesForSize to create an ArrayList
        /// ordered by string length.
        /// </summary>
        /// <exclude/>
#if WPF
        public class LenComparer : IComparer
#else
        public class LenComparer : IComparer<object>
#endif
        {
            /// <summary>
            /// Compares 2 strings based on their length.
            /// </summary>
            /// <param name="x">String 1.</param>
            /// <param name="y">String 2.</param>
            public int Compare(object x, object y)
            {
                return y.ToString().Length - x.ToString().Length;
            }
        }

        /// <summary>
        /// Holds hashtables containing cells that depend upon namedranges.
        /// </summary>
        /// <remarks>
        /// The key properties in DependentNamedRangeCells are namedranges. The
        /// value properties are hashtables.
        /// </remarks>
        public Hashtable DependentNamedRangeCells
        {
            get
            {
                if (IsSheeted)
                {
                    GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                    if (family.sheetDependentNamedRangeCells == null)
                        family.sheetDependentNamedRangeCells = new Hashtable();
                    return family.sheetDependentNamedRangeCells;
                }
                else
                {
                    if (dependentNamedRangeCells == null)
                        dependentNamedRangeCells = new Hashtable();
                    return dependentNamedRangeCells;
                }
            }
        }


        /// <summary>
        /// Adds a named range to the namedranges collection.
        /// </summary>
        /// <param name="name">The name of the range to be added.</param>
        /// <param name="range">The range to be added.</param>
        /// <returns>True if successfully added, False otherwise.</returns>
        /// <remarks>
        /// The range should be a string such as A4:C8.
        /// </remarks>
        public bool AddNamedRange(string name, string range)
        {
            string s = name;//.ToUpper(CultureInfo.InvariantCulture);
            if (NamedRanges[s] == null)
            {
                if (range.StartsWith("="))
                    range = range.Substring(1);
                if (!this.SupportBlanksInSheetNames)
                    NamedRanges.Add(s, range.Replace(" ", ""));
                else
                    NamedRanges.Add(s, range);
                NamedRangesOriginalNames.Add(s, name);
                namedRangesSized = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a range from the namedranges collection.
        /// </summary>
        /// <param name="name">The name of the range to be removed.</param>
        /// <returns>True is successfully remove, False otherwise.</returns>
        public bool RemoveNamedRange(string name)
        {
            //name = name.ToUpper(CultureInfo.InvariantCulture);
            if (NamedRanges[name] != null)
            {
                NamedRanges.Remove(name);
                NamedRangesOriginalNames.Remove(name);
                namedRangesSized = null;
                return true;
            }
            return false;
        }
        #endregion

        #region Library Functions Maintenance

        /// <summary>
        /// Contains the current library functions.
        /// </summary>
        /// <remarks>
        /// This field gives you direct access to all Library Functions. 
        /// The function name serves as the hash key, and the function delegate
        /// serves as the hash value.
        /// </remarks>
        public Hashtable LibraryFunctions
        {
            get
            {

                return libraryFunctions;
            }
        }

        Hashtable libraryFunctions;

        /// <summary>
        /// Adds a function to the Function Library.
        /// </summary>
        /// <param name="name">The name of the function to be added.</param>
        /// <param name="func">The function to be added.</param>
        /// <returns>True if successfully removed, False otherwise.</returns>
        /// <remarks>
        /// LibraryFunction is a delegate the defines the signature of functions that
        /// you can add to the Function Library.
        /// <code>
        /// public delegate string LibraryFunction(string args);
        /// </code>
        /// </remarks>
        public bool AddFunction(string name, LibraryFunction func)
        {
#if !WinRT
            name = name.ToUpper(CultureInfo.InvariantCulture);
#else
            name = name.ToUpper();
#endif

            if (UseCommonLibrary)
            {
                GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                if (family.GridModelToToken != null)
                {
                    foreach (GridModel g in family.GridModelToToken.Keys)
                    {
                        GridFormulaEngine e = ((GridCellFormulaModel)g.CellModels["FormulaCell"]).Engine;
                        if (e.LibraryFunctions[name] == null)
                        {
                            e.LibraryFunctions.Add(name, func);
                        }
                    }
                    return true;
                }
            }

            if (LibraryFunctions[name] == null)
            {
                LibraryFunctions.Add(name, func);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a function from the Function Library.
        /// </summary>
        /// <param name="name">The name of the function to be removed.</param>
        /// <returns>True if successfully removed, False otherwise.</returns>
        public bool RemoveFunction(string name)
        {
            if (UseCommonLibrary)
            {
                GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                if (family.GridModelToToken != null)
                {
                    foreach (GridModel g in family.GridModelToToken.Keys)
                    {
                        GridFormulaEngine e = ((GridCellFormulaModel)g.CellModels["FormulaCell"]).Engine;
                        if (e.LibraryFunctions[name] == null)
                        {
                            e.LibraryFunctions.Remove(name);
                        }
                    }
                    return true;
                }
            }

            if (LibraryFunctions[name] != null)
            {
                LibraryFunctions.Remove(name);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Delegate used to define functions that you add to the Function Library.
        /// </summary>
        public delegate string LibraryFunction(string args);

        //Used to avoid recursively setting dependencies when cells are being
        //refreshed because a dependent cell changed.
        internal bool lockDependencies = false;

        internal void UpdateDependencies(string s)
        {
            //#if DEBUG
            //            if (Switches.FormulaCell.TraceVerbose)
            //                TraceUtil.TraceCurrentMethodInfo(this);
            //#else
            //            ;
            //#endif

            if (lockDependencies)
                return;

            GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
            string cell1 = cell;
            if (family.SheetNameToGridModel != null && cell1.IndexOf(sheetToken) == -1)
            {
                string token = family.GridModelToToken[this.grid] as string;
                cell1 = token + cell1;
            }

            if (family.SheetNameToGridModel != null && s.IndexOf(sheetToken) == -1)
            {
                string token = family.GridModelToToken[this.grid] as string;
                s = token + s;
            }

            if (!DependentCells.ContainsKey(s))
            {
                //#if DEBUG
                //                if (Switches.FormulaCell.TraceVerbose)
                //                    TraceUtil.TraceCurrentMethodInfo(this, " new trigger cell (" + s + " -> " + cell1 + ")");
                //#else
                //                ;
                //#endif


                DependentCells.Add(s, new Hashtable());
                ((Hashtable)DependentCells[s]).Add(cell1, cell1);
                AddToFormulaDependentCells(s);
            }
            else if (!((Hashtable)DependentCells[s]).ContainsKey(cell1))
            {
                ((Hashtable)DependentCells[s]).Add(cell1, cell1);
                AddToFormulaDependentCells(s);
                //#if DEBUG
                //                if (Switches.FormulaCell.TraceVerbose)
                //                    TraceUtil.TraceCurrentMethodInfo(this, " old trigger cell (" + s + " -> " + cell1 + ")");
                //#else
                //                ;
                //#endif

            }
        }

        //When a formula cell changes, call this method to clear it from its dependent cells.
        internal void ClearFormulaDependentCells(string cell)
        {
            Hashtable ht = (Hashtable)DependentFormulaCells[cell];
            if (ht != null)
            {
                foreach (object o in ht.Keys)
                {
                    string s = o as string;
                    DependentCells.Remove(s);
                    //#if DEBUG
                    //                    if (Switches.FormulaCell.TraceVerbose)
                    //                        TraceUtil.TraceCurrentMethodInfo(this, " clearing formula dependencies (" + cell + " -> " + s + ")");
                    //#else
                    //                    ;
                    //#endif

                }
                DependentFormulaCells.Remove(cell);
            }
        }

        //Maintains a list of cells that a formula cell is dependent upon.
        private void AddToFormulaDependentCells(string s)
        {
            string cell1 = cell;
            GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
            if (family.SheetNameToGridModel != null && cell1.IndexOf(sheetToken) == -1)
            {
                string token = family.GridModelToToken[this.grid] as string;
                cell1 = token + cell1;
            }

            if (!DependentFormulaCells.ContainsKey(cell1))
            {
                DependentFormulaCells.Add(cell1, new Hashtable());
                ((Hashtable)DependentFormulaCells[cell1]).Add(s, s);
            }
            else if (!((Hashtable)DependentFormulaCells[cell1]).ContainsKey(s))
            {
                ((Hashtable)DependentFormulaCells[cell1]).Add(s, s);
            }
        }

        /// <summary>
        /// Returns the simple average of all values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the simple average of all values listed in the argument.</returns>
        public string ComputeAvg(string range)
        {
            double sum = 0;
            int count = 0;
            double d;
            string s1;

            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1)
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
#if WPF
                            TraceUtil.TraceExceptionCatched(ex);
#endif
                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0)
                        {
                            if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            {
                                sum = sum + d;
                                count++;
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                        {
                            sum = sum + d;
                            count++;
                        }
                    }

                }
            }
            if (count > 0)
                sum = sum / (double)count;

            return sum.ToString();
        }

        /// <summary>
        /// Returns the inclusive OR of all values treated as logical values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers. Each item in the list is considered True if it is nonzero,
        /// and False if it is zero.</param>
        /// <returns>A string holding the OR of all values listed in the argument.</returns>
        public string ComputeOr(string range)
        {
            bool sum = false;
            string s1;
            double d;

            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
#if WPF
                            if (!BrowserInteropHelper.IsBrowserHosted)
                            {
                                TraceUtil.TraceExceptionCatched(ex);
                            }
#endif
                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0)
                        {
                            if (s1 == this.TRUEVALUESTR)
                            {
                                sum = true;
                                break;
                            }
                            else if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            {
                                if (Math.Abs(d) > 1e-10)
                                {
                                    sum = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (s1 == this.TRUEVALUESTR)
                        {
                            sum = true;
                            break;
                        }
                        else if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                        {
                            if (Math.Abs(d) > 1e-10)
                            {
                                sum = true;
                                break;
                            }
                        }
                    }

                }
                if (sum)
                    break;
            }
            //return sum ? "1" : "0";
            return sum ? this.TRUEVALUESTR : this.FALSEVALUESTR;
        }

        /// <summary>
        /// Returns the AND of all values treated as logical values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers. Each item in the list is considered True if it is nonzero,
        /// and False if it is zero.</param>
        /// <returns>A string holding the AND of all values listed in the argument.</returns>
        public string ComputeAnd(string range)
        {
            bool sum = true;
            string s1;
            double d;

            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
#if WPF
                            if (!BrowserInteropHelper.IsBrowserHosted)
                            {
                                TraceUtil.TraceExceptionCatched(ex);
                            }
#endif
                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0)
                        {
                            if (s1 == this.FALSEVALUESTR)
                            {
                                sum = false;
                                break;
                            }
                            else if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            {
                                if (Math.Abs(d) < 1e-10)
                                {
                                    sum = false;
                                    break;
                                }
                            }
                        }
                        else //empty cell
                        {
                            sum = false;
                            break;
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (s1 == this.FALSEVALUESTR)
                        {
                            sum = false;
                            break;
                        }
                        else if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                        {
                            if (Math.Abs(d) < 1e-10)
                            {
                                sum = false;
                                break;
                            }
                        }
                    }
                    else //empty cell
                    {
                        sum = false;
                        break;
                    }
                }
                if (!sum)
                    break;
            }
            //return sum ? "1" : "0";
            return sum ? this.TRUEVALUESTR : this.FALSEVALUESTR;
        }

        /// <summary>
        ///  Flips the logical value represented by the argument.
        /// </summary>
        /// <remarks>
        ///	The argument is
        /// treated as a logical expression with a non-zero value considered True and a zero value considered False.
        /// </remarks>
        /// <param name="args">A string holding either a single argument consisting of a 
        /// cell reference, a formula, or a number. 
        /// </param>
        /// <returns>Returns 0 if the argument evaluates to a non-zero value. Otherwise, it returns 1.</returns>
        public string ComputeNot(string args)
        {
            double d1;
            string s = args;
            if (args.Length > 0 && !char.IsLetter(args[0]) && args.IndexOfAny(new char[] { ParseArgumentSeparator, ':' }) > -1) //parsed formula
            {
                return FormulaErrorStrings[requires_a_single_argument];
            }
            else
            {

                try
                {
                    s = GetValueFromArg(s);
                    if (s == this.FALSEVALUESTR)
                    {
                        s = "0";
                    }
                    else if (s == this.TRUEVALUESTR)
                    {
                        s = "1";
                    }

                    if (double.TryParse(s, NumberStyles.Number | NumberStyles.AllowExponent, null, out d1))
                    {
                        //Flip the value.
                        if (Math.Abs(d1) > 1e-10)
                        {
                            s = this.FALSEVALUESTR;
                        }
                        else
                        {
                            s = this.TRUEVALUESTR;
                        }
                    }
                }
                catch (Exception ex)
                {
#if WPF
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                    }
#endif
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        throw ex;
                    return ex.Message;
                }
            }
            return s;
        }

        /// <summary>
        /// Conditionally computes one of two alternatives depending upon a logical expression.
        /// </summary>
        /// <remarks>
        ///	The first argument is
        /// treated as a logical expression with a non-zero value considered True and a zero value considered False.
        /// The value of only one of the alternatives is computed depending upon the logical expression.
        /// </remarks>
        /// <param name="args">A string holding a list of three arguments. 
        /// </param>
        /// <returns>Returns a string holding the second argument if the first argument is True (non-zero). Otherwise, it returns a string holding the third argument.</returns>
        public string ComputeIf(string args)
        {
            double d1;
            string s1 = "";
            if (args.Length > 0 && args.IndexOfAny(new char[] { ParseArgumentSeparator, ':' }) == -1) //parsed formula
            {
                return FormulaErrorStrings[requires_3_args];
            }
            else
            {
                string[] s = this.GetCellsFromArgs(args);

                if (s.GetLength(0) >= 2)
                {
                    try
                    {
                        s1 = GetValueFromArg(s[0]);
                        if (GridCellFormulaModel.IsEmpty(s1))
                            s1 = "0"; //empty is zero in calculation
                        if (s1 == this.FALSEVALUESTR)
                        {
                            s1 = "0";
                        }
                        else if (s1 == this.TRUEVALUESTR)
                        {
                            s1 = "1";
                        }

                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d1))
                        {
                            if (Math.Abs(d1) > 1e-10)
                            {
                                s1 = GetValueFromArg(s[1]);
                            }
                            else
                            {
                                s1 = s.GetLength(0) == 3 ? GetValueFromArg(s[2]) : FALSEVALUESTR;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }
                }
                else
                    return FormulaErrorStrings[requires_3_args];
            }

            return s1;
        }


        /// <summary>
        /// Returns the sum of all values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the sum of all values listed in the argument.</returns>
        public string ComputeSum(string range)
        {
            double sum = 0;
            string s1;
            double d;

            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s).Replace(TIC, "");
                        }
                        catch (Exception ex)
                        {
#if WPF
                            if (!BrowserInteropHelper.IsBrowserHosted)
                            {
                                TraceUtil.TraceExceptionCatched(ex);
                            }
#endif
                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0)
                        {
                            if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            {
                                sum = sum + d;
                            }
                        }
                    }
                }
                else
                {
                    try
                    {

                        s1 = GetValueFromArg(r).Replace(TIC, "");
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                        {
                            sum = sum + d;
                        }
                    }

                }
            }
            return sum.ToString();
        }

        /// <summary>
        /// Returns the maximum value of all values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the maximum value of all values listed in the argument.</returns>
        public string ComputeMax(string range)
        {
            double max = double.MinValue;
            double d;
            string s1;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //cell range
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
#if WPF
                            TraceUtil.TraceExceptionCatched(ex);
#endif
                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0)
                        {
                            if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            {
                                max = Math.Max(max, d);
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = (r == "") ? "0" : GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                        {
                            max = Math.Max(max, d);
                        }
                    }

                }
            }

            if (max != double.MinValue)
                return max.ToString();

            return "";
        }

        /// <summary>
        /// Returns the minimum value of all values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the minimum value of all values listed in the argument.</returns>
        public string ComputeMin(string range)
        {
            double min = double.MaxValue;
            double d;
            string s1;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //cell range
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            //TraceUtil.TraceExceptionCatched(ex);
                            //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            //	throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0)
                        {
                            if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            {
                                min = Math.Min(min, d);
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = (r == "") ? "0" : GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                        {
                            min = Math.Min(min, d);
                        }
                    }

                }
            }

            if (min != double.MaxValue)
                return min.ToString();

            return "";
        }


        /// <summary>
        /// Returns the number pi.
        /// </summary>
        /// <param name="args">Ignored. Can be empty.</param>
        /// <returns>A string holding the number pi.</returns>
        public string ComputePI(string args)
        {
            return Math.PI.ToString();
        }

        /// <summary>
        /// Returns a number indicating the sign of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding a number representing the sign of the argument.</returns>
        public string ComputeSign(string args)
        {
            int sign = 0;
            double d;
            string s1;
            if (args.Length > 0 && !char.IsLetter(args[0]) && args.IndexOfAny(new char[] { ParseArgumentSeparator, ':' }) == -1) //parsed formula
            {
                if (double.TryParse(args, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                    sign = Math.Sign(d);
                else
                    return FormulaErrorStrings[invalid_Math_argument];
            }
            else if (args.Length > 0 && (args[0] == BMARKER || args[0] == 'u' || args[0] == 'n' || args.IndexOfAny(tokens) > -1)) //parsed formula
            {
                args = args.Replace('{', '(');
                args = args.Replace('}', ')');

                try
                {
                    s1 = this.ComputedValue(args);
                }
                catch (Exception ex)
                {
#if WPF
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                    }
#endif
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        throw ex;
                    return ex.Message;
                }

                if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                    sign = Math.Sign(d);
                else
                    return FormulaErrorStrings[invalid_Math_argument];
            }
            else
            {

                foreach (string s in this.GetCellsFromArgs(args))
                {
                    try
                    {
                        s1 = GetValueFromArg(s);
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                        {
                            sign = Math.Sign(d);
                            break;//Skip out after first value.
                        }
                    }
                }
            }
            return sign.ToString();
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// </summary>
        /// <param name="args">String containing two parameters separated by commas:
        /// the first being base number,
        /// the second being the exponent.</param>
        /// <returns>A string holding the value of the base number raised to the exponent.</returns>
        public string ComputePow(string args)
        {
            double pow = 0;
            double d1, d2;
            string s1, s2;
            if (args.Length > 0 && !char.IsLetter(args[0]) && args.IndexOfAny(new char[] { ParseArgumentSeparator, ':' }) == -1) //parsed formula
            {
                return FormulaErrorStrings[requires_2_args];
            }
            else
            {
                string[] s = this.GetCellsFromArgs(args);

                if (s.GetLength(0) == 2)
                {
                    try
                    {
                        s1 = GetValueFromArg(s[0]);
                        s2 = GetValueFromArg(s[1]);
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }

                    if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d1)
                        && double.TryParse(s2, NumberStyles.Number | NumberStyles.AllowExponent, null, out d2))
                    {
                        pow = Math.Pow(d1, d2);
                    }
                }
                else
                    return FormulaErrorStrings[requires_2_args];
            }

            return pow.ToString();
        }

        private delegate double MathFunc(double d);

        private string ComputeMath(string args, MathFunc func)
        {
            double ans = 0;
            double d1;
            string s1;
            if (args.Length > 0 && ((!char.IsLetter(args[0]) && args[0] != sheetToken) || args[0] == 'u')
                 && args.IndexOfAny(new char[] { ParseArgumentSeparator, ':' }) == -1) //parsed formula
            {
                //Swap out unary minus.
                args = args.Replace('u', '-');
                if (double.TryParse(args, NumberStyles.Number | NumberStyles.AllowExponent, null, out d1))
                    ans = func(d1);
                else
                    return FormulaErrorStrings[invalid_Math_argument];
            }
            else if (args.Length > 0 && (args[0] == BMARKER || args[0] == 'n' || args.IndexOfAny(tokens) > -1)) //parsed formula
            {
                args = args.Replace('{', '(');
                args = args.Replace('}', ')');

                try
                {
                    s1 = this.ComputedValue(args);
                }
                catch (Exception ex)
                {
#if WPF
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                    }
#endif
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        throw ex;
                    return ex.Message;
                }

                if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d1))
                {
                    ans = func(d1);
                }
                else
                    return FormulaErrorStrings[invalid_Math_argument];
            }
            else
                foreach (string s in this.GetCellsFromArgs(args))
                {
                    try
                    {
                        s1 = GetValueFromArg(s);
                    }
                    catch (Exception ex)
                    {
#if WPF
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                        }
#endif
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            throw ex;
                        return ex.Message;
                    }

                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d1))
                            ans = func(d1);
                    }
                    break;//do only the first one... ignore any others
                }
            return ans.ToString();
        }


        /// <summary>
        /// Computes angle whose cosine is the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding angle whose cosine is the argument.</returns>
        public string ComputeAcos(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Acos)).ToString();
        }

        /// <summary>
        /// Computes angle whose sine is the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding angle whose sine is the argument.</returns>
        public string ComputeAsin(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Asin)).ToString();
        }

        /// <summary>
        /// Computes angle whose tangent is the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the tangent of the argument.</returns>
        public string ComputeAtan(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Atan)).ToString();
        }

        /// <summary>
        /// Computes the cosine of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the cosine of the argument.</returns>
        public string ComputeCos(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Cos)).ToString();
        }

        /// <summary>
        /// Computes the sine of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the sine of the argument.</returns>
        public string ComputeSin(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Sin)).ToString();
        }

        /// <summary>
        /// Computes the hyperbolic cosine of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the hyperbolic cosine of the argument.</returns>
        public string ComputeCosh(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Cosh)).ToString();
        }

        /// <summary>
        /// Computes the hyperbolic sine of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the hyperbolic sine of the argument.</returns>
        public string ComputeSinh(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Sinh)).ToString();
        }

        /// <summary>
        /// Computes the hyperbolic tangent of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the hyperbolic tangent of the argument.</returns>
        public string ComputeTanh(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Tanh)).ToString();
        }

        //		/// <summary>
        //		/// Computes the whole number nearest the argument.
        //		/// </summary>
        //		/// <param name="args">A cell reference, formula, or number.</param>
        //		/// <returns>A string holding the whole number nearest the argument.</returns>
        //		public string ComputeRound(string args)
        //		{
        //			return ComputeMath(args, new MathFunc(Math.Round)).ToString();
        //		}

        /// <summary>
        /// Computes the smallest whole number greater than or equal to the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the smallest whole number greater than or equal to the argument.</returns>
        public string ComputeCeiling(string args)
        {
            //return ComputeMath(args, new MathFunc(Math.Ceiling)).ToString();

            string[] range = args.Split(new char[] { ParseArgumentSeparator });
            int argCount = range.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double d1, d2, d3 = 0;
            if (double.TryParse(GetValueFromArg(range[0]), NumberStyles.Any, null, out d1)
                && double.TryParse(GetValueFromArg(range[1]), NumberStyles.Any, null, out d2))
            {
                if (d1 * d2 <= 0)
                    return "#NUM!";
                d3 = Math.Floor(d1 / d2) * d2;
                if (d2 > 0)
                {
                    while (d3 < d1)
                        d3 += d2;
                }
                else
                {
                    while (d3 > d1)
                        d3 += d2;
                }
            }
            return d3.ToString();
        }


        /// <summary>
        /// Computes the largest whole number less than or equal to the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the largest whole number less than or equal to the argument.</returns>
        public string ComputeFloor(string args)
        {
            //return ComputeMath(args, new MathFunc(Math.Floor)).ToString();

            string[] range = args.Split(new char[] { ParseArgumentSeparator });
            int argCount = range.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double d1, d2, d3 = 0;
            if (double.TryParse(GetValueFromArg(range[0]), NumberStyles.Any, null, out d1)
                && double.TryParse(GetValueFromArg(range[1]), NumberStyles.Any, null, out d2))
            {
                if (d1 * d2 <= 0)
                    return "#NUM!";
                d3 = Math.Ceiling(d1 / d2) * d2;
                if (d2 > 0)
                {
                    while (d3 > d1)
                        d3 -= d2;
                }
                else
                {
                    while (d3 < d1)
                        d3 -= d2;
                }
            }
            return d3.ToString();
        }

        /// <summary>
        /// Computes the natural logarithm of the value in the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding natural logarithm of the value in the argument.</returns>
        public string ComputeLn(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Log)).ToString();
        }

        /// <summary>
        /// Computes the logarithm of the first value using the second value as the base.
        /// </summary>
        /// <param name="argList">A cell reference, formula, or number.</param>
        /// <returns>A string holding logarithm of the value in the argument using the second argument as the base.</returns>
        public string ComputeLog(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 1 && argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x = 10.0;
            double bas = 10.0;
            if (double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out x)
                && x > 0 &&
                (argCount == 1 || double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out bas)))
            {
                return Math.Log(x, bas).ToString();
            }
            else
                return FormulaErrorStrings[wrong_number_arguments];
        }

        /// <summary>
        /// Computes the base 10 logarithm of the value in the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding base 10 logarithm of the value in the argument.</returns>
        public string ComputeLog10(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Log10)).ToString();
        }

        /// <summary>
        /// Computes e raised to the value of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the e raised to the value of the argument.</returns>
        public string ComputeExp(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Exp)).ToString();

        }


        /// <summary>
        /// Computes the square root of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the square root of the argument.</returns>
        public string ComputeSqrt(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Sqrt)).ToString();

        }


        /// <summary>
        /// Computes the absolute value of the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the absolute value of the argument.</returns>
        public string ComputeAbs(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Abs)).ToString();
        }

        /// <summary>
        /// Computes the tangent the argument.
        /// </summary>
        /// <param name="args">A cell reference, formula, or number.</param>
        /// <returns>A string holding the tangent of the argument.</returns>
        public string ComputeTan(string args)
        {
            return ComputeMath(args, new MathFunc(Math.Tan)).ToString();
        }

        private Random rand;
        /// <summary>
        /// Returns an evenly distributed random number greater than or equal
        /// zero and less than one.
        /// </summary>
        /// <param name="args">Ignored. Can be empty.</param>
        /// <returns>A string holding the random number.</returns>
        public string ComputeRand(string args)
        {
            if (rand == null)
                rand = new Random();

            return rand.NextDouble().ToString("0.0#############");
        }

        /// <summary>
        /// Computes the sum of range2
        /// if the item in the range1 satisfies the condition.
        /// </summary>
        /// <param name="range">Range1, condition, range2.</param>
        /// <returns>The conditional sum.</returns>
        public string ComputeSumIf(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            string criteriaRange = args[0];
            string criteria = args[1];//.Replace(TIC, "");

            if (criteria.Length > 1 && criteria[0] == TIC[0]
                && "=><".IndexOf(criteria[1]) == -1)
                criteria = "=" + criteria;
            else
                criteria = criteria.Replace(TIC, "");


            string sumRange = (argCount == 2) ? criteriaRange : args[2];



            string[] s1 = this.GetCellsFromArgs(criteriaRange);
            string[] s2 = this.GetCellsFromArgs(sumRange);


            double sum = 0;
            int count = s1.GetLength(0);
            double d;
            double[] vector = new double[count];
            string s;

            for (int index = 0; index < count; ++index)
            {
                s = s1[index] + criteria;
                //s = this.ParseSimple(s);
                s = this.Parse(s);
                s = this.ComputedValue(s);
                //if(double.TryParse(s , NumberStyles.Any, null, out d) && d > 0)
                if (s == this.TRUEVALUESTR)
                {
                    s = s2[index];
                    s = GetValueFromArg(s);
                    if (double.TryParse(s, NumberStyles.Any, null, out d))
                    {
                        sum += d;
                    }

                }
            }


            return sum.ToString();
            //			string[] ss = range.Split(new char[]{ParseArgumentSeparator});
            //			if(ss.GetLength(0) != 3) 
            //			{
            //				return FormulaErrorStrings[requires_3_args];
            //			}
            //			else 
            //			{
            //				string[] s1 = this.GetCellsFromArgs(ss[0]);
            //				string[] s2 = this.GetCellsFromArgs(ss[2]);
            //
            //			
            //				double sum = 0;
            //				int count = s1.GetLength(0);
            //				double d;
            //				double[] vector = new double[count];
            //				string s;
            //				
            //				for(int index = 0; index < count; ++index)
            //				{
            //					s = s1[index] + ss[1];
            //					s = this.ParseSimple(s);
            //					s = this.ComputeAbs(s);
            //					if(double.TryParse(s , NumberStyles.Any, null, out d) && d > 0)
            //					{
            //						s = s2[index];
            //						s = GetValueFromArg(s);
            //						if(double.TryParse(s , NumberStyles.Any, null, out d))
            //						{
            //							sum += d;
            //						}
            //						
            //					}
            //				}
            //				
            //
            //				return sum.ToString();
            //			}
        }

        //New functions start

        /// <summary>
        /// Computes the declining balance of an asset.
        /// </summary>
        /// <param name="argList"> Delimited string containing the initial cost, 
        /// salvage value, life of asset, period of calculation, and months in initial year.</param>
        /// <returns>Declining balance.</returns>
        public string ComputeDb(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 5 && argCount != 4)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double cost;
            double salvage;
            double life;
            double period;
            double month = 12;

            double deprec = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out cost)
                && double.TryParse(args[1], NumberStyles.Any, null, out salvage)
                && double.TryParse(args[2], NumberStyles.Any, null, out life)
                && double.TryParse(args[3], NumberStyles.Any, null, out period)
                && (argCount == 4 || double.TryParse(args[4], NumberStyles.Any, null, out month)))
            {
                double rate = Math.Round(1 - Math.Pow(salvage / cost, 1 / life), 3);
                double priorDeprec = 0;
                for (int i = 1; i <= period; ++i)
                {

                    if (i == 1)
                        deprec = cost * rate * month / 12;
                    else if (i > life)
                        deprec = (cost - priorDeprec) * rate * (12 - month) / 12;
                    else
                        deprec = (cost - priorDeprec) * rate;

                    priorDeprec += deprec;
                }
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return deprec.ToString();
        }

        /// <summary>
        /// Computes the double declining balance of an asset.
        /// </summary>
        /// <param name="argList"> Delimited string containing the initial cost, 
        /// salvage value, life of asset, period of calculation, factor.</param>
        /// <returns>Double declining balance.</returns>
        public string ComputeDdb(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 5 && argCount != 4)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double cost;
            double salvage;
            double life;
            double period;
            double factor = 2;

            double deprec = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out cost)
                && double.TryParse(args[1], NumberStyles.Any, null, out salvage)
                && double.TryParse(args[2], NumberStyles.Any, null, out life)
                && double.TryParse(args[3], NumberStyles.Any, null, out period)
                && (argCount == 4 || double.TryParse(args[4], NumberStyles.Any, null, out factor)))
            {
                double rate = factor / life;
                double priorDeprec = 0;
                for (int i = 1; i <= period; ++i)
                {
                    if (i == life)
                        deprec = cost - salvage - priorDeprec;
                    else
                        deprec = (cost - priorDeprec) * rate;

                    priorDeprec += deprec;
                }
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return deprec.ToString();
        }


        /// <summary>
        /// Computes the future value of an investment.
        /// </summary>
        /// <param name="argList"> Delimited string containing the rate as percentage per period, 
        /// number of periods, payment per period, present value, and payment type (0 = end of period, 1 = start of period).</param>
        /// <returns>Future value of an investment.</returns>
        public string ComputeFv(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 5 && argCount != 4 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double rate;
            double nper;
            double pmt = 0;
            double pv = 0;
            double type = 0;

            double fv = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out rate)
                && double.TryParse(args[1], NumberStyles.Any, null, out nper)
                && double.TryParse(args[2], NumberStyles.Any, null, out pmt)
                )
            {
                if (argCount >= 4)
                    double.TryParse(args[3], NumberStyles.Any, null, out pv);
                if (argCount == 5)
                    double.TryParse(args[4], NumberStyles.Any, null, out type);

                if (Math.Abs(type) > 0.5)
                    type = 1;
                else
                    type = 0;

                //FV   = (PMT*(1+rate*type)*(1-(1+ rate)^NPER)/rate)-PV*(1+rate)^NPER

                double pow = Math.Pow((1 + rate), nper);
                fv = (pmt * (1 + rate * type) * (1 - pow) / rate) - pv * pow;
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return fv.ToString();
        }

        /// <summary>
        /// Computes the interest payment for a period.
        /// </summary>
        /// <param name="argList"> Delimited string containing the rate as percentage per period, the period,
        /// number of periods, present value, future value, and payment type (0 = end of period, 1 = start of period).</param>
        /// <returns></returns>
        public string ComputeIpmt(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 6 && argCount != 5 && argCount != 4)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double rate;
            double per;
            double nper;
            double pv = 0;
            double fv = 0;
            double type = 0;

            double impt = 0;
            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out rate)
                && double.TryParse(args[1], NumberStyles.Any, null, out per)
                && double.TryParse(args[2], NumberStyles.Any, null, out nper)
                && double.TryParse(args[3], NumberStyles.Any, null, out pv)
                && (argCount == 4 || double.TryParse(args[4], NumberStyles.Any, null, out fv))
                && (argCount <= 5 || double.TryParse(args[5], NumberStyles.Any, null, out type)))
            {
                if (Math.Abs(type) > 0.5)
                    type = 1;
                else
                    type = 0;

                double x0 = Math.Pow((1 + rate), nper);
                double x1 = Math.Pow((1 + rate), per);
                double pmt = (rate * (fv + pv * x0)) / ((1 + rate * type) * (1 - x0));
                double fv1 = (pmt * (1 + rate * type) * (1 - x1) / rate) - pv * x1;
                double x2 = Math.Pow((1 + rate), per - 1);
                double fv2 = (pmt * (1 + rate * type) * (1 - x2) / rate) - pv * x2;

                impt = pmt - fv2 + fv1;
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return impt.ToString();
        }

        /// <summary>
        /// Computes the internal rate of return of a series of cash flows.
        /// </summary>
        /// <param name="argList"> Delimited string containing a range of cells and an initial guess.</param>
        /// <returns>Internal rate of return.</returns>
        /// <remarks>
        /// This IRR calculation uses Newton's method to approximate a root of 
        ///           f(r) = Sum( values[i]/(1+r)^i) = 0
        /// where the Sum index is i = 1 to the number of values. The algorithm returns a value if 
        /// the relative difference between root approximations is less than 1e-5. It fails if this
        /// accuracy is not attained in 20 iterations.
        /// </remarks>
        public string ComputeIrr(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 1 && argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double guess = .1;
            string arg1 = args[0];
            if (argCount == 2)
                double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out guess);

            args = GetCellsFromArgs(arg1);
            argCount = args.GetLength(0);
            double[] values = new double[argCount];
            int i = 0;
            double d;
            foreach (string s in args)
            {
                if (double.TryParse(GetValueFromArg(s), NumberStyles.Any, null, out d))
                    values[i] = d;
                ++i;
            }

            int iteration = 0;
            double gp1, numer, denom, powgp1;
            while (iteration < 20) //hard coded excel values
            {
                numer = 0;
                denom = 0;
                gp1 = Math.Abs(guess + 1);
                powgp1 = gp1;

                for (i = 0; i < argCount; ++i)
                {
                    numer += values[i] / powgp1;
                    powgp1 *= gp1;
                    denom += (i + 1) * values[i] / powgp1;
                }

                numer = numer / denom;

                if (Math.Abs(numer / guess) < 1e-5) //hard coded excel values
                {
                    guess = guess + numer;
                    break;
                }
                guess = guess + numer;
                iteration++;
            }

            if (iteration >= 20)	//hard coded excel values
                return FormulaErrorStrings[invalid_arguments];
            else
                return guess.ToString();
        }

        //used in XIRR calc
        private bool GetDaySerialNumberFromDateTime(string s, out double days)
        {
            bool b = true;
            DateTime dt = DateTime.Now;
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
            b = DateTime.TryParse(s, out dt);
#else
            try
            {
                dt = Convert.ToDateTime(s);
                days = 1 + ((TimeSpan)(dt.Date - dateTime1900)).Days;
                if(Treat1900AsLeapYear && days > 59)
                    days += 1;
            }
            catch 
            {
                b = false;
            }
#endif
            days = 1 + ((TimeSpan)(dt.Date - dateTime1900)).Days;
            if (Treat1900AsLeapYear && days > 59)
                days += 1;

            return b;
        }

        private double[] guesses = new double[] { .1, -.1, .2, -.2, .3, -.3, .4, -.4, .5, -.5, .6, -.6, .7, -.7, .8, -.8, .9, -.9 };
        /// <summary>
        /// Computes the internal rate of return of a series of cash flows.
        /// </summary>
        /// <param name="argList">values, dates, guess. Values and dates are ranges of cells holding the values and dates.
        /// Guess is the initial guess. The first date is the the start date for the calculation.</param>
        /// <returns>Internal rate of return.</returns>
        /// <remarks>
        /// This XIRR calculation is similar to IRR except that the values are not equally spaced in time.
        ///  The algorithm returns a value if 
        /// the relative difference between root approximations is less than 1e-5. It fails if this
        /// accuracy is not attained in 20 iterations.
        /// </remarks>
        public string ComputeXirr(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double guess = .1;
            string arg1 = args[0];
            string arg2 = args[1];
            if (argCount == 3)
                double.TryParse(GetValueFromArg(args[2]), NumberStyles.Any, null, out guess);

            if (Math.Abs(guess) >= 1)
                return FormulaErrorStrings[invalid_arguments];

            double initialGuess = guess;
            int numberOfGuessAdjustments = 0;
            args = GetCellsFromArgs(arg1);
            argCount = args.GetLength(0);
            double[] values = new double[argCount];
            int i = 0;
            double d;
            foreach (string s in args)
            {
                if (double.TryParse(GetValueFromArg(s), NumberStyles.Any, null, out d))
                    values[i] = d;
                ++i;
            }

            args = GetCellsFromArgs(arg2);
            double[] dates = new double[argCount];
            i = 0;
            foreach (string s in args)
            {
                string s1 = GetValueFromArg(s);
                if (double.TryParse(s1, NumberStyles.Any, null, out d)
                    || GetDaySerialNumberFromDateTime(s1, out d))
                    dates[i] = d;
                ++i;
            }

            int iteration = 0;
            double gp1, exp, pow, f, fprime;
            double diff = 0;
            double oldF = 0, oldGuess = 0;
            while (iteration < 100) //hard coded excel values
            {
                f = 0;
                fprime = 0;
                gp1 = guess + 1;
                for (i = 0; i < argCount; ++i)
                {
                    exp = (dates[0] - dates[i]) / 365.0;
                    pow = Math.Pow(Math.Abs(gp1), exp);
                    f += values[i] * pow;
                    fprime += exp * values[i] * pow / gp1;
                }

                oldGuess = guess;
                oldF = f;
                f = f / fprime;
                if (Math.Abs(f / guess) < 1e-6) //hard coded excel values
                {
                    guess = guess - f;
                    break;
                }
                //truncate guess diff at one
                if (f > 1)
                {
                    f = f / 2;
                }
                else if (f < -2)
                    f = -2;

                if (numberOfGuessAdjustments < guesses.Length && iteration > 10 && Math.Abs(f / guess) > diff)
                {

                    guess = guesses[numberOfGuessAdjustments];
                    numberOfGuessAdjustments++;
                    iteration = 0;
                }
                else
                {
                    diff = Math.Abs(f / guess);
                    guess = guess - f;
                }


                iteration++;
            }


            if (iteration >= 100)	//hard coded excel values
                return FormulaErrorStrings[invalid_arguments];
            else
                return guess.ToString();

        }

        /// <summary>
        /// Computes the simple interest payment.
        /// </summary>
        /// <param name="argList"> Delimited string containing the rate as percentage per period, the period,
        /// number of periods, and present value.</param>
        /// <returns>Interest payment.</returns>
        public string ComputeIspmt(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 4)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double rate;
            double per;
            double nper;
            double pv = 0;

            double impt = 0;
            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out rate)
                && double.TryParse(args[1], NumberStyles.Any, null, out per)
                && double.TryParse(args[2], NumberStyles.Any, null, out nper)
                && double.TryParse(args[3], NumberStyles.Any, null, out pv)
                )
            {
                impt = -rate * pv * (nper - per) / nper;
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return impt.ToString();
        }

        /// <summary>
        /// Computes the modified internal rate of return of a series of cash flows.
        /// </summary>
        /// <param name="argList"> Delimited string containing a range of cells, 
        /// finance interest rate, and a reinvest interest rate.</param>
        /// <returns>Modified internal rate of return.</returns>
        public string ComputeMirr(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double val = 0;
            double frate = .1;
            double rrate = .1;
            double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out frate);
            double.TryParse(GetValueFromArg(args[2]), NumberStyles.Any, null, out rrate);

            string arg1 = args[0];
            args = GetCellsFromArgs(arg1);
            argCount = args.GetLength(0);
            double[] values = new double[argCount];
            int i = 0;
            double d;
            foreach (string s in args)
            {
                if (double.TryParse(GetValueFromArg(s), NumberStyles.Any, null, out d))
                    values[i] = d;
                ++i;
            }

            double posValues = 0;
            double negValues = 0;
            double rpow = 1;
            double fpow = 1 + frate;

            for (i = 0; i < argCount; ++i)
            {
                rpow *= (1 + rrate);

                if (values[i] > 0)
                {
                    posValues += values[i] / rpow;
                }
                else
                {
                    negValues += values[i] / fpow;
                }
                fpow *= (1 + frate);
            }

            posValues = -posValues * rpow;
            negValues = negValues * (1 + frate);
            val = Math.Pow(posValues / negValues, 1d / (argCount - 1d)) - 1d;
            return val.ToString();
        }

        /// <summary>
        /// Computes the number of periods in an investment.
        /// </summary>
        /// <param name="argList">Delimited string containing the rate as percentage per period, 
        /// payment per period, present value, future value, and payment type (0 = end of period, 1 = start of period).</param>
        /// <returns>Number of periods.</returns>
        public string ComputeNper(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 5 && argCount != 4 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double rate;
            double pmt = 0;
            double pv = 0;
            double fv = 0;
            double type = 0;

            double val = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out rate)
                && double.TryParse(args[1], NumberStyles.Any, null, out pmt)
                && double.TryParse(args[2], NumberStyles.Any, null, out pv)
                && (argCount == 3 || double.TryParse(args[3], NumberStyles.Any, null, out fv))
                && (argCount <= 4 || double.TryParse(args[4], NumberStyles.Any, null, out type)))
            {

                if (Math.Abs(type) > 0.5)
                    type = 1;
                else
                    type = 0;

                //NPER = LOG10((PMT*(1+rate*type)-FV*rate)/(PMT*(1+rate*type)+PV*rate))/ LOG10(1+rate)
                val = Math.Log10((pmt * (1 + rate * type) - fv * rate)
                    / (pmt * (1 + rate * type) + pv * rate)
                    ) / Math.Log10(1 + rate);
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return val.ToString();
        }

        /// <summary>
        /// Computes the net present value of an investment.
        /// </summary>
        /// <param name="argList">Delimited string containing the rate as percentage per period  
        /// and a list of invested values.</param>
        /// <returns>Net present value.</returns>
        public string ComputeNpv(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount < 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double rate;
            double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out rate);

            double val = 0;
            double denom = 1;
            double d;

            for (int i = 1; i < argCount; ++i)
            {
                string r = args[i];

                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        if (double.TryParse(GetValueFromArg(s), NumberStyles.Any, null, out d))
                        {
                            denom *= 1 + rate;
                            val += d / denom;
                        }
                    }
                }
                else
                {
                    if (double.TryParse(GetValueFromArg(r), NumberStyles.Any, null, out d))
                    {
                        denom *= 1 + rate;
                        val += d / denom;
                    }
                }
            }
            return val.ToString();
        }

        /// <summary>
        /// Computes the payment for a loan.
        /// </summary>
        /// <param name="argList">Delimited string containing the rate as percentage per period, 
        /// number of periods, present value, future value, and payment type (0 = end of period, 1 = start of period).</param>
        /// <returns>Payment amount.</returns>
        public string ComputePmt(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 5 && argCount != 4 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double rate;
            double nper = 0;
            double pv = 0;
            double fv = 0;
            double type = 0;

            double val = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out rate)
                && double.TryParse(args[1], NumberStyles.Any, null, out nper)
                && double.TryParse(args[2], NumberStyles.Any, null, out pv)
                && (argCount == 3 || double.TryParse(args[3], NumberStyles.Any, null, out fv))
                && (argCount <= 4 || double.TryParse(args[4], NumberStyles.Any, null, out type)))
            {

                if (Math.Abs(type) > 0.5)
                    type = 1;
                else
                    type = 0;

                //PMT  = (rate*(FV+PV*(1+ rate)^NPER))/((1+rate*type)*(1-(1+ rate)^NPER))

                double pow = Math.Pow(1 + rate, nper);
                val = (rate * (fv + pv * pow)) / ((1 + rate * type) * (1 - pow));
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return val.ToString();
        }

        /// <summary>
        /// Computes the principal payment for a period.
        /// </summary>
        /// <param name="argList"> Delimited string containing the rate as percentage per period, the period,
        /// number of periods, present value, future value, and payment type (0 = end of period, 1 = start of period).</param>
        /// <returns>Principal payment.</returns>
        public string ComputePpmt(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 6 && argCount != 5 && argCount != 4)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double rate;
            double per;
            double nper;
            double pv = 0;
            double fv = 0;
            double type = 0;

            double ppmt = 0;
            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out rate)
                && double.TryParse(args[1], NumberStyles.Any, null, out per)
                && double.TryParse(args[2], NumberStyles.Any, null, out nper)
                && double.TryParse(args[3], NumberStyles.Any, null, out pv)
                && (argCount == 4 || double.TryParse(args[4], NumberStyles.Any, null, out fv))
                && (argCount <= 5 || double.TryParse(args[5], NumberStyles.Any, null, out type)))
            {
                if (Math.Abs(type) > 0.5)
                    type = 1;
                else
                    type = 0;

                double x0 = Math.Pow((1 + rate), nper);
                //double x1 = Math.Pow((1 + rate), per);
                double pmt = (rate * (fv + pv * x0)) / ((1 + rate * type) * (1 - x0));
                double ipmt = 0;
                double.TryParse(ComputeIpmt(argList), NumberStyles.Any, null, out ipmt);
                //double fv1 = (pmt*(1 + rate * type) * (1 - x1) /rate) - pv * x1;
                //double x2 = Math.Pow((1 + rate), per - 1);
                //double fv2 = (pmt*(1 + rate * type) * (1 - x2) /rate) - pv * x2;

                ppmt = pmt - ipmt;
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return ppmt.ToString();
        }

        /// <summary>
        /// Computes the present value of an investment.
        /// </summary>
        /// <param name="argList"> Delimited string containing the rate as percentage per period, 
        /// number of periods, payment per period, future value, and payment type (0 = end of period, 1 = start of period).</param>
        /// <returns>Present value.</returns>
        public string ComputePv(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 5 && argCount != 4 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double rate;
            double nper;
            double pmt = 0;
            double fv = 0;
            double type = 0;

            double pv = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out rate)
                && double.TryParse(args[1], NumberStyles.Any, null, out nper)
                && double.TryParse(args[2], NumberStyles.Any, null, out pmt)
                )
            {
                if (argCount >= 4)
                    double.TryParse(args[3], NumberStyles.Any, null, out pv);
                if (argCount == 5)
                    double.TryParse(args[4], NumberStyles.Any, null, out type);

                if (Math.Abs(type) > 0.5)
                    type = 1;
                else
                    type = 0;

                if (Math.Abs(type) > 0.5)
                    type = 1;
                else
                    type = 0;

                //PV   = (PMT*(1+rate*type)*(1-(1+rate)^NPER)-rate*FV)/(rate*(1+rate)^NPER)
                double pow = Math.Pow(1 + rate, nper);
                pv = (pmt * (1 + rate * type) * (1 - pow) - rate * fv) / (rate * pow);
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return pv.ToString();
        }

        /// <summary>
        /// Computes the internal rate of return of a series of cash flows.
        /// </summary>
        /// <param name="argList"> Delimited string containing a range of cells and an initial guess.</param>
        /// <returns>Internal rate of return.</returns>
        /// <remarks>
        /// This IRR calculation uses Newton's method to approximate a root of 
        ///           f(r) = Sum( values[i]/(1+r)^i) = 0
        /// where the Sum index is i = 1 to the number of values. The algorithm returns a value if 
        /// the relative difference between root approximations is less than 1e-7. It fails if this
        /// accuracy is not attained in 20 iterations.
        /// </remarks>
        public string ComputeRate(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 6 && argCount != 5 && argCount != 4 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double nper;
            double pmt;
            double pv;
            double fv = 0;
            double type = 0;
            double r = .1;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out nper)
                && double.TryParse(args[1], NumberStyles.Any, null, out pmt)
                && double.TryParse(args[2], NumberStyles.Any, null, out pv)
                && (argCount == 3 || double.TryParse(args[3], NumberStyles.Any, null, out fv))
                && (argCount <= 4 || double.TryParse(args[4], NumberStyles.Any, null, out type))
                && (argCount <= 5 || double.TryParse(args[5], NumberStyles.Any, null, out r)))
            {
                if (Math.Abs(type) > 0.5)
                    type = 1;
                else
                    type = 0;

                // seeking root of f(r) = 0 where
                // f(r) = pmt - u(r) / v(r) where
                // u(r) = r * (fv + pv * (1+r)^nper
                // v(r) = (1 + r * type) * (1 - (1+r)^nper);

                int iteration = 0;
                double ur,  //u(r)
                    vr,     //v(r)
                    upr,    //u'(r)
                    vpr,    //v'(r)
                    fr,     //f(r)
                    fpr,    //f'(r)
                    rp1,    // r + 1
                    rn,     // (1 + r)^nper
                    rn1,    // (1 + r)^(nper-1)
                    h;      // h from Taylor series expansion of f(r+h).
                while (iteration < 20) //hard coded excel values
                {
                    rp1 = Math.Abs(r + 1);
                    rn = Math.Pow(rp1, nper);
                    rn1 = Math.Pow(rp1, nper - 1);

                    ur = r * (fv + pv * rn);
                    vr = (1 + r * type) * (1 - rn);
                    upr = fv + pv * (rn + r * nper * rn1);
                    vpr = (1 + r * type) * (-nper) * rn1 + (1 - rn) * type;
                    fpr = (upr * vr - ur * vpr) / (vr * vr);
                    fr = pmt - ur / vr;
                    h = fr / fpr;

                    if (Math.Abs(h) < 1e-7) //hard coded excel values
                    {
                        r = r + h;
                        break;
                    }
                    r = r + h;
                    iteration++;
                }


                if (iteration < 20)	//hard coded excel values
                    return r.ToString();
            }
            return FormulaErrorStrings[invalid_arguments];
        }

        /// <summary>
        /// Computes the straight-line depreciation of an asset per period.
        /// </summary>
        /// <param name="argList"> Delimited string containing the cost, 
        /// salvage value, and life.</param>
        /// <returns>Depreciation.</returns>
        public string ComputeSln(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[this.requires_3_args];
            }
            double cost;
            double salvage;
            double life;

            double sln = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out cost)
                && double.TryParse(args[1], NumberStyles.Any, null, out salvage)
                && double.TryParse(args[2], NumberStyles.Any, null, out life)
                )
            {

                sln = (cost - salvage) / life;
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return sln.ToString();
        }

        /// <summary>
        /// Computes the sum of years digits depreciation of an asset per period.
        /// </summary>
        /// <param name="argList"> Delimited string containing the cost, 
        /// salvage value, life, and period.</param>
        /// <returns>Depreciation for the requested period.</returns>
        public string ComputeSyd(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 4)
            {
                return FormulaErrorStrings[this.wrong_number_arguments];
            }
            double cost;
            double salvage;
            double life;
            double per;

            double syd = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out cost)
                && double.TryParse(args[1], NumberStyles.Any, null, out salvage)
                && double.TryParse(args[2], NumberStyles.Any, null, out life)
                && double.TryParse(args[3], NumberStyles.Any, null, out per)
                )
            {

                syd = (cost - salvage) * (life - per + 1) * 2 / (life * (life + 1));
            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return syd.ToString();
        }


        /// <summary>
        /// Computes the variable declining balance of an asset
        /// </summary>
        /// <param name="argList"> Delimited string containing the initial cost, 
        /// salvage value, life of asset, period of calculation, factor.</param>
        /// <returns>Variable declining balance.</returns>
        public string ComputeVdb(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 5 && argCount != 6 && argCount != 7)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double cost;
            double salvage;
            double life;
            double start;
            double end;
            double factor = 2;
            double no_switch = 0;

            double deprec = 0;
            double vdb = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out cost)
                && double.TryParse(args[1], NumberStyles.Any, null, out salvage)
                && double.TryParse(args[2], NumberStyles.Any, null, out life)
                && double.TryParse(args[3], NumberStyles.Any, null, out start)
                && double.TryParse(args[4], NumberStyles.Any, null, out end)
                && (argCount == 5 || double.TryParse(args[5], NumberStyles.Any, null, out factor))
                && (argCount <= 6 || double.TryParse(args[6], NumberStyles.Any, null, out no_switch))
                )
            {
                double rate = factor / life;
                double priorDeprec = 0;
                double sldeprec = (cost - salvage) / life;

                bool change = Math.Abs(no_switch) > .5;

                //for(int i = 1; i <= end; ++i)
                int i = 0;
                while (i < end)
                {
                    i += 1;
                    if (i == life)
                        deprec = cost - salvage - priorDeprec;
                    else
                    {
                        deprec = (cost - priorDeprec) * rate;
                        if (change && deprec < sldeprec)
                            deprec = sldeprec;
                    }

                    priorDeprec += deprec;
                    if (i > start && i <= end)
                        vdb += deprec;
                    else if (i - end < .99 && i - end > 0) //1
                    {
                        vdb += deprec * (end - i + 1);
                    }
                }

            }
            else
                return FormulaErrorStrings[invalid_arguments];

            return vdb.ToString();
        }



        /// <summary>
        /// The inverse of Cosh.
        /// </summary>
        /// <param name="args">Value >= 1.</param>
        /// <returns>ACosh(value).</returns>
        public string ComputeAcosh(string args)
        {
            double z;
            if (double.TryParse(GetValueFromArg(args), NumberStyles.Any, null, out z)
                && z >= 1)
            {
                z = Math.Log(z + Math.Sqrt(z * z - 1));
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return z.ToString();
        }

        /// <summary>
        /// The inverse of Sinh.
        /// </summary>
        /// <param name="args">Value.</param>
        /// <returns>ASinh(value).</returns>
        public string ComputeAsinh(string args)
        {
            double z;
            if (double.TryParse(GetValueFromArg(args), NumberStyles.Any, null, out z))
            {
                z = Math.Sign(z) * Math.Log(Math.Abs(z) + Math.Sqrt(z * z + 1));
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return z.ToString();
        }

        /// <summary>
        /// The inverse of Tanh.
        /// </summary>
        /// <param name="args">|Value| &lt; 1.</param>
        /// <returns>ATanh(value).</returns>
        public string ComputeAtanh(string args)
        {
            double z;
            double z1;
            if (double.TryParse(GetValueFromArg(args), NumberStyles.Any, null, out z)
                && ((z1 = Math.Abs(z)) < 1))
            {
                z = .5 * Math.Sign(z) * Math.Log((1 + z1) / (1 - z1));
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return z.ToString();
        }

        /// <summary>
        /// The ArcTangent of the x and y values. 
        /// </summary>
        /// <param name="argList">x_value and y_value.</param>
        /// <returns>Angle whose tangent is y_value/x_value.</returns>
        public string ComputeAtan2(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[this.requires_2_args];
            }
            double x;
            double y;
            double atan2 = 0;
            if (double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out x)
                && double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out y))
            {
                atan2 = Math.Atan2(y, x);
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return atan2.ToString();
        }

        /// <summary>
        /// The number of combinations of a given number of items. 
        /// </summary>
        /// <param name="argList">number, number_items.</param>
        /// <returns>The number of combinations.</returns>
        public string ComputeCombin(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[this.requires_2_args];
            }
            //n things taken k at the time
            double nd;
            double kd;
            double combin = 0;
            if (double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out nd)
                && double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out kd))
            {
                int k = (int)(kd + 0.1);
                int n = (int)(nd + 0.1);
                combin = comb(k, n);
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return combin.ToString();
        }

        /// <summary>
        /// Converts radians into degrees.
        /// </summary>
        /// <param name="args">Value in radians.</param>
        /// <returns>Degrees.</returns>
        public string ComputeDegrees(string args)
        {
            double radians;
            double degrees = 0;
            if (double.TryParse(GetValueFromArg(args), NumberStyles.Any, null, out radians))
            {
                degrees = 180 * radians / Math.PI;
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return degrees.ToString();
        }

        /// <summary>
        /// Rounds up to larger in magnitude even number.
        /// </summary>
        /// <param name="args">Number to be rounded.</param>
        /// <returns>Rounded even value.</returns>
        public string ComputeEven(string args)
        {
            double number;
            double even = 0;
            if (double.TryParse(GetValueFromArg(args), NumberStyles.Any, null, out number))
            {
                int sgn = Math.Sign(number);
                number = Math.Abs(number);
                if ((int)number != number)
                    number = (int)(number + 1);

                if ((number % 2) == 1)
                    even = sgn * (number + 1);
                else
                    even = sgn * number;

            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return even.ToString();
        }

        private static int[] factorialTable = new int[]{
                                                           1, 1, 2, 6, 24, 120, 720, 
                                                           5040, 40320, 362880, 3628800, 
                                                           39916800, 479001600};

        /// <summary>
        /// Factorial of a given number.
        /// </summary>
        /// <param name="args">x.</param>
        /// <returns>x!.</returns>
        public string ComputeFact(string args)
        {
            double number = 0;
            double fact = 0;
            if (double.TryParse(GetValueFromArg(args), NumberStyles.Any, null, out number)
                && number >= 0)
            {
                int x = (int)number;
                if (x > 12)
                {
                    fact = factorialTable[12];
                    for (int i = 13; i <= x; i++)
                        fact *= i;
                }
                else
                    fact = factorialTable[x];
            }
            else if (number < 0)
                return "#NUM!";
            else
                return FormulaErrorStrings[invalid_arguments];
            return fact.ToString();
        }

        /// <summary>
        /// Rounds up to larger in magnitude odd number.
        /// </summary>
        /// <param name="args">Number to be rounded.</param>
        /// <returns>Rounded odd value.</returns>
        public string ComputeOdd(string args)
        {
            double number;
            double odd = 0;
            if (double.TryParse(GetValueFromArg(args), NumberStyles.Any, null, out number))
            {
                int sgn = Math.Sign(number);
                number = Math.Abs(number);
                if ((int)number != number)
                    number = (int)(number + 1);

                if ((number % 2) == 0)
                    odd = sgn * (number + 1);
                else
                    odd = sgn * number;

            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return odd.ToString();
        }

        /// <summary>
        /// Converts degrees into radians.
        /// </summary>
        /// <param name="args">Value in degrees.</param>
        /// <returns>Radians.</returns>
        public string ComputeRadians(string args)
        {
            double radians = 0;
            double degrees;
            if (double.TryParse(GetValueFromArg(args), NumberStyles.Any, null, out degrees))
            {
                radians = Math.PI * degrees / 180;
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return radians.ToString();
        }

        /// <summary>
        /// Rounds a number to a specified number of digits.
        /// </summary>
        /// <param name="argList">Number and number of digits.</param>
        /// <returns>Rounded number.</returns>
        public string ComputeRound(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount == 1)
                return ComputeMath(argList, new MathFunc(Math.Round)).ToString();

            if (argCount != 2)
            {
                return FormulaErrorStrings[this.invalid_arguments];
            }
            double x = 0;
            double digits = 0;
            double round = 0;
            string numStr = GetValueFromArg(args[0]);
            string digStr = GetValueFromArg(args[1]);

            if ((numStr.Length == 0 || double.TryParse(numStr, NumberStyles.Any, null, out x))
            && (digStr.Length == 0 || double.TryParse(digStr, NumberStyles.Any, null, out digits)))
            {
                if (digits > 0)
                    round = Math.Round(x, (int)digits);
                else
                {
                    double mult = Math.Pow(10, -digits);
                    round = Math.Round(x / mult) * mult;
                }
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return round.ToString();
        }

        /// <summary>
        /// Rounds a number to a specified number of digits.
        /// </summary>
        /// <param name="argList">Number and number of digits.</param>
        /// <returns>Rounded number.</returns>
        public string ComputeRounddown(string argList)
        {
            double x = 0;

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount == 1)
            {
                double.TryParse(GetValueFromArg(argList), NumberStyles.Any, null, out x);
                x = x - .5 * Math.Sign(x);
                return ComputeRound(string.Format("{0}", x));
            }
            else if (argCount != 2)
            {
                return FormulaErrorStrings[this.invalid_arguments];
            }
            double digits = 0;
            double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out x);
            double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out digits);
            x = x - .5 * Math.Pow(10, -digits) * Math.Sign(x);
            return ComputeRound(string.Format("{0}{1}{2}", x, ParseArgumentSeparator, digits));
        }

        /// <summary>
        /// Rounds a number to a specified number of digits.
        /// </summary>
        /// <param name="argList">Number and number of digits.</param>
        /// <returns>Rounded number.</returns>
        public string ComputeRoundup(string argList)
        {

            double x = 0;

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount == 1)
            {
                double.TryParse(GetValueFromArg(argList), NumberStyles.Any, null, out x);
                x = x + .5 * Math.Sign(x);
                return ComputeRound(string.Format("{0}", x));
            }
            else if (argCount != 2)
            {
                return FormulaErrorStrings[this.invalid_arguments];
            }
            double digits = 0;
            double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out x);
            double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out digits);
            x = x + .5 * Math.Pow(10, -digits) * Math.Sign(x);
            return ComputeRound(string.Format("{0}{1}{2}", x, ParseArgumentSeparator, digits));

        }

        private string TRUEVALUESTR = "TRUE";
        private string FALSEVALUESTR = "FALSE";


        ////		/// <summary>
        ////		/// Sums the cells specified by some criteria.
        ////		/// </summary>
        ////		/// <param name="range">The criteria range, the criteria, and the sum range.</param>
        ////		public string ComputeSumif(string argList)
        ////		{
        ////			string[] args = argList.Split(new char[]{ParseArgumentSeparator});
        ////			int argCount = args.GetLength(0);
        ////			if(argCount != 2 && argCount != 3)
        ////			{
        ////				return FormulaErrorStrings[wrong_number_arguments];
        ////			}
        ////			string criteriaRange = args[0];
        ////			string criteria = args[1];//.Replace(TIC, "");
        ////
        ////			if(criteria.Length > 1 && criteria[0] == TIC[0]
        ////				&& "=><".IndexOf(criteria[1]) == -1)
        ////				criteria = "=" + criteria;
        ////			else
        ////				criteria = criteria.Replace(TIC, "");
        ////
        ////
        ////			string sumRange = (argCount == 2) ? criteriaRange : args[2];
        ////			
        ////			
        ////			
        ////			string[] s1 = this.GetCellsFromArgs(criteriaRange);
        ////			string[] s2 = this.GetCellsFromArgs(sumRange);
        ////
        ////			
        ////			double sum = 0;
        ////			int count = s1.GetLength(0);
        ////			double d;
        ////			double[] vector = new double[count];
        ////			string s;
        ////				
        ////			for(int index = 0; index < count; ++index)
        ////			{
        ////				s = s1[index] + criteria;
        ////				//s = this.ParseSimple(s);
        ////				s = this.Parse(s);
        ////				s = this.ComputedValue(s);
        ////				//if(double.TryParse(s , NumberStyles.Any, null, out d) && d > 0)
        ////				if(s == this.TRUEVALUESTR)
        ////				{
        ////					s = s2[index];
        ////					s = GetValueFromArg(s);
        ////					if(double.TryParse(s , NumberStyles.Any, null, out d))
        ////					{
        ////						sum += d;
        ////					}
        ////						
        ////				}
        ////			}
        ////				
        ////
        ////			return sum.ToString();
        ////		}

        /// <summary>
        /// Returns the sum of the square of all values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the sum of the squares of all values listed in the argument.</returns>
        public string ComputeSumsq(string range)
        {
            double sum = 0;
            string s1;
            double d;

            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            //TraceUtil.TraceExceptionCatched(ex);
                            //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            //	throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0)
                        {
                            if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            {
                                sum = sum + d * d;
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                        {
                            sum = sum + d * d;
                        }
                    }

                }
            }
            return sum.ToString();
        }

        /// <summary>
        /// Returns the sum of the differences of squares of the two ranges.
        /// </summary>
        /// <param name="range">x_range and y_range.</param>
        /// <returns>A string holding sum of the differences of squares.</returns>
        public string ComputeSumx2my2(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[this.requires_2_args];
            }
            string x_range = args[0];
            string y_range = args[1];

            string[] x = this.GetCellsFromArgs(x_range);
            string[] y = this.GetCellsFromArgs(y_range);

            int len = x.GetLength(0);
            if (len != y.GetLength(0))
                return FormulaErrorStrings[this.invalid_arguments];

            double sum = 0;
            double x1 = 0;
            double y1 = 0;
            for (int i = 0; i < len; ++i)
            {
                if (double.TryParse(GetValueFromArg(x[i]), NumberStyles.Any, null, out x1)
                    && double.TryParse(GetValueFromArg(y[i]), NumberStyles.Any, null, out y1))
                {
                    sum += (x1 * x1 - y1 * y1);
                }
            }

            return sum.ToString();
        }

        /// <summary>
        /// Returns the sum of the sums of squares of the two ranges.
        /// </summary>
        /// <param name="range">x_range and y_range.</param>
        /// <returns>A string holding sum of the sums of squares.</returns>
        public string ComputeSumx2py2(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[this.requires_2_args];
            }
            string x_range = args[0];
            string y_range = args[1];

            string[] x = this.GetCellsFromArgs(x_range);
            string[] y = this.GetCellsFromArgs(y_range);

            int len = x.GetLength(0);
            if (len != y.GetLength(0))
                return FormulaErrorStrings[this.invalid_arguments];

            double sum = 0;
            double x1 = 0;
            double y1 = 0;
            for (int i = 0; i < len; ++i)
            {
                if (double.TryParse(GetValueFromArg(x[i]), NumberStyles.Any, null, out x1)
                    && double.TryParse(GetValueFromArg(y[i]), NumberStyles.Any, null, out y1))
                {
                    sum += (x1 * x1 + y1 * y1);
                }
            }

            return sum.ToString();
        }


        /// <summary>
        /// Returns the sum of the squares of the differences between two ranges.
        /// </summary>
        /// <param name="range">x_range and y_range.</param>
        /// <returns>A string holding sum of the squares of the differences.</returns>
        public string ComputeSumxmy2(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[this.requires_2_args];
            }
            string x_range = args[0];
            string y_range = args[1];

            string[] x = this.GetCellsFromArgs(x_range);
            string[] y = this.GetCellsFromArgs(y_range);

            int len = x.GetLength(0);
            if (len != y.GetLength(0))
                return FormulaErrorStrings[this.invalid_arguments];

            double sum = 0;
            double x1 = 0;
            double y1 = 0;
            for (int i = 0; i < len; ++i)
            {
                if (double.TryParse(GetValueFromArg(x[i]), NumberStyles.Any, null, out x1)
                    && double.TryParse(GetValueFromArg(y[i]), NumberStyles.Any, null, out y1))
                {
                    sum += Math.Pow(x1 - y1, 2);
                }
            }

            return sum.ToString();
        }

        /// <summary>
        /// Specifies if 1900 should be treated as Leap Year (Excel Compatibility)
        /// </summary>
        public static bool Treat1900AsLeapYear = true;

        private DateTime dateTime1900 = new DateTime(1900, 1, 1, 0, 0, 0);
        /// <summary>
        /// Returns the number of days since 01 Jan 1900.
        /// </summary>
        /// <param name="argList">Year, month, and day.</param>
        /// <returns>Number of days.</returns>
        public string ComputeDate(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[this.wrong_number_arguments];
            }
            double year;
            double month;
            double day;
            int days = 0;
            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out year)
                && double.TryParse(args[1], NumberStyles.Any, null, out month)
                && double.TryParse(args[2], NumberStyles.Any, null, out day)
                )
            {
                //				if(year < 1900)
                //					year += 1900;
                //			     days = 1 + ((TimeSpan)(new DateTime((int)year, (int)month, (int)day, 0, 0, 0) - dateTime1900)).Days;
                //				if(Treat1900AsLeapYear && days > 59)
                //					days += 1;
                days = GetSerialDateFromDate((int)year, (int)month, (int)day);
            }
            return days.ToString();
        }

        private int GetSerialDateFromDate(int y, int m, int d)
        {
            int days = 0;
            if (y < 1900)
                y += 1900;
            if (m > 12)
            {
                y = y + m - 12;
                m = m - 12;
            }
            days = 1 + ((TimeSpan)(new DateTime(y, m, d, 0, 0, 0) - dateTime1900)).Days;
            if (Treat1900AsLeapYear && days > 59)
                days += 1;
            return days;
        }

        private DateTime GetDateFromSerialDate(int days)
        {

            days -= 1;
            if (Treat1900AsLeapYear && days > 59)
                days -= 1;
            return dateTime1900.AddDays(days);
        }

        /// <summary>
        /// Returns the number of days since 01 Jan 1900.
        /// </summary>
        /// <param name="argList">Text containing a date.</param>
        /// <returns>Number of days.</returns>
        public string ComputeDatevalue(string argList)
        {
            argList = GetValueFromArg(argList);
            DateTime dt;
            try
            {
                dt = DateTime.Parse(argList.Replace(TIC, ""));
            }
            catch
            {
                return FormulaErrorStrings[this.invalid_arguments];
            }

            int days = 1 + ((TimeSpan)(dt - dateTime1900)).Days;
            if (Treat1900AsLeapYear && days > 59)
                days += 1;

            return days.ToString();
        }

        /// <summary>
        /// Returns the day of the serial number date.
        /// </summary>
        /// <param name="argList">Serial number date.</param>
        /// <returns>Day.</returns>
        public string ComputeDay(string argList)
        {

            double day = 1;
            double serialdate;
            DateTime dt;
            string result = GetValueFromArg(argList);
            result = result.Replace("\"", "");
            if (double.TryParse(result, NumberStyles.Any, null, out serialdate))
            {
                //serialdate = serialdate - 1 - ((Treat1900AsLeapYear && serialdate > 59) ?  1 : 0);
                dt = GetDateFromSerialDate((int)serialdate);//dateTime1900.AddDays(serialdate);
                day = dt.Day;
            }
            else if (DateTime.TryParse(result, out dt))
                day = dt.Day;
            else
                return FormulaErrorStrings[invalid_arguments];


            return day.ToString();
        }

        /// <summary>
        /// Number of days between 2 dates using 360 day year.
        /// </summary>
        /// <param name="argList">Serial number date1, serial number date1, and method.</param>
        /// <returns>Days between the dates.</returns>
        public string ComputeDays360(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double serialdate1;
            double serialdate2;
            bool method = false;
            int days = 0;

            if (double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out serialdate1)
                && double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out serialdate2)
                && (argCount == 2 || (method = (args[2] == TRUEVALUESTR)))
                )
            {
                DateTime dt1 = GetDateFromSerialDate((int)serialdate1);
                DateTime dt2 = GetDateFromSerialDate((int)serialdate2);
                bool flipSign = false;

                if (dt1.Day == 31)
                    dt1 = dt1.AddDays(-1);
                if (dt2.Day == 31 && !method && dt1.Day < 30)
                    dt2 = dt2.AddDays(1);
                else if (dt2.Day == 31)
                    dt2 = dt2.AddDays(-1);

                if (dt2 < dt1)
                {
                    flipSign = true;
                    DateTime t = dt1;
                    dt1 = dt2;
                    dt2 = t;
                }
                days = dt2.Day - dt1.Day;
                days += 30 * (dt2.Month - dt1.Month);
                days += 360 * (dt2.Year - dt1.Year);
                if (flipSign)
                    days = -days;
            }
            else
                return FormulaErrorStrings[invalid_arguments];


            return days.ToString();
        }

        /// <summary>
        /// Returns the hour of the given time.
        /// </summary>
        /// <param name="argList">Given time.</param>
        /// <returns>Hour.</returns>
        public string ComputeHour(string argList)
        {

            double time;
            DateTime dt = DateTime.Now;
            argList = GetValueFromArg(argList);
            if (argList.IndexOf(TIC) > -1)
            {
                argList = argList.Replace(TIC, "");
                dt = DateTime.Parse(argList);
            }
            else
            {
                try
                {
                    if (double.TryParse(argList, NumberStyles.Any, null, out time))
                    {
#if !WinRT
                        dt = DateTime.FromOADate(time);
#else
                        dt = DateTime.Parse(argList);
#endif
                    }
                    else
                        dt = DateTime.Parse(argList);
                }
                catch
                {
                    return FormulaErrorStrings[invalid_arguments];
                }
            }

            return dt.Hour.ToString();
        }

        /// <summary>
        /// Returns the Minute of the given time.
        /// </summary>
        /// <param name="argList">Given time.</param>
        /// <returns>Minute.</returns>
        public string ComputeMinute(string argList)
        {

            double time;
            DateTime dt = DateTime.Now;
            argList = GetValueFromArg(argList);
            if (argList.IndexOf(TIC) > -1)
            {
                argList = argList.Replace(TIC, "");
                dt = DateTime.Parse(argList);
            }
            else
            {
                try
                {
                    if (double.TryParse(argList, NumberStyles.Any, null, out time))
                    {
#if !WinRT
                        dt = DateTime.FromOADate(time);
#else
                        dt = DateTime.Parse(argList);
#endif
                    }
                    else
                        dt = DateTime.Parse(argList);
                }
                catch
                {
                    return FormulaErrorStrings[invalid_arguments];
                }
                //				try
                //				{
                //					dt = DateTime.Parse(argList);
                //				}
                //				catch
                //				{
                //					if(double.TryParse(argList, NumberStyles.Any, null, out time))
                //					{
                //						dt = DateTime.FromOADate(time);
                //					} 
                //				}
            }

            return dt.Minute.ToString();
        }
        /// <summary>
        /// Returns the second of the given time.
        /// </summary>
        /// <param name="argList">Given time.</param>
        /// <returns>Second.</returns>
        public string ComputeSecond(string argList)
        {

            double time;
            DateTime dt = DateTime.Now;
            argList = GetValueFromArg(argList);
            if (argList.IndexOf(TIC) > -1)
            {
                argList = argList.Replace(TIC, "");
                dt = DateTime.Parse(argList);
            }
            else
            {
                try
                {
                    if (double.TryParse(argList, NumberStyles.Any, null, out time))
                    {
#if !WinRT
                        dt = DateTime.FromOADate(time);
#else
                        dt = DateTime.Parse(argList);
#endif
                    }
                    else
                        dt = DateTime.Parse(argList);
                }
                catch
                {
                    return FormulaErrorStrings[invalid_arguments];
                }
                //				try
                //				{
                //					dt = DateTime.Parse(argList);
                //				}
                //				catch
                //				{
                //					if(double.TryParse(argList, NumberStyles.Any, null, out time))
                //					{
                //						dt = DateTime.FromOADate(time);
                //					} 
                //				}
            }

            return dt.Second.ToString();
        }

        /// <summary>
        /// Returns the month of the given date.
        /// </summary>
        /// <param name="argList">Given time.</param>
        /// <returns>Month.</returns>
        public string ComputeMonth(string argList)
        {

            double month = 1;
            double serialdate;
            DateTime dt;
            string result = GetValueFromArg(argList);
            result = result.Replace("\"", "");
            if (double.TryParse(result, NumberStyles.Any, null, out serialdate))
            {
                //serialdate = serialdate - 1 - ((Treat1900AsLeapYear && serialdate > 59) ?  1 : 0);
                dt = GetDateFromSerialDate((int)serialdate);//dateTime1900.AddDays(serialdate);
                month = dt.Month;
            }
            else if (DateTime.TryParse(result, out dt))
                month = dt.Month;
            else
                return FormulaErrorStrings[invalid_arguments];


            return month.ToString();
        }

        /// <summary>
        /// Returns the current date and time as a date serial number.
        /// </summary>
        /// <param name="argList">Ignored.</param>
        /// <returns>Current date and time as serial number.</returns>
        public string ComputeNow(string argList)
        {
            DateTime dt = DateTime.Now;
#if !WinRT
            return dt.ToOADate().ToString();
#else
            return dt.ToString();
#endif
        }

        /// <summary>
        /// Returns the current date as a date serial number.
        /// </summary>
        /// <param name="argList">Ignored.</param>
        /// <returns>Current date as date serial number.</returns>
        public string ComputeToday(string argList)
        {
            DateTime dt = DateTime.Now;
            return this.GetSerialDateFromDate(dt.Year, dt.Month, dt.Day).ToString();
        }

        /// <summary>
        /// Returns a fraction of a day.
        /// </summary>
        /// <param name="argList">Hour, minute, and second.</param>
        /// <returns>Fraction of a day.</returns>
        public string ComputeTime(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[this.wrong_number_arguments];
            }
            double hour;
            double minute;
            double second;
            double time = 0;
            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out hour)
                && double.TryParse(args[1], NumberStyles.Any, null, out minute)
                && double.TryParse(args[2], NumberStyles.Any, null, out second)
                )
            {
                time = (hour + (minute + second / 60d) / 60d) / 24d;
            }
            return time.ToString("0.0########");
        }

        /// <summary>
        /// Returns a fraction of a day.
        /// </summary>
        /// <param name="argList">Time as a text string.</param>
        /// <returns>Fraction of a day.</returns>
        public string ComputeTimevalue(string argList)
        {
            double time;
            DateTime dt = DateTime.Now;
            argList = GetValueFromArg(argList);
            try
            {
                if (argList.IndexOf(TIC) > -1)
                {
                    argList = argList.Replace(TIC, "");
                    dt = DateTime.Parse(argList);
                    time = (dt.Hour + (dt.Minute + dt.Second / 60d) / 60d) / 24d;
                }
                else
                {

                    dt = DateTime.Parse(argList);
                    time = (dt.Hour + (dt.Minute + dt.Second / 60d) / 60d) / 24d;

                }
            }
            catch
            {
                return FormulaErrorStrings[this.invalid_arguments];
            }
            return time.ToString("0.0###########");
        }

        /// <summary>
        /// Day of the week.
        /// </summary>
        /// <param name="argList">Serial number date1 and return_type.</param>
        /// <returns>Days between the dates.</returns>
        public string ComputeWeekday(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 1 && argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double serialdate;
            double return_type = 1;

            double day = 0;

            if (double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out serialdate)
                && (argCount == 1 || double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out return_type))
                )
            {
                DateTime dt1 = GetDateFromSerialDate((int)serialdate);
                day = (int)dt1.DayOfWeek;
                if (return_type == 1)
                    day += 1;
                else
                {
                    if (day == 0)
                        day = 7;
                    if (return_type == 3)
                    {
                        day -= 1;
                    }
                }

            }

            return day.ToString();
        }

        /// <summary>
        /// Returns the year of the given date.
        /// </summary>
        /// <param name="argList">Given date.</param>
        /// <returns>Month.</returns>
        public string ComputeYear(string argList)
        {

            double year = 1;
            DateTime dt;
            double serialdate;
            string result = GetValueFromArg(argList);
            result = result.Replace("\"", "");
            if (double.TryParse(result, NumberStyles.Any, null, out serialdate))
            {
                //serialdate = serialdate - 1 - ((Treat1900AsLeapYear && serialdate > 59) ?  1 : 0);
                dt = GetDateFromSerialDate((int)serialdate);//dateTime1900.AddDays(serialdate);
                year = dt.Year;
            }
            else if (DateTime.TryParse(result, out dt))
                year = dt.Year;
            else
                return FormulaErrorStrings[invalid_arguments];


            return year.ToString();
        }




        /// <summary>
        /// Returns the average deviation of all values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the average deviation of all values listed in the argument.</returns>
        public string ComputeAvedev(string range)
        {
            double sum = 0;
            string s1;
            double d;
            List<double> x = new List<double>();
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;

                        }
                        if (s1.Length > 0)
                        {
                            if (double.TryParse(s1, NumberStyles.Any, null, out d))
                            {
                                sum = sum + d;
                                x.Add(d);
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Any, null, out d))
                        {
                            sum = sum + d;
                            x.Add(d);
                        }
                    }
                }
            }

            if (x.Count > 0)
            {
                double ave = sum / x.Count;
                sum = 0;
                for (int i = 0; i < x.Count; ++i)
                {
                    sum += Math.Abs(((double)x[i]) - ave);
                }
                sum = sum / x.Count;
            }
            return sum.ToString();
        }

        /// <summary>
        /// Returns the simple average of all values (including text) listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the simple average of all values listed in the argument.</returns>
        public string ComputeAveragea(string range)
        {
            double sum = 0;
            string s1;
            double d;
            int count = 0;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {

                        try
                        {
                            s1 = GetValueFromArg(s, true);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;

                        }
                        if (s1.Length > 0)
                        {
                            count++;
                            if (double.TryParse(s1, NumberStyles.Any, null, out d))
                            {
                                sum = sum + d;
                            }
                        }
                    }
                }
                else
                {

                    try
                    {
                        s1 = GetValueFromArg(r, true);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        count++;
                        if (double.TryParse(s1, NumberStyles.Any, null, out d))
                        {
                            sum = sum + d;
                        }
                    }
                }
            }

            if (count > 0)
            {
                sum = sum / count;
            }
            return sum.ToString();
        }

        // http://lib.stat.cmu.edu/griffiths-hill/acm291
        private static double[] gammaAs = new double[]{0.918938533204673d, 0.000595238095238d,
                                                          0.000793650793651d, 0.002777777777778d, 0.083333333333333d};
        private double gammaln(double x)
        {
            double y = x;
            double f = 0d;
            if (y < 7d)
            {
                f = y;
                y++;
                while (y < 7d)
                {
                    f = f * y;
                    if (y < 7d)
                        y++;
                }
                f = -Math.Log(f);
            }
            double z = 1d / (y * y);
            return f + (y - 0.5d) * Math.Log(y) - y + gammaAs[0]
                + (((-gammaAs[1] * z + gammaAs[2]) * z - gammaAs[3]) * z + gammaAs[4]) / y;
        }


        /// <summary>
        /// Returns the natural logarithm of the gamma function.
        /// </summary>
        /// <param name="argList">The value to be evaluated.</param>
        /// <returns>Returns the natural logarithm of the gamma function.</returns>
        public string ComputeGammaln(string argList)
        {
            double x = 0;
            if (double.TryParse(GetValueFromArg(argList), NumberStyles.Any, null, out x) && x > 0)
            {
                x = gammaln(x);
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return x.ToString();
        }

        private double gammainv(double p, double a, double b)
        {
            double guess = (a > b) ? b * Math.Abs(Math.Log(b * p)) : a * Math.Abs(Math.Log(a * p));
            if (a == b)
            {
                guess = p;
            }

            double lastIncrement = guess / 2;
            double gi = 0;
            double eps = 1e-7;
            int k = 100;
            int tries = 3;
            while (k == 100 && tries > 0)
            {
                tries--;
                guess = guess / 2;
                lastIncrement = guess / 2;

                for (k = 0; k < 100; ++k)
                {
                    gi = gammadist(a, b, guess);
                    if (Math.Abs((gi - p) / p) < eps)
                    {
                        break;
                    }
                    if (gi < p)
                        guess += lastIncrement;
                    else
                    {
                        lastIncrement = lastIncrement / 2;
                        if (guess - lastIncrement < 0)
                            lastIncrement = guess / 2;
                        guess -= lastIncrement;
                    }
                }
            }
            if (k == 100)
                guess = -1;

            return guess;
        }


        private double gammadist(double a, double b, double x)
        {

            int nPanels = 4;

            double h = x / nPanels;  //h = (b-a)/(3n)
            double mult = x / 12;
            double sum1 = gammadensity(a, b, x);
            double sum4 = 4 * (gammadensity(a, b, h) + gammadensity(a, b, 3 * h));
            double sum2 = 2 * gammadensity(a, b, 2 * h);
            double gd = mult * (sum1 + sum2 + sum4);
            double oldgd = gd;
            double eps = 1e-7;

            for (int k = 0; k < 30; ++k)
            {
                nPanels *= 2;

                sum2 += sum4 / 2;
                sum4 = 0;
                h = x / nPanels;
                for (int i = 0; i < nPanels; ++i)
                {
                    if (i % 2 == 1)
                    {
                        double d = gammadensity(a, b, h * i);
                        sum4 += d;
                    }
                }
                sum4 = 4 * sum4;
                mult /= 2;
                gd = mult * (sum1 + sum2 + sum4);
                //Console.WriteLine(gd);
                if (Math.Abs((gd - oldgd) / oldgd) < eps)
                    break;

                oldgd = gd;

            }
            return gd;
        }

        private double gammadensity(double a, double b, double x)
        {
            // excel version     dist = 1 / (Math.Pow(b, a) * gammaln(a)) * Math.Pow(x, a - 1) * Math.Exp(-x / b);
            // http://www.itl.nist.gov/div898/handbook/eda/section3/eda366b.htm
            return Math.Pow(x / b, a - 1) * Math.Exp(-x / b) / (b * Math.Exp(gammaln(a)));
        }

        /// <summary>
        /// Returns the gamma distribution.
        /// </summary>
        /// <param name="argList">X, alpha, beta, cumulative.</param>
        /// <returns>Returns the gamma distribution.</returns>
        /// <remarks>
        /// X, alpha and beta should be positive real numbers. Cumulative should be either 
        /// True if you want to return the value of the distribution function, or False
        /// if you want to return the value of the density function. The distribution value
        /// is computed interactively using Trapezoidal Rule to six to seven significant digits
        /// or 20 iteration maximum.
        /// </remarks>
        public string ComputeGammadist(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 4 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double a;
            double b;
            double cum = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out a)
                && double.TryParse(args[2], NumberStyles.Any, null, out b)
                )
            {
                if (argCount != 3)
                    cum = (args[3] == TRUEVALUESTR || args[3] == "1") ? 1 : 0;
                if (cum == 0)
                {
                    // excel version     dist = 1 / (Math.Pow(b, a) * gammaln(a)) * Math.Pow(x, a - 1) * Math.Exp(-x / b);
                    // http://www.itl.nist.gov/div898/handbook/eda/section3/eda366b.htm
                    dist = gammadensity(a, b, x);//Math.Pow(x/b, a-1) * Math.Exp(-x/b) / (b * Math.Exp(gammaln(a)));
                }
                else
                {
                    //Integrates the density function.
                    dist = gammadist(a, b, x);
                }
            }

            return dist.ToString();
        }


        /// <summary>
        /// Returns the inverse of gamma distribution.
        /// </summary>
        /// <param name="argList">P, alpha, beta</param>
        /// <returns>Returns x such that gamma distribution at x is p.</returns>
        /// <remarks>
        /// P, alpha and beta should be positive real numbers, with p between 0 and 1.
        /// </remarks>
        public string ComputeGammainv(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double p;
            double a;
            double b;
            double invdist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out p)
                && (p > 0 && p < 1)
                && double.TryParse(args[1], NumberStyles.Any, null, out a)
                && double.TryParse(args[2], NumberStyles.Any, null, out b)
                )
            {
                invdist = gammainv(p, a, b);
            }

            if (invdist <= 0)
                return FormulaErrorStrings[iterations_dont_converge];

            return invdist.ToString();
        }

        /// <summary>
        /// Returns the geometric mean of all values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>The geometric mean of all values listed in the argument.</returns>
        public string ComputeGeomean(string range)
        {
            double sum = 1d;
            string s1;
            double d;
            int count = 0;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {

                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;

                        }
                        if (s1.Length > 0)
                        {

                            if (double.TryParse(s1, NumberStyles.Any, null, out d))
                            {
                                count++;
                                sum = sum * d;
                            }
                        }
                    }
                }
                else
                {

                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {

                        if (double.TryParse(s1, NumberStyles.Any, null, out d))
                        {
                            count++;
                            sum = sum * d;
                        }
                    }
                }
            }

            if (count > 0)
            {
                sum = Math.Pow(sum, 1d / (double)count);
            }
            return sum.ToString();
        }

        /// <summary>
        /// Returns the harmonic mean of all values listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>The harmonic mean all values listed in the argument.</returns>
        public string ComputeHarmean(string range)
        {
            double sum = 0d;
            string s1;
            double d;
            int count = 0;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {

                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;

                        }
                        if (s1.Length > 0)
                        {

                            if (double.TryParse(s1, NumberStyles.Any, null, out d) && d != 0)
                            {
                                count++;
                                sum = sum + 1 / d;
                            }
                        }
                    }
                }
                else
                {

                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {

                        if (double.TryParse(s1, NumberStyles.Any, null, out d) && d != 0)
                        {
                            count++;
                            sum = sum + 1 / d;
                        }
                    }
                }
            }

            if (count > 0)
            {
                sum = ((double)count) / sum;
            }
            return sum.ToString();
        }

        //N things taken k at the time.
        private double comb(int k, int n)
        {
            double top = 1;
            for (int i = k + 1; i <= n; ++i)
                top = top * i;
            double bottom = 1;
            for (int i = 2; i <= (n - k); ++i)
                bottom = bottom * i;
            return top / bottom;
        }

        /// <summary>
        /// Returns the hypergeometric distribution.
        /// </summary>
        /// <param name="argList">Number of sample successes, number of sample, number of population successes, number of population.</param>
        /// <returns>Returns the gamma distribution.</returns>
        public string ComputeHypgeomdist(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 4)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double xnss = 0;
            double xns = 0;
            double xnps = 0;
            double xnp = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Integer, null, out xnss)
                && double.TryParse(args[1], NumberStyles.Integer, null, out xns)
                && double.TryParse(args[2], NumberStyles.Integer, null, out xnps)
                && double.TryParse(args[3], NumberStyles.Integer, null, out xnp)
                )
            {
                int nss = (int)xnss;
                int ns = (int)xns;
                int nps = (int)xnps;
                int np = (int)xnp;
                if (nss < 0 || ns < nss || nps < 0 || np < nps || nss > nps || ns > np)
                    return FormulaErrorStrings[invalid_Math_argument];

                dist = comb(nss, nps) * comb(ns - nss, np - nps) / comb(ns, np);
            }

            return dist.ToString();
        }


        /// <summary>
        /// Returns the y-intercept of the least square fit line through the given points.
        /// </summary>
        /// <param name="range">y_range, x_range.</param>
        /// <returns>y-intercept.</returns>
        public string ComputeIntercept(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double[] y = GetDoubleArray(args[0]);
            double[] x = GetDoubleArray(args[1]);
            int n = x.GetLength(0);

            if (n <= 0 || n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];
            double sumx = 0;
            double sumy = 0;
            for (int i = 0; i < n; ++i)
            {
                sumx += (double)x[i];
                sumy += (double)y[i];
            }
            sumx = sumx / n;
            sumy = sumy / n;

            double sumxy = 0;
            double sumx2 = 0;
            double d;
            for (int i = 0; i < n; ++i)
            {
                d = (double)x[i] - sumx;
                sumxy += d * (y[i] - sumy);
                sumx2 += d * d;
            }
            return (sumy - sumxy / sumx2 * sumx).ToString();
        }


        /// <summary>
        /// Returns the binomial distribution.
        /// </summary>
        /// <param name="argList">Number of successes, number of trials, probability, cumulative.</param>
        /// <returns>Returns the binomial distribution.</returns>
        public string ComputeBinomdist(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 4)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double successes;
            double trials;
            double p;
            double cum = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out successes)
                && double.TryParse(args[1], NumberStyles.Any, null, out trials)
                && double.TryParse(args[2], NumberStyles.Any, null, out p)
                )
            {
                cum = (args[3] == TRUEVALUESTR || args[3] == "1") ? 1 : 0;
                if (cum == 0)
                {
                    dist = comb((int)successes, (int)trials) * Math.Pow(p, successes) * Math.Pow(1 - p, trials - successes);
                }
                else
                {
                    dist = binomdist((int)trials, (int)successes, p); ;

                    //					for(int i = 0; i <= successes; ++ i)
                    //						dist += comb(i, (int)trials) * Math.Pow(p, i) * Math.Pow(1-p, trials - i);
                    //			
                }
            }

            return dist.ToString();
        }

        private double binomdist(int trials, int successes, double p)
        {
            double pPow = 1;
            double pm1 = 1 - p;

            double p1Pow = Math.Pow(pm1, trials);

            double dist = 0;
            double cbn = pPow * Math.Pow(pm1, trials);
            if (cbn == 0)
                return double.NaN;

            for (int i = 0; i <= successes; ++i)
            {
                dist += cbn;
                cbn = cbn * p / pm1 * (trials - i) / (i + 1);
                if (double.IsInfinity(cbn) || double.IsNaN(cbn))
                {
                    dist = double.NaN;
                    break;
                }
            }
            return dist;
        }

        /// <summary>
        /// Returns the chi-squared distribution.
        /// </summary>
        /// <param name="argList">X degrees of freedom.</param>
        /// <returns>Returns the chi-squared distribution.</returns>
        public string ComputeChidist(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double v;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out v)
                )
            {
                dist = 1 - chidist(x, v);
            }

            return dist.ToString();
        }


        private static int gauss_n = 16;
        private static double[] gauss_x = new double[]{0.04830766569,
                                                           0.14447196158,
                                                           0.23928736225,
                                                           0.33186860228,
                                                           0.42135127613,
                                                           0.50689990893,
                                                           0.58771575724,
                                                           0.66304426693,
                                                           0.73218211874,
                                                           0.79448379597,
                                                           0.84936761373,
                                                           0.89632115577,
                                                           0.93490607594,
                                                           0.96476225559,
                                                           0.98561151155,
                                                           0.99726386185,
        };
        private static double[] gauss_w = new double[]{0.09654008851,
                                                           0.09563872008,
                                                           0.09384439908,
                                                           0.09117387870,
                                                           0.08765209300,
                                                           0.08331192423,
                                                           0.07819389579,
                                                           0.07234579411,
                                                           0.06582222278,
                                                           0.05868409348,
                                                           0.05099805926,
                                                           0.04283589802,
                                                           0.03427386291,
                                                           0.02539206531,
                                                           0.01627439473,
                                                           0.00701861001,
        };



        //uses guass quad to estimate the integral
        private double chidist(double x, double v)
        {
            double ex = v / 2;
            double mult = (1 / (Math.Pow(2, ex) * Math.Exp(this.gammaln(ex))));
            ex = ex - 1;

            double a = 0;
            double b = x;

            double midPt = (a + b) / 2;
            double radius = (b - a) / 2;

            double sum = 0;

            double offset;
            for (int i = 0; i < gauss_n; ++i)
            {
                offset = gauss_x[i] * radius;
                sum = sum + gauss_w[i] * (Math.Pow(midPt + offset, ex) * Math.Exp(-(midPt + offset) / 2)
                    + Math.Pow(midPt - offset, ex) * Math.Exp(-(midPt - offset) / 2));
            }
            sum = mult * sum * radius;
            return sum;
        }

        private double chiinv(double p, double v)
        {
            double guess = p;//Math.Sqrt(p);

            double lastIncrement = guess / 2;
            double gi = 0;
            double eps = 1e-7;
            int k = 100;
            int tries = 3;
            while (k == 100 && tries > 0)
            {
                tries--;
                guess = guess / 2;
                lastIncrement = guess / 2;

                for (k = 0; k < 100; ++k)
                {
                    gi = 1 - chidist(guess, v);
                    if (Math.Abs((gi - p) / p) < eps)
                    {
                        break;
                    }
                    if (gi > p)
                        guess += lastIncrement;
                    else
                    {
                        lastIncrement = lastIncrement / 2;
                        if (guess - lastIncrement < 0)
                            lastIncrement = guess / 2;
                        guess -= lastIncrement;
                    }
                }
            }
            if (k == 100)
                guess = -1;

            return guess;
        }

        /// <summary>
        /// Returns the inverse of the chi-squared distribution.
        /// </summary>
        /// <param name="argList">X degrees of freedom.</param>
        /// <returns>Returns the inverse of the chi-squared distribution.</returns>
        public string ComputeChiinv(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double p;
            double v;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out p)
                && double.TryParse(args[1], NumberStyles.Any, null, out v)
                )
            {
                dist = chiinv(p, v);
            }

            return dist.ToString();
        }

        /// <summary>
        /// Returns the Chi Test for independence.
        /// </summary>
        /// <param name="range">Actual_range, expected_range.</param>
        /// <returns>y-intercept.</returns>
        public string ComputeChitest(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            string r = args[0];
            string s1;
            double d = 0;
            double v = 0;
            if (r.IndexOf(':') > -1)
            {
                s1 = r.Substring(0, r.IndexOf(':'));
                int col = this.ColIndex(s1);
                int row = this.RowIndex(s1);

                s1 = r.Substring(r.IndexOf(':') + 1);
                int col1 = this.ColIndex(s1);
                int row1 = this.RowIndex(s1);
                v = Math.Abs(col - col1) * Math.Abs(row - row1);
                if (v < 1)
                    return FormulaErrorStrings[invalid_Math_argument];
            }
            double[] y = GetDoubleArray(args[0]);
            double[] x = GetDoubleArray(args[1]);
            int n = x.GetLength(0);

            if (n <= 0 || n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];
            double sumx = 0;

            for (int i = 0; i < n; ++i)
            {
                d = y[i] - x[i];
                sumx += d * d / x[i];
            }

            return (1 - this.chidist(sumx, v)).ToString();
        }

        private double normaldensity(double x, double u, double s)
        {
            return 1 / (Math.Sqrt(2 * Math.PI) * s) * Math.Exp(-(x - u) * (x - u) / (2 * s * s));
        }

        private double normaldist(double x, double u, double s)
        {
            ////						int nTraps = 4;
            ////						
            ////						
            ////						
            ////						double h = x / 2;
            ////						double sum = normaldensity(-x , u, s) + 2*(normaldensity(-x + h , u, s) + normaldensity(-x + 2 * h , u, s) + normaldensity(-x + 3 * h , u, s)) + normaldensity(x , u, s);
            ////						double gd = h * sum / 2;
            ////						double oldgd = gd;
            ////						double eps = 1e-7;
            ////			
            ////						for(int k = 0; k < 30; ++k)
            ////						{
            ////							nTraps *= 2;
            ////			
            ////							h = 2 * x / nTraps;
            ////							for(int i = 0; i < nTraps; ++i)
            ////							{
            ////								if(i % 2 == 1)
            ////								{
            ////									double d = normaldensity(-x + h * i, u, s);
            ////									sum += 2 * d;
            ////								}
            ////							}
            ////							gd = h * sum / 2;
            ////							Console.WriteLine(gd);
            ////							if(Math.Abs((gd - oldgd)/oldgd) < eps)
            ////								break;
            ////							
            ////							oldgd = gd;
            ////							
            ////						}
            ////			    return gd;

            int nPanels = 32;


            double leftside;
            double rightside;
            if (x > u)
            {
                leftside = u - (x - u);
                rightside = x;
            }
            else
            {
                leftside = x;
                rightside = u + (u - x);
            }

            double h = (rightside - leftside) / nPanels;
            double mult = h / 3;

            double sum1 = normaldensity(leftside, u, s) + normaldensity(rightside, u, s);
            double sum4 = 0;
            for (int i = 1; i < nPanels; i += 2)
                sum4 += 4 * normaldensity(leftside + i * h, u, s);
            double sum2 = 0;
            for (int i = 2; i < nPanels; i += 2)
                sum2 += 2 * normaldensity(leftside + i * h, u, s);
            double gd = mult * (sum1 + sum2 + sum4);
            double oldgd = gd;
            double eps = 1e-7;

            int k = 0;

            for (k = 0; k < 20; ++k)
            {
                nPanels *= 2;

                sum2 += sum4 / 2;
                sum4 = 0;
                h = h = (rightside - leftside) / nPanels;
                for (int i = 0; i < nPanels; ++i)
                {
                    if (i % 2 == 1)
                    {
                        double d = normaldensity(leftside + h * i, u, s);
                        sum4 += d;
                    }
                }
                sum4 = 4 * sum4;
                mult = h / 3;
                gd = mult * (sum1 + sum2 + sum4);

                if (Math.Abs((gd - oldgd) / oldgd) < eps)
                    break;

                oldgd = gd;

            }
            if (x > u)
            {
                //Add the left tails.
                gd = gd + (1 - gd) / 2;
            }
            else
            {
                //return only the left tial
                gd = (1 - gd) / 2;
            }
            //Console.WriteLine(k.ToString() + "  " + gd.ToString());
            return gd;
        }

        /// <summary>
        /// Returns the normal distribution.
        /// </summary>
        /// <param name="argList">X, mean, standarddev, cumulative.</param>
        /// <returns>Returns the normal distribution.</returns>
        /// <remarks>
        /// Cumulative should be either 
        /// True if you want to return the value of the distribution function or False
        /// if you want to return the value of the density function. The distribution value
        /// is computed interactively using Trapezoidal Rule to six to seven significant digits
        /// or 20 iteration maximum.
        /// </remarks>
        public string ComputeNormdist(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 4 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double u;
            double s;
            double cum = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out u)
                && double.TryParse(args[2], NumberStyles.Any, null, out s)
                )
            {
                if (argCount != 3)
                    cum = (args[3] == TRUEVALUESTR || args[3] == "1") ? 1 : 0;
                if (cum == 0)
                {
                    dist = normaldensity(x, u, s);
                }
                else
                {
                    dist = normaldist(x, u, s);
                }
            }

            return dist.ToString();
        }

        /// <summary>
        /// Returns the standard normal cumulative distribution function. The distribution has a mean of 0 (zero) and a standard deviation of one.
        /// <para>Syntax: NORMSDIST(z)</para>
        /// </summary>
        /// <param name="argList">Z is the value for which you want the distribution.</param>
        /// <returns>Returns string standard normal cumulative distribution function.</returns>
        public string ComputeNormsDist(string argList)
        {
            string args = argList + ", 0, 1, " + TRUEVALUESTR;
            return ComputeNormdist(args);
        }

        /// <summary>
        /// Returns the inverse of normal distribution.
        /// </summary>
        /// <param name="argList">P, mean, standard deviation.</param>
        /// <returns>Returns x such that normal distribution at x is p.</returns>
        /// <remarks>
        /// P should be between 0 and 1.
        /// </remarks>
        public string ComputeNorminv(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double p;
            double u;
            double s;
            double invdist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out p)
                && (p > 0 && p < 1)
                && double.TryParse(args[1], NumberStyles.Any, null, out u)
                && double.TryParse(args[2], NumberStyles.Any, null, out s)
                )
            {
                invdist = normalinv(p, u, s);
            }

            if (invdist <= 0)
                return FormulaErrorStrings[iterations_dont_converge];

            return invdist.ToString();
        }

        private double normalinv(double p, double u, double s)
        {
            double guess = u;//(u == 0) ? 1 : u;
            if (p < .05)
                guess = u - 2 * s;
            else if (p < .5)
                guess = u;
            else if (p < .95)
                guess = u + 2 * s;
            else
                guess = u + 5 * s;

            double lastIncrement = guess / 2;
            double gi = 0;
            double eps = 1e-7;
            int k = 100;
            int tries = 3;
            int its = 0;
            while (k == 100 && tries > 0)
            {
                tries--;
                guess = guess / 2;
                lastIncrement = guess / 2;

                for (k = 0; k < 100; ++k)
                {
                    its++;
                    gi = normaldist(guess, u, s);
                    if (Math.Abs((gi - p) / p) < eps)
                    {
                        break;
                    }
                    if (gi < p)
                        guess += lastIncrement;
                    else
                    {
                        lastIncrement = lastIncrement / 2;
                        if (guess - lastIncrement < 0)
                            lastIncrement = guess / 2;
                        guess -= lastIncrement;
                    }
                }
            }
            if (k == 100)
                guess = -1;

            //Console.WriteLine(its);
            return guess;
        }

        /// <summary>
        /// Returns the inverse of the standard normal cumulative distribution. The distribution has a mean of zero and a standard deviation of one.
        /// <para>Syntax: NORMSINV(p)</para>
        /// </summary>
        /// <param name="argList">p is a probability corresponding to the normal distribution.</param>
        /// <remarks>
        /// p should be between 0 and 1.
        /// </remarks>        
        public string ComputeNormsInv(string argList)
        {
            string args = argList + ", 0, 1";
            return ComputeNorminv(args);
        }

        /// <summary>
        /// Returns a confidence interval radius.
        /// </summary>
        /// <param name="argList">Alpha, standard deviation, size.</param>
        /// <returns>Returns x such that normal distribution at x is p.</returns>
        /// <remarks>
        /// P should be between 0 and 1.
        /// </remarks>
        public string ComputeConfidence(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double alpha;
            double s;
            double sz;
            double val = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out alpha)
                && double.TryParse(args[1], NumberStyles.Any, null, out s)
                && double.TryParse(args[2], NumberStyles.Any, null, out sz)
                )
            {
                // 1 - alpha  would be from -inf to 1-alpha. adding alpha + 2 
                // takes away the left tail making the integral from alpha-1 to 1-alpha.
                val = normalinv(1 - alpha + alpha / 2, 0, 1);
                //val = normalinv(1 - alpha, 0, 1, false);
                val = val * s / Math.Sqrt(sz);
            }

            return val.ToString();
        }

        /// <summary>
        /// Returns the correlation coefficient between the two sets of points.
        /// </summary>
        /// <param name="range">Range1, range2.</param>
        /// <returns>Correlation coefficient.</returns>
        public string ComputeCorrel(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double[] y = GetDoubleArray(args[0]);
            double[] x = GetDoubleArray(args[1]);
            int n = x.GetLength(0);
            if (n <= 0 || n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];

            double sumx = 0;
            double sumy = 0;
            for (int i = 0; i < n; ++i)
            {
                sumx += x[i];
                sumy += y[i];
            }
            sumx = sumx / n;
            sumy = sumy / n;

            double sumxy = 0;
            double sumxb2 = 0;
            double sumyb2 = 0;
            double xb = 0;
            double yb = 0;

            for (int i = 0; i < n; ++i)
            {
                xb = x[i] - sumx;
                yb = y[i] - sumy;
                sumxy += xb * yb;
                sumxb2 += xb * xb;
                sumyb2 += yb * yb;
            }
            return (sumxy / Math.Sqrt(sumxb2 * sumyb2)).ToString();
        }

        /// <summary>
        /// Returns the count of all values (including text) listed in the argument
        /// evaluate to a number.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the count of all numerical values listed in the argument.</returns>
        public string ComputeCount(string range)
        {
            int count = 0;
            string s1;
            double d;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {

                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;

                        }
                        if (s1.Length > 0)
                        {

                            if (double.TryParse(s1, NumberStyles.Any, null, out d))
                            {
                                count++;
                            }
                        }
                    }
                }
                else
                {

                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Any, null, out d))
                        {
                            count++;
                        }
                    }
                }
            }
            return count.ToString();
        }


        /// <summary>
        /// Returns the count of all values (including text) listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the count of all values listed in the argument.</returns>
        public string ComputeCounta(string range)
        {
            int count = 0;
            string s1;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {

                        try
                        {
                            s1 = GetValueFromArg(s, true);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;

                        }
                        if (s1.Length > 0)
                            count++;
                    }
                }
                else
                {

                    try
                    {
                        s1 = GetValueFromArg(r, true);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                        count++;

                }
            }
            return count.ToString();
        }

        /// <summary>
        /// Returns the count of blank cells listed in the argument.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the count of blank cells listed in the argument.</returns>
        public string ComputeCountblank(string range)
        {
            int count = 0;
            string s1;
            string TICS2 = TIC + TIC;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {

                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;

                        }
                        if (s1.Length == 0 || s1 == TICS2)
                            count++;
                    }
                }
                else
                {

                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length == 0 || s1 == TICS2)
                        count++;

                }
            }
            return count.ToString();
        }


        /// <summary>
        /// Counts the cells specified by some criteria.
        /// </summary>
        /// <param name="argList">The criteria range, the criteria.</param>
        /// <returns>Returns string to the cell count</returns>
        public string ComputeCountif(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            string criteriaRange = args[0];
            //string criteria = args[1].Replace(TIC, "");
            string criteria = args[1];
            if (criteria.Length > 1 && criteria[0] == TIC[0]
                && "=><".IndexOf(criteria[1]) == -1)
                criteria = "=" + criteria;
            else
                criteria = criteria.Replace(TIC, "");


            string[] s1 = this.GetCellsFromArgs(criteriaRange);


            int sum = 0;
            int count = s1.GetLength(0);
            double[] vector = new double[count];
            string s;

            for (int index = 0; index < count; ++index)
            {
                s = s1[index] + criteria;
                //s = this.ParseSimple(s);
                s = this.Parse(s);
                s = this.ComputedValue(s);
                //if(double.TryParse(s , NumberStyles.Any, null, out d) && d > 0)
                if (s == this.TRUEVALUESTR)
                {
                    sum++;
                }
            }


            return sum.ToString();
        }

        /// <summary>
        /// Returns the covariance between the two sets of points.
        /// </summary>
        /// <param name="range">Range1, range2.</param>
        /// <returns>Covariance.</returns>
        public string ComputeCovar(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double[] y = GetDoubleArray(args[0]);
            double[] x = GetDoubleArray(args[1]);
            int n = x.GetLength(0);

            if (n <= 0 || n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];

            double sumx = 0;
            double sumy = 0;
            for (int i = 0; i < n; ++i)
            {
                sumx += x[i];
                sumy += y[i];
            }
            sumx = sumx / n;
            sumy = sumy / n;

            double sumxy = 0;

            for (int i = 0; i < n; ++i)
            {
                sumxy += (x[i] - sumx) * (y[i] - sumy);
            }
            return (sumxy / n).ToString();
        }

        /// <summary>
        /// Returns the smallest value for which the cumulative binomial distribution is greater than or equal to a criterion value.
        /// </summary>
        /// <param name="argList">Number of trials, probability, alpha.</param>
        /// <returns>Returns the critcal value.</returns>
        public string ComputeCritbinom(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double trials;
            double p;
            double alpha = 0;
            int dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out trials)
                && double.TryParse(args[1], NumberStyles.Any, null, out p)
                && double.TryParse(args[2], NumberStyles.Any, null, out alpha)
                )
            {
                if (p > 0 && p < 1 && alpha > 0 && alpha < 1)
                {
                    dist = critbinom((int)trials, p, alpha);
                    if (dist == int.MaxValue)
                    {
                        return FormulaErrorStrings[this.calculation_overflow];
                    }
                }
                else
                    return FormulaErrorStrings[this.invalid_arguments];
            }

            return dist.ToString();
        }

        private int critbinom(int nTrials, double p, double alpha)
        {
            int checkval = nTrials;
            int half = nTrials;
            double dist = 1;
            double dist1 = 1;
            do
            {
                half = half / 2 + 1;
                if (dist >= alpha)
                {
                    dist1 = binomdist(nTrials, checkval - 1, p);
                    if (double.IsNaN(dist1))
                        return int.MaxValue;
                    if (dist1 < alpha && dist1 > 0)
                    {
                        break;
                    }
                    checkval = checkval - half;
                }
                else
                {
                    dist1 = binomdist(nTrials, checkval + 1, p);
                    if (dist1 >= alpha)
                    {
                        checkval = checkval + 1;
                        break;
                    }
                    checkval = checkval + half;
                }
                dist = binomdist(nTrials, checkval, p);

            }
            while (checkval < nTrials && checkval > 0);

            return checkval;

        }

        /// <summary>
        /// Returns the sum of the squares of the mean deviations.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>Sum of the squares of the mean deviation.</returns>
        public string ComputeDevsq(string range)
        {

            double d;

            double[] x = GetDoubleArray(range);
            int n = x.GetLength(0);
            if (n <= 0)
                return FormulaErrorStrings[wrong_number_arguments];
            double sumx = 0;
            for (int i = 0; i < n; ++i)
            {
                sumx += x[i];
            }
            sumx = sumx / n;
            double sumx2 = 0;

            for (int i = 0; i < n; ++i)
            {
                d = x[i] - sumx;
                sumx2 += d * d;
            }
            return sumx2.ToString();
        }

        /// <summary>
        /// Returns the exponential distribution.
        /// </summary>
        /// <param name="argList">X, lambda, cumulative.</param>
        /// <returns>Returns the exponential distribution.</returns>
        /// <remarks>
        /// Cumulative should be either 
        /// True if you want to return the value of the distribution function or False
        /// if you want to return the value of the density function. 
        /// </remarks>
        public string ComputeExpondist(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double lambda;
            double cum = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out lambda)
                )
            {
                cum = (args[2] == "1" || args[2] == TRUEVALUESTR) ? 1 : 0;
                if (cum == 0)
                {
                    dist = lambda * Math.Exp(-lambda * x);
                }
                else
                {
                    dist = 1 - Math.Exp(-lambda * x);
                }
            }

            return dist.ToString();
        }

        /// <summary>
        /// Returns the F (Fisher) probability distribution.
        /// </summary>
        /// <param name="argList">X, degreesfreedom1, degreesfreedom2.</param>
        /// <returns>Returns the F probability distribution.</returns>
        public string ComputeFdist(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double df1 = 0;
            double df2 = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out df1)
                && double.TryParse(args[2], NumberStyles.Any, null, out df2)
                )
            {
                double mult = Math.Exp
                    (
                    gammaln((df1 + df2) / 2) -
                    gammaln(df1 / 2) -
                    gammaln(df2 / 2) +
                    (df1 / 2) * Math.Log(df1 / df2)
                    );

                dist = 1 - mult * fdist(x, (int)df1, (int)df2);
            }

            return dist.ToString();
        }

        private double fdensity(double x, int df1, int df2)
        {
            return Math.Pow(x, (df1 - 2) / 2d) / Math.Pow(1 + df1 * x / df2, (df1 + df2) / 2d);
        }

        private double fdist(double x, int df1, int df2)
        {
            int nPanels = 32;

            double leftside = 0;
            double rightside = x;

            double h = (rightside - leftside) / nPanels;
            double mult = h / 3;

            double sum1 = fdensity(leftside, df1, df2) + fdensity(rightside, df1, df2);
            double sum4 = 0;
            for (int i = 1; i < nPanels; i += 2)
                sum4 += 4 * fdensity(leftside + i * h, df1, df2);
            double sum2 = 0;
            for (int i = 2; i < nPanels; i += 2)
                sum2 += 2 * fdensity(leftside + i * h, df1, df2);
            double gd = mult * (sum1 + sum2 + sum4);
            double oldgd = gd;
            double eps = 1e-7;

            int k = 0;

            for (k = 0; k < 20; ++k)
            {
                nPanels *= 2;

                sum2 += sum4 / 2;
                sum4 = 0;
                h = (rightside - leftside) / nPanels;
                for (int i = 0; i < nPanels; ++i)
                {
                    if (i % 2 == 1)
                    {
                        double d = fdensity(leftside + h * i, df1, df2);
                        sum4 += d;
                    }
                }
                sum4 = 4 * sum4;
                mult = h / 3;
                gd = mult * (sum1 + sum2 + sum4);

                if (Math.Abs((gd - oldgd) / oldgd) < eps)
                    break;

                oldgd = gd;

            }
            //Console.WriteLine(k.ToString() + "  " + gd.ToString());
            return gd;
        }

        private double finv(double p, int df1, int df2)
        {
            double mult = Math.Exp
                (
                gammaln((df1 + df2) / 2d) -
                gammaln(df1 / 2d) -
                gammaln(df2 / 2d) +
                (df1 / 2d) * Math.Log((double)df1 / df2)
                );
            double guess = mult;
            //			if( p < .05)
            //				guess = u - 2 * s;
            //			else if( p < .5)
            //				guess = u;
            //			else if( p < .95)
            //				guess = u + 2 * s;
            //			else
            //				guess = u + 5 * s;

            double lastIncrement = guess / 2;
            double gi = 0;
            double eps = 1e-7;
            int k = 100;
            int tries = 3;
            int its = 0;
            while (k == 100 && tries > 0)
            {
                tries--;
                guess = guess / 2;
                lastIncrement = guess / 2;

                for (k = 0; k < 100; ++k)
                {
                    its++;
                    gi = 1 - mult * fdist(guess, df1, df2);
                    //Console.WriteLine("{0}: guess {1}  computed {2}  target {3}", its, guess, gi, p);
                    if (Math.Abs((gi - p) / p) < eps)
                    {
                        break;
                    }
                    if (gi > p) //greater since looking for 1 - xxxx
                        guess += lastIncrement;
                    else
                    {
                        lastIncrement = lastIncrement / 2;
                        if (guess - lastIncrement < 0)
                            lastIncrement = guess / 2;
                        guess -= lastIncrement;
                    }
                }
            }
            if (k == 100)
                guess = -1;

            //Console.WriteLine(its);
            return guess;
        }

        /// <summary>
        /// Returns the inverse of F distribution.
        /// </summary>
        /// <param name="argList">P, degreesfreedom1, degreesfreedom2.</param>
        /// <returns>Returns x such that F distribution at x is p.</returns>
        /// <remarks>
        /// P should be between 0 and 1.
        /// </remarks>
        public string ComputeFinv(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double p;
            double df1;
            double df2;
            double invdist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out p)
                && (p > 0 && p < 1)
                && double.TryParse(args[1], NumberStyles.Any, null, out df1)
                && double.TryParse(args[2], NumberStyles.Any, null, out df2)
                )
            {
                invdist = finv(p, (int)df1, (int)df2);
            }

            if (invdist <= 0)
                return FormulaErrorStrings[iterations_dont_converge];

            return invdist.ToString();
        }

        /// <summary>
        /// Returns the Fisher transformation of the input variable.
        /// </summary>
        /// <param name="argList">Input variable x.</param>
        /// <returns>Fisher transformation of x.</returns>
        /// <remarks>
        /// X should be between -1 and 1.
        /// </remarks>
        public string ComputeFisher(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 1)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double z = 0;

            string s = GetValueFromArg(args[0]);


            if (double.TryParse(s, NumberStyles.Any, null, out x)
                && (x > -1 && x < 1)
                )
            {
                z = 0.5 * Math.Log((1 + x) / (1 - x));
            }

            return z.ToString();
        }

        /// <summary>
        /// Returns the inverse of Fisher transformation.
        /// </summary>
        /// <param name="argList">Input variable y.</param>
        /// <returns>The value x such that the Fisher transformation y is x.</returns>

        public string ComputeFisherinv(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 1)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double y;
            double x = 0;

            string s = GetValueFromArg(args[0]);


            if (double.TryParse(s, NumberStyles.Any, null, out y))
            {
                double d = Math.Exp(2 * y);
                x = (d - 1) / (d + 1);
            }

            return x.ToString();
        }

        /// <summary>
        /// Returns a forecasted value based on two sets of points using Least Square Fit regression.
        /// </summary>
        /// <param name="range">X, rangex, rangey.</param>
        /// <returns>Forecasted.</returns>
        public string ComputeForecast(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double x0, d;
            string s1 = GetValueFromArg(args[0]);
            if (!double.TryParse(s1, NumberStyles.Any, null, out x0))
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            double[] y = GetDoubleArray(args[1]);
            double[] x = GetDoubleArray(args[2]);
            int n = x.GetLength(0);

            if (n <= 0 || n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];
            double sumx = 0;
            double sumy = 0;
            for (int i = 0; i < n; ++i)
            {
                sumx += x[i];
                sumy += y[i];
            }
            sumx = sumx / n;
            sumy = sumy / n;

            double sumxy = 0;
            double sumx2 = 0;
            for (int i = 0; i < n; ++i)
            {
                d = x[i] - sumx;
                sumxy += d * (y[i] - sumy);
                sumx2 += d * d;
            }

            double b = sumxy / sumx2;
            double a = sumy - b * sumx;
            return (a + b * x0).ToString();
        }

        ////////		/// <summary>
        ////////		/// Returns the beta distribution.
        ////////		/// </summary>
        ////////		/// <param name="args">X, alpha, beta, a, b.</param>
        ////////		/// <returns>Returns the beta distribution.</returns>
        ////////		/// <remarks>
        ////////		/// If a and b are missing, a = 0 and b = 1. 
        ////////		/// </remarks>
        ////////		public string ComputeBetadist(string argList)
        ////////		{
        ////////			string[] args = argList.Split(new char[]{ParseArgumentSeparator});
        ////////			int argCount = args.GetLength(0);
        ////////			if(argCount != 3 && argCount != 4 && argCount != 5)
        ////////			{
        ////////				return FormulaErrorStrings[wrong_number_arguments];
        ////////			}
        ////////			double x;
        ////////			double alpha = 0;
        ////////			double beta = 0;
        ////////			double a = 0;
        ////////			double b = 1;
        ////////			double dist = 0;
        ////////			
        ////////			for(int i = 0; i < argCount; ++i)
        ////////			{
        ////////				args[i] = GetValueFromArg(args[i]);
        ////////			}
        ////////
        ////////			if(double.TryParse(args[0], NumberStyles.Any, null, out x)
        ////////				&& double.TryParse(args[1], NumberStyles.Any, null, out alpha)
        ////////				&& double.TryParse(args[2], NumberStyles.Any, null, out beta)
        ////////				&& (argCount < 4 || double.TryParse(args[3], NumberStyles.Any, null, out a))
        ////////				&& (argCount < 5 || double.TryParse(args[4], NumberStyles.Any, null, out b))
        ////////				)
        ////////			{
        ////////				dist = betadist(x, alpha, beta, a ,b);
        ////////			}
        ////////
        ////////			return dist.ToString();
        ////////		}
        ////////
        ////////		private double betadist(double x, double alpha, double beta, double a, double b)
        ////////		{
        ////////			double bmult = 1 / (betac(alpha, beta) * Math.Pow(b - a, alpha + beta - 1));
        ////////
        ////////			int nPanels = 32;
        ////////	
        ////////			double leftside = a;
        ////////			double rightside = x;
        ////////				
        ////////			double h = (rightside - leftside) / nPanels;  
        ////////			double mult = bmult * h / 3;
        ////////			
        ////////			double sum1 = betadensity(leftside, alpha, beta, a, b) + betadensity(rightside, alpha, beta, a, b);
        ////////			double sum4 =  0;
        ////////			for(int i = 1; i < nPanels; i += 2)
        ////////				sum4 += 4 * betadensity(leftside + i * h, alpha, beta, a, b);
        ////////			double sum2 = 0;
        ////////			for(int i = 2; i < nPanels; i += 2)
        ////////				sum2 += 2 * betadensity(leftside + i * h, alpha, beta, a, b);
        ////////			double gd = mult * (sum1 + sum2 + sum4);
        ////////			double oldgd = gd;
        ////////			double eps = 1e-7;
        ////////
        ////////			int k = 0;
        ////////
        ////////			for(k = 0; k < 20; ++k)
        ////////			{
        ////////				nPanels *= 2;
        ////////
        ////////				sum2 += sum4 / 2;
        ////////				sum4 = 0;
        ////////				h = (rightside - leftside) / nPanels;
        ////////				for(int i = 0; i < nPanels; ++i)
        ////////				{
        ////////					if(i % 2 == 1)
        ////////					{
        ////////						double d = betadensity(leftside + h * i, alpha, beta, a, b);
        ////////						sum4 += d;
        ////////					}
        ////////				}
        ////////				sum4 = 4 * sum4;
        ////////				mult = bmult * h / 3;
        ////////				gd = mult * (sum1 + sum2 + sum4);
        ////////				
        ////////				if(Math.Abs((gd - oldgd)/oldgd) < eps)
        ////////					break;
        ////////				
        ////////				oldgd = gd;
        ////////				
        ////////			}
        ////////			//Console.WriteLine(k.ToString() + "  " + gd.ToString());
        ////////			return gd;
        ////////		
        ////////		}
        ////////
        ////////		private double betadensity(double x, double alpha, double beta, double a, double b)
        ////////		{
        ////////			double d = Math.Pow(x-a, alpha-1) * Math.Pow(b-x, beta-1);
        ////////			return d;
        ////////		}
        ////////
        ////////		private double betac(double p, double q)
        ////////		{
        ////////			int nPanels = 8;
        ////////	
        ////////			double leftside = 0;
        ////////			double rightside = 1;
        ////////				
        ////////			double h = (rightside - leftside) / nPanels;  
        ////////			double mult = h / 3;
        ////////			
        ////////			double sum1 = betaf(leftside , p, q) + betaf(rightside, p, q);
        ////////			double sum4 =  0;
        ////////			for(int i = 1; i < nPanels; i += 2)
        ////////				sum4 += 4 * betaf(leftside + i * h , p, q);
        ////////			double sum2 = 0;
        ////////			for(int i = 2; i < nPanels; i += 2)
        ////////				sum2 += 2 * betaf(leftside + i * h , p, q);
        ////////			double gd = mult * (sum1 + sum2 + sum4);
        ////////			double oldgd = gd;
        ////////			double eps = 1e-7;
        ////////
        ////////			int k = 0;
        ////////
        ////////			for(k = 0; k < 20; ++k)
        ////////			{
        ////////				nPanels *= 2;
        ////////
        ////////				sum2 += sum4 / 2;
        ////////				sum4 = 0;
        ////////				h = h = (rightside - leftside) / nPanels;
        ////////				for(int i = 0; i < nPanels; ++i)
        ////////				{
        ////////					if(i % 2 == 1)
        ////////					{
        ////////						double d = betaf(leftside + h * i, p, q);
        ////////						sum4 += d;
        ////////					}
        ////////				}
        ////////				sum4 = 4 * sum4;
        ////////				mult = h / 3;
        ////////				gd = mult * (sum1 + sum2 + sum4);
        ////////				// Console.WriteLine(k.ToString() + " c " + gd.ToString());
        ////////				if(Math.Abs((gd - oldgd)/oldgd) < eps)
        ////////					break;
        ////////				
        ////////				oldgd = gd;
        ////////				
        ////////			}
        ////////			//Console.WriteLine(k.ToString() + " c " + gd.ToString());
        ////////			return gd;
        ////////		}
        ////////
        ////////		private double betain(double x, double p, double q)
        ////////		{
        ////////			double betaln = this.gammaln(p) + gammaln(q) - gammaln(p+q);
        ////////			double retVal = x;
        ////////
        ////////			bool flip = false;
        ////////			double psq = p + q;
        ////////			double cx = 1 - x;
        ////////			double xx, qq, pp;
        ////////			if( p < psq * x)
        ////////			{
        ////////				xx = cx;
        ////////				cx = x;
        ////////				pp = q;
        ////////				qq = p;
        ////////				flip = true;
        ////////			}
        ////////			else
        ////////			{
        ////////				xx = x;
        ////////				pp = p;
        ////////				qq = q;
        ////////				flip = false;
        ////////			}
        ////////
        ////////			double term = 1;
        ////////			double ai = 1;
        ////////				retVal = 1;
        ////////			double ns = qq + cx * psq;
        ////////			double rx = xx / cx;
        ////////	three:		double temp = qq - ai;
        ////////			if(ns == 0)
        ////////				rx = xx;
        ////////	four:		term = term * temp * rx / (pp+ai);
        ////////			retVal += term;
        ////////			temp = Math.Abs(term);
        ////////			if(temp <= 1e-7 && temp <= 1e-7 * retVal) goto five;
        ////////			ai += 1;
        ////////			ns -= 1;
        ////////			if(ns >= 0) goto three;
        ////////			temp = psq;
        ////////			psq += 1;
        ////////			goto four;
        ////////			five:
        ////////				retVal = retVal * Math.Exp(pp * Math.Log(xx) + qq-1) * Math.Log(cx) - betaln /pp;
        ////////
        ////////			if(flip)
        ////////				retVal = 1 - retVal;
        ////////			
        ////////			return retVal;
        ////////
        ////////		}
        ////////
        ////////		private double betaf(double x, double p, double q)
        ////////		{
        ////////			return Math.Pow(x, p-1) * Math.Pow((1 - x), q-1);
        ////////		}
        ////////
        ////////		/// <summary>
        ////////		/// Returns the inverse of beta distribution.
        ////////		/// </summary>
        ////////		/// <param name="argList">P, alpha, beta, a, b.</param>
        ////////		/// <returns>Returns x such that normal distribution at x is p.</returns>
        ////////		/// <remarks>
        ////////		/// P should be between 0 and 1.
        ////////		/// </remarks>
        ////////		public string ComputeBetainv(string argList)
        ////////		{
        ////////			string[] args = argList.Split(new char[]{ParseArgumentSeparator});
        ////////			int argCount = args.GetLength(0);
        ////////			if(argCount != 3 && argCount != 4 && argCount != 5)
        ////////			{
        ////////				return FormulaErrorStrings[wrong_number_arguments];
        ////////			}
        ////////			double p;
        ////////			double alpha = 0;
        ////////			double beta = 0;
        ////////			double a = 0;
        ////////			double b = 1;
        ////////			double invdist = 0;
        ////////			
        ////////			for(int i = 0; i < argCount; ++i)
        ////////			{
        ////////				args[i] = GetValueFromArg(args[i]);
        ////////			}
        ////////
        ////////			if(double.TryParse(args[0], NumberStyles.Any, null, out p)
        ////////				&& double.TryParse(args[1], NumberStyles.Any, null, out alpha)
        ////////				&& double.TryParse(args[2], NumberStyles.Any, null, out beta)
        ////////				&& (argCount < 4 || double.TryParse(args[3], NumberStyles.Any, null, out a))
        ////////				&& (argCount < 5 || double.TryParse(args[4], NumberStyles.Any, null, out b))
        ////////				)
        ////////			{
        ////////				invdist = betainv(p, alpha, beta, a, b);
        ////////			}
        ////////			
        ////////			if(invdist <= 0)
        ////////				return FormulaErrorStrings[iterations_dont_converge];
        ////////
        ////////			return invdist.ToString();
        ////////		}
        ////////
        ////////		private double betainv(double p, double alpha, double beta, double a, double b)
        ////////		{
        ////////			double guess = (a + b) / 2;//(u == 0) ? 1 : u;
        ////////	
        ////////			double lastIncrement = (b - a) / 4;
        ////////			double gi = 0;
        ////////			double eps = 1e-7;
        ////////			int k = 100;
        ////////			int tries = 3;
        ////////			int its = 0;
        ////////			while(k == 100 && tries > 0)
        ////////			{
        ////////				tries--;
        ////////				lastIncrement = (guess - a)/2;
        ////////				guess = guess - (guess - a)/2;
        ////////				
        ////////				 
        ////////				for(k = 0; k < 100; ++k)
        ////////				{
        ////////					its++;
        ////////					gi = betadist(guess, alpha, beta, a, b);
        ////////					if(Math.Abs((gi - p)/p) < eps)
        ////////					{
        ////////						break;
        ////////					}
        ////////					if(gi < p)
        ////////					{
        ////////						if(guess + lastIncrement > b)
        ////////							lastIncrement = (b - guess) /2;
        ////////						guess += lastIncrement;
        ////////					}
        ////////					else
        ////////					{
        ////////						lastIncrement = lastIncrement / 2;
        ////////						if(guess - lastIncrement < a)
        ////////							lastIncrement = (guess - a) / 2;
        ////////						guess -= lastIncrement;
        ////////					}
        ////////				}
        ////////			}
        ////////			if(k == 100)
        ////////				guess = -1;
        ////////
        ////////			//Console.WriteLine(its);
        ////////			return guess;
        ////////		}
        ////////
        ////////		/// <summary>
        ////////		/// Returns an F-test statistic.
        ////////		/// </summary>
        ////////		/// <param name="range">Range1, range2.</param>
        ////////		/// <returns>F-test statistic.</returns>
        ////////		public string ComputeFtest(string range)
        ////////		{
        ////////
        ////////			string[] args = range.Split(new char[]{ParseArgumentSeparator});
        ////////			int argCount = args.GetLength(0);
        ////////			if(argCount != 2 )
        ////////			{
        ////////				return FormulaErrorStrings[wrong_number_arguments];
        ////////			}
        ////////
        ////////			double[] yy = GetDoubleArray(args[0]);
        ////////			double[] xx = GetDoubleArray(args[1]);
        ////////			 
        ////////			if(xx.GetLength(0) < 2 ||  yy.GetLength(0) < 2)
        ////////				return FormulaErrorStrings[wrong_number_arguments];
        ////////
        ////////			double varx = var(xx);
        ////////			double vary = var(yy);
        ////////
        ////////			double vratio;
        ////////			double dft;
        ////////			double dfb;
        ////////			if(varx < vary)
        ////////			{
        ////////				vratio = vary / varx;
        ////////				dft = yy.GetLength(0) - 1;
        ////////				dfb = xx.GetLength(0) - 1;
        ////////			}
        ////////			else
        ////////			{
        ////////				vratio = varx / vary;
        ////////				dft = xx.GetLength(0) - 1;
        ////////				dfb = yy.GetLength(0) - 1;
        ////////			}
        ////////			
        ////////			double d = dfb / (dfb + dft * vratio);
        ////////			d = 2 * betadist(d, dfb / 2, dft / 2, 0, 1); //both halves
        ////////			if(d > 1)
        ////////			{
        ////////				d = 2 - d;
        ////////			}
        ////////			return d.ToString();
        ////////		}

        private double var(double[] x)
        {
            double sumx = 0;
            int n = x.GetLength(0);
            for (int i = 0; i < n; ++i)
            {
                sumx += x[i];
            }
            sumx = sumx / n;

            double sumx2 = 0;
            double d;
            for (int i = 0; i < n; ++i)
            {
                d = x[i] - sumx;
                sumx2 += d * d;
            }
            return sumx2 / (n - 1);
        }


        /// <summary>
        /// Returns the kurtosis of the passed-in values.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>The kurtosis of the data.</returns>
        public string ComputeKurt(string range)
        {


            double[] xx = GetDoubleArray(range);
            int n = xx.GetLength(0);
            if (n <= 3)
                return FormulaErrorStrings[this.wrong_number_arguments];

            double xbar = 0;
            double sdev = sd(xx, out xbar);

            double sum = 0;
            for (int i = 0; i < n; ++i)
            {

                sum += Math.Pow((xx[i] - xbar) / sdev, 4);
            }
            xbar = (double)n; // make it a double to avoid integer arithmetic
            sum = xbar * (xbar + 1) / ((xbar - 1) * (xbar - 2) * (xbar - 3)) * sum - 3 * (xbar - 1) * (xbar - 1) / ((xbar - 2) * (xbar - 3));
            return sum.ToString();
        }

        /// <summary>
        /// Returns the kth largest value in the range.
        /// </summary>
        /// <param name="range">Range, k.</param>
        /// <returns>Kth largest value.</returns>
        public string ComputeLarge(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double x0;
            string s1 = GetValueFromArg(args[1]);
            if (!double.TryParse(s1, NumberStyles.Integer, null, out x0))
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            int k = (int)x0;
            double[] dd = GetDoubleArray(args[0]);
            int n = dd.GetLength(0);
            if (k < 1 || k > n)
            {
                return FormulaErrorStrings[invalid_arguments];
            }
            Array.Sort(dd);
            return dd[n - k].ToString();
        }

        /// <summary>
        /// Returns the lognormal distribution.
        /// </summary>
        /// <param name="argList">X, mean, standarddev.</param>
        /// <returns>Returns the lognormal distribution.</returns>
        public string ComputeLognormdist(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double u;
            double s;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out u)
                && double.TryParse(args[2], NumberStyles.Any, null, out s)
                )
            {

                dist = normaldist(Math.Log(x), u, s);
            }

            return dist.ToString();
        }

        /// <summary>
        /// Returns the inverse of the lognormal distribution.
        /// </summary>
        /// <param name="argList">P, mean, standarddev.</param>
        /// <returns>Returns the value x where the lognormal distribution of x is p.</returns>
        public string ComputeLoginv(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double p;
            double u;
            double s;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out p)
                && double.TryParse(args[1], NumberStyles.Any, null, out u)
                && double.TryParse(args[2], NumberStyles.Any, null, out s)
                )
            {
                dist = Math.Exp(normalinv(p, u, s));
            }

            return dist.ToString();
        }

        /// <summary>
        /// Returns the maximum value of all values listed in the argument including logical values.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the maximum value of all values listed in the argument.</returns>
        /// <remarks> True is treated as 1 and False is treated as 0.
        /// </remarks>
        public string ComputeMaxa(string range)
        {
            double max = double.MinValue;
            double d;
            string s1;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //cell range
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            //TraceUtil.TraceExceptionCatched(ex);
                            //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            //	throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0) //ignore if empty
                        {
                            d = 0;
                            if (s1.ToUpper() == TRUEVALUESTR)
                            {
                                d = 1;
                            }
                            else
                            {
                                double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d);
                            }
                            max = Math.Max(max, d);
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        d = 0;
                        if (s1.ToUpper() == TRUEVALUESTR)
                        {
                            d = 1;
                        }
                        else
                        {
                            double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d);
                        }
                        max = Math.Max(max, d);
                    }
                }
            }

            if (max != double.MinValue)
                return max.ToString();

            return "";
        }

        /// <summary>
        /// Returns the median value in the range.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>Median value.</returns>
        public string ComputeMedian(string range)
        {
            double[] dd = GetDoubleArray(range);
            Array.Sort(dd);
            int n = dd.GetLength(0) / 2;
            string s1 = "";
            if (dd.GetLength(0) % 2 == 1)
            {
                s1 = dd[n].ToString();
            }
            else
            {
                s1 = ((dd[n] + dd[n - 1]) / 2).ToString();
            }
            return s1;
        }

        /// <summary>
        /// Returns the minimum value of all values listed in the argument including logical values.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>A string holding the minimum value of all values listed in the argument.</returns>
        /// <remarks> True is treated as 1 and False is treated as 0.
        /// </remarks>
        public string ComputeMina(string range)
        {
            double min = double.MaxValue;
            double d;
            string s1;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //cell range
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            //TraceUtil.TraceExceptionCatched(ex);
                            //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            //	throw ex;
                            return ex.Message;
                            //throw new ArgumentException(ex.Message);
                        }
                        if (s1.Length > 0)
                        {
                            d = 0;
                            if (s1.ToUpper() == TRUEVALUESTR)
                            {
                                d = 1;
                            }
                            else
                            {
                                double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d);
                            }
                            min = Math.Min(min, d);
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch (Exception ex)
                    {
                        //TraceUtil.TraceExceptionCatched(ex);
                        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        //	throw ex;
                        return ex.Message;
                    }
                    if (s1.Length > 0)
                    {
                        d = 0;
                        if (s1.ToUpper() == TRUEVALUESTR)
                        {
                            d = 1;
                        }
                        else
                        {
                            double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d);
                        }
                        min = Math.Min(min, d);
                    }
                }
            }

            if (min != double.MaxValue)
                return min.ToString();

            return "";
        }

        /// <summary>
        /// Returns the most frequent value in the range.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>The most frequent value.</returns>
        public string ComputeMode(string range)
        {
            double[] dd = GetDoubleArray(range);
            double[] ddSave = dd.Clone() as Double[];
            int n = dd.GetLength(0);
            if (n <= 1)
            {
                return "#N/A";
            }

            Array.Sort(dd);
            List<double> ties = new List<double>();
            double mode = double.NaN;
            int count = 0;
            int maxCount = 0;

            for (int i = 1; i < n; ++i)
            {
                if (dd[i] == dd[i - 1])
                {
                    count++;
                }
                else
                {
                    if (count > maxCount)
                    {
                        maxCount = count;
                        mode = dd[i - 1];
                        ties.Clear();
                        ties.Add(mode);
                    }
                    else if (count == maxCount)
                    {
                        ties.Add(dd[i - 1]);
                    }

                    count = 0;
                }
            }

            if (count > maxCount)
            {
                maxCount = count;
                mode = dd[n - 1];
                ties.Clear();
                ties.Add(mode);
            }

            if (maxCount > 0)
            {
                if (count == maxCount)
                {
                    ties.Add(dd[n - 1]);
                }

                if (ties.Count > 1)
                {
                    foreach (double d in ddSave)
                    {
                        if (ties.IndexOf(d) > -1)
                        {
                            mode = d;
                            break;
                        }
                    }
                }

                return mode.ToString();
            }

            return "#N/A";
        }


        /// <summary>
        /// Returns the negative binomial distribution.
        /// </summary>
        /// <param name="argList">Number of failures, success threshold, probability, cumulative.</param>
        /// <returns>Returns the negative binomial distribution.</returns>
        public string ComputeNegbinomdist(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double successes;
            double failures;
            double p;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out failures)
                && double.TryParse(args[1], NumberStyles.Any, null, out successes)
                && double.TryParse(args[2], NumberStyles.Any, null, out p)
                )
            {
                dist = negbinomdensity((int)failures, (int)successes, p);
            }

            return dist.ToString();
        }


        private double negbinomdensity(int failures, int successes, double p)
        {
            return this.comb(successes - 1, failures + successes - 1) * Math.Pow(p, successes) * Math.Pow(1 - p, failures);
        }

        /// <summary>
        /// Returns the Pearson product moment correlation coefficient.
        /// </summary>
        /// <param name="range">range1, range2</param>
        /// <returns>Pearson product</returns>
        public string ComputePearson(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double[] y = GetDoubleArray(args[0]);
            double[] x = GetDoubleArray(args[1]);

            int n = x.GetLength(0);

            if (n < 2 || n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];


            double sumx = 0;
            double sumy = 0;
            for (int i = 0; i < n; ++i)
            {
                sumx += x[i];
                sumy += y[i];
            }
            sumx = sumx / n;
            sumy = sumy / n;

            double sumxy = 0;
            double sumx2 = 0;
            double sumy2 = 0;
            double d1, d;
            for (int i = 0; i < n; ++i)
            {
                d = x[i] - sumx;
                d1 = y[i] - sumy;
                sumxy += d * d1;
                sumx2 += d * d;
                sumy2 += d1 * d1;
            }

            return (sumxy / Math.Sqrt(sumx2 * sumy2)).ToString();
        }

        /// <summary>
        /// Returns the percentile position in the range.
        /// </summary>
        /// <param name="range">Range, k.</param>
        /// <returns>Percentile position.</returns>
        /// <remarks>K is a value between 0 and 1.</remarks>
        public string ComputePercentile(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double k;
            string s1 = GetValueFromArg(args[1]);
            if (!double.TryParse(s1, NumberStyles.Any, null, out k) && (k < 0 || k > 1))
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            double[] dd = GetDoubleArray(args[0]);
            int n = dd.GetLength(0);

            Array.Sort(dd);

            double h = 1d / (n - 1);
            double d = dd[n - 1];
            for (int i = 0; i < n - 1; ++i)
            {
                if ((i + 1) * h > k)
                {
                    k = (k - i * h) / h;
                    d = dd[i] + k * (dd[i + 1] - dd[i]);
                    break;
                }
            }

            return d.ToString();
        }

        /// <summary>
        /// Returns the percentage rank in the range.
        /// </summary>
        /// <param name="range">Range, x, signifcant digits.</param>
        /// <returns>Percentile position.</returns>
        /// <remarks>Signifcant digits are optional, defaulting to 3.</remarks>
        public string ComputePercentrank(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double x;
            double signif = 3;
            string s1 = GetValueFromArg(args[1]);
            if (!double.TryParse(s1, NumberStyles.Any, null, out x))
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            if (argCount == 3)
            {
                s1 = GetValueFromArg(args[2]);
                if (!double.TryParse(s1, NumberStyles.Integer, null, out signif) && signif < 1)
                {
                    return FormulaErrorStrings[invalid_arguments];
                }
            }

            double[] dd = GetDoubleArray(args[0]);
            int n = dd.GetLength(0);

            Array.Sort(dd);

            double d = 1;
            for (int i = 0; i < n; ++i)
            {
                if (dd[i] > x)
                {
                    int k = 0;
                    while (k + i < n && dd[k + i] == x)
                        k++;

                    d = ((double)(i - 1)) / (i + n - i - k - 1);

                    if (i > 0 && dd[i - 1] < x)
                    {
                        double di = ((double)i) / (n - 1);
                        d = di + (d - di) * (1 - (x - dd[i - 1]) / (dd[i] - dd[i - 1]));
                    }
                    break;
                }
            }
            string fmt = "0." + new string('#', (int)(signif + 1));
            fmt = d.ToString(fmt);
            return fmt.Substring(0, fmt.Length - 1);
        }

        /// <summary>
        /// The number of permutations of n items taken k at the time. 
        /// </summary>
        /// <param name="argList">N, k</param>
        /// <returns>The number of combinations.</returns>
        public string ComputePermut(string argList)
        {

            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[this.requires_2_args];
            }
            //N things taken k at the time.
            double nd;
            double kd;
            double combin = 0;
            if (double.TryParse(GetValueFromArg(args[0]), NumberStyles.Any, null, out nd)
                && double.TryParse(GetValueFromArg(args[1]), NumberStyles.Any, null, out kd))
            {
                int k = (int)(kd + 0.1);
                int n = (int)(nd + 0.1);

                double top = 1;
                for (int i = (n - k) + 1; i <= n; ++i)
                    top = top * i;

                combin = top;
            }
            else
                return FormulaErrorStrings[invalid_arguments];
            return combin.ToString();
        }

        /// <summary>
        /// Returns the Poisson distribution.
        /// </summary>
        /// <param name="argList">X, mean, cumulative.</param>
        /// <returns>Returns the exponential distribution.</returns>
        /// <remarks>
        /// Cumulative should be either 
        /// True if you want to return the value of the distribution function or False
        /// if you want to return the value of the density function. 
        /// </remarks>
        public string ComputePoisson(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double u;
            double cum = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out u)
                )
            {
                cum = (args[2] == TRUEVALUESTR || args[2] == "1") ? 1 : 0;
                int n = (int)x;
                if (cum == 0)
                {
                    double prod = 1;
                    for (int i = 2; i <= n; ++i)
                    {
                        prod *= i;
                    }
                    dist = Math.Exp(-u) * Math.Pow(u, n) / prod;
                }
                else
                {
                    double prod = 1;
                    dist = 0;
                    double ui = 1;
                    for (int i = 0; i <= n; ++i)
                    {
                        dist += ui / prod;
                        prod *= (i + 1);
                        ui *= u;
                    }
                    dist = Math.Exp(-u) * dist;
                }
            }

            return dist.ToString();
        }

        /// <summary>
        /// Returns the probability that a value in the given range occurs.
        /// </summary>
        /// <param name="range">Xrange1, prange2, lowerbound, upperbound.</param>
        /// <returns>The probability.</returns>
        public string ComputeProb(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3 && argCount != 4)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            string s1 = GetValueFromArg(args[2]);
            double lower = 0;
            if (!double.TryParse(s1, NumberStyles.Any, null, out lower))
            {
                return FormulaErrorStrings[this.invalid_arguments];
            }

            double upper = lower;
            if (argCount == 4)
            {
                s1 = GetValueFromArg(args[3]);
                double.TryParse(s1, NumberStyles.Any, null, out upper);
            }

            double[] y = GetDoubleArray(args[1]);
            double[] x = GetDoubleArray(args[0]);
            int n = x.GetLength(0);

            if (n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];

            double sum = 0;
            double d;
            for (int i = 0; i < n; ++i)
            {
                d = x[i];
                if (d >= lower && d <= upper)
                    sum += y[i];
            }

            return sum.ToString();
        }

        /// <summary>
        /// Returns the quartile position in the range.
        /// </summary>
        /// <param name="range">Range, q.</param>
        /// <returns>Percentile position.</returns>
        /// <remarks>Q is 0, 1, 2, 3, 4.</remarks>
        public string ComputeQuartile(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double q;
            string s1 = GetValueFromArg(args[1]);
            if (double.TryParse(s1, NumberStyles.Integer, null, out q) && q >= 0 && q <= 4)
            {
                return this.ComputePercentile(args[0] + ParseArgumentSeparator + (q * .25).ToString());
            }
            return FormulaErrorStrings[this.invalid_arguments];
        }

        /// <summary>
        /// Returns the rank of x in the range.
        /// </summary>
        /// <param name="range">X, range, order.</param>
        /// <returns>Rank.</returns>
        public string ComputeRank(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            int rank = 0;
            double x;
            string s1 = GetValueFromArg(args[0]);
            if (double.TryParse(s1, NumberStyles.Any, null, out x))
            {
                double order = 0;
                if (argCount == 3)
                {
                    s1 = GetValueFromArg(args[2]);
                    double.TryParse(s1, NumberStyles.Integer, null, out order);
                }


                string r = args[1];

                double d = 0;
                bool eq = false;
                if (r.IndexOf(':') > -1)
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                        if (double.TryParse(s1, NumberStyles.Any, null, out d))
                        {
                            if (order == 1)
                            {
                                if (d < x)
                                    rank += 1;
                                else if (d == x)
                                    eq = true;
                            }
                            else
                            {
                                if (d > x)
                                    rank += 1;
                                else if (d == x)
                                    eq = true;
                            }
                        }
                    }
                    if (eq)
                        rank += 1;
                }

            }
            return rank.ToString();
        }

        /// <summary>
        /// Returns the square of the Pearson product moment correlation coefficient.
        /// </summary>
        /// <param name="range">Range1, range2.</param>
        /// <returns>Square of the Pearson product.</returns>
        public string ComputeRsq(string range)
        {
            string s = this.ComputePearson(range);
            double d = 0;
            if (double.TryParse(s, NumberStyles.Any, null, out d))
            {
                d = d * d;
            }
            return d.ToString();
        }

        /// <summary>
        /// Returns the skewness of a distribution.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>Skewness of a distribution.</returns>
        public string ComputeSkew(string range)
        {
            double[] x = GetDoubleArray(range);
            int n = x.GetLength(0);

            if (n < 3)
                return FormulaErrorStrings[this.invalid_arguments];

            double xbar = 0;
            double sdev = sd(x, out xbar);
            double skew = 0;
            for (int i = 0; i < n; ++i)
                skew += Math.Pow((x[i] - xbar) / sdev, 3);
            return (n * skew / (n - 1) / (n - 2)).ToString();
        }

        private double sd(double[] x, out double xbar)
        {
            int n = x.GetLength(0);
            xbar = 0;
            for (int i = 0; i < n; ++i)
            {
                xbar += x[i];
            }
            xbar = xbar / n;
            double sumx2 = 0;
            double d = 0;

            for (int i = 0; i < n; ++i)
            {
                d = x[i] - xbar;
                sumx2 += d * d;
            }
            return Math.Sqrt(sumx2 / (n - 1));
        }

        /// <summary>
        /// Returns the slope of the least square fit line through the given points.
        /// </summary>
        /// <param name="range">Y_range, x_range.</param>
        /// <returns>y-intercept.</returns>
        public string ComputeSlope(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double[] y = GetDoubleArray(args[0]);
            double[] x = GetDoubleArray(args[1]);
            int n = x.GetLength(0);

            if (n <= 2 || n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];

            double sumx = 0;
            double sumy = 0;
            for (int i = 0; i < n; ++i)
            {
                sumx += x[i];
                sumy += y[i];
            }
            sumx = sumx / n;
            sumy = sumy / n;

            double sumxy = 0;
            double sumx2 = 0;
            double d;
            for (int i = 0; i < n; ++i)
            {
                d = x[i] - sumx;
                sumxy += d * (y[i] - sumy);
                sumx2 += d * d;
            }
            return (sumxy / sumx2).ToString();
        }

        /// <summary>
        /// Returns the kth smallest value in the range.
        /// </summary>
        /// <param name="range">Range, k.</param>
        /// <returns>Kth smallest value.</returns>
        public string ComputeSmall(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double x0;
            string s1 = GetValueFromArg(args[1]);
            if (!double.TryParse(s1, NumberStyles.Integer, null, out x0))
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            int k = (int)x0;
            double[] dd = GetDoubleArray(args[0]);
            if (k < 1 || k > dd.GetLength(0))
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            Array.Sort(dd);
            return dd[k - 1].ToString();
        }

        /// <summary>
        /// Returns a normalized value.
        /// </summary>
        /// <param name="argList">X, mean, stddev.</param>
        /// <returns>Normalized value.</returns>
        public string ComputeStandardize(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double u;
            double sd = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out u)
                && double.TryParse(args[2], NumberStyles.Any, null, out sd)
                )
            {
                dist = (x - u) / sd;
            }
            return dist.ToString();
        }

        /// <summary>
        /// Returns the sample standard deviation.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>The sample standard deviation.</returns>
        public string ComputeStdev(string range)
        {
            double[] dd = GetDoubleArray(range);
            int n = dd.GetLength(0);

            if (n < 2)
                return FormulaErrorStrings[this.invalid_arguments];

            double xbar = 0;
            double sdev = sd(dd, out xbar);
            return sdev.ToString();
        }

        /// <summary>
        /// Returns the sample standard deviation.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>The sample standard deviation.</returns>
        /// <remarks>Treats True as 1 and False as 0.
        /// </remarks>
        public string ComputeStdeva(string range)
        {
            double[] dd = GetDoubleArrayA(range);
            int n = dd.GetLength(0);

            if (n < 2)
                return FormulaErrorStrings[this.invalid_arguments];

            double xbar = 0;
            double sdev = sd(dd, out xbar);
            return sdev.ToString();
        }

        /// <summary>
        /// Returns the population standard deviation.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>The population standard deviation.</returns>
        public string ComputeStdevp(string range)
        {
            double[] dd = GetDoubleArray(range);
            int n = dd.GetLength(0);

            if (n < 2)
                return FormulaErrorStrings[this.invalid_arguments];

            double xbar = 0;
            double sdev = sd(dd, out xbar);
            return (sdev * Math.Sqrt(n - 1) / Math.Sqrt(n)).ToString();
        }

        /// <summary>
        /// Returns the population standard deviation.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.
        /// </param>
        /// <returns>The population standard deviation.</returns>
        /// <remarks>Treats True as 1 and False as 0.
        /// </remarks>
        public string ComputeStdevpa(string range)
        {
            double[] dd = GetDoubleArrayA(range);
            int n = dd.GetLength(0);

            if (n < 2)
                return FormulaErrorStrings[this.invalid_arguments];

            double xbar = 0;
            double sdev = sd(dd, out xbar);
            return (sdev * Math.Sqrt(n - 1) / Math.Sqrt(n)).ToString();
        }

        /// <summary>
        /// Returns the standard error of the least square fit line through the given points.
        /// </summary>
        /// <param name="range">Y_range, x_range.</param>
        /// <returns>Standard error.</returns>
        public string ComputeSteyx(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double[] y = GetDoubleArray(args[0]);
            double[] x = GetDoubleArray(args[1]);
            int n = x.GetLength(0);

            if (n <= 2 || n != y.GetLength(0))
                return FormulaErrorStrings[wrong_number_arguments];
            double sumx = 0;
            double sumy = 0;
            for (int i = 0; i < n; ++i)
            {
                sumx += x[i];
                sumy += y[i];
            }
            sumx = sumx / n;
            sumy = sumy / n;

            double sumxy = 0;
            double sumx2 = 0;
            double sumy2 = 0;
            double dy = 0;
            double d = 0;
            for (int i = 0; i < n; ++i)
            {
                d = x[i] - sumx;
                dy = y[i] - sumy;
                sumxy += d * dy;
                sumx2 += d * d;
                sumy2 += dy * dy;
            }
            return (Math.Sqrt((sumy2 - sumxy * sumxy / sumx2) / (n - 2))).ToString();
        }


        /// <summary>
        /// Returns the mean of the range after removing points on either extreme.
        /// </summary>
        /// <param name="range">Range, percent.</param>
        /// <returns>Kth smallest value.</returns>
        public string ComputeTrimmean(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double percent;
            string s1 = GetValueFromArg(args[1]);
            if (!double.TryParse(s1, NumberStyles.Any, null, out percent))
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            double[] dd = GetDoubleArray(args[0]);
            int n = dd.GetLength(0);
            int k = (int)(percent * n);
            k = k / 2;
            if (k < 1 || 2 * k >= n)
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            Array.Sort(dd);

            double sum = 0;
            n = n - k;
            for (int i = k; i < n; ++i)
            {
                sum += dd[i];
            }
            return (sum / (n - k)).ToString();
        }

        /// <summary>
        /// Returns sample variance of the listed values.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>The sample variance.</returns>
        public string ComputeVar(string range)
        {
            double[] dd = GetDoubleArray(range);
            return var(dd).ToString();
        }

        private double[] GetDoubleArray(string range)
        {
            double d;
            string s1 = "";
            List<double> x = new List<double>();
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //cell range
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch { }
                        if (s1.Length > 0) //ignore if empty
                        {
                            if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                                x.Add(d);
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch { }
                    if (s1.Length > 0)
                    {
                        if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            x.Add(d);
                    }
                }
            }

#if WPF
            return (double[])x.ToArray(typeof(double));
#else
            return (double[])x.ToArray();
#endif
        }

        private double[] GetDoubleArrayA(string range)
        {
            double d;
            string s1 = "";
            List<double> x = new List<double>();
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //cell range
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch { }
                        if (s1.Length > 0) //ignore if empty
                        {
                            d = 0;
                            if (s1.ToUpper() == TRUEVALUESTR)
                            {
                                d = 1;
                            }
                            else
                            {
                                double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d);
                            }
                            x.Add(d);
                        }
                    }
                }
                else
                {
                    try
                    {
                        s1 = GetValueFromArg(r);
                    }
                    catch { }
                    if (s1.Length > 0) //ignore if empty
                    {
                        d = 0;
                        if (s1.ToUpper() == TRUEVALUESTR)
                        {
                            d = 1;
                        }
                        else
                        {
                            double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d);
                        }
                        x.Add(d);
                    }
                }
            }

#if WPF
            return (double[])x.ToArray(typeof(double));
#else
            return (double[])x.ToArray();
#endif
        }

        /// <summary>
        /// Returns sample variance of the listed values.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>The sample variance.</returns>
        /// <remarks> True is treated as 1 and False is treated as 0.
        /// </remarks>
        public string ComputeVara(string range)
        {
            double[] dd = GetDoubleArrayA(range);
            return var(dd).ToString();
        }

        /// <summary>
        /// Returns population variance of the listed values.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>The population variance.</returns>
        public string ComputeVarp(string range)
        {
            double[] dd = GetDoubleArray(range);//(double[]) x.ToArray(typeof(double));
            int n = dd.GetLength(0);
            return ((n - 1) * var(dd) / n).ToString();
        }

        /// <summary>
        /// Returns population variance of the listed values.
        /// </summary>
        /// <param name="range">A string holding a list (separated by commas) of:
        /// cell references,
        /// formulas, or numbers.</param>
        /// <returns>The population variance.</returns>
        /// <remarks> True is treated as 1 and False is treated as 0.
        /// </remarks>
        public string ComputeVarpa(string range)
        {
            double[] dd = GetDoubleArrayA(range);
            int n = dd.GetLength(0);
            return ((n - 1) * var(dd) / n).ToString();
        }

        /// <summary>
        /// Returns the Weibull distribution.
        /// </summary>
        /// <param name="argList">X, alpha, beta, cumulative.</param>
        /// <returns>Returns the Weibull distribution.</returns>
        public string ComputeWeibull(string argList)
        {
            string[] args = argList.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 4 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }
            double x;
            double a;
            double b;
            double cum = 0;
            double dist = 0;

            for (int i = 0; i < argCount; ++i)
            {
                args[i] = GetValueFromArg(args[i]);
            }

            if (double.TryParse(args[0], NumberStyles.Any, null, out x)
                && double.TryParse(args[1], NumberStyles.Any, null, out a)
                && double.TryParse(args[2], NumberStyles.Any, null, out b)
                )
            {
                if (argCount != 3)
                    cum = (args[3] == TRUEVALUESTR || args[3] == "1") ? 1 : 0;
                if (cum == 0)
                {
                    dist = a / Math.Pow(b, a) * Math.Pow(x, a - 1) * Math.Exp(-Math.Pow(x / b, a));
                }
                else
                {
                    dist = 1 - Math.Exp(-Math.Pow(x / b, a));
                }
            }

            return dist.ToString();
        }

        /// <summary>
        /// Returns the one-tailed probability value of a Z test.
        /// </summary>
        /// <param name="range">Range, mu, sigma.</param>
        /// <returns>Kth smallest value.</returns>
        public string ComputeZtest(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 2 && argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            double mu;
            double sigma;
            string s1 = GetValueFromArg(args[1]);
            if (!double.TryParse(s1, NumberStyles.Any, null, out mu))
            {
                return FormulaErrorStrings[invalid_arguments];
            }

            double[] dd = GetDoubleArray(args[0]);
            double xbar;
            sigma = sd(dd, out xbar);
            if (argCount == 3)
                double.TryParse(s1, NumberStyles.Any, null, out sigma);

            return (1 - this.normaldist((xbar - mu) / (sigma / Math.Sqrt(dd.GetLength(0))), 0, 1)).ToString();
        }

        /// <summary>
        /// Returns a horizontal table look up value.
        /// </summary>
        /// <param name="range">Contains search value, table, return index and match properties.</param>
        /// <returns>Matching value found in the table.</returns>
        /// <remarks> For example, =HLOOKUP("Axles",A1:C4,2,TRUE) looks for the exact 
        /// match for Axles in A1:C1 and returns the corresponding value in A2:C2. 
        /// </remarks>
        public string ComputeHLookUp(string range)
        {

            string[] s = range.Split(new char[] { ParseArgumentSeparator });
            string lookUp = GetValueFromArg(s[0]);
            lookUp = lookUp.Replace(TIC, "").ToUpper();
            string r = s[1];
            string o1 = GetValueFromArg(s[2]).ToString().ToUpper();
            double d = 0;
            if (!double.TryParse(o1, NumberStyles.Any, null, out d))
                return "#N/A";

            //int row = int.Parse(o1);	
            int row = (int)d;

            bool match = true;
            if (s.GetLength(0) == 4)
            {
                string matchString = GetValueFromArg(s[3]);
                match = (matchString == TRUEVALUESTR || matchString == "1");
            }

            bool typeIsNumber = match ? double.TryParse(lookUp, NumberStyles.Any, null, out d) : false;


            int i = r.IndexOf(":");
            if (i == -1)//single cell
            {
                r = r + ":" + r;
                i = r.IndexOf(":");
            }
            int k = r.Substring(0, i).LastIndexOf(sheetToken);
            GridModel grd = this.grid;
            GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);

            if (k > -1)
            {
                this.grid = (GridModel)family.TokenToGridModel[r.Substring(0, k + 1)];
            }

            int row1 = RowIndex(r.Substring(0, i));
            int col1 = ColIndex(r.Substring(0, i));

            int row2 = RowIndex(r.Substring(i + 1));
            int col2 = ColIndex(r.Substring(i + 1));

            string val = "";
            int lastCol = col1;
            string s1 = "";
            double d1 = 0;
            for (int col = col1; col <= col2; ++col)
            {
                s1 = this.grid[row1, col].FormattedText.ToUpper().Replace("\"", "");
                //if(s1 == lookUp ||(match && s1.CompareTo(lookUp) > 0))
                if (s1 == lookUp ||
                    (match && (typeIsNumber
                    ? (double.TryParse(s1, NumberStyles.Any, null, out d1) && (d1.CompareTo(d) > 0))
                    : (s1.CompareTo(lookUp) > 0)
                    )))
                {
                    if (s1 == lookUp)
                        lastCol = col;

                    break;
                }
                lastCol = col;
            }
            if (match || s1 == lookUp)
            {
                val = this.grid[row + row1 - 1, lastCol].FormattedText;
                if (val.Length > 0 && val[0] == this.formulaChar)
                {
                    val = this.Parse(val);
                }
                d = 0; //zzz
                if (val.Length > 0 && val[0] != TIC[0] && !double.TryParse(val, NumberStyles.Any, null, out d))
                {
                    val = TIC + val + TIC;
                }
                //Console.WriteLine("found {0} returned {1}", lookUp, val);
            }
            else
                val = "#N/A";
            //val = "\"#N/A\"";


            //Console.WriteLine(" ");
            this.grid = grd;



            return val;


        }

        /// <summary>
        /// Returns a range that is the offset of the reference range by rows and cols.
        /// </summary>
        /// <param name="arg">reference, rows, cols, [height], [width]</param>
        /// <returns>A range offset.</returns>
        /// <remarks>The returned range is the range passed in through the reference variable offset
        /// by the number of rows in the rows variable and number of columns in the cols variable. If height and
        /// width are present in the argument list, they determine the number of rows and columns
        /// in the returned range. Otherwise, the dimensions of the returned range match the input range.
        /// </remarks>
        public string ComputeOffSet(string arg)
        {
            string[] args = arg.Split(new char[] { ParseArgumentSeparator }); ;
            int argCount = args.GetLength(0);
            if (argCount != 3 && argCount != 5)
            {
                return this.FormulaErrorStrings[wrong_number_arguments];
            }

            string r = args[0];
            double d;
            int rows = double.TryParse(this.GetValueFromArg(args[1]), NumberStyles.Integer, null, out d) ? (int)d : -1;
            int cols = double.TryParse(this.GetValueFromArg(args[2]), NumberStyles.Integer, null, out d) ? (int)d : -1;
            int width = -1;
            int height = -1;
            if (argCount == 5)
            {
                height = (double.TryParse(this.GetValueFromArg(args[3]), NumberStyles.Integer, null, out d) ? (int)d : 2) - 1;
                width = (double.TryParse(this.GetValueFromArg(args[4]), NumberStyles.Integer, null, out d) ? (int)d : 2) - 1;
            }

            int i = r.IndexOf(":");
            bool singleCell = i == -1;
            ////single cell
            if (singleCell)
            {
                r = r + ":" + r;
                i = r.IndexOf(":");
            }

            singleCell &= width <= 0 && height <= 0; ////only treat as single cell if no hieght or width given...
            int k = r.Substring(0, i).LastIndexOf(sheetToken);
            //GridModel grd = this.grid;
            //GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
            string sheet = string.Empty;
            if (k > -1)
            {
                sheet = r.Substring(0, k + 1);
                //Here the sheet name was added in front of the cell address. So here no need to change the Grid model.
                //this.grid = (GridModel)family.TokenToGridModel[r.Substring(0, k + 1)];
            }

            int row1 = this.RowIndex(r.Substring(0, i)) + rows;
            int col1 = this.ColIndex(r.Substring(0, i)) + cols;

            int row2 = this.RowIndex(r.Substring(i + 1)) + rows;
            int col2 = this.ColIndex(r.Substring(i + 1)) + cols;
            if (height < 1)
            {
                height = Math.Abs(row1 - row2);
            }

            if (width < 1)
            {
                width = Math.Abs(col1 - col2);
            }
            return singleCell ? this.ComputedValue(string.Format("{2}{0}{1}", GridRangeInfo.GetAlphaLabel(col1), row1, sheet))
                : string.Format("{4}{0}{1}:{2}{3}", GridRangeInfo.GetAlphaLabel(col1), row1, GridRangeInfo.GetAlphaLabel(col1 + width), row1 + height, sheet);
        }

        private int matchCompare(object o1, object o2)
        {
            string s1 = o1.ToString();
            string s2 = o2.ToString();
            double d1, d2;
            if (double.TryParse(s1, NumberStyles.Any, null, out d1)
                && double.TryParse(s2, NumberStyles.Any, null, out d2))
            {
                return d1.CompareTo(d2);
            }
            else
            {
                return s1.CompareTo(s2);
            }
        }

        /// <summary>
        /// Finds the index a specified value in a lookup_range.
        /// </summary>
        /// <param name="arg">look_value, lookup_range, match_type</param>
        /// <returns>The relative index of the lookup_value in the lookup_range.</returns>
        /// <remarks>
        /// Lookup_range should be a either a single row range or a single column range.
        /// If match_type is 0, the relative index of the first exact match (ignoring case)
        /// in the specified range is returned. If match_type is 1, the values in the range
        /// should be in ascending order, and the index of the largest value less than or
        /// equal to the lookup_value is returned. If match_type is -1, the values in the range
        /// should be in descending order, and the index of the smallest value greater than or
        /// equal to the lookup_value is returned.
        /// </remarks>
        public string ComputeMatch(string arg)
        {
            string[] args = arg.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3 && argCount != 2)
            {
                return this.FormulaErrorStrings[wrong_number_arguments];
            }

            string r = args[1];
            int i = r.IndexOf(":");
            ////single cell
            if (i == -1)
            {
                return this.FormulaErrorStrings[invalid_arguments];
            }

            int m = 1;
            if (argCount == 3)
            {
                double d;
                m = double.TryParse(this.GetValueFromArg(args[2]), NumberStyles.Integer, null, out d) ? (int)d : 1;
            }

            string searchItem = this.GetValueFromArg(args[0]).Replace(this.TIC, "").ToUpper();

            string[] cells = this.GetCellsFromArgs(r);
            int index = 1;
            string oldValue = "";
            string newValue;
            foreach (string s in cells)
            {
                newValue = this.GetValueFromArg(s).Replace(this.TIC, "").ToUpper();
                if (oldValue != "")
                {
                    if (m == 1)
                    {
                        if (this.matchCompare(newValue, oldValue) < 0)
                        {
                            index = -1;
                            break;
                        }
                    }
                    else if (m == -1)
                    {
                        if (this.matchCompare(newValue, oldValue) > 0)
                        {
                            index = -1;
                            break;
                        }
                    }
                }

                if (m == 0 && newValue == searchItem)
                {
                    break;
                }
                else if (m == 1 && this.matchCompare(searchItem, newValue) < 0)
                {
                    index--;
                    break;
                }
                else if (m == -1 && this.matchCompare(searchItem, newValue) > 0)
                {
                    index--;
                    break;
                }

                index++;
                oldValue = newValue;
            }

            if (m != 0 && index == cells.Length + 1)
            {
                index = cells.Length;
            }

            if (index > 0 && index <= cells.Length)
            {
                return index.ToString();
            }
            else
            {
                return "#N/A";
            }
        }

        /// <summary>
        /// Returns a vertical table look up value.
        /// </summary>
        /// <param name="range">Contains search value, table, return index and match properties.</param>
        /// <returns>Matching value found in the table.</returns>
        /// <remarks> For example, =VLOOKUP("Axles",A1:C4,2,TRUE) looks for the exact 
        /// match for Axles in A1:A4 and returns the corresponding value in B1:B4. 
        /// </remarks>
        public string ComputeVLookUp(string range)
        {
            string[] s = range.Split(new char[] { ParseArgumentSeparator });
            string lookUp = GetValueFromArg(s[0]);
            lookUp = lookUp.Replace(TIC, "").ToUpper();
            if (GridCellFormulaModel.IsEmpty(lookUp))
                lookUp = "0"; //empty is zero in calculation
            string r = s[1];
            string o1 = GetValueFromArg(s[2]).ToString().Replace("\"", "");

            double d = 0;
            if (!double.TryParse(o1, NumberStyles.Any, null, out d) || o1 == "NaN")
                return "#N/A";
            int col = (int)d;
            bool match = true;
            if (s.GetLength(0) == 4)
            {
                string matchString = GetValueFromArg(s[3]);
                match = (matchString == TRUEVALUESTR || matchString == "1");
            }

            bool typeIsNumber = match ? double.TryParse(lookUp, NumberStyles.Any, null, out d) : false;

            int i = r.IndexOf(":");
            if (i == -1)//single cell
            {
                r = r + ":" + r;
                i = r.IndexOf(":");
            }
            string sheet = string.Empty;
            int k = r.Substring(0, i).LastIndexOf(sheetToken);
            GridModel grd = this.grid;
            GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);

            if (k > -1)
            {
                sheet = r.Substring(0, k + 1);
                this.grid = (GridModel)family.TokenToGridModel[r.Substring(0, k + 1)];
            }

            //Console.WriteLine(string.Format("{0},{1},{2},{3}", lookUp, r, col, match));
            //To provide the support for the formula =VLOOKUP(N18,INPUT!A:H,6,FALSE)
            //Support to recognize the A: H range notation
            var rCopy = r;
            if (!string.IsNullOrEmpty(sheet))
                rCopy = rCopy.Replace(sheet, "");
            if (!Regex.IsMatch(rCopy, @"\d"))
            {
                int index = r.IndexOf(':');
                var temparg1 = r.Substring(0, index);
                temparg1 = temparg1 + "1";
                var temparg2 = r.Substring(index + 1, r.Length - index - 1);
                temparg2 = temparg2 + grid.RowCount;
                r = temparg1 + ":" + temparg2;
                i = r.IndexOf(':');
            }
            int row1 = RowIndex(r.Substring(0, i));
            int col1 = ColIndex(r.Substring(0, i));

            int row2 = RowIndex(r.Substring(i + 1));
            int col2 = ColIndex(r.Substring(i + 1));

            string val = "";
            int lastRow = row1;
            string s1 = "";
            double d1 = 0;
            for (int row = row1; row <= row2; ++row)
            {
                s1 = this.grid[row, col1].FormattedText.ToUpper().Replace("\"", "");
                //	if(s1 == lookUp || (match && s1.CompareTo(lookUp) > 0))
                if (s1 == lookUp ||
                    (match && (typeIsNumber
                    ? (double.TryParse(s1, NumberStyles.Any, null, out d1) && (d1.CompareTo(d) > 0))
                    : (s1.CompareTo(lookUp) > 0)
                    )))
                {
                    if (s1.ToUpper() == lookUp)
                        lastRow = row;
                    break;
                }
                lastRow = row;
            }

            if ((match && lastRow != row1) || s1 == lookUp)
            {
                val = this.grid[lastRow, col + col1 - 1].FormattedText;
                if (val.Length > 0 && val[0] == this.formulaChar)
                {
                    val = this.Parse(val);
                }
                d = 0;
                if (val.Length > 0 && val[0] != TIC[0] && !double.TryParse(val, NumberStyles.Any, null, out d))
                {
                    val = TIC + val + TIC;
                }
            }
            else
                val = "#N/A";
            //val = "\"#N/A\"";

            //Console.WriteLine("found {0} returned {1}", lookUp, val);


            //Console.WriteLine(" ");
            this.grid = grd;

            return val;
        }

        /// <summary>
        /// Returns the left so many characters in the given string.
        /// </summary>
        /// <param name="range">Contains the string and the number of characters.</param>
        /// <returns>A string.</returns>
        public string ComputeLeft(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 1 && argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            string s1 = args[0];
            s1 = GetValueFromArg(s1);
            bool hasTics = s1.StartsWith(TIC) && s1.EndsWith(TIC);
            string s2 = (argCount == 2) ? args[1] : "1";
            int len = int.Parse(s2) + (hasTics ? 1 : 0);
            len = (s1.Length >= len) ? len : s1.Length;
            s1 = s1.Substring(0, len);
            if (hasTics && !s1.EndsWith(TIC))
                s1 = s1 + TIC;
            return s1;
        }

        /// <summary>
        /// Returns the length of the given string.
        /// </summary>
        /// <param name="range">Contains the string.</param>
        /// <returns>An integer length.</returns>
        public string ComputeLen(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 1)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            string s1 = args[0];
            s1 = GetValueFromArg(s1);
            bool hasTics = s1.StartsWith(TIC) && s1.EndsWith(TIC);
            return (hasTics ? s1.Length - 2 : s1.Length).ToString();
        }


        /// <summary>
        /// Returns a substring of the given string.
        /// </summary>
        /// <param name="range">Contains the original string, start position of the substring, 
        /// and the number of characters in the substring.</param>
        /// <returns>A string.</returns>
        public string ComputeMid(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 3)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            string s1 = args[0];
            s1 = GetValueFromArg(s1);
            bool hasTics = s1.StartsWith(TIC) && s1.EndsWith(TIC);
            string s2 = args[2];
            int len = int.Parse(s2);
            s2 = args[1];
            int start = int.Parse(s2) + (hasTics ? 1 : 0);

            if (start + len > s1.Length)
                return FormulaErrorStrings[invalid_arguments];

            s1 = s1.Substring(start, len);
            if (hasTics && !s1.StartsWith(TIC))
                s1 = TIC + s1;
            if (hasTics && !s1.EndsWith(TIC))
                s1 = s1 + TIC;
            return s1;
        }


        /// <summary>
        /// Returns the right so many characters in the given string.
        /// </summary>
        /// <param name="range">Contains the string and the number of characters.</param>
        /// <returns>A string.</returns>
        public string ComputeRight(string range)
        {
            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            if (argCount != 1 && argCount != 2)
            {
                return FormulaErrorStrings[wrong_number_arguments];
            }

            string s1 = args[0];
            s1 = GetValueFromArg(s1);
            bool hasTics = s1.StartsWith(TIC) && s1.EndsWith(TIC);
            string s2 = (argCount == 2) ? args[1] : "1";
            int len = int.Parse(s2) + (hasTics ? 1 : 0);
            int start = (s1.Length >= len) ? s1.Length - len : 0;
            s1 = s1.Substring(start);
            if (hasTics && !s1.StartsWith(TIC))
                s1 = TIC + s1;
            return s1;
        }

        /// <summary>
        /// Returns True if the ParseArgumentSeparator character is included in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <returns>True or False.</returns>
        public bool IsSeparatorInTIC(string s)
        {
            int i = s.IndexOf(TIC) + 1;
            bool inTic = true;
            while (i > 0 && i < s.Length)
            {
                if (s[i] == ParseArgumentSeparator && inTic)
                    return true;
                if (s[i] == TIC[0])
                    inTic = !inTic;
                i++;
            }

            return false;
        }

        /// <summary>
        /// Returns an array of strings from an argument list.
        /// </summary>
        /// <param name="s">A delimited argument list.</param>
        /// <returns>Array of strings from an argument list.</returns>
        public string[] GetStringArray(string s)
        {
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
            System.Collections.Generic.List<string> argList = new System.Collections.Generic.List<string>();
#else
            ArrayList argList = new ArrayList();
#endif
            int argStart = 0;
            bool inQuote = false;
            for (int argEnd = 0; argEnd < s.Length; argEnd++)
            {
                char ch = s[argEnd];
                if (ch == TIC[0])
                    inQuote = !inQuote;
                else if (!inQuote && ch == ParseArgumentSeparator)
                {
                    argList.Add(s.Substring(argStart, argEnd - argStart));
                    argStart = argEnd + 1;
                }
            }
            argList.Add(s.Substring(argStart));
            string[] ar = new string[argList.Count];
            argList.CopyTo(ar);
            return ar;
        }

        //public string[] GetStringArray(string s)
        //{
        //    int i = s.IndexOf(TIC);
        //    int count = 0;
        //    if (i > 0)
        //    {
        //        count = s.Substring(0, i + 1).Split(new char[] { ParseArgumentSeparator }).GetLength(0) - 1;
        //    }
        //    int j = s.LastIndexOf(TIC);
        //    if (j > -1 && j < s.Length - 1)
        //        count += s.Substring(j).Split(new char[] { ParseArgumentSeparator }).GetLength(0) - 1;
        //    count += s.Substring(i, j - i).Split(new char[] { TIC[0] }).GetLength(0) / 2;

        //    string[] ar = new string[count];

        //    int k = 0;
        //    if (i > 0)
        //    {
        //        foreach (string s1 in s.Substring(0, i - 1).Split(new char[] { ParseArgumentSeparator }))
        //        {
        //            ar[k] = s1;
        //            k++;
        //        }
        //    }

        //    foreach (string s1 in s.Substring(i, j - i + 1).Split(new char[] { TIC[0] }))
        //    {
        //        if (s1.Length != 0 && s1[0] != ParseArgumentSeparator)
        //        {
        //            ar[k] = TIC + s1 + TIC;
        //            k++;
        //        }
        //    }

        //    if (j > -1 && j < s.Length - 2)
        //    {
        //        foreach (string s1 in s.Substring(j + 2).Split(new char[] { ParseArgumentSeparator }))
        //        {
        //            ar[k] = s1;
        //            k++;
        //        }
        //    }
        //    return ar;
        //}


        /// <summary>
        /// Returns a single character string.
        /// </summary>
        /// <param name="range">List of strings to be concatenated.</param>
        /// <returns>A single string.</returns>
        public string ComputeConcatenate(string range)
        {
            string text = "";

            string[] ar = IsSeparatorInTIC(range) //range.IndexOf(TIC) > 0 
                ? GetStringArray(range) : range.Split(new char[] { ParseArgumentSeparator });
            foreach (string r in ar)
            {
                if (r.Length > 0 && r[0] == TIC[0])
                    //To remove the Double codes form the string
                    text += r.Replace(TIC, "");
                else
                    text += GetValueFromArg(r);//TIC + GetValueFromArg(r) + TIC;

            }
            if (text.IndexOf("#N/A") > -1)
                text = "#N/A";
            else
            {
                //text = text.Replace(TIC + TIC, "");
                //if (text.Length > 0 && text[0] != TIC[0])
                text = TIC + text + TIC;
            }
            return text;

            //			string text = "";
            //			
            //			foreach(string r in range.Split(new char[]{ParseArgumentSeparator}))
            //			{
            //				if(r.IndexOf(':') > -1)
            //				{
            //					foreach(string s in GetCellsFromArgs(r))
            //					{
            //						text += TIC + GetValueFromArg(s) + TIC;
            //					}
            //				}
            //				else
            //				{
            //					text += TIC + GetValueFromArg(r) + TIC;
            //				}
            //			}
            //			text = text.Replace(TIC + TIC, "");
            //			return text;
        }

        /// <summary>
        /// Returns the reference specified by a text string. References are immediately evaluated to display their contents.
        /// <para>Syntax: INDIRECT(CellRefString, [IsA1Style])</para>
        /// </summary>
        /// <param name="args">Cell reference string.</param>
        /// <returns>Reference specified the argument.</returns>
        public string ComputeIndirect(string args)
        {

            if (args[args.Length - 1] == BMARKER)
                args = GetValueFromArg(args);
            string[] arg = args.Split(new char[] { ParseArgumentSeparator });
            if (arg.Length == 0 || arg.Length > 2)
                throw new ArgumentException("No. of argument cant be less than 1 or more than 2.");
            arg[0] = arg[0].ToUpper();
            PutTokensForSheets(ref arg[0]);
            string sheetToken = SheetToken(arg[0].Replace(TIC, ""));

            //Remove SheetToken
            if (!String.IsNullOrEmpty(sheetToken))
                arg[0] = arg[0].Replace(sheetToken, "");

            if (arg.Length == 2 && arg[1] == FALSEVALUESTR)
            {
                bool hasTIC = arg[0].StartsWith(TIC) && arg[0].EndsWith(TIC);
                string rcCell = arg[0].ToUpper().Replace(TIC, "");
                string[] cells = rcCell.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                string[] rc = cells[0].Split(new char[] { 'R', 'C' }, StringSplitOptions.RemoveEmptyEntries);
                if (cells.Length > 2)
                    return "#REF!";

                arg[0] = GridRangeInfo.GetAlphaLabel(Convert.ToInt32(rc[1])) + rc[0];

                if (cells.Length == 2)
                {
                    string st = SheetToken(cells[1]);
                    if (!String.IsNullOrEmpty(st))
                        cells[1] = cells[1].Replace(st, "");
                    rc = cells[1].Split(new char[] { 'R', 'C' }, StringSplitOptions.RemoveEmptyEntries);
                    arg[0] += ":" + GridRangeInfo.GetAlphaLabel(Convert.ToInt32(rc[1])) + rc[0];
                }

                if (hasTIC)
                    arg[0] = TIC + arg[0] + TIC;

            }

            string cellReference = string.Empty;
            if (arg[0].StartsWith(TIC)) //String
            {
                cellReference = sheetToken + arg[0].Replace(TIC, "");
            }
            else
            {
                if (IsCellReference(arg[0]))
                    cellReference = GetValueFromArg(sheetToken + arg[0]);
            }

            if (!IsCellReference(cellReference))
            {
                if (NamedRanges.Contains(cellReference.ToUpper()))
                    cellReference = Convert.ToString(NamedRanges[cellReference.ToUpper()]);
                else
                    return "#REF!";
            }

            if (cellReference.Contains(":"))
                return cellReference;

            return GetValueFromArg(cellReference);
        }

        /// <summary>
        /// In a given string, this method substitutes an occurrence of one string with another string.
        /// </summary>
        /// <param name="range">A list of 3 or 4 arguments: the original string, the search string, the 
        /// replacement string, and optionally, an integer representing the occurrence to be replaced.
        ///    </param>
        /// <returns>The modified string.</returns>
        public string ComputeSubstitute(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            if (args.Length != 3 && args.Length != 4)
                return FormulaErrorStrings[wrong_number_arguments];

            //strip off outside tics (if any) on the evaluated arguments
            string s1 = StripTics0(GetValueFromArg(args[0]));
            string s2 = StripTics0(GetValueFromArg(args[1]));
            string s3 = StripTics0(GetValueFromArg(args[2]));
            if (args.Length == 3)
            {
                s1 = s1.Replace(s2, s3);
            }
            else
            {
                string s4 = GetValueFromArg(args[3]);
                double d = 0;
                if (double.TryParse(s4, NumberStyles.Integer, null, out d))
                {
                    int count = (int)d;
                    int loc = -1;
                    while (count > 0 && (loc = s1.IndexOf(s2, loc + 1)) > -1)
                    {
                        count--;
                    }
                    if (count == 0)
                    {
                        s1 = s1.Substring(0, loc) + s3 + s1.Substring(loc + s2.Length);
                    }
                }
            }

            return TIC + s1 + TIC;
        }

        private string StripTics0(string s)
        {
            if (s.Length > 1 && s[0] == TIC[0] && s[s.Length - 1] == TIC[0])
            {
                s = s.Substring(1, s.Length - 2);
            }
            return s;
        }

        /// <summary>
        /// Returns the product of the arguments in the list.
        /// </summary>
        /// <param name="range">List of arguments.</param>
        /// <returns>Product of the arguments.</returns>
        public string ComputeProduct(string range)
        {
            double prod = 1;
            double d;
            string s1;
            bool nohits = true;
            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1)
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s, true);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;

                        }
                        if (s1.Length > 0)
                        {
                            if (double.TryParse(s1, NumberStyles.Any, null, out d))
                            {
                                prod = prod * d;
                                nohits = false;
                            }
                        }
                    }
                }
                else
                {
                    if (double.TryParse(GetValueFromArg(r), NumberStyles.Any, null, out d))
                    {
                        prod = prod * d;
                        nohits = false;
                    }
                }
            }

            return nohits ? "0" : prod.ToString();
        }

        /// <summary>
        /// Returns a quoted string from a date or number.
        /// </summary>
        /// <param name="range">Value to be converted to a string.</param>
        /// <returns>Quoted string.</returns>
        public string ComputeText(string range)
        {
            string s1 = range.Split(new char[] { ParseArgumentSeparator })[0];
            int length = s1.Length;
            string s2 = (range.Split(new char[] { ParseArgumentSeparator })[1]).Replace(TIC, "");
            //remove the [&-409] and ;@ form format {e.g: [$-409]mmmm d, yyyy;@ => mmmm d, yyyy}
            int start = s2.IndexOf('[');
            int end = s2.IndexOf(']');
            if (start > -1 && end > -1)
                s2 = s2.Remove(start, (end - start) + 1);
            s2 = s2.Replace("@", "");

            s1 = GetValueFromArg(s1);
            double d;
            DateTime date;
            if (double.TryParse(s1, NumberStyles.Any, null, out d))
            {
                s1 = d.ToString(s2);
                if (s1 == s2)
                {
                    s2 = s2.Replace('m', 'M').Replace('Y', 'y').Replace('D', 'd');
#if !WinRT
                    DateTime dt = DateTime.FromOADate(d);
#else
                    DateTime dt = new DateTime((long)d);
#endif
                    s1 = dt.ToString(s2);
                }
            }
            else if (DateTime.TryParse(s1, out date))
            {
                string format = range.Substring(length + 1).Replace(TIC, "");
                format = format.Replace('m', 'M').Replace('Y', 'y').Replace('D', 'd');
                s1 = date.ToString(format);
            }
            return TIC + s1 + TIC;
        }

        /// <summary>
        /// Returns a number.
        /// </summary>
        /// <param name="range">A date or number string.</param>
        /// <returns>A number.</returns>
        public string ComputeValue(string range)
        {
            double d;
            if (!double.TryParse(range.Replace(TIC, ""), NumberStyles.Any, null, out d))
            {
                try
                {
                    DateTime dt = DateTime.Parse(range.Replace(TIC, ""));
#if !WinRT
                    d = dt.ToOADate();
#else
                    d = dt.Ticks;
#endif
                }
                catch { }
            }
            return d.ToString();
        }

        /// <summary>
        /// Returns the remainder after dividing one number by another.
        /// </summary>
        /// <param name="range">Two numbers in a list.</param>
        /// <returns>The remainder.</returns>
        public string ComputeMod(string range)
        {
            string s1 = range.Split(new char[] { ParseArgumentSeparator })[0];
            string s2 = range.Split(new char[] { ParseArgumentSeparator })[1];
            s1 = GetValueFromArg(s1);
            s2 = GetValueFromArg(s2);
            int i = int.Parse(s2);
            return (Math.Sign(i) * Math.Abs((int.Parse(s1) % i))).ToString();
        }

        /// <summary>
        /// Returns the integer value.
        /// </summary>
        /// <param name="range">Number to be truncated.</param>
        /// <returns>An integer.</returns>
        public string ComputeInt(string range)
        {
            range = GetValueFromArg(range);
            double d;
            if (double.TryParse(range, NumberStyles.Any, null, out d))
            {
                double d1 = (d < 0) ? -1 : 1;
                double d2 = (d < 0) ? 1 : 0;
                return (d1 * Math.Floor(d2 + Math.Abs(d))).ToString("F0");
            }
            return "0";
        }

        /// <summary>
        /// Truncates a number to an integer.
        /// </summary>
        /// <param name="range">Value and number of digits.</param>
        /// <returns>Truncated value.</returns>
        public string ComputeTrunc(string range)
        {

            string[] args = range.Split(new char[] { ParseArgumentSeparator });
            int argCount = args.GetLength(0);
            double digits = 0;

            if (argCount == 2)
            {
                //ignore return value...
                double.TryParse(GetValueFromArg(args[1]), NumberStyles.Integer, null, out digits);
            }
            range = GetValueFromArg(args[0]);

            double d;
            if (double.TryParse(range, NumberStyles.Any, null, out d))
            {
                string format = digits == 0 ? "F0" : ("0." + new string('0', (int)digits));
                double normalizer = Math.Pow(10, digits);
                double d1 = (d < 0) ? -1 : 1;
                return (d1 * Math.Floor(normalizer * Math.Abs(d)) / normalizer).ToString(format);
            }

            return "0";
        }


        /// <summary>
        /// Returns the sum of the products of corresponding values.
        /// </summary>
        /// <param name="range">Two cell ranges.</param>
        /// <returns>Sum of the products.</returns>
        public string ComputeSumProduct(string range)
        {

            double sum = 0;
            int count = 0;
            double d;
            double[] vector = null;

            foreach (string r in range.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1)
                {
                    int i = r.IndexOf(":");
                    int row1 = RowIndex(r.Substring(0, i));
                    int col1 = ColIndex(r.Substring(0, i));
                    int row2 = RowIndex(r.Substring(i + 1));
                    int col2 = ColIndex(r.Substring(i + 1));

                    if (vector == null)
                    {
                        count = (row2 - row1 + 1) * (col2 - col1 + 1);
                        vector = new double[count];
                        for (i = 0; i < count; ++i)
                        {
                            vector[i] = 1d;
                        }
                    }


                    int k = 0;
                    string s1;
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        try
                        {
                            s1 = GetValueFromArg(s);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                        if (s1.Length > 0)
                        {
                            if (double.TryParse(s1, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                            {
                                vector[k] = vector[k] * d;

                            }
                        }
                        k++;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < count; ++i)
                sum += vector[i];

            return sum.ToString();
        }

        /// <summary>
        /// Returns True is the string denotes an error.
        /// </summary>
        /// <param name="range">String to be tested.</param>
        /// <returns>True if the value is an error.</returns>
        public string ComputeIsError(string range)
        {
            if (range.StartsWith(TIC.ToString()))
                return FALSEVALUESTR;

            try
            {
                string arg = range;

                if (!GridCellFormulaModel.IsEmpty(range) && range[0] == BMARKER)
                {
                    range = range.Replace(BMARKER, ' ');
                    range = range.Trim();
                    if (range.StartsWith("NAN")
                || range.StartsWith("-NAN")
                || range.StartsWith("INFINITY")
                || range.StartsWith("-INFINITY")
                || range.StartsWith("#")
                || range.StartsWith("n#")
                )
                        return TRUEVALUESTR;
                }

                range = arg;
                range = GetValueFromArg(range).ToUpper().Replace(TIC, "");
            }
            catch
            {
                range = range.ToUpper();
            }
            if (range.StartsWith("NAN")
                || range.StartsWith("-NAN")
                || range.StartsWith("INFINITY")
                || range.StartsWith("-INFINITY")
                || range.StartsWith("#")
                || range.StartsWith("n#")
                )
                return TRUEVALUESTR;
            else
                return FALSEVALUESTR;
        }

        /// <summary>
        /// Returns a value you specify if a formula evaluates to an error
        /// otherwise, returns the result of the formula.
        /// </summary>
        /// <param name="args">String to be tested.</param>
        public string ComputeIfError(string args)
        {
            string[] argsArray = args.Split(new char[] { ParseArgumentSeparator });
            if (argsArray.Length != 2)
                return FormulaErrorStrings[wrong_number_arguments];
            string range = argsArray[0];

            if (range.StartsWith(TIC.ToString()))
                return GetValueFromArg(argsArray[0]);
            try
            {
                if (!GridCellFormulaModel.IsEmpty(range) && range[0] == BMARKER)
                {
                    range = range.Replace(BMARKER, ' ');
                    range = range.Trim();
                    if (range.StartsWith("NAN")
                || range.StartsWith("-NAN")
                || range.StartsWith("INFINITY")
                || range.StartsWith("-INFINITY")
                || range.StartsWith("#")
                || range.StartsWith("n#")
                )
                        return GetValueFromArg(argsArray[1]);
                }

                range = argsArray[0];

                range = GetValueFromArg(range).ToUpper().Replace(TIC, "");
            }
            catch
            {
                range = range.ToUpper();
            }
            if (range.StartsWith("NAN")
                || range.StartsWith("-NAN")
                || range.StartsWith("INFINITY")
                || range.StartsWith("-INFINITY")
                || range.StartsWith("#")
                || range.StartsWith("n#")
                )
                return GetValueFromArg(argsArray[1]);
            else
                return GetValueFromArg(argsArray[0]);
        }

        /// <summary>
        /// Determines whether the string contains a number or not.
        /// </summary>
        /// <param name="range">String to be tested.</param>
        /// <returns>True if the string is a number.</returns>
        public string ComputeIsNumber(string range)
        {
            double d;
            try
            {
                range = GetValueFromArg(range);
            }
            catch
            {
            }
            if (double.TryParse(range, NumberStyles.Any, null, out d))
                return TRUEVALUESTR;
            else
                return FALSEVALUESTR;
        }

        /// <summary>
        ///Returns the logical value False. 
        /// </summary>
        /// <param name="empty">Empty string.</param>
        /// <returns>Logical False value string.</returns>
        public string ComputeFalse(string empty)
        {
            return FALSEVALUESTR;
        }

        /// <summary>
        /// Returns the logical value True.
        /// </summary>
        /// <returns>Logical True value string.</returns>
        public string ComputeTrue(string empty)
        {
            return TRUEVALUESTR;
        }

        /// <summary>
        /// Converts a number to text using currency format.
        /// </summary>
        /// <param name="args">Number and the number of digits.</param>
        /// <returns>Currency format string.</returns>
        public string ComputeDollar(string args)
        {
            string[] argsArray = args.Split(new char[] { ParseArgumentSeparator });
            string s1 = argsArray[0];
            string s2 = "2";
            if (argsArray.GetLength(0) == 2)
                s2 = argsArray[1];

            s1 = GetValueFromArg(s1);
            s2 = GetValueFromArg(s2);

            double number, decimals;

            double.TryParse(s1, NumberStyles.Any, null, out number);
            if (!double.TryParse(s2, NumberStyles.Any, null, out decimals))
                decimals = 2;

            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            nfi.CurrencyDecimalDigits = (int)decimals;

            return string.Format(nfi, "{0:C}", number);
        }

        /// <summary>
        /// Rounds a number to the specified number of decimals, formats the number
        /// in decimal format using a period and commas, and return the result as text.
        /// </summary>
        /// <param name="args">
        /// Number, number of digits, a flag that prevents from include
        /// commas in the returned text.
        /// </param>
        /// <returns>Formatted number as string.</returns>
        public string ComputeFixed(string args)
        {
            string[] argsArray = args.Split(new char[] { ParseArgumentSeparator });
            string s1 = argsArray[0];
            string s2 = "2";
            string s3 = "FALSE";

            int argCount = argsArray.GetLength(0);
            if (argCount > 1)
                s2 = argsArray[1];
            if (argCount > 2)
                s3 = argsArray[2];

            s1 = GetValueFromArg(s1);
            s2 = GetValueFromArg(s2);
            s3 = GetValueFromArg(s3);

            double number, decimals;
            double.TryParse(s1, NumberStyles.Any, null, out number);
            if (!double.TryParse(s2, NumberStyles.Any, null, out decimals))
                decimals = 2;

            double no_commas_flag;
            bool no_commas;
            if (double.TryParse(s3, NumberStyles.Any, null, out no_commas_flag))
            {
                if (no_commas_flag == 0)
                    no_commas = false;
                else
                    no_commas = true;
            }
            else
            {
                if (s3.ToUpper() == FALSEVALUESTR)
                    no_commas = false;
                else
                    if (s3.ToUpper() == TRUEVALUESTR)
                        no_commas = true;
                    else
                        return "#NAME?";
            }


            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            nfi.NumberDecimalDigits = (int)decimals;
            if (no_commas)
                nfi.NumberGroupSeparator = "";

            //Added UNIQUESTRINGMARKER to avoid parsing in GetFormattedText().
            return string.Format(nfi, "{0}{1:N}", UNIQUESTRINGMARKER, number);
            //return string.Format(nfi, "{0:N}", number); 
        }

        /// <summary>
        /// Converts text to lowercase.
        /// </summary>
        /// <param name="args">Value to convert.</param>
        /// <returns>Converted string.</returns>
        public string ComputeLower(string args)
        {
            return GetValueFromArg(args).ToLower();
        }

        /// <summary>
        /// Converts text to uppercase.
        /// </summary>
        /// <param name="args">Value to convert.</param>
        /// <returns>Converted string.</returns>
        public string ComputeUpper(string args)
        {
            return GetValueFromArg(args).ToUpper();
        }

        /// <summary>
        /// Changes full-width characters to half-width characters.
        /// </summary>
        /// <param name="args">Value to convert.</param>
        /// <returns>Converted string.</returns>
        public string ComputeAsc(string args)
        {
            string s = GetValueFromArg(args);
            s = s.Replace(TIC, string.Empty);
            Encoding unicode = Encoding.UTF8;
            byte[] byteArray = Encoding.Unicode.GetBytes(s);
            byte[] asciiArray = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, byteArray);
            string finalString = unicode.GetString(asciiArray, 0, asciiArray.Length);
            return finalString;
        }

        public string ComputeT(string args)
        {
            string s = GetValueFromArg(args);
            string pattern = @"[a-zA-Z!#$%&'()*+,/:;<=>?@\^_`{|}~-]";
            if (Regex.IsMatch(s, pattern))
            {
                //Console.WriteLine("String");
                return s;
            }
            else
            {
                //Console.WriteLine("Digit");
                return string.Empty;
            }
        }

        public string ComputeColumn(string args)
        {
            int col = ColIndex(args);
            return col.ToString();
        }

        public string ComputeN(string args)
        {
            string s = GetValueFromArg(args);
            int integerValue;
            double doubleValue;
            DateTime dateTimeValue;
            bool booleanValue;
            if (int.TryParse(s, out integerValue))
            {
                return integerValue.ToString();
            }
            else if (double.TryParse(s, out doubleValue))
            {
                return doubleValue.ToString();
            }
            else if (DateTime.TryParse(s, out dateTimeValue))
            {
                integerValue = GetSerialDateFromDate(dateTimeValue.Year, dateTimeValue.Month, dateTimeValue.Day);
                return integerValue.ToString();
            }
            else if (bool.TryParse(s, out booleanValue))
            {
                if (booleanValue)
                    return "1";
                else
                    return "0";
            }
            return "0";
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters.
        /// </summary>
        /// <param name="args">Value to trim.</param>
        /// <returns>
        /// The string that remains after all leading and trailing white-space characters
        /// were removed.
        ///</returns>
        public string ComputeTrim(string args)
        {
            string s = GetValueFromArg(args).Trim(new char[] { '\"', ' ' });
            int len = 0;
            //strip out interior double, triple, etc spaces...
            while (s.Length != len)
            {
                len = s.Length;
                s = s.Replace("  ", " ");
            }
            return s;
        }

        /// <summary>
        /// Determines whether the value is a logical value.
        /// </summary>
        /// <param name="args">Value to be tested.</param>
        /// <returns>True if the value is a logical value, False otherwise.</returns>
        public string ComputeIsLogical(string args)
        {
            string value = args.ToUpper();
            try
            {
                value = GetValueFromArg(args).ToUpper();
            }
            catch
            {
                return FALSEVALUESTR;
            }

            if (value == FALSEVALUESTR || value == TRUEVALUESTR
                || args.ToUpper() == FALSEVALUESTR || args.ToUpper() == TRUEVALUESTR)
                return TRUEVALUESTR;

            return FALSEVALUESTR;
        }

        /// <summary>
        /// Determines whether the value is the #NA error value.
        /// </summary>
        /// <param name="args">Value to be tested.</param>
        /// <returns>True if the value is the #NA error value, False otherwise.</returns>
        public string ComputeIsNA(string args)
        {
            if (args.StartsWith(TIC.ToString()))
                return FALSEVALUESTR;

            try
            {
                args = GetValueFromArg(args).ToUpper();
            }
            catch
            {
                args = args.ToUpper();
            }
            if (args.StartsWith("#N/A"))
                return TRUEVALUESTR;

            return FALSEVALUESTR;
        }

        /// <summary>
        /// Returns True is the string denotes an error except #N/A.
        /// </summary>
        /// <param name="range">Value to be tested.</param>
        /// <returns>True if the value is an error except #N/A, false otherwise.</returns>
        public string ComputeIsErr(string range)
        {
            if (range.StartsWith(TIC.ToString()))
                return FALSEVALUESTR;

            try
            {
                range = GetValueFromArg(range).ToUpper().Replace(TIC, "");
            }
            catch
            {
                range = range.ToUpper();
            }
            if ((range.StartsWith("NAN")
                || range.StartsWith("-NAN")
                || range.StartsWith("INFINITY")
                || range.StartsWith("-INFINITY")
                || range.StartsWith("#")
                || range.StartsWith("n#")
                ) && !range.StartsWith("#N/A"))
                return TRUEVALUESTR;
            else
                return FALSEVALUESTR;
        }

        /// <summary>
        /// Determines whether the value is empty string.
        /// </summary>
        /// <param name="args">Value to be tested.</param>
        /// <returns>True if the value is empty, False otherwise.</returns>
        public string ComputeIsBlank(string args)
        {
            if (args.Length == 0)
                throw new ArgumentException("Argument cant be empty.");
            try
            {
                if (GetValueFromArg(args).Equals(string.Empty))
                    return TRUEVALUESTR;
            }
            catch
            {
                return FALSEVALUESTR;
            }

            return FALSEVALUESTR;
        }

        /// <summary>
        /// Determines whether the value is string or not.
        /// </summary>
        /// <param name="args">Value to be tested.</param>
        /// <returns>True if the value is a string, false otherwise.</returns>
        public string ComputeIsText(string args)
        {
            string s;
            double d;
            try
            {
                s = GetValueFromArg(args);
            }
            catch
            {
                return FALSEVALUESTR;
            }
            if (s.Length > 0 && !double.TryParse(s, NumberStyles.Any, null, out d) &&
                (s.ToUpper() != FALSEVALUESTR && s.ToUpper() != TRUEVALUESTR) &&
                !(s.StartsWith("NAN")
                || s.StartsWith("-NAN")
                || s.StartsWith("INFINITY")
                || s.StartsWith("-INFINITY")
                || s.StartsWith("#")
                || s.StartsWith("n#")
                ))
            {
                return TRUEVALUESTR;
            }
            else
            {
                return FALSEVALUESTR;
            }
            ////UpdateDependencies
            //if(IsCellReference(args))
            //    GetValueFromArg(args);
            //if (IsFormulaResultString(args, cell))
            //    return TRUEVALUESTR;
            //else
            //    return FALSEVALUESTR;
        }

        /// <summary>
        /// Determines whether the value is not a string.
        /// </summary>
        /// <param name="args">Value to be tested.</param>
        /// <returns>True if the value is not a string, false otherwise.</returns>
        public string ComputeIsNonText(string args)
        {
            if (ComputeIsText(args) == TRUEVALUESTR)
                return FALSEVALUESTR;
            else
                return TRUEVALUESTR;
        }


        //New functions end

        #region Methods for string function(ISTEXT/ISNONTEXT)
        /// <summary>
        /// Determines whether the arg is a valid cell name.
        /// </summary>
        /// <param name="args">Cell name.</param>
        /// <returns>True is the arg is a valid cell name, false otherwise.</returns>
        bool IsCellReference(string args)
        {

            PutTokensForSheets(ref args);
            string sheetToken = SheetToken(args);
            if (!String.IsNullOrEmpty(sheetToken))
                args = args.Replace(sheetToken, "");
            bool isAlpha = false, isNum = false;
            if (args.IndexOf(':') != args.LastIndexOf(':'))
                return false;
            foreach (char c in args.ToCharArray())
            {
                if (char.IsLetter(c))
                {
                    if (isNum)
                        return false;
                    isAlpha = true;
                }
                if (char.IsDigit(c))
                {
                    if (!isAlpha)
                        return false;
                    isNum = true;
                }

                if (char.Equals(c, ':'))
                {
                    isAlpha = false;
                    isNum = false;
                }
            }

            if (isAlpha && isNum)
                return true;

            return false;
        }
        /// <summary>
        /// Determines whether the computed value is a valid cell name.
        /// </summary>
        /// <param name="formula">Formula to </param>
        /// <param name="cell"></param>
        /// <returns>True if the computed value is string type, false otherwise.</returns>
        bool IsFormulaResultString(string formula, string cell)
        {
            int row = RowIndex(cell), col = ColIndex(cell);
            GridStyleInfo style = grid[row, col];
            if (style.CellValueType == typeof(string))
                return true;

            if (style.CellType == "FormulaCell")
            {
                string innerFormula = cell.Equals(this.cell) ? "=ISTEXT(" + formula + ")" : grid[row, col].Text;
                if (!(innerFormula.IndexOf(formulaChar) < 0 || innerFormula.Equals(formula))
                    && IsCellReference(formula))
                {
                    string pf, err, compVal;
                    if (IsFormulaValid(formula, out pf, out err, out compVal))
                    {
                        if (compVal.EndsWith("\""))
                            return true;
                        if (formula.Equals(pf) && !formula.Equals(compVal))
                        {

                            row = RowIndex(pf);
                            col = ColIndex(pf);
                            innerFormula = grid[row, col].Text;
                            if (innerFormula != string.Empty && !innerFormula.Equals(compVal))
                            {
                                if (!RecurciveFormulaCheck(innerFormula, cell))
                                    return false;
                                else
                                    return true;
                            }
                        }
                        cell = GridRangeInfo.GetAlphaLabel(col) + row.ToString();
                        style = grid[row, col];
                    }

                }
            }

            if (((style.CellType == "TextBox" || style.CellType == "RichText"
                || style.CellType == "OriginalTextBox") &&
                style.CellValueType == null) || style.CellValueType == typeof(string))
                return true;
            else if (style.CellValueType == null && style.CellType == "FormulaCell")
            {
                string val = formula;
                if (cell.Equals(this.cell))
                {
                    if (!formula.EndsWith("\""))
                        return false;
                }
                else
                    val = GetValueFromArg(cell);
                if (val == string.Empty)
                    return false;
                if (val.EndsWith("\""))
                    return true;
                double d;
                if (!double.TryParse(val, NumberStyles.Any, null, out d))
                {
                    if (!(val.StartsWith("NAN")
                || val.StartsWith("-NAN")
                || val.StartsWith("INFINITY")
                || val.StartsWith("-INFINITY")
                || val.StartsWith("#")
                || val.StartsWith("n#")
                ))
                        return true;
                }
            }



            return false;
        }
        /// <summary>
        /// Determines whether the computed value is a valid cell name 
        /// by checking all it dependent cells.
        /// </summary>
        bool RecurciveFormulaCheck(string formula, string cell)
        {
            int row = RowIndex(cell), col = ColIndex(cell);
            GridStyleInfo style = grid[row, col];
            //string formula = style.Text;
            if (formula.Length == 0)
            {
                if (style.CellValueType == null || style.CellValueType == typeof(string))
                    return true;
                else
                    return false;
            }

            //Check if the last function returns string type value.
            int rightParens = formula.IndexOf(')');
            if (rightParens == formula.Length - 1)
            {
                int firstLeftParens = formula.IndexOf('(');
                if (formula.StartsWith("="))
                    formula = formula.Remove(0, 1);
                string funcName = formula.Substring(0, firstLeftParens - 1).ToUpper().Trim();
                //check for all functions that returns Text.
                foreach (string strFunction in textFunctions)
                {
                    if (funcName.Equals(strFunction))
                        return true;
                }
                return false;

            }
            if (formula.IndexOf('(') > 0)
                return false;
            Hashtable depFormulaCells = (Hashtable)DependentFormulaCells[cell];
            if (depFormulaCells == null)
            {
                if (IsFormulaResultString(formula, cell))
                    return true;
                else
                    return false;
            }


            if (depFormulaCells.Count > 0)
            {
                foreach (var entry in depFormulaCells)
                    cell = (string)entry.Value;
                formula = grid[RowIndex(cell), ColIndex(cell)].Text;
                if (formula.IndexOf(formulaChar) == 0)
                    formula = formula.Remove(0, 1);
                if (IsFormulaResultString(formula, cell))
                    return true;
            }

            return false;
        }
        #endregion

        private static bool useCommonLibraryFunction = false;
        /// <summary>
        /// Gets or sets whether you want all grids to share the same collection of library functions.
        /// </summary>
        public static bool UseCommonLibrary
        {
            get { return useCommonLibraryFunction; }
            set { useCommonLibraryFunction = value; }
        }

#if WPF
        System.Collections.Specialized.StringCollection textFunctions;
#else
        List<string> textFunctions;
#endif
        /// <summary>
        /// Creates and initially loads the Function Library with the supported functions.
        /// </summary>
        public virtual void InitLibraryFunctions()
        {
            libraryFunctions = new Hashtable();

            bool save = UseCommonLibrary;
            UseCommonLibrary = false;

            //hand coded forumlas
            AddFunction("Sum", new LibraryFunction(ComputeSum));
            AddFunction("Avg", new LibraryFunction(ComputeAvg));
            AddFunction("Max", new LibraryFunction(ComputeMax));
            AddFunction("Min", new LibraryFunction(ComputeMin));
            AddFunction("Pi", new LibraryFunction(ComputePI));
            AddFunction("Sign", new LibraryFunction(ComputeSign));
            AddFunction("Pow", new LibraryFunction(ComputePow)); //for compatibility
            AddFunction("Power", new LibraryFunction(ComputePow));

            //logical functions
            AddFunction("And", new LibraryFunction(ComputeAnd));
            AddFunction("Or", new LibraryFunction(ComputeOr));
            AddFunction("If", new LibraryFunction(ComputeIf));
            AddFunction("Not", new LibraryFunction(ComputeNot));
            AddFunction("False", new LibraryFunction(ComputeFalse));
            AddFunction("True", new LibraryFunction(ComputeTrue));
            AddFunction("Offset", new LibraryFunction(ComputeOffSet));
            AddFunction("IfError", new LibraryFunction(ComputeIfError));

            //uses ComputeMath delegate (System.Math)
            AddFunction("Acos", new LibraryFunction(ComputeAcos));
            AddFunction("Asin", new LibraryFunction(ComputeAsin));
            AddFunction("Atan", new LibraryFunction(ComputeAtan));
            AddFunction("Cos", new LibraryFunction(ComputeCos));
            AddFunction("Sin", new LibraryFunction(ComputeSin));
            AddFunction("Cosh", new LibraryFunction(ComputeCosh));
            AddFunction("Sinh", new LibraryFunction(ComputeSinh));
            AddFunction("Tanh", new LibraryFunction(ComputeTanh));
            AddFunction("Round", new LibraryFunction(ComputeRound));
            AddFunction("Ceiling", new LibraryFunction(ComputeCeiling));
            AddFunction("Floor", new LibraryFunction(ComputeFloor));
            AddFunction("Log", new LibraryFunction(ComputeLog));
            AddFunction("Log10", new LibraryFunction(ComputeLog10));
            AddFunction("Exp", new LibraryFunction(ComputeExp));
            AddFunction("Sqrt", new LibraryFunction(ComputeSqrt));
            AddFunction("Abs", new LibraryFunction(ComputeAbs));
            AddFunction("Tan", new LibraryFunction(ComputeTan));

            //added...
            AddFunction("Rand", new LibraryFunction(ComputeRand));
            AddFunction("SumIf", new LibraryFunction(ComputeSumIf));

            //missing formulas
            AddFunction("HLookUp", new LibraryFunction(ComputeHLookUp));
            AddFunction("VLookUp", new LibraryFunction(ComputeVLookUp));
            AddFunction("Left", new LibraryFunction(ComputeLeft));
            AddFunction("Len", new LibraryFunction(ComputeLen));
            AddFunction("Mid", new LibraryFunction(ComputeMid));
            AddFunction("Right", new LibraryFunction(ComputeRight));
            AddFunction("Product", new LibraryFunction(ComputeProduct));
            AddFunction("Value", new LibraryFunction(ComputeValue));
            AddFunction("Mod", new LibraryFunction(ComputeMod));
            AddFunction("Trunc", new LibraryFunction(ComputeTrunc));
            AddFunction("SumProduct", new LibraryFunction(ComputeSumProduct));
            AddFunction("Average", new LibraryFunction(ComputeAvg));
            AddFunction("Int", new LibraryFunction(ComputeInt));
            AddFunction("Match", new LibraryFunction(ComputeMatch));

            //information functions
            AddFunction("IsError", new LibraryFunction(ComputeIsError));
            AddFunction("IsNumber", new LibraryFunction(ComputeIsNumber));
            AddFunction("IsLogical", new LibraryFunction(ComputeIsLogical));
            AddFunction("IsNA", new LibraryFunction(ComputeIsNA));
            AddFunction("IsErr", new LibraryFunction(ComputeIsErr));
            AddFunction("IsBlank", new LibraryFunction(ComputeIsBlank));
            AddFunction("IsText", new LibraryFunction(ComputeIsText));
            AddFunction("IsNonText", new LibraryFunction(ComputeIsNonText));

            //string and data functions
            AddFunction("Dollar", new LibraryFunction(ComputeDollar));
            AddFunction("Fixed", new LibraryFunction(ComputeFixed));
            AddFunction("Lower", new LibraryFunction(ComputeLower));
            AddFunction("Upper", new LibraryFunction(ComputeUpper));
            AddFunction("Trim", new LibraryFunction(ComputeTrim));
            AddFunction("Text", new LibraryFunction(ComputeText));
            AddFunction("Concatenate", new LibraryFunction(ComputeConcatenate));
            AddFunction("Substitute", new LibraryFunction(ComputeSubstitute));
            AddFunction("T", new LibraryFunction(ComputeT));
            AddFunction("Column", new LibraryFunction(ComputeColumn));
            AddFunction("N", new LibraryFunction(ComputeN));
            AddFunction("Asc", new LibraryFunction(ComputeAsc));

            //storing string functions in array for ISTEXT/ISNONTEXT function to check
#if WPF
            textFunctions = new System.Collections.Specialized.StringCollection();
#else
            textFunctions = new List<string>();
#endif
            textFunctions.AddRange(new string[]{ "DOLLAR", "FIXED", "LOWER", "UPPER", "TRIM",
                "TEXT", "CONCATENATE", "SUBSTITUTE"});

            AddFunction("Indirect", new LibraryFunction(ComputeIndirect));

            //financial
            // http://support.microsoft.com/default.aspx?scid=kb;en-us;123757
            AddFunction("Db", new LibraryFunction(ComputeDb));
            AddFunction("Ddb", new LibraryFunction(ComputeDdb));
            AddFunction("Fv", new LibraryFunction(ComputeFv));
            AddFunction("Ipmt", new LibraryFunction(ComputeIpmt));
            AddFunction("Irr", new LibraryFunction(ComputeIrr));
            AddFunction("Xirr", new LibraryFunction(ComputeXirr));
            AddFunction("Ispmt", new LibraryFunction(ComputeIspmt));
            AddFunction("Mirr", new LibraryFunction(ComputeMirr));
            AddFunction("Nper", new LibraryFunction(ComputeNper));
            AddFunction("Npv", new LibraryFunction(ComputeNpv));
            AddFunction("Pmt", new LibraryFunction(ComputePmt));
            AddFunction("Ppmt", new LibraryFunction(ComputePpmt));
            AddFunction("Pv", new LibraryFunction(ComputePv));
            AddFunction("Rate", new LibraryFunction(ComputeRate));
            AddFunction("Sln", new LibraryFunction(ComputeSln));
            AddFunction("Syd", new LibraryFunction(ComputeSyd));
            AddFunction("Vdb", new LibraryFunction(ComputeVdb));

            //new functions
            //> asinh(x) = sgn(x) ln(|x| + sqrt(x^2 + 1))     all x
            //> acosh(x) = ln(x + sqrt(x^2-1))                x >= 1
            //> atanh(x) = 1/2 sgn(x) ln((1+|x|)/(1-|x|))     |x| < 1

            AddFunction("Rand", new LibraryFunction(ComputeRand));
            AddFunction("Acosh", new LibraryFunction(ComputeAcosh));
            AddFunction("Asinh", new LibraryFunction(ComputeAsinh));
            AddFunction("Atanh", new LibraryFunction(ComputeAtanh));
            AddFunction("Atan2", new LibraryFunction(ComputeAtan2));
            AddFunction("Combin", new LibraryFunction(ComputeCombin));
            AddFunction("Degrees", new LibraryFunction(ComputeDegrees));
            AddFunction("Even", new LibraryFunction(ComputeEven));
            AddFunction("Fact", new LibraryFunction(ComputeFact));
            AddFunction("Ln", new LibraryFunction(ComputeLn));
            AddFunction("Odd", new LibraryFunction(ComputeOdd));
            AddFunction("Radians", new LibraryFunction(ComputeRadians));
            AddFunction("Round", new LibraryFunction(ComputeRound));
            AddFunction("Rounddown", new LibraryFunction(ComputeRounddown));
            AddFunction("Roundup", new LibraryFunction(ComputeRoundup));

            //needs strings AddFunction("Sumif", new LibraryFunction(ComputeSumif));
            AddFunction("Sumsq", new LibraryFunction(ComputeSumsq));
            AddFunction("Sumx2my2", new LibraryFunction(ComputeSumx2my2));
            AddFunction("Sumx2py2", new LibraryFunction(ComputeSumx2py2));
            AddFunction("Sumxmy2", new LibraryFunction(ComputeSumxmy2));

            AddFunction("Date", new LibraryFunction(ComputeDate));
            AddFunction("Datevalue", new LibraryFunction(ComputeDatevalue));
            AddFunction("Day", new LibraryFunction(ComputeDay));
            AddFunction("Days360", new LibraryFunction(ComputeDays360));
            AddFunction("Hour", new LibraryFunction(ComputeHour));
            AddFunction("Minute", new LibraryFunction(ComputeMinute));
            AddFunction("Second", new LibraryFunction(ComputeSecond));
            AddFunction("Month", new LibraryFunction(ComputeMonth));
            AddFunction("Now", new LibraryFunction(ComputeNow));
            AddFunction("Time", new LibraryFunction(ComputeTime));
            AddFunction("Timevalue", new LibraryFunction(ComputeTimevalue));
            AddFunction("Today", new LibraryFunction(ComputeToday));
            AddFunction("Weekday", new LibraryFunction(ComputeWeekday));
            AddFunction("Year", new LibraryFunction(ComputeYear));


            //stats
            AddFunction("Avedev", new LibraryFunction(ComputeAvedev));
            AddFunction("Averagea", new LibraryFunction(ComputeAveragea));
            AddFunction("Gammaln", new LibraryFunction(ComputeGammaln));
            AddFunction("Gammadist", new LibraryFunction(ComputeGammadist));
            AddFunction("Gammainv", new LibraryFunction(ComputeGammainv));
            AddFunction("Geomean", new LibraryFunction(ComputeGeomean));
            AddFunction("Harmean", new LibraryFunction(ComputeHarmean));
            AddFunction("Hypgeomdist", new LibraryFunction(ComputeHypgeomdist));
            AddFunction("Intercept", new LibraryFunction(ComputeIntercept));
            AddFunction("Binomdist", new LibraryFunction(ComputeBinomdist));
            AddFunction("Chidist", new LibraryFunction(ComputeChidist));
            AddFunction("Chiinv", new LibraryFunction(ComputeChiinv));
            AddFunction("Chitest", new LibraryFunction(ComputeChitest));
            AddFunction("Normdist", new LibraryFunction(ComputeNormdist));
            AddFunction("Norminv", new LibraryFunction(ComputeNorminv));
            AddFunction("NormsInv", new LibraryFunction(ComputeNormsInv));
            AddFunction("NormsDist", new LibraryFunction(ComputeNormsDist));
            AddFunction("Confidence", new LibraryFunction(ComputeConfidence));
            AddFunction("Correl", new LibraryFunction(ComputeCorrel));
            AddFunction("Count", new LibraryFunction(ComputeCount));
            AddFunction("Counta", new LibraryFunction(ComputeCounta));
            AddFunction("Countblank", new LibraryFunction(ComputeCountblank));
            AddFunction("Countif", new LibraryFunction(ComputeCountif));
            AddFunction("Covar", new LibraryFunction(ComputeCovar));
            AddFunction("CritBinom", new LibraryFunction(ComputeCritbinom));
            AddFunction("Devsq", new LibraryFunction(ComputeDevsq));
            AddFunction("Expondist", new LibraryFunction(ComputeExpondist));
            AddFunction("Fdist", new LibraryFunction(ComputeFdist));
            AddFunction("Finv", new LibraryFunction(ComputeFinv));
            AddFunction("Fisher", new LibraryFunction(ComputeFisher));
            AddFunction("Fisherinv", new LibraryFunction(ComputeFisherinv));
            AddFunction("Forecast", new LibraryFunction(ComputeForecast));
            AddFunction("Kurt", new LibraryFunction(ComputeKurt));
            AddFunction("Large", new LibraryFunction(ComputeLarge));
            AddFunction("Lognormdist", new LibraryFunction(ComputeLognormdist));
            AddFunction("Loginv", new LibraryFunction(ComputeLoginv));
            AddFunction("Maxa", new LibraryFunction(ComputeMaxa));
            AddFunction("Median", new LibraryFunction(ComputeMedian));
            AddFunction("Mina", new LibraryFunction(ComputeMina));
            AddFunction("Mode", new LibraryFunction(ComputeMode));
            AddFunction("Negbinomdist", new LibraryFunction(ComputeNegbinomdist));
            AddFunction("Pearson", new LibraryFunction(ComputePearson));
            AddFunction("Percentile", new LibraryFunction(ComputePercentile));
            AddFunction("Percentrank", new LibraryFunction(ComputePercentrank));
            AddFunction("Permut", new LibraryFunction(ComputePermut));
            AddFunction("Poisson", new LibraryFunction(ComputePoisson));
            AddFunction("Prob", new LibraryFunction(ComputeProb));
            AddFunction("Quartile", new LibraryFunction(ComputeQuartile));
            AddFunction("Rank", new LibraryFunction(ComputeRank));
            AddFunction("Rsq", new LibraryFunction(ComputeRsq));
            AddFunction("Skew", new LibraryFunction(ComputeSkew));
            AddFunction("Slope", new LibraryFunction(ComputeSlope));
            AddFunction("Small", new LibraryFunction(ComputeSmall));
            AddFunction("Standardize", new LibraryFunction(ComputeStandardize));
            AddFunction("Stdev", new LibraryFunction(ComputeStdev));
            AddFunction("Stdeva", new LibraryFunction(ComputeStdeva));
            AddFunction("Stdevp", new LibraryFunction(ComputeStdevp));
            AddFunction("Stdevpa", new LibraryFunction(ComputeStdevpa));
            AddFunction("Steyx", new LibraryFunction(ComputeSteyx));
            AddFunction("Trimmean", new LibraryFunction(ComputeTrimmean));
            AddFunction("Var", new LibraryFunction(ComputeVar));
            AddFunction("Vara", new LibraryFunction(ComputeVara));
            AddFunction("Varp", new LibraryFunction(ComputeVarp));
            AddFunction("Varpa", new LibraryFunction(ComputeVarpa));
            AddFunction("Weibull", new LibraryFunction(ComputeWeibull));
            AddFunction("Ztest", new LibraryFunction(ComputeZtest));

            UseCommonLibrary = save;
        }

        #endregion

        #region Processing Arguments

        private string ZapQuotedColons(string s)
        {
            int start = s.IndexOf(TIC);
            while (start > -1)
            {
                int end = s.Substring(start + 1).IndexOf(TIC);
                if (end > -1)
                {
                    int i = s.Substring(start + 1).IndexOf(':');
                    while (i > -1 && i < end)
                    {
                        s = s.Substring(i + start) + " " + s.Substring(i + start + 1);
                        i = s.Substring(start + 1).IndexOf(':');
                    }
                }
                start = end;
            }
            return s;
        }

        private char MarkerChar = '`';

        private void MarkColonsInQuotes(ref string args)
        {
            bool inQuotes = false;
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == TIC[0])
                    inQuotes = !inQuotes;
                else if (args[i] == ':' && inQuotes)
                {
                    args = args.Replace(':', MarkerChar);
                }
            }
        }

        /// <summary>
        /// Accepts an argument string and returns a string array of cells.
        /// </summary>
        /// <remarks>
        ///	Converts arguments in these forms to a string array of individual cells.<para/>
        ///		A1,A2,B4,C1,...,D8<para/>
        ///		A1:A5<para/>
        ///		A1:C5<para/>
        /// </remarks>
        /// <param name="args">String containing a cell range.</param>
        /// <returns>String array of cells.</returns>
        public string[] GetCellsFromArgs(string args)
        {
            RemoveQuotesOnSingleRange(ref args);
            MarkColonsInQuotes(ref args);

            int row1;
            int col1;

            int i = args.IndexOf(':');
            if (i == -1)
            {
                args = args.Replace(MarkerChar, ':');
                i = args.IndexOf(ParseArgumentSeparator);
                if (i == -1)
                {
                    row1 = RowIndex(args);//maybe throw exception
                    col1 = ColIndex(args);//maybe throw exception
                    return new string[] { args };
                }
                else
                    return args.Split(new char[] { ParseArgumentSeparator });
            }

            //Support to recognize the A: H range notation
            string sheet = "";
            var argsCopy = args;
            int sheetTokenIndexOf = argsCopy.IndexOf(sheetToken);
            if (sheetTokenIndexOf > -1)
            {
                int sheetTokenIndexOf1 = argsCopy.IndexOf(sheetToken, sheetTokenIndexOf + 1);
                if (sheetTokenIndexOf1 > -1)
                {
                    sheet = argsCopy.Substring(sheetTokenIndexOf, sheetTokenIndexOf1 + 1);
                    argsCopy = argsCopy.Replace(sheet, "");
                }
            }
            if (!Regex.IsMatch(argsCopy, @"\d"))
            {
                int IndexOfsheetToken = args.IndexOf(sheetToken);
                if (IndexOfsheetToken > -1)
                {
                    argsCopy = args;
                    int j1 = argsCopy.IndexOf(sheetToken, IndexOfsheetToken + 1);
                    if (j1 > -1)
                    {
                        sheet = argsCopy.Substring(IndexOfsheetToken, j1 + 1);
                        argsCopy = argsCopy.Replace(sheet, "");
                    }
                    GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                    GridModel grd = grid;
                    grid = (GridModel)family.TokenToGridModel[sheet];
                    int index = args.IndexOf(':');
                    var temparg1 = args.Substring(0, index);
                    temparg1 = temparg1 + "1";
                    var temparg2 = args.Substring(index + 1, args.Length - index - 1);
                    temparg2 = temparg2 + grid.RowCount;
                    grid = grd;
                    args = temparg1 + ":" + temparg2;
                    i = args.IndexOf(':');
                }
                else
                {
                    int index = args.IndexOf(':');
                    var temparg1 = args.Substring(0, index);
                    temparg1 = temparg1 + "1";
                    var temparg2 = args.Substring(index + 1, args.Length - index - 1);
                    temparg2 = temparg2 + grid.RowCount;
                    args = temparg1 + ":" + temparg2;
                    i = args.IndexOf(':');
                }
            }

            sheet = "";
            int j = args.IndexOf(sheetToken);
            if (j > -1)
            {
                int j1 = args.IndexOf(sheetToken, j + 1);
                if (j1 > -1)
                {
                    sheet = args.Substring(j, j1 + 1);
                    args = args.Replace(sheet, "");
                    i = args.IndexOf(':');
                }
            }

            if (!args.Contains(sheetToken.ToString()))
            {
                row1 = RowIndex(args.Substring(0, i));
                col1 = ColIndex(args.Substring(0, i));
                int row2 = RowIndex(args.Substring(i + 1, args.Length - i - 1));
                int col2 = ColIndex(args.Substring(i + 1, args.Length - i - 1));

                if (row1 > row2)
                {
                    i = row2;
                    row2 = row1;
                    row1 = i;
                }

                if (col1 > col2)
                {
                    i = col2;
                    col2 = col1;
                    col1 = i;
                }

                int numCells = (row2 - row1 + 1) * (col2 - col1 + 1);
                string[] cells = new String[numCells];
                int k = 0;
                for (i = row1; i <= row2; ++i)
                    for (
                        j = col1;
                        j <= col2;
                        ++j)
                    {
                        try
                        {
                            cells[k++] = sheet + GridRangeInfo.GetAlphaLabel(j) + i.ToString();
                        }
                        catch (Exception ex)
                        {
#if WPF
                            TraceUtil.TraceExceptionCatched(ex);
#endif
                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                throw ex;
                            continue;
                        }
                    }
                return cells;
            }
            else
            {
                //args = ":!2!A1"
                string sheet1 = string.Empty;
                int jj = args.IndexOf(sheetToken);
                if (jj > -1)
                {
                    int J1 = args.IndexOf(sheetToken, jj + 1);
                    if (J1 > -1)
                    {
                        sheet1 = args.Substring(jj, J1);
                        args = args.Replace(sheet1, "");
                        i = args.IndexOf(':');
                    }
                }
                row1 = RowIndex(args.Substring(i + 1, args.Length - i - 1));
                col1 = ColIndex(args.Substring(i + 1, args.Length - i - 1));
                int from = int.Parse(sheet.Replace('!', ' '));
                int to = int.Parse(sheet1.Replace('!', ' '));
                if (to < from)
                {
                    var t = from;
                    from = to;
                    to = t;
                }
                int numCells = to - from;
                string[] cells = new String[numCells + 1];
                int k = 0;
                GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);
                if (family.sheetNamesSized != null)
                {
                    foreach (string name in family.sheetNamesSized)
                    {
                        string token = (string)family.SheetNameToToken[name];
                        int sheettoken = int.Parse(token.Replace('!', ' '));
                        if (sheettoken >= from && sheettoken <= to)
                        {
                            try
                            {
                                cells[k++] = token + GridRangeInfo.GetAlphaLabel(col1) + row1.ToString();
                            }
                            catch (Exception ex)
                            {
#if WPF
                                TraceUtil.TraceExceptionCatched(ex);
#endif
                                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                    throw ex;
                                continue;
                            }
                        }
                    }
                }
                return cells;
            }
        }

        private void RemoveQuotesOnSingleRange(ref string args)
        {
            if (args.Length > 2
                && args[0] == TIC[0]
                && args[args.Length - 1] == TIC[0]
                && args.IndexOf(":") > 0
                && args.IndexOf(ParseArgumentSeparator) == -1)
            {
                args = args.Substring(1, args.Length - 2);
            }
        }

        /// <summary>
        /// Conditionally gets either the formula value or the cell 
        /// value depending upon whether the requested cell is a FormulaCell.
        /// </summary>
        /// <param name="row">Row index of the requested cell.</param>
        /// <param name="col">Column index of the requested cell.</param>
        /// <returns>String holding either the cell value or the computed formula value.</returns>
        public string GetValueFromGrid(int row, int col)
        {
            //#if DEBUG
            //            if (Switches.FormulaCell.TraceVerbose)
            //                TraceUtil.TraceCurrentMethodInfo(row, col);
            //#else
            //            ;
            //#endif
            if (!grid.Data.ContainsKey(new RowColumnIndex(row, col)))
            {
                GridQueryDependentCellValueEventArgs e = new GridQueryDependentCellValueEventArgs(row, col);
                if (QueryDependentCellValue != null)
                {
                    QueryDependentCellValue(grid, e);
                    if (!e.Cancel)
                        return e.CellValue;
                }
            }
            GridStyleInfo style = grid[row, col];
            return GetValueFromGrid(style, row, col);
        }

        /// <summary>
        /// Conditionally gets either the formula value or the cell 
        /// value depending upon whether the requested cell is a FormulaCell.
        /// </summary>
        /// <param name="cell1">The alphanumeric cell label, like A1, or EE14.</param>
        /// <returns>String holding either the cell value or the computed formula value.</returns>
        public string GetValueFromGrid(string cell1)
        {
            if (cell1 == TRUEVALUESTR || cell1 == FALSEVALUESTR)
                return cell1;


            int i = cell1.LastIndexOf(sheetToken);
            int row, col;
            GridModel grd = grid;
            if (i > -1)
            {
                GridSheetFamilyItem family = GetSheetFamilyItem(this.grid);

                grid = (GridModel)family.TokenToGridModel[cell1.Substring(0, i + 1)];
                row = RowIndex(cell1);
                col = ColIndex(cell1);
            }
            else
            {
                row = RowIndex(cell1);
                col = ColIndex(cell1);
            }
            string saveCell = this.cell;
            this.cell = cell1;

            if (!grid.Data.ContainsKey(new RowColumnIndex(row, col)))
            {
                GridQueryDependentCellValueEventArgs e = new GridQueryDependentCellValueEventArgs(row, col);
                if (QueryDependentCellValue != null)
                {
                    QueryDependentCellValue(grid, e);
                    if (!e.Cancel)
                    {
                        this.grid = grd;
                        this.cell = saveCell;
                        return e.CellValue;
                    }
                }
            }
            GridStyleInfo style = grid[row, col];
            string s = GetValueFromGrid(style, row, col);
            this.grid = grd;
            this.cell = saveCell;
            return s;
        }

        private string GetValueFromGrid(GridStyleInfo style, int row, int col)
        {
            string styleText = style.Text;
            GetFormulaText(ref styleText);

            if (style.CellModel is GridCellFormulaModel
                && styleText.Length > 0 && (this.formulaChar == (char)0 || (this.formulaChar != (char)0 &&
                this.formulaChar == styleText[0])))
            {
                if (style.FormulaTag == null)
                {
                    style.FormulaTag = new GridFormulaTag(Parse(styleText), null, row, col);
                }

                GridFormulaTag tag = style.FormulaTag;
                if (tag != null)
                {
                    if ((tag.Text == null || tag.Text.Length == 0) && tag.Formula.Length > 0)
                    {
                        string save = this.cell;
                        this.cell = GridRangeInfo.GetAlphaLabel(col) + row.ToString();

                        tag.Text = ComputedValue(tag.Formula);

                        this.cell = save;
                    }
                    return tag.Text;
                }
                else
                    return "";
            }
            else
            {
                if (styleText == this.TRUEVALUESTR || styleText == this.FALSEVALUESTR)
                    return styleText;
                double d;
                if (styleText.Length > 0)
                {
                    if (stringOK || double.TryParse(styleText, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                    {
                        return styleText;
                    }
                }
                return "";
            }
        }

        /// <summary>
        /// Computes the value contained in the argument.
        /// </summary>
        /// <remarks>
        /// This method takes the argument and checks whether it is a 
        /// parsed formula, raw number, or cell reference like A21.
        /// The return value is a string that holds the computed value of 
        /// the passed-in argument.
        /// </remarks>
        /// <param name="arg">A parsed formula, raw number, or cell reference.</param>
        /// <returns>A string with the computed number in it.</returns>
        public string GetValueFromArg(string arg)
        {
            return GetValueFromArg(arg, false);
        }

        private bool stringOK = false;

        /// <summary>
        /// Computes the value contained in the argument.
        /// </summary>
        /// <remarks>
        /// This method takes the argument and checks whether it is a 
        /// parsed formula, raw number, or cell reference like A21.
        /// The return value is a string that holds the computed value of 
        /// the passed-in argument.
        /// </remarks>
        /// <param name="arg">A parsed formula, raw number, or cell reference.</param>
        /// <param name="returnString">If False, this method returns the empty 
        /// string if the arg is a string. If True, this method will return the string value.</param>
        /// <returns>A string with the arg value in it.</returns>
        public string GetValueFromArg(string arg, bool returnString)
        {
            double d;
            if (GridCellFormulaModel.IsEmpty(arg))
                return "";
            else if (arg[0] == TIC[0]) //string
            {
                return arg;
            }
            else if (arg[0] == BMARKER) //parsed formula
            {
                arg = arg.Replace('{', '(');
                arg = arg.Replace('}', ')');
                return this.ComputedValue(arg);
            }
            else if (arg == this.TRUEVALUESTR || arg == 'n' + this.TRUEVALUESTR)
            {
                return "1";
            }
            else if (arg == this.FALSEVALUESTR || arg == 'n' + this.FALSEVALUESTR)
            {
                return "0";
            }
            else if (arg[0] == this.TIC[0])
            {
                return arg;
            }
            else
            {
                arg = arg.Replace('u', '-');
                if (!char.IsUpper(arg[0])
                    && (char.IsDigit(arg[0]) || arg[0] == ParseDecimalSeparator || arg[0] == '-' || arg[0] == 'n'))
                {
                    if (arg[0] == 'n')
                        arg = arg.Substring(1);
                    else if (arg.Length > 1 && arg[1] == 'n')
                        arg = arg.Remove(1, 1);
                    if (double.TryParse(arg, NumberStyles.Number | NumberStyles.AllowExponent, null, out d))
                    {
                        return d.ToString();
                    }
                }
            }
            //not a number
            if ((arg.IndexOfAny(new char[] { '+', '-', '/', '*', ')', ')', '{' }) == -1
                && char.IsUpper(arg[0])) || arg[0] == sheetToken)
            {
                if (arg == cell && !isFormulainConditionalFormat) //check for circular reference
                {
                    Hashtable ht = (Hashtable)DependentCells[arg];
                    if (ht != null && ht.ContainsKey(arg))
                        ht.Remove(arg);
                    ClearFormulaDependentCells(cell);
                    throw new ArgumentException(FormulaErrorStrings[circular_reference_] + arg);
                }

                this.stringOK = true;
                string s1 = this.GetValueFromGrid(arg);
                if (!string.IsNullOrEmpty(s1) && s1.EndsWith("%"))
                {
                    s1 = s1.Remove(s1.Length - 1);
                    s1 = (Double.Parse(s1) / 100).ToString();
                }
                this.stringOK = false;

                if (s1.Length > 0 && double.TryParse(s1.Replace(TIC, ""), NumberStyles.Any, null, out d))
                    s1 = d.ToString();
                UpdateDependencies(arg);
                return s1;
            }

            //Must be a formula, so try to parse it and compute.

            arg = arg.Replace('{', '(');
            arg = arg.Replace('}', ')');
            arg = this.Parse(arg);

            if (arg.EndsWith("%") && double.TryParse(arg.Substring(0, arg.Length - 1), NumberStyles.Any, null, out d))
            {
                arg = (d / 100).ToString();
            }

            return this.ComputedValue(arg);
        }


        internal string SheetToken(string s)
        {
            int i = 0;
            string s1 = "";
            if (i < s.Length && s[i] == sheetToken)
            {
                i++;
                while (i < s.Length && s[i] != sheetToken)
                    i++;
                s1 = s.Substring(0, i + 1);
            }

            if (i < s.Length)
                return s1;

            throw new ArgumentException(FormulaErrorStrings[bad_index]);
        }

        /// <summary>
        /// Returns the row index from a cell reference.
        /// </summary>
        /// <param name="s">String holding a cell reference such as C21 or AB11.</param>
        /// <returns>An integer with the corresponding row number.</returns>
        public int RowIndex(string s)
        {
            if (this.CurrentRowNotationEnabled && s.Length == 0)
            {
                return 0;
            }
            else
            {
                //To remove the $ symbol in the cells address
                s = s.Replace(STRING_fixedreference, STRING_empty);
                int i = 0;
                if (i < s.Length && s[i] == sheetToken)
                {
                    i++;
                    while (i < s.Length && s[i] != sheetToken)
                        i++;
                    i++;
                }

                while (i < s.Length && char.IsLetter(s[i]))
                    i++;
                if (i < s.Length)
                {
                    i = int.Parse(s.Substring(i));
                    if (i == 0 && this.CurrentRowNotationEnabled && this.cell.Length > 1)
                    {
                        i = RowIndex(this.cell);
                    }

                    return i;
                }

            }

            throw new ArgumentException(FormulaErrorStrings[bad_index]);
        }

        /// <summary>
        /// Returns a column index from a cell reference.
        /// </summary>
        /// <param name="s">String holding a cell reference such as C21 or AB11.</param>
        /// <returns>An integer with the corresponding column number.</returns>
        public int ColIndex(string s)
        {
            int i = 0;
            int k = 0;
            //To remove the $ symbol in the cells address
            s = s.Replace(STRING_fixedreference, STRING_empty);
#if !WinRT
            s = s.ToUpper(CultureInfo.InvariantCulture);
#else
            s = s.ToUpper();
#endif

            if (i < s.Length && s[i] == sheetToken)
            {
                i++;
                while (i < s.Length && s[i] != sheetToken)
                    i++;
                i++;
            }

            while (i < s.Length && char.IsLetter(s[i]))
            {
                k = (int)(k * 26 + (int)s[i] - (int)('A') + 1);
                i++;
            }

            if (k == 0)
                throw new ArgumentException(FormulaErrorStrings[bad_index]);

            return k;
        }


        #endregion

        #region Computations

        int FindLastqNotInBrackets(string s)
        {
            int found = -1;
            bool lastBracket = false;
            int i = s.Length - 1;
            while (i > -1)
            {
                if (s[i] == 'q' && lastBracket)
                {
                    found = i;
                    break;
                }
                if (s[i] == LEFTBRACKET)
                    lastBracket = true;
                else if (s[i] == RIGHTBRACKET)
                    lastBracket = false;
                i--;
            }

            return found;
        }

        int computeFunctionLevel = 0;
        internal string ComputeInteriorFunctions(string formula, List<string> circCheckList)
        {
            try
            {
                if (GridCellFormulaModel.IsEmpty(formula))
                    return formula;

                computeFunctionLevel++;

                //int q = formula.LastIndexOf('q');
                int q = FindLastqNotInBrackets(formula);


                while (q > 0)
                {
                    int last = formula.Substring(q).IndexOf(RIGHTBRACKET);
                    if (last == -1)
                        return FormulaErrorStrings[bad_formula];
                    string s = formula.Substring(q, last + 1);
                    s = ComputedValue(s, circCheckList);
                    if (!GridCellFormulaModel.IsEmpty(s) && s[0] == '-')
                        s = "nu" + s.Substring(1);
                    else if (s.Length > 0 &&
                        (s[0] == TIC[0] || s[0] == BMARKER || s[0] == '#'))
                    {
                        //pass on the string...
                    }

                    else if (s.StartsWith(TRUEVALUESTR) || s.StartsWith(FALSEVALUESTR))
                    {
                        //pass on the bool...
                    }
                    else
                    {
                        double d = 0;
                        if (double.TryParse(s, NumberStyles.Any, null, out d))
                        {
                            //need to strip out any group separators - possible with FIXED function
                            s = d.ToString();
                            s = 'n' + s;
                        }
                        else
                        {
                            s = TIC + s + TIC;
                        }


                    }

                    formula = formula.Substring(0, q) + s + formula.Substring(q + last + 1);
                    //q = formula.LastIndexOf('q');
                    q = FindLastqNotInBrackets(formula);
                }
            }

            catch (Exception ex)
            {
#if WPF
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                }
#endif
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw ex;
                return ex.Message;
            }
            finally
            {
                computeFunctionLevel--;
            }

            return formula;

        }

        /// <summary>
        /// Code copied form our CalcEngine (Syncfusion.Calculate.Base ) to improve the nested if calculation
        /// </summary>
        /// <param name="s">string</param>
        private void MarkupResultToIncludeInFormula(ref string s)
        {
            double d3;
            if (s.Length > 0 && s[0] == '-' && double.TryParse(s, out d3))
            {
                s = "nu" + s.Substring(1);
            }
            else if (s.Length > 0 &&
                (s[0] == this.TIC[0] || s[0] == BMARKER || s[0] == '#'))
            {
                ////Pass on the string...
            }
            else if (s.StartsWith(this.TRUEVALUESTR) || s.StartsWith(FALSEVALUESTR))
            {
                ////Pass on the bool...
            }
            else
            {
                double d = 0;

                ////if(double.TryParse(s, NumberStyles.Any, null, out d))
                if (double.TryParse(s, out d))
                {
                    // s = d.ToString(); ////defect 8721
                    s = string.Format("{0:d}", s);
                    s = s.Replace(ParseArgumentSeparator, (char)32);

                    s = 'n' + s;
                }
                else
                {
                    s = this.TIC + s + this.TIC;
                }
            }

        }

        private bool useNoAmpersandQuotes = false;

        /// <summary>
        /// Gets or sets whether strings concatenated using the ampersand operator should 
        /// be returned inside double quote marks.
        /// </summary>
        public bool UseNoAmpersandQuotes
        {
            get { return useNoAmpersandQuotes; }
            set { useNoAmpersandQuotes = value; }
        }

        //max calculation stack depth
        private int maxStackDepth = 100;

        /// <summary>
        /// Depth of the engine's calculation stack.
        /// </summary>
        /// <remarks>
        /// It is unlikely that you will need to adjust this value, as
        /// its default value of 100 is quite large.
        /// Any time a formula is to be computed, a Stack object is created
        /// with this number of elements to hold temporary calculations
        /// as the formula is being computed. 
        /// For example, this formula: 1+1+1+...+1 requires a stack 
        /// depth of 2 as there are only 2 temporary values needed as the 
        /// formula is evaluated. However, this formula: 1+(1+(1+(1+1)))
        /// requires a depth of 5, as the five 1's are pushed onto the 
        /// stack before the first addition (the right-most one) is
        /// performed.
        /// </remarks>
        public int MaximumStackDepth
        {
            get { return maxStackDepth; }
            set { maxStackDepth = value; }
        }

        private int computedValueLevel = 0;

        [ThreadStaticAttribute]
        private static GridFormulaEngine functionEngineContext = null;

        /// <summary>
        /// Gets the formula engine that holds the proper state information for LibraryFunction call.
        /// </summary>
        /// <remarks>
        /// If you are adding your own custom library functions and need to access GridFormulaEngine members like
        /// GetCellsFromArgs or GetValueFromArg from your code, then you should use this static property to retrieve the proper
        /// GridFormulaEngine object. This only matters if you are using multiple grids that you have registered using
        /// RegisterGridAsSheet, and are adding your own custom formulas. Note that GridFormulaEngine.FunctionEngineContext
        /// is only defined within the scope of a library function call, and will be null at all other times.
        /// </remarks>
        /// <example> Use GridFormulaEngine.FunctionEngineContext to retrieve the engine 
        /// when writing custom functions.
        /// <code lang="C#">
        /// public string ComputeSumPosNums(string args)
        /// {
        /// 	GridFormulaEngine engine = GridFormulaEngine.FunctionEngineContext;
        /// 	string sum = "";
        /// 	foreach(string r in args.Split(new char[]{','}))
        /// 	{
        /// 		if(r.IndexOf(':') > -1) //is a cellrange
        /// 		{
        /// 			foreach(string s in engine.GetCellsFromArgs(r))
        /// 			{
        /// 				s1 = engine.GetValueFromArg(s).Replace("'","");
        /// 				//... do some calculations to compute sum
        /// 			}
        /// 		}
        /// 	}	
        /// 	return sum.ToString();
        /// }
        /// </code>
        /// <code lang="VB">
        /// Public Function ComputeSumPosNums(args As String) As String
        ///    Dim engine As GridFormulaEngine = GridFormulaEngine.FunctionEngineContext
        ///    Dim sum As String = ""
        ///    Dim r As String
        ///    For Each r In  args.Split(New Char() {","c})
        ///			If r.IndexOf(":"c) > - 1 Then 'is a cellrange
        ///				Dim s As String
        ///				For Each s In  engine.GetCellsFromArgs(r)
        ///					s1 = engine.GetValueFromArg(s).Replace("'", "")
        ///					'... do some calculations to compute sum
        ///				Next s 
        ///			End If
        ///    Next r
        ///    Return sum.ToString()
        /// End Function 'ComputeSumPosNums
        /// </code>
        /// </example>
        public static GridFormulaEngine FunctionEngineContext
        {
            get { return functionEngineContext; }
        }

        private string activeFunctionName;

        /// <summary>
        /// Returns the function name of the active function call. Empty if not in a call.
        /// </summary>
        public string ActiveFunctionName
        {
            get { return activeFunctionName; }
        }

        /// <summary>
        /// Computes the value of a parsed formula.
        /// </summary>
        /// <param name="formula">The formula to be computed.</param>
        /// <returns>A string holding the computed value.</returns>
        /// <remarks> 
        ///  The string passed into ComputedValue must have been parsed using the Parse
        ///  method. Before calling the method, you should set FormulaContextCell
        ///  to properly reflect which cell owns this formula.
        /// </remarks>
        public string ComputedValue(string formula)
        {
            return ComputedValue(formula, null);
        }

        internal void GetCellsInArgs(List<string> circCheckList, string args)
        {
            foreach (string r in args.Split(new char[] { ParseArgumentSeparator }))
            {
                if (r.IndexOf(':') > -1) //is a cellrange
                {
                    foreach (string s in GetCellsFromArgs(r))
                    {
                        circCheckList.Add(s);
                    }
                }
                else
                {
                    if (r.Length > 2 && r[0] == BMARKER && r[r.Length - 1] == BMARKER)
                        this.ComputedValue(r, circCheckList);
                    else if (r.Length > 2 && char.IsUpper(r[0]) && char.IsDigit(r[r.Length - 1]))
                        circCheckList.Add(r);
                }
            }
        }

        internal string ComputedValue(string formula, List<string> circCheckList)
        {

            //#if DEBUG
            //            if (Switches.FormulaCell.TraceVerbose)
            //                TraceUtil.TraceCurrentMethodInfo(formula, computedValueLevel, this);
            //#else
            //            ;
            //#endif

            if (GridCellFormulaModel.IsEmpty(formula))
                return formula;
            try
            {
                computedValueLevel++;

                ParseDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                if (computedValueLevel > maximumRecursiveCalls)
                {
                    computedValueLevel = 0;
                    if (circCheckList != null)
                    {
                        circCheckList.Add(this.cell);
                        return "0";
                    }
                    throw new ArgumentException(FormulaErrorStrings[too_complex]);
                }

                Stack<object> _stack = new Stack<object>();

                int i = 0;
                _stack.Clear();
                string sheet = "";

                //Code copied form our CalcEngine (Syncfusion.Calculate.Base ) to improve the nested if calculation
                if (this.AllowShortCircuitIFs && EnsureIFCallDuringShortCircuit)
                {
                    int loc = -1;
                    do
                    {
                        ////code peels all only the outer most IF functions  and relies on a custom IF to handle the proper alternative
                        if (i < formula.Length && (i = formula.IndexOf(this.IFMarker, i)) > -1)
                        {
                            loc = this.MatchingRightBracket(formula.Substring(i));
                            if (loc > -1)
                            {
                                LibraryFunction func = (LibraryFunction)this.LibraryFunctions["IF"];
                                string result = "";
                                try
                                {
                                    result = func(formula.Substring(i + this.IFMarker.Length, loc - this.IFMarker.Length));
                                }
                                catch (Exception ex)
                                {
                                    if (circCheckList != null)
                                    {
                                        circCheckList.Add(this.cell);
                                        return "0";
                                    }
                                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                        throw ex;
                                    return ex.Message;
                                }

                                MarkupResultToIncludeInFormula(ref result);

                                //double d = 0;
                                //if (double.TryParse(result, out d))
                                //{
                                //    if (d < 0)
                                //    {
                                //        result = "nu" + (-d).ToString();
                                //    }
                                //    else
                                //    {
                                //        result = "n" + d.ToString();
                                //    }
                                //}
                                string rightPiece = "";
                                if (i + loc + 1 < formula.Length)
                                    rightPiece = formula.Substring(i + loc + 1);
                                formula = formula.Substring(0, i) + result + rightPiece;
                            }
                        }
                    }
                    while (formula.Contains(this.IFMarker) && loc > -1);
                }

                i = 0;
                while (i < formula.Length)
                {
                    if (formula[i] == BMARKER)
                    {
                        i = i + 1;
                        continue;
                    }

                    bool uFound = formula[i] == 'u';
                    if (uFound)
                    {
                        i++;
                        if (i >= formula.Length)
                            continue; //error case....
                    }

                    if (formula[i] == '%' && _stack.Count > 0)
                    {
                        object o = _stack.Peek();
                        double d;
                        if (double.TryParse(o.ToString(), NumberStyles.Any, null, out d))
                        {
                            _stack.Pop(); //remove it
                            _stack.Push(d / 100); //push it back
                        }
                        i = i + 1;
                        continue;
                    }



                    if (formula[i] == sheetToken)
                    {
                        sheet = formula[i].ToString();
                        i++;
                        while (i < formula.Length && formula[i] != sheetToken)
                        {
                            sheet += formula[i];
                            i++;
                        }
                        if (i < formula.Length)
                        {
                            sheet += formula[i];
                            i++;
                        }
                        else
                            continue;//skip out as something is wrong
                    }
                    if (formula.Substring(i).StartsWith(TRUEVALUESTR))
                    {
                        _stack.Push(TRUEVALUESTR);
                        i += TRUEVALUESTR.Length;
                    }
                    else if (formula.Substring(i).StartsWith(FALSEVALUESTR))
                    {
                        _stack.Push(FALSEVALUESTR);
                        i += FALSEVALUESTR.Length;
                    }
                    else if (formula[i] == TIC[0])
                    {
                        string s = formula[i].ToString();
                        i++;
                        while (i < formula.Length && formula[i] != TIC[0])
                        {
                            s = s + formula[i];
                            i = i + 1;
                        }

                        _stack.Push(s + TIC);
                        i += 1;
                    }
                    else if (char.IsUpper(formula[i]))
                    {//cell loc
                        string s = "";
                        while (i < formula.Length && char.IsUpper(formula[i]))
                        {
                            s = s + formula[i];
                            i = i + 1;
                        }
                        while (i < formula.Length && char.IsDigit(formula[i]))
                        {
                            s = s + formula[i];
                            i = i + 1;
                        }

                        s = sheet + s;
                        sheet = "";
                        if (s == cell && !isFormulainConditionalFormat)
                        {
                            computedValueLevel = 0;
                            Hashtable ht = (Hashtable)DependentCells[s];
                            if (ht != null && ht.ContainsKey(s))
                                ht.Remove(s);
                            ClearFormulaDependentCells(cell);
                            if (circCheckList != null)
                            {
                                circCheckList.Add(this.cell);
                                return "0";
                            }
                            throw new ArgumentException(FormulaErrorStrings[circular_reference_] + s);
                        }

                        if (circCheckList != null)
                        {
                            circCheckList.Add(s);
                        }

                        //UpdateDependencies(s);

                        stringOK = true;
                        string o = GetValueFromGrid(s);
                        stringOK = false;
                        double d;
                        if (o != null && o.EndsWith("%") && double.TryParse(o.Substring(0, o.Length - 1), NumberStyles.Any, null, out d))
                        {
                            o = (d / 100).ToString();
                        }
                        if (uFound && o != null && double.TryParse(o, NumberStyles.Any, null, out d))
                        {
                            o = (-d).ToString();
                        }
                        uFound = false;
                        _stack.Push(o);
                    }
                    else if (formula[i] == 'q')//library
                    {
                        formula = ComputeInteriorFunctions(formula, circCheckList);

                        int ii = formula.Substring(i + 1).IndexOf(LEFTBRACKET);
                        if (ii > 0)
                        {
                            int bracketCount = 0;
                            bool bracketFound = false;
                            int start = ii + i + 2;
                            int k = start;
                            while (k < formula.Length && (formula[k] != RIGHTBRACKET || bracketCount > 0))
                            {
                                if (formula[k] == LEFTBRACKET)
                                {
                                    bracketCount++;
                                    bracketFound = true;
                                }
                                else if (formula[k] == LEFTBRACKET)
                                    bracketCount--;

                                k++;
                            }

                            if (bracketFound)
                            {
                                string s = formula.Substring(start, k - start - 2);
                                string s1 = "";
                                foreach (string t in s.Split(new Char[] { ParseArgumentSeparator }))
                                {
                                    if (s1.Length > 0)
                                        s1 += ",";
                                    //int j = t.LastIndexOf('q');
                                    int j = FindLastqNotInBrackets(t);
                                    if (j > 0)
                                    {
                                        s1 += t.Substring(0, j) + ComputedValue(t.Substring(j), circCheckList);
                                    }
                                    else
                                        s1 += ComputedValue(t, circCheckList);
                                }
                                formula = formula.Substring(0, start) + s1 + formula.Substring(k - 2);
                            }
                            string name = formula.Substring(i + 1, ii);
                            if (this.LibraryFunctions[name] != null)
                            {
                                int j = formula.Substring(i + ii + 1).IndexOf(RIGHTBRACKET);
                                string args = formula.Substring(i + ii + 2, j - 1);
                                if (circCheckList != null)
                                {
                                    GetCellsInArgs(circCheckList, args);
                                    _stack.Push("0");
                                }
                                else
                                {
                                    try
                                    {
                                        functionEngineContext = this;
                                        LibraryFunction func = (LibraryFunction)this.LibraryFunctions[name];
                                        this.activeFunctionName = name;
                                        String result = func(args);
                                        if (UseNoAmpersandQuotes && result.Length > 1 && result.StartsWith(TIC) && result.EndsWith(TIC))
                                        {
                                            result = result.Substring(1, result.Length - 2);
                                        }

                                        double d = 0;
                                        if (uFound && result != null && double.TryParse(result, NumberStyles.Any, null, out d))
                                        {
                                            result = (-d).ToString();
                                        }
                                        uFound = false;
                                        _stack.Push(result);
                                        this.activeFunctionName = "";
                                        functionEngineContext = null;
                                    }
                                    catch (Exception ex)
                                    {
#if WPF
                                        if (!BrowserInteropHelper.IsBrowserHosted)
                                        {
                                            TraceUtil.TraceExceptionCatched(ex);
                                        }
#endif
                                        if (circCheckList != null)
                                        {
                                            circCheckList.Add(this.cell);
                                            return "0";
                                        }
                                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                            throw ex;
                                        return ex.Message;
                                    }
                                }

                                i += j + ii + 2;
                            }
                            else
                            {
                                if (circCheckList != null)
                                {
                                    circCheckList.Add(this.cell);
                                    return "0";
                                }
                                return FormulaErrorStrings[missing_formula];
                            }
                        }
                        else if (formula[0] == BMARKER)
                        {
                            //Restart the processing with the formula without library finctions.
                            i = 0;
                            continue;
                        }
                        else
                        {
                            if (circCheckList != null)
                            {
                                circCheckList.Add(this.cell);
                                return "0";
                            }
                            return FormulaErrorStrings[improper_formula];
                        }
                    }
                    else if (char.IsDigit(formula[i]) || formula[i] == 'u')
                    {
                        string s = "";
                        if (formula[i] == 'u')
                        {
                            s = "-";
                            i++;
                        }
                        else if (uFound)
                        {
                            s = "-";
                        }
                        uFound = false;
                        while (i < formula.Length && (char.IsDigit(formula[i]) || formula[i] == ParseDecimalSeparator))
                        {
                            s = s + formula[i];
                            i = i + 1;
                        }

                        _stack.Push(s);
                    }
                    else
                    {
                        switch (formula[i])
                        {
                            case '#':
                                {
                                    i = i + 4;
                                    _stack.Push("#N/A");

                                    break;
                                }
                            case 'n':
                                {
                                    i = i + 1;

                                    string s = "";
                                    if (formula.Substring(i).StartsWith("Infinity"))
                                    {
                                        s = "Infinity";
                                        i += s.Length;
                                    }
                                    else if (formula.Substring(i).StartsWith(TRUEVALUESTR))
                                    {

                                        s = TRUEVALUESTR;
                                        i += s.Length;
                                    }
                                    else if (formula.Substring(i).StartsWith(FALSEVALUESTR))
                                    {
                                        s = FALSEVALUESTR;
                                        i += s.Length;
                                    }
                                    else if (i < formula.Length - 3 && formula.Substring(i, 3) == "NaN")
                                    {
                                        i += 3;
                                        s = "0";
                                    }
                                    else
                                    {
                                        if (formula[i] == 'u')
                                        {
                                            s = "-";
                                            i = i + 1;
                                        }
                                        while (i < formula.Length && (char.IsDigit(formula[i]) || formula[i] == ParseDecimalSeparator))
                                        {
                                            s = s + formula[i];
                                            i = i + 1;
                                        }

                                        //Check for exp values.
                                        if (i < formula.Length - 3 && formula[i] == 'E' && (formula[i + 1] == '+' || formula[i + 1] == '-'))
                                        {
                                            s = s + formula[i];//E
                                            i = i + 1;
                                            s = s + formula[i];//sign
                                            i = i + 1;
                                            s = s + formula[i];//first digit
                                            i = i + 1;
                                            while (i < formula.Length && (char.IsDigit(formula[i]) || formula[i] == ParseDecimalSeparator))
                                            {
                                                s = s + formula[i];
                                                i = i + 1;
                                            }
                                        }
                                        if (i < formula.Length && formula[i] == '%')
                                        {
                                            i = i + 1;
                                            if (s.Length == 0)
                                            {
                                                if (_stack.Count > 0)
                                                {
                                                    object o = _stack.Peek();
                                                    double d;
                                                    if (double.TryParse(o.ToString(), NumberStyles.Any, null, out d))
                                                    {
                                                        _stack.Pop(); //remove it
                                                        s = (d / 100).ToString(); //push it back
                                                    }
                                                }
                                            }
                                            else
                                                s = (double.Parse(s) / 100).ToString();
                                        }
                                    }
                                    _stack.Push(s);
                                }
                                break;
                            case TOKEN_add:
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    _stack.Push((d1 + d).ToString());
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_subtract:
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    _stack.Push((d1 - d).ToString());
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_multiply:
                                {
                                    double d = Pop(_stack);//double.Parse(g);
                                    double d1 = Pop(_stack);//double.Parse(g1);

                                    _stack.Push((d1 * d).ToString());
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_divide:
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    _stack.Push((d1 / d).ToString());
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_EP:
                                {
                                    double d = Pop(_stack);//double.Parse(g);
                                    double d1 = Pop(_stack);//double.Parse(g1);

                                    _stack.Push((d1 * Math.Pow(10, d)).ToString());
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_EM:
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    _stack.Push((d1 * Math.Pow(10, -d)).ToString());
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_less:
                                {
                                    string s1 = PopString(_stack);
                                    string s2 = PopString(_stack);
                                    double d, d1;
                                    string val;
                                    //	if(double.TryParse(s1, NumberStyles.Any, null, out d)
                                    //		&& double.TryParse(s2, NumberStyles.Any, null, out d1))
                                    if (ParsePossibleEmptyStrings(s1, s2, out d, out d1))
                                    {

                                        val = (d1 < d) ? TRUEVALUESTR : FALSEVALUESTR;

                                    }
                                    else
                                    {
                                        val = (s1.ToUpper().Replace(TIC, "").CompareTo(s2.ToUpper().Replace(TIC, "")) < 0) ? TRUEVALUESTR : FALSEVALUESTR;
                                    }
                                    _stack.Push(val);
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_greater:
                                {
                                    string s1 = PopString(_stack);
                                    string s2 = PopString(_stack);
                                    double d, d1;
                                    string val;
                                    //if(double.TryParse(s1, NumberStyles.Any, null, out d)
                                    //    && double.TryParse(s2, NumberStyles.Any, null, out d1))
                                    if (ParsePossibleEmptyStrings(s1, s2, out d, out d1))
                                    {
                                        val = (d1 > d) ? TRUEVALUESTR : FALSEVALUESTR;
                                    }
                                    else
                                    {
                                        val = (s1.ToUpper().Replace(TIC, "").CompareTo(s2.ToUpper().Replace(TIC, "")) > 0) ? TRUEVALUESTR : FALSEVALUESTR;
                                    }
                                    _stack.Push(val);
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_equal:
                                {
                                    string s1 = PopString(_stack);
                                    string s2 = PopString(_stack);
                                    double d, d1;
                                    string val;
                                    //if(double.TryParse(s1, NumberStyles.Any, null, out d)
                                    //	&& double.TryParse(s2, NumberStyles.Any, null, out d1))
                                    if (ParsePossibleEmptyStrings(s1, s2, out d, out d1))
                                    {

                                        val = (d1 == d) ? TRUEVALUESTR : FALSEVALUESTR;

                                    }
                                    else
                                    {
                                        if ((s1.Length > 0 && s1[0] == '#')
                                            || (s2.Length > 0 && s2[0] == '#'))
                                        {
                                            val = "#N/A";
                                        }
                                        else
                                        {

                                            val = (s1.ToUpper().Replace(TIC, "") == s2.ToUpper().Replace(TIC, "")) ? TRUEVALUESTR : FALSEVALUESTR;
                                        }
                                    }
                                    _stack.Push(val);
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_lesseq:
                                {
                                    string s1 = PopString(_stack);
                                    string s2 = PopString(_stack);
                                    double d, d1;
                                    string val;
                                    //if(double.TryParse(s1, NumberStyles.Any, null, out d)
                                    //	&& double.TryParse(s2, NumberStyles.Any, null, out d1))
                                    if (ParsePossibleEmptyStrings(s1, s2, out d, out d1))
                                    {

                                        val = (d1 <= d) ? TRUEVALUESTR : FALSEVALUESTR;

                                    }
                                    else
                                    {
                                        val = (s1.ToUpper().Replace(TIC, "").CompareTo(s2.ToUpper().Replace(TIC, "")) <= 0) ? TRUEVALUESTR : FALSEVALUESTR;
                                    }
                                    _stack.Push(val);
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_greatereq:
                                {
                                    string s1 = PopString(_stack);
                                    string s2 = PopString(_stack);
                                    double d, d1;
                                    string val;
                                    //if(double.TryParse(s1, NumberStyles.Any, null, out d)
                                    //    && double.TryParse(s2, NumberStyles.Any, null, out d1))
                                    if (ParsePossibleEmptyStrings(s1, s2, out d, out d1))
                                    {

                                        val = (d1 >= d) ? TRUEVALUESTR : FALSEVALUESTR;

                                    }
                                    else
                                    {
                                        val = (s1.ToUpper().Replace(TIC, "").CompareTo(s2.ToUpper().Replace(TIC, "")) >= 0) ? TRUEVALUESTR : FALSEVALUESTR;
                                    }
                                    _stack.Push(val);
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_noequal:
                                {
                                    string s1 = PopString(_stack);
                                    string s2 = PopString(_stack);
                                    double d, d1;
                                    string val;
                                    //if(double.TryParse(s1, NumberStyles.Any, null, out d)
                                    //    && double.TryParse(s2, NumberStyles.Any, null, out d1))
                                    if (ParsePossibleEmptyStrings(s1, s2, out d, out d1))
                                    {

                                        val = (d1 != d) ? TRUEVALUESTR : FALSEVALUESTR;

                                    }
                                    else
                                    {
                                        val = (s1.ToUpper().Replace(TIC, "") != s2.ToUpper().Replace(TIC, "")) ? TRUEVALUESTR : FALSEVALUESTR;
                                    }
                                    _stack.Push(val);
                                    i = i + 1;
                                }
                                //									{
                                //										double d = Pop(_stack);
                                //										double d1 = Pop(_stack); 
                                //										int val = (d1 != d) ? 1 : 0;
                                //										_stack.Push(val.ToString());
                                //										i = i + 1;
                                //									}
                                break;
                            ////							case TOKEN_like://like
                            ////							{
                            ////								string s1 = PopString(_stack);
                            ////								string s = PopString(_stack); 
                            ////								bool b = true;
                            ////								if (s.Length == 0 && s1.Length == 0)
                            ////									b = true;
                            ////								else if (s.Length == 0 || s1.Length == 0)
                            ////									b = false;
                            ////								else
                            ////									b = Microsoft.VisualBasic.CompilerServices.StringType.StrLike(
                            ////										s, s1, Microsoft.VisualBasic.CompareMethod.Text);	
                            ////
                            ////								int val = (b) ? TRUEVALUE : FALSEVALUE;
                            ////								_stack.Push(val.ToString());
                            ////								i = i + 1;
                            ////							}
                            ////								break;
                            ////							case TOKEN_match://match
                            ////							{
                            ////								string s1 = PopString(_stack);
                            ////								string s = PopString(_stack); 
                            ////								bool b = true;
                            ////								if (s.Length == 0 && s1.Length == 0)
                            ////									b = true;
                            ////								else if (s.Length == 0 || s1.Length == 0)
                            ////									b = false;
                            ////								else
                            ////								{
                            ////									if (regexValue == null || oldregexValueCompare != s1)
                            ////									{
                            ////										regexValue = new System.Text.RegularExpressions.Regex(s1);
                            ////										oldregexValueCompare = s1;
                            ////									}
                            ////									b = regexValue.IsMatch(s);
                            ////								}
                            ////								int val = (b) ? TRUEVALUE : FALSEVALUE;
                            ////								_stack.Push(val.ToString());
                            ////								i = i + 1;
                            ////							} 
                            ////								break;
                            ////							case TOKEN_in://in uses Like
                            ////							{
                            ////								string s1 = PopString(_stack);
                            ////								string s = PopString(_stack); 
                            ////								bool b = false;
                            ////								if (s.Length == 0 && s1.Length == 0)
                            ////									b = true;
                            ////								else if (s.Length == 0 || s1.Length == 0)
                            ////									b = false;
                            ////								else
                            ////								{
                            ////									foreach(string s2 in s1.Split(BRACEDELIMETER))
                            ////									{
                            ////										b |= Microsoft.VisualBasic.CompilerServices.StringType.StrLike(
                            ////											s, s2, Microsoft.VisualBasic.CompareMethod.Text);	
                            ////
                            ////										//								if (regexValue == null || oldregexValueCompare != s2)
                            ////										//								{
                            ////										//									regexValue = new System.Text.RegularExpressions.Regex(s2);
                            ////										//									oldregexValueCompare = s2;
                            ////										//								}
                            ////										//								b |= regexValue.IsMatch(s);
                            ////									}
                            ////								}
                            ////								int val = (b) ? TRUEVALUE : FALSEVALUE;
                            ////								_stack.Push(val.ToString());
                            ////								i = i + 1;
                            ////							} 
                            ////								break;
                            ////							case TOKEN_between: //handles dates - use TODAY for todays's date
                            ////							{
                            ////								string s1 = PopString(_stack);
                            ////								string s = PopString(_stack); 
                            ////								bool b = false;
                            ////								if (s.Length == 0 && s1.Length == 0)
                            ////									b = true;
                            ////								else if (s.Length != 0 && s1.Length != 0)
                            ////								{
                            ////									//Uses only dates, no time...
                            ////									DateTime leftDate = (s.Length == 0) ? DateTime.MinValue.Date : DateTime.Parse(s).Date;
                            ////									DateTime startDate = DateTime.MinValue.Date;
                            ////									DateTime endDate = DateTime.MaxValue.Date;
                            ////									string[] dates = s1.Split(BRACEDELIMETER);
                            ////									if(dates.GetLength(0) == 2)
                            ////									{
                            ////										if(dates[0] == "today")
                            ////											startDate = DateTime.Now.Date;
                            ////										else if(dates[0].Length > 0)
                            ////											startDate = DateTime.Parse(dates[0]).Date;
                            ////										if(dates[1] == "today")
                            ////											endDate = DateTime.Now.Date;
                            ////										else if(dates[1].Length > 0)
                            ////											endDate = DateTime.Parse(dates[1]).Date;
                            ////									}
                            ////									b = (startDate <= leftDate && endDate > leftDate)
                            ////										|| (endDate == startDate && endDate == leftDate);
                            ////							
                            ////								}
                            ////								int val = (b) ? TRUEVALUE : FALSEVALUE;
                            ////								_stack.Push(val.ToString());
                            ////								i = i + 1;
                            ////							} 
                            ////								break;
                            case TOKEN_and://and strings....
                                {
                                    string s1 = PopString(_stack);
                                    if (s1.Length > 0 && s1[0] == TIC[0])
                                    {
                                        if (s1.Length > 1 && s1[s1.Length - 1] == TIC[0])
                                            s1 = s1.Substring(1, s1.Length - 2);
                                    }
                                    string s2 = PopString(_stack);
                                    if (s2.Length > 0 && s2[0] == TIC[0])
                                    {
                                        if (s2.Length > 1 && s2[s2.Length - 1] == TIC[0])
                                            s2 = s2.Substring(1, s2.Length - 2);
                                    }
                                    if (useNoAmpersandQuotes)
                                        _stack.Push(s2 + s1);
                                    else
                                        _stack.Push(TIC + s2 + s1 + TIC);
                                    //								double d = Pop(_stack);
                                    //								double d1 = Pop(_stack);
                                    //					
                                    //								bool b = IsEqual(d1, TRUEVALUE) && IsEqual(d, TRUEVALUE);
                                    //								int val = (b) ? TRUEVALUE : FALSEVALUE;
                                    //								_stack.Push(val.ToString());
                                    i = i + 1;
                                }
                                break;
                            case TOKEN_pow://^ power
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);
                                    double val = Math.Pow(d1, d);
                                    _stack.Push(val.ToString());
                                    i = i + 1;
                                }
                                break;
                            default:
                                {
                                    computedValueLevel = 0;
                                    if (circCheckList != null)
                                    {
                                        circCheckList.Add(this.cell);
                                        return "0";
                                    }
                                    throw new ArgumentException(FormulaErrorStrings[invalid_expression]);
                                }

                        }
                    }
                }

                if (_stack.Count == 0)
                    return "";
                else
                {
                    //					if(_stack.Count > 1 && computedValueLevel == 1)
                    //					{
                    //						return FormulaErrorStrings[missing_operand];
                    //					}
                    string s = _stack.Pop().ToString();
                    if (GridCellFormulaModel.IsEmpty(s) && !UseNoAmpersandQuotes)
                        s = "0"; //empty is zero in calculation
                    return s;
                }
            }
            catch (Exception ex)
            {
                computedValueLevel = 0;
                if (circCheckList != null)
                {
                    circCheckList.Add(this.cell);
                    return "0";
                }

                if (ex.Message.IndexOf(FormulaErrorStrings[circular_reference_]) > -1)
                    throw ex;
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw ex;
                if (ex.Message.IndexOf(FormulaErrorStrings[cell_empty]) > -1)
                    return "";
                else
                    return ex.Message; // + ":" + formula;

            }
            finally
            {
                computedValueLevel--;
            }
        }

        #region code to handle support for short circuiting IF calculations

        private bool allowShortCircuitIFs = false;
        /// <summary>
        /// Gets or sets whether IF function calculations should specifically avoid
        /// computing the non-used alternative.
        /// </summary>
        /// <remarks>
        /// The default value is false for code legacy consistency. When AllowShortCircuitIFs  
        /// is set true, only the necessary alternative of an IF function is computed. To support
        /// this behavior, a change in how nested IF function calculations are done is necessary.
        /// The default way of calculating nested functions is inside-out, with the inner most 
        /// functions being computed to a value before the next outer function is evaluated. To 
        /// support short circuiting IF functions, nested IF functions need to be computed from 
        /// the outside-in to know what alternative needs to be evaluated. This outside-in calculation
        /// pattern only applies to IF functions, and only when AllowShortCircuitIFs is true.
        /// </remarks>
        public bool AllowShortCircuitIFs
        {
            get { return allowShortCircuitIFs; }
            set { allowShortCircuitIFs = value; }
        }

        private bool ensureIFCallDuringShortCircuit = false;
        /// <summary>
        /// Gets or sets whether the IF function implementation is called when <see cref="AllowShortCircuitIFs"/> is true.
        /// The default behavior is to not call the IF Function code in the library, but instead, work directly with the
        /// IF clauses.
        /// </summary>
        public bool EnsureIFCallDuringShortCircuit
        {
            get { return ensureIFCallDuringShortCircuit; }
            set { ensureIFCallDuringShortCircuit = value; }
        }

        private int MatchingRightBracket(string formula)
        {
            int ret = -1;
            int loc = 1;
            int bracketLevel = 0;
            while (ret == -1 && loc < formula.Length)
            {
                if (formula[loc] == RIGHTBRACKET)
                {
                    if (bracketLevel == 0)
                        ret = loc;
                    else
                        bracketLevel--;
                }
                else if (formula[loc] == 'q')
                {
                    bracketLevel++;
                }
                loc++;
            }
            return ret;
        }

        private string HandleEmbeddedIf(string formula)
        {
            int sep1 = IFMarker.Length;

            int sep2 = sep1 + 1;
            double d = 0;
            if (EnsureIFCallDuringShortCircuit)
            {
                if (this.FindNextSeparator(formula, ref sep1))
                {
                    sep2 = sep1 + 1;
                    FindRightBracket(formula, ref sep2);
                    if (sep2 > -1)
                    {
                        LibraryFunction func = (LibraryFunction)this.LibraryFunctions["IF"];
                        if (func != null)
                        {
                            string args = formula.Substring(this.IFMarker.Length, sep2 - this.IFMarker.Length);
                            int embeddedIF = args.IndexOf(this.IFMarker);
                            while (embeddedIF > -1)
                            {
                                int loc = this.MatchingRightBracket(args.Substring(embeddedIF));
                                if (loc > -1)
                                {
                                    args = args.Substring(0, embeddedIF) + this.HandleEmbeddedIf(args.Substring(embeddedIF)) + args.Substring(embeddedIF + loc + 1);
                                }
                                embeddedIF = args.IndexOf(this.IFMarker);
                            }
                            ////check to see if there is a Function call in the if test clause
                            //int locq = args.IndexOf('q');
                            //int loc4 = args.IndexOf(ParseArgumentSeparator);
                            //if (locq > -1 && locq < loc4)
                            //{
                            //    this.FindNextSeparator(args, ref locq);
                            //    string gg = args.Substring(0, locq);
                            //    gg = this.ComputeInteriorFunctions(gg);
                            //    args = gg + args.Substring(locq);
                            //}
                            string result = func(args);
                            if (double.TryParse(result, out d))
                            {
                                if (d < 0)
                                {
                                    result = "nu" + (-d).ToString();
                                }
                                else
                                {
                                    result = "n" + d.ToString();
                                }
                            }
                            formula = result;
                        }
                    }
                }
                return formula;
            }
            if (FindNextSeparator(formula, ref sep1))
            {
                sep2 = sep1 + 1;
                if (!FindNextSeparator(formula, ref sep2))
                {
                    sep2 = formula.IndexOf(RIGHTBRACKET, sep1);
                    if (sep2 > -1)
                    {
                        formula = formula.Substring(0, sep2) + ParseArgumentSeparator + FALSEVALUESTR + formula.Substring(sep2);
                    }
                }
                string ifValue = ComputedValue(formula.Substring(IFMarker.Length, sep1 - IFMarker.Length));
                if (ifValue == TRUEVALUESTR ||
                    (double.TryParse(ifValue, NumberStyles.Any, null, out d) && d != 0))
                {
                    formula = formula.Substring(sep1 + 1, sep2 - sep1 - 1);
                }
                else
                {
                    int rightBracket = sep2 + 1;
                    if (FindRightBracket(formula, ref rightBracket))
                    {
                        formula = formula.Substring(sep2 + 1, rightBracket - sep2 - 1);
                    }
                }
            }
            return formula;
        }

        private bool FindNextSeparator(string formula, ref int location)
        {
            int qCount = 0;
            bool found = false;
            while (!found && location < formula.Length)
            {
                if (formula[location] == 'q')
                {
                    qCount++;
                }
                else if (formula[location] == RIGHTBRACKET)
                {
                    qCount--;
                }
                else if (qCount == 0 && formula[location] == ParseArgumentSeparator)
                {
                    found = true;
                    location--;
                }
                location++;
            }
            return found;
        }

        private bool FindRightBracket(string formula, ref int location)
        {
            int qCount = 0;
            bool found = false;
            while (!found && location < formula.Length)
            {
                if (formula[location] == 'q')
                {
                    qCount++;
                }
                else if (qCount == 0 && formula[location] == RIGHTBRACKET)
                {
                    found = true;
                    location--;
                }
                else if (formula[location] == RIGHTBRACKET)
                {
                    qCount--;
                }
                location++;
            }
            return found;
        }

        #endregion

        //parses so an empty string is treated as 0 instead of being an invalid double.
        //used in logical expression calculations
        private bool ParsePossibleEmptyStrings(string s1, string s2, out double d1, out double d2)
        {
            bool b = false;
            d1 = 0;
            d2 = 0;
            if ((s1.Length == 0 || double.TryParse(s1, NumberStyles.Any, null, out d1))
                && (s2.Length == 0 || double.TryParse(s2, NumberStyles.Any, null, out d2)))
            {
                b = true;
            }

            return b;
        }

        private double Pop(Stack<object> _stack)
        {
            object o = _stack.Pop();
            string s = "0";
            if (o != null)
            {
                s = o.ToString().Replace(TIC, "");
                if (s.Length > 0 && s[0] == UNIQUESTRINGMARKER)
                {
                    s = s.Substring(1);
                }
                //change 070704
                if (s == TRUEVALUESTR)
                    return 1;
                else if (s == FALSEVALUESTR)
                    return 0;
                /////////////////////////
            }
            double d;
            if (double.TryParse(s, NumberStyles.Any, null, out d))
                return d;
            else
                //return double.NaN;
                return 0;

            //			object o = _stack.Pop();
            //			if(TextIsEmpty(((string)o)))
            //			{
            //				//throw new ArgumentException("Cell Empty");
            //				o = "0";
            //			}
            //			double d;
            //			if(double.TryParse((string) o, NumberStyles.Any, null, out d))
            //				return d;
            //			else
            //				//return double.NaN;
            //				return 0;

            //return double.Parse((string) o);
        }

        //Logical and string variable used in parsing / computing.
        //System.Text.RegularExpressions.Regex regexValue = null; //used in string matches
        //string oldregexValueCompare = "";
        private double ABSOLUTEZERO = 1e-20;
        //private int TRUEVALUE = 1;
        //private int FALSEVALUE = 0;




        private string PopString(Stack<object> _stack)
        {
            object o = _stack.Pop();
            if (o == null)
            {
                //throw new ArgumentException("Cell Empty");
                o = "";
            }
            return o.ToString();
        }

        private bool IsEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < ABSOLUTEZERO;
        }

        /// <summary>
        /// Returns whether or not the calculation for this GridFormulaTag is current or not.
        /// </summary>
        /// <param name="tag">The GridFormulaTag to be tested.</param>
        /// <returns>Returns true if the tag has been computed with the most recent values, false otherwise.
        /// </returns>
        public bool IsDirty(GridFormulaTag tag)
        {
            return tag == null || tag.Text == null;
        }

        /// <summary>
        /// Marks the underlying formula as dirty, indicating it needs to be recomputed. It does this by setting the Text property to null.
        /// </summary>
        /// <param name="tag">The GridFormulaTag to be marked as not computed.</param>
        public void SetDirty(GridFormulaTag tag)
        {
            if (tag != null)
                tag.Text = null;
        }


        #endregion

        public event GridQueryDependentCellValueEventHandler QueryDependentCellValue;

        #region GridFormulaParsing event

        /// <summary>
        /// Occurs whenever a string needs to be tested to determine whether it should be treated as a formula string and parsed,
        /// or be treated as a non-formula string. This event allows for preprocessing the unparsed formula.
        /// </summary>
        /// <remarks>This event may be raised more than once in the processing of a string into a formula.
        /// </remarks>
        public event GridFormulaParsingEventHandler FormulaParsing;

        /// <summary>
        /// Given a string, it returns a string that is passed through the FormulaParsing event
        /// to allow any listener to modify it.
        /// </summary>
        /// <param name="s">A string that is to be potentially parsed.</param>
        /// <returns>The string returned by GridFormulaParsingEventArgs.Text.</returns>
        internal void GetFormulaText(ref string s)
        {
            if (FormulaParsing != null)
            {
                GridFormulaParsingEventArgs e = new GridFormulaParsingEventArgs(s);
                FormulaParsing(this, e);
                s = e.Text;
            }
        }
        #endregion

        #region Circular Check
        /// <summary>
        /// Determines whether the given formula at the given cell will cause a circular reference.
        /// </summary>
        /// <param name="cell">The alphanumeric cell label, like A1, or EE14.</param>
        /// <param name="formula">The formula to be tested.</param>
        /// <returns>True if the given formula causes a circular reference, false otherwise.</returns>
        public bool IsCircularReference(string cell, string formula)
        {
            string parsedFormula = this.Parse(formula);
            bool save = this.lockDependencies;
            this.lockDependencies = true;
            try
            {
                if (CircularCheckFailed(cell, parsedFormula))
                    return true;
            }
            finally
            {
                this.lockDependencies = save;
            }

            return false;
        }

        internal bool CircularCheckFailed(string hostCell, string parsedFormula)
        {
            GridFormulaEngine engine = this;
            GridSheetFamilyItem family = GridFormulaEngine.GetSheetFamilyItem(this.grid);
            List<string> dependCells = GetCellsList(parsedFormula);
            if (dependCells.IndexOf(hostCell) > -1)
            {
                return true;
            }
            foreach (string s in dependCells)
            {
                string sheet = engine.SheetToken(s);

                GridModel grd1 = (sheet.Length == 0)
                    ? this.grid : family.TokenToGridModel[sheet] as GridModel;
                int row = engine.RowIndex(s);
                int col = engine.ColIndex(s);
                string formula = grd1[row, col].Text;
                engine.GetFormulaText(ref formula);
                bool usesFormulaChar = engine.formulaChar != (char)0 && formula.Length > 0 && formula[0] == engine.formulaChar;
                if (engine.formulaChar == (char)0 || usesFormulaChar)
                {
                    if (grd1[row, col].FormulaTag != null && grd1[row, col].FormulaTag.Formula.Length > 0)
                    {
                        if (CircularCheckFailed(hostCell, grd1[row, col].FormulaTag.Formula))
                            return true;
                    }
                }

            }
            return false;
        }

        internal List<string> GetCellsList(string parsedFormula)
        {
            GridFormulaEngine engine = this;
            List<string> checkList = new List<string>();
            engine.ComputedValue(parsedFormula, checkList);

            //fix for defect 786 - make sure sheet reference is part of cell
            string sheet = engine.SheetToken(engine.cell);
            if (sheet.Length > 0)
            {
                for (int i = 0; i < checkList.Count; ++i)
                {
                    string s = checkList[i].ToString();
                    if (engine.SheetToken(s).Length == 0)
                    {
                        checkList[i] = sheet + s;
                    }
                }
            }

            return checkList;
        }

        #endregion


        public void Dispose()
        {
            grid.CommittedCellInfo -= new GridCommitCellInfoEventHandler(grid_CommittedCellInfo);
            grid = null;
            LibraryFunctions.Clear();
            libraryFunctions = null;
            if (dependentCells != null)
                dependentCells.Clear();
            if (DependentFormulaCells != null)
                DependentFormulaCells.Clear();
            if (DependentNamedRangeCells != null)
                DependentNamedRangeCells.Clear();
            if (errorStrings != null)
                errorStrings.Clear();
            if (namedRanges != null)
                namedRanges.Clear();
            if (namedRangesOriginalNames != null)
                namedRangesOriginalNames.Clear();
            if (namedRangesSized != null)
                namedRangesSized.Clear();
            if (refreshedCells != null)
                refreshedCells.Clear();
            if (sheetFamiliesList != null)
                sheetFamiliesList.Clear();
            if (defaultFamilyItem != null)
            {
                if (defaultFamilyItem.sheetDependentCells != null)
                    defaultFamilyItem.sheetDependentCells.Clear();
                if (defaultFamilyItem.GridModelToToken != null)
                    defaultFamilyItem.GridModelToToken.Clear();
                if (defaultFamilyItem.sheetDependentFormulaCells != null)
                    defaultFamilyItem.sheetDependentFormulaCells.Clear();
                if (defaultFamilyItem.sheetDependentNamedRangeCells != null)
                    defaultFamilyItem.sheetDependentNamedRangeCells.Clear();
                if (defaultFamilyItem.sheetNamedRanges != null)
                    defaultFamilyItem.sheetNamedRanges.Clear();
                if (defaultFamilyItem.sheetNamedRangesOriginalNames != null)
                    defaultFamilyItem.sheetNamedRangesOriginalNames.Clear();
                if (defaultFamilyItem.sheetNamesSized != null)
                    defaultFamilyItem.sheetNamesSized.Clear();
                if (defaultFamilyItem.SheetNameToGridModel != null)
                    defaultFamilyItem.SheetNameToGridModel.Clear();
                if (defaultFamilyItem.SheetNameToToken != null)
                    defaultFamilyItem.SheetNameToToken.Clear();
                if (defaultFamilyItem.TokenToGridModel != null)
                    defaultFamilyItem.TokenToGridModel.Clear();
                if (defaultFamilyItem.TokenToSheetName != null)
                    defaultFamilyItem.TokenToSheetName.Clear();
            }
        }
    }
    #endregion

    #region GridSheetFamilyItem Class
    ///<summary>
    ///Encapsulates the properties that are needed to support multiple families of crossed-referenced grids.
    ///This class is for internal use only.
    ///</summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridSheetFamilyItem
    {
        internal Hashtable sheetDependentCells = null;
        internal Hashtable sheetDependentFormulaCells = null;
        internal Hashtable sheetDependentNamedRangeCells = null;
        internal Hashtable sheetNamedRanges = null;
        internal Hashtable sheetNamedRangesOriginalNames = null;
        internal Hashtable SheetNameToGridModel = null;
        internal Hashtable TokenToGridModel = null;
        internal Hashtable GridModelToToken = null;
        internal Hashtable SheetNameToToken = null;
        internal Hashtable TokenToSheetName = null;
        internal bool isSheeted = false;
        internal List<string> sheetNamesSized = null;
    }
    #endregion

    #region GridQueryCellValueEventArgs class

    public delegate void GridQueryDependentCellValueEventHandler(object sender, GridQueryDependentCellValueEventArgs e);

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridQueryDependentCellValueEventArgs : EventArgs
    {

        #region Filed
        int rowIndex;
        int columnIndex;
        bool cancel;
        string cellValue;
        #endregion

        public GridQueryDependentCellValueEventArgs()
        {

        }

        public GridQueryDependentCellValueEventArgs(int rowindex, int columnindex)
        {
            rowIndex = rowindex;
            columnIndex = columnindex;
        }

        public int RowIndex
        {
            get { return rowIndex; }
        }

        public int ColumnIndex
        {
            get { return columnIndex; }
        }

        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }

        public string CellValue
        {
            get { return cellValue; }
            set { cellValue = value; }
        }
    }

    #endregion

    #region GridFormulaParsingEventArgs class

    /// <summary>
    /// Event delegate for the GridFormulaParsing event 
    /// </summary>
    public delegate void GridFormulaParsingEventHandler(object sender, GridFormulaParsingEventArgs e);

    /// <summary>
    /// Used by the <see cref="GridFormulaParsing"/> event, GridFormulaParsingEventArgs holds a reference 
    /// to the string that is to be parsed. The GridFormulaParsing event allows the listener to preprocess
    /// the string that is being parsed.
    /// </summary>
    /// <remarks>
    /// Please note that this event may be raised more than once as a string is parsed. 
    /// </remarks>
    /// <example> Here is a code snippet that shows how to tell a grid to also treat any text in a formula cell 
    /// that begins with a minus(-) or a plus(+) as formulas. The default behavior is to treat only text beginning
    /// with equal(=) as formulas.
    /// <code lang="C#">
    ///		//subscribe to the event before any formulas are loaded into the grid...
    ///		this.engine = ((GridCellFormulaModel)gridControl1.CellModels["FormulaCell"]).Engine;
    ///     this.engine.FormulaParsing += new GridFormulaParsingEventHandler(engine_FormulaParsing);
    ///		
    ///		//Here is the handler code that adds an = if necessary so any string beginning with +, - or =
    ///     //is treated as a formula.
    ///       void engine_FormulaParsing(object sender, GridFormulaParsingEventArgs e)
    ///       {
    ///           //allow cells starting with + and - to be treated as formula cells.
    ///           if (e.Text.StartsWith("-"))
    ///                e.Text = "=" + e.Text;
    ///           else if (e.Text.StartsWith("+"))
    ///                e.Text = "=" + e.Text.Substring(1);
    ///       }         
    /// </code>
    /// </example>
    public class GridFormulaParsingEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GridFormulaParsingEventArgs()
        {
        }

        /// <summary>
        /// Holds a reference to the string that is to be parsed..
        /// </summary>
        /// <param name="text">The formula that is to be parsed.</param>
        public GridFormulaParsingEventArgs(string text)
        {
            this.text = text;
        }

        string text;

        /// <summary>
        /// Get or sets the formula about to be parsed.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }

    #endregion

    #region GridShowFormulaBehavior enums

    /// <summary>
    /// Specifies behavior for displaying formula text.
    /// </summary>
    public enum GridShowFormulaBehavior
    {
        /// <summary>
        /// Display the formula text only when the cell is actively being edited.
        /// </summary>
        WhenEditing,
        /// <summary>
        /// Display the formula text whenever the cell is the current cell.
        /// </summary>
        WhenCurrent,
        /// <summary>
        /// Always display the formula text.
        /// </summary>
        Always,
        /// <summary>
        /// Never display the formula text.
        /// </summary>
        Never
    };
    #endregion
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class Hashtable : Dictionary<object, object>
    {
        public new object this[object key]
        {
            get
            {
                object value = null;
                this.TryGetValue(key, out value);
                return value;
            }

            set
            {
                this.Add(key, value);
            }
        }

        public Hashtable Clone()
        {
            var newTable = new Hashtable();
            foreach (var kvp in this)
            {
                newTable.Add(kvp.Key, kvp.Value);
            }

            return newTable;
        }

        public bool Contains(object key)
        {
            return this.ContainsKey(key);
        }
    }
}

