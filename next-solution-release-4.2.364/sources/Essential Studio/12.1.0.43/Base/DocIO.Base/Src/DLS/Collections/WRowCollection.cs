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

using Syncfusion.DocIO.DLS.XML;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.WTableRow"/> objects. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WRowCollection : EntityCollection
    {
        #region Class constants
        private static readonly System.Type[] DEF_ELEMENT_TYPES = new System.Type[1] { typeof(WTableRow) };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WTableRow"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        new public WTableRow this[int index]
        {
            get
            {
                return InnerList[index] as WTableRow;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Type[] TypesOfElement
        {
            get
            {
                return DEF_ELEMENT_TYPES;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WRowCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public WRowCollection(WTable owner)
            : base(owner.Document, owner)
        { }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds a table row to collection.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public int Add(WTableRow row)
        {
            return base.Add(row);
        }
        /// <summary>
        /// Inserts a table row into collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="row">The row.</param>
        public void Insert(int index, WTableRow row)
        {
            base.Insert(index, row);
        }
        /// <summary>
        /// Returns index of a specified row in collection.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public int IndexOf(WTableRow row)
        {
            return base.IndexOf(row);
        }
        /// <summary>
        /// Removes a specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        public void Remove(WTableRow row)
        {
            base.Remove(row);
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Implementation / xml
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        protected override string GetTagItemName()
        {
            return XDLSConstants.RowItemTag;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new WTableRow(Document);
        }
        #endregion
#endif
    }
}