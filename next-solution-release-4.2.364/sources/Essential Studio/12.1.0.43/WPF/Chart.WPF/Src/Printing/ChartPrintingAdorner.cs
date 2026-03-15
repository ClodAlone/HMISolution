// <copyright file="ChartPrintingAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents ChartPrintButton
    /// </summary>
    /// <exclude/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartPrintButton : Button
    {
        #region Public methods
        /// <summary>
        /// Initializes static members of the <see cref="ChartPrintButton"/> class.
        /// </summary>
        static ChartPrintButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPrintButton), new FrameworkPropertyMetadata(typeof(ChartPrintButton)));
        }
        #endregion
    }

    /// <summary>
    /// Represents ChartPrintingAdorner class. Used as adorner for Chart printing
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartPrintingAdorner : Adorner
    {
        #region Constants
        /// <summary>
        /// Initializes c_thumbSize
        /// </summary>
        private const double C_thumbSize = 10;
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_targetElement
        /// </summary>
        private FrameworkElement m_targetElement;

        /// <summary>
        /// Initializes m_elements
        /// </summary>
        private UIElementCollection m_elements;

        /// <summary>
        /// Initializes m_lastMousePos
        /// </summary>
        private Point m_lastMousePos = new Point();

        /// <summary>
        /// Initializes m_leftcoefficient
        /// </summary>
        private double m_leftcoefficient;

        /// <summary>
        /// Initializes m_topcoefficient
        /// </summary>
        private double m_topcoefficient;

        /// <summary>
        /// Initializes m_rightcoefficient
        /// </summary>
        private double m_rightcoefficient = 1;

        /// <summary>
        /// Initializes m_bottomcoefficient
        /// </summary>
        private double m_bottomcoefficient = 1;

        /// <summary>
        /// Initializes m_thumbTopLeft
        /// </summary>
        private Thumb m_thumbTopLeft;

        /// <summary>
        /// Initializes m_thumbTopRight
        /// </summary>
        private Thumb m_thumbTopRight;

        /// <summary>
        /// Initializes m_thumbBottomLeft
        /// </summary>
        private Thumb m_thumbBottomLeft;

        /// <summary>
        /// Initializes m_thumbBottomRight
        /// </summary>
        private Thumb m_thumbBottomRight;

        /// <summary>
        /// Initializes m_rectangle
        /// </summary>
        private Rectangle m_rectangle;

        /// <summary>
        /// Initializes m_buttonPrint
        /// </summary>
        private ChartPrintButton m_buttonPrint;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the SelectedPrintingArea
        /// </summary>
        public Rect SelectedPrintingArea
        {
            get
            {
                double realLeft = (m_targetElement.ActualWidth + ChartLayoutUtils.GetUIElementBounds(m_targetElement).X) * m_leftcoefficient;
                double realTop = (m_targetElement.ActualHeight + ChartLayoutUtils.GetUIElementBounds(m_targetElement).Y) * m_topcoefficient;
                double realRight = (m_targetElement.ActualWidth + ChartLayoutUtils.GetUIElementBounds(m_targetElement).X) * m_rightcoefficient;
                double realBottom = (m_targetElement.ActualHeight + ChartLayoutUtils.GetUIElementBounds(m_targetElement).Y) * m_bottomcoefficient;

                return new Rect(new Point(realLeft, realTop), new Point(realRight, realBottom));
            }
        }

        /// <summary>
        /// Gets the print button.
        /// </summary>
        /// <value>The print button.</value>
        public ChartPrintButton PrintButton
        {
            get
            {
                return m_buttonPrint;
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return m_elements.Count;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPrintingAdorner"/> class.
        /// </summary>
        /// <param name="adorningTarget">The AdorningTarget.</param>
        public ChartPrintingAdorner(FrameworkElement adorningTarget)
            : base(adorningTarget)
        {
            m_targetElement = adorningTarget;
            m_elements = new UIElementCollection(this, this);

            m_thumbTopLeft = CreateThumb(Cursors.SizeNWSE);
            m_thumbTopRight = CreateThumb(Cursors.SizeNESW);
            m_thumbBottomLeft = CreateThumb(Cursors.SizeNESW);
            m_thumbBottomRight = CreateThumb(Cursors.SizeNWSE);

            m_thumbTopLeft.DragDelta += new DragDeltaEventHandler(OnThumbTopLeftDragDelta);
            m_thumbTopRight.DragDelta += new DragDeltaEventHandler(OnThumbTopRightDragDelta);
            m_thumbBottomLeft.DragDelta += new DragDeltaEventHandler(OnThumbBottomLeftDragDelta);
            m_thumbBottomRight.DragDelta += new DragDeltaEventHandler(OnThumbBottomRightDragDelta);

            m_rectangle = new Rectangle();
            m_rectangle.Stroke = Brushes.Black;
            m_rectangle.StrokeThickness = 1;
            m_rectangle.StrokeDashArray = new DoubleCollection(new double[] { 5, 5, 5, 5 });
            m_rectangle.Fill = new SolidColorBrush(Color.FromArgb(0x4F, 0x00, 0x0F, 0xFF));
            m_rectangle.Stretch = Stretch.Fill;
            m_rectangle.MouseDown += new MouseButtonEventHandler(OnRectangleMouseDown);
            m_rectangle.MouseUp += new MouseButtonEventHandler(OnRectangleMouseUp);
            m_rectangle.MouseMove += new MouseEventHandler(OnRectangleMouseMove);

            m_buttonPrint = new ChartPrintButton();
            m_buttonPrint.Click += new RoutedEventHandler(OnButtonPrintClick);

            m_elements.Add(m_rectangle);
            m_elements.Add(m_thumbTopLeft);
            m_elements.Add(m_thumbTopRight);
            m_elements.Add(m_thumbBottomLeft);
            m_elements.Add(m_thumbBottomRight);
            m_elements.Add(m_buttonPrint);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double realLeft = m_targetElement.ActualWidth * m_leftcoefficient;
            double realTop = m_targetElement.ActualHeight * m_topcoefficient;
            double realRight = m_targetElement.ActualWidth * m_rightcoefficient;
            double realBottom = m_targetElement.ActualHeight * m_bottomcoefficient;

            m_rectangle.Arrange(new Rect(new Point(realLeft, realTop), new Point(realRight, realBottom)));

            m_thumbTopLeft.Arrange(ChartLayoutUtils.GetRectByCenter(realLeft, realTop, C_thumbSize, C_thumbSize));
            m_thumbTopRight.Arrange(ChartLayoutUtils.GetRectByCenter(realRight, realTop, C_thumbSize, C_thumbSize));
            m_thumbBottomLeft.Arrange(ChartLayoutUtils.GetRectByCenter(realLeft, realBottom, C_thumbSize, C_thumbSize));
            m_thumbBottomRight.Arrange(ChartLayoutUtils.GetRectByCenter(realRight, realBottom, C_thumbSize, C_thumbSize));

            m_buttonPrint.Arrange(ChartLayoutUtils.GetRectByCenter((realLeft + realRight) / 2, (realTop + realBottom) / 2, m_buttonPrint.Width, m_buttonPrint.Height));

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// The GetVisualChild method
        /// </summary>
        /// <param name="index">The index value</param>
        /// <returns>Returns the visual element</returns>
        protected override Visual GetVisualChild(int index)
        {
            return m_elements[index];
        }

        /// <summary>
        /// Creates the thumb.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <returns>Returns the Thumb</returns>
        private static Thumb CreateThumb(Cursor cursor)
        {
            Thumb thumb = new Thumb();

            thumb.Cursor = cursor;
            thumb.Width = C_thumbSize;
            thumb.Height = C_thumbSize;
            thumb.Background = Brushes.Gray;

            return thumb;
        }

        /// <summary>
        /// Raised On RectangleMouseMove
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The MouseEvent Arguments e</param>
        private void OnRectangleMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currPos = e.GetPosition(m_targetElement);

                m_leftcoefficient = ChartMath.MinMax(m_leftcoefficient + (currPos.X - m_lastMousePos.X) / m_targetElement.ActualWidth, 0, 1);
                m_topcoefficient = ChartMath.MinMax(m_topcoefficient + (currPos.Y - m_lastMousePos.Y) / m_targetElement.ActualHeight, 0, 1);
                m_rightcoefficient = ChartMath.MinMax(m_rightcoefficient + (currPos.X - m_lastMousePos.X) / m_targetElement.ActualWidth, 0, 1);
                m_bottomcoefficient = ChartMath.MinMax(m_bottomcoefficient + (currPos.Y - m_lastMousePos.Y) / m_targetElement.ActualHeight, 0, 1);

                this.InvalidateArrange();
            }

            m_lastMousePos = e.GetPosition(m_targetElement);
        }

        /// <summary>
        /// Raised On RectangleMouseUp
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The MouseButtonEvent Arguments e</param>
        private void OnRectangleMouseUp(object sender, MouseButtonEventArgs e)
        {
            m_rectangle.ReleaseMouseCapture();
        }

        /// <summary>
        /// Raised On RectangleMouseDown
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The MouseButtonEvent Arguments e</param>
        private void OnRectangleMouseDown(object sender, MouseButtonEventArgs e)
        {
            m_rectangle.CaptureMouse();
        }

        /// <summary>
        /// Called when the bottom right thumb drag delta changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void OnThumbBottomRightDragDelta(object sender, DragDeltaEventArgs e)
        {
            m_rightcoefficient = ChartMath.MinMax(m_rightcoefficient + e.HorizontalChange / m_targetElement.ActualWidth, 0, 1);
            m_bottomcoefficient = ChartMath.MinMax(m_bottomcoefficient + e.VerticalChange / m_targetElement.ActualHeight, 0, 1);

            this.InvalidateArrange();
        }

        /// <summary>
        /// Called when [thumb bottom left drag delta].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void OnThumbBottomLeftDragDelta(object sender, DragDeltaEventArgs e)
        {
            m_leftcoefficient = ChartMath.MinMax(m_leftcoefficient + e.HorizontalChange / m_targetElement.ActualWidth, 0, 1);
            m_bottomcoefficient = ChartMath.MinMax(m_bottomcoefficient + e.VerticalChange / m_targetElement.ActualHeight, 0, 1);

            this.InvalidateArrange();
        }

        /// <summary>
        /// Called when [thumb top right drag delta].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void OnThumbTopRightDragDelta(object sender, DragDeltaEventArgs e)
        {
            m_rightcoefficient = ChartMath.MinMax(m_rightcoefficient + e.HorizontalChange / m_targetElement.ActualWidth, 0, 1);
            m_topcoefficient = ChartMath.MinMax(m_topcoefficient + e.VerticalChange / m_targetElement.ActualHeight, 0, 1);

            this.InvalidateArrange();
        }

        /// <summary>
        /// Called when [thumb top left drag delta].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void OnThumbTopLeftDragDelta(object sender, DragDeltaEventArgs e)
        {
            m_leftcoefficient = ChartMath.MinMax(m_leftcoefficient + e.HorizontalChange / m_targetElement.ActualWidth, 0, 1);
            m_topcoefficient = ChartMath.MinMax(m_topcoefficient + e.VerticalChange / m_targetElement.ActualHeight, 0, 1);

            this.InvalidateArrange();
        }

        /// <summary>
        /// Called when [button print click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnButtonPrintClick(object sender, RoutedEventArgs e)
        {
            if (m_targetElement is Chart)
            {
                (m_targetElement as Chart).Print(SelectedPrintingArea);
            }
        }
        #endregion
    }
}
