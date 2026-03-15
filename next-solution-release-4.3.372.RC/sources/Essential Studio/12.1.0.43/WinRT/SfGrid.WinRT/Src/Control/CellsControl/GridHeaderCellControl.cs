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
#if WinRT
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using System.Collections.Specialized;
using Windows.UI.Xaml.Data;
using System.Threading.Tasks;
using Syncfusion.Data;
#else
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Threading;
using Syncfusion.Data;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
using System.Collections.Generic;
using Syncfusion.Data.Extensions;
#endif
#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using MouseEventArgs = PointerRoutedEventArgs;
    using MouseButtonEventArgs = PointerRoutedEventArgs;
    using GestureEventArgs = HoldingRoutedEventArgs;
    using ManipulationStartedEventArgs = ManipulationStartedRoutedEventArgs;
#endif
    [ClassReference(IsReviewed = false)]

#if !WP
    [TemplatePart(Name = "PART_FilterToggleButton", Type = typeof(FilterToggleButton))]
    [TemplatePart(Name = "PART_FilterPopUpPresenter", Type = typeof(Border))]
#endif
    public sealed class GridHeaderCellControl : ContentControl, IDisposable
    {
        #region Fields

        internal SfDataGrid dataGrid;
        internal string hiddenResizingVisualState;
        internal bool isTouchPressed = false;
        private Point mouseDownPoint;
        private Point pointerDown;
        internal static bool isFilterToggleButtonClicked = false;
#if WPF
        private TouchDevice touchDevice;
#endif
#if WP
        internal PopupContentPositionChanged PopupContentPositionChanged;
        internal PopupContentDropped PopupContentDropped;
        double mouseHorizontalPosition;
        double mouseVerticalPosition;
        bool canDrop;
#endif
#if WinRT
        private Pointer pointer = null;
#endif

        #endregion

        #region Ctor

        public GridHeaderCellControl()
        {
            this.DefaultStyleKey = typeof(GridHeaderCellControl);
#if WinRT
            this.IsHoldingEnabled = true;
            this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
#endif
            this.IsTabStop = false;
#if WPF
            this.IsManipulationEnabled = true;
#endif
        }

        #endregion
#if !WP
        #region Filtering Properties

        /// <summary>
        /// Gets Filter popup Control type of GridFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        private GridFilterControl FilterPopupHost;

        /// <summary>
        /// Gets or sets Filter toggle button.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>

        private FilterToggleButton FilterToggleButton;

        /// <summary>
        /// Gets or sets Presenter for Filter Popup.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        private Border FilterPopUpPresenter;

        /// <summary>
        /// Gets a value indicating whether the filter s applied .
        /// </summary>
        /// <value><see langword="true"/> if this instance ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        internal bool IsFilterApplied
        {
            get { return this.Column.FilterPredicates.Count > 0 ? true : false; }
        }

        #endregion
#endif
        #region Sorting Dependency Properties

        /// <summary>
        /// Gets or sets Associated GridColumn.
        /// </summary>
        /// <value></value>
        /// <remarks>Using this Column all other operations will be done</remarks>
        public GridColumn Column
        {
            get { return (GridColumn)GetValue(ColumnProperty); }
            set
            {
                SetValue(ColumnProperty, value);
            }
        }

        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof(GridColumn), typeof(GridHeaderCellControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets ColumnOptionsWidth.
        /// </summary>
        /// <value></value>
        /// <remarks>this width will be set for column width rest than for Content presenter</remarks>
        public double ColumnOptionsWidth
        {
            get { return (double)this.GetValue(ColumnOptionsWidthProperty); }
            internal set { this.SetValue(ColumnOptionsWidthProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsWidthProperty =
            DependencyProperty.Register("ColumnOptionsWidth", typeof(double), typeof(GridHeaderCellControl),
                                        new PropertyMetadata((double)0, null));

        /// <summary>
        /// Gets or sets Order/Number for sort columns.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string SortNumber
        {
            get { return (string)this.GetValue(SortNumberProperty); }
            internal set { this.SetValue(SortNumberProperty, value); }
        }

        public static readonly DependencyProperty SortNumberProperty = DependencyProperty.Register("SortNumber", typeof(string), typeof(GridHeaderCellControl), new PropertyMetadata(String.Empty));

        /// <summary>
        /// <summary>
        /// Gets or sets Sorting Number visibility.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Visibility SortNumberVisibility
        {
            get { return (Visibility)this.GetValue(SortNumberVisibilityProperty); }
            set { this.SetValue(SortNumberVisibilityProperty, value); }
        }

        public static readonly DependencyProperty SortNumberVisibilityProperty =
            DependencyProperty.Register("SortNumberVisibility", typeof(Visibility), typeof(GridHeaderCellControl),
                                        new PropertyMetadata(Visibility.Collapsed, null));

        /// Gets or sets Path direction (Ascending/Descending).
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public object SortDirection
        {
            get { return (object)this.GetValue(SortDirectionProperty); }
            set { this.SetValue(SortDirectionProperty, value); }
        }

        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register("SortDirection", typeof(object), typeof(GridHeaderCellControl), new PropertyMetadata(null));

        public Visibility FilterIconVisiblity
        {
            get { return (Visibility)this.GetValue(FilterIconVisiblityProperty); }
            set { this.SetValue(FilterIconVisiblityProperty, value); }
        }

        public static readonly DependencyProperty FilterIconVisiblityProperty =
            DependencyProperty.Register("FilterIconVisiblity", typeof(Visibility), typeof(GridHeaderCellControl),
                                        new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Override Methods

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
#if !WP
            if (FilterPopUpPresenter != null)
            {
                FilterPopUpPresenter.Child = null;
            }
#endif
            UnWireGridHeaderCellControlEvents();
#if !WP
#if WPF
            UnWireExcelLikeFilteringEvents(true);
#else
            UnWireExcelLikeFilteringEvents(false);
#endif
#endif
            base.OnApplyTemplate();
#if SILVERLIGHT
            if(FilterToggleButton!=null)
                FilterToggleButton.Tag = this;
#endif

#if !WP
            FilterToggleButton = GetTemplateChild("PART_FilterToggleButton") as FilterToggleButton;
            FilterPopUpPresenter = GetTemplateChild("PART_FilterPopUpPresenter") as Border;
#else
            if(this.PopupContentPositionChanged == null)
                this.PopupContentPositionChanged = this.dataGrid. GridColumnDragDropController.OnPopupContentPositionChanged;
            if(this.PopupContentDropped == null)
                this.PopupContentDropped = this.dataGrid. GridColumnDragDropController.OnPopupContentDropped;
#endif
            WireGridHeaderCellControlEvents();
#if !WP
            WireExcelLikeFilteringEvents();
            ApplyFilterToggleButtonVisualState();
            // Applying Visual state for Hidden Column Resizing
            if (!string.IsNullOrEmpty(hiddenResizingVisualState))
                VisualStateManager.GoToState(this, hiddenResizingVisualState, true);
#if SILVERLIGHT
             OnInitializeFilterPopup();
#endif
#endif
        }
#if SILVERLIGHT
        private void OnInitializeFilterPopup()
        {
            if (FilterPopupHost == null) 
                return;

            if (FilterPopupHost.Parent is Border)
                (FilterPopupHost.Parent as Border).Child = null;
            this.FilterPopUpPresenter.Child = FilterPopupHost;
            FilterPopupHost.ClearValue(GridFilterControl.IsOpenProperty);
            Binding binding = new Binding();
            binding.Source = FilterToggleButton;
            binding.Path = new PropertyPath("IsChecked");
            binding.Mode = BindingMode.TwoWay;
            FilterPopupHost.SetBinding(GridFilterControl.IsOpenProperty, binding);
        }
#endif

        private void GridHeaderCellControlLoaded(object sender, RoutedEventArgs e)
        {
#if !WP
            ApplyFilterToggleButtonVisualState();
#endif
#if WPF
            this.ContextMenuOpening += OnContextMenuOpening;
#endif
        }

        private void GridHeaderCellControlUnloaded(object sender, RoutedEventArgs e)
        {
#if WPF
            this.ContextMenuOpening -= OnContextMenuOpening;
#endif
            this.Loaded -= GridHeaderCellControlLoaded;
            this.Unloaded -= GridHeaderCellControlUnloaded;
        }

#if WinRT

        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
            if (this.dataGrid.GridColumnResizingController.dragLine != null)
                this.CapturePointer(pointer);
            base.OnManipulationStarting(e);
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            this.dataGrid.GridColumnResizingController.dragLine = null;
            if (!this.dataGrid.Validations.CheckForValidation(false))
                return;

            if (Column.AllowSorting && this.dataGrid.AllowSorting &&
                this.dataGrid.SortClickAction == SortClickAction.DoubleClick && !this.dataGrid.GridColumnResizingController.isHovering)
                Sort();

            Point pp = e.GetPosition(this.dataGrid.VisualContainer);
            var cursor = CoreCursorType.Arrow;
            var dragline = this.dataGrid.GridColumnResizingController.HitTest(pp,out cursor);
            if (cursor != CoreCursorType.Arrow)
                this.dataGrid.GridColumnResizingController.SetPointerCursor(cursor);
            if (CanResizeColumn() && Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow && dragline != null)
            {
                var colIndex = this.dataGrid.ResolveToGridVisibleColumnIndex(dragline.LineIndex);
                if (colIndex >= 0 && colIndex < this.dataGrid.Columns.Count)                                      
                    this.dataGrid.GridColumnSizer.SetAutoFitWidth(dataGrid.Columns[colIndex]);
            }
            else
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            base.OnDoubleTapped(e);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (!this.dataGrid.Validations.CheckForValidation(false))
                return;

            if (Column.AllowSorting && this.dataGrid.AllowSorting &&
                this.dataGrid.SortClickAction == SortClickAction.SingleClick && !this.dataGrid.GridColumnResizingController.isHovering)
                Sort();
            base.OnTapped(e);
        }

        protected override void OnPointerMoved(MouseEventArgs e)
        {
            if (this.CanResizeHiddenColumn() && e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                this.dataGrid.GridColumnResizingController.DoActionOnMouseMove(e.GetCurrentPoint(this.dataGrid.VisualContainer), this);
            }
            base.OnPointerMoved(e);
        }

        protected override void OnPointerExited(MouseButtonEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint pp = e.GetCurrentPoint(this.dataGrid.VisualContainer);
                if (pp != null)
                {
                    if (Window.Current != null && Window.Current.CoreWindow != null)
                    {
                        if (!pp.Properties.IsLeftButtonPressed &&
                            Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow)
                        {
                            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                        }
                    }
                }
            }
            base.OnPointerExited(e);
        }



        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            if (!this.dataGrid.Validations.CheckForValidation(true))
                return;
            if (e.HoldingState == HoldingState.Started && e.PointerDeviceType != PointerDeviceType.Mouse &&
                pointer != null)
            {
                if (CanShowPopup() && CanResizeColumn())
                {
                    Point mouseDown = e.GetPosition(this);
                    var point = this.TransformToVisual(null).TransformPoint(new Point(0, 0));
                    var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);
                    this.dataGrid.GridColumnDragDropController.ShowPopup(this.Column, rect, mouseDown.X, mouseDown.Y, true, pointer);
                    e.Handled = true;
                }
            }
            base.OnHolding(e);
        }

#else

#if WPF
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (!this.dataGrid.Validations.CheckForValidation(false))
                return;
            if (Column.AllowSorting && this.dataGrid.AllowSorting && this.dataGrid.SortClickAction == SortClickAction.DoubleClick && !this.dataGrid.GridColumnResizingController.isHovering && this.Cursor == Cursors.Arrow)
                Sort();
            Point pp = e.GetPosition(this.dataGrid.VisualContainer);
            var cursor = this.Cursor;
            var dragline = this.dataGrid.GridColumnResizingController.HitTest(pp, out cursor);
            var indentColumnCount = this.dataGrid.ResolveToScrollColumnIndex(0);
            if (CanResizeColumn() && cursor != Cursors.Arrow && dragline != null && dragline.LineIndex >= indentColumnCount)
            {
                var colIndex = this.dataGrid.ResolveToGridVisibleColumnIndex(dragline.LineIndex);
                this.dataGrid.GridColumnSizer.SetAutoFitWidth(dataGrid.Columns[colIndex]);
                e.Handled = true;
                this.ReleaseMouseCapture();
            } 
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            touchDevice = e.TouchDevice;
            mouseDownPoint = e.GetTouchPoint(this).Position;
            if (!dataGrid.Validations.CheckForValidation(true))
            {
                e.Handled = true;
                return;
            }
            if (this.CanResizeColumn() && this.dataGrid.GridColumnResizingController.isHovering)
            {
                Point pp = e.GetTouchPoint(this.dataGrid.VisualContainer).Position;
                var cursor = this.Cursor;
                this.dataGrid.GridColumnResizingController.dragLine = this.dataGrid.GridColumnResizingController.HitTest(pp, out cursor);
                if (this.dataGrid.GridColumnResizingController.dragLine != null)
                    this.CaptureTouch(e.TouchDevice);
                e.Handled = true;
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }
            isTouchPressed = true;
            base.OnTouchDown(e);
        }
        
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var mouseUpPosition = e.GetTouchPoint(this).Position;
            if (this.dataGrid.GridColumnResizingController.dragLine != null && Math.Abs(mouseUpPosition.X - mouseDownPoint.X) <= 0)
                this.dataGrid.GridColumnResizingController.dragLine = null;
            if (!this.dataGrid.Validations.CheckForValidation(false))
                return;
            if (Column.AllowSorting && this.dataGrid.AllowSorting && this.dataGrid.SortClickAction == SortClickAction.SingleClick && !this.dataGrid.GridColumnResizingController.isHovering && this.Cursor == Cursors.Arrow && isTouchPressed)
                Sort();
            isTouchPressed = false;
            base.OnTouchUp(e);
            e.Handled = true;
        }
