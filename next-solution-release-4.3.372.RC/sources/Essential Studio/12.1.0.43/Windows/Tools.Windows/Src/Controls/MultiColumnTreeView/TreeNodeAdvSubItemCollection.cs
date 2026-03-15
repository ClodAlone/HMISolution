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
    [
    Editor(typeof(TreeNodeAdvSubItemCollectionEditor), typeof(UITypeEditor))
    ]
    public class TreeNodeAdvSubItemCollection :
      CollectionBase,
      ICloneable
    {
        #region Class members

        private TreeNodeAdv m_node;

        /// <summary> Reference on the first SubItem in current collection. </summary>
        private TreeNodeAdvSubItem m_firstSubItem;
        #endregion

        #region Class properties

        protected TreeNodeAdv TreeNode
        {
            get
            {
                return m_node;
            }
        }

        /// <summary>Access to collection items by index.</summary>
        /// <param name="index">Treenode index</param>
        public TreeNodeAdvSubItem this[int index]
        {
            get
            {
                return this.List[index] as TreeNodeAdvSubItem;
            }
            set
            {
                this.List[index] = value;
            }
        }
        #endregion

        #region Class events

        public event CollectionChangeEventHandler CollectionChanged;
        #endregion

        #region Class Initialize/Finalize methods

        internal TreeNodeAdvSubItemCollection()
        {
        }

        public TreeNodeAdvSubItemCollection(TreeNodeAdv node)
        {
            m_node = node;
        }
        #endregion

        #region Class Public Methods

        public int Add(TreeNodeAdvSubItem subitem)
        {
            return this.List.Add(subitem);
        }

        public void AddRange(TreeNodeAdvSubItem[] subitems)
        {
            foreach (TreeNodeAdvSubItem subitem in subitems)
            {
                this.Add(subitem);
            }
        }

        public void AddRange(ICollection subitems)
        {
            foreach (TreeNodeAdvSubItem subitem in subitems)
            {
                this.Add(subitem);
            }
        }

        public void Remove(TreeNodeAdvSubItem subitem)
        {
            this.List.Remove(subitem);
        }

        public void Insert(int index, TreeNodeAdvSubItem subitem)
        {
            this.List.Insert(index, subitem);
        }

        public bool Contains(TreeNodeAdvSubItem subitem)
        {
            return this.List.Contains(subitem);
        }
 
        public int IndexOf(TreeNodeAdvSubItem subitem)
        {
            return this.List.IndexOf(subitem);
        }

        public TreeNodeAdvSubItemCollection Clone()
        {
            TreeNodeAdvSubItemCollection collection = new TreeNodeAdvSubItemCollection();

            foreach (TreeNodeAdvSubItem sub in this.InnerList)
            {
                collection.Add(sub.Clone());
            }

            return collection;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>Create array of items instead of sizable collection.</summary>
        /// <returns>Copy of items from collection to array.</returns>
        public TreeNodeAdvSubItem[] ToArray()
        {
            TreeNodeAdvSubItem[] array = new TreeNodeAdvSubItem[this.Count];
            this.InnerList.CopyTo(array);
            return array;
        }

        public TreeNodeAdvSubItem CreateItem()
        {
            return new TreeNodeAdvSubItem(m_node);
        }
        #endregion

        #region Class utility methods

        private void RaiseCollectionChanged(CollectionChangeEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }
        }

        protected internal void SetParent(TreeNodeAdv node)
        {
            m_node = node;

            // update parent for all nodes
            foreach (TreeNodeAdvSubItem item in this.InnerList)
            {
                item.SetParent(m_node);
            }
        }
        #endregion

        #region Class overrides
        /// <summary> Saves first reference on first SubItem </summary>
        protected override void OnClear()
        {
            if (Count > 0)
            {
                m_firstSubItem = this[0];
            }
        }

        protected override void OnClearComplete()
        {
            base.OnClearComplete();

            // Collection should be to contain node, m_firstSubItem is reference on first SubItem. 
            // First SubItem is node.
            if (null != m_firstSubItem)
            {
                InnerList.Add(m_firstSubItem);
            }

            RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        protected override void OnInsertComplete(int index, object value)
        {
            TreeNodeAdvSubItem subItem = value as TreeNodeAdvSubItem;

            if (subItem != null)
            {
                subItem.SetParent(this.TreeNode);
            }

            base.OnInsertComplete(index, value);

            RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            TreeNodeAdvSubItem subItem = value as TreeNodeAdvSubItem;

            if (subItem != null)
            {
                subItem.ResetParent();
            }

            base.OnRemoveComplete(index, value);

            RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            TreeNodeAdvSubItem subItemNew = newValue as TreeNodeAdvSubItem;
            TreeNodeAdvSubItem subItemOld = oldValue as TreeNodeAdvSubItem;

            if (subItemNew != null)
            {
                subItemNew.SetParent(this.TreeNode);
            }

            if (subItemOld != null)
            {
                subItemOld.ResetParent();
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