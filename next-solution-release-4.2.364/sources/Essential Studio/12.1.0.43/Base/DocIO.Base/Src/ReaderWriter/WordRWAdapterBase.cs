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
using System.Collections;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
#endregion

namespace Syncfusion.DocIO
{
    /// <summary>
    /// Summary description for WordRWAdapterBase.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal abstract class WordRWAdapterBase
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected IWordReader m_mainReader;

        /// <summary>
        /// 
        /// </summary>
        protected IWordWriter m_mainWriter;

        /// <summary>
        /// 
        /// </summary>
        protected int m_textPos;

        /// <summary>
        /// 
        /// </summary>
        //protected ArrayList m_commentCollection;

        /// <summary>
        /// 
        /// </summary>
        //protected ArrayList m_footnoteCollection;

        /// <summary>
        /// 
        /// </summary>
        //protected ArrayList m_endnoteCollection;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal WordRWAdapterBase()
        {
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected void Read(IWordReader reader)
        {
            m_mainReader = reader;
            m_mainReader.ReadDocumentHeader(null);
            ReadBody(m_mainReader);
            m_mainReader.ReadDocumentEnd();
        }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="document"></param>
        //    /// <param name="writer"></param>
        //    protected void Write( WordDocument document, IWordWriter writer )
        //    {
        //      m_mainWriter = writer;
        //      PreBuild( m_mainWriter );
        //      m_mainWriter.WriteDocumentHeader();
        //      WriteBody( document );
        //      m_mainWriter.WriteDocumentEnd();
        //    }
        #endregion

        #region Class methods / reading
        /// <summary>
        /// 
        /// </summary>
        protected virtual void ReadBody(IWordReader reader)
        {
            ReadStyleSheet(reader);
            ReadSubDocumentBody(reader, WordSubdocument.Footnote);
            ReadSubDocumentBody(reader, WordSubdocument.Annotation);
            ReadSubDocumentBody(reader, WordSubdocument.Endnote);
            ReadShapeObjectsBody(reader);

            (reader as WordReader).UnfreezeStreamPos();
            m_textPos = reader.CurrentTextPosition;

            while (reader.ReadChunk() != WordChunkType.DocumentEnd)
            {
                ReadChunkBefore(reader);
                ReadChunk(reader);
            }

            ReadHFBody(reader);
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void ReadHFBody(IWordReader reader);

        /// <summary>
        /// Textbox reader
        /// </summary>
        /// <param name="txbxReader"></param>
        /// <param name="txbxIndex"></param>
        protected abstract void ReadTextBoxBody(WordSubdocumentReader txbxReader, int txbxIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseReader"></param>
        protected virtual void ReadChunk(IWordReaderBase baseReader)
        {
            IWordReader reader = baseReader as IWordReader;
            switch (reader.ChunkType)
            {
                case WordChunkType.SectionEnd:
                    ReadSectionEnd(reader);
                    break;
                case WordChunkType.PageBreak:
                    //ReadPageBreak( reader );
                    ReadBreak(reader, BreakType.PageBreak);
                    break;
                case WordChunkType.ColumnBreak:
                    ReadBreak(reader, BreakType.ColumnBreak);
                    break;
                case WordChunkType.Footnote:
                    ReadFootnote(reader);
                    break;
                case WordChunkType.Annotation:
                    ReadAnnotation(reader);
                    break;
                default:
                    ReadChunkBase(reader);
                    return;
                //          break;
            }

            m_textPos = reader.CurrentTextPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void ReadChunkBase(IWordReaderBase reader)
        {
            switch (reader.ChunkType)
            {
                case WordChunkType.Text:
                    if (reader is IWordReader)
                    {
                        IWordReader wreader = reader as IWordReader;
                        if (wreader.IsEndnote || wreader.IsFootnote)
                        {
                            ReadFootnote(wreader);
                            break;
                        }
                    }

                    ReadText(reader);
                    break;
                case WordChunkType.ParagraphEnd:
                    ReadParagraphEnd(reader);
                    break;
                case WordChunkType.Image:
                    ReadImage(reader);
                    break;
                case WordChunkType.Table:
                    ReadTable(reader);
                    break;
                case WordChunkType.TableRow:
                    ReadTableRow(reader);
                    break;
                case WordChunkType.TableCell:
                    ReadTableCell(reader);
                    break;
                case WordChunkType.FieldBeginMark:
                    ReadField(reader);
                    break;
                case WordChunkType.FieldSeparator:
                    break;
                case WordChunkType.FieldEndMark:
                    ReadFieldEnd(reader);
                    break;
                case WordChunkType.LineBreak:
                    ReadLineBreak(reader);
                    break;
                case WordChunkType.Shape:
                    ReadShape(reader);
                    break;
                case WordChunkType.Symbol:
                    if (reader is IWordReader)
                    {
                        IWordReader wreader = reader as IWordReader;
                        if (wreader.IsEndnote || wreader.IsFootnote)
                        {
                            ReadFootnote(wreader);
                            break;
                        }
                    }

                    ReadSymbol(reader);
                    break;
                case WordChunkType.Footnote:
                    if (reader is WordFootnoteReader || reader is WordEndnoteReader)
                    {
                        ReadFootnoteMarker(reader);
                    }

                    break;
                case WordChunkType.Annotation:
                    //ReadAnnotationBody( reader );
                    break;

                case WordChunkType.CurrentPageNumber:
                    ReadCurrentPageNumber(reader);
                    break;
            }

            m_textPos = reader.CurrentTextPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="subDocument"></param>
        protected abstract void ReadSubDocumentBody(IWordReaderBase reader, WordSubdocument subDocument);
        #endregion

        #region Class abstract methods / reading chunk
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="reader"></param>
        //    protected abstract void ReadChunkAfter( IWordReader reader );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadChunkBefore(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadPageBreak(IWordReader reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="breakType"></param>
        protected abstract void ReadBreak(IWordReader reader, BreakType breakType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadSectionEnd(IWordReader reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadField(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadTable(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadTableRow(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadTableCell(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadImage(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadParagraphEnd(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadText(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadStyleSheet(IWordReader reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadLineBreak(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadShape(IWordReaderBase reader);

        /// <summary>
        /// Read textbox's shape data ( including FSPA )
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="txtShape"></param>
        protected abstract void ReadTextBoxShape(IWordReaderBase reader, TextBoxShape txtShape);

        /// <summary>
        /// Read image's shape data ( including FSPA )
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="imageShape"></param>
        protected abstract void ReadImageShape(IWordReaderBase reader, PictureShape imageShape);

        //    /// <summary>
        //    /// Read textboxs body
        //    /// </summary>
        //    /// <param name="reader"></param>
        //protected abstract void ReadTextBoxsBody( IWordReaderBase reader ); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadShapeObjectsBody(IWordReader reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadSymbol(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        protected abstract void ReadFieldEnd(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        protected abstract void ReadCurrentPageNumber(IWordReaderBase reader);

        /// <summary>
        /// Reads the annotation body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected abstract void ReadAnnotationBody(IWordReaderBase reader);

        /// <summary>
        /// Reads the annotation.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected abstract void ReadAnnotation(IWordReader reader);

        /// <summary>
        /// Reads the footnote body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected abstract void ReadFootnoteBody(IWordReaderBase reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadFootnote(IWordReader reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void ReadFootnoteMarker(IWordReaderBase reader);
        #endregion
    }
}