#endif

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (this.dataGrid != null)
            {
                Point pp = e.GetPosition(this.dataGrid.VisualContainer);
                isMouseLeftButtonPressed = false;
                if (this.Cursor == Cursors.SizeWE && this.dataGrid.GridColumnResizingController.dragLine == null && !isMouseLeftButtonPressed)
                {
                    this.dataGrid.GridColumnResizingController.SetPointerCursor(Cursors.Arrow, this);
                }
            }
            base.OnMouseLeave(e);
        }

#if WP 
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            if (canDrop)
            {
                if (this.PopupContentDropped != null)
                {
                    Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
                    Point pp = new Point(0,0);
                    pp.X = Microsoft.Xna.Framework.Input.Mouse.GetState().X;
                    pp.Y = Microsoft.Xna.Framework.Input.Mouse.GetState().Y;
                    this.PopupContentDropped(pp);
                }
                canDrop = false;
            }
            base.OnManipulationCompleted(e);
        }
#endif

#if WPF || WP
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
#if WPF
            if (!this.dataGrid.Validations.CheckForValidation(true))
                return;
            if (this.CanResizeColumn())
            {
                this.dataGrid.GridColumnResizingController.DoActionOnMouseMove(touchDevice.GetTouchPoint(this.dataGrid.VisualContainer).Position, this);
            }
            if (!dataGrid.GridColumnResizingController.isHovering && isTouchPressed && this.Cursor == Cursors.Arrow)
            {
                if (CanShowPopup())
                {
                    List<IManipulator> devices = e.Manipulators.ToList();
                    pointerDown = devices[0].GetPosition(this);
                    ShowPopup(touchDevice);
                    isTouchPressed = false;
                    e.Handled = true;
                    return;
                }
            }
            base.OnManipulationDelta(e);
