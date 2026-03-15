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
using Syncfusion.DocIO.DLS;
using System.Collections;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collections of list formatting for each level in a list.
    /// </summary>
    public class ListLevelCollection : XDLSSerializableCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WListLevel"/> at the specified index.
        /// </summary>
        /// <value></value>
        public WListLevel this[int index]
        {
            get
            {
                return (WListLevel)InnerList[index];
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListLevelCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal ListLevelCollection(ListStyle owner)
            : base(owner.Document, owner)
        { }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds List level to collection.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        internal int Add(WListLevel level)
        {
            if (level == null)
                throw new ArgumentNullException("level");

            level.SetOwner(OwnerBase);
            return InnerList.Add(level);
        }
        /// <summary>
        /// Gets index of <see cref="T:Syncfusion.DocIO.DLS.WListLevel"/> in the collection.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        internal int IndexOf(WListLevel level)
        {
            return InnerList.IndexOf(level);
        }
        /// <summary>
        /// Removes all levels. 
        /// </summary>
        internal void Clear()
        {
            InnerList.Clear();
        }
        #endregion

        #region Implementation / xml
        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new WListLevel(OwnerBase as ListStyle);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        /// <returns></returns>
        protected override string GetTagItemName()
        {
            return XDLSConstants.ListLevelItemTag;
        }
        #endregion
    }
}