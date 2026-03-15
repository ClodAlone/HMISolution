#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;


#if SILVERLIGHT
namespace Syncfusion.OlapSilverlight.Engine
#else
using Syncfusion.Olap.Common;
namespace Syncfusion.Olap.Engine
#endif
{
    /// <summary>
    /// Collection used to store cells.
    /// </summary>

#if !SILVERLIGHT
    [Serializable]
#endif
    public class PivotCellCollection : List<PivotCellDescriptor>
    {
        #region indexer
        /// <summary>
        /// Releases all resources used by collection.
        /// </summary>
        public void Dispose()
        {
            this.Clear();
        }

        /// <summary>
        /// Creates cloned collection.
        /// </summary>
        /// <returns>Cloned collection.</returns>
        public PivotCellCollection Clone()
        {
            PivotCellCollection collection = new PivotCellCollection();

            for (int i = 0; i < Count; i++)
                collection.Add(this[i].Clone());

            return collection;
        }
        #endregion

    }

    /// <summary>
    /// Descriptor for pivot column.
    /// </summary>
#if !SILVERLIGHT
    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
#endif
    public class PivotColumnDescriptor
#if !SILVERLIGHT
 : ICloneable<PivotColumnDescriptor>
#endif

    {
        #region class members
        private PivotCellCollection m_cells = null;
        private string m_mappingName = string.Empty;
        #endregion

        #region class properties
        /// <summary>
        /// Gets or sets the name of the mapping.
        /// </summary>
        /// <value>The name of the mapping.</value>
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
#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Browsable(true), NotifyParentProperty(true)]
#endif
        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <value>The cells.</value>
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
        /// Initializes a new instance of the <see cref="PivotColumnDescriptor"/> class.
        /// </summary>
        public PivotColumnDescriptor()
        {
            m_cells = new PivotCellCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotColumnDescriptor"/> class.
        /// </summary>
        /// <param name="cellCount">The cell count.</param>
        public PivotColumnDescriptor(int cellCount)
        {
            m_cells = new PivotCellCollection();
            for (int cell = 0; cell < cellCount; cell++)
            {
                m_cells.Add(new PivotCellDescriptor());
            }
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

    public class PivotRowDescriptor
#if !SILVERLIGHT
 : ICloneable<PivotRowDescriptor>
#endif
    {
        #region class members
        private PivotCellCollection m_cells = null;
        #endregion

        #region class properties
        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <value>The cells.</value>
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
        /// Initializes a new instance of the <see cref="PivotRowDescriptor"/> class.
        /// </summary>
        public PivotRowDescriptor()
        {
            m_cells = new PivotCellCollection();
        }
        #endregion

        #region ICloneable<PivotRowDescriptor> Members
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public PivotRowDescriptor Clone()
        {
            PivotRowDescriptor descriptor = new PivotRowDescriptor();

            foreach (PivotCellDescriptor cellDesc in this.Cells)
                descriptor.Cells.Add(cellDesc.Clone());

            return descriptor;
        }
        #endregion
    }
}
