#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents an item of the list fields.
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
    /// // Creates list items
    /// PdfListFieldItemCollection itemCollection = listBox.Items;
    /// //Add the items to the list box
    /// itemCollection.Add(new PdfListFieldItem("English", "English"));
    /// itemCollection.Add(new PdfListFieldItem("French", "French"));
    /// itemCollection.Add(new PdfListFieldItem("German", "German"));
    /// listBox.SelectedIndex = 0;
    /// //Gets the selected item
    /// PdfListFieldItem selectedItem = listBox.SelectedItem;
    /// MessageBox.Show("Selected Item text : " + selectedItem.Text);
    /// MessageBox.Show("Selected Item value : " + selectedItem.Value);
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
    /// ' Creates list items
    /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
    /// 'Add the items to the list box
    /// itemCollection.Add(New PdfListFieldItem("English", "English"))
    /// itemCollection.Add(New PdfListFieldItem("French", "French"))
    /// itemCollection.Add(New PdfListFieldItem("German", "German"))
    /// listBox.SelectedIndex = 0
    /// 'Gets the selected item
    /// Dim selectedItem As PdfListFieldItem = listBox.SelectedItem
    /// MessageBox.Show("Selected Item text : " & selectedItem.Text)
    /// MessageBox.Show("Selected Item value : " & selectedItem.Value)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="IPdfWrapper"/> Interface 
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfPage"/> Class 
    /// <seealso cref="PdfListFieldItemCollection"/> Class 
    public class PdfListFieldItem : IPdfWrapper
    {
        #region Constants
        /// <summary>
        /// Text position it the primitive array.
        /// </summary>
        private int c_textIndex = 1;

        /// <summary>
        /// Value position in the primitive array.
        /// </summary>
        private int c_valueIndex = 0;
        #endregion

        #region Fields
        /// <summary>
        /// Internal variable to store caption of the list item.
        /// </summary>
        private string m_text = String.Empty;

        /// <summary>
        /// Internal variable to store value of the list item.
        /// </summary>
        private string m_value = String.Empty;

        /// <summary>
        /// Internal variable to store array primitive.
        /// </summary>
        private PdfArray m_array = new PdfArray();
        #endregion

        #region Constructros
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListFieldItem"/> class.
        /// </summary>
        public PdfListFieldItem()
            : base()
        {
            Initialize(m_text, m_value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListFieldItem"/> class.
        /// </summary>
        /// <param name="text">The item text, it is displayed in the list.</param>
        /// <param name="value">The item value, it is exported when form content is exported.</param>
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
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// listBox.SelectedIndex = 0;
        /// //Gets the selected item
        /// PdfListFieldItem selectedItem = listBox.SelectedItem;
        /// MessageBox.Show("Selected Item text : " + selectedItem.Text);
        /// MessageBox.Show("Selected Item value : " + selectedItem.Value);
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
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// listBox.SelectedIndex = 0
        /// 'Gets the selected item
        /// Dim selectedItem As PdfListFieldItem = listBox.SelectedItem
        /// MessageBox.Show("Selected Item text : " & selectedItem.Text)
        /// MessageBox.Show("Selected Item value : " & selectedItem.Value)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListFieldItemCollection"/> Class 
        public PdfListFieldItem(string text, string value)
            : base()
        {
            Initialize(text, value);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text of the list item field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();               
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
        /// listBox.HighlightMode = PdfHighlightMode.Outline;            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// listBox.SelectedIndex = 0;
        /// //Gets the selected item
        /// PdfListFieldItem selectedItem = listBox.SelectedItem;
        /// MessageBox.Show("Selected Item text : " + selectedItem.Text);
        /// MessageBox.Show("Selected Item value : " + selectedItem.Value);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// listBox.HighlightMode = PdfHighlightMode.Outline
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// listBox.SelectedIndex = 0
        /// 'Gets the selected item
        /// Dim selectedItem As PdfListFieldItem = listBox.SelectedItem
        /// MessageBox.Show("Selected Item text : " & selectedItem.Text)
        /// MessageBox.Show("Selected Item value : " & selectedItem.Value)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListFieldItemCollection"/> Class 
        public string Text
        {
            get
            {
                return m_text;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Text");
                }

                if (m_text != value)
                {
                    m_text = value;

                    PdfString str = (PdfString)m_array[c_textIndex];

                    str.Value = m_text;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value of the list item field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();               
        /// PdfListBoxField listBox = new PdfListBoxField(page, "list1");
        /// //Add the field to listbox.
        /// document.Form.Fields.Add(listBox);            
        /// //Set the properties.
        /// listBox.Bounds = new RectangleF(100, 350, 100, 50);
        /// listBox.HighlightMode = PdfHighlightMode.Outline;            
        /// // Creates list items
        /// PdfListFieldItemCollection itemCollection = listBox.Items;
        /// //Add the items to the list box
        /// itemCollection.Add(new PdfListFieldItem("English", "English"));
        /// itemCollection.Add(new PdfListFieldItem("French", "French"));
        /// itemCollection.Add(new PdfListFieldItem("German", "German"));
        /// listBox.SelectedIndex = 0;
        /// //Gets the selected item
        /// PdfListFieldItem selectedItem = listBox.SelectedItem;
        /// MessageBox.Show("Selected Item text : " + selectedItem.Text);
        /// MessageBox.Show("Selected Item value : " + selectedItem.Value);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim listBox As PdfListBoxField = New PdfListBoxField(page, "list1")
        /// 'Add the field to listbox.
        /// document.Form.Fields.Add(listBox)
        /// 'Set the properties.
        /// listBox.Bounds = New RectangleF(100, 350, 100, 50)
        /// listBox.HighlightMode = PdfHighlightMode.Outline
        /// ' Creates list items
        /// Dim itemCollection As PdfListFieldItemCollection = listBox.Items
        /// 'Add the items to the list box
        /// itemCollection.Add(New PdfListFieldItem("English", "English"))
        /// itemCollection.Add(New PdfListFieldItem("French", "French"))
        /// itemCollection.Add(New PdfListFieldItem("German", "German"))
        /// listBox.SelectedIndex = 0
        /// 'Gets the selected item
        /// Dim selectedItem As PdfListFieldItem = listBox.SelectedItem
        /// MessageBox.Show("Selected Item text : " & selectedItem.Text)
        /// MessageBox.Show("Selected Item value : " & selectedItem.Value)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfListFieldItemCollection"/> Class 
        public string Value
        {
            get
            {
                return m_value;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Value");
                }

                if (m_value != value)
                {
                    m_value = value;

                    PdfString str = (PdfString)m_array[c_valueIndex];
                    str.Value = m_value;
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        private void Initialize(string text, string value)
        {
            if (c_valueIndex < c_textIndex)
            {
                m_array.Add(new PdfString(value));
                m_array.Add(new PdfString(text));
            }
            else
            {
                m_array.Add(new PdfString(text));
                m_array.Add(new PdfString(value));
            }

            m_text = text;
            m_value = value;
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value>The primitive.</value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_array;
            }
        }
        #endregion
    }
}
