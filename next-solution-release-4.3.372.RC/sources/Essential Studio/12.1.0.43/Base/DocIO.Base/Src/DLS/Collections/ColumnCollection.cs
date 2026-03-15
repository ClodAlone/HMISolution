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
using Syncfusion.DocIO.DLS.XML;
#endregion

using System.Collections;
namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// A collection of <see cref="Syncfusion.DocIO.DLS.Column"/> objects that 
    /// represent all the columns of text in a section of a document.
    /// </summary>
  public class ColumnCollection :
#if !SILVERLIGHT && !WP
    XDLSSerializableCollection
#else
    CollectionImpl
    ,IEnumerable
#endif
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.Column"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Column this[int index]
        {
            get
            {
                return (Column)InnerList[index];
            }
        }
        /// <summary>
        /// Gets the owner section.
        /// </summary>
        /// <value>The owner section.</value>
        internal WSection OwnerSection
        {
            get
            {
                return OwnerBase as WSection;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnCollection"/> class.
        /// </summary>
        /// <param name="section">The section.</param>
        internal ColumnCollection(WSection section)
            : base(section.Document, section)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds <see cref="Syncfusion.DocIO.DLS.Column"/> object to the collection.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public int Add(Column column)
        {
            column.SetOwner(OwnerBase);
            return InnerList.Add(column);
        }
        /// <summary>
        /// Populates the specified number of columns with specified spacing.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="spacing">The spacing.</param>
        public void Populate(int count, float spacing)
        {
            float width = OwnerSection.PageSetup.ClientWidth / count;
            width -= spacing;

            InnerList.Clear();

            for (int i = 0; i < count; i++)
            {
                Column col = new Column(Document);
                col.Width = width;
                col.Space = spacing;
                Add(col);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones items to specified collection.
        /// </summary>
        /// <param name="coll">The collection.</param>
        internal void CloneTo(ColumnCollection coll)
        {
            Column column = null;
            for (int i = 0, cnt = InnerList.Count; i < cnt; i++)
            {
                column = InnerList[i] as Column;
                coll.Add(column.Clone());
            }
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Implementation / xml
        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new Column(Document);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        /// <returns></returns>
        protected override string GetTagItemName()
        {
            return XDLSConstants.ColumnTag;
        }
        #endregion
#endif
    }
}