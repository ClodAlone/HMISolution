//-------------------------------------------------------------------------------------------------
// <copyright file="FilterBarButtons.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using System.Text;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;

    using Syncfusion.ComponentModel;
    using Syncfusion.Diagnostics;
    using Syncfusion.Grouping;
    using Syncfusion.Windows.Forms;
    using Syncfusion.Collections.BinaryTree;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;

    /// <summary>
    /// Defines a filter button which is typically used with <see cref="GridDynamicFilter"/>.
    /// </summary>
    public class FilterButton : GridCellButton
    {
        /// <summary>
        /// Constructor for FilterButton.
        /// </summary>
        /// <param name="control">Filter text box cell renderer control.</param>
        public FilterButton(GridTextBoxCellRenderer control)
            : base(control)
        {
        }

        /// <override/>
        /// <summary>
        /// Draws the filter button at the specified row index and column index.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="bActive">True if this is the active current cell.</param>
        /// <param name="style">Cell style information.</param>
        public override void Draw(Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, bActive, style.CellValue);

            base.Draw(g, rowIndex, colIndex, bActive, style);

            // draw the button
            bool hovering = IsHovering(rowIndex, colIndex);
            bool mouseDown = IsMouseDown(rowIndex, colIndex);
            bool disabled = !style.Clickable;

            ButtonState buttonState = ButtonState.Normal;

            if (disabled)
            {
                buttonState |= ButtonState.Inactive | ButtonState.Flat;
            }
            else if (!hovering && !mouseDown)
            {
                buttonState |= ButtonState.Flat;
            }

            Point ptOffset = Point.Empty;
            if (mouseDown)
            {
                ptOffset = new Point(1, 1);
                buttonState |= ButtonState.Pushed;
            }

            DrawButton(g, Bounds, buttonState, style);

            GridTableFilterBarExtCellRenderer renderer = (GridTableFilterBarExtCellRenderer)Owner;
            Bitmap bm = renderer.Model.GetCompareOperatorImage(style);

            // with an existing bitmap. GridIconPaint is convenient because it lets
            // us easily draw over existing background and replace the black color
            // in the bitmap with a different color.
            // GridIconPaint.
            DynamicFilterBitmaps.IconPainter.PaintIcon(g, Bounds, ptOffset, bm, Color.Blue);
        }
    }

    /// <summary>
    /// Implements a cell button which is typically used with grid filter bar to clear the existing filter.
    /// </summary>
    public class ClearFilterButton : GridCellButton
    {
        /// <summary>
        /// Constructor for ClearFilterButton.
        /// </summary>
        /// <param name="control">The text box cell renderer.</param>
        public ClearFilterButton(GridTextBoxCellRenderer control)
            : base(control)
        {
        }

        /// <override/>
        /// <summary>
        /// Draws a cell button at the specified row index and column index.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="bActive">True if this is the active current cell.</param>
        /// <param name="style">Cell style information.</param>
        public override void Draw(Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, bActive, style.CellValue);

            base.Draw(g, rowIndex, colIndex, bActive, style);

            // draw the button
            bool hovering = IsHovering(rowIndex, colIndex);
            bool mouseDown = IsMouseDown(rowIndex, colIndex);
            bool disabled = !style.Clickable;

            ButtonState buttonState = ButtonState.Normal;
            if (disabled)
            {
                buttonState |= ButtonState.Inactive | ButtonState.Flat;
            }
            else if (!hovering && !mouseDown)
            {
                buttonState |= ButtonState.Flat;
            }

            Point ptOffset = Point.Empty;
            if (mouseDown)
            {
                ptOffset = new Point(1, 1);
                buttonState |= ButtonState.Pushed;
            }

            DrawButton(g, Bounds, buttonState, style);
            GridTableFilterBarExtCellRenderer renderer = (GridTableFilterBarExtCellRenderer)Owner;

            Bitmap bm = null;
            bool filteredColumn = false;
            foreach (RecordFilterDescriptor filterdesc in renderer.Grid.TableDescriptor.RecordFilters)
            {
                if (filterdesc.MappingName == (style as GridTableCellStyleInfo).TableCellIdentity.Column.MappingName)
                {
                    filteredColumn = true;
                    break;
                }
            }
            if (renderer.Model.HasFilter((style as GridTableCellStyleInfo).TableCellIdentity) && filteredColumn)
            {
                switch (renderer.Grid.Model.Options.GridVisualStyles)
                {
                    case GridVisualStyles.Office2010Black:
                    case GridVisualStyles.Office2010Blue:
                    case GridVisualStyles.Office2010Silver:
                        bm = DynamicFilterBitmaps.GetBitmap("filter2010_delete");
                        break;
                    case GridVisualStyles.Metro:
                        bm = DynamicFilterBitmaps.GetBitmap("filtered_metro");
                        break;
                    default:
                        bm = DynamicFilterBitmaps.GetBitmap("filter_delete"); // make sure this is included in project and marked as "Embedded Resource"
                        break;
                }
            }
            else
            {
                switch (renderer.Grid.Model.Options.GridVisualStyles)
                {
                    case GridVisualStyles.Office2010Black:
                    case GridVisualStyles.Office2010Blue:
                    case GridVisualStyles.Office2010Silver:
                        bm = DynamicFilterBitmaps.GetBitmap("filter2010");
                        break;
                    case GridVisualStyles.Metro:
                        bm = DynamicFilterBitmaps.GetBitmap("filter_metro");
                        break;
                    default:
                        bm = DynamicFilterBitmaps.GetBitmap("filter");
                        break;
                }
            }

            // Instead of using GridIconPaint you could simple use Image.Draw here
            // with an existing bitmap. GridIconPaint is convenient because it lets
            // us easily draw over existing background and replace the black color
            // in the bitmap with a different color.
            DynamicFilterBitmaps.IconPainter.PaintIcon(g, Bounds, ptOffset, bm, Color.Blue);
        }
    }
    /// <summary>
    /// Defines a filter button which is typically used with <see cref="GridDynamicFilter"/>.
    /// </summary>
    public class FilterButtonExt : GridCellButton
    {
        /// <summary>
        /// Constructor for FilterButton.
        /// </summary>
        /// <param name="control">Filter text box cell renderer control.</param>
        public FilterButtonExt(GridTextBoxCellRenderer control)
            : base(control)
        {
        }

        /// <override/>
        /// <summary>
        /// Draws the filter button at the specified row index and column index.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="bActive">True if this is the active current cell.</param>
        /// <param name="style">Cell style information.</param>
        public override void Draw(Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, bActive, style.CellValue);
            base.Draw(g, rowIndex, colIndex, bActive, style);
            bool hovering = IsHovering(rowIndex, colIndex);
            bool mouseDown = IsMouseDown(rowIndex, colIndex);
            bool disabled = !style.Clickable;
            ButtonState buttonState = ButtonState.Normal;

            if (disabled)
            {
                buttonState |= ButtonState.Inactive | ButtonState.Flat;
            }
            else if (!hovering && !mouseDown)
            {
                buttonState |= ButtonState.Flat;
            }
            Point ptOffset = Point.Empty;
            if (mouseDown)
            {
                ptOffset = new Point(1, 1);
                buttonState |= ButtonState.Pushed;
            }

            DrawButton(g, Bounds, buttonState, style);
            GridListFilterBarCellRenderer renderer = (GridListFilterBarCellRenderer)Owner;
            Bitmap bm = renderer.Model.GetCompareOperatorImage(style);
            if (bm == null)
                bm = DynamicFilterBitmaps.GetBitmap("StartsWith");
            DynamicFilterBitmaps.IconPainter.PaintIcon(g, Bounds, ptOffset, bm, Color.Blue);
        }
    }

    /// <summary>
    /// Implements a cell button which is typically used with grid filter bar to clear the existing filter.
    /// </summary>
    public class ClearFilterButtonExt : GridCellButton
    {
        /// <summary>
        /// Constructor for ClearFilterButton.
        /// </summary>
        /// <param name="control">The text box cell renderer.</param>
        public ClearFilterButtonExt(GridTextBoxCellRenderer control)
            : base(control)
        {
        }
        /// <override/>
        /// <summary>
        /// Draws a cell button at the specified row index and column index.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="bActive">True if this is the active current cell.</param>
        /// <param name="style">Cell style information.</param>
        public override void Draw(Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, bActive, style.CellValue);
            base.Draw(g, rowIndex, colIndex, bActive, style);

            // draw the button
            bool hovering = IsHovering(rowIndex, colIndex);
            bool mouseDown = IsMouseDown(rowIndex, colIndex);
            bool disabled = !style.Clickable;
            ButtonState buttonState = ButtonState.Normal;
            if (disabled)
            {
                buttonState |= ButtonState.Inactive | ButtonState.Flat;
            }
            else if (!hovering && !mouseDown)
            {
                buttonState |= ButtonState.Flat;
            }
            Point ptOffset = Point.Empty;
            if (mouseDown)
            {
                ptOffset = new Point(1, 1);
                buttonState |= ButtonState.Pushed;
            }
            DrawButton(g, Bounds, buttonState, style);
            GridListFilterBarCellRenderer renderer = (GridListFilterBarCellRenderer)Owner;
            Bitmap bm = null;
            bool filteredColumn = false;
            foreach (RecordFilterDescriptor filterdesc in renderer.Grid.TableDescriptor.RecordFilters)
            {
                if (filterdesc.MappingName == (style as GridTableCellStyleInfo).TableCellIdentity.Column.MappingName)
                {
                    filteredColumn = true;
                    break;
                }
            }
            if (renderer.Model.HasFilter((style as GridTableCellStyleInfo).TableCellIdentity) && filteredColumn)
            {
                switch (renderer.Grid.Model.Options.GridVisualStyles)
                {
                    case GridVisualStyles.Office2010Black:
                    case GridVisualStyles.Office2010Blue:
                    case GridVisualStyles.Office2010Silver:
                        bm = DynamicFilterBitmaps.GetBitmap("filter2010_delete");
                        break;
                    default:
                        bm = DynamicFilterBitmaps.GetBitmap("filter_delete"); // make sure this is included in project and marked as "Embedded Resource"
                        break;
                }
            }
            else
            {
                switch (renderer.Grid.Model.Options.GridVisualStyles)
                {
                    case GridVisualStyles.Office2010Black:
                    case GridVisualStyles.Office2010Blue:
                    case GridVisualStyles.Office2010Silver:
                        bm = DynamicFilterBitmaps.GetBitmap("filter2010");
                        break;
                    default:
                        bm = DynamicFilterBitmaps.GetBitmap("filter");
                        break;
                }
            }
            DynamicFilterBitmaps.IconPainter.PaintIcon(g, Bounds, ptOffset, bm, Color.Blue);
        }
    }
}
