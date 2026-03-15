#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents text box field in the PDF form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();           
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
    /// //Create a text box
    /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");  
    /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
    /// firstNameTextBox.Font = font;
    /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
    /// //Add the textbox in document
    /// document.Form.Fields.Add(firstNameTextBox);                      
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// 'Create a text box
    /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")   
    /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
    /// firstNameTextBox.Font = font
    /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
    /// 'Add the textbox in document
    /// document.Form.Fields.Add(firstNameTextBox)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfAppearanceField"/> Class   
    /// <seealso cref="PdfDocument"/> Class  
    /// <seealso cref="PdfPage"/> Class  
    /// <seealso cref="PdfFont"/> Class  
    public class PdfTextBoxField : PdfAppearanceField
    {
        #region Constants
        /// <summary>
        /// The password chrackter.
        /// </summary>
        const string m_passwordValue = "*";
        #endregion

        #region Fields
        /// <summary>
        /// Internal variable to store value.
        /// </summary>
        private string m_text = String.Empty;

        /// <summary>
        /// Internal variable to store default value.
        /// </summary>
        private string m_defaultValue = String.Empty;

        /// <summary>
        /// Internal variable to store value whether to check spelling.
        /// </summary>
        private bool m_spellCheck = false;

        /// <summary>
        /// Internal variable to store value whether the field has comb behavior.
        /// </summary>
        private bool m_insertSpaces = false;

        /// <summary>
        /// Internal variable to store value whether the field should be multiline.
        /// </summary>
        private bool m_multiline = false;

        /// <summary>
        /// Internal variable to store value whether it is a password field.
        /// </summary>
        private bool m_password = false;

        /// <summary>
        /// Internal variable to store value whether the field is scrollable.
        /// </summary>
        private bool m_scrollable = true;

        /// <summary>
        /// Internal variable to store field's maximum length. in characters.
        /// </summary>
        private int m_maxLength = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text of the text box field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");
        /// firstNameTextBox.Text = "Cris";
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")
        /// firstNameTextBox.Text = "Cris"
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
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
                    Dictionary.SetString(DictionaryProperties.V, m_text);
                }
            }
        }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value of the text box field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");
        /// firstNameTextBox.DefaultValue = "Cris";
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")
        /// firstNameTextBox.DefaultValue = "Cris"
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public string DefaultValue
        {
            get
            {
                return m_defaultValue;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("DefaultValue");
                }

                if (m_defaultValue != value)
                {
                    m_defaultValue = value;
                    Dictionary.SetString(DictionaryProperties.DV, m_defaultValue);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check spelling.
        /// </summary>
        /// <value><c>true</c> if check spelling; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");
        /// firstNameTextBox.SpellCheck = true;
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")
        /// firstNameTextBox.SpellCheck = True
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public bool SpellCheck
        {
            get
            {
                return m_spellCheck;
            }

            set
            {
                if (m_spellCheck != value)
                {
                    m_spellCheck = value;

                    if (m_spellCheck)
                    {
                        Flags &= ~FieldFlags.DoNotSpellCheck;
                    }
                    else
                    {
                        Flags |= FieldFlags.DoNotSpellCheck;
                    }
                }
            }
        }

        /// <summary>
        /// Meaningful only if the MaxLength property is set and the Multiline, Password properties are false.
        /// If set, the field is automatically divided into as many equally spaced positions, or combs, 
        /// as the value of MaxLength, and the text is laid out into those combs.
        /// </summary>
        /// <value><c>true</c> if need to insert spaces; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");
        /// firstNameTextBox.InsertSpaces = true;
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")
        /// firstNameTextBox.InsertSpaces = True
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public bool InsertSpaces
        {
            get
            {
                return m_insertSpaces;
            }

            set
            {
                if (m_insertSpaces != value)
                {
                    m_insertSpaces = value;

                    if (m_insertSpaces)
                    {
                        Flags |= FieldFlags.Comb;
                    }
                    else
                    {
                        Flags &= ~FieldFlags.Comb;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfTextBoxField"/> is multiline.
        /// </summary>
        /// <value><c>true</c> if multiline; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");
        /// firstNameTextBox.Multiline = true;
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")
        /// firstNameTextBox.Multiline = True
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public bool Multiline
        {
            get
            {
                return m_multiline;
            }

            set
            {
                if (m_multiline != value)
                {
                    m_multiline = value;

                    if (m_multiline)
                    {
                        Flags |= FieldFlags.Multiline;
                        StringFormat.LineAlignment = PdfVerticalAlignment.Top;
                    }
                    else
                    {
                        Flags &= ~FieldFlags.Multiline;
                        StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfTextBoxField"/> is password field.
        /// </summary>
        /// <value><c>true</c> if password field; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");
        /// firstNameTextBox.Password = true;
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")
        /// firstNameTextBox.Password = True
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class 
        public bool Password
        {
            get
            {
                return m_password;
            }

            set
            {
                if (m_password != value)
                {
                    m_password = value;

                    if (m_password)
                    {
                        Flags |= FieldFlags.Password;
                    }
                    else
                    {
                        Flags &= ~FieldFlags.Password;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfTextBoxField"/> is scrollable.
        /// </summary>
        /// <value><c>true</c> if scrollable; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");
        /// firstNameTextBox.Scrollable = true;
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")
        /// firstNameTextBox.Scrollable = True
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class 
        public bool Scrollable
        {
            get
            {
                return m_scrollable;
            }

            set
            {
                if (m_scrollable != value)
                {
                    m_scrollable = value;

                    if (m_scrollable)
                    {
                        Flags &= ~FieldFlags.DoNotScroll;
                    }
                    else
                    {
                        Flags |= FieldFlags.DoNotScroll;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of characters that can be entered in the text box.
        /// </summary>
        /// <value>An integer value specifying the maximum number of characters that can be entered in the text box.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");
        /// firstNameTextBox.MaxLength = 8;
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")
        /// firstNameTextBox.MaxLength = 8
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class 
        public int MaxLength
        {
            get
            {
                return m_maxLength;
            }

            set
            {
                if (m_maxLength != value)
                {
                    m_maxLength = value;
                    Dictionary.SetNumber(DictionaryProperties.MaxLen, m_maxLength);
                }
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextBoxField"/> class.
        /// </summary>
        /// <param name="page">Page which the field to be placed on.</param>
        /// <param name="name">The name of the text box field.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// //Create a text box
        /// PdfTextBoxField firstNameTextBox = new PdfTextBoxField(page, "firstNameTextBox");  
        /// firstNameTextBox.Bounds = new RectangleF(100, 20, 200, 20);
        /// firstNameTextBox.Font = font;
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55);
        /// //Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox);                      
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a text box
        /// Dim firstNameTextBox As PdfTextBoxField = New PdfTextBoxField(page, "firstNameTextBox")   
        /// firstNameTextBox.Bounds = New RectangleF(100, 20, 200, 20)
        /// firstNameTextBox.Font = font
        /// page.Graphics.DrawString("First Name", font, PdfBrushes.Black, 10, 55)
        /// 'Add the textbox in document
        /// document.Form.Fields.Add(firstNameTextBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>         
        /// <seealso cref="PdfDocument"/> Class  
        /// <seealso cref="PdfPage"/> Class  
        /// <seealso cref="PdfFont"/> Class  
        public PdfTextBoxField(PdfPageBase page, string name)
            : base(page, name)
        {
            Font = PdfDocument.DefaultFont;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextBoxField"/> class.
        /// </summary>
        internal PdfTextBoxField()
        {
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();

            if (Widget.GetAppearance() != null)
            {
                Page.Graphics.DrawPdfTemplate(Appearance.Normal, Location);
            }
            else
            {
                PaintParams parameters
                = new PaintParams(Bounds, BackBrush, ForeBrush, BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

                FieldPainter.DrawTextBox(Page.Graphics, parameters, Text, Font, StringFormat,Multiline,Scrollable);
            }
        }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Flags |= FieldFlags.DoNotSpellCheck;
            Dictionary.SetProperty(DictionaryProperties.FT, new PdfName(DictionaryProperties.Tx));
        }

        /// <summary>
        /// Draws the appearance.
        /// </summary>
        /// <param name="template">The template.</param>
        protected override void DrawAppearance(PdfTemplate template)
        {
            base.DrawAppearance(template);

            PaintParams paintParams = new PaintParams(
                new RectangleF(PointF.Empty, Size), BackBrush, ForeBrush,
                BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

            string text = Text;

            if (Password)
            {
                text = string.Empty;
                for (int i = 0; i < Text.Length; ++i)
                {
                    text += m_passwordValue;
                }
            }

            FieldPainter.DrawTextBox(template.Graphics, paintParams, text, GetFont(), StringFormat,Multiline,Scrollable);
        }
        #endregion
    }
}
