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
using System.Windows.Data;
using System.Windows.Input;
namespace Syncfusion.Windows.Controls.Grid
{
#else
using Windows.UI.Xaml;
using Syncfusion.WinRT.Controls.Cells;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Input;
using Syncfusion.WinRT.Controls.Scroll;
using Windows.Foundation;
namespace Syncfusion.WinRT.Controls.Grid
{
#endif
    /// <summary>
    /// A helper class to show tooltips for grid cells.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridTooltipService : IDisposable
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

        #region DataValidationPopupHost

        private static readonly DependencyProperty DataValidationPopupHostProperty = DependencyProperty.RegisterAttached("DataValidationPopupHost", typeof(PopupDragWindow), typeof(GridTooltipService), new PropertyMetadata(null));

        private static PopupDragWindow GetDataValidationPopupHost(DependencyObject dpo)
        {
            return (PopupDragWindow)dpo.GetValue(GridTooltipService.DataValidationPopupHostProperty);
        }

        private static void SetDataValidationPopupHost(DependencyObject dpo, PopupDragWindow value)
        {
            dpo.SetValue(GridTooltipService.DataValidationPopupHostProperty, value);
        }
#if SILVERLIGHT || WinRT
        private static readonly DependencyProperty DataValidationTimerProperty = DependencyProperty.RegisterAttached("DataValidationTimer", typeof(DispatcherTimerExt), typeof(GridTooltipService), new PropertyMetadata(null));

        private static DispatcherTimerExt GetDataValidationTimer(DependencyObject dpo)
        {
            return (DispatcherTimerExt)dpo.GetValue(GridTooltipService.DataValidationTimerProperty);
        }

        private static void SetDataValidationTimer(DependencyObject dpo, DispatcherTimerExt timer)
        {
            dpo.SetValue(GridTooltipService.DataValidationTimerProperty, timer);
        }
#else
        private static readonly DependencyProperty DataValidationTimerProperty = DependencyProperty.RegisterAttached("DataValidationTimer", typeof(DispatcherTimer), typeof(GridTooltipService), new PropertyMetadata(null));

        private static DispatcherTimer GetDataValidationTimer(DependencyObject dpo)
        {
            return (DispatcherTimer)dpo.GetValue(GridTooltipService.DataValidationTimerProperty);
        }

        private static void SetDataValidationTimer(DependencyObject dpo, DispatcherTimer timer)
        {
            dpo.SetValue(GridTooltipService.DataValidationTimerProperty, timer);
        }
#endif

        private static readonly DependencyProperty DataValidationCachedRowColumnIndexProperty = DependencyProperty.RegisterAttached("DataValidationCachedRowColIndex", typeof(RowColumnIndex), typeof(GridTooltipService), new PropertyMetadata(RowColumnIndex.Empty));

        private static RowColumnIndex GetDataValidationCachedRowColIndex(DependencyObject dpo)
        {
            return (RowColumnIndex)dpo.GetValue(GridTooltipService.DataValidationCachedRowColumnIndexProperty);
        }

        private static void SetDataValidationCachedRowColIndex(DependencyObject dpo, RowColumnIndex value)
        {
            dpo.SetValue(GridTooltipService.DataValidationCachedRowColumnIndexProperty, value);
        }

        private static bool HasDataValidationPopupHost(DependencyObject dpo)
        {
            var popup = dpo.GetValue(GridTooltipService.DataValidationPopupHostProperty);
            return popup != null;
        }

        #endregion


        #region ShowTooltips

        private static readonly DependencyProperty CachedRowColumnIndexProperty = DependencyProperty.RegisterAttached("CachedRowColIndex", typeof(RowColumnIndex), typeof(GridTooltipService), new PropertyMetadata(RowColumnIndex.Empty));

        private static RowColumnIndex GetCachedRowColIndex(DependencyObject dpo)
        {
            return (RowColumnIndex)dpo.GetValue(GridTooltipService.CachedRowColumnIndexProperty);
        }

        private static void SetCachedRowColIndex(DependencyObject dpo, RowColumnIndex value)
        {
            dpo.SetValue(GridTooltipService.CachedRowColumnIndexProperty, value);
        }

        private static readonly DependencyProperty PopupHostProperty = DependencyProperty.RegisterAttached("PopupHost", typeof(PopupDragWindow), typeof(GridTooltipService), new PropertyMetadata(null));

        private static PopupDragWindow GetPopupHost(DependencyObject dpo)
        {
            return (PopupDragWindow)dpo.GetValue(GridTooltipService.PopupHostProperty);
        }

        private static void SetPopupHost(DependencyObject dpo, PopupDragWindow value)
        {
            dpo.SetValue(GridTooltipService.PopupHostProperty, value);
        }

        private static bool HasPopupHost(DependencyObject dpo)
        {
            var popup = dpo.GetValue(GridTooltipService.PopupHostProperty);
            return popup != null;
        }

#if (!SILVERLIGHT && !WinRT)
        private static readonly DependencyProperty TimerProperty = DependencyProperty.RegisterAttached("Timer", typeof(DispatcherTimer), typeof(GridTooltipService), new PropertyMetadata(null));

        private static DispatcherTimer GetTimer(DependencyObject dpo)
        {
            return (DispatcherTimer)dpo.GetValue(GridTooltipService.TimerProperty);
        }

        private static void SetTimer(DependencyObject dpo, DispatcherTimer timer)
        {
            dpo.SetValue(GridTooltipService.TimerProperty, timer);
        }
#else
        private static readonly DependencyProperty TimerProperty = DependencyProperty.RegisterAttached("Timer", typeof(DispatcherTimerExt), typeof(GridTooltipService), new PropertyMetadata(null));

        private static DispatcherTimerExt GetTimer(DependencyObject dpo)
        {
            return (DispatcherTimerExt)dpo.GetValue(GridTooltipService.TimerProperty);
        }

        private static void SetTimer(DependencyObject dpo, DispatcherTimerExt timer)
        {
            dpo.SetValue(GridTooltipService.TimerProperty, timer);
        }
#endif

        private static readonly DependencyProperty TooltipDelayProperty = DependencyProperty.RegisterAttached("TooltipDelay", typeof(int?), typeof(GridTooltipService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of TooltipDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static int? GetTooltipDelay(DependencyObject dpo)
        {
            return (int?)dpo.GetValue(GridTooltipService.TooltipDelayProperty);
        }

        /// <summary>
        /// Sets the value of TooltipDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetTooltipDelay(DependencyObject dpo, int value)
        {
            dpo.SetValue(GridTooltipService.TooltipDelayProperty, value);
        }

        private static PopupDragWindow GetInitializedDragWindow(FrameworkElement parent)
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

        private static readonly DependencyProperty ShowTooltipsProperty = DependencyProperty.RegisterAttached("ShowTooltips", typeof(bool), typeof(GridTooltipService),
#if !WinRT
            new PropertyMetadata(OnShowTooltipsChanged));
#else
 new PropertyMetadata(false, OnShowTooltipsChanged));
#endif

        /// <summary>
        /// Gets the value of ShowTooltips property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool GetShowTooltips(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridTooltipService.ShowTooltipsProperty);
        }

