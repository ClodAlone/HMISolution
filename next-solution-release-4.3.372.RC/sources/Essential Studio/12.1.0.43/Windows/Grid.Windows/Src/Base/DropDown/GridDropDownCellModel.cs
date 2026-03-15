//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownCellModel.cs" company="syncfusion">
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
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for a drop-down cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridDropDownCellModel"/> can serve as model for several <see cref="GridDropDownCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridDropDownCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridDropDownCellModel : GridTextBoxCellModel
    {
        private bool supportsChoiceList = false;

        /// <overload>
        /// Initializes a new <see cref="GridDropDownCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDropDownCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridDropDownCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            AllowMerging = false;
            ////ButtonBarSize = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.VerticalScrollBarArrowHeight);
            ButtonBarSize = new Size(15, 18);
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the cell model.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the cell model.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("SupportsChoiceList", supportsChoiceList); // Boolean
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Initializes a new <see cref="GridDropDownCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDropDownCellModel(SerializationInfo info, StreamingContext context)
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
            supportsChoiceList = info.GetBoolean("SupportsChoiceList");
        }
        
        /// <override/>
        /// <summary>Throws NotImplemented exception.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>NotImplemented exception.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            throw new NotImplementedException("GridCellRenderer is not implemented in derived class");
            ////return null;
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">grsphical bounds</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <override/>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {            
            Size textSize = base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
            Size comboSize = Size.Empty;  
            Font font = style.GdipFont;
            if (queryBounds == GridQueryBounds.Height)
            {
                comboSize = GridMargins.AddMargins(WinFormsUtils.MeasureSampleWString(g, font), style.ReadOnlyTextMargins.ToMargins());
                if (ButtonBarSize.Height > comboSize.Height && ButtonBarSize.Height < int.MaxValue)
                {
                    comboSize.Height = ButtonBarSize.Height;
                }

                comboSize.Height++;
            }
            else
            {
                if (supportsChoiceList && style.ChoiceList != null)
                {
                    foreach (string item in style.ChoiceList)
                    {
                        Size size = GridMargins.AddMargins(g.MeasureString(item, font).ToSize(), style.ReadOnlyTextMargins.ToMargins());
                        comboSize.Width = Math.Max(comboSize.Width, size.Width);
                    }

                    comboSize.Width++;
                }
            }

            return GridUtil.Max(comboSize, textSize);
        }

        /// <summary>
        /// Gets or sets a value indicating whether cells with this cell type should fill
        /// the drop-down list with items from the choice list or datasource in GridStyleInfo.
        /// </summary>
        /// <remarks>
        /// If you implement a custom combo box or drop-down list and you do want the item list
        /// to be independent from cell settings (and share the same item list among all cells
        /// with this cell type) you should set SupportsChoiceList to be False.</remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsChoiceList
        {
            get
            {
                return supportsChoiceList;
            }

            set
            {
                supportsChoiceList = value;
            }
        }
    }
}
