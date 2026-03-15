// <copyright file="IAutocompleteLevel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// An interface that is implemented by classes which are
    /// responsible for generating levels for auto-complete.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IAutocompleteLevel : IAutocompleteItem
    {
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets a value indicating whether true if items are already loaded, otherwise, false. This property indicates whether the level items were loaded.
        /// </summary>
        Boolean IsItemsLoaded
        {
            get;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets splitter for this level.This property contains level's splitter.
        /// </summary>
        string Splitter
        {
            get;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets items for this level.This property contains level's items.
        /// </summary>
        AutocompleteItemCollection Items
        {
            get;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets parent for this level.This property contains item's parent.
        /// </summary>
        IAutocompleteLevel Parent
        {
            get;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that occurs when level's items were loaded. Invokes
        /// when items async loading completes.
        /// </summary>
        event EventHandler ItemsLoaded;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method synchronously loads items.
        /// </summary>
        void LoadItems();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method loads async items, runs parallel thread.
        /// </summary>
        void LoadItemsAsync();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method gets full way to this level.
        /// </summary>
        /// <returns>
        /// Full way to this level including all parents.
        /// </returns>
        string GetFullPath();
    }
}