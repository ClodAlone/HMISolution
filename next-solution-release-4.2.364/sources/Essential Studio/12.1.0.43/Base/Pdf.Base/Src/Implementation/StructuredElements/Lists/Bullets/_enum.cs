#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Pdf.Lists
{
    /// <summary>
    /// Specifies the marker style.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfGraphics graphics = page.Graphics;
    /// //Create a unordered list
    /// PdfUnorderedList list = new PdfUnorderedList();            
    /// //Set the marker style
    /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk;
    /// //Create a font and write title
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
    /// //Create string format
    /// PdfStringFormat format = new PdfStringFormat();
    /// format.LineSpacing = 20f;
    /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
    /// //Apply formattings to list
    /// list.Font = font;
    /// list.StringFormat = format;
    /// //Set list indent
    /// list.Indent = 10;
    /// //Add items to the list
    /// list.Items.Add("List of Essential Studio products");
    /// list.Items.Add("IO products");
    /// //Set text indent
    /// list.TextIndent = 10;
    /// //Draw list
    /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
    /// document.Save("UnOrderList.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim graphics As PdfGraphics = page.Graphics
    /// 'Create a unordered list
    /// Dim list As PdfUnorderedList = New PdfUnorderedList()
    /// 'Set the marker style
    /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
    /// 'Create a font and write title
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
    /// 'Create string format
    /// Dim format As PdfStringFormat = New PdfStringFormat()
    /// format.LineSpacing = 20f
    /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
    /// 'Apply formattings to list
    /// list.Font = font
    /// list.StringFormat = format
    /// 'Set list indent
    /// list.Indent = 10
    /// 'Add items to the list
    /// list.Items.Add("List of Essential Studio products")
    /// list.Items.Add("IO products")
    /// 'Set text indent
    /// list.TextIndent = 10
    /// 'Draw list
    /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
    /// document.Save("UnOrderList.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfMarker"/> Class    
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfFont"/> Class   
    /// <seealso cref="PdfUnorderedList"/> Class 
    public enum PdfUnorderedMarkerStyle
    {
        /// <summary>
        /// Marker have  no style.
        /// </summary>
        None = 0,

        /// <summary>
        /// Marker is like a disk.
        /// </summary>
        Disk = 1,

        /// <summary>
        /// Marker is like a square.
        /// </summary>
        Square = 2,

        /// <summary>
        /// Marker is like a Asterisk.
        /// </summary>
        Asterisk = 3,

        /// <summary>
        /// Marker is like a circle.
        /// </summary>
        Circle = 4,

        /// <summary>
        /// Marker is custom string.
        /// </summary>
        CustomString = 5,

        /// <summary>
        /// Marker is custom image.
        /// </summary>
        CustomImage = 6,

        /// <summary>
        /// Marker is custom template.
        /// </summary>
        CustomTemplate = 7
    }

    /// <summary>
    /// Represents marker alignment.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfGraphics graphics = page.Graphics;
    /// //Create a unordered list
    /// PdfUnorderedList list = new PdfUnorderedList();            
    /// //Set the marker style
    /// list.Marker.Alignment = PdfListMarkerAlignment.Left
    /// //Create a font and write title
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
    /// //Create string format
    /// PdfStringFormat format = new PdfStringFormat();
    /// format.LineSpacing = 20f;
    /// font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
    /// //Apply formattings to list
    /// list.Font = font;
    /// list.StringFormat = format;
    /// //Set list indent
    /// list.Indent = 10;
    /// //Add items to the list
    /// list.Items.Add("List of Essential Studio products");
    /// list.Items.Add("IO products");
    /// //Set text indent
    /// list.TextIndent = 10;
    /// //Draw list
    /// list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
    /// document.Save("UnOrderList.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim graphics As PdfGraphics = page.Graphics
    /// 'Create a unordered list
    /// Dim list As PdfUnorderedList = New PdfUnorderedList()
    /// 'Set the marker alignment
    /// list.Marker.Alignment = PdfListMarkerAlignment.Left
    /// 'Create a font and write title
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
    /// 'Create string format
    /// Dim format As PdfStringFormat = New PdfStringFormat()
    /// format.LineSpacing = 20f
    /// font = New PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold)
    /// 'Apply formattings to list
    /// list.Font = font
    /// list.StringFormat = format
    /// 'Set list indent
    /// list.Indent = 10
    /// 'Add items to the list
    /// list.Items.Add("List of Essential Studio products")
    /// list.Items.Add("IO products")
    /// 'Set text indent
    /// list.TextIndent = 10
    /// 'Draw list
    /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
    /// document.Save("UnOrderList.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfMarker"/> Class    
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfFont"/> Class   
    /// <seealso cref="PdfUnorderedList"/> Class 
    public enum PdfListMarkerAlignment
    {
        /// <summary>
        /// Left alignment for marker.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Right alignment for marker.
        /// </summary>
        Right = 1
    }
}
