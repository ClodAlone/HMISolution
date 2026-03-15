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
using Syncfusion.Data;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif
#if WinRT
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls.Primitives;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Windows.Data;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class GroupDropAreaItem : ContentControl, IDisposable
    {
        #region Fields
        
        GroupDropArea groupDropArea;
        bool isCloseButtonClicked = false;
        
        internal GroupDropAreaItemTapped GroupDropAreaItemTapped;
        internal GroupDropAreaItemRemoved GroupDropAreaItemRemoved;
#if WP
        Point mouseDownPosition;
#if !WP7
        bool canDrop;
#endif
#endif

#if !WinRT
        bool isMouseButtonPressed = false;
#else
        private Point pointerDown;
#endif
        
        #endregion

        #region Ctor

        public GroupDropAreaItem() 
        {
            this.DefaultStyleKey = typeof(GroupDropAreaItem);
#if WinRT
            this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
#endif
        }

        #endregion

        #region Property

        public GroupDropArea GroupDropArea
        {
            get
            {
                return groupDropArea;
            }
            internal set
            {
                groupDropArea = value;
            }
        }

        #endregion

        #region Dependency Property

        public GridColumn GridColumn
        {
            get { return (GridColumn)GetValue(GridColumnProperty); }
            set { SetValue(GridColumnProperty, value); }
        }

        public static readonly DependencyProperty GridColumnProperty =
            DependencyProperty.Register("GridColumn", typeof(GridColumn), typeof(GroupDropAreaItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets Path direction (Ascending/Descending).
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public object SortDirection
        {
            get { return (ListSortDirection)this.GetValue(SortDirectionProperty); }
            set { this.SetValue(SortDirectionProperty, value); }
        }

        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof(object), typeof(GroupDropAreaItem), new PropertyMetadata(null));

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(GroupDropAreaItem), new PropertyMetadata(null));

        #endregion

        #region override methods

#if WinRT
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (!this.GroupDropArea.dataGrid.Validations.CheckForValidation(true))
                return;
            if (!isCloseButtonClicked)
            {
                if (this.GroupDropAreaItemTapped != null)
                    this.GroupDropAreaItemTapped(this.GridColumn, 1);
            }
            base.OnTapped(e);
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            if (!this.GroupDropArea.dataGrid.Validations.CheckForValidation(true))
                return;
            if (!isCloseButtonClicked)
            {
                if (!this.GroupDropArea.dataGrid.Validations.CheckForValidation(false))
                    return;
                if (this.GroupDropAreaItemTapped != null)
                    this.GroupDropAreaItemTapped(this.GridColumn, 2);
            }
            base.OnDoubleTapped(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (!this.GroupDropArea.dataGrid.Validations.CheckForValidation(true))
                return;
            pointerDown = e.GetCurrentPoint(this).Position;
            pointer = e.Pointer;
            base.OnPointerPressed(e);
        }

        Pointer pointer;
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            if (!this.GroupDropArea.dataGrid.Validations.CheckForValidation(true))
                return;
            var point = this.TransformToVisual(null).TransformPoint(new Point(0, 0));
            var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);
            this.GroupDropArea.dataGrid.GridColumnDragDropController.ShowPopup(this.GridColumn, rect, false, pointerDown.X, pointerDown.Y, true, false, pointer);
            e.Handled = true;
            base.OnManipulationStarted(e);
        }

        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            if (!this.GroupDropArea.dataGrid.Validations.CheckForValidation(true))
                return;
            if (e.HoldingState == HoldingState.Started && e.PointerDeviceType != PointerDeviceType.Mouse)
            {
                var point = this.TransformToVisual(null).TransformPoint(new Point(0, 0));
                var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);
                this.GroupDropArea.dataGrid.GridColumnDragDropController.ShowPopup(this.GridColumn, rect, true, pointerDown.X, pointerDown.Y, true, false, pointer);
                e.Handled = true;
            }
            base.OnHolding(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                VisualStateManager.GoToState(this, "MouseEntered", true);
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                VisualStateManager.GoToState(this, "MouseExited", true);
            base.OnPointerExited(e);
        }
