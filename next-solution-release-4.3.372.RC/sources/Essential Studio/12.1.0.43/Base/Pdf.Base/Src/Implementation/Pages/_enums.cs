#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Enumerator that implements page orientations.
    /// </summary>
    public enum PdfPageOrientation
    {
        /// <summary>
        /// Portrait orientation.
        /// </summary>
        Portrait,
        /// <summary>
        /// Landscape orientation.
        /// </summary>
        Landscape,
    }

    /// <summary>
    /// The number of degrees by which the page should be rotated clockwise when displayed or printed.
    /// </summary>
    public enum PdfPageRotateAngle
    {
        /// <summary>
        /// The page is rotated as 0 angle.
        /// </summary>
        RotateAngle0,
        /// <summary>
        /// The page is rotated as 90 angle.
        /// </summary>
        RotateAngle90,
        /// <summary>
        /// The page is rotated as 180 angle.
        /// </summary>
        RotateAngle180,
        /// <summary>
        /// The page is rotated as 270 angle.
        /// </summary>
        RotateAngle270
    }

    /// <summary>
    /// Specifies numbering style of page labels.
    /// </summary>
    public enum PdfNumberStyle
    {
        /// <summary>
        /// No numbering at all.
        /// </summary>
        None = 0,
        /// <summary>
        /// Decimal arabic numerals.
        /// </summary>
        Numeric,
        /// <summary>
        /// Lowercase letters a-z.
        /// </summary>
        LowerLatin,
        /// <summary>
        /// Lowercase roman numerals.
        /// </summary>
        LowerRoman,
        /// <summary>
        /// Uppercase letters A-Z.
        /// </summary>
        UpperLatin,
        /// <summary>
        /// Uppercase roman numerals.
        /// </summary>
        UpperRoman
    }

    /// <summary>
    /// Specifies the docking style of the page template.
    /// </summary>
    /// <remarks>This enumeration is used in <see cref="PdfPageTemplateElement"/> class.</remarks>
    /// <example>
    /// <code lang="C#">
    /// //Create a PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Create a page
    /// PdfPage page = doc.Pages.Add();
    /// RectangleF rect = new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height);
    /// //Create a page template
    /// PdfPageTemplateElement footer = new PdfPageTemplateElement(rect);
    /// footer.Dock = PdfDockStyle.Right;
    /// //Set the template alignment as top right
    /// footer.Alignment = PdfAlignmentStyle.TopRight;
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
    /// PdfSolidBrush brush = new PdfSolidBrush(Color.Gray);
    /// //Create page number field
    /// PdfPageNumberField pageNumber = new PdfPageNumberField(font, brush);            
    /// //Create page count field
    /// PdfPageCountField count = new PdfPageCountField(font, brush);
    /// PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count);
    /// compositeField.Bounds = footer.Bounds;            
    /// compositeField.Draw(footer.Graphics, new PointF(40, footer.Height - 50));
    /// //Add the footer template at the bottom
    /// doc.Template.Right = footer;
    /// doc.Save("Template.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a PDF document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim rect As RectangleF = New RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height)
    /// 'Create a page template
    /// Dim footer As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// footer.Dock = PdfDockStyle.Right
    /// 'Set the template alignment as top right
    /// footer.Alignment = PdfAlignmentStyle.TopRight
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
    /// Dim brush As PdfSolidBrush = New PdfSolidBrush(Color.Gray)
    /// 'Create page number field
    /// Dim pageNumber As PdfPageNumberField = New PdfPageNumberField(font, brush)
    /// 'Create page count field
    /// Dim count As PdfPageCountField = New PdfPageCountField(font, brush)
    /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count)
    /// compositeField.Bounds = footer.Bounds
    /// compositeField.Draw(footer.Graphics, New PointF(40, footer.Height - 50))
    /// 'Add the footer template at the bottom
    /// doc.Template.Right = footer
    /// doc.Save("Template.pdf")
    /// </code>
    /// </example>
    public enum PdfDockStyle
    {
        /// <summary>
        /// The page template is not docked.
        /// </summary>
        None,
        /// <summary>
        /// The page template edge is docked to the bottom page's side.
        /// </summary>
        Bottom,
        /// <summary>
        /// The page template edge is docked to the top page's side.
        /// </summary>
        Top,
        /// <summary>
        /// The page template edge is docked to the left page's side.
        /// </summary>
        Left,
        /// <summary>
        /// The page template edge is docked to the right page's side.
        /// </summary>
        Right,
        /// <summary>
        /// The page template stretch on full page.
        /// </summary>
        Fill
    }

    /// <summary>
    /// Specifies how the page template is aligned relative to the template area.    
    /// </summary>
    /// <example>
    /// <code lang="C#">    
    /// //Create a PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Create a page
    /// PdfPage page = doc.Pages.Add();
    /// RectangleF rect = new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height);
    /// //Create a page template
    /// PdfPageTemplateElement footer = new PdfPageTemplateElement(rect);
    /// //Set the template alignment as top right
    /// footer.Alignment = PdfAlignmentStyle.TopRight;
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
    /// PdfSolidBrush brush = new PdfSolidBrush(Color.Gray);
    /// //Create page number field
    /// PdfPageNumberField pageNumber = new PdfPageNumberField(font, brush);
    /// //Create page count field
    /// PdfPageCountField count = new PdfPageCountField(font, brush);
    /// PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count);
    /// compositeField.Bounds = footer.Bounds;
    /// compositeField.Draw(footer.Graphics, new PointF(40, footer.Height - 50));
    /// //Add the footer template at the bottom
    /// doc.Template.Bottom = footer;
    /// doc.Save("Template.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a PDF document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim rect As RectangleF = New RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height)
    /// 'Create a page template
    /// Dim footer As PdfPageTemplateElement = New PdfPageTemplateElement(rect)
    /// 'Set the template alignment as top right
    /// footer.Alignment = PdfAlignmentStyle.TopRight
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 8)
    /// Dim brush As PdfSolidBrush = New PdfSolidBrush(Color.Gray)
    /// 'Create page number field
    /// Dim pageNumber As PdfPageNumberField = New PdfPageNumberField(font, brush)
    /// 'Create page count field
    /// Dim count As PdfPageCountField = New PdfPageCountField(font, brush)
    /// Dim compositeField As PdfCompositeField = New PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count)
    /// compositeField.Bounds = footer.Bounds
    /// compositeField.Draw(footer.Graphics, New PointF(40, footer.Height - 50))
    /// 'Add the footer template at the bottom
    /// doc.Template.Bottom = footer
    /// doc.Save("Template.pdf")
    /// </code>
    /// </example>
    /// <remarks>This enumeration is used in <see cref="PdfPageTemplateElement"/> class.</remarks>
    public enum PdfAlignmentStyle
    {
        /// <summary>
        /// Specifies no alignment.
        /// </summary>
        None,
        /// <summary>
        /// The template is top left aligned.
        /// </summary>
        TopLeft,
        /// <summary>
        /// The template is top center aligned.
        /// </summary>
        TopCenter,
        /// <summary>
        /// The template is top right aligned.
        /// </summary>
        TopRight,
        /// <summary>
        /// The template is middle left aligned.
        /// </summary>
        MiddleLeft,
        /// <summary>
        /// The template is middle center aligned.
        /// </summary>
        MiddleCenter,
        /// <summary>
        /// The template is middle right aligned.
        /// </summary>
        MiddleRight,
        /// <summary>
        /// The template is bottom left aligned.
        /// </summary>
        BottomLeft,
        /// <summary>
        /// The template is bottom center aligned.
        /// </summary>
        BottomCenter,
        /// <summary>
        ///  The template is bottom right aligned.
        /// </summary>
        BottomRight,
    }

    /// <summary>
    /// A name object specifying the page layout to be used when the
    /// document is opened.
    /// </summary>
    public enum PdfPageLayout
    {
        /// <summary>
        /// Default Value. Display one page at a time.
        /// </summary>
        SinglePage = 0,
        /// <summary>
        /// Display the pages in one column.
        /// </summary>
        OneColumn,
        /// <summary>
        /// Display the pages in two columns, with odd numbered
        /// pages on the left.
        /// </summary>
        TwoColumnLeft,
        /// <summary>
        /// Display the pages in two columns, with odd numbered
        /// pages on the right.
        /// </summary>
        TwoColumnRight,
        /// <summary>
        /// Display the pages two at a time, with odd-numbered pages on the left
        /// </summary>
        TwoPageLeft,
        /// <summary>
        /// Display the pages two at a time, with odd-numbered pages on the right
        /// </summary>
        TwoPageRight
    }

    /// <summary>
    /// Represents mode of document displaying.
    /// </summary>
    public enum PdfPageMode
    {
        /// <summary>
        /// Default value. Neither document outline nor thumbnail images visible.
        /// </summary>
        UseNone = 0,
        /// <summary>
        /// Document outline visible.
        /// </summary>
        UseOutlines,
        /// <summary>
        /// Thumbnail images visible.
        /// </summary>
        UseThumbs,
        /// <summary>
        /// Full-screen mode, with no menu bar, window
        /// controls, or any other window visible.
        /// </summary>
        FullScreen,
        /// <summary>
        /// Optional content group panel visible.
        /// </summary>
        UseOC,
        /// <summary>
        /// Attachments are visible.
        /// </summary>
        UseAttachments
    }

    /// <summary>
    /// TemplateArea can be header/footer on of the following types. 
    /// </summary>
    internal enum TemplateType
    {
        /// <summary>
        /// Page template is not used as header.
        /// </summary>
        None,
        /// <summary>
        /// Page template is used as Top.
        /// </summary>
        Top,
        /// <summary>
        /// Page template is used as Bottom.
        /// </summary>
        Bottom,
        /// <summary>
        /// Page template is used as Left.
        /// </summary>
        Left,
        /// <summary>
        /// Page template is used as Right.
        /// </summary>
        Right
    }

    /// <summary>
    /// Enumeration of possible transition styles when moving to the page from another 
    /// during a presentation
    /// </summary>
    public enum PdfTransitionStyle
    {
        /// <summary>
        /// Two lines sweep across the screen, revealing the new page. The lines may be either 
        /// horizontal or vertical and may move inward from the edges of the page or outward 
        /// from the center.
        /// </summary>
        Split,
        /// <summary>
        /// Multiple lines, evenly spaced across the screen, synchronously sweep in the same 
        /// direction to reveal the new page. The lines may be either horizontal or vertical.
        /// Horizontal lines move downward; vertical lines move to the right.
        /// </summary>
        Blinds,
        /// <summary>
        /// A rectangular box sweeps inward from the edges of the page or outward from the center,
        /// revealing the new page.
        /// </summary>
        Box,
        /// <summary>
        /// A single line sweeps across the screen from one edge to the other, revealing the new page.
        /// </summary>
        Wipe,
        /// <summary>
        /// The old page dissolves gradually to reveal the new one.
        /// </summary>
        Dissolve,
        /// <summary>
        /// Similar to Dissolve, except that the effect sweeps across the page in a wide band moving from 
        /// one side of the screen to the other.
        /// </summary>
        Glitter,
        /// <summary>
        /// The new page simply replaces the old one with no special transition effect.
        /// </summary>
        Replace,
        /// <summary>
        /// Changes are flown out or in, to or from a location that is offscreen.
        /// </summary>
        Fly,
        /// <summary>
        /// The old page slides off the screen while the new page slides in, pushing the old page out.
        /// </summary>
        Push,
        /// <summary>
        /// The new page slides on to the screen, covering the old page.
        /// </summary>
        Cover,
        /// <summary>
        /// The old page slides off the screen, uncovering the new page.
        /// </summary>
        Uncover,
        /// <summary>
        /// The new page gradually becomes visible through the old one.
        /// </summary>
        Fade
    }

    /// <summary>
    /// Enumeration of transition dimensions.
    /// </summary>
    public enum PdfTransitionDimension
    {
        /// <summary>
        /// Horizontal effect.
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical effect.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Enumeration of transition motions.
    /// </summary>
    public enum PdfTransitionMotion
    {
        /// <summary>
        /// Inward motion from the edges of the page to center..
        /// </summary>
        Inward,
        /// <summary>
        /// Outward motion from the center of the page to edges.
        /// </summary>
        Outward
    }

    /// <summary>
    /// Enumeration of transition directions.
    /// </summary>
    public enum PdfTransitionDirection
    {
        /// <summary>
        /// Left to Right direction.
        /// </summary>
        LeftToRight = 0,
        /// <summary>
        /// Bottom to Top direction.
        /// </summary>
        BottomToTop = 90,
        /// <summary>
        /// Right to Left direction.
        /// </summary>
        RightToLeft = 180,
        /// <summary>
        /// Top to Bottom direction.
        /// </summary>
        TopToBottom = 270,
        /// <summary>
        /// TopLeft to BottomRight direction.
        /// </summary>
        TopLeftToBottomRight = 315
    }
}