#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

using Syncfusion.Windows.ComponentModel;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Represents a collection of <see cref="GridCellModelBase"/> objects in the <see cref="GridModel"/>.
    /// </summary>
    /// <remarks>
    /// On the <see cref="GridModel"/>, you access the <see cref="GridCellModelCollection"/> through the <see cref="GridModel.CellModels"/> property.
    /// <para/>
    /// The <see cref="GridCellModelCollection"/> uses standard <see cref="Add"/> and <see cref="Remove"/>
    /// methods to manipulate the collection.
    /// Use the Contains method to determine if a specific cell type exists in the collection.
    /// </remarks>
    /// <example>
    ///  <para>The following example registers a checkbox celltype into the cell models collection.</para>
    ///   <code lang="C#">
    ///   GridControl grid = new GridControl();
    ///   grid.Model.CellModels.Add("CheckBox", new GridCellCheckboxModel());
    ///   </code>
    /// </example>
    //[Editor(typeof(GridCellModelCollectionEditor), typeof(UITypeEditor))]
#if ENABLE_PARTIAL_TRUST
    public class GridCellModelCollection : Disposable, ICollection, ICloneable
#else
    public class GridCellModelCollection : Disposable, ICollection,ISerializable, ICloneable//, IDeserializationCallback
#endif
    {
        // Fields
        string defaultKey = "";
        internal Hashtable content = new Hashtable();

        GridModel gridModel;

        /// <summary>
        /// Returns the grid model.
        /// </summary>
        public GridModel GridModel
        {
            get { return gridModel; }
            //set { model = value; }
        }

        /// <overload>
        /// Initializes a new <see cref="GridCellModelCollection"/>.
        /// </overload>
        /// <summary>
        /// Initializes an empty <see cref="GridCellModelCollection"/> and associates it with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">The <see cref="GridModel"/> that owns this collection of cell types.</param>
        public GridCellModelCollection(GridModel model)
        {
            this.gridModel = model;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (content != null)
                {
                    foreach (object obj in content.Values)
                    {
                        GridCellModelBase cm = obj as GridCellModelBase;
                        if (cm != null)
                            cm.Dispose();
                    }
                    content.Clear();
                }
                this.defaultKey = null;
            }
            base.Dispose(disposing);
        }


        // Serialize
        /// <summary>
        /// Initializes a new <see cref="GridCellModelCollection"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridCellModelCollection(SerializationInfo info, StreamingContext context)
        {
            content = new Hashtable();
            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                if (sie.Name != "DefaultCellType")
                    content[sie.Name] = sie.Value as GridCellModelBase;
                else
                    defaultKey = sie.Value as string;
            }
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridCellModelCollection"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
#if !SyncfusionFramework4_0
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
#else
        [SecurityCritical]
#endif
#if !ENABLE_PARTIAL_TRUST

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (content != null)
            {
                foreach (string cellTypeName in content.Keys)
                    info.AddValue(cellTypeName, content[cellTypeName]);
            }
            info.AddValue("DefaultCellType", defaultKey); // GridCellModelBase
        }
