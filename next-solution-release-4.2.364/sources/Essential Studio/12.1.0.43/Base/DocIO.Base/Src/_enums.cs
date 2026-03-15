#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Diagnostics;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    #region Hidden enums
    /// <summary>
    /// Specifies type of a break inside a document.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum WordBreakCode
    {
        /// <summary>
        /// Specifies no break.  
        /// </summary>
        NoBreak = 0,

        /// <summary>
        /// Explicit column break.
        /// </summary>
        NewColumn = 1,

        /// <summary>
        /// Explicit page break.
        /// </summary>
        NewPage = 2,

        /// <summary>
        /// Specifies start of new section on a new even page.
        /// </summary>
        EvenPage = 3,

        /// <summary>
        /// Specifies start of new section on a new odd page.
        /// </summary>
        Oddpage = 4
    }

    /// <summary>
    /// Type of header / footer.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum HeaderType
    {
        /// <summary>
        /// Specifies InvalidValue.
        /// </summary>
        InvalidValue = -1,

        /// <summary>
        /// Header for even numbered pages.
        /// </summary>
        EvenHeader = 0,

        /// <summary>
        /// Header for odd numbered pages.
        /// </summary>
        OddHeader = 1,

        /// <summary>
        /// Footer for even numbered pages.
        /// </summary>
        EvenFooter = 2,

        /// <summary>
        /// Footer for odd numbered pages.
        /// </summary>
        OddFooter = 3,

        /// <summary>
        /// Header for the first page of the section.
        /// </summary>
        FirstPageHeader = 4,

        /// <summary>
        /// Footer for the first page of the section.
        /// </summary>
        FirstPageFooter = 5
    }

    /// <summary>
    /// Type of elementary text chunk, which reading / writing. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum WordChunkType
    {
        /// <summary>
        /// Specifies text element.
        /// </summary>
        Text,

        /// <summary>
        /// Specifies end of the paragraph.
        /// </summary>
        ParagraphEnd,

        /// <summary>
        /// Specifies section`s end.
        /// </summary>
        SectionEnd,

        /// <summary>
        /// Specifies page break.
        /// </summary>
        PageBreak,

        /// <summary>
        /// Specifies column break.
        /// </summary>
        ColumnBreak,

        /// <summary>
        /// Specifies document end.
        /// </summary>
        DocumentEnd,

        /// <summary>
        /// Specifies an Image.
        /// </summary>
        Image,

        /// <summary>
        /// Specifies a Shape.
        /// </summary>
        Shape,

        /// <summary>
        /// Specifies Table.
        /// </summary>
        Table,

        /// <summary>
        /// Specifies row of the table.
        /// </summary>
        TableRow,

        /// <summary>
        /// specifies table`s cell.
        /// </summary>
        TableCell,

        /// <summary>
        /// Specifies FootNote.
        /// </summary>
        Footnote,

        /// <summary>
        /// Specifies field begin mark.
        /// </summary>
        FieldBeginMark,

        /// <summary>
        /// Specifies separator for a field.
        /// </summary>
        FieldSeparator,

        /// <summary>
        /// Specifies field begin end.
        /// </summary>
        FieldEndMark,

        /// <summary>
        /// Specifies tab.
        /// </summary>
        Tab,

        /// <summary>
        /// Specifies an annotation.
        /// </summary>
        Annotation,

        /// <summary>
        /// Specifies a line break.
        /// </summary>
        LineBreak,

        /// <summary>
        /// Specifies a symbol.
        /// </summary>
        Symbol,

        /// <summary>
        /// Specifies a current page number.
        /// </summary>
        CurrentPageNumber,

        /// <summary>
        /// Specifies end of the document text.
        /// </summary>
        EndOfSubdocText
    }

    /// <summary>
    /// Type of subdocument as Header/Footer, Annotation, etc.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum WordSubdocument
    {
        /// <summary>
        /// Specifies sub document type as Main.
        /// </summary>
        Main = 0,

        /// <summary>
        /// Specifies sub document type as FootNote.
        /// </summary>
        Footnote = 1,

        /// <summary>
        /// Specifies sub document type as HeaderFooter.
        /// </summary>
        HeaderFooter = 2,

        /// <summary>
        /// Specifies sub document type as EndNote.
        /// </summary>
        Endnote = 3,

        /// <summary>
        /// Specifies Annotation type.
        /// </summary>
        Annotation = 4,

        /// <summary>
        /// Specifies sub document type as TextBox.
        /// </summary>
        TextBox = 5,

        /// <summary>
        /// Specifies sub document type as HeaderTextBox.
        /// </summary>
        HeaderTextBox = 6
    }
    /// <summary>
    /// Type of subdocument as Header/Footer, Annotation, etc.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal enum SprmCompareType
    {
        Boolean,
        ByteArray,
        ByteValue,
        IntValue,
        ShortValue,
        UIntValue,
        UShortValue
    }

    #endregion
}

namespace Syncfusion.DocIO
{
    #region Public enums
    /// <summary>
    /// Type of file format.
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// Microsoft Word file format.
        /// </summary>
        Doc,

        /// <summary>
        /// Microsoft Word document template
        /// </summary>
        Dot,
        /// <summary>
        /// Microsoft Word 2007 file format.
        /// </summary>
        Docx,
        /// <summary>
        /// Microsoft Word 2007 file format.
        /// </summary>
        Word2007,
        /// <summary>
        /// Microsoft Word 2010 file format.
        /// </summary>
        Word2010,
        /// <summary>
        /// Microsoft Word 2013 file format.
        /// </summary>
        Word2013,
        /// <summary>
        /// Microsoft Word 2007 Template format.
        /// </summary>
        Word2007Dotx,
        /// <summary>
        /// Microsoft Word 2010 Template format.
        /// </summary>
        Word2010Dotx,
        /// <summary>
        /// Microsoft Word 2013 Template format.
        /// </summary>
        Word2013Dotx,
        /// <summary>
        /// Microsoft Word 2007 macro enabled file format.
        /// </summary>
        Word2007Docm,
        /// <summary>
        /// Microsoft Word 2010 macro enabled file format.
        /// </summary>
        Word2010Docm,
        /// <summary>
        /// Microsoft Word 2013 macro enabled file format.
        /// </summary>
        Word2013Docm,
        /// <summary>
        /// Microsoft Word 2007 macro enabled template format.
        /// </summary>
        Word2007Dotm,
        /// <summary>
        /// Microsoft Word 2010 macro enabled template format.
        /// </summary>
        Word2010Dotm,
        /// <summary>
        /// Microsoft Word 2013 macro enabled template format.
        /// </summary>
        Word2013Dotm,

