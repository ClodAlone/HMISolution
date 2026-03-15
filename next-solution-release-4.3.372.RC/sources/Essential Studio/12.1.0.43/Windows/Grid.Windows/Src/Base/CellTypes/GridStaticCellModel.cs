//-------------------------------------------------------------------------------------------------
// <copyright file="GridStaticCellModel.cs" company="syncfusion">
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
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the data / model part of a static cell. A static cell is also a base class for many
    /// other cell types that display text.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridStaticCellModel"/> can serve as model for several <see cref="GridStaticCellRenderer"/>
    /// instances if a there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridStaticCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridStaticCellModel: GridCellModelBase
    {
        bool allowFloating = false;
        bool allowMerging = false;

        /// <overload>
        /// Initializes a new <see cref="GridStaticCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridStaticCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridStaticCellModel(GridModel grid)
            : base(grid)
        {
            this.allowMerging = GetType().IsAssignableFrom(typeof(GridStaticCellModel));
        }

        /// <summary>
        /// Initializes a new <see cref="GridStaticCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridStaticCellModel(SerializationInfo info, StreamingContext context)
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

            this.allowFloating = info.GetBoolean("AllowFloating");

            if (SerializeSchemeVersion > 0)
            {
                this.allowMerging = info.GetBoolean("AllowMerging");
            }
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
            info.AddValue("AllowFloating", this.allowFloating); // Boolean
            info.AddValue("AllowMerging", this.allowMerging); // Boolean
            base.GetObjectData(info, context);
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridStaticCellRenderer(control, this);
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
            string sOutput = String.Empty;

            try
            {
                sOutput = this.GetFormattedOrActiveTextAt(rowIndex, colIndex, style);
                ////Trace.WriteLine(String.Format("OnQueryPrefferedClientSize({0}, {1}, {2})", rowIndex, colIndex, sOutput));
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
#if DEBUG
                Trace.WriteLineIf(Switches.ValueConversion.TraceWarning, ex.ToString());
#endif
            }

            Font font = style.GdipFont;
            Size clientSize = GetCellClientSize(rowIndex, colIndex, style);
            GridMargins margins = style.ReadOnlyTextMargins.ToMargins();
            Size textSize = GridMargins.RemoveMargins(clientSize, margins);

            if (GridUtil.IsEmpty(sOutput))
            {
                textSize = WinFormsUtils.MeasureSampleWString(g, font);
            }
            else
            {
                StringFormat format = new StringFormat();
                format.LineAlignment = GridUtil.ConvertToStringAlignment(style.VerticalAlignment);
                format.Alignment = GridUtil.ConvertToStringAlignment(style.HorizontalAlignment);
                bool allowWrapText = style.WrapText;
                if (!allowWrapText)
                {
                    format.FormatFlags = StringFormatFlags.NoWrap;
                }

                int angle = style.ReadOnlyFont.Orientation;
                Size sizeWg = WinFormsUtils.MeasureSampleWString(g, font);

                if (queryBounds == GridQueryBounds.Height)
                {
                    if (GridUtil.IsEmpty(sOutput))
                    {
                        textSize.Height = sizeWg.Height;
                    }
                    else if (angle != 0)
                    {
                        textSize = RotatePaint.MeasureStringBounds(g, sOutput, font, int.MaxValue, format, angle).ToSize();
                    }
                    else
                    {
                        textSize.Height = (int) g.MeasureString(sOutput, font, textSize.Width, format).Height;
                    }
                }
                else 
                {
                    // GridQueryBounds.Width
                    int savedHeight = textSize.Height;
                    textSize = g.MeasureString(sOutput, font, textSize.Width, format).ToSize();
                    int height = 0;
                    int width = 0;

                    // Increase width to be the size of a line in the text
                    // if the cell is too small for the text to fit or if wraptext is false.
                    if (!allowWrapText || textSize.Height > savedHeight)
                    {
                        // Determine the width for each line of text (separated with a newline).
                        string[] lines = sOutput.Split(new char[] { '\r', '\n' });
                        foreach (string line in lines)
                        {
                            Size lineSize = g.MeasureString(line, font, Point.Empty, format).ToSize();
                            width = Math.Max(width, lineSize.Width);
                            height += lineSize.Height;
                        }

                        textSize.Width = width;
                    }

                    if (angle != 0)
                    {
                        textSize = RotatePaint.MeasureStringBounds(new SizeF(textSize.Width, textSize.Height), angle).ToSize();
                    }
                }

                format.Dispose();
            }

            GridMargins m = style.ReadOnlyTextMargins.ToMargins();
            textSize.Width += m.Width+1; // TODO: +1 might be done in CalculatePreferredCellSize.
            textSize.Height += m.Height+1;

            int imageIndex = style.ImageIndex;
            ImageList imageList = style.ImageList;
            if (imageIndex != -1 && imageList != null && imageIndex < imageList.Images.Count)
            {
                textSize.Height = Math.Max(textSize.Height, imageList.ImageSize.Height);
                textSize.Width += imageList.ImageSize.Width+2;
            }

            return textSize;
        }

        /// <override/>
        /// <summary>
        /// Determines whether the cell supports merging of neighboring cells.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="mergeCellDirection">Specifies if rows or columns should be merged.</param>
        /// <returns>True if merging is possible; False otherwise.</returns>
        public override bool OnQueryCanMergeCell(int rowIndex, int colIndex, GridStyleInfo style, GridMergeCellDirection mergeCellDirection)
        {
            if (!AllowMerging)
            {
                return false;
            }

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
            if (!AllowFloating)
            {
                return false;
            }

            string text = this.GetFormattedOrActiveTextAt(rowIndex, colIndex, style);

            // Float over other cells if value is not empty.
            if (query == GridQueryFloatCell.FloatCell)
            {
                return text.Length > 0;
            }
            else
            {
                int imageIndex = style.ImageIndex;
                ImageList imageList = style.ImageList;
                if (imageIndex != -1 && imageList != null && imageIndex < imageList.Images.Count)
                {
                    return false;
                }

                // Static cells can be flooded by other floatable cells if value is empty (or default).
                return GridUtil.IsEmpty(text) && GetActiveText(rowIndex, colIndex) == null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this celltype supports being floated or flooded.
        /// </summary>
        public bool AllowFloating
        {
            get
            {
                return this.allowFloating;
            }

            set
            {
                this.allowFloating = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this celltype supports being merged.
        /// </summary>
        public bool AllowMerging
        {
            get
            {
                return this.allowMerging;
            }

            set
            {
                this.allowMerging = value;
            }
        }
    }
}
