#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.Text;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of entities.
    /// </summary>
    internal class Range : CollectionImpl
    {
        #region Properties
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        internal IList Items
        {
            get
            {
                return this.InnerList;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        internal Range(WordDocument doc, OwnerHolder owner)
            : base(doc, owner)
        {
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Clones the items to new collection.
        /// </summary>
        /// <param name="items">The items.</param>
        internal void CloneItemsTo(Range range)
        {
            for (int i = 0, cnt = Count; i < cnt; i++)
            {
                Entity item = (InnerList[i] as Entity).Clone();
                if (item != null)
                {
                    range.Items.Add(item);
                }
            }
        }
        /// <summary>
        /// Contains the text body items.
        /// </summary>
        /// <returns></returns>
        internal bool ContainTextBodyItems()
        {
            bool hasTextBodyItems = false;
            foreach (Entity entity in Items)
            {
                if (entity is WParagraph
                    || entity is WTable)
                {
                    hasTextBodyItems = true;
                    break;
                }
            }
            return hasTextBodyItems;
        }
        /// <summary>
        /// Lasts the index of the paragraph item.
        /// </summary>
        /// <returns></returns>
        internal int GetLastParagraphItemIndex()
        {
            int index = 0;
            foreach (Entity entity in Items)
            {
                if (entity is WParagraph
                    || entity is WTable)
                    break;
                index = Items.IndexOf(entity);
            }
            return index;
        }
        #endregion
    }
}
