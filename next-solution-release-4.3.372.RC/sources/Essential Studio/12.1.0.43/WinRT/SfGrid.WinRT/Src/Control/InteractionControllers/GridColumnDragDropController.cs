#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Linq;
using Syncfusion.Data.Extensions;
#if !WP
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
#endif

#if WinRT
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Syncfusion.Data;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Data;
using System.Threading;
using Pointer = System.Object;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif
#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using MouseEventArgs = PointerRoutedEventArgs;
    using MouseButtonEventArgs = PointerRoutedEventArgs;
    using Windows.UI.Input;
    using Windows.UI.Core;
    using System.Diagnostics;
#endif

    public class GridColumnDragDropController : IDisposable
    {
        #region Fields
        SfDataGrid dataGrid;
        #region DraggablePopup Fields
        private bool isPopupInitlized = false;
        DispatcherTimer dpTimer = new DispatcherTimer();
        private Popup upIndicator;
        private Popup downIndicator;
        bool allowScrollOnHorizontalRightLimits = false;
        bool allowScrollOnHorizontalLeftLimits = false;
        private Point previousIndicatorPosition;
        private int dropedColIndex = 0;
        private double _PopupMinWidth =
#if !WP
            75d
#else
            100d
#endif
            ;
        private double _PopupMaxWidth = 250d;
        private double _PopupMinHeight =
#if WinRT
                40d
#elif WP
                70d
#else
                20d
#endif
                ;
        private double _PopupMaxHeight =
#if WinRT
                60d
#elif WP
                85d
#else
                34d
#endif
                ;

#if !WPF
        private bool suspendReverseAnimationByColumnChooser;
#endif

#if WP
        internal PopupContentControl PopupContent;
        internal Popup DraggablePopup;
#else
        /// <summary>
        /// Gets PopupContentControl for Drag and Drop.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        protected PopupContentControl PopupContentControl
        {
            get { return PopupContent; }
        }
        private PopupContentControl PopupContent;
        internal Popup DraggablePopup;
#endif
        internal VisibleLineInfo DragLeftLine;
        internal VisibleLineInfo DragRightLine;
        internal Storyboard reverseAnimationStoryboard;
        private EasingDoubleKeyFrame horizontalDoubleAnimation;
        private EasingDoubleKeyFrame verticalDoubleAnimation;
        internal double PreviousLeftLineSize;
        internal double PreviousRightLineSize;
        private double mouseHorizontalPosition;
        private double mouseVerticalPosition;
        internal bool needToUpdatePosition;
        private bool isDragState;
        private bool isExpandedInternally;

        #endregion

        #endregion

        #region Ctor
        public GridColumnDragDropController(SfDataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
            dpTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            InitializePopup();
        }
        #endregion

        #region Draggable popup

        #region Public properties

        /// <summary>
        /// Gets or sets Minimum Width for Draggable Popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupMinWidth
        {
            get { return _PopupMinWidth; }
            set { _PopupMinWidth = value; }
        }

        /// <summary>
        /// Gets or sets Maximum Width for Draggable Popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupMaxWidth
        {
            get { return _PopupMaxWidth; }
            set { _PopupMaxWidth = value; }
        }

        /// <summary>
        /// Gets or sets Minimum Height for Draggable Popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupMinHeight
        {
            get { return _PopupMinHeight; }
            set { _PopupMinHeight = value; }
        }
        
        /// <summary>
        /// Gets or sets Maximum Height for Draggable Popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupMaxHeight
        {
            get { return _PopupMaxHeight; }
            set { _PopupMaxHeight = value; }
        }
        #endregion

        #region internal Methods

        internal void ShowPopup(GridColumn gridColumn, Rect rect, double adjustXPoint, double adjustYPoint, bool isInTouch,
                              object pointer)
        {
#if WP
            this.dataGrid.suspendZooming = true;
#endif
            ShowPopup(gridColumn, rect, isInTouch, adjustXPoint, adjustYPoint, false, false, pointer);
        }

        internal void ShowPopup(GridColumn gridColumn, Rect rect, bool isInTouch, double adjustXPoint, double adjustYPoint,
                              bool isDragFromGroupDropArea, bool isDragFromColumnChooser, object pointer)
        {
            
            if (!this.dataGrid.Validations.CheckForValidation(false))
                return;
            if (this.PopupContent != null)
            {
#if WinRT
                if (this.DraggablePopup.IsOpen || pointer == null)
                    return;
                this.PopupContent.CapturePointer(pointer as Pointer);
#endif
                if (this.DraggablePopup.IsOpen)
                    return;
                if (!isInTouch)
                {
                    var args = new QueryColumnDraggingEventArgs(this.dataGrid)
                    {
                        From = this.dataGrid.Columns.IndexOf(gridColumn),
                        To = -1,
                        PopupPosition = new Point(rect.X, rect.Y),
                        Reason = QueryColumnDraggingReason.DragStarting
                    };
                    if (this.dataGrid.RaiseQueryColumnDragging(args))
                    {
                        this.DraggablePopup.IsOpen = false;
                        return;
                    }
                }

                if (CanResizeColumn(gridColumn))
                {
                    int index = this.dataGrid.Columns.IndexOf(gridColumn);
                    index = this.dataGrid.ResolveToScrollColumnIndex(index);
                    this.DragRightLine = this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(index);
                    this.DragLeftLine =
                        this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(index - 1);
                    if (this.DragRightLine != null)
                        this.PreviousRightLineSize = this.DragRightLine.Size;
                    if (this.DragLeftLine != null)
                        this.PreviousLeftLineSize = this.DragLeftLine.Size;
                    if (isInTouch && !isDragFromGroupDropArea)
                    {
                        if (DragLeftLine != null &&
                            DragLeftLine.LineIndex < this.dataGrid.View.GroupDescriptions.Count || DragLeftLine == null)
                            this.PopupContent.LeftResizeThumbVisibility = Visibility.Collapsed;
                        else
                            this.PopupContent.LeftResizeThumbVisibility = Visibility.Visible;

                        if (DragRightLine != null &&
                            DragRightLine.LineIndex < this.dataGrid.View.GroupDescriptions.Count)
                            this.PopupContent.RightResizeThumbVisibility = Visibility.Collapsed;
                        else
                            this.PopupContent.RightResizeThumbVisibility = Visibility.Visible;
                    }
                    else
                    {
                        this.PopupContent.LeftResizeThumbVisibility = Visibility.Collapsed;
                        this.PopupContent.RightResizeThumbVisibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    this.PopupContent.LeftResizeThumbVisibility = Visibility.Collapsed;
                    this.PopupContent.RightResizeThumbVisibility = Visibility.Collapsed;
                }
                this.PopupContent.IsDragFromGroupDropArea = isDragFromGroupDropArea;
                this.PopupContent.IsDragFromColumnChooser = isDragFromColumnChooser;
                this.PopupContent.InitialRect = rect;
                mouseHorizontalPosition = adjustXPoint;
                mouseVerticalPosition = adjustYPoint;
                this.PopupContent.IsOpenInTouch = isInTouch;
                this.PopupContent.Content = CreatePopupContent(gridColumn);
                if (!double.IsNaN(gridColumn.MinimumWidth))
                    this.PopupContent.MinWidth = gridColumn.MinimumWidth;
                else
                    this.PopupContent.MinWidth = this.PopupMinWidth;
                if (!double.IsNaN(gridColumn.MaximumWidth))
                    this.PopupContent.MaxWidth = gridColumn.MaximumWidth + this.PopupContent.ThumbWidth;
                else
                    this.PopupContent.MaxWidth = this.PopupMaxWidth;
                this.PopupContent.MinHeight = PopupMinHeight;
                this.PopupContent.MaxHeight = PopupMaxHeight;
                this.PopupContent.Width = rect.Width + this.PopupContent.ThumbWidth;
                this.PopupContent.Height = rect.Height;
                this.PopupContent.Tag = gridColumn;
                var popupActualWidth = 0d;
                var popupActualHeight = 0d;
                if (PopupContent.Width > this.PopupContent.MaxWidth || this.PopupContent.Width < this.PopupContent.MinWidth)
                {
                    if (PopupContent.Width > PopupContent.MaxWidth)
                        popupActualWidth = PopupContent.MaxWidth;
                    if (PopupContent.Width < PopupContent.MinWidth)
                        popupActualWidth = PopupContent.MinWidth;
                }
                else
                    popupActualWidth = this.PopupContent.Width;

                if (PopupContent.Height > this.PopupContent.MaxHeight || this.PopupContent.Height < this.PopupContent.MinHeight)
                {
                    if (PopupContent.Height > PopupContent.MaxHeight)
                        popupActualHeight = PopupContent.MaxHeight;
                    if (PopupContent.Height < PopupContent.MinHeight)
                        popupActualHeight = PopupContent.MinHeight;
                }
                else
                    popupActualHeight = this.PopupContent.Height;
#if WPF
                var popupPositionRect = new Rect(rect.Left - this.PopupContent.ThumbWidth / 2, rect.Top, rect.Width,
                                                 rect.Height);
                if (popupActualWidth != PopupContent.Width && (rect.X + popupActualWidth) < (mouseHorizontalPosition + popupPositionRect.X))
                {
                    popupPositionRect.X = mouseHorizontalPosition + popupPositionRect.X - (popupActualWidth / 2);
                    popupPositionRect.Width = popupActualWidth;
                    mouseHorizontalPosition = popupActualWidth / 2;
                }

                if (popupActualHeight != PopupContent.Width && (rect.Y + popupActualHeight) < (mouseVerticalPosition + popupPositionRect.Y))
                {
                    popupPositionRect.Y = mouseVerticalPosition + popupPositionRect.Y - (popupActualHeight / 2);
                    popupPositionRect.Height = popupActualHeight;
                    mouseVerticalPosition = popupActualHeight / 2;
                }
                this.DraggablePopup.PlacementRectangle = popupPositionRect;
#else
                var horizontalOffset = rect.X - ((this.PopupContent.ThumbWidth / 2) + ((rect.X * 0.05) / 2));
                if (popupActualWidth != PopupContent.Width && (rect.X + popupActualWidth) < (mouseHorizontalPosition + horizontalOffset))
                {
                    horizontalOffset = mouseHorizontalPosition + horizontalOffset - (popupActualWidth / 2);
                    mouseHorizontalPosition = (popupActualWidth / 2);
                }
                var verticalOffset = rect.Y - ((rect.Height * 0.05)) / 2;
                if (popupActualHeight != PopupContent.Width && (rect.Y + popupActualHeight) < (mouseVerticalPosition + verticalOffset))
                {
                    verticalOffset = mouseVerticalPosition + verticalOffset - (popupActualHeight / 2);
                    mouseVerticalPosition = (popupActualHeight / 2);
                }
                this.DraggablePopup.HorizontalOffset = Math.Round(horizontalOffset);
                this.DraggablePopup.VerticalOffset = Math.Round(verticalOffset);
#endif

#if WP
                FrameworkElement popup = FrameworkElementExtensions.FindRootParent(this.dataGrid.GroupDropArea);
                if (popup != null)
                {
                    popup.AddHandler(Popup.MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown, true);
                }
#endif
                this.dataGrid.VisualContainer.SuspendManipulationScroll = true;
                if (!isDragFromGroupDropArea)
                    UpdateReverseAnimationOffset(this.DraggablePopup.HorizontalOffset,
                                                 this.DraggablePopup.VerticalOffset, false);
                this.PopupContent.ApplyState(true);
                needToUpdatePosition = false;
                this.DraggablePopup.Opacity = 1;
                if (!isInTouch)
                {
                    var args = new QueryColumnDraggingEventArgs(this.dataGrid);
                    args.From = dataGrid.Columns.IndexOf(gridColumn);
                    args.To = -1;
                    args.Reason = QueryColumnDraggingReason.DragStarted;
                    args.PopupPosition = new Point(rect.X, rect.Y);
                    if (this.dataGrid.RaiseQueryColumnDragging(args))
                        return;
                }
#if WPF
                // When creating two windows the second created window does not perform drag and drop if first window is closed.
                DraggablePopup.PlacementTarget = this.dataGrid;
#endif
                this.DraggablePopup.IsOpen = true;
                VisualStateManager.GoToState(this.PopupContent, "Valid", true);
#if !WinRT && !WP
#if WPF
                if (pointer != null)
                {
                    var touch = pointer as TouchDevice;
                    this.PopupContent.CaptureTouch(touch);
                }
                else
#endif
                    this.PopupContent.CaptureMouse();
#endif
            }
        }

        internal void UpdatePopupPosition()
        {
            if (needToUpdatePosition && this.DraggablePopup.IsOpen)
            {
                if (this.PopupContent.Tag != null && this.PopupContent.Tag is GridColumn)
                {
                    var gridcolumn = this.PopupContent.Tag as GridColumn;
                    var index = this.dataGrid.Columns.IndexOf(gridcolumn);
                    SetPopupPosition(this.dataGrid.ResolveToScrollColumnIndex(index));
                    needToUpdatePosition = false;
                }
            }
        }

        internal void UnWireEvents()
        {
            if (DraggablePopup != null)
                DraggablePopup.Closed -= OnDraggablePopupClosed;
            if (reverseAnimationStoryboard != null)
                reverseAnimationStoryboard.Completed -= OnReverseAnimationStoryboardCompleted;
        }

        internal void ColumnHiddenChanged(GridColumn column)
        {
            this.OnColumnHiddenChanged(column);
        }

        #endregion

        #region private methods

        /// <summary>
        /// Initialize the Draggable popup
        /// </summary>
        /// <remarks></remarks>
        private void InitializePopup()
        {
            if (isPopupInitlized)
                return;

            if (DraggablePopup == null)
                DraggablePopup = new Popup();

            if (upIndicator == null)
                upIndicator = new Popup();

            if (downIndicator == null)
                downIndicator = new Popup();
#if WinRT
            DraggablePopup.IsLightDismissEnabled = true;
            upIndicator.IsLightDismissEnabled = true;
            downIndicator.IsLightDismissEnabled = true;
#elif WPF
               DraggablePopup.Placement = PlacementMode.Absolute;
               DraggablePopup.StaysOpen = false;
               DraggablePopup.AllowsTransparency = true;
               upIndicator.Placement = PlacementMode.Absolute;
               upIndicator.StaysOpen = false;
               upIndicator.AllowsTransparency = true;
               downIndicator.Placement = PlacementMode.Absolute;
               downIndicator.StaysOpen = false;
               downIndicator.AllowsTransparency = true;  
#endif
            PopupContent = new PopupContentControl(this.dataGrid)
            {
                PopupContentPositionChanged = OnPopupContentPositionChanged,
                PopupContentDropped = OnPopupContentDropped,
                PopupContentResizing = this.dataGrid.GridColumnResizingController.OnPopupContentResizing,
                PopupContentResized = this.dataGrid.GridColumnResizingController.OnPopupContentResized
            };
            DraggablePopup.HorizontalAlignment = HorizontalAlignment.Center;
            DraggablePopup.VerticalAlignment = VerticalAlignment.Center;
            DraggablePopup.Child = PopupContent;
            CreateReverseAnimationStoryboard();
            DraggablePopup.Closed += OnDraggablePopupClosed;

            if (upIndicator.Child == null)
                upIndicator.Child = new UpIndicatorContentControl();

            if (downIndicator.Child == null)
                downIndicator.Child = new DownIndicatorContentControl();

            dpTimer.Tick += OnDispatcherTimerTick;

        }
        
#if WP
        private void OnMouseLeftButtonDown(Pointer sender, MouseButtonEventArgs e)
        {
            if (this.DraggablePopup == null || this.dataGrid == null)
                return;
            this.DraggablePopup.IsOpen = false;
            this.dataGrid.suspendZooming = false;
        }
#endif
        private void CreateReverseAnimationStoryboard()
        {
            var duration = new TimeSpan(0, 0, 0, 0, 200);
            horizontalDoubleAnimation = new EasingDoubleKeyFrame();
            verticalDoubleAnimation = new EasingDoubleKeyFrame();
            horizontalDoubleAnimation.KeyTime = KeyTime.FromTimeSpan(duration);
            verticalDoubleAnimation.KeyTime = KeyTime.FromTimeSpan(duration);
            horizontalDoubleAnimation.EasingFunction = new CircleEase();
            verticalDoubleAnimation.EasingFunction = new CircleEase();
            DoubleAnimationUsingKeyFrames horizontalAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            DoubleAnimationUsingKeyFrames verticalAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
#if WinRT
            horizontalAnimationUsingKeyFrames.EnableDependentAnimation = true;
            verticalAnimationUsingKeyFrames.EnableDependentAnimation = true;
#endif
            horizontalAnimationUsingKeyFrames.KeyFrames.Add(horizontalDoubleAnimation);
            verticalAnimationUsingKeyFrames.KeyFrames.Add(verticalDoubleAnimation);
            reverseAnimationStoryboard = new Storyboard { Duration = duration };
            reverseAnimationStoryboard.Children.Add(horizontalAnimationUsingKeyFrames);
            reverseAnimationStoryboard.Children.Add(verticalAnimationUsingKeyFrames);
            Storyboard.SetTarget(horizontalAnimationUsingKeyFrames, this.DraggablePopup);
            Storyboard.SetTarget(verticalAnimationUsingKeyFrames, this.DraggablePopup);
#if WinRT
            Storyboard.SetTargetProperty(horizontalAnimationUsingKeyFrames, "HorizontalOffset");
            Storyboard.SetTargetProperty(verticalAnimationUsingKeyFrames, "VerticalOffset");
#else
            Storyboard.SetTargetProperty(horizontalAnimationUsingKeyFrames, new PropertyPath("HorizontalOffset"));
            Storyboard.SetTargetProperty(verticalAnimationUsingKeyFrames, new PropertyPath("VerticalOffset"));
#endif
            reverseAnimationStoryboard.Completed += OnReverseAnimationStoryboardCompleted;
            this.DraggablePopup.Resources.Add("storyboard", reverseAnimationStoryboard);
            isPopupInitlized = true;
        }

        private void OnReverseAnimationStoryboardCompleted(object sender, object e)
        {
            if (this.PopupContent.IsOpenInTouch && !this.PopupContent.IsDragFromGroupDropArea)
            {
                this.PopupContent.ApplyState(true);
                var gridColumn = this.PopupContent.Tag as GridColumn;
                if (CanResizeColumn(gridColumn))
                {
                    if (DragLeftLine != null && DragLeftLine.LineIndex < this.dataGrid.View.GroupDescriptions.Count)
                        this.PopupContent.LeftResizeThumbVisibility = Visibility.Collapsed;
                    else
                        this.PopupContent.LeftResizeThumbVisibility = Visibility.Visible;

                    if (DragRightLine != null && DragRightLine.LineIndex < this.dataGrid.View.GroupDescriptions.Count)
                        this.PopupContent.RightResizeThumbVisibility = Visibility.Collapsed;
                    else
                        this.PopupContent.RightResizeThumbVisibility = Visibility.Visible;
                }
                this.PopupContent.IsOpenInTouch = false;
            }
            else
            {
                this.DraggablePopup.IsOpen = false;
#if WinRT
                this.dataGrid.Focus(FocusState.Programmatic);
#else
                this.dataGrid.Focus();
#endif
            }
            this.PopupContent.ResetMousePosition();
            this.dataGrid.VisualContainer.SuspendManipulationScroll = false;
            this.DraggablePopup.ClearValue(Popup.HorizontalOffsetProperty);
            this.DraggablePopup.ClearValue(Popup.VerticalOffsetProperty);
        }

        private void UpdateReverseAnimationOffset(double horizontalOffset, double verticalOffset, bool canIncrement)
        {
            if (canIncrement)
            {
                this.horizontalDoubleAnimation.Value += horizontalOffset;
                this.verticalDoubleAnimation.Value += verticalOffset;
            }
            else
            {
                this.horizontalDoubleAnimation.Value = horizontalOffset;
                this.verticalDoubleAnimation.Value = verticalOffset;
            }
        }

        /// <summary>
        /// Calls on Popup closed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void OnDraggablePopupClosed(object sender, object e)
        {
            isDragState = false;
            if (this.isExpandedInternally)
            {
                this.dataGrid.GroupDropArea.IsExpanded = false;
                this.isExpandedInternally = false;
            }
            this.PopupContent.ApplyState(false);
            this.PopupContent.ResetMousePosition();
            this.dataGrid.VisualContainer.SuspendManipulationScroll = false;
        }


#if !WP
        /// <summary>
        /// Calls on popup position changed
        /// </summary>
        /// <param name="HorizontalDelta"></param>
        /// <param name="VerticalDelta"></param>
        /// <param name="mousePoint"></param>
        /// <param name="mousePointOverGrid"></param>
        /// <remarks></remarks>
        protected virtual void OnPopupContentPositionChanged(double HorizontalDelta, double VerticalDelta, Point mousePoint, Point mousePointOverGrid)
#else
        internal void OnPopupContentPositionChanged(double HorizontalDelta, double VerticalDelta, Point mousePoint, Point mousePointOverGrid)
#endif
        {
#if WP && WP7
            this.dataGrid.GroupDropArea.scrollersDisabled = true;
#endif
            if (this.DraggablePopup.IsOpen)
            {
                if (!isDragState && HorizontalDelta != 0 && VerticalDelta != 0)
                {
                    this.PopupContent.ApplyState(false);
                    isDragState = true;
                    if (this.dataGrid.GroupDropArea != null && !this.dataGrid.GroupDropArea.IsExpanded)
                    {
                        this.dataGrid.GroupDropArea.IsExpanded = true;
                        this.isExpandedInternally = true;
                    }
                }
#if WPF
                this.DraggablePopup.PlacementRectangle = new Rect(HorizontalDelta - mouseHorizontalPosition,
                                                                  VerticalDelta - mouseVerticalPosition,
                                                                  this.PopupContent.ActualWidth,
                                                                  this.PopupContent.ActualHeight);
#else
                this.DraggablePopup.HorizontalOffset = HorizontalDelta - mouseHorizontalPosition;
                this.DraggablePopup.VerticalOffset = VerticalDelta - mouseVerticalPosition;
#endif

#if WinRT
                var gridRect = new Rect(this.dataGrid.TransformToVisual(null).TransformPoint(new Point(0, 0)), this.dataGrid.DesiredSize);
#elif WPF
                var gridRect = this.GetControlRect(this.dataGrid);
#elif WP
                var gridPoint = this.dataGrid.TransformToVisual(null).Transform(new Point(0, 0));
                var gridRect = new Rect(gridPoint, new Point(this.dataGrid.ActualWidth + gridPoint.X, this.dataGrid.ActualHeight + gridPoint.Y));
#else
                var gridPoint = this.dataGrid.TransformToVisual(null).Transform(new Point(0, 0));
                var gridRect = new Rect(gridPoint, new Point(this.dataGrid.ActualWidth + gridPoint.X, this.dataGrid.ActualHeight + gridPoint.Y));
#endif
#if !WP
                allowScrollOnHorizontalRightLimits = gridRect.Right - 20 < mousePoint.X;
                allowScrollOnHorizontalLeftLimits = gridRect.Left + 20 > mousePoint.X;
#else
                allowScrollOnHorizontalRightLimits = gridRect.Right - 50 < mousePoint.X;
                allowScrollOnHorizontalLeftLimits = gridRect.Left + 50 > mousePoint.X;
#endif

                bool isTimerRunning = false;
                if (gridRect.Contains(mousePoint) && GetHeaderRowRect().Bottom + 10 > mousePoint.Y)
                {
                    this.ShowDragIndication(gridRect, mousePoint, mousePointOverGrid);
                    VisualStateManager.GoToState(this.PopupContent, "Valid", true);
                }
                else
                {
                    this.CloseDragIndication();
                    VisualStateManager.GoToState(this.PopupContent, "InValid", true);
                }

                if (allowScrollOnHorizontalRightLimits || allowScrollOnHorizontalLeftLimits && !isTimerRunning)
                {
                    dpTimer.Start();
                    isTimerRunning = true;
                }
                else
                {
                    dpTimer.Stop();
                    isTimerRunning = false;
                }
            }
        }

        private void OnDispatcherTimerTick(object sender, object e)
        {
            var linesCollection = this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLines();
            if (allowScrollOnHorizontalRightLimits)
            {
                var rightLastColumn = linesCollection[linesCollection.LastBodyVisibleIndex];
                if (this.dataGrid.Columns.Count >= rightLastColumn.LineIndex + 1)
                {
                    int scollIndex = rightLastColumn.LineIndex + 1;
                    int count;
                    while (this.dataGrid.VisualContainer.ColumnWidths.GetHidden(scollIndex, out count))
                    {
                        scollIndex += 1;
                        if (scollIndex + 1 > this.dataGrid.VisualContainer.ColumnCount)
                        {
                            scollIndex = this.dataGrid.VisualContainer.ColumnCount - 1;
                            break;
                        }
                    }
                    this.dataGrid.VisualContainer.ScrollColumns.ScrollInView(scollIndex);
                    this.dataGrid.VisualContainer.UpdateScrollBars();
                    this.dataGrid.VisualContainer.ScrollOwner.InvalidateScrollInfo();
                }
            }
            else if (allowScrollOnHorizontalLeftLimits && linesCollection.Count > linesCollection.FirstBodyVisibleIndex)
            {
                var leftLastColumn = linesCollection[linesCollection.FirstBodyVisibleIndex];
                if (leftLastColumn.LineIndex - 1 >= 0)
                {
                    int scollIndex = leftLastColumn.LineIndex - 1;
                    int count;
                    while (this.dataGrid.VisualContainer.ColumnWidths.GetHidden(scollIndex, out count))
                    {
                        scollIndex -= 1;
                        if (scollIndex - 1 < 0)
                        {
                            scollIndex = 0;
                            break;
                        }
                    }
                    this.dataGrid.VisualContainer.ScrollColumns.ScrollInView(scollIndex);
                    this.dataGrid.VisualContainer.UpdateScrollBars();
                    this.dataGrid.VisualContainer.ScrollOwner.InvalidateScrollInfo();
                }
            }
        }

        /// <summary>
        /// Calls for show drag indicator for popup
        /// </summary>
        /// <param name="gridRect"></param>
        /// <param name="mousePoint"></param>
        /// <param name="mousePointOverGrid"></param>
        /// <remarks></remarks>
        private void ShowDragIndication(Rect gridRect, Point mousePoint, Point mousePointOverGrid)
        {
            if (upIndicator != null && downIndicator != null)
            {
                dropedColIndex = 0;
                bool canShowIndicator = false;
                bool isPointerInGroupArea;
                var headerRowRect = GetHeaderRowRect();
                int indicatorColumnIndex;
                var column = this.PopupContent.Tag as GridColumn;
                var point = this.GetArrowIndicatorLocation(gridRect, mousePoint, column, mousePointOverGrid, out isPointerInGroupArea, out canShowIndicator, out indicatorColumnIndex);

                if (canShowIndicator && !isPointerInGroupArea && !previousIndicatorPosition.Equals(point))
                {
                    var args = new QueryColumnDraggingEventArgs(this.dataGrid)
                    {
                        From = this.dataGrid.Columns.IndexOf(column),
                        To = indicatorColumnIndex,
                        PopupPosition = mousePoint,
                        Reason = QueryColumnDraggingReason.Dragging
                    };
                    if (this.dataGrid.RaiseQueryColumnDragging(args))
                    {
                        this.DraggablePopup.IsOpen = false;
                        return;
                    }
                }
                previousIndicatorPosition = point;
                double VOffsetForUpIndicator, VOffsetForDownIndicator, HOffset = point.X;
                if (isPointerInGroupArea)
                {
                    var adjustValue = this.dataGrid.GroupDropArea.groupItemsGrid.ActualHeight / 2;
                    VOffsetForUpIndicator = point.Y + adjustValue;
                    VOffsetForDownIndicator = point.Y - adjustValue - (downIndicator.Child as ContentControl).ActualHeight;
                }
                else
                {
                    VOffsetForUpIndicator = headerRowRect.Y + headerRowRect.Height;
                    VOffsetForDownIndicator = headerRowRect.Y - (downIndicator.Child as ContentControl).ActualHeight;
                }
                var upIndicatorContent = upIndicator.Child as UpIndicatorContentControl;
                var downIndicatorContent = downIndicator.Child as DownIndicatorContentControl;

                if ((!isPointerInGroupArea && !this.dataGrid.AllowDraggingColumns) || !canShowIndicator)
                    this.CloseDragIndication();
                else
                {
#if WPF
                    // When creating two windows the second created window does not show the Up and Down indicator if first window is closed.
                    upIndicator.PlacementTarget = this.dataGrid;
                    downIndicator.PlacementTarget = this.dataGrid;
#endif
                    upIndicator.IsOpen = true;
                    downIndicator.IsOpen = true;
#if WinRT
                    upIndicatorContent.IsOpen = true;
                    downIndicatorContent.IsOpen = true;
#else
                    VisualStateManager.GoToState(upIndicatorContent, "Open", false);
                    VisualStateManager.GoToState(downIndicatorContent, "Open", false);
#endif
                }
#if WPF
                upIndicator.PlacementRectangle = new Rect(HOffset, VOffsetForUpIndicator, upIndicatorContent.ActualWidth, upIndicatorContent.ActualHeight);
                downIndicator.PlacementRectangle = new Rect(HOffset, VOffsetForDownIndicator, downIndicatorContent.ActualWidth, downIndicatorContent.ActualHeight);
#else
                upIndicator.HorizontalOffset = downIndicator.HorizontalOffset = HOffset;
                upIndicator.VerticalOffset = VOffsetForUpIndicator;
                downIndicator.VerticalOffset = VOffsetForDownIndicator;
#endif
            }
        }


        /// <summary>
        /// Gets Arrow Indicator Location for Draggable popup
        /// </summary>
        /// <param name="gridRect"></param>
        /// <param name="mousePoint"></param>
        /// <param name="column"></param>
        /// <param name="mousePointOverGrid"></param>
        /// <param name="isPointerInGroupArea">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="canShowIndicators">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="indicatorColumnIndex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private Point GetArrowIndicatorLocation(Rect gridRect, Point mousePoint, GridColumn column, Point mousePointOverGrid, out bool isPointerInGroupArea, out bool canShowIndicators, out int indicatorColumnIndex)
        {
            bool isPointerInHeaderArea;
            var groupAreaRect = this.GetGroupDropAreaRect();
            var headerAreaRect = this.GetHeaderRowRect();
            isPointerInHeaderArea = headerAreaRect.Contains(mousePoint);
            isPointerInGroupArea = groupAreaRect.Contains(mousePoint) && this.dataGrid.ShowGroupDropArea;
            if (!isPointerInGroupArea)
            {
                if (!this.dataGrid.AllowDraggingColumns)
                {
                    canShowIndicators = false;
                    indicatorColumnIndex = -1;
                    return new Point(0, 0);
                }
#if WPF
                var rowColIndex = this.dataGrid.VisualContainer.PointToCellRowColumnIndex(Mouse.GetPosition(this.dataGrid));
#elif WP
                var rowColIndex = new RowColumnIndex();
                if (this.dataGrid.ZoomScale != 1.0)
                {
                    var columnVisibleLines = this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLines();
                    var xStartingPoint = columnVisibleLines[0].Origin;
                    int colIndex = -1, rowIndex = this.dataGrid.StackedHeaderRows.Count;
                    foreach (var columnVisibleLine in columnVisibleLines)
                    {
                        if (mousePointOverGrid.X > xStartingPoint)
                        {
                            xStartingPoint += (columnVisibleLine.Size*this.dataGrid.ZoomScale);
                            colIndex = columnVisibleLine.LineIndex;
                        }
                    }
                    if (colIndex == -1)
                        colIndex = this.dataGrid.VisualContainer.PointToCellRowColumnIndex(mousePointOverGrid).ColumnIndex;
                    rowColIndex = new RowColumnIndex(){ RowIndex = rowIndex, ColumnIndex = colIndex};
                }
                else
                {
                     rowColIndex = this.dataGrid.VisualContainer.PointToCellRowColumnIndex(mousePointOverGrid);
                }
#else
                var rowColIndex = this.dataGrid.VisualContainer.PointToCellRowColumnIndex(mousePointOverGrid);
#endif
                if (this.dataGrid.ResolveToGridVisibleColumnIndex(rowColIndex.ColumnIndex) >= 0)
                    canShowIndicators = true;
                else
                    canShowIndicators = false;
                if (isPointerInHeaderArea)
                    canShowIndicators = true;
                Rect columnRect = Rect.Empty;
                if (isPointerInHeaderArea && this.dataGrid.ResolveToGridVisibleColumnIndex(rowColIndex.ColumnIndex) < 0)
                    columnRect = this.dataGrid.VisualContainer.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, rowColIndex, false, true);
                else
                    columnRect = this.dataGrid.VisualContainer.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, rowColIndex, false, true);
                if (isPointerInHeaderArea && this.dataGrid.ResolveToGridVisibleColumnIndex(rowColIndex.ColumnIndex) < 0)
                {
                    var increasedValue = 0d;
                    if (this.dataGrid.ShowRowHeader)
                        increasedValue += this.dataGrid.RowHeaderWidth;
                    this.dataGrid.View.GroupDescriptions.ForEach(desc => increasedValue += this.dataGrid.GridModel.IndentColumnSize);
#if !WP
                    if (this.dataGrid.DetailsViewManager.HasDetailsView)
                        increasedValue += this.dataGrid.GridModel.IndentColumnSize;
#endif
                    if (!columnRect.IsEmpty)
                        columnRect.X = increasedValue;
                }
#if WP
                columnRect.X = columnRect.X*this.dataGrid.ZoomScale;
                columnRect.Width = columnRect.Width*this.dataGrid.ZoomScale;
#endif
                Point pt;
                const int adjustValue = 8;
                if (mousePoint.X >= (((gridRect.X + (columnRect != Rect.Empty ? columnRect.X : 0)) - adjustValue) + ((columnRect != Rect.Empty ? columnRect.Width : 0) / 2)))
                {
                    var xPoint = (gridRect.X + (columnRect != Rect.Empty ? columnRect.X : 0) + (columnRect != Rect.Empty ? columnRect.Width : 0));
                    if (xPoint > gridRect.Right)
                        xPoint = xPoint - (columnRect != Rect.Empty ? columnRect.Width : 0);
                    pt = new Point(xPoint - adjustValue, (columnRect != Rect.Empty ? columnRect.Y : 0) + gridRect.Y);
                }
                else
                {
                    var xPoint = gridRect.X + (columnRect != Rect.Empty ? columnRect.X : 0);
                    if (xPoint < gridRect.Left)
                        xPoint = xPoint + (columnRect != Rect.Empty ? columnRect.Width : 0);
                    pt = new Point(xPoint - adjustValue, (columnRect != Rect.Empty ? columnRect.Y : 0) + gridRect.Y);
                }
                
                indicatorColumnIndex = rowColIndex.ColumnIndex;
                return pt;
            }
            else
            {
                if (column != null && !this.CanGroupColumn(column))
                {
                    canShowIndicators = false;
                    indicatorColumnIndex = -1;
                    return new Point(0, 0);
                }
                double Vmiddlepoint = groupAreaRect.Height / 2 + groupAreaRect.Y;
                double Hpoint = groupAreaRect.X + this.dataGrid.GroupDropArea.groupItemsGrid.Margin.Left;

                if (this.dataGrid.GroupDropArea.Panel.Children.Count >= 0)
                {
                    foreach (GroupDropAreaItem item in this.dataGrid.GroupDropArea.Panel.Children)
                    {
#if WinRT
                        Rect itemRect = new Rect(item.TransformToVisual(null).TransformPoint(new Point(0, 0)), item.DesiredSize);
#elif WPF
                        var itemRect = this.GetControlRect(item);
#else
                        Rect itemRect = new Rect(item.TransformToVisual(null).Transform(new Point(0, 0)), item.DesiredSize);
#endif
                        double margins = item.Margin.Left + item.Margin.Right;
                        if (itemRect.Right > mousePoint.X)
                        {

                            if (mousePoint.X >= (itemRect.X + (itemRect.Width / 2)))
                            {
#if WPF
                                Hpoint = itemRect.Right;
#else
                                Hpoint = itemRect.Right - margins;
#endif
                                dropedColIndex++;
                            }
                            else
                            {
                                Hpoint = itemRect.X - margins;
                            }
                            break;
                        }
                        else
#if WPF
                            Hpoint = itemRect.Right;
#else
                            Hpoint = itemRect.Right - margins;
#endif
                        dropedColIndex++;
                    }
                }
                canShowIndicators = true;
                indicatorColumnIndex = dropedColIndex;
                return new Point(Hpoint, Vmiddlepoint);
            }
        }

        /// <summary>
        /// Hides the Draggable Popup
        /// </summary>
        /// <remarks></remarks>
        internal void HidePopup()
        {
            if (this.isExpandedInternally)
            {
                this.dataGrid.GroupDropArea.IsExpanded = false;
                this.isExpandedInternally = false;
            }
#if !WPF
            if (!suspendReverseAnimationByColumnChooser)
                reverseAnimationStoryboard.Begin();
            else
#else
            this.DraggablePopup.IsOpen = false;
            this.dataGrid.Focus();
#endif
                this.DraggablePopup.IsOpen = false;
        }

        /// <summary>
        /// Close the Drag arrow indication
        /// </summary>
        /// <remarks></remarks>
        internal void CloseDragIndication()
        {
            if (upIndicator != null && downIndicator != null)
            {
                upIndicator.IsOpen = false;
                downIndicator.IsOpen = false;
            }
        }

        /// <summary>
        /// Popup dropped on Group Drop Area
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        protected virtual void PopupContentDroppedOnGroupDropArea(GridColumn column)
        {
            if (this.PopupContent.IsDragFromGroupDropArea)
            {
                var sortColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(desc => desc.ColumnName == column.MappingName);
                var direction = sortColumn != null ? sortColumn.SortDirection : ListSortDirection.Ascending;
                var dropIndex = 0;
                if (dropedColIndex > this.dataGrid.View.GroupDescriptions.IndexOf(this.dataGrid.View.GroupDescriptions.FirstOrDefault(col => (col as PropertyGroupDescription).PropertyName.Equals(column.MappingName))))
                    dropIndex = dropedColIndex - 1 >= 0 ? dropedColIndex - 1 : 0;
                else
                    dropIndex = dropedColIndex;
                this.dataGrid.GroupDropArea.MoveGroupDropAreaItem(column, direction, dropIndex);
                this.DraggablePopup.IsOpen = false;
            }
            else
            {
                if (!this.dataGrid.GroupDropArea.IsExpanded)
                    this.dataGrid.GroupDropArea.IsExpanded = true;

                if (CanGroupColumn(column))
                {
                    this.DraggablePopup.IsOpen = false;
                    if (this.isExpandedInternally)
                        this.isExpandedInternally = false;

                    if (this.dataGrid.GroupDropArea.Panel.Children.Count > 0)
                    {
                        var sortColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(desc => desc.ColumnName == column.MappingName);
                        var direction = sortColumn != null ? sortColumn.SortDirection : ListSortDirection.Ascending;
                        this.dataGrid.GroupDropArea.AddGroupAreaItem(column, direction, dropedColIndex);
                    }
                    else
                    {
                        var sortColumn = this.dataGrid.SortColumnDescriptions.FirstOrDefault(desc => desc.ColumnName == column.MappingName);
                        var direction = sortColumn != null ? sortColumn.SortDirection : ListSortDirection.Ascending;
                        this.dataGrid.GroupDropArea.AddGroupAreaItem(column, direction);
                    }
                }
                else
                {
                    if (this.isExpandedInternally)
                    {
                        this.dataGrid.GroupDropArea.IsExpanded = false;
                        this.isExpandedInternally = false;
                    }
#if !WPF
                    if (!this.PopupContent.IsDragFromColumnChooser)
                        reverseAnimationStoryboard.Begin();
                    else
                        this.DraggablePopup.IsOpen = false;
#else
                                this.DraggablePopup.IsOpen = false;
                                this.dataGrid.Focus();
#endif
                    return;
                }
            }
        }

        /// <summary>
        /// Popup dropped on Header row
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newColumnIndex"></param>
        /// <remarks></remarks>
        protected virtual void PopupContentDroppedOnHeaderRow(int oldIndex, int newColumnIndex)
        {
            var column = this.PopupContent.Tag as GridColumn;
            if (oldIndex != newColumnIndex || this.PopupContent.IsDragFromGroupDropArea || this.PopupContent.IsDragFromColumnChooser)
            {
                if (this.PopupContent.IsDragFromGroupDropArea)
                    this.dataGrid.GroupDropArea.RemoveGroupDropAreaItem(column);

                if (CanDropColumn(column))
                {
#if WinRT
                    this.dataGrid.Columns.Move(oldIndex, newColumnIndex);
#else
                    this.dataGrid.suspendForColumnMove = true;
                    var currCellIndex = this.dataGrid.SelectionController.CurrentCellManager.CurrentCellIndex;
#if !WP
                    if (this.dataGrid.SelectionController.CurrentCellManager.EndEdit(false))
                    {
                        var newCurrentCellIndex = RowColumnIndex.Empty;
                        if (this.dataGrid.ResolveToScrollColumnIndex(oldIndex) == currCellIndex.ColumnIndex)
                        {
                            newCurrentCellIndex.RowIndex = currCellIndex.RowIndex;
                            newCurrentCellIndex.ColumnIndex = this.dataGrid.ResolveToScrollColumnIndex(newColumnIndex);
                        }
                        else if (this.dataGrid.ResolveToScrollColumnIndex(newColumnIndex) < this.dataGrid.ResolveToScrollColumnIndex(oldIndex) && this.dataGrid.ResolveToScrollColumnIndex(oldIndex) > currCellIndex.ColumnIndex && this.dataGrid.ResolveToScrollColumnIndex(newColumnIndex) <= currCellIndex.ColumnIndex)
                        {
                            newCurrentCellIndex.RowIndex = currCellIndex.RowIndex;
                            newCurrentCellIndex.ColumnIndex = currCellIndex.ColumnIndex + 1;
                        }
                        else if (this.dataGrid.ResolveToScrollColumnIndex(newColumnIndex) > this.dataGrid.ResolveToScrollColumnIndex(oldIndex) && this.dataGrid.ResolveToScrollColumnIndex(oldIndex) < currCellIndex.ColumnIndex && this.dataGrid.ResolveToScrollColumnIndex(newColumnIndex) >= currCellIndex.ColumnIndex)
                        {
                            newCurrentCellIndex.RowIndex = currCellIndex.RowIndex;
                            newCurrentCellIndex.ColumnIndex = currCellIndex.ColumnIndex - 1;
                        }

                        this.dataGrid.Columns.MoveTo(oldIndex, newColumnIndex);
                    }
#else
                    this.dataGrid.Columns.MoveTo(oldIndex, newColumnIndex);
#endif
                    this.dataGrid.suspendForColumnMove = false;
                    this.dataGrid.GridColumnSizer.RefreshAll();
#endif
                }
            }
            if (this.isExpandedInternally)
            {
                this.dataGrid.GroupDropArea.IsExpanded = false;
                this.isExpandedInternally = false;
            }
            this.DraggablePopup.IsOpen = false;
        }

        /// <summary>
        /// Popup dropped on DataGrid
        /// </summary>
        /// <param name="point"></param>
        /// <remarks></remarks>
        protected virtual void PopupContentDroppedOnGrid(Point point)
        {
            var column = this.PopupContent.Tag as GridColumn;
            if (this.PopupContent.IsDragFromGroupDropArea && column != null)
            {
                this.dataGrid.GroupDropArea.RemoveGroupDropAreaItem(column);
                this.DraggablePopup.IsOpen = false;
            }
            HidePopup();
        }

