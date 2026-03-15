// <copyright file="IAutocompleteItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// An interface that is implemented by classes which are
    /// responsible for generating items for auto-complete.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IAutocompleteItem
    {
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets item text. This property contains item's text.
        /// </summary>
        string Text
        {
            get;
        }
    }
}