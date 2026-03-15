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
    /// responsible for generating items for auto-complete.
    /// </summary>
    /// <exclude/>
    public interface IAutocompleteItem
    {
        /// <summary>
        /// Gets this property contains item's text. Gets item text.
        /// </summary>
        string Text
        {
            get;
        }
    }
}