#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Represents the base class for all combos with data binding support. Like
    /// <see cref="ComboBoxAdv"/> and <see cref="MultiColumnComboBox"/>.
    /// </summary>
    public abstract class ComboBoxBaseDataBound : ComboBoxBase
    {
        #region FIELDS
        private object dataSource = null;
        private CurrencyManager dataManager;
        private BindingMemberInfo displayMember;
        private BindingMemberInfo valueMember;
        private int maxDropDownItems = 8;
        private bool integralHeight = true;
        private bool sorted = false;
        private ObjectCollection itemsCollection;
        private bool internalItemsChange = false;
        private int oldSelectedIndex = -1;
        private bool m_bAllowNewText = true;
        internal bool isComboTextSet = false;
        private int sIndex = -1;
        #endregion FIELDS
        #region INIT
        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxBaseDataBound"/> class.
        /// </summary>
        /// <remarks>
        /// Initializing this ComboBoxBase also requires you to set the
        /// <see cref="Syncfusion.Windows.Forms.Tools.ComboBoxBase.ListControl"/> property.
        /// </remarks>
        public ComboBoxBaseDataBound() :
            base()
        {
            this.Init();
        }

        private void Init()
        {
            if (this.ListControl == null)
            {
                this.ListControl = this.CreateListControl();
                this.InitListControl(this.ListControl);
            }
        }

        /// <summary>
        ///  Releases all resources used by the control.
        /// </summary>
        /// <param name="disposing">bool disposing</param>
        /// <overload>
        /// Releases all resources used by the control.
        /// </overload>
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dataManager != null)
                {
                    this.dataManager.ItemChanged -= new ItemChangedEventHandler(this.DataManager_ItemChanged);
                    this.dataManager.PositionChanged -= new EventHandler(this.DataManager_PositionChanged);
                }

                if (this.dataSource is IComponent)
                    ((IComponent)this.dataSource).Disposed -= new EventHandler(this.DataSourceDisposed);

                this.dataManager = null;
                this.dataSource = null;

                if (null != this.itemsCollection)
                {
                    ((IDisposable)this.itemsCollection).Dispose();
                    this.itemsCollection = null;
                }
            }
            if(!this.DesignMode)
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates the list control.
        /// </summary>
        /// <returns>Returns list control</returns>
        protected abstract ListControl CreateListControl();
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void InitListControl(ListControl listControl)
        {
            this.UpdateListBoxBorderStyle();
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.HandleDestroyed"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.Disposing)
            {
                this.ListControl.SelectedIndex = -1;
            }
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.FontChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        protected override void OnFontChanged(EventArgs e)
        {
            if (this.ListControl != null)
                this.ListControl.Font = this.Font;

            base.OnFontChanged(e);
        }

        [Browsable(false)]
        public override ListControl ListControl
        {
            get
            {
                if (base.ListControl == null)
                {
                    this.ListControl = this.CreateListControl();
                    this.InitListControl(this.ListControl);
                }

                return base.ListControl;
            }
            set
            {
                if (base.ListControl != value)
                {
                    base.ListControl = value;
                    base.ListControl.Font = this.Font;
                }
            }
        }
        #endregion INIT
        #region LISTBOX_INTERFACE
        /// <summary>
        /// Gets or sets a value indicating whether the items in the combo box are sorted.
        /// </summary>
        [
        Description(@"Controls whether items in the list portion are sorted."),
        DefaultValue(false),
        SRCategory(@"Behavior")
        ]
        public bool Sorted
        {
            get 
            {
                return this.sorted; 
            }
            set
            {
                if (this.sorted != value)
                {
                    if (this.sorted != value)
                    {
                        if (this.DataSource != null && value)
                            throw new ArgumentException("ComboBox cannot be sorted with a DataSource.");
                        this.sorted = value;
                        this.RefreshItems();
                        this.SelectedIndex = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control should resize to avoid showing partial items.
        /// </summary>
        [
        SRCategory(@"Behavior"),
        Description(@"Indicates whether the list portion can contain only complete items."),
        Localizable(true),
        DefaultValue(true)
        ]
        public virtual bool IntegralHeight
        {
            get 
            { 
                return this.integralHeight; 
            }
            set
            {
                if (this.integralHeight != value)
                {
                    this.integralHeight = value;
                    this.SetNeedLayout(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of items to be shown
        /// in the drop-down portion of the control.
        /// </summary>
        [
        Localizable(true),
        Description(@"The maximum number of entries to display in the drop-down list."),
        SRCategory(@"Behavior"),
        DefaultValue(8 /*0x0008*/)
        ]
        public int MaxDropDownItems
        {
            get 
            { 
                return this.maxDropDownItems;
            }
            set
            {
                this.maxDropDownItems = value;
            }
        }

        /// <summary>
        ///   <para> Gets an object representing the collection of the items
        /// contained in this control.</para>
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRCategory(@"Data"),
        Description(@"The items in the combo box."),
        Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
            "System.Drawing.Design.UITypeEditor, System.Drawing"),
        Localizable(true)
        ]
        public virtual ComboBoxBaseDataBound.ObjectCollection Items
        {
            get
            {
                if (this.itemsCollection == null)
                    this.itemsCollection = new ComboBoxBaseDataBound.ObjectCollection(this);
                return this.itemsCollection;
            }
        }

        private bool m_bSuspendSelectedIndexRising = false;

        /// <summary>
        ///   <para>Gets or sets the data source for this <see cref="T:System.Windows.Forms.ListControl" /> object.</para>
        /// </summary>
        [
        Description(@"Indicates the list that this control will use to get its items."),
        SRCategory(@"Data"),
        DefaultValue(null),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design"),
        ]
        public object DataSource
        {
            get 
            { 
                return this.dataSource; 
            }
            set
            {
                if (value != null && value as IList == null)
                    if (value as IListSource == null)
                        throw new Exception("Bad DataSource for ComplexBinding while setting DataSource in ComboBoxBaseDataBound.");
                if (this.dataSource == value)
                    return;

                int prevSelectedIndex = this.SelectedIndex;

                try
                {
                    m_bSuspendSelectedIndexRising = true;

                    this.SetDataConnection(value, this.displayMember, false);
                    m_bSuspendSelectedIndexRising = false;
                }
                catch (Exception)
                {
                    this.DisplayMember = string.Empty;
                }
                if (value == null)
                    this.DisplayMember = string.Empty;
                if (prevSelectedIndex == -1)
                {
                    if (dataManager != null)
                        this.SelectedIndex = dataManager.Position;
                }

                if (prevSelectedIndex != this.SelectedIndex)
                {
                    this.OnSelectedItemChanged(EventArgs.Empty);
                    this.OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Gets or sets a string that specifies the property of the data source 
        /// whose contents you want to display.
        /// </summary>
        [
        SRCategory(@"Data"),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design",
            "System.Drawing.Design.UITypeEditor, System.Drawing"),
        DefaultValue(@""),
        TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
        Description(@"Indicates the property to display for the items in this control.")
        ]
        public string DisplayMember
        {
            get
            {
                return this.displayMember.BindingMember;
            }
            set
            {
                BindingMemberInfo displayMember0;

                displayMember0 = this.displayMember;
                try
                {
                    m_bSuspendSelectedIndexRising = true;

                    this.SetDataConnection(this.dataSource, new BindingMemberInfo(value), false);

                    m_bSuspendSelectedIndexRising = false;
                }
                catch (Exception)
                {
                    this.displayMember = displayMember0;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allowing NewText.
        /// </summary>
        [
        Description(@"Controls whether new items can be input in TextBox."),
        DefaultValue(true),
        SRCategory(@"Behavior")
        ]
        public bool AllowNewText
        {
            get
            {
                return m_bAllowNewText;
            }
            set
            {
                if (m_bAllowNewText != value)
                    m_bAllowNewText = value;
            }
        }

        /// <summary>
        ///   <para>Gets or sets the zero-based index of the currently selected item.</para>
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         Description("Gets or sets the zero-based index of the currently selected item.")]
        public virtual int SelectedIndex
        {
            get
            {
                if (this.ListControl != null)
                    return this.ListControl.SelectedIndex;
                return -1;
            }
            set
            {
                if (this.SelectedIndex != value)
                {                    
                        SelectedIndexChangingArgs e = new SelectedIndexChangingArgs(this.SelectedIndex, value);

                        this.OnSelectedIndexChanging(e);

                        if (!e.Cancel)
                        {
                            bool ignoreSelectedIndexRising = false;
                            if (this.PopupContainer != null && this.PopupContainer.IsShowing() &&
                                this.GetIndexFromPoint(MousePosition) != -1 && value == -1 &&
                                oldSelectedIndex == -1)
                                ignoreSelectedIndexRising = true;

                            if (this.ListControl != null)
                            {
                                this.ListControl.SelectedIndex = value;

                                if (this.DropDownStyle == ComboBoxStyle.DropDownList)
                                {
                                    this.TextBox.Text = ListControl.Text;
                                }
                            }

                            if (!m_bSuspendSelectedIndexRising && !ignoreSelectedIndexRising)
                            {
                                this.OnSelectedItemChanged(EventArgs.Empty);
                                this.OnSelectedIndexChanged(EventArgs.Empty);
                            }
                        }
                    }
                }
            }          

        /// <summary>
        ///   <para>Gets or sets the currently selected item in the ComboBox.</para>
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        Description(@"The currently selected item in the combo box, or null."),
        Bindable(true)
        ]
        public object SelectedItem
        {
            get
            {
                if (this.SelectedIndex != -1)
                    return this.Items[this.SelectedIndex];
                else
                    return null;
            }
            set
            {
                int index = -1;
                if (value != null)
                    index = this.Items.IndexOf(value);
                else
                    this.SelectedIndex = -1;
                if (index != -1)
                    this.SelectedIndex = index;
            }
        }

        /// <summary>
        ///   <para> Gets or sets the value of the member property
        /// specified by the <see cref="P:System.Windows.Forms.ListControl.ValueMember" /> property.</para>
        /// </summary>
        [
        Browsable(false),
        SRCategory(@"Data"),
        DefaultValue(null),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description(@"Indicates the actual value of the currently selected item.  Setting it will cause the item who's actual value is equal to become selected."),
        Bindable(true)
        ]
        public object SelectedValue
        {
            get
            {
                if (this.SelectedIndex != -1 && this.dataManager != null)
                {
                    object item = this.Items[this.SelectedIndex];
                    object filteredItem = this.FilterItemOnProperty(item, this.valueMember.BindingField);
                    return filteredItem;
                }
                return null;
            }
            set
            {
                string bindingField;
                PropertyDescriptorCollection pdc;
                PropertyDescriptor pd;
                int selectedIndex;

                if (this.dataManager != null)
                {
                    bindingField = this.valueMember.BindingField;
                    if (bindingField == String.Empty)
                        throw new Exception("Empty ValueMember encountered while setting SelectedValue property in ComboBoxBaseDataBound.");
                    pdc = this.dataManager.GetItemProperties();
                    pd = pdc.Find(bindingField, true);
                    selectedIndex = this.Find(this.dataManager, pd, value, true);
                    this.SelectedIndex = selectedIndex;

                    if (value is DBNull)
                    {
                        this.TextBox.Clear();
                    }
                }
            }
        }

        /// <summary>
        ///   <para>Gets / sets the text associated with this control.</para>
        /// </summary>
        [
        Bindable(true),
        Localizable(true)
        ]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                if (!this.DesignMode && this.IsHandleCreated)
                {
                    if (value == null)
                    {
                        this.SelectedIndex = -1;
                        return;
                    }

                    if (value != null &&
                        (this.SelectedItem == null || String.Compare(value, this.FilterItemOnProperty(this.SelectedIndex).ToString(), false, CultureInfo.CurrentCulture) != 0))
                    {
                        int index = FindStringExact(value, false);

                        if (index > 0)
                        {
                            this.SelectedIndex = index;
                            return;
                        }

                        index = FindStringExact(value, true);
                        if (index > 0)
                        {
                            this.SelectedIndex = index;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   <para> Gets or sets the text that is selected in the editable
        /// portion of the combo box.
        /// </para>
        /// </summary>
        [
        Description(@"The selected text in the edit component of the combo box."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public string SelectedText
        {
            get
            {
                if (this.DropDownStyle == ComboBoxStyle.DropDownList)
                    return String.Empty;

                return this.TextBox.SelectedText;
            }
            set
            {
                if (this.TextBox != null)
                    this.TextBox.SelectedText = value;
            }
        }

        /// <summary>
        ///   <para>Gets or sets the number of characters selected in the editable portion of the
        /// combo box.</para>
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description(@"The length of the selected text in the edit of the combo box."),
        Browsable(false)
        ]
        public int SelectionLength
        {
            get
            {
                if (this.DropDownStyle == ComboBoxStyle.DropDownList)
                    return 0;
                else
                    return this.TextBox.SelectionLength;
            }
            set
            {
                if (this.TextBox.Visible)
                    this.TextBox.SelectionLength = value;
            }
        }

        /// <summary>
        ///   <para>Gets or sets the starting index of text selected in the combo box.</para>
        /// </summary>
        [
        Browsable(false),
        SRDescription(@"The index of the first character in the selected text."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int SelectionStart
        {
            get
            {
                if (this.DropDownStyle == ComboBoxStyle.DropDownList)
                    return 0;
                else
                    return this.TextBox.SelectionStart;
            }
            set
            {
                if (this.TextBox.Visible)
                    this.TextBox.SelectionStart = value;
            }
        }
        internal int Find(CurrencyManager dataManager, PropertyDescriptor property, object key, bool keepIndex)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (property != null && this.dataManager.List as IBindingList != null && ((IBindingList)this.dataManager.List).SupportsSearching)
                return ((IBindingList)this.dataManager.List).Find(property, key);
            int index = 0;
            while (index < this.dataManager.List.Count)
            {
                object obj = property.GetValue(this.dataManager.List[index]);
                if (key.Equals(obj))
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        ///   <para>Gets or sets a string that specifies the property of the data source 
        /// from which to draw the value.</para>
        /// </summary>
        [
        Description(@"Indicates the property to use as the actual value for the items in the control."),
        SRCategory(@"Data"),
        DefaultValue(@""),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design",
            "System.Drawing.Design.UITypeEditor, System.Drawing"),
        ]
        public string ValueMember
        {
            get
            {
                return this.valueMember.BindingMember;
            }
            set
            {
                BindingMemberInfo newDisplayMember;

                if (value == null)
                    value = string.Empty;
                newDisplayMember = new BindingMemberInfo(value);
                if (!newDisplayMember.Equals(this.valueMember))
                {
                    if (this.DisplayMember.Length == 0)
                        this.SetDataConnection(this.DataSource, newDisplayMember, false);
                    if (this.dataManager != null && string.Empty != value && !this.BindingMemberInfoInDataManager(newDisplayMember))
                        throw new ArgumentException("Wrong ValueMember specified in ComboBoxBaseDataBound", "value");
                    this.valueMember = newDisplayMember;
                    this.OnValueMemberChanged(EventArgs.Empty);
                    this.OnSelectedValueChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///   <para> Adds the specified items to the combo box.</para>
        /// </summary>
        /// <param name="value">An array of <see cref="System.Object" /> to append to the combo box. </param>
        protected virtual void AddItemsCore(object[] value)
        {
            int n = (value != null) ? value.Length : 0;

            if (n == 0)
                return;

            this.Items.AddRange(value);
        }
        private void CheckNoDataSource()
        {
            if (!this.internalItemsChange && this.DataSource != null)
                throw new ArgumentException("DataSource lacks Items in ComboBoxBaseDataBound");
        }

        /// <overload>Finds the first item in the <see cref="ComboBoxBaseDataBound"/> that starts with the specified string.</overload>
        /// <summary>
        /// Finds the first item in the combo box that starts with the specified string.
        /// </summary>
        /// <param name="s">The string to search for.</param>
        /// <returns>The zero-based index of the first item found; -1 if no match is found.</returns>
        /// <remarks>The search performed by this method is not case-sensitive.
        /// The s parameter is a substring to compare against the text associated
        /// with the items in the combo box list. The search performs a partial
        /// match starting from the beginning of the text and returning the first
        /// item in the list that matches the specified substring. You can then
        /// perform tasks, such as removing the item that contains the search text
        /// using the <see cref="ComboBoxBaseDataBound.ObjectCollection.Remove"/> method
        /// or changing the item's text. Once you have
        /// found the specified text, if you want to search for other instances of
        /// the text in the ComboBoxBaseDataBound, you must use the version of the FindString
        /// method that provides a parameter for specifying a starting index
        /// within the ComboBox. If you want to perform a search for an exact word
        /// match instead of a partial match, use the <see cref="FindStringExact(string)"/> method.</remarks>
        public abstract int FindString(string s);

        /// <summary>
        /// Finds the first item after the given index which starts with the given string. The search is not case sensitive.
        /// </summary>
        /// <param name="s">The string to search for.</param>
        /// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to -1 to search from the beginning of the control.</param>
        /// <returns>The zero-based index of the first item found; -1 if no match is found.</returns>
        /// <remarks>The search performed by this method is not case-sensitive.
        /// The s parameter is a substring to compare against the text associated
        /// with the items in the combo box list. The search performs a partial
        /// match starting from the beginning of the text, returning the first
        /// item in the list that matches the specified substring. You can then
        /// perform tasks, such as removing the item that contains the search text
        /// using the <see cref="ComboBoxBaseDataBound.ObjectCollection.Remove"/>
        /// method or changing the item's text. This method is
        /// typically used after a call has been made using the version of this
        /// method that does not specify a starting index. Once an initial item has
        /// been found in the list, this method is typically used to find further
        /// instances of the search text by specifying the index position in the
        /// startIndex parameter of the item after the first found instance of the
        /// search text. If you want to perform a search for an exact word match
        /// instead of a partial match, use the <see cref="FindStringExact(string, int)"/> method.</remarks>
        public abstract int FindString(string s, int startIndex);

        /// <overload>Finds the item that exactly matches the specified string.</overload>
        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="s">The string to search for.</param>
        /// <returns>The zero-based index of the first item found; returns -1 if no match is found.</returns>
        /// <remarks>
        /// The search performed by this method is not case-sensitive.
        /// The s parameter is a string to compare against the text associated
        /// with the items in the combo box list. The search looks for a match
        /// starting from the beginning of the text, returning the first item in
        /// the list that matches the specified substring. You can then perform
        /// tasks, such as removing the item that contains the search text using
        /// the <see cref="ComboBoxBaseDataBound.ObjectCollection.Remove"/>
        /// method or changing the item's text. Once you have found the
        /// specified text, if you want to search for other instances of the text
        /// in the ComboBoxBaseDataBound, you must use the version of the FindStringExact method
        /// that provides a parameter for specifying a starting index within the
        /// ComboBox. If you want to perform partial word search instead of an
        /// exact word match, use the <see cref="FindString(string)"/> method.
        /// </remarks>
        public abstract int FindStringExact(string s);

        /// <summary>
        /// Finds the first item after the specified index that matches the specified string.
        /// </summary>
        /// <param name="s">The string to search for.</param>
        /// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to -1 to search from the beginning of the control.</param>
        /// <returns>The zero-based index of the first item found; returns -1 if no match is found.</returns>
        /// <remarks>
        /// The search performed by this method is not case-sensitive. The s
        /// parameter is a string to compare against the text associated with the
        /// items in the combo box list. The search looks for a match starting from
        /// the beginning of the text, returning the first item in the list that
        /// matches the specified substring. You can then perform tasks, such as
        /// removing the item that contains the search text using the <see cref="ComboBoxBaseDataBound.ObjectCollection.Remove"/>
        /// method or changing the item's text. This method is typically used after a call
        /// has been made using the version of this method that does not specify a
        /// starting index. Once an intial item has been found in the list, this
        /// method is typically used to find further instances of the search text
        /// by specifying the index position in the startIndex parameter of the
        /// item after the first found instance of the search text. If you want
        /// to perform partial word search instead of an exact word match, use the
        /// <see cref="FindString(string, int)"/> method.
        /// </remarks>
        public abstract int FindStringExact(string s, int startIndex);

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="text">Text to search.</param>
        /// <param name="ignoreCase">Indicates whether to ignore case during serach.</param>
        /// <returns>Index of specified text in list; -1, if nothing found.</returns>
        public abstract int FindStringExact(string text, bool ignoreCase);

        #endregion LISTBOX_INTERFACE
        #region UISTUFF

        /// <summary>
        /// Specifies the FlatStyle. (overridden property)
        /// </summary>
        public override ComboFlatStyle FlatStyle
        {
            get 
            {
                return base.FlatStyle;
            }
            set
            {
                if (base.FlatStyle != value)
                {
                    base.FlatStyle = value;
                    this.UpdateListBoxBorderStyle();
                }
            }
        }

        /// <summary>
        /// Raises the DropDownStyleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <override/>
        protected override void OnDropDownStyleChanged(EventArgs e)
        {
            this.UpdateListBoxBorderStyle();
            base.OnDropDownStyleChanged(e);
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void UpdateListBoxBorderStyle()
        {
        }

        /// <summary>
        /// Called to determine the height of textArea of this control.
        /// </summary>
        /// <param name="g">A <see cref="System.Drawing.Graphics"/> object.</param>
        /// <param name="textAreaHeight">A reference variable through which to return the height for the text area.</param>
        /// <remarks>
        /// <para>
        /// This method expects you to return a height for the text area through the reference variable,
        /// set the height of this control (if not in ComboBoxStyle.Simple mode) and the height
        /// of the drop-down button (<see cref="Syncfusion.Windows.Forms.Tools.ComboDropDown.DropDownButtonHeight"/>) based on the Font specified.
        /// </para>
        /// </remarks>
        protected override void DetermineHeightsBasedOnFont(Graphics g, ref int textAreaHeight)
        {
            base.DetermineHeightsBasedOnFont(g, ref textAreaHeight);
            if (this.IntegralHeight && this.DropDownStyle == ComboBoxStyle.Simple)
            {
                Rectangle childBounds = this.GetEmbeddedChildBounds();

                int originalHeight = this.Height;
                this.ListControl.Height = childBounds.Height;

                if (this.ListControl.Height != childBounds.Height)
                {
                    // ListControl will not expand, so shrink my height
                    int diff = childBounds.Height - this.ListControl.Height;
                    this.PreventHeightChange = false;
                    this.Height -= diff;
                }
            }
        }

        /// <summary>
        /// Returns height of NonClientArea of ListControl.
        /// </summary>
        /// <returns>Returns height of NonClientArea of ListControl</returns>
        protected virtual int GetListNCHeight()
        {
            return this.ListControl.Height - this.ListControl.ClientSize.Height;
        }
        #endregion UISTUFF
        #region DATABINDING_EVENTS

        /// <summary>
        ///   <para>Occurs when the <see cref="SelectedIndex" /> property has changed.</para>
        /// </summary>
        [
        SRCategory(@"Behavior"),
        Description(@"Occurs whenever the 'SelectedIndex' property for this control changes.")
        ]
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        ///   <para>Occurs when the <see cref="DataSource" /> changes.</para>
        /// </summary>
        [
        Description(@"Event fired when the value of DataSource property is changed."),
        SRCategory(@"Property Changed")
        ]
        public event EventHandler DataSourceChanged;

        /// <summary>
        ///   <para>Occurs when the <see cref="DisplayMember" /> property changes.</para>
        /// </summary>
        [
        Description(@"Event fired when the value of DisplayMember property is changed."),
        SRCategory(@"Property Changed")
        ]
        public event EventHandler DisplayMemberChanged;

        /// <summary>
        ///   <para>Occurs when the <see cref="ValueMember" /> property changes.</para>
        /// </summary>
        [
        Description(@"Event fired when the value of ValueMember property is changed."),
        SRCategory(@"Property Changed")
        ]
        public event EventHandler ValueMemberChanged;

        /// <summary>
        ///   <para>Occurs when the <see cref="SelectedValue" /> property changes.</para>
        /// </summary>
        [
        SRCategory(@"Behavior"),
        Description(@"Event fired when the value of SelectedValue property is changed.")
        ]
        public event EventHandler SelectedValueChanged;

        #endregion DATABINDING_EVENTS
        #region DATABINDING_STUFF
        protected override void OnListSelectedValueChangedWhileNotShowingPopup(int newIndex)
        {
            base.OnListSelectedValueChangedWhileNotShowingPopup(newIndex);
            this.OnSelectedIndexChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called when selection changed on popup close].
        /// </summary>
        protected override void OnSelectionChangedOnPopupClose()
        {
            base.OnSelectionChangedOnPopupClose();

            this.OnSelectedIndexChanged(EventArgs.Empty);
        }

        protected override void OnSelectionChangedByKey()
        {
            base.OnSelectionChangedByKey();

            this.OnSelectedIndexChanged(EventArgs.Empty);
        }
        protected override void OnUpdateSelectionBeforeValidate()
        {
            // The index before changing selection:
            int selIndex = this.ListControl.SelectedIndex;

            base.OnUpdateSelectionBeforeValidate();

            if (selIndex != this.ListControl.SelectedIndex)
            {
                // Selection has changed, fire event:
                this.OnSelectedIndexChanged(EventArgs.Empty);
            }
        }

        protected override bool OnNewListItemKeyedIn(int indexFound)
        {
            if (this.SelectedIndex != indexFound)
                this.SelectedIndex = indexFound;

            return true;
        }

        /// <summary>
        /// Raises the text box KeyUp event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks > Keys.Delete and Keys.Back are not caught in Keypress </remarks>
        protected override void OnTextBoxKeyUp(KeyEventArgs e)
        {
            // Don't call the base class. We do not want the list control selection to get updated.

            // Keys that we didn't catch in KeyPress.
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                if (this.PopupControl != null && this.DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    this.ResetList();
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (fireTextChangedEvent)
            {
            IList iList = this.GetListControlList();

            if (this.ListControl != null)
            {
                int itemIndex = 0;

                if (this.CharacterCasing == CharacterCasing.Normal)
                {
                    itemIndex = this.FindItem(this.TextBox.Text, false, -1, false);
                }
                else
                {
                    itemIndex = this.FindItem(this.TextBox.Text, false, -1, true);
                }

                if (itemIndex != -1)
                {
                    for (int i = itemIndex; i < iList.Count; i++)
                    {
                        string itemText = this.ListControl.GetItemText(iList[i]);

                        if (itemText.Length > this.TextBox.Text.Length)
                        {
                            itemIndex = -1;
                            continue;
                        }

                        if (this.CharacterCasing == CharacterCasing.Upper)
                            itemText = itemText.ToUpper();
                        else if (this.CharacterCasing == CharacterCasing.Lower)
                            itemText = itemText.ToLower();

                        if (itemText == this.TextBox.Text)
                        {
                            if (i == this.ListControl.SelectedIndex)
                            {
                                itemIndex = i;
                                break;
                            }
                        }
                    }
                }

                if (this.IsValidIndex(itemIndex) && this.IsValidIndex(m_lastFoundIndex) &&
                    itemIndex != m_lastFoundIndex &&
                    this.ListControl.GetItemText(iList[itemIndex]) == this.ListControl.GetItemText(iList[m_lastFoundIndex]))
                {
                    itemIndex = m_lastFoundIndex;
                }

                if (itemIndex != -1)
                {
                    this.SelectedIndex = itemIndex;
                }
                else if (this.AllowNewText && this.DropDownStyle!=ComboBoxStyle.DropDownList)
                {
                    bool oldValue = base.IgnorePopupValueChange;
                    IgnorePopupValueChange = true;
                    this.SelectedIndex = itemIndex;
                    base.IgnorePopupValueChange = oldValue;
                }
            }

            }
            base.OnTextChanged(e);
        }

        /// <summary>
        /// Raises KeyPress event and  resets the list control if the popup is not showing.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            this.autoCompleteSuccess = false;

            base.OnKeyPress(e);

            if (this.PopupControl != null && this.DropDownStyle != ComboBoxStyle.DropDownList

                // Reset only if the popup is not showing
                && !this.PopupContainer.IsShowing() && !this.autoCompleteSuccess)
            {
                this.ResetList();
            }
        }
        private void ResetList()
        {
            bool oldValue = this.IgnorePopupValueChange;
            this.IgnorePopupValueChange = true;

            // Any change should reset the list control.
            this.SetPopupText(String.Empty);
            this.IgnorePopupValueChange = oldValue;
        }

        /// <summary>
        /// Verifies whether new text is allowed to be entered from native message.
        /// </summary>
        /// <param name="m">The message.</param>
        /// <returns>True if the message is handled.</returns>
        public override bool PerformAllowNewText(Message m)
        {
            if (this.AllowNewText)
                return false;

            string strSet = null;

            string strAfterDel = null;
            int deleteStart = this.TextBox.SelectionStart;
            int deleteLength = this.TextBox.SelectionLength;

            if (deleteLength != 0)
                strAfterDel = this.TextBox.Text.Substring(0, deleteStart) +
                    this.TextBox.Text.Substring(deleteStart + deleteLength, this.TextBox.Text.Length - (deleteStart + deleteLength));

            if (m.Msg == NativeMethods.WM_KEYDOWN)
            {
                if ((Keys)m.WParam.ToInt32() == Keys.Delete)
                {
                    if (deleteLength == 0 && (deleteStart != this.TextBox.Text.Length))
                        strAfterDel = this.TextBox.Text.Remove(deleteStart, 1);

                    if (this.PerformShowPopUpAfterDel(strAfterDel))
                        return true;
                }
            }

            if (m.Msg == NativeMethods.WM_CHAR)
            {
                char inputChar = (char)m.WParam;

                if (Char.IsLetterOrDigit(inputChar) || Char.IsSymbol(inputChar) ||
                    Char.IsSeparator(inputChar) || Char.IsPunctuation(inputChar))
                {
                    strSet = inputChar.ToString();
                }
                else
                {
                    if ((Keys)m.WParam.ToInt32() == Keys.Back)
                    {
                        if (deleteLength == 0 && deleteStart >= 1)
                            strAfterDel = this.TextBox.Text.Remove(deleteStart - 1, 1);

                        if (this.PerformShowPopUpAfterDel(strAfterDel))
                            return true;
                    }
                }
            }
            else if (m.Msg == NativeMethods.WM_PASTE)
            {
                IDataObject iData = Clipboard.GetDataObject();

                if (iData.GetDataPresent(DataFormats.Text))
                    strSet = (string)iData.GetData(DataFormats.Text);
            }
            else if (m.Msg == NativeMethods.WM_CUT)
            {
                if (this.PerformShowPopUpAfterDel(strAfterDel))
                {
                    NativeMethods.SendMessage(this.TextBox.Handle, 769/*WM_COPY*/, 0, 0);
                    return true;
                }
            }

            if (strSet == null || strSet == String.Empty)
                return false;

            bool equalDisplayMember = this.DisplayMember == this.ListControl.DisplayMember;

            if (equalDisplayMember)
            {
                int selectionStart = this.TextBox.SelectionStart;
                int selectionLength = this.TextBox.SelectionLength;

                string strToSearch = this.TextBox.Text.Substring(0, selectionStart) + strSet +
                    this.TextBox.Text.Substring(selectionStart + selectionLength, this.TextBox.Text.Length - (selectionStart + selectionLength));

                bool bIgnoreCase = this.CharacterCasing != CharacterCasing.Normal || !this.CaseSensitiveAutocomplete;

                int index = this.FindItem(strToSearch, false, -1, bIgnoreCase);

                if (index == -1)
                {
                    if (!this.PopupContainer.IsShowing())
                        this.ShowPopup();

                    return true;
                }
            }

            return false;
        }

        private bool PerformShowPopUpAfterDel(string strAfterDel)
        {
            if (this.TextBox.SelectionStart != this.TextBox.Text.Length)
            {
                bool isUpperOrLower = !(this.CharacterCasing == CharacterCasing.Normal);
                if (strAfterDel != string.Empty && this.FindItem(strAfterDel, false, -1, isUpperOrLower) == -1)
                {
                    if (!this.PopupContainer.IsShowing())
                        this.ShowPopup();

                    return true;
                }

                if (this.PopupContainer.IsShowing())
                    this.HidePopup();
            }
            else
            {
                if (this.PopupContainer.IsShowing())
                    this.HidePopup();
            }

            return false;
        }

        /// <summary>
        /// Called when the popup is closed.
        /// </summary>
        protected override void OnPopupClosed(PopupClosedEventArgs e)
        {
            if(this.ListControl.SelectedIndex == -1 && this.Text != string.Empty)
                this.ListControl.SelectedIndex = this.FindStringExact(this.Text, -1);
            base.OnPopupClosed(e);
        }	  

        /// <summary>
        /// Sets the popup text.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void SetPopupText(string value)
        {
            if (this.ListControl == null)
                return;

            if (value == null) 
            {
                this.ListControl.SelectedIndex = -1;
            }
            else
            {
                // If the current current selection is set to this value, then leave it there.
                if (this.GetPopupText().CompareTo(value) == 0)
                    return;
                else
                {
                    // Specify StartIndex as (selected index - 1) to start searching at the SelectedIndex OR -1
                    int startIndex = this.ListControl.SelectedIndex - 1;
                    if (startIndex < -1)
                        startIndex = -1;
                    int index = this.FindItem(value, false, startIndex, false);
                    this.SelectedIndex = index;
                    this.ListControl.SelectedIndex = this.FindStringExact(value, startIndex);
                }
            }
        }

        /// <summary>
        /// Gets the popup text.
        /// </summary>
        /// <returns>Returns popup text</returns>
        protected override string GetPopupText()
        {
            object selItem = this.SelectedItem;
            if (selItem != null)
                return this.GetItemText(selItem);
            else
                return String.Empty;
        }

        /// <summary>
        /// Sets the selected text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="foundIndex">Index of the found item.</param>
        protected override void SetSelectedText(string text, int foundIndex)
        {
            base.SetSelectedText(text, foundIndex);

            if (foundIndex >= 0)
            {
                this.SelectedIndex = foundIndex;
            }
        }

        /// <summary>
        ///   <para>Gets the <see cref="System.Windows.Forms.CurrencyManager" /> object associated with this control.</para>
        /// </summary>
        protected CurrencyManager DataManager
        {
            get
            {
                return this.dataManager;
            }
        }
        internal bool BindingFieldEmpty
        {
            get
            {
                if (this.displayMember.BindingField.Length <= 0)
                    return true;
                return false;
            }
        }

        private bool BindingMemberInfoInDataManager(BindingMemberInfo bindingMemberInfo)
        {
            PropertyDescriptorCollection pdc;
            int count;
            bool retValue;
            int index;

            if (this.dataManager == null)
                return false;
            pdc = this.dataManager.GetItemProperties();
            count = pdc.Count;
            retValue = false;
            index = 0;
            while (index < count)
            {
                if (!typeof(IList).IsAssignableFrom(pdc[index].PropertyType) && pdc[index].Name == bindingMemberInfo.BindingField)
                {
                    retValue = true;
                    break;
                }
                index++;
            }
            return retValue;
        }
        private void DataManager_ItemChanged(object sender, ItemChangedEventArgs e)
        {
            this.OnDataManagerItemChanged(e.Index);
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        private void OnDataManagerItemChanged(int index)
        {
            if (this.dataManager != null)
            {
                if (index == -1)
                {
                    this.SetItemsCore(this.dataManager.List);
                    return;
                }
                this.SetItemCore(index, this.dataManager.List[index]);
            }
        }
        private void DataManager_PositionChanged(object sender, EventArgs e)
        {
            this.OnDataManagerPositionChanged();
        }
        [Syncfusion.Documentation.DocumentationExclude()]

        // Update the SelectedIndex to the datamanager's position after checking the count.
        protected abstract void OnDataManagerPositionChanged();
        private void DataSourceDisposed(object sender, EventArgs e)
        {
            this.SetDataConnection(null, new BindingMemberInfo(string.Empty), true);
        }

        /// <summary>
        /// Filters the item .
        /// </summary>
        /// <param name="item">The item that is usually an entry in the list</param>
        /// <returns>Returns an object of Filter Item</returns>
        protected object FilterItemOnProperty(object item)
        {
            return this.FilterItemOnProperty(item, this.displayMember.BindingField);
        }
        protected object FilterItemOnProperty(object item, string field)
        {
            PropertyDescriptor pd;

            if (item != null && field.Length > 0)
            {
                try
                {
                    if (this.dataManager != null)
                        pd = this.dataManager.GetItemProperties().Find(field, true);
                    else
                        pd = TypeDescriptor.GetProperties(item).Find(field, true);
                    if (pd != null)
                        item = pd.GetValue(item);
                }
                catch (Exception) 
                { 
                }
            }
            return item;
        }
        internal int FindStringInternal(string str, IList items, int startIndex, bool exact)
        {
            bool retVal;
            int length;

            if (str == null || items == null)
                return -1;
            if (startIndex < -1 || startIndex >= items.Count - 1)
                return -1;
            retVal = false;
            length = str.Length;

            do
            {
                startIndex++;
                if (exact)
                {
                    retVal = String.Compare(str, this.GetItemText(items[startIndex]), true, CultureInfo.CurrentCulture) == 0;
                }
                else
                {
                    retVal = String.Compare(str, 0, this.GetItemText(items[startIndex]), 0, length, true, CultureInfo.CurrentCulture) == 0;
                }
                if (retVal)
                {
                    return startIndex;
                }
                if (startIndex != items.Count - 1)
                {
                    continue;
                }
                startIndex = -1;
            } 
            while (false);

            return -1;
        }

        /// <summary>
        /// Returns the text associated with an item.
        /// </summary>
        /// <param name="item">The item that is usually an entry in the list.</param>
        /// <returns>The item's text.</returns>
        public string GetItemText(object item)
        {
            item = this.FilterItemOnProperty(item, this.displayMember.BindingField);

            if (item == null)
                return string.Empty;

            TypeConverter tc = null;
            Type type = TypeDescriptor.GetReflectionType(item);
            if (this.DataSource != null)
            {
                while (type.HasElementType)
                {
                    type = type.GetElementType();
                }

                tc = TypeDescriptor.GetConverter(type);
            }

            if(tc!=null)
                return tc.ConvertTo(item, typeof(String)).ToString();
            else
                return Convert.ToString(item, CultureInfo.CurrentCulture);
        }
        
        // TODO: Handle PageDown etc. in IsInputKeys?
        // TODO: Should implement OwnerDraw?
        // TODO: Add Select, SelectAll method. IntegralHeight, ItemHeight?
        protected override void OnBindingContextChanged(EventArgs e)
        {
            if (!m_bSuspendSelectedIndexRising)
                m_bSuspendSelectedIndexRising = true;

           
                sIndex = this.SelectedIndex;
            
            this.SetDataConnection(this.dataSource, this.displayMember, true);

            if (!this.initializing && sIndex != -1)
            {
                this.SelectedIndex = sIndex;
                IList list = this.GetListControlList();
                this.TextBox.Text = this.ListControl.GetItemText(list[this.SelectedIndex]);
            }

            m_bSuspendSelectedIndexRising = false;

            base.OnBindingContextChanged(e);
        }

        /// <summary>
        /// Raises the DataSourceChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDataSourceChanged(EventArgs e)
        {
            if (this.Sorted && this.DataSource != null && this.Created)
                throw new Exception("Cannot set ComboBoxBaseDataBound's DataSource when Sorted is true.");

            if (this.DataSourceChanged != null)
                this.DataSourceChanged(this, e);

            // Clear items if datasource is null:
            if (this.DataSource == null)
            {
                this.SelectedIndex = -1;
                this.Items.ClearInternal();
            }

            if (this.DataSource == null || this.SelectedIndex == -1)
                UpdateText(false);

            // this.RefreshItems();
        }

        /// <summary>
        /// Raises the DisplayMemberChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDisplayMemberChanged(EventArgs e)
        {
            if (this.DisplayMemberChanged != null)
                this.DisplayMemberChanged(this, e);
        }
        private bool itemSelected = false;
        /// <summary>
        /// Raises the SelectedIndexChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            itemSelected = true;
            this.OnSelectedValueChanged(e);

            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(this, e);

            if (this.DataManager != null && this.DataManager.Position != this.SelectedIndex && this.SelectedIndex != -1)
                this.DataManager.Position = this.SelectedIndex;

            if (this.ListControl != null)
                oldSelectedIndex = this.ListControl.SelectedIndex;
            if (this.SelectedIndex == -1 && this.Text == string.Empty)
                itemSelected = false;
        }

        /// <summary>
        /// Raises the SelectedValueChanged event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedValueChanged(EventArgs e)
        {
            if (this.SelectedValueChanged != null)
                this.SelectedValueChanged(this, e);
        }
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the ValueMemberChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnValueMemberChanged(EventArgs e)
        {
            if (this.ValueMemberChanged != null)
                this.ValueMemberChanged(this, e);
        }

        /// <summary>
        /// Raises the validating event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            this.TextBox.SelectAll();
        }

        private bool fireTextChangedEvent= true;
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            this.TextBox.Select(0, 0);

            if (!this.AllowNewText && this.Items.Count > 0 && itemSelected && this.FindStringExact(this.TextBox.Text) == -1)
            {
                this.Focus();

                if (!this.PopupContainer.IsShowing())
                    this.ShowPopup();
            }
        }

        private void SetDataConnection(object newDataSource, BindingMemberInfo newDisplayMember, bool force)
        {
            CurrencyManager dataManager;

            bool isNewDataSource = !(this.dataSource == newDataSource);
            bool isNewDisplayMember = !this.displayMember.Equals(newDisplayMember);

            if (force || isNewDataSource || isNewDisplayMember)
            {
                if (this.dataSource as IComponent != null)
                    ((IComponent)this.dataSource).Disposed -= new EventHandler(this.DataSourceDisposed);

                this.dataSource = newDataSource;
                this.displayMember = newDisplayMember;

                if (this.dataSource as IComponent != null)
                    ((IComponent)this.dataSource).Disposed += new EventHandler(this.DataSourceDisposed);

                dataManager = null;
                if (newDataSource != null && this.BindingContext != null && newDataSource != Convert.DBNull)
                    dataManager = (CurrencyManager)this.BindingContext[newDataSource, newDisplayMember.BindingPath];
                if (this.dataManager != dataManager)
                {
                    if (this.dataManager != null)
                    {
                        this.dataManager.ItemChanged -= new ItemChangedEventHandler(this.DataManager_ItemChanged);
                        this.dataManager.PositionChanged -= new EventHandler(this.DataManager_PositionChanged);
                    }
                    this.dataManager = dataManager;
                    if (this.dataManager != null)
                    {
                        this.dataManager.ItemChanged += new ItemChangedEventHandler(this.DataManager_ItemChanged);
                        this.dataManager.PositionChanged += new EventHandler(this.DataManager_PositionChanged);
                    }
                }
                if (this.dataManager != null && (isNewDisplayMember || isNewDataSource) && string.Empty != this.displayMember.BindingMember && !this.BindingMemberInfoInDataManager(this.displayMember))
                   
                    throw new ArgumentException("Wrong DisplayMember specified in ComboBoxBaseDataBound.", "newDisplayMember");

                if (this.dataManager != null && (isNewDataSource || isNewDisplayMember || force))
                    this.OnDataManagerItemChanged(-1);
            }
            if (isNewDataSource)
                this.OnDataSourceChanged(EventArgs.Empty);
            if (this.DroppedDown)
            {
                this.DroppedDown = false;
                this.ShowPopup();
            }
            if (isNewDisplayMember)
                this.OnDisplayMemberChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Sets the internal items. 
        /// </summary>
        /// <param name="index">The index of item.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetItemCore(int index, object value)
        {
            this.internalItemsChange = true;
            this.Items[index] = value;
            this.internalItemsChange = false;
        }
        protected virtual void SetItemsCore(IList items)
        {
            this.internalItemsChange = true;
            (this as ISupportInitialize).BeginInit();
            int index = this.SelectedIndex;
            this.Items.ClearInternal();
            this.Items.AddRangeInternal(items);
            (this as ISupportInitialize).EndInit();
            this.internalItemsChange = false;
            if (!this.isComboTextSet && !this.PopupContainer.IsShowing())
            {
                this.SelectedIndex = index;
            }
            else if (!this.isComboTextSet && this.PopupContainer.IsShowing())
            {
                this.SelectedIndex = this.dataManager.Position;
            }
        }

        /// <summary>
        /// Indicates whether the Text property should be serialized in the designer.
        /// </summary>
        /// <returns>Return true if string is not empty</returns>
        public virtual bool ShouldSerializeText()
        {
            return this.Text != String.Empty;
        }
        public override /*Component*/ string ToString()
        {
            string str = base.ToString();
            int count = this.Items.Count;

            return string.Concat(str, ", Items.Count: ", count.ToString());
        }
        private void RefreshItems()
        {
            object[] ocarray;

            int oldSelIndex = this.SelectedIndex;
            ObjectCollection oldC = this.itemsCollection;
            this.itemsCollection = null;
            this.NativeClear();
            ocarray = null;
            if (this.DataManager != null && this.DataManager.Count != -1)
            {
                ocarray = new object[checked((uint)this.DataManager.Count)];
                int index = 0;
                while (index < (int)ocarray.Length)
                {
                    ocarray[index] = this.DataManager.List[index];
                    index++;
                }
            }
            else if (oldC != null)
            {
                ocarray = new object[oldC.Count];
                oldC.CopyTo(ocarray, 0);
            }
            if (ocarray != null)
            {
                this.Items.AddRangeInternal(ocarray);
            }
            if (this.DataManager != null)
            {
                this.SelectedIndex = this.DataManager.Position;
                return;
            }
            this.SelectedIndex = oldSelIndex;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void NativeAdd(object item)
        {
            IList list = this.GetListControlList();
            if (list == null)
                return;

            list.Add(this.GetItemText(item));
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void NativeClear()
        {
            IList list = this.GetListControlList();
            if (list == null)
                return;

            string oldText = String.Empty;

            oldText = null;
            if (this.DropDownStyle != ComboBoxStyle.DropDownList)
                oldText = this.Text;

            list.Clear();

            if (oldText != String.Empty)
                this.Text = oldText;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void NativeInsert(int index, object item)
        {
            IList list = this.GetListControlList();
            if (list == null)
                return;

            list.Insert(index, this.GetItemText(item));
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void NativeRemoveAt(int index)
        {
            IList list = this.GetListControlList();
            if (list == null)
                return;

            list.RemoveAt(index);
        }

        /// <summary>
        /// Overriden. Indicates whether selected index of the ListControl has been changed.
        /// </summary>
        /// <returns>TRUE if index has been changed; FALSE otherwise.</returns>
        protected override bool OtherChangesMade()
        {
            bool fromParent = base.OtherChangesMade();
            bool indexChanged = oldSelectedIndex != this.ListControl.SelectedIndex;
            return fromParent || indexChanged;
        }
        #endregion DATABINDING_STUFF
        private sealed class ItemComparer : IComparer
        {
            // Fields
            private ComboBoxBaseDataBound comboBox;

            // Constructors
            public ItemComparer(ComboBoxBaseDataBound comboBox)
            {
                this.comboBox = comboBox;
            }

            // Methods
            public /*IComparer*/ int Compare(object item1, object item2)
            {
                CompareInfo ci;

                if (item1 == null)
                {
                    if (item2 == null)
                        return 0;
                    return -1;
                }
                if (item2 == null)
                    return 1;
                string item1Text = this.comboBox.GetItemText(item1);
                string item2Text = this.comboBox.GetItemText(item2);
                ci = Application.CurrentCulture.CompareInfo;
                return ci.Compare(item1Text, item2Text, CompareOptions.StringSort);
            }
        }

        public enum CollectionChangeOperation
        {
            /// <summary>
            /// Represents RemoveAt
            /// </summary>
            RemoveAt,

            /// <summary>
            /// Represents Remove
            /// </summary>
            Remove,

            /// <summary>
            /// Represents Insert
            /// </summary>
            Insert,

            /// <summary>
            /// Represents Clear
            /// </summary>
            Clear,

            /// <summary>
            /// Represents SetItem
            /// </summary>
            SetItem,

            /// <summary>
            /// Represents Add
            /// </summary>
            Add,

            /// <summary>
            /// Represents AddRange
            /// </summary>
            AddRange
        }

        internal class CollectionChangeEventArgs : EventArgs
        {
            private CollectionChangeOperation m_operation;
            private int m_index;
            private object m_item = null;

            public CollectionChangeEventArgs(CollectionChangeOperation operation, int index, object item)      
            {
                this.m_operation = operation;
                this.m_index = index;
                this.m_item = item;
            }

            public CollectionChangeOperation Operation
            {
                get
                {
                    return this.m_operation;
                }
            }

            public int Index
            {
                get
                {
                    return this.m_index;
                }
            }

            public object Item
            {
                get
                {
                    return this.m_item;
                }
            }
        }

        internal delegate void CollectionChangeEventHandler(object sender, CollectionChangeEventArgs args);

        [
            ListBindable(false)
        ]
        public class ObjectCollection :
            IList,
            IDisposable
        {
            // Fields
            private ComboBoxBaseDataBound owner;
            private ArrayList innerList;
            private IComparer comparer;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectCollection"/> class.
            /// </summary>
            /// <param name="owner">The <see cref="Syncfusion.Windows.Forms.Tools.ComboBoxBaseDataBound" /> that owns this object collection.</param>
            public ObjectCollection(ComboBoxBaseDataBound owner)
            {
                this.owner = owner;
            }

            internal event CollectionChangeEventHandler CollectionChange;

            protected void OnCollectionChange(CollectionChangeOperation operation, int index, object item)
            {
                if (CollectionChange != null)
                {
                    CollectionChange(this, new CollectionChangeEventArgs(operation, index, item));
                }
            }

            /// <summary>
            ///   <para>Returns an enumerator that can be used to iterate through the item
            /// collection.</para>
            /// </summary>
            /// <returns>
            ///   <para>An <see cref="System.Collections.IEnumerator" /> object that represents the item
            /// collection.</para>
            /// </returns>
            public virtual /*IEnumerable*/ IEnumerator GetEnumerator()
            {
                return this.InnerList.GetEnumerator();
            }

            /// <summary>
            ///   <para>Gets the number of items in the collection.</para>
            /// </summary>
            public virtual /*ICollection*/ int Count
            {
                get
                {
                    return this.InnerList.Count;
                }
            }

            public virtual void RemoveAt(int index)
            {
                object[] args;

                this.owner.CheckNoDataSource();
                if (index < 0 || index >= this.InnerList.Count)
                {
                    args = new object[2];
                    args[0] = "index";
                    args[1] = index.ToString();
                    throw new ArgumentOutOfRangeException(SR.GetString("InvalidArgument", args));
                }
                this.owner.NativeRemoveAt(index);
                this.InnerList.RemoveAt(index);
                this.owner.UpdateText(true);

                this.OnCollectionChange(CollectionChangeOperation.RemoveAt, index, null);
            }

            /// <summary>
            ///   <para>Removes the specified item from the <see cref="ComboBoxBaseDataBound" />.</para>
            /// </summary>
            /// <param name="value">The <see cref="System.Object" /> to remove from the list. </param>
            public virtual void Remove(object value)
            {
                int index;

                index = this.InnerList.IndexOf(value);
                if (index != -1)
                {
                    this.RemoveAt(index);

                    this.OnCollectionChange(CollectionChangeOperation.Remove, index, value);
                }
            }

            /// <summary>
            ///   <para>Inserts an item into the collection at the specified index.</para>
            /// </summary>
            /// <param name="index">The zero-based index location where the item is inserted.</param>
            /// <param name="item">An object representing the item to insert.</param>
            public virtual void Insert(int index, object item)
            {
                bool inserted;
                object[] args;

                this.owner.CheckNoDataSource();
                if (item == null)
                    throw new ArgumentNullException("item");
                if (index < 0 || index > this.InnerList.Count)
                {
                    args = new object[2];
                    args[0] = "index";
                    args[1] = index.ToString();
                    throw new ArgumentOutOfRangeException(SR.GetString("InvalidArgument", args));
                }
                if (this.owner.sorted)
                {
                    this.Add(item);
                    return;
                }
                this.InnerList.Insert(index, item);
                inserted = false;
                try
                {
                    this.owner.NativeInsert(index, item);
                    inserted = true;
                }
                finally
                {
                    if (!inserted)
                        this.InnerList.RemoveAt(index);
                }

                this.OnCollectionChange(CollectionChangeOperation.Insert, index, item);
            }
            public virtual int IndexOf(object value)
            {
                object[] args;

                if (value == null)
                {
                    args = new object[2];
                    args[0] = "value";
                    args[1] = "null";
                    throw new ArgumentNullException(SR.GetString("InvalidArgument", args));
                }
                return this.InnerList.IndexOf(value);
            }

            /// <summary>
            ///   <para> Gets a value indicating whether this collection can be modified.</para>
            /// </summary>
            public virtual /*IList*/ bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///   <para>Removes all items from the <see cref="ComboBoxBaseDataBound" />.</para>
            /// </summary>
            public virtual /*IList*/ void Clear()
            {
                this.owner.CheckNoDataSource();
                this.InnerList.Clear();
                this.owner.TextBox.Text = string.Empty;
                this.owner.NativeClear();
            }

            /// <summary>
            ///   <para>Indicates whether the specified item is located within the collection.</para>
            /// </summary>
            /// <param name="value">An object representing the item to locate in the collection.</param>
            /// <returns>
            ///   <para>
            ///     <see langword="true" /> if the item is located within the collection;
            /// <see langword="false" /> otherwise.</para>
            /// </returns>
            public virtual /*IList*/ bool Contains(object value)
            {
                return this.IndexOf(value) != -1;
            }

            /// <summary>
            /// Gets / sets the item at the specified index. This is the Indexer property.
            /// </summary>
            /// <param name="index">Index value of the item.</param>
            /// <returns>Item at the specified index.</returns>
            [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            Browsable(false)
            ]
            public virtual /*IList*/ object this[int index]
            {       
                get
                {
                    if (this.InnerList.Count == 0)
                    {
                        return null;
                    }

                    if (index < 0 || index >= this.InnerList.Count)
                    {
                        object[] args = new object[2];
                        args[0] = "index";
                        args[1] = index.ToString();
                        throw new ArgumentOutOfRangeException(SR.GetString("InvalidArgument", args));
                    }

                    return this.InnerList[index];
                }
                set
                {
                    this.owner.CheckNoDataSource();
                    this.SetItemInternal(index, value);
                }
            }

            void System.Collections.ICollection.CopyTo(Array dest, int index)
            {
                this.InnerList.CopyTo(dest, index);
            }

            int System.Collections.IList.Add(object item)
            {
                return this.Add(item);
            }

            bool System.Collections.IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            bool System.Collections.ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object System.Collections.ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            private IComparer Comparer
            {
                get
                {
                    if (this.comparer == null)
                        this.comparer = new ItemComparer(this.owner);
                    return this.comparer;
                }
            }

            private ArrayList InnerList
            {
                get
                {
                    if (this.innerList == null)
                        this.innerList = new ArrayList();
                    return this.innerList;
                }
            }

            /// <summary>
            ///   <para>Adds an item to the list of items for a ComboBoxBaseDataBound.</para>
            /// </summary>
            /// <param name="item">An object representing the item to add to the collection.</param>
            /// <returns>
            ///   <para>The zero-based index of the item in the collection.</para>
            /// </returns>
            public int Add(object item)
            {
                int index;
                bool added;

                this.owner.CheckNoDataSource();
                if (item == null)
                    throw new ArgumentNullException("item");
                this.InnerList.Add(item);
                index = -1;
                added = false;
                try
                {
                    if (this.owner.sorted)
                    {
                        this.InnerList.Sort(this.Comparer);
                        index = this.InnerList.IndexOf(item);
                        this.owner.NativeInsert(index, item);
                    }
                    else
                    {
                        index = this.InnerList.Count - 1;
                        this.owner.NativeAdd(item);
                    }
                    added = true;
                }
                finally
                {
                    if (!added)
                        this.InnerList.Remove(item);
                }

                this.OnCollectionChange(CollectionChangeOperation.Add, index, item);

                return index;
            }

            /// <summary>
            ///   <para>Adds an array of items to the list of items for a ComboBoxBaseDataBound.</para>
            /// </summary>
            /// <param name="items">An array of objects to add to the list.</param>
            public void AddRange(object[] items)
            {
                this.owner.CheckNoDataSource();
                this.AddRangeInternal((IList)items);
            }

            internal void AddRangeInternal(IList items)
            {
                Exception exp1;

                if (items == null)
                    throw new ArgumentNullException("items");

                // If owner is sorted
                if (this.owner.sorted)
                {
                    IEnumerator itemsEnum = items.GetEnumerator();
                    try
                    {
                        while (itemsEnum.MoveNext())
                        {
                            if (itemsEnum.Current != null)
                                continue;
                            throw new ArgumentNullException("item");
                        }
                    }
                    catch (Exception e)
                    {
                        string s = e.Message;
                    }
                    finally
                    {
                        IDisposable disposable = itemsEnum as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                    }
                    this.InnerList.AddRange(items);
                    this.InnerList.Sort(this.Comparer);
                    Exception exp = null;
                    object[] sortedArray = new object[checked((uint)items.Count)];
                    items.CopyTo(sortedArray, 0);
                    Array.Sort(sortedArray, this.Comparer);
                    int n = 0;
                    while (n < (int)sortedArray.Length)
                    {
                        object value = sortedArray[n];
                        if (exp == null)
                        {
                            try
                            {
                                int index = this.InnerList.IndexOf(value);
                                this.owner.NativeInsert(index, value);
                            }
                            catch (Exception e)
                            {
                                exp = e;
                                this.InnerList.Remove(value);
                            }
                        }
                        else
                            this.InnerList.Remove(value);
                        n++;
                    }
                    if (exp == null)
                        return;
                    throw exp;
                }

                // Owner is not sorted.
                if (true)
                {
                    IEnumerator itemsEnum = items.GetEnumerator();
                    try
                    {
                        while (itemsEnum.MoveNext())
                        {
                            if (itemsEnum.Current != null)
                                continue;
                            throw new ArgumentNullException("item");
                        }
                    }
                    catch (Exception e)
                    {
                        string s = e.Message;
                    }
                    finally
                    {
                        IDisposable disposable = itemsEnum as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                    }
                    this.InnerList.AddRange(items);
                    exp1 = null;
                    itemsEnum = items.GetEnumerator();
                    try
                    {
                        while (itemsEnum.MoveNext())
                        {
                            object current = itemsEnum.Current;
                            if (exp1 == null)
                            {
                                try
                                {
                                    this.owner.NativeAdd(current);
                                }
                                catch (Exception exp2)
                                {
                                    exp1 = exp2;
                                    this.InnerList.Remove(current);
                                }
                            }
                            else
                                this.InnerList.Remove(current);
                        }
                    }
                    finally
                    {
                        IDisposable disposable = itemsEnum as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                    }
                    if (exp1 != null)
                        throw exp1;
                }

                this.OnCollectionChange(CollectionChangeOperation.AddRange, 0, items);
            }

            internal void ClearInternal()
            {
                this.InnerList.Clear();
                this.owner.NativeClear();
                this.owner.SelectedIndex = -1;

                this.OnCollectionChange(CollectionChangeOperation.Clear, 0, null);
            }

            /// <summary>
            ///   <para>Copies the entire collection into an existing array of objects at a specified location within the array.</para>
            /// </summary>
            /// <param name="dest">The object array to copy the collection to.</param>
            /// <param name="arrayIndex">The location in the destination array to copy the collection to. </param>
            public void CopyTo(object[] dest, int arrayIndex)
            {
                this.InnerList.CopyTo(dest, arrayIndex);
            }

            internal void SetItemInternal(int index, object value)
            {
                object[] args;

                if (value == null)
                    throw new ArgumentNullException("value");
                if (index < 0 || index >= this.InnerList.Count)
                {
                    args = new object[2];
                    args[0] = "index";
                    args[1] = index.ToString();
                    throw new ArgumentOutOfRangeException(SR.GetString("InvalidArgument", args));
                }
                this.InnerList[index] = value;
                bool changingCurrentSelection = index == this.owner.SelectedIndex;
                this.owner.NativeRemoveAt(index);
                this.owner.NativeInsert(index, value);
                if (changingCurrentSelection)
                {
                    this.owner.SelectedIndex = index;

                    // this.owner.UpdateText(true);
                }

                this.OnCollectionChange(CollectionChangeOperation.SetItem, index, value);
            }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                this.owner = null;
            }

            #endregion
        }
    }
}
