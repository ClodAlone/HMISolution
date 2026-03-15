#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.PivotGrid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using System.Windows.Data;
    using Syncfusion.Windows.Controls.Grid;

    /// <summary>
    /// A helper class to show tooltips for grid cells.
    /// </summary>
    public class PivotGridTooltipService : IDisposable
    {
        private static GridControlBase FindGrid(DependencyObject d)
        {
            var frameWorkElement = d as FrameworkElement;
            if (frameWorkElement == null)
            {
                return null;
            }

            if (frameWorkElement is GridDataControl)
            {
                return frameWorkElement.FindElementOfType<GridDataControlBaseImpl>();
            }

            // this will find the GridControlBase from the visual tree of any templated grid control
            return frameWorkElement.FindElementOfType<GridControlBase>();
        }

        #region ShowTooltips

        private static readonly DependencyProperty CachedRowColumnIndexProperty = DependencyProperty.RegisterAttached("CachedRowColIndex", typeof(RowColumnIndex), typeof(PivotGridTooltipService), new PropertyMetadata(RowColumnIndex.Empty));

        private static RowColumnIndex GetCachedRowColIndex(DependencyObject dpo)
        {
            return (RowColumnIndex)dpo.GetValue(PivotGridTooltipService.CachedRowColumnIndexProperty);
        }

        private static void SetCachedRowColIndex(DependencyObject dpo, RowColumnIndex value)
        {
            dpo.SetValue(PivotGridTooltipService.CachedRowColumnIndexProperty, value);
        }

        private static readonly DependencyProperty PopupHostProperty = DependencyProperty.RegisterAttached("PopupHost", typeof(PopupDragWindow), typeof(PivotGridTooltipService), new PropertyMetadata(null));

        private static PopupDragWindow GetPopupHost(DependencyObject dpo)
        {
            return (PopupDragWindow)dpo.GetValue(PivotGridTooltipService.PopupHostProperty);
        }

        private static void SetPopupHost(DependencyObject dpo, PopupDragWindow value)
        {
            dpo.SetValue(PivotGridTooltipService.PopupHostProperty, value);
        }

        private static bool HasPopupHost(DependencyObject dpo)
        {
            var popup = dpo.GetValue(PivotGridTooltipService.PopupHostProperty);
            return popup != null;
        }

#if !SILVERLIGHT
        private static readonly DependencyProperty TimerProperty = DependencyProperty.RegisterAttached("Timer", typeof(DispatcherTimer), typeof(PivotGridTooltipService), new PropertyMetadata(null));

        private static DispatcherTimer GetTimer(DependencyObject dpo)
        {
            return (DispatcherTimer)dpo.GetValue(PivotGridTooltipService.TimerProperty);
        }

        private static void SetTimer(DependencyObject dpo, DispatcherTimer timer)
        {
            dpo.SetValue(PivotGridTooltipService.TimerProperty, timer);
        }
#else
        private static readonly DependencyProperty TimerProperty = DependencyProperty.RegisterAttached("Timer", typeof(DispatcherTimerExt), typeof(PivotGridTooltipService), new PropertyMetadata(null));

        private static DispatcherTimerExt GetTimer(DependencyObject dpo)
        {
            return (DispatcherTimerExt)dpo.GetValue(PivotGridTooltipService.TimerProperty);
        }

        private static void SetTimer(DependencyObject dpo, DispatcherTimerExt timer)
        {
            dpo.SetValue(PivotGridTooltipService.TimerProperty, timer);
        }
#endif

        private static readonly DependencyProperty TooltipDelayProperty = DependencyProperty.RegisterAttached("TooltipDelay", typeof(int?), typeof(PivotGridTooltipService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of TooltipDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static int? GetTooltipDelay(DependencyObject dpo)
        {
            return (int?)dpo.GetValue(PivotGridTooltipService.TooltipDelayProperty);
        }

        /// <summary>
        /// Sets the value of TooltipDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetTooltipDelay(DependencyObject dpo, int value)
        {
            dpo.SetValue(PivotGridTooltipService.TooltipDelayProperty, value);
        }

        private static PopupDragWindow GetInitializedDragWindow(FrameworkElement parent)
        {

#if !SILVERLIGHT
            var popupDragWindow = new PopupDragWindow() { DestroyChildOnStopDrag = false, Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint, HorizontalOffset = 10d, VerticalOffset = 20d, AllowsTransparency = true };
            var innerGrid = new Grid();
            innerGrid.Children.Add(new ContentControl());
            popupDragWindow.Child = innerGrid;

#else
            var popupDragWindow = new PopupDragWindow() { DestroyChildOnStopDrag = false, IsPopupUnderMousePoint = true, HorizontalOffset = 10d, VerticalOffset = 20d };
            var innerGrid = new Grid();
            innerGrid.Children.Add(new ContentControl());
            popupDragWindow.ProvideVisual(innerGrid);
#endif
            return popupDragWindow;
        }

        private static readonly DependencyProperty ShowTooltipsProperty = DependencyProperty.RegisterAttached("ShowTooltips", typeof(bool), typeof(PivotGridTooltipService), new PropertyMetadata(OnShowTooltipsChanged));

        /// <summary>
        /// Gets the value of ShowTooltips property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool GetShowTooltips(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(PivotGridTooltipService.ShowTooltipsProperty);
        }

        /// <summary>
        /// Sets the value of ShowTooltips property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetShowTooltips(DependencyObject dpo, bool value)
        {
            dpo.SetValue(PivotGridTooltipService.ShowTooltipsProperty, value);
        }

        private static void OnShowTooltipsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
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
        private static readonly DependencyProperty SelectionChangedFlagProperty = DependencyProperty.RegisterAttached("SelectionChangedFlag", typeof(bool?), typeof(PivotGridTooltipService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool? GetSelectionChangedFlag(DependencyObject dpo)
        {
            return (bool?)dpo.GetValue(PivotGridTooltipService.SelectionChangedFlagProperty);
        }

        /// <summary>
        /// Sets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetSelectionChangedFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(PivotGridTooltipService.SelectionChangedFlagProperty, value);
        }
        #endregion

        #region MouseDown


        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty MouseDownFlagProperty = DependencyProperty.RegisterAttached("MouseDownFlag", typeof(bool?), typeof(PivotGridTooltipService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool? GetMouseDownFlag(DependencyObject dpo)
        {
            return (bool?)dpo.GetValue(PivotGridTooltipService.MouseDownFlagProperty);
        }

        /// <summary>
        /// Sets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetMouseDownFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(PivotGridTooltipService.MouseDownFlagProperty, value);
        }

        #endregion

        #region MouseLeave


        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty MouseLeaveFlagProperty = DependencyProperty.RegisterAttached("MouseLeaveFlag", typeof(bool?), typeof(PivotGridTooltipService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of TooltipDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool? GetMouseLeaveFlag(DependencyObject dpo)
        {
            return (bool?)dpo.GetValue(PivotGridTooltipService.MouseLeaveFlagProperty);
        }

        /// <summary>
        /// Sets the value of TooltipDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetMouseLeaveFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(PivotGridTooltipService.MouseLeaveFlagProperty, value);
        }
        #endregion

        /// <summary>
        /// Releases all the resources used by this component.
        /// </summary>
        /// <param name="dop">The parent grid.</param>
        public static void Dispose(DependencyObject dop)
        {
            var d = FindGrid(dop);
            if (d != null)
            {
                var popuphost = PivotGridTooltipService.GetPopupHost(d);
                if (popuphost != null)
                {
                    popuphost.Dispose();
                    popuphost = null;
                }

                d.ClearValue(PivotGridTooltipService.MouseLeaveFlagProperty);
                d.ClearValue(PivotGridTooltipService.PopupHostProperty);
                d.ClearValue(PivotGridTooltipService.CachedRowColumnIndexProperty);
                d.ClearValue(PivotGridTooltipService.ShowTooltipsProperty);
                d.ClearValue(PivotGridTooltipService.TimerProperty);
                d.ClearValue(PivotGridTooltipService.TooltipDelayProperty);
                UnwireMouseMove(d);
            }
        }

        private static void UnwireMouseMove(DependencyObject d)
        {
            var grid = FindGrid(d);
            var frameworkElement = d as FrameworkElement;
            if (grid != null)
            {
#if !SILVERLIGHT
                grid.PreviewMouseMove -= new System.Windows.Input.MouseEventHandler(OnGridShowTooltipPreviewMouseMove);
                grid.PreviewMouseDown -= new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseDown);
                grid.SelectionChanged -= new GridSelectionChangedEventHandler(OnSelectionChanged);
                grid.MouseLeave -= new System.Windows.Input.MouseEventHandler(grid_MouseLeave);
#endif
                grid.MouseMove -= OnGridShowTooltipPreviewMouseMove;
                grid.MouseLeave -= new System.Windows.Input.MouseEventHandler(OnGridMouseLeave);
                grid.CurrentCellActivated -= new ComponentModel.GridRoutedEventHandler(grid_CurrentCellActivated);
                frameworkElement.Loaded -= new RoutedEventHandler(frameworkElement_Loaded);
                frameworkElement.Unloaded -= new RoutedEventHandler(frameworkElement_Unloaded);
            }
        }

        private static void WireMouseMove(DependencyObject d)
        {
            var frameworkElement = d as FrameworkElement;
#if !SILVERLIGHT
            if (frameworkElement != null && !frameworkElement.IsLoaded)
#else
            if (frameworkElement != null)
#endif
            {
                frameworkElement.Loaded += new RoutedEventHandler(frameworkElement_Loaded);
                frameworkElement.Unloaded += new RoutedEventHandler(frameworkElement_Unloaded);
#if SILVERLIGHT
                var grid = FindGrid(d);
                if (grid != null)
                {
                    InitializeGridForTooltip(grid);
                }
#endif
            }
#if !SILVERLIGHT
            else
            {
                var grid = FindGrid(d);
                if (grid != null)
                {
                    InitializeGridForTooltip(grid);
                }
            }
#endif
        }

        static void frameworkElement_Unloaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var grid = FindGrid(frameworkElement);
            if (grid != null)
            {
                var popup = PivotGridTooltipService.GetPopupHost(grid);
                if (popup != null && popup.IsShowing)
                    popup.Hide();
            }
        }

        private static void frameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
#if !SILVERLIGHT
            frameworkElement.Loaded -= new RoutedEventHandler(frameworkElement_Loaded);
#endif
            var grid = FindGrid(frameworkElement);
            if (grid != null)
            {
                InitializeGridForTooltip(grid);
            }
        }



        private static void InitializeGridForTooltip(GridControlBase grid)
        {
            if (!PivotGridTooltipService.HasPopupHost(grid))
            {
                var tpopupHost = GetInitializedDragWindow(grid);
                PivotGridTooltipService.SetPopupHost(grid, tpopupHost);

                var validationpopupHost = GetInitializeDataValidationDragWindow(grid);

#if !SILVERLIGHT
                PivotGridTooltipService.SetTimer(grid, new DispatcherTimer());
                grid.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(OnGridShowTooltipPreviewMouseMove);
                grid.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseDown);
                grid.SelectionChanged += new GridSelectionChangedEventHandler(OnSelectionChanged);
                grid.CurrentCellActivated += new ComponentModel.GridRoutedEventHandler(grid_CurrentCellActivated);
                grid.MouseLeave += new System.Windows.Input.MouseEventHandler(grid_MouseLeave);
#else
                PivotGridTooltipService.SetTimer(grid, new DispatcherTimerExt());
                grid.MouseMove += OnGridShowTooltipPreviewMouseMove;
                grid.MouseLeave += new System.Windows.Input.MouseEventHandler(OnGridMouseLeave);
                grid.CurrentCellActivated += new ComponentModel.GridRoutedEventHandler(grid_CurrentCellActivated);
#endif

            }
        }
        // This event is hooked foe the issue SD10328-In GDC, Tooltip showing when we changed to other window by ALT+TAB
        static void grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var grid = sender as GridControlBase;
            var tooltip = PivotGridTooltipService.GetPopupHost(grid);
            var timer = PivotGridTooltipService.GetTimer(grid);
            if (timer.IsEnabled)
            {
                timer.Tick -= OnDataValidationTick;
                timer.Stop();
            }
            if (tooltip != null)
            {
                if (tooltip.IsShowing)
                    tooltip.Hide();
            }
        }

        private static PopupDragWindow GetInitializeDataValidationDragWindow(FrameworkElement parent)
        {
#if !SILVERLIGHT
            var popupDragWindow = new PopupDragWindow() { DestroyChildOnStopDrag = false, Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute, HorizontalOffset = 10d, VerticalOffset = 20d, PlacementTarget = parent, AllowsTransparency = true };
            var innerGrid = new Grid();
            innerGrid.Children.Add(new ContentControl());
            popupDragWindow.Child = innerGrid;
#else
            var popupDragWindow = new PopupDragWindow() { DestroyChildOnStopDrag = false, IsPopupUnderMousePoint = false };
            var innerGrid = new Grid();
            innerGrid.Children.Add(new ContentControl());
            popupDragWindow.ProvideVisual(innerGrid);
#endif
            return popupDragWindow;
        }



