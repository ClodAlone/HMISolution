//-------------------------------------------------------------------------------------------------
// <copyright file="GridFormulaTag.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Windows.Forms;
using Syncfusion.Styles;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
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
    [Serializable]
    public class GridFormulaTag : ISerializable, ICloneable
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
            _formula = string.Empty;
            _text = string.Empty;
            _parsedRow = -1;
            _parsedCol = -1;
        }

        /// <summary>
        /// Constructor for GridFormulaTag.
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
        /// Constructor for GridFormulaTag.
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
        protected GridFormulaTag(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            this._parsedRow = (int)info.GetValue("_parsedRow", typeof(int));
            this._parsedCol = (int)info.GetValue("_parsedCol", typeof(int));
            this._formula = (string)info.GetValue("_formula", typeof(string));
            this._text = (string)info.GetValue("_text", typeof(string));
        }

        /// <summary>
        /// Gets or sets the parsed formula.
        /// </summary>
        public string Formula
        {
            get { return _formula; }
            set { _formula = value; }
        }

        /// <summary>
        /// Gets or sets the column where the formula was last parsed.
        /// </summary>
        public int ParsedRow
        {
            get { return _parsedRow; }
            set { _parsedRow = value; }
        }

        /// <summary>
        /// Gets or sets the column where the formula was last parsed.
        /// </summary>
        public int ParsedCol
        {
            get { return _parsedCol; }
            set { _parsedCol = value; }
        }

        /// <summary>
        /// Gets or sets text representing the computed value.
        /// </summary>
        public string Text
        {
            get 
            { 
                return _text; 
            }

            set
            {
                ////                if(value.Length == 0)
                ////                {
                ////                    GridFormulaTag.DisplayChange("zap", this);
                ////                }
                _text = value;
                ////                GridFormulaTag.DisplayChange("set", this);
            }
        }

        /// <summary>
        /// Overriden ToString method.
        /// </summary>
        /// <returns>Displays the parsed formula and computed value.</returns>
        public override string ToString()
        {
            return string.Format("GridFormulaTag({2},{3}) [ {0}-> {1}]", _formula, _text, _parsedRow, _parsedCol);
            ////return "GridFormulaTag { " + _formula + "-> " + _text + "}";
        }

        #region Implementation of ISerializable
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("_parsedRow", _parsedRow);
            info.AddValue("_parsedCol", _parsedCol);
            info.AddValue("_formula", _formula);
            info.AddValue("_text", _text);
        }
        #endregion

        #region Implementation of ICloneable
        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>A copy of this object.</returns>
        [DebuggerStepThrough()]
        public object Clone()
        {
            return new GridFormulaTag(this.Formula, this.Text, this.ParsedRow, this.ParsedCol);
        }
        #endregion

        ////        private static string timeStamp = "0";
        ////        public static void DisplayChange(string type, GridFormulaTag tag)
        ////        {
        ////            //            string s = DateTime.Now.ToString("hhmmss");
        ////            //            if(int.Parse(s) - int.Parse(timeStamp) > 2)
        ////            //            {
        ////            //                Console.WriteLine("==================================");
        ////            //                timeStamp = s;
        ////            //            }
        ////            //            Console.WriteLine(string.Format("{3}:rowcol({1},{2}): {4} tag.Text:[{0}]", tag.Text,  tag.ParsedRow, tag.ParsedCol,  s, type));
        ////            //                
        ////        }
    }
    #endregion
}
