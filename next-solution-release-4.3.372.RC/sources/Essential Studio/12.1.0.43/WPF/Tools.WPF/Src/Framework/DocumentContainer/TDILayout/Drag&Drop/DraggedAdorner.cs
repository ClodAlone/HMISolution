// <copyright file="DraggedAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Dragged Adorner class
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DraggedAdorner : Adorner
    {
        #region Private members
        /// <summary>
        /// Represents contentPresenter
        /// </summary>
        internal ContentPresenter m_contentPresenter;
        
        /// <summary>
        /// Represents left
        /// </summary>
        private double m_left;
        
        /// <summary>
        /// Represents top
        /// </summary>
        private double m_top;
        
        /// <summary>
        /// Represents adornerLayer
        /// </summary>
        private AdornerLayer m_adornerLayer;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="DraggedAdorner"/> class.
        /// </summary>
        /// <param name="dragDropData">The drag drop data.</param>
        /// <param name="dragDropTemplate">The drag drop template.</param>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="adornerLayer">The adorner layer.</param>
        public DraggedAdorner(object dragDropData, DataTemplate dragDropTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            m_adornerLayer = adornerLayer;

            m_contentPresenter = new ContentPresenter
            {
                ////DataContext = dragDropData,
                Content = dragDropData,
                ContentTemplate = dragDropTemplate,
                Opacity = 0.7,
                Width = 75,
                Height = 75,
            };

            adornerLayer.Add(this);
        }
        #endregion

        #region Proeprties
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
                return 1;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns a <see cref="T:System.Windows.Media.Transform"/> for the adorner, based on the transform that is currently applied to the adorned element.
        /// </summary>
        /// <param name="transform">The transform that is currently applied to the adorned element.</param>
        /// <returns>A transform to apply to the adorner.</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(m_left, m_top));

            return result;
        }
        
        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="left">The contentPresenter left.</param>
        /// <param name="top">The contentPresenter top.</param>
        public void SetPosition(double left, double top)
        {
            m_left = left + m_contentPresenter.ActualWidth;
            m_top = top;

            if (m_adornerLayer != null)
            {
                m_adornerLayer.Update(AdornedElement);
            }
        }
        
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            m_adornerLayer.Remove(this);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> object representing the amount of layout space needed by the adorner.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            m_contentPresenter.Measure(constraint);
            return m_contentPresenter.DesiredSize;
        }
        
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }
        
        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return m_contentPresenter;
        }
        #endregion
    }
}