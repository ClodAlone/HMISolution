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

using System;
using System.Text;
using Syncfusion.DocIO.DLS.XML;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.IWTable"/> objects.
    /// </summary>
    public class WTableCollection
      : EntitySubsetCollection
      , IWTableCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IWTable"/> at the specified index.
        /// </summary>
        new public IWTable this[int index]
        {
            get
            {
                ClearIndexes();
                return GetByIndex(index) as IWTable;
            }
        }
        /// <summary>
        /// Gets the owner <see cref="Syncfusion.DocIO.DLS.ITextBody"/> of the collection.
        /// </summary>
        internal ITextBody OwnerTextBody
        {
            get
            {
                return base.Owner as ITextBody;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTableCollection"/> class.
        /// </summary>
        /// <param name="bodyItems">The body items.</param>
        public WTableCollection(BodyItemCollection bodyItems)
            : base(bodyItems, EntityType.Table)
        { }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds a table to end of text body.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public int Add(IWTable table)
        {
            return InternalAdd((Entity)table);
        }
        /// <summary>
        /// Determines whether the <see cref="Syncfusion.DocIO.DLS.IWTableCollection"/> contains a specific value.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        /// 	If table found, set to <c>true</c>.
        /// </returns>
        public bool Contains(IWTable table)
        {
            return InternalContains((Entity)table);
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="Syncfusion.DocIO.DLS.IWTableCollection"/>.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public int IndexOf(IWTable table)
        {
            return InternalIndexOf((Entity)(Entity)table);
        }
        /// <summary>
        /// Inserts a table into collection at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public int Insert(int index, IWTable table)
        {
            return InternalInsert(index, (Entity)table);
        }
        /// <summary>
        /// Removes the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        public void Remove(IWTable table)
        {
            InternalRemove((Entity)table);
        }
        /// <summary>
        /// Removes the table at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            InternalRemoveAt(index);
        }
        #endregion
    }
}
