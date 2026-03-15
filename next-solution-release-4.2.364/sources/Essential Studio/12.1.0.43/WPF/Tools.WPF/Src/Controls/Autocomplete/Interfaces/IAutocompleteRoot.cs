// <copyright file="IAutocompleteRoot.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// An interface that is implemented by classes which are
    /// responsible for generating root for auto-complete.
    /// </summary>
    /// <property name="flag" value="Finished"/>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IAutocompleteRoot
    {
        /// <summary>
        /// Creates the filtered ghost.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="filterstring">The filterstring.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        AutocompleteItemCollection CreateFilteredGhost(IAutocompleteLevel level, string filterstring, StringMode mode, int index);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets root for this case.
        /// </summary>
        /// <param name="rootText">Current root text.</param>
        /// <returns>
        /// Root for this root text.
        /// </returns>
        IAutocompleteLevel GetRoot(string rootText);
    }
}