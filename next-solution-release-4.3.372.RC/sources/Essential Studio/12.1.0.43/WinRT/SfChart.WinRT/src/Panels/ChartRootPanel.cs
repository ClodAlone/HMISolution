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
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the panel where all the child elements of Chart will be arranged.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartRootPanel: Panel
    {
        internal ChartBase Area { get; set; }

        /// <summary>
        /// Gets measure priority for this obj.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetMeasurePriorityIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(MeasurePriorityIndexProperty);
        }

        /// <summary>
        /// Sets the measure priority for this obj.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMeasurePriorityIndex(DependencyObject obj, int value)
        {
            obj.SetValue(MeasurePriorityIndexProperty, value);
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for MeasurePriority.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MeasurePriorityIndexProperty =
            DependencyProperty.RegisterAttached("MeasurePriorityIndex", typeof(int), typeof(ChartRootPanel), new PropertyMetadata(0));
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            var elements = new List<UIElement>();

            Size size = ChartLayoutUtils.CheckSize(availableSize);

            if (Area != null)
            {
                Area.RootPanelDesiredSize = size;
            }

            foreach (UIElement element in Children)
            {
                elements.Add(element);
            }

            IEnumerable<UIElement> uiElements = elements.OrderBy(GetMeasurePriorityIndex);

            foreach (UIElement element in uiElements)
            {
                element.Measure(availableSize);
            }

            return size;
        }
        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Children[i].Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            return finalSize;
        }
    }
}
