//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownColorUICellModel.cs" company="syncfusion">
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
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part of a drop-down color selection cell that lets users drop-down a
    /// color selection panel from a cell just like a combo box.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridDropDownColorUICellModel"/> can serve as model for several <see cref="GridDropDownColorUICellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridDropDownColorUICellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridDropDownColorUICellModel : GridDropDownCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridDropDownColorUICellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDropDownColorUICellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridDropDownColorUICellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(21, 0);
        }

        /// <summary>
        /// Initializes a new <see cref="GridDropDownColorUICellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDropDownColorUICellModel(SerializationInfo info, StreamingContext context)
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
            return new GridDropDownColorUICellRenderer(control, this);
        }

        ////        public override GridStyleInfo PrepareViewStyleInfo(int rowIndex, int colIndex, GridStyleInfo style)
        ////        {
        ////            ////string displayText = style.FormattedText;
        ////            try
        ////            {
        ////                string displayText = GetFormattedOrActiveTextAt(rowIndex, colIndex, style);
        ////                if (displayText.Length > 0)
        ////                {
        ////                    Color color = ColorConvert.ColorFromString(displayText);
        ////                    GridStyleInfo styleCopy = style.GetOffLineCopy();
        ////                    styleCopy.Interior = new BrushInfo(color);
        ////                    return styleCopy;
        ////                }
        ////            }
        ////            catch
        ////            {
        ////            }
        ////            return style;
        ////        }
    }
}