#else 
#if WP && !WP7
        double mouseVerticalPosition = 0, mouseHorizontalPosition = 0;
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
            Point pp = this.TransformToVisual(null).Transform(new Point(0, 0));
            if (Microsoft.Xna.Framework.Input.Mouse.GetState().X > 0 && Microsoft.Xna.Framework.Input.Mouse.GetState().Y > 0)
            {
                pp.X = Microsoft.Xna.Framework.Input.Mouse.GetState().X;
                pp.Y = Microsoft.Xna.Framework.Input.Mouse.GetState().Y;
            }
            if (this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent != null && this.GroupDropArea.dataGrid. GridColumnDragDropController.DraggablePopup.IsOpen)
            {
                if (mouseVerticalPosition > 0 && mouseHorizontalPosition > 0)
                {
                    double deltaH = pp.X;
                    double deltaV = pp.Y;
                    if (this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.PopupContentPositionChanged != null)
                        this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.PopupContentPositionChanged(deltaH, deltaV, pp, pp);
                    if (!canDrop && !this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.IsEmpty)
                    {
                        var rect = this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.GetRect();
                        if ((rect.X >= this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.X + 5) || (rect.X <= this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.X - 5) ||
                            (rect.Y >= this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.Y + 5) || (rect.Y <= this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.InitialRect.Y - 5))
                        {
                            canDrop = true;
                            if (this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.LeftResizeThumbVisibility == Visibility.Visible)
                                this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.LeftResizeThumbVisibility = Visibility.Collapsed;
                            if (this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.RightResizeThumbVisibility == Visibility.Visible)
                                this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.RightResizeThumbVisibility = Visibility.Collapsed;
                        }
                    }
                }
                mouseVerticalPosition = pp.Y;
                mouseHorizontalPosition = pp.X;
                base.OnManipulationDelta(e);
            }
        }
        
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            if (canDrop)
            {
                if (this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.PopupContentDropped != null)
                {
                    Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
                    Point pp = new Point(0, 0);
                    pp.X = Microsoft.Xna.Framework.Input.Mouse.GetState().X;
                    pp.Y = Microsoft.Xna.Framework.Input.Mouse.GetState().Y;
                    this.GroupDropArea.dataGrid. GridColumnDragDropController.PopupContent.PopupContentDropped(pp);
                }
                canDrop = false;
            }
            base.OnManipulationCompleted(e);
        }
#endif
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!isCloseButtonClicked && isMouseButtonPressed)
            {
                Point mouseDown = e.GetPosition(this);
                if (this.groupDropArea==null || this.GroupDropArea.dataGrid.GridColumnDragDropController != null)
                {
#if WPF
                    var rect = this.GroupDropArea.dataGrid.GridColumnDragDropController.GetControlRect(this);
#else
                var point = this.TransformToVisual(null).Transform(new Point(0, 0));
                var rect = new Rect(point.X, point.Y, this.ActualWidth, this.ActualHeight);
#endif
#if WP
                if (this.GroupDropArea != null && Math.Abs(mouseDownPosition.Y - mouseDown.Y) >= 2)
                {
                    this.GroupDropArea.scrollersDisabled = true;
                    this.GroupDropArea.dataGrid. GridColumnDragDropController.ShowPopup(this.GridColumn, rect, false, mouseDown.X, mouseDown.Y, true, false, null);
                }

#else
                    this.GroupDropArea.dataGrid.GridColumnDragDropController.ShowPopup(this.GridColumn, rect, false, mouseDown.X, mouseDown.Y, true, false, null);
#endif
                }
                isMouseButtonPressed = false;
#if WPF
                e.Handled = true;
#endif
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.groupDropArea==null || !this.GroupDropArea.dataGrid.Validations.CheckForValidation(true))
                return;
            isMouseButtonPressed = false;
            if (!isCloseButtonClicked)
            {
                if (this.GroupDropAreaItemTapped != null)
                {
#if WPF
                    this.GroupDropAreaItemTapped(this.GridColumn, e.ClickCount);
#else
                    bool doubleClick = MouseButtonHelper.IsDoubleClick(this, e);
                    this.GroupDropAreaItemTapped(this.GridColumn, doubleClick ? 2 : 1);
#endif
                }
            }
            base.OnMouseLeftButtonUp(e);
        }

