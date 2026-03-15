//-------------------------------------------------------------------------------------------------
// <copyright file="GridPropertyTypeDefaultStyle.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

#if ASPNET
using Syncfusion.Windows.Forms.Grid;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region GridPropertyTypeDefaultStyle

    /// <summary>
    /// An entry for the <see cref="GridPropertyTypeDefaultStyleCollection"/>
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GridPropertyTypeDefaultStyleTypeConverter), typeof(System.Drawing.Design.UITypeEditor))]
    public class GridPropertyTypeDefaultStyle : DescriptorBase, IStandardValuesProvider, ICloneable
    {
        #region Fields

        string name;
        GridTableCellStyleInfo style;
        bool allowDropDown = true;
        #endregion

        #region CtorWithName

        /// <overload>
        /// Initializes a new object.
        /// </overload>
        /// <summary>
        /// Initializes a new object.
        /// </summary>
        public GridPropertyTypeDefaultStyle()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="name">The Name for GridPropertyTypeDefaultStyle.</param>
        public GridPropertyTypeDefaultStyle(string name)
        {
            this.name = name;
        }

        #endregion

        /// <summary>
        /// Returns the name.
        /// </summary>
        /// <returns>Returns the Name.</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                if (style != null)
                {
                    style.Changed -= new StyleChangedEventHandler(style_Changed);
                    style.Changing -= new StyleChangedEventHandler(style_Changing);
                    style = null;
                }
            }

            base.Dispose(disposing);
        }

        #region ParentCollection

        GridPropertyTypeDefaultStyleCollection collection;

        internal void SetCollection(GridPropertyTypeDefaultStyleCollection collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// The parent collection
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridPropertyTypeDefaultStyleCollection Collection
        {
            get
            {
                return collection;
            }
        }

        #endregion

        #region PropertyChange

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="PropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanging(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }

            if (collection != null)
            {
                collection.RaisePropertyItemChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanged(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (collection != null)
            {
                collection.RaisePropertyItemChanged(this, e);
            }
        }

        #endregion

        #region Copy

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Copies this object.
        /// </summary>
        /// <returns>Copied object.</returns>
        public GridPropertyTypeDefaultStyle Clone()
        {
            GridPropertyTypeDefaultStyle cd = new GridPropertyTypeDefaultStyle();
            cd.InitializeFrom(this);
            return cd;
        }

        /// <summary>
        /// Initializes this object with contents from another source.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridPropertyTypeDefaultStyle other)
        {
            Name = other.name;
            Style.CopyFrom(other.Style);
        }

        #endregion

        #region Equals

        /// <summary>
        /// Compares two objects
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if both objects are equivalent.</returns>
        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is GridPropertyTypeDefaultStyle))
            {
                return false;
            }

            return Equals((GridPropertyTypeDefaultStyle)obj);
        }

        bool Equals(GridPropertyTypeDefaultStyle other)
        {
            return other.name == name; //// name identifies this object in IndexOf operation
        }

        /// <override/>
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Name

        bool inSetName = false;

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            return collection.GetStandardValues();
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InSetName
        {
            get
            {
                return inSetName;
            }
        }

        /// <summary>
        /// The Name (e.g. System.Boolean)
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    inSetName = true;
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    name = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                    inSetName = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// Specifies if cells of this type should show UITypeEditor, StandardValues
        /// if available.
        /// </summary>
        [DefaultValue(true)]
        [Description("Specifies if cells of this type should show UITypeEditor, StandardValues if available.")]
        public bool AllowDropDown
        {
            get
            {
                return allowDropDown;
            }

            set
            {
                allowDropDown = value;
            }
        }

        /// <summary>
        /// The default style setting
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellStyleInfo Style
        {
            get
            {
                if (style == null)
                {
                    style = new GridTableCellStyleInfo(new GridPropertyTypeDefaultStyleInfoIdentity(this));
                    style.Changed += new StyleChangedEventHandler(style_Changed);
                    style.Changing += new StyleChangedEventHandler(style_Changing);
                }

                return style;
            }
        }

        private void style_Changed(object sender, StyleChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Style", e));
        }

        private void style_Changing(object sender, StyleChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Style", e));
        }
    }

    #endregion

    #region TypeConverter

    internal class GridPropertyTypeDefaultStyleTypeConverter : ExpandableObjectConverter
    {
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            ////            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            ////                return true;
            ////            else
            return base.CanConvertTo(context, destinationType);
        }

        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is GridPropertyTypeDefaultStyle))
            {
                GridPropertyTypeDefaultStyle typename = (GridPropertyTypeDefaultStyle)value;
                System.Type[] args;
                args = new System.Type[1];
                args[0] = typeof(string);

                System.Reflection.ConstructorInfo constructorInfo;
                constructorInfo = typeof(GridPropertyTypeDefaultStyle).GetConstructor(args);
                if (constructorInfo != null)
                {
                    object[] argValues;
                    argValues = (object[])new System.Object[1];
                    argValues[0] = typename.Name;
                    return (object)new InstanceDescriptor(constructorInfo, argValues);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } //// end of method ConvertTo

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "Style"
            };

            return pds.Sort(atts);
        }
    }

    #endregion

    #region GridPropertyTypeDefaultStyleCollection

    /// <summary>
    /// A collection of <see cref="GridPropertyTypeDefaultStyle"/> with default
    /// <see cref="GridTableCellStyleInfo"/> information for RecordFieldCell elements
    /// based on the column's System.Type. Each basic type has default style information
    /// registered with this collection.
    /// </summary>
    /// <remarks>
    /// The collection contains pre-defined settings such as HorizontalAlignment for numbers and
    /// cell type (e.g. CheckBox for boolean).<para/>
    /// GridPropertyTypeDefaultStyle settings have less precedence in styles inheritance than
    /// <see cref="Appearance"/> styles. <para/>
    /// Note: Changes you make to this collection do not get serialized, you will need
    /// to reapply any changes even if you read back the schema from an XML file.
    /// </remarks>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class GridPropertyTypeDefaultStyleCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal bool insideCollectionEditor = false;
        bool autoPopulated = false;
        bool modified = false;
        bool inReset = false;
        internal GridEngine engine;

        bool isDefault = false;
        
        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsDefault
        {
            get
            {
                return isDefault;
            }
        }

        /// <summary>
        /// Disposes the collection.
        /// </summary>
        public void Dispose()
        {
            foreach (DescriptorBase db in inner)
            {
                db.Dispose();
            }

            inner.Clear();
            engine = null;
            GC.SuppressFinalize(this);
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static readonly GridPropertyTypeDefaultStyleCollection Empty = new GridPropertyTypeDefaultStyleCollection();
        [ThreadStatic]
        static GridPropertyTypeDefaultStyleCollection _default;

        /// <summary>
        /// The default collection 
        /// </summary>
        public static GridPropertyTypeDefaultStyleCollection Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new GridPropertyTypeDefaultStyleCollection();
                    int _char = _default.IndexOf("System.Char");
                    int _byte = _default.IndexOf("System.Byte");
                    int _int16 = _default.IndexOf("System.Int16");
                    int _int32 = _default.IndexOf("System.Int32");
                    int _int64 = _default.IndexOf("System.Int64");
                    int _uint16 = _default.IndexOf("System.UInt16");
                    int _uint32 = _default.IndexOf("System.UInt32");
                    int _uint64 = _default.IndexOf("System.UInt64");
                    int _single = _default.IndexOf("System.Single");
                    int _double = _default.IndexOf("System.Double");
                    int _string = _default.IndexOf("System.String");
                    int _boolean = _default.IndexOf("System.Boolean");
                    int _datetime = _default.IndexOf("System.DateTime");
                    int _decimal = _default.IndexOf("System.Decimal");
                    int _dbnull = _default.IndexOf("System.DBNull");
                    int _byteArray = _default.IndexOf("System.Byte[]");
                    int _foreignKey = _default.IndexOf("ForeignKey");
                    int _standardValues = _default.IndexOf("StandardValues");
                    int _autoCompleteStandardValues = _default.IndexOf("AutoCompleteStandardValues");
                    int _uITypeEditor = _default.IndexOf("UITypeEditor");
                    int _propertyGrid = _default.IndexOf("PropertyGrid");
                    //// NOTE: If you add values here - you also need to add them in Reset() method!

                    _default[_char].Style.CellValueType = typeof(System.Char);
                    _default[_byte].Style.CellValueType = typeof(System.Byte);
                    _default[_int16].Style.CellValueType = typeof(System.Int16);
                    _default[_int32].Style.CellValueType = typeof(System.Int32);
                    _default[_int64].Style.CellValueType = typeof(System.Int64);
                    _default[_uint16].Style.CellValueType = typeof(System.UInt16);
                    _default[_uint32].Style.CellValueType = typeof(System.UInt32);
                    _default[_uint64].Style.CellValueType = typeof(System.UInt64);
                    _default[_single].Style.CellValueType = typeof(System.Single);
                    _default[_double].Style.CellValueType = typeof(System.Double);
                    _default[_string].Style.CellValueType = typeof(System.String);
                    _default[_boolean].Style.CellValueType = typeof(System.Boolean);
                    _default[_datetime].Style.CellValueType = typeof(System.DateTime);
                    _default[_decimal].Style.CellValueType = typeof(System.Decimal);
                    _default[_dbnull].Style.CellValueType = typeof(System.DBNull);
                    _default[_byteArray].Style.CellValueType = typeof(System.Byte[]);
                    _default[_byteArray].AllowDropDown = false;

                    _default[_char].Style.CellType = "TextBox";
                    _default[_byte].Style.CellType = "TextBox";
                    _default[_byte].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_int16].Style.CellType = "TextBox";
                    _default[_int16].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_int32].Style.CellType = "TextBox";
                    _default[_int32].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_int64].Style.CellType = "TextBox";
                    _default[_int64].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_uint16].Style.CellType = "TextBox";
                    _default[_uint16].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_uint32].Style.CellType = "TextBox";
                    _default[_uint32].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_uint64].Style.CellType = "TextBox";
                    _default[_uint64].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_single].Style.CellType = "TextBox";
                    _default[_single].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_double].Style.CellType = "TextBox";
                    _default[_double].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_string].Style.CellType = "TextBox";
                    _default[_boolean].Style.CellType = "CheckBox";
                    _default[_boolean].Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                    _default[_datetime].Style.CellType = "MonthCalendar";
                    _default[_datetime].AllowDropDown = false;
                    _default[_decimal].Style.CellType = "TextBox";
                    _default[_decimal].Style.HorizontalAlignment = GridHorizontalAlignment.Right;
                    _default[_dbnull].Style.CellType = "TextBox";
                    _default[_byteArray].Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                    _default[_byteArray].Style.CellType = "Image";
                    _default[_byteArray].Style.ImageSizeMode = GridImageSizeMode.AutoSize;
                    _default[_foreignKey].Style.CellType = "ForeignKeyCell";
                    _default[_foreignKey].Style.DropDownStyle = GridDropDownStyle.AutoComplete;
                    _default[_standardValues].Style.CellType = "StandardValuesCell";
                    _default[_standardValues].Style.DropDownStyle = GridDropDownStyle.Editable;
                    _default[_standardValues].Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                    _default[_autoCompleteStandardValues].Style.DropDownStyle = GridDropDownStyle.AutoComplete;
                    _default[_uITypeEditor].Style.CellType = "UITypeEditorCell";
                    _default[_propertyGrid].Style.CellType = "PropertyGridCell";
                    _default.isDefault = true;
                }

                return _default;
            }
        }

        #region ctor

        /// <summary>
        /// Initializes an empty collection
        /// </summary>
        public GridPropertyTypeDefaultStyleCollection()
        {
            Reset();
        }

        ////        public GridPropertyTypeDefaultStyleCollection(GridPropertyTypeDefaultStyle[] propertyTypeDefaultStyles)
        ////        {
        ////            this.AddRange(propertyTypeDefaultStyles);
        ////        }

        #endregion

        #region Initialize

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool InsideCollectionEditor
        {
            get
            {
                return insideCollectionEditor;
            }

            set
            {
                if (insideCollectionEditor != value)
                {
                    insideCollectionEditor = value;
                }
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this);
                }