        /// <summary>
        /// Sets the value of ShowTooltips property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetShowTooltips(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridTooltipService.ShowTooltipsProperty, value);
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
        #endregion

        #region SelectionChanged
        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty SelectionChangedFlagProperty = DependencyProperty.RegisterAttached("SelectionChangedFlag", typeof(bool?), typeof(GridTooltipService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool? GetSelectionChangedFlag(DependencyObject dpo)
        {
            return (bool?)dpo.GetValue(GridTooltipService.SelectionChangedFlagProperty);
        }

        /// <summary>
        /// Sets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetSelectionChangedFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridTooltipService.SelectionChangedFlagProperty, value);
        }
        #endregion

        #region MouseDown


        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty MouseDownFlagProperty = DependencyProperty.RegisterAttached("MouseDownFlag", typeof(bool?), typeof(GridTooltipService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool? GetMouseDownFlag(DependencyObject dpo)
        {
            return (bool?)dpo.GetValue(GridTooltipService.MouseDownFlagProperty);
        }

        /// <summary>
        /// Sets the value of CommentDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetMouseDownFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridTooltipService.MouseDownFlagProperty, value);
        }

        #endregion

        #region MouseLeave


        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty MouseLeaveFlagProperty = DependencyProperty.RegisterAttached("MouseLeaveFlag", typeof(bool?), typeof(GridTooltipService), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of TooltipDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool? GetMouseLeaveFlag(DependencyObject dpo)
        {
            return (bool?)dpo.GetValue(GridTooltipService.MouseLeaveFlagProperty);
        }

        /// <summary>
        /// Sets the value of TooltipDelay property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetMouseLeaveFlag(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridTooltipService.MouseLeaveFlagProperty, value);
        }
        #endregion

        /// <summary>
        /// Releases all the resources used by this component.
        /// </summary>
        /// <param name="d">The parent grid.</param>
        public static void Dispose(DependencyObject dop)
        {
            var timer = GridTooltipService.GetTimer(dop);
            if (timer != null)
            {
#if !WinRT
                timer.Tick -= OnTick;
#endif
                timer.Tag = null;
                timer = null;
            }
            var timer2 = GridTooltipService.GetErrorPopupTimer(dop);
            if (timer2 != null)
            {
#if !WinRT
                timer2.Tick -= OnTick;
#endif
                timer2.Tag = null;
                timer2 = null;
            }
            var timer3 = GridTooltipService.GetDataValidationTimer(dop);
            if (timer3 != null)
            {
#if !WinRT
                timer3.Tick -= OnTick;
#endif
                timer3.Tag = null;
                timer3 = null;
            }
            var d = FindGrid(dop);
            if (d != null)
            {
                var popuphost = GridTooltipService.GetPopupHost(d);
                if (popuphost != null)
                {
#if !WinRT
                    if (((System.Windows.Controls.Grid)popuphost.Child).Children.Count > 0)
                    {
                        var contentControl =
                            ((System.Windows.Controls.Grid)popuphost.Child).Children[0] as ContentControl;
                        if (contentControl != null)
                            contentControl.Content = null;
                    }
#endif
                    popuphost.Dispose();
                    popuphost = null;
                }
                popuphost = GridTooltipService.GetDataValidationPopupHost(d);
                if (popuphost != null)
                {
                    popuphost.Dispose();
                    popuphost = null;
                }
                popuphost = (PopupDragWindow)d.GetValue(GridTooltipService.ErrorPopupHostProperty);
                if (popuphost != null)
                {
#if !WinRT
                    if (((System.Windows.Controls.Grid)popuphost.Child).Children.Count > 0)
                    {
                        var contentControl =
                            ((System.Windows.Controls.Grid)popuphost.Child).Children[0] as ContentControl;
                        if (contentControl != null)
                            contentControl.Content = null;
                    }
#endif
                    popuphost.Dispose();
                    popuphost = null;
                }
                d.ClearValue(GridTooltipService.CachedErrorRowColIndexProperty);
                d.ClearValue(GridTooltipService.CachedRowColumnIndexProperty);
                d.ClearValue(GridTooltipService.ErrorPopupHostProperty);
                d.ClearValue(GridTooltipService.ErrorPopupTimerProperty);
                d.ClearValue(GridTooltipService.IsErrorTooltipShowingProperty);
                d.ClearValue(GridTooltipService.MouseLeaveFlagProperty);
                d.ClearValue(GridTooltipService.PopupHostProperty);
                d.ClearValue(GridTooltipService.ShowErrorTooltipsProperty);
                d.ClearValue(GridTooltipService.ShowTooltipsProperty);
                d.ClearValue(GridTooltipService.TimerProperty);
                d.ClearValue(GridTooltipService.TooltipDelayProperty);
                d.ClearValue(GridTooltipService.DataValidationPopupHostProperty);
                d.ClearValue(GridTooltipService.DataValidationTimerProperty);
                d.ClearValue(GridTooltipService.DataValidationCachedRowColumnIndexProperty);
                UnwireMouseMove(d);
                //GridTooltipService.SetPopupHost(d, null);
                //GridTooltipService.SetTimer(d, null);
                //GridTooltipService.SetErrorPopupHost(d, null);
                //GridTooltipService.SetErrorPopupTimer(d, null);     
            }
        }

        private static void UnwireMouseMove(DependencyObject d)
        {
            var grid = FindGrid(d);
            var frameworkElement = d as FrameworkElement;
            if (grid != null)
            {
#if (!SILVERLIGHT && !WinRT)
                grid.PreviewMouseMove -= new System.Windows.Input.MouseEventHandler(OnGridShowTooltipPreviewMouseMove);
                grid.PreviewMouseDown -= new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseDown);
                grid.SelectionChanged -= new GridSelectionChangedEventHandler(OnSelectionChanged);
                grid.MouseLeave -= new System.Windows.Input.MouseEventHandler(grid_MouseLeave);
#endif
#if SILVERLIGHT
                grid.MouseMove -= OnGridShowTooltipPreviewMouseMove;
                grid.MouseLeave -= new System.Windows.Input.MouseEventHandler(OnGridMouseLeave);
#endif
#if WinRT
                grid.PointerMoved -= grid_PointerMoved;
                grid.PointerExited -= grid_PointerExited;
#endif
                grid.CurrentCellActivated -= new ComponentModel.GridRoutedEventHandler(grid_CurrentCellActivated);
                frameworkElement.Loaded -= new RoutedEventHandler(frameworkElement_Loaded);
                frameworkElement.Unloaded -= new RoutedEventHandler(frameworkElement_Unloaded);
            }
        }

        private static void WireMouseMove(DependencyObject d)
        {
            var frameworkElement = d as FrameworkElement;
#if (!SILVERLIGHT && !WinRT)
            if (frameworkElement != null && !frameworkElement.IsLoaded)
#else
            if (frameworkElement != null)
#endif
            {
                frameworkElement.Loaded += new RoutedEventHandler(frameworkElement_Loaded);
                frameworkElement.Unloaded += new RoutedEventHandler(frameworkElement_Unloaded);
#if (SILVERLIGHT || WinRT)
                var grid = FindGrid(d);
                if (grid != null)
                {
                    InitializeGridForTooltip(grid);
                }
#endif
            }
#if (!SILVERLIGHT && !WinRT)
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
                var popup = GridTooltipService.GetPopupHost(grid);
                if (popup != null && popup.IsShowing)
                    popup.Hide();

                var DataPopup = GridTooltipService.GetDataValidationPopupHost(grid);
                if (DataPopup != null && DataPopup.IsShowing)
                    DataPopup.Hide();
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
                //GridTooltipService.SetTimer(grid, new DispatcherTimer());
                InitializeGridForTooltip(grid);
            }
        }

