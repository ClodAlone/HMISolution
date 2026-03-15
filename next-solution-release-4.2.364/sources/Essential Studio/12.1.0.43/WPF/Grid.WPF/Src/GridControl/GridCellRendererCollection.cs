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
using System.Collections;
using System.ComponentModel;
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Represents a collection of <see cref="GridCellRendererBase"/> objects in the <see cref="GridControlBase"/> view.
    /// </summary>
    /// <remarks>
    /// On the <see cref="GridControlBase"/>, you access the <see cref="GridCellRendererCollection"/> through the <see cref="GridControlBase.CellRenderers"/> property.
    /// <para/>
    /// The <see cref="GridCellRendererCollection"/> uses standard <see cref="Add"/> and <see cref="Remove"/>
    /// methods to manipulate the collection.
    /// Use the Contains method to determine if a specific cell type exists in the collection.
    /// </remarks>
    /// <example>
    /// <para>The following example shows how to retrieve the checkbox cell renderer from this collection.</para>
    /// <code lang="C#">
    /// GridCellCheckboxRenderer rend = (GridCellCheckboxRenderer)gridControl1.CellRenderers["CheckBox"];
    /// </code>
    /// </example>
    public class GridCellRendererCollection : Disposable, ICollection
    {
        // Fields
        string cachedKey = "";
        IGridCellRenderer cachedRenderer = null;
        internal Hashtable content = new Hashtable();
        GridControlBase grid;

        // Ctors

        /// <summary>
        /// Initializes a <see cref="GridCellRendererCollection"/> and 
        /// associates it with the <see cref="GridControlBase"/>.
        /// </summary>
        /// <param name="grid">The parent grid.</param>
        public GridCellRendererCollection(GridControlBase grid)
        {
            this.grid = grid;
            grid.Model.CellModelsChanged += new CollectionChangeEventHandler(GridModelCellModelsChanged);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.grid != null && this.grid.Model != null)
                    grid.Model.CellModelsChanged -= new CollectionChangeEventHandler(GridModelCellModelsChanged);
                Clear();
                if (this.cachedRenderer != null)
                {
                    this.cachedRenderer.Dispose();
                }
                this.content = null;

                this.grid = null;
            }
            base.Dispose(disposing);
        }

        void GridModelCellModelsChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
                return;

            if (e.Element == null)
                this.Clear();

            if (ContainsKey((string)e.Element))
                Remove((string)e.Element);
        }

        /// <summary>
        ///   <para>Returns an enumerator that can iterate through the cell grid dictionary.</para>
        /// </summary>
        /// <returns>
        ///   <para>An <see cref="System.Collections.IEnumerator" /> that can iterate through the string dictionary.</para>
        /// </returns>
        public virtual /*IEnumerable*/ IEnumerator GetEnumerator()
        {
            return this.content.GetEnumerator();
        }

        /// <summary>
        ///   <para>Removes the entry with the specified key from the cell grid dictionary.</para>
        /// </summary>
        /// <param name="key">The key of the entry to remove.</param>
        public virtual void Remove(string key)
        {
            if (ContainsKey(key))
            {
                IGridCellRenderer control = content[key] as IGridCellRenderer;
                if (control != null && control.CellModel != null)
                    control.Dispose();
                this.content.Remove(key);
            }
            cachedKey = "";
            cachedRenderer = null;
        }

        /// <summary>
        ///   <para>Copies the cell grid dictionary values to a one-dimensional <see cref="System.Array" /> instance at the
        /// specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the values copied from the <see cref="GridCellRendererCollection" />.</param>
        /// <param name=" index">The index in the array where copying begins.</param>
        public virtual void CopyTo(GridCellRendererBase[] array, int index)
        {
            this.content.CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridCellRendererBase[])array, index);
        }


        /// <summary>
        ///   <para>Determines if the <see cref="GridCellRendererCollection" /> contains a specific value.</para>
        /// </summary>
        /// <param name="cellRenderer">The value to locate in the <see cref="GridCellRendererCollection" />.</param>
        /// <returns>
        ///   <para>
        ///     <see langword="true" /> if the
        /// <see cref="GridCellRendererCollection" /> contains an element with the specified value;
        /// otherwise, <see langword="false" />. </para>
        /// </returns>
        public virtual bool ContainsValue(GridCellRendererBase cellRenderer)
        {
            return this.content.ContainsValue(cellRenderer);
        }



        /// <summary>
        ///   <para>Determines if the <see cref="GridCellRendererCollection" /> contains a specific key.</para>
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="GridCellRendererCollection" />.</param>
        /// <returns>
        ///   <para>
        ///     <see langword="true" /> if the
        /// <see cref="GridCellRendererCollection" /> contains an entry with the specified key;
        /// otherwise, <see langword="false" />.</para>
        /// </returns>
        public virtual bool ContainsKey(string key)
        {
            return this.content.ContainsKey(key);
        }



        /// <summary>
        ///   <para>Removes all entries from the <see cref="GridCellRendererCollection" />.</para>
        /// </summary>
        public virtual void Clear()
        {
            foreach (object obj in content.Values)
            {
                GridCellRendererBase control = obj as GridCellRendererBase;
                if (control != null && control.GridControl != null)
                    control.Dispose();
            }
            this.content.Clear();
            cachedKey = "";
            cachedRenderer = null;
        }



        /// <summary>
        ///   <para>Adds cell grid with the specified key into the
        /// <see cref="GridCellRendererCollection" />.</para>
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="grid">The cell grid of the entry to add.</param>
        public virtual void Add(string key, IGridCellRenderer grid)
        {
            this.content.Add(key, grid);
        }

        /// <summary>
        ///   <para>Gets a collection of values in the <see cref="GridCellRendererCollection" />.</para>
        /// </summary>
        public virtual ICollection Values
        {
            get
            {
                return this.content.Values;
            }
        }


        /// <summary>
        ///   <para>Gets an object that can be used to synchronize access to
        /// the <see cref="GridCellRendererCollection" />.</para>
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                return this.content.SyncRoot;
            } // end of method get_SyncRoot
        }


        /// <summary>
        ///   <para>Gets a collection of keys in the <see cref="GridCellRendererCollection" />.</para>
        /// </summary>
        public virtual ICollection Keys
        {
            get
            {
                return this.content.Keys;
            } // end of method get_Keys
        }

        /// <summary>
        /// The <see cref="GridCellRendererBase"/> for the given key.
        /// </summary>
        public virtual IGridCellRenderer this[string key]
        {
            set
            {
                if (this.ContainsKey(key) && content[key] != value)
                    this.Remove(key);
                this.Add(key, value);
            }
            get
            {
                if (key == cachedKey)
                    return cachedRenderer;

                cachedKey = key;
                if (!this.ContainsKey(key))
                {
                    cachedKey = key;
                    GridCellModelBase cellModel = grid.Model.CellModels[key];
                    cachedRenderer = cellModel.CreateRenderer();
                    cachedRenderer.GridControl = grid;
                    this.Add(cachedKey, cachedRenderer);
                    //if (cellModel.BindingContext == null)
                    //    cellModel.BindingContext = this.grid.BindingContext;
                }
                else
                    cachedRenderer = (IGridCellRenderer)content[key];
                return cachedRenderer;
            }
        }


        /// <summary>
        ///   <para>Gets a value that indicates whether access to the <see cref="GridCellRendererCollection" /> is synchronized (thread-safe).</para>
        /// </summary>
        public virtual bool IsSynchronized
        {
            get
            {
                return this.content.IsSynchronized;
            }
        }


        /// <summary>
        ///   <para>Gets the number of key-and-value pairs in the <see cref="GridCellRendererCollection" />.</para>
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.content.Count;
            }
        }

        internal void EmptyRecycleBin()
        {

            foreach (IGridCellRenderer renderer in content.Values)
            {
                if (renderer != null)
                    renderer.EmptyRecycleBin();
            }
        }
    }
}