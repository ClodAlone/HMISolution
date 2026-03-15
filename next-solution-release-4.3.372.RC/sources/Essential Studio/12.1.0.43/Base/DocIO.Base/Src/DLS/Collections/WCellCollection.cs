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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.WTableCell"/> objects.
    /// </summary>
    public class WCellCollection : EntityCollection
    {
        #region Class constants
        private static readonly System.Type[] DEF_ELEMENT_TYPES = new System.Type[1] { typeof(WTableCell) };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WTableCell"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        new public WTableCell this[int index]
        {
            get
            {
                return InnerList[index] as WTableCell;
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

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WCellCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public WCellCollection(WTableRow owner)
            : base(owner.Document, owner)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        public int Add(WTableCell cell)
        {
            int index = base.Add(cell);
            OnInsertCell(index, cell.CellFormat);
            return index;
        }
        /// <summary>
        /// Inserts a specified table cell into collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="cell">The cell.</param>
        public void Insert(int index, WTableCell cell)
        {
            base.Insert(index, cell);
            OnInsertCell(index, cell.CellFormat);
        }
        /// <summary>
        /// Returns index of a specified cell in collection.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        public int IndexOf(WTableCell cell)
        {
            //OnCellsChange();
            return base.IndexOf(cell);
        }
        /// <summary>
        /// Removes the specified cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public void Remove(WTableCell cell)
        {
            int index = cell.GetCellIndex();
            RemoveCellBookmark(index);
            base.Remove(cell);
            OnRemoveCell(index);
        }
        /// <summary>
        /// Removes the entity at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            RemoveCellBookmark(index);
            base.RemoveAt(index);
            OnRemoveCell(index);
        }
        /// <summary>
        /// Remuves the cell bookmark.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private void RemoveCellBookmark(int index)
        {
            WTableCell cell = this[index];
            WordDocument doc = cell.Document;

            for (int j = 0, cnt = doc.Bookmarks.Count; j < cnt; j++)
            {
                Bookmark bookmark = doc.Bookmarks[j] as Bookmark;
                int cellIndex = cell.GetCellIndex();

                if (cellIndex >= bookmark.BookmarkStart.ColumnFirst && cellIndex <= bookmark.BookmarkStart.ColumnLast)
                {
                    doc.Bookmarks.Remove(bookmark);
                }
                else if (cellIndex < bookmark.BookmarkStart.ColumnFirst)
                {
                    bookmark.BookmarkStart.ColumnFirst -= 1;
                    bookmark.BookmarkStart.ColumnLast -= 1;
                }
            }
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Implementation / xml
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        /// <value></value>
        protected override string GetTagItemName()
        {
            return XDLSConstants.CellItemTag;
        }
        /// <summary>
        /// Creates a new cell
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new WTableCell(Document);
        }

        #endregion
#endif

        #region Implementation
        /// <summary>
        /// Clones all cells to destination collection.
        /// </summary>
        /// <param name="destColl">The destination collection.</param>
        internal void CloneTo(EntityCollection destColl)
        {
            for (int i = 0, cnt = Count; i < cnt; i++)
            {
                destColl.Add(this[i].CloneCell());
            }
        }
        /// <summary>
        /// Called when cell inserted to cell collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="cellFormat">The cell format.</param>
        private void OnInsertCell(int index, CellFormat cellFormat)
        {
            if (Owner != null && Owner is WTableRow && !Owner.Document.IsOpening)
            {
                (Owner as WTableRow).OnInsertCell(index, cellFormat);
            }
        }
        /// <summary>
        /// Called when cell removed from cell collection.
        /// </summary>
        /// <param name="index">The index.</param>
        private void OnRemoveCell(int index)
        {
            if (Owner != null && Owner is WTableRow && !Owner.Document.IsOpening)
            {
                (Owner as WTableRow).OnRemoveCell(index);
            }
        }
        #endregion
    }
}