#if WPF
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (this.groupDropArea==null || !this.GroupDropArea.dataGrid.Validations.CheckForValidation(false))
                return;
            if (this.GroupDropAreaItemTapped != null)
            {
                this.GroupDropAreaItemTapped(this.GridColumn, 2);
            }
            base.OnMouseDoubleClick(e);
        }
#endif

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.groupDropArea==null || !this.GroupDropArea.dataGrid.Validations.CheckForValidation(true))
                return;
#if WP
			mouseDownPosition = e.GetPosition(this);
#endif
            isMouseButtonPressed = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseEntered", true);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseExited", true);
            base.OnMouseLeave(e);
        }
#endif

        private Button PART_CloseButton;
#if WinRT 
        protected override void OnApplyTemplate() 
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            if (PART_CloseButton != null)
            {
                PART_CloseButton.Click -= OnCloseClick;
            }
            PART_CloseButton = this.GetTemplateChild("PART_CloseButton") as Button;
            if (PART_CloseButton != null)
            {
                PART_CloseButton.Click += OnCloseClick;
            }
#if WPF
            if (this.GroupDropArea.dataGrid != null)
            {
                this.ContextMenuOpening += OnContextMenuOpening;
                var bind = new Binding
                {
                    Path = new PropertyPath("GroupDropItemContextMenu"),
                    Source = this.GroupDropArea.dataGrid,
                    Mode = BindingMode.TwoWay,
                };
                this.SetBinding(GroupDropArea.ContextMenuProperty, bind);
            }
#endif
            
        }


        #endregion

        #region private Methods

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            if (!this.GroupDropArea.dataGrid.Validations.CheckForValidation(true))
                return;
            if (this.GroupDropAreaItemRemoved != null)
                this.GroupDropAreaItemRemoved(this.GridColumn);
            isCloseButtonClicked = true;
        }

        #endregion

#if WPF
        #region ContextMenu Event

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.ContextMenu == null)
                return;

            var dataGrid = this.GroupDropArea.dataGrid;
            var dataContext = new GridColumnContextMenuInfo() { Column = this.GridColumn, DataGrid = dataGrid};
            var args = new GridContextMenuEventArgs(dataGrid.GroupDropItemContextMenu, dataContext, RowColumnIndex.Empty, ContextMenuType.GroupDropAreaItem);
            this.ContextMenu.DataContext = dataContext;
            dataGrid.RaiseGridContextMenuEvent(args);
            e.Handled = args.Handled;
        }

        #endregion
#endif

        public void Dispose()
        {
            if (PART_CloseButton != null)
            {
                PART_CloseButton.Click -= OnCloseClick;
                this.PART_CloseButton = null;
            }
#if WPF
            if (this.GroupDropArea != null && this.GroupDropArea.dataGrid != null && this.GroupDropArea.dataGrid.GroupDropItemContextMenu != null)
            {
                this.ContextMenuOpening -= OnContextMenuOpening;
                this.ContextMenu = null;
            }
#endif
            this.GridColumn = null;
            this.groupDropArea = null;
            this.GroupDropAreaItemRemoved = null;
            this.GroupDropAreaItemTapped = null;
#if WinRT
            this.pointer = null;
#endif

        }
    }

    internal delegate void GroupDropAreaItemTapped(GridColumn GridColumn, int ClickCount);

    internal delegate void GroupDropAreaItemRemoved(GridColumn GridColumn);
}
