#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{

    /// <summary>
    /// A collection of <see cref="TreeLevel"/> objects. 
    /// </summary>
    public class TreeLevels : IList<TreeLevel>
    {
        TreeModel treeModel;
        List<TreeLevel> inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeLevels"/> class.
        /// </summary>
        /// <param name="treeModel">The tree model.</param>
        public TreeLevels(TreeModel treeModel)
        {
            this.treeModel = treeModel;
            inner = new List<TreeLevel>();
        }

        /// <summary>
        /// Gets or sets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get { return treeModel; }
            internal set { treeModel = value; }
        }

        #region IList<TreeLevel> Members

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(TreeLevel item)
        {
            return inner.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the collection.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the collection.</exception>
        public void Insert(int index, TreeLevel item)
        {
            item.Levels = this;
            inner.Insert(index, item);
        }

        /// <summary>
        /// Removes the collection item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the collection.</exception>
        public void RemoveAt(int index)
        {
            if (inner[index].Levels == this)
                inner[index].Levels = null;
            inner.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Controls.VirtualTreeView.TreeLevel"/> at the specified index.
        /// </summary>
        /// <value></value>
        public TreeLevel this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                inner[index].Levels = this;
                inner[index] = value;
            }
        }

        #endregion

        #region ICollection<TreeLevel> Members

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection.</param>
        public void Add(TreeLevel item)
        {
            item.Levels = this;
            inner.Add(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            foreach (TreeLevel column in inner)
            {
                if (column.Levels == this)
                    column.Levels = null;
            }
            inner.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the collection; otherwise, false.
        /// </returns>
        public bool Contains(TreeLevel item)
        {
            return item.Levels == this;
        }

        /// <summary>
        /// Copies the elements of the collection to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from collection. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source collection is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(TreeLevel[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the collection.</returns>
        public int Count
        {
            get { return inner.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>this collection is never read-only.</returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the collection; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original collection.
        /// </returns>
        public bool Remove(TreeLevel item)
        {
            if (inner.Remove(item))
            {
                if (item.Levels == this)
                    item.Levels = null;
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<TreeLevel> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TreeLevel> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)inner).GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// A tree level holds default cell style appearance for cells
    /// of this level.
    /// </summary>
    public class TreeLevel
    {
        TreeLevels levels;
        string name = "";
        double height = double.NaN;
        TreeStyleInfo cellStyle = new TreeStyleInfo();

        /// <summary>
        /// Gets the collection this element belongs to.
        /// </summary>
        /// <value>The levels.</value>
        public TreeLevels Levels
        {
            get { return levels; }
            internal set { levels = value; }
        }

        /// <summary>
        /// Gets or sets the default cell style appearance for cells
        /// of this level.
        /// </summary>
        /// <value>The cell style.</value>
        public TreeStyleInfo CellStyle
        {
            get { return cellStyle; }
            set { cellStyle.CopyFrom(value); }
        }

        /// <summary>
        /// Gets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get
            {
                if (levels == null)
                    return null;
                else
                    return levels.TreeModel;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <returns></returns>
        public double GetHeight()
        {
            if (!double.IsNaN(height))
                return height;
            return TreeModel.DefaultHeight;
        }

    }


}
