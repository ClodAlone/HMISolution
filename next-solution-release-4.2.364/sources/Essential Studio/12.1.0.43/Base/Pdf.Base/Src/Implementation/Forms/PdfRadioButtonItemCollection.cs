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
    /// Represents collection of radio buttons items.
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
    /// <seealso cref="PdfCollection"/> Class  
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfPage"/> Class 
    /// <seealso cref="PdfRadioButtonListItem"/> Class 
    public class PdfRadioButtonItemCollection : PdfCollection, IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store array of item's primitives.
        /// </summary>
        private PdfArray m_array = new PdfArray();

        /// <summary>
        /// Internal variable to store field.
        /// </summary>
        private PdfRadioButtonListField m_field = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRadioButtonItemCollection"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        public PdfRadioButtonItemCollection(PdfRadioButtonListField field)
        {
            m_field = field;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The <see cref="PdfRadioButtonListItem"/> object to be added to collection.</param>
        /// <returns>The index of the added field.</returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
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
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
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
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public int Add(PdfRadioButtonListItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return DoAdd(item);
        }

        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">The index where to insert the new item..</param>
        /// <param name="item">A <see cref="PdfRadioButtonListItem"/> object to be added to collection.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
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
        /// employeesRadioList.Items.Insert(0, radioItem2);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
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
        /// employeesRadioList.Items.Insert(0, radioItem2)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public void Insert(int index, PdfRadioButtonListItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            DoInsert(index, item);
        }

        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <param name="item">The <see cref="PdfRadioButtonListItem"/> object which is to be removed from the collection.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
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
        /// // Remove the item
        /// employeesRadioList.Items.Remove(radioItem1);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
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
        /// ' Remove the item
        /// employeesRadioList.Items.Remove(radioItem1)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public void Remove(PdfRadioButtonListItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            DoRemove(item);
        }

        /// <summary>
        /// Removes the item at the specified position.
        /// </summary>
        /// <param name="index">The index where to remove the item.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
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
        /// // Remove the item
        /// employeesRadioList.Items.RemoveAt(0);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
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
        /// ' Remove the item
        /// employeesRadioList.Items.RemoveAt(0)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= List.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            PdfRadioButtonListItem item = (PdfRadioButtonListItem)List[index];
            m_array.RemoveAt(index);
            List.RemoveAt(index);
        }

        /// <summary>
        /// Gets the index of the item within the collection.
        /// </summary>
        /// <param name="item">A <see cref="PdfRadioButtonListItem"/> object whose index is requested.</param>
        /// <returns>Index of the item with the collection.</returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
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
        /// // Find the index
        /// int index = employeesRadioList.Items.IndexOf(radioItem1);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
        /// 'Add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// Dim radioItem2 As PdfRadioButtonListItem = New PdfRadioButtonListItem("10-49")
        /// radioItem2.Bounds = New RectangleF(100, 170, 20, 20)
        /// ' Insert the item as first item
        /// employeesRadioList.Items.Add(radioItem2)
        /// ' Find the index
        /// Dim index As Integer = employeesRadioList.Items.IndexOf(radioItem1)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public int IndexOf(PdfRadioButtonListItem item)
        {
            return List.IndexOf(item);
        }

        /// <summary>
        /// Determines whether the collection contains the specified item.
        /// </summary>
        /// <param name="item">Check whether <see cref="PdfRadioButtonListItem"/> object is exists in the collection or not.</param>
        /// <returns>
        /// <c>true</c> if collection contains specified item; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
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
        /// // Check whether the specified item is in Collection
        /// if (employeesRadioList.Items.Contains(radioItem1))
        ///  MessageBox.Show("Item already added in the collection");
        /// else
        ///  //add the items to radio button group
        ///  employeesRadioList.Items.Add(radioItem1);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
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
        /// ' Check whether the specified item is in Collection
        /// If employeesRadioList.Items.Contains(radioItem1) Then
        ///  MessageBox.Show("Item already added in the collection")
        /// Else
        ///   'add the items to radio button group
        ///   employeesRadioList.Items.Add(radioItem1)
        /// End If
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public bool Contains(PdfRadioButtonListItem item)
        {
            return List.Contains(item);
        }

        /// <summary>
        /// Clears the item collection.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
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
        /// // Clears the item
        /// employeesRadioList.Items.Clear();
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
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
        /// ' Clears the item
        /// employeesRadioList.Items.Clear()
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public void Clear()
        {
            DoClear();
        }

        /// <summary>
        /// Gets the <see cref="PdfRadioButtonListItem"/> at the specified index.
        /// </summary>
        /// <value>Returns item at the specified position.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
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
        /// // Gets the first item from the collection
        /// PdfRadioButtonListItem item = employeesRadioList.Items[0];
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
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
        /// ' Gets the first item from the collection
        /// Dim item As PdfRadioButtonListItem = employeesRadioList.Items(0)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListItem"/> Class 
        public PdfRadioButtonListItem this[int index]
        {
            get
            {
                return (PdfRadioButtonListItem)List[index];
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Index of the inserted item.</returns>
        private int DoAdd(PdfRadioButtonListItem item)
        {
            m_array.Add(new PdfReferenceHolder(item));
            item.SetField(m_field);
            return List.Add(item);
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        private void DoInsert(int index, PdfRadioButtonListItem item)
        {
            m_array.Insert(index, new PdfReferenceHolder(item));
            item.SetField(m_field);
            List.Insert(index, item);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void DoRemove(PdfRadioButtonListItem item)
        {
            if (List.Contains(item))
            {
                int index = List.IndexOf(item);
                m_array.RemoveAt(index);
                item.SetField(null);
                List.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        private void DoClear()
        {
            foreach (PdfRadioButtonListItem item in List)
            {
                item.SetField(null);
            }

            m_array.Clear();
            List.Clear();
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        Syncfusion.Pdf.Primitives.IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_array;
            }
        }
        #endregion
    }
}
