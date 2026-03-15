#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Interface of the objects that support caching of their internals.
    /// </summary>
    internal interface IPdfCache
    {
        /// <summary>
        /// Checks whether the object is similar to another object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True - if the objects have equal internals and can share them, False otherwise.</returns>
        bool EqualsTo(IPdfCache obj);

        /// <summary>
        /// Returns internals of the object.
        /// </summary>
        /// <returns>Returns internals of the object.</returns>
        IPdfPrimitive GetInternals();

        /// <summary>
        /// Sets internals to the object.
        /// </summary>
        /// <param name="internals">Internals of the object.</param>
        void SetInternals(IPdfPrimitive internals);
    }
}
