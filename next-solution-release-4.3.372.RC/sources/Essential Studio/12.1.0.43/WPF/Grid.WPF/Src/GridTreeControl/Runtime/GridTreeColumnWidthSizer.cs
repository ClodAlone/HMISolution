#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Grid;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <exclude/>
    internal class GridTreeColumnWidthSizer
    {
        GridModel gridModel;
        IEnumerable columnWidthPercents;
        ContentControl gridHost;
        /// <exclude/>
        public GridTreeColumnWidthSizer(GridModel gridModel, IEnumerable columnWidthPercents)
        {
            this.gridModel = gridModel;
     
            //use this event to make sure grid fills client area
            gridModel.ColumnWidths.LineSizeChanged += new RangeChangedEventHandler(ColumnWidths_LineSizeChanged);
            gridModel.Views.First().ResizingColumns += new GridResizingColumnsEventHandler(GridTreeColumnWidthSizer_ResizingColumns);
            this.columnWidthPercents = columnWidthPercents;
            this.gridHost = gridModel.Views.First().Parent as ContentControl;
            if (this.gridHost != null)
            {
                this.gridHost.SizeChanged += new SizeChangedEventHandler(_SizeChanged);
#if !SILVERLIGHT
                if (gridHost is ScrollViewer)
                {
                    ((ScrollViewer)this.gridHost).ScrollChanged += new ScrollChangedEventHandler(gridHost_ScrollChanged);
                }
#endif
            }
        }

        void GridTreeColumnWidthSizer_ResizingColumns(object sender, GridResizingColumnsEventArgs args)
        {
            if (!inApplySizes && args.Reason == GridResizeCellsReason.MouseMove && 
                SizingBehavior == GridPercentColumnSizingBehavior.SizeUntouchedColumns)
            {
                gridModel.ColumnWidths[args.Columns.Left] = args.Width;// Math.Max(1, args.Width);
            }
        }
#if !SILVERLIGHT
        void gridHost_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CheckIfVerticalScrollBarVisiblityChanged();
        }
#endif

        Visibility lastVerticalVisibility =
#if SILVERLIGHT
            Visibility.Collapsed;
#else
            Visibility.Hidden;