        private static void InitializeGridForTooltip(GridControlBase grid)
        {
            if (!GridTooltipService.HasPopupHost(grid) || !GridTooltipService.HasDataValidationPopupHost(grid))
            {
                var tpopupHost = GetInitializedDragWindow(grid);
                GridTooltipService.SetPopupHost(grid, tpopupHost);

                var validationpopupHost = GetInitializeDataValidationDragWindow(grid);
                GridTooltipService.SetDataValidationPopupHost(grid, validationpopupHost);
                
                grid.CurrentCellActivated += new ComponentModel.GridRoutedEventHandler(grid_CurrentCellActivated);
#if (!SILVERLIGHT && !WinRT)
                GridTooltipService.SetTimer(grid, new DispatcherTimer());
                GridTooltipService.SetDataValidationTimer(grid, new DispatcherTimer());
                grid.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(OnGridShowTooltipPreviewMouseMove);
                grid.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseDown);
                grid.SelectionChanged += new GridSelectionChangedEventHandler(OnSelectionChanged);
                grid.MouseLeave += new System.Windows.Input.MouseEventHandler(grid_MouseLeave);

#else
                GridTooltipService.SetTimer(grid, new DispatcherTimerExt());
                GridTooltipService.SetDataValidationTimer(grid, new DispatcherTimerExt());
#if SILVERLIGHT
                grid.MouseMove += OnGridShowTooltipPreviewMouseMove;
                grid.MouseLeave += new System.Windows.Input.MouseEventHandler(OnGridMouseLeave);
#endif
#if WinRT
                grid.PointerMoved += grid_PointerMoved;
                grid.PointerExited += grid_PointerExited;
#endif
#endif
            }
        }
#if !WinRT
        // This event is hooked foe the issue SD10328-In GDC, Tooltip showing when we changed to other window by ALT+TAB
        static void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            var grid = sender as GridControlBase;
            var tooltip = GridTooltipService.GetPopupHost(grid);
            if (tooltip != null)
            {
                if (tooltip.IsShowing)
                    tooltip.Hide();
            }

            var datatooltip = GridTooltipService.GetDataValidationPopupHost(grid);
            if (datatooltip != null)
            {
                if (datatooltip.IsShowing)
                    datatooltip.Hide();
            }
        }
#endif
        private static PopupDragWindow GetInitializeDataValidationDragWindow(FrameworkElement parent)
        {
#if WPF
            var popupDragWindow = new PopupDragWindow() { DestroyChildOnStopDrag = false, Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute, HorizontalOffset = 10d, VerticalOffset = 20d, PlacementTarget = parent, AllowsTransparency = true };
            var innerGrid = new System.Windows.Controls.Grid();
            innerGrid.Children.Add(new ContentControl());
            popupDragWindow.Child = innerGrid;
#else
            var popupDragWindow = new PopupDragWindow() { DestroyChildOnStopDrag = false, IsPopupUnderMousePoint = false };
#if !WinRT
            var innerGrid = new System.Windows.Controls.Grid();
#else
            var innerGrid = new Windows.UI.Xaml.Controls.Grid();
#endif
            innerGrid.Children.Add(new ContentControl());
            popupDragWindow.ProvideVisual(innerGrid);
#endif
            return popupDragWindow;
        }


#if (!SILVERLIGHT && !WinRT)
        private static void OnGridMouseLeave(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            GridTooltipService.SetMouseLeaveFlag(grid, true);

            var timer = GridTooltipService.GetErrorPopupTimer(grid);
            if (timer != null)
            {
                timer.Tick -= OnErrorTooltipTick;
            }
        }

        private static void OnSelectionChanged(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            GridTooltipService.SetSelectionChangedFlag(grid, true);
        }

        private static void OnPreviewMouseDown(object sender, EventArgs e)
        {
            var grid = UpdatePopUpHost(sender);
            GridTooltipService.SetMouseDownFlag(grid, true);
        }

#endif

#if SILVERLIGHT 
        private static void OnGridMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var grid = sender as GridControlBase;
            var popupHost = GridTooltipService.GetPopupHost(grid);
            if (popupHost != null && popupHost.IsDragging)
            {
                popupHost.StopDrag();
                popupHost.Hide();
            }
            GridTooltipService.SetMouseLeaveFlag(grid, true);
        }
#endif
#if (!SILVERLIGHT && !WinRT)
        private static GridControlBase UpdatePopUpHost(object sender)
        {
            var grid = sender as GridControlBase;
            var popupHost = GridTooltipService.GetPopupHost(grid);
            if (popupHost != null && popupHost.IsDragging)
            {
                popupHost.StopDrag();
                popupHost.IsOpen = false;
            }
            return grid;
        }
#endif

#if WinRT
        static void grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var grid = sender as GridControlBase;
            var popupHost = GridTooltipService.GetPopupHost(grid);
            if (popupHost != null && popupHost.IsDragging)
            {
                popupHost.StopDrag();
                popupHost.Hide();
            }
            GridTooltipService.SetMouseLeaveFlag(grid, true);
        }

#endif

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
#if (SILVERLIGHT || WinRT)
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
#if DEBUG
            System.Diagnostics.Debug.WriteLine("CurrentCellActivated");