#if !SILVERLIGHT
        private static void OnGridMouseLeave(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            PivotGridTooltipService.SetMouseLeaveFlag(grid, true);
        }

        private static void OnSelectionChanged(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            PivotGridTooltipService.SetSelectionChangedFlag(grid, true);
        }

        private static void OnPreviewMouseDown(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            PivotGridTooltipService.SetMouseDownFlag(grid, true);
        }

#endif

#if SILVERLIGHT
        private static void OnGridMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
#else
        private static GridControlBase UpdatePopUpHost(object sender)
#endif
        {
            var grid = sender as GridControlBase;
            var popupHost = PivotGridTooltipService.GetPopupHost(grid);
            if (popupHost != null && popupHost.IsDragging)
            {
                popupHost.StopDrag();
#if !SILVERLIGHT
                popupHost.IsOpen = false;
#else
                popupHost.Hide();
#endif
            }
#if !SILVERLIGHT
            return grid;
#else
            PivotGridTooltipService.SetMouseLeaveFlag(grid, true);
#endif

        }

        private static Point displayPosition { get; set; }

        private static Point GetCellOffset(GridControlBase grid, Point gridOffset, RowColumnIndex RowColIndex)
        {
            VisibleLineInfo visibleRow = null;
            VisibleLineInfo visibleColumn = null;
            Rect cellrect = Rect.Empty;
            Point position = new Point();
            grid.GetVisibleRowAndColumn(RowColIndex, out visibleRow, out visibleColumn);
            if (visibleRow != null && visibleColumn != null)
            {
                cellrect = new Rect(visibleColumn.Origin, visibleRow.Origin, visibleColumn.Size, visibleRow.Size);
            }
            if (!cellrect.IsEmpty)
            {
                position.X = cellrect.Top;
                position.Y = cellrect.Left;
            }
            return position;
        }

        private static Point GetTooltipPosition(GridControlBase grid, Point gridOffset, RowColumnIndex RowColIndex)
        {
            Point position = new Point();
            VisibleLineInfo visibleRow = null;
            VisibleLineInfo visibleColumn = null;
            Rect cellrect = Rect.Empty;
            grid.GetVisibleRowAndColumn(RowColIndex, out visibleRow, out visibleColumn);
            if (visibleRow != null && visibleColumn != null)
            {
                cellrect = new Rect(visibleColumn.Origin, visibleRow.Origin, visibleColumn.Size, visibleRow.Size);
            }
            if (!cellrect.IsEmpty)
            {
                position.X = gridOffset.X + cellrect.Right - (cellrect.Width / 2);
                position.Y = gridOffset.Y + cellrect.Bottom + 2;
            }
            return position;
        }

        private static Point GetGridOffset(GridControlBase grid)
        {
            Point position = new Point();
#if SILVERLIGHT
            position = grid.PointFromRootVisual();
#else
            Point locationToScreen = grid.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(grid);
            position = source.CompositionTarget.TransformFromDevice.Transform(locationToScreen);
#endif
            return position;
        }

        static void grid_CurrentCellActivated(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            var grid = sender as GridControlBase;
            System.Diagnostics.Debug.WriteLine("CurrentCellActivated");

            RowColumnIndex RowColIndex = new RowColumnIndex();
            if (grid.CurrentCell.HasCurrentCell)
                RowColIndex = grid.CurrentCell.CellRowColumnIndex;
            if (!RowColIndex.IsEmpty)
            {
                Point gridPosition = GetGridOffset(grid);
                Point tooltipPosition = new Point();
                tooltipPosition = GetTooltipPosition(grid, gridPosition, RowColIndex);
                displayPosition = tooltipPosition;
            }
        }