#else
            Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
            Point pp = this.TransformToVisual(null).Transform(new Point(0, 0));
            if (Microsoft.Xna.Framework.Input.Mouse.GetState().X > 0 && Microsoft.Xna.Framework.Input.Mouse.GetState().Y > 0)
            {
                pp.X = Microsoft.Xna.Framework.Input.Mouse.GetState().X;
                pp.Y = Microsoft.Xna.Framework.Input.Mouse.GetState().Y;
            }
            else
            {
                pp.X = 0;
            }
            if (this.dataGrid. GridColumnDragDropController.PopupContent != null && this.dataGrid. GridColumnDragDropController.DraggablePopup.IsOpen)
            {
                if (mouseVerticalPosition > 0 && mouseHorizontalPosition > 0)
                {
                    double deltaH = pp.X;
                    double deltaV = pp.Y;
                    if (this.PopupContentPositionChanged != null)
                        this.PopupContentPositionChanged(deltaH, deltaV, pp, pp);
                    if (!canDrop && !this.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.IsEmpty)
                    {
                        var rect = this.dataGrid. GridColumnDragDropController.PopupContent.GetRect();
                        if ((rect.X >= this.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.X + 5) || (rect.X <= this.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.X - 5) ||
                            (rect.Y >= this.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.Y + 5) || (rect.Y <= this.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.Y - 5))
                        {
                            canDrop = true;
                            if (this.dataGrid. GridColumnDragDropController.PopupContent.LeftResizeThumbVisibility == Visibility.Visible)
                                this.dataGrid. GridColumnDragDropController.PopupContent.LeftResizeThumbVisibility = Visibility.Collapsed;
                            if (this.dataGrid. GridColumnDragDropController.PopupContent.RightResizeThumbVisibility == Visibility.Visible)
                                this.dataGrid. GridColumnDragDropController.PopupContent.RightResizeThumbVisibility = Visibility.Collapsed;
                        }
                    }
                }
                mouseVerticalPosition = pp.Y;
                mouseHorizontalPosition = pp.X;
                base.OnManipulationDelta(e);
            }