#endif

            var popupHost = GridTooltipService.GetDataValidationPopupHost(grid);

            if (popupHost == null)
            {
                throw new InvalidOperationException("Could not find Popup host container");
            }
            var timer = GridTooltipService.GetDataValidationTimer(grid);
            var cell = grid.CurrentCell.CellRowColumnIndex;
            var cachedRowColIndex = GridTooltipService.GetDataValidationCachedRowColIndex(grid);
            if (cell.IsEmpty)
            {
                cachedRowColIndex = cell;
                CloseDataValidationTooltip(popupHost, timer);
                return;
            }
            GridRenderStyleInfo style = grid.GetRenderStyleInfo(cell);
            RowColumnIndex RowColIndex = new RowColumnIndex();
            if (grid.CurrentCell.HasCurrentCell)
                RowColIndex = grid.CurrentCell.CellRowColumnIndex;
            if (!RowColIndex.IsEmpty && style.ShowDataValidationTooltip && style.DataValidationTooltip != string.Empty)
            {
                Point gridPosition = GetGridOffset(grid);
                Point tooltipPosition = new Point();
                if (style.HasDataValidationTooltipLocation && style.DataValidationTooltipLocation != null)
                {
                    // Point cellPosition = GetCellOffset(grid, gridPosition, RowColIndex); Unused local variable
                    tooltipPosition.X = style.DataValidationTooltipLocation.X;
                    tooltipPosition.Y = style.DataValidationTooltipLocation.Y;
                }
                else
                {
                    tooltipPosition = GetTooltipPosition(grid, gridPosition, RowColIndex);
                }
#if SILVERLIGHT
                popupHost.popupWindow.HorizontalOffset = tooltipPosition.X;
                popupHost.popupWindow.VerticalOffset = tooltipPosition.Y;
#else
                popupHost.HorizontalOffset = tooltipPosition.X;
                popupHost.VerticalOffset = tooltipPosition.Y;
#endif
                displayPosition = tooltipPosition;
            }

            if (style.ShowDataValidationTooltip && style.DataValidationTooltip != string.Empty && style.DataValidationTooltip != " : ")
            {

#if !WinRT
                var contentControl = ((System.Windows.Controls.Grid)popupHost.Child).Children[0] as ContentControl;
#else
                var contentControl = ((Windows.UI.Xaml.Controls.Grid)popupHost.Child).Children[0] as ContentControl;
#endif
                if (cachedRowColIndex != cell)
                {
                    CloseDataValidationTooltip(popupHost, timer);
                    cachedRowColIndex = cell;
                    GridTooltipService.SetDataValidationCachedRowColIndex(grid, cachedRowColIndex);
                    contentControl.Content = style;
                    DataTemplate dt = null;

                    if (style.HasDataValidationTooltipTemplateKey && style.DataValidationTooltipTemplateKey != null)
                    {
                        dt = (DataTemplate)style.GridControl.TryFindResource(style.DataValidationTooltipTemplateKey);
                    }
                    else
                    {
                        dt = style.GridControl.GetDataValidationTemplate();
                    }
                    if (dt != null)
                    {
                        contentControl.ContentTemplate = dt;
                        if (timer != null)
                        {
#if !WinRT
                            timer.Tick += OnDataValidationTick;
#else
                            timer.Tick += OnDataValidationTick;
#endif
                            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                            timer.Start();
                            timer.Tag = new Dictionary<string, object>() { { "CachedRowColIndex", cachedRowColIndex }, { "PopupHost", popupHost }, { "Grid", grid } };
                        }
                    }
                }
            }
            else
            {
                GridTooltipService.SetDataValidationCachedRowColIndex(grid, RowColumnIndex.Empty);
                CloseDataValidationTooltip(popupHost, timer);
            }
        }
#if WinRT
        static void OnDataValidationTick(object sender, object e)
        {
#if (SILVERLIGHT || WinRT)
            var timer = sender as DispatcherTimerExt;
#else
            var timer = sender as DispatcherTimer;
#endif
            var dict = timer.Tag as Dictionary<string, object>;
            var prevRowColIndex = (RowColumnIndex)dict["CachedRowColIndex"]; //(RowColumnIndex)timer.Tag;
            var grid = (GridControlBase)dict["Grid"];
            var cachedRowColIndex = GridTooltipService.GetDataValidationCachedRowColIndex(grid);
            var mouseLeaveFlag = GridTooltipService.GetMouseLeaveFlag(grid);
            if (mouseLeaveFlag == true)
            {
                GridTooltipService.SetMouseLeaveFlag(grid, false);
                timer.Tick -= timer_Tick;
                return;
            }
            if (prevRowColIndex != null && prevRowColIndex == cachedRowColIndex)
            {
                var tpopupHost = (PopupDragWindow)dict["PopupHost"];
                tpopupHost.StartDrag();
                tpopupHost.Show();
            }
            timer.Stop();
            timer.Tick -= timer_Tick;
        }
#endif

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
#if (SILVERLIGHT || WinRT)
            var timer = sender as DispatcherTimerExt;
#else
            var timer = sender as DispatcherTimer;
#endif
            var dict = timer.Tag as Dictionary<string, object>;
            var prevRowColIndex = (RowColumnIndex)dict["CachedRowColIndex"]; //(RowColumnIndex)timer.Tag;
            var grid = (GridControlBase)dict["Grid"];
            var cachedRowColIndex = GridTooltipService.GetDataValidationCachedRowColIndex(grid);
            var mouseLeaveFlag = GridTooltipService.GetMouseLeaveFlag(grid);
            if (mouseLeaveFlag == true)
            {
                GridTooltipService.SetMouseLeaveFlag(grid, false);
                timer.Tick -= OnDataValidationTick;
                return;
            }
            if (prevRowColIndex != null && prevRowColIndex == cachedRowColIndex)
            {
                var tpopupHost = (PopupDragWindow)dict["PopupHost"];
                tpopupHost.StartDrag();
                tpopupHost.Show();
            }
            timer.Stop();
            timer.Tick -= OnDataValidationTick;
        }

        private static GridRenderStyleInfo GetStyleInfo(GridControlBase gcb, RowColumnIndex cell)
        {
            GridRenderStyleInfo style = gcb.GetRenderStyleInfo(cell);
            CoveredCellInfo cc = gcb.Model.CoveredCells.GetCoveredCell(cell.RowIndex, cell.ColumnIndex);
            if (cc != null)
            {
                for (int r = cc.Top; r <= cc.Bottom; r++)
                {
                    for (int c = cc.Left; c <= cc.Right; c++)
                    {
                        var rowcol = new RowColumnIndex(r, c);
                        GridRenderStyleInfo styl = gcb.GetRenderStyleInfo(rowcol);
                        if (styl.ShowTooltip)
                            return styl;
                    }
                }
            }
            return style;
        }
#if WinRT
        static void grid_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
#else
        private static void OnGridShowTooltipPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
#endif
            var grid = sender as GridControlBase;
            var popupHost = GridTooltipService.GetPopupHost(grid);
            GridTooltipService.SetMouseLeaveFlag(grid, false);
            GridTooltipService.SetMouseDownFlag(grid, false);
            GridTooltipService.SetSelectionChangedFlag(grid, false);

