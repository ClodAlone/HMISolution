#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Data
{

#if WinRT
    public enum ListSortDirection
    {
        Ascending,
        Descending
    }
#endif
    public enum LiveDataUpdateMode
    {
        Default,
        AllowSummaryUpdate,
        AllowDataShaping
    }

#if !WPF
    // Summary:
    //     Specifies where the placeholder for a new item appears in the collection.
    public enum NewItemPlaceholderPosition
    {
        // Summary:
        //     The collection does not use a new item placeholder. The position of items
        //     that are added depends on the underlying collection. Usually, they are added
        //     at the end of the collection.
        None = 0,
        //
        // Summary:
        //     The placeholder for a new item appears at the beginning of the collection.
        //     New items are at the beginning of the collection, after the new item placeholder.
        AtBeginning = 1,
        //
        // Summary:
        //     The placeholder for a new item appears at the end of the collection. New
        //     items are added at the end of the collection, before the new item placeholder.
        AtEnd = 2,
    }
#endif
}
