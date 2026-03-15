//-------------------------------------------------------------------------------------------------
// <copyright file="GridData.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridData holds StyleInfoStore objects with cell specific style properties. Rows and
    /// columns are allocated on demand and only for cells that have actual contents will a 
    /// StyleInfoStore object be allocated. 
    /// </summary>
    /// <remarks>
    /// GridData also holds information about row, column, and table styles and row and column headers.
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(GridDataTypeConverter))]
    public class GridData : ICloneable, ISerializable, IDeserializationCallback
    {
        internal GridRowCollection sfTable;
        private object extendedInfo;
        internal bool modified = false;
        private SFTable table_old;

        private const int delta = 1;

        /// <summary>
        /// Initializes a new <see cref="GridData"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        public GridData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            foreach (SerializationEntry entry in info)
            {
                if (entry.Name == "Rows" && entry.Value is GridRowCollection)
                {
                    sfTable = (GridRowCollection)entry.Value;
                    break;
                }
            }

            if (sfTable == null)
            {
                table_old = (SFTable)info.GetValue("SFTable", typeof(SFTable));
            }

            extendedInfo = info.GetValue("Tag", typeof(object));
            bool mod = true;
            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                if (sie.Name == "Modified")
                {
                    mod = Convert.ToBoolean(sie.Value);
                }
            }

            this.modified = mod;
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            if (sfTable == null && table_old != null)
            {
                sfTable = new GridRowCollection(table_old);
                table_old = null;
            }
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridData"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("Rows", sfTable); // SFTable
            info.AddValue("Tag", extendedInfo); // object
            info.AddValue("Modified", modified);
        }

        /// <summary>
        /// Initializes a new instance of the GridData class.
        /// </summary>
        public GridData()
        {
            sfTable = new GridRowCollection();
            sfTable.RowCount = delta + 1;
            sfTable.ColCount = delta + 1;
        }

        private GridData(GridData data)
        {
            sfTable = (GridRowCollection)data.sfTable.Clone();
        }

        object ICloneable.Clone()
        {
            return new GridData(this);
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary was modified.
        /// </summary>
        public bool Modified
        {
            get
            {
                return modified;
            }
        }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        public void ResetModified()
        {
            modified = false;
        }

        /// <summary>
        /// Resets the row and column count to zero and clears all cells.
        /// </summary>
        public virtual void Clear()
        {
            sfTable.Clear();
            sfTable.RowCount = delta + 1;
            sfTable.ColCount = delta + 1;
            modified = false;
        }

        /// <summary>
        /// Gets or sets the row count in the <see cref="GridData"/> table.
        /// </summary>
        /// <remarks>
        /// The row count does not include headers and column styles. If you set the row count to be 2, there will
        /// be one column header, one column style, and two grid rows with cells.
        /// </remarks>
        public virtual int RowCount
        {
            get
            {
                // There is a delta of two (or delta) rows because GridData has 
                // one first header row (row 0) that is not counted with RowCount
                // and one column styles row (row -1) that is not counted.
                return Math.Max(0, sfTable.RowCount - delta - 1);
            }

            set
            {
                sfTable.RowCount = value + delta + 1;
            }
        }

        /// <summary>
        /// Gets or sets the column count in the <see cref="GridData"/> table.
        /// </summary>
        /// <remarks>
        /// The column count does not include headers and row styles. If you set the column count to be 2, there will
        /// be one row header, one row style, and two grid columns with cells.
        /// </remarks>
        public virtual int ColCount
        {
            get
            {
                // There is a delta of two (or delta) columns because GridData has 
                // one first header column (column 0) that is not counted with ColCount
                // and one row styles column (column -1) that is not counted.
                return Math.Max(0, sfTable.ColCount - delta - 1);
            }

            set
            {
                sfTable.ColCount = value + delta + 1;
            }
        }

        /// <summary>
        /// Gets or sets a custom object or tag to be saved with the <see cref="GridData"/> object. 
        /// </summary>
        /// <remarks>
        /// Clipboard copy / paste will for example store covered ranges information in ExtendedInfo.
        /// </remarks>
        public object ExtendedInfo
        {
            get
            {
                return extendedInfo;
            }

            set
            {
                extendedInfo = value;
            }
        }

        /// <summary>
        /// Insert a specified number of rows at a specified row index.
        /// </summary>
        /// <param name="rowIndex">The starting row index where new rows should be inserted.</param>
        /// <param name="count">The number of rows to insert.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public virtual bool InsertRows(int rowIndex, int count)
        {
            sfTable.InsertRows(rowIndex + 1, count);
            return true;
        }

        /// <summary>
        /// Insert a specified number of columns at a specified column index.
        /// </summary>
        /// <param name="colIndex">The starting column index where new columns should be inserted.</param>
        /// <param name="count">The number of columns to insert.</param>
        /// <returns>true if successful; false otherwise.</returns>
        public virtual bool InsertCols(int colIndex, int count)
        {
            sfTable.InsertCols(colIndex + 1, count);
            return true;
        }

        /// <summary>
        /// Removes a specified number of rows at a specified row index.
        /// </summary>
        /// <param name="rowIndex">The starting row index where rows should be removed.</param>
        /// <param name="count">The number of rows to remove.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public virtual bool RemoveRows(int rowIndex, int count)
        {
            sfTable.RemoveRows(rowIndex + 1, count);
            return true;
        }

        /// <summary>
        /// Removes a specified number of columns at a specified column index.
        /// </summary>
        /// <param name="colIndex">The starting column index where columns should be removed.</param>
        /// <param name="count">The number of columns to remove.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public virtual bool RemoveCols(int colIndex, int count)
        {
            sfTable.RemoveCols(colIndex + 1, count);
            return true;
        }

        /// <summary>
        /// Moves a specified number of rows to a new position.
        /// </summary>
        /// <param name="rowIndex">The starting row index of rows to be moved.</param>
        /// <param name="count">The number of rows to move.</param>
        /// <param name="target">The new position for the rows.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public virtual bool MoveRows(int rowIndex, int count, int target)
        {
            sfTable.MoveRows(rowIndex + 1, count, target + 1);
            return true;
        }

        /// <summary>
        /// Moves a specified number of columns to a new position.
        /// </summary>
        /// <param name="colIndex">The starting column index of columns to be moved.</param>
        /// <param name="count">The number of columns to move.</param>
        /// <param name="target">The new position for the columns.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public virtual bool MoveCols(int colIndex, int count, int target)
        {
            sfTable.MoveCols(colIndex + 1, count, target + 1);
            return true;
        }

        /// <summary>
        /// Copies cell contents from a given range of cells to another range of cells.
        /// </summary>
        /// <param name="sourceRange">The original range of cells that hold cell contents to be copied.</param>
        /// <param name="destRange">The target range where contents will be copied to.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public virtual bool CopyCells(GridRangeInfo sourceRange, GridRangeInfo destRange)
        {
            int rowCount = sourceRange.Height;
            int colCount = sourceRange.Width;

            GridRowCollection newTable = new GridRowCollection();
            newTable.RowCount = rowCount;
            newTable.ColCount = colCount;

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    GridStyleInfoStore store = sfTable[r + sourceRange.Top, c + sourceRange.Left];
                    newTable[r, c] = (GridStyleInfoStore)store.Clone();
                }
            }

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    sfTable[r + destRange.Top, c + destRange.Left] = newTable[r, c];
                }
            }

            return true;
        }

        /// <summary>
        /// Moves cell contents from a given range of cells to another range of cells.
        /// </summary>
        /// <param name="sourceRange">The original range of cells that holds cell contents to be moved. This range cell contents will be cleared after the move operation is complete.</param>
        /// <param name="destRange">The target range where contents will be moved to.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public virtual bool MoveCells(GridRangeInfo sourceRange, GridRangeInfo destRange)
        {
            int rowCount = sourceRange.Height;
            int colCount = sourceRange.Width;

            GridRowCollection newTable = new GridRowCollection();
            newTable.RowCount = rowCount;
            newTable.ColCount = colCount;

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    newTable[r, c] = sfTable[r + sourceRange.Top, c + sourceRange.Left];
                    sfTable[r + sourceRange.Top, c + sourceRange.Left] = null;
                }
            }

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    sfTable[r + destRange.Top, c + destRange.Left] = newTable[r, c];
                }
            }

            return true;
        }

        /// <summary>
        /// Tests if the cell at the specified row and column index has been initialized.
        /// </summary>
        /// <param name="rowIndex">The row index. Can be 0 for column headers and -1 for column styles.</param>
        /// <param name="colIndex">The column index. Can be 0 for row headers and -1 for row styles.</param>
        /// <returns>True if an element is at the specified co-ordinates.</returns>
        public bool Contains(int rowIndex, int colIndex)
        {
            return sfTable.Contains(rowIndex + 1, colIndex + 1);
        }

        /// <summary>
        /// The cell contents at the specified row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index. Can be 0 for column headers and -1 for column styles.</param>
        /// <param name="colIndex">The column index. Can be 0 for row headers and -1 for row styles.</param>
        /// <returns>A <see cref="GridStyleInfoStore"/> with the specified cell's data. Can be NULL if cell has not been initialized
        /// for the specified position.</returns>
        public GridStyleInfoStore this[int rowIndex, int colIndex]
        {
            get
            {
                return sfTable[rowIndex + 1, colIndex + 1] as GridStyleInfoStore;
            }

            set
            {
                sfTable[rowIndex + 1, colIndex + 1] = value;
                modified = true;
            }
        }

        /// <summary>
        /// Gets or sets access to table styles data.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public GridStyleInfoStore TableStyle
        {
            get
            {
                return this[-1, -1];
            }

            set
            {
                this[-1, -1] = value;
                modified = true;
            }
        }

        [NonSerialized]
        private GridDataColStylesIndexer colStyles = null;

        /// <summary>
        /// Gets access to column styles data.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public GridDataColStylesIndexer ColStyles
        {
            get
            {
                if (colStyles == null)
                {
                    colStyles = new GridDataColStylesIndexer(this);
                }

                return colStyles;
            }
        }

        [NonSerialized]
        private GridDataRowStylesIndexer rowStyles = null;

        /// <summary>
        /// Gets access to row styles data.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public GridDataRowStylesIndexer RowStyles
        {
            get
            {
                if (rowStyles == null)
                {
                    rowStyles = new GridDataRowStylesIndexer(this);
                }

                return rowStyles;
            }
        }

        /// <summary>
        /// Gets access to row styles data.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridRowCollection Rows
        {
            get
            {
                return sfTable;
            }
        }

        /// <overload>
        /// Sorts tha data object by a specified column.
        /// </overload>
        /// <summary>
        /// Sorts tha data object ascending by the specified column.
        /// </summary>
        /// <param name="colIndex">The column that holds the key for sorting this object.</param>
        public void SortByColumn(int colIndex)
        {
            SortByColumn(colIndex, ListSortDirection.Ascending);
        }

        /// <summary>
        /// Sorts the data object by the specified column.
        /// </summary>
        /// <param name="colIndex">The column that holds the key for sorting this object.</param>
        /// <param name="direction">The sort direction: ascending or descending.</param>
        public void SortByColumn(int colIndex, ListSortDirection direction)
        {
            SortByColumn(colIndex, direction, 0, null);
        }

        /// <summary>
        /// Sorts the data object by the specified column.
        /// </summary>
        /// <param name="colIndex">The column that holds the key for sorting this object.</param>
        /// <param name="direction">The sort direction: ascending or descending.</param>
        /// <param name="extraHeaderCount">You should pass in GridModel.Rows.HeaderCount to specify if there are additional headers 
        /// displayed in the grid that should not be sorted.</param>
        public void SortByColumn(int colIndex, ListSortDirection direction, int extraHeaderCount)
        {
            SortByColumn(colIndex, direction, extraHeaderCount, null);
        }

        /// <summary>
        /// Sorts the data object by the specified column.
        /// </summary>
        /// <param name="colIndex">The column that holds the key for sorting this object.</param>
        /// <param name="direction">The sort direction: ascending or descending.</param>
        /// <param name="comparer">A custom comparer class that implements a CompareTo method as shown in the example.</param>
        public void SortByColumn(int colIndex, ListSortDirection direction, IComparer comparer)
        {
            SortByColumn(colIndex, direction, 0, comparer);
        }

        /// <summary>
        /// Sorts the data object by the specified column.
        /// </summary>
        /// <param name="colIndex">The column that holds the key for sorting this object.</param>
        /// <param name="direction">The sort direction: ascending or descending.</param>
        /// <param name="extraHeaderCount">You should pass in GridModel.Rows.HeaderCount to specify if there are additional headers 
        /// displayed in the grid that should not be sorted.</param>
        /// <param name="comparer">A custom comparer class that implements a CompareTo method as shown in the example.</param>
        public void SortByColumn(int colIndex, ListSortDirection direction, int extraHeaderCount, IComparer comparer)
        {
            int hc = extraHeaderCount + 2;
            ArrayList sortArray = new ArrayList();
            int count = RowCount - extraHeaderCount;
            for (int n = 0; n < count; n++)
            {
                sortArray.Add(sfTable.Rows[n + hc]);
            }

            GridColumnSorter sorter = new GridColumnSorter(colIndex + 1, direction, sortArray, comparer);
            sortArray.Sort(sorter);
            for (int n = 0; n < sortArray.Count; n++)
            {
                sfTable.Rows[n + hc] = sortArray[n];
            }
        }
    }

    class GridColumnSorter : IComparer
    {
        int colIndex;
        ListSortDirection direction;
        ArrayList array;
        IComparer styleComparer;

        public GridColumnSorter(int colIndex, ListSortDirection direction, ArrayList array)
        {
            this.colIndex = colIndex;
            this.direction = direction;
            this.array = array;
            this.styleComparer = null;
        }

        public GridColumnSorter(int colIndex, ListSortDirection direction, ArrayList array, IComparer styleComparer)
        {
            this.colIndex = colIndex;
            this.direction = direction;
            this.array = array;
            this.styleComparer = styleComparer;
        }

        public int Compare(object obj1, object obj2)
        {
            int r = _Compare(obj1, obj2);
            if (r == 0)
            {
                return this.array.IndexOf(obj1).CompareTo(array.IndexOf(obj2));
            }

            return direction == ListSortDirection.Ascending ? r : -r;
        }

        int _Compare(object obj1, object obj2)
        {
            SFArrayList item1 = obj1 as SFArrayList;
            SFArrayList item2 = obj2 as SFArrayList;

            GridStyleInfoStore style1 = null;
            GridStyleInfoStore style2 = null;

            if (item1 != null)
            {
                style1 = item1[colIndex] as GridStyleInfoStore;
            }

            if (item2 != null)
            {
                style2 = item2[colIndex] as GridStyleInfoStore;
            }

            if (this.styleComparer != null)
            {
                return this.styleComparer.Compare(style1, style2);
            }

            return DefaultCompare(style1, style2);
        }

        int DefaultCompare(GridStyleInfoStore style1, GridStyleInfoStore style2)
        {
            object val1 = null;
            object val2 = null;

            if (style1 != null)
            {
                val1 = style1.GetValue(GridStyleInfoStore.CellValueProperty);
            }

            if (style2 != null)
            {
                val2 = style2.GetValue(GridStyleInfoStore.CellValueProperty);
            }

            if (val1 == null && val2 == null)
            {
                return 0;
            }
            else if (val1 == null)
            {
                return -1;
            }
            else if (val2 == null)
            {
                return 1;
            }
            else
            {
                try
                {
                    if (val1 is IComparable && val2 is IComparable)
                    {
                        int r = ((IComparable)val1).CompareTo(val2);
                        ////Trace.WriteLine(val1.ToString() + " / " + val2.ToString() + " : " + r.ToString());
                        return r;
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }

                return val1.ToString().CompareTo(val2.ToString());
            }
        }
    }

    /// <summary>
    /// Provides access to row styles data.
    /// </summary>
    /// <remarks>
    /// You access this object using the <see cref="GridData.RowStyles"/> property of a <see cref="GridData"/> instance.
    /// </remarks>
    public class GridDataRowStylesIndexer
    {
        GridData dataParent;
        internal GridDataRowStylesIndexer(GridData data)
        {
            dataParent = data;
        }

        /// <summary>
        /// The <see cref="GridStyleInfoStore"/> with style information at the given index.
        /// </summary>
        public GridStyleInfoStore this[int rowIndex]
        {
            get
            {
                return dataParent[rowIndex, -1];
            }

            set
            {
                dataParent[rowIndex, -1] = value;
                dataParent.modified = true;
            }
        }
    }

    /// <summary>
    /// Provides access to column styles data.
    /// </summary>
    /// <remarks>
    /// You access this object using the <see cref="GridData.ColStyles"/> property of a <see cref="GridData"/> instance.
    /// </remarks>
    public class GridDataColStylesIndexer
    {
        GridData dataParent;
        internal GridDataColStylesIndexer(GridData data)
        {
            dataParent = data;
        }

        /// <summary>
        /// The <see cref="GridStyleInfoStore"/> with style information at the given index.
        /// </summary>
        public GridStyleInfoStore this[int colIndex]
        {
            get
            {
                return dataParent[-1, colIndex];
            }

            set
            {
                dataParent[-1, colIndex] = value;
                dataParent.modified = true;
            }
        }
    }

    /// <internalonly/>
    /// <summary>For internal use.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridDataTypeConverter : TypeConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridDataTypeConverter()
            : base()
        {
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        sealed class Binder : SerializationBinder
        {
            public override Type BindToType(
                string assemblyName, string typeName)
            {
                Type t = Type.GetType(typeName);

                if (t != null)
                {
                    return t;
                }
                else
                {
#if SINGLE_DLL_BUILD
                    t = Syncfusion.SharedBaseAssembly.Assembly.GetType(typeName);
#else
                    t = Syncfusion.SharedBaseBaseAssembly.Assembly.GetType(typeName);
#endif
                }

                if (t != null)
                {
                    return t;
                }
                else
                {
                    t = Syncfusion.AssemblyInfo.Assembly.GetType(typeName);
                }

                return t;
            }
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(byte[]))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            byte[] bytes = value as byte[];

            if (bytes == null)
            {
                return base.ConvertFrom(context, culture, value);
            }

            object o = null;
            MemoryStream ms = new MemoryStream(bytes);
            BinaryFormatter bf = new BinaryFormatter();
            bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            bf.Binder = new Binder();
            o = bf.Deserialize(ms);
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            ms.Close();

            return o as GridData;
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(byte[]))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            GridData serType = value as GridData;

            if (serType != null && destinationType == typeof(byte[]))
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.AssemblyFormat = FormatterAssemblyStyle.Simple;

                bf.Serialize(ms, value);
                byte[] bytes = ms.ToArray();
                ms.Close();
                return bytes;
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    /// <summary>
    /// Derives from <see cref="SFArrayList"/> and provides a strongly-typed <see cref="Add"/> 
    /// method and a strongly typed indexer (<see cref="GridCellCollection.this[int]"/>).
    /// </summary>
    [Serializable]
    public class GridCellCollection : SFArrayList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SFArrayList" />
        /// class that is empty and has the default initial capacity.
        /// </summary>
        public GridCellCollection()
            : base() 
        { 
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SFArrayList" /> class that contains elements copied from the specified
        /// collection and that has the same initial capacity as the number of elements copied.</para>
        /// </summary>
        /// <param name="c">The <see cref="System.Collections.ICollection" /> whose elements are copied to the new list.</param>
        public GridCellCollection(ICollection c)
            : base(c) 
        { 
        }

        /// <summary>
        ///   <para>Creates a deep copy of the <see cref="SFArrayList" />.</para>
        /// </summary>
        /// <returns>
        ///   <para>A deep copy of the <see cref="SFArrayList" />.</para>
        /// </returns>
        public override object Clone()
        {
            GridCellCollection al = new GridCellCollection();
            foreach (GridStyleInfoStore o in this)
            {
                al.Add(o.Clone());
            }

            return al;
        }

        /// <summary>
        /// Gets / sets a <see cref="GridStyleInfoStore"/> for the specified index.
        /// </summary>
        public new GridStyleInfoStore this[int index]
        {
            set
            {
                base[index] = value;
            }

            get
            {
                return base[index] as GridStyleInfoStore;
            }
        }

        /// <summary>
        /// Appends a <see cref="GridStyleInfoStore"/> object to the list.
        /// </summary>
        /// <param name="store">The object to be added.</param>
        /// <returns>The index of the new object.</returns>
        public int Add(GridStyleInfoStore store)
        {
            return base.Add(store);
        }
    }

    /// <summary>
    /// Implements a two-dimensional table that holds an array of <see cref="GridCellCollection"/> 
    /// arrays with <see cref="GridStyleInfoStore"/> items. 
    /// </summary>
    /// <remarks>
    /// <p>This is a memory efficient way to represent a table where values can remain empty. Only rows
    /// that actually contain data will allocate a <see cref="GridCellCollection" /> and the array only holds
    /// as many <see cref="GridStyleInfoStore"/>objects as the specific row contains columns.</p>
    /// <p>When you access data that are out of range, an empty () object will be returned.
    /// If you set data that are out of range, an exception will be thrown. If you set data for
    /// a row that was empty, the row will be allocated before the value is stored.</p>
    /// <p>GridRowCollection provides methods that let you insert, remove, or rearrange columns or rows
    /// in the table.</p>
    /// </remarks>
    /// <seealso cref="GridCellCollection"/>
    [Serializable]
    public class GridRowCollection : SFTable, IList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowCollection"/> class from the specified instances
        /// of the <see cref="SerializationInfo"/> and <see cref="StreamingContext"/> classes.
        /// </summary>
        /// <param name="info">An instance of the <see cref="System.Runtime.Serialization.SerializationInfo"/> class containing the information required to serialize the new <see cref="GridRowCollection"/> instance.</param>
        /// <param name="context">An instance of the <see cref="System.Runtime.Serialization.StreamingContext"/> class containing the source of the serialized stream associated with the new <see cref="GridRowCollection"/> instance. </param>
        /// <remarks>This constructor implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface for the <see cref="GridRowCollection"/>  class.</remarks>
        protected GridRowCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        /// <override/>
        /// <summary>
        /// Returns the data needed to serialize the <see cref="SFTable"/>.
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> object containing the information required 
        /// to serialize the object.</param>
        /// <param name="context">A <see cref="StreamingContext"/> object containins the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            base.GetObjectData(info, context);
        }

        /// <overload>
        /// Initializes a new instance of the <see cref="GridRowCollection" />
        /// class.
        /// </overload>
        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowCollection" />
        /// class that is empty.
        /// </summary>
        public GridRowCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowCollection" />
        /// class from an existing table. References to cell objects will be
        /// copied into this table from the original table.
        /// </summary>
        /// <param name="data">The original table.</param>
        /// <remarks>
        /// For each row in the original table, a <see cref="GridCellCollection"/>
        /// is added to this GridRowCollection and the references to the cell objects will be copied
        /// into the new <see cref="GridCellCollection"/>.
        /// </remarks>
        public GridRowCollection(SFTable data)
            : base(data, false)
        {
            int count = data.Rows.Count;

            Rows.EnsureCount(count);
            for (int n = 0; n < count; n++)
            {
                ArrayList al = (ArrayList)data.Rows[n];
                if (al != null)
                {
                    GridCellCollection c = new GridCellCollection();
                    this[n] = c;
                    for (int i = 0; i < al.Count; i++)
                    {
                        if (al[i] != null)
                        {
                            ////    Trace.WriteLine(al[i].ToString());
                            c.Add((GridStyleInfoStore)al[i]);
                        }
                        else
                        {
                            c.Add(null);
                        }
                    }
                }
            }
            ////Trace.WriteLine("GridRowCollection");
        }

        /// <summary>
        /// Creates an empty <see cref="GridCellCollection"/>.
        /// </summary>
        /// <returns>An empty <see cref="GridCellCollection"/>.</returns>
        public override SFArrayList CreateCellCollection()
        {
            return new GridCellCollection();
        }

        /// <summary>
        ///   <para>Creates a deep copy of the <see cref="GridRowCollection" /> where each row and cell object is cloned.</para>
        /// </summary>
        /// <returns>
        ///   <para>A deep copy of the <see cref="GridRowCollection" />.</para>
        /// </returns>
        public override object Clone()
        {
            GridRowCollection newColl = new GridRowCollection();
            int count = Rows.Count;
            newColl.Rows.EnsureCount(count);
            for (int n = 0; n < count; n++)
            {
                newColl.Rows[n] = (GridCellCollection)this[n].Clone();
            }

            return new GridRowCollection(this);
        }

        /// <summary>
        ///   <para>Gets / sets a <see cref="GridStyleInfoStore"/> object at the 
        ///   specified coordinates in the <see cref="GridRowCollection" />.</para>
        /// </summary>
        /// <param name="rowIndex">The zero-based row index.</param>
        /// <param name="colIndex">The zero-based column index.</param>
        /// <remarks>
        /// If you query for an element and the coordinates are out of range, an empty (<see langword="null" />) object will be returned.<para/>
        /// If you set an element and the coordinates are out of range, an exception is thrown.
        /// </remarks>
        public new GridStyleInfoStore this[int rowIndex, int colIndex]
        {
            get
            {
                return base[rowIndex, colIndex] as GridStyleInfoStore;
            }

            set
            {
                base[rowIndex, colIndex] = value;
            }
        }

        #region IList Members

        /// <summary>
        /// Gets a value indicating whether the array is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return Rows.IsReadOnly;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return Rows[index];
            }

            set
            {
                Rows[index] = value;
            }
        }

        /// <summary>
        ///   Gets / sets a <see cref="GridCellCollection"/> object at the 
        ///   specified row index.
        /// </summary>
        /// <param name="index">The zero-based row index.</param>
        public GridCellCollection this[int index]
        {
            get
            {
                return (GridCellCollection)((IList)this)[index];
            }

            set
            {
                ((IList)this)[index] = value;
            }
        }

        /// <summary>
        /// Removes the <see cref="GridCellCollection"/> at the specified row index.
        /// </summary>
        /// <param name="index">Index of the value to be removed.</param>
        public void RemoveAt(int index)
        {
            Rows.RemoveAt(index);
        }

        void IList.Insert(int index, object value)
        {
            Rows.Insert(index, value);
        }

        /// <summary>
        /// Inserts a <see cref="GridCellCollection"/> at the specified row index.
        /// </summary>
        /// <param name="index">Row index.</param>
        /// <param name="value">Value to insert.</param>
        public void Insert(int index, GridCellCollection value)
        {
            ((IList)this).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            Rows.Remove(value);
        }

        /// <summary>
        /// Removes the <see cref="GridCellCollection"/> from the array.
        /// </summary>
        /// <param name="value">Value to remove</param>
        public void Remove(GridCellCollection value)
        {
            ((IList)this).Remove(value);
        }

        bool IList.Contains(object value)
        {
            return Rows.Contains(value);
        }

        /// <summary>
        /// Determines if the array contains the specified <see cref="GridCellCollection"/>.
        /// </summary>
        /// <param name="value">Value to search.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(GridCellCollection value)
        {
            return ((IList)this).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return Rows.IndexOf(value);
        }

        /// <summary>
        /// Determines the index of the specified <see cref="GridCellCollection"/> in the array.
        /// </summary>
        /// <param name="value">The value whose index needs to be retrieved.</param>
        /// <returns>returns Index.</returns>
        public int IndexOf(GridCellCollection value)
        {
            return ((IList)this).IndexOf(value);
        }

        int IList.Add(object value)
        {
            return Rows.Add(value);
        }

        /// <summary>
        /// Adds a <see cref="GridCellCollection"/> to the array.
        /// </summary>
        /// <param name="value">Value to add.</param>
        /// <returns>The position into which the value was inserted</returns>
        public int Add(GridCellCollection value)
        {
            return ((IList)this).Add(value);
        }

        /// <summary>
        /// Gets a value indicating whether IsFixedSize. Always false.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return Rows.IsSynchronized;
            }
        }

        /// <summary>
        /// Gets the number of row elements in this array.
        /// </summary>
        public int Count
        {
            get
            {
                return Rows.Count;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Rows.CopyTo(array, index);
        }

        /// <summary>
        /// Copied references of row elements to the specified array, starting at a particular index.
        /// </summary>
        /// <param name="array">The target collection.</param>
        /// <param name="index">The starting index.</param>
        public void CopyTo(GridCellCollection[] array, int index)
        {
            ((IList)this).CopyTo(array, index);
        }

        /// <summary>
        /// Gets not supported.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return Rows.SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator for the list of rows.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        #endregion
    }
}