#endif
        }
#endif
        internal bool isMouseLeftButtonPressed = false;        
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            pointerDown = e.GetPosition(this);
            var newPosition = Math.Abs(mouseDownPoint.X - pointerDown.X);
            if (this.CanResizeHiddenColumn() && newPosition > 0)
            {
#if WinRT
                this.dataGrid.GridColumnResizingController.DoActionOnMouseMove(e.GetCurrentPoint(this.dataGrid.VisualContainer), this);
#else
                this.dataGrid.GridColumnResizingController.DoActionOnMouseMove(e.GetPosition(this.dataGrid.VisualContainer), this);
#endif
            }
            if (!this.dataGrid.GridColumnResizingController.isHovering && isMouseLeftButtonPressed && this.Cursor == Cursors.Arrow)
            {
                if (CanShowPopup())
                {
                    ShowPopup(null);
                    isMouseLeftButtonPressed = false;
#if WPF
                    e.Handled = true;
#endif
                    return;
                }
            }
            base.OnMouseMove(e);
        }

#if WP
        protected override void OnHold(GestureEventArgs e)
        {
            Point mouseDown = e.GetPosition(this);
            if (CanShowPopup() || CanResizeColumn())
            {
                var point = this.TransformToVisual(null).Transform(new Point(0, 0));
                var rect = new Rect(point.X, point.Y, this.ActualWidth * this.dataGrid.ZoomScale, this.ActualHeight * this.dataGrid.ZoomScale);
                this.dataGrid. GridColumnDragDropController.ShowPopup(this.Column, rect, mouseDown.X, mouseDown.Y, true, null);
                e.Handled = true;
            }
            base.OnHold(e);
        }

#endif  

#if SILVERLIGHT

        protected override void OnHold(GestureEventArgs e)
        {
            if (!this.dataGrid.Validations.CheckForValidation(true))
                return;

            Point mouseDown = e.GetPosition(this);
            //if (e.HoldingState == HoldingState.Started && e.PointerDeviceType != PointerDeviceType.Mouse && pointer != null)
            {
                if (CanShowPopup() && CanResizeColumn())
                {
                    var point = this.TransformToVisual(null).Transform(new Point(0, 0));
                    var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);
                    this.dataGrid.GridColumnDragDropController.ShowPopup(this.Column, rect, mouseDown.X, mouseDown.Y, false, null);
                    e.Handled = true;
                }
            }
            base.OnHold(e);
        }
#endif
#endif

#if WinRT
        protected override void OnPointerPressed(MouseButtonEventArgs e)
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#endif
        {
            ValidationHelper.IsFocusSetBack = false;
            if (!dataGrid.Validations.CheckForValidation(true))
            {
                e.Handled = true;
                return;
            }
#if WPF || WP
            mouseDownPoint = e.GetPosition(this);
#endif            
#if WinRT
            pointerDown = e.GetCurrentPoint(this).Position;
            if (this.CanResizeHiddenColumn() && e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && this.dataGrid.GridColumnResizingController.isHovering && this.dataGrid.GridColumnResizingController.dragLine == null)
#else
            isMouseLeftButtonPressed = true;
            if (this.CanResizeHiddenColumn() && this.dataGrid.GridColumnResizingController.isHovering)
#endif
            {

#if WinRT
                PointerPoint pp = e.GetCurrentPoint(this.dataGrid.VisualContainer);
                double hScrollChange = 0;

                for (int i = 0; i < this.dataGrid.VisualContainer.FrozenColumns; i++)
                {
                    hScrollChange += this.dataGrid.VisualContainer.ColumnWidths[i];
                }
                var pointerPoint = new Point(Math.Abs(pp.Position.X - (dataGrid.VisualContainer.HScrollBar.Value - hScrollChange)), Math.Abs(pp.Position.Y - dataGrid.VisualContainer.VScrollBar.Value));
                var cursor = CoreCursorType.Arrow;
                this.dataGrid.GridColumnResizingController.dragLine = this.dataGrid.GridColumnResizingController.HitTest(pointerPoint,out cursor);
                if (cursor != CoreCursorType.Arrow)
                    this.dataGrid.GridColumnResizingController.SetPointerCursor(cursor);
#else
                Point pp = e.GetPosition(this.dataGrid.VisualContainer);
                var cursor = this.Cursor;
                this.dataGrid.GridColumnResizingController.dragLine = this.dataGrid.GridColumnResizingController.HitTest(pp, out cursor);
#endif
#if !WinRT
                if (this.dataGrid.GridColumnResizingController.dragLine != null)
                    this.CaptureMouse();
                e.Handled=true;
#endif
            }
            else
            {
#if !WinRT
                this.Cursor = Cursors.Arrow;
#else
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
#endif
            }
#if WinRT
            pointer = e.Pointer;
            base.OnPointerPressed(e);
#else
            base.OnMouseLeftButtonDown(e);
#endif
        }

