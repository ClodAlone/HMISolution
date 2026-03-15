//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableBaseStyle.cs" company="syncfusion">
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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

using Syncfusion.Grouping;

#if ASPNET
using System.Web.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Provides identity information for a <see cref="GridTableBaseStyle"/> object.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridTableBaseStyleInfoIdentity : StyleInfoIdentityBase
    {
        GridTableBaseStyle baseStyle;

        /// <override/>
        public override void Dispose()
        {
            baseStyle = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the identity object for a <see cref="GridTableBaseStyle"/>.
        /// </summary>
        /// <param name="baseStyle">The base style object.</param>
        public GridTableBaseStyleInfoIdentity(GridTableBaseStyle baseStyle)
        {
            this.baseStyle = baseStyle;
        }

        /// <summary>
        /// The base style object.
        /// </summary>
        public GridTableBaseStyle BaseStyle
        {
            get { return baseStyle; }
        }

        /// <summary>
        /// Returns an array with base styles for the specified style object.
        /// </summary>
        /// <param name="thisStyleInfo">The style object.</param>
        /// <returns>
        /// An array of style objects that are base styles for the current style object.
        /// </returns>
        /// <override/>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (baseStyle != null && baseStyle.Collection != null)
            {
                GridStyleInfo gridStyleInfo = (GridStyleInfo)thisStyleInfo;
                string baseStyleName = gridStyleInfo.HasBaseStyle ? gridStyleInfo.BaseStyle : string.Empty;
                int level;
                GridStyleInfo[] gridStyles = baseStyle.Collection.GetBaseStylesMapStyles(baseStyleName, out level);
                IStyleInfo[] levels = new IStyleInfo[level];
                Array.Copy(gridStyles, levels, level);
                return levels;
            }

            return null;
        }
    }

    #region TypeConverter
    /// <summary>
    /// The type converter for <see cref="GridTableBaseStyle"/> objects. The <see cref="GridTableBaseStyleTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// ConvertTo method and adds support for design-time code serialization.
    /// </summary>
    public class GridTableBaseStyleTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridTableBaseStyleTypeConverter()
            : base()
        { 
        }

        /// <override/>
        /// <summary>
        /// Indicates whether this object can be converted to the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">The type
        /// you want to convert to. </param>
        /// <returns>True if this conversion is allowed.</returns>
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
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
        /// Converts the given value to the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                Type type = value.GetType();
                return new InstanceDescriptor(
                    type.GetConstructor(new Type[] { typeof(string) }),
                    new object[] { ((GridTableBaseStyle)value).Name },
                    false);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <override/>
        /// <summary>
        /// Gets a collection of properties for the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Value specifying the type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "StyleInfo",
            };

            return pds.Sort(atts);
        }
    }
    #endregion

    #region TypeConverter
    /// <summary>
    /// The type converter for the <see cref="GridTableBaseStyle.Name"/> string of a
    /// <see cref="GridTableBaseStyle"/>. <see cref="GridTableBaseStyleNameConverter"/>
    /// overrides the GetStandardValues method and returns possible BaseStyles from
    /// the <see cref="GridEngine.BaseStyles"/> of the <see cref="GridEngine"/>.
    /// </summary>
    public class GridTableBaseStyleNameConverter : TypeConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridTableBaseStyleNameConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is
        /// designed for when provided with a format context.
        /// </summary>
        /// <param name="context">Format
        /// context that can be used to extract additional information about the environment
        /// from which this converter is invoked. This parameter or properties of this
        /// parameter can be null. </param>
        /// <returns>
        /// A <see cref="TypeConverter.StandardValuesCollection" />
        /// that holds a standard set of valid values, or null if the data type does not
        /// support a standard set of values.
        /// </returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context == null)
            {
                return base.GetStandardValues(context);
            }

            GridTableCellStyleInfo style = context.Instance as GridTableCellStyleInfo;
            GridTableCellStyleInfoIdentity tableCellStyleInfoIdentity = style.Identity as GridTableCellStyleInfoIdentity;
            GridTableCellAppearanceStyleInfoIdentity tableCellAppearanceStyleInfoIdentity = style.Identity as GridTableCellAppearanceStyleInfoIdentity;
            GridTableBaseStyleInfoIdentity tableBaseStyleInfoIdentity = style.Identity as GridTableBaseStyleInfoIdentity;
            ArrayList al = new ArrayList();
            if (tableBaseStyleInfoIdentity != null)
            {
                if (tableBaseStyleInfoIdentity.BaseStyle != null && tableBaseStyleInfoIdentity.BaseStyle.Collection != null)
                {
                    foreach (GridTableBaseStyle tableBaseStyle in tableBaseStyleInfoIdentity.BaseStyle.Collection)
                    {
                        if (tableBaseStyle != tableBaseStyleInfoIdentity.BaseStyle)
                        {
                            al.Add(tableBaseStyle.Name);
                        }
                    }
                }
            }
            else if (tableCellStyleInfoIdentity != null)
            {
                GridEngine engine = tableCellStyleInfoIdentity.GetEngine();
                if (engine != null)
                {
                    foreach (GridTableBaseStyle tableBaseStyle in engine.BaseStyles)
                    {
                        al.Add(tableBaseStyle.Name);
                    }

                    foreach (GridTableBaseStyle tableBaseStyle in engine.DefaultBaseStyles)
                    {
                        if (!al.Contains(tableBaseStyle.Name))
                        {
                            al.Add(tableBaseStyle.Name);
                        }
                    }
                }
            }
            else if (tableCellAppearanceStyleInfoIdentity != null)
            {
                GridEngine engine = tableCellAppearanceStyleInfoIdentity.Engine;
                if (engine != null)
                {
                    foreach (GridTableBaseStyle tableBaseStyle in engine.BaseStyles)
                    {
                        al.Add(tableBaseStyle.Name);
                    }

                    foreach (GridTableBaseStyle tableBaseStyle in engine.DefaultBaseStyles)
                    {
                        if (!al.Contains(tableBaseStyle.Name))
                        {
                            al.Add(tableBaseStyle.Name);
                        }
                    }
                }
            }

            return new TypeConverter.StandardValuesCollection(al);
        }

        /// <override/>
        /// <summary>
        /// Returns whether the collection of standard values returned from <see
        /// cref="GetStandardValuesExclusive" /> is an
        /// exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">A format
        /// context. </param>
        /// <returns>
        /// true if the <see
        /// cref="TypeConverter.StandardValuesCollection" />
        /// returned from <see
        /// cref="GetStandardValuesExclusive" /> is an
        /// exhaustive list of possible values; false if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;    //// enables support for late bound scenario
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">A format context.</param>
        /// <returns>returns true</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>      
        /// <param name="context">A format
        /// context. </param>
        /// <param name="sourceType">The type
        /// you want to convert from. </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">A format
        /// context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <returns>
        /// An <see cref="System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (string)value;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
    #endregion
    /// <summary>
    /// A GridTableBaseStyle
    /// declares BaseStyles for the whole engine.
    /// GridTableBaseStyles are managed by the <see cref="GridTableBaseStyleCollection"/> which
    /// is returned by the <see cref="GridEngine.BaseStyles"/> property
    /// of a <see cref="GridEngine"/>.
    /// </summary>
    [TypeConverter(typeof(GridTableBaseStyleTypeConverter))]
    public class GridTableBaseStyle : DescriptorBase, ICloneable
    {
        string name = string.Empty;
        GridTableCellStyleInfo styleInfo;
        GridTableBaseStyleCollection collection;
        internal int index;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return GetType().Name + "{ " + Name + "}";
        }

        /// <summary>
        /// Initializes a new empty object.
        /// </summary>
        public GridTableBaseStyle()
        {
        }

        /// <summary>
        /// Initializes a new object with a name.
        /// </summary>
        /// <param name="name">The name of the GridTableBaseStyle.</param>
        public GridTableBaseStyle(string name)
        {
            this.name = name;
        }

        /// <override/>
        /// <summary>Gets the name of the descriptor.</summary>
        /// <returns>The name of this descriptor. This name is used to look up the summary in the
        /// <see cref="GridTableBaseStyleCollection"/>.</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <override/>
        /// <summary>Resets the style.</summary>
        public override void Reset()
        {
            this.ResetStyle();
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public virtual void InitializeFrom(GridTableBaseStyle other)
        {
            this.Name = other.Name;
            if (other.ShouldSerializeStyle())
            {
                this.StyleInfo.CopyFrom(other.StyleInfo);
            }
            else
            {
                this.ResetStyle();
            }
        }

        internal void SetCollection(GridTableBaseStyleCollection collection)
        {
            this.collection = collection;
        }
        
        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableBaseStyleCollection Collection
        {
            get
            {
                return collection;
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

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
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

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public GridTableBaseStyle Clone()
        {
            GridTableBaseStyle newTableBaseStyle = new GridTableBaseStyle();
            newTableBaseStyle.InitializeFrom(this);
            return newTableBaseStyle;
        }

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
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is GridTableBaseStyle))
            {
                return false;
            }

            return InternalEquals((GridTableBaseStyle)obj);
        }

        bool InternalEquals(GridTableBaseStyle other)
        {
            return other.name == name;
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

        /// <summary>
        /// The name of this descriptor. This name is used to look up the summary in the
        /// <see cref="GridTableBaseStyleCollection"/>.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of this descriptor.  This name is used to look up the summary in the GridTableBaseStyleCollection"),
        Category("Look and Feel")]
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
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    string str = name;
                    name = value;
                    try
                    {
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                    }
                    catch
                    {
                        name = str;
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if name is not empty.
        /// </summary>
        /// <returns>True if not empty; False if empty.</returns>
        public bool ShouldSerializeName()
        {
            return Name != string.Empty;
        }

        /// <summary>
        /// Resets the name to empty string.
        /// </summary>
        public void ResetName()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// The style information for this base style.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("The style information for this base style"),
        Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo StyleInfo
        {
            get
            {
                if (styleInfo == null)
                {
                    styleInfo = new GridTableCellStyleInfo(new GridTableBaseStyleInfoIdentity(this));
                    styleInfo.Changed += new StyleChangedEventHandler(style_Changed);
                    styleInfo.Changing += new StyleChangedEventHandler(style_Changing);
                }

                return styleInfo;
            }

            set
            {
                StyleInfo.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines if style information has been set and should be serialized into code at design-time.
        /// </summary>
        /// <returns>True if style information has been set; False otherwise.</returns>
        public bool ShouldSerializeStyle()
        {
            return this.styleInfo != null && !StyleInfo.IsEmpty;
        }

        /// <summary>
        /// Resets style information.
        /// </summary>
        public void ResetStyle()
        {
            if (ShouldSerializeStyle())
            {
                StyleInfo = GridTableCellStyleInfo.Empty;
            }
        }

        private void style_Changed(object sender, StyleChangedEventArgs e)
        {
            OnPropertyChanged(new DescriptorPropertyChangedEventArgs("StyleInfo", e));
        }

        private void style_Changing(object sender, StyleChangedEventArgs e)
        {
            OnPropertyChanging(new DescriptorPropertyChangedEventArgs("StyleInfo", e));
        }
    }

    #region GridTableBaseStyleCollection
    /// <summary>
    /// A collection from <see cref="GridTableBaseStyle"/> that
    /// declares BaseStyles for the whole engine.
    /// An instance of this collection
    /// is returned by the <see cref="GridEngine.BaseStyles"/> property
    /// of a <see cref="GridEngine"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    public class GridTableBaseStyleCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        internal ArrayList _inner;
        internal SortedList _sorted;
        internal GridEngine _engine;
        internal int version;
        internal bool readOnly = false;
        bool modified = false;

        /// <summary>
        /// Occurs after a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static readonly GridTableBaseStyleCollection Empty = new GridTableBaseStyleCollection(null);

        internal bool insideCollectionEditor = false;

        internal void SetEngine(GridEngine engine)
        {
            this._engine = engine;
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                this[n].SetCollection(this);
            }
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return String.Format("GridTableBaseStyleCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        /// <summary>
        /// Ensures type correctness when a new element is added to the collection.
        /// </summary>
        /// <param name="obj">The newly added object.</param>
        protected virtual void CheckType(object obj)
        {
            if (obj != null && !(obj is GridTableBaseStyle))
            {
                throw new ArgumentException("Wrong type");
            }
        }

        /// <summary>
        /// Gets / sets whether the collection is manipulated inside a collection editor.
        /// </summary>
        public bool InsideCollectionEditor
        {
            get
            {
                return insideCollectionEditor;
            }

            set
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this);
                }
#else
                ;
#endif
                if (insideCollectionEditor != value)
                {
                    insideCollectionEditor = value;
                }
            }
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((GridTableBaseStyleCollection)other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridTableBaseStyleCollection other)
        {
            int i;

            try
            {
                int count = Math.Min(Count, other.Count);

                _sorted = null;

                while (Count > other.Count)
                {
                    RemoveAt(Count - 1);
                }

                for (i = 0; i < count; i++)
                {
                    this[i].InitializeFrom(other[i]);
                }

                for (; i < other.Count; i++)
                {
                    GridTableBaseStyle cd = new GridTableBaseStyle();
                    cd.InitializeFrom(other[i]);
                    Add(cd);
                }

                _sorted = new SortedList();
                foreach (GridTableBaseStyle cd in this._inner)
                {
                    _sorted.Add(cd.Name, cd);
                }
            }
            finally
            {
            }
        }

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridTableBaseStyleCollection()
            : this(null)
        {
        }

        internal GridTableBaseStyleCollection(GridEngine engine)
        {
            this._inner = new ArrayList();
            this._sorted = new SortedList();
            _engine = engine;
        }

        internal GridTableBaseStyleCollection(GridEngine engine, GridTableBaseStyle[] tableBaseStyles)
            : this(engine)
        {
            this.AddRange(tableBaseStyles);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Reset()
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            this.modified = false;
            _inner.Clear();
            if (_sorted != null)
            {
                _sorted.Clear();
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="tableBaseStyles">The array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(GridTableBaseStyle[] tableBaseStyles)
        {
            EnsureInitialized(false);
            int index = 0;
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, 0, null, null));
            if (modified)
            {
                index = _inner.Count;
            }
            else
            {
                _inner.Clear();
                if (_sorted != null)
                {
                    _sorted.Clear();
                }
            }

            modified = true;
            this._inner.AddRange(tableBaseStyles);
            for (int n = 0; n < tableBaseStyles.Length; n++)
            {
                CheckType(tableBaseStyles[n]);
                tableBaseStyles[n].index = index++;
                tableBaseStyles[n].SetCollection(this);
                if (_sorted != null)
                {
                    this._sorted.Add(tableBaseStyles[n].Name, tableBaseStyles[n]);
                }

                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index - 1, tableBaseStyles[n], null));
            }
        }

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor && !this.inEnsureInitialized)
            {
                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            version++;

            foreach (GridTableBaseStyle gbs in _inner)
            {
                if (gbs.StyleInfo.HasFont)
                {
                    gbs.StyleInfo.Font.ResetGdipFont();
                }
            }

            modified = true;
            if (!this.insideCollectionEditor && !this.inEnsureInitialized)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
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

        internal void RaisePropertyItemChanged(GridTableBaseStyle column, DescriptorPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (_sorted != null)
                {
                    int index = _sorted.IndexOfValue(column);
                    if (index != -1)
                    {
                        _sorted.RemoveAt(index);
                    }

                    _sorted.Add(column.Name, column);
                }
            }

            if (!this.InsideCollectionEditor && e.PropertyName != "FieldDescriptor")
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, Find(column.Name), column, e.PropertyName, e));
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
            }
        }

        internal void RaisePropertyItemChanging(GridTableBaseStyle column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, Find(column.Name), column, e.PropertyName, e));
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
            }
        }

        bool inEnsureInitialized = false;

        internal void EnsureInitialized(bool populate)
        {
        }

        /// <summary>
        /// Creates a new empty GridTableBaseStyle with the specified name.
        /// </summary>
        /// <param name="name">The name of the new base style.</param>
        /// <returns>A new GridTableBaseStyle.</returns>
        protected virtual GridTableBaseStyle InternalCreateGridTableBaseStyle(string name)
        {
            return new GridTableBaseStyle(name);
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridTableBaseStyleCollection Clone()
        {
            return InternalClone();
        }

        /// <summary>
        /// Creates a copy of this collection and all its inner elements. This method is called from Clone.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        protected GridTableBaseStyleCollection InternalClone()
        {
            int count = Count;
            GridTableBaseStyle[] tableBaseStyles = new GridTableBaseStyle[count];
            for (int n = 0; n < count; n++)
            {
                tableBaseStyles[n] = this[n].Clone();
            }

            GridTableBaseStyleCollection c = CreateCollection(_engine, tableBaseStyles);
            c.modified = modified;
            c.version = version + 1000;
            return c;
        }

        /// <summary>
        /// Called from InternalClone to create a new collection and attach it to the specified table descriptor
        /// and insert the specified columns. The BaseStyle objects have already been cloned.
        /// </summary>
        /// <param name="td">The GridEngine.</param>
        /// <param name="tableBaseStyles">The cloned base styles.</param>
        /// <returns>A new GridTableBaseStyleCollection.</returns>
        protected virtual GridTableBaseStyleCollection CreateCollection(GridEngine td, GridTableBaseStyle[] tableBaseStyles)
        {
            return new GridTableBaseStyleCollection(td, tableBaseStyles);
        }

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
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is GridTableBaseStyleCollection))
            {
                return false;
            }

            return InternalEquals((GridTableBaseStyleCollection)obj);
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

        /// <summary>
        /// The version number of this collection. The version is increased each time the
        /// collection or an element within the collection was modified.
        /// </summary>
        public int Version
        {
            get
            {
                EnsureInitialized(true);
                return version;
            }
        }

        /// <summary>
        /// Gets / sets whether the collection is modified from its default state.
        /// </summary>
        public bool IsModified
        {
            get
            {
                return modified;
            }
        }

        /// <summary>
        /// Compares each element with the element of another collection.
        /// </summary>
        /// <param name="other">The collection to compare to.</param>
        /// <returns>True if all elements are equal and in the same order; False otherwise.</returns>
        protected bool InternalEquals(GridTableBaseStyleCollection other)
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

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public GridTableBaseStyle this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (GridTableBaseStyle)_inner[index];
            }

            set
            {
                if (readOnly)
                {
                    throw new InvalidOperationException("Collection is Read-only.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                CheckType(value);

                EnsureInitialized(false);
                if (_inner[index] != value)
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    if (_sorted != null)
                    {
                        _sorted.Remove(this[index].Name);
                    }

                    _inner[index] = value;
                    value.index = index;
                    value.SetCollection(this);
                    if (_sorted != null)
                    {
                        _sorted.Add(value.Name, value);
                    }

                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
#if ASPNET
        public GridTableBaseStyle GetStyle(string name){return this[name];}
#endif

#if ASPNET
        internal
#else
        public
#endif
 GridTableBaseStyle this[string name]
        {
            get
            {
                EnsureInitialized(true);
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridTableBaseStyle)_inner[index];
            }

            set
            {
                if (readOnly)
                {
                    throw new InvalidOperationException("Collection is Read-only.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                CheckType(value);

                EnsureInitialized(false);
                int index = Find(name);
                value.Name = name;
                if (index == -1)
                {
                    Add(value);
                }
                else
                {
                    this[index] = value;
                }
            }
        }

        internal int Find(string name)
        {
            if (_sorted != null)
            {
                GridTableBaseStyle cd = (GridTableBaseStyle)_sorted[name];
                if (cd != null)
                {
                    return cd.index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(GridTableBaseStyle value)
        {
            if (value == null)
            {
                return false;
            }

            CheckType(value);

            EnsureInitialized(true);
            return _sorted != null && _sorted.Contains(value.Name);
        }

        /// <summary>
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(string name)
        {
            EnsureInitialized(true);
            return _sorted != null && _sorted.Contains(name);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULLreference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(GridTableBaseStyle value)
        {
            EnsureInitialized(true);
            if (!Contains(value))
            {
                return -1;
            }

            return this[value.Name].index;
        }

        /// <summary>
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element with matching name within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            EnsureInitialized(true);
            return Find(name);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(GridTableBaseStyle[] array, int index)
        {
            int n = 0;
            foreach (GridTableBaseStyle item in this)
            {
                array[index + n] = item;
                n++;
            }
        }

        GridTableBaseStyleCollection SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public GridTableBaseStyleCollectionEnumerator GetEnumerator()
        {
            return new GridTableBaseStyleCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridTableBaseStyle value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            CheckType(value);

            this.EnsureInitialized(true);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            _inner.Insert(index, value);
            if (_sorted != null)
            {
                _sorted.Add(value.Name, value);
            }

            value.SetCollection(this);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection the method will do nothing.</param>
        public void Remove(GridTableBaseStyle value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                return;
            }

            CheckType(value);

            this.EnsureInitialized(true);

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, -1, value, null));
            if (_sorted != null)
            {
                _sorted.Remove(value.Name);
            }

            int index = IndexOf(value);
            _inner.Remove(value);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridTableBaseStyle value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            CheckType(value);

            ////            if (_inner.Count > 15)
            ////                System.Diagnostics.Debugger.Break();
            ////            TraceUtil.TraceCurrentMethodInfo(value.Name);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            if (this.modified == false && !this.inEnsureInitialized)
            {
                if (_sorted != null)
                {
                    _sorted.Clear();
                }

                _inner.Clear();
            }
            else
            {
                this.EnsureInitialized(false);
            }

            if (value.Name == null || value.Name.Length == 0)
            {
                SuggestName(value);
            }

            int index = _inner.Add(value);
            value.index = index;
            if (_sorted != null)
            {
                _sorted.Add(value.Name, value);
            }

            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Called to get a new default name when a new BaseStyle is created (e.g. when pressing "Add" in a collection editor).
        /// </summary>
        /// <param name="value">The base style to be named.</param>
        protected virtual void SuggestName(GridTableBaseStyle value)
        {
            int n = 1;
            foreach (GridTableBaseStyle tableBaseStyle in this)
            {
                if (tableBaseStyle.Name.StartsWith("BaseStyle "))
                {
                    double d;
                    if (double.TryParse(tableBaseStyle.Name.Substring("BaseStyle ".Length), System.Globalization.NumberStyles.Number, null, out d))
                    {
                        n = (int)d + 1;
                    }
                }
            }

            value.Name = "BaseStyle " + n.ToString();
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name)
        {
            return InternalAdd(name);
        }

        /// <summary>
        /// Called from Add(string name) to create a new base style with the given name.
        /// </summary>
        /// <param name="name">The name of the new base style.</param>
        /// <returns>A new GridTableBaseStyle</returns>
        protected virtual int InternalAdd(string name)
        {
            return Add(new GridTableBaseStyle(name));
        }

        /// <summary>
        /// Removes the specified descriptor element with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the element to remove from the collection. If no element with that name is found
        /// in the collection, the method will do nothing.</param>
        public void Remove(string name)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            this.EnsureInitialized(true);
            int index = Find(name);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            this.EnsureInitialized(true);
            GridTableBaseStyle value = (GridTableBaseStyle)_inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            if (_sorted != null)
            {
                _sorted.Remove(value.Name);
            }

            _inner.RemoveAt(index);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Disposes the object and collection items.
        /// </summary>
        public void Dispose()
        {
            foreach (DescriptorBase db in _inner)
            {
                db.Dispose();
            }

            _inner.Clear();
            if (_sorted != null)
            {
                _sorted.Clear();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            this.EnsureInitialized(false);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            _inner.Clear();
            if (_sorted != null)
            {
                _sorted.Clear();
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
        }

        /// <summary>
        /// Determines if the collection is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        /// <summary>
        /// Returns normally False since this collection has no fixed size. Only when it is Read-only
        /// IsFixedSize returns True.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return readOnly;
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
        /// Gets the number of elements contained in the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </summary>
        public int Count
        {
            get
            {
                this.EnsureInitialized(true);
                return _inner.Count;
            }
        }

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
                this[index] = (GridTableBaseStyle)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridTableBaseStyle)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridTableBaseStyle)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridTableBaseStyle)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridTableBaseStyle)value);
        }

        int IList.Add(object value)
        {
            return Add((GridTableBaseStyle)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridTableBaseStyle[])array, index);
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
        
        /// <summary>
        /// Copies a BaseStyle and all BaseStyles it depends on into an array.
        /// </summary>
        /// <param name="name">The name of the base style.</param>
        /// <param name="level">The maximum number of levels to look at when walking referenced
        /// BaseStyles.</param>
        /// <returns>An array of <see cref="GridBaseStyle"/> objects with the first BaseStyle and all BaseStyles it depends on.</returns>
        public GridTableCellStyleInfo[] GetBaseStylesMapStyles(string name, out int level)
        {
            GridTableCellStyleInfo[] styleLevels = new GridTableCellStyleInfo[16];

            bool exitLoop = false;
            level = 0;
            while (level < 16)
            {
                if (this.Contains(name))
                {
                    GridTableBaseStyle baseStyle = this[name];
                    GridTableCellStyleInfo style = baseStyle.StyleInfo;
                    styleLevels[level++] = style;
                    if (name == "Standard" || name == string.Empty)
                    {
                        return styleLevels;
                    }
                    else if (style.HasBaseStyle)
                    {
                        name = style.BaseStyle;
                        continue;
                    }
                }

                if (exitLoop)
                {
                    break;
                }

                name = "Standard";
                exitLoop = true;
            }

            return styleLevels;
        }

        #region ICustomTypeDescriptor
        System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            ArrayList pds = new ArrayList();
            Attribute[] att = new Attribute[] 
            {
             new BrowsableAttribute(true),
             new System.Xml.Serialization.XmlIgnoreAttribute(),
             new RefreshPropertiesAttribute(RefreshProperties.All),
             new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
             new CategoryAttribute("Items")
            };

            ArrayList names = new ArrayList();
            foreach (DescriptorBase descriptor in this)
            {
                pds.Add(new DescriptorBasePropertyDescriptor(descriptor.GetName(), descriptor, att, GetType()));
                names.Add(descriptor.GetName());
            }

            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection((PropertyDescriptor[])pds.ToArray(typeof(PropertyDescriptor)));
            return pdc.Sort((string[])names.ToArray(typeof(string)));
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="GridTableBaseStyle"/> elements of a <see cref="GridTableBaseStyleCollection"/>.
    /// </summary>
    public class GridTableBaseStyleCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridTableBaseStyleCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GridTableBaseStyleCollectionEnumerator(GridTableBaseStyleCollection collection)
        {
            _coll = collection;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
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
        /// Gets the current element in the collection.
        /// </summary>
        public GridTableBaseStyle Current
        {
            get
            {
                return _coll[_cursor];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == -1 || _next >= _coll.Count)
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
}
