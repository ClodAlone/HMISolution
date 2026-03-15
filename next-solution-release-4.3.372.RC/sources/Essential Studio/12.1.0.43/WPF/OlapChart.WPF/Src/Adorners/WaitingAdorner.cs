#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Represents WaitingAdorner.
    /// </summary>
    /// 
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class WaitingAdorner : Adorner
    {
        #region Members
        private UIElementCollection m_elements;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Raised when adornedElement is null.
        /// </exception>
        public WaitingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            m_elements = new UIElementCollection(this, this);
            Border border = new Border();
            border.Background = Brushes.Gray;
            border.Opacity = 0.7;
            WaitingControl waitingControl = new WaitingControl();
            border.Child = waitingControl;
            m_elements.Add(border);
        }
        #endregion

        #region Implementation
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

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in m_elements)
            {
                element.Arrange(new Rect(new Point(0, 0), finalSize));
            }
            return base.ArrangeOverride(finalSize);
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
            return m_elements[index];
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
            foreach (UIElement element in m_elements)
            {
                element.Measure(constraint);
            }
            return base.MeasureOverride(constraint);
        }
        #endregion
    }
}