#if SILVERLIGHT
        private static void CloseDataValidationTooltip(PopupDragWindow popupHost, DispatcherTimerExt timer)
#else
        private static void CloseDataValidationTooltip(PopupDragWindow popupHost, DispatcherTimer timer)
#endif
        {
            if (timer.IsEnabled)
            {
                timer.Tick -= OnDataValidationTick;
                timer.Stop();
            }
            if (popupHost.IsDragging)
            {
                popupHost.StopDrag();
                popupHost.Hide();
            }
        }

        private static void OnDataValidationTick(object sender, EventArgs e)
        {
#if SILVERLIGHT
            var timer = sender as DispatcherTimerExt;
#else
            var timer = sender as DispatcherTimer;
#endif
            var dict = timer.Tag as Dictionary<string, object>;
            var prevRowColIndex = (RowColumnIndex)dict["CachedRowColIndex"];
            var grid = (GridControlBase)dict["Grid"];
            var mouseLeaveFlag = PivotGridTooltipService.GetMouseLeaveFlag(grid);
            if (mouseLeaveFlag == true)
            {
                PivotGridTooltipService.SetMouseLeaveFlag(grid, false);
                timer.Tick -= OnDataValidationTick;
                return;
            }
            if (prevRowColIndex != null)
            {
                var tpopupHost = (PopupDragWindow)dict["PopupHost"];
                tpopupHost.StartDrag();
                tpopupHost.Show();
            }
            timer.Stop();
            timer.Tick -= OnDataValidationTick;
        }



        private static void OnGridShowTooltipPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var grid = sender as GridControlBase;
            var popupHost = PivotGridTooltipService.GetPopupHost(grid);
            PivotGridTooltipService.SetMouseLeaveFlag(grid, false);
            PivotGridTooltipService.SetMouseDownFlag(grid, false);
            PivotGridTooltipService.SetSelectionChangedFlag(grid, false);

            if (popupHost == null)
            {
                throw new InvalidOperationException("Could not find Popup host container");
            }
            var timer = PivotGridTooltipService.GetTimer(grid);
            var cell = grid.PointToCellRowColumnIndexOutsideCells(e.GetPosition(grid), false);
            var cachedRowColIndex = PivotGridTooltipService.GetCachedRowColIndex(grid);
            if (cell.IsEmpty)
            {
                cachedRowColIndex = cell;
                Close(popupHost, timer);
                return;
            }
            GridRenderStyleInfo style = grid.GetRenderStyleInfo(cell);
            var canShowTooltips = true;

            var renderer = style.CellRenderer as GridCellRendererBase;
            if (grid.CurrentCell != null && grid.CurrentCell.HasCurrentCellAt(cell) && grid.CurrentCell.IsEditing)
            {
                canShowTooltips = false;
            }
            if (canShowTooltips && style.ShowTooltip)
            {
                var mouseLeaveFlag = PivotGridTooltipService.GetMouseLeaveFlag(grid);
                if (mouseLeaveFlag == true)
                {
                    PivotGridTooltipService.SetMouseLeaveFlag(grid, false);
                }
                var contentControl = ((Grid)popupHost.Child).Children[0] as ContentControl;
                if (cachedRowColIndex != cell || (cachedRowColIndex == cell && !popupHost.IsShowing))
                {
                    Close(popupHost, timer);
                    cachedRowColIndex = cell;
                    PivotGridTooltipService.SetCachedRowColIndex(grid, cachedRowColIndex);
                    contentControl.Content = style;
                    DataTemplate dt = null;

                    if (style.TooltipTemplateKey != null)
                    {
                        dt = (DataTemplate)style.GridControl.TryFindResource(style.TooltipTemplateKey);
                    }

                    if (dt != null)
                    {
                        contentControl.ContentTemplate = dt;
                        if (timer != null)
                        {
                            timer.Tick += OnTick;
                            var tooltipDelay = PivotGridTooltipService.GetTooltipDelay(grid);
                            timer.Interval = new TimeSpan(0, 0, 0, 0, tooltipDelay.HasValue ? tooltipDelay.Value : 500);
#if !SILVERLIGHT
                            timer.IsEnabled = true;
#endif
                            timer.Start();
                            timer.Tag = new Dictionary<string, object>() { { "CachedRowColIndex", cachedRowColIndex }, { "PopupHost", popupHost }, { "Grid", grid } };
                        }
                    }
                }
            }
            else
            {
                PivotGridTooltipService.SetCachedRowColIndex(grid, RowColumnIndex.Empty);
                Close(popupHost, timer);
            }
        }

