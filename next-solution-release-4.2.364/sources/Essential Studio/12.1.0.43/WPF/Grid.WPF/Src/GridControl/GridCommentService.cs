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
#if !WinRT
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
namespace Syncfusion.Windows.Controls.Grid
{
#else
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
namespace Syncfusion.WinRT.Controls.Grid
{
#endif

    /// <summary>
    /// Defines the options to display Cell Comment Tip.
    /// </summary>
    public enum CommentAlignment
    {
        /// <summary>
        /// Top Left edge.
        /// </summary>
        TopLeft,
        /// <summary>
        /// Top Right edge.
        /// </summary>
        TopRight,
        /// <summary>
        /// Bottom Left edge.
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Bottom Right edge.
        /// </summary>
        BottomRight
    };

    /// <summary>
    /// A helper class to show comment tips for grid cells.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCommentService
    {
        private static GridControlBase FindGrid(DependencyObject d)
        {
            var frameWorkElement = d as FrameworkElement;
            if (frameWorkElement == null)
            {
                return null;
            }
#if !WinRT
            if (frameWorkElement is GridDataControl)
            {
                return frameWorkElement.FindElementOfType<GridDataControlBaseImpl>();
            }
#endif
            // this will find the GridControlBase from the visual tree of any templated grid control
            return frameWorkElement.FindElementOfType<GridControlBase>();
        }

        #region ShowComment

        private static readonly DependencyProperty CachedRowColumnIndexProperty = DependencyProperty.RegisterAttached("CachedRowColIndex", typeof(RowColumnIndex), typeof(GridCommentService), new PropertyMetadata(RowColumnIndex.Empty));

        private static RowColumnIndex GetCachedRowColIndex(DependencyObject dpo)
        {
            return (RowColumnIndex)dpo.GetValue(GridCommentService.CachedRowColumnIndexProperty);
        }

        private static void SetCachedRowColIndex(DependencyObject dpo, RowColumnIndex value)
        {
            dpo.SetValue(GridCommentService.CachedRowColumnIndexProperty, value);
        }

        public static string GetCachedForeCorner(DependencyObject obj)
        {
            return (string)obj.GetValue(CachedForeCornerProperty);
        }

        public static void SetCachedForeCorner(DependencyObject obj, string value)
        {
            obj.SetValue(CachedForeCornerProperty, value);
        }

        public static readonly DependencyProperty CachedForeCornerProperty =
            DependencyProperty.RegisterAttached("CachedForeCorner", typeof(string), typeof(GridCommentService), new PropertyMetadata(string.Empty));

        private static readonly DependencyProperty PopupHostProperty = DependencyProperty.RegisterAttached("PopupHost", typeof(PopupDragWindow), typeof(GridCommentService), new PropertyMetadata(null));

        private static PopupDragWindow GetPopupHost(DependencyObject dpo)
        {
            return (PopupDragWindow)dpo.GetValue(GridCommentService.PopupHostProperty);
        }

        private static void SetPopupHost(DependencyObject dpo, PopupDragWindow value)
        {
            dpo.SetValue(GridCommentService.PopupHostProperty, value);
        }

#if (!SILVERLIGHT &&  !WinRT)
        private static readonly DependencyProperty TimerProperty = DependencyProperty.RegisterAttached("Timer", typeof(DispatcherTimer), typeof(GridCommentService), new PropertyMetadata(null));

        private static DispatcherTimer GetTimer(DependencyObject dpo)
        {
            return (DispatcherTimer)dpo.GetValue(GridCommentService.TimerProperty);
        }

        private static void SetTimer(DependencyObject dpo, DispatcherTimer timer)
        {
            dpo.SetValue(GridCommentService.TimerProperty, timer);
        }
#else
        private static readonly DependencyProperty TimerProperty = DependencyProperty.RegisterAttached("Timer", typeof(DispatcherTimerExt), typeof(GridCommentService), new PropertyMetadata(null));

        private static DispatcherTimerExt GetTimer(DependencyObject dpo)
        {
            return (DispatcherTimerExt)dpo.GetValue(GridCommentService.TimerProperty);
        }

        private static void SetTimer(DependencyObject dpo, DispatcherTimerExt timer)
        {
            dpo.SetValue(GridCommentService.TimerProperty, timer);
        }
#endif

        private static readonly DependencyProperty CommentDelayProperty = DependencyProperty.RegisterAttached("CommentDelay", typeof(int), typeof(GridCommentService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static int GetCommentDelay(DependencyObject dpo)
        {
            return (int)dpo.GetValue(GridCommentService.CommentDelayProperty);
        }

        /// <summary>
        /// Sets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetCommentDelay(DependencyObject dpo, int value)
        {
            dpo.SetValue(GridCommentService.CommentDelayProperty, value);
        }

        private static PopupDragWindow GetInitializedDragWindow()
        {

#if (!SILVERLIGHT && !WinRT)
            var popupDragWindow = new PopupDragWindow() { DestroyChildOnStopDrag = false, Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint, HorizontalOffset = 10d, VerticalOffset = 20d, AllowsTransparency = true };
            var innerGrid = new System.Windows.Controls.Grid();
            innerGrid.Children.Add(new ContentControl());
            popupDragWindow.Child = innerGrid;
#else
            var popupDragWindow = new PopupDragWindow() { DestroyChildOnStopDrag = false, IsPopupUnderMousePoint = true, HorizontalOffset = 10d, VerticalOffset = 20d };
#if WinRT
            var innerGrid = new Windows.UI.Xaml.Controls.Grid();
#else
            var innerGrid = new System.Windows.Controls.Grid();
#endif
            innerGrid.Children.Add(new ContentControl());
            popupDragWindow.ProvideVisual(innerGrid);
#endif
            return popupDragWindow;
        }

        private static readonly DependencyProperty ShowCommentProperty = DependencyProperty.RegisterAttached("ShowComment", typeof(bool), typeof(GridCommentService),
#if !WinRT
 new PropertyMetadata(OnShowCommentsChanged));
#else
            new PropertyMetadata(null, OnShowCommentsChanged));
#endif

        /// <summary>
        /// Gets the value of ShowTooltips property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool GetShowComment(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridCommentService.ShowCommentProperty);
        }

        /// <summary>
        /// Sets the value of ShowTooltips property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetShowComment(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridCommentService.ShowCommentProperty, value);
        }

