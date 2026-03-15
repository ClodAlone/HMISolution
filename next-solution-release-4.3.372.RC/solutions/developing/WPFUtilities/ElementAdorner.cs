using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Utilities
{
    /// <summary>
    /// An adorner that can display one and only one UIElement.  
    /// That element can be a panel, which contains multiple other elements.
    /// The element is added to the adorner's visual and logical trees, enabling it to 
    /// particpate in dependency property value inheritance, amongst other things.
    /// </summary>
    public class ElementAdorner : Adorner
    {
        #region Data

        UIElement _child = null;
        double _offsetLeft = 0;
        double _offsetTop = 0;

        #endregion // Data

        #region Constructor

        /// <summary>
        /// Constructor.  Adds 'childElement' to the adorner's visual and logical trees.
        /// </summary>
        /// <param name="adornedElement">The element to which the adorner will be bound.</param>
        /// <param name="childElement">The element to be displayed in the adorner.</param>
        public ElementAdorner(UIElement adornedElement, UIElement childElement)
            : base(adornedElement)
        {
            if (childElement == null)
                throw new ArgumentNullException("childElement");

            _child = childElement;
            this.AddLogicalChild(childElement);
            this.AddVisualChild(childElement);
        }

        #endregion // Constructor

        #region Public Interface

        #region Child

        /// <summary>
        /// Returns the child element hosted in the adorner.
        /// </summary>
        public UIElement Child
        {
            get { return _child; }
        }

        #endregion // Child

        #region GetDesiredTransform

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_offsetLeft, _offsetTop));
            return result;
        }

        #endregion // GetDesiredTransform

        #region OffsetLeft

        /// <summary>
        /// Gets/sets the horizontal offset of the adorner.
        /// </summary>
        public double OffsetLeft
        {
            get { return _offsetLeft; }
            set
            {
                _offsetLeft = value;
                UpdateLocation();
            }
        }

        #endregion // OffsetLeft

        #region SetOffsets

        /// <summary>
        /// Updates the location of the adorner in one atomic operation.
        /// </summary>
        public void SetOffsets(double left, double top)
        {
            _offsetLeft = left;
            _offsetTop = top;
            this.UpdateLocation();
        }

        #endregion // SetOffsets

        #region OffsetTop

        /// <summary>
        /// Gets/sets the vertical offset of the adorner.
        /// </summary>
        public double OffsetTop
        {
            get { return _offsetTop; }
            set
            {
                _offsetTop = value;
                UpdateLocation();
            }
        }

        #endregion // OffsetTop

        #endregion // Public Interface

        #region Protected Overrides

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(_child);
                return list.GetEnumerator();
            }
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        /// <summary>
        /// Override.  Always returns 1.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        #endregion // Protected Overrides

        #region Private Helpers

        void UpdateLocation()
        {
            AdornerLayer adornerLayer = base.Parent as AdornerLayer;
            if (adornerLayer != null)
                adornerLayer.Update(base.AdornedElement);
        }

        #endregion // Private Helpers
    }
}