#if !SILVERLIGHT
        private static void Close(PopupDragWindow popupHost, DispatcherTimer timer)
#else
        private static void Close(PopupDragWindow popupHost, DispatcherTimerExt timer)
#endif
        {
            if (timer.IsEnabled)
            {
                timer.Tick -= OnTick;
                timer.Stop();
#if !SILVERLIGHT
                timer.IsEnabled = false;
#endif
            }
            if (popupHost.IsDragging)
            {
                popupHost.StopDrag();
                popupHost.Hide();
            }
        }

        private static void OnTick(object sender, EventArgs e)
        {
#if !SILVERLIGHT
            var timer = sender as DispatcherTimer;
#else
            var timer = sender as DispatcherTimerExt;
#endif
            var dict = timer.Tag as Dictionary<string, object>;
            var prevRowColIndex = (RowColumnIndex)dict["CachedRowColIndex"];
            var grid = (GridControlBase)dict["Grid"];
            var cachedRowColIndex = PivotGridTooltipService.GetCachedRowColIndex(grid);
            var mouseLeaveFlag = PivotGridTooltipService.GetMouseLeaveFlag(grid);
            if (mouseLeaveFlag == true)
            {
                PivotGridTooltipService.SetMouseLeaveFlag(grid, false);
                timer.Tick -= OnTick;
                return;
            }
#if !SILVERLIGHT
            var mouseDownFlag = PivotGridTooltipService.GetMouseDownFlag(grid);
            if (mouseDownFlag == true)
            {
                PivotGridTooltipService.SetMouseDownFlag(grid, false);
                timer.Tick -= OnTick;
                return;
            }

            var selectionChangedFlag = PivotGridTooltipService.GetSelectionChangedFlag(grid);
            if (selectionChangedFlag == true)
            {
                PivotGridTooltipService.SetSelectionChangedFlag(grid, false);
                timer.Tick -= OnTick;
                return;
            }
#endif
            if (prevRowColIndex != null && prevRowColIndex == cachedRowColIndex)
            {
                var tpopupHost = (PopupDragWindow)dict["PopupHost"];
                tpopupHost.StartDrag();
                                    RowColumnIndex RowColIndex = new RowColumnIndex();
                
                    if (grid.CurrentCell.HasCurrentCell)
                        RowColIndex = grid.CurrentCell.CellRowColumnIndex;
                    if (!RowColIndex.IsEmpty)
                    {
                        Point gridPosition = GetGridOffset(grid);
                        Point tooltipPosition = new Point();
                        RowColIndex.RowIndex = cachedRowColIndex.RowIndex;
                        RowColIndex.ColumnIndex = cachedRowColIndex.ColumnIndex;
                        tooltipPosition = GetTooltipPosition(grid, gridPosition, RowColIndex);
                        displayPosition = tooltipPosition;
                    }
                                           

#if !SILVERLIGHT
                if (grid.IsMouseOver)
#endif
                    tpopupHost.MoveTo(displayPosition);
                    tpopupHost.Show();
            }
            timer.Stop();
            timer.Tick -= OnTick;
        }

        #endregion
        /// <summary>
        /// Releases all the resources used by this component.
        /// </summary>
        public void Dispose()
        { }
    }

#if SILVERLIGHT
    internal class DispatcherTimerExt : DispatcherTimer
    {
        public object Tag { get; set; }
    }
#endif
}