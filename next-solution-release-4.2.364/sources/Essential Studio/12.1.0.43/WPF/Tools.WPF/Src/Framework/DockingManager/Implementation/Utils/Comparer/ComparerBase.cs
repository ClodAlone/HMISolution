// <copyright file="ComparerBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Base class for compare children indexes.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class ComparerBase : IComparer, IComparer<FrameworkElement>
    {
        #region Public methods
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero <paramref name="x"/> is less than <paramref name="y"/>. Zero <paramref name="x"/> equals <paramref name="y"/>. Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.-or- <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other. </exception>
        public int Compare(object x, object y)
        {
            return ((IComparer<FrameworkElement>)this).Compare((FrameworkElement)x, (FrameworkElement)y);
        }

        /// <summary>
        /// Compares the specified x element.
        /// </summary>
        /// <param name="xElement">The x element.</param>
        /// <param name="yElement">The y element.</param>
        /// <returns>return result index.</returns>
        public int Compare(FrameworkElement xElement, FrameworkElement yElement)
        {
            ValidateElements(xElement, yElement);

            int orderX = GetIndex(xElement);
            int orderY = GetIndex(yElement);
            return Compare(orderX, orderY);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return index.</returns>
        protected abstract int GetIndex(FrameworkElement element);

        /// <summary>
        /// Validates the elements.
        /// </summary>
        /// <param name="element1">The element1.</param>
        /// <param name="element2">The element2.</param>
        protected virtual void ValidateElements(FrameworkElement element1, FrameworkElement element2)
        {
            if (element1 == null || element2 == null)
            {
                throw new ArgumentNullException("Any parameter can not be null!");
            }
        }

        /// <summary>
        /// Calculates the result.
        /// </summary>
        /// <param name="orderX">The order X.</param>
        /// <param name="orderY">The order Y.</param>
        /// <returns>return result index.</returns>
        private static int Compare(int orderX, int orderY)
        {
            int result = 0;

            if (orderX > orderY)
            {
                result = 1;
            }
            else if (orderX < orderY)
            {
                result = -1;
            }

            return result;
        }
        #endregion
    }
}
