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
using System.Windows;
using System.Windows.Input;
using Syncfusion.Data.Extensions;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using System.Diagnostics;
using Windows.UI.Xaml.Controls.Primitives;
#else
using System.Windows.Controls.Primitives;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class GridColumnChooserController : GridColumnDragDropController
    {
        #region Fields
        internal SfDataGrid dataGrid;
        IColumnChooser chooser;
        Style style;
        bool popUpFromChooser;
        bool allowHidingForFinalColumn;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the final column can be hide to drop in column chooser.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowHidingForFinalColumn
        {
            get { return allowHidingForFinalColumn; } 
            set { allowHidingForFinalColumn = value; }
        }
        
        #endregion

        #region ctor
        public GridColumnChooserController(SfDataGrid dataGrid, IColumnChooser columnChooserwindow)
            : base(dataGrid)
        {
            this.dataGrid = dataGrid;
            this.chooser = columnChooserwindow;
            style = this.PopupContentControl.Style;
        }
        #endregion

        #region Public method
        /// <summary>
        /// Shows the popup for drag and drop
        /// </summary>
        /// <param name="colIndex"></param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.Input.PointerRoutedEventArgs">PointerRoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
#if !WinRT
        public void Show(int colIndex, MouseEventArgs e)
        {
            var point = e.GetPosition(null);
            this.ShowPopup(colIndex, new Rect(point.X - 60, point.Y, 120, 30), null);
            popUpFromChooser = true;
        }
#else
        public void Show(int colIndex, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(null).Position;
            this.ShowPopup(colIndex, new Rect(point.X - 70, point.Y, 140, 45), e.Pointer);
            popUpFromChooser = true;
        }
#endif
        #endregion

        #region Overrides

        public override GridRegion PointToGridRegion(Point point)
        {
            if (this.chooser.GetControlRect().Contains(point) && this.PopupContentControl != null)
                return GridRegion.ColumnChooser;
            return base.PointToGridRegion(point);
        }

        public override bool CanShowPopup(GridColumn column)
        {
            return true;
        }
        /// <summary>
        /// Overrides the Popup position changed
        /// </summary>
        /// <param name="HorizontalDelta"></param>
        /// <param name="VerticalDelta"></param>
        /// <param name="mousePoint"></param>
        /// <param name="mousePointOverGrid"></param>
        /// <remarks></remarks>
        protected override void OnPopupContentPositionChanged(double HorizontalDelta, double VerticalDelta, Point mousePoint, Point mousePointOverGrid)
        {
            base.OnPopupContentPositionChanged(HorizontalDelta, VerticalDelta, mousePoint, mousePointOverGrid);
            var rect = this.chooser.GetControlRect();
            if (rect.Contains(mousePoint) && this.PopupContentControl != null)
            {
                this.CloseDragIndication();
                if (this.dataGrid.Columns.Where(col => col.IsHidden).Count() == this.dataGrid.Columns.Count - 1 && !AllowHidingForFinalColumn)
                {
                    VisualStateManager.GoToState(this.PopupContentControl, "InValid", true);
                }
                else
                    VisualStateManager.GoToState(this.PopupContentControl, "Valid", true);
            }
        }

        /// <summary>
        /// Popup on dropped
        /// </summary>
        /// <param name="point"></param>
        /// <remarks></remarks>
        protected override void OnPopupContentDropped(Point point, Point pointOverGrid)
        {
            var rect = this.chooser.GetControlRect();
            var headerRowRect = this.GetHeaderRowRect();
            var groupDropAreaRect = this.GetGroupDropAreaRect();
            var gridColumn = this.PopupContentControl.Tag as GridColumn;
            if (rect.Contains(point) && this.PopupContentControl != null)
            {
                var count = this.dataGrid.Columns.Where(col => col.IsHidden).Count();
                if (this.dataGrid.Columns.Where(col => col.IsHidden).Count() == this.dataGrid.Columns.Count - 1 && !AllowHidingForFinalColumn)
                {
                    (this.PopupContentControl.Parent as Popup).IsOpen = false;
                    return;
                }
                gridColumn.IsHidden = true;
#if !WPF
                this.SuspendReverseAnimation(true);
#endif
                if (this.PopupContentControl.IsDragFromGroupDropArea)
                    base.OnPopupContentDropped(point, pointOverGrid);
                else
                    this.HidePopup();
            }
            else if (popUpFromChooser && this.PopupContentControl != null)
            {
#if !WPF
                if (!gridColumn.IsHidden)
                    this.SuspendReverseAnimation(true);
                else
                    this.SuspendReverseAnimation(false);
#endif
                base.OnPopupContentDropped(point, pointOverGrid);
                if (this.dataGrid.AllowGrouping && (groupDropAreaRect.Contains(point) && this.dataGrid.ShowColumnWhenGrouped))
                    gridColumn.IsHidden = false;
                else if (headerRowRect.Contains(point))
                    gridColumn.IsHidden = false;
            }
            else
            {
#if !WPF
                this.SuspendReverseAnimation(false);
#endif
                base.OnPopupContentDropped(point, pointOverGrid);
            }
            popUpFromChooser = false;
        }

        /// <summary>
        /// Indicates the Hidden Property value changed for GridColumn.
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        protected override void OnColumnHiddenChanged(GridColumn column)
        {
            if (column.IsHidden)
            {
                this.chooser.AddChild(column);
            }
            else if (this.PopupContentControl.IsDragFromGroupDropArea)
            {
                column.IsHidden = true;
            }
            else
                this.chooser.RemoveChild(column);
            base.OnColumnHiddenChanged(column);
        }
        #endregion
    }
}
