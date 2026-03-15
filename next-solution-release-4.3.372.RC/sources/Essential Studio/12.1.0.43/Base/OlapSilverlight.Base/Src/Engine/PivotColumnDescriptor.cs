#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Syncfusion.OlapSilverlight.Engine
{
    /// <summary>
    /// Collection used to store cells.
    /// </summary>
    //[Serializable]
    //public class PivotCellCollection
    //    : Collection<PivotCellDescriptor>
    //{
    //    #region indexer
    //    /// <summary>
    //    /// Gets/sets cell at specific index.
    //    /// </summary>
    //    /// <param name="index">cell index.</param>
    //    /// <returns>cell.</returns>
    //    public PivotCellDescriptor this[int index]
    //    {
    //        get
    //        {
    //            return this[index] as PivotCellDescriptor;
    //        }
    //        set
    //        {
    //            if (index < 0 || index > this.Count)
    //            {
    //                throw new IndexOutOfRangeException("Incorrect index.");
    //            }

    //            if (value == null)
    //            {
    //                throw new NullReferenceException("Incorrect value (null).");
    //            }

    //            if (this[index] != value)
    //            {
    //                this[index] = value;
    //            }
    //        }
    //    }
    //    #endregion

    //    #region class public methods
    //    /// <summary>
    //    /// Adds cell to collection.
    //    /// </summary>
    //    /// <param name="cellDescriptor"></param>
    //    //public virtual void Add(PivotCellDescriptor cellDescriptor)
    //    //{
    //    //    if (cellDescriptor == null)
    //    //        throw new NullReferenceException("Incorrect argument (null).");

    //    //    if (cellDescriptor.CellIndex == -1)
    //    //        cellDescriptor.CellIndex = this.Count;

    //    //    this.Add(cellDescriptor);
    //    //}

    //    /// <summary>
    //    /// Returns if collection contains cell descriptor.
    //    /// </summary>
    //    /// <param name="cellDescriptor">Cell descriptor</param>
    //    /// <returns>True if contains.</returns>
    //    public bool Contains(PivotCellDescriptor cellDescriptor)
    //    {
    //        if (cellDescriptor == null)
    //            throw new NullReferenceException("Incorrect argument (null).");

    //        return this.Contains(cellDescriptor);
    //    }

    //    /// <summary>
    //    /// Removes cell descriptor form collection.
    //    /// </summary>
    //    /// <param name="cellDescriptor">Cell descriptor to remove.</param>
    //    public void Remove(PivotCellDescriptor cellDescriptor)
    //    {
    //        if (cellDescriptor == null)
    //            throw new NullReferenceException("Incorrect argument (null).");

    //        if (this.Contains(cellDescriptor) == false)
    //            throw new ArgumentException("Not in list.");

    //        this.Remove(cellDescriptor);
    //    }

    //    /// <summary>
    //    /// Returns index of specified cell descriptor in the collection.
    //    /// </summary>
    //    /// <param name="cellDescriptor">Cell descriptor to get index for.</param>
    //    /// <returns>Index.</returns>
    //    public int IndexOf(PivotCellDescriptor cellDescriptor)
    //    {
    //        if (cellDescriptor == null)
    //            throw new NullReferenceException("Incorrect argument (null).");

    //        return this.IndexOf(cellDescriptor);
    //    }

    //    /// <summary>
    //    /// Inserts cell descriptor at specified index.
    //    /// </summary>
    //    /// <param name="index">Index to insert at.</param>
    //    /// <param name="cellDescriptor">Cell descriptor to insert.</param>
    //    public virtual void Insert(int index, PivotCellDescriptor cellDescriptor)
    //    {
    //        if (cellDescriptor == null)
    //            throw new NullReferenceException("Incorrect argument (null).");

    //        if (index < 0 || index > this.Count)
    //            throw new ArgumentException("Incorrect index");

    //        if (cellDescriptor.CellIndex == -1)
    //            cellDescriptor.CellIndex = index;

    //        this.Insert(index, cellDescriptor);
    //    }

    //    /// <summary>
    //    /// Releases all resources used by collection.
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        this.Clear();
    //    }

    //    /// <summary>
    //    /// Creates cloned collection.
    //    /// </summary>
    //    /// <returns>Cloned collection.</returns>
    //    public PivotCellCollection Clone()
    //    {
    //        PivotCellCollection collection = new PivotCellCollection();

    //        for (int i = 0; i < Count; i++)
    //            collection.Add(this[i].Clone());

    //        return collection;
    //    }
    //    #endregion

    //    #region class overrides
    //    /// <summary>
    //    /// Override. Fires before the item will be added to the collection.
    //    /// </summary>
    //    /// <param name="index">Insertion index.</param>
    //    /// <param name="value">Inserted object (Cell descriptor).</param>
    //    //protected override void OnInsert(int index, object value)
    //    //{
    //    //    PivotCellDescriptor descriptor = value as PivotCellDescriptor;

    //    //    if (descriptor != null)
    //    //    {
    //    //        if (descriptor.CellIndex == -1)
    //    //            descriptor.CellIndex = index;

    //    //        this.OnInsert(index, value);
    //    //    }
    //    //}
    //    #endregion
    //}

    /// <summary>
    /// Descriptor for pivot column.
    /// </summary>
    //[TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class PivotColumnDescriptor:IPivotCellHost 
       
        
    {
        #region class members
        private PivotCellCollection m_cells = null;
        private string m_mappingName = string.Empty;
        #endregion

        #region class properties
        public string MappingName
        {
            get
            {
                return m_mappingName;
            }
            set
            {
                if (m_mappingName != value)
                {
                    m_mappingName = value;
                }
            }
        }

        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        //  Browsable(true), NotifyParentProperty(true)]
        public PivotCellCollection Cells
        {
            get
            {
                return m_cells;
            }
        }
        #endregion

        #region class initialize/finalize methods
        /// <summary>
        /// Initializes new instance of <see cref="Syncfusion.Web.UI.WebControls.Grid.Pivot.PivotColumnDescriptor"/>
        /// </summary>
        public PivotColumnDescriptor()
        {
            m_cells = new PivotCellCollection();
        }
        #endregion

        #region ICloneable<PivotColumnDescriptor> Members
        /// <summary>
        /// Clones current instance of PivotColumnDescriptor class.
        /// </summary>
        /// <returns></returns>
        public PivotColumnDescriptor Clone()
        {
            PivotColumnDescriptor columnClone = new PivotColumnDescriptor();
            columnClone.MappingName = this.MappingName;

            foreach (PivotCellDescriptor cellDescriptor in this.Cells)
                columnClone.Cells.Add(cellDescriptor.Clone());

            return columnClone;
        }
        #endregion

        #region IPivotCellHost Members

      

        #endregion
    }

    /// <summary>
    /// Rows descriptor class can be used to to easily perform
    /// several actions in Column-based model for example Insert or get row at index.
    /// </summary>
    /// <remarks>
    /// Row objects are created dynamically and they are now getting serialized.
    /// User can perform operations with cells in the row, manipulation with cell sequence
    /// will not be saved.
    /// </remarks>
    //public class PivotRowDescriptor
    //    : ICloneable<PivotRowDescriptor>
    //    , IPivotCellHost
    //{
    //    #region class members
    //    private PivotCellCollection m_cells = null;
    //    #endregion

    //    #region class properties
    //    public PivotCellCollection Cells
    //    {
    //        get
    //        {
    //            return m_cells;
    //        }
    //    }
    //    #endregion

    //    #region class initialize/finalize methods
    //    public PivotRowDescriptor()
    //    {
    //        m_cells = new PivotCellCollection();
    //    }
    //    #endregion

    //    #region ICloneable<PivotRowDescriptor> Members
    //    //public PivotRowDescriptor Clone()
    //    //{
    //    //    PivotRowDescriptor descriptor = new PivotRowDescriptor();

    //    //    foreach (PivotCellDescriptor cellDesc in this.Cells)
    //    //        descriptor.Cells.Add(cellDesc.Clone());

    //    //    return descriptor;
    //    //}
    //    #endregion
}
