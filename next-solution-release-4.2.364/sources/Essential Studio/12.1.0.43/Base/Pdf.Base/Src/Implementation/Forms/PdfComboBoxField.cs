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
    /// Represents combo box field in the PDF Form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);          
    /// //Create a combo box
    /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");   
    /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
    /// positionComboBox.Font = font;
    /// positionComboBox.Editable = true;
    /// //Add it to document
    /// document.Form.Fields.Add(positionComboBox);
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// 'Create a combo box
    /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox") 
    /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
    /// positionComboBox.Font = font
    /// positionComboBox.Editable = True
    /// 'Add it to document
    /// document.Form.Fields.Add(positionComboBox)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfListField"/> Class    
    /// <seealso cref="PdfStyledField"/> Class 
    /// <seealso cref="PdfPage"/> Class 
    /// <seealso cref="PdfDocument"/> Class 
    /// <seealso cref="PdfFont"/> Class 
    public class PdfComboBoxField : PdfListField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store value whether the combobox is editable.
        /// </summary>
        private bool m_editable = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfComboBoxField"/> class.
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
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");    
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;
        /// //Add it to document
        /// document.Form.Fields.Add(positionComboBox);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")     
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// 'Add it to document
        /// document.Form.Fields.Add(positionComboBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>         
        /// <seealso cref="PdfStyledField"/> Class 
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfFont"/> Class 
        public PdfComboBoxField(PdfPageBase page, string name)
            : base(page, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfComboBoxField"/> class.
        /// </summary>
        internal PdfComboBoxField()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfComboBoxField"/> is editable.
        /// </summary>
        /// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);          
        /// //Create a combo box
        /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
        /// positionComboBox.Editable = true
        /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
        /// positionComboBox.Font = font;
        /// positionComboBox.Editable = true;
        /// //Add it to document
        /// document.Form.Fields.Add(positionComboBox);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// 'Create a combo box
        /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")        
        /// positionComboBox.Editable = True
        /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
        /// positionComboBox.Font = font
        /// positionComboBox.Editable = True
        /// 'Add it to document
        /// document.Form.Fields.Add(positionComboBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>         
        /// <seealso cref="PdfStyledField"/> Class 
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfFont"/> Class        
        public bool Editable
        {
            get
            {
                return m_editable;
            }

            set
            {
                if (m_editable != value)
                {
                    m_editable = value;
                    if (m_editable)
                    {
                        Flags |= FieldFlags.Edit;
                    }
                    else
                    {
                        Flags &= FieldFlags.Edit;
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

                string text = String.Empty;

                if (SelectedIndex != -1)
                {
                    text = SelectedItem.Text;
                }

                FieldPainter.DrawComboBox(template.Graphics, parameters);

                template.Graphics.DrawString(text, font, ForeBrush, rect, StringFormat);

                Page.Graphics.DrawPdfTemplate(template, Bounds.Location, rect.Size);
            }
        }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Flags |= FieldFlags.Combo;
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

            FieldPainter.DrawComboBox(template.Graphics, paintParams);
        }
        #endregion
    }
}
