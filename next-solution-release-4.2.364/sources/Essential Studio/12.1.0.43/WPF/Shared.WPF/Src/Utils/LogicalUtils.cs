// <copyright file="LogicalUtils.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class to find the logical elements using GetParent and GetChild methods.
    /// </summary>
    public sealed class LogicalUtils
    {
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns>Parent dependency object.</returns>
        public static DependencyObject GetParent(DependencyObject current)
        {
            return LogicalTreeHelper.GetParent(current);
        }

        /// <summary>
        /// Gets the root parent.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns>Root parent dependency object.</returns>
        public static DependencyObject GetRootParent(DependencyObject current)
        {
            DependencyObject root = current;

            while (current != null)
            {
                root = current;
                current = GetParent(current);
            }

            return root;
        }

        /// <summary>
        /// Gets the type of the parent of.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="type">The looking type.</param>
        /// <returns>Parent Dependency object with specific type.</returns>
        public static DependencyObject GetParentOfType(DependencyObject current, Type type)
        {
            while (current != null && !type.IsInstanceOfType(current))
            {
                current = GetParent(current);
            }

            return current;
        }
    }
}