#if(!SILVERLIGHT &&  !WinRT)         
            if (e.LeftButton == MouseButtonState.Released)
                grid.ReleaseMouseCapture();
#endif
            if (popupHost == null)
            {
                throw new InvalidOperationException("Could not find Popup host container");
            }
            var timer = GridTooltipService.GetTimer(grid);
#if !WinRT
            var cell = grid.PointToCellRowColumnIndexOutsideCells(e.GetPosition(grid), false);
#else
            var cell = grid.PointToCellRowColumnIndexOutsideCells(e.GetCurrentPoint(grid).Position, false);
#endif
            var cachedRowColIndex = GridTooltipService.GetCachedRowColIndex(grid);
            if (cell.IsEmpty)
            {
                cachedRowColIndex = cell;
                Close(popupHost, timer);
                return;
            }

            GridRenderStyleInfo style = GetStyleInfo(grid, cell);
            var canShowTooltips = true;
            //style.ShowTooltip = true;
            //if AllowKeepAliveOnlyCurrentCell is false, then the UIElement will be intialized on PreviewMouseMove internally in the grid itself, we simply check it thru the interface
            //var renderer = style.CellRenderer as IAllowKeepAliveOnlyCurrentCell;

            //if (renderer != null && !renderer.AllowKeepAliveOnlyCurrentCell)
            //{
            //    canShowTooltips = false;
            //}           
            //var renderer = style.CellRenderer as GridCellRendererBase; Unused local variable
            if (grid.CurrentCell != null && grid.CurrentCell.HasCurrentCellAt(cell) && grid.CurrentCell.IsEditing)//|| !(style.CellRenderer as GridCellRendererBase).SupportsRenderOptimization)
            {
                canShowTooltips = false;
            }
            // To close the parent Tooltip in nested grid.
            if (style.CellType == "NestedGrid")
            {
                GridTooltipService.SetCachedRowColIndex(grid, RowColumnIndex.Empty);
                Close(popupHost, timer);
                return;
            }
            //Closing the tooltip in Error icon area.
            bool firstVisible, lastVisible;
            VisibleLineInfo ColumnfirstLine, ColumnlastLine, RowfirstLine, RowlastLine;
            grid.ScrollColumns.GetLinesAndVisibility(cell.ColumnIndex, cell.ColumnIndex, false, out firstVisible, out lastVisible, out ColumnfirstLine, out ColumnlastLine);
            grid.ScrollRows.GetLinesAndVisibility(cell.RowIndex, cell.RowIndex, false, out firstVisible, out lastVisible, out RowfirstLine, out RowlastLine);
            Rect rect = grid.RangeToRect(RowlastLine.Region, ColumnlastLine.Region, GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex), false, true);
            Rect imgRect = Rect.Empty;
            var width = style.ErrorInfo.GetImageWidth() * rect.Width;
            var height = style.ErrorInfo.GetImageHeight() * rect.Height;
            var errorContentAlignment = style.ErrorInfo.ErrorContentAlignment;
            if (errorContentAlignment == ImageContentAlignment.Left)
            {
                imgRect = new Rect(rect.X, rect.Y, width, height);
            }
            else if (errorContentAlignment == ImageContentAlignment.Right)
            {
                imgRect = new Rect(rect.X + (rect.Width - width), rect.Y, width, rect.Height);
            }
#if !WinRT
            var pt = e.GetPosition(grid);
#else
            var pt = e.GetCurrentPoint(grid).Position;
