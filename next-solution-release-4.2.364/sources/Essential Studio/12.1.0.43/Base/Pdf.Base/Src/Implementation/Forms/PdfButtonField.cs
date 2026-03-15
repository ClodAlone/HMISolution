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
    /// Represents button field in the PDF form.
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
    /// // Creating action
    /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
    /// submitAction.DataFormat = SubmitDataFormat.Html;
    /// //Create submit button to transfer the values in the form
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(100, 500, 90, 20);
    /// submitButton.Font = font;
    /// submitButton.Text = "Submit";
    /// submitButton.Actions.MouseUp = submitAction;
    /// document.Form.Fields.Add(submitButton);
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
    /// ' Creating action
    /// Dim submitAction As PdfSubmitAction = New PdfSubmitAction("http://stevex.net/dump.php")
    /// submitAction.DataFormat = SubmitDataFormat.Html
    /// 'Create submit button to transfer the values in the form
    /// Dim submitButton As PdfButtonField = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(100, 500, 90, 20)
    /// submitButton.Font = font
    /// submitButton.Text = "Submit"
    /// submitButton.Actions.MouseUp = submitAction
    /// document.Form.Fields.Add(submitButton)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfAppearanceField"/> Class   
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfSubmitAction"/> Class   
    public class PdfButtonField : PdfAppearanceField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store button's caption.
        /// </summary>
        private string m_text = String.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfButtonField"/> class.
        /// </summary>
        /// <param name="page">The page where the fields should be placed.</param>
        /// <param name="name">The name of the button.</param>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics graphics = page.Graphics;
        /// // Creating action
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// submitAction.DataFormat = SubmitDataFormat.Html;
        /// //Create submit button to transfer the values in the form
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(100, 500, 90, 20);
        /// submitButton.Font = font;
        /// submitButton.Text = "Submit";
        /// submitButton.Actions.MouseUp = submitAction;
        /// document.Form.Fields.Add(submitButton);
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
        /// ' Creating action
        /// Dim submitAction As PdfSubmitAction = New PdfSubmitAction("http://stevex.net/dump.php")
        /// submitAction.DataFormat = SubmitDataFormat.Html
        /// 'Create submit button to transfer the values in the form
        /// Dim submitButton As PdfButtonField = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(100, 500, 90, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Submit"
        /// submitButton.Actions.MouseUp = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfAppearanceField"/> Class   
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfSubmitAction"/> Class  
        public PdfButtonField(PdfPageBase page, string name)
            : base(page, name)
        {
            StringFormat.Alignment = PdfTextAlignment.Center;
            Widget.WidgetAppearance.NormalCaption = name;
            Widget.TextAlignment = PdfTextAlignment.Center;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfButtonField"/> class.
        /// </summary>
        internal PdfButtonField()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the caption text.
        /// </summary>
        /// <value>The caption text.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics graphics = page.Graphics;
        /// // Creating action
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// submitAction.DataFormat = SubmitDataFormat.Html;
        /// //Create submit button to transfer the values in the form
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(100, 500, 90, 20);
        /// submitButton.Font = font;
        /// submitButton.Text = "Submit";
        /// submitButton.Actions.MouseUp = submitAction;
        /// document.Form.Fields.Add(submitButton);
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
        /// ' Creating action
        /// Dim submitAction As PdfSubmitAction = New PdfSubmitAction("http://stevex.net/dump.php")
        /// submitAction.DataFormat = SubmitDataFormat.Html
        /// 'Create submit button to transfer the values in the form
        /// Dim submitButton As PdfButtonField = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(100, 500, 90, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Submit"
        /// submitButton.Actions.MouseUp = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>   
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfSubmitAction"/> Class   
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
                    Widget.WidgetAppearance.NormalCaption = m_text;
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds Print action to current button field.
        /// <remarks>Clicking on the specified button will trigger the Print Dialog Box.</remarks>
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
        /// // Creating action
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// submitAction.DataFormat = SubmitDataFormat.Html;
        /// //Create submit button to transfer the values in the form
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(100, 500, 90, 20);
        /// submitButton.Font = font;
        /// submitButton.Text = "Submit";
        /// // Subscribing print action
        /// submitButton.AddPrintAction();
        /// submitButton.Actions.MouseUp = submitAction;
        /// document.Form.Fields.Add(submitButton);
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
        /// ' Creating action
        /// Dim submitAction As PdfSubmitAction = New PdfSubmitAction("http://stevex.net/dump.php")
        /// submitAction.DataFormat = SubmitDataFormat.Html
        /// 'Create submit button to transfer the values in the form
        /// Dim submitButton As PdfButtonField = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(100, 500, 90, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Submit"
        /// ' Subscribing print action
        /// submitButton.AddPrintAction()
        /// submitButton.Actions.MouseUp = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfAppearanceField"/> Class   
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfSubmitAction"/> Class           
        public void AddPrintAction()
        {
            PdfDictionary actionDictionary = new PdfDictionary();
            actionDictionary.SetProperty(DictionaryProperties.N, new PdfName(DictionaryProperties.Print));
            actionDictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.Named));
            PdfArray kidsArray = Dictionary[DictionaryProperties.Kids] as PdfArray;
            PdfReferenceHolder buttonObject = kidsArray[0] as PdfReferenceHolder;
            PdfDictionary buttonDictionary = buttonObject.Object as PdfDictionary;
            buttonDictionary.SetProperty(DictionaryProperties.A, actionDictionary);
        }

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

                FieldPainter.DrawButton(template.Graphics, parameters, Text, font, StringFormat);

                Page.Graphics.DrawPdfTemplate(template, Bounds.Location, rect.Size);
            }
        }

        /// <summary>
        /// Saves an instance.
        /// </summary>
        internal override void Save()
        {
            base.Save();

            if (Form != null && !Form.NeedAppearances)
            {
                if (Widget.Appearance.GetPressedTemplate() == null)
                {
                    DrawPressedAppearance(Widget.Appearance.Pressed);
                }
            }
        }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.FT, new PdfName(DictionaryProperties.Btn));

            BackColor = new PdfColor(255, 211, 211, 211);
            Flags |= FieldFlags.PushButton;
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

            FieldPainter.DrawButton(template.Graphics, paintParams, Text, GetFont(), StringFormat);
        }

        /// <summary>
        /// Draws the pressed appearance.
        /// </summary>
        /// <param name="template">The template.</param>
        protected void DrawPressedAppearance(PdfTemplate template)
        {
            PaintParams paintParams = new PaintParams(
                new RectangleF(PointF.Empty, Size), BackBrush, ForeBrush,
                BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

            FieldPainter.DrawPressedButton(template.Graphics, paintParams, Text, GetFont(), StringFormat);
        }
        #endregion
    }
}
