//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellInfo.cs" company="syncfusion">
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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using Syncfusion.Collections;
using Syncfusion.Styles;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    #region GridCellInfo
    /// <summary>
    ///     Defines a specific cell in the grid.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(GridCellInfoTypeConverter))]
    public class GridCellInfo : IDisposable, ISerializable
    {
        int row;
        int col;
        GridStyleInfo cellStyle;
        internal GridCellInfoCollection collection;

        /// <overload>
        ///   <para>Initializes a <see cref="GridCellInfo"/> object.</para>
        /// </overload>
        /// <summary>
        ///   <para>Initializes an empty <see cref="GridCellInfo"/> object.</para>
        /// </summary>
        /// <remarks>
        ///   <para>This constructor initializes a new <see cref="GridCellInfo"/> object with default values.</para>
        /// </remarks>
        public GridCellInfo()
        {
            row = int.MinValue;
            col = int.MinValue;
            this.cellStyle = new GridBaseStyleInfo(null, new GridStyleInfoStore());
            cellStyle.Changed += new StyleChangedEventHandler(style_Changed);
        }

        /// <overload>
        ///   <para>Initializes a <see cref="GridCellInfo"/> object.</para>
        /// </overload>
        /// <summary>
        ///   <para>Initializes an empty <see cref="GridCellInfo"/> object.</para>
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <remarks>
        ///   <para>This constructor initializes a new <see cref="GridCellInfo"/> object representing a specific cell with default <see cref="GridStyleInfo"/>.</para>
        /// </remarks>
        public GridCellInfo(int row, int col)
        {
            this.row = row;
            this.col = col;
            this.cellStyle = new GridBaseStyleInfo(null, new GridStyleInfoStore());
            cellStyle.Changed += new StyleChangedEventHandler(style_Changed);
        }

        /// <overload>
        ///   <para>Initializes a <see cref="GridCellInfo"/> object.</para>
        /// </overload>
        /// <summary>
        ///   <para>Initializes an empty <see cref="GridCellInfo"/> object.</para>
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <param name="styleInfoStore">The style information for the cell.</param>
        /// <remarks>
        ///   <para>This constructor initializes a new <see cref="GridCellInfo"/> object with a <see cref="GridStyleInfo"/> defined by the supplied <see cref="GridStyleInfoStore"/>.</para>
        /// </remarks>
        public GridCellInfo(int row, int col, GridStyleInfoStore styleInfoStore)
        {
            this.row = row;
            this.col = col;
            this.cellStyle = new GridBaseStyleInfo(null, styleInfoStore);
            cellStyle.Changed += new StyleChangedEventHandler(style_Changed);
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellInfo"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridCellInfo(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            row = (int)info.GetValue("Row", typeof(int));
            col = (int)info.GetValue("Col", typeof(int));
            cellStyle = (GridStyleInfo)info.GetValue("Style", typeof(GridStyleInfo));
        }
        
        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridCellInfo"/>.
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
            GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the data needed to serialize the <see cref="GridCellInfo"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Row", row);
            info.AddValue("Col", col);
            info.AddValue("Style", cellStyle);
        }

        /// <summary>
        /// Gets or sets the Row in the grid that the cell belongs to     
        /// </summary>
        [XmlAttribute("Row")]
        public int Row
        {
            get
            {
                return row;
            }

            set
            {
                row = value;
            }
        }

        /// <summary>
        ///  Gets or sets the Column in the grid that the cell belongs to     
        /// </summary>
        [XmlAttribute("Col")]
        public int Col
        {
            get
            {
                return col;
            }

            set
            {
                col = value;
            }
        }

        /// <copyfrom cref="GridStyleInfo"/>
        /// <summary>
        /// Gets or sets the style object that contains the cell information.
        /// </summary>
        [XmlElement("CellContents")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        public GridStyleInfo StyleInfo
        {
            get
            {
                if (cellStyle == null)
                {
                    cellStyle = new GridBaseStyleInfo(null, new GridStyleInfoStore());
                    cellStyle.Changed += new StyleChangedEventHandler(style_Changed);
                }

                return cellStyle;
            }

            set
            {
                StyleInfo.ModifyStyle(value, StyleModifyType.Copy);
                ////cellStyle = value;
            }
        }

        #region IDisposable Members

        /// <override/>
        /// <summary>
        /// Releases all resources used by the.
        /// <see cref="T:Syncfusion.Windows.Forms.Grid.GridCellInfo">GridCellInfo</see>.
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            if (cellStyle != null)
            {
                cellStyle.Changed -= new StyleChangedEventHandler(style_Changed);
                this.cellStyle.Dispose();
            }

            this.cellStyle = null;
        }

        #endregion

        private void style_Changed(object sender, StyleChangedEventArgs e)
        {
            if (collection != null)
            {
                collection.WriteToData(this, true);
            }
        }
    }
    #endregion

    #region "'GridCellInfoCollection' strongly typed collection class"

    /// <summary>For internal use.</summary>
    /// <exclude/>
    public class GridCellInfoCollectionEditor : CollectionEditor
    {
        private PropertyGridContextMenu pgMenu;

        /// <summary>
        /// Initializes a new <see cref="GridRangeStyleCollectionEditor"/> object.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public GridCellInfoCollectionEditor(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.CollectionEditor.CollectionForm"/> to provide as the user interface for editing the collection.
        /// </returns>
        /// <override/>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();

            PropertyGrid pg = WinFormsUtils.GetPropertyGridInControl(collectionForm);

            if (pg != null)
            {
                this.pgMenu = new PropertyGridContextMenu(pg);
            }

            return collectionForm;
        }
    }

    /// <summary>
    ///     A collection that stores 'GridCellInfo' objects.
    /// </summary>
    [Serializable()]
    [EditorAttribute(typeof(GridCellInfoCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class GridCellInfoCollection : System.Collections.CollectionBase, ISerializable
    {
        internal SFTable data;
        internal GridControlBase control;
        private bool createdFromTemplate = false;

        internal SFTable InternalData
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        ///     Initializes a new instance of 'GridCellInfoCollection'.
        /// </summary>
        public GridCellInfoCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of 'GridCellInfoCollection' based on an already existing instance.
        /// </summary>
        /// <param name='griValue'>
        ///     A 'GridCellInfoCollection' from which the contents is copied
        /// </param>
        public GridCellInfoCollection(GridCellInfoCollection griValue)
        {
            this.AddRange(griValue);
        }

        /// <summary>
        ///     <para>Initializes a new instance of the GridCellInfoCollection class based on an existing <see cref="GridData"/> object.</para>
        /// </summary>
        /// <param name="gdata" type="Syncfusion.Windows.Forms.Grid.GridData">
        ///     <para>
        ///         The Data to construct the new collection.
        ///     </para>
        /// </param>
        public GridCellInfoCollection(GridData gdata)
            : this(gdata.sfTable)
        {
        }

        /// <summary>
        ///     <para>Initializes a new instance of the GridCellInfoCollection class based on an existing Data object.</para>
        /// </summary>
        /// <param name="newdata" type="Syncfusion.Collections.SFTable">
        ///     <para>
        ///         The Data to construct the new collection.
        ///     </para>
        /// </param>
        public GridCellInfoCollection(SFTable newdata)
            : this(newdata, false)
        {
        }

        /// <summary>
        ///     <para>Initializes a new instance of the GridCellInfoCollection class based on an existing Data object.</para>
        /// </summary>
        internal GridCellInfoCollection(SFTable newdata, bool fromTemplate)
        {
            if (newdata != null)
            {
                this.createdFromTemplate = fromTemplate;

                this.data = newdata;
                int count = newdata.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    ArrayList cols = (ArrayList)newdata.Rows[i];
                    if (cols != null)
                    {
                        int cellColCount = cols.Count;
                        for (int j = 0; j < cellColCount; j++)
                        {
                            if (cols[j] != null)
                            {
                                GridStyleInfoStore cellInfo = cols[j] as GridStyleInfoStore;
                                if (cellInfo != null)
                                {
                                    GridCellInfo info = new GridCellInfo(i - 1, j - 1, (GridStyleInfoStore)cellInfo.Clone()); ////new GridStyleInfo(null,cellInfo)); ////clone);
                                    info.collection = this;
                                    this.InnerList.Add(info);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of 'GridCellInfoCollection' with an array of 'GridCellInfo' objects.
        /// </summary>
        /// <param name='griValue'>
        ///     An array of 'GridCellInfo' objects with which to initialize the collection
        /// </param>
        public GridCellInfoCollection(GridCellInfo[] griValue)
        {
            this.AddRange(griValue);
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellInfoCollection"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridCellInfoCollection(SerializationInfo info, StreamingContext context)
            : this((SFTable)info.GetValue("Data", typeof(SFTable)), true)
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

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridCellInfoCollection"/>.
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
            GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the data needed to serialize the <see cref="GridCellInfoCollection"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", this.data);
        }

        /// <summary>
        ///     Represents the 'GridCellInfo' item at the specified index position.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index of the entry to locate in the collection.
        /// </param>
        /// <value>
        ///     The entry at the specified index of the collection.
        /// </value>
        public GridCellInfo this[int intIndex]
        {
            get
            {
                return (GridCellInfo)List[intIndex];
            }

            set
            {
                List[intIndex] = value;
            }
        }

        /// <summary>
        ///     Adds a 'GridCellInfo' item with the specified value to the 'GridCellInfoCollection'
        /// </summary>
        /// <param name='griValue'>
        ///     The 'GridCellInfo' to add.
        /// </param>
        /// <returns>
        ///     The index at which the new element was inserted.
        /// </returns>
        public int Add(GridCellInfo griValue)
        {
            int index = List.Add(griValue);
            WriteToData(griValue, true);
            return index;
        }

        /// <summary>
        ///     Copies the elements of an array at the end of this instance of 'GridCellInfoCollection'.
        /// </summary>
        /// <param name='griValue'>
        ///     An array of 'GridCellInfo' objects to add to the collection.
        /// </param>
        public void AddRange(GridCellInfo[] griValue)
        {
            for (int intCounter = 0; intCounter < griValue.Length; intCounter = intCounter + 1)
            {
                ////griValue[intCounter].collection = this;
                this.Add(griValue[intCounter]);
            }
        }

        /// <summary>
        ///     Adds the contents of another 'GridCellInfoCollection' at the end of this instance.
        /// </summary>
        /// <param name='griValue'>
        ///     A 'GridCellInfoCollection' containing the objects to add to the collection.
        /// </param>
        public void AddRange(GridCellInfoCollection griValue)
        {
            if (!griValue.createdFromTemplate)
            {
                for (int intCounter = 0; intCounter < griValue.Count; intCounter = intCounter + 1)
                {
                    ////griValue[intCounter].collection = this;
                    if (griValue[intCounter].StyleInfo.CellType == GridCellTypeName.Image)
                    {
                        if (griValue[intCounter].StyleInfo.CellValue is System.Drawing.Bitmap)
                        {
                            byte[] bytes = Syncfusion.Windows.Forms.Grid.Design.GridSyncProperties.GetImageBytes(griValue[intCounter].StyleInfo.CellValue as System.Drawing.Bitmap);
                            string base64 = Convert.ToBase64String(bytes);
                            griValue[intCounter].StyleInfo.CellValue = base64;
                        }
                        else if (griValue[intCounter].StyleInfo.CellValue is string)
                        {
                            byte[] bytes = Convert.FromBase64String(griValue[intCounter].StyleInfo.CellValue as string);
                            System.Drawing.Bitmap bmp = Syncfusion.Windows.Forms.Grid.Design.GridSyncProperties.GetImageFromBytes(bytes);
                            griValue[intCounter].StyleInfo.CellValue = bmp;
                        }
                    }
                    this.Add(griValue[intCounter]);
                }
            }
            else
            {
                ////use the data object directly
                for (int row = 0; row <= griValue.data.RowCount; row++)
                {
                    for (int col = 0; col <= griValue.data.ColCount; col++)
                    {
                        GridStyleInfoStore dataStore = griValue.data[row, col] as GridStyleInfoStore;
                        if (dataStore != null)
                        {
                            GridCellInfo info = new GridCellInfo(row - 1, col - 1, (GridStyleInfoStore)dataStore.Clone()); ////new GridStyleInfo(null,cellInfo));//clone);
                            info.collection = this;
                            this.InnerList.Add(info);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the 'GridCellInfoCollection' contains the specified value.
        /// </summary>
        /// <param name='griValue'>
        ///     The item to locate.
        /// </param>
        /// <returns>
        ///     True if the item exists in the collection; false otherwise.
        /// </returns>
        public bool Contains(GridCellInfo griValue)
        {
            return List.Contains(griValue);
        }

        /// <summary>
        ///     Copies the 'GridCellInfoCollection' values to a one-dimensional System.Array
        ///     instance starting at the specified array index.
        /// </summary>
        /// <param name='griArray'>
        ///     The one-dimensional System.Array that represents the copy destination.
        /// </param>
        /// <param name='intIndex'>
        ///     The index in the array where copying begins.
        /// </param>
        public void CopyTo(GridCellInfo[] griArray, int intIndex)
        {
            List.CopyTo(griArray, intIndex);
        }

        /// <summary>
        ///     Returns the index of a 'GridCellInfo' object in the collection.
        /// </summary>
        /// <param name='griValue'>
        ///     The 'GridCellInfo' object whose index will be retrieved.
        /// </param>
        /// <returns>
        ///     If found, the index of the value; otherwise, -1.
        /// </returns>
        public int IndexOf(GridCellInfo griValue)
        {
            return List.IndexOf(griValue);
        }

        /// <summary>
        ///     Inserts an existing 'GridCellInfo' into the collection at the specified index.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index where the new item should be inserted.
        /// </param>
        /// <param name='griValue'>
        ///     The item to insert.
        /// </param>
        public void Insert(int intIndex, GridCellInfo griValue)
        {
            List.Insert(intIndex, griValue);
        }

        /// <summary>
        ///     Returns an enumerator that can be used to iterate through
        ///     the 'GridCellInfoCollection'.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public new GridCellInfoEnumerator GetEnumerator()
        {
            return new GridCellInfoEnumerator(this);
        }

        /// <summary>
        ///     Removes a specific item from the 'GridCellInfoCollection'.
        /// </summary>
        /// <param name='griValue'>
        ///     The item to remove from the 'GridCellInfoCollection'.
        /// </param>
        public void Remove(GridCellInfo griValue)
        {
            List.Remove(griValue);
        }

        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before setting an item in the collection
        /// </summary>
        protected override void OnSet(int intIndex, object objOldValue, object objNewValue)
        {
            ////  TODO: Add code here to handle an existing value within
            ////  the collection be replaced with a new value
        }

        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before inserting a new item in the collection
        /// </summary>
        protected override void OnInsert(int intIndex, object objValue)
        {
            ////  TODO: Add code here to handle inserting a new item into the collection
        }

        internal void WriteToData(GridCellInfoCollection infoCollection)
        {
            if (infoCollection == null)
            {
                return;
            }

            foreach (GridCellInfo cellInfo in infoCollection)
            {
                WriteToData(cellInfo, false);
            }
        }

        internal void WriteToData(GridCellInfo[] cellInfos, bool invalidate)
        {
            foreach (GridCellInfo cellInfo in cellInfos)
            {
                WriteToData(cellInfo, false);
            }
        }

        internal void WriteToData(GridCellInfo cellInfo, bool invalidate)
        {
            if (data == null)
            {
                return;
            }

            data.RowCount = Math.Max(cellInfo.Row + 2, data.RowCount);
            data.ColCount = Math.Max(cellInfo.Col + 2, data.ColCount);
            data[cellInfo.Row + 1, cellInfo.Col + 1] = cellInfo.StyleInfo.Store;
            if (control != null)
            {
                control.Model.ResetVolatileData();
                control.Invalidate();
            }
        }

        /// <summary>
        ///     A strongly typed enumerator for 'GridCellInfoCollection'
        /// </summary>
        public class GridCellInfoEnumerator : object, System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator iEnBase;

            private System.Collections.IEnumerable iEnLocal;

            /// <summary>
            ///     Enumerator constructor
            /// </summary>
            /// <param name="griMappings">A collection of GridCellInfo objects.</param>
            public GridCellInfoEnumerator(GridCellInfoCollection griMappings)
            {
                this.iEnLocal = (System.Collections.IEnumerable)griMappings;
                this.iEnBase = iEnLocal.GetEnumerator();
            }

            /// <summary>
            ///     Gets the current element from the collection (strongly typed)
            /// </summary>
            public GridCellInfo Current
            {
                get
                {
                    return (GridCellInfo)iEnBase.Current;
                }
            }

            /// <summary>
            ///     Gets the current element from the collection
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return iEnBase.Current;
                }
            }

            /// <summary>
            ///     Advances the enumerator to the next element of the collection
            /// </summary>
            /// <returns>True if next element exists; False otherwise.</returns>
            public bool MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            bool System.Collections.IEnumerator.MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            public void Reset()
            {
                iEnBase.Reset();
            }

            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            void System.Collections.IEnumerator.Reset()
            {
                iEnBase.Reset();
            }
        }
    }

    #endregion //('GridCellInfoCollection' strongly typed collection class)

    #region GridCellInfoTypeConverter

    /// <summary>
    /// The type converter for <see cref="GridCellInfo"/> objects. <see cref="GridCellInfoTypeConverter"/> 
    /// is a <see cref="ExpandableObjectConverter"/>. It overrides the default behavior of the 
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class GridCellInfoTypeConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridCellInfoTypeConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type,
        /// using the specified context.
        /// </summary>
        /// <param name="context">Format
        /// context. </param>
        /// <param name="destinationType">The type you want to convert to. </param>       
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <override/>
        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">Format
        /// context. </param>
        /// <param name="culture">Current culture information.</param>
        /// <returns>
        /// <param name="value">The object to convert. </param>
        /// <param name="destinationType">The type to convert the
        /// value parameter to. </param>        
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                GridCellInfo cell = (GridCellInfo)value;
                Type type = typeof(GridCellInfo);

                ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    return new InstanceDescriptor(constructorInfo, null, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <override/>
        /// <summary>
        /// A collection of properties for a specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">The type of array for which to get properties.</param>
        /// <param name="attributes">A list of System.Atribute objects that will be used as a filter.</param>
        /// <returns>A collection of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(value, attributes, false);

            string[] atts = new string[]
            {
                "Row",
                "Col",
                "StyleInfo",
            };

            return pds.Sort(atts);
        }
    }
    #endregion
}
