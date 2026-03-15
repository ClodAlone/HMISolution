//-------------------------------------------------------------------------------------------------
// <copyright file="GridTextBoxCellModel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Globalization;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for a text box cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridTextBoxCellModel"/> can serve as model for several <see cref="GridTextBoxCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridTextBoxCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridTextBoxCellModel : GridStaticCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridTextBoxCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTextBoxCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTextBoxCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = true;
            AllowMerging = true;
        }

        /// <summary>
        /// Initializes a new <see cref="GridTextBoxCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTextBoxCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
               ;
#endif
        }

        /// <override/>
        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridTextBoxCellRenderer(control, this);
        }

        /// <override/>
        /// <summary>
        /// Return formatted text for the specified value.
        /// GridStyleInfo.CultureInfo is used for conversion to string.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>Formatted text.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(string.Empty, style, value, textInfo);
            Grid.RaiseQueryCellFormattedText(ea);
            if (ea.Handled)
            {
                return ea.Text;
            }

            if (value != null && value is string && ((string)value).Length > 0)
            {
                if (textInfo != GridCellBaseTextInfo.CurrentText)
                {
                    char pwc = style.PasswordChar;
                    if (pwc != ' ')
                    {
                        return new string(pwc, value.ToString().Length);
                    }
                }

                CharacterCasing casing = style.CharacterCasing;
                if (casing == CharacterCasing.Lower)
                {
                    return value.ToString().ToLower(style.GetCulture(true));
                }
                else if (casing == CharacterCasing.Upper)
                {
                    return value.ToString().ToUpper(style.GetCulture(true));
                }
            }

            //// copied from base.GetFormattedText(style, value, textInfo);
            CultureInfo ci = style.CultureInfo;
            NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
            return GridCellValueConvert.FormatValue(value, style.CellValueType, style.Format, ci, nfi);
        }
    }
}
