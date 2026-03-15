//-------------------------------------------------------------------------------------------------
// <copyright file="GridNumericUpDownCellModel.cs" company="syncfusion">
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
using System.Windows.Forms;
using System.Text;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the data / model part of a numeric up / down cell that lets users increase and decrease
    /// values with spin buttons.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridNumericUpDownCellModel"/> can serve as model for several <see cref="GridNumericUpDownCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridNumericUpDownCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridNumericUpDownCellModel : GridTextBoxCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridNumericUpDownCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridNumericUpDownCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridNumericUpDownCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(13, int.MaxValue);
        }

        /// <summary>
        /// Initializes a new <see cref="GridNumericUpDownCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridNumericUpDownCellModel(SerializationInfo info, StreamingContext context)
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
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridNumericUpDownCellRenderer(control, this);
        }

        private bool acceptAlphaKeys = true;

        /// <summary>
        /// Gets or sets a value indicating whether NumericUpDown cell should accept Alpha keys
        /// </summary>
        /// <example>The following code shows how to get a reference to the Model and change this property:
        /// <code lang="C#">
        /// GridNumericUpDownCellModel cm = (GridNumericUpDownCellModel) this.gridControl1.CellModels["NumericUpDown"];
        /// cm.AcceptAlphaKeys = false;
        /// </code>
        /// <para/>
        /// <code lang="VB">
        /// Dim cm As GridNumericUpDownCellModel = CType(Me.gridControl1.CellModels("NumericUpDown"), GridNumericUpDownCellModel)
        /// cm.AcceptAlphaKeys = False
        /// </code>
        /// </example>
        public bool AcceptAlphaKeys
        {
            get
            {
                return this.acceptAlphaKeys;
            }

            set
            {
                this.acceptAlphaKeys = value;
            }
        }
    }
}