        private static void OnShowCommentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var value = (bool)args.NewValue;
            if (value)
            {
                WireMouseMove(d);
            }
            else
            {
                Dispose(d);
            }
        }

        #region SelectionChanged
        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty SelectionChangedFlagProperty = DependencyProperty.RegisterAttached("SelectionChangedFlag", typeof(bool), typeof(GridCommentService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool GetSelectionChangedFlag(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridCommentService.SelectionChangedFlagProperty);
        }

        /// <summary>
        /// Sets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetSelectionChangedFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridCommentService.SelectionChangedFlagProperty, value);
        }
        #endregion

        #region MouseDown


        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty MouseDownFlagProperty = DependencyProperty.RegisterAttached("MouseDownFlag", typeof(bool?), typeof(GridCommentService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool GetMouseDownFlag(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridCommentService.MouseDownFlagProperty);
        }

        /// <summary>
        /// Sets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetMouseDownFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridCommentService.MouseDownFlagProperty, value);
        }

        #endregion

        #region MouseLeave


        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty MouseLeaveFlagProperty = DependencyProperty.RegisterAttached("MouseLeaveFlag", typeof(bool), typeof(GridCommentService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool GetMouseLeaveFlag(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridCommentService.MouseDownFlagProperty);
        }

        /// <summary>
        /// Sets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetMouseLeaveFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridCommentService.MouseLeaveFlagProperty, value);
        }
        #endregion

        /// <summary>
        /// Releases all the resources used by this component.
        /// </summary>
        /// <param name="d">The parent grid.</param>
        public static void Dispose(DependencyObject d)
        {
            var popuphost = GridCommentService.GetPopupHost(d);
            if (popuphost != null)
                popuphost.Dispose();
            GridCommentService.SetPopupHost(d, null);
            GridCommentService.SetTimer(d, null);
            UnwireMouseMove(d);
        }

        private static void UnwireMouseMove(DependencyObject d)
        {
            var frameworkElement = d as FrameworkElement;
            var grid = FindGrid(d);
            if (grid != null)
            {
#if (!SILVERLIGHT &&  !WinRT)
                grid.PreviewMouseMove -= new System.Windows.Input.MouseEventHandler(OnGridShowCommentPreviewMouseMove);
                grid.PreviewMouseDown -= new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseDown);
                grid.SelectionChanged -= new GridSelectionChangedEventHandler(OnSelectionChanged);
#endif
#if SILVERLIGHT
                grid.MouseMove -= OnGridShowCommentPreviewMouseMove;
                grid.MouseLeave -= new System.Windows.Input.MouseEventHandler(OnGridMouseLeave);
#endif
#if WinRT
                grid.PointerMoved += grid_PointerMoved;
                grid.PointerExited += grid_PointerExited;
#endif
                frameworkElement.Loaded -= new RoutedEventHandler(frameworkElement_Loaded);
                frameworkElement.Unloaded -= new RoutedEventHandler(frameworkElement_Unloaded);
            }
        }
