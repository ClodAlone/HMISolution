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

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents check box field in the PDF form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
    /// PdfBrush brush = PdfBrushes.Black;
    /// PdfGraphics graphics = page.Graphics;
    /// //Create a check box
    /// PdfCheckBoxField checkBox = new PdfCheckBoxField(page, "C#.NET");   
    /// checkBox.Bounds = new RectangleF(100, 290, 20, 20);
    /// document.Form.Fields.Add(checkBox);
    /// checkBox.HighlightMode = PdfHighlightMode.Push;
    /// checkBox.BorderStyle = PdfBorderStyle.Beveled;
    /// //Set the value for the check box
    /// checkBox.Checked = true;
    /// document.Form.Fields.Add(checkBox);
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// Dim brush As PdfBrush = PdfBrushes.Black
    /// Dim graphics As PdfGraphics = page.Graphics
    /// 'Create a check box
    /// Dim checkBox As PdfCheckBoxField = New PdfCheckBoxField(page, "C#.NET") 
    /// checkBox.Bounds = New RectangleF(100, 290, 20, 20)
    /// document.Form.Fields.Add(checkBox)
    /// checkBox.HighlightMode = PdfHighlightMode.Push
    /// checkBox.BorderStyle = PdfBorderStyle.Beveled
    /// 'Set the value for the check box
    /// checkBox.Checked = True
    /// document.Form.Fields.Add(checkBox)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfCheckFieldBase"/> Class 
    /// <seealso cref="PdfDocument"/> Class 
    /// <seealso cref="PdfPage"/> Class 
    public class PdfCheckBoxField : PdfCheckFieldBase
    {
        #region Fields
        /// <summary>
        /// Internal variable to store value whether the check box is checked.
        /// </summary>
        private bool m_checked = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCheckBoxField"/> class.
        /// </summary>
        /// <param name="page">The page where the fields should be placed.</param>
        /// <param name="name">The name of the check box field.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a check box
        /// PdfCheckBoxField checkBox = new PdfCheckBoxField(page, "C#.NET");        
        /// checkBox.Bounds = new RectangleF(100, 290, 20, 20);
        /// document.Form.Fields.Add(checkBox);
        /// checkBox.HighlightMode = PdfHighlightMode.Push;
        /// checkBox.BorderStyle = PdfBorderStyle.Beveled;
        /// //Set the value for the check box
        /// checkBox.Checked = true;
        /// document.Form.Fields.Add(checkBox);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a check box
        /// Dim checkBox As PdfCheckBoxField = New PdfCheckBoxField(page, "C#.NET")      
        /// checkBox.Bounds = New RectangleF(100, 290, 20, 20)
        /// document.Form.Fields.Add(checkBox)
        /// checkBox.HighlightMode = PdfHighlightMode.Push
        /// checkBox.BorderStyle = PdfBorderStyle.Beveled
        /// 'Set the value for the check box
        /// checkBox.Checked = True
        /// document.Form.Fields.Add(checkBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfFont"/> Class 
        public PdfCheckBoxField(PdfPageBase page, string name)
            : base(page, name)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfCheckBoxField"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics graphics = page.Graphics;
        /// //Create a check box
        /// PdfCheckBoxField checkBox = new PdfCheckBoxField(page, "C#.NET");
        /// //Set the value for the check box
        /// checkBox.Checked = true;       
        /// checkBox.Bounds = new RectangleF(100, 290, 20, 20);
        /// document.Form.Fields.Add(checkBox);
        /// checkBox.HighlightMode = PdfHighlightMode.Push;
        /// checkBox.BorderStyle = PdfBorderStyle.Beveled;
        /// //Set the value for the check box
        /// checkBox.Checked = true;
        /// document.Form.Fields.Add(checkBox);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim graphics As PdfGraphics = page.Graphics
        /// 'Create a check box
        /// Dim checkBox As PdfCheckBoxField = New PdfCheckBoxField(page, "C#.NET")
        /// 'Set the value for the check box
        /// checkBox.Checked = True
        /// checkBox.Bounds = New RectangleF(100, 290, 20, 20)
        /// document.Form.Fields.Add(checkBox)
        /// checkBox.HighlightMode = PdfHighlightMode.Push
        /// checkBox.BorderStyle = PdfBorderStyle.Beveled
        /// 'Set the value for the check box
        /// checkBox.Checked = True
        /// document.Form.Fields.Add(checkBox)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfFont"/> Class       
        public bool Checked
        {
            get
            {
                return m_checked;
            }

            set
            {
                if (m_checked != value)
                {
                    m_checked = value;

                    if (m_checked)
                    {
                        Dictionary.SetName(DictionaryProperties.V, DictionaryProperties.Yes);
                    }
                    else
                    {
                        Dictionary.Remove(DictionaryProperties.V);
                    }
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves an instance.
        /// </summary>
        internal override void Save()
        {
            base.Save();

            if (Form != null)
            {
                if (!Checked)
                {
                    Widget.AppearanceState = DictionaryProperties.Off;
                }
                else
                {
                    Widget.AppearanceState = DictionaryProperties.Yes;
                }
            }
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();
            PaintParams parameters
            = new PaintParams(Bounds, BackBrush, ForeBrush, BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

            PdfCheckFieldState state = PdfCheckFieldState.Checked;

            if (!Checked)
            {
                state = PdfCheckFieldState.Unchecked;
            }

            FieldPainter.DrawCheckBox(Page.Graphics, parameters, StyleToString(Style), state);
        }

        /// <summary>
        /// Draws the appearance of the field.
        /// </summary>
        protected override void DrawAppearance()
        {
            base.DrawAppearance();

            PaintParams paintParams = new PaintParams(
                new RectangleF(PointF.Empty, Size), BackBrush, ForeBrush,
                BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

            PdfTemplate template = Widget.ExtendedAppearance.Normal.On;
            FieldPainter.DrawCheckBox(template.Graphics, paintParams, StyleToString(Style),
                PdfCheckFieldState.Checked,Font);

            template = Widget.ExtendedAppearance.Normal.Off;
            FieldPainter.DrawCheckBox(template.Graphics, paintParams, StyleToString(Style),
                PdfCheckFieldState.Unchecked,Font);

            template = Widget.ExtendedAppearance.Pressed.On;
            FieldPainter.DrawCheckBox(template.Graphics, paintParams, StyleToString(Style),
                PdfCheckFieldState.PressedChecked,Font);

            template = Widget.ExtendedAppearance.Pressed.Off;
            FieldPainter.DrawCheckBox(template.Graphics, paintParams, StyleToString(Style),
                PdfCheckFieldState.PressedUnchecked,Font);
        }

        #endregion
    }
}