#if !WPF
        /// <summary>
        /// Suspends the reverse animation for the popup
        /// </summary>
        /// <param name="suspend">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <remarks></remarks>
        protected void SuspendReverseAnimation(bool suspend)
        {
            suspendReverseAnimationByColumnChooser = suspend;
        }
#endif

#if WP
        internal void OnPopupContentDropped(Point point)
#else
        /// <summary>
        /// Popup on dropped
        /// </summary>
        /// <param name="point"></param>
        /// <remarks></remarks>
        protected virtual void OnPopupContentDropped(Point point, Point pointOverGrid)
#endif
        {
            this.CloseDragIndication();
            dpTimer.Stop();
            if (this.DraggablePopup.IsOpen)
            {
#if WP
                this.dataGrid.suspendZooming = false;
                this.dataGrid.GroupDropArea.scrollersDisabled = false;
#endif
                isDragState = false;
#if WinRT
                var containerpoint = this.dataGrid.VisualContainer.TransformToVisual(null).TransformPoint(new Point(0, 0));
                var gridpoint = new Point(point.X - containerpoint.X, point.Y - containerpoint.Y);
#elif WPF
                var gridRect = this.GetControlRect(this.dataGrid);
                var gridpoint = new Point(gridRect.X, gridRect.Y);
#else
                var containerpoint = this.dataGrid.VisualContainer.TransformToVisual(null).Transform(new Point(0, 0));
                var gridpoint = new Point(point.X - containerpoint.X, point.Y - containerpoint.Y);
#endif
                if (this.PopupContent.Tag is GridColumn)
                {
                    var column = this.PopupContent.Tag as GridColumn;
                    var oldIndex = this.dataGrid.Columns.IndexOf(column);
                    var newColumnIndex = -1;
                    var region = PointToGridRegion(point);
                    if (region == GridRegion.Header)
                    {
                        var groupDropAreaRect = GetGroupDropAreaRect();
                        if (!groupDropAreaRect.IsEmpty)
                        {
                            point.X = point.X - groupDropAreaRect.X;
                            point.Y = point.Y - groupDropAreaRect.Height;
                        }
#if !WP
                        else
                            point = pointOverGrid;
#endif
                        var rowColIndex = this.dataGrid.VisualContainer.PointToCellRowColumnIndex(point);
                        var cellrect = this.dataGrid.VisualContainer.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, rowColIndex, false, true);
                        newColumnIndex = this.dataGrid.ResolveToGridVisibleColumnIndex(rowColIndex.ColumnIndex);

                        if (newColumnIndex < 0 || newColumnIndex >= dataGrid.Columns.Count)
                            newColumnIndex = newColumnIndex < 0 ? 0 : dataGrid.Columns.Count - 1;
                        else if (oldIndex < newColumnIndex)
                        {
                            if (point.X < (cellrect.X + cellrect.Width / 2))
                                newColumnIndex--;
                        }
                        else if (oldIndex > newColumnIndex)
                        {
                            if (point.X > (cellrect.X + cellrect.Width / 2))
                                newColumnIndex++;
                        }
                    }

                    var args = new QueryColumnDraggingEventArgs(this.dataGrid)
                    {
                        From = oldIndex,
                        To = this.dataGrid.AllowDraggingColumns ? newColumnIndex : -1,
                        PopupPosition = point,
                        Reason = QueryColumnDraggingReason.Dropping
                    };
                    if (this.dataGrid.RaiseQueryColumnDragging(args))
                    {
                        this.DraggablePopup.IsOpen = false;
                        return;
                    }
                    if (region == GridRegion.GroupDropArea)
                    {
                        if (!CanGroupColumn(column))
                        {
                            this.DraggablePopup.IsOpen = false;
                            return;
                        }
                        PopupContentDroppedOnGroupDropArea(this.PopupContent.Tag as GridColumn);
                    }
                    else
                    {
                        if (region == GridRegion.Header)
                        {
                            PopupContentDroppedOnHeaderRow(oldIndex, newColumnIndex);
                        }
                        else if (region == GridRegion.Grid)
                        {
                            PopupContentDroppedOnGrid(point);
                        }
                        else
                        {
                            if (this.PopupContent.IsDragFromGroupDropArea)
                            {
                                this.dataGrid.GroupDropArea.RemoveGroupDropAreaItem(column);
                                this.DraggablePopup.IsOpen = false;
                            }
                            else
                            {
                                HidePopup();
                            }
                        }
                    }
                    args.Reason = QueryColumnDraggingReason.Dropped;
                    this.dataGrid.RaiseQueryColumnDragging(args);
                    //this.DraggablePopup.IsOpen = false;
#if WinRT
                    this.dataGrid.Focus(FocusState.Programmatic);
#else
                    this.dataGrid.Focus();
#endif
                }
            }
        }

        /// <summary>
        /// Returns whether the column can be grouped or not
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CanGroupColumn(GridColumn column)
        {
            bool canGroup = false;
            var groupcolumn = column.ReadLocalValue(GridColumn.AllowGroupingProperty);
            if (this.dataGrid.AllowGrouping)
                canGroup = true;
            if (groupcolumn != DependencyProperty.UnsetValue || canGroup)
                canGroup = column.AllowGrouping;
            return canGroup;
        }

        /// <summary>
        /// Returns whether the column can be drop or not
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CanDropColumn(GridColumn column)
        {
            if (column.MappingName == null)
            {
                throw new InvalidOperationException("MappingName is necessary for Sorting,Grouping & Filtering");
            }
            bool candrop = false;
            var dragColumn = column.ReadLocalValue(GridColumn.AllowDraggingProperty);
            if (dragColumn != DependencyProperty.UnsetValue)
                candrop = column.AllowDragging;
            else if (this.dataGrid.AllowDraggingColumns)
                candrop = true;
            return candrop;
        }
