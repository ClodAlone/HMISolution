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
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public abstract class XDLSSerializableCollection
      : CollectionImpl
      , IXDLSSerializableCollection
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XDLSSerializableCollection"/> class.
        /// </summary>
        protected XDLSSerializableCollection(WordDocument doc, OwnerHolder owner)
            : base(doc, owner)
        {
        }
        #endregion

        #region IXDLSSerializableCollection implement
        /// <summary>
        /// Collection must creates and adds new empty item.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        IXDLSSerializable IXDLSSerializableCollection.AddNewItem(IXDLSContentReader reader)
        {
            OwnerHolder item = CreateItem(reader);

            if (item != null)
            {
                InnerList.Add(item);
                item.SetOwner(OwnerBase);
            }

            return item as IXDLSSerializable;
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        string IXDLSSerializableCollection.TagItemName
        {
            get
            {
                return GetTagItemName();
            }
        }
        #endregion

        #region Class virtual / abstract methods
        /// <summary>
        /// Clones to other collection.
        /// </summary>
        /// <param name="coll">The coll.</param>
        internal virtual void CloneToImpl(CollectionImpl coll)
        {
            foreach (XDLSSerializableBase serObj in InnerList)
            {
                object clonedItem = serObj.CloneInt();
                coll.InnerList.Add(clonedItem);
                if (clonedItem is OwnerHolder)
                    //Sets the owner for cloned item.
                    (clonedItem as OwnerHolder).SetOwner(coll.OwnerBase);
            }
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        protected abstract string GetTagItemName();
        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract OwnerHolder CreateItem(IXDLSContentReader reader);
        #endregion
    }
}
