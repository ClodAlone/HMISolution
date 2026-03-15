#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Syncfusion.Windows.Shared
{

    /// <summary>
    /// Represents the TileView Item Base. This Class is for the maintaining the drag events of the report cards
    /// </summary>
    public class TileViewItemBase : TileViewItemAnimationBase
    {
        #region   region declaration

        /// <summary>
        /// stores the details of the area of the float panels
        /// </summary>
        private const string FloatPanelArea = "FloatPanelArea";

        /// <summary>
        /// stores the details of the current Z-index 
        /// </summary>
        private static int currZIndex = 1;

        /// <summary>
        /// stores the mouse point details when the drag is completed
        /// </summary>
        private Point dragCompletedPoint;

        /// <summary>
        /// returns the value of dragging in bool
        /// </summary>
        private bool dragging = false;

        /// <summary>
        /// stores the details of the object UI element
        /// </summary>
        public UIElement UIElementObj;

        /// <summary>
        /// declares the drag started event of the report card
        /// </summary>
        public event TileViewDragEventHandler DragStartedEvent;

        /// <summary>
        /// declares the drag mouse move event of the report card
        /// </summary>
        public event TileViewDragEventHandler DragMouseMoveEvent;

        /// <summary>
        /// declares the drag completed event of the report card
        /// </summary>
        public event TileViewDragEventHandler DragCompletedEvent;

        /// <summary>
        /// declares the on focus event of the report card
        /// </summary>
        public event TileViewDragEventHandler PanelFocused;

        #endregion

        #region Get Set Region

        /// <summary>
        /// Gets or Sets report card movable
        /// </summary>
        internal bool IsMovable
        {
            get { return (bool)GetValue(IsMovableProperty); }
            set { SetValue(IsMovableProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the current Z index
        /// </summary>
        internal static int CurrentZIndex
        {
            get { return TileViewItemBase.currZIndex; }
            set { TileViewItemBase.currZIndex = value; }
        }

        /// <summary>
        /// Gets or Sets the mouse point when drag is completed
        /// </summary>
        internal Point DragCompletedPoint
        {
            get { return dragCompletedPoint; }
            set { dragCompletedPoint = value; }
        }

        #endregion

        #region Constructor Region 
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        internal TileViewItemBase()
        {
#if WPF
            try
            {
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                        System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                }
            }
            catch (InvalidOperationException)
            {
                if (Application.Current != null && Application.Current.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                        {
                            if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                                System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                        }));
                }
            }
#endif
        } 
        #endregion

        #region Dependency Property Region

        /// <summary>
        /// Identifies the IsMovable Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsMovableProperty =
            DependencyProperty.Register("IsMovable", typeof(bool), typeof(TileViewItemBase), new PropertyMetadata(true));

        #endregion

        #region Methods Region

        /// <summary>
        /// Gets the template parts from the report card.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            bool Draggble = IsMovable;
            if (Draggble)
            {
                IsMovable = true;
            }
            else
            {
                IsMovable = false;
            }

            bool Dragable = true;
            if (this.ParentTileViewControl != null)
            {
                Dragable = (this.ParentTileViewControl as TileViewControl).AllowItemRepositioning;
            }

            if (Dragable)
            {
                FrameworkElement FloatPanelArea = GetTemplateChild(TileViewItemBase.FloatPanelArea) as FrameworkElement;
                if (FloatPanelArea != null)
                {
                    FloatPanelArea.MouseLeftButtonDown += new MouseButtonEventHandler(FloatPanelArea_MouseLeftButtonDown);
                    FloatPanelArea.MouseMove += new MouseEventHandler(FloatPanelArea_MouseMove);
                    FloatPanelArea.MouseLeftButtonUp += new MouseButtonEventHandler(FloatPanelArea_MouseLeftButtonUp);
                }
            }
        }

        /// <summary>
        /// Base method for updating report card coordinates
        /// </summary>
        /// <param name="Pt">The new mouse coordinates</param>
        public virtual void UpdateCoordinate(Point Pt)
        {
            Canvas.SetLeft(UIElementObj, Math.Max(0, Pt.X));
            Canvas.SetTop(UIElementObj, Math.Max(0, Pt.Y));
        }

        /// <summary>
        /// Base method for updating the report card size
        /// </summary>
        /// <param name="Width">report card width</param>
        /// <param name="Height">report card height</param>
        public virtual void UpdateFloatPanelSize(double Width, double Height)
        {
            Width = Math.Max(MinWidth, Width);
            Height = Math.Max(MinHeight, Height);
        }

        #endregion

        #region Events Region

        /// <summary>
        /// starthe dragging of report card when mouse left button is clicked
        /// </summary>
        /// <param name="sender">The float panel area</param>
        /// <param name="e">Mouse button event args.</param>
        public void FloatPanelArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool Dragable = true;

            if (this.ParentTileViewControl != null)
            {
                Dragable = ParentTileViewControl.AllowItemRepositioning;
            }

            if (Dragable && IsMovable)
            {
                ((FrameworkElement)sender).CaptureMouse();
                dragCompletedPoint = e.GetPosition(sender as UIElement);
                dragging = true;
                if (DragStartedEvent != null)
                {
                    DragStartedEvent(this, new TileViewDragEventArgs(0, 0, e, string.Empty));
                }
            }
        }

        /// <summary>
        /// stops the report catd dragging in the float panel area
        /// </summary>
        /// <param name="sender">float panel area</param>
        /// <param name="e">Mouse button event args.</param>
        public void FloatPanelArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((FrameworkElement)sender).ReleaseMouseCapture();
            dragging = false;
            Point position = e.GetPosition(sender as UIElement);
            TileViewDragEventArgs Dargs = new TileViewDragEventArgs(position.X - dragCompletedPoint.X, position.Y - dragCompletedPoint.Y, e, string.Empty);

            if (DragCompletedEvent != null)
            {
                DragCompletedEvent(this, Dargs);
            }

        }

        /// <summary>
        /// fires when report card is dragging
        /// </summary>
        /// <param name="sender">float panel area</param>
        /// <param name="e">Mouse button event args.</param>
        public void FloatPanelArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging && IsMovable)
            {
                Point position = e.GetPosition(sender as UIElement);
                double x = Convert.ToDouble(Canvas.GetLeft(this) + position.X - dragCompletedPoint.X);
                double y = (Canvas.GetTop(this) + position.Y - dragCompletedPoint.Y);
                if (Math.Abs(position.X - dragCompletedPoint.X) > 0d || Math.Abs(position.Y - dragCompletedPoint.Y) > 0d)
                {
                    Point CoordinatePt = new Point(x, y);
                    UpdateCoordinate(CoordinatePt);
                    if (DragMouseMoveEvent != null)
                    {
                        DragMouseMoveEvent(this, new TileViewDragEventArgs(position.X - dragCompletedPoint.X, position.Y - dragCompletedPoint.Y, e, string.Empty));
                    }
                }
            }
        }

        /// <summary>
        /// it brings the report card to the front
        /// </summary>
        /// <param name="e">Routed Event Args.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            Canvas.SetZIndex(this, CurrentZIndex++);
            if (PanelFocused != null)
            {
                PanelFocused(this, null);
            }

        }

        /// <summary>
        /// event to bring the report card in front of the other report cards
        /// </summary>
        /// <param name="e">Mouse Button Event Args.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (dragging == true)
            {
                base.OnMouseLeftButtonDown(e);
                Canvas.SetZIndex(this, CurrentZIndex++);
                if (PanelFocused != null)
                {
                    PanelFocused(this, null);
                }
            }
            else
            {
                e.Handled = true;
            }

        }

        #endregion

    }
}
