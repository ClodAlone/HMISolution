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
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
    public sealed class ImageIndexesCollection :
        ICollection,
        IDisposable
    {
        #region Members

        /// <summary>
        /// Image indexes.
        /// </summary>
        private Hashtable m_htIndexes = null;

        /// <summary>
        /// ComboBoxAdv Items.
        /// </summary>
        private ComboBoxBaseDataBound.ObjectCollection m_items = null;
        #endregion

        #region Constructor
        internal ImageIndexesCollection(ComboBoxBaseDataBound.ObjectCollection items)
        {
            m_htIndexes = new Hashtable();
            m_items = items;
            m_items.CollectionChange += new ComboBoxBaseDataBound.CollectionChangeEventHandler(M_Items_CollectionChange);
        }
        #endregion

        #region Indexer
        public int this[int index]
        {
            get
            {
                int ret = -1;
                if (m_htIndexes.ContainsKey(index))
                {
                    ret = (int)m_htIndexes[index];
                }

                return ret;
            }
            set
            {
                if (m_htIndexes.ContainsKey(index))
                {
                    m_htIndexes[index] = value;
                }
            }
        }
        #endregion

        #region ICollection

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="System.Collections.ICollection"></see> is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return this.m_htIndexes.IsSynchronized;
            }
        }

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="System.Collections.ICollection"></see>.</returns>
        public int Count
        {
            get
            {
                return this.m_htIndexes.Count;
            }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CopyTo(Array array, int index)
        {
            // Do nothing here.
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="System.Collections.ICollection"></see>.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this.m_htIndexes.SyncRoot;
            }
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return m_htIndexes.Values.GetEnumerator();
        }

        #endregion

        #region Events
        private void M_Items_CollectionChange(object sender, ComboBoxBaseDataBound.CollectionChangeEventArgs args)
        {
            switch (args.Operation)
            {
                case ComboBoxBaseDataBound.CollectionChangeOperation.AddRange:
                    IList list = args.Item as IList;

                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            this.m_htIndexes.Add(m_htIndexes.Count, -1);
                        }
                    }
                    break;

                case ComboBoxBaseDataBound.CollectionChangeOperation.Add:
                    if (!this.m_htIndexes.ContainsKey(args.Index))
                    {
                        this.m_htIndexes.Add(args.Index, -1);
                    }
                    break;

                case ComboBoxBaseDataBound.CollectionChangeOperation.Clear:
                    this.m_htIndexes.Clear();
                    break;

                case ComboBoxBaseDataBound.CollectionChangeOperation.RemoveAt:
                    if (this.m_htIndexes.ContainsKey(args.Index))
                    {
                        for (int i = args.Index; i < this.m_htIndexes.Count; i++)
                        {
                            this.m_htIndexes[i] = this.m_htIndexes[i + 1];
                        }
                        this.m_htIndexes.Remove(this.m_htIndexes.Count - 1);
                    }
                    break;

                case ComboBoxBaseDataBound.CollectionChangeOperation.Insert:
                    this.m_htIndexes.Add(this.m_htIndexes.Count, -1);
                    for (int i = this.m_htIndexes.Count - 1; i > args.Index - 1; i--)
                    {
                        this.m_htIndexes[i] = this.m_htIndexes[i - 1];
                    }

                    this.m_htIndexes[args.Index] = -1;
                    break;
            }
        }
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (null != m_htIndexes)
            {
                m_htIndexes.Clear();
                m_htIndexes = null;
            }

            m_items = null;
        }

        #endregion
    }

    #region ComboBoxAdv
    /// <summary>
    /// Represents a combo box control.
    /// </summary>
    /// <remarks>
    /// <para>Similar to the Windows Forms' <see cref="System.Windows.Forms.ComboBox"/>,
    /// the <b>ComboBoxAdv</b> displays an editing field combined with a listbox,
    /// allowing the user to select from the list or to enter new text. The
    /// <see cref="DropDownStyle"/> property determines the style of combo box to display.</para>
    /// </remarks>
    [
    Designer(typeof(Syncfusion.Windows.Forms.Tools.ComboBoxAdvDesigner),
        typeof(System.ComponentModel.Design.IDesigner)),
    System.Drawing.ToolboxBitmap(typeof(ComboBoxAdv), "ToolboxIcons.ComboBoxAdv.bmp"),
    Description("Represents an advanced combo box control.")
    ]
    public class ComboBoxAdv : ComboBoxBaseDataBound
    {
        #region Class members

        private const int c_iImageIndent = 2;

        /// <summary>
        /// Width of the borders.
        /// </summary>
        private const int c_bordersWidth = 4;

        /// <summary>
        /// Default Item height
        /// </summary>
        private static int ITMHEIGHT = default(int);
        /// <summary>
        /// Default height of combobox when comboBoxStyle is simple.
        /// </summary>
        private const int c_defaultDropDownSimpleHeight = 150;

        /// <summary>
        /// Image indexes.
        /// </summary>
        private ImageIndexesCollection m_imageIndexes = null;

        /// <summary>
        /// Collection of ImageIndexItems.
        /// </summary>
        private ImageIndexItemCollection m_imageItemsCollection = null;

        /// <summary>
        /// Image list.
        /// </summary>
        private ImageList m_imageList = null;

        /// <summary>
        /// Show image of selected item in text box.
        /// </summary>
        private bool m_bShowImageInTextBox = false;

        /// <summary>
        /// Show images in combo list box.
        /// </summary>
        private bool m_bShowImageInComboListBox = false;

        /// <summary>
        /// Show indent for images in combo list box.
        /// </summary>
        private bool m_bForceImageIndent = false;

        /// <summary>
        /// Specifies whether selected index is changed or not.
        /// </summary>
        private bool isSelectedIndexChanged = false;
        /// <summary>
        /// Specifies whether selection change is committed.
        /// </summary>
        private bool isSelectionChangeCommitted = false;
        #endregion

        #region Class initialization

        public ComboBoxAdv()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ComboBoxAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            m_imageIndexes = new ImageIndexesCollection(this.Items);
            m_imageItemsCollection = new ImageIndexItemCollection(this);
        }

        protected override ListControl CreateListControl() 
        { 
            return new ComboListBox(this); 
        }
        protected override void InitListControl(ListControl listControl)
        {
            this.ListBox.TabStop = false;
            base.InitListControl(listControl);
            ITMHEIGHT = this.ListBox.ItemHeight;
            if (base.EnableTouchMode)
            {
                this.ListBox.ItemHeight = (int)(ITMHEIGHT * 1.5F); 
            }
        }

        protected override void UpdateListBoxBorderStyle()
        {
            if (ListBox == null)
                return;

            if (this.DropDownStyle == ComboBoxStyle.Simple)
            {
                this.ListBox.BorderStyle = BorderStyle.None;
            }
            else
            {
                if (this.DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    if (DesignMode)
                    {
                        this.TextBox.Clear();
                    }
                    else
                    {
                        if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                        {
                            this.TextBox.Text = ListBox.Items[SelectedIndex].ToString();
                        }
                    }
                }

                if (this.FlatStyle == ComboFlatStyle.System)
                {
                    this.ListBox.BorderStyle = BorderStyle.Fixed3D;
                }
                else
                {
                    this.ListBox.BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }
        protected override void OnDataManagerPositionChanged()
        {
            if (DataManager != null)
            {
                if (ListBox.Items.Count > DataManager.Position)
                {
                    SelectedIndex = DataManager.Position;
                }
            }
        }
        protected override void OnDisplayMemberChanged(EventArgs e)
        {
            base.OnDisplayMemberChanged(e);

            if (this.ListControl == null)
                this.ListControl = this.CreateListControl();

            if (this.DataSource == null)
            {
                ArrayList list = new ArrayList();

                foreach (object item in this.Items)
                    list.Add(item);

                this.Items.ClearInternal();

                foreach (object item in list)
                    this.Items.Add(item);

                list.Clear();
            }

            this.ListControl.DisplayMember = this.DisplayMember;
        }

        protected override void NativeAdd(object item)
        {
            base.NativeAdd(item);

            if (this.initializing)
                return;

            if (item != null)
            {
                m_imageItemsCollection.Add(new ImageIndexItem(this, item, this.ImageIndexes[this.Items.IndexOf(item)]));
            }
        }

        protected override void NativeClear()
        {
            base.NativeClear();

            m_imageItemsCollection.Clear();
        }

        protected override void NativeInsert(int index, object item)
        {
            base.NativeInsert(index, item);

            m_imageItemsCollection.Insert(index, new ImageIndexItem(this, item));
        }

        protected override void NativeRemoveAt(int index)
        {
            base.NativeRemoveAt(index);

            if (m_imageItemsCollection[index] != null)
            {
                m_imageItemsCollection.RemoveAt(index);
            }
        }

        #endregion INIT

        #region Class internal declarations

        [Serializable]
        [TypeConverter(typeof(ComboBoxAdv.ImageIndexItemTypeConverter))]
        public class ImageIndexItem
        {
            private ComboBoxAdv m_combo = null;
            private object m_item = String.Empty;
            private int m_index = -1;

            internal object Item
            {
                get { return m_item; }
            }

            public int ImageIndex
            {
                get
                { 
                    return m_index;
                }
                set
                {
                    if (m_index != value)
                    {
                        m_index = value;
                        m_combo.ImageIndexes[m_combo.Items.IndexOf(this.Item)] = m_index;
                    }
                }
            }

            internal ComboBoxAdv ComboBox
            {
                get { return m_combo; }
            }

            public string Name
            {
                get { return m_combo.GetItemText(this.Item); }
            }

            public ImageIndexItem()
            {
            }

            public ImageIndexItem(ComboBoxAdv combo, object item)
            {
                m_combo = combo;
                m_item = item;
            }

            public ImageIndexItem(ComboBoxAdv combo, object item, int imageIndex)
                : this(combo, item)
            {
                m_index = imageIndex;
            }
        }

        public class ImageIndexItemTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
                {
                    ComboBoxAdv.ImageIndexItem item = value as ComboBoxAdv.ImageIndexItem;

                    if (item != null)
                    {
                        if (item.ImageIndex != -1)
                        {
                            System.Reflection.ConstructorInfo ci = typeof(ComboBoxAdv.ImageIndexItem).GetConstructor(
                            new Type[] { typeof(ComboBoxAdv), typeof(object), typeof(int) });
                          return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { item.ComboBox, item.Item, item.ImageIndex });                      
                        }
                        else
                        {
                            System.Reflection.ConstructorInfo ci = typeof(ComboBoxAdv.ImageIndexItem).GetConstructor(
                            new Type[] { typeof(ComboBoxAdv), typeof(object) });

                            return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { item.ComboBox, item.Item });
                        }
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
            {
                if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }
        }

        [Serializable]
        public class ImageIndexItemCollection :
            CollectionBase,
            IDisposable
        {
            #region Members
           private ComboBoxAdv m_combo = null;
            #endregion

            #region Constructor
            internal ImageIndexItemCollection(ComboBoxAdv combo)
            {
                m_combo = combo;

                if (!m_combo.DesignMode)
                    return;

                if (m_combo.Items != null && m_combo.Items.Count > 0)
                {
                    for (int i = 0; i < m_combo.Items.Count; i++)
                    {
                        object item = m_combo.Items[i];
                        this.Add(new ImageIndexItem(m_combo, item, m_combo.ImageIndexes[m_combo.Items.IndexOf(item)]));
                    }
                }
            }
            #endregion

            #region Indexer
            public ImageIndexItem this[int index]
            {
                get
                {
                    ImageIndexItem item = null;
                    if (index >= 0 && index < List.Count)
                    {
                        item = List[index] as ImageIndexItem;
                    }

                    return item;
                }
                set
                {
                    if (index < 0 || index >= this.List.Count)
                    {
                        throw new IndexOutOfRangeException("index");
                    }
                    if (value == null)
                    {
                        throw new NullReferenceException("value can't be NULL");
                    }

                    if (this.List[index] != value)
                    {
                        this.List[index] = value;
                    }
                }
            }
            #endregion

            #region Methods

            public void Add(ImageIndexItem item)
            {
                if (item == null)
                {
                    throw new NullReferenceException("Item can't be NULL");
                }

                this.List.Add(item);
            }

            public bool Contains(ImageIndexItem item)
            {
                if (item == null)
                {
                    throw new NullReferenceException("Item can't be NULL");
                }

                return this.List.Contains(item);
            }

            public void Remove(ImageIndexItem item)
            {
                if (item == null)
                {
                    throw new NullReferenceException("Item can't be NULL");
                }

                if (!this.Contains(item))
                {
                    throw new NullReferenceException("Item doesn't exist in collection");
                }

                this.List.Remove(item);
            }

            public int IndexOf(ImageIndexItem item)
            {
                if (item == null)
                {
                    throw new NullReferenceException("Item can't be NULL");
                }

                return this.List.IndexOf(item);
            }

            public void Insert(int index, ImageIndexItem item)
            {
                if (item == null)
                {
                    throw new NullReferenceException("Item can't be NULL");
                }

                if (index < 0 || (index > this.List.Count))
                {
                    throw new IndexOutOfRangeException("index");
                }

                this.List.Insert(index, item);
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                m_combo = null;
            }

            #endregion
        }

        #region *** ImageIndexesCollectionEditor
    
        public class ImageIndexesCollectionEditor : UITypeEditor
        {
            #region Overrides

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                ImageIndexItemCollection collection = value as ImageIndexItemCollection;
                if (collection != null)
                {
                    IWindowsFormsEditorService editorSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                    if (editorSvc != null)
                    {
                        using (EditorForm frmEditor = new EditorForm(collection))
                        {
                            if (editorSvc.ShowDialog(frmEditor) == DialogResult.OK)
                            {
                                collection.Clear();

                                IList items = frmEditor.Items;

                                for (int i = 0; i < items.Count; i++)
                                {
                                    collection.Add(items[i] as ImageIndexItem);
                                }
                            }
                        }
                    }
                }
                return value;
            }

            #endregion

            #region *** EditorForm
          public class EditorForm : Form
            {
                #region Constructor
    
                public EditorForm(ImageIndexItemCollection collection)
                {
                    InitializeComponent();

                    m_items = new ArrayList();

                    if (collection != null)
                    {
                        int nItems = collection.Count;

                        if (nItems > 0)
                        {
                            ListBox.ObjectCollection listItems = itemsList.Items;
                            for (int i = 0; i < nItems; i++)
                            {
                                ImageIndexItem item = collection[i];

                                listItems.Add(item.Item);
                                m_items.Add(new ImageIndexItem(item.ComboBox, item.Item, item.ImageIndex));
                            }

                            itemsList.SelectedIndex = 0;
                        }
                    }
                }
                #endregion

                #region Properties

                public IList Items
                {
                    get { return m_items; }
                }
                #endregion

                #region Implementation

                private void InitializeComponent()
                {
                    this.itemsPanel = new System.Windows.Forms.Panel();
                    this.itemsList = new System.Windows.Forms.ListBox();
                    this.itemsProperty = new System.Windows.Forms.PropertyGrid();
                    this.buttonsPanel = new System.Windows.Forms.Panel();
                    this.btOK = new System.Windows.Forms.Button();
                    this.btCancel = new System.Windows.Forms.Button();
                    this.itemsPanel.SuspendLayout();
                    this.buttonsPanel.SuspendLayout();
                    this.SuspendLayout();

                    // panel1
                    this.itemsPanel.Controls.Add(this.itemsList);
                    this.itemsPanel.Controls.Add(this.itemsProperty);
                    this.itemsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.itemsPanel.Location = new System.Drawing.Point(0, 0);
                    this.itemsPanel.Name = "panel1";
                    this.itemsPanel.Size = new System.Drawing.Size(592, 322);

                    // listBox1
                    this.itemsList.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.itemsList.IntegralHeight = false;
                    this.itemsList.Location = new System.Drawing.Point(0, 0);
                    this.itemsList.Name = "listBox1";
                    this.itemsList.Size = new System.Drawing.Size(361, 322);
                    this.itemsList.Sorted = false;
                    this.itemsList.TabIndex = 0;

                    this.itemsList.SelectedIndexChanged += new EventHandler(OnSelectedItemChanged);

                    // propertyGrid1
                    this.itemsProperty.Dock = System.Windows.Forms.DockStyle.Right;
                    this.itemsProperty.Location = new System.Drawing.Point(361, 0);
                    this.itemsProperty.Name = "propertyGrid1";
                    this.itemsProperty.Size = new System.Drawing.Size(231, 322);
                    this.itemsProperty.TabIndex = 1;

                    // panel2
                    this.buttonsPanel.Controls.Add(this.btOK);
                    this.buttonsPanel.Controls.Add(this.btCancel);
                    this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
                    this.buttonsPanel.Location = new System.Drawing.Point(0, 322);
                    this.buttonsPanel.Name = "panel2";
                    this.buttonsPanel.Size = new System.Drawing.Size(592, 44);

                    // btOK
                    this.btOK.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
                    this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.btOK.Location = new System.Drawing.Point(424, 9);
                    this.btOK.Name = "btOK";
                    this.btOK.Size = new System.Drawing.Size(75, 23);
                    this.btOK.TabIndex = 0;
                    this.btOK.Text = "OK";
                   
                    // btCancel
                    this.btCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
                    this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.btCancel.Location = new System.Drawing.Point(505, 9);
                    this.btCancel.Name = "btCancel";
                    this.btCancel.Size = new System.Drawing.Size(75, 23);
                    this.btCancel.TabIndex = 1;
                    this.btCancel.Text = "Cancel";

                    // Form1
                    this.AcceptButton = this.btOK;
                    this.CancelButton = this.btCancel;
                    this.ClientSize = new System.Drawing.Size(592, 366);
                    this.Controls.Add(this.itemsPanel);
                    this.Controls.Add(this.buttonsPanel);
                    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    this.Text = "Items Image Indexes";
                    this.itemsPanel.ResumeLayout(false);
                    this.buttonsPanel.ResumeLayout(false);
                    this.ResumeLayout(false);
                }
                #endregion

                #region Event handlers
     
                private void OnSelectedItemChanged(object sender, EventArgs e)
                {
                    object objSelected = null;

                    if (itemsList.SelectedIndex >= 0)
                    {
                        objSelected = m_items[itemsList.SelectedIndex];
                    }

                    itemsProperty.SelectedObjects = new object[] { objSelected };
                }
                #endregion

                #region Fields
                private ArrayList m_items;
                private System.Windows.Forms.Panel itemsPanel;
                private System.Windows.Forms.ListBox itemsList;
                private System.Windows.Forms.PropertyGrid itemsProperty;
                private System.Windows.Forms.Panel buttonsPanel;
                private System.Windows.Forms.Button btOK;
                private System.Windows.Forms.Button btCancel;
                #endregion
            }
            #endregion
        }
        #endregion

        #endregion

        #region Class properties

        /// <summary>
        /// Gets or sets image list.
        /// </summary>
        [
            DefaultValue(null),
            Category("Appearance - Images"),
            Description("Gets or sets image list.")
        ]
        public ImageList ImageList
        {
            get
            {
                return this.m_imageList;
            }
            set
            {
                if (this.m_imageList != value)
                {
                    this.m_imageList = value;
                }
            }
        }

        /// <summary>
        /// Gets image list indexes.
        /// </summary>
        [Browsable(false)]
        public ImageIndexesCollection ImageIndexes
        {
            get
            {
                return m_imageIndexes;
            }
        }

        /// <summary>
        /// Gets collection of image indexes.
        /// </summary>
        [Editor(typeof(ComboBoxAdv.ImageIndexesCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets collection of image indexes.")]
        public ImageIndexItemCollection ItemsImageIndexes
        {
            get { return m_imageItemsCollection; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether draw selected item image in text box.
        /// </summary>
        [
            DefaultValue(false),
            Category("Appearance - Images"),
            Description("Gets or sets draw selected item image in text box.")
        ]
        public bool ShowImageInTextBox
        {
            get
            {
                return m_bShowImageInTextBox;
            }
            set
            {
                if (m_bShowImageInTextBox != value)
                {
                    m_bShowImageInTextBox = value;
                    this.SetNeedLayout(true);
                }
            }
        }

        [Browsable(false)]
        public new bool UseMnemonic
        {
            get
            {
                return base.UseMnemonic;
            }
            set
            {
                base.UseMnemonic = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether draw images in combo list box.
        /// </summary>
        [
            DefaultValue(false),
            Category("Appearance - Images"),
            Description("Gets or sets draw images in combo list box.")
        ]
        public bool ShowImagesInComboListBox
        {
            get
            {
                return m_bShowImageInComboListBox;
            }
            set
            {
                if (m_bShowImageInComboListBox != value)
                {
                    m_bShowImageInComboListBox = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show indent for images in combo list box.
        /// </summary>
        [
            DefaultValue(false),
            Category("Appearance - Images"),
            Description("Gets or sets show indent for images in combo list box.")
        ]
        public bool ForceImageIndent
        {
            get
            {
                return m_bForceImageIndent;
            }
            set
            {
                if (m_bForceImageIndent != value)
                {
                    m_bForceImageIndent = value;
                }
            }
        }

        /// <summary>
        /// Gets the collection of Items
        /// </summary>
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
        "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public override ComboBoxBaseDataBound.ObjectCollection Items
        {
            get
            {
                return base.Items;
            }
        }

        /// <summary>
        /// Gets the ListBox associated with this combo.
        /// </summary>
        [Browsable(false)]
        public virtual ListBox ListBox
        {
            get { return base.ListControl as ListBox; }
        }

        /// <summary>
        /// Gets or sets the IntegralHeight
        /// </summary>
        public override bool IntegralHeight
        {
            get
            {
                return base.IntegralHeight;
            }
            set
            {
                if (this.IntegralHeight != value)
                {
                    base.IntegralHeight = value;
                    this.ListBox.IntegralHeight = value;
                }
            }
        }

        private int textBoxHeight = 13;
        /// <summary>
        /// Gets or sets the textbox height in ComboBoxAdv.
        /// </summary>
        [
            Browsable(false),
            DefaultValue(13),
            Category("Layout"),
            Description("Gets or sets the textbox height in ComboBoxAdv.")
        ]
        public int TextBoxHeight
        {
            get
            {
                return this.textBoxHeight;
            }
            set
            {
                if (this.textBoxHeight != value)
                    this.textBoxHeight = value;
            }
        }

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private bool IsMirrored
#else
        private new bool IsMirrored
#endif
        {
            get
            {
                return this.RightToLeft == RightToLeft.Yes;
            }
        }

        /// <summary>
        /// Gets or sets the DropDown Style
        /// </summary>
        public new ComboBoxStyle DropDownStyle
        {
            get
            {
                return base.DropDownStyle;
            }

            set
            {
                base.DropDownStyle = value;
                if (base.DropDownStyle == ComboBoxStyle.Simple)
                {
                    this.Height = c_defaultDropDownSimpleHeight;
                }
            }
        }

        #endregion

        #region Class overrides

        public override bool UpdateText(bool fireEvent)
        {
            bool returnValue = false;
            
            string oldText = this.Text;
            if (this.PopupControl != null)
            {
                string newText = this.GetPopupText();

                if (this.CharacterCasing == CharacterCasing.Upper)
                    newText = newText.ToUpper();
                else if (this.CharacterCasing == CharacterCasing.Lower)
                    newText = newText.ToLower();

                if (this.IsTextValid(newText) || newText == String.Empty)
                {
                    this.TextBox.Text = newText;
                }

                this.SetNeedLayout(true);
            }
            else if (this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                this.TextBox.Text = String.Empty;
            }

            if (this.DropDownStyle==ComboBoxStyle.DropDownList && this.FindStringExact(this.Text)==-1)
            {
                fireEvent = false;
            }
            else if (oldText == this.Text && this.DropDownStyle!=ComboBoxStyle.DropDownList)
            {
                fireEvent = false;
            }
            // Fire the selection change committed event.
            if (fireEvent && !this.Disposing)
            {
                this.OnSelectionChangeCommitted(EventArgs.Empty);
                this.isSelectionChangeCommitted = true;
                returnValue = true;
            }
            return returnValue;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            this.isSelectedIndexChanged = true;
        }

        protected override void DetermineHeightsBasedOnFont(Graphics g, ref int textAreaHeight)
        {
            Size textAreaSize = ControlDrawing.MeasureDisplayStringSize(g, this.Text, this.Font, (this.RightToLeft == RightToLeft.Yes));
            if (this.TextBoxHeight > this.Font.Height + 2 && textAreaSize.Height < this.TextBoxHeight)
            {
                textAreaHeight = this.TextBoxHeight;
                int ncwidth = (this.Style == VisualStyle.Default) ? 2 : 1;

                // Button Size.
                this.DropDownButtonHeight = textAreaHeight + 4;
                if (this.Style != VisualStyle.Default)
                    this.DropDownButtonHeight += 2; // because ncheight is only 1

                // Determine control height.
                this.EditPortionHeight = this.DropDownButtonHeight + ncwidth * 2;

                if (this.DropDownStyle != ComboBoxStyle.Simple
                    || this.Height < this.EditPortionHeight)
                {
                    this.Height = this.EditPortionHeight;
                }
            }
            else
                base.DetermineHeightsBasedOnFont(g, ref textAreaHeight);
        }

        protected override void OnStyleChanged(EventArgs e)
        {
            if (this.Style == VisualStyle.Metro)
            {
                ScrollersFrame scrollFrame = new ScrollersFrame();
                scrollFrame.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                scrollFrame.AttachedTo = ListBox;
            }
   
            base.OnStyleChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw Image
            if (this.ImageList != null && this.ShowImageInTextBox)
            {
                int index = this.ListBox.SelectedIndex;
                if (index >= 0)
                {
                    int imageIndex = this.ImageIndexes[index];
                    if (imageIndex >= 0 && this.ImageList.Images.Count > imageIndex)
                    {
                        Rectangle rect;
                        if (this.IsMirrored)
                        {
                            rect = new Rectangle(this.TextBox.Bounds.X + this.TextBox.Bounds.Width + c_iImageIndent, this.TextBox.Bounds.Y, this.TextBox.Bounds.Height, this.TextBox.Bounds.Height);
                        }
                        else
                        {
                            rect = new Rectangle(this.TextBox.Bounds.X - this.TextBox.Bounds.Height - c_iImageIndent, this.TextBox.Bounds.Y, this.TextBox.Bounds.Height, this.TextBox.Bounds.Height);
                        }

                        if (this.Enabled)
                        {
                            e.Graphics.DrawImage(this.ImageList.Images[imageIndex], rect);
                        }
                        else
                        {
                            ControlPaint.DrawImageDisabled(e.Graphics, new Bitmap(ImageList.Images[imageIndex], rect.Size), rect.X, rect.Y, this.BackColor);      
                        }
                    }
                }
            }
            if (this.isSelectedIndexChanged)
            {
                if (this.isSelectionChangeCommitted)
                {
                    this.TextBox.SelectAll();
                    this.isSelectionChangeCommitted = false;
                }
                this.isSelectedIndexChanged = false;
            }
        }

        protected override void SetTextBoxBounds(Rectangle rect)
        {
            Rectangle correctedRect;

            int iImg = this.ImageIndexes[this.SelectedIndex];

            if (this.ShowImageInTextBox && this.ImageList != null && iImg >= 0 && this.ImageList.Images.Count > iImg)
            {
                int indent = rect.Height + c_iImageIndent;
                if (this.IsMirrored)
                {
                    correctedRect = new Rectangle(rect.X, rect.Y, rect.Width - indent, rect.Height);
                }
                else
                {
                    correctedRect = new Rectangle(rect.X + indent, rect.Y, rect.Width - indent, rect.Height);
                }
            }
            else
            {
                correctedRect = rect;
            }

            base.SetTextBoxBounds(correctedRect);
        }

        /// <summary>
        /// Finds the first item in the combo box that starts with the specified string.
        /// </summary>
        /// <param name="s">The string to search for.</param>
        /// <returns>
        /// The zero-based index of the first item found; -1 if no match is found.
        /// </returns>
        /// <override/>
        public override int FindString(string s)
        {
            return this.FindString(s, -1);
        }

        /// <summary>
        /// Finds the first item after the given index which starts with the given string. The search is not case sensitive.
        /// </summary>
        /// <param name="s">The string to search for.</param>
        /// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to -1 to search from the beginning of the control.</param>
        /// <returns>
        /// The zero-based index of the first item found; -1 if no match is found.
        /// </returns>
        /// <override/>
        public override int FindString(string s, int startIndex)
        {
            if (s == null)
                return -1;

            return this.ListBox.FindString(s, startIndex);
        }

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="s">The string to search for.</param>
        /// <returns>
        /// The zero-based index of the first item found; returns -1 if no match is found.
        /// </returns>
        /// <override/>
        public override int FindStringExact(string s)
        {
            return this.FindStringExact(s, -1);
        }

        /// <summary>
        /// Finds the first item after the specified index that matches the specified string.
        /// </summary>
        /// <param name="s">The string to search for.</param>
        /// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to -1 to search from the beginning of the control.</param>
        /// <returns>
        /// The zero-based index of the first item found; returns -1 if no match is found.
        /// </returns>
        /// <override/>
        public override int FindStringExact(string s, int startIndex)
        {
            if (s == null)
                return -1;
            s = s.Replace("&&", "&");
            return this.ListBox.FindStringExact(s, startIndex);
        }

        /// <summary>
        /// Finds the first item in the combo box that matches the specified string.
        /// </summary>
        /// <param name="text">Text to search.</param>
        /// <param name="ignoreCase">Indicates whether to ignore case during serach.</param>
        /// <returns>
        /// Index of specified text in list; -1, if nothing found.
        /// </returns>
        public override int FindStringExact(string text, bool ignoreCase)
        {
            return this.FindItem(text,false, -1,ignoreCase);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)       
        {
            if (this.DropDownStyle == ComboBoxStyle.Simple && this.IntegralHeight && height > this.EditPortionHeight)  
            {
                // Restrict the height to be a multiple of the ItemHeight.
                int specifiedListHeight = this.GetEmbeddedChildBounds(height).Height;
                int listNCHeight = this.GetListNCHeight();

                int n = (specifiedListHeight - listNCHeight) / this.ListBox.ItemHeight;

                int rightListHeight = n * this.ListBox.ItemHeight + listNCHeight;

                int delta = rightListHeight - specifiedListHeight;

                if (delta > 0)
                {
                    height += delta;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnBeforePopup()
        {
            object oSelItem = this.ListBox.SelectedItem;
            string sText = this.TextBox.Text;

            if (null == oSelItem || oSelItem.ToString() != sText)
            {
                int index = 0;
                if (!this.CaseSensitiveAutocomplete)
                    index = this.FindItem(sText, false, -1, true);
                else
                    index = this.ListBox.Items.IndexOf(sText);

                if (index > -1)
                {
                    this.ListBox.SelectedIndex = index;
                }
            }

            this.ListBox.IntegralHeight = true;
            int itemsToShow = this.ListBox.Items.Count > this.MaxDropDownItems ? this.MaxDropDownItems : this.ListBox.Items.Count;
            if (itemsToShow == 0)
                itemsToShow = 1;
            if (this.Style == VisualStyle.Metro)
            {
                this.ListBox.ItemHeight = 23;                
            }
            // Make sure the handle is created so that when ItemHeight is called, it will be the right value.
            IntPtr handle = this.ListBox.Handle;

            int itemHeight = Math.Max(this.ListBox.Font.Height, this.ListBox.ItemHeight);
            int padding = 2;

            if (this.ListBox.ItemHeight != itemHeight)
                this.ListBox.ItemHeight = itemHeight + padding;

            int listBoxHeight = itemsToShow * this.ListBox.ItemHeight;
            int screenHeight = Screen.GetWorkingArea(this).Height;

            if (listBoxHeight > screenHeight)
            {
                listBoxHeight = screenHeight;
            }
            if (this.Style == VisualStyle.Metro)
                listBoxHeight = listBoxHeight + 30;
            this.ListBox.ClientSize = new Size(this.ListBox.ClientSize.Width, listBoxHeight);
            base.OnBeforePopup();
        }
        protected override void OnPopupClosed(PopupClosedEventArgs e)
        {
            if((e.PopupCloseType == PopupCloseType.Canceled || e.PopupCloseType == PopupCloseType.Deactivated) && this.Text != this.ListControl.Text)
                this.ListControl.SelectedIndex = this.FindStringExact(this.Text, -1);

            base.OnPopupClosed(e);
            this.ListBox.IntegralHeight = this.IntegralHeight;
        }

        protected override Point CorrectPopupLocation(Point location)
        {
            Point ret = base.CorrectPopupLocation(location);
            return ret;
        }

        protected override void OnEndInit()
        {
            base.OnEndInit();

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (ItemsImageIndexes[i] != null)
                {
                    ImageIndexes[i] = this.ItemsImageIndexes[i].ImageIndex;
                }
            }
        }

        protected override void AttachPopupControl()
        {
            base.AttachPopupControl();

            if (this.DropDownStyle == ComboBoxStyle.Simple)
            {
                UpdatePopupControlRelationship();
            }
        }

        protected override void DetachPopupControl(bool disposing)
        {
            base.DetachPopupControl(disposing);

            if (this.PopupControl != null && this.DropDownStyle == ComboBoxStyle.Simple && !disposing)
            {
                this.PopupControl.Parent.Controls.Remove(this.PopupControl);
            }
        }

       protected override void UpdatePopupControlRelationship()
        {
            base.UpdatePopupControlRelationship();

            if (this.PopupControl != null && this.DropDownStyle == ComboBoxStyle.Simple)
            {
                this.SuspendLayout();

                this.Controls.Add(this.PopupControl);

                this.ResumeLayout(false);
            }
        }
        protected override void UpdatePopupControlBounds()
        {
            base.UpdatePopupControlBounds();

            if (PopupControl != null && this.DropDownStyle == ComboBoxStyle.Simple)
            {
                SuspendLayout();

                this.PopupControl.Visible = true;
                this.PopupControl.Top = this.EditPortionHeight + c_bordersWidth / 2;
                this.PopupControl.Left = c_bordersWidth / 2;
                this.PopupControl.Width = this.Width - c_bordersWidth;

                ResumeLayout(false);
            }
        }

        protected override TextBox CreateTextBox()
        {
            return new ComboBoxAdvTextBox();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_imageIndexes != null)
                {
                    ((IDisposable)m_imageIndexes).Dispose();
                    m_imageIndexes = null;
                }

                if (m_imageItemsCollection != null)
                {
                    ((IDisposable)m_imageItemsCollection).Dispose();
                    m_imageItemsCollection = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Class utility methods

        private void RefreshTextSelection()
        {
            TextBox.HideSelection = false;
            TextBox.SelectionLength = 0;
            TextBox.SelectAll();
        }

        #endregion
    }
    #endregion

    #region ComboBoxAdvDesigner
    public class ComboBoxAdvDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public ComboBoxAdvDesigner()
            : base()
        {
        }
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

       private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    actionLists.Add(
                        new ComboBoxAdvActionList(this.Component));
                }
                return actionLists;
            }
        }

#endif
    }
    #endregion

    #region ComboListBox
    /// <summary>
    /// Specifies the list box used in a <see cref="ComboBoxAdv"/>.
    /// </summary>
    [ToolboxItem(false), Documentation.DocumentationExclude()]
    public class ComboListBox : ListBox
    {
        #region Constants
        // Office2007 Colors
        private static readonly Color c_textColorOffice2007 = Color.Black;
        private static readonly Color c_borderColorOffice2007 = Color.FromArgb(200, 177, 128);
        private static readonly Color c_foreColorTopFirstOffice2007 = Color.FromArgb(255, 253, 235);
        private static readonly Color c_foreColorTopLastOffice2007 = Color.FromArgb(255, 235, 178);
        private static readonly Color c_foreColorBottomFirstOffice2007 = Color.FromArgb(255, 213, 98);
        private static readonly Color c_foreColorBottomLastOffice2007 = Color.FromArgb(255, 228, 145);
        private static readonly Color c_foreColorBottomLineOffice2007 = Color.FromArgb(255, 248, 181);
        #endregion

        #region Members
        /// <summary>
        /// Parent ComboBoxAdv.
        /// </summary>
        private ComboBoxAdv m_comboBox = null;
        /// <summary>
        /// Specifies Metro color scheme.
        /// </summary>

        #endregion

        #region Constructors
        public ComboListBox(ComboBoxAdv comboBox)
            : base()
        {
            if (comboBox == null)
            {
                throw new NullReferenceException("comboBox");
            }

            m_comboBox = comboBox;

            if (!m_comboBox.IsInitializing)
            {                
                IntegralHeight = true;
                foreach (object item in m_comboBox.Items)
                    this.Items.Add(item);               
            }

            this.DrawMode = DrawMode.OwnerDrawFixed;
        }
        #endregion

        #region Properties

        public override DrawMode DrawMode
        {
            get
            {
                return base.DrawMode;
            }
            set
            {
                base.DrawMode = DrawMode.OwnerDrawFixed;
            }
        } 
        
        #endregion

        #region Drawing
        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        protected virtual void DrawImage(DrawItemEventArgs e)
        {
            if (m_comboBox.ShowImagesInComboListBox && m_comboBox.ImageList != null && m_comboBox.ImageList.Images.Count > 0)
            {
                int imageIndex = (int)m_comboBox.ImageIndexes[e.Index];

                if (imageIndex >= 0 && m_comboBox.ImageList.Images.Count > imageIndex)
                {
                    Rectangle rect;
                    if (this.m_comboBox.RightToLeft == RightToLeft.Yes)
                    {
                        rect = new Rectangle(e.Bounds.X + e.Bounds.Width - this.ItemHeight, e.Bounds.Y, this.ItemHeight, this.ItemHeight);
                    }
                    else
                    {
                        rect = new Rectangle(e.Bounds.Location, new Size(this.ItemHeight, this.ItemHeight));
                    }

                    rect.Inflate(-1, -1);
                    e.Graphics.DrawImage(m_comboBox.ImageList.Images[imageIndex], rect);
                }
            }
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        protected virtual void DrawText(DrawItemEventArgs e)
        {
            int index = e.Index;
            if (index != -1)
            {
                TextFormatFlags tf = TextFormatFlags.SingleLine | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPrefix;
                HorizontalAlignment hAlign = m_comboBox.TextAlign;

                switch (hAlign)
                {
                    case HorizontalAlignment.Left:
                        tf |= TextFormatFlags.Left;
                        break;
                    case HorizontalAlignment.Center:
                        tf |= TextFormatFlags.HorizontalCenter;
                        break;
                    case HorizontalAlignment.Right:
                        tf |= TextFormatFlags.Right;
                        break;
                }

                Rectangle textRect = e.Bounds;

                int iImg = m_comboBox.ImageIndexes[index];

                if (m_comboBox.ForceImageIndent || (m_comboBox.ImageList != null && iImg >= 0 && iImg < m_comboBox.ImageList.Images.Count && m_comboBox.ShowImagesInComboListBox))
                {
                    textRect.Width -= this.ItemHeight;
                }

                if (m_comboBox.RightToLeft == RightToLeft.Yes)
                {
                    tf |= TextFormatFlags.RightToLeft;

                    if (hAlign != HorizontalAlignment.Center)
                    {
                        tf ^= TextFormatFlags.Left | TextFormatFlags.Right;
                    }
                }
                else
                {
                    textRect.X = e.Bounds.Right - textRect.Width;
                    if (m_comboBox.Style == VisualStyle.Metro)
                    {
                        if (m_comboBox.TextAlign == HorizontalAlignment.Right)
                        {
                            textRect.X = textRect.X - 4;
                            textRect.Y = textRect.Y + 6;
                        }
                        else
                        {
                            textRect.X = textRect.X + 4;
                            textRect.Y = textRect.Y + 6;
                        }
                    }
                }
                Color foreColor = (m_comboBox.Style == VisualStyle.Office2007 || m_comboBox.Style == VisualStyle.Office2010) ? c_textColorOffice2007 : e.ForeColor;
                
                TextRenderer.DrawText(e.Graphics, this.Items[index].ToString(), this.Font, textRect, foreColor, tf);
            }
        }

        /// <summary>
        /// Draws the Background
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        protected virtual void DrawBackground(DrawItemEventArgs e)
        {
            if (m_comboBox.Style == VisualStyle.Office2007)
            {
                DrawBackgroundOffice2007(e);
            }
            else if (m_comboBox.Style == VisualStyle.Office2010)
            {
                DrawBackgroundOffice2007(e);
            }
            else if (m_comboBox.Style == VisualStyle.Metro)
            {
                DrawBackgroundMetro(e);
            }
            else
            {
                // Default background
                using(Brush brush =new SolidBrush(e.BackColor))
                    e.Graphics.FillRectangle(brush, e.Bounds);
            }
        }
         protected virtual void DrawBackgroundMetro(DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) != 0)
            {
                Graphics g = e.Graphics;
                Rectangle rect = e.Bounds;

                SolidBrush solidBrush = new SolidBrush(m_comboBox.MetroColor);
                    g.FillRectangle(solidBrush, rect);
                    solidBrush.Dispose();
                
            }
            else
            {
                using (Brush brush = new SolidBrush(e.BackColor))
                    e.Graphics.FillRectangle(brush, e.Bounds);
            }
        }
        protected virtual void DrawBackgroundOffice2007(DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) != 0)
            {
                Graphics g = e.Graphics;
                Rectangle rect = e.Bounds;

                // Draw Border
                using (Pen borderPen = new Pen(c_borderColorOffice2007))
                {
                    g.DrawLine(borderPen, rect.X + 1, rect.Y, rect.Right - 2, rect.Y);
                    g.DrawLine(borderPen, rect.X, rect.Y + 1, rect.X, rect.Bottom - 2);
                    g.DrawLine(borderPen, rect.X + 1, rect.Bottom - 1, rect.Right - 2, rect.Bottom - 1);
                    g.DrawLine(borderPen, rect.Right - 1, rect.Y + 1, rect.Right - 1, rect.Bottom - 2);
                }

                rect.Inflate(-2, -2);

                // Draw foreground
                RectangleF foreRectTop = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height / 2f);
                RectangleF foreRectBottom = new RectangleF(rect.X, rect.Y + rect.Height / 2f, rect.Width, rect.Height / 2f);
                using (LinearGradientBrush brush = new LinearGradientBrush(foreRectTop, c_foreColorTopFirstOffice2007, c_foreColorTopLastOffice2007, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, foreRectTop);
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(foreRectBottom, c_foreColorBottomFirstOffice2007, c_foreColorBottomLastOffice2007, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, foreRectBottom);
                }

                using (Pen linePen = new Pen(c_foreColorBottomLineOffice2007))
                {
                    g.DrawLine(linePen, foreRectBottom.X - 1, foreRectBottom.Y, foreRectBottom.X - 1, foreRectBottom.Bottom);
                    g.DrawLine(linePen, foreRectBottom.Right, foreRectBottom.Y, foreRectBottom.Right, foreRectBottom.Bottom);
                    g.DrawLine(linePen, foreRectBottom.X, foreRectBottom.Bottom, foreRectBottom.Right, foreRectBottom.Bottom);
                }
            }
            else
            {
                using (Brush brush = new SolidBrush(e.BackColor))
                    e.Graphics.FillRectangle(brush, e.Bounds);
            }
        }

        /// <summary>
        /// Raises the DrawItem event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DrawItemEventArgs"></see> that contains the event data.</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            this.DrawBackground(e);
            this.DrawImage(e);
            this.DrawText(e);
            base.OnDrawItem(e);
        }
        #endregion Drawing

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_comboBox = null;
            }
            base.Dispose(disposing);
        }

        // TIP: In MultiColumnComboBox we override the GridListControl's OnBindingContextChanged and prevent calling the base class
        // to avoid unnecessary multiple calls. That might speed things up here too.
        protected override void WndProc(ref Message m)
        {   
            // Prevent the list box from processing the LeftMouse Down message, 
            // since it activates it's parent Form in it's Default Wnd Proc. 
            if (m.Msg == 0x201/*WM_LBUTTONDOWN*/)
            {
                // This is true only when in dropdown.
                if (this.FindForm() is PopupHost)
                    return;
                if (!this.Parent.ContainsFocus)
                {
                    this.Parent.Focus();
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            int padding = 2;
            this.ItemHeight = this.Font.Height + padding;
            base.OnFontChanged(e);
        }

        #endregion
    }
    #endregion

    #region ComboBoxAdvTextBox
    [ToolboxItem(false)]
    public class ComboBoxAdvTextBox : TextBox
    {
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (this.BorderStyle == BorderStyle.None && !this.Multiline)
            {
                if (this.Left != x || this.Top != y || this.Width != width || this.Height != height)
                {
                    if (!this.IsHandleCreated)
                    {
                        this.UpdateBounds(x, y, width, height);
                    }
                    else
                    {
                        NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, x, y, width, height, 20);
                    }
                }
            }
            else
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }
    }
    #endregion
}