#if WinRT
        protected override void OnPointerReleased(MouseButtonEventArgs e)
#else
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#endif
        {
            pointerDown = new Point(0, 0);            
#if WinRT
            var mouseUpPosition = e.GetCurrentPoint(this).Position;
            if (this.CanResizeHiddenColumn() && e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && this.dataGrid.GridColumnResizingController.dragLine != null)
#else
            var mouseUpPosition = e.GetPosition(this);
            if (this.CanResizeHiddenColumn() && this.dataGrid.GridColumnResizingController.dragLine != null && Math.Abs(mouseUpPosition.X - mouseDownPoint.X) > 0)
#endif
            {
                this.dataGrid.GridColumnResizingController.DoActionOnMouseUp(e, this);
            }

            if (this.dataGrid.GridColumnResizingController.dragLine != null && Math.Abs(mouseUpPosition.X - mouseDownPoint.X) <= 0)
                this.dataGrid.GridColumnResizingController.dragLine = null;
            if (!this.dataGrid.Validations.CheckForValidation(false))
                return;
#if !WinRT
#if !WPF
            bool doubleClick = MouseButtonHelper.IsDoubleClick(this, e);
            if (doubleClick && Column.AllowSorting && this.dataGrid.AllowSorting && this.dataGrid.SortClickAction == SortClickAction.DoubleClick && !this.dataGrid.GridColumnResizingController.isHovering && this.Cursor == Cursors.Arrow)
                Sort();

            Point pp = e.GetPosition(this.dataGrid.VisualContainer);
            var cursor = this.Cursor;
            var dragline = this.dataGrid.GridColumnResizingController.HitTest(pp, out cursor);
            if (CanResizeColumn() && cursor != Cursors.Arrow && doubleClick && dragline != null)
            {
                var colIndex = this.dataGrid.ResolveToGridVisibleColumnIndex(dragline.LineIndex);
                this.dataGrid.GridColumnSizer.SetAutoFitWidth(dataGrid.Columns[colIndex]);
            }
#endif
            if (Column.AllowSorting && this.dataGrid.AllowSorting && this.dataGrid.SortClickAction == SortClickAction.SingleClick && !this.dataGrid.GridColumnResizingController.isHovering && this.Cursor == Cursors.Arrow && isMouseLeftButtonPressed)
                Sort();
            isMouseLeftButtonPressed = false;
#endif
#if WinRT
            base.OnPointerReleased(e);
#else
            base.OnMouseLeftButtonUp(e);
#endif

        }

#if !WPF
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            if (!this.dataGrid.Validations.CheckForValidation(true))
                return;

#if WinRT
            var device = pointer;
#else
            var device = e.OriginalSource;
#endif
            if (!dataGrid.GridColumnResizingController.isHovering && device != null)
            {
                if (CanShowPopup())
                {
#if WinRT
                    var point = this.TransformToVisual(null).TransformPoint(new Point(0, 0));
                    var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);
#else 
                    var point = this.TransformToVisual(null).Transform(new Point(0, 0));
                    var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);
#endif


#if WP
                        this.ReleaseMouseCapture();
                        this.dataGrid.suspendZooming = true;
#endif
                    if (this.dataGrid.GridColumnDragDropController != null)
                        this.dataGrid.GridColumnDragDropController.ShowPopup(this.Column, rect, pointerDown.X, pointerDown.Y, false,
                                                              device);
#if !WP
                    e.Handled = true;
#endif
                    return;
                }
            }
            base.OnManipulationStarted(e);
        }
