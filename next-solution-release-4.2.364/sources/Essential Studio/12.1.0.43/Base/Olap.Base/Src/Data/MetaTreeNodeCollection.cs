//-------------------------------------------------------------------------------------------------
// <copyright file="MetaTreeNodeCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;



#if !SILVERLIGHT
namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Collection of MetaTreeNodes
    /// </summary>
    [Serializable]
    public class MetaTreeNodeCollection : CollectionBase, IEnumerable<MetaTreeNode>
    {
#else
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Collection of MetaTreeNodes
    /// </summary>
    [CollectionDataContract]
    public class MetaTreeNodeCollection : ObservableCollection<MetaTreeNode>
    {
#endif
        #region Internal Variables
        internal MetaTreeNode _parentNode;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaTreeNodeCollection"/> class.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        public MetaTreeNodeCollection(MetaTreeNode parentNode)
        {
            _parentNode = parentNode;
        }
        #endregion

        #region Public Methods

#if !SILVERLIGHT
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<MetaTreeNode> GetEnumerator()
        {
            foreach (MetaTreeNode metaTreeNode in base.List)
            {
                yield return metaTreeNode;
            }
        }

        /// <summary>
        /// Inserts the MetaTreeNode in specified index
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="metaTreeNode">The meta tree node.</param>
        public void Insert(int index, MetaTreeNode metaTreeNode)
        {
            base.List.Insert(index, metaTreeNode);
        }

        /// <summary>
        /// Removes the specified meta tree node.
        /// </summary>
        /// <param name="metaTreeNode">The meta tree node.</param>
        public void Remove(MetaTreeNode metaTreeNode)
        {
            base.List.Remove(metaTreeNode);
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.MetaTreeNode"/> at the specified index.
        /// </summary>
        /// <value></value>
        public MetaTreeNode this[int index]
        {
            get
            {
                return (MetaTreeNode)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified meta tree node.
        /// </summary>
        /// <param name="metaTreeNode">The meta tree node.</param>
        /// <returns>returns the index of the meta tree node</returns>
        public int Add(MetaTreeNode metaTreeNode)
        {
            //// Sorting according to the Display folder
            if (metaTreeNode.NodeType == MetaTreeNodeType.DisplayFolder)
            {
                int insertIndex = GetFolderNodeCount();
                base.List.Insert(insertIndex, metaTreeNode);
                return insertIndex;
            }

            return base.List.Add(metaTreeNode);
        }

#else
        /// <summary>
        /// Adds the specified meta tree node.
        /// </summary>
        /// <param name="metaTreeNode">The meta tree node.</param>
        /// <returns>returns the index of the metatree node</returns>
        public void Add(MetaTreeNode metaTreeNode)
        {
            //// Sorting accoding to the Display folder
            if (metaTreeNode.NodeType == MetaTreeNodeType.DisplayFolder)
            {
                int insertIndex = GetFolderNodeCount();
                base.Items.Insert(insertIndex, metaTreeNode);
            }
            else
            {
                base.Items.Add(metaTreeNode);
            }
            UpdateParent(metaTreeNode);
        }
#endif

        /// <summary>
        /// Gets the <see cref="Syncfusion.Olap.Data.MetaTreeNode"/> with the specified name.
        /// </summary>
        /// <value></value>
        public MetaTreeNode this[string name]
        {
            get
            {
#if !SILVERLIGHT
                foreach (MetaTreeNode mtNode in base.List)
#else
                foreach (MetaTreeNode mtNode in base.Items)
#endif
                {
                    if (mtNode.Name == name)
                    {
                        return mtNode;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Accepts the is selected changes.
        /// </summary>
        /// <param name="updateParent">if set to <c>true</c> [update parent].</param>
        public void AcceptIsSelectedChanges(bool updateParent)
        {
#if !SILVERLIGHT
            foreach (MetaTreeNode mtNode in base.List)
#else
            foreach (MetaTreeNode mtNode in base.Items)
#endif
            {
                mtNode.AcceptIsSelectedChanges(updateParent);
                if (mtNode.ChildNodes.Count > 0)
                {
                    mtNode.ChildNodes.AcceptIsSelectedChanges(updateParent);
                }
            }
        }

        /// <summary>
        /// Reverts the is selected changed.
        /// </summary>
        /// <param name="updateParent">if set to <c>true</c> [update parent].</param>
        public void RevertIsSelectedChanged(bool updateParent)
        {
#if !SILVERLIGHT
            foreach (MetaTreeNode mtNode in base.List)
#else
            foreach (MetaTreeNode mtNode in base.Items)
#endif
            {
                mtNode.RevertIsSelectedChanged(updateParent);
                if (mtNode.ChildNodes.Count > 0)
                {
                    mtNode.ChildNodes.RevertIsSelectedChanged(updateParent);
                }
            }
        }

        /// <summary>
        /// Finds the name of the by unique.
        /// </summary>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <returns></returns>
        public MetaTreeNode FindByUniqueName(string uniqueName)
        {
#if !SILVERLIGHT
            foreach (MetaTreeNode mtNode in base.List)
#else
            foreach (MetaTreeNode mtNode in base.Items)
#endif
            {
                if (mtNode.UniqueName == uniqueName)
                {
                    return mtNode;
                }
            }

            return null;
        }

        //public void ForceAcceptChanges()
        //{
        //    foreach (MetaTreeNode mtNode in base.List)
        //    {
        //        mtNode.ForceAcceptChanges();
        //        if (mtNode.ChildNodes.Count > 0)
        //        {
        //            mtNode.ChildNodes.ForceAcceptChanges();
        //        }
        //    }
        //}

        #endregion

        #region Private Methods
        int GetFolderNodeCount()
        {
            int count = 0;
#if !SILVERLIGHT
            foreach (MetaTreeNode mtNode in base.List)
#else
            foreach (MetaTreeNode mtNode in base.Items)
#endif
            {
                if (mtNode.NodeType == MetaTreeNodeType.DisplayFolder)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Updates the parent.
        /// </summary>
        /// <param name="metaTreeNodeObj">The meta tree node obj.</param>
        void UpdateParent(object metaTreeNodeObj)
        {
            if (metaTreeNodeObj is MetaTreeNode)
            {
                ((MetaTreeNode)metaTreeNodeObj).ParentNode = _parentNode;
            }
        }
        #endregion

        #region Protected Methods

#if !SILVERLIGHT
        /// <summary>
        /// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnInsertComplete(int index, object value)
        {
            this.UpdateParent(value);
        }

        /// <summary>
        /// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            this.UpdateParent(newValue);
        }
#else

        protected override void  InsertItem(int index, MetaTreeNode item)
        {
             base.InsertItem(index, item);
             this.UpdateParent(item);
        }

        protected override void SetItem(int index, MetaTreeNode item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item);
        }
#endif
        #endregion
    }
}
