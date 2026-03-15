#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    
    /// <summary>
    /// An interface that is implemented by classes which are
    /// responsible for generating levels for auto-complete.
    /// </summary>
    /// <exclude/>
    public interface IAutocompleteLevel : IAutocompleteItem
    {
        /// <summary>
        /// Event that occurs when level's items were loaded. Invokes
        /// when items async loading completes.
        /// </summary>
        event EventHandler ItemsLoaded;

        /// <summary>
        /// Gets a value indicating whether this property indicates whether the level items were loaded.
        /// Gets true if items are already loaded, otherwise, false.
        /// </summary>
        bool IsItemsLoaded
        {
            get;
        }
        
        /// <summary>
        /// Gets this property contains level's splitter. Gets splitter this level.
        /// </summary>
        string Splitter
        {
            get;
        }
        
        /// <summary>
        /// Gets this property contains level's items. Gets items for this
        /// level.
        /// </summary>
        AutocompleteItemCollection Items
        {
            get;
        }
        
        /// <summary>
        /// Gets this property contains item's parent. Gets parent for this
        /// level.
        /// </summary>
        IAutocompleteLevel Parent
        {
            get;
        }

        /// <summary>
        /// This method synchronously loads items.
        /// </summary>
        void LoadItems();
  
        /// <summary>
        /// This method loads async items, runs parallel thread.
        /// </summary>
        void LoadItemsAsync();
        
        /// <summary>
        /// This method gets full way to this level.
        /// </summary>
        /// <returns>
        /// Full way to this level including all parents.
        /// </returns>
        string GetFullPath();
    }
}