#endif
        #endregion

        #region InitialUpdation For Sorting
        /// <summary>
        /// Makes Sure headercell property for updation
        /// </summary>
        /// <remarks>
        /// initial sort
        /// itemsource change checking
        /// </remarks>
        public void Update()
        {
            if (this.dataGrid != null && this.dataGrid.View != null)
            {
                #region SortIcon Visibility stuff
                if (this.dataGrid.View.SortDescriptions.Any(x => x.PropertyName == this.Column.MappingName))
                {
                    var sortColumn = this.dataGrid.View.SortDescriptions.FirstOrDefault(x => x.PropertyName == this.Column.MappingName);
                    var sortNumber = this.dataGrid.View.SortDescriptions.IndexOf(sortColumn) + 1;
                    this.SortDirection = sortColumn.Direction;
                    if (this.dataGrid.View.SortDescriptions.Count > 1 && this.dataGrid.ShowSortNumbers)
                    {
                        this.SortNumber = sortNumber.ToString();
                        this.SortNumberVisibility = Visibility.Visible;
                        //this.ColumnOptionsWidth = 25.0d;
                    }
                    else
                    {
                        this.ColumnOptionsWidth = 18.0d;
                        this.SortNumberVisibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    this.SortDirection = null;
                    this.SortNumber = string.Empty;
                    this.SortNumberVisibility = Visibility.Collapsed;
                }
                #endregion
#if !WP
                #region FilterIconVisibility Stuff

                if (this.dataGrid.CanSetAllowFilters(Column))
                    FilterIconVisiblity = Visibility.Visible;
                else
                    FilterIconVisiblity = Visibility.Collapsed;

                #endregion

                if (this.FilterPopupHost != null)
                    this.FilterPopupHost.Column = this.Column;

                ApplyFilterToggleButtonVisualState();
#endif
            }
        }
        #endregion

        #region Sorting Module


#if (WinRT || WP) && !WP7
        private async void Sort()
#else
             private void Sort()
#endif
        {
#if WinRT || WP && !WP7
            this.dataGrid.SetBusyState("Busy");
            await Task.Delay(100);
#elif WPF
             BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                Thread.Sleep(50);
                this.Dispatcher.Invoke((Action)(() => { 
#endif
            this.dataGrid.GridModel.MakeSort(Column);
#if (WinRT || WP) && !WP7
            this.dataGrid.SetBusyState("Normal");
#elif WPF
            }));

            };
            worker.RunWorkerCompleted += (o, args) => { this.dataGrid.SetBusyState("Normal"); };
           this.dataGrid.SetBusyState("Busy");
            worker.RunWorkerAsync();
#endif
        }

        private bool CanShowPopup()
        {
            return this.dataGrid.GridColumnDragDropController.CanShowPopup(this.Column);
        }

        private bool CanResizeColumn()
        {
            return this.dataGrid.GridColumnResizingController.CanResizeColumn(this.Column);
        }

        internal bool CanResizeHiddenColumn()
        {
            var canResizeColumn = false;
            var isResizingandHidden = false;
            int rc;
            if (this.dataGrid.GridColumnResizingController.dragLine != null)
            {
                var columnIndex = this.dataGrid.ResolveToGridVisibleColumnIndex(this.dataGrid.GridColumnResizingController.dragLine.LineIndex);
                if (columnIndex >= 0 && columnIndex < this.dataGrid.Columns.Count)
                {
                    var column = this.dataGrid.Columns[columnIndex];
                    canResizeColumn = this.dataGrid.GridColumnResizingController.CanResizeColumn(column);
                }
            }
            else
                canResizeColumn = this.CanResizeColumn();
            var lineIndex = this.dataGrid.ResolveToScrollColumnIndex(this.dataGrid.Columns.IndexOf(Column));
            if (lineIndex > 0 && lineIndex < this.dataGrid.Columns.Count)
            {
                if (dataGrid.VisualContainer.ColumnWidths.GetHidden(lineIndex - 1, out rc))
                    isResizingandHidden = true;
            }           
         return canResizeColumn || isResizingandHidden;
        }
        #endregion

        #region Filtering Module
#if !WP
        #region Wire & UnWire
        /// <summary>
        /// Wires the excel like filtering events.
        /// </summary>
        void WireExcelLikeFilteringEvents()
        {
            if (FilterToggleButton != null)
            {

#if WPF
                FilterToggleButton.MouseEnter += OnFilterToggleButtonMouseEnter;
                FilterToggleButton.MouseLeave += OnFilterToggleButtonMouseLeave;
                FilterToggleButton.PreviewMouseDown += OnFilterToggleButtonPreviewMouseDown;
#endif
                FilterToggleButton.Click += OnFilterToggleButtonClick;

#if WinRT
                FilterToggleButton.Tapped += OnFilterToggleButtonTapped;
#endif
            }

            if (this.FilterPopupHost != null)
            {
                FilterPopupHost.Column = this.Column;
            }
        }

        private void OnFilterToggleButtonPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ValidationHelper.IsFocusSetBack = false;
            if (!dataGrid.Validations.CheckForValidation(true))
            {
                ValidationHelper.IsFocusSetBack = true;
                OpenFilterPopUp();
                e.Handled = true;
                return;
            }
        }
#if WPF
        void OnFilterToggleButtonMouseLeave(object sender, MouseEventArgs e)
        {
            if(this.FilterPopupHost != null && this.FilterPopupHost.FilterPopUp != null)
                this.FilterPopupHost.FilterPopUp.StaysOpen = !this.FilterPopupHost.FilterPopUp.StaysOpen; ;
        }

        void OnFilterToggleButtonMouseEnter(object sender, MouseEventArgs e)
        {
            if (this.FilterPopupHost != null && this.FilterPopupHost.FilterPopUp != null)
                this.FilterPopupHost.FilterPopUp.StaysOpen = !this.FilterPopupHost.FilterPopUp.StaysOpen;
        }

