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
using System.IO;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
#if!SILVERLIGHT && !WP
using System.Drawing;
#else
using Syncfusion.DocIO.DLS.Entities;
#endif

#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Common interface for main IWordReader interface and 
    /// IWordSubdocumentReader interface.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal interface IWordReaderBase
    {
        /// <summary>
        /// 
        /// </summary>
        bool HasTableBody { get; }

        /// <summary>
        /// Gets document stylesheet.
        /// </summary>
        WordStyleSheet StyleSheet { get; }

        /// <summary>
        /// Gets current style index.
        /// </summary>
        int CurrentStyleIndex { get; }

        /// <summary>
        /// Gets current read text chunk type.
        /// </summary>
        WordChunkType ChunkType { get; }

        /// <summary>
        /// Gets current read text chunk.
        /// </summary>
        string TextChunk { get; set; }

        /// <summary>
        /// Gets character properties for current text chunk.
        /// </summary>
        CharacterProperties CharacterProperties { get; }

        /// <summary>
        /// Gets paragraph properties for current text chunk.
        /// </summary>
        ParagraphProperties ParagraphProperties { get; }

        //// <summary>
        //// Gets character properties merged with current style properties.
        //// </summary>
        ////CharacterProperties FullCharacterProperties { get; }

        /// <summary>
        /// Gets paragraph properties merged with current style properties.
        /// </summary>
        ParagraphProperties FullParagraphProperties { get; }

        /// <summary>
        /// Gets current text position in document
        /// </summary>
        int CurrentTextPosition { get; set; }

        /// <summary>
        /// Gets bookmarks.
        /// </summary>
        BookmarkInfo[] Bookmarks { get; }

        /// <summary>
        /// Read next elementary text string*.
        /// </summary>
        /// <remarks>
        /// * - "elementary text string" - string, in which all symbols have the 
        /// identical character/paragraph/section properties.
        /// </remarks>
        /// <returns></returns>
        WordChunkType ReadChunk();

        /// <summary>
        /// Returns interface for reading images from word file.
        /// </summary>
        /// <returns></returns>
        IWordImageReader GetImageReader(WordDocument doc);

        /// <summary>
        /// Return object of ShapeClass.
        /// </summary>
        /// <returns></returns>
        ShapeBase GetDrawingObject();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        FormField GetFormField(FieldType fieldType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        bool ReadWatermark(WordDocument doc);
    }

    /// <summary>
    /// Interface for forward-only reading data from word file.
    /// </summary>
    [CLSCompliant(false)]
    internal interface IWordReader : IWordReaderBase
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsFootnote { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsEndnote { get; }

        /// <summary>
        /// 
        /// </summary>
        event NeedPasswordEventHandler NeedPassword;

        /// <summary>
        /// Gets current section number.
        /// </summary>
        int SectionNumber { get; }

        /// <summary>
        /// Gets section properties for current text chunk.
        /// </summary>
        SectionProperties SectionProperties { get; }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets built-in Document properties
        /// </summary>
        BuiltinDocumentProperties BuiltinDocumentProperties { get; }

        /// <summary>
        /// Gets custom Document properties
        /// </summary>
        CustomDocumentProperties CustomDocumentProperties { get; }
#endif

        /// <summary>
        /// 
        /// </summary>
        DOPDescriptor DOP { get; }

        /// <summary>
        /// Gets subdocument reader specified type.
        /// </summary>
        /// <returns></returns>
        IWordSubdocumentReader GetSubdocumentReader(WordSubdocument subDocumentType);

        /// <summary>
        /// Reads document header.
        /// </summary>
        /// <returns></returns>
        void ReadDocumentHeader(WordDocument doc);

        /// <summary>
        /// Reads document end.
        /// </summary>
        /// <returns></returns>
        void ReadDocumentEnd();

        /// <summary>
        /// Returns Bookmark array from word file
        /// </summary>
        /// <returns></returns>
        BookmarkInfo[] GetBookmarks();
    }

    /// <summary>
    /// Interface for reading data from subdocuments as header/footer, annotations, 
    /// footnotes, endnotes, etc.
    /// </summary>
    [CLSCompliant(false)]
    internal interface IWordSubdocumentReader : IWordReaderBase
    {
        //// <summary>         
        //// </summary>
        ////bool HasTableBody { get; }

        /// <summary>
        /// Gets subdocument reader type.
        /// </summary>
        WordSubdocument Type { get; }

        /// <summary>
        /// Gets current type of header / footer.
        /// ( if Type != WordSubdocument.HeaderFooter returns HeaderType.InvalidValue )
        /// </summary>
        HeaderType HeaderType { get; }

        /// <summary>
        /// 
        /// </summary>
        int ItemNumber
        {
            get;
        }

        /// <summary>
        /// Resets subdocument reader state, reading process restarted.
        /// </summary>
        void Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        void MoveToItem(int itemIndex);
    }

    /// <summary>
    /// Interface for reading images from word file.
    /// </summary>
    internal interface IWordImageReader
    {
        /// <summary>
        /// Gets image as bitmap.
        /// </summary>
        Image Image { get; }

        /// <summary>
        /// 
        /// </summary>
        int WidthScale { get; }

        /// <summary>
        /// 
        /// </summary>
        int HeightScale { get; }

        /// <summary>
        /// Gets image width.
        /// </summary>
        short Width { get; }

        /// <summary>
        /// Gets image height
        /// </summary>
        short Height { get; }
    }
}