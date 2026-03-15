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

using System;
using System.Diagnostics;
//using System.Drawing;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser;
using Syncfusion.Layouting;

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for IWordWriter.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal interface IWordWriterBase
    {
        /// <summary>
        /// Gets/sets stylesheet.
        /// </summary>
        WordStyleSheet StyleSheet { get; }

        /// <summary>
        /// Gets/sets index of current style.
        /// </summary>
        int CurrentStyleIndex { get; set; }

        /// <summary>
        /// Gets/sets character properties
        /// </summary>
        CharacterProperties CharacterProperties { get; set; }

        /// <summary>
        /// Gets/sets paragraph properties
        /// </summary>
        ParagraphProperties ParagraphProperties { get; set; }

        /// <summary>
        /// Gets/sets character properties for paragraph end symbol
        /// </summary>
        CharacterProperties BreakCharProperties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ListProperties ListProperties { get; }

        /// <summary>
        /// Writes text to word file
        /// </summary>
        /// <returns></returns>
        void WriteChunk(string textChunk);

        /// <summary>
        /// Writes marker to word file
        /// </summary>
        /// <param name="chunkType"></param>
        void WriteMarker(WordChunkType chunkType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nestingLevel"></param>
        void WriteCellMark(int nestingLevel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nestingLevel"></param>
        /// <param name="cellCount"></param>
        void WriteRowMark(int nestingLevel, int cellCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldcode"></param>
        /// <param name="hasSeparator"></param>
        void InsertStartField(string fieldcode, bool hasSeparator);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldcode"></param>
        /// <param name="fieldType"></param>
        /// <param name="hasSeparator"></param>
        void InsertStartField(string fieldcode, WField field, bool hasSeparator);

        /// <summary>
        /// 
        /// </summary>
        void InsertEndField();

        /// <summary>
        /// 
        /// </summary>
        void InsertFieldSeparator();

        /// <summary>
        /// Insert image in the document
        /// </summary>
        /// <param name="pict"></param>
        void InsertImage(WPicture pict);

        /// <summary>
        /// Insert image in the document
        /// </summary>
        /// <param name="pict"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        void InsertImage(WPicture pict, int height, int width);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pict"></param>
        /// <param name="pictProps"></param>
        void InsertShape(WPicture pict, PictureShapeProps pictProps);

        /// <summary>
        /// Insert textbox shape to the document
        /// </summary>
        /// <param name="txbxProps">Textbox shape's properties</param>
        int InsertTextBox(WTextBoxFormat txbxFormat);

        /// <summary>
        /// Inserts the form field.
        /// </summary>
        /// <param name="fieldcode">The fieldcode.</param>
        /// <param name="formField">The form field.</param>
        void InsertFormField(string fieldcode, FormField formField);

        /// <summary>
        /// Insert start of the bookmark in the document
        /// </summary>
        /// <param name="name"></param>
        void InsertBookmarkStart(string name, BookmarkStart start);

        /// <summary>
        /// Insert end of the bookmark in the document
        /// </summary>
        /// <returns></returns>
        void InsertBookmarkEnd(string name);

        /// <summary>
        /// Inserts the watermark.
        /// </summary>
        /// <param name="watermark">The watermark.</param>
        /// <param name="initsConvertor">The initialize converter.</param>
        /// <param name="maxWidth">Maximal width.</param>
        void InsertWatermark(Watermark watermark, UnitsConvertor initsConvertor, float maxWidth);

        /// <summary>
        /// Writes text to word file
        /// </summary>
        /// <returns></returns>
        void WriteSafeChunk(string textChunk);

        /// <summary>
        /// Inserts the field index entry.
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        void InsertFieldIndexEntry(string fieldCode);
    }

    /// <summary>
    /// Summary description for IWordWriter.
    /// </summary>
    [CLSCompliant(false)]
    internal interface IWordWriter : IWordWriterBase
    {
        /// <summary>
        /// Gets/sets document properties.
        /// </summary>
        DOPDescriptor DOP { get; }

        /// <summary>
        /// Gets/sets section properties.
        /// </summary>
        SectionProperties SectionProperties { get; }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets Built-in Document Properties.
        /// </summary>
        BuiltinDocumentProperties BuiltinDocumentProperties { get; }

        /// <summary>
        /// Gets Custom Document Properties.
        /// </summary>
        CustomDocumentProperties CustomDocumentProperties { get; }
#endif

        /// <summary>
        /// Writes document header to word file.
        /// </summary>
        void WriteDocumentHeader();

        /// <summary>
        /// Writes document end to word file.
        /// </summary>
        void WriteDocumentEnd(string password);

        /// <summary>
        /// Returns interface for writing subdocument.
        /// </summary>
        /// <returns></returns>
        IWordSubdocumentWriter GetSubdocumentWriter(WordSubdocument subDocumentType);

        /// <summary>
        /// Inserts PageBreak into the document.
        /// </summary>
        void InsertPageBreak();
    }

    /// <summary>
    /// Summary description for IWordSubdocumentWriter.
    /// </summary>
    [CLSCompliant(false)]
    internal interface IWordSubdocumentWriter : IWordWriterBase
    {
        /// <summary>
        /// Gets/sets type of subdocument
        /// </summary>
        WordSubdocument Type { get; }

        /// <summary>
        /// Writes end of the document.
        /// </summary>
        void WriteDocumentEnd();

        /// <summary>
        /// 
        /// </summary>
        void WriteItemStart();

        /// <summary>
        /// 
        /// </summary>
        void WriteItemEnd();
    }
}