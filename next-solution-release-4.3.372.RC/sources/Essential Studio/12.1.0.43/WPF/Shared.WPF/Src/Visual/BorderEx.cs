// <copyright file="BorderEx.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Collections;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents element that is used to draw 3 borders at once.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TrippleBorder : Decorator
    {
        #region Constants
        /// <summary>
        /// Using for correct rendering of rounded corners of element.
        /// </summary>
        private const int CornerRadiusOffset = 2;
        #endregion

        #region Private fields
        /// <summary>
        /// Utilized for rendering outside border.
        /// </summary>
        private Rect m_rectOutside;

        /// <summary>
        /// Utilized for rendering middle border.
        /// </summary>
        private Rect m_rectBorder;

        /// <summary>
        /// Utilized for rendering inside border.
        /// </summary>
        private Rect m_rectInside;

        /// <summary>
        /// Pen of outside border.
        /// </summary>
        private Pen m_outsidePen;

        /// <summary>
        /// Pen of middle border.
        /// </summary>
        private Pen m_borderPen;

        /// <summary>
        /// Pen of inside border.
        /// </summary>
        private Pen m_insidePen;

        /// <summary>
        /// Adorner child.
        /// </summary>
        private UIElement m_adornerChild;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the adorner child.
        /// </summary>
        /// <value>The adorner child.</value>
        public UIElement AdornerChild
        {
            get
            {
                return m_adornerChild;
            }

            set
            {
                if (m_adornerChild != value)
                {
                    if (m_adornerChild != null)
                    {
                        base.RemoveVisualChild(m_adornerChild);
                        base.RemoveLogicalChild(m_adornerChild);
                    }

                    m_adornerChild = value;

                    if (m_adornerChild != null)
                    {
                        base.AddLogicalChild(value);
                        base.AddVisualChild(value);
                    }

                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value that specifies the radius of the corners. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Double value that represents the radius of corners.
        /// </value>
        /// <remarks>
        /// While the standard border takes value of the <see cref="Thickness"/> type, we use a double value to improve rendering performance.
        /// </remarks>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that specifies the brush that should be used for drawing central border. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Value that specifies the brush that should be used for drawing central border.
        /// </value>        
        public Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(Border.BorderBrushProperty);
            }

            set
            {
                SetValue(Border.BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that specifies the thickness of the borders. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Double value that represents the thickness of the borders.
        /// </value>
        /// <remarks>
        /// While the standard border takes value of the <see cref="Thickness"/> type, we use a double value to improve rendering performance.
        /// </remarks>
        public double BorderThickness
        {
            get
            {
                return (double)GetValue(BorderThicknessProperty);
            }

            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that specifies the brush that should be used for drawing inside border. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Value that specifies the brush that should be used for drawing inside border.
        /// </value>
        public Brush InsideBorderBrush
        {
            get
            {
                return (Brush)GetValue(InsideBorderBrushProperty);
            }

            set
            {
                SetValue(InsideBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that specifies the thickness of the inside border. 
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Double value that specifies the thickness of the inside border.
        /// </value>
        /// <remarks>
        /// While the standard border takes value of the <see cref="Thickness"/> type, we use a double value to improve rendering performance.
        /// </remarks>
        public double InsideBorderThickness
        {
            get
            {
                return (double)GetValue(InsideBorderThicknessProperty);
            }

            set
            {
                SetValue(InsideBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that specifies the brush that should be used for drawing outside border.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Value that represents the brush that should be used for drawing outside border.
        /// </value>
        public Brush OutsideBorderBrush
        {
            get
            {
                return (Brush)GetValue(OutsideBorderBrushProperty);
            }

            set
            {
                SetValue(OutsideBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that specifies the thickness of the outside border.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Double value that specifies the thickness of the outside border.
        /// </value>
        /// <remarks>
        /// While the standard border takes value of the <see cref="Thickness"/> type, we use a double value to improve rendering performance.
        /// </remarks>
        public double OutsideBorderThickness
        {
            get
            {
                return (double)GetValue(OutsideBorderThicknessProperty);
            }

            set
            {
                SetValue(OutsideBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that specifies the brush used for filling background.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Value that specifies the brush used for filling background.
        /// </value>
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(Border.BackgroundProperty);
            }

            set
            {
                SetValue(Border.BackgroundProperty, value);
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TrippleBorder"/> class.
        /// </summary>
        static TrippleBorder()
        {
            Border.BorderBrushProperty.AddOwner(typeof(TrippleBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBorderBrushChanged)));
            Border.BackgroundProperty.AddOwner(typeof(TrippleBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrippleBorder"/> class.
        /// </summary>
        public TrippleBorder()
        {
            m_outsidePen = new Pen(OutsideBorderBrush, OutsideBorderThickness);
            m_borderPen = new Pen(BorderBrush, BorderThickness);
            m_insidePen = new Pen(InsideBorderBrush, InsideBorderThickness);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Arranges child element and light-weight adorner, calculates areas of borders drawing.
        /// </summary>
        /// <param name="arrangeSize">Specifies the supposed size of the control.</param>
        /// <returns>Returns the actually used size. It can be larger than the initial size in case when the initial size is too small to draw all borders.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double outsideThickness = OutsideBorderThickness;
            double borderThickness = BorderThickness;
            double insideThickness = InsideBorderThickness;

            double minThickness = 2 * (outsideThickness + insideThickness + borderThickness);
            arrangeSize.Width = Math.Max(arrangeSize.Width, minThickness);
            arrangeSize.Height = Math.Max(arrangeSize.Height, minThickness);

            ////Calculation of x, y of co-ordinates for m_rectOutside (outside border).
            double temp = 0.5 * outsideThickness;
            Point outsidePoint = new Point(temp, temp);
            ////Calculation of width and height for m_rectOutside.
            Size outsideSize = new Size(arrangeSize.Width - outsideThickness, arrangeSize.Height - outsideThickness);
            m_rectOutside = new Rect(outsidePoint, outsideSize);

            ////Calculation of x, y of co-ordinates for m_rectBorder (middle border).
            temp = outsideThickness + 0.5 * borderThickness;
            Point borderPoint = new Point(temp, temp);
            ////Calculation of width and height for m_rectBorder.
            Size borderSize = new Size(outsideSize.Width - outsideThickness - borderThickness, outsideSize.Height - outsideThickness - borderThickness);
            m_rectBorder = new Rect(borderPoint, borderSize);

            ////Calculation of x, y of co-ordinates for m_rectInside (inside border).
            temp = outsideThickness + borderThickness + 0.5 * insideThickness;
            Point insidePoint = new Point(temp, temp);
            ////Calculation of width and height for m_rectInside.
            Size insideSize = new Size(borderSize.Width - borderThickness - insideThickness, borderSize.Height - borderThickness - insideThickness);
            m_rectInside = new Rect(insidePoint, insideSize);

            ////Calculation of x, y of co-ordinates and size for rect where will be arranged the child.
            temp = outsideThickness + borderThickness + insideThickness;
            Point point = new Point(temp, temp);
            Size size = new Size(insideSize.Width - insideThickness, insideSize.Height - insideThickness);

            Rect rect = new Rect(point, size);
            UIElement element = Child;

            if (element != null)
            {
                element.Arrange(rect);
            }

            if (m_adornerChild != null)
            {
                m_adornerChild.Arrange(rect);
            }

            return arrangeSize;
        }

        /// <summary>
        /// Renders three rectangles.
        /// </summary>
        /// <param name="drawingContext">Context for drawing.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            double cornerRadiusX = CornerRadius.TopLeft;
            double cornerRadiusY = CornerRadius.BottomLeft;
            int cornerRadiusOffset = (cornerRadiusX > 0 && cornerRadiusY > 0) ? CornerRadiusOffset : 0;

            if (m_outsidePen.Thickness > 0)
            {
                drawingContext.DrawRoundedRectangle(null, m_outsidePen, m_rectOutside, cornerRadiusX, cornerRadiusY);
            }

            if (m_borderPen.Thickness > 0)
            {
                drawingContext.DrawRoundedRectangle(null, m_borderPen, m_rectBorder, cornerRadiusX - cornerRadiusOffset, cornerRadiusY - cornerRadiusOffset);
            }

            if (m_insidePen.Thickness > 0 || Background != null)
            {
                drawingContext.DrawRectangle(Background, m_insidePen, m_rectInside);
            }
        }

        /// <summary>
        /// Measures the child element of a <see cref="TrippleBorder"/> to prepare
        /// for arranging it during the <see cref="ArrangeOverride(Size)"/> pass.
        /// </summary>
        /// <param name="constraint">An upper limit <see cref="Size"/> that should not be exceeded.</param>
        /// <returns>The target <see cref="Size"/> of the element.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            UIElement element = Child;
            Size sizeConstraint = new Size(0, 0);

            if (element != null)
            {
                element.Measure(sizeConstraint);
            }

            if (m_adornerChild != null)
            {
                m_adornerChild.Measure(sizeConstraint);
            }

            return sizeConstraint;
        }

        /// <summary>
        /// Gets the child Visual element at the specified index position.
        /// </summary>
        /// <param name="index">Index position of the child element.</param>
        /// <returns>
        /// The child element at the specified index position.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Index is greater than the number of visual child elements.
        /// </exception>
        protected override Visual GetVisualChild(int index)
        {
            UIElement element;

            switch (index)
            {
                case 0:
                    element = Child;
                    break;

                case 1:
                    element = m_adornerChild;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("index", index, "Index can be 0 or 1.");
            }

            return element;
        }

        /// <summary>
        /// Gets an enumerator that can be used to iterate the logical child elements of a <see cref="TrippleBorder"/>.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate the logical child elements of a <see cref="TrippleBorder"/>.
        /// </returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (Child != null)
                {
                    yield return Child;
                }

                if (m_adornerChild != null)
                {
                    yield return m_adornerChild;
                }
            }
        }

        /// <summary>
        /// Gets a value that is equal to the number of visual child elements of this instance of <see cref="TrippleBorder"/>.
        /// </summary>
        /// <returns>
        /// The number of visual child elements.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                int count = 0;

                if (Child != null)
                {
                    count++;
                }

                if (m_adornerChild != null)
                {
                    count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Calls OnBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrippleBorder instance = (TrippleBorder)d;

            instance.ChangePen(ref instance.m_borderPen, instance.BorderBrush, instance.BorderThickness);

            instance.OnBorderBrushChanged(e);
        }

        /// <summary>
        /// Calls OnBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrippleBorder instance = (TrippleBorder)d;

            instance.ChangePen(ref instance.m_borderPen, instance.BorderBrush, instance.BorderThickness);

            instance.OnBorderThicknessChanged(e);
        }

        /// <summary>
        /// Calls OnInsideBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInsideBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrippleBorder instance = (TrippleBorder)d;

            instance.ChangePen(ref instance.m_insidePen, instance.InsideBorderBrush, instance.InsideBorderThickness);

            instance.OnBorderBrushChanged(e);
        }

        /// <summary>
        /// Calls OnInsideBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInsideBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrippleBorder instance = (TrippleBorder)d;

            instance.ChangePen(ref instance.m_insidePen, instance.InsideBorderBrush, instance.InsideBorderThickness);

            instance.OnInsideBorderThicknessChanged(e);
        }

        /// <summary>
        /// Calls OnOutsideBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOutsideBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrippleBorder instance = (TrippleBorder)d;

            instance.ChangePen(ref instance.m_outsidePen, instance.OutsideBorderBrush, instance.OutsideBorderThickness);

            instance.OnOutsideBorderBrushChanged(e);
        }

        /// <summary>
        /// Calls OnOutsideBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOutsideBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrippleBorder instance = (TrippleBorder)d;

            instance.ChangePen(ref instance.m_outsidePen, instance.OutsideBorderBrush, instance.OutsideBorderThickness);

            instance.OnOutsideBorderThicknessChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises BorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BorderBrushChanged != null)
            {
                BorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises BorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BorderThicknessChanged != null)
            {
                BorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises InsideBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnInsideBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (InsideBorderBrushChanged != null)
            {
                InsideBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises InsideBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnInsideBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (InsideBorderThicknessChanged != null)
            {
                InsideBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises OutsideBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnOutsideBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (OutsideBorderBrushChanged != null)
            {
                OutsideBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises OutsideBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnOutsideBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (OutsideBorderThicknessChanged != null)
            {
                OutsideBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Changes pen properties: Brush and Thickness.
        /// </summary>
        /// <param name="pen">Pen that should be changed.</param>
        /// <param name="brush">New value for Pen Brush property.</param>
        /// <param name="thickness">New value for Pen Thickness property.</param>
        private void ChangePen(ref Pen pen, Brush brush, double thickness)
        {
            pen.Brush = brush;
            pen.Thickness = thickness;
        }
        #endregion

        #region	Events

        /// <summary>
        /// Event that is raised when <see cref="BorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="BorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="InsideBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback InsideBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="InsideBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback InsideBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="OutsideBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback OutsideBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="OutsideBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback OutsideBorderThicknessChanged;

        #endregion

        #region	Dependency properties

        /// <summary>
        /// Identifies <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TrippleBorder), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies <see cref="BorderThickness"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(TrippleBorder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBorderThicknessChanged)));

        /// <summary>
        /// Identifies <see cref="InsideBorderBrush"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty InsideBorderBrushProperty =
            DependencyProperty.Register("InsideBorderBrush", typeof(Brush), typeof(TrippleBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnInsideBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="InsideBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InsideBorderThicknessProperty =
            DependencyProperty.Register("InsideBorderThickness", typeof(double), typeof(TrippleBorder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnInsideBorderThicknessChanged)));

        /// <summary>
        /// Identifies <see cref="OutsideBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutsideBorderBrushProperty =
            DependencyProperty.Register("OutsideBorderBrush", typeof(Brush), typeof(TrippleBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnOutsideBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="OutsideBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutsideBorderThicknessProperty =
            DependencyProperty.Register("OutsideBorderThickness", typeof(double), typeof(TrippleBorder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnOutsideBorderThicknessChanged)));

        #endregion
    }
}
