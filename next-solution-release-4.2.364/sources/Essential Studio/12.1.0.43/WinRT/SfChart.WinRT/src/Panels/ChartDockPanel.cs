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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents Chart Dockpanel.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public enum ChartDock
    {
        /// <summary>
        /// Docks element at the left side of panel.
        /// </summary>
        Left,

        /// <summary>
        /// Docks element at the top side of panel.
        /// </summary>
        Top,

        /// <summary>
        /// Docks element at the right side of panel.
        /// </summary>
        Right,

        /// <summary>
        /// Docks element at the bottom side of panel.
        /// </summary>
        Bottom,

        /// <summary>
        /// Docks element at any position on  panel
        /// </summary>
        Floating,
    }


    /// <summary>
    /// Arranges child elements around the edges of the panel.  Optionally, 
    /// last added child element can occupy the remaining space.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    [ClassReference(IsReviewed = false)]
    public class ChartDockPanel : Panel
    {
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private static bool _ignorePropertyChange;

        #region public bool LastChildFill
        /// <summary>
        /// Get or Set lastChildFillProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool LastChildFill
        {
            get { return (bool)GetValue(LastChildFillProperty); }
            set { SetValue(LastChildFillProperty, value); }
        }

        /// <summary>
        /// Identifies the LastChildFill dependency property.
        /// </summary>
        public static readonly DependencyProperty LastChildFillProperty =
            DependencyProperty.Register(
                "LastChildFill",
                typeof(bool),
                typeof(ChartDockPanel),
                new PropertyMetadata(true, OnLastChildFillPropertyChanged));

        /// <summary>
        /// LastChildFillProperty property changed handler.
        /// </summary>
        /// <param name="d">DockPanel that changed its LastChildFill.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnLastChildFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartDockPanel source = d as ChartDockPanel;
            source.InvalidateArrange();
        }
        #endregion public bool LastChildFill

        #region public attached ChartDock ChartDock
        /// <summary>
        /// Gets an element's dock position in the Chart area.
        /// </summary>
        /// <param name="element">any UIElement</param>
        /// <returns>returns dock position of UIElement.</returns>
        public static ChartDock GetDock(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ChartDock)element.GetValue(DockProperty);
        }

        /// <summary>
        ///Sets an element's dock position in the Chart area.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dock"></param>
        public static void SetDock(UIElement element, ChartDock dock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(DockProperty, dock);
        }

        /// <summary>
        ///  Identifies the Dock Attached property.
        /// </summary>
        public static readonly DependencyProperty DockProperty =
            DependencyProperty.RegisterAttached(
                "Dock",
                typeof(ChartDock),
                typeof(ChartDockPanel),
                new PropertyMetadata(ChartDock.Top, OnDockPropertyChanged));

        /// <summary>
        /// DockProperty property changed handler.
        /// </summary>
        /// <param name="d">UIElement that changed its ChartDock.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(e.OldValue)) return;
            // Ignore the change if requested
            if (_ignorePropertyChange)
            {
                _ignorePropertyChange = false;
                return;
            }

            UIElement element = (UIElement)d;

            //if (element is ChartLegend)
            //{
            //    (element as ChartLegend).DockPosition = (ChartDock)e.NewValue;
            //}

            ChartDock value = (ChartDock)e.NewValue;

            //// Validate the ChartDock property
            //if ((value != ChartDock.Left) &&
            //    (value != ChartDock.Top) &&
            //    (value != ChartDock.Right) &&
            //    (value != ChartDock.Bottom))
            //{
            //    // Reset the property to its original state before throwing
            //    _ignorePropertyChange = true;
            //    element.SetValue(DockProperty, (ChartDock)e.OldValue);

            //    //string message = string.Format(
            //    //    CultureInfo.InvariantCulture,
            //    //    Syncfusion.Windows.Controls.Properties.Resources.DockPanel_OnDockPropertyChanged_InvalidValue,
            //    //    value);
            //    //throw new ArgumentException(message, "value");
            //}

            // Cause the DockPanel to update its layout when a child changes
            ChartDockPanel panel = VisualTreeHelper.GetParent(element) as ChartDockPanel;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }

        #endregion public attached ChartDock ChartDock

        #region Dependency Property
        internal static readonly DependencyProperty HostProperty = DependencyProperty.Register("Host", typeof(string), typeof(ChartDockPanel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the ElementMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementMarginProperty =
          DependencyProperty.Register("ElementMargin", typeof(Thickness), typeof(ChartDockPanel), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Identifies the RootElement dependency property.
        /// </summary>
        public static readonly DependencyProperty RootElementProperty =
          DependencyProperty.Register("RootElement", typeof(UIElement), typeof(ChartDockPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnRootElementChanged)));


        /// <summary>
        /// Identifies the Alignment dependency property.
        /// </summary>
        public static readonly DependencyProperty AlignmentProperty =
          DependencyProperty.RegisterAttached("Alignment", typeof(ChartAlignment), typeof(ChartDockPanel), new PropertyMetadata(ChartAlignment.Center, new PropertyChangedCallback(OnAlignmentChanged)));


        /// <summary>
        /// Called when alignment is changed.
        /// </summary>
        /// <param name="dpObj">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAlignmentChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
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
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
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
        /// Gets or sets the sync chart area.
        /// </summary>
        /// <value>The sync chart area.</value>
        internal string Host
        {
            set { SetValue(HostProperty, value); }
            get { return (string)GetValue(HostProperty); }
        }
        #endregion

        internal Grid GetGrid()
        {
            DependencyObject element = this;
            while (!(element is Grid) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as Grid;
            }

            return null;
        }

        /// <summary>
        /// Initializes m_rootElement
        /// </summary>
        public UIElement m_rootElement;

        /// <summary>
        /// Initializes m_controlsThickness
        /// </summary>
        private Thickness m_controlsThickness = new Thickness();

        /// <summary>
        /// Initializes m_resultDockRect
        /// </summary>
        private Rect m_resultDockRect = new Rect();

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
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            var topSizes = new List<double>();
            var leftSizes = new List<double>();
            var bottomSizes = new List<double>();
            var rightSizes = new List<double>();

            Thickness margin = this.ElementMargin;
            m_controlsThickness = this.ElementMargin;
            foreach (UIElement element in Children)
            {
                if (element == null) continue;
                if (element == m_rootElement) continue;
                element.Measure(availableSize);
                Size elemSize = ChartLayoutUtils.Addthickness(element.DesiredSize, margin);
                switch (GetDock(element))
                {
                    case ChartDock.Left:
                        if (element is ChartLegend)
                        {
                            var index = (element as ChartLegend).RowColumnIndex;
                            if (leftSizes.Count <= index)
                                leftSizes.Add(elemSize.Width);
                            else if (leftSizes[index] < elemSize.Width)
                                leftSizes[index] = elemSize.Width;
                        }
                        else
                            m_controlsThickness.Left += elemSize.Width;
                        break;

                    case ChartDock.Right:
                        if (element is ChartLegend)
                        {
                            var index = (element as ChartLegend).RowColumnIndex;
                            if (rightSizes.Count <= index)
                                rightSizes.Add(elemSize.Width);
                            else if (rightSizes[index] < elemSize.Width)
                                rightSizes[index] = elemSize.Width;
                        }
                        else
                            m_controlsThickness.Right += elemSize.Width;
                        break;

                    case ChartDock.Top:
                        if (element is ChartLegend)
                        {
                            var index = (element as ChartLegend).RowColumnIndex;
                            if (topSizes.Count <= index)
                                topSizes.Add(elemSize.Height);
                            else if (topSizes[index] < elemSize.Height)
                                topSizes[index] = elemSize.Height;
                        }
                        else
                            m_controlsThickness.Top += elemSize.Height;
                        break;

                    case ChartDock.Bottom:
                        if (element is ChartLegend)
                        {
                            var index = (element as ChartLegend).RowColumnIndex;
                            if (bottomSizes.Count <= index)
                                bottomSizes.Add(elemSize.Height);
                            else if (bottomSizes[index] < elemSize.Height)
                                bottomSizes[index] = elemSize.Height;
                        }
                        else
                            m_controlsThickness.Bottom += elemSize.Height;
                        break;
                }
            }

            m_controlsThickness.Left += leftSizes.Sum();
            m_controlsThickness.Right += rightSizes.Sum();
            m_controlsThickness.Top += topSizes.Sum();
            m_controlsThickness.Bottom += bottomSizes.Sum();
          
            try
            {
                m_rootElement.Measure(ChartLayoutUtils.Subtractthickness(availableSize, m_controlsThickness));
            }
            catch (Exception)
            {
            }
            return availableSize;
        }

        void parentgrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
        }

        private void ArrangeElement(UIElement element, ChartDock dock, Rect rect)
        {
           // ChartAlignment aligment = GetAlignment(element);
            Size sz = ChartLayoutUtils.Addthickness(element.DesiredSize, ElementMargin);
            element.Arrange(ChartLayoutUtils.Subtractthickness(rect, ElementMargin));
        }
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_resultDockRect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(0, 0), finalSize), m_controlsThickness);
            Rect currRect = new Rect(new Point(0, 0), finalSize);
            Rect resRect = currRect;

            #region Arrange Central Element
            if (m_rootElement != null)
            {
                try
                {
                    m_rootElement.Arrange(m_resultDockRect);
                }
                catch (Exception)
                {
                }
            }
            #endregion

            #region Arrange All Elements

            double legendTop = -1d;

            for (int i = 0; i < Children.Count; i++)
            {
                UIElement element = Children[i];
                Size elemSize = ChartLayoutUtils.Addthickness(element.DesiredSize, ElementMargin);
                double scale;
                if (element != null && element != m_rootElement)
                {
                    #region Orientation == Orientation.Horizontal
                    switch (GetDock(element))
                    {
                        case ChartDock.Left:
                            if (element is ChartLegend)
                            {
                                var arrangeRect = (element as ChartLegend).ArrangeRect;
                                ArrangeElement(element, ChartDock.Left, new Rect(arrangeRect.Left + currRect.Left, arrangeRect.Top + m_controlsThickness.Top, arrangeRect.Width, arrangeRect.Height));
                            }
                            else
                            {
                                ArrangeElement(element, ChartDock.Left, new Rect(currRect.Left, resRect.Y, elemSize.Width, resRect.Height));
                                currRect.X += elemSize.Width;
                                scale = currRect.Width - elemSize.Width;
                                currRect.Width = scale > 0 ? scale : 0;
                            }
                            break;

                        case ChartDock.Right:
                            if (element is ChartLegend)
                            {
                                var arrangeRect = (element as ChartLegend).ArrangeRect;
                                ArrangeElement(element, ChartDock.Right, new Rect(arrangeRect.Left + m_controlsThickness.Left, arrangeRect.Top + m_controlsThickness.Top, arrangeRect.Width, arrangeRect.Height));

                            }
                            else
                            {
                                scale = currRect.Width - elemSize.Width;
                                currRect.Width = scale > 0 ? scale : 0;
                                ArrangeElement(element, ChartDock.Right, new Rect(currRect.Right, resRect.Top + m_controlsThickness.Top, elemSize.Width, resRect.Height));
                            }
                            break;

                        case ChartDock.Top:
                            if (element is ChartLegend)
                            {
                                if (legendTop == -1)
                                {
                                    legendTop = currRect.Top;
                                }
                                var arrangeRect = (element as ChartLegend).ArrangeRect;
                                ArrangeElement(element, ChartDock.Top, new Rect(arrangeRect.Left + m_controlsThickness.Left , arrangeRect.Top + legendTop, arrangeRect.Width, arrangeRect.Height));
                               
                            }
                            else
                            {
                                ArrangeElement(element, ChartDock.Top, new Rect(0, currRect.Top, finalSize.Width, elemSize.Height));
                                currRect.Y += elemSize.Height;
                                scale = currRect.Height - elemSize.Height;
                                currRect.Height = scale > 0 ? scale : 0;
                            }
                            break;

                        case ChartDock.Bottom:
                            if (element is ChartLegend)
                            {
                                var arrangeRect = (element as ChartLegend).ArrangeRect;
                                ArrangeElement(element, ChartDock.Bottom, new Rect(arrangeRect.Left + m_controlsThickness.Left , arrangeRect.Top + m_controlsThickness.Top, arrangeRect.Width, arrangeRect.Height));
                            }
                            else
                            {
                                scale = currRect.Height - elemSize.Height;
                                currRect.Height = scale > 0 ? scale : 0;
                                ArrangeElement(element, ChartDock.Bottom, new Rect(0, currRect.Bottom, finalSize.Width, elemSize.Height));
                            }
                            break;
                        case ChartDock.Floating:
                            Rect elementRect = new Rect(new Point(0, 0), new Size(element.DesiredSize.Width, element.DesiredSize.Height));
                            if (element is ChartLegend)
                            {
                                var legend = element as ChartLegend;
                                var offsetX = legend.DockPosition == ChartDock.Floating ? legend.OffsetX : 0d;
                                var offsetY = legend.DockPosition == ChartDock.Floating ? legend.OffsetY : 0d;
                                element.Arrange(EnsureRectIsInside(resRect,
                                    new Rect(legend.ArrangeRect.Left + offsetX + m_controlsThickness.Left,
                                        legend.ArrangeRect.Top + offsetY + m_controlsThickness.Top, element.DesiredSize.Width,
                                        legend.DesiredSize.Height)));
                            }
                            else
                            {
                                element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                            }
                            break;
                    }
                    #endregion
                }
            }
            #endregion

            return base.ArrangeOverride(finalSize);
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
    }
}
