using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace UFWebClient
{
    public static class LogicalTreeHelper
    {
        /// <summary>
        /// Retrieves all the logical children of a framework element using a 
        /// depth-first search.  A visual element is assumed to be a logical 
        /// child of another visual element if they are in the same namescope.
        /// For performance reasons this method manually manages the stack 
        /// instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The logical children of the framework element.</returns>
        internal static IEnumerable<FrameworkElement> GetLogicalChildren(this FrameworkElement parent)
        {
            string parentName = parent.Name;
            Stack<FrameworkElement> stack =
                new Stack<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());

            while (stack.Count > 0)
            {
                FrameworkElement element = stack.Pop();
                if (element.FindName(parentName) == parent)
                {
                    yield return element;
                }
                else
                {
                    foreach (FrameworkElement visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                    {
                        stack.Push(visualChild);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all the visual children of a framework element.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The visual children of the framework element.</returns>
        internal static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }


    }
}
