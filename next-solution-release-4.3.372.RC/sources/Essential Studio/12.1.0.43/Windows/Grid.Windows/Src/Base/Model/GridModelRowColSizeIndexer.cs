//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelRowColSizeIndexer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Security;
using System.Security.Permissions;
using System.Globalization;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines an interface used by <see cref="GridModelRowColSizeIndexer"/> to store row heights and column widths.
    /// </summary>
    public interface IGridRowColSizeDictionary
    {
        /// <summary>
        /// Occurs when rows or columns are moved.
        /// </summary>
        /// <param name="from">First row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        /// <param name="dest">The Destination.</param>
        void MoveItems(int from, int count, int dest);

        /// <summary>
        /// Occurs when rows or columns are removed.
        /// </summary>
        /// <param name="from">First row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        void RemoveItems(int from, int count);

        /// <summary>
        /// Occurs when rows or columns are inserted.
        /// </summary>
        /// <param name="index">Row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        void InsertItems(int index, int count);

        /// <summary>
        /// The row height or column with a given index.
        /// </summary>
        int/*float*/ this[int index] 
        { 
            get; set; 
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary was modified.
        /// </summary>
        bool Modified { get; }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        void ResetModified();
    }

    /// <internalonly/>
    /// <summary>Internal only.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridRowColSizeDictionaryTypeConverter : TypeConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridRowColSizeDictionaryTypeConverter()
            : base()
        {
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        sealed class Binder : SerializationBinder
        {
            /// <summary>
            /// Internal only.
            /// </summary>
            /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly"/> name of the serialized object.</param>
            /// <param name="typeName">Specifies the <see cref="T:System.Type"/> name of the serialized object.</param>
            /// <returns>
            /// The type of the object the formatter creates a new instance of.
            /// </returns>
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
                    return null;
                }
            }
        }

        /// <summary>
        /// Internal only.
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
        /// Internal only.
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

            return o as GridRowColSizeDictionary;
        }

        /// <summary>
        /// Internal only.
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
        /// Internal only.
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
            GridRowColSizeDictionary serType = value as GridRowColSizeDictionary;

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
    /// This class implements the default dictionary used by the <see cref="GridModelRowColSizeIndexer"/> 
    /// to store row heights and column widths.
    /// </summary>
    /// <remarks>
    /// This is the default implementation for <see cref="IGridRowColSizeDictionary"/>.
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(GridRowColSizeDictionaryTypeConverter))]
    public class GridRowColSizeDictionary : IGridRowColSizeDictionary, ISerializable
    {
        internal GridIndexDictionary innerDict = new GridIndexDictionary(typeof(int));
        internal bool isModified = false;

        /// <summary>
        /// Initialize a new instance of GridRowColSizeDictionary.
        /// </summary>
        public GridRowColSizeDictionary()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dictionary was modified.
        /// </summary>
        public bool Modified
        {
            get
            {
                return isModified;
            }

            set
            {
                isModified = value;
            }
        }

        /// <internalonly/>
        /// <summary>Gets InnerDictionary. Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridIndexDictionary InnerDict
        {
            get
            {
                return innerDict;
            }
        }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        public void ResetModified()
        {
            isModified = false;
        }

        /// <summary>
        /// Initializes a new <see cref="GridRowColSizeDictionary"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridRowColSizeDictionary(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            innerDict = (GridIndexDictionary)info.GetValue("Dictionary", typeof(GridIndexDictionary));
            bool mod = true;
            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                if (sie.Name == "Modified")
                {
                    mod = Convert.ToBoolean(sie.Value);
                }
            }

            this.isModified = mod;
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridRowColSizeDictionary"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("Dictionary", innerDict); // GridIndexDictionary
            info.AddValue("Modified", this.isModified);
        }

        /// <summary>
        /// Occurs when rows or columns are moved.
        /// </summary>
        /// <param name="from">First row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        /// <param name="dest">The Destination.</param>
        /// <remarks>
        /// <seealso cref="IGridRowColSizeDictionary.MoveItems"/>
        /// </remarks>
        public void MoveItems(int from, int count, int dest)
        {
            innerDict.MoveIndex(from, count, dest);
        }

        /// <summary>
        /// Occurs when rows or columns are removed.
        /// </summary>
        /// <param name="from">First row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        /// <remarks>
        /// <seealso cref="IGridRowColSizeDictionary.RemoveItems"/>
        /// </remarks>
        public void RemoveItems(int from, int count)
        {
            innerDict.RemoveIndex(from, count);
        }

        /// <summary>
        /// Occurs when rows or columns are inserted.
        /// </summary>
        /// <param name="index">Row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        /// <remarks>
        /// <seealso cref="IGridRowColSizeDictionary.InsertItems"/>
        /// </remarks>
        public void InsertItems(int index, int count)
        {
            innerDict.InsertIndex(index, count);
        }

        /// <summary>
        /// The size of a given row or column index.
        /// </summary>
        /// <remarks>
        /// <seealso cref="IGridRowColSizeDictionary.this[int]"/>
        /// </remarks>
        public int/*float*/ this[int index]
        {
            get
            {
                int/*float*/ value;
                if (!innerDict.Lookup(index, out value))
                {
                    value = -1;
                }

                return value;
            }

            set
            {
                if (value >= 0)
                {
                    innerDict[index] = value;
                }
                else
                {
                    innerDict.Remove(index);
                }

                this.isModified = true;
            }
        }
    }

    /// <summary>
    /// This is an abstract base class that manages row heights
    /// and column widths in a grid and lets you change them.
    /// Events will be raised in the grid when settings are changed.
    /// </summary>
    /// <remarks>
    /// You typically access this class from a grid using the <see cref="GridModel.RowHeights"/>
    /// and <see cref="GridModel.ColWidths"/> properties of a <see cref="GridModel"/>.
    /// <para/>
    /// This class raises the following events in a <see cref="GridModel"/>:
    /// <list type="bullet">
    /// <listheader><term>Items</term><description>Descriptions</description></listheader>
    /// <item><term><see cref="GridModel.RowHeightsChanged"/></term></item>
    /// <item><term><see cref="GridModel.RowHeightsChanging"/></term></item>
    /// <item><term><see cref="GridModel.ColWidthsChanged"/></term></item>
    /// <item><term><see cref="GridModel.ColWidthsChanging"/></term></item>
    /// <item><term><see cref="GridModel.QueryColWidth"/></term></item>
    /// <item><term><see cref="GridModel.QueryRowHeight"/></term></item>
    /// <item><term><see cref="GridModel.SaveColWidth"/></term></item>
    /// <item><term><see cref="GridModel.SaveRowHeight"/></term></item>
    /// </list>
    /// </remarks>
    [Serializable]
    public abstract class GridModelRowColSizeIndexer : GridModelBound, ISerializable
    {
        // Fields
        private IGridRowColSizeDictionary dictionary;

        // Events

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public abstract void OnChanged(GridRowColSizeChangedEventArgs e);

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public abstract bool OnChanging(GridRowColSizeChangingEventArgs e);

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void OnSaveSize(GridRowColSizeEventArgs e);

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void OnQuerySize(GridRowColSizeEventArgs e);

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void OnQuerySizeTotal(GridRowColSizeTotalEventArgs e);
        
        // Constructor.

        /// <summary>
        /// Initializes a new <see cref="GridModelRowColSizeIndexer"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridModelRowColSizeIndexer(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            { 
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount); 
            }
