// <copyright file="Binder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Used as a binding helper. 
    /// </summary>
    public static class Binder
    {
        /// <summary>
        /// Creates a new binding to the given source path.
        /// </summary>
        /// <param name="source">The source element to bind to.</param>
        /// <param name="path">The path in the source element to bind to.</param>
        /// <returns>
        /// The binding to the given source path.
        /// </returns>
        public static Binding Bind(FrameworkElement source, string path)
        {
            Binding binding = new Binding(path);
            binding.Source = source;
            return binding;
        }

        /// <summary>
        /// Creates a new binding to the given source path.
        /// </summary>
        /// <param name="source">The source element to bind to.</param>
        /// <param name="path">The path in the source element to bind to.</param>
        /// <returns>
        /// The binding to the given source path.
        /// </returns>
        public static Binding Bind<T>(T source, string path)
        {
            Binding binding = new Binding(path);
            binding.Source = source;
            return binding;
        }
    }
}