#else
                ;
#endif
            }
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((GridPropertyTypeDefaultStyleCollection)other);
        }

        /// <summary>
        /// Initializes collection from another collection
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridPropertyTypeDefaultStyleCollection other)
        {
            int i;
            int count = Math.Min(Count, other.Count);
            for (i = 0; i < count; i++)
            {
                this[i].InitializeFrom(other[i].Clone());
            }

            for (; i < other.Count; i++)
            {
                Add(other[i].Clone());
            }

            while (Count > other.Count)
            {
                RemoveAt(Count - 1);
            }
        }

        /// <summary>
        /// Reset and initialize with default settings.
        /// </summary>
        public void Reset()
        {
            inReset = true;
            ////isReset = true;
            autoPopulated = false;
            modified = false;
            inner.Clear();
            Add("System.Char");
            Add("System.Byte");
            Add("System.Int16");
            Add("System.Int32");
            Add("System.Int64");
            Add("System.UInt16");
            Add("System.UInt32");
            Add("System.UInt64");
            Add("System.Single");
            Add("System.Double");
            Add("System.String");
            Add("System.Boolean");
            Add("System.DateTime");
            Add("System.Decimal");
            Add("System.DBNull");
            Add("System.Byte[]");
            Add("ForeignKey");
            Add("StandardValues");
            Add("AutoCompleteStandardValues");
            Add("UITypeEditor");
            Add("PropertyGrid");

            modified = false;
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            inReset = false;
        }

        internal ICollection GetStandardValues()
        {
            ArrayList al = new ArrayList();
            return al;
        }

        bool inEnsureInitialized = false;
        ////bool isReset = true;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void EnsureInitialized(bool populate)
        {
            ////            if (inEnsureInitialized || !isReset)
            ////                return;
            ////
            ////            inEnsureInitialized = true;
            ////            try
            ////            {
            ////                if (populate && inner.Count == 0 && !this.modified)
            ////                {
            ////                    isReset = false;
            ////                    foreach (string s in GetStandardValues())
            ////                    {
            ////                        GridPropertyTypeDefaultStyle column = new GridPropertyTypeDefaultStyle(s);
            ////                        column.SetCollection(this);
            ////                        Add(column);
            ////                    }
            ////                    autoPopulated = true;
            ////                    modified = false;
            ////                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            ////                }
            ////            }
            ////            finally
            ////            {
            ////                inEnsureInitialized = false;
            ////            }
        }

        #endregion

        #region Clone
        
        /// <summary>
        /// Creates a copy of this collection.
        /// </summary>
        /// <returns>Copied object.</returns>
        public GridPropertyTypeDefaultStyleCollection Clone()
        {
            GridPropertyTypeDefaultStyleCollection coll = new GridPropertyTypeDefaultStyleCollection();
            coll.inner = new ArrayList();
            coll.insideCollectionEditor = insideCollectionEditor;
            coll.version = version + 1000;
            int count = Count;
            GridPropertyTypeDefaultStyle[] propertyTypeDefaultStyles = new GridPropertyTypeDefaultStyle[count];
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }

            coll.autoPopulated = this.autoPopulated;
            coll.modified = this.modified;
            return coll;
        }

        #endregion

        #region Equals

        /// <override/>
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the
        /// current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
        /// <see cref="T:System.Object" />. </param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is GridPropertyTypeDefaultStyleCollection))
            {
                return false;
            }

            return Equals((GridPropertyTypeDefaultStyleCollection)obj);
        }

        /// <override/>
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int Version
        {
            get
            {
                return version;
            }
        }

        bool Equals(GridPropertyTypeDefaultStyleCollection other)
        {
            int count = Count;
            if (other.Count != count)
            {
                return false;
            }

            for (int n = 0; n < count; n++)
            {
                if (!this[n].Equals(other[n]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsModified
        {
            get
            {
                return modified;
            }
        }

        #endregion

        #region Item

        /// <summary>
        /// Gets or sets the element at an index
        /// </summary>
        public GridPropertyTypeDefaultStyle this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (GridPropertyTypeDefaultStyle)inner[index];
            }

            set
            {
                ////                TraceUtil.TraceCurrentMethodInfo(this, index, value);
                if (inner[index] != value)
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    inner[index] = value;
                    value.SetCollection(this);
                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified name. 
        /// </summary>
        public GridPropertyTypeDefaultStyle this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridPropertyTypeDefaultStyle)inner[index];
            }

            set
            {
                ////                TraceUtil.TraceCurrentMethodInfo(this, name, value);
                int index = Find(name);
                if (index == -1)
                {
                    value.Name = name;
                    Add(value);
                }
                else
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    inner[index] = value;
                    value.Name = name;
                    value.SetCollection(this);
                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        internal int Find(string name)
        {
            EnsureInitialized(true);
            for (int n = 0; n < Count; n++)
            {
                if (this[n].Name == name)
                {
                    return n;
                }
            }

            return -1;
        }

        #endregion

        #region StronglyTypedList

        /// <summary>
        /// Adds multiple elements.
        /// </summary>
        /// <param name="propertyTypeDefaultStyles">An array of elements to add.</param>
        public void AddRange(GridPropertyTypeDefaultStyle[] propertyTypeDefaultStyles)
        {
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this, propertyTypeDefaultStyles.Length);
            }
#else
            ;
#endif

            this.inner.AddRange(propertyTypeDefaultStyles);
            for (int n = 0; n < propertyTypeDefaultStyles.Length; n++)
            {
                propertyTypeDefaultStyles[n].SetCollection(this);
            }
        }

        /// <overload>
        /// Checks if the group belongs to the details section and is visible.
        /// </overload>
        /// <summary>
        /// Checks if the group belongs to the details section and is visible.
        /// </summary>
        /// <param name="value">The given group.</param>
        /// <returns>True if it belongs to the details section and is visible.</returns>
        public bool Contains(GridPropertyTypeDefaultStyle value)
        {
            EnsureInitialized(true);

            if (value == null)
            {
                return false;
            }

            return inner.Contains(value);
        }

        /// <summary>
        /// Checks if collection contains element.
        /// </summary>
        /// <param name="name">Element name to check.</param>
        /// <returns>True if it contains.</returns>
        public bool Contains(string name)
        {
            return Find(name) != -1;
        }

        /// <overload>
        /// Gets the index of the occurence of an GridPropertyTypeDefaultStyle from the GridPropertyTypeDefaultStyleCollection.
        /// </overload>
        /// <summary>
        /// Gets the index of the occurence of an GridPropertyTypeDefaultStyle  from the GridPropertyTypeDefaultStyleCollection.
        /// </summary>
        /// <param name="value">The GridPropertyTypeDefaultStyle.</param>
        /// <returns>The Index of the GridPropertyTypeDefaultStyle.</returns>
        public int IndexOf(GridPropertyTypeDefaultStyle value)
        {
            EnsureInitialized(true);
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Gets the index of the occurence of an element.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <returns>returns the Index of the element.</returns>
        public int IndexOf(string name)
        {
            return Find(name);
        }

        /// <summary>
        /// Copies the elements to an array
        /// </summary>
        /// <param name="array">The Array of GridPropertyTypeDefaultStyle.</param>
        /// <param name="index">Index to copy.</param>
        public void CopyTo(GridPropertyTypeDefaultStyle[] array, int index)
        {
            EnsureInitialized(true);
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public GridPropertyTypeDefaultStyleCollection SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <returns>The GridPropertyTypeDefaultStyleCollectionEnumerator</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public GridPropertyTypeDefaultStyleCollectionEnumerator GetEnumerator()
        {
            return new GridPropertyTypeDefaultStyleCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element
        /// </summary>
        /// <param name="index">The Index.</param>
        /// <param name="value">Value to insert.</param>
        public void Insert(int index, GridPropertyTypeDefaultStyle value)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this, index, value.Name);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            inner.Insert(index, value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Removes an element
        /// </summary>
        /// <param name="value">Value to remove.</param>
        public void Remove(GridPropertyTypeDefaultStyle value)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this, value.Name);
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.Remove(value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds a new element
        /// </summary>
        /// <param name="value">Value to add.</param>
        /// <returns>Index of the new value.</returns>
        public int Add(GridPropertyTypeDefaultStyle value)
        {
            if (this.modified == false && this.autoPopulated && !this.inEnsureInitialized)
            {
                inner.Clear();
            }
            else if (!this.inEnsureInitialized)
            {
                this.EnsureInitialized(false);
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            if (value.Name != null && value.Name.Length > 0)
            {
                if (this.inner.Count > 0 && Contains(value.Name))
                {
                    throw new Exception(String.Format("Column '{0}': Duplicates are not allowed ", value.Name));
                }
            }
            else
            {
                SuggestName(value);
            }

            int index = inner.Add(value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Adds a new element.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <returns>Index of the new element.</returns>
        public int Add(string name)
        {
            return Add(new GridPropertyTypeDefaultStyle(name));
        }

        /// <summary>
        /// Removes an element.
        /// </summary>
        /// <param name="name">Name of the element to be removed.</param>
        public void Remove(string name)
        {
            EnsureInitialized(true);
            int index = Find(name);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes an element.
        /// </summary>
        /// <param name="index">Index of the element to be removed.</param>
        public void RemoveAt(int index)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this, index);
            EnsureInitialized(true);
            object value = inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.RemoveAt(index);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        #endregion

        #region TypeNeutral IList Members

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            inner.Clear();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The number of elements in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                EnsureInitialized(true);
                return inner.Count;
            }
        }

        #endregion

        #region Change Events

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public event ListPropertyChangedEventHandler Changed;

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            version++;
            if (!this.inEnsureInitialized && !this.inReset)
            {
                modified = true;
            }

            if (!this.inEnsureInitialized && !this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                }
#else
                ;
#endif
                if (Changed != null)
                {
                    Changed(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanged(GridPropertyTypeDefaultStyle column, DescriptorPropertyChangedEventArgs e)
        {
            ////            if (e.PropertyName == "Name")
            ////            {
            ////                foreach (GridPropertyTypeDefaultStyle sc in this)
            ////                {
            ////                    if (sc != column && sc.Name == column.Name)
            ////                        throw new Exception(String.Format("Column '{0}': Duplicates are not allowed ", column.Name));
            ////                }
            ////            }

            if (!this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, inner.IndexOf(column), column, e.PropertyName, e));
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                }
#else
                ;
#endif
                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanging(GridPropertyTypeDefaultStyle column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, inner.IndexOf(column), column, e.PropertyName, e));
            }
        }

        #endregion

        #region ICloneable Private Members
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region IList Private Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (GridPropertyTypeDefaultStyle)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridPropertyTypeDefaultStyle)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridPropertyTypeDefaultStyle)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridPropertyTypeDefaultStyle)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridPropertyTypeDefaultStyle)value);
        }

        int IList.Add(object value)
        {
            return Add((GridPropertyTypeDefaultStyle)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridPropertyTypeDefaultStyle[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Private Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return String.Format("GridPropertyTypeDefaultStyleCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        void SuggestName(GridPropertyTypeDefaultStyle value)
        {
            if (value.Name == null || value.Name.Length == 0)
            {
                ICollection values = GetStandardValues();
                foreach (string name in values)
                {
                    if (IndexOf(name) == -1)
                    {
                        value.Name = name;
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Enumerator class for <see cref="GridPropertyTypeDefaultStyle"/> elements of a <see cref="GridPropertyTypeDefaultStyleCollection"/>.
    /// </summary>
    public class GridPropertyTypeDefaultStyleCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridPropertyTypeDefaultStyleCollection _coll;

        /// <summary>
        /// Initializes the enumerator
        /// </summary>
        /// <param name="collection">The collection.</param>
        public GridPropertyTypeDefaultStyleCollectionEnumerator(GridPropertyTypeDefaultStyleCollection collection)
        {
            _coll = collection;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Resets the enumerator
        /// </summary>
        public virtual void Reset()
        {
            _cursor = -1;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Returns the current element
        /// </summary>
        public GridPropertyTypeDefaultStyle Current
        {
            get
            {
                return _coll[_cursor];
            }
        }

        /// <summary>
        /// Moves to the next position
        /// </summary>
        /// <returns>True if next element exists.</returns>
        public bool MoveNext()
        {
            if (_next == -1)
            {
                return false;
            }

            _cursor = _next;

            _next++;
            if (_next >= _coll.Count)
            {
                _next = -1;
            }

            return _cursor != -1;
        }
        #endregion
    }
    #endregion
    
    /// <internalonly/>
    /// <summary>Internal only.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridPropertyTypeDefaultStyleInfoIdentity : Syncfusion.Styles.StyleInfoIdentityBase
    {
#if WEAKREF
        // Cache
        WeakReference __cachedBaseStyles;

        IStyleInfo[] cachedBaseStyles
        {
            get
            {
                if (__cachedBaseStyles != null)
                    return (IStyleInfo[]) __cachedBaseStyles.Target;
                return null;
            }
            set
            {
                if (value != null)
                    __cachedBaseStyles = new WeakReference(value);
                else
                    __cachedBaseStyles = null;
            }
        }

//        ~GridPropertyTypeDefaultStyleInfoIdentity()
//        {
//            //if (displayElement is IGridPropertyTypeDefaultStyleInfoWeakReferences)
//            //    ((IGridPropertyTypeDefaultStyleInfoWeakReferences) displayElement).FinalizingPropertyTypeDefaultStyleInfoIdentity();
//        }
#else
        IStyleInfo[] cachedBaseStyles;
#endif

        // Identity properties.
        private GridPropertyTypeDefaultStyle owner;

        /// <summary>
        /// Property Owner (GridPropertyTypeDefaultStyle).
        /// </summary>
        public GridPropertyTypeDefaultStyle Owner
        {
            get
            {
                return this.owner;
            }

            set
            {
                this.owner = value;
            }
        }

        int version;

        /// <override/>
        /// <summary>Disposes the object.</summary>
        public override void Dispose()
        {
            cachedBaseStyles = null;
            base.Dispose();
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public GridPropertyTypeDefaultStyleInfoIdentity(GridPropertyTypeDefaultStyle owner)
        {
            this.owner = owner;
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridTableCellType tableCellType = GridTableCellType.AnyRecordFieldCell;

        /// <summary>
        /// Overridden. Returns base styles from <see cref="IGridData"/> by calling <see cref="IGridData.GetBaseStyles"/>.
        /// </summary>
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/>.</param>
        /// <returns>An array of base styles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            GridEngine engine = owner.Collection.engine;

            if (engine != null && (cachedBaseStyles == null || version != engine.Version))
            {
                ArrayList styleList = new ArrayList();

                //// Engine
                if (engine != null)
                {
                    if (engine.Appearance.IsModifiedStyle(this.tableCellType))
                    {
                        styleList.Add(engine.Appearance.GetStyle(this.tableCellType));
                    }

                    foreach (GridTableCellStyleInfo style in engine.Appearance.GetBaseStyles(this.tableCellType))
                    {
                        if (style != null)
                        {
                            styleList.Add(style);
                        }
                    }
                }

                //// Base Styles.
                if (engine != null)
                {
                    string baseStyle = string.Empty;
                    foreach (GridStyleInfo style in styleList)
                    {
                        if (style.HasBaseStyle)
                        {
                            baseStyle = style.BaseStyle;
                            break;
                        }
                    }

                    int level = 0;
                    if (baseStyle != string.Empty)
                    {
                        GridTableCellStyleInfo[] baseStyles = engine.BaseStyles.GetBaseStylesMapStyles(baseStyle, out level);
                        for (int n = 0; n < level; n++)
                        {
                            styleList.Add(baseStyles[n]);
                        }
                    }
                }

                styleList.Add(GridTableCellAppearance.Default.GetStyle(this.tableCellType));
                styleList.AddRange(GridTableCellAppearance.Default.GetBaseStyles(this.tableCellType));

                version = engine.Version;
                cachedBaseStyles = new GridTableCellStyleInfo[styleList.Count];
                styleList.CopyTo(cachedBaseStyles);
            }

            return cachedBaseStyles;
        }

        /// <override/>
        /// <summary>
        /// Returns a sring that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" {");
            sb.AppendFormat("{0}", this.tableCellType);
            sb.AppendFormat("{0}", this.owner);
            sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// Overridden. If the style is not offline, saves its changes in the <see cref="IGridData"/>.
        /// </summary>
        /// <param name="style">A reference to the <see cref="GridStyleInfo"/> object.</param>
        /// <param name="sip">The <see cref="StyleInfoProperty"/> that identifies the changed style property.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            cachedBaseStyles = null;
            base.OnStyleChanged(style, sip);
        }

        ////        /// <summary>
        ////        /// Results of ToString method.
        ////        /// </summary>
        ////        public string Info
        ////        {
        ////            get
        ////            {
        ////                return ToString();
        ////            }
        ////        }
    }
}

