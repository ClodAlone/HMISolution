//-------------------------------------------------------------------------------------------------
// <copyright file="GridCheckBoxCellModel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for a check box cell. 
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridCheckBoxCellModel"/> can serve as model for several <see cref="GridCheckBoxCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// </remarks>
    [Serializable]
    public class GridCheckBoxCellModel : GridCellModelBase
    {
        /// <overload>
        /// Initializes a new <see cref="GridCheckBoxCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridCheckBoxCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridCheckBoxCellModel(SerializationInfo info, StreamingContext context)
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

        /// <summary>
        /// Initializes a new <see cref="GridCheckBoxCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridCheckBoxCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Creates a <see cref="GridCheckBoxCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridCheckBoxCellRenderer"/> specific for a <see cref="GridControlBase"/>.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridCheckBoxCellRenderer(control, this);
        }

        /// <override/>
        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// GridStyleInfo.CultureInfo is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="str">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string str, int textInfo)
        {
            // Load representations for True / False.
            string strTrue = string.Empty, strFalse = string.Empty;
            GridCheckBoxCellInfo ua = style.ReadOnlyCheckBoxOptions;
            if (ua != null && ua.CheckedValue != null)
            {
                strTrue = ua.CheckedValue;
            }

            if (GridUtil.IsEmpty(strTrue))
            {
                strTrue = "1";
            }

            if (ua != null && ua.UncheckedValue != null)
            {
                strFalse = ua.UncheckedValue;
            }

            if (GridUtil.IsEmpty(strFalse))
            {
                strFalse = "0";
            }

            string strIndeterm;
            if (ua != null && ua.IndetermValue != null)
            {
                strIndeterm = ua.IndetermValue;
            }
            else
            {
                strIndeterm = string.Empty;
            }

            bool bTriState = style.TriState;
            CultureInfo culture = style.GetCulture(true);

            if (str.ToUpper(culture).Equals(strTrue.ToUpper(culture)))
            {    // valid str
            }
            else if (!bTriState || str.ToUpper(culture).Equals(strFalse.ToUpper(culture)))
            {
                str = strFalse;
            }
            else
            {
                str = String.Empty;
            }

            style.Text = str;

            return true;
        }

        /// <override/>
        /// <summary>
        /// Determines whether the cell supports floating over a neighboring cell or can be
        /// flooded by a previous cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="query">A <see cref="GridQueryFloatCell"/> value that specifies whether a cell is asked
        /// about support for floating over another cell or being flooded by a previous cell.</param>
        /// <returns>True if floating is possible; False otherwise.</returns>
        public override bool OnQueryCanFloatCell(int rowIndex, int colIndex, GridStyleInfo style, GridQueryFloatCell query)
        {
            return query == GridQueryFloatCell.FloatCell;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal Size checkBoxSize = new Size(13, 13);

        /// <summary>Gets or sets CheckBoxSize. For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public Size CheckBoxSize
        {
            get
            {
                return checkBoxSize;
            }

            set
            {
                checkBoxSize = value;
            }
        }

        /// <override/>
        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">The GridQueryBounds</param>
        /// <returns>The optimal size of the cell.</returns>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size checkerSize = this.checkBoxSize;
            Font font = style.GdipFont;
            Size sizeWg = WinFormsUtils.MeasureSampleWString(g, font);
            string text = style.Description;
            if (text.Length > 0)
            {
                Size size;
                if (queryBounds == GridQueryBounds.Height)
                {
                    Size clientSize = GetCellClientSize(rowIndex, colIndex, style);
                    Size textSize = GridMargins.RemoveMargins(clientSize, style.ReadOnlyTextMargins.ToMargins());

                    size = g.MeasureString(text, font, Math.Max(checkerSize.Width, textSize.Width - checkerSize.Width)).ToSize();
                }
                else
                {
                    size = g.MeasureString(text, font).ToSize();
                }

                checkerSize = GridUtil.Max(size, sizeWg);
            }

            checkerSize.Width += ((checkBoxSize.Width * 3) / 2) + style.ReadOnlyTextMargins.Left;
            checkerSize.Height = Math.Max(checkerSize.Height, checkBoxSize.Height);

            return checkerSize;
        }
    }
}
