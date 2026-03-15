// <copyright file="FloatIndexComparer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is used to compare DockingManager children indexes in Float state.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FloatIndexComparer : ComparerBase
    {
        #region Implementation
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return index.</returns>
        protected override int GetIndex(FrameworkElement element)
        {
            return DockingManager.GetIndexInFloatMode(element);
        }
        #endregion
    }
}