        /// <summary>
        /// Rtf format
        /// </summary>
        Rtf,
        /// <summary>
        /// Text file format.
        /// </summary>
        Txt,
#if !SILVERLIGHT && !WP
        /// <summary>
        /// E-book format.
        /// </summary>
        EPub,
        /// <summary>
        /// Html format.
        /// </summary>
        Html,
        /// <summary>
        /// Xml file format.
        /// </summary>
        Xml,
        /// <summary>
        /// Support all Format Types.
        /// </summary>
        Automatic
#endif
#if WINRT
        /// <summary>
        /// Html format.
        /// </summary>
        Html
#endif
    }

    /// <summary>
    /// Specifies options to stream content to browser.
    /// </summary>
    public enum HttpContentDisposition
    {
        /// <summary>
        /// Open document directly in browser.
        /// </summary>
        InBrowser,

        /// <summary>
        /// Save document as attachment to the disk.
        /// </summary>
        Attachment
    }

    /// <summary>
    /// Type of the protection in the document
    /// </summary>
    public enum ProtectionType
    {
        /// <summary>
        /// Only Comments are allowed
        /// </summary>
        AllowOnlyComments = 1,

        /// <summary>
        /// Only FormFields are allowed
        /// </summary>
        AllowOnlyFormFields = 2,

        /// <summary>
        /// Only reading are allowed
        /// </summary>
        AllowOnlyReading = 3,

        /// <summary>
        /// Only Revisions are allowed
        /// </summary>
        AllowOnlyRevisions = 0,

        /// <summary>
        /// No protection
        /// </summary>
        NoProtection = -1
    }

    /// <summary>
    /// Style of the Texture
    /// </summary>
    public enum TextureStyle
    {
        Texture10Percent = 3,
        Texture12Pt5Percent = 0x25,
        Texture15Percent = 0x26,
        Texture17Pt5Percent = 0x27,
        Texture20Percent = 4,
        Texture22Pt5Percent = 40,
        Texture25Percent = 5,
        Texture27Pt5Percent = 0x29,
        Texture2Pt5Percent = 0x23,
        Texture30Percent = 6,
        Texture32Pt5Percent = 0x2a,
        Texture35Percent = 0x2b,
        Texture37Pt5Percent = 0x2c,
        Texture40Percent = 7,
        Texture42Pt5Percent = 0x2d,
        Texture45Percent = 0x2e,
        Texture47Pt5Percent = 0x2f,
        Texture50Percent = 8,
        Texture52Pt5Percent = 0x30,
        Texture55Percent = 0x31,
        Texture57Pt5Percent = 50,
        Texture5Percent = 2,
        Texture60Percent = 9,
        Texture62Pt5Percent = 0x33,
        Texture65Percent = 0x34,
        Texture67Pt5Percent = 0x35,
        Texture70Percent = 10,
        Texture72Pt5Percent = 0x36,
        Texture75Percent = 11,
        Texture77Pt5Percent = 0x37,
        Texture7Pt5Percent = 0x24,
        Texture80Percent = 12,
        Texture82Pt5Percent = 0x38,
        Texture85Percent = 0x39,
        Texture87Pt5Percent = 0x3a,
        Texture90Percent = 13,
        Texture92Pt5Percent = 0x3b,
        Texture95Percent = 60,
        Texture97Pt5Percent = 0x3d,
        TextureCross = 0x18,
        TextureDarkCross = 0x12,
        TextureDarkDiagonalCross = 0x13,
        TextureDarkDiagonalDown = 0x10,
        TextureDarkDiagonalUp = 0x11,
        TextureDarkHorizontal = 14,
        TextureDarkVertical = 15,
        TextureDiagonalCross = 0x19,
        TextureDiagonalDown = 0x16,
        TextureDiagonalUp = 0x17,
        TextureHorizontal = 20,
        TextureNone = 0,
        TextureSolid = 1,
        TextureVertical = 0x15,
        TextureNil = 0xFFFF
    }

    /// <summary>
    /// Type of fields
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        ///  Field type is not Specifies. 
        /// </summary>
        FieldNone = 0,

        /// <summary>
        /// Specifies Addins.
        /// </summary>
        FieldAddin = 0x51,

        /// <summary>
        ///   Offset subsequent text within a line to the left, right, up or down. 
        /// </summary>
        FieldAdvance = 0x54,

        /// <summary>
        ///  Prompt the user for text to assign to a bookmark. 
        /// </summary>
        FieldAsk = 0x26,

        /// <summary>
        /// The name of the document's author from Summary Info. 
        /// </summary>
        FieldAuthor = 0x11,

        /// <summary>
        /// Insert an automatic number. 
        /// </summary>
        FieldAutoNum = 0x36,

        /// <summary>
        ///    Insert an automatic number in legal format. 
        /// </summary>
        FieldAutoNumLegal = 0x35,

        /// <summary>
        /// Insert an automatic number in outline format.
        /// </summary>
        FieldAutoNumOutline = 0x34,

        /// <summary>
        /// Insert an AutoText entry. 
        /// </summary>
        FieldAutoText = 0x4f,

        /// <summary>
        ///  Insert text based on style. 
        /// </summary>
        FieldAutoTextList = 0x59,

        /// <summary>
        /// Insert a delivery point barcode. 
        /// </summary>
        FieldBarCode = 0x3f,

        /// <summary>
        /// The comments from Summary Info. 
        /// </summary>
        FieldComments = 0x13,

        /// <summary>
        /// Compares two values. 
        /// </summary>
        FieldCompare = 80,

        /// <summary>
        /// The date the document was created.
        /// </summary>
        FieldCreateDate = 0x15,

        /// <summary>
        /// Specifies data. 
        /// </summary>
        FieldData = 40,

        /// <summary>
        /// Insert data from an external database. 
        /// </summary>
        FieldDatabase = 0x4e,

        /// <summary>
        /// Specified Today`s Date.
        /// </summary>
        FieldDate = 0x1f,

        /// <summary>
        /// Specified Type as FieldDDE. 
        /// </summary>
        FieldDDE = 0x2d,

        /// <summary>
        /// Specified Type as FieldDDEAuto. 
        /// </summary>
        FieldDDEAuto = 0x2e,

        /// <summary>
        ///  Insert the value of the property
        /// </summary>
        FieldDocProperty = 0x55,

        /// <summary>
        /// Insert the value of the document variable. 
        /// </summary>
        FieldDocVariable = 0x40,

        /// <summary>
        /// The total document editing time. 
        /// </summary>
        FieldEditTime = 0x19,

        /// <summary>
        /// Specifies OLE embedded object.
        /// </summary>
        FieldEmbed = 0x3a,

        /// <summary>
        /// Specified Empty Field.
        /// </summary>
        FieldEmpty = -1,

        /// <summary>
        /// Specifies Field Expression.
        /// </summary>
        FieldExpression = 0x22,

        /// <summary>
        ///  The document's name.
        /// </summary>
        FieldFileName = 0x1d,

        /// <summary>
        /// The size on disk of the active document. 
        /// </summary>
        FieldFileSize = 0x45,

        /// <summary>
        ///  Prompt the user for text to insert in the document. 
        /// </summary>
        FieldFillIn = 0x27,

        /// <summary>
        /// Specifies FieldType as FootnoteRef.
        /// </summary>
        FieldFootnoteRef = 5,

        /// <summary>
        /// Specifies Check box control.
        /// </summary>
        FieldFormCheckBox = 0x47,

        /// <summary>
        /// Specifies Drop Down box control.
        /// </summary>
        FieldFormDropDown = 0x53,

        /// <summary>
        /// Specifies Text control.
        /// </summary>
        FieldFormTextInput = 70,

        /// <summary>
        ///  Calculates the result of an expression.
        /// </summary>
        FieldFormula = 0x31,

        /// <summary>
        /// Specifies FieldGlossary.
        /// </summary>
        FieldGlossary = 0x2f,

        /// <summary>
        /// Specifies GoToButton control.
        /// </summary>
        FieldGoToButton = 50,

        /// <summary>
        /// Specifies HTMLActiveX control.
        /// </summary>
        FieldHTMLActiveX = 0x5b,

        /// <summary>
        /// Specifies Hyperlink control.
        /// </summary>
        FieldHyperlink = 0x58,

        /// <summary>
        /// Evaluate arguments conditionally. 
        /// </summary>
        FieldIf = 7,

        /// <summary>
        /// Specifies FieldType as Import.
        /// </summary>
        FieldImport = 0x37,

        /// <summary>
        /// Specifies FieldType as Export.
        /// </summary>
        FieldInclude = 0x24,

        /// <summary>
        /// Insert a picture from a file. 
        /// </summary>
        FieldIncludePicture = 0x43,

        /// <summary>
        /// Insert text from a file. 
        /// </summary>
        FieldIncludeText = 0x44,

        /// <summary>
        /// Create an index. 
        /// </summary>
        FieldIndex = 8,

        /// <summary>
        ///   Mark an index entry. 
        /// </summary>
        FieldIndexEntry = 4,

        /// <summary>
        /// Data from Summary Info. 
        /// </summary>
        FieldInfo = 14,

        /// <summary>
        /// The keywords from Summary Info. 
        /// </summary>
        FieldKeyWord = 0x12,

        /// <summary>
        /// Name of user who last saved the document. 
        /// </summary>
        FieldLastSavedBy = 20,

        /// <summary>
        /// Linked OLE2 object.
        /// </summary>
        FieldLink = 0x38,

        /// <summary>
        /// Insert an element in a list. 
        /// </summary>
        FieldListNum = 90,

        /// <summary>
        /// Run a macro. 
        /// </summary>
        FieldMacroButton = 0x33,

        /// <summary>
        /// Insert a mail merge field. 
        /// </summary>
        FieldMergeField = 0x3b,

        /// <summary>
        /// The number of the current merge record.
        /// </summary>
        FieldMergeRec = 0x2c,

        /// <summary>
        /// Merge record sequence number. 
        /// </summary>
        FieldMergeSeq = 0x4b,

        /// <summary>
        /// Go to the next record in a mail merge. 
        /// </summary>
        FieldNext = 0x29,

        /// <summary>
        /// Conditionally go to the next record in a mail merge. 
        /// </summary>
        FieldNextIf = 0x2a,

        /// <summary>
        /// Insert the number of a footnote or endnote.
        /// </summary>
        FieldNoteRef = 0x48,

        /// <summary>
        /// The number of characters in the document. 
        /// </summary>
        FieldNumChars = 0x1c,

        /// <summary>
        ///  The number of pages in the document. 
        /// </summary>
        FieldNumPages = 0x1a,

        /// <summary>
        /// The number of words in the document. 
        /// </summary>
        FieldNumWords = 0x1b,

        /// <summary>
        /// Represents an ActiveX control such as a command button etc.
        /// </summary>
        FieldOCX = 0x57,

        /// <summary>
        /// Insert the number of the current page. 
        /// </summary>
        FieldPage = 0x21,

        /// <summary>
        /// Insert the number of the page containing the specified bookmark. 
        /// </summary>
        FieldPageRef = 0x25,

        /// <summary>
        /// Download commands to a printer. 
        /// </summary>
        FieldPrint = 0x30,

        /// <summary>
        /// The date the document was last printed. 
        /// </summary>
        FieldPrintDate = 0x17,

        /// <summary>
        /// Stores data for documents converted from other file formats.
        /// </summary>
        FieldPrivate = 0x4d,

        /// <summary>
        /// Insert literal text. 
        /// </summary>
        FieldQuote = 0x23,

        /// <summary>
        /// Insert the text marked by a bookmark. 
        /// </summary>
        FieldRef = 3,

        /// <summary>
        /// Create an index, table of contents, table of figures, and/or table of authorities by using multiple documents. 
        /// </summary>
        FieldRefDoc = 11,

        /// <summary>
        /// Insert the number of times the document has been saved. 
        /// </summary>
        FieldRevisionNum = 0x18,

        /// <summary>
        /// The date the document was last saved.
        /// </summary>
        FieldSaveDate = 0x16,

        /// <summary>
        /// Insert the number of the current section. 
        /// </summary>
        FieldSection = 0x41,

        /// <summary>
        /// Insert the total number of pages in the section. 
        /// </summary>
        FieldSectionPages = 0x42,

        /// <summary>
        /// Insert an automatic sequence number. 
        /// </summary>
        FieldSequence = 12,

        /// <summary>
        /// Assign new text to a bookmark. 
        /// </summary>
        FieldSet = 6,

        /// <summary>
        /// Conditionally skip a record in a mail merge. 
        /// </summary>
        FieldSkipIf = 0x2b,

        /// <summary>
        /// Insert the text from a like-style paragraph. 
        /// </summary>
        FieldStyleRef = 10,

        /// <summary>
        ///  The document's subject from Summary Info. 
        /// </summary>
        FieldSubject = 0x10,

        /// <summary>
        ///  The document's Subscriber from Summary Info. 
        /// </summary>
        FieldSubscriber = 0x52,

        /// <summary>
        ///  Insert a special character
        /// </summary>
        FieldSymbol = 0x39,

        /// <summary>
        /// The name of the template attached to the document. 
        /// </summary>
        FieldTemplate = 30,

        /// <summary>
        ///  The current time. 
        /// </summary>
        FieldTime = 0x20,

        /// <summary>
        /// The document's title from Summary Info. 
        /// </summary>
        FieldTitle = 15,

        /// <summary>
        /// Create a table of authorities. 
        /// </summary>
        FieldTOA = 0x49,

        /// <summary>
        /// Make a table of authorities entry. 
        /// </summary>
        FieldTOAEntry = 0x4a,

        /// <summary>
        /// Create a table of contents. 
        /// </summary>
        FieldTOC = 13,

        /// <summary>
        ///  Make a table of contents entry. 
        /// </summary>
        FieldTOCEntry = 9,

        /// <summary>
        /// Address from Tools Options User Info. 
        /// </summary>
        FieldUserAddress = 0x3e,

        /// <summary>
        /// Initials form Tools Options User Info. 
        /// </summary>
        FieldUserInitials = 0x3d,

        /// <summary>
        ///  Name from Tools Options User Info. 
        /// </summary>
        FieldUserName = 60,

        /// <summary>
        /// Specifies FieldType as Shape.
        /// </summary>
        FieldShape = 95,

        /// <summary>
        /// Specifies FieldType as BIDIOUTLINE.
        /// </summary>
        FieldBidiOutline = 0x5c,

        /// <summary>
        /// Specifies AddressBlock
        /// </summary>
        FieldAddressBlock = 0x5D,

        /// <summary>
        ///  Specifies FieldType as Unknown.
        /// </summary>
        FieldUnknown = 1000
    }

    /// <summary>
    /// Type of Caption Numbering
    /// </summary>
    public enum CaptionNumberingFormat
    {
        /// <summary>
        /// caption format, Numbers 
        /// </summary>
        Number,

        /// <summary>
        /// Roman Numerals
        /// </summary>
        Roman,

        /// <summary>
        /// caption format, Alphabets
        /// </summary>
        Alphabetic
    }

    /// <summary>
    /// Type of Image Caption Numbering
    /// </summary>
    public enum CaptionPosition
    {
        /// <summary>
        /// Caption is positioned above the Image.
        /// </summary>
        AboveImage,

        /// <summary>
        /// Caption is positioned below the image.
        /// </summary>
        AfterImage,
    }

    /// <summary>
    /// Specifies view mode in MSWord.
    /// </summary>
    public enum DocumentViewType
    {
        /// <summary>
        /// Specifies that the document will be rendered in the default view of the application.
        /// </summary>
        None = 0,

        /// <summary>
        /// Everything that will appear in the printed document appears on the screen.
        /// </summary>
        PrintLayout = 1,

        /// <summary>
        /// Shows the headings and subheadings in the word document.
        /// </summary>
        OutlineLayout = 3,

        /// <summary>
        /// Document appears with a dotted line separating the pages and/or document sections.
        /// Columns, drawings, headers/footers, footnotes/endnotes, and comments do not appear. 
        /// </summary>
        NormalLayout = 4,

        /// <summary>
        /// Designed to show the word document will look as a web page.
        /// </summary>
        WebLayout = 5
    }

    /// <summary>
    /// Specifies zooming type in MSWord.
    /// </summary>
    public enum ZoomType
    {
        /// <summary>
        /// Indicates to use the explicit zoom percentage. 
        /// </summary>
        None = 0,

        /// <summary>
        /// Zoom percentage is automatically recalculated to fit one full page. 
        /// </summary>
        FullPage = 1,

        /// <summary>
        /// Zoom percentage is automatically recalculated to fit page width. 
        /// </summary>
        PageWidth = 2,

        /// <summary>
        /// Zoom percentage is automatically recalculated to fit text. 
        /// </summary>
        TextFit = 3
    }

    /// <summary>
    /// Specifies when line numbering is restarted. 
    /// </summary>
    public enum LineNumberingMode
    {
        /// <summary>
        /// Line numbering restarts at the start of every page
        /// </summary>
        RestartPage = 0,

        /// <summary>
        /// Line numbering restarts at the section start. 
        /// </summary>
        RestartSection = 1,

        /// <summary>
        /// Line numbering continuous from the previous section. 
        /// </summary>
        Continuous = 2,

        /// <summary>
        /// Specifies LineNumberingMode as None.
        /// </summary>
        None = 255
    }

    /// <summary>
    /// Specifies on which pages border is applied.
    /// </summary>
    public enum PageBordersApplyType
    {
        /// <summary>
        /// Page border applies to all pages.
        /// </summary>
        AllPages = 0,

        /// <summary>
        /// Page border applies only to first pages.
        /// </summary>
        FirstPage = 1,

        /// <summary>
        /// Page border applies to all pages except the first.
        /// </summary>
        AllExceptFirstPage = 2
    }

    /// <summary>
    /// Specifies the position of page border.
    /// </summary>
    public enum PageBorderOffsetFrom
    {
        /// <summary>
        /// Page border is measured from text.
        /// </summary>
        Text,

        /// <summary>
        /// Page border is measured from the edge of the page.
        /// </summary>
        PageEdge
    }

    /// <summary>
    /// Animation effect for text.
    /// </summary>
    public enum TextEffect
    {
        /// <summary>
        /// specifies no animation.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that this text shall be surrounded by a border consisting of a series of
        /// colored lights, which constantly change colors in sequence.
        /// </summary>
        LasVegasLights,

        /// <summary>
        /// Specifies that this text shall be surrounded by a background 
        /// color which alternates between black and white.
        /// </summary>
        BlinkingBackground,

        /// <summary>
        /// Specifies that this text shall have a background consisting of a random pattern of
        /// colored lights, which constantly change colors in sequence.
        /// </summary>
        SparkleText,

        /// <summary>
        /// Specifies that this text shall be surrounded by an animated black dashed line border.
        /// </summary>
        MarchingBlackAnts,

        /// <summary>
        /// Specifies that this text shall be surrounded by an animated red dashed line border.
        /// </summary>
        MarchingRedAnts,

        /// <summary>
        /// Specifies that this text shall be animated by alternating between normal and blurry states.
        /// </summary>
        Shimmer
    }

    /// <summary>
    /// The enum, which defines paragraph format's outline level
    /// </summary>
    public enum OutlineLevel
    {
        /// <summary>
        /// Outline level: "Level 1"
        /// </summary>
        Level1 = 0,

        /// <summary>
        /// Outline level: "Level 2"
        /// </summary>
        Level2 = 1,

        /// <summary>
        /// Outline level: "Level 3"
        /// </summary>
        Level3 = 2,

        /// <summary>
        /// Outline level: "Level 4"
        /// </summary>
        Level4 = 3,

        /// <summary>
        /// Outline level: "Level 5"
        /// </summary>
        Level5 = 4,

        /// <summary>
        /// Outline level: "Level 6"
        /// </summary>
        Level6 = 5,

        /// <summary>
        /// Outline level: "Level 7"
        /// </summary>
        Level7 = 6,

        /// <summary>
        /// Outline level: "Level 8"
        /// </summary>
        Level8 = 7,

        /// <summary>
        /// Outline level: "Level 9"
        /// </summary>
        Level9 = 8,

        /// <summary>
        /// Outline level: "Body Text"
        /// </summary>
        BodyText = 9
    }
    #region enum FrameWrapMode
    /// <summary>
    /// Specifies the Wrapping mode of the frame
    /// </summary>
    internal enum FrameWrapMode
    {
        /// <summary>
        /// This value specifies automatic text wrapping.
        /// </summary>
        Auto,
        /// <summary>
        /// This value specifies that there is no text wrapping to either side of the frame.
        /// </summary>
        NotBeside,
        /// <summary>
        /// This value specifies that text is wrapped around the frame.
        /// </summary>
        Around,
        /// <summary>
        /// Text is not wrapped around the frame.
        /// </summary>
        None,
        /// <summary>
        /// This value specifies that text is tightly wrapped around the frame.
        /// </summary>
        Tight,
        /// <summary>
        /// This value specifies that text is wrapped through the frame, to the contours of the contents of the frame.
        /// </summary>
        Through
    }
    #endregion
    /// <summary>
    /// The enum defines the horizontal relation
    /// </summary>
    public enum HorizontalRelation
    {
        /// <summary>
        /// The "Column" horizontal relation
        /// </summary>
        Column,
        /// <summary>
        /// The "Margin" horizontal relation
        /// </summary>
        Margin,
        /// <summary>
        /// The "Page" horizontal relation
        /// </summary>
        Page
    }

    /// <summary>
    /// The enum defines the vertical relation
    /// </summary>
    public enum VerticalRelation
    {
        /// <summary>
        /// The "Margin" vertical relation
        /// </summary>
        Margin,
        /// <summary>
        /// The "Page" vertical relation
        /// </summary>
        Page,
        /// <summary>
        /// The "Paragraph" vertical relation
        /// </summary>
        Paragraph
    }

    /// <summary>
    /// Specifies the absolute horizontal position.
    /// </summary>
    public enum HorizontalPosition
    {
        /// <summary>
        /// The object is aligned to the left of the reference origin.
        /// </summary>
        Left = 0,
        /// <summary>
        /// The object is centered to the reference origin.
        /// </summary>
        Center = -4,
        /// <summary>
        /// The object is aligned to the right of the reference origin.
        /// </summary>
        Right = -8,
        /// <summary>
        /// "Inside" horizontal position.
        /// </summary>
        Inside = -12,
        /// <summary>
        /// "Outside" horizontal position.
        /// </summary>
        Outside = -16
    }

    /// <summary>
    /// Specifies the absolute vertical position.
    /// </summary>
    public enum VerticalPosition
    {
        /// <summary>
        /// No vertical positioning
        /// </summary>
        None = 0,
        /// <summary>
        /// "Top" vertical position
        /// </summary>
        Top = -4,
        /// <summary>
        /// "Center" vertical position
        /// </summary>
        Center = -8,
        /// <summary>
        /// "Bottom" vertical position
        /// </summary>
        Bottom = -12,
        /// <summary>
        /// "Inside" vertical position
        /// </summary>
        Inside = -16,
        /// <summary>
        /// "Outside" vertical position.
        /// </summary>    
        Outside = -20
    }

    #endregion

    #region enum LineDashing
    /// <summary>
    /// Line dashing.
    /// </summary>
    public enum LineDashing
    {
        /// <summary>
        /// Solid (continuous) pen.
        /// </summary>
        Solid = 0,

        /// <summary>
        /// PS_DASH system dash style.
        /// </summary>
        Dash = 1,

        /// <summary>
        /// PS_DOT system dash style.
        /// </summary>
        Dot = 2,

        /// <summary>
        /// PS_DASHDOT system dash style.
        /// </summary>
        DashDot = 3,

        /// <summary>
        /// PS_DASHDOTDOT system dash style.
        /// </summary>
        DashDotDot = 4,

        /// <summary>
        /// Square dot style.
        /// </summary>
        DotGEL = 5,

        /// <summary>
        /// Dash style.
        /// </summary>
        DashGEL = 6,

        /// <summary>
        /// Long dash style.
        /// </summary>
        LongDashGEL = 7,

        /// <summary>
        /// Dash short dash.
        /// </summary>
        DashDotGEL = 8,

        /// <summary>
        /// Long dash short dash.
        /// </summary>
        LongDashDotGEL = 9,

        /// <summary>
        /// Long dash short dash short dash.
        /// </summary>
        LongDashDotDotGEL = 10
    }
    #endregion

    #region enum WrapMode
    /// <summary>
    /// Specifies Wrap mode.
    /// </summary>
    public enum WrapMode
    {
        /// <summary>
        /// Square wrap mode.
        /// </summary>
        Square = 0,

        /// <summary>
        /// ByPoints wrap mode.
        /// </summary>
        ByPoints = 1,

        /// <summary>
        /// No wrap mode.
        /// </summary>
        None = 2,

        /// <summary>
        /// TopBottom wrap mode.
        /// </summary>
        TopBottom = 3,

        /// <summary>
        /// Through wrap mode.
        /// </summary>
        Through = 4,
    }
    #endregion

    #region enums DocumentProperty
    /// <summary>
    /// Specifies PropertyValueType.
    /// </summary>
    public enum PropertyValueType
    {
        /// <summary>
        /// Specifies Value type as boolean.
        /// </summary>
        Boolean,

        /// <summary>
        ///  Specifies Value type as date.
        /// </summary>
        Date,

        /// <summary>
        ///  Specifies Value type as float.
        /// </summary>
        Float,

        /// <summary>
        ///  Specifies Value type as double.
        /// </summary>
        Double,

        /// <summary>
        ///  Specifies Value type as integer.
        /// </summary>
        Int,

        /// <summary>
        ///  Specifies Value type as String.
        /// </summary>
        String,

        /// <summary>
        ///  Specifies Value type as byte array.
        /// </summary>
        ByteArray,

        /// <summary>
        ///  Specifies Value type as ClipData.
        /// </summary>
        ClipData
    }

    /// <summary>
    /// Specifies Type of the Property.
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// Specifies Property Type as Summary.
        /// </summary>
        Summary,

        /// <summary>
        /// Specifies Property Type as DocumentSummary.
        /// </summary>
        DocumentSummary,

        /// <summary>
        /// Specifies Property Type as Custom.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Paragraph line spacing rule
    /// </summary>
    public enum LineSpacingRule
    {
        /// <summary>
        /// The line spacing can be greater than or equal to, but never less than,
        /// the value specified in the LineSpacing property. 
        /// </summary>
        AtLeast = 0,

        /// <summary>
        /// The line spacing never changes from the value specified in the LineSpacing property, 
        /// even if a larger font is used within the paragraph. 
        /// </summary>
        Exactly = 1,

        /// <summary>
        /// The line spacing is specified in the LineSpacing property as the number of lines. 
        /// One line equals 12 points. 
        /// </summary>
        Multiple = 2
    }

    #endregion

    #region enums ShapeAlignment
    /// <summary>
    /// Specifies horizontal alignment of a floating shape.
    /// </summary>
    public enum ShapeHorizontalAlignment
    {
        /// <summary>
        /// The object is explicitly positioned using position properties.
        /// </summary>
        None,

        /// <summary>
        /// The object is aligned to the left of the reference origin.
        /// </summary>
        Left,

        /// <summary>
        /// The object is centered to the reference origin.
        /// </summary>
        Center,

        /// <summary>
        /// The object is aligned to the right of the reference origin.
        /// </summary>
        Right,

        /// <summary>
        /// Not documented.
        /// </summary>
        Inside,

        /// <summary>
        /// Not documented.
        /// </summary>
        Outside
    }

    /// <summary>
    /// Specifies vertical alignment of a floating shape.
    /// </summary>
    public enum ShapeVerticalAlignment
    {
        /// <summary>
        /// The object is aligned to the bottom of the reference origin.
        /// </summary>
        Bottom = 3,

        /// <summary>
        /// The object is centered relative to the reference origin.
        /// </summary>
        Center = 2,

        /// <summary>
        /// Not documented.
        /// </summary>
        Inline = -1,

        /// <summary>
        /// Not documented.
        /// </summary>
        Inside = 4,

        /// <summary>
        /// The object is explicitly positioned using position properties.
        /// </summary>
        None = 0,

        /// <summary>
        /// Not documented.
        /// </summary>
        Outside = 5,

        /// <summary>
        /// The object is aligned to the top of the reference origin.
        /// </summary>
        Top = 1
    }
    #endregion

    #region enums FormField
    /// <summary>
    /// Specifies the type of a text form field.
    /// </summary>
    public enum TextFormFieldType
    {
        /// <summary>
        /// Text form field can contain any text. 
        /// </summary>
        RegularText = 0,

        /// <summary>
        /// Text form field can contain only numbers.
        /// </summary>
        NumberText = 1,

        /// <summary>
        /// Text for field can contain only a valid date value. 
        /// </summary>
        DateText = 2,
        ///<summary>
        ///Specifies the TextFormFieldType as Calculation
        ///</summary>
        Calculation = 5
    }
    #endregion

    #region enum FootnotePosition
    /// <summary>
    /// Specifies FootnotePosition.
    /// </summary>
    public enum FootnotePosition
    {
        /// <summary>
        /// Footnotes are output at the bottom of each end notes. 
        /// </summary>
        PrintAsEndnotes = 0,

        /// <summary>
        /// Footnotes are output at the bottom of each page. 
        /// </summary>
        PrintAtBottomOfPage = 1,

        /// <summary>
        /// Footnotes are output beneath text on each page. 
        /// </summary>
        PrintImmediatelyBeneathText = 2
    }
    #endregion

    #region enum FootEndNoteRestartIndex
    /// <summary>
    /// Specifies FootnoteRestartIndex.
    /// </summary>
    public enum FootnoteRestartIndex
    {
        /// <summary>
        /// Numbering continuous throughout the document. 
        /// </summary>
        DoNotRestart = 0,

        /// <summary>
        /// Numbering restarts at each section.
        /// </summary>
        RestartForEachSection = 1,

        /// <summary>
        /// Numbering restarts at each page.
        /// </summary>
        RestartForEachPage = 2
    }

    /// <summary>
    /// Specifies End notes restart index.
    /// </summary>
    public enum EndnoteRestartIndex
    {
        /// <summary>
        /// Numbering continuous throughout the document. 
        /// </summary>
        DoNotRestart = 0,

        /// <summary>
        /// Numbering restarts at each section.
        /// </summary>
        RestartForEachSection = 1
    }
    #endregion

    #region enum EndnotePosition
    /// <summary>
    /// Endnote position of the Document.
    /// </summary>
    public enum EndnotePosition
    {
        /// <summary>
        /// Placed the EndNote on End of the section.
        /// </summary>
        DisplayEndOfSection = 0,

        /// <summary>
        /// Placed the EndNote on End of the Document.
        /// </summary>
        DisplayEndOfDocument = 3
    }
    #endregion

    #region enum FootEndNoteNumberFormat
    /// <summary>
    /// Specifies Numberformat of FootEndNote. 
    /// </summary>
    public enum FootEndNoteNumberFormat
    {
        /// <summary>
        /// Specifies FootNote number style in Arabic format (1, 2, 3, ...).
        /// </summary>
        Arabic = 0,

        /// <summary>
        /// Specifies FootNote number style in upper case Roman format (I, II, III, ...) .
        /// </summary>
        UpperCaseRoman = 1,

        /// <summary>
        /// Specifies FootNote number style in lower case Roman format (i, ii, iii, ...) .
        /// </summary>
        LowerCaseRoman = 2,

        /// <summary>
        /// Specifies FootNote number style in upper case letters format (A, B, C, ...) .
        /// </summary>
        UpperCaseLetter = 3,

        /// <summary>
        /// Specifies FootNote number style in lower case letters format (a, b, c, ...) .
        /// </summary>
        LowerCaseLetter = 4
    }
    #endregion

    #region enum FootnoteType
    /// <summary>
    /// Specifies the Type of the FootNote.
    /// </summary>
    public enum FootnoteType
    {
        /// <summary>
        /// Specifies object is a footnote. 
        /// </summary>
        Footnote = 0,

        /// <summary>
        /// Specifies object is a endnote. 
        /// </summary>
        Endnote = 1
    }
    #endregion
    #region enum FontFamilyType
    /// <summary>
    /// Specifies the Type of the FontFamily.
    /// </summary>
    internal enum FontFamilyType
    {
        /// <summary>
        /// Font family is unspecified for this font. 
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Roman (Serif).
        /// </summary>
        Roman = 1,
        /// <summary>
        /// Swiss (Sans-serif).
        /// </summary>
        Swiss = 2,
        /// <summary>
        /// Swiss (Sans-serif).
        /// </summary>
        Modern = 3,
        /// <summary>
        /// Script (Cursive).
        /// </summary>
        Script = 4,
        /// <summary>
        /// Decorative (Fantasy).
        /// </summary>
        Decorative = 5,
    }
    #endregion
    #region enum FontPitchType
    /// <summary>
    /// Specifies the Type of the Font PitchRequest.
    /// </summary>
    public enum FontPitchType
    {
        /// <summary>
        /// Default pitch.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Fixed pitch.
        /// </summary>
        Fixed = 1,
        /// <summary>
        /// Variable pitch. 
        /// </summary>
        Variable = 2
    }
    #endregion
    #region enum LanguageIDs
    /// <summary>
    /// 
    /// </summary>
    public enum LocaleIDs
    {
        /// <summary>
        /// 
        /// </summary>
        af_ZA = 1072,
        /// <summary>
        /// 
        /// </summary>
        sq_AL = 1052,
        /// <summary>
        /// 
        /// </summary>
        am_ET = 1118,
        /// <summary>
        /// 
        /// </summary>
        gsw_FR = 1156,
        /// <summary>
        /// 
        /// </summary>
        ar_DZ = 5121,
        /// <summary>
        /// 
        /// </summary>
        ar_BH = 15361,
        /// <summary>
        /// 
        /// </summary>
        ar_EG = 3073,
        /// <summary>
        /// 
        /// </summary>
        ar_IQ = 2049,
        /// <summary>
        /// 
        /// </summary>
        ar_JO = 11265,
        /// <summary>
        /// 
        /// </summary>
        ar_KW = 13313,
        /// <summary>
        /// 
        /// </summary>
        ar_LB = 12289,
        /// <summary>
        /// 
        /// </summary>
        ar_LY = 4097,
        /// <summary>
        /// 
        /// </summary>
        ar_MA = 6145,
        /// <summary>
        /// 
        /// </summary>
        ar_OM = 8193,
        /// <summary>
        /// 
        /// </summary>
        ar_QA = 16385,
        /// <summary>
        /// 
        /// </summary>
        ar_SA = 1025,
        /// <summary>
        /// 
        /// </summary>
        ar_SY = 10241,
        /// <summary>
        /// 
        /// </summary>
        ar_TN = 7169,
        /// <summary>
        /// 
        /// </summary>
        ar_AE = 14337,
        /// <summary>
        /// 
        /// </summary>
        ar_YE = 9217,
        /// <summary>
        /// 
        /// </summary>
        hy_AM = 1067,
        /// <summary>
        /// 
        /// </summary>
        as_IN = 1101,
        /// <summary>
        /// 
        /// </summary>
        az_Cyrl_AZ = 2092,
        /// <summary>
        /// 
        /// </summary>
        az_Latn_AZ = 1068,
        /// <summary>
        /// 
        /// </summary>
        ba_RU = 1133,
        /// <summary>
        /// 
        /// </summary>
        eu_ES = 1069,
        /// <summary>
        /// 
        /// </summary>
        be_BY = 1059,
        /// <summary>
        /// 
        /// </summary>
        bn_BD = 2117,
        /// <summary>
        /// 
        /// </summary>
        bn_IN = 1093,
        /// <summary>
        /// 
        /// </summary>
        bs_Cyrl_BA = 8218,
        /// <summary>
        /// 
        /// </summary>
        bs_Latn_BA = 5146,
        /// <summary>
        /// 
        /// </summary>
        bg_BG = 1026,
        /// <summary>
        /// 
        /// </summary>
        br_FR = 1150,
        /// <summary>
        /// 
        /// </summary>
        my_MM = 1109,
        /// <summary>
        /// 
        /// </summary>
        ca_ES = 1027,
        /// <summary>
        /// 
        /// </summary>
        chr_US = 1116,
        /// <summary>
        /// 
        /// </summary>
        zh_HK = 3076,
        /// <summary>
        /// 
        /// </summary>
        zh_MO = 5124,
        /// <summary>
        /// 
        /// </summary>
        zh_CN = 2052,
        /// <summary>
        /// 
        /// </summary>
        zh_SG = 4100,
        /// <summary>
        /// 
        /// </summary>
        zh_TW = 1028,
        /// <summary>
        /// 
        /// </summary>
        co_FR = 1155,
        /// <summary>
        /// 
        /// </summary>
        hr_BA = 4122,
        /// <summary>
        /// 
        /// </summary>
        hr_HR = 1050,
        /// <summary>
        /// 
        /// </summary>
        cs_CZ = 1029,
        /// <summary>
        /// 
        /// </summary>
        da_DK = 1030,
        /// <summary>
        /// 
        /// </summary>
        prs_AF = 1164,
        /// <summary>
        /// 
        /// </summary>
        dv_MV = 1125,
        /// <summary>
        /// 
        /// </summary>
        nl_BE = 2067,
        /// <summary>
        /// 
        /// </summary>
        nl_NL = 1043,
        /// <summary>
        /// 
        /// </summary>
        bin_NG = 1126,
        /// <summary>
        /// 
        /// </summary>
        et_EE = 1061,
        /// <summary>
        /// 
        /// </summary>
        en_AU = 3081,
        /// <summary>
        /// 
        /// </summary>
        en_BZ = 10249,
        /// <summary>
        /// 
        /// </summary>
        en_CA = 4105,
        /// <summary>
        /// 
        /// </summary>
        en_029 = 9225,
        /// <summary>
        /// 
        /// </summary>
        en_HK = 15369,
        /// <summary>
        /// 
        /// </summary>
        en_IN = 16393,
        /// <summary>
        /// 
        /// </summary>
        en_ID = 14345,
        /// <summary>
        /// 
        /// </summary>
        en_IE = 6153,
        /// <summary>
        /// 
        /// </summary>
        en_JM = 8201,
        /// <summary>
        /// 
        /// </summary>
        en_MY = 17417,
        /// <summary>
        /// 
        /// </summary>
        en_NZ = 5129,
        /// <summary>
        /// 
        /// </summary>
        en_PH = 13321,
        /// <summary>
        /// 
        /// </summary>
        en_SG = 18441,
        /// <summary>
        /// 
        /// </summary>
        en_ZA = 7177,
        /// <summary>
        /// 
        /// </summary>
        en_TT = 11273,
        /// <summary>
        /// 
        /// </summary>
        en_GB = 2057,
        /// <summary>
        /// 
        /// </summary>
        en_US = 1033,
        /// <summary>
        /// 
        /// </summary>
        en_ZW = 12297,
        /// <summary>
        /// 
        /// </summary>
        fo_FO = 1080,
        /// <summary>
        /// 
        /// </summary>
        fil_PH = 1124,
        /// <summary>
        /// 
        /// </summary>
        fi_FI = 1035,
        /// <summary>
        /// 
        /// </summary>
        fr_BE = 2060,
        /// <summary>
        /// 
        /// </summary>
        fr_CM = 11276,
        /// <summary>
        /// 
        /// </summary>
        fr_CA = 3084,
        /// <summary>
        /// 
        /// </summary>
        fr_CD = 9228,
        /// <summary>
        /// 
        /// </summary>
        fr_CI = 12300,
        /// <summary>
        /// 
        /// </summary>
        fr_FR = 1036,
        /// <summary>
        /// 
        /// </summary>
        fr_HT = 15372,
        /// <summary>
        /// 
        /// </summary>
        fr_LU = 5132,
        /// <summary>
        /// 
        /// </summary>
        fr_ML = 13324,
        /// <summary>
        /// 
        /// </summary>
        fr_MC = 6156,
        /// <summary>
        /// 
        /// </summary>
        fr_MA = 14348,
        /// <summary>
        /// 
        /// </summary>
        fr_RE = 8204,
        /// <summary>
        /// 
        /// </summary>
        fr_SN = 10252,
        /// <summary>
        /// 
        /// </summary>
        fr_CH = 4108,
        /// <summary>
        /// 
        /// </summary>
        fr_fr_WINDIES = 7180,
        /// <summary>
        /// 
        /// </summary>
        fy_NL = 1122,
        /// <summary>
        /// 
        /// </summary>
        ff_NG = 1127,
        /// <summary>
        /// 
        /// </summary>
        gd_GB = 1084,
        /// <summary>
        /// 
        /// </summary>
        gl_ES = 1110,
        /// <summary>
        /// 
        /// </summary>
        ka_GE = 1079,
        /// <summary>
        /// 
        /// </summary>
        de_AT = 3079,
        /// <summary>
        /// 
        /// </summary>
        de_DE = 1031,
        /// <summary>
        /// 
        /// </summary>
        de_LI = 5127,
        /// <summary>
        /// 
        /// </summary>
        de_LU = 4103,
        /// <summary>
        /// 
        /// </summary>
        de_CH = 2055,
        /// <summary>
        /// 
        /// </summary>
        el_GR = 1032,
        /// <summary>
        /// 
        /// </summary>
        gn_PY = 1140,
        /// <summary>
        /// 
        /// </summary>
        gu_IN = 1095,
        /// <summary>
        /// 
        /// </summary>
        kl_GL = 1135,
        /// <summary>
        /// 
        /// </summary>
        ha_Latn_NG = 1128,
        /// <summary>
        /// 
        /// </summary>
        haw_US = 1141,
        /// <summary>
        /// 
        /// </summary>
        he_IL = 1037,
        /// <summary>
        /// 
        /// </summary>
        hi_IN = 1081,
        /// <summary>
        /// 
        /// </summary>
        hu_HU = 1038,
        /// <summary>
        /// 
        /// </summary>
        ibb_NG = 1129,
        /// <summary>
        /// 
        /// </summary>
        is_IS = 1039,
        /// <summary>
        /// 
        /// </summary>
        ig_NG = 1136,
        /// <summary>
        /// 
        /// </summary>
        id_ID = 1057,
        /// <summary>
        /// 
        /// </summary>
        iu_Latn_CA = 2141,
        /// <summary>
        /// 
        /// </summary>
        iu_Cans_CA = 1117,
        /// <summary>
        /// 
        /// </summary>
        it_IT = 1040,
        /// <summary>
        /// 
        /// </summary>
        it_CH = 2064,
        /// <summary>
        /// 
        /// </summary>
        ga_IE = 2108,
        /// <summary>
        /// 
        /// </summary>
        xh_ZA = 1076,
        /// <summary>
        /// 
        /// </summary>
        zu_ZA = 1077,
        /// <summary>
        /// 
        /// </summary>
        kn_IN = 1099,
        /// <summary>
        /// 
        /// </summary>
        kr_NG = 1137,
        /// <summary>
        /// 
        /// </summary>
        ks_Deva = 2144,
        /// <summary>
        /// 
        /// </summary>
        ks_Arab = 1120,
        /// <summary>
        /// 
        /// </summary>
        kk_KZ = 1087,
        /// <summary>
        /// 
        /// </summary>
        km_KH = 1107,
        /// <summary>
        /// 
        /// </summary>
        kok_IN = 1111,
        /// <summary>
        /// 
        /// </summary>
        ko_KR = 1042,
        /// <summary>
        /// 
        /// </summary>
        ky_KG = 1088,
        /// <summary>
        /// 
        /// </summary>
        qut_GT = 1158,
        /// <summary>
        /// 
        /// </summary>
        rw_RW = 1159,
        /// <summary>
        /// 
        /// </summary>
        lo_LA = 1108,
        /// <summary>
        /// 
        /// </summary>
        la_Latn = 1142,
        /// <summary>
        /// 
        /// </summary>
        lv_LV = 1062,
        /// <summary>
        /// 
        /// </summary>
        lt_LT = 1063,
        /// <summary>
        /// 
        /// </summary>
        dsb_DE = 2094,
        /// <summary>
        /// 
        /// </summary>
        lb_LU = 1134,
        /// <summary>
        /// 
        /// </summary>
        mk_MK = 1071,
        /// <summary>
        /// 
        /// </summary>
        ms_BN = 2110,
        /// <summary>
        /// 
        /// </summary>
        ms_MY = 1086,
        /// <summary>
        /// 
        /// </summary>
        ml_IN = 1100,
        /// <summary>
        /// 
        /// </summary>
        mt_MT = 1082,
        /// <summary>
        /// 
        /// </summary>
        mni_IN = 1112,
        /// <summary>
        /// 
        /// </summary>
        mi_NZ = 1153,
        /// <summary>
        /// 
        /// </summary>
        mr_IN = 1102,
        /// <summary>
        /// 
        /// </summary>
        arn_CL = 1146,
        /// <summary>
        /// 
        /// </summary>
        mn_MN = 1104,
        /// <summary>
        /// 
        /// </summary>
        mn_Mong_CN = 2128,
        /// <summary>
        /// 
        /// </summary>
        ne_NP = 1121,
        /// <summary>
        /// 
        /// </summary>
        ne_IN = 2145,
        /// <summary>
        /// 
        /// </summary>
        nb_NO = 1044,
        /// <summary>
        /// 
        /// </summary>
        nn_NO = 2068,
        /// <summary>
        /// 
        /// </summary>
        oc_FR = 1154,
        /// <summary>
        /// 
        /// </summary>
        or_IN = 1096,
        /// <summary>
        /// 
        /// </summary>
        om_Ethi_ET = 1138,
        /// <summary>
        /// 
        /// </summary>
        pap_AN = 1145,
        /// <summary>
        /// 
        /// </summary>
        ps_AF = 1123,
        /// <summary>
        /// 
        /// </summary>
        fa_IR = 1065,
        /// <summary>
        /// 
        /// </summary>
        pl_PL = 1045,
        /// <summary>
        /// 
        /// </summary>
        pt_BR = 1046,
        /// <summary>
        /// 
        /// </summary>
        pt_PT = 2070,
        /// <summary>
        /// 
        /// </summary>
        pa_IN = 1094,
        /// <summary>
        /// 
        /// </summary>
        pa_PK = 2118,
        /// <summary>
        /// 
        /// </summary>
        quz_BO = 1131,
        /// <summary>
        /// 
        /// </summary>
        guz_EC = 2155,
        /// <summary>
        /// 
        /// </summary>
        guz_PE = 3179,
        /// <summary>
        /// 
        /// </summary>
        ro_RO = 1048,
        /// <summary>
        /// 
        /// </summary>
        ro_MO = 2072,
        /// <summary>
        /// 
        /// </summary>
        rm_CH = 1047,
        /// <summary>
        /// 
        /// </summary>
        ru_RU = 1049,
        /// <summary>
        /// 
        /// </summary>
        ru_MO = 2073,
        /// <summary>
        /// 
        /// </summary>
        smn_FI = 9275,
        /// <summary>
        /// 
        /// </summary>
        smj_NO = 4155,
        /// <summary>
        /// 
        /// </summary>
        smj_SE = 5179,
        /// <summary>
        /// 
        /// </summary>
        se_FI = 3131,
        /// <summary>
        /// 
        /// </summary>
        se_NO = 1083,
        /// <summary>
        /// 
        /// </summary>
        se_SE = 2107,
        /// <summary>
        /// 
        /// </summary>
        sms_FI = 8251,
        /// <summary>
        /// 
        /// </summary>
        sma_NO = 6203,
        /// <summary>
        /// 
        /// </summary>
        sma_SE = 7227,
        /// <summary>
        /// 
        /// </summary>
        sa_IN = 1103,
        /// <summary>
        /// 
        /// </summary>
        sr_Cyrl_BA = 7194,
        /// <summary>
        /// 
        /// </summary>
        sr_Cyrl_CS = 3098,
        /// <summary>
        /// 
        /// </summary>
        sr_Latn_BA = 6170,
        /// <summary>
        /// 
        /// </summary>
        sr_Latn_CS = 2074,
        /// <summary>
        /// 
        /// </summary>
        nso_ZA = 1132,
        /// <summary>
        /// 
        /// </summary>
        tn_ZA = 1074,
        /// <summary>
        /// 
        /// </summary>
        sd_Arab_PK = 2137,
        /// <summary>
        /// 
        /// </summary>
        sd_Deva_IN = 1113,
        /// <summary>
        /// 
        /// </summary>
        si_LK = 1115,
        /// <summary>
        /// 
        /// </summary>
        sk_SK = 1051,
        /// <summary>
        /// 
        /// </summary>
        sl_SI = 1060,
        /// <summary>
        /// 
        /// </summary>
        so_SO = 1143,
        /// <summary>
        /// Spanish (Argentina)
        /// </summary>
        es_AR = 11274,
        /// <summary>
        /// Spanish (Bolivia)
        /// </summary>
        es_BO = 16394,
        /// <summary>
        /// Spanish (Chile)
        /// </summary>
        es_CL = 13322,
        /// <summary>
        /// Spanish (Colombia)
        /// </summary>
        es_CO = 9226,
        /// <summary>
        /// Spanish (Costa Rica)
        /// </summary>
        es_CR = 5130,
        /// <summary>
        /// Spanish (Dominican Republic)
        /// </summary>
        es_DO = 7178,
        /// <summary>
        /// Spanish (Ecuador)
        /// </summary>
        es_EC = 12298,
        /// <summary>
        /// Spanish (El Salvador)
        /// </summary>
        es_SV = 17418,
        /// <summary>
        /// Spanish (Guatemala)
        /// </summary>
        es_GT = 4106,
        /// <summary>
        /// Spanish (Honduras)
        /// </summary>
        es_HN = 18442,
        /// <summary>
        /// Spanish (Mexico)
        /// </summary>
        es_MX = 2058,
        /// <summary>
        /// Spanish (Nicaragua)
        /// </summary>
        es_NI = 19466,
        /// <summary>
        /// Spanish (Panama)
        /// </summary>
        es_PA = 6154,
        /// <summary>
        /// Spanish (Paraguay)
        /// </summary>
        es_PY = 15370,
        /// <summary>
        /// Spanish (Paraguay
        /// </summary>
        es_PE = 10250,
        /// <summary>
        /// Spanish (Puerto Rico)
        /// </summary>
        es_PR = 20490,
        /// <summary>
        /// Spanish (International Sort)
        /// </summary>
        es_ES = 3082,
        //Spanish(Spain � Traditional Sort)
        /// <summary>
        /// 
        /// </summary>
        es_ES_tradnl = 1034,
        //Spanish(United Sates)
        /// <summary>
        /// 
        /// </summary>
        es_US = 21514,
        //Spanish (Uruguay)
        /// <summary>
        /// 
        /// </summary>
        es_UY = 14346,
        //Spanish (Venezuela)
        /// <summary>
        /// 
        /// </summary>
        es_VE = 8202,
        //Sutu
        /// <summary>
        /// 
        /// </summary>
        st_ZA = 1072,
        //Swahili
        /// <summary>
        /// 
        /// </summary>
        sw_KE = 1089,
        //Swedish (Finland)
        /// <summary>
        /// 
        /// </summary>
        sv_FI = 2077,
        //Swedish (Sweden)
        /// <summary>
        /// 
        /// </summary>
        sv_SE = 1053,
        /// <summary>
        /// Syriac
        /// </summary>
        syr_SY = 1114,
        /// <summary>
        /// Tajik
        /// </summary>
        tg_Cyrl_TJ = 1064,
        /// <summary>
        /// Tamazight
        /// </summary>
        tzm_Arab_MA = 1119,
        /// <summary>
        /// Tamazight (Latin)
        /// </summary>
        tzm_Latn_DZ = 2143,
        /// <summary>
        /// Tamil
        /// </summary>
        ta_IN = 1097,
        /// <summary>
        /// Tatar
        /// </summary>
        tt_RU = 1092,
        /// <summary>
        /// Telugu
        /// </summary>
        te_IN = 1098,
        /// <summary>
        /// Thai
        /// </summary>
        th_TH = 1054,
        /// <summary>
        /// Tibetan (PRC)
        /// </summary>
        bo_CN = 1105,
        /// <summary>
        /// Tigrigna (Eritrea)
        /// </summary>
        ti_ER = 2163,
        /// <summary>
        /// Tigrigna (Ethiopia)
        /// </summary>
        ti_ET = 1139,
        /// <summary>
        /// Tsonga
        /// </summary>
        ts_ZA = 1073,
        /// <summary>
        /// Turkish
        /// </summary>
        tr_TR = 1055,
        /// <summary>
        /// Turkmen
        /// </summary>
        tk_TM = 1090,
        /// <summary>
        /// 
        /// </summary>
        ug_CN = 1152,
        /// <summary>
        /// Ukrainian
        /// </summary>
        uk_UA = 1058,
        /// <summary>
        /// 
        /// </summary>
        hsb_DE = 1070,
        /// <summary>
        /// Urdu
        /// </summary>
        ur_PK = 1056,
        /// <summary>
        /// Uzbek (Cyrillic)
        /// </summary>
        uz_Cyrl_UZ = 2115,
        /// <summary>
        /// Uzbek (Latin)
        /// </summary>
        uz_Latn_UZ = 1091,
        /// <summary>
        /// Venda
        /// </summary>
        ve_ZA = 1075,
        /// <summary>
        /// Vietnamese
        /// </summary>
        vi_VN = 1066,
        /// <summary>
        /// Welsh
        /// </summary>
        cy_GB = 1106,
        /// <summary>
        /// 
        /// </summary>
        wo_SN = 1160,
        /// <summary>
        /// 
        /// </summary>
        sah_RU = 1157,
        /// <summary>
        /// Yi
        /// </summary>
        ii_CN = 1144,
        /// <summary>
        /// Yiddish
        /// </summary>
        yi_Hebr = 1085,
        /// <summary>
        /// Yoruba
        /// </summary>
        yo_NG = 1130,
        /// <summary>
        /// Japanese
        /// </summary>
        ja_JP = 1041
    }
    #endregion

    /// <summary>
    /// Specifies preferred width type
    /// </summary>
    internal enum FtsWidth
    {
        /// <summary>
        /// No Preffered Width
        /// </summary>
        None = 0,
        /// <summary>
        /// Preferred table width specified as Auto
        /// </summary>
        Auto = 1,
        /// <summary>
        /// Preferred table width specified in percentage
        /// </summary>
        Percentage = 2,
        /// <summary>
        /// Preferred table width specified in points
        /// </summary>
        Point = 3
    }



}
