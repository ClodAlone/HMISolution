#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Identifies a type of changes made to a collection.
    /// </summary>
    public enum CollectionExChangeType
    {
        /// <summary>
        /// One or more items were inserted into collection.
        /// </summary>
        Insert,

        /// <summary>
        /// One or more items were removed from collection.
        /// </summary>
        Remove,

        /// <summary>
        /// All of the items were removed from collection.
        /// </summary>
        Clear,

        /// <summary>
        /// One of the collection items was modified.
        /// </summary>
        Set
    }
}