#if WinRT
        static void grid_PointerExited(object sender, global::Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            GridCommentService.SetMouseLeaveFlag(grid, true);
        }

        
#endif
        private static void OnGridMouseLeave(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            GridCommentService.SetMouseLeaveFlag(grid, true);
        }

        private static void OnSelectionChanged(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            GridCommentService.SetSelectionChangedFlag(grid, true);
        }

        private static void OnPreviewMouseDown(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            GridCommentService.SetMouseDownFlag(grid, true);
        }

        private static GridControlBase UpdatePopUpHost(object sender)
        {
            var grid = sender as GridControlBase;
            var popupHost = GridCommentService.GetPopupHost(grid);
            if (popupHost != null && popupHost.IsDragging)
            {
                popupHost.StopDrag();
#if (!SILVERLIGHT &&  !WinRT)
                popupHost.IsOpen = false;
#else
                popupHost.Hide();
#endif
            }
            return grid;
        }

        private static void WireMouseMove(DependencyObject d)
        {
            var frameworkElement = d as FrameworkElement;
#if (!SILVERLIGHT &&  !WinRT)
            if (frameworkElement != null && !frameworkElement.IsLoaded)
#else
            if (frameworkElement != null)
#endif
            {
                frameworkElement.Loaded += new RoutedEventHandler(frameworkElement_Loaded);
                frameworkElement.Unloaded += new RoutedEventHandler(frameworkElement_Unloaded);
            }
#if (!SILVERLIGHT &&  !WinRT)
            else
            {
                var grid = FindGrid(d);
                if (grid != null)
                {
                    grid.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseDown);
                    grid.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(OnGridShowCommentPreviewMouseMove);
                    grid.MouseLeave += new System.Windows.Input.MouseEventHandler(OnGridMouseLeave);
                    grid.SelectionChanged += new GridSelectionChangedEventHandler(OnSelectionChanged);
                }
            }
#endif
        }

        static void frameworkElement_Unloaded(object sender, RoutedEventArgs e)
        {
            var d = sender as DependencyObject;
            var grid = FindGrid(d);
            if (grid != null)
            {
                var popup = GridCommentService.GetPopupHost(grid);
                if (popup != null)
                    popup.Hide();
            }
        }

        static void frameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var d = sender as DependencyObject;
            var grid = FindGrid(d);
            if (grid != null)
            {
                var popup = GridCommentService.GetPopupHost(grid);
                if (popup != null)
                    return;
                var tpopupHost = GetInitializedDragWindow();
                GridCommentService.SetPopupHost(grid, tpopupHost);
#if (!SILVERLIGHT &&  !WinRT)
                GridCommentService.SetTimer(grid, new DispatcherTimer());
#else
                GridCommentService.SetTimer(grid, new DispatcherTimerExt());
#endif
#if (!SILVERLIGHT &&  !WinRT)
                grid.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(OnGridShowCommentPreviewMouseMove);
                grid.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseDown);
                grid.SelectionChanged += new GridSelectionChangedEventHandler(OnSelectionChanged);
#endif
#if SILVERLIGHT
                grid.MouseMove += OnGridShowCommentPreviewMouseMove;
                grid.MouseLeave += new System.Windows.Input.MouseEventHandler(OnGridMouseLeave);
#endif
#if WinRT
                grid.PointerMoved += grid_PointerMoved;
                grid.PointerExited += grid_PointerExited;
#endif

            }
        }

