// <copyright file="FakeItemsAdorner.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the FakeItem Adorner
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FackItemsAdorner : Adorner
    {
        /// <summary>
        /// Presents the contentPresenter.
        /// </summary>
        private ContentPresenter contentPresenter;

        /// <summary>
        /// Presents the left offset value.
        /// </summary>
        private double leftOffset;

        /// <summary>
        /// Presents the top offset value.
        /// </summary>
        private double topOffset;

        /// <summary>
        /// Presents the adorner layer value.
        /// </summary>
        private AdornerLayer adornerLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FackItemsAdorner"/> class.
        /// </summary>
        /// <param name="dragDropData">The drag drop data.</param>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="adornerLayer">The adorner layer.</param>
        public FackItemsAdorner(FrameworkElement dragDropData, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            this.adornerLayer = adornerLayer;

            this.contentPresenter = new ContentPresenter();
            this.contentPresenter.Content = dragDropData;
            this.contentPresenter.Opacity = 1;

            this.adornerLayer.Add(this);
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="leftOffset">The left offset.</param>
        /// <param name="topOffset">The top offset.</param>
        public void SetPosition(double leftOffset, double topOffset)
        {
            this.leftOffset = leftOffset;
            this.topOffset = topOffset;
            if (this.adornerLayer != null)
            {
                this.adornerLayer.Update(this.AdornedElement);
            }
        }

        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> object representing the amount of layout space needed by the adorner.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.contentPresenter.Measure(constraint);
            return this.contentPresenter.DesiredSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.contentPresenter.Arrange(new Rect(finalSize));
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
            return this.contentPresenter;
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
            get { return 1; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.Windows.Media.Transform"/> for the adorner, based on the transform that is currently applied to the adorned element.
        /// </summary>
        /// <param name="transform">The transform that is currently applied to the adorned element.</param>
        /// <returns>A transform to apply to the adorner.</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.leftOffset, this.topOffset));

            return result;
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            this.adornerLayer.Remove(this);
        }
    }
}