#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [Editor(typeof(TreeColumnAdvCollectionEditor), typeof(UITypeEditor))]
    public class TreeColumnAdvCollection :
      CollectionBase,
      ICloneable
    {
        #region Class members

        private MultiColumnTreeView m_tree;
        #endregion

        #region Class properties

        public MultiColumnTreeView TreeView
        {
            get
            {
                return m_tree;
            }
        }

        /// <summary>Access to collection items by index.</summary>
        /// <param name="index"> Treecolumn index</param>
        public TreeColumnAdv this[int index]
        {
            get
            {
                return this.List[index] as TreeColumnAdv;
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary> Gets array of visible columns.</summary>
        public TreeColumnAdv[] VisibleColumns
        {
            get
            {
                ArrayList list = GetVisibleColumnsList();

                return (TreeColumnAdv[])list.ToArray(typeof(TreeColumnAdv));
            }
        }
        #endregion

        #region Class events
        /// <summary>Raised when collection detect own changes.</summary>
        public event CollectionChangeEventHandler CollectionChanged;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary> Initializes a new instance of the TreeColumnAdvCollection class.</summary>
        /// <param name="tree">Reference on parent Tree.</param>
        public TreeColumnAdvCollection(MultiColumnTreeView tree)
        {
            if (null == tree)
            {
                throw new ArgumentNullException("tree");
            }

            m_tree = tree;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Add column into collection.
        /// </summary>
        /// <param name="column">reference on column.</param>
        /// <returns>Order Index of column.</returns>
        public int Add(TreeColumnAdv column)
        {
            return this.List.Add(column);
        }

        /// <summary>
        /// Add range of columns into collection.
        /// </summary>
        /// <param name="columns">Array of columns.</param>
        public void AddRange(TreeColumnAdv[] columns)
        {
            this.TreeView.BeginUpdate();

            foreach (TreeColumnAdv column in columns)
            {
                this.List.Add(column);
            }

            this.TreeView.EndUpdate();
        }

        /// <summary>
        /// Add range of columns into collection.
        /// </summary>
        /// <param name="columns">Array of columns.</param>
        public void AddRange(ICollection columns)
        {
            this.TreeView.BeginUpdate();

            foreach (TreeColumnAdv column in columns)
            {
                this.List.Add(column);
            }

            this.TreeView.EndUpdate();
        }

        /// <summary>
        /// Remove column from collection.
        /// </summary>
        /// <param name="column">Treecolumn object</param>
        public void Remove(TreeColumnAdv column)
        {
            this.List.Remove(column);
        }

        /// <summary>Insert column into collection.</summary>
        /// <param name="index">insert position.</param>
        /// <param name="column">Column reference.</param>
        public void Insert(int index, TreeColumnAdv column)
        {
            this.List.Insert(index, column);
        }

        /// <summary>
        /// Method check is column in collection or not.
        /// </summary>
        /// <param name="column">reference on column to check.</param>
        /// <returns>True - column found in collection, otherwise False.</returns>
        public bool Contains(TreeColumnAdv column)
        {
            if (column == null)
            {
                return false;
            }

            return this.List.Contains(column);
        }

        /// <summary>
        /// Method return order index of item if it exists in collection, otherwise -1.
        /// </summary>
        /// <param name="column">reference on column.</param>
        /// <returns>-1 if nothing found, otherwise column order index.</returns>
        public int IndexOf(TreeColumnAdv column)
        {
            return this.List.IndexOf(column);
        }

        /// <summary>Clone collection and it items.</summary>
        /// <returns>Copy of the this collection.</returns>
        public TreeColumnAdvCollection Clone()
        {
            TreeColumnAdvCollection collection = new TreeColumnAdvCollection(this.TreeView);

            foreach (TreeColumnAdv column in this.InnerList)
            {
                collection.Add(column.Clone());
            }

            return collection;
        }

        /// <summary>Clone collection.</summary>
        /// <returns>Reference on cloned version of the current collection.</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>Method return accumulated width of all columns.</summary>
        /// <remarks>Method ignore visibility settings.</remarks>
        /// <returns> Return Total Column WIdth</returns>
        public int GetTotalColumnsWidth()
        {
            return GetTotalColumnsWidth(false);
        }

        /// <summary>Method return accumulated width of all columns.</summary>
        /// <param name="visible">True - count only visible columns, otherwise False.</param>
        /// <returns>Accumulated width in pixels.</returns>
        public int GetTotalColumnsWidth(bool visible)
        {
            int width = 0;

            for (int i = 0, len = this.Count; i < len; i++)
            {
                TreeColumnAdv column = this.InnerList[i] as TreeColumnAdv;

                width += visible ? (column.Visible ? column.Width : 0) : column.Width;
            }

            return width;
        }

        public int GetVisibleColumnsCount()
        {
            int count = 0;

            for (int i = 0, len = this.Count; i < len; i++)
            {
                TreeColumnAdv column = this.InnerList[i] as TreeColumnAdv;
                if (column.Visible)
                {
                    count++;
                }
            }

            return count;
        }
        #endregion

        #region Class utility methods
        /// <summary>Methods return array infilled by visible to user columns.</summary>
        /// <returns>Return array infilled by visible to user columns.</returns>
        private ArrayList GetVisibleColumnsList()
        {
            ArrayList list = new ArrayList(this.Count);

            for (int i = 0, len = this.Count; i < len; i++)
            {
                TreeColumnAdv column = this.InnerList[i] as TreeColumnAdv;
                if (column.Visible)
                {
                    list.Add(column);
                }
            }

            return list;
        }

        /// <summary>Utility method used for <see cref="CollectionChanged"/> event
        /// raising.</summary>
        /// <param name="args">Event parameters.</param>
        protected void RaiseCollectionChanged(CollectionChangeEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }
        }
        #endregion

        #region Class overrides
        /// <summary>On collection clearing method raise <see cref="CollectionChanged"/> event 
        /// with corresponding parameters.</summary>
        protected override void OnClearComplete()
        {
            base.OnClearComplete();

            RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <summary>On item inserting into collection set correct parent. Method raise 
        /// <see cref="CollectionChanged"/> event with corresponding parameters.</summary>
        /// <param name="index">Treenode index</param>
        /// <param name="value">TreecolumnAdv value</param>
        protected override void OnInsertComplete(int index, object value)
        {
            TreeColumnAdv column = value as TreeColumnAdv;

            if (column != null)
            {
                column.SetParent(this.TreeView);
            }

            base.OnInsertComplete(index, value);

            RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
        }

        /// <summary>On item remove from collection reset column parent. Method raise 
        /// <see cref="CollectionChanged"/> event with corresponding parameters.</summary>
        /// <param name="index"> Tree node index</param>
        /// <param name="value"> TreeColumn object</param>
        protected override void OnRemoveComplete(int index, object value)
        {
            TreeColumnAdv column = value as TreeColumnAdv;

            if (column != null)
            {
                column.ResetParent();
            }

            base.OnRemoveComplete(index, value);

            RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
        }

        /// <summary>On item replace/set in collection change items Parents. Method raise 
        /// <see cref="CollectionChanged"/> event with corresponding parameters.</summary>
        /// <param name="index">Tree node Index</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue"> new Value</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            TreeColumnAdv columnNew = newValue as TreeColumnAdv;
            TreeColumnAdv columnOld = oldValue as TreeColumnAdv;

            if (columnNew != null)
            {
                columnNew.SetParent(this.TreeView);
            }

            if (columnOld != null)
            {
                columnOld.ResetParent();
            }

            base.OnSetComplete(index, oldValue, newValue);

            RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, newValue));
        }

        /// <summary>Method check correctness of input parameters.</summary>
        /// <param name="value">parameter to check.</param>
        protected override void OnValidate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Into collection can be placed NULL references.");
            }

            base.OnValidate(value);
        }
        #endregion
    }
}