#else
               
            ;
#endif
            dictionary = (IGridRowColSizeDictionary)info.GetValue("SizeDictionary", typeof(IGridRowColSizeDictionary));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModelRowColSizeIndexer"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("SizeDictionary", dictionary);
        }

        /// <summary>
        /// Initializes a <see cref="GridModelRowColSizeIndexer"/> and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        protected GridModelRowColSizeIndexer(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Gets "RowHeight" or "ColumnWidth" string.
        /// </summary>
        public abstract string RowColName { get; }

        /// <summary>
        /// Gets a reference to <see cref="GridModel.Rows"/> or <see cref="GridModel.Cols"/>.
        /// </summary>
        public abstract GridModelRowColOperations RowColObject { get; }

        /// <summary>
        /// Gets a reference to <see cref="GridModel.HideRows"/> or <see cref="GridModel.HideCols"/>.
        /// </summary>
        public GridModelHideRowColsIndexer IsHidden
        {
            get
            {
                return RowColObject.Hidden;
            }
        }

        /// <summary>
        /// Gets or sets storage for all row heights or column widths in the grid. 
        /// </summary>
        /// <remarks>
        /// You can replace this dictionary at run-time with a custom dictionary 
        /// if you implement the <see cref="IGridRowColSizeDictionary"/> interface.
        /// </remarks>
        public IGridRowColSizeDictionary Dictionary
        {
            get
            {
                if (dictionary == null)
                {
                    dictionary = new GridRowColSizeDictionary();
                }

                return dictionary;
            }

            set
            {
                dictionary = value;
            }
        }

        /// <overload>
        /// Returns or sets the row height or column width of the specified row or column. 
        /// </overload>
        /// <summary>
        /// Returns or sets the row height or column width for the specified index. Hidden rows or
        /// columns will return 0. If row or column has default size, the DefaultSize is returned.
        /// </summary>
        /// <remarks>
        /// Call ResetRange reset values to default. 
        /// Call IsDefault to check if a value is reset to default.
        /// Call IsHidden[n] to check if a row or column is hidden.
        /// </remarks>
        public int/*float*/ this[int index]
        {
            get
            {
                if (IsHidden[index])
                {
                    return 0;
                }

                int/*float*/ size = GetSize(index);
                return size < 0 ? RowColObject.DefaultSize : size;
            }

            set
            {
                SetRange(index, index, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary was modified.
        /// </summary>
        public bool Modified
        {
            get { return this.Dictionary.Modified; }
        }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        public void ResetModified()
        {
            this.Dictionary.ResetModified();
        }

        /// <summary>
        /// Returns or sets the row height or column width for the row or column that matches the specified name. Hidden rows or
        /// columns will return 0. If row or column has default size, the DefaultSize is returned.
        /// </summary>
        /// <remarks>
        /// Call ResetRange reset values to default. 
        /// Call IsDefault to check if a value is reset to default.
        /// Call IsHidden[n] to check if a row or column is hidden.
        /// </remarks>
        public abstract int/*float*/ this[string name] 
        { 
            get; set; 
        }

        /// <summary>
        /// Gets an array of row or column sizes. The array will have negative values for 
        /// rows and columns that are reset default size. Hidden rows and columns are returned
        /// in their original size.
        /// </summary>
        /// <param name="from">First row or column.</param>
        /// <param name="last">Last row or column.</param>
        /// <returns>An array with row and column sizes.</returns>
        /// <remarks>
        /// Call EvalRange to change the array and determine actual sizes for rows or
        /// or columns that are reset to default or hidden.
        /// </remarks>
        public int/*float*/[] GetRange(int from, int last)
        {
            int count = last - from + 1;
            int/*float*/[] values = new int/*float*/[count];
            for (int n = 0; n < count; n++)
            {
                values[n] = GetSize(from + n);
            }

            return values;
        }

        /// <overload>
        /// Call EvalRange to change a given array of row and column sizes and determine 
        /// actual sizes for rows or columns that are reset to default or hidden.
        /// </overload>
        /// <summary>
        /// Call EvalRange to change a given array of row and column sizes and determine 
        /// actual sizes for rows or columns that are reset to default or hidden.
        /// </summary>
        /// <param name="startAt">First row or column index.</param>
        /// <param name="values">The array to be evaluated. The values in the array will be 
        /// changed.</param>
        /// <returns>A reference to the values array (see param values).</returns>
        public int/*float*/[] EvalRange(int startAt, int/*float*/[] values)
        {
            return EvalRange(startAt, values, RowColObject.Hidden.GetRange(startAt, startAt + values.Length - 1), RowColObject.DefaultSize);
            /*
                    if (values == null)
                        throw new ArgumentNullException("values");
                    int defaultSize = RowColObject.DefaultSize;
                    GridModelHideRowColsIndexer hidden = RowColObject.Hidden;
                    int count = values.Length;
                    for (int n = 0; n < count; n++)
                    {
                        if (hidden[startAt+n])
                            values[n] = 0;
                        else if (values[n] < 0)
                            values[n] = defaultSize;
                    }*/
        }

        /// <summary>
        /// Call EvalRange to change a given array of row and column sizes and determine 
        /// actual sizes for rows or columns that are reset to default or hidden.
        /// </summary>
        /// <param name="startAt">First row or column index.</param>
        /// <param name="values">The array to be evaluated. The values in the array will be 
        /// changed.</param>
        /// <param name="hidden">An array that specifies which rows or columns are hidden.</param>
        /// <param name="defaultSize">The default row or column width in the grid.</param>
        /// <returns>A reference to the values array (see param values).</returns>
        public int/*float*/[] EvalRange(int startAt, int/*float*/[] values, bool[] hidden, int/*float*/ defaultSize)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            int count = values.Length;
            for (int n = 0; n < count; n++)
            {
                if (hidden != null && n < hidden.Length && hidden[n])
                {
                    values[n] = 0;
                }
                else if (values[n] < 0)
                {
                    values[n] = defaultSize;
                }
            }

            return values;
        }

        /// <summary>
        /// Gets the total size for the specified range of rows or columns.
        /// </summary>
        /// <param name="from">The first row or column.</param>
        /// <param name="last">The last row or column.</param>
        /// <returns>The total size.</returns>
        public int/*float*/ GetTotal(int from, int last)
        {
            return GetTotal(from, last, int/*float*/.MaxValue);
        }

        /// <summary>
        /// Gets the total size for the specified range of rows or columns.
        /// </summary>
        /// <param name="from">The first row or column.</param>
        /// <param name="last">The last row or column.</param>
        /// <param name="maximum">Maximum value for total.</param>
        /// <returns>The total size.</returns>
        public int/*float*/ GetTotal(int from, int last, int/*float*/ maximum)
        {
            GridRowColSizeTotalEventArgs e = new GridRowColSizeTotalEventArgs(from, last, maximum, 0);
            OnQuerySizeTotal(e);
            if (e.Handled)
            {
                return e.Size;
            }
            else
            {
                int/*float*/ defaultSize = RowColObject.DefaultSize;
                GridModelHideRowColsIndexer hidden = RowColObject.Hidden;
                int/*float*/ total = 0;
                for (int n = from; n <= last && total <= maximum; n++)
                {
                    if (!hidden[n])
                    {
                        int/*float*/ size = GetSize(n);
                        total += size < 0 ? defaultSize : size;
                    }
                }

                return total;
            }
        }

        /// <summary>
        /// Determines if row or column at the specified index is reset to default.
        /// </summary>
        /// <param name="index">Row or column index.</param>
        /// <returns>True if value is reset to default; False otherwise.</returns>
        public bool IsDefault(int index)
        {
            int/*float*/ size = GetSize(index);
            return size < 0;
        }

        /// <summary>
        /// Resets the row or column to default size.
        /// </summary>
        /// <param name="index">Row or column index.</param>
        public void ResetSize(int index)
        {
            SetSize(index, -1);
        }

        /// <summary>
        /// Resets the range of rows or columns to default size.
        /// </summary>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        public void ResetRange(int from, int last)
        {
            SetRange(from, last, -1);
        }

        /// <overload>
        /// Changes the size of a range of rows or columns.
        /// </overload>
        /// <summary>
        /// Changes the size of a range of rows or columns.
        /// </summary>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="value">The size to be applied.</param>
        /// <remarks>
        /// The method will generate undo information and push it onto the
        /// grid's command stack. <para/>
        /// A <see cref="GridModel.RowHeightsChanging"/> or (<see cref="GridModel.ColWidthsChanging"/>) event 
        /// is raised before the values are modified and gives
        /// event listeners a chance to discard the operation before any change happens.<para/>
        /// If the <see cref="GridModel.RowHeightsChanging"/> event did not signal to cancel, the operation
        /// will go ahead, apply changes and raise a <see cref="GridModel.RowHeightsChanged"/> or (<see cref="GridModel.ColWidthsChanged"/>) 
        /// event. The Changed event will indicate if changes were successful or not.
        /// </remarks>
        public void SetRange(int from, int last, int/*float*/ value)
        {
            SetRange(from, last, new int/*float*/[] { value });
        }

        /// <summary>
        /// Changes the size of a range of rows or columns.
        /// </summary>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">An array with sizes to be applied.</param>
        /// <genoverload/>
        public void SetRange(int from, int last, int/*float*/[] values)
        {
            SetRange(from, last, values, false);
        }

        /// <summary>
        /// Changes the size of rows or columns in the specified range.
        /// </summary>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">The sizes to be applies to the range.</param>
        /// <param name="discardUndo">Set this true if you do not want undo information to 
        /// be pushed onto the grid's command stack.</param>
        /// <genoverload/>
        public void SetRange(int from, int last, int/*float*/[] values, bool discardUndo)
        {
            Model.NotifyChangingLayoutCells(RowColObject.CreateRangeFromTo(from, GridConstants.MaxRowCol));
            Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, this.RowColName + ".SetRange(int, int, int)");
            try
            {
                if (OnChanging(new GridRowColSizeChangingEventArgs(from, last, values)))
                {
                    bool success = false;
                    int/*float*/[] savedValues = null;
                    try
                    {
                        savedValues = GetRange(from, last);
                        int count = savedValues.Length;

                        for (int n = 0; n < count; n++)
                        {
                            SetSize(from + n, values[values.Length > 1 ? n : 0]);
                        }

                        RowColObject.DelayFloatingCells(from, last);

                        success = true;

                        ////                        if (!discardUndo && model.CommandList.ShouldRecordCommandInfo)
                        ////                            model.CommandList.Add(new GridModelSetRowColSizeCommand(this, from, last, values));

                        if (!discardUndo && model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            model.CommandStack.Push(new GridModelSetRowColSizeCommand(this, from, last, savedValues));
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
                    finally
                    {
                        OnChanged(new GridRowColSizeChangedEventArgs(from, last, savedValues, success));
                    }
                }
            }
            finally
            {
                Model.EndUpdate();
                Model.NotifyChangedLayoutCells(RowColObject.CreateRangeFromTo(from, GridConstants.MaxRowCol));
            }
        }

        /// <summary>
        /// Saves the raw size data for the given row or column.
        /// </summary>
        /// <remarks>
        /// A negative value means that the size of the row or column should be reset 
        /// to default. This method will not raise neither Changing nor Changed events.
        /// </remarks>
        /// <param name="index">The row or column index.</param>
        /// <param name="value">The new size of the row or column. -1 if the size 
        /// of the row or column should be reset to default. </param>
        public void SetSize(int index, int/*float*/ value)
        {
            GridRowColSizeEventArgs e = new GridRowColSizeEventArgs(index, value);
            OnSaveSize(e);
            if (!e.Handled)
            {
                Dictionary[index] = value;
            }
        }

        /// <summary>
        /// Returns the raw size data for the given row or column.
        /// </summary>
        /// <param name="index">The row or column index.</param>
        /// <returns>The size of the row or column. It is -1 if the size 
        /// of the row or column is reset to default.</returns>
        public int/*float*/ GetSize(int index)
        {
            GridRowColSizeEventArgs e = new GridRowColSizeEventArgs(index, 0);
            OnQuerySize(e);
            if (e.Handled)
            {
                return e.Size;
            }

            return Dictionary[index];
        }

        /// <overload>
        /// Resizes a range of rows or columns to optimally fit contents of the
        /// specified range of cells.
        /// </overload>
        /// <summary>
        /// Resizes a range of rows or columns to optimally fit contents of the
        /// specified range of cells.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        public bool ResizeToFit(GridRangeInfo range)
        {
            return ResizeToFit(range, GridResizeToFitOptions.None);
        }
        /// <summary>
        /// Resize the range of rows to oprimally fit
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        public bool ResizeToFitOptimized(GridRangeInfo range)
        {
            return ResizeToFitOptimized(range, GridResizeToFitOptions.None,GridTextOptions.Text);
        }
        /// <summary>
        /// Resize the range of rows to oprimally fit
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="option">get the result for the purticular ResizeToFit Options</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        public bool ResizeToFitOptimized(GridRangeInfo range,GridResizeToFitOptions option)
        {
            return ResizeToFitOptimized(range, option, GridTextOptions.Text);
        }
        /// <summary>
        /// Resize the range of rows to oprimally fit
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="textOption">get the result for the purticular text option</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        public bool ResizeToFitOptimized(GridRangeInfo range, GridTextOptions textOption)
        {
            return ResizeToFitOptimized(range, GridResizeToFitOptions.None, textOption);
        }
        /// <summary>
        /// Resizes a range of rows or column to optimally fit contents of the
        /// specified range of cells and given options.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        public abstract bool ResizeToFit(GridRangeInfo range, GridResizeToFitOptions options);
        /// <summary>
        /// Resizes a range of rows or column to optimally fit
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <param name="textOption">text option</param>
        /// <returns></returns>
        public abstract bool ResizeToFitOptimized(GridRangeInfo range,GridResizeToFitOptions options, GridTextOptions textOption);
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        /// <returns>returns boolean value</returns>
        [Obsolete("Use GridResizeToFitOptions instead")]
        public bool ResizeToFit(GridRangeInfo range, bool resizeCovered, bool noShrink)
        {
            GridResizeToFitOptions rfo = GridResizeToFitOptions.None;
            if (resizeCovered)
            {
                rfo |= GridResizeToFitOptions.ResizeCoveredCells;
            }

            if (noShrink)
            {
                rfo |= GridResizeToFitOptions.NoShrinkSize;
            }

            return ResizeToFit(range, rfo);
        }
    }

    /// <summary>
    /// This command object holds all information to execute the SetRange 
    /// command. 
    /// </summary>
    /// <remarks>
    /// GridModelSetRowColSizeCommand is typically generated by the SetRange command
    /// and pushed onto the grid's command stack. 
    /// </remarks>
    [Syncfusion.Documentation.DocumentationExclude()]
    class GridModelSetRowColSizeCommand : GridModelCommand
    {
        private int from;
        private int to;
        private int/*float*/[] values;
        private GridModelRowColSizeIndexer rcsi;

        // Constructor.

        /// <summary>
        /// Initializes a new GridModelSetRowColSizeCommand object with all the commands.
        /// </summary>
        /// <param name="rcsi">A reference to the target GridModelRowColSizeIndexer.</param>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">The sizes to be applies to the range.</param>
        public GridModelSetRowColSizeCommand(GridModelRowColSizeIndexer rcsi, int from, int last, int/*float*/[] values)
            : base(rcsi.model)
        {
            SetDescription(SR.GetString("Command" + rcsi.RowColName, from, to));
            this.rcsi = rcsi;
            this.from = from;
            this.to = last;
            this.values = values;
        }

        /// <override/>
        public override void Execute()
        {
            rcsi.SetRange(from, to, values);
            Grid.ScrollCellInView(rcsi.RowColObject.CreateRangeFromTo(from, to), GridScrollCurrentCellReason.Command);
        }
    }

    [Serializable]
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class GridModelRowHeightsIndexer : GridModelRowColSizeIndexer, ISerializable
    {
        /// <summary>
        /// Initializes a new <see cref="GridModelRowHeightsIndexer"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridModelRowHeightsIndexer(SerializationInfo info, StreamingContext context)
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

        public GridModelRowHeightsIndexer(GridModel model)
            : base(model)
        {
        }

        public override GridModelRowColOperations RowColObject
        {
            get { return model.Rows; }
        }

        public override string RowColName
        {
            get { return "RowHeight"; }
        }

        /// <override/>
        public override void OnChanged(GridRowColSizeChangedEventArgs e)
        {
            Model.RaiseRowHeightsChanged(e);
            Model.Modified = true;
            Model.rowHeightEntries = null;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <override/>
        public override bool OnChanging(GridRowColSizeChangingEventArgs e)
        {
            Model.RaiseRowHeightsChanging(e);
            return !e.Cancel;
        }

        /// <override/>
        protected override void OnSaveSize(GridRowColSizeEventArgs e)
        {
            Model.RaiseSaveRowHeight(e);
        }

        /// <override/>
        protected override void OnQuerySize(GridRowColSizeEventArgs e)
        {
            Model.RaiseQueryRowHeight(e);
        }

        /// <override/>
        protected override void OnQuerySizeTotal(GridRowColSizeTotalEventArgs e)
        {
            Model.RaiseQueryRowHeightTotal(e);
        }

        /// <override/>
        public override int/*float*/ this[string name]
        {
            get { return this[Model.NameToRowIndex(name)]; }
            set { this[Model.NameToRowIndex(name)] = value; }
        }

        /// <summary>
        /// Resizes a range of rows or column to exactly fit contents of the
        /// specified range of cells.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <param name="textOption">text option</param>
        /// <returns>
        /// True if any changes were made.
        /// </returns>
        /// <override/>
        public override bool ResizeToFitOptimized(GridRangeInfo range, GridResizeToFitOptions options,GridTextOptions textOption)
        {
            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            bool isStyleText = (textOption & GridTextOptions.FormattedText) != 0;

            IGraphicsProvider graphicsProvider = model.GetGraphicsProvider();
            Graphics g = graphicsProvider.Graphics; // Do not dispose this object! IGraphicsProvider will dispose it.
            SizeF stringSize;
            GridStyleInfo style;
            GridRangeInfo coveredRange;
            range = range.ExpandRange(0, 0, Model.RowCount, Model.ColCount);
            string text = string.Empty;

            for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
            {
                long maxHeight = 0;
                for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                {
                    if (Model.ActiveGridView != null)
                        style = this.Model.ActiveGridView.GetViewStyleInfo(rowIndex, colIndex);
                    else
                        style = Model[rowIndex, colIndex];

                    if (isStyleText)
                        text = style.FormattedText;
                    else
                        text = style.Text;

                    if (model.GetType().Name != "GridTableModel")
                    {
                        bool isCovered = Model.GetSpannedRangeInfo(rowIndex, colIndex, out coveredRange);

                        #region Coverred Region
                        if (isCovered && resizeCoveredCells)
                        {
                            if (includeCellsWithinCoveredRange)
                            {
                                if (style.CellType != "ColumnHeaderCell" && style.CellType != "RowHeaderCell" && style.CellType != GridCellTypeName.Header)
                                {
                                    stringSize = g.MeasureString(text, style.GdipFont);
                                    if (text != string.Empty)
                                    {
                                        if (maxHeight < stringSize.Height)
                                        {
                                            maxHeight = (long)Math.Round(stringSize.Height);
                                        }
                                        model.RowHeights[rowIndex] = (int)maxHeight;
                                    }
                                }
                            }
                            else
                            {
                                GridCellModelBase cellModel = style.CellModel;
                                Size size = cellModel.CalculatePreferredCellSize(g, rowIndex, colIndex, style, GridQueryBounds.Height);
                                model.RowHeights[rowIndex] = size.Height;
                                
                            }
                        }
                        #endregion
                        #region other options
                        else if (!includeHeaders)
                        {

                            if (style.CellType != "ColumnHeaderCell" && style.CellType != "RowHeaderCell" && style.CellType != GridCellTypeName.Header)
                            {
                                stringSize = g.MeasureString(text, style.GdipFont, Model.ColWidths[colIndex]);
                                if (text != string.Empty)
                                {
                                    if (maxHeight < stringSize.Height)
                                    {
                                        maxHeight = (long)Math.Round(stringSize.Height);
                                    }
                                    model.RowHeights[rowIndex] = (int)maxHeight + 3;
                                }
                            }
                            else
                            {
                                GridCellModelBase cellModel = style.CellModel;
                                Size size = cellModel.CalculatePreferredCellSize(g, rowIndex, colIndex, style, GridQueryBounds.Height);
                                model.RowHeights[rowIndex] = size.Height;
                            }
                        }
                        #endregion
                        #region IncludeHeaders
                        else
                        {
                            stringSize = g.MeasureString(text, style.GdipFont, Model.ColWidths[colIndex]);
                            if (text != string.Empty)
                            {
                                if (maxHeight < stringSize.Height)
                                {
                                    maxHeight = (long)Math.Round(stringSize.Height);
                                }
                                model.RowHeights[rowIndex] = (int)maxHeight + 3;
                            }
                        }
                        #endregion
                    }

                    #region GridGroupingControl
                    else
                    {
                        #region Covered cells
                        bool isCovered = Model.GetSpannedRangeInfo(rowIndex, colIndex, out coveredRange);
                        if (isCovered && resizeCoveredCells)
                        {
                            if (includeCellsWithinCoveredRange)
                            {
                                stringSize = g.MeasureString(text, style.GdipFont, Model.ColWidths[colIndex]);
                                if (maxHeight < stringSize.Height)
                                {
                                    maxHeight = (long)Math.Round(stringSize.Height);
                                }
                                if (Model.RowHeights[rowIndex] <= maxHeight)
                                {
                                    model.RowHeights[rowIndex] = (int)maxHeight + 3;
                                    maxHeight = 0;
                                }
                            }

                        }
                        #endregion
                        #region Other options
                        else if (!includeHeaders)
                         {
                             if (style.CellType != "ColumnHeaderCell" && style.CellType != "RowHeaderCell" && style.CellType != GridCellTypeName.Header)
                             {
                                 if (text != string.Empty)
                                 {
                                     int numofOccurrence = 0;
                                     // To find the Occurence of tab in the string
                                     if (text.Contains("\t"))
                                         numofOccurrence = Regex.Matches(text, "\t").Count;

                                     // If a string has both tab and newline in adjacent, then the tab is not needed, since the next string will be in newline.
                                     if (text.Contains("\t\r\n"))
                                     {
                                         numofOccurrence += numofOccurrence;
                                         style.Text = text = text.Replace("\t\r\n", "\r\n");
                                     }
                                     else if (text.Contains("\t\n"))
                                     {
                                         numofOccurrence += numofOccurrence;
                                         style.Text = text = text.Replace("\t\n", "\n");
                                     }
                                     stringSize = g.MeasureString(text, style.GdipFont, Model.ColWidths[colIndex]);
                                     if (maxHeight < stringSize.Height)
                                     {
                                         maxHeight = (long)Math.Round(stringSize.Height);
                                     }
                                     if (Model.RowHeights[rowIndex] <= maxHeight)
                                     {
                                         model.RowHeights[rowIndex] = (int)maxHeight + numofOccurrence;
                                         maxHeight = 0;
                                     }
                                 }
                             }
                             else
                             {
                                 GridCellModelBase cellModel = style.CellModel;
                                 Size size = cellModel.CalculatePreferredCellSize(g, rowIndex, colIndex, style, GridQueryBounds.Height);
                                 model.RowHeights[rowIndex] = size.Height;
                             }
                         }
                        #endregion
                        #region Include Headers
                        else
                        {
                            stringSize = g.MeasureString(text, style.GdipFont, Model.ColWidths[colIndex]);
                            if (text != string.Empty)
                            {
                                if (maxHeight < stringSize.Height)
                                {
                                    maxHeight = (long)Math.Round(stringSize.Height);
                                }
                                if (Model.RowHeights[rowIndex] <= maxHeight)
                                {
                                    model.RowHeights[rowIndex] = (int)maxHeight + 3;
                                    maxHeight = 0;
                                }
                            }
                        }
                        #endregion

                    }
                    #endregion
                }
                
            }
            return true;
        }

        public override bool ResizeToFit(GridRangeInfo range, GridResizeToFitOptions options)
        {
            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            IGraphicsProvider graphicsProvider = model.GetGraphicsProvider();
            Graphics g = graphicsProvider.Graphics; // Do not dispose this object! IGraphicsProvider will dispose it.
            range = range.ExpandRange(0, 0, Model.RowCount, Model.ColCount);
            Model.FloatingCells.EvaluateFloatingCells(range);

            using (OperationFeedback op = new OperationFeedback(Model))
            {
                op.Description = SR.GetString("GRID_IDM_RESIZEROWS");

                int/*float*/ maxHeight = RowColObject.MaxSize;

                try
                {
                    ////bool abort = false;
                    GridRangeInfo coveredRange;
                    int/*float*/[] newHeights = this.GetRange(range.Top, range.Bottom);
                    this.EvalRange(range.Top, newHeights);
                    int/*float*/[] oldHeights = (int/*float*/[])newHeights.Clone();
                    for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                    {
                        int/*float*/ height;
                        height = -1;
                        bool doHeader = includeHeaders && range.Left > 0;

                        for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                        {
                            if (doHeader)
                            {
                                colIndex = 0;
                            }

                            bool isCovered = Model.GetSpannedRangeInfo(rowIndex, colIndex, out coveredRange);

                            // Skip invisible columns
                            bool canResize = false;

                            // Covered cells
                            if (isCovered && !includeCellsWithinCoveredRange)
                            {
                                if (resizeCoveredCells || coveredRange.Height == 1)
                                {
                                    canResize =
                                        colIndex == coveredRange.Left // must be the first covered column
                                        && rowIndex == coveredRange.Bottom // and the last covered row
                                        // all cells of covered cell must be with in range to be resized
                                        && range.Top <= coveredRange.Top && range.Right >= coveredRange.Right;
                                }
                            }
                            else
                            {  
                                // skip invisible columns
                                canResize = this.Model.ColWidths[colIndex] > 0;
                            }

                            if (canResize)
                            {
                                GridStyleInfo styleInfo = null;

                                Size size;

                                if (isCovered)
                                {
                                    styleInfo = Model[coveredRange.Top, coveredRange.Left];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    ////cellModel.LoadStyle(coveredRange.Top, coveredRange.Left, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(g, coveredRange.Top, coveredRange.Left, styleInfo, GridQueryBounds.Height);

                                    //// Subtract row heights of previous rows (only the last row can be resized)
                                    if (rowIndex > coveredRange.Top)
                                    {
                                        size.Height -= (int)GetTotal(coveredRange.Top, rowIndex - 1);
                                    }
                                }
                                else
                                {
                                    styleInfo = Model[rowIndex, colIndex];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    ////cellModel.LoadStyle(rowIndex, colIndex, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(g, rowIndex, colIndex, styleInfo, GridQueryBounds.Height);
                                }

                                height = Math.Max(height, (int/*float*/)size.Height);
                            }

                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }

                            if (doHeader)
                            {
                                doHeader = false;
                                colIndex = range.Left - 1;
                            }

                            if (isCovered && canResize)
                            {
                                if (coveredRange.Right <= range.Right)
                                {
                                    colIndex = coveredRange.Right;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        if (height != -1)
                        {
                            height = Math.Min(maxHeight, height);

                            if (noShrinkSize)
                            {
                                if (height > newHeights[rowIndex - range.Top])
                                {
                                    newHeights[rowIndex - range.Top] = height;
                                }
                            }
                            else
                            {
                                if (height != newHeights[rowIndex - range.Top] && height > 0)
                                {
                                    newHeights[rowIndex - range.Top] = height;
                                }
                            }
                        }

                        op.PercentComplete = (rowIndex - range.Top) * 100 / range.Height;
                    }

                    bool equal = true;
                    for (int n = 0; n < newHeights.Length; n++)
                    {
                        equal &= oldHeights[n] == newHeights[n];
                    }

                    if (!equal)
                    {
                        SetRange(range.Top, range.Bottom, newHeights);
                    }
                }
                catch (GridUserCanceledException ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    return false;
                }
                finally
                {
#if DEBUG
                    Trace.WriteLineIf(Switches.ResizeToFit.TraceVerbose, "Exit ResizeToFit(" + range.ToString() + ")");
#endif
                }

                return true;
            }
        }
    }

    [Serializable]
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class GridModelColWidthsIndexer : GridModelRowColSizeIndexer
    {
        /// <summary>
        /// Initializes a new <see cref="GridModelColWidthsIndexer"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridModelColWidthsIndexer(SerializationInfo info, StreamingContext context)
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

        public GridModelColWidthsIndexer(GridModel model)
            : base(model)
        {
        }

        /// <override/>
        public override GridModelRowColOperations RowColObject
        {
            get { return model.Cols; }
        }

        /// <override/>
        public override string RowColName
        {
            get { return "ColumnWidth"; }
        }

        /// <override/>
        public override void OnChanged(GridRowColSizeChangedEventArgs e)
        {
            Model.RaiseColWidthsChanged(e);
            Model.Modified = true;
            Model.colWidthEntries = null;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <override/>
        public override bool OnChanging(GridRowColSizeChangingEventArgs e)
        {
            Model.RaiseColWidthsChanging(e);
            return !e.Cancel;
        }

        /// <override/>
        protected override void OnSaveSize(GridRowColSizeEventArgs e)
        {
            Model.RaiseSaveColWidth(e);
        }

        /// <override/>
        protected override void OnQuerySize(GridRowColSizeEventArgs e)
        {
            Model.RaiseQueryColWidth(e);
        }

        /// <override/>
        protected override void OnQuerySizeTotal(GridRowColSizeTotalEventArgs e)
        {
        }

        /// <override/>
        public override int/*float*/ this[string name]
        {
            get { return this[Model.NameToColIndex(name)]; }
            set { this[Model.NameToColIndex(name)] = value; }
        }

        /// <summary>
        /// Resizes a range of rows or column to exactly fit contents of the
        /// specified range of cells.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <param name="textOption">text option</param>
        /// <returns>
        /// True if any changes were made.
        /// </returns>
        /// <override/>
        public override bool ResizeToFitOptimized(GridRangeInfo range, GridResizeToFitOptions options,GridTextOptions textOption)
        {
            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            bool isStyleText = (textOption & GridTextOptions.FormattedText) != 0;

            IGraphicsProvider graphicsProvider = model.GetGraphicsProvider();
            Graphics g = graphicsProvider.Graphics; // Do not dispose this object! IGraphicsProvider will dispose it.
            long maxWidth=0;
            SizeF stringSize;
            GridCurrentCellInfo info = model.CurrentCellInfo;
            GridStyleInfo style;
            range = range.ExpandRange(0, 0, Model.RowCount, Model.ColCount);
            bool isHeader = false,isEmpty = false;
            Size size = Size.Empty;
            string text = string.Empty;

            for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
            {
                for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                {
                    if (Model.ActiveGridView != null)
                        style = this.Model.ActiveGridView.GetViewStyleInfo(rowIndex, colIndex);
                    else
                        style = Model[rowIndex, colIndex];

                    if (isStyleText)
                        text = style.FormattedText;
                    else
                        text = style.Text;

                        if (model.GetType().Name != "GridTableModel")
                        {
                            if (!includeHeaders)
                            {
                                if (style.CellType != "ColumnHeaderCell" && style.CellType != "RowHeaderCell" && style.CellType != GridCellTypeName.Header)
                                {
                                    if (text != string.Empty)
                                    {
                                        isHeader = false;
                                        isEmpty = false;
                                        stringSize = g.MeasureString(text, style.GdipFont);

                                        if (maxWidth <= stringSize.Width || maxWidth == 0)
                                        {
                                            maxWidth = (long)stringSize.Width;
                                        }
                                    }
                                    else
                                    {
                                        isEmpty = true;
                                        GridCellModelBase cellModel = style.CellModel;
                                        size = cellModel.CalculatePreferredCellSize(g, rowIndex, colIndex, style, GridQueryBounds.Height);
                                    }
                                }
                                else
                                {
                                    if (text == string.Empty)
                                    {
                                        GridCellModelBase cellModel = style.CellModel;
                                        size = cellModel.CalculatePreferredCellSize(g, rowIndex, colIndex, style, GridQueryBounds.Height);
                                    }
                                    isHeader = true;
                                }
                            }
                            else
                            {
                                stringSize = g.MeasureString(text, style.GdipFont);

                                if (maxWidth <= stringSize.Width || maxWidth == 0)
                                {
                                    maxWidth = (long)stringSize.Width;
                                }
                            }
                        }
                        else
                        {
                            stringSize = g.MeasureString(text, style.GdipFont);

                            if (maxWidth <= stringSize.Width || maxWidth == 0)
                            {
                                maxWidth = (long)stringSize.Width;
                                
                            }
                            if (Model.ColWidths[colIndex] <= maxWidth)
                            {
                                Model.ColWidths[colIndex] = (int)maxWidth + 5;
                                maxWidth = 0;
                            }
                        }
                   
                }
                if (model.GetType().Name != "GridTableModel")
                {
                        if (!isHeader && !isEmpty)
                        {
                            Model.ColWidths[colIndex] = (int)maxWidth + 5;
                            maxWidth = 0;
                        }
                        else if (!isHeader && isEmpty)
                        {
                            Model.ColWidths[colIndex] = maxWidth != 0 ? (int)maxWidth + 5 : size.Width;
                            maxWidth = 0;
                        }
                        else
                        {
                            Model.ColWidths[colIndex] = size.Width;
                            isHeader = false;
                            isEmpty = false;
                        }
                }
            }
            return true;
        }
        /// <summary>
        /// Resizes a range of rows or column to optimally fit contents of the
        /// specified range of cells and given options.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <returns>
        /// True if any changes were made; False if all sizes were already optimal.
        /// </returns>
        /// <override/>
        public override bool ResizeToFit(GridRangeInfo range, GridResizeToFitOptions options)
        {
            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            IGraphicsProvider graphicsProvider = model.GetGraphicsProvider();
            Graphics g = graphicsProvider.Graphics; // Do not dispose this object! IGraphicsProvider will dispose it.
            range = range.ExpandRange(0, 0, Model.RowCount, Model.ColCount);
            Model.FloatingCells.EvaluateFloatingCells(range);

            using (OperationFeedback op = new OperationFeedback(Model))
            {
                op.Description = SR.GetString("GRID_IDM_RESIZECOLS");

                int/*float*/ maxWidth = RowColObject.MaxSize;

                try
                {
                    ////bool bAbort = false;
                    GridRangeInfo coveredRange;
                    int/*float*/[] newWidths = this.GetRange(range.Left, range.Right);
                    this.EvalRange(range.Left, newWidths);
                    int/*float*/[] oldWidths = (int/*float*/[])newWidths.Clone();
                    if (!noShrinkSize)
                    {
                        for (int n = 0; n < newWidths.Length; n++)
                        {
                            newWidths[n] = -1;
                        }
                    }

                    bool doHeader = includeHeaders && range.Top > 0;

                    for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                    {
                        if (doHeader)                        
                        {   
                            rowIndex = 0;
                        }

                        int/*float*/ width = 0;
                        for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                        {
                            bool isCovered = Model.GetSpannedRangeInfo(rowIndex, colIndex, out coveredRange);

                            // Skip invisible rows.
                            bool canResize = false;

                            // Covered cells.
                            if (isCovered && !includeCellsWithinCoveredRange)
                            {
                                if (resizeCoveredCells || coveredRange.Width == 1)
                                {
                                    canResize =
                                        rowIndex == coveredRange.Top // Must be the first covered row.
                                        && colIndex == coveredRange.Right // And the last covered col.
                                        // All cells of covered cell must be with in range to be resized.
                                        && range.Left <= coveredRange.Left && range.Bottom >= coveredRange.Bottom;
                                }
                            }
                            else
                            {
                                // Skip invisible rows.
                                canResize = this.Model.RowHeights[rowIndex] > 0;
                            }

                            if (canResize)
                            {
                                GridStyleInfo styleInfo = null;

                                Size size;

                                if (isCovered)
                                {
                                    styleInfo = Model[coveredRange.Top, coveredRange.Left];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    ////cellModel.LoadStyle(coveredRange.Left, coveredRange.Top, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(g, coveredRange.Top, coveredRange.Left, styleInfo, GridQueryBounds.Width);

                                    //// Subtract col heights of previous cols (only the last col can be resized).
                                    if (colIndex > coveredRange.Left)
                                    {
                                        size.Width -= (int)GetTotal(coveredRange.Left, colIndex - 1);
                                    }
                                }
                                else
                                {
                                    styleInfo = Model[rowIndex, colIndex];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    ////cellModel.LoadStyle(colIndex, rowIndex, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(g, rowIndex, colIndex, styleInfo, GridQueryBounds.Width);
                                }

                                width = size.Width + 2;
                            }
                            else if (model.GetType().Name != "GridTableModel" || !includeHeaders || (model.GetType().Name == "GridTableModel" && rowIndex > 1))
                            {
                                width = this.Model.ColWidths[colIndex];
                            }

                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }

                            if (isCovered && canResize)
                            {
                                if (coveredRange.Right <= range.Right)
                                {
                                    colIndex = coveredRange.Right;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (width > newWidths[colIndex - range.Left])
                            {
                                newWidths[colIndex - range.Left] = width;
                            }
                        }

                        op.PercentComplete = (rowIndex - range.Top) * 100 / range.Height;

                        width = Math.Min(maxWidth, width);

                        if (doHeader)
                        {
                            doHeader = false;
                            rowIndex = range.Top - 1;
                        }
                    }

                    bool equal = true;
                    for (int n = 0; n < newWidths.Length; n++)
                    {
                        equal &= newWidths[n] == oldWidths[n];
                    }

                    if (!equal)
                    {
                        SetRange(range.Left, range.Right, newWidths);
                    }
                }
                catch (GridUserCanceledException ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    return false;
                }
                finally
                {
#if DEBUG
                    Trace.WriteLineIf(Switches.ResizeToFit.TraceVerbose, "Exit ResizeToFit(" + range.ToString() + ")");
#endif
                }

                return true;
            }
        }
    }
}
