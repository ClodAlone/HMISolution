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
using System.Net;
using System.Windows;
using System.Diagnostics;

#if !WinRT

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Controls.Grid
#else

using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Syncfusion.WinRT.Controls.Cells;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCell : ContentControl
    {
        RowColumnIndex cellUnderMouse = RowColumnIndex.Empty;  //cell under mouse...

#if (!SILVERLIGHT5 && !WinRT)
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            InvalidateCellUnderMouse();
        }
#endif
#if !WinRT
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (GridControl != null)
            {
                cellUnderMouse = GridControl.PointToCellRowColumnIndex(e);
                SubscribeAllMouseLeftButtonDown();
            }
            else
            {
                cellUnderMouse = RowColumnIndex.Empty;
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (GridControl != null)
            {
                UnsubscribeAllMouseLeftButtonDown();
            }
            cellUnderMouse = RowColumnIndex.Empty;
        }
        //Invalidate the cell which is under mouse.
        void InvalidateCellUnderMouse()
        {
            if (GridControl != null && !cellUnderMouse.IsEmpty)
            {
                GridControl.Model.InvalidateCell(cellUnderMouse);
            }
        }

        void c_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GridControl != null && !cellUnderMouse.IsEmpty)
            {
                if (!GridControl.CurrentCell.HasCurrentCellAt(this.RowColumnIndex))
                    GridControl.CurrentCell.MoveTo(this.RowColumnIndex);
            }
            InvalidateCellUnderMouse();
        }

        void GridCell_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            InvalidateCellUnderMouse();
        }

        //Hook the cell uielement events.
        private void SubscribeAllMouseLeftButtonDown()
        {
            Rect r = this.GridControl.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(cellUnderMouse.RowIndex, cellUnderMouse.ColumnIndex), false, false);
            var list = VisualTreeHelper.FindElementsInHostCoordinates(r, this);
            foreach (object c in list)
            {
                if (c is UIElement)
                {
                    ((UIElement)c).MouseLeftButtonDown += new MouseButtonEventHandler(c_MouseLeftButtonDown);
                    ((UIElement)c).MouseLeftButtonUp += new MouseButtonEventHandler(GridCell_MouseLeftButtonUp);
                    ((UIElement)c).KeyDown += new KeyEventHandler(GridCell_KeyDown);
                    ((UIElement)c).KeyUp += new KeyEventHandler(GridCell_KeyUp);
                    ((UIElement)c).MouseWheel += new MouseWheelEventHandler(GridCell_MouseWheel);
                }
                if (c is ScrollBar)
                {
                    ((ScrollBar)c).ValueChanged += new RoutedPropertyChangedEventHandler<double>(GridCell_ValueChanged);
                }
            }
        }

        void GridCell_KeyUp(object sender, KeyEventArgs e)
        {
            InvalidateCellUnderMouse();
        }

        void GridCell_KeyDown(object sender, KeyEventArgs e)
        {
            InvalidateCellUnderMouse();
        }

        void GridCell_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InvalidateCellUnderMouse();
        }

        void GridCell_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (sender as ScrollBar).InvalidateMeasure();
            InvalidateCellUnderMouse();
        }

        //UnHook the cell uielement events.
        private void UnsubscribeAllMouseLeftButtonDown()
        {
            Rect r = this.GridControl.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex), false, false);
            var list = VisualTreeHelper.FindElementsInHostCoordinates(r, this);
            foreach (object c in list)
            {
                if (c is UIElement)
                {
                    ((UIElement)c).MouseLeftButtonDown -= new MouseButtonEventHandler(c_MouseLeftButtonDown);
                    ((UIElement)c).MouseLeftButtonUp -= new MouseButtonEventHandler(GridCell_MouseLeftButtonUp);
                    ((UIElement)c).KeyDown -= new KeyEventHandler(GridCell_KeyDown);
                    ((UIElement)c).KeyUp -= new KeyEventHandler(GridCell_KeyUp);
                    ((UIElement)c).MouseWheel -= new MouseWheelEventHandler(GridCell_MouseWheel);
                }
                if (c is ScrollBar)
                {
                    ((ScrollBar)c).ValueChanged -= new RoutedPropertyChangedEventHandler<double>(GridCell_ValueChanged);
                }
            }
        }

#endif
        public GridCell()
        {
            this.DefaultStyleKey = typeof(GridCell);
            this.TabNavigation = KeyboardNavigationMode.Local;
            this.TabIndex = 0;
            this.IsTemplateApplied = false;
            this.Loaded += new RoutedEventHandler(GridCell_Loaded);
        }

        internal void UnHookEvents()
        {
            //this.Loaded -= new RoutedEventHandler(GridCell_Loaded);
        }

        internal void HookEvents()
        {
            //this.Loaded += new RoutedEventHandler(GridCell_Loaded);
        }

        //Refresh the GridCell when it is loaded at first time.
        void GridCell_Loaded(object sender, RoutedEventArgs e)
        {
            GridControl.Model.InvalidateCell((sender as GridCell).RowColumnIndex);
            this.Loaded -= new RoutedEventHandler(GridCell_Loaded);
        }

        // We have commented this line Since this affect the Excel like selection behaviour in Silverlight GridControl (issue SD8544).

        //void Model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        //{
        //    if (this.GridControl.Model.SelectedRanges.Contains(GridRangeInfo.Cell(this.RowColumnIndex.RowIndex, this.RowColumnIndex.ColumnIndex)))
        //    {
              
        //       VisualStateManager.GoToState(this, "Selected", true);
               
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(this, "NotSelected", true);
        //    }
        //}


        //private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (this.GridControl == null || this.RowColumnIndex == null)
        //    {
        //        return;
        //    }

        //    if (!VirtualizingCellsControl.GetHasFocusWithin(this)
        //        && this.GridControl.CurrentCell.CellRowColumnIndex != this.RowColumnIndex)
        //    {
        //       this.GridControl.CurrentCell.MoveTo(this.RowColumnIndex);
        //       this.Focus();
        //        // We have commented this line Since this affect the Excel like selection behaviour in Silverlight GridControl (issue SD8544).
        //       //this.GridControl.CurrentCell.MoveTo(this.RowColumnIndex);
        //        //Added the line at the top to fix the issue of not focusing the datatemplate cell
        //    }
        //}



        internal bool IsTemplateApplied
        {
            get;
            set;
        }

        internal GridControlBase GridControl
        {
            get;
            set;
        }

        // private RowColumnIndex rowColumnIndex;

        internal RowColumnIndex RowColumnIndex
        {
            get;
            set;
        }       

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.IsTemplateApplied = true;
        }
    }
}