#endif
        /// <summary>
        /// UnWire the excel like filtering events.
        /// </summary>
        void UnWireExcelLikeFilteringEvents(bool candispose)
        {
            if (FilterToggleButton != null)
            {
#if WPF
                FilterToggleButton.MouseEnter -= OnFilterToggleButtonMouseEnter;
                FilterToggleButton.MouseLeave -= OnFilterToggleButtonMouseLeave;
                FilterToggleButton.PreviewMouseDown -= OnFilterToggleButtonPreviewMouseDown;
#endif
                FilterToggleButton.Click -= OnFilterToggleButtonClick;
#if WinRT
                FilterToggleButton.Tapped -= OnFilterToggleButtonTapped;
#endif
                FilterToggleButton = null;
            }

            if (this.FilterPopupHost != null && candispose)
            {
                (FilterPopupHost as IDisposable).Dispose();
                FilterPopupHost = null;
            }
        }

        #endregion

        #region FilterPredicates Collection changed

        void OnFilterPredicatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            this.ApplyFilterToggleButtonVisualState();
        }

        #endregion

        #region Private Methods

        internal void ApplyFilterToggleButtonVisualState()
        {
            if (this.FilterToggleButton != null && this.Column != null)
                this.FilterToggleButton.toggleButtonVisualState = VisualStateManager.GoToState(this.FilterToggleButton, IsFilterApplied ? "Filtered" : "UnFiltered", true) ? string.Empty : (IsFilterApplied ? "Filtered" : "UnFiltered");
        }
        private void OpenFilterPopUp()
        {
            this.dataGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.FilterPopupOpening, null));
            if (FilterPopupHost == null)
            {
                FilterPopupHost = new GridFilterControl()
                {
                    FilterPopupHeight = 450,
                    FilterPopupWidth = 300,
                    Column = this.Column,
                    AdvancedFilterType = this.GetAdvancedFilterType()
                };
                FilterPopUpPresenter.Child = FilterPopupHost;
                Binding binding = new Binding();
                binding.Source = FilterToggleButton;
                binding.Path = new PropertyPath("IsChecked");
                binding.Mode = BindingMode.TwoWay;
                FilterPopupHost.SetBinding(GridFilterControl.IsOpenProperty, binding);
            }
        }
        #endregion

        #region FilterToggleButtonClick

        void OnFilterToggleButtonClick(object sender, RoutedEventArgs e)
        {
#if !WPF
            isFilterToggleButtonClicked = true;
            ValidationHelper.IsFocusSetBack = false;
            if (!this.dataGrid.Validations.CheckForValidation(true))
            {
                ValidationHelper.IsFocusSetBack = true;
                return;
            }
#endif
            OpenFilterPopUp();

#if WinRT
            var toggleBtnPoint = this.FilterToggleButton.TransformToVisual(this).TransformPoint(new Point(0, 0));
            var windowPoint = this.FilterToggleButton.TransformToVisual(null).TransformPoint(new Point(0, 0));
            Rect toggleRect = new Rect(toggleBtnPoint.X, toggleBtnPoint.Y, FilterToggleButton.ActualWidth, this.ActualHeight);
            if (FilterPopupHost != null)
                FilterPopupHost.SetPopupPosition(toggleRect, windowPoint, FilterToggleButton.Padding.Left);
#elif SILVERLIGHT
            var toggleBtnPoint = this.FilterToggleButton.TransformToVisual(this).Transform(new Point(0, 0));
            var windowPoint = this.FilterToggleButton.TransformToVisual(Application.Current.RootVisual).Transform(new Point(0, 0));
            Rect toggleRect = new Rect(toggleBtnPoint.X, toggleBtnPoint.Y, FilterToggleButton.ActualWidth, this.ActualHeight);
            if (FilterPopupHost != null)
                this.FilterPopupHost.SetPopupPosition(toggleRect, windowPoint, FilterToggleButton.Padding.Left);
#endif
        }

#if WinRT

        private void OnFilterToggleButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

#endif
        #endregion

#endif
        #endregion

        #region Private Methods
#if !WinRT
        private void ShowPopup(TouchDevice touchDevice)
        {
#if WPF
            Point locationFromWindow = this.TranslatePoint(new Point(0, 0), this);
            Point locationFromScreen = this.PointToScreen(locationFromWindow);
            var rect = new Rect(locationFromScreen.X - locationFromWindow.X, locationFromScreen.Y - locationFromWindow.Y, this.ActualWidth, this.ActualHeight);
#elif WP
            Point point = this.TransformToVisual(null).Transform(new Point(0, 0));
            var rect = new Rect(point.X, point.Y, this.ActualWidth * this.dataGrid.ZoomScale, this.ActualHeight * this.dataGrid.ZoomScale);
#else
            Point point = this.TransformToVisual(null).Transform(new Point(0, 0));
            var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);
#endif
            //var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);

#if WPF 
            if (Math.Abs(pointerDown.Y - mouseDownPoint.Y) >= 1 || Math.Abs(pointerDown.X - mouseDownPoint.X) >= 1)
#elif WP
            if (Math.Abs(pointerDown.Y - mouseDownPoint.Y) >= 2 && Math.Abs(pointerDown.X - mouseDownPoint.X) <= 2)
#endif
            {
                if (this.dataGrid.GridColumnDragDropController != null)
                {
#if WP

                this.dataGrid. GridColumnDragDropController.ShowPopup(this.Column, rect, pointerDown.X*this.dataGrid.ZoomScale,
                                                              pointerDown.Y*this.dataGrid.ZoomScale, false, touchDevice);
#else
                    this.dataGrid.GridColumnDragDropController.ShowPopup(this.Column, rect, pointerDown.X,
                                                                  pointerDown.Y, false, touchDevice);
#endif
                }
            }
        }
