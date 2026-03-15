#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the IEnumerableExtensions class.
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Maxes the or default.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The default double value.</returns>
        internal static double MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, double defaultValue)
        {
            if (source.Any<TSource>())
            {
                return source.Max<TSource>(selector);
            }

            return defaultValue;
        }
    }
}