#endif
            if (imgRect.Contains(pt))
            {
                if (style.HasErrorInfo)
                {
                    GridTooltipService.SetCachedRowColIndex(grid, RowColumnIndex.Empty);
                    Close(popupHost, timer);
                    return;
                }
            }
            if (canShowTooltips && style.ShowTooltip) // && style.CellValue != null && style.CellValue != string.Empty)
            {
                var mouseLeaveFlag = GridTooltipService.GetMouseLeaveFlag(grid);
                if (mouseLeaveFlag == true)
                {
                    GridTooltipService.SetMouseLeaveFlag(grid, false);
                }
#if !WinRT
                var contentControl = ((System.Windows.Controls.Grid)popupHost.Child).Children[0] as ContentControl;
#else
                var contentControl = ((Windows.UI.Xaml.Controls.Grid)popupHost.Child).Children[0] as ContentControl;
#endif
                if (cachedRowColIndex != cell || (cachedRowColIndex == cell && !popupHost.IsShowing))
                {
                    Close(popupHost, timer);
                    cachedRowColIndex = cell;
                    GridTooltipService.SetCachedRowColIndex(grid, cachedRowColIndex);
                    contentControl.Content = style;
                    DataTemplate dt = null;

                    if (style.TooltipTemplateKey != null)
                    {
                        dt = (DataTemplate)style.GridControl.TryFindResource(style.TooltipTemplateKey);
#if (!SILVERLIGHT &&  !WinRT)
                        if (dt == null)
                            dt = (DataTemplate)Application.Current.TryFindResource(style.TooltipTemplateKey);
#endif
                    }
#if (!WinRT)
#if (!SILVERLIGHT)
                    else if (style.TooltipTemplate != null)
                    {
                        dt = style.TooltipTemplate;
                    }
#endif
                    else if (style.ToolTip != null && style.ToolTip.ToString() != string.Empty)
                    {
                        dt = style.GridControl.GetCustomTooltipTemplate();
                    }
#endif
                    else
                    {
                        if (style.CellValue != null && style.CellValue.ToString() != string.Empty)
                            dt = style.GridControl.GetDefaultTooltipTemplate();
                    }

                    if (dt != null)
                    {
                        contentControl.ContentTemplate = dt;
                        if (timer != null)
                        {
#if !WinRT
                            timer.Tick += OnTick;
#else
                            timer.Tick += timer_Tick;
#endif
                            var tooltipDelay = GridTooltipService.GetTooltipDelay(grid);
                            timer.Interval = new TimeSpan(0, 0, 0, 0, tooltipDelay.HasValue ? tooltipDelay.Value : 500);
#if (!SILVERLIGHT &&  !WinRT)
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
                GridTooltipService.SetCachedRowColIndex(grid, RowColumnIndex.Empty);
                Close(popupHost, timer);
            }

            var RowColIndex = GridTooltipService.GetDataValidationCachedRowColIndex(grid);
            if (RowColIndex != null && !RowColIndex.IsEmpty)
            {
                Point gridPosition = GetGridOffset(grid);
                Point tooltipPosition = new Point();
                if (style.HasDataValidationTooltipLocation && style.DataValidationTooltipLocation != null)
                {
                    // Point cellPosition = GetCellOffset(grid, gridPosition, RowColIndex); Unused local variable
                    tooltipPosition.X = style.DataValidationTooltipLocation.X;
                    tooltipPosition.Y = style.DataValidationTooltipLocation.Y;
                }
                else
                {
                    tooltipPosition = GetTooltipPosition(grid, gridPosition, RowColIndex);
                }

                if (!tooltipPosition.Equals(displayPosition))
                {
                    var datavalidation = GetDataValidationPopupHost(grid);
                    var validationtimer = GetDataValidationTimer(grid);
                    if (datavalidation != null && validationtimer != null)
                        CloseDataValidationTooltip(datavalidation, validationtimer);
                }
            }
#if SILVERLIGHT
            UIElement applicationRoot = Application.Current.RootVisual;
            var p = e.GetPosition(applicationRoot);
            if (style.CellValue != null)
            {
                var tooltipwidth = grid.Model.ActiveGridView.MeasureText(style.CellValue.ToString(), style).Width + 5;
                var popUpXEnd = p.X + popupHost.HorizontalOffset + tooltipwidth;
                if (popUpXEnd > applicationRoot.RenderSize.Width)
                    popupHost.popupWindow.HorizontalOffset = p.X - tooltipwidth - popupHost.HorizontalOffset;
                else
                    popupHost.popupWindow.HorizontalOffset = p.X + popupHost.HorizontalOffset;

                var tooltipheight = grid.Model.ActiveGridView.MeasureText(style.CellValue.ToString(), style).Height + 5;
                var PopUpYEnd = p.Y + popupHost.VerticalOffset + tooltipheight;
                if (PopUpYEnd > applicationRoot.RenderSize.Height)
                    popupHost.popupWindow.VerticalOffset = p.Y - tooltipheight - popupHost.VerticalOffset;
                else
                    popupHost.popupWindow.VerticalOffset = p.Y + popupHost.VerticalOffset;
           }  
#endif
        }

#if (!SILVERLIGHT &&  !WinRT)
        private static void Close(PopupDragWindow popupHost, DispatcherTimer timer)
#else
        private static void Close(PopupDragWindow popupHost, DispatcherTimerExt timer)
#endif
        {
            if (timer.IsEnabled)
            {
#if !WinRT
                timer.Tick -= OnTick;
#else
                timer.Tick-=timer_Tick;
#endif
                timer.Stop();
#if (!SILVERLIGHT &&  !WinRT)
                timer.IsEnabled = false;
#endif
            }
            if (popupHost.IsDragging)
            {
                popupHost.StopDrag();
                popupHost.Hide();
            }
        }
#if WinRT
        static void timer_Tick(object sender, object e)
#else
        //private static PopupDragWindow tpopupHost;
        private static void OnTick(object sender, EventArgs e)
#endif
        {
#if (!SILVERLIGHT &&  !WinRT)
            var timer = sender as DispatcherTimer;
#else
            var timer = sender as DispatcherTimerExt;
#endif
            var dict = timer.Tag as Dictionary<string, object>;
            var prevRowColIndex = (RowColumnIndex)dict["CachedRowColIndex"]; //(RowColumnIndex)timer.Tag;
            var grid = (GridControlBase)dict["Grid"];
            var cachedRowColIndex = GridTooltipService.GetCachedRowColIndex(grid);
            var mouseLeaveFlag = GridTooltipService.GetMouseLeaveFlag(grid);
            if (mouseLeaveFlag == true)
            {
                GridTooltipService.SetMouseLeaveFlag(grid, false);
#if !WinRT
                timer.Tick -= OnTick;
#else
                timer.Tick -= timer_Tick;
#endif
                return;
            }
#if (!SILVERLIGHT &&  !WinRT)
            var mouseDownFlag = GridTooltipService.GetMouseDownFlag(grid);
            if (mouseDownFlag == true)
            {
                GridTooltipService.SetMouseDownFlag(grid, false);
                timer.Tick -= OnTick;
                return;
            }

            var selectionChangedFlag = GridTooltipService.GetSelectionChangedFlag(grid);
            if (selectionChangedFlag == true)
            {
                GridTooltipService.SetSelectionChangedFlag(grid, false);
                timer.Tick -= OnTick;
                return;
            }
#endif
            if (prevRowColIndex != null && prevRowColIndex == cachedRowColIndex)
            {
                var tpopupHost = (PopupDragWindow)dict["PopupHost"];
                tpopupHost.StartDrag();
#if (!SILVERLIGHT &&  !WinRT)

                var canShowTooltip = Mouse.Captured as FrameworkElement == null || !((Mouse.Captured as FrameworkElement).Parent is System.Windows.Controls.Primitives.Popup || 
                         (Mouse.Captured as FrameworkElement).TemplatedParent is GridDataFilteringPane || (Mouse.Captured as FrameworkElement).TemplatedParent is GridDataColumnOptionsPane);
                if (grid.IsMouseOver && canShowTooltip)
                {
                    
                    //Raising the CellTooltip opening method before the popup gets open.
                    grid.RaiseCellToolTipOpening(cachedRowColIndex, ((System.Windows.Controls.Grid)tpopupHost.Child).Children[0] as ContentControl);
#endif
                    tpopupHost.Show();
#if (!SILVERLIGHT &&  !WinRT)

                    //SD17475 - To avoid the blinking of tooltip when it shown at the bottom right most corner of the screen
                    Mouse.Capture(grid, CaptureMode.Element);
                }
#endif
            }
            timer.Stop();
#if !WinRT
            timer.Tick -= OnTick;
#else
            timer.Tick -= timer_Tick;
#endif
        }
    
        private static readonly DependencyProperty CachedErrorRowColIndexProperty = DependencyProperty.RegisterAttached(
            "CachedErrorRowColIndex",
            typeof(RowColumnIndex),
            typeof(GridTooltipService)
#if !WinRT
, null
#else
            , new PropertyMetadata(null)
#endif
);

        private static RowColumnIndex GetCachedErrorRowColIndex(DependencyObject dpo)
        {
            return (RowColumnIndex)dpo.GetValue(GridTooltipService.CachedErrorRowColIndexProperty);
        }

        private static void SetCachedErrorRowColIndex(DependencyObject dpo, RowColumnIndex value)
        {
            dpo.SetValue(GridTooltipService.CachedErrorRowColIndexProperty, value);
        }

        private static readonly DependencyProperty ShowErrorTooltipsProperty = DependencyProperty.RegisterAttached(
        "ShowErrorTooltips",
        typeof(bool),
        typeof(GridTooltipService),
#if !WinRT
        new PropertyMetadata(OnShowErrorTooltipsChanged));
#else
        new PropertyMetadata(null,OnShowErrorTooltipsChanged));
#endif

        /// <summary>
        /// Gets the value of ShowErrorTooltips property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <returns>True if the operation is successful; false otherwise.</returns>
        public static bool GetShowErrorTooltips(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridTooltipService.ShowErrorTooltipsProperty);
        }

        /// <summary>
        /// Sets the value of ShowErrorTooltips property.
        /// </summary>
        /// <param name="dpo">The grid.</param>
        /// <param name="value">The new value.</param>
        public static void SetShowErrorTooltips(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridTooltipService.ShowErrorTooltipsProperty, value);
        }

        private static readonly DependencyProperty IsErrorTooltipShowingProperty = DependencyProperty.RegisterAttached(
            "IsErrorTooltipShowing",
            typeof(bool),
            typeof(GridTooltipService)
#if !WinRT
, null
#else
            ,new PropertyMetadata(null)
#endif
);
        private static bool GetIsErrorTooltipShowing(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridTooltipService.IsErrorTooltipShowingProperty);
        }

        private static void SetIsErrorTooltipShowing(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridTooltipService.IsErrorTooltipShowingProperty, value);
        }

        private static readonly DependencyProperty ErrorPopupHostProperty = DependencyProperty.RegisterAttached(
            "ErrorPopupHost",
            typeof(PopupDragWindow),
            typeof(GridTooltipService)
#if !WinRT
, null
#else
            ,new PropertyMetadata(null)
#endif
);

        private static PopupDragWindow GetErrorPopupHost(DependencyObject dpo)
        {
            return (PopupDragWindow)dpo.GetValue(GridTooltipService.ErrorPopupHostProperty);
        }

        private static void SetErrorPopupHost(DependencyObject dpo, PopupDragWindow value)
        {
            dpo.SetValue(GridTooltipService.ErrorPopupHostProperty, value);
        }