#endif

        void CheckIfVerticalScrollBarVisiblityChanged()
        {
            if (applySizing && !inApplySizes && gridHost != null && gridHost is ScrollViewer
                && ((ScrollViewer)gridHost).ComputedVerticalScrollBarVisibility != lastVerticalVisibility)
            {
                ApplySizes();
            }
        }

        void ColumnWidths_LineSizeChanged(object sender, RangeChangedEventArgs e)
        {
            if (!inApplySizes)
            {
                switch (SizingBehavior)
                {
                    case GridPercentColumnSizingBehavior.SizeAlwaysPercent:
                        EnableSizing();
                        break;
                    case GridPercentColumnSizingBehavior.NoSizingIfAnyTouched:
                        applySizing = false;
                        break;
                    case GridPercentColumnSizingBehavior.SizeUntouchedColumns:
                        {
                            int i = leadingFixedSizedColumnsCount;
                            foreach (IPercentWidth col in columnWidthPercents)
                            {
                                if (i > gridModel.ColumnWidths.LineCount)
                                    break;
                                int hiddenCount = 0;
                                if (gridModel.ColumnWidths[i] == 0 || gridModel.ColumnWidths.GetHidden(i, out hiddenCount))
                                {
                                    i++;
                                    continue;
                                }
                                if (i >= e.From && i <= e.To)
                                {
                                    if (col.PercentWidth != NotSet)
                                    {
                                        col.PercentWidth = NotSet;
                                    }
                                }
                                i++;
                            }
                         }
                        break;
                    default:
                        break;
                }
                ApplySizes();
            }
           
        }

        void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplySizes();
        }


        double scrollBarWidth = 17; //width - 1....
        internal bool inApplySizes = false;
        double border = 8;

        bool applySizing = true;
      /// <summary>
      /// Enables the percentage sizing.
      /// </summary>
        public void EnableSizing()
        {
            applySizing = true;
        }

        private GridPercentColumnSizingBehavior sizingBehavior = GridPercentColumnSizingBehavior.None;

        internal GridPercentColumnSizingBehavior SizingBehavior
        {
            get { return sizingBehavior; }
            set 
            { 
                sizingBehavior = value;
                IMouseController mc = gridModel.Views.First().MouseControllerDispatcher.Find("ResizeColumnsMouseController");
                if (value == GridPercentColumnSizingBehavior.SizeAlwaysPercent)
                {
                    if (mc != null)
                        gridModel.Views.First().MouseControllerDispatcher.Remove(mc);
                }
                else
                {
                    if (mc == null)
                    {
                        gridModel.Views.First().MouseControllerDispatcher.Add(new GridResizeColumnsMouseController(gridModel.Views.First()));
                    }
                }
                EnableSizing();
                ApplySizes();
            }
        }

        int leadingFixedSizedColumnsCount = 1;//2;//1;//2;

        /// <summary>
        /// Recomputes and sets the actual columns widths based on the currenct parent client width and the PercentWidth values.
        /// </summary>
        public void ApplySizes()
        {
            if (SizingBehavior == GridPercentColumnSizingBehavior.None)
                return;

            if (gridModel == null || gridModel.Views == null || gridModel.Views.First() == null 
                || gridModel.Views.First().ColumnWidths.LineCount <= 1)
            {
                return;
            }

            if (this.gridHost == null || this.gridHost.ActualWidth == 0)
                return; //not sized yet
       
             if (!applySizing)
                return;
             if (gridHost is ScrollViewer)
             {
                 lastVerticalVisibility = ((ScrollViewer)gridHost).ComputedVerticalScrollBarVisibility;
             }
    
            inApplySizes = true;
            double w = 0;
            for (int j = 0; j < leadingFixedSizedColumnsCount; ++j)
            {
               
                w += gridModel.ColumnWidths[j];
            }
            double totalPercents = 0;
            int i = leadingFixedSizedColumnsCount;
            foreach (IPercentWidth col in columnWidthPercents)
            {
                if (i > gridModel.ColumnWidths.LineCount)
                    break;
                int hiddenCount = 0;
                if (gridModel.ColumnWidths[i] == 0 || gridModel.ColumnWidths.GetHidden(i, out hiddenCount))
                {
                    i++;
                    continue;
                }
                if (col.PercentWidth == NotSet)
                {
                    w +=  gridModel.ColumnWidths[i];
                }
                else
                    totalPercents += col.PercentWidth;
                i++;
             }
            
            
           double clientWidth = this.gridHost.ActualWidth +
               ((gridHost != null && gridHost is ScrollViewer && ((ScrollViewer)this.gridHost).ComputedVerticalScrollBarVisibility == Visibility.Visible)  
               ? 0d : scrollBarWidth);
            
            if (w < clientWidth)
            {
                i = leadingFixedSizedColumnsCount;
                double targetWidth = clientWidth - 2 * border - 2 - w;
                if (targetWidth > 0)
                {
                    foreach (IPercentWidth col in columnWidthPercents)
                    {
                        if (i > gridModel.ColumnWidths.LineCount)
                            break;
                        int hiddenCount = 0;
                        if (gridModel.ColumnWidths[i] == 0 || gridModel.ColumnWidths.GetHidden(i, out hiddenCount))
                        {
                            i++;
                            continue;
                        }

                        if (col.PercentWidth != NotSet)
                        {
                            //grid.Model.ColumnWidths[i] 
                            col.Width = targetWidth / totalPercents * col.PercentWidth;
                            gridModel.ColumnWidths[i] = col.Width;
                        }
                        i++;
                    }
                }
                gridModel.InvalidateVisual();
            }
            inApplySizes = false;
        }

        /// <summary>
        /// Use this value to indicate that the PercentWidth value is unset and its column
        /// will not participate in the percent sizing calculations.
        /// </summary>
        public static readonly double NotSet = double.MinValue;
    }

    /// <summary>
    /// Allows percent sizing in column objects.
    /// </summary>
    public interface IPercentWidth
    {
        double PercentWidth { get; set; }
        double Width { get; set; }
    }

    /// <summary>
    /// Enumerates the possible precent sizing behaviors.
    /// </summary>
    public enum GridPercentColumnSizingBehavior
    {
        /// <summary>
        /// Indicates no percent column width sizing calculations will be done.
        /// </summary>
        None,
        /// <summary>
        /// Indicates the columns will always be sized according to the PercentWidth value and the user cannot change 
        /// the column width through the UI.
        /// </summary>
        SizeAlwaysPercent,
        /// <summary>
        /// Indicates the columns will be sized according to the PercentWidth value only as long as the user does not change 
        /// the column width through the UI. Once a column width has been user set, that column no longer participates in 
        /// the percent sizing, but the other columns will continue to participate in the percent sizing.
        /// </summary>
        SizeUntouchedColumns,
        /// <summary>
        /// Indicates the columns will be sized according to the PercentWidth value only as long as the user does not change 
        /// the column width through the UI. Once any column's width has been user set, no further percent sizing will be done
        /// on any column.
        /// </summary>
        NoSizingIfAnyTouched
    }

    
}
