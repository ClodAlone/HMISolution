#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents radio button field in the PDF form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();           
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
    /// PdfBrush brush = PdfBrushes.Black;
    /// PdfGraphics g = page.Graphics;
    /// //Create a Radiobutton
    /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
    /// //Add to document
    /// document.Form.Fields.Add(employeesRadioList);
    /// //Create radiobutton items 
    /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
    /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
    /// g.DrawString("1-9", font, brush, new RectangleF(150, 145, 180, 20));
    /// PdfRadioButtonListItem radioItem2 = new PdfRadioButtonListItem("10-49");
    /// radioItem2.Bounds = new RectangleF(100, 170, 20, 20);
    /// g.DrawString("10-49", font, brush, new RectangleF(150, 175, 180, 20));
    /// PdfRadioButtonListItem radioItem3 = new PdfRadioButtonListItem("50-99");
    /// radioItem3.Bounds = new RectangleF(100, 200, 20, 20);
    /// g.DrawString("50-99", font, brush, new RectangleF(150, 205, 180, 20));
    /// PdfRadioButtonListItem radioItem4 = new PdfRadioButtonListItem("100-499");
    /// radioItem4.Bounds = new RectangleF(100, 230, 20, 20);
    /// g.DrawString("100-499", font, brush, new RectangleF(150, 235, 180, 20));
    /// PdfRadioButtonListItem radioItem5 = new PdfRadioButtonListItem("500-more");
    /// radioItem5.Bounds = new RectangleF(100, 260, 20, 20);
    /// g.DrawString("500-more", font, brush, new RectangleF(150, 265, 180, 20));
    /// //add the items to radio button group
    /// employeesRadioList.Items.Add(radioItem1);
    /// employeesRadioList.Items.Add(radioItem2);
    /// employeesRadioList.Items.Add(radioItem3);
    /// employeesRadioList.Items.Add(radioItem4);
    /// employeesRadioList.Items.Add(radioItem5);
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// Dim brush As PdfBrush = PdfBrushes.Black
    /// Dim g As PdfGraphics = page.Graphics
    /// 'Create a Radiobutton
    /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
    /// 'Add to document
    /// document.Form.Fields.Add(employeesRadioList)
    /// 'Create radiobutton items 
    /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
    /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
    /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
    /// Dim radioItem2 As PdfRadioButtonListItem = New PdfRadioButtonListItem("10-49")
    /// radioItem2.Bounds = New RectangleF(100, 170, 20, 20)
    /// g.DrawString("10-49", font, brush, New RectangleF(150, 175, 180, 20))
    /// Dim radioItem3 As PdfRadioButtonListItem = New PdfRadioButtonListItem("50-99")
    /// radioItem3.Bounds = New RectangleF(100, 200, 20, 20)
    /// g.DrawString("50-99", font, brush, New RectangleF(150, 205, 180, 20))
    /// Dim radioItem4 As PdfRadioButtonListItem = New PdfRadioButtonListItem("100-499")
    /// radioItem4.Bounds = New RectangleF(100, 230, 20, 20)
    /// g.DrawString("100-499", font, brush, New RectangleF(150, 235, 180, 20))
    /// Dim radioItem5 As PdfRadioButtonListItem = New PdfRadioButtonListItem("500-more")
    /// radioItem5.Bounds = New RectangleF(100, 260, 20, 20)
    /// g.DrawString("500-more", font, brush, New RectangleF(150, 265, 180, 20))
    /// 'add the items to radio button group
    /// employeesRadioList.Items.Add(radioItem1)
    /// employeesRadioList.Items.Add(radioItem2)
    /// employeesRadioList.Items.Add(radioItem3)
    /// employeesRadioList.Items.Add(radioItem4)
    /// employeesRadioList.Items.Add(radioItem5)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfField"/> Class  
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfPage"/> Class 
    /// <seealso cref="PdfRadioButtonListItem"/> Class 
    public class PdfRadioButtonListField : PdfField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store collection of items.
        /// </summary>
        private PdfRadioButtonItemCollection m_items = null;

        /// <summary>
        /// Internal variable to store selected item index.
        /// </summary>
        private int m_selectedIndex = -1;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRadioButtonListField"/> class.
        /// </summary>
        /// <param name="page">Page which the field to be placed on.</param>
        /// <param name="name">The name of the field.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics g = page.Graphics;
        /// //Create a Radiobutton
        /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
        /// //Add to document
        /// document.Form.Fields.Add(employeesRadioList);
        /// //Create radiobutton items 
        /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
        /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
        /// g.DrawString("1-9", font, brush, new RectangleF(150, 145, 180, 20));
        /// PdfRadioButtonListItem radioItem2 = new PdfRadioButtonListItem("10-49");
        /// radioItem2.Bounds = new RectangleF(100, 170, 20, 20);
        /// g.DrawString("10-49", font, brush, new RectangleF(150, 175, 180, 20));
        /// PdfRadioButtonListItem radioItem3 = new PdfRadioButtonListItem("50-99");
        /// radioItem3.Bounds = new RectangleF(100, 200, 20, 20);
        /// g.DrawString("50-99", font, brush, new RectangleF(150, 205, 180, 20));
        /// PdfRadioButtonListItem radioItem4 = new PdfRadioButtonListItem("100-499");
        /// radioItem4.Bounds = new RectangleF(100, 230, 20, 20);
        /// g.DrawString("100-499", font, brush, new RectangleF(150, 235, 180, 20));
        /// PdfRadioButtonListItem radioItem5 = new PdfRadioButtonListItem("500-more");
        /// radioItem5.Bounds = new RectangleF(100, 260, 20, 20);
        /// g.DrawString("500-more", font, brush, new RectangleF(150, 265, 180, 20));
        /// //add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1);
        /// employeesRadioList.Items.Add(radioItem2);
        /// employeesRadioList.Items.Add(radioItem3);
        /// employeesRadioList.Items.Add(radioItem4);
        /// employeesRadioList.Items.Add(radioItem5);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim g As PdfGraphics = page.Graphics
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
        /// Dim radioItem2 As PdfRadioButtonListItem = New PdfRadioButtonListItem("10-49")
        /// radioItem2.Bounds = New RectangleF(100, 170, 20, 20)
        /// g.DrawString("10-49", font, brush, New RectangleF(150, 175, 180, 20))
        /// Dim radioItem3 As PdfRadioButtonListItem = New PdfRadioButtonListItem("50-99")
        /// radioItem3.Bounds = New RectangleF(100, 200, 20, 20)
        /// g.DrawString("50-99", font, brush, New RectangleF(150, 205, 180, 20))
        /// Dim radioItem4 As PdfRadioButtonListItem = New PdfRadioButtonListItem("100-499")
        /// radioItem4.Bounds = New RectangleF(100, 230, 20, 20)
        /// g.DrawString("100-499", font, brush, New RectangleF(150, 235, 180, 20))
        /// Dim radioItem5 As PdfRadioButtonListItem = New PdfRadioButtonListItem("500-more")
        /// radioItem5.Bounds = New RectangleF(100, 260, 20, 20)
        /// g.DrawString("500-more", font, brush, New RectangleF(150, 265, 180, 20))
        /// 'add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// employeesRadioList.Items.Add(radioItem2)
        /// employeesRadioList.Items.Add(radioItem3)
        /// employeesRadioList.Items.Add(radioItem4)
        /// employeesRadioList.Items.Add(radioItem5)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>       
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public PdfRadioButtonListField(PdfPageBase page, string name)
            : base(page, name)
        {
            Flags |= FieldFlags.Radio;
            Dictionary.SetProperty(DictionaryProperties.FT, new PdfName(DictionaryProperties.Btn));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the first selected item in the list. 
        /// </summary>
        /// <value>The index of the selected item.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics g = page.Graphics;
        /// //Create a Radiobutton
        /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");     
        /// //Add to document
        /// document.Form.Fields.Add(employeesRadioList);
        /// //Create radiobutton items 
        /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
        /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
        /// g.DrawString("1-9", font, brush, new RectangleF(150, 145, 180, 20));
        /// //add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1);
        /// // Set the selected item index index
        /// employeesRadioList.SelectedIndex = 0;
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim g As PdfGraphics = page.Graphics
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
        /// 'add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// ' Set the selected item index index
        /// employeesRadioList.SelectedIndex = 0
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
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
                    PdfRadioButtonListItem item = m_items[m_selectedIndex];
                    Dictionary.SetName(DictionaryProperties.V, item.Value);
                    Dictionary.SetName(DictionaryProperties.DV, item.Value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the first selected item in the list.
        /// </summary>
        /// <value>The selected value of the list field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics g = page.Graphics;
        /// //Create a Radiobutton
        /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
        /// //Add to document
        /// document.Form.Fields.Add(employeesRadioList);
        /// //Create radiobutton items 
        /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
        /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
        /// g.DrawString("1-9", font, brush, new RectangleF(150, 145, 180, 20));
        /// //add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1);
        /// PdfRadioButtonListItem radioItem2 = new PdfRadioButtonListItem("10-49");
        /// radioItem2.Bounds = new RectangleF(100, 170, 20, 20);
        /// // Insert the item as first item
        /// employeesRadioList.Items.Add(radioItem2);
        /// // Set the selected item value
        /// employeesRadioList.SelectedValue = "1-9";
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim g As PdfGraphics = page.Graphics
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
        /// 'add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// Dim radioItem2 As PdfRadioButtonListItem = New PdfRadioButtonListItem("10-49")
        /// radioItem2.Bounds = New RectangleF(100, 170, 20, 20)
        /// ' Insert the item as first item
        /// employeesRadioList.Items.Add(radioItem2)
        /// ' Set the selected item value
        /// employeesRadioList.SelectedValue = "1-9"
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public string SelectedValue
        {
            get
            {
                PdfRadioButtonListItem item = m_items[m_selectedIndex];
                return item.Value;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("SelectedValue");
                }

                PdfRadioButtonListItem item = m_items[m_selectedIndex];
                if (item.Value != value)
                {
                    item.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets the first selected item in the list.
        /// </summary>
        /// <value>The selected item of the field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics g = page.Graphics;
        /// //Create a Radiobutton
        /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
        /// //Add to document
        /// document.Form.Fields.Add(employeesRadioList);
        /// //Create radiobutton items 
        /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
        /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
        /// g.DrawString("1-9", font, brush, new RectangleF(150, 145, 180, 20));
        /// //add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1);
        /// PdfRadioButtonListItem radioItem2 = new PdfRadioButtonListItem("10-49");
        /// radioItem2.Bounds = new RectangleF(100, 170, 20, 20);
        /// // Insert the item as first item
        /// employeesRadioList.Items.Add(radioItem2);
        /// // Set the selected item
        /// employeesRadioList.SelectedItem = radioItem1;
        /// document.Save("Form.pdf")
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim g As PdfGraphics = page.Graphics
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
        /// 'add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// Dim radioItem2 As PdfRadioButtonListItem = New PdfRadioButtonListItem("10-49")
        /// radioItem2.Bounds = New RectangleF(100, 170, 20, 20)
        /// ' Insert the item as first item
        /// employeesRadioList.Items.Add(radioItem2)
        /// ' Set the selected item
        /// employeesRadioList.SelectedItem = radioItem1
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public PdfRadioButtonListItem SelectedItem
        {
            get
            {
                PdfRadioButtonListItem item = null;

                if (m_selectedIndex != -1)
                {
                    item = m_items[m_selectedIndex];
                }

                return item;
            }
        }

        /// <summary>
        /// Gets the items of the radio button field.
        /// </summary>
        /// <value>The radio button field item collection.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics g = page.Graphics;
        /// //Create a Radiobutton
        /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
        /// //Add to document
        /// document.Form.Fields.Add(employeesRadioList);
        /// //Create radiobutton items 
        /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
        /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
        /// g.DrawString("1-9", font, brush, new RectangleF(150, 145, 180, 20));
        /// //add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1);
        /// PdfRadioButtonListItem radioItem2 = new PdfRadioButtonListItem("10-49");
        /// radioItem2.Bounds = new RectangleF(100, 170, 20, 20);
        /// // Insert the item as first item
        /// employeesRadioList.Items.Add(radioItem2);
        /// // Set the selected item value
        /// employeesRadioList.SelectedValue = "1-9";
        /// // Getting item collection
        /// PdfRadioButtonItemCollection itemCollection = employeesRadioList.Items; 
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim g As PdfGraphics = page.Graphics
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
        /// 'add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// Dim radioItem2 As PdfRadioButtonListItem = New PdfRadioButtonListItem("10-49")
        /// radioItem2.Bounds = New RectangleF(100, 170, 20, 20)
        /// ' Insert the item as first item			employeesRadioList.Items.Add(radioItem2)
        /// ' Set the selected item value
        /// employeesRadioList.SelectedValue = "1-9"
        /// ' Getting item collection
        /// Dim itemCollection As PdfRadioButtonItemCollection = employeesRadioList.Items
        /// document.Save("Form.pdf");
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class
        public PdfRadioButtonItemCollection Items
        {
            get
            {
                if (m_items == null)
                {
                    m_items = new PdfRadioButtonItemCollection(this);
                    Dictionary.SetProperty(DictionaryProperties.Kids, m_items);
                }

                return m_items;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            for (int i = 0, size = Items.Count; i < size; ++i)
            {
                Items[i].Draw();
            }
        }
        #endregion
    }
}
