#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents class which can concatenate multiple automatic fields into single string.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();            
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
    /// PdfBrush brush = PdfBrushes.Black;
    /// PdfCompositeField compositeField = new PdfCompositeField(font, brush);
    /// compositeField.Text = "AutomaticFields";
    /// for (int i = 0; i < 3; i++)
    /// { 
    ///  //Creates a new page and adds it as the last page of the document
    ///  PdfPage page = doc.Pages.Add();
    ///  compositeField.Draw(page.Graphics);
    /// }
    /// doc.Save("AutomaticField.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
    /// Dim brush As PdfBrush = PdfBrushes.Black
    /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font, brush)
    /// compositeField.Text = "AutomaticFields"
    /// For i As Integer = 0 To 2
    ///  ' Create a page
    ///  Dim page As PdfPage = doc.Pages.Add()
    ///  compositeField.Draw(page.Graphics)
    /// Next i
    /// doc.Save("AutomaticField.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfMultipleValueField"/> Class    
    public class PdfCompositeField : PdfMultipleValueField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store list of automatic fields.
        /// </summary>
        private PdfAutomaticField[] m_automaticFields = null;

        /// <summary>
        /// Internal variable to store value.
        /// </summary>
        private string m_text = String.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCompositeField"/> class.
        /// </summary>  
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();                   
        /// PdfCompositeField compositeField = new PdfCompositeField();
        /// compositeField.Text = "AutomaticFields";
        /// for (int i = 0; i < 3; i++)
        /// { 
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  compositeField.Draw(page.Graphics);
        /// }
        /// doc.Save("AutomaticField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()      
        /// Dim compositeField As PdfCompositeField = New PdfCompositeField()
        /// compositeField.Text = "AutomaticFields"
        /// For i As Integer = 0 To 2
        ///  ' Create a page
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  compositeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("AutomaticField.pdf")
        /// </code>
        /// </example>
        public PdfCompositeField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCompositeField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/>object that specifies the font attributes (the family name, the size, and the style of the font) to use. </param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();            
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);        
        /// PdfCompositeField compositeField = new PdfCompositeField(font);
        /// compositeField.Text = "AutomaticFields";
        /// for (int i = 0; i < 3; i++)
        /// { 
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  compositeField.Draw(page.Graphics);
        /// }
        /// doc.Save("AutomaticField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)        
        /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font)
        /// compositeField.Text = "AutomaticFields"
        /// For i As Integer = 0 To 2
        ///  ' Create a page
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  compositeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("AutomaticField.pdf")
        /// </code>
        /// </example>
        public PdfCompositeField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCompositeField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();            
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfCompositeField compositeField = new PdfCompositeField(font, brush);
        /// compositeField.Text = "AutomaticFields";
        /// for (int i = 0; i < 3; i++)
        /// { 
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  compositeField.Draw(page.Graphics);
        /// }
        /// doc.Save("AutomaticField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font, brush)
        /// compositeField.Text = "AutomaticFields"
        /// For i As Integer = 0 To 2
        ///  ' Create a page
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  compositeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("AutomaticField.pdf")
        /// </code>
        /// </example>
        public PdfCompositeField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCompositeField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/> object that specifies the font attributes (the family name, the size, and the style of the font) to use. </param>
        /// <param name="text">The wide-character string to be drawn.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();            
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfCompositeField compositeField = new PdfCompositeField(font, "AutomaticFields");            
        /// for (int i = 0; i < 3; i++)
        /// { 
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  compositeField.Draw(page.Graphics);
        /// }
        /// doc.Save("AutomaticField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font, "AutomaticFields")
        /// For i As Integer = 0 To 2
        ///  '  Create a page
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  compositeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("AutomaticField.pdf")
        /// </code>
        /// </example>
        public PdfCompositeField(PdfFont font, string text)
            : base(font)
        {
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCompositeField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/> object that specifies the font attributes (the family name, the size, and the style of the font) to use. </param>
        /// <param name="text">The wide-character string to be drawn.</param>
        /// <param name="brush">A <see cref="PdfBrush"/> object that is used to fill the string. </param> 
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();            
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfCompositeField compositeField = new PdfCompositeField(font,PdfBrushes.Blue, "AutomaticFields");            
        /// for (int i = 0; i < 3; i++)
        /// { 
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  compositeField.Draw(page.Graphics);
        /// }
        /// doc.Save("AutomaticField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font,PdfBrushes.Blue, "AutomaticFields")
        /// For i As Integer = 0 To 2
        ///  ' Create a page 
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  compositeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("AutomaticField.pdf")
        /// </code>
        /// </example>
        public PdfCompositeField(PdfFont font, PdfBrush brush, string text)
            : base(font, brush)
        {
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCompositeField"/> class.
        /// </summary>
        /// <param name="text">The wide-character string to be drawn.</param>
        /// <param name="list">The list of <see cref="PdfAutomaticField"/> objects.</param>
        public PdfCompositeField(string text, params PdfAutomaticField[] list)
            : base()
        {
            m_automaticFields = list;
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCompositeField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/> object that specifies the font attributes (the family name, the size, and the style of the font) to use.</param>
        /// <param name="text">The wide-character string to be drawn.</param>
        /// <param name="list">The list of <see cref="PdfAutomaticField"/> objects.</param>
        public PdfCompositeField(PdfFont font, string text, params PdfAutomaticField[] list)
            : base(font)
        {
            Text = text;
            m_automaticFields = list;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCompositeField"/> class.
        /// </summary>
        /// <param name="font">A <see cref="PdfFont"/> object that specifies the font attributes (the family name, the size, and the style of the font) to use.</param>
        /// <param name="brush">A <see cref="PdfBrush"/> object that is used to fill the string. </param>
        /// <param name="text">The wide-character string to be drawn.</param>
        /// <param name="list">The list of <see cref="PdfAutomaticField"/> objects.</param>
        public PdfCompositeField(PdfFont font, PdfBrush brush, string text, params PdfAutomaticField[] list)
            : base(font, brush)
        {
            Text = text;
            m_automaticFields = list;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The wide-character string to be drawn.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();            
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfCompositeField compositeField = new PdfCompositeField(font, brush);
        /// compositeField.Text = "AutomaticFields";
        /// for (int i = 0; i < 3; i++)
        /// { 
        ///  //Creates a new page and adds it as the last page of the document
        ///  PdfPage page = doc.Pages.Add();
        ///  compositeField.Draw(page.Graphics);
        /// }
        /// doc.Save("AutomaticField.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font, brush)
        /// compositeField.Text = "AutomaticFields"
        /// For i As Integer = 0 To 2
        ///  ' Create a page
        ///  Dim page As PdfPage = doc.Pages.Add()
        ///  compositeField.Draw(page.Graphics)
        /// Next i
        /// doc.Save("AutomaticField.pdf")
        /// </code>
        /// </example>
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

                m_text = value;
            }
        }

        /// <summary>
        /// Gets or sets the automatic fields.
        /// </summary>
        /// <value>The automatic fields.</value>
        public PdfAutomaticField[] AutomaticFields
        {
            get
            {
                return m_automaticFields;
            }

            set
            {
                m_automaticFields = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the value of the field at the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns></returns>
        /// <exclude/>
        internal protected override string GetValue(PdfGraphics graphics)
        {
            string[] values = null;

            if (m_automaticFields != null && m_automaticFields.Length > 0)
            {
                values = new string[m_automaticFields.Length];

                int i = 0;
                foreach (PdfAutomaticField automaticField in m_automaticFields)
                {
                    values[i++] = automaticField.GetValue(graphics);
                }

                return String.Format(m_text, values);
            }

            return m_text;
        }
        #endregion
    }
}