#endif

        ///// <override/>
        //protected override void OnModelDeserialization(object sender, GridModel model)
        //{
        //    base.OnModelDeserialization(sender, model);

        //    if (content != null && content.Count > 0)
        //    {
        //        foreach (object obj in content.Values)
        //        {
        //            GridCellModelBase cm = obj as GridCellModelBase;
        //            if (cm != null)
        //                cm.RaiseModelDeserialization(sender, model);
        //        }
        //    }
        //}

        /// <summary>
        /// Creates a copy of this object using <see cref="System.Object.MemberwiseClone"/>.
        /// </summary>
        /// <returns>An exact copy of this object.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        // Properties, Methods

        /// <summary>
        /// Raises the <see cref="GridModel.CellModelsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CollectionChangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            GridModel.RaiseCellModelsChanged(e);
        }

        internal void RaiseCollectionChanged(CollectionChangeEventArgs e)
        {
            OnCollectionChanged(e);
        }

        /// <summary>
        /// The default cell type to be used for cells where a specific cell type could not be loaded.
        /// </summary>
        public string DefaultCellType
        {
            get
            {
                return defaultKey;
            }
            set
            {
                if (!this.ContainsKey(value))
                    throw new ArgumentException(value + " not found in collection.", value);
                defaultKey = value;
            }
        }

        /// <summary>
        ///   <para>Returns an enumerator that can iterate through the cell model dictionary.</para>
        /// </summary>
        /// <returns>
        ///   <para>An <see cref="System.Collections.IEnumerator" /> that can iterate through the string dictionary.</para>
        /// </returns>
        public virtual /*IEnumerable*/ IEnumerator GetEnumerator()
        {
            return this.content.GetEnumerator();
        }

        /// <summary>
        ///   <para>Removes the entry with the specified cellTypeName from the cell model dictionary.</para>
        /// </summary>
        /// <param name="cellTypeName">The cellTypeName of the entry to remove.</param>
        public virtual void Remove(string cellTypeName)
        {
            GridCellModelBase cellModel = content[cellTypeName] as GridCellModelBase;
            if (cellModel != null)
            {
                cellModel.Dispose();
                cellModel.GridModel = null;
            }

            // TODO: Reset cellModel.GridModel
            this.content.Remove(cellTypeName);
            if (cellTypeName == defaultKey)
                defaultKey = "";
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, cellTypeName));
        }

        /// <summary>
        ///   <para>Copies the cell model dictionary values to a one-dimensional <see cref="System.Array" /> instance at the
        /// specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the values copied from the <see cref="GridCellModelCollection" />.</param>
        /// <param name=" index">The index in the array where copying begins.</param>
        public virtual void CopyTo(GridCellModelBase[] array, int index)
        {
            this.content.CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridCellModelBase[])array, index);
        }



        /// <summary>
        ///   <para>Determines if the <see cref="GridCellModelCollection" /> contains a specific value.</para>
        /// </summary>
        /// <param name="cellModel">The value to locate in the <see cref="GridCellModelCollection" />.</param>
        /// <returns>
        ///   <para>
        ///     <see langword="true" /> if the
        /// <see cref="GridCellModelCollection" /> contains an element with the specified value;
        /// otherwise, <see langword="false" />. </para>
        /// </returns>
        public virtual bool ContainsValue(GridCellModelBase cellModel)
        {
            return this.content.ContainsValue(cellModel);
        }



        /// <summary>
        ///   <para>Determines if the <see cref="GridCellModelCollection" /> contains a specific cellTypeName.</para>
        /// </summary>
        /// <param name="cellTypeName">The cellTypeName to locate in the <see cref="GridCellModelCollection" />.</param>
        /// <returns>
        ///   <para>
        ///     <see langword="true" /> if the
        /// <see cref="GridCellModelCollection" /> contains an entry with the specified cellTypeName;
        /// otherwise, <see langword="false" />.</para>
        /// </returns>
        public virtual bool ContainsKey(string cellTypeName)
        {
            return this.content.ContainsKey(cellTypeName);
        }



        /// <summary>
        ///   <para>Removes all entries from the <see cref="GridCellModelCollection" />.</para>
        /// </summary>
        public virtual void Clear()
        {
            // TODO: Reset cellModel.GridModel
            foreach (GridCellModelBase cellModel in content.Values)
            {
                if (cellModel != null)
                {
                    cellModel.Dispose();
                    cellModel.GridModel = null;
                }
            }
            this.content.Clear();
            this.defaultKey = "";
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }



        /// <summary>
        ///   <para>Adds cell model with the specified cellTypeName into the
        /// <see cref="GridCellModelCollection" />.</para>
        /// </summary>
        /// <param name="cellTypeName">The cellTypeName of the entry to add.</param>
        /// <param name="model">The cell model of the entry to add.</param>
        public virtual void Add(string cellTypeName, GridCellModelBase cellModel)
        {
            this.content.Add(cellTypeName, cellModel);

            if (String.IsNullOrEmpty(defaultKey))
                defaultKey = cellTypeName;

            cellModel.GridModel = this.gridModel;
            cellModel.OnCreated();

            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, cellTypeName));
        }

        /// <summary>
        ///   <para>Gets a collection of values in the <see cref="GridCellModelCollection" />.</para>
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
        /// the <see cref="GridCellModelCollection" />.</para>
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                return this.content.SyncRoot;
            } // end of method get_SyncRoot
        }


        /// <summary>
        ///   <para>Gets a collection of cellTypeNames in the <see cref="GridCellModelCollection" />.</para>
        /// </summary>
        public virtual ICollection Keys
        {
            get
            {
                return this.content.Keys;
            } // end of method get_Keys
        }

        /// <summary>
        /// The <see cref="GridCellModelBase"/> for the specific cell type name.
        /// </summary>
        public virtual GridCellModelBase this[string cellTypeName]
        {
            set
            {
                if (this.ContainsKey(cellTypeName) && content[cellTypeName] != value)
                    this.Remove(cellTypeName);
                this.Add(cellTypeName, value);
            }
            get
            {
                if (!this.ContainsKey(cellTypeName))
                {
                    gridModel.ignoreCellModelsChanged = true;
                    try
                    {
                        GridQueryCellModelEventArgs qe = new GridQueryCellModelEventArgs(gridModel, cellTypeName);
                        gridModel.RaiseQueryCellModel(qe);
                        if (qe.CellModel != null)
                        {
                            if (!this.ContainsKey(qe.CellType))
                                this.Add(qe.CellType, qe.CellModel);
                            return qe.CellModel;
                        }
                        cellTypeName = defaultKey;
                    }
                    finally
                    {
                        gridModel.ignoreCellModelsChanged = false;
                    }
                }
                else if (String.IsNullOrEmpty(defaultKey))
                    defaultKey = cellTypeName;

                return (GridCellModelBase)this.content[cellTypeName];
            }
        }


        /// <summary>
        ///   <para>Gets a value that indicates whether access to the <see cref="GridCellModelCollection" /> is synchronized (thread-safe).</para>
        /// </summary>
        public virtual bool IsSynchronized
        {
            get
            {
                return this.content.IsSynchronized;
            }
        }


        /// <summary>
        ///   <para>Gets the number of cellTypeName-and-value pairs in the <see cref="GridCellModelCollection" />.</para>
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.content.Count;
            }
        }
    }

}
