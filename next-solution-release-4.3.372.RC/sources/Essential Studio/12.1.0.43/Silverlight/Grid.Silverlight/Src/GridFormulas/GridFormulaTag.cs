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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Runtime.Serialization;

#if !WinRT
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using System.Security.Permissions;

namespace Syncfusion.Windows.Controls.Grid
{
#else

namespace Syncfusion.WinRT.Controls.Grid
{
#endif
	#region GridFormulaTag class

	/// <summary>
	/// Support class that encapsulates the parsed formula and computed value
	/// for a cell.
	/// </summary>
	/// <remarks>
	/// This class holds the parsed value of the formula in its Formula property,
	/// and holds the computed value of the formula in its Text property. Each
	/// cell that is a formula cell stores a GridFormulaTag object in its GridStyleInfo.FormulaTag
	/// property. When the cell is drawn, if its GridFormulaTag.Formula property is empty,
	/// the formula in the GridStyleInfo.Text property is parsed, and placed into the
	/// GridFormulaTag.Formula property. If the GridFormulaTag.Text property is empty, the 
	/// GridFormulaTag.Formula is computed, and the computed value is stored in GridFormulaTag.Text.
	/// Thus, formulas are parsed and computed only when the GridFormulaTag member is empty,
	/// otherwise, the stored values are used. Emptying these properties is how the
	/// Formula Engine initiates a refresh of the calculated values when dependent
	/// cells are modified.
	/// </remarks>

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
	public class GridFormulaTag : ICloneable
	{
		private string _formula;
		private string _text;
		private int _parsedRow;
		private int _parsedCol;

		/// <summary>
        /// Default Constructor.
        /// </summary>
        public GridFormulaTag()
        {
            _formula = "";
            _text = "";
            _parsedRow = -1;
            _parsedCol = -1;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="formula">The parsed formula.</param>
		/// <param name="text">The computed value.</param>
		public GridFormulaTag(string formula, string text)
		{
			_formula = formula;
			_text = text;
			_parsedRow = -1;
			_parsedCol = -1;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="formula">The parsed formula.</param>
		/// <param name="text">The computed value.</param>
		/// /// <param name="row">The row at which this formula is being parsed.</param>
		/// <param name="col">The col at which this formula is being parsed.</param>
		public GridFormulaTag(string formula, string text, int row, int col)
		{
			_formula = formula;
			_text = text;
			_parsedRow = row;
			_parsedCol = col;
		}
		
		/// <summary>
		/// Initializes a new <see cref="GridFormulaTag"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
	

		/// <summary>
		/// Gets / sets the parsed formula.
		/// </summary>
		public string Formula
		{
			get{ return _formula;}
			set{ _formula = value;}
		}

		/// <summary>
		/// Gets the column where the formula was last parsed.
		/// </summary>
		public int ParsedRow
		{
			get{ return _parsedRow;}
			set{ _parsedRow = value;}
		}

		/// <summary>
		/// Gets the column where the formula was last parsed.
		/// </summary>
		public int ParsedCol
		{
			get{ return _parsedCol;}
			set{ _parsedCol = value;}
		}

		/// <summary>
		/// Get / sets text representing the computed value.
		/// </summary>
		public string Text
		{
			get{ return _text;}
			set
			{ 
				_text = value;
			}
		}

		/// <summary>
		/// Overriden.
		/// </summary>
		/// <returns>Displays the parsed formula and computed value.</returns>
		public override string ToString()
		{
			return string.Format("GridFormulaTag({2},{3}) [ {0}-> {1}]", _formula, _text, _parsedRow, _parsedCol);
		}

		

		#region Implementation of ICloneable
		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		/// <returns>A copy of this object.</returns>
		[DebuggerStepThrough()] public object Clone()
		{
			return new GridFormulaTag(this.Formula, this.Text, this.ParsedRow, this.ParsedCol);
		}
		#endregion

        /// <summary>
        /// Sets Text and Formula to null and ParsedRow and ParsedCol to -1.
        /// </summary>
        public void Reset()
        {
            _formula = null;
            _text = null;
            _parsedRow = -1;
            _parsedCol = -1;
        }

        /// <summary>
        /// Sets Text to null.
        /// </summary>
        public void ResetText()
        {
            _text = null;
        }

        /// <summary>
        /// Sets Text and Formula to null.
        /// </summary>
        public void ResetFormula()
        {
            _formula = null;
            _text = null;
        }
	}
	#endregion
}