#endif
#if !WP
        private AdvancedFilterType GetAdvancedFilterType()
        {
#if SILVERLIGHT
            if (this.Column.IsUnbound)
                return AdvancedFilterType.TextFilter;
#endif
#if !WP
            if (this.Column.DataGrid.View.IsDynamicBound || this.Column.IsUnbound)
                return AdvancedFilterType.TextFilter;
#endif
            if (this.Column.FilterBehavior == FilterBehavior.StronglyTyped)
            {
                var pdc = this.dataGrid.View.GetItemProperties();
                var pd = pdc.GetPropertyDescriptor(this.Column.MappingName);
                if (pd == null)
                    return AdvancedFilterType.TextFilter;
                var columnType = pd.PropertyType;

                if (columnType == typeof(int) || columnType == typeof(Double) || columnType == typeof(Decimal) || columnType == typeof(int?) || columnType == typeof(double?) || columnType == typeof(decimal?) || columnType == typeof(long) || columnType == typeof(long?) || columnType == typeof(uint) || columnType == typeof(uint?) || columnType == typeof(byte) || columnType == typeof(byte?) || columnType == typeof(float)
                    || columnType == typeof(float?) || columnType == typeof(sbyte) || columnType == typeof(sbyte?) || columnType == typeof(ulong) || columnType == typeof(ulong?) || columnType == typeof(short) || columnType == typeof(short?) || columnType == typeof(ushort) || columnType == typeof(ushort?))
                    return AdvancedFilterType.NumberFilter;
                else if (columnType == typeof(DateTime) || columnType == typeof(DateTime?) || columnType == typeof(TimeSpan) || columnType == typeof(TimeSpan?))
                    return AdvancedFilterType.DateFilter;
                else
                    return AdvancedFilterType.TextFilter;
            }
            else
                return AdvancedFilterType.TextFilter;
        }
#endif

        private void WireGridHeaderCellControlEvents()
        {
            this.Loaded += GridHeaderCellControlLoaded;
            this.Unloaded += GridHeaderCellControlUnloaded;
        }

        private void UnWireGridHeaderCellControlEvents()
        {
            this.Loaded -= GridHeaderCellControlLoaded;
            this.Unloaded -= GridHeaderCellControlUnloaded;
        }

        #endregion

#if WPF
        #region ContextMenu Event

        void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.ContextMenu == null)
                return;

            var dataContext = this.ContextMenu.DataContext as GridColumnContextMenuInfo;
            if (dataContext != null)
                dataContext.Column = this.Column;
            else
                dataContext = new GridColumnContextMenuInfo() { Column = this.Column, DataGrid = this.dataGrid };

            var rowColIndex = new RowColumnIndex(dataGrid.GetHeaderIndex(), dataGrid.ResolveToScrollColumnIndex(dataGrid.Columns.IndexOf(Column)));
            var args = new GridContextMenuEventArgs(dataGrid.HeaderContextMenu, dataContext, rowColIndex, ContextMenuType.Header);
            this.ContextMenu.DataContext = dataContext;
            dataGrid.RaiseGridContextMenuEvent(args);
            e.Handled = args.Handled;
        }
        #endregion
#endif

        #region IDisposable Member

        public void Dispose()
        {
#if !WP
            this.UnWireExcelLikeFilteringEvents(true);
#endif
            UnWireGridHeaderCellControlEvents();
            this.dataGrid = null;
#if WinRT
            this.pointer = null;
#endif
        }
        #endregion
    }
#if !WP
    public class FilterToggleButton : ToggleButton
    {
        internal string toggleButtonVisualState;
        public FilterToggleButton()
        {
            this.DefaultStyleKey = typeof(FilterToggleButton);
        }

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {

            base.OnApplyTemplate();
            if (!string.IsNullOrEmpty(toggleButtonVisualState))
                VisualStateManager.GoToState(this, toggleButtonVisualState, true);
        }
    }
#endif

    #region StackedHeaderCellControl
    [ClassReference(IsReviewed = false)]
    public sealed class GridStackedHeaderCellControl : ContentControl, IDisposable
    {
        public GridStackedHeaderCellControl()
        {
            this.DefaultStyleKey = typeof(GridStackedHeaderCellControl);
        }
        public void Dispose()
        {

        }
    }
    #endregion

#if SILVERLIGHT|| WP
    internal static class MouseButtonHelper
    {
        private const long k_DoubleClickSpeed = 500;
        private const double k_MaxMoveDistance = 10;

        private static long _LastClickTicks = 0;
        private static Point _LastPosition;
        private static WeakReference _LastSender;

        internal static bool IsDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(null);
            long clickTicks = DateTime.Now.Ticks;
            long elapsedTicks = clickTicks - _LastClickTicks;
            long elapsedTime = elapsedTicks / TimeSpan.TicksPerMillisecond;
            bool quickClick = (elapsedTime <= k_DoubleClickSpeed);
            bool senderMatch = (_LastSender != null && sender.Equals(_LastSender.Target));

            if (senderMatch && quickClick && position.Distance(_LastPosition) <= k_MaxMoveDistance)
            {
                // Double click!
                _LastClickTicks = 0;
                _LastSender = null;
                return true;
            }

            // Not a double click
            _LastClickTicks = clickTicks;
            _LastPosition = position;
            if (!quickClick)
                _LastSender = new WeakReference(sender);
            return false;
        }

        private static double Distance(this Point pointA, Point pointB)
        {
            double x = pointA.X - pointB.X;
            double y = pointA.Y - pointB.Y;
            return Math.Sqrt(x * x + y * y);
        }
    }
#endif

    public class SortDirectionToWidthConverter : IValueConverter
    {
#if WinRT
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
            if (value == null)
                return 0;
            return 25;
        }

#if WinRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
            throw new NotImplementedException();
        }

    }

    public class SortDirectionToVisibilityConverter : IValueConverter
    {
#if WinRT
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
            if (value == null)
                return Visibility.Collapsed;

            var direction = (ListSortDirection)value;
            if (parameter.ToString().Equals("Ascending"))
            {
                if (direction == ListSortDirection.Ascending)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
            else
            {
                if (direction == ListSortDirection.Descending)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

#if WinRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
            throw new NotImplementedException();
        }
    }
}