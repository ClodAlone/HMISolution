#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

#region file using directives
using System;
using System.Collections;

using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Collection of blocks of the element.
    /// </summary>
    public sealed class BlocksCollection : CollectionBase
    {
        #region Class properties

        /// <summary>
        /// Returns the block object with the specified index.
        /// </summary>
        /// <param name="index">Index number</param>
        public Block this[int index]
        {
            get
            {
                return this.InnerList[index] as Block;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the BlocksCollection class
        /// </summary>
        public BlocksCollection()
            : base()
        {
        }
        #endregion

        #region Class Overrides

        /// <summary>
        /// Adds the specified block to the collection.
        /// </summary>
        /// <param name="block">Object to be added into the collection.</param>
        public void Add(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            this.InnerList.Add(block);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="block">Object for removing from the collection.</param>
        public void Remove(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            block.ClearEvents();
            this.InnerList.Remove(block);
        }

        /// <summary>
        /// Removes the object at the specified index of the collection.
        /// </summary>
        /// <param name="index">Index of the element in the collection.</param>
        public new void RemoveAt(int index)
        {
            Block block = this[index];

            if (block != null)
            {
                block.ClearEvents();
            }
            this.InnerList.RemoveAt(index);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public new void Clear()
        {
            Block block = null;

            for (int i = 0, len = base.Count; i < len; i++)
            {
                block = this[i];

                if (block != null)
                {
                    block.ClearEvents();
                }
            }

            this.InnerList.Clear();
        }
        #endregion
    }
}
