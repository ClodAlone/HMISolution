// <copyright file="QuickAccessToolBarPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Items panel for QuickAccessToolBar layout.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class QuickAccessToolBarPanel : Panel
    {
        #region Constants
        /// <summary>
        /// Left offset of bottom line of geometry.
        /// </summary>
        private  double LeftGeometryOffset = 10;

        /// <summary>
        /// Right offset of geometry.
        /// </summary>
        private  double RightGeometryOffset = 12;

        /// <summary>
        /// Left child offset.
        /// </summary>
        private const double LeftChildOffset = 3;

        /// <summary>
        /// Right child offset.
        /// </summary>
        private const double RightChildOffset = 3;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickAccessToolBarPanel"/> class.
        /// </summary>
        public QuickAccessToolBarPanel()
        {
        }
        #endregion

        #region Private members
        /// <summary>
        /// QuickAccessToolBar control.
        /// </summary>
        private QuickAccessToolBar m_quickAccess = null;
        #endregion

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (SkinStorage.GetVisualStyle(this).Contains("Office2010") || SkinStorage.GetVisualStyle(this).Contains("Office2013") || SkinStorage.GetVisualStyle(this).Contains("Windows8")  )
            {
                this.LeftGeometryOffset = 1;
                this.RightGeometryOffset = 1;
            }
            else
            {
                this.LeftGeometryOffset = 10;
                this.RightGeometryOffset = 12;
            }

            if (m_quickAccess == null)
            {
                m_quickAccess = (QuickAccessToolBar)VisualUtils.FindAncestor(this, typeof(QuickAccessToolBar));
            }

            if (m_quickAccess.Items.Count == 0)
            {
                m_quickAccess.VisibleItemsCount = 0;
                return new Size();
            }

            int visibleCount = 0;
            Size size = availableSize;

            double width = HasGeometry ? LeftGeometryOffset + LeftChildOffset + RightChildOffset + RightGeometryOffset : 0;
            if (this.QuickAccessToolBar != null && this.QuickAccessToolBar.ItemsSource != null)
            {
                if (this.QuickAccessToolBar.DefaultItems == null)
                {
                    this.QuickAccessToolBar.m_defaultItems = new List<UIElement>();
                }
                else
                {
                    this.QuickAccessToolBar.m_defaultItems.Clear();
                }
            }
            foreach (var elem in m_quickAccess.Items)
            {
                FrameworkElement child;
                child = elem as FrameworkElement;
                if (m_quickAccess.ItemsSource != null)
                {
                    if (child == null)
                        child = this.m_quickAccess.ItemContainerGenerator.ContainerFromItem(elem) as FrameworkElement;
                    if (child == null && this.m_quickAccess.m_mainItemsControl != null)
                    {
                        child = this.m_quickAccess.m_mainItemsControl.ItemContainerGenerator.ContainerFromItem(elem) as FrameworkElement;
                        if (child != null)
                        {                           
                                FrameworkElement felem = (child as ContentPresenter).ContentTemplate.LoadContent() as FrameworkElement;
                                if (!this.m_quickAccess.DefaultItems.Contains(felem as UIElement))
                                    this.m_quickAccess.DefaultItems.Add(felem as UIElement);                            
                        }
                    }
                }
                if (child != null)
                {
                    child.Measure(availableSize);

                    if (child.DesiredSize.Width == 0.0 && (child as UIElement) != null && (child as UIElement).Visibility == Visibility.Visible)
                    {
                        //SU I71890_I82316
                        ThreadStart thread = new ThreadStart(InvalidateMeasure);
                        Dispatcher.BeginInvoke(thread, DispatcherPriority.Background);
                        //EU I71890_I82316
                    }

                    if (width + child.DesiredSize.Width <= availableSize.Width)
                    {
                        visibleCount++;
                        width += child.DesiredSize.Width;
                    }
                    else
                    {
                        break;
                    }
                }

                size.Width = Math.Min(availableSize.Width, width);
                m_quickAccess.VisibleItemsCount = visibleCount;
            }
            return size;
        }

        /// <summary>
        /// Gets the quick access tool bar.
        /// </summary>
        /// <value>The quick access tool bar.</value>
        internal QuickAccessToolBar QuickAccessToolBar
        {
            get
            {
                return m_quickAccess;
            }
        }

        /// <summary>
        /// Arranges children.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (SkinStorage.GetVisualStyle(this).Contains("Office2010") || SkinStorage.GetVisualStyle(this).Contains("Office2013")  || SkinStorage.GetVisualStyle(this).Contains("Windows8") )
            {
                this.LeftGeometryOffset = 1;
                this.RightGeometryOffset = 1;
            }
            else
            {
                this.LeftGeometryOffset = 10;
                this.RightGeometryOffset = 12;
            }

            UIElementCollection internalChildren = base.InternalChildren;
            if (internalChildren.Count == 0)
            {
                return new Size();
            }

            double leftGeometryOffset = HasGeometry ? LeftGeometryOffset : 0;
            double leftChildOffset = HasGeometry ? LeftChildOffset : 0;
            double rightGeometryOffset = HasGeometry ? RightGeometryOffset : 0;
            double rightChildOffset = HasGeometry ? RightChildOffset : 0;

            double widthAvailable = finalSize.Width;

            double currentWidth = leftGeometryOffset + leftChildOffset;

            Size size = new Size();
            size.Height = finalSize.Height;

            foreach (UIElement child in internalChildren)
            {
                double height = child.DesiredSize.Height;
                double width = child.DesiredSize.Width;

                double d1 = Math.Ceiling(widthAvailable - currentWidth - rightGeometryOffset - rightChildOffset);
                double d2 = Math.Round(width);

                if (d1 >= d2)
                {
                    child.Arrange(new Rect(currentWidth, 1, width, height));

                    currentWidth += width;
                }
            }

            size.Width = currentWidth + rightGeometryOffset + rightChildOffset;

            return size;
        }

        /// <summary>
        /// Draws the content of a DrawingContext object during the
        /// render pass of a Panel element.
        /// </summary>
        /// <param name="drawingContext">The DrawingContext object to
        /// draw.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (HasGeometry)
            {
                if (!SkinStorage.GetVisualStyle(this).Contains ("Office2010"))
                {
                    double x = 0;
                    double width = this.RenderSize.Width;
                    double height = this.RenderSize.Height;

                PathGeometry pathGeometry = new PathGeometry();

                PathFigure pathFigure = new PathFigure();
                pathFigure.StartPoint = new Point(x + LeftGeometryOffset, height);
                pathFigure.IsClosed = true;

                ArcSegment leftArcSegment = new ArcSegment();
                leftArcSegment.Size = new Size(40, 30);
                leftArcSegment.Point = new Point(x, x);

                LineSegment lineSegment = new LineSegment();
                lineSegment.Point = new Point(width - RightGeometryOffset, x);

                ArcSegment rightArcSegment = new ArcSegment();
                rightArcSegment.Size = new Size(1, 1);
                rightArcSegment.Point = new Point(width - RightGeometryOffset, height);
                rightArcSegment.SweepDirection = SweepDirection.Clockwise;

                pathFigure.Segments.Add(leftArcSegment);
                pathFigure.Segments.Add(lineSegment);
                pathFigure.Segments.Add(rightArcSegment);

                pathGeometry.Figures.Add(pathFigure);

                    drawingContext.DrawGeometry(GeometryBackground, new Pen(GeometryStroke, GeometryStrokeThickness), pathGeometry);
                }
            }

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Calls OnHasGeometryChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnHasGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBarPanel instance = (QuickAccessToolBarPanel)d;
            instance.OnHasGeometryChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HasGeometryChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHasGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasGeometryChanged != null)
            {
                HasGeometryChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGeometryBackgroundChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGeometryBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBarPanel instance = (QuickAccessToolBarPanel)d;
            instance.OnGeometryBackgroundChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// GeometryBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnGeometryBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GeometryBackgroundChanged != null)
            {
                GeometryBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGeometryStrokeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGeometryStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBarPanel instance = (QuickAccessToolBarPanel)d;
            instance.OnGeometryStrokeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises GeometryStrokeChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnGeometryStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GeometryStrokeChanged != null)
            {
                GeometryStrokeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGeometryStrokeThicknessChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGeometryStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBarPanel instance = (QuickAccessToolBarPanel)d;
            instance.OnGeometryStrokeThicknessChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// GeometryStrokeThicknessChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnGeometryStrokeThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GeometryStrokeThicknessChanged != null)
            {
                GeometryStrokeThicknessChanged(this, e);
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance has geometry.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has geometry; otherwise, <c>false</c>.
        /// </value>
        public bool HasGeometry
        {
            get
            {
                return (bool)GetValue(HasGeometryProperty);
            }

            set
            {
                SetValue(HasGeometryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the GeometryBackground dependency
        /// property.
        /// </summary>
        public Brush GeometryBackground
        {
            get
            {
                return (Brush)GetValue(GeometryBackgroundProperty);
            }

            set
            {
                SetValue(GeometryBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the GeometryStroke dependency
        /// property.
        /// </summary>
        public Brush GeometryStroke
        {
            get
            {
                return (Brush)GetValue(GeometryStrokeProperty);
            }

            set
            {
                SetValue(GeometryStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the GeometryStrokeThickness
        /// dependency property.
        /// </summary>
        public double GeometryStrokeThickness
        {
            get
            {
                return (double)GetValue(GeometryStrokeThicknessProperty);
            }

            set
            {
                SetValue(GeometryStrokeThicknessProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when HasGeometry property is changed.
        /// </summary>
        public event PropertyChangedCallback HasGeometryChanged;

        /// <summary>
        /// Event that is raised when GeometryBackground property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback GeometryBackgroundChanged;

        /// <summary>
        /// Event that is raised when GeometryStroke property is changed.
        /// </summary>
        public event PropertyChangedCallback GeometryStrokeChanged;

        /// <summary>
        /// Event that is raised when GeometryStrokeThickness property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback GeometryStrokeThicknessChanged;
        #endregion

        #region Dependency Properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines when panel has geometry. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HasGeometryProperty =
            DependencyProperty.Register("HasGeometry", typeof(bool), typeof(QuickAccessToolBarPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnHasGeometryChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines geometry stroke color. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryStrokeProperty =
            DependencyProperty.Register("GeometryStroke", typeof(Brush), typeof(QuickAccessToolBarPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGeometryStrokeChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines geometry background color or gradient. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryBackgroundProperty =
            DependencyProperty.Register("GeometryBackground", typeof(Brush), typeof(QuickAccessToolBarPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGeometryBackgroundChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines geometry stroke thickness. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryStrokeThicknessProperty =
            DependencyProperty.Register("GeometryStrokeThickness", typeof(double), typeof(QuickAccessToolBarPanel), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGeometryStrokeThicknessChanged)));
        #endregion
    }
}