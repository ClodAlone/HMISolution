//-------------------------------------------------------------------------------------------------
// <copyright file="GridComboBoxCellModel.cs" company="syncfusion">
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
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for a combo box cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridComboBoxCellModel"/> can serve as model for several <see cref="GridComboBoxCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridComboBoxCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridComboBoxCellModel : GridDropDownCellModel
    {
        bool allowDoubleClickChangeSelectedIndex = true;

        /// <overload>
        /// Initializes a new <see cref="GridComboBoxCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridComboBoxCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridComboBoxCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            SupportsChoiceList = true;
        }

        /// <summary>
        /// Initializes a new <see cref="GridComboBoxCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridComboBoxCellModel(SerializationInfo info, StreamingContext context)
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
        protected override void Dispose(bool disposing)
        {
            if (this.listBox != null)
            {
                ////this.listBox.DataBindings.Clear();
                this.listBox.DataSource = null;
                this.listBox.Dispose();
                this.listBox = null;
            }

            base.Dispose(disposing);
        }
        
        /// <summary>
        /// Creates a <see cref="GridComboBoxCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridComboBoxCellRenderer"/> specific for a <see cref="GridControlBase"/>.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridComboBoxCellRenderer(control, this);
        }

        /// <summary>
        /// This is called to initialize data source on demand. This lets you calculate the datasource
        /// only when it is needed and not every time in QueryStyleInfo. Default behavior is to return
        /// style.ChoiceList if not empty. If style.ChoiceList is empty, style.DataSource is returned.
        /// </summary>
        /// <param name="style">The style object with binding information.</param>
        /// <returns>Data source.</returns>
        public virtual object GetDataSource(GridStyleInfo style)
        {
            return Grid.GetStyleDataSource(style);
        }

        /// <summary>
        /// Initializes a <see cref="ListBox"/> with data binding information from a <see cref="GridStyleInfo"/>
        /// object.
        /// </summary>
        /// <param name="listBox">The list box to be initialized with data binding information</param>
        /// <param name="style">The style object with binding information.</param>
        /// <param name="exclusive">A place holder that returns whether the list box is filled with an exclusive
        /// list of possible choices or if non-standard values are allowed.
        /// </param>
        public virtual void FillWithChoices(ListBox listBox, GridStyleInfo style, out bool exclusive)
        {
            exclusive = style.ExclusiveChoiceList;
            listBox.Name = "Combobox";
            object dataSource = GetDataSource(style);
            if (style.ChoiceList == null || style.ChoiceList.Count == 0)
            {
                if (dataSource != null)
                {
                    if (listBox.BindingContext == null
                        || dataSource != listBox.DataSource
                        || listBox.DisplayMember != style.DisplayMember
                        || listBox.ValueMember != style.ValueMember)
                    {
                        // fill with Choices
                        listBox.DataSource = null;
                        listBox.DisplayMember = string.Empty;
                        listBox.ValueMember = string.Empty;
                        listBox.DataSource = dataSource;
                        if (string.IsNullOrEmpty(style.DisplayMember) && !string.IsNullOrEmpty(style.ValueMember))
                            style.DisplayMember = style.ValueMember;
                        else if (!string.IsNullOrEmpty(style.DisplayMember) && string.IsNullOrEmpty(style.ValueMember) && !style.HasChoiceList && style.CellValueType==typeof(string) )
                            style.ValueMember = style.DisplayMember;
                        listBox.DisplayMember = style.DisplayMember;
                        listBox.ValueMember = style.ValueMember;
                        listBox.BindingContext = this.BindingContext;
                        TypeConverter tc = TypeDescriptor.GetConverter(TypeDescriptor.GetReflectionType(dataSource));
                        if (style.CellValueType == null && tc != null && (tc is EnumConverter || tc is CollectionConverter || tc is TypeConverter) && !(tc is ComponentConverter) && !(style.DataSource is ArrayConverter) && !(style.DataSource is ArrayList) && string.IsNullOrEmpty(style.DisplayMember) && string.IsNullOrEmpty(style.ValueMember))
                        {
                            TypeConverter convert = this.GetTypeConverter(style);
                            ICollection collection = convert.GetStandardValues();
                            if (collection != null && collection.Count > 0)
                            {
                                listBox.DataSource = null;
                                listBox.Items.Clear();
                                if (convert != null)
                                {
                                    foreach (object item in collection)
                                    {
                                        string s = convert.ConvertTo(item, typeof(String)).ToString();
                                        listBox.Items.Add(s);
                                    }
                                }
                            }
                            else
                            {
                                ICollection collect = style.DataSource as ICollection;
                                TypeConverter converter = null;
                                if (collect != null && collect.Count > 0)
                                {
                                    TypeConverter.StandardValuesCollection coll = new TypeConverter.StandardValuesCollection(collect);
                                    listBox.DataSource = null;
                                    listBox.Items.Clear();
                                    if (string.IsNullOrEmpty(style.DisplayMember) && !string.IsNullOrEmpty(style.ValueMember))
                                        style.DisplayMember = style.ValueMember;
                                    else if (!string.IsNullOrEmpty(style.DisplayMember) && string.IsNullOrEmpty(style.ValueMember) && style.CellValueType==typeof(string))
                                        style.ValueMember = style.DisplayMember;
                                    listBox.DisplayMember = style.DisplayMember;
                                    listBox.ValueMember = style.ValueMember;
                                    listBox.BindingContext = this.BindingContext;
                                    string s = string.Empty;
                                    foreach (object item in coll)
                                    {
                                        converter = TypeDescriptor.GetConverter(TypeDescriptor.GetReflectionType(item));
                                        if (converter != null)
                                        {
                                            s = converter.ConvertTo(item, typeof(string)).ToString();
                                            if (!converter.GetType().Name.Equals("TypeConverter"))
                                                listBox.Items.Add(s);
                                            else
                                                listBox.Items.Add(item);
                                        }
                                    }
                                }
                            }
                            if (style.DropDownStyle == GridDropDownStyle.Exclusive)
                            {
                                exclusive = true;
                            }
                        }
                        else if ((style.CellValueType != null && style.CellValueType.IsEnum) && tc != null && (tc is ArrayConverter))
                        {
                            TypeConverter convert = this.GetTypeConverter(style);
                            ICollection collection = convert.GetStandardValues();
                            if (collection != null && collection.Count > 0)
                            {
                                listBox.DataSource = null;
                                listBox.Items.Clear();
                                if (convert != null)
                                {
                                    foreach (object item in collection)
                                    {
                                        string s = convert.ConvertTo(item, typeof(String)).ToString();
                                        listBox.Items.Add(s);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Type valueType = style.CellValueType;
                    if (valueType != null)
                    {
                        listBox.DataSource = null;
                        listBox.Items.Clear();
                        TypeConverter tc = TypeDescriptor.GetConverter(valueType);
                        if (tc != null && tc.GetStandardValuesSupported())
                        {
                            ICollection collection = tc.GetStandardValues();
                            foreach (object item in collection)
                            {
                                listBox.Items.Add(item.ToString());
                            }

                            exclusive = tc.GetStandardValuesExclusive();
                        }
                    }
                    else
                    {
                        listBox.DataSource = null;
                        listBox.Items.Clear();
                    }
                }
            }
            else
            {
                listBox.DataSource = null;
                listBox.Items.Clear();
                if (style.ChoiceList != null)
                {
                    foreach (string item in style.ChoiceList)
                    {
                        listBox.Items.Add(item);
                    }
                }
            }
        }

        GridComboBoxListBoxHelper listBox;

        /// <internalonly/>
        private GridComboBoxListBoxHelper ListBox
        {
            get
            {
                if (listBox == null)
                {
                    listBox = new GridComboBoxListBoxHelper();
                }

                return listBox;
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns GridComboBoxListBoxHelper.</returns>
        /// <internalonly/>
        public GridComboBoxListBoxHelper GetInternalListBox()
        {
            return ListBox;
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            if (text.Length > 0 && (style.ChoiceList == null || style.ChoiceList.Count == 0))
            {
                if (style.DataSource != null && style.DisplayMember != style.ValueMember)
                {
                    bool exclusive;
                    this.FillWithChoices(ListBox, style, out exclusive);
                    ListBox.DisplayMember = style.DisplayMember;
                    ListBox.ValueMember = style.ValueMember;
                    if (string.IsNullOrEmpty(style.DisplayMember) && !string.IsNullOrEmpty(style.ValueMember))
                        listBox.DisplayMember = style.ValueMember;
                    else if (!string.IsNullOrEmpty(style.DisplayMember) && string.IsNullOrEmpty(style.ValueMember) && style.CellValueType == typeof(string))
                        listBox.ValueMember = style.DisplayMember;                   
                    ListBox.DataSource = style.DataSource;
                    int index = ListBox.FindStringExact(text);
                    if (index != -1)
                    {
                        ////listBox.SelectedIndex = index;
                        style.CellValue = ListBox.GetItemValue(index);
                        return true;
                    }

                    if (exclusive)
                    {
                        ////Throw new GridException(text + " is not found in choice list").
                        return false;
                    }
                }
            }

            return base.ApplyFormattedText(style, text, textInfo);
        }

        /// <summary>
        /// Returns the index in the drop-down list box for the specified cell value / key.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value (same as ValueMember).</param>
        /// <returns>The index in the drop-down list box or -1 if not found.</returns>
        public int FindValue(GridStyleInfo style, object value)
        {
            if (value == null || (value is string && value.Equals(string.Empty)))
            {
                return -1;
            }

            if (style.ChoiceList == null || style.ChoiceList.Count == 0)
            {
                bool exclusive;
                this.FillWithChoices(ListBox, style, out exclusive);
                if (ListBox.BindingContext != null && ListBox.DataSource != null)
                {
                    ////ListBox.SelectedIndex = -1;
                    ////ListBox.SelectedValue = value;
                    return ListBox.FindValue(value);
                }
            }
            else if (style.ChoiceList != null && value is string)
            {
                return style.ChoiceList.IndexOf((string)value);
            }

            return -1;
        }

        Hashtable notfound = new Hashtable();

        /// <summary>
        /// Returns the value for the ValueMember of the specified item.
        /// </summary>        
        /// <param name="dataSource">The datasource list</param>
        /// <param name="valueMember">The name of the value member</param>
        /// <param name="item">The row item.</param>
        /// <returns>The value of the ValueMember.</returns>
        public object GetItemValue(object dataSource, string valueMember, object item)
        {
            return ListUtil.GetItemValue(dataSource, valueMember, item);
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            //// Fix for defect 1331: Do not check for value == null. null can
            //// be a valid value with associated display text.

            if (style.ChoiceList == null || style.ChoiceList.Count == 0)
            {
                if (style.DataSource != null && (string.IsNullOrEmpty(style.ValueMember) || style.DisplayMember != style.ValueMember))
                {
                    bool exclusive;
                    this.FillWithChoices(ListBox, style, out exclusive);
                    TypeConverter tc = null;
                    tc = TypeDescriptor.GetConverter(TypeDescriptor.GetReflectionType(style.DataSource));
                    if (ListBox.BindingContext != null)
                    {
                        if (style.CellValueType == null && tc != null && (tc is EnumConverter || tc is CollectionConverter || tc is TypeConverter) && !(tc is ComponentConverter) && !(style.DataSource is ArrayConverter) && !(style.DataSource is ArrayList) && string.IsNullOrEmpty(style.DisplayMember) && string.IsNullOrEmpty(style.ValueMember))
                        {
                            TypeConverter converter = this.GetTypeConverter(style);
                            ICollection collection = converter.GetStandardValues();
                            if (collection != null && collection.Count > 0)
                            {
                                converter = style.PropertyDescriptor.Converter;
                                string s = string.Empty;
                                if (converter != null)
                                    s = converter.ConvertTo(value, typeof(string)).ToString();
                                return s;
                            }
                            else
                            {
                                ICollection collect = style.DataSource as ICollection;
                                TypeConverter convert = null;
                                if (collect != null && collect.Count > 0 && value!=null)
                                {
                                    TypeConverter.StandardValuesCollection coll = new TypeConverter.StandardValuesCollection(collect);
                                    convert = TypeDescriptor.GetConverter(TypeDescriptor.GetReflectionType(value));
                                    if (!convert.GetType().Name.Equals("TypeConverter"))
                                    {                                        
                                        string s = convert.ConvertTo(value, typeof(string)).ToString();
                                        return s; 
                                    }
                                    else
                                    {
                                        return ListBox.GetItemText(value);                                                                        
                                    }
                                }
                                if (style.DropDownStyle == GridDropDownStyle.Exclusive)
                                {
                                    exclusive = true;
                                }
                            }
                        }
                        else if ((style.CellValueType != null && style.CellValueType.IsEnum) && tc != null && (tc is ArrayConverter))
                        {
                            TypeConverter converter = this.GetTypeConverter(style);
                            ICollection collection = converter.GetStandardValues();
                            if (collection != null && collection.Count > 0)
                            {
                                converter = style.PropertyDescriptor.Converter;
                                string s = string.Empty;
                                if (converter != null)
                                    s = converter.ConvertTo(value, typeof(string)).ToString();
                                return s;
                            }
                        }
                        if (style.ValueMember == string.Empty)
                        {
                            if (value == null || (value is string && value.Equals(string.Empty)) || value.GetType().IsPrimitive)
                            {
                                if (exclusive)
                                {
                                    return string.Empty;
                                }
                            }
                            else
                            {
                                return ListBox.GetItemText(value);
                            }
                        }
                        else
                        {
                            int index = ListBox.FindValue(value);
                            if (index != -1)
                            {
                                return ListBox.GetItemText(index);
                            }
                            if ((value == null || (value is string && (value.Equals(string.Empty) || ListBox.FindString(value.ToString()) == -1)) || value.GetType().IsPrimitive || value.Equals(Guid.Empty)))
                            {
                                if (exclusive)
                                    return string.Empty;
                            }
                        }

                    }
                }
            }
            
            return base.GetFormattedText(style, value, textInfo);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or turn off selecting the next index when user double clicks inside cell.
        /// </summary>
        public bool AllowDoubleClickChangeSelectedIndex
        {
            get
            {
                return allowDoubleClickChangeSelectedIndex;
            }

            set
            {
                allowDoubleClickChangeSelectedIndex = value;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Obsolete("Problem with sluggish list boxes has been fixed. Set also GridStyleInfoStore.DataSourceProperty.IsCloneable and GridStyleInfoStore.DataSourceProperty.IsDisposable = false")]
        public bool CacheDataSource 
        { 
            get 
            { 
                return false; 
            } 
            
            set 
            { 
            }
        }
    }

    /// <internalonly/>
    /// <summary>For internal use.</summary>
    [ToolboxItem(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridComboBoxListBoxHelper : ListBox
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridComboBoxListBoxHelper()
            : base()
        {
        }

        /// <override/>
        protected override void OnDataSourceChanged(EventArgs e)
        {
            if (this.IsHandleCreated)
            {
                base.OnDataSourceChanged(e);
            }
        }

        /// <override/>
        protected override void OnCreateControl()
        {
            throw new Exception("GridComboBoxListBoxHelper is only usasble for filtering items, not for displaying them.");
            ////base.OnCreateControl ();
        }

        /// <override/>
        protected override void SetItemsCore(IList items)
        {
            if (this.IsHandleCreated)
            {
                base.SetItemsCore(items);
            }
        }

        /// <override/>
        protected override void SetItemCore(int index, object value)
        {
            if (this.IsHandleCreated)
            {
                base.SetItemCore(index, value);
            }
        }

        /// <override/>
        /// <summary>
        /// Gets or sets the zero-based index of the currently selected item in a list box.
        /// </summary>
        public override int SelectedIndex
        {
            get
            {
                if (this.IsHandleCreated)
                {
                    return base.SelectedIndex;
                }

                return 0;
            }

            set
            {
                if (this.IsHandleCreated)
                {
                    base.SelectedIndex = value;
                }
            }
        }

        object _FilterItemOnProperty(object item, string field)
        {
            if (item != null && field.Length > 0)
            {
                try
                {
                    PropertyDescriptor pd;
                    if (this.DataManager != null)
                    {
                        pd = this.DataManager.GetItemProperties().Find(field, true);
                    }
                    else
                    {
                        pd = TypeDescriptor.GetProperties(item).Find(field, true);
                    }

                    if (pd != null)
                    {
                        if (item.GetType().Name.Equals("String"))
                        {
                            item = pd.GetValue(this.DataManager.Current);
                        }
                        else
                        {
                            item = pd.GetValue(item);
                        }
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
            }

            return item;
        }

        /// <summary>
        /// Returns the text representation of the specified item.
        /// </summary>
        /// <param name="item">The object from which to get the contents to display.</param>
        /// <returns>
        /// If the <see cref="P:System.Windows.Forms.ListControl.DisplayMember"/> property is not specified, the value returned by <see cref="M:System.Windows.Forms.ListControl.GetItemText(System.Object)"/> is the value of the item's ToString method. Otherwise, the method returns the string value of the member specified in the <see cref="P:System.Windows.Forms.ListControl.DisplayMember"/> property for the object specified in the <paramref name="item"/> parameter.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public new string GetItemText(object item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            if (GridUtil.IsEmpty(this.DisplayMember))
            {
                return item.ToString();
            }

            object value = this._FilterItemOnProperty(item, this.DisplayMember);
            if (value != null)
                return value.ToString();
            return string.Empty;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="item">The object instance.</param>
        /// <returns>returns the ItemValue</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public object GetItemValue(object item)
        {
            if (GridUtil.IsEmpty(this.ValueMember))
            {
                return item;
            }

            object value = this._FilterItemOnProperty(item, this.ValueMember);
            return value;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>returns the ItemValue</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public object GetItemValue(int index)
        {
            object item = this.DataManager.List[index];
            object value = this._FilterItemOnProperty(item, this.ValueMember);
            return value;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>returns the ItemText</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public string GetItemText(int index)
        {
            object item = this.DataManager.List[index];
            return base.GetItemText(item);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns the ItemCount.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int GetItemCount()
        {
            return this.DataManager != null ? this.DataManager.List.Count : 0;
        }

        int FindKey(IList thisList, PropertyDescriptor property, object key)
        {
            //// Fix for defect 1331: Do not check for value == null. null can
            //// be a valid value with associated display text.
            ////if (key == null)
            ////    throw new ArgumentNullException("key");

            try
            {
                if (property != null
                    && (thisList is System.ComponentModel.IBindingList)
                    && ((IBindingList)thisList).SupportsSearching)
                {
                    return ((IBindingList)thisList).Find(property, key); //// == null ? DBNull.Value : key);
                }

                bool keyIsNull = key == null || key is DBNull;

                for (int n = 0; n < thisList.Count; n++)
                {
                    object obj = property != null ? property.GetValue(thisList[n]) : thisList[n];
                    bool objIsNull = obj == null || obj is DBNull;
                    if ((keyIsNull && objIsNull) || Object.ReferenceEquals(key, obj) || (!keyIsNull && key.Equals(obj)))
                    {
                        return n;
                    }
                }
            }
            catch (FormatException ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
            }
            catch (NullReferenceException ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
            }

            return -1;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>returns the value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int FindValue(object value)
        {
            if (this.DataManager != null)
            {
                PropertyDescriptorCollection pdc = this.DataManager.GetItemProperties();
                PropertyDescriptor pd = pdc.Find(this.ValueMember, true);

                if (pd == null)
                {
                    return DataManager.List.IndexOf(value);
                }

                if (pd.PropertyType == typeof(string) && (value is DBNull))
                {
                    // do nothing
                }
                else
                {
                    value = GridCellValueConvert.ChangeType(value, pd.PropertyType, System.Globalization.CultureInfo.CurrentCulture, true);
                }

                return FindKey(DataManager.List, pd, value);
            }

            return -1;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="s">The text to search for.</param>
        /// <returns>
        /// The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public new int FindStringExact(string s)
        {
            IList thisList = this.DataManager.List;
            PropertyDescriptorCollection pdc = this.DataManager.GetItemProperties();
            PropertyDescriptor property = pdc.Find(this.DisplayMember, true);

            object obj = null;

            try
            {
                for (int n = 0; n < thisList.Count; n++)
                {
                    obj = property != null ? property.GetValue(thisList[n]) : thisList[n];

                    string str = obj != null ? obj.ToString() : string.Empty;
                    if (string.Compare(s, str, true, System.Globalization.CultureInfo.CurrentCulture) == 0)
                    {
                        return n;
                    }
                }
            }
            catch (FormatException ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
            }

            return -1;
        }

        /// <override/>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            this.DataSource = null;
            base.OnHandleDestroyed(e);
        }

        BindingContext bindingContext;

        /// <override/>
        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Forms.BindingContext" /> for the
        /// control.
        /// </summary>
        public override BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                {
                    bindingContext = new BindingContext();
                }

                return bindingContext;
            }

            set
            {
            }
        }
    }
}
