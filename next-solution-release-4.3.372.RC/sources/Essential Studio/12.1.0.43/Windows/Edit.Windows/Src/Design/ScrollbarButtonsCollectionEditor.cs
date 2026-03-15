#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.ComponentModel.Design;

using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Design
{
    /// <summary>
    /// Designer editor for ScrollbarButtonsCollection.
    /// </summary>
    public class ScrollbarButtonsCollectionEditor
    : CollectionEditor
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the ScrollbarButtonsCollectionEditor class .
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public ScrollbarButtonsCollectionEditor(Type type)
            : base(type)
        {
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Allow creation of ScrollbarButton instances in design time.
        /// </summary>
        /// <returns>An array of data types that this collection can contain.</returns>
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(ScrollbarButton) };
        }
        #endregion
    }
}
