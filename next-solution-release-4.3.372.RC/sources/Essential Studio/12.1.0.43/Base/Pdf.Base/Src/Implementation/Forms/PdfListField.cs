#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents base class form's list fields.
	/// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
    /// //Create list box
    /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
    /// //Add the field to listbox.
    /// document.Form.Fields.Add(listBox);
    /// //Set the properties.
    /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
    /// listBox.HighlightMode = PdfHighlightMode.Outline;
    /// //Add the items to the list box
    /// listBox.Items.Add(new PdfListFieldItem("English", "English"));
    /// listBox.Items.Add(new PdfListFieldItem("French", "French"));
    /// listBox.Items.Add(new PdfListFieldItem("German", "German"));
    /// //Select the item
    /// listBox.SelectedIndex = 2;
    /// //Set the multiselect option
    /// listBox.MultiSelect = true;                    
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// 'Create list box
    /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
    /// 'Add the field to listbox.
    /// document.Form.Fields.Add(listBox)
    /// 'Set the properties.
    /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
    /// listBox.HighlightMode = PdfHighlightMode.Outline
    /// 'Add the items to the list box
    /// listBox.Items.Add(New PdfListFieldItem("English", "English"))
    /// listBox.Items.Add(New PdfListFieldItem("French", "French"))
    /// listBox.Items.Add(New PdfListFieldItem("German", "German"))
    /// 'Select the item
    /// listBox.SelectedIndex = 2
    /// 'Set the multiselect option
    /// listBox.MultiSelect = True
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfAppearanceField"/> Class  
    public abstract class PdfListField : PdfAppearanceField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store collection of items.
        /// </summary>
        private PdfListFieldItemCollection m_items = null;

        /// <summary>
        /// Internal variable to store selected item index.
        /// </summary>
        private int m_selectedIndex = -1;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListField"/> class.
        /// </summary>
        /// <param name="page">Page which the field to be placed on.</param>
        /// <param name="name">The name of the field.</param>
        public PdfListField(PdfPageBase page, string name)
            : base(page, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListField"/> class.
        /// </summary>
        internal PdfListField()
        {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// </code>
        /// <code lang="VB">
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// </code>
        /// </example>
        public PdfListFieldItemCollection Items
        {
            get
            {
                if (m_items == null)
                {
                    m_items = new PdfListFieldItemCollection();
                    Dictionary.SetProperty(DictionaryProperties.Opt, m_items);
                }

                return m_items;
            }
        }

        /// <summary>
        /// Gets or sets the first selected item in the list. 
        /// </summary>
        /// <value>The index of the selected item.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// //Select the item
        /// listBox.SelectedIndex = 2;
        /// </code>
        /// <code lang="VB">
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// 'Select the item
        /// listBox.SelectedIndex = 2
        /// </code>
        /// </example>
        public int SelectedIndex
        {
            get
            {
                return m_selectedIndex;
            }

            set
            {
                if ((value < 0) || (value >= Items.Count))
                {
                    throw new ArgumentOutOfRangeException("SelectedIndex");
                }

                if (m_selectedIndex != value)
                {
                    m_selectedIndex = value;
                    Dictionary.SetProperty(DictionaryProperties.I, new PdfArray(new int[] { m_selectedIndex }));
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the first selected item in the list.
        /// </summary>
        /// <value>The selected value.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// //Select the item
        /// listBox.SelectedValue = "English";
        /// </code>
        /// <code lang="VB">
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// 'Select the item
        /// listBox.SelectedValue = "English"
        /// </code>
        /// </example>
        public string SelectedValue
        {
            get
            {
                PdfListFieldItem item = m_items[m_selectedIndex];
                return item.Value;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("SelectedValue");
                }

                PdfListFieldItem item = m_items[m_selectedIndex];
                if (item.Value != value)
                {
                    item.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets the first selected item in the list.
        /// </summary>
        /// <value>The selected item.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create list box
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// //Gets the selected item
        /// PdfListFieldItem selectedItem = listBox.SelectedItem;        
        /// </code>
        /// <code lang="VB">
        /// 'Create list box
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// 'Gets the selected item
        /// Dim selectedItem As PdfListFieldItem = listBox.SelectedItem
        /// </code>
        /// </example>
        public PdfListFieldItem SelectedItem
        {
            get
            {
                return m_items[m_selectedIndex];
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();
        }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Dictionary.SetProperty(DictionaryProperties.FT, new PdfName(DictionaryProperties.Ch));
        }
        #endregion
    }
}
