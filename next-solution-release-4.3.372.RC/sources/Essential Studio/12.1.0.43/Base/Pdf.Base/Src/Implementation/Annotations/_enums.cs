#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Specifies the name of an icon to be used in displaying the sound annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new cectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new sound annotation.
    /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
    /// //Sets the sound icon
    /// soundAnnotation.Icon = PdfSoundIcon.Speaker;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(soundAnnotation);
    /// //Save the  document to disk.
    /// document.Save("SoundIcon.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new sound annotation.
    /// 'Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Datastartup.wav")
    /// 'Sets the sound icon
    ///  soundAnnotation.Icon = PdfSoundIcon.Speaker
    /// 'Add this annotation to a new page.
    ///  page.Annotations.Add(soundAnnotation)
    /// 'Save the  document to disk.
    ///  document.Save("SoundIcon.pdf")
    /// </code>
    /// </example> 
    public enum PdfSoundIcon
    {
        /// <summary>
        /// Speaker icon of sound link.
        /// </summary>
        Speaker,

        /// <summary>
        /// Microphone icon of sound link.
        /// </summary>
        Mic
    }

    /// <summary>
    /// Specifies the type of icon to be used in displaying file attachment annotations.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF attachmentRectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new attachment annotation.
    /// PdfAttachmentAnnotation attachmentAnnotation = new PdfAttachmentAnnotation(attachmentRectangle,@"..\..\Images\business.jpg");
    /// //Set the Attachment icon to attachment annotation.
    /// attachmentAnnotation.Icon = PdfAttachmentIcon.PushPin;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(attachmentAnnotation);
    /// //Save the  document to disk.
    /// document.Save("AttachmentIcon.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim attachmentRectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new pdf attachment annotation.
    /// Dim attachmentAnnotation As PdfAttachmentAnnotation = New PdfAttachmentAnnotation(attachmentRectangle, "..\..\Images\business.jpg")
    /// 'Set the attachment icon.
    /// attachmentAnnotation.Icon = PdfAttachmentIcon.PushPin
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(attachmentAnnotation)
    /// 'Save the  document to disk.
    /// document.Save("AttachmentIcon.pdf")
    /// </code>
    /// </example> 
    public enum PdfAttachmentIcon
    {
        /// <summary>
        /// Type of icon used in file attachment annotation.
        /// </summary>
        PushPin,

        /// <summary>
        /// Type of icon used in file attachment annotation.
        /// </summary>
        Tag,

        /// <summary>
        /// Type of icon used in file attachment annotation.
        /// </summary>
        Graph,

        /// <summary>
        /// Type of icon used in file attachment annotation.
        /// </summary>
        Paperclip
    }

    /// <summary>
    /// Specifies the enumeration of the annotation flags.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF docLinkAnnotationRectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new document link annotation.
    /// PdfDocumentLinkAnnotation documentAnnotation = new PdfDocumentLinkAnnotation(docLinkAnnotationRectangle);
    /// //Set the annotation flags to document annotation.
    /// documentAnnotation.AnnotationFlags = PdfAnnotationFlags.NoRotate;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(attachmentAnnotation);
    /// //Save the document to disk.
    /// document.Save("AnnotationFlags.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim docLinkAnnotationRectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new document link annotation.
    /// Dim documentAnnotation As PdfDocumentLinkAnnotation = New PdfDocumentLinkAnnotation(docLinkAnnotationRectangle)
    /// 'Set the annotation flags .
    /// documentAnnotation.AnnotationFlags = PdfAnnotationFlags.NoRotate
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(documentAnnotation)
    /// 'Save the  document to disk.
    /// document.Save("AnnotationFlags.pdf")
    /// </code>
    /// </example> 
    [Flags]
    public enum PdfAnnotationFlags
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        Invisible = 1,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        Hidden = 2,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        Print = 4,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        NoZoom = 8,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        NoRotate = 16,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        NoView = 32,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        ReadOnly = 64,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        Locked = 128,

        /// <summary>
        /// Annotation flag's key.
        /// </summary>
        ToggleNoView = 256
    }

    /// <summary>
    /// Specifies the enumeration of popup annotation icons.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF popupAnnotationRectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new popup annotation.
    /// PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(popupAnnotationRectangle,"Test popup annotation");
    /// //Sets the popup icon.
    /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(popupAnnotation);
    /// //Save the  document to disk.
    /// document.Save("PopupIcon.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim popupAnnotationRectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new popup annotation.
    /// Dim popupAnnotation As PdfPopupAnnotation = New PdfPopupAnnotation(popupAnnotationRectangle, "Test popup annotation")
    /// 'Set the popup icon.
    /// popupAnnotation.Icon = PdfPopupIcon.NewParagraph
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(popupAnnotation)
    /// 'Save the  document to disk.
    /// document.Save("PopupIcon.pdf")
    /// </code>
    /// </example> 
    public enum PdfPopupIcon
    {
        /// <summary>
        /// Indicates note popup annotation.
        /// </summary>
        Note,

        /// <summary>
        /// Indicates comment popup annotation.
        /// </summary>
        Comment,

        /// <summary>
        /// Indicates help popup annotation.
        /// </summary>
        Help,

        /// <summary>
        /// Indicates insert popup annotation.
        /// </summary>
        Insert,

        /// <summary>
        /// Indicates key popup annotation.
        /// </summary>
        Key,

        /// <summary>
        /// Indicates new paragraph popup annotation.
        /// </summary>
        NewParagraph,

        /// <summary>
        /// Indicates paragraph popup annotation.
        /// </summary>
        Paragraph
    }

    /// <summary>
    /// Specifies the enumeration of rubber stamp annotation icons.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedRubberStampAnnotation rubberStampAnnotation = document.Pages[1].Annotations[5] as PdfLoadedRubberStampAnnotation;
    /// //Set the icon
    /// rubberStampAnnotation.Icon = PdfRubberStampAnnotationIcon.Approved;
    /// //Save the document.
    /// document.Save("RubberStampAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim popupAnnotation As PdfLoadedRubberStampAnnotation = document.Pages(1).Annotations(5) as PdfLoadedRubberStampAnnotation
    /// 'Set the icon
    /// rubberStampAnnotation.Icon = PdfRubberStampAnnotationIcon.Approved
    /// 'Save the document.
    /// document.Save("RubberStampAnnotation.pdf")
    /// </code>
    /// </example>
    public enum PdfRubberStampAnnotationIcon
    {
        /// <summary>
        /// Indicates approved rubber stamp annotation
        /// </summary>
        Approved,

        /// <summary>
        /// Indicates AaIs rubber stamp annotation
        /// </summary>
        AsIs,

        /// <summary>
        /// Indicates confidential rubber stamp annotation
        /// </summary>
        Confidential,

        /// <summary>
        /// Indicates departmental rubber stamp annotation
        /// </summary>
        Departmental,

        /// <summary>
        /// Indicates draft rubber stamp annotation
        /// </summary>
        Draft,

        /// <summary>
        /// Indicates experimental rubber stamp annotation
        /// </summary>
        Experimental,

        /// <summary>
        /// Indicates expired rubber stamp annotation
        /// </summary>
        Expired,

        /// <summary>
        /// Indicates final rubber stamp annotation
        /// </summary>
        Final,

        /// <summary>
        /// Indicates for comment rubber stamp annotation
        /// </summary>
        ForComment,

        /// <summary>
        /// Indicates for public release rubber stamp annotation
        /// </summary>
        ForPublicRelease,

        /// <summary>
        /// Indicates not approved rubber stamp annotation
        /// </summary>
        NotApproved,

        /// <summary>
        /// Indicates not for public release rubber stamp annotation
        /// </summary>
        NotForPublicRelease,

        /// <summary>
        /// Indicates sold rubber stamp annotation
        /// </summary>
        Sold,

        /// <summary>
        /// Indicates topsecret rubber stamp annotation
        /// </summary>
        TopSecret,
    }

    /// <summary>
    /// Specifies the Line Ending Style to be used in the Line annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //To specify the line end points
    /// int[] points = new int[] { 80, 420, 150, 420 };
    /// //Create a new line annotation.
    ///  PdfLineAnnotation linkannotation = new PdfLineAnnotation(points, "Line Annoation");
    /// linkannotation.EndLineStyle = PdfLineEndingStyle.Diamond;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(linkannotation);
    /// //Save the  document to disk.
    /// document.Save("LineEndingStyle.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'To specify the line end points
    /// Dim points() As Integer = { 80, 420, 150, 420 }
    /// 'Create a new line annotation.
    /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
    /// linkannotation.EndLineStyle = PdfLineEndingStyle.Diamond
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(linkannotation)
    /// 'Save the  document to disk.
    /// document.Save("LineEndingStyle.pdf")
    /// </code>
    /// </example> 
    public enum PdfLineEndingStyle
    {
        /// <summary>
        /// Indicates Square
        /// </summary>
        Square,

        /// <summary>
        /// Indicates Circle
        /// </summary>
        Circle,

        /// <summary>
        /// Indicates Diamond
        /// </summary>
        Diamond,

        /// <summary>
        /// Indicates OpenArrow
        /// </summary>
        OpenArrow,

        /// <summary>
        /// Indicates ClosedArrow
        /// </summary>
        ClosedArrow,

        /// <summary>
        /// Indicates None
        /// </summary>
        None,

        /// <summary>
        /// Indicates ROpenArrow
        /// </summary>
        ROpenArrow,

        /// <summary>
        /// Indicates Butt
        /// </summary>
        Butt,

        /// <summary>
        /// IdicaIndicatestes RClosedArrow
        /// </summary>
        RClosedArrow,

        /// <summary>
        /// Indicates Slash
        /// </summary>
        Slash,
    }

    /// <summary>
    /// Specifies the Line Border Style is to be used in the Line annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //To specify the line end points
    /// int[] points = new int[] { 80, 420, 150, 420 };
    /// //Create a new line annotation.
    ///  PdfLineAnnotation lineannotation = new PdfLineAnnotation(points, "Line Annoation");
    /// //Set the line border style.
    /// lineannotation.PdfLineBorderStyle = PdfLineBorderStyle.Dashed;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(lineannotation);
    /// //Save the  document to disk.
    /// document.Save("LineBorderStyle.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'To specify the line end points
    /// Dim points() As Integer = { 80, 420, 150, 420 }
    /// 'Create a new line annotation.
    /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
    /// 'Set the line border style.
    /// lineannotation.PdfLineBorderStyle = PdfLineBorderStyle.Dashed
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(lineannotation)
    /// Save the  document to disk.
    /// document.Save("LineBorderStyle.pdf")
    /// </code>
    /// </example> 
    public enum PdfLineBorderStyle
    {
        /// <summary>
        /// Indicates Solid
        /// </summary>
        Solid,

        /// <summary>
        /// Indicates Dashed
        /// </summary>
        Dashed,

        /// <summary>
        /// Indicates Beveled
        /// </summary>
        Beveled,

        /// <summary>
        /// Indicates Inset
        /// </summary>
        Inset,

        /// <summary>
        /// Indicates Underline
        /// </summary>
        Underline,
    }

    /// <summary>
    /// Specifies the Line Intent Style is to be used in the Line annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //To specify the line end points
    /// int[] points = new int[] { 80, 420, 150, 420 };
    /// //Create a new line annotation.
    /// PdfLineAnnotation lineannotation = new PdfLineAnnotation(points, "Line Annoation");
    /// //Set the pdf line indent.
    /// lineannotation.PdfLineIntent = PdfLineIntent.LineArrow;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(lineannotation);
    /// //Save the  document to disk.
    /// document.Save("LineEndingStyle.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'To specify the line end points
    /// Dim points() As Integer = { 80, 420, 150, 420 }
    /// 'Create a new line annotation.
    /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
    /// 'Set the Line intent.
    /// lineannotation.PdfLineIntent = PdfLineIntent.LineArrow
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(lineannotation)
    /// Save the  document to disk.
    /// document.Save("LineEndingStyle.pdf")
    /// </code>
    /// </example> 
    public enum PdfLineIntent
    {
        /// <summary>
        /// Indicates Line Arrow as intent of the line annotation
        /// </summary>
        LineArrow,

        /// <summary>
        /// Indicates LineDimension as intent of the line annotation
        /// </summary>
        LineDimension,
    }

    /// <summary>
    /// Specifies the Line Caption Type is to be used in the Line annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //To specify the line end points
    /// int[] points = new int[] { 80, 420, 150, 420 };
    /// //Create a new line annotation.
    /// PdfLineAnnotation lineannotation = new PdfLineAnnotation(points, "Line Annoation");
    /// //Set the line caption type.
    /// lineannotation.PdfLineCaptionType = PdfLineCaptionType.Inline
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(lineannotation);
    /// //Save the  document to disk.
    /// document.Save("LineCaptionType.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'To specify the line end points
    /// Dim points() As Integer = { 80, 420, 150, 420 }
    /// 'Create a new line annotation.
    /// Dim lineannotation As PdfLineAnnotation = New PdfLineAnnotation(points, "Line Annoation")
    /// 'Set the line caption type.
    /// lineannotation.PdfLineCaptionType = PdfLineCaptionType.Inline
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(lineannotation)
    /// 'Save the  document to disk.
    /// document.Save("LineCaptionType.pdf")
    /// </code>
    /// </example> 
    public enum PdfLineCaptionType
    {
        /// <summary>
        /// Indicates Inline as annotation�s caption positioning
        /// </summary>
        Inline,

        /// <summary>
        /// Indicates Top as annotation�s caption positioning
        /// </summary>
        Top,
    }

    /// <summary>
    /// Specifies the Style of the Text Markup Annotation
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedTextMarkupAnnotation textMarkupAnnotation = document.Pages[1].Annotations[5] as PdfLoadedTextMarkupAnnotation;
    /// //Sets the pdf text markup annotation type
    /// textMarkupAnnotation.TextMarkupAnnotationType=PdfTextMarkupAnnotationType.Highlight
    /// //Save the document.
    /// document.Save("TextMarkupAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim textMarkupAnnotation As PdfLoadedTextMarkupAnnotation = document.Pages(1).Annotations(5) as PdfLoadedTextMarkupAnnotation
    /// 'Sets the pdf text markup annotation type
    /// textMarkupAnnotation.TextMarkupAnnotationType=PdfTextMarkupAnnotationType.Highlight
    /// 'Save the document.
    /// document.Save("TextMarkupAnnotation.pdf")
    /// </code>
    /// </example>
    public enum PdfTextMarkupAnnotationType
    {
        /// <summary>
        /// The Text Markup Annotation Type is Highlight.
        /// </summary>
        Highlight,

        /// <summary>
        /// The Text Markup Annotation Type is Underline.
        /// </summary>
        Underline,

        /// <summary>
        /// The Text Markup Annotation Type is Squiggly.
        /// </summary>
        Squiggly,

        /// <summary>
        /// The Text Markup Annotation Type is StrikeOut.
        /// </summary>
        StrikeOut,
    }

    /// <summary>
    /// Specifies the annotation types.
    /// </summary>
    public enum PdfLoadedAnnotationTypes
    {
        /// <summary>
        /// Highlight type annotation.
        /// </summary>
        Highlight,

        /// <summary>
        /// Underline type annotation.
        /// </summary>
        Underline,

        /// <summary>
        /// StrikeOut type annotation.
        /// </summary>
        StrikeOut,

        /// <summary>
        /// Squiggly type annotation.
        /// </summary>
        Squiggly,

        /// <summary>
        /// AnnotationStates type.
        /// </summary>
        AnnotationStates,

        /// <summary>
        /// TextAnnotation type.
        /// </summary>
        TextAnnotation,

        /// <summary>
        /// LinkAnnotation type.
        /// </summary>
        LinkAnnotation,

        /// <summary>
        /// DocumentLinkAnnotation type.
        /// </summary>
        DocumentLinkAnnotation,

        /// <summary>
        /// FileLinkAnnotation type.
        /// </summary>
        FileLinkAnnotation,

        /// <summary>
        /// FreeTextAnnotation type.
        /// </summary>
        FreeTextAnnotation,

        /// <summary>
        /// LineAnnotation type.
        /// </summary>
        LineAnnotation,

        /// <summary>
        /// SquareandCircleAnnotation type.
        /// </summary>
        SquareandCircleAnnotation,

        /// <summary>
        /// PolygonandPolylineAnnotation type.
        /// </summary>
        PolygonandPolylineAnnotation,

        /// <summary>
        /// TextMarkupAnnotation type.
        /// </summary>
        TextMarkupAnnotation,

        /// <summary>
        /// CaretAnnotation type.
        /// </summary>
        CaretAnnotation,

        /// <summary>
        /// RubberStampAnnotation type.
        /// </summary>
        RubberStampAnnotation,

        /// <summary>
        /// LnkAnnotation type.
        /// </summary>
        LnkAnnotation,

        /// <summary>
        /// PopupAnnotation type.
        /// </summary>
        PopupAnnotation,

        /// <summary>
        /// FileAttachmentAnnotation type.
        /// </summary>
        FileAttachmentAnnotation,

        /// <summary>
        /// SoundAnnotation type.
        /// </summary>
        SoundAnnotation,

        /// <summary>
        /// MovieAnnotation type.
        /// </summary>
        MovieAnnotation,

        /// <summary>
        /// ScreenAnnotation type.
        /// </summary>
        ScreenAnnotation,

        /// <summary>
        /// WidgetAnnotation type.
        /// </summary>
        WidgetAnnotation,

        /// <summary>
        /// PrinterMarkAnnotation type.
        /// </summary>
        PrinterMarkAnnotation,

        /// <summary>
        /// TrapNetworkAnnotation type.
        /// </summary>
        TrapNetworkAnnotation,

        /// <summary>
        /// WatermarkAnnotation type.
        /// </summary>
        WatermarkAnnotation,

        /// <summary>
        /// TextWebLinkAnnotation type.
        /// </summary>
        TextWebLinkAnnotation,

        /// <summary>
        /// InkAnnotation type
        /// </summary>
        InkAnnotation,

        /// <summary>
        /// No annotation.
        /// </summary>
        Null
    }

    public enum PdfAnnotationIntent
    {
        FreeTextCallout,
        FreeTextTypeWriter
    }
}