#if WPF
        /// <summary>
        /// Returns rect for given Control
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal Rect GetControlRect(FrameworkElement control)
        {
            Point locationfromWindow = control.TranslatePoint(new Point(0, 0), control);
            Point locationfromScreen = control.PointToScreen(locationfromWindow);
            return new Rect((locationfromScreen.X - locationfromWindow.X),
                                (locationfromScreen.Y - locationfromWindow.Y),
                                control.ActualWidth, control.ActualHeight);
        }
#endif
        /// <summary>
        /// Returns GroupDropAreaRect
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected Rect GetGroupDropAreaRect()
        {
            if (this.dataGrid.GroupDropArea == null)
                return Rect.Empty;
#if WinRT
            var point = this.dataGrid.GroupDropArea.TransformToVisual(null).TransformPoint(new Point(0, 0));
            var rect = new Rect(point.X, point.Y, this.dataGrid.GroupDropArea.ActualWidth, this.dataGrid.GroupDropArea.ActualHeight);
#elif WPF
            var rect = this.GetControlRect(this.dataGrid.GroupDropArea);
#else
            var point = this.dataGrid.GroupDropArea.TransformToVisual(null).Transform(new Point(0, 0));
            var rect = new Rect(point.X, point.Y, this.dataGrid.GroupDropArea.ActualWidth, this.dataGrid.GroupDropArea.ActualHeight);
#endif
            return rect;
        }

        /// <summary>
        /// Returns Popup Rect
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected Rect GetPopupRect()
        {
#if WinRT
            var point = this.PopupContent.TransformToVisual(null).TransformPoint(new Point(0, 0));
            var rect = new Rect(point.X, point.Y, this.PopupContent.ActualWidth, this.PopupContent.ActualHeight);
#elif WPF
            var rect = this.GetControlRect(this.PopupContent);
#elif WP
            var point = this.PopupContent.TransformToVisual(null).Transform(new Point(0, 0));
            var rect = new Rect(point.X, point.Y, this.PopupContent.ActualWidth * this.dataGrid.ZoomScale, this.PopupContent.ActualHeight * this.dataGrid.ZoomScale);
#else
            var point = this.PopupContent.TransformToVisual(null).Transform(new Point(0, 0));
            var rect = new Rect(point.X, point.Y, this.PopupContent.ActualWidth, this.PopupContent.ActualHeight);
#endif

            return rect;
        }

        /// <summary>
        /// Returns HeaderRow Rect
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected Rect GetHeaderRowRect()
        {
            if (this.dataGrid.RowGenerator.Items.Count > 0)
            {
                double actualHeight = 0;
                var headerRows = this.dataGrid.RowGenerator.Items.Where(row => row.RowRegion == RowRegion.Header && !row.IsAddNewRow);
                var firstHeaderRow = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowRegion == RowRegion.Header && !row.IsAddNewRow);
                var lastHeaderRow = this.dataGrid.RowGenerator.Items.LastOrDefault(row => row.RowRegion == RowRegion.Header && !row.IsAddNewRow);
                headerRows.ForEach(row =>
                {
                    if (row != null && row.WholeRowElement != null)
                    {
                        actualHeight += row.WholeRowElement.ActualHeight;
                    }
                });
                if (firstHeaderRow != null && firstHeaderRow.WholeRowElement != null && lastHeaderRow != null && lastHeaderRow != null)
                {
#if WinRT
                    var lastHeaderYPoint = lastHeaderRow.WholeRowElement.TransformToVisual(null).TransformPoint(new Point(0, 0));
                    var point = firstHeaderRow.WholeRowElement.TransformToVisual(null).TransformPoint(new Point(0, 0));
                    var rect = new Rect(lastHeaderYPoint.X, point.Y, lastHeaderRow.WholeRowElement.ActualWidth, actualHeight);
#elif WPF
                    Point locationfromWindow = lastHeaderRow.WholeRowElement.TranslatePoint(new Point(0, 0), lastHeaderRow.WholeRowElement);
                    Point locationfromScreen = lastHeaderRow.WholeRowElement.PointToScreen(locationfromWindow);

                    var x = (locationfromScreen.X - locationfromWindow.X);

                    locationfromWindow = firstHeaderRow.WholeRowElement.TranslatePoint(new Point(0, 0), firstHeaderRow.WholeRowElement);
                    locationfromScreen = firstHeaderRow.WholeRowElement.PointToScreen(locationfromWindow);

                    var y = (locationfromScreen.Y - locationfromWindow.Y);

                    var rect = new Rect(x, y, lastHeaderRow.WholeRowElement.ActualWidth, actualHeight);
#else
                    var lastHeaderYPoint = lastHeaderRow.WholeRowElement.TransformToVisual(null).Transform(new Point(0, 0));
                    var point = firstHeaderRow.WholeRowElement.TransformToVisual(null).Transform(new Point(0, 0));
#if WP
                    var rect = new Rect(lastHeaderYPoint.X, point.Y, lastHeaderRow.WholeRowElement.ActualWidth * this.dataGrid.ZoomScale, actualHeight * this.dataGrid.ZoomScale);
#else
                    var rect = new Rect(lastHeaderYPoint.X, point.Y, lastHeaderRow.WholeRowElement.ActualWidth, actualHeight);
#endif
#endif
                    return rect;
                }
            }
            return Rect.Empty;
        }

        /// <summary>
        /// Returns whether the column can be resize or not
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CanResizeColumn(GridColumn column)
        {
            bool canResizeColumn = false;
            var resizeColumn = column.ReadLocalValue(GridColumn.AllowResizingProperty);
            if (resizeColumn != DependencyProperty.UnsetValue)
                canResizeColumn = column.AllowResizing;
            if ((resizeColumn == DependencyProperty.UnsetValue) && this.dataGrid.AllowResizingColumns)
                canResizeColumn = true;
            return canResizeColumn;
        }

        /// <summary>
        /// Sets the popup position
        /// </summary>
        /// <param name="index"></param>
        /// <remarks></remarks>
        private void SetPopupPosition(int index)
        {
            var datarow = this.dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowRegion == RowRegion.Header && row.RowIndex == this.dataGrid.GetHeaderIndex());
            if (datarow != null && datarow.VisibleColumns != null)
            {
                var datacolumn = datarow.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index);
                if (datacolumn != null && datacolumn.ColumnElement != null)
                {
#if WinRT
                    var point = datacolumn.ColumnElement.TransformToVisual(null).TransformPoint(new Point(0, 0));
#elif WPF
                    var controlRect = this.GetControlRect(datacolumn.ColumnElement as Control);
                    var point = new Point(controlRect.X, controlRect.Y);
#else
                    var point = datacolumn.ColumnElement.TransformToVisual(null).Transform(new Point(0, 0));
#endif
                    var rect = new Rect(point.X, point.Y, datacolumn.ColumnElement.ActualWidth, datacolumn.ColumnElement.ActualHeight);
                    var horizontalOffset = rect.X - ((this.PopupContent.ThumbWidth / 2) + ((this.PopupContent.InitialRect.Width * 0.05) / 2));
                    var verticalOffset = rect.Y - ((this.PopupContent.InitialRect.Height * 0.05) / 2);
                    this.DraggablePopup.HorizontalOffset = Math.Round(horizontalOffset);
                    this.DraggablePopup.VerticalOffset = Math.Round(verticalOffset);
                    this.UpdateReverseAnimationOffset(this.DraggablePopup.HorizontalOffset, this.DraggablePopup.VerticalOffset, false);
                }
            }
        }
        #endregion

        #region public methods

        /// <summary>
        /// Initiate the Popup to Drag And Drop.
        /// </summary>
        /// <param name="gridColumnIndex"></param>
        /// <param name="rect"></param>
        /// <param name="pointer"></param>
        /// <remarks></remarks>
        public void ShowPopup(int gridColumnIndex, Rect rect, Pointer pointer)
        {
            var GridColumn = this.dataGrid.Columns[gridColumnIndex];
            if (GridColumn != null)
            {
                ShowPopup(GridColumn, rect, false, rect.Width / 2, rect.Height / 2, false, true, pointer);
            }

        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Resolve the point to view region (GroupDropArea, Header, Grid & None).
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual GridRegion PointToGridRegion(Point point)
        {
            var groupDropAreaRect = GetGroupDropAreaRect();
            var popupRect = GetPopupRect();
            groupDropAreaRect.Intersect(popupRect);
            var isOverGroupDropArea = groupDropAreaRect.Height >= 10 && groupDropAreaRect.Contains(point) && this.dataGrid.ShowGroupDropArea;
            if (isOverGroupDropArea)
                return GridRegion.GroupDropArea;

            var headerRowRect = GetHeaderRowRect();
            headerRowRect.Intersect(popupRect);
            if (headerRowRect.Height >= 5 && headerRowRect.Contains(point))
                return GridRegion.Header;

#if WinRT
            var gridRect = new Rect(this.dataGrid.TransformToVisual(null).TransformPoint(new Point(0, 0)), this.dataGrid.DesiredSize);
#elif WPF
            var gridRect = GetControlRect(this.dataGrid);
#else
            var gridRect = new Rect(this.dataGrid.TransformToVisual(null).Transform(new Point(0, 0)), this.dataGrid.DesiredSize);
#endif

            if (gridRect.Contains(point))
                return GridRegion.Grid;
            else
                return GridRegion.None;
        }

        /// <summary>
        /// Decide the column can show popup or not.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual bool CanShowPopup(GridColumn column)
        {
            var canShowPopup = false;
            var dragColumn = column.ReadLocalValue(GridColumn.AllowDraggingProperty);
            var groupcolumn = column.ReadLocalValue(GridColumn.AllowGroupingProperty);
            if (groupcolumn != DependencyProperty.UnsetValue)
                canShowPopup = column.AllowGrouping;
            if (!canShowPopup && dragColumn != DependencyProperty.UnsetValue)
                canShowPopup = column.AllowDragging;
            if ((groupcolumn == DependencyProperty.UnsetValue && (this.dataGrid.AllowGrouping && this.dataGrid.ShowGroupDropArea)) ||
                (dragColumn == DependencyProperty.UnsetValue && this.dataGrid.AllowDraggingColumns))
                canShowPopup = true;
            return canShowPopup;
        }

        /// <summary>
        /// Creates the Content for Draggable Popup.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual UIElement CreatePopupContent(GridColumn column)
        {
            var textblock = new TextBlock { Text = column.HeaderText };
            return textblock;
        }

        /// <summary>
        /// Indicates the Hidden Property value changed for GridColumn.
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        protected virtual void OnColumnHiddenChanged(GridColumn column)
        {
 
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            UnWireEvents();
            if (this.PopupContent != null)
            {
                this.PopupContent.PopupContentPositionChanged -= OnPopupContentPositionChanged;
                this.PopupContent.PopupContentDropped -= OnPopupContentDropped;
                this.PopupContent.PopupContentResizing -= this.dataGrid.GridColumnResizingController.OnPopupContentResizing;
                this.PopupContent.PopupContentResized -= this.dataGrid.GridColumnResizingController.OnPopupContentResized;
                this.PopupContent.Dispose();
                this.PopupContent = null;
            }
            this.dpTimer.Tick -= OnDispatcherTimerTick;
            this.DraggablePopup = null;
            this.upIndicator = null;
            this.downIndicator = null;
            this.DragLeftLine = null;
            this.DragRightLine = null;
            this.horizontalDoubleAnimation = null;
            this.verticalDoubleAnimation = null;
            this.reverseAnimationStoryboard = null;
            if (dataGrid != null)
            {
                this.dataGrid.GridColumnResizingController.dragLine = null;
                this.dataGrid = null;
            }
        }
        
        #endregion   
    }
}