#if WinRT
        static void grid_PointerMoved(object sender, global::Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
#else
        private static void OnGridShowCommentPreviewMouseMove(object sender, MouseEventArgs e)
        {
#endif
            //Debug.WriteLine("Closed1");
            var grid = sender as GridControlBase;
            var popupHost = GridCommentService.GetPopupHost(grid);
            GridCommentService.SetMouseLeaveFlag(grid, false);
            GridCommentService.SetMouseDownFlag(grid, false);
            GridCommentService.SetSelectionChangedFlag(grid, false);
            if (popupHost == null)
            {
                throw new InvalidOperationException("Could not find Popup host container");
            }
            var timer = GridCommentService.GetTimer(grid);
#if !WinRT
            var cell = grid.PointToCellRowColumnIndexOutsideCells(e.GetPosition(grid), false);
#else
            var cell = grid.PointToCellRowColumnIndexOutsideCells(e.GetCurrentPoint(grid).Position, false);
#endif
            //Debug.WriteLine("OriginalCached " + cell.ColumnIndex + " " + cell.RowIndex);

            var cachedRowColIndex = GridCommentService.GetCachedRowColIndex(grid);
            var cachedForeCorner = GridCommentService.GetCachedForeCorner(grid);

            if (cell.IsEmpty)
            {
                cachedRowColIndex = cell;
                //Debug.WriteLine("Closed2");
                Close(popupHost, timer);
                return;
            }



            GridRenderStyleInfo style = grid.GetRenderStyleInfo(cell);
            var canShowComment = true;
            // if AllowKeepAliveOnlyCurrentCell is false, then the UIElement will be intialized on PreviewMouseMove internally in the grid itself, we simply check it thru the interface
            // var renderer = style.CellRenderer as IAllowKeepAliveOnlyCurrentCell; Unused local variable

            //if (renderer != null && !renderer.AllowKeepAliveOnlyCurrentCell )
            //{
            //    canShowComment = false ;
            //}
            if (grid.CurrentCell != null && grid.CurrentCell.HasCurrentCellAt(cell) && grid.CurrentCell.IsEditing)
            {
                canShowComment = false;
            }

            if (canShowComment && (style.Comment != null || style.CommentTemplateKey != null || style.HasGridCommentStyleInfo))
            {
#if !WinRT
                var contentControl =((System.Windows.Controls.Grid)popupHost.Child).Children[0] as ContentControl;
#else
                var contentControl = ((Windows.UI.Xaml.Controls.Grid)popupHost.Child).Children[0] as ContentControl;
#endif
                bool firstVisible, lastVisible;
                VisibleLineInfo ColumnfirstLine, ColumnlastLine, RowfirstLine, RowlastLine;
                grid.ScrollColumns.GetLinesAndVisibility(cell.ColumnIndex, cell.ColumnIndex, false, out firstVisible, out lastVisible, out ColumnfirstLine, out ColumnlastLine);
                grid.ScrollRows.GetLinesAndVisibility(cell.RowIndex, cell.RowIndex, false, out firstVisible, out lastVisible, out RowfirstLine, out RowlastLine);
                Rect rect = grid.RangeToRect(RowlastLine.Region, ColumnlastLine.Region, GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex), false, true);
#if !WinRT
                Point mousePos = e.GetPosition(grid);
#else
                Point mousePos = e.GetCurrentPoint(grid).Position;
#endif

                var Forecorner = string.Empty;

                if (style.GridCommentStyleInfo != null)
                {

                    Forecorner = GetForeCorner(mousePos, rect);

                    if (string.IsNullOrEmpty(Forecorner))
                    {
                        cachedForeCorner = Forecorner;
                        Close(popupHost, timer);
                        return;
                    }
                }
                if (cachedForeCorner == Forecorner && cachedRowColIndex == cell && timer.IsEnabled)
                    return;
                if (style.GridCommentStyleInfo != null)
                {
                    Close(popupHost, timer);
                    cachedRowColIndex = cell;
                    cachedForeCorner = Forecorner;
                    GridCommentService.SetCachedRowColIndex(grid, cachedRowColIndex);
                    GridCommentService.SetCachedForeCorner(grid, cachedForeCorner);
                    DataTemplate dt = null;

                    if (cachedForeCorner == "BottomLeft" && style.GridCommentStyleInfo.BottomLeftComment != null)
                    {
                        style.Comment = style.GridCommentStyleInfo.BottomLeftComment;

                        if (style.GridCommentStyleInfo.BottomLeftCommentTemplateKey != null)
                            dt = (DataTemplate)style.GridControl.TryFindResource(style.GridCommentStyleInfo.BottomLeftCommentTemplateKey);
                        else
                            dt = style.GridControl.GetDefaultCommentTemplate();
                    }
                    else if (cachedForeCorner == "TopLeft" && style.GridCommentStyleInfo.TopLeftComment != null)
                    {
                        style.Comment = style.GridCommentStyleInfo.TopLeftComment;

                        if (style.GridCommentStyleInfo.TopLeftCommentTemplateKey != null)
                            dt = (DataTemplate)style.GridControl.TryFindResource(style.GridCommentStyleInfo.TopLeftCommentTemplateKey);
                        else
                            dt = style.GridControl.GetDefaultCommentTemplate();
                    }
                    else if (cachedForeCorner == "BottomRight" && style.GridCommentStyleInfo.BottomRightComment != null)
                    {
                        style.Comment = style.GridCommentStyleInfo.BottomRightComment;

                        if (style.GridCommentStyleInfo.BottomRightCommentTemplateKey != null)
                            dt = (DataTemplate)style.GridControl.TryFindResource(style.GridCommentStyleInfo.BottomRightCommentTemplateKey);
                        else
                            dt = style.GridControl.GetDefaultCommentTemplate();
                    }
                    else if (cachedForeCorner == "TopRight" && style.GridCommentStyleInfo.TopRightComment != null)
                    {
                        style.Comment = style.GridCommentStyleInfo.TopRightComment;

                        if (style.GridCommentStyleInfo.TopRightCommentTemplateKey != null)
                            dt = (DataTemplate)style.GridControl.TryFindResource(style.GridCommentStyleInfo.TopRightCommentTemplateKey);
                        else
                            dt = style.GridControl.GetDefaultCommentTemplate();
                    }
                    //It will show the Default Comment service
                    else if (cachedForeCorner == "TopRight")
                    {
                        if (style.CommentTemplateKey != null)
                            dt = (DataTemplate)style.GridControl.TryFindResource(style.CommentTemplateKey);
                        else
                            dt = style.GridControl.GetDefaultCommentTemplate();
                    }

                    contentControl.Content = style;

                    if (dt != null && style.Comment != null)
                    {
                        contentControl.ContentTemplate = dt;
                        if (timer != null)
                        {
                            timer.Tick += OnTick;
                            var CommentDelay = GridCommentService.GetCommentDelay(grid);
                            timer.Interval = new TimeSpan(0, 0, 0, 0, CommentDelay != 0 ? CommentDelay : 500);
#if (!SILVERLIGHT &&  !WinRT)
                            timer.IsEnabled = true;
#endif

                            timer.Start();
                            timer.Tag = new Dictionary<string, object>() { { "CachedRowColIndex", cachedRowColIndex }, { "PopupHost", popupHost }, { "Grid", grid }, { "Corner", Forecorner } };
                        }
                    }

                }

                else
                {
                    Close(popupHost, timer);
                    cachedRowColIndex = cell;
                    GridCommentService.SetCachedRowColIndex(grid, cachedRowColIndex);
                    GridCommentService.SetCachedForeCorner(grid, "BottomLeft");
                    contentControl.Content = style;
                    DataTemplate dt = null;

                    Forecorner = GetForeCorner(mousePos, rect);
                    if (string.IsNullOrEmpty(Forecorner))
                        return;

                    if (style.CommentTemplateKey != null)
                        dt = (DataTemplate)style.GridControl.TryFindResource(style.CommentTemplateKey);
                    else
                        dt = style.GridControl.GetDefaultCommentTemplate();
                    if (dt != null)
                    {
                        contentControl.ContentTemplate = dt;
                        if (timer != null)
                        {
                            timer.Tick += OnTick;
                            var CommentDelay = GridCommentService.GetCommentDelay(grid);
                            timer.Interval = new TimeSpan(0, 0, 0, 0, CommentDelay != 0 ? CommentDelay : 500);
#if (!SILVERLIGHT &&  !WinRT)
                            timer.IsEnabled = true;
#endif

                            timer.Start();
                            timer.Tag = new Dictionary<string, object>() { { "CachedRowColIndex", cachedRowColIndex }, { "PopupHost", popupHost }, { "Grid", grid }, { "Corner", Forecorner } };
                        }
                    }
                }
            }
            else
            {
                //Debug.WriteLine("Closed4");
                GridCommentService.SetCachedRowColIndex(grid, RowColumnIndex.Empty);
                Close(popupHost, timer);
            }
#if SILVERLIGHT
            if (!popupHost.IsShowing)
            {
                var p = e.GetPosition(Application.Current.RootVisual);
                popupHost.popupWindow.HorizontalOffset = p.X + popupHost.HorizontalOffset;
                popupHost.popupWindow.VerticalOffset = p.Y + popupHost.VerticalOffset;
            }
#endif
        }

        private static string GetForeCorner(Point mousePos, Rect rect)
        {
            string Forecorner = "";

            if ((rect.Bottom >= mousePos.Y) && (mousePos.Y >= rect.Bottom - 7) && (rect.Left <= mousePos.X) && (mousePos.X <= rect.Left + 7))
            {
                Forecorner = "BottomLeft";
            }
            else if ((rect.Top <= mousePos.Y) && (mousePos.Y <= rect.Top + 7) && (rect.Left <= mousePos.X) && (mousePos.X <= rect.Left + 7))
            {
                Forecorner = "TopLeft";
            }
            else if ((rect.Bottom >= mousePos.Y) && (mousePos.Y >= rect.Bottom - 7) && (rect.Right >= mousePos.X) && (mousePos.X >= rect.Right - 7))
            {
                Forecorner = "BottomRight";
            }
            else if ((rect.Top <= mousePos.Y) && (mousePos.Y <= rect.Top + 7) && (rect.Right >= mousePos.X) && (mousePos.X >= rect.Right - 7))
            {
                Forecorner = "TopRight";
            }
            else
            {
                Forecorner = string.Empty;
            }
            return Forecorner;
        }