#if (!SILVERLIGHT &&  !WinRT)
        private static readonly DependencyProperty ErrorPopupTimerProperty = DependencyProperty.RegisterAttached(
            "ErrorPopupTimer",
            typeof(DispatcherTimer),
            typeof(GridTooltipService)
#if WinRT
            ,new PropertyMetadata(null)
#else
#endif
            );

        private static DispatcherTimer GetErrorPopupTimer(DependencyObject dpo)
        {
            return (DispatcherTimer)dpo.GetValue(GridTooltipService.ErrorPopupTimerProperty);
        }

        private static void SetErrorPopupTimer(DependencyObject dpo, DispatcherTimer timer)
        {
            dpo.SetValue(GridTooltipService.ErrorPopupTimerProperty, timer);
        }
#else
        private static readonly DependencyProperty ErrorPopupTimerProperty = DependencyProperty.RegisterAttached(
         "ErrorPopupTimer",
         typeof(DispatcherTimerExt),
         typeof(GridTooltipService), null);

        private static DispatcherTimerExt GetErrorPopupTimer(DependencyObject dpo)
        {
            return (DispatcherTimerExt)dpo.GetValue(GridTooltipService.ErrorPopupTimerProperty);
        }

        private static void SetErrorPopupTimer(DependencyObject dpo, DispatcherTimerExt timer)
        {
            dpo.SetValue(GridTooltipService.ErrorPopupTimerProperty, timer);
        }
#endif

        private static void OnShowErrorTooltipsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var value = (bool)args.NewValue;
            if (value)
            {
                WireErrorTooltipMouseMove(d);
            }
            else
            {

            }
        }

        private static void WireErrorTooltipMouseMove(DependencyObject d)
        {
            var frameworkElement = d as FrameworkElement;
            if (frameworkElement != null
#if (!SILVERLIGHT &&  !WinRT)
 && !frameworkElement.IsLoaded
#endif
)
            {
                frameworkElement.Loaded += new RoutedEventHandler(OnErrorTooltipsWiredInLoaded);
            }
            else
            {
                var grid = FindGrid(d);
                if (grid != null)
                {
                    var errPopupHost = GetInitializedDragWindow(grid);
                    GridTooltipService.SetErrorPopupHost(d, errPopupHost);
#if (!SILVERLIGHT &&  !WinRT)
                    GridTooltipService.SetErrorPopupTimer(d, new DispatcherTimer());

                    grid.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(OnErrorTooltipPreviewMouseMove);
                    grid.MouseLeave += new System.Windows.Input.MouseEventHandler(OnErrorTooltipMouseLeave);
#endif
#if SILVERLIGHT
                    grid.MouseMove += new System.Windows.Input.MouseEventHandler(OnErrorTooltipPreviewMouseMove);
#endif

#if WinRT
                    grid.PointerMoved -= grid_PointerMoved;
                    grid.PointerExited -= grid_PointerExited;
#endif
                }
            }
        }
#if (!SILVERLIGHT &&  !WinRT)
        static void OnErrorTooltipMouseLeave(object sender, MouseEventArgs e)
        {
            var grid = sender as GridControlBase;
            var popupHost = GridTooltipService.GetErrorPopupHost(grid);
            if (popupHost != null && popupHost.IsDragging)
            {
                popupHost.StopDrag();
                popupHost.IsOpen = false;
            }

            var timer = GridTooltipService.GetErrorPopupTimer(grid);
            if (timer != null)
            {
                timer.Tick -= OnErrorTooltipTick;
            }

            GridTooltipService.SetMouseLeaveFlag(grid, true);
        }
#endif
        private static void OnErrorTooltipsWiredInLoaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            frameworkElement.Loaded -= new RoutedEventHandler(OnErrorTooltipsWiredInLoaded);
            var grid = FindGrid(frameworkElement);
            if (grid != null)
            {
                var errPopupHost = GetInitializedDragWindow(grid);
                GridTooltipService.SetErrorPopupHost(frameworkElement, errPopupHost);
#if (!SILVERLIGHT &&  !WinRT)
                GridTooltipService.SetErrorPopupTimer(frameworkElement, new DispatcherTimer());
                grid.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(OnErrorTooltipPreviewMouseMove);
                grid.MouseLeave += new System.Windows.Input.MouseEventHandler(OnErrorTooltipMouseLeave);
#else
                GridTooltipService.SetErrorPopupTimer(frameworkElement, new DispatcherTimerExt());
#endif
#if SILVERLIGHT
                grid.MouseMove += new System.Windows.Input.MouseEventHandler(OnErrorTooltipPreviewMouseMove);
#endif
#if WinRT
                grid.PointerMoved -= grid_PointerMoved;
                grid.PointerExited -= grid_PointerExited;
#endif
            }
        }

        //private static RowColumnIndex cachedErrorRowColIndex = RowColumnIndex.Empty;
        private static void OnErrorTooltipPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var grid = (GridControlBase)sender;
            var popupHost = GridTooltipService.GetErrorPopupHost(grid);
            if (popupHost == null)
            {
                throw new InvalidOperationException("Could not find Popup host container");
            }
            
#if !WinRT
            var timer = GridTooltipService.GetErrorPopupTimer(grid);
            var cell = grid.PointToCellRowColumnIndexOutsideCells(e.GetPosition(grid), false);
