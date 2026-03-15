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
    /// Represents base class for field which can be in checked and unchecked states.
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
    /// <seealso cref="PdfStyledField"/> Class 
    /// <seealso cref="PdfPage"/> Class 
    /// <seealso cref="PdfDocument"/> Class 
    /// <seealso cref="PdfFont"/> Class 
    public class PdfCheckFieldBase : PdfStyledField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store check box' style.
        /// </summary>
        private PdfCheckBoxStyle m_style = PdfCheckBoxStyle.Check;

        /// <summary>
        /// Internal variable to store template for checked state.
        /// </summary>
        private PdfTemplate m_checkedTemplate = null;

        /// <summary>
        /// Internal variable to store template for unchecked state.
        /// </summary>
        private PdfTemplate m_uncheckedTemplate = null;

        /// <summary>
        /// Internal variable to store template for pressed checked state.
        /// </summary>
        private PdfTemplate m_pressedCheckedTemplate = null;

        /// <summary>
        /// Internal variable to store template for presssed unchecked state.
        /// </summary>
        private PdfTemplate m_pressedUncheckedTemplate = null;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCheckFieldBase"/> class.
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
        /// <seealso cref="PdfStyledField"/> Class 
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfFont"/> Class 
        public PdfCheckFieldBase(PdfPageBase page, string name)
            : base(page, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCheckFieldBase"/> class.
        /// </summary>
        internal PdfCheckFieldBase()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The <see cref="PdfCheckBoxStyle"/> object specifies the style of the check box field.</value>
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
        /// checkBox.Style = PdfCheckBoxStyle.Circle;        
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
        /// checkBox.Style = PdfCheckBoxStyle.Circle        
        /// checkBox.Bounds = New RectangleF(100, 290, 20, 20)
        /// document.Form.Fields.Add(checkBox)
        /// checkBox.HighlightMode = PdfHighlightMode.Push
        /// checkBox.BorderStyle = PdfBorderStyle.Beveled
        /// 'Set the value for the check box
        /// checkBox.Checked = True
        /// document.Form.Fields.Add(checkBox);
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfStyledField"/> Class 
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfDocument"/> Class 
        /// <seealso cref="PdfFont"/> Class 
        public PdfCheckBoxStyle Style
        {
            get
            {
                return m_style;
            }

            set
            {
                if (m_style != value)
                {
                    m_style = value;
                    Widget.WidgetAppearance.NormalCaption = StyleToString(m_style);
                }
            }
        }

        /// <summary>
        /// Gets or sets the checked template.
        /// </summary>
        /// <value>The checked template.</value>
        internal PdfTemplate CheckedTemplate
        {
            get
            {
                return m_checkedTemplate;
            }

            set
            {
                m_checkedTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the unchecked template.
        /// </summary>
        /// <value>The unchecked template.</value>
        internal PdfTemplate UncheckedTemplate
        {
            get
            {
                return m_uncheckedTemplate;
            }

            set
            {
                m_uncheckedTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the pressed checked template.
        /// </summary>
        /// <value>The pressed checked template.</value>
        internal PdfTemplate PressedCheckedTemplate
        {
            get
            {
                return m_pressedCheckedTemplate;
            }

            set
            {
                m_pressedCheckedTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the pressed unchecked template.
        /// </summary>
        /// <value>The pressed unchecked template.</value>
        internal PdfTemplate PressedUncheckedTemplate
        {
            get
            {
                return m_pressedUncheckedTemplate;
            }

            set
            {
                m_pressedUncheckedTemplate = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Styles to string.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>String representation of the check box' style.</returns>
        protected string StyleToString(PdfCheckBoxStyle style)
        {
            switch (style)
            {
                case PdfCheckBoxStyle.Check:
                default:
                    return "4";

                case PdfCheckBoxStyle.Circle:
                    return "l";

                case PdfCheckBoxStyle.Cross:
                    return "8";

                case PdfCheckBoxStyle.Diamond:
                    return "u";

                case PdfCheckBoxStyle.Square:
                    return "n";

                case PdfCheckBoxStyle.Star:
                    return "H";
            }
        }

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.FT, new PdfName(DictionaryProperties.Btn));
        }

        /// <summary>
        /// Saves an instance.
        /// </summary>
        internal override void Save()
        {
            base.Save();

            if (Form != null)
            {
                CreateTemplate(ref m_checkedTemplate);
                CreateTemplate(ref m_uncheckedTemplate);
                CreateTemplate(ref m_pressedCheckedTemplate);
                CreateTemplate(ref m_pressedUncheckedTemplate);

                Widget.ExtendedAppearance.Normal.On = m_checkedTemplate;
                Widget.ExtendedAppearance.Normal.Off = m_uncheckedTemplate;
                Widget.ExtendedAppearance.Pressed.On = m_pressedCheckedTemplate;
                Widget.ExtendedAppearance.Pressed.Off = m_pressedUncheckedTemplate;

                DrawAppearance();
            }
            else
            {
                ReleaseTemplate(m_checkedTemplate);
                ReleaseTemplate(m_uncheckedTemplate);
                ReleaseTemplate(m_pressedCheckedTemplate);
                ReleaseTemplate(m_pressedUncheckedTemplate);
            }
        }

        /// <summary>
        /// Draws the appearance.
        /// </summary>
        protected virtual void DrawAppearance()
        {
        }

        /// <summary>
        /// Ensures the template is created.
        /// </summary>
        /// <param name="template">The template.</param>
        private void CreateTemplate(ref PdfTemplate template)
        {
            if (template == null)
            {
                template = new PdfTemplate(Size);
            }
            else
            {
                template.Reset(Size);
            }
        }

        /// <summary>
        /// Releases the template.
        /// </summary>
        private void ReleaseTemplate(PdfTemplate template)
        {
            if (template != null)
            {
                template.Reset();
                Widget.ExtendedAppearance = null;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();
        }
        #endregion
    }
}