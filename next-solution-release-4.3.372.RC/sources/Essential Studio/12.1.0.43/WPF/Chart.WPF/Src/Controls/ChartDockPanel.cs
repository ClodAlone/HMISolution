// <copyright file="ChartDockPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents main chart docking panel.Chart docking panel provides docking abilities for chart elements.
    /// </summary>
    /// <seealso cref="ChartDockPanel"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartDockPanel : Panel
    {
        #region DependencyProperties
        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty DockProperty =
          DependencyProperty.RegisterAttached("Dock", typeof(ChartDock), typeof(ChartDockPanel), new PropertyMetadata(new PropertyChangedCallback(OnDockChanged)));

        /// <summary>
        /// Identifies the Alignment dependency property.
        /// </summary>
        public static readonly DependencyProperty AlignmentProperty =
          DependencyProperty.RegisterAttached("Alignment", typeof(ChartAlignment), typeof(ChartDockPanel), new PropertyMetadata(ChartAlignment.Center, new PropertyChangedCallback(OnAlignmentChanged)));

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
          DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartDockPanel));

        /// <summary>
        /// Identifies the FloatingOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty FloatingOffsetProperty =
          DependencyProperty.Register("FloatingOffset", typeof(Vector), typeof(ChartDockPanel), new PropertyMetadata(new Vector(10, 10)));

        /// <summary>
        /// Identifies the ElementMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementMarginProperty =
          DependencyProperty.Register("ElementMargin", typeof(Thickness), typeof(ChartDockPanel), new FrameworkPropertyMetadata(new Thickness(4), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the RootElement dependency property.
        /// </summary>
        public static readonly DependencyProperty RootElementProperty =
          DependencyProperty.Register("RootElement", typeof(UIElement), typeof(ChartDockPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnRootElementChanged)));

        /// <summary>
        /// Identifies the FloatAlignmentX dependency property.
        /// </summary>
        public static readonly DependencyProperty FloatAlignmentXProperty =
          DependencyProperty.Register("FloatAlignmentX", typeof(AlignmentX), typeof(ChartDockPanel), new PropertyMetadata(AlignmentX.Right, new PropertyChangedCallback(OnFloatAlignmentChanged)));

        /// <summary>
        /// Identifies the FloatAlignmentY dependency property.
        /// </summary>
        public static readonly DependencyProperty FloatAlignmentYProperty =
          DependencyProperty.Register("FloatAlignmentY", typeof(AlignmentY), typeof(ChartDockPanel), new PropertyMetadata(AlignmentY.Top, new PropertyChangedCallback(OnFloatAlignmentChanged)));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the panel's orientation. This is a dependency property.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(ChartDockPanel.OrientationProperty);
            }

            set
            {
                SetValue(ChartDockPanel.OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the floating offset. This is a dependency property.
        /// </summary>
        /// <value>The floating offset.</value>
        public Vector FloatingOffset
        {
            get
            {
                return (Vector)GetValue(ChartDockPanel.FloatingOffsetProperty);
            }

            set
            {
                SetValue(ChartDockPanel.FloatingOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the element margin. This is a dependency property.
        /// </summary>
        /// <value>The element margin.</value>
        public Thickness ElementMargin
        {
            get
            {
                return (Thickness)GetValue(ChartDockPanel.ElementMarginProperty);
            }

            set
            {
                SetValue(ChartDockPanel.ElementMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the root element. This is a dependency property.
        /// </summary>
        /// <value>The root element.</value>
        public UIElement RootElement
        {
            get
            {
                return m_rootElement;
            }

            set
            {
                if (m_rootElement == null && m_rootElement != value)
                {
                    SetValue(ChartDockPanel.RootElementProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the float alignment X. This is a dependency property.
        /// </summary>
        /// <value>The float alignment X.</value>
        public AlignmentX FloatAlignmentX
        {
            get
            {
                return (AlignmentX)GetValue(ChartDockPanel.FloatAlignmentXProperty);
            }

            set
            {
                SetValue(ChartDockPanel.FloatAlignmentXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the float alignment Y. This is a dependency property.
        /// </summary>
        /// <value>The float alignment Y.</value>
        public AlignmentY FloatAlignmentY
        {
            get
            {
                return (AlignmentY)GetValue(ChartDockPanel.FloatAlignmentYProperty);
            }

            set
            {
                SetValue(ChartDockPanel.FloatAlignmentYProperty, value);
            }
        }
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_movedElement
        /// </summary>
        private UIElement m_movedElement;

        /// <summary>
        /// Initializes m_rootElement
        /// </summary>
        private UIElement m_rootElement;

        /// <summary>
        /// Initializes m_controlsThickness
        /// </summary>
        private Thickness m_controlsThickness = new Thickness();

        /// <summary>
        /// Initializes m_resultDockRect
        /// </summary>
        private Rect m_resultDockRect = new Rect();

        /// <summary>
        /// Initializes m_mouseOffset
        /// </summary>
        private Point m_mouseOffset = new Point();
        private Point m_mouseDown = new Point();
        private Point m_mouseUp = new Point();
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the dock of Chart Dock panel's child element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dock">The docking value.</param>
        public static void SetDock(UIElement element, ChartDock dock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ChartDockPanel.DockProperty, dock);
        }

        /// <summary>
        /// Gets the dock attached property on passed <see cref="UIElement"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>DOcking value for element.</returns>
        public static ChartDock GetDock(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (ChartDock)element.GetValue(ChartDockPanel.DockProperty);
        }

        /// <summary>
        /// Sets the alignment of Chart Dock panel's child element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="alignment">The alignment.</param>
        public static void SetAlignment(UIElement element, ChartAlignment alignment)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ChartDockPanel.AlignmentProperty, alignment);
        }

        /// <summary>
        /// Gets the alignment attached property on passed <see cref="UIElement"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The ChartAlignment</returns>
        public static ChartAlignment GetAlignment(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (ChartAlignment)element.GetValue(ChartDockPanel.AlignmentProperty);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Invoked when the <see cref="T:System.Windows.Media.VisualCollection"></see> of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The <see cref="T:System.Windows.Media.Visual"></see> that was added to the collection.</param>
        /// <param name="visualRemoved">The <see cref="T:System.Windows.Media.Visual"></see> that was removed from the collection.</param>
        /// <seealso cref="ChartDockPanel"/>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            UIElement elementAdded = visualAdded as UIElement;
            UIElement elementRemoved = visualRemoved as UIElement;

            if (elementAdded != null)
            {
                elementAdded.MouseMove += new MouseEventHandler(OnElementMouseMove);
                elementAdded.MouseDown += new MouseButtonEventHandler(OnElementMouseDown);
                elementAdded.MouseUp += new MouseButtonEventHandler(OnElementMouseUp);
            }

            if (elementRemoved != null)
            {
                elementRemoved.MouseMove -= new MouseEventHandler(OnElementMouseMove);
                elementRemoved.MouseDown -= new MouseButtonEventHandler(OnElementMouseDown);
                elementRemoved.MouseUp -= new MouseButtonEventHandler(OnElementMouseUp);
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_resultDockRect = ChartLayoutUtils.Subtractthickness(new Rect(finalSize), m_controlsThickness);
            Rect currRect = new Rect(new Point(0, 0), finalSize);
            Rect resRect = currRect;

            #region Arrange Central Element
            if (m_rootElement != null)
            {
                try
                {
                    m_rootElement.Arrange(m_resultDockRect);
                }
                catch
                {
                }
            }
            #endregion

            #region Arrange All Elements
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement elenemt = InternalChildren[i];
                Size elemSize = ChartLayoutUtils.Addthickness(elenemt.DesiredSize, ElementMargin);

                if (elenemt != null && elenemt != m_rootElement)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        #region Orientation == Orientation.Vertical
                        switch (GetDock(elenemt))
                        {
                            case ChartDock.Left:
                                ArrangeElement(elenemt, ChartDock.Left, new Rect(currRect.Left, 0, elemSize.Width, finalSize.Height));
                                currRect.X += elemSize.Width;
                                currRect.Width -= elemSize.Width;
                                break;

                            case ChartDock.Right:
                                currRect.Width -= elenemt.DesiredSize.Width;
                                ArrangeElement(elenemt, ChartDock.Right, new Rect(currRect.Right, 0, elemSize.Width, finalSize.Height));
                                break;

                            case ChartDock.Top:
                                ArrangeElement(elenemt, ChartDock.Top, new Rect(resRect.X, currRect.Top, resRect.Width, elemSize.Height));
                                currRect.Y += elemSize.Height;
                                currRect.Height -= elemSize.Height;
                                break;

                            case ChartDock.Bottom:
                                currRect.Height -= elemSize.Height;
                                ArrangeElement(elenemt, ChartDock.Bottom, new Rect(resRect.X, currRect.Bottom, resRect.Width, elemSize.Height));
                                break;

                            case ChartDock.Floating:
                                if (elenemt is ChartLegend)
                                {
                                    Rect elementRect = ChartLayoutUtils.GetUIElementBounds(elenemt);
                                    elementRect.X = (elenemt as ChartLegend).OffsetX;
                                    elementRect.Y = (elenemt as ChartLegend).OffsetY;
                                    Rect legendrect = EnsureRectIsInside(resRect, elementRect);
                                    BringToFront(elenemt);
                                    elenemt.Arrange(legendrect);
                                    (elenemt as ChartLegend).OnLocationChanged(new LegendLocationChangedeventArgs((elenemt as ChartLegend)));
                                }
                                else
                                {
                                    elenemt.Arrange(EnsureRectIsInside(resRect, ChartLayoutUtils.GetUIElementBounds(elenemt)));
                                }
                                break;
                        }
                        #endregion
                    }
                    else
                    {
                        #region Orientation == Orientation.Horizontal
                        switch (GetDock(elenemt))
                        {
                            case ChartDock.Left:
                                ArrangeElement(elenemt, ChartDock.Left, new Rect(currRect.Left, resRect.Y, elemSize.Width, resRect.Height));
                                currRect.X += elemSize.Width;
                                currRect.Width -= elemSize.Width;
                                break;

                            case ChartDock.Right:
                                currRect.Width -= elemSize.Width;
                                ArrangeElement(elenemt, ChartDock.Right, new Rect(currRect.Right, resRect.Y, elemSize.Width, resRect.Height));
                                break;

                            case ChartDock.Top:
                                ArrangeElement(elenemt, ChartDock.Top, new Rect(0, currRect.Top, finalSize.Width, elemSize.Height));
                                currRect.Y += elemSize.Height;
                                currRect.Height -= elemSize.Height;
                                break;

                            case ChartDock.Bottom:
                                currRect.Height -= elemSize.Height;
                                ArrangeElement(elenemt, ChartDock.Bottom, new Rect(0, currRect.Bottom, finalSize.Width, elemSize.Height));
                                break;

                            case ChartDock.Floating:
                                if (elenemt is ChartLegend)
                                {                         
                                    Rect elementRect =  ChartLayoutUtils.GetUIElementBounds(elenemt);
                                    elementRect.X = (elenemt as ChartLegend).OffsetX;
                                    elementRect.Y = (elenemt as ChartLegend).OffsetY;
                                    Rect legendrect = EnsureRectIsInside(resRect,elementRect);
                                    BringToFront(elenemt);
                                    elenemt.Arrange(legendrect);
                                    (elenemt as ChartLegend).OnLocationChanged(new LegendLocationChangedeventArgs((elenemt as ChartLegend)));
                                }
                                else
                                {
                                    elenemt.Arrange(EnsureRectIsInside(resRect, ChartLayoutUtils.GetUIElementBounds(elenemt)));
                                }
                                break;
                        }
                        #endregion
                    }
                }
            }
            #endregion

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Thickness margin = this.ElementMargin;
            m_controlsThickness = this.ElementMargin;

            foreach (UIElement element in InternalChildren)
            {
                if (element != null && element != m_rootElement)
                {
                    element.Measure(availableSize);

                    Size elemSize = ChartLayoutUtils.Addthickness(element.DesiredSize, margin);

                    switch (GetDock(element))
                    {
                        case ChartDock.Left:
                            m_controlsThickness.Left += elemSize.Width;
                            break;

                        case ChartDock.Right:
                            m_controlsThickness.Right += elemSize.Width;
                            break;

                        case ChartDock.Top:
                            m_controlsThickness.Top += elemSize.Height;
                            break;

                        case ChartDock.Bottom:
                            m_controlsThickness.Bottom += elemSize.Height;
                            break;
                    }
                }
            }

            try
            {
                m_rootElement.Measure(ChartLayoutUtils.Subtractthickness(availableSize, m_controlsThickness));
            }
            catch (Exception)
            {
            }

            return ChartLayoutUtils.Addthickness(new Size(0, 0), m_controlsThickness);
        }

        /// <summary>
        /// Called when dock is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartDockPanel dockPanel = VisualTreeHelper.GetParent(d) as ChartDockPanel;

            if (dockPanel != null)
            {
                dockPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Called when alignment is changed.
        /// </summary>
        /// <param name="dpObj">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAlignmentChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ChartDockPanel dockPanel = VisualTreeHelper.GetParent(dpObj) as ChartDockPanel;

            if (dockPanel != null)
            {
                dockPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Called when float alignment is changed.
        /// </summary>
        /// <param name="dpObj">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFloatAlignmentChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ChartDockPanel dockPanel = VisualTreeHelper.GetParent(dpObj) as ChartDockPanel;

            if (dockPanel != null)
            {
                dockPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Called when root element is changed.
        /// </summary>
        /// <param name="dpObj">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRootElementChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ChartDockPanel dockPanel = dpObj as ChartDockPanel;

            if (dockPanel != null)
            {
                if (e.OldValue != null)
                {
                    dockPanel.Children.Remove(e.OldValue as UIElement);
                    dockPanel.m_rootElement = null;
                }

                if (e.NewValue != null)
                {
                    dockPanel.m_rootElement = e.NewValue as UIElement;
                    dockPanel.Children.Add(e.NewValue as UIElement);
                }
            }
        }

        /// <summary>
        /// Ensures the rectangle is inside specified bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="rect">The rectangle.</param>
        /// <returns>Returns the Rectangle</returns>
        private static Rect EnsureRectIsInside(Rect bounds, Rect rect)
        {
            if (rect.Bottom > bounds.Bottom)
            {
                rect.Y -= rect.Bottom - bounds.Bottom;
            }

            if (rect.Right > bounds.Right)
            {
                rect.X -= rect.Right - bounds.Right;
            }

            if (rect.Top < bounds.Top)
            {
                rect.Y -= rect.Top - bounds.Top;
            }

            if (rect.Left < bounds.Left)
            {
                rect.X -= rect.Left - bounds.Left;
            }

            return rect;
        }

        /// <summary>
        /// Called when mouse button released over panel's element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnElementMouseUp(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;

            if (element != null && element == m_movedElement)
            {
                m_movedElement.ReleaseMouseCapture();
                m_movedElement = null;
            }
        }

        /// <summary>
        /// Called when mouse button is presssed over panel's element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;

            if (element != null && element != m_rootElement)
            {
                Point position = e.GetPosition(element);
                m_mouseDown = e.GetPosition(element);
                HitTestResult hitTest = VisualTreeHelper.HitTest(element, position);
                if (hitTest != null)
                {
                    FrameworkElement elementCheck = hitTest.VisualHit as FrameworkElement;
                    if (elementCheck != null)
                    {
                        if (elementCheck.GetType() != typeof(ToolBarItem))
                        {
                            m_movedElement = element;
                            m_mouseOffset = e.GetPosition(m_movedElement);
                            m_mouseOffset.X = m_mouseOffset.X / m_movedElement.DesiredSize.Width;
                            m_mouseOffset.Y = m_mouseOffset.Y / m_movedElement.DesiredSize.Height;
                            m_movedElement.CaptureMouse();
                            BringToFront(element);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when mouse is moved over panel's element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnElementMouseMove(object sender, MouseEventArgs e)
        {
            UIElement element = sender as UIElement;
            m_mouseUp = e.GetPosition(element);
            Rect insideRect = ChartLayoutUtils.Subtractthickness(
              new Rect(0, 0, this.ActualWidth, this.ActualHeight), m_controlsThickness);

            if (element != null && element == m_movedElement)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    ChartDock currDock = ChartDockPanel.GetDock(m_movedElement);
                    Point pt = e.GetPosition(this);
                    if (insideRect.Contains(pt) && m_mouseDown != m_mouseUp)
                    {
                        if (element is ChartToolBar)
                        {
                            if (m_mouseDown != m_mouseUp)
                                pt = MoveFloatingElement(currDock, pt);
                            return;
                        }
                        else
                            if (m_mouseDown != m_mouseUp)
                                pt = MoveFloatingElement(currDock, pt);
                    }
                    else if (currDock == ChartDock.Floating)
                    {
                        #region Docking
                        if (pt.X < insideRect.Left)
                        {
                            ChartDockPanel.SetDock(m_movedElement, ChartDock.Left);
                        }
                        else if (pt.X > insideRect.Right)
                        {
                            ChartDockPanel.SetDock(m_movedElement, ChartDock.Right);
                        }
                        else if (pt.Y < insideRect.Top)
                        {
                            ChartDockPanel.SetDock(m_movedElement, ChartDock.Top);
                        }
                        else if (pt.Y > insideRect.Bottom)
                        {
                            ChartDockPanel.SetDock(m_movedElement, ChartDock.Bottom);
                        }
                        #endregion
                    }
                    else
                    {
                        pt = MoveDockingElement(currDock, pt);
                    }
                }
            }
        }

        /// <summary>
        /// Moves the docking element.
        /// </summary>
        /// <param name="currentDock">The current dock.</param>
        /// <param name="point">The point.</param>
        /// <returns>Returns the docking point</returns>
        private Point MoveDockingElement(ChartDock currentDock, Point point)
        {
            #region Dock moving
            int index = this.InternalChildren.IndexOf(m_movedElement);

            for (int i = 0; i < this.InternalChildren.Count; i++)
            {
                UIElement celement = this.InternalChildren[i];
                Rect bounds = ChartLayoutUtils.GetUIElementBounds(celement);
                Point center = ChartLayoutUtils.GetCenter(bounds);
                ChartDock dock = ChartDockPanel.GetDock(celement);

                if (i != index && currentDock == dock)
                {
                    switch (currentDock)
                    {
                        case ChartDock.Left:
                            if (((index > i) && (point.X < center.X)) || ((index < i) && (point.X > center.X)))
                            {
                                this.InternalChildren.Remove(m_movedElement);
                                this.InternalChildren.Insert(Math.Min(i, this.InternalChildren.Count), m_movedElement);
                            }

                            break;

                        case ChartDock.Top:
                            if (((index > i) && (point.Y < center.Y)) || ((index < i) && (point.Y > center.Y)))
                            {
                                this.InternalChildren.Remove(m_movedElement);
                                this.InternalChildren.Insert(Math.Min(i, this.InternalChildren.Count), m_movedElement);
                            }

                            break;

                        case ChartDock.Right:
                            if (((index > i) && (point.X > center.X)) || ((index < i) && (point.X < center.X)))
                            {
                                this.InternalChildren.Remove(m_movedElement);
                                this.InternalChildren.Insert(Math.Min(i, this.InternalChildren.Count), m_movedElement);
                            }

                            break;

                        case ChartDock.Bottom:
                            if (((index > i) && (point.Y > center.Y)) || ((index < i) && (point.Y < center.Y)))
                            {
                                this.InternalChildren.Remove(m_movedElement);
                                if(this.InternalChildren.Count>1)
                                    this.InternalChildren.Insert(Math.Max(i, this.InternalChildren.Count-2), m_movedElement);
                                else
                                    this.InternalChildren.Insert(Math.Min(i, this.InternalChildren.Count), m_movedElement);
                            }

                            break;
                    }
                }
            }
            #endregion
            return point;
        }

        /// <summary>
        /// Moves the floating element.
        /// </summary>
        /// <param name="currentDock">The current dock.</param>
        /// <param name="point">The point value.</param>
        /// <returns>The floating point</returns>
        private Point MoveFloatingElement(ChartDock currentDock, Point point)
        {
            if (currentDock != ChartDock.Floating)
            {
                ChartDockPanel.SetDock(m_movedElement, ChartDock.Floating);
            }

            Rect newRect = new Rect(point.X - m_mouseOffset.X * m_movedElement.DesiredSize.Width, point.Y - m_mouseOffset.Y * m_movedElement.DesiredSize.Height, m_movedElement.DesiredSize.Width, m_movedElement.DesiredSize.Height);

            m_movedElement.Arrange(EnsureRectIsInside(new Rect(0, 0, this.ActualWidth, this.ActualHeight), newRect));

            if (m_movedElement is ChartLegend)
            {
                (m_movedElement as ChartLegend).OffsetX = newRect.X;
                (m_movedElement as ChartLegend).OffsetY = newRect.Y;
            }

            return point;
        }

        /// <summary>
        /// Arranges the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dock">The dock value.</param>
        /// <param name="rect">The rectangle.</param>
        private void ArrangeElement(UIElement element, ChartDock dock, Rect rect)
        {
            ChartAlignment aligment = GetAlignment(element);
            Size sz = ChartLayoutUtils.Addthickness(element.DesiredSize, ElementMargin);

            if (dock == ChartDock.Left || dock == ChartDock.Right)
            {
                #region Vertial Elements
                switch (aligment)
                {
                    case ChartAlignment.Center:
                        break;

                    case ChartAlignment.Far:
                        rect = new Rect(rect.X, rect.Bottom - sz.Height, rect.Width, sz.Height);
                        break;

                    case ChartAlignment.Near:
                        rect = new Rect(rect.X, rect.Y, rect.Width, sz.Height);
                        break;
                }
                #endregion
            }
            else if (dock == ChartDock.Top || dock == ChartDock.Bottom)
            {
                #region Vertial Elements
                switch (aligment)
                {
                    case ChartAlignment.Center:
                        break;

                    case ChartAlignment.Far:
                        rect = new Rect(rect.Right - sz.Width, rect.Y, sz.Width, rect.Height);
                        break;

                    case ChartAlignment.Near:
                        rect = new Rect(rect.X, rect.Y, sz.Width, rect.Height);
                        break;
                }
                #endregion
            }
            else
            {
                #region Float Elements
                switch (FloatAlignmentX)
                {
                    case AlignmentX.Left:
                        rect = new Rect(rect.X + FloatingOffset.X, rect.Y, sz.Width, rect.Height);
                        break;

                    case AlignmentX.Center:
                        rect = new Rect(rect.X + 0.5 * (rect.Width - sz.Width), rect.Y, sz.Width, rect.Height);
                        break;

                    case AlignmentX.Right:
                        rect = new Rect(rect.X + rect.Width - sz.Width - FloatingOffset.X, rect.Y, sz.Width, rect.Height);
                        break;
                }

                switch (FloatAlignmentY)
                {
                    case AlignmentY.Top:
                        rect = new Rect(rect.X, rect.Y + FloatingOffset.Y, rect.Width, sz.Height);
                        break;

                    case AlignmentY.Center:
                        rect = new Rect(rect.X, rect.Y + 0.5 * (rect.Height - sz.Height), rect.Width, sz.Height);
                        break;

                    case AlignmentY.Bottom:
                        rect = new Rect(rect.X, rect.Y + rect.Height - sz.Height - FloatingOffset.Y, rect.Width, sz.Height);
                        break;
                }
                #endregion
            }

            element.Arrange(ChartLayoutUtils.Subtractthickness(rect, ElementMargin));
        }

        /// <summary>
        /// Brings element to front.
        /// </summary>
        /// <param name="element">The element.</param>
        private void BringToFront(UIElement element)
        {
            if (GetZIndex(element) < Children.Count)
            {
                SetZIndex(element, Children.Count);
            }
        }



        internal void Dispose()
        {
            this.m_movedElement = null;
            this.m_rootElement = null;
            if (this.Children != null)
            {

                this.Children.Clear();
            }
            this.Background = null;
            this.ClearValue(ChartDockPanel.DockProperty);
        }
        #endregion
    }
}
