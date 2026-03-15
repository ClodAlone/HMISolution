#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class DetailsViewRowControl : VirtualizingCellsControl
    {
        #region Ctor
        public DetailsViewRowControl()
        {
            this.DefaultStyleKey = typeof(DetailsViewRowControl);
        }
        #endregion

        protected override void SetContent()
        {
            this.Content = this.ItemsPanel = new DetailsViewRowPanel();
        }

        internal void InitializeDetailsViewRowControl(Func<IList<IColumnElement>> getVisibleColumn, Func<int, Rect> getCellPosition, Func<string, object> getDetailsViewInfo)
        {
            var detailsViewRowPanel = this.ItemsPanel as DetailsViewRowPanel;
            if (detailsViewRowPanel == null) return;
            detailsViewRowPanel.GetVisibleColumns = getVisibleColumn;
            detailsViewRowPanel.GetCellPosition = getCellPosition;
            detailsViewRowPanel.GetDetailsViewInfo = getDetailsViewInfo;
        }
    }
    
    public class DetailsViewRowPanel : Panel, IDisposable
    {
        internal Func<Rect> GetDetailsViewPosition;
        internal Func<UIElement> GetDetailsViewContent;

        internal Func<string, object> GetDetailsViewInfo;

        internal Func<IList<IColumnElement>> GetVisibleColumns;
        internal Func<int, Rect> GetCellPosition; 

        public DetailsViewRowPanel()
        {
            
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (GetVisibleColumns != null && GetCellPosition != null)
            {
                MeasureCells();
                return availableSize;
            }
            return base.MeasureOverride(availableSize);
        }

        private void MeasureCells()
        {
            if (this.GetVisibleColumns == null)
                return;
            var visibleCells = this.GetVisibleColumns();
            foreach (var column in visibleCells)
            {
                if (column.Element.Visibility != Visibility.Visible || column.Index < 0) continue;
                if (!this.Children.Contains(column.Element))
                    this.Children.Add(column.Element);
                var rect = GetCellPosition(column.Index);
                if (rect.IsEmpty) continue;
                var cell = column.Element;
                if (cell is IDetailsViewInfo && GetDetailsViewInfo != null)
                {
                    var scrollInfo = cell as IDetailsViewInfo;
                    var clipRect = (Rect)GetDetailsViewInfo("Clip");
                    scrollInfo.SetClipRect(clipRect);

                    var offset = (double)GetDetailsViewInfo("HorizontalOffset");
                    scrollInfo.SetHorizontalOffset(offset);

                    offset = (double)GetDetailsViewInfo("VerticalOffset");
                    scrollInfo.SetVerticalOffset(offset);

                    var padding = (Thickness)GetDetailsViewInfo("Padding");
                    var control = cell as Control;
                    if (control != null) control.Padding = padding;
                }
                cell.Measure(new Size(rect.Width, rect.Height));
            }
        }

        private void ArrangeCells()
        {
            if (this.GetVisibleColumns == null)
                return;
            var visibleColumns = this.GetVisibleColumns();
            foreach (var column in visibleColumns)
            {
                if (column.Element.Visibility != Visibility.Visible) continue;
                var rect = GetCellPosition(column.Index);
                if (rect.IsEmpty) continue;
                column.Element.Arrange(rect);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (GetVisibleColumns != null && GetCellPosition != null)
            {
                ArrangeCells();
                return finalSize;
            }
            return base.ArrangeOverride(finalSize);
        }

        public void Dispose()
        {
            GetDetailsViewPosition = null;
            GetDetailsViewContent = null;
        }
    }

    public class DetailsViewContentPresenter : ContentControl, IDetailsViewInfo
    {
        public DetailsViewContentPresenter()
        {
            this.DefaultStyleKey = typeof (DetailsViewContentPresenter);
        }

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            if (!string.IsNullOrEmpty(_currentVisualState))
                ApplyVisualState(_currentVisualState);
        }

        private string _currentVisualState;
        internal void ApplyVisualState(string visualState)
        {
            _currentVisualState = VisualStateManager.GoToState(this, visualState, true) ? string.Empty : visualState;
        }


        public void SetClipRect(Rect rect)
        {
            if (this.Content is IDetailsViewInfo)
                (this.Content as IDetailsViewInfo).SetClipRect(rect);
        }

        public void SetHorizontalOffset(double offset)
        {
            if(this.Content is IDetailsViewInfo)
                (this.Content as IDetailsViewInfo).SetHorizontalOffset(offset);
        }

        public void SetVerticalOffset(double offset)
        {
            if (this.Content is IDetailsViewInfo)
                (this.Content as IDetailsViewInfo).SetVerticalOffset(offset);
        }

        public double GetExtendedWidth()
        {
            if (this.Content is IDetailsViewInfo)
                return (this.Content as IDetailsViewInfo).GetExtendedWidth();
            return double.NaN;
        }
    }

    public class GridDetailsViewIndentCell : ContentControl
    {
        public GridDetailsViewIndentCell()
        {
            this.DefaultStyleKey = typeof (GridDetailsViewIndentCell);
        }

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            if (!string.IsNullOrEmpty(_currentVisualState))
                ApplyVisualState(_currentVisualState);
        }

        private string _currentVisualState;
        internal void ApplyVisualState(string visualState)
        {
            _currentVisualState = VisualStateManager.GoToState(this, visualState, true) ? string.Empty : visualState;
        }
    }
}