#else
            DispatcherTimerExt timer = GridTooltipService.GetErrorPopupTimer(grid) as DispatcherTimerExt;
            var cell = grid.PointToCellRowColumnIndexOutsideCells(new Point(e.MouseDelta.X,e.MouseDelta.Y), false);
#endif
            var cachedErrorRowColIndex = GridTooltipService.GetCachedErrorRowColIndex(grid);
            if (cell.IsEmpty)
            {
                cachedErrorRowColIndex = cell;
                CloseErrorPopup(popupHost, timer);
                GridTooltipService.SetCachedErrorRowColIndex(grid, RowColumnIndex.Empty);
                return;
            }
            if (grid.RenderStyles == null)
                return;
            GridRenderStyleInfo style = grid.GetRenderStyleInfo(cell);
            var canShowTooltips = style.HasErrorInfo && style.ErrorInfo.HasErrorMessage;
            if (canShowTooltips)
            {
                var rect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex), false, false);
                Rect imgRect = Rect.Empty;
                var width = style.ErrorInfo.GetImageWidth() * rect.Width;
                var height = style.ErrorInfo.GetImageHeight() * rect.Height;
                var errorContentAlignment = style.ErrorInfo.ErrorContentAlignment;
                if (errorContentAlignment == ImageContentAlignment.Left)
                {
                    imgRect = new Rect(rect.X, rect.Y, width, height);
                }
                else if (errorContentAlignment == ImageContentAlignment.Right)
                {
                    imgRect = new Rect(rect.X + (rect.Width - width), rect.Y, width, rect.Height);
                }
#if !WinRT
                var pt = e.GetPosition(grid);
#else
                var pt = new Point(e.MouseDelta.X, e.MouseDelta.Y);
#endif
                if (imgRect.Contains(pt))
                {
                    //ensure that if mouseLeaveFlag is set, when mouse again hovers back into the grid, reset the flag
                    var mouseLeaveFlag = GridTooltipService.GetMouseLeaveFlag(grid);
                    if (mouseLeaveFlag == true)
                    {
                        GridTooltipService.SetMouseLeaveFlag(grid, false);

                    }
#if !WinRT
                    var contentControl = ((System.Windows.Controls.Grid)popupHost.Child).Children[0] as ContentControl;
#else
                    var contentControl = ((Windows.UI.Xaml.Controls.Grid)popupHost.Child).Children[0] as ContentControl;
#endif
                    if (cachedErrorRowColIndex != cell)
                    {
                        cachedErrorRowColIndex = cell;
                        GridTooltipService.SetCachedErrorRowColIndex(grid, cachedErrorRowColIndex);
                        contentControl.Content = style;
                        DataTemplate dt = null;
                        if (style.ErrorInfo.ErrorTooltipTemplateKey != null)
                        {
                            dt = (DataTemplate)style.GridControl.TryFindResource(style.ErrorInfo.ErrorTooltipTemplateKey);
                        }
                        else
                        {
                            dt = grid.GetDefaultErrorTemplate();
                        }
                        if (dt != null)
                        {
                            contentControl.ContentTemplate = dt;
                            if (timer != null)
                            {
#if !WinRT
                                timer.Tick += OnErrorTooltipTick;
#else
                                timer.Tick +=OnErrorTooltipTick;
#endif
                                var tooltipDelay = GridTooltipService.GetTooltipDelay(grid);
                                timer.Interval = new TimeSpan(0, 0, 0, 0, tooltipDelay.HasValue ? tooltipDelay.Value : 500);
#if (!SILVERLIGHT &&  !WinRT)
                                timer.IsEnabled = true;
#endif

                                timer.Start();
#if !WinRT
                                timer.Tag = new Dictionary<string, object>() { { "CachedErrorRowColIndex", cachedErrorRowColIndex }, { "PopupHost", popupHost }, { "Grid", grid } };
#else
                                timer.Tag = new Dictionary<string, object>() { { "CachedErrorRowColIndex", cachedErrorRowColIndex }, { "PopupHost", popupHost }, { "Grid", grid } };
#endif
                            }
                        }
                    }
                }
                else
                {
                    CloseErrorPopup(popupHost, timer);
                    GridTooltipService.SetCachedErrorRowColIndex(grid, RowColumnIndex.Empty);
                }
            }
            else
            {
                CloseErrorPopup(popupHost, timer);
                GridTooltipService.SetCachedErrorRowColIndex(grid, RowColumnIndex.Empty);
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
        public const double ErrorMargin = 20d;
        public const double ErrorImageBounds = 15d;
        // private static PopupDragWindow errPopupHost; Variable is never used
#if WinRT
        static void OnErrorTooltipTick(object sender, object e)
        {
#else
        private static void OnErrorTooltipTick(object sender, EventArgs e)
        {
#endif
#if (!SILVERLIGHT &&  !WinRT)
            var timer = sender as DispatcherTimer;
#else
            var timer = sender as DispatcherTimerExt;
#endif
            var dict = timer.Tag as Dictionary<string, object>;
            var prevRowColIndex = (RowColumnIndex)dict["CachedErrorRowColIndex"];
            var grid = (GridControlBase)dict["Grid"];
            var mouseLeaveFlag = GridTooltipService.GetMouseLeaveFlag(grid);
            if (mouseLeaveFlag == true)
            {
                GridTooltipService.SetMouseLeaveFlag(grid, false);
                timer.Tick -= OnErrorTooltipTick;
                return;
            }
            var cachedErrorRowColIndex = GridTooltipService.GetCachedErrorRowColIndex(grid);
            if (prevRowColIndex != null && prevRowColIndex == cachedErrorRowColIndex)
            {
                var errPopupHost = dict["PopupHost"] as PopupDragWindow;
                errPopupHost.StartDrag();
#if (!SILVERLIGHT &&  !WinRT)
                if (grid.IsMouseOver)
#endif
                errPopupHost.Show();
            }
            //To Stop the timer to show the popup again and again
            timer.Stop();
            timer.Tick -= OnErrorTooltipTick;
        }
        private static void CloseErrorPopup(PopupDragWindow popupHost, DispatcherTimer timer)
        {
            if (timer.IsEnabled)
            {
                timer.Tick -= OnErrorTooltipTick;
                timer.Stop();
#if (!SILVERLIGHT &&  !WinRT)
                timer.IsEnabled = false;
#endif
            }
            if (popupHost.IsDragging)
            {
                popupHost.StopDrag();
                popupHost.Hide();
            }
        }


        public void Dispose()
        {

        }
    }

#if (SILVERLIGHT || WinRT)
    internal class DispatcherTimerExt : DispatcherTimer
    {
        public object Tag { get; set; }
    }
#endif

#if (!SILVERLIGHT &&  !WinRT)
    [ValueConversion(typeof(object), typeof(string))]
    public class TooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var text = ((System.Windows.Controls.ContentControl)(value)).Content;
                return text.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
#endif
}