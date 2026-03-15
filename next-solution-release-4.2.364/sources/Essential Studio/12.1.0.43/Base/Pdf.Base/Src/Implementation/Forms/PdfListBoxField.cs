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

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents list box field of the PDF form.
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
    /// <seealso cref="PdfListField"/> Class    
    /// <seealso cref="PdfDocument"/> Class
    /// <seealso cref="PdfPage"/> Class
    /// <seealso cref="PdfFont"/> Class
    /// <seealso cref="PdfListFieldItem"/> Class    
    public class PdfListBoxField : PdfListField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store value whether the fiels is multiselectable.
        /// </summary>
        private bool m_multiselect = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListBoxField"/> class.
        /// </summary>
        /// <param name="page">Page the field to be placed on.</param>
        /// <param name="name">The name of the field.</param>
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
        /// <seealso cref="PdfListField"/> Class    
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfFont"/> Class
        public PdfListBoxField(PdfPageBase page, string name)
            : base(page, name)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the field is multiselectable.
        /// </summary>
        /// <value><c>true</c> if multiselectable; otherwise, <c>false</c>.</value>
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
        /// <seealso cref="PdfListField"/> Class    
        /// <seealso cref="PdfDocument"/> Class
        /// <seealso cref="PdfPage"/> Class
        /// <seealso cref="PdfFont"/> Class
        public bool MultiSelect
        {
            get
            {
                return m_multiselect;
            }

            set
            {
                if (m_multiselect != value)
                {
                    m_multiselect = value;

                    if (m_multiselect)
                    {
                        Flags |= FieldFlags.MultiSelect;
                    }
                    else
                    {
                        Flags -= FieldFlags.MultiSelect;
                    }
                }
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

            if (Widget.GetAppearance() != null)
            {
                Page.Graphics.DrawPdfTemplate(Appearance.Normal, Location);
            }
            else
            {
                RectangleF rect = Bounds;
                rect.Location = PointF.Empty;

                PdfFont font = Font;
                if (font == null)
                {
                    font = PdfDocument.DefaultFont;
                }

                PaintParams parameters
                    = new PaintParams(rect, BackBrush, ForeBrush, BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

                PdfTemplate template = new PdfTemplate(rect.Size);

                FieldPainter.DrawListBox(template.Graphics, parameters, Items,new int[]{ SelectedIndex}, font, StringFormat);

                Page.Graphics.DrawPdfTemplate(template, Bounds.Location, rect.Size);
            }
        }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
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

            PdfFont font = Font;

            if (font == null)
            {
                font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
            }

            FieldPainter.DrawListBox(template.Graphics, paintParams, Items,new int[] {SelectedIndex}, font, StringFormat);
        }

        #endregion
    }
}
