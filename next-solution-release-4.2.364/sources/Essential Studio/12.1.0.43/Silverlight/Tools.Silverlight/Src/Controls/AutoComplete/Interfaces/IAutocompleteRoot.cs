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
    /// responsible for generating root for auto-complete.
    /// </summary>
    /// <exclude/>
    public interface IAutocompleteRoot
    {
        /// <summary>
        /// This method searches items which meet the requirements of the appropriate
        /// conditions.
        /// </summary>
        /// <param name="level">searches items which meet conditions using level</param>
        /// <param name="filterstring">filtering the string</param>
        /// <returns>
        /// Type : AutocompleteItemCollection
        /// </returns>
        AutocompleteItemCollection CreateFilteredGhost(IAutocompleteLevel level, string filterstring);
        
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