#if (!SILVERLIGHT &&  !WinRT)
        private static void Close(PopupDragWindow popupHost, DispatcherTimer timer)
#else
        private static void Close(PopupDragWindow popupHost, DispatcherTimerExt timer)
#endif
        {
            if (timer.IsEnabled)
            {
                timer.Tick -= OnTick;
                timer.Tick += OnTick;
                timer.Stop();
#if (!SILVERLIGHT &&  !WinRT)
                timer.IsEnabled = false;
#endif
            }
            if (popupHost.IsDragging)
            {
                //Debug.WriteLine("Closed5");
                popupHost.StopDrag();
                popupHost.Hide();
            }
        }
#if WinRT
        static void OnTick(object sender, object e)
        {
#else
        //private static PopupDragWindow tpopupHost;
        private static void OnTick(object sender, EventArgs e)
        {
#endif
#if (!SILVERLIGHT &&  !WinRT)
            var timer = sender as DispatcherTimer;
#else
            var timer = sender as DispatcherTimerExt;
#endif
            var dict = timer.Tag as Dictionary<string, object>;
            var prevRowColIndex = (RowColumnIndex)dict["CachedRowColIndex"]; //(RowColumnIndex)timer.Tag;
            var grid = (GridControlBase)dict["Grid"];
            // string corner = (string)dict["Corner"];
            var cachedRowColIndex = GridCommentService.GetCachedRowColIndex(grid);
            var mouseLeaveFlag = GridCommentService.GetMouseLeaveFlag(grid);
            if (mouseLeaveFlag == true)
            {
                GridCommentService.SetMouseLeaveFlag(grid, false);
                timer.Tick -= OnTick;
                return;
            }

            var mouseDownFlag = GridCommentService.GetMouseDownFlag(grid);
            if (mouseDownFlag == true)
            {
                GridCommentService.SetMouseDownFlag(grid, false);
                timer.Tick -= OnTick;
                return;
            }

            var selectionChangedFlag = GridCommentService.GetSelectionChangedFlag(grid);
            if (selectionChangedFlag == true)
            {
                GridCommentService.SetSelectionChangedFlag(grid, false);
                timer.Tick -= OnTick;
                return;
            }

            if (prevRowColIndex != null && prevRowColIndex == cachedRowColIndex)
            {
                //Debug.WriteLine("Cached " + cachedRowColIndex.ColumnIndex + " " + cachedRowColIndex.RowIndex);


                var tpopupHost = (PopupDragWindow)dict["PopupHost"];

#if !WinRT
                grid.RaiseCellCommentOpening(cachedRowColIndex, ((System.Windows.Controls.Grid)tpopupHost.Child).Children[0] as ContentControl, (string)dict["Corner"]);
#else
                grid.RaiseCellCommentOpening(cachedRowColIndex, ((Windows.UI.Xaml.Controls.Grid)tpopupHost.Child).Children[0] as ContentControl, (string)dict["Corner"]);
#endif
                tpopupHost.StartDrag();
#if (!SILVERLIGHT &&  !WinRT)
                if (grid.IsMouseOver)
#endif
                tpopupHost.Show();
            }
            timer.Stop();
            timer.Tick -= OnTick;
        }

        #endregion
    }

}
