// <copyright file="TemplatedAdornerBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Base class for the adorners with templates support.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TemplatedAdornerBase : Adorner, IDisposable
    {
        #region Private Members

        /// <summary>
        /// Control that is the only child of the adorner and represents adorner.
        /// </summary>
        private readonly TemplatedAdornerInternalControl m_innerControl;

        /// <summary>
        /// Cached value of the <see cref="OffsetX"/> property.
        /// </summary>
        private double m_offsetX = 0d;

        /// <summary>
        /// Cached value of the OffsetY property.
        /// </summary>
        private double m_offsetY = 0d;

        #endregion

        #region Properties

        /// <summary>
        /// Gets <see cref="TemplatedAdornerInternalControl"/> that represents inner control.
        /// </summary>
        protected TemplatedAdornerInternalControl InnerControl
        {
            get
            {
                return m_innerControl;
            }
        }

        /// <summary>
        /// Gets desired size of the internal control.
        /// </summary>
        public Size DesiredSizeInternal
        {
            get
            {
                return m_innerControl.DesiredSize;
            }
        }

        /// <summary>
        /// Gets or sets the value that describes horizontal offset of adorner.
        /// </summary>
        public double OffsetX
        {
            get
            {
                return m_offsetX;
            }

            set
            {
                SetValue(OffsetXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes vertical offset of adorner.
        /// </summary>
        public double OffsetY
        {
            get
            {
                return m_offsetY;
            }

            set
            {
                SetValue(OffsetYProperty, value);
            }
        }

        /// <summary>
        /// Gets an enumerator for logical child elements.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                yield return m_innerControl;
            }
        }

        #endregion

        #region Class Initialization

        /// <summary>
        /// Initializes static members of the TemplatedAdornerBase class.
        /// </summary>
        static TemplatedAdornerBase()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(TemplatedAdornerBase), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalAlignmentProperty.OverrideMetadata(typeof(TemplatedAdornerBase), new FrameworkPropertyMetadata(VerticalAlignment.Top));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatedAdornerBase"/> class.
        /// </summary>
        /// <param name="adornedElement">The actual adorned element</param>
        public TemplatedAdornerBase(UIElement adornedElement)
            : base(adornedElement)
        {
            if (adornedElement != null)
            {
                m_innerControl = new TemplatedAdornerInternalControl(this);
                AddLogicalChild(m_innerControl);
                AddVisualChild(m_innerControl);
            }
            else
            {
                throw new ArgumentNullException("adornedElement");
            }
        }

        /// <summary>
        /// Dispose the element. 
        /// </summary>
        public void Dispose()
        {
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="OffsetX"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback OffsetXChanged;

        /// <summary>
        /// Event that is raised when <see cref="OffsetY"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback OffsetYChanged;
        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies <see cref="OffsetX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(TemplatedAdornerBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnOffsetXChanged)));

        /// <summary>
        /// Identifies <see cref="OffsetY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(TemplatedAdornerBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnOffsetYChanged)));

        #endregion

        #region Class Overrides

        /// <summary>
        /// Calculates <see cref="Transform"/> for the adorner, based on the transform
        /// that is currently applied to the adorned element.
        /// </summary>
        /// <param name="transform">The transform that is currently applied to the adorned element.</param>
        /// <returns>A transform to apply to the adorner.</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup group = new GeneralTransformGroup();
            if(transform != null)
            group.Children.Add(transform);
            group.Children.Add(new TranslateTransform(OffsetX, OffsetY));

            return group;
        }

        /// <summary>
        /// Measures content.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>A <see cref="Size"/> value representing the amount of layout space needed by the adorner.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = constraint;
            if (double.IsInfinity(size.Height) || double.IsInfinity(size.Width))
            {
                size = AdornedElement.RenderSize;
            }

            m_innerControl.Measure(size);
            return AdornedElement.RenderSize;
        }

        /// <summary>
        /// Arranges inner control to the full size.
        /// </summary>
        /// <param name="finalSize">The final area that 
        /// this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_innerControl.Arrange(new Rect(finalSize));
            return m_innerControl.RenderSize;
        }

        /// <summary>
        /// Gets visual by index.
        /// </summary>
        /// <param name="index">Index of the child, the only valid value is 0.</param>
        /// <returns>Visual child.</returns>
        protected override Visual GetVisualChild(int index)
        {
            return m_innerControl;
        }

        /// <summary>
        /// Gets visual children count, always 1.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Calls OnOffsetXChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TemplatedAdornerBase instance = (TemplatedAdornerBase)d;
            instance.OnOffsetXChanged(e);
        }

        /// <summary>
        /// Calls OnOffsetYChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TemplatedAdornerBase instance = (TemplatedAdornerBase)d;
            instance.OnOffsetYChanged(e);
        }

        /// <summary>
        /// Raises OffsetXChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnOffsetXChanged(DependencyPropertyChangedEventArgs e)
        {
            m_offsetX = (double)e.NewValue;

            if (OffsetXChanged != null)
            {
                OffsetXChanged(this, e);
            }
        }

        /// <summary>
        /// Raises OffsetYChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnOffsetYChanged(DependencyPropertyChangedEventArgs e)
        {
            m_offsetY = (double)e.NewValue;

            if (OffsetYChanged != null)
            {
                OffsetYChanged(this, e);
            }
        }

        #endregion
    }
}