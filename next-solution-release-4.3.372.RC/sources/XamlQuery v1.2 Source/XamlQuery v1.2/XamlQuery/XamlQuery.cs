//XamlQuery.cs

/*
 * XamlQuery Silverlight Library v1.2
 * http://XamlQuery.codeplex.com/
 * 
 * Author: cincoutprabu (cincoutprabu@gmail.com)
 * Copyright 2010 cincoutprabu. All rights reserved.
 * 
 * Released under Microsoft Public License.
 * http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 *
 * Date: 2010-Sep-10 06:00 IST
 * Revision: 1
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using XamlQuery.XamlSelector;

namespace XamlQuery
{
    /// <summary>
    /// Provides methods to find parent and child controls in a rendered Silverlight page.
    /// </summary>
    public class XamlQuery
    {
        #region Parent-Methods

        /// <summary>
        /// Finds all parents of a control, by traversing the visual tree upwards.
        /// </summary>
        /// <param name="control">Dependency object whose parents are found.</param>
        /// <returns>The ControlSet object containing all the parents from given control to root of the rendered Silverlight page.</returns>
        public static ControlSet AllParents(DependencyObject control)
        {
            ControlSet allParents = new ControlSet();

            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(control);
                if (parent == null) break;
                else
                {
                    allParents.Add(parent);
                    control = parent;
                }
            }

            return (allParents);
        }

        /// <summary>
        /// Finds all parents of a control, until the parent with a specified name is found.
        /// </summary>
        /// <param name="control">Dependency object whose parents are found.</param>
        /// <param name="parentName">The name of the parent control where the traversal stops.</param>
        /// <returns>The ControlSet object containing all the parents found.</returns>
        public static ControlSet ParentsUpto(DependencyObject control, string parentName)
        {
            ControlSet allParents = new ControlSet();

            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(control);
                if (parent == null) break;
                else if (parent is FrameworkElement && ((FrameworkElement)parent).Name.Equals(parentName))
                {
                    allParents.Add(parent);
                    break;
                }
                else
                {
                    allParents.Add(parent);
                    control = parent;
                }
            }

            return (allParents);
        }

        /// <summary>
        /// Finds all parents of a control, until a parent of specified type is found.
        /// </summary>
        /// <typeparam name="T">The type of parent control where the traversal stops.</typeparam>
        /// <param name="control">Dependency object whose parents are found.</param>
        /// <returns>The ControlSet object containing all the parents found.</returns>
        public static ControlSet ParentsUpto<T>(DependencyObject control)
        {
            ControlSet allParents = new ControlSet();

            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(control);
                if (parent == null) break;
                else if (parent is T)
                {
                    allParents.Add(parent);
                    break;
                }
                else
                {
                    allParents.Add(parent);
                    control = parent;
                }
            }

            return (allParents);
        }

        /// <summary>
        /// Finds the first parent of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the parent to be found.</typeparam>
        /// <param name="control">Dependency object whose parent is found.</param>
        /// <returns>The parent control found; null if not found.</returns>
        public static DependencyObject ParentByType<T>(DependencyObject control)
        {
            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(control);
                if (parent is T) return (parent);
                else if (parent == null) return (null);
                control = parent;
            }
        }

        /// <summary>
        /// Finds all parents of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the parents to be found.</typeparam>
        /// <param name="control">Dependency object whose parents are found.</param>
        /// <returns>The ControlSet object containing all the parents found.</returns>
        public static ControlSet ParentsByType<T>(DependencyObject control)
        {
            return (AllParents(control).FilterByType<T>());
        }

        /// <summary>
        /// Finds the first parent with the specified name.
        /// </summary>
        /// <param name="control">Dependency object whose parent is found.</param>
        /// <param name="parentName">Name of the parent to be found.</param>
        /// <returns>The parent control found; null if not found.</returns>
        public static DependencyObject ParentByName(DependencyObject control, string parentName)
        {
            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(control);
                if (parent is FrameworkElement && ((FrameworkElement)parent).Name.Equals(parentName)) return (parent);
                else if (parent == null) return (null);
                control = parent;
            }
        }

        /// <summary>
        /// Finds the root control of the rendered Silverlight page, by traversing the visual tree upwards.
        /// </summary>
        /// <param name="control">Dependency object from where the traversing starts.</param>
        /// <returns>The root control of the rendered Silverlight page. (Always returns the outermost Page/UserControl object. That is, if a Page/UserControl is embedded within another Page/UserControl, the later is returned as root.)</returns>
        public static DependencyObject Root(DependencyObject control)
        {
            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(control);
                if (parent == null) return (parent);
                control = parent;
            }
        }

        #endregion

        #region Children-Methods

        /// <summary>
        /// Finds all child controls of a given control. The given control need not to be a container control like Panel or Grid. It can be of primitive types also (like ContentControl, Selector), because the rendered Silverlight output can contain any control within any other control (through the use of data-templates and control-templates). For example, there can be a Grid control inside the control-template of a ContentControl.
        /// </summary>
        /// <param name="container">Dependency object whose children are found.</param>
        /// <returns>The ControlSet object containing all the children of the given control.</returns>
        public static ControlSet All(DependencyObject container)
        {
            ChildrenFinder childrenFinder = new ChildrenFinder();
            childrenFinder.All(container);
            return (childrenFinder.Children);
        }

        /// <summary>
        /// Finds all child controls of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the children required.</typeparam>
        /// <param name="container">Dependency object whose children are found.</param>
        /// <returns>The ControlSet object containing the children found.</returns>
        public static ControlSet ByType<T>(DependencyObject container)
        {
            ChildrenFinder childrenFinder = new ChildrenFinder();
            childrenFinder.FindByType<T>(container);
            return (childrenFinder.Children);
        }

        /// <summary>
        /// Finds all child controls whose property value is equal to a given value.
        /// </summary>
        /// <param name="container">Dependency object whose children are found.</param>
        /// <param name="property">Dependency property whose value is used for finding.</param>
        /// <param name="value">The required value of the given dependency property. The children whose property value is equal to this value are chosen.</param>
        /// <returns>The ControlSet object containing the children found.</returns>
        public static ControlSet ByProperty(DependencyObject container, DependencyProperty property, object value)
        {
            return (ByProperty(container, property, value, FilterType.Equals));
        }

        /// <summary>
        /// Finds all child controls whose property value matches a specified criteria. The criteria is defined by the "filterType" parameter. For "Equal" and "NotEqual" filters, the "Equals()" method of the property is used for comparison and for other filters ("StartsWith", "EndsWith", "Contains") the string representation of the property value is used for comparison.
        /// </summary>
        /// <param name="container">Dependency object whose children are found.</param>
        /// <param name="property">Dependency property whose value is used for finding.</param>
        /// <param name="value">The matching value of the given dependency property. This value is matched with all child controls of the given control.</param>
        /// <param name="filterType">The filter-type enumeration value that defines the search criteria.</param>
        /// <returns>The ControlSet object containing the children found.</returns>
        public static ControlSet ByProperty(DependencyObject container, DependencyProperty property, object value, FilterType filterType)
        {
            return (All(container).FilterByProperty(property, value, filterType));
        }

        #endregion

        #region XamlSelector-Methods

        internal static void InspectQuery(string query)
        {
            List<string> queryLog = new List<string>();
            string tab = "\t";

            List<Selector> allSelectors = Parser.Parse(query);
            queryLog.Add(allSelectors.Count + " selectors");

            for (int index = 0; index < allSelectors.Count; index++)
            {
                Selector selector = allSelectors[index];
                queryLog.Add("selector" + index + ": query: " + selector.Query);

                if (selector == null)
                {
                    queryLog.Add(tab + "null");
                }
                else
                {
                    queryLog.Add(selector.SimpleSelectors.Count + " simple-selectors");

                    foreach (SimpleSelector simpleSelector in selector.SimpleSelectors)
                    {
                        queryLog.Add(tab + "simple-selector: type: " + simpleSelector.Type + ", query: " + simpleSelector.Query);
                        queryLog.Add(tab + "simple-selector-maintoken: type: " + simpleSelector.MainToken.Type + ", content: [" + simpleSelector.MainToken.Content + "]");
                        queryLog.Add(tab + simpleSelector.FilterSelectors.Count + " filter-selectors");

                        foreach (FilterSelector filterSelector in simpleSelector.FilterSelectors)
                        {
                            queryLog.Add(tab + tab + "filter-selector: type: " + filterSelector.Type + ", query: " + filterSelector.Query);

                            foreach (Token token in filterSelector.Tokens)
                            {
                                queryLog.Add(tab + tab + tab + "token: type:" + token.Type + ", content: [" + token.Content + "]");
                            }
                        }
                    }

                    queryLog.Add(selector.Combinators.Count + " combinators");
                    foreach (Combinator combinator in selector.Combinators)
                    {
                        queryLog.Add(tab + "combinator: type: " + combinator.Type + ", token: " + combinator.Token.Content);
                    }
                }

                queryLog.Add(string.Empty);
            }
            Common.Show(string.Join("\r\n", queryLog.ToArray()));
        }

        /// <summary>
        /// Searches the children of a control using the specified CSS selector query.
        /// </summary>
        /// <param name="sourceControl">The control whose visual children are searched. It is important to search in a lesser scope in order to prevent including unwanted controls in search result, because Silverlight introduces many controls dynamically that are not part of the original XAML markup. For example, if you need to search inside a Canvas, pass that Canvas control instead of passing root element (LayoutRoot) as argument.</param>
        /// <param name="query">The CSS selector query.</param>
        /// <returns>The ControlSet object containing the matched controls.</returns>
        public static ControlSet Search(DependencyObject sourceControl, string query)
        {
            Common.ClearLog();
            //Common.TraceLog = true;
            //InspectQuery(query);

            ControlSet matchedControls = new ControlSet();
            List<Selector> allSelectors = Parser.Parse(query);
            foreach (Selector selector in allSelectors)
            {
                ControlSet selectorControls = selector.Execute(sourceControl);
                matchedControls.AddRange(selectorControls);
            }

            //Common.Show(matchedControls.Count + " matched-controls");
            //Common.Show(string.Join("\r\n", (from control in matchedControls select control.ToString()).ToArray()));
            //Common.Show(string.Join("\r\n", Common.Log.ToArray()));
            return (matchedControls);
        }

        #endregion
    }
}
