// <copyright file="ObservableFrameworkElements.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class represents <see cref="ObservableFrameworkElements"/> of <see cref="FrameworkElement"/> instances.
    /// </summary>
    public class ObservableFrameworkElements : ObservableCollection<FrameworkElement>
    {
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="list">The list with framework element type.</param>
        public void AddRange(IEnumerable<FrameworkElement> list)
        {
            foreach (FrameworkElement item in list)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Gets the copy.
        /// </summary>
        /// <returns>Returns copy from items collection.</returns>
        public ObservableFrameworkElements GetCopy()
        {
            ObservableFrameworkElements copy = new ObservableFrameworkElements();

            foreach (FrameworkElement element in Items)
            {
                copy.Add(element);
            }

            return copy;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="list">The list with framework element type.</param>
        public void CopyTo(List<FrameworkElement> list)
        {
            list.Clear();

            foreach (FrameworkElement element in Items)
            {
                list.Add(element);
            }
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="list">The list with framework element type.</param>
        public void RemoveRange(IEnumerable<FrameworkElement> list)
        {
            foreach (FrameworkElement item in list)
            {
                Remove(item);
            }
        }
    }
}
