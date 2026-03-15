#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

//#define CLXTABLE
#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.Layouting;
using Syncfusion.DocIO.Utilities;
using System.Collections.Generic;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    internal class AdapterListIDHolder
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        private static AdapterListIDHolder m_instance;
        /// <summary>
        /// Instance members
        /// </summary>
        private Dictionary<int, string> m_listStyleIDtoName;
        private Dictionary<int, string> m_lfoStyleIDtoName;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        private AdapterListIDHolder()
        {
            m_listStyleIDtoName = new Dictionary<int, string>();
            m_lfoStyleIDtoName = new Dictionary<int, string>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets AdapterListHelper object.
        /// </summary>
        internal static AdapterListIDHolder Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new AdapterListIDHolder();
                }
                return m_instance;
            }
        }
        /// <summary>
        /// Get the collection of list styles and their id's. 
        /// </summary>
        internal Dictionary<int, string> ListStyleIDtoName
        {
            get
            {
                return m_listStyleIDtoName;
            }
        }
        /// <summary>
        ///Get the collection of list format override indexes and appropriate list format override styles.
        /// </summary>
        internal Dictionary<int, string> LfoStyleIDtoName
        {
            get
            {
                return m_lfoStyleIDtoName;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Defines if current collection contains style.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	if it is a list name, set to <c>true</c>.
        /// </returns>
        internal bool ContainsListName(string name)
        {
            bool contains = false;
            IDictionaryEnumerator enumerator = m_listStyleIDtoName.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Value.Equals(name))
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }
        #endregion
    }
    internal class DocReaderAdapterBase
    {
        #region Fields
        protected List<WPicture> m_listPic;
        protected WTextBody m_textBody = null;
        protected ITextBodyItem m_currParagraph = null;
        private bool m_cellFinished = false;
        private bool m_rowFinished = false;
#if CLXTABLE
    private ComplexTable m_currCxTable;
#endif
        private WTable m_currTable;
        private Stack<WTable> m_tablesNested = new Stack<WTable>();
        protected Stack<WTextBody> m_nestedTextBodies = new Stack<WTextBody>();
        private WField m_currField = null;
        private Stack<WField> m_fieldStack = new Stack<WField>();
        private bool m_isPostFixBkmkStart;
        protected bool m_finalize;
        private WordChunkType m_prevChunkType;
        private bool m_ignoreField;
        private BookmarkInfo m_bookmarkInfo;
        private CharacterProperties m_characterProperties;
        #endregion

        #region Properties
        protected WordDocument DocumentEx;
        /// <summary>
        /// Gets the current paragraph.
        /// </summary>
        /// <value>The current paragraph.</value>
        protected WParagraph CurrentParagraph
        {
            get
            {
                if (m_currParagraph == null)
                {
                    m_currParagraph = m_textBody.AddParagraph();
                }

                return m_currParagraph as WParagraph;
            }
        }
        /// <summary>
        /// Gets current field.
        /// </summary>
        protected WField CurrentField
        {
            get
            {
                if (m_fieldStack.Count > 0)
                {
                    m_currField = m_fieldStack.Peek();
                }
                else
                {
                    m_currField = null;
                }

                return m_currField;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<WPicture> ListPictures
        {
            get
            {
                if (m_listPic == null)
                {
                    m_listPic = new List<WPicture>();
                }
                return m_listPic;
            }
        }
        /// <summary>
        /// Gets or sets if field text body.
        /// </summary>
        /// <value>If field text body.</value>
        //private WTextBody IfFieldTextBody
        //{
        //    get
        //    {
        //        if (m_ifFieldTextBody == null)
        //        {
        //            m_ifFieldTextBody = new WTextBody(DocumentEx, null);
        //        }
        //        return m_ifFieldTextBody;
        //    }
        //    set
        //    {
        //        m_ifFieldTextBody = value;
        //    }
        //}
        #endregion

        #region Public methods
        /// <summary>
        /// Initialize the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal void Init(WordDocument doc)
        {
            DocumentEx = doc;
            m_textBody = null;
            m_finalize = true;
        }
        #endregion

        #region Implementation / common
        /// <summary>
        /// Reads the text body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="textBody">The text body.</param>
        protected void ReadTextBody(WordReaderBase reader, WTextBody textBody)
        {
            m_textBody = textBody;
            m_currParagraph = null;

            while (!EndOfTextBody(reader, reader.ReadChunk()))
            {
                #region /* TRACE CHUNKS */
                // ==== DEBUG =========================== 
                /*
                if( inTable )
                {
                  Debug.Write( "[Level " + level + "]" );
                }

                switch( reader.ChunkType )
                {
                  case WordChunkType.Text:
                    Debug.WriteLine( reader.TextChunk );
                    break;
                  case WordChunkType.ParagraphEnd:
                    Debug.WriteLine( "[BR]" );
                    break;
                  case WordChunkType.TableRow:
                    Debug.WriteLine( "[TR]" );
                    break;
                  case WordChunkType.TableCell:
                    Debug.WriteLine( "[TD]" );
                    break;
                  default:
                    Debug.WriteLine( "[UNKNOWN]" );
                    break;
                }
                // ==== DEBUG =========================== */
                #endregion
                Preparation(reader);
                ProcessChunk(reader);
            }
            if (m_finalize) Finalize(reader);
        }
        /// <summary>
        /// Ends the of text body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="chunkType">Type of the chunk.</param>
        /// <returns></returns>
        protected virtual bool EndOfTextBody(WordReaderBase reader, WordChunkType chunkType)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Reads the chunk before.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void Preparation(WordReaderBase reader)
        {
            int prevLevel = m_nestedTextBodies.Count == 0 ? 0 : m_tablesNested.Count + 1;
            if (m_prevChunkType != WordChunkType.TableRow && prevLevel > reader.ParagraphProperties.TablesNestingLevel)
            {
                reader.ParagraphProperties.TablesNestingLevel = prevLevel;
            }
            PrepareTableInfo prepti = new PrepareTableInfo(reader, prevLevel);

            PrepareParagraph(reader, ref prepti);
            PrepareTable(reader, ref prepti);
            SetPostfixBkmks();
        }
        /// <summary>
        /// Reads the chunk.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ProcessChunk(WordReaderBase reader)
        {
            ProcessBookmarkAfterParaEnd(reader);
            switch (reader.ChunkType)
            {
                case WordChunkType.Text:
                    if (reader is IWordReader)
                    {
                        IWordReader wreader = reader as IWordReader;
                        if (wreader.IsEndnote || wreader.IsFootnote)
                        {
                            ReadFootnote(reader);
                            break;
                        }
                    }
                    ReadText(reader);
                    break;
                case WordChunkType.ParagraphEnd:
                    ReadParagraphEnd(reader);
                    break;
                case WordChunkType.PageBreak:
                    ReadBreak(reader, BreakType.PageBreak);
                    break;
                case WordChunkType.ColumnBreak:
                    ReadBreak(reader, BreakType.ColumnBreak);
                    break;
                case WordChunkType.LineBreak:
                    ReadBreak(reader, BreakType.LineBreak);
                    break;
                case WordChunkType.DocumentEnd:
                    ReadDocumentEnd(reader);
                    break;
                case WordChunkType.Image:
                    ReadImage(reader);
                    break;
                case WordChunkType.Shape:
                    ReadShape(reader);
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
                case WordChunkType.Footnote:
                    if (reader is WordFootnoteReader || reader is WordEndnoteReader)
                    {
                        ReadFootnoteMarker(reader);
                    }
                    else if (reader is IWordReader)
                    {
                        ReadFootnote(reader);
                    }
                    break;
                case WordChunkType.FieldBeginMark:
                    if (reader.CurrentBookmark != null)
                        AppendBookmark(reader.CurrentBookmark, reader.IsBookmarkStart);
                    ReadFldBeginMark(reader);
                    break;
                case WordChunkType.FieldSeparator:
                    InsertFldSeparator(reader);
                    break;
                case WordChunkType.FieldEndMark:
                    InsertFldEndMark(reader);
                    break;
                case WordChunkType.Tab:
                    ReadTab(reader);
                    break;
                case WordChunkType.Annotation:
                    ReadAnnotation(reader);
                    break;
                case WordChunkType.Symbol:
                    if (reader is IWordReader)
                    {
                        IWordReader wreader = reader as IWordReader;
                        if (wreader.IsEndnote || wreader.IsFootnote)
                        {
                            ReadFootnote(reader);
                            break;
                        }
                    }
                    ReadSymbol(reader);
                    break;
                case WordChunkType.CurrentPageNumber:
                    ReadCurrentPageNumber(reader);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported WordChunkType occured");
            }

            ProcessCommText(reader, m_currParagraph as WParagraph);
            ProcessBookmarks(reader);
            m_prevChunkType = reader.ChunkType;
        }
        /// <summary>
        /// 
        /// </summary>
        private void ProcessBookmarkAfterParaEnd(WordReaderBase reader)
        {
            if (reader.BookmarkAfterParaEnd != null)
            {
                AppendBookmark(reader.BookmarkAfterParaEnd, reader.IsBKMKStartAfterParaEnd);
                reader.BookmarkAfterParaEnd = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadFootnoteMarker(WordReaderBase reader)
        {
            IWTextRange marker = CurrentParagraph.AppendText(reader.TextChunk);
            ReadCharacterFormat(reader, marker.CharacterFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="prepti"></param>
        private void PrepareParagraph(WordReaderBase reader, ref PrepareTableInfo prepti)
        { }
        #endregion

        #region Implementation / common : table
        /// <summary>
        /// Reads the table before.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="prepti">The prepare table info.</param>
        private void PrepareTable(IWordReaderBase reader, ref PrepareTableInfo prepti)
        {
            // In table... handles cell- and row-finished state
            if (prepti.InTable)
            {
                if (m_cellFinished && reader.ChunkType != WordChunkType.TableRow)
                {
                    AppendTableCell(ref prepti);
                }
                else if (m_rowFinished && prepti.State != PrepareTableState.LeaveTable)
                {
                    AppendTableRow();
                }
            }

            // Handles entring or leaving table state...
            switch (prepti.State)
            {
                case PrepareTableState.EnterTable:
                    if (prepti.PrevLevel == 0)
                    {
                        m_nestedTextBodies.Push(m_textBody);
                    }
                    EnsureUpperTable(prepti.Level, reader.ChunkType);
                    break;

                case PrepareTableState.LeaveTable:
                    EnsureLowerTable(prepti.Level, reader.ChunkType);
                    break;
            }

            if (m_cellFinished) m_cellFinished = false;
            if (m_rowFinished)
            {
                m_currParagraph = null;
                m_rowFinished = false;
            }
        }
#if CLXTABLE
    /// <summary>
    /// Ensures the lower table.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="chunkType">Type of the chunk.</param>
    private void EnsureLowerTable( int level, WordChunkType chunkType )
    {
      while( m_tablesNested.Count > level )
      {
        m_tablesNested.Pop();
      }

      if( level == 0 )
      {
        m_textBody = ( WTextBody )m_nestedTextBodies.Pop();
        m_textBody.Items.Add( m_currCxTable.Table );
        m_currParagraph = null;
        m_currCxTable.SetNull();
      }
      else
      {
        WTable currTable = m_currCxTable.Table;
        m_currCxTable = ( ComplexTable )m_tablesNested.Pop();
        m_textBody = m_currCxTable.OneRowTable.LastCell;
        m_textBody.Items.Add( currTable );
        m_currParagraph = null;
      }
    }
    /// <summary>
    /// Ensures the upper table.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="chunkType">Type of the chunk.</param>
    private void EnsureUpperTable( int level, WordChunkType chunkType )
    {
      do
      {
        if( m_currCxTable.OneRowTable != null )
        {
          m_tablesNested.Push( m_currCxTable );
        }

        m_currCxTable.SetNull();
        m_currCxTable.CreateOneRowTable( DocumentEx );
        m_textBody = m_currCxTable.OneRowTable.LastCell;
      }
      while( m_tablesNested.Count < level - 1 );
    }
    /// <summary>
    /// Appends the table row.
    /// </summary>
    /// <param name="prepti"></param>
    private void AppendTableRow()
    {
      m_currCxTable.CreateOneRowTable( DocumentEx );
      m_textBody = m_currCxTable.OneRowTable.LastCell;
    }
    /// <summary>
    /// Appends the table cell.
    /// </summary>
    /// <param name="prepti"></param>
    private void AppendTableCell( ref PrepareTableInfo prepti )
    {
      m_currCxTable.OneRowTable.LastRow.AddCell();
      m_textBody = m_currCxTable.OneRowTable.LastCell;
    }
#else
        /// <summary>
        /// Ensures the lower table.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="chunkType">Type of the chunk.</param>
        private void EnsureLowerTable(int level, WordChunkType chunkType)
        {
            while (m_tablesNested.Count > level)
            {
                m_tablesNested.Pop();
            }

            if (level == 0)
            {
                m_textBody = m_nestedTextBodies.Pop();
                //m_textBody.Items.Add( m_currCxTable.Table );
                UpdateTableCellWidth();
                if (m_currTable.Owner == null)
                    m_textBody.Items.Add(m_currTable);
                m_currParagraph = null;
                //m_currCxTable.SetNull();
                m_currTable = null;
            }
            else
            {
                WTable currTable = m_currTable;
                m_currTable = m_tablesNested.Pop();
                m_textBody = m_currTable.LastCell;
                if (m_currTable.Owner == null)
                    m_textBody.Items.Add(currTable);
                m_currParagraph = null;
            }
        }
        /// <summary>
        /// Updates the table cells.
        /// </summary>
        private void UpdateTableCellWidth()
        {
            if (m_currTable.Rows.Count == 1
                && m_currTable.Rows[0].Cells.Count > 0)
            {
                WTableRow row = m_currTable.Rows[0];
                int mergedStartIndex = -1;
                float width = 0;
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (row.Cells[i].CellFormat.HorizontalMerge == CellMerge.Start)
                    {
                        mergedStartIndex = i;
                    }
                    else if (row.Cells[i].CellFormat.HorizontalMerge == CellMerge.Continue)
                    {
                        width += row.Cells[i].Width;
                        if (i == row.Cells.Count - 1
                            && mergedStartIndex > -1)
                        {
                            row.Cells[mergedStartIndex].Width += width;
                            mergedStartIndex = -1;
                            width = 0;
                        }
                    }
                    else if (mergedStartIndex > -1)
                    {
                        row.Cells[mergedStartIndex].Width += width;
                        mergedStartIndex = -1;
                        width = 0;
                    }
                }
            }
        }
        /// <summary>
        /// Ensures the upper table.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="chunkType">Type of the chunk.</param>
        private void EnsureUpperTable(int level, WordChunkType chunkType)
        {
            do
            {
                //if( m_currCxTable.OneRowTable != null )
                //{
                //  m_tablesNested.Push( m_currCxTable );
                //}

                //m_currCxTable.SetNull();
                //m_currCxTable.CreateOneRowTable( DocumentEx );
                //m_textBody = m_currCxTable.OneRowTable.LastCell;
                if (m_currTable != null)
                {
                    m_tablesNested.Push(m_currTable);
                }

                m_currTable = new WTable(DocumentEx);
                //m_currTable.TableFormat.Borders.BorderType = BorderStyle.None;
                AppendTableRow();
            }
            while (m_tablesNested.Count < level - 1);
        }
        /// <summary>
        /// Appends the table row.
        /// </summary>
        private void AppendTableRow()
        {
            //m_currCxTable.CreateOneRowTable( DocumentEx );
            //m_textBody = m_currCxTable.OneRowTable.LastCell;
            m_textBody = m_currTable.AddRow(false, false).AddCell(false);
        }
        /// <summary>
        /// Appends the table cell.
        /// </summary>
        /// <param name="prepti"></param>
        private void AppendTableCell(ref PrepareTableInfo prepti)
        {
            //m_currCxTable.OneRowTable.LastRow.AddCell();
            //m_textBody = m_currCxTable.OneRowTable.LastCell;
            m_textBody = m_currTable.LastRow.AddCell(false);
        }
#endif
        /// <summary>
        /// Finalize current part of read process
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void Finalize(WordReaderBase reader)
        {
            int prevLevel = m_nestedTextBodies.Count == 0 ? 0 : m_tablesNested.Count + 1;
            PrepareTableInfo prepti = new PrepareTableInfo(reader, prevLevel);

            PrepareTable(reader, ref prepti);

            if (reader is WordTextBoxReader || reader.ChunkType == WordChunkType.SectionEnd)
            {
                reader.RestoreBookmark();
                m_bookmarkInfo = null;
            }
            else
            {
                SetPostfixBkmks();
            }
        }
        #endregion

        #region Implementation / common : bookmark
        /// <summary>
        /// 
        /// </summary>
        private void SetPostfixBkmks()
        {
            if (m_bookmarkInfo != null)
            {
                AppendBookmark(m_bookmarkInfo, m_isPostFixBkmkStart);
                m_bookmarkInfo = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ProcessBookmarks(WordReaderBase reader)
        {
            if (reader.CurrentBookmark != null && reader.CurrentBookmark.Name != "_PictureBullets")
            {
                bool isCellGroupBkmk = reader.CurrentBookmark.IsCellGroupBookmark;
                if ((reader.ChunkType == WordChunkType.ParagraphEnd && IsParagraphBefore(reader)) ||
                  reader.ChunkType == WordChunkType.TableCell ||
                  reader.ChunkType == WordChunkType.TableRow)
                {
                    if (reader is WordTextBoxReader && reader.ChunkType == WordChunkType.ParagraphEnd ||
                      (reader.CurrentBookmark.IsCellGroupBookmark && !reader.IsBookmarkStart &&
                      (reader.CurrentBookmark.EndPos > reader.EndTextPos)))
                    {
                        reader.RestoreBookmark();
                    }
                    else
                    {
                        if ((reader.CurrentBookmark.IsCellGroupBookmark && !reader.IsBookmarkStart) &&
                          (reader.ChunkType == WordChunkType.TableCell || reader.ChunkType == WordChunkType.TableRow))
                        {
                            // Append BookmarkEnd to the specified by "CurrentBookmark.EndCellIndex" cell.
                            WTableRow lastRow = m_currTable.LastRow;
                            WTableCell cell = null;
                            if (lastRow.Cells.Count > reader.CurrentBookmark.EndCellIndex)
                                cell = lastRow.Cells[reader.CurrentBookmark.EndCellIndex];
                            else
                                cell = lastRow.Cells[lastRow.Cells.Count - 1];

                            IWParagraph para = (cell.LastParagraph == null) ? cell.AddParagraph() : cell.LastParagraph;
                            BookmarkEnd bkmkEnd = para.AppendBookmarkEnd(reader.CurrentBookmark.Name);
                            bkmkEnd.IsCellGroupBkmk = isCellGroupBkmk;
                        }
                        else
                        {
                            m_bookmarkInfo = reader.CurrentBookmark;
                            m_isPostFixBkmkStart = reader.IsBookmarkStart;
                        }
                    }
                }
                else if (reader.ChunkType != WordChunkType.FieldBeginMark)
                {
                    if (reader.ChunkType != WordChunkType.ParagraphEnd)
                    {
                        AppendBookmark(reader.CurrentBookmark, reader.IsBookmarkStart);
                        reader.BookmarkAfterParaEnd = null;
                    }
                    else
                    {
                        reader.BookmarkAfterParaEnd = reader.CurrentBookmark;
                        reader.IsBKMKStartAfterParaEnd = reader.IsBookmarkStart;
                    }
                }
                reader.CurrentBookmark = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsParagraphBefore(WordReaderBase reader)
        {
            if (reader.IsBookmarkStart)
            {
                string chunk = reader.TextChunk;
                int chunkStartPos = reader.CurrentTextPosition - chunk.Length;
                string prevString = chunk.Substring(0, reader.CurrentBookmark.StartPos - chunkStartPos);
                if (prevString == "\r")
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Appends bookmark.
        /// </summary>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <param name="isBookmarkStart">if it is a bookmark start, set to <c>true</c>.</param>
        /// <param name="isCellGroupBkmk">if it is a cell group BKMK, set to <c>true</c>.</param>
        private void AppendBookmark(BookmarkInfo bkmrInfo, bool isBookmarkStart)
        {
            if (isBookmarkStart)
            {
                BookmarkStart bkmkStart = CurrentParagraph.AppendBookmarkStart(bkmrInfo.Name);
                bkmkStart.ColumnFirst = bkmrInfo.StartCellIndex;
                bkmkStart.ColumnLast = bkmrInfo.EndCellIndex;
                bkmkStart.IsCellGroupBkmk = bkmrInfo.IsCellGroupBookmark;
            }
            else
            {
                BookmarkEnd bkmkEnd = CurrentParagraph.AppendBookmarkEnd(bkmrInfo.Name);
                bkmkEnd.IsCellGroupBkmk = bkmrInfo.IsCellGroupBookmark;
            }
        }
        #endregion

        #region Implementation / paragraph items : common
        /// <summary>
        /// Reads the text.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadText(WordReaderBase reader)
        {
            if (m_ignoreField)
                return;

            if (reader.TextChunk != null && reader.TextChunk.Length != 0)
            {
                if (reader.TextChunk == "\r")
                {
                    Break curBreak = new Break(DocumentEx, BreakType.LineBreak);
                    curBreak.TextRange.Text = "\r";
                    AddItem(curBreak, CurrentParagraph);
                    ReadCharacterFormat(reader, curBreak.TextRange.CharacterFormat);
                }
                else
                {
                    IWTextRange textRange = new WTextRange(DocumentEx);
                    textRange.Text = reader.TextChunk;
                    AddItem(textRange as ParagraphItem, CurrentParagraph);
                    ReadCharacterFormat(reader, textRange.CharacterFormat);
                    if (CurrentField is WTextFormField)
                        //Removes Text Range SPRM for TextFormField
                        textRange.CharacterFormat.Sprms.RemoveValue(WordSprmOptions.sprmCFSpec);
                }
            }
        }
        /// <summary>
        /// Reads the paragraph end.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadParagraphEnd(WordReaderBase reader)
        {

            ReadListFormat(reader, CurrentParagraph.ListFormat);
            ReadCharacterFormat(reader, CurrentParagraph.BreakCharacterFormat);
            ReadParagraphFormat(reader, CurrentParagraph);

            UpdateParagraphStyle(CurrentParagraph, reader);
            m_currParagraph = null;
        }
        /// <summary>
        /// Reads the symbol.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadSymbol(WordReaderBase reader)
        {
            SymbolDescriptor symbolDescriptor = reader.CharacterProperties.Symbol;
            WSymbol symbol = new WSymbol(DocumentEx);
            AddItem(symbol, CurrentParagraph);
            symbol.CharacterCode = symbolDescriptor.CharCode;
            symbol.CharCodeExt = symbolDescriptor.CharCodeExt;
            symbol.FontName = (string)reader.StyleSheet.FontNamesList[symbolDescriptor.FontCode];
            ReadCharacterFormat(reader, symbol.CharacterFormat);
            //Special case for Consecutive  symbol
            for(int i=1;i<reader.TextChunk.Length;i++)
                AddItem((WSymbol)symbol.Clone(), CurrentParagraph);
        }
        /// <summary>
        /// Reads the current page number.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadCurrentPageNumber(WordReaderBase reader)
        {
            CurrentParagraph.AppendField("", FieldType.FieldPage);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        private void ReadTab(WordReaderBase reader)
        {
            //throw new Exception( "The method or operation is not implemented." );
        }
        #endregion

        #region Implementation / paragraph items : table
#if CLXTABLE
    /// <summary>
    /// Reads the table cell.
    /// </summary>
    /// <param name="reader">The reader.</param>
    private void ReadTableCell( WordReaderBase reader )
    {
      m_cellFinished = true;
      WTableCell cell = ( WTableCell )m_currCxTable.OneRowTable.LastCell;
      if( m_currParagraph != null || m_prevChunkType == WordChunkType.ParagraphEnd 
        || m_prevChunkType == WordChunkType.TableRow )
      {
        ReadListFormat( reader, CurrentParagraph.ListFormat );
        ReadParagraphFormat( reader, CurrentParagraph );
      }
      ReadCharacterFormat( reader, cell.CharacterFormat );
      m_currParagraph = null;
    }
    /// <summary>
    /// Reads the table row.
    /// </summary>
    /// <param name="reader">The reader.</param>
    private void ReadTableRow( WordReaderBase reader )
    {
      m_rowFinished = true;

      if( m_currCxTable.OneRowTable != null )
      {
        ReadTableRowFormat( reader, m_currCxTable.OneRowTable );

        if( m_currCxTable.Table == null )
        {
          m_currCxTable.AppendOneRowToTable();
        }
        else
        {
          WTableRow lastRow = m_currCxTable.OneRowTable.LastRow;

          m_currCxTable.Table.Rows.Add( lastRow );
          m_currCxTable.OneRowTable = null;
        }
      }
    }
#else
        /// <summary>
        /// Reads the table cell.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadTableCell(WordReaderBase reader)
        {
            m_cellFinished = true;
            WTableCell cell = m_currTable.LastCell;
            if (cell.Items.Count == 0)
                m_currParagraph = cell.AddParagraph() as WParagraph;

            if (m_currParagraph != null || m_prevChunkType == WordChunkType.ParagraphEnd
                || m_prevChunkType == WordChunkType.TableRow || m_prevChunkType == WordChunkType.TableCell)
            {
                ReadListFormat(reader, CurrentParagraph.ListFormat);
                ReadParagraphFormat(reader, CurrentParagraph);
            }
            ReadCharacterFormat(reader, cell.CharacterFormat);
            CurrentParagraph.BreakCharacterFormat.ImportContainer(cell.CharacterFormat);
            m_currParagraph = null;
        }
        /// <summary>
        /// Reads the table row.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadTableRow(WordReaderBase reader)
        {
            m_rowFinished = true;
            if (m_prevChunkType != WordChunkType.TableCell && m_currTable != null)
            {
                m_currTable.LastRow.Cells.RemoveAt(m_currTable.LastRow.Cells.Count - 1);
            }

            if (m_currTable != null)
            {
                if (m_currTable.Rows.Count > 1
                && IsSplitTableRows(reader, m_currTable.Rows[m_currTable.Rows.Count-2].RowFormat.Sprms, reader.ParagraphProperties.Sprms))
                {
                    WTextBody textBody = m_textBody;
                    //Splits the adjacent table row to different table. 
                    if (m_textBody == m_currTable.LastCell)
                    {
                        textBody = m_nestedTextBodies.Peek();
                        WTable table = new WTable(m_currTable.Document);
                        table.Rows.Add(m_currTable.LastRow);
                        if (m_currTable.Owner == null)
                        {
                            textBody.Items.Add(m_currTable);
                            if (reader.ParagraphProperties.Sprms[WordSprmOptions.sprmTPositionCode] == null)
                            {
                                //Inserts hidden paragraph between adjacent tables.
                                WParagraph para = new WParagraph(m_currTable.Document);
                                textBody.Items.Add(para);
                                para.BreakCharacterFormat.Hidden = true;
                            }
                        }
                        m_currTable = table;
                    }
                }
                ReadTableRowFormat(reader, m_currTable);
                if (m_currTable.LastRow.Cells.Count < 1)
                {
                    m_currTable.Rows.Remove(m_currTable.LastRow);
                }
                else
                {
                    m_currTable.LastRow.HasTblPrEx = true;
                }
            }
            else
            {
                WSymbol symbol = CurrentParagraph.AppendSymbol((byte)SpecialCharacters.TableAscii);
                ReadCharacterFormat(reader, symbol.CharacterFormat);
                symbol.FontName = reader.CharacterProperties.Sprms.Contain(WordSprmOptions.sprmCFtcBi) ?
                    reader.CharacterProperties.FontNameBi : reader.CharacterProperties.FontName;
                m_rowFinished = false;
            }
        }
        /// <summary>
        /// Determines whether to split adjacent table rows.
        /// </summary>
        /// <param name="previousRowSprms">The previous row SPRMS.</param>
        /// <param name="currentRowSprms">The current row SPRMS.</param>
        /// <returns>
        /// 	<c>true</c> if split adjacent table rows; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSplitTableRows(WordReaderBase reader, SinglePropertyModifierArray previousRowSprms, SinglePropertyModifierArray currentRowSprms)
        {
            bool isSplitTableRows = false;
            //Two adjacent table rows of the same table depth are considered part of the same table unless they differ in one of the following properties:
            //sprmTIpgp,sprmTIstd,sprmTFBidi,sprmTFBidi90,sprmTPc, sprmTFNoAllowOverlap, sprmTDxaAbs, sprmTDyaAbs, sprmTDxaFromText, sprmTDyafromText, sprmTDxaFromTextRight,sprmTDyaFromTextBottom
            int[] sprmsToCheck = { 0x7469, 0x563A, 0x5664, 0x560B, 0x360D, 0x3465, 0x940E, 0x940F, 0x9410, 0x9411, 0x941E, 0x941F }; //, 0x740A
            if (previousRowSprms != null
                && currentRowSprms != null)
            {
                foreach (int val in sprmsToCheck)
                {
                    SinglePropertyModifierRecord previousRowSprm = previousRowSprms[val];
                    SinglePropertyModifierRecord currentRowSprm = currentRowSprms[val];
                    if (previousRowSprm != null
                        && currentRowSprm != null)
                    {
                        if (!CompareArray(previousRowSprm.ByteArray, currentRowSprm.ByteArray))
                        {
                            isSplitTableRows = true;
                            break;
                        }
                    }
                    else if ((previousRowSprm != null && currentRowSprm == null) ||
                        (previousRowSprm == null && currentRowSprm != null))
                    {
                        isSplitTableRows = true;
                        break;
                    }
                }
            }
            //Checks whether the file format is latest to Word 2000.
            //Uncomment this condition if there is any issue in Word 97, 2000 generated documents, which does not support table styles.
            //if (DocumentEx.WordVersion > 0xD9)
            //For the file format Word 2002, and later.
            if (!isSplitTableRows)
            {
                SinglePropertyModifierRecord previoussprmTIstd = null, currentsprmTIstd = null;
                SinglePropertyModifierRecord extendedTableProps = previousRowSprms[WordSprmOptions.sprmPTableProps];
                if (extendedTableProps != null && reader.m_streamsManager.DataStream != null)
                {
                    if (reader.m_streamsManager.DataStream.Length > extendedTableProps.UIntValue)
                        reader.m_streamsManager.DataStream.Position = extendedTableProps.UIntValue;
                    if (reader.m_streamsManager.DataStream.Position + 2 < reader.m_streamsManager.DataStream.Length)
                    {
                        int cbGrpprl = reader.m_streamsManager.DataReader.ReadUInt16();
                        if (cbGrpprl <= 16290)
                        {
                            SinglePropertyModifierArray sprms = new SinglePropertyModifierArray();
                            if (reader.m_streamsManager.DataStream.Position + cbGrpprl < reader.m_streamsManager.DataStream.Length)
                                sprms.Parse(reader.m_streamsManager.DataStream, cbGrpprl);
                            previoussprmTIstd = sprms[WordSprmOptions.sprmTIstd];
                        }
                    }
                }
                extendedTableProps = currentRowSprms[WordSprmOptions.sprmPTableProps];
                if (extendedTableProps != null && reader.m_streamsManager.DataStream != null)
                {
                    if (reader.m_streamsManager.DataStream.Length > extendedTableProps.UIntValue)
                        reader.m_streamsManager.DataStream.Position = extendedTableProps.UIntValue;
                    if (reader.m_streamsManager.DataStream.Position + 2 < reader.m_streamsManager.DataStream.Length)
                    {
                        int cbGrpprl = reader.m_streamsManager.DataReader.ReadUInt16();
                        if (cbGrpprl <= 16290)
                        {
                            SinglePropertyModifierArray sprms = new SinglePropertyModifierArray();
                            if (reader.m_streamsManager.DataStream.Position + cbGrpprl < reader.m_streamsManager.DataStream.Length)
                                sprms.Parse(reader.m_streamsManager.DataStream, cbGrpprl);
                            currentsprmTIstd = sprms[WordSprmOptions.sprmTIstd];
                        }
                    }
                }
                if (previoussprmTIstd != null
                    && currentsprmTIstd != null)
                    if (!CompareArray(previoussprmTIstd.ByteArray, currentsprmTIstd.ByteArray))
                        isSplitTableRows = true;
                    else if ((previoussprmTIstd != null && currentsprmTIstd == null) ||
                        (previoussprmTIstd == null && currentsprmTIstd != null))
                        isSplitTableRows = true;
            }
            return isSplitTableRows;
        }
        /// <summary>
        /// Compares the array.
        /// </summary>
        /// <param name="buffer1">The buffer1.</param>
        /// <param name="buffer2">The buffer2.</param>
        /// <returns></returns>
        private bool CompareArray(byte[] buffer1, byte[] buffer2)
        {
            bool equal = true;
            for (int i = 0; i < buffer1.Length; i++)
            {
                if (buffer1[i] != buffer2[i])
                {
                    equal = false;
                    break;
                }
            }
            return equal;
        }
#endif
        private void ReadTable(WordReaderBase reader)
        {
            ReadTableRow(reader);
        }
        #endregion

        #region Implementation / paragraph items : subdocument, other
        /// <summary>
        /// Reads the annotation.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadAnnotation(WordReaderBase reader)
        {
            //throw new Exception( "The method or operation is not implemented." );
        }
        /// <summary>
        /// Reads the footnote.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadFootnote(WordReaderBase reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Reads the break.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="breakType">Type of the break.</param>
        private void ReadBreak(WordReaderBase reader, BreakType breakType)
        {
            Break curBreak = null;
            if (breakType == BreakType.LineBreak)
            {
                curBreak = new Break(DocumentEx, BreakType.LineBreak);
                curBreak.TextRange.Text = reader.TextChunk;
            }
            else
            {
                curBreak = new Break(DocumentEx, breakType);
            }
            AddItem(curBreak, CurrentParagraph);
            ReadCharacterFormat(reader, curBreak.TextRange.CharacterFormat);
        }
        /// <summary>
        /// Reads the document end.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadDocumentEnd(WordReaderBase reader)
        { }
        #endregion

        #region Implementation / paragraph items : shapes
        /// <summary>
        /// Reads the shape.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadShape( WordReaderBase reader )
        {
          if( reader.ReadWatermark( DocumentEx ) )
          {
            if( m_textBody is HeaderFooter )
            {
              ( m_textBody as HeaderFooter ).WriteWatermark = true;
            }
          }
          else
          {
            FileShapeAddress fspa = reader.GetFSPA();
            if (fspa == null)
                return;
            fspa.IsHeaderShape = ( reader is WordHeaderFooterReader ) ? true : false;

            MsofbtSpContainer container = null;
            if( DocumentEx.Escher.Containers.ContainsKey( fspa.Spid ) )
            {
              container = DocumentEx.Escher.Containers[ fspa.Spid ] as MsofbtSpContainer;
            }

            if( container != null && container.Shape.ShapeType == EscherShapeType.msosptTextBox )
            {
              ReadTextBox( reader, fspa );
            }
            else if( container != null && container.Shape.ShapeType == EscherShapeType.msosptPictureFrame )
            {
              ShapeBase shape = reader.GetDrawingObject();
              if( shape is PictureShape )
              {
                ReadPictureShape( reader, shape, container);
              }
            }
            else
            {
              ReadAutoShape( reader );
            }
          }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool ReadWatermark(WordReaderBase reader)
        {
            return false;
        }
        /// <summary>
        /// Read textbox.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fspa">The File Shape Address.</param>
        protected virtual void ReadTextBox(WordReaderBase reader, FileShapeAddress fspa)
        { }
        /// <summary>
        /// Reads picture shape.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="shape"></param>
        private void ReadPictureShape(WordReaderBase reader, ShapeBase shape, MsofbtSpContainer shapeContainer)
        {
            PictureShape pictShape = shape as PictureShape;
            if (pictShape.ImageRecord != null)
            {
                WPicture picture = new WPicture(DocumentEx);
                CurrentParagraph.LoadPicture(picture, pictShape.ImageRecord);
                AddItem(picture, CurrentParagraph);

                BaseProps shapeProps = pictShape.ShapeProps;
                float height = (float)shapeProps.Height / DLSConstants.TwipsInOnePoint;
                float width = (float)shapeProps.Width / DLSConstants.TwipsInOnePoint;
                float vertPos = (float)shapeProps.YaTop / DLSConstants.TwipsInOnePoint;
                float horPos = (float)shapeProps.XaLeft / DLSConstants.TwipsInOnePoint;
                
                SizeF pictureSize = new SizeF(width, height);
                picture.Size = pictureSize;
                picture.HeightScale = 100;
                picture.WidthScale = 100;
                picture.VerticalOrigin = pictShape.ShapeProps.RelVrtPos;
                picture.HorizontalOrigin = pictShape.ShapeProps.RelHrzPos;
                picture.VerticalPosition = vertPos;
                picture.HorizontalPosition = horPos;
                picture.TextWrappingStyle = pictShape.ShapeProps.TextWrappingStyle;
                picture.TextWrappingType = pictShape.ShapeProps.TextWrappingType;
                picture.IsBelowText = pictShape.ShapeProps.IsBelowText;
                picture.HorizontalAlignment = pictShape.ShapeProps.HorizontalAlignment;
                picture.VerticalAlignment = pictShape.ShapeProps.VerticalAlignment;
                picture.ShapeId = pictShape.ShapeProps.Spid;
                picture.IsHeaderPicture = pictShape.ShapeProps.IsHeaderShape;
                picture.AlternativeText = pictShape.PictureProps.AlternativeText;
                if (shapeContainer.ShapePosition != null)
                    picture.LayoutInCell = shapeContainer.ShapePosition.AllowInTableCell;
                if (shapeContainer.ShapeOptions != null)
                {
                    picture.DistanceFromBottom = shapeContainer.ShapeOptions.DistanceFromBottom / DLSConstants.EmusPerPoint;
                    picture.DistanceFromLeft = shapeContainer.ShapeOptions.DistanceFromLeft / DLSConstants.EmusPerPoint;
                    picture.DistanceFromRight = shapeContainer.ShapeOptions.DistanceFromRight / DLSConstants.EmusPerPoint;
                    picture.DistanceFromTop = shapeContainer.ShapeOptions.DistanceFromTop/ DLSConstants.EmusPerPoint;
                }
                if (!picture.IsBelowText && picture.TextWrappingStyle == TextWrappingStyle.Behind)
                    picture.TextWrappingStyle = TextWrappingStyle.InFrontOfText;
                //Read the wrap polygon vertices.
                if (picture.TextWrappingStyle == TextWrappingStyle.Through || picture.TextWrappingStyle == TextWrappingStyle.Tight)
                {
                    if (shapeContainer.ShapeOptions != null && shapeContainer.ShapeOptions.Properties.Contains((int)FOPTEGroupShape.pWrapPolygonVertices))
                    {
                        picture.WrapPolygon = new WrapPolygon();
                        picture.WrapPolygon.Edited = false;
                        for (int i = 0; i < shapeContainer.ShapeOptions.WrapPolygonVertices.Coords.Count; i++)
                            picture.WrapPolygon.Vertices.Add(shapeContainer.ShapeOptions.WrapPolygonVertices.Coords[i]);
                    }
                }
                CheckTextEmbed(shape, picture);
                picture.PictureShape.ShapeContainer = shapeContainer;
            }
        }
        /// <summary>
        /// Checks the text embed.
        /// </summary>
        /// <param name="shape">The shape.</param>
        protected virtual void CheckTextEmbed(ShapeBase shape, WPicture picture)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadAutoShape(WordReaderBase reader)
        {
            ShapeObject shapeObj = new ShapeObject(DocumentEx);
            shapeObj.FSPA = reader.GetFSPA();
            ReadCharacterFormat(reader, shapeObj.CharacterFormat);
            if (reader is WordHeaderFooterReader)
            {
                shapeObj.IsHeaderAutoShape = true;
            }
            //if( shapeObj.FSPA != null )
            //{
            if (DocumentEx.Escher.Containers.ContainsKey(shapeObj.FSPA.Spid) 
                /*&& !IsUnsupportedSpType( baseContainer )*/ )
            {
                AddItem(shapeObj, CurrentParagraph);
                ReadAutoShapeText(DocumentEx.Escher.Containers[shapeObj.FSPA.Spid], shapeObj);
            }
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        private bool IsUnsupportedSpType(BaseContainer baseContainer)
        {
            bool retVal = false;
            if (baseContainer is MsofbtSpContainer)
            {
                EscherShapeType spType = (baseContainer as MsofbtSpContainer).Shape.ShapeType;
                if (spType == EscherShapeType.msosptHostControl)
                {
                    retVal = true;
                    Debug.WriteLine("MsosptHostControl shape type (OLE object): shape is skipped");
                }
            }
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeContainer"></param>
        /// <param name="shapeObj"></param>
        private void ReadAutoShapeText(BaseContainer shapeContainer, ShapeObject shapeObj)
        {
            BaseEscherRecord baseRecord = null;
            for (int i = 0, cnt = shapeContainer.Children.Count; i < cnt; i++)
            {
                baseRecord = shapeContainer.Children[i] as BaseEscherRecord;
                if (baseRecord is MsofbtSp)
                {
                    MsofbtSp shape = baseRecord as MsofbtSp;
                    if (shape.ShapeType != EscherShapeType.msosptPictureFrame)
                    {
                        ReadAutoShapeTextBox(shape.ShapeId, shapeObj);
                    }
                }
                else if (baseRecord is BaseContainer)
                {
                    ReadAutoShapeText(baseRecord as BaseContainer, shapeObj);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeId"></param>
        /// <param name="shapeObj"></param>
        protected virtual void ReadAutoShapeTextBox(int shapeId, ShapeObject shapeObj)
        { }
        /// <summary>
        /// Reads inline images.
        /// </summary>
        /// <param name="reader"></param>
        private void ReadImage(WordReaderBase reader)
        {
            if (CurrentField is WMergeField)
            {
                FormField formField = reader.GetFormField(CurrentField.FieldType);
                FormFieldPropertiesConverter.ReadFormFieldProperties(CurrentField as WFormField, formField);
            }
            else if (CurrentField is WFormField && reader.CharacterProperties.IsData)
            {
                FormField formField = reader.GetFormField(CurrentField.FieldType);
                FormFieldPropertiesConverter.ReadFormFieldProperties(CurrentField as WFormField, formField);
                (CurrentField as WFormField).HasFFData = true;
            }
            else
            {
                WordImageReader imReader = (WordImageReader)reader.GetImageReader(DocumentEx);
                ShapeObject shapeObj = new InlineShapeObject(DocumentEx);
                (shapeObj as InlineShapeObject).ShapeContainer = imReader.InlineShapeContainer;
                (shapeObj as InlineShapeObject).PictureDescriptor = imReader.PictureDescriptor;
                if (imReader.UnparsedData != null)
                    (shapeObj as InlineShapeObject).UnparsedData = imReader.UnparsedData;

                CharacterPropertiesConverter.CHPToFormat(reader, shapeObj.CharacterFormat);

                if (imReader.ImageRecord == null || imReader.ImageRecord.m_imageBytes == null)
                {
                    if (reader.CharacterProperties.IsOle2)
                    {
                        (shapeObj as InlineShapeObject).IsOLE = true;
                        (shapeObj as InlineShapeObject).OLEContainerId = reader.CharacterProperties.PicLocation;
                    }
                    if (!reader.CharacterProperties.Hidden)
                        AddItem(shapeObj, CurrentParagraph);
                }
                else
                {
                    WPicture picture = null;
                    if (reader.CharacterProperties.Hidden && imReader.ImageRecord.m_imageBytes.Length > 0)
                    {
                        picture = new WPicture(DocumentEx);
                        picture.LoadImage(imReader.ImageRecord);
                        ListPictures.Add(picture);
                    }
                    else
                    {
                        picture = new WPicture(DocumentEx);
                        CurrentParagraph.LoadPicture(picture, imReader.ImageRecord);
                        AddItem(picture, CurrentParagraph);
                        picture.IsShape = true;
                        if (CurrentField != null)
                        {
                            if ((CurrentField.FieldType == FieldType.FieldLink || CurrentField.FieldType == FieldType.FieldEmbed)
                              && CurrentField.Owner is WOleObject)
                            {
                                WOleObject oleObject = CurrentField.Owner as WOleObject;
                                oleObject.SetOlePicture(picture);
                            }
                        }
                    }

                    float height = (float)imReader.Height / DLSConstants.TwipsInOnePoint;
                    float heightScale = (float)imReader.HeightScale / DLSConstants.ImageScalingFactor;
                    float width = (float)imReader.Width / DLSConstants.TwipsInOnePoint;
                    float widthScale = (float)imReader.WidthScale / DLSConstants.ImageScalingFactor;
                    CharacterPropertiesConverter.CHPToFormat(reader, picture.PictureCharacterFormat);

                    SizeF pictureSize = new SizeF(width, height);
                    picture.Size = pictureSize;

                    //          picture.Height = height;
                    //          picture.Width = width;

                    picture.HeightScale = heightScale;
                    picture.WidthScale = widthScale;
                    picture.PictureShape = shapeObj as InlineShapeObject;
                    picture.AlternativeText = imReader.AlternativeText;
                    if (picture.PictureShape != null && picture.PictureShape.ShapeContainer != null && picture.PictureShape.ShapeContainer.ShapePosition != null)
                        picture.LayoutInCell = picture.PictureShape.ShapeContainer.ShapePosition.AllowInTableCell;
                }
            }
        }

        #endregion

        #region Implementation / paragraph items : fields
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="para">The para.</param>
        private void AddItem(ParagraphItem item, IWParagraph para)
        {
            para.Items.Add(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadFldBeginMark(WordReaderBase reader)
        {
            FieldDescriptor fld = reader.GetFld();

            if (fld != null)
            {
                //if( fld.Type == FieldType.FieldOCX )
                //{
                //  if( DocumentEx.ThrowExceptionsForUnsupportedElements )
                //  {
                //    throw new NotImplementedException( "Controls are not supported." );
                //  }
                //  m_ignoreField = true;
                //  return;
                //}
            }

            ReadFldBeginMark(reader, fld);
        }
        /// <summary>
        /// Reads field separator.
        /// </summary>
        /// <param name="reader"></param>
        private void InsertFldSeparator(WordReaderBase reader)
        {
            if (m_ignoreField)
                return;

            if (CurrentField is WControlField && reader.CharacterProperties.PicLocation > 0)
            {
                (CurrentField as WControlField).StoragePicLocation = reader.CharacterProperties.PicLocation;
                if (reader.m_streamsManager.ObjectPoolStream != null)
                    (CurrentField as WControlField).OleObject.ParseObjectPool(reader.m_streamsManager.ObjectPoolStream, "_" + (CurrentField as WControlField).StoragePicLocation.ToString());
            }

            WFieldMark fldSeparator = new WFieldMark(DocumentEx, FieldMarkType.FieldSeparator);
            AddItem(fldSeparator, CurrentParagraph);
            CurrentField.FieldSeparator = fldSeparator;
            CharacterPropertiesConverter.CHPToFormat(reader, fldSeparator.CharacterFormat);
        }
        /// <summary>
        /// Reads field end.
        /// </summary>
        /// <param name="reader"></param>
        private void InsertFldEndMark(WordReaderBase reader)
        {
            if (m_ignoreField)
            {
                m_ignoreField = false;
                return;
            }

            if (m_fieldStack.Count > 0)
            {
                if (CurrentField.FieldType == FieldType.FieldFillIn &&
                    CurrentParagraph.Items.Count > 0 && CurrentParagraph.LastItem == CurrentField)
                {
                    InsertFldSeparator(reader);
                }

                WFieldMark fldEnd = new WFieldMark(DocumentEx, FieldMarkType.FieldEnd);
                AddItem(fldEnd, CurrentParagraph);
                CurrentField.FieldEnd = fldEnd;
                CharacterPropertiesConverter.CHPToFormat(reader, fldEnd.CharacterFormat);

                if (CurrentField != null 
                    && (CurrentField.FieldType == FieldType.FieldDate
                    || CurrentField.FieldType == FieldType.FieldTime))
                    CurrentField.Update();
                // Remove current field from stack
                m_fieldStack.Pop();
            }
            else
            {
                Debug.WriteLine("Field end not found.");
            }
        }
        /// <summary>
        /// Reads the FLD begin mark.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fld">The FLD.</param>
        private void ReadFldBeginMark(WordReaderBase reader, FieldDescriptor fld)
        {
            // Read field code
            FormField formField = null;
            string fieldCode = ReadFieldCode(reader, fld, ref formField);
            m_currField = ParseFieldCode(fieldCode);
            if ( fld!=null && m_currField.FieldType == FieldType.FieldUnknown)
                m_currField.SourceFieldType = (int)fld.Type;
            WordChunkType chunkType = reader.ChunkType;

            // Read MergeField, FieldNext, FieldFormula, OleObject or FieldExpression field. 
            ReadSpecialField(reader, fld, fieldCode);

            if (m_currField.FieldType != FieldType.FieldMergeField &&
                m_currField.FieldType != FieldType.FieldNext &&
                m_currField.FieldType != FieldType.FieldTOC &&
                m_currField.FieldType != FieldType.FieldLink &&
                m_currField.FieldType != FieldType.FieldEmbed)
            {
                // Convert FormField Propeties
                if (formField != null)
                {
                    FormFieldPropertiesConverter.ReadFormFieldProperties(m_currField as WFormField, formField);
                }
                else if (m_currField is WFormField)
                    (m_currField as WFormField).HasFFData = false;

                InsertStartField(reader);

                if (chunkType == WordChunkType.FieldSeparator ||
                  chunkType == WordChunkType.FieldBeginMark ||
                  chunkType == WordChunkType.FieldEndMark)
                {
                    ProcessChunk(reader);
                }
            }
        }
        /// <summary>
        /// Reads field code.
        /// </summary>
        /// <returns></returns>
        private string ReadFieldCode(WordReaderBase reader, FieldDescriptor fld, ref FormField formField)
        {
            bool isFormField = FieldTypeDefiner.IsFormField(fld);
            string fieldCode = string.Empty;
            WordChunkType chunkType;

            do
            {
                chunkType = reader.ReadChunk();

                if (chunkType == WordChunkType.FieldSeparator ||
                    chunkType == WordChunkType.FieldBeginMark ||
                    chunkType == WordChunkType.FieldEndMark ||
                    chunkType == WordChunkType.DocumentEnd)
                {
                    break;
                }
                //Holds the character properties of the field.
                m_characterProperties = reader.CharacterProperties;
                if (isFormField &&
                    chunkType == WordChunkType.Image &&
                    reader.CharacterProperties.IsData)
                {
                    formField = reader.GetFormField(fld.Type);
                }
                else
                {
                    if (chunkType != WordChunkType.Image)
                        fieldCode += reader.TextChunk;

                    if (fieldCode.StartsWith(" IF"))
                    {
                        break;
                    }
                }
                if (reader.CurrentBookmark != null)
                    AppendBookmark(reader.CurrentBookmark, reader.IsBookmarkStart);
            }
            while (chunkType != WordChunkType.FieldEndMark);

            return fieldCode;
        }
        /// <summary>
        /// Reads the special field.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fld">The FLD.</param>
        /// <param name="fieldCode">The field code.</param>
        private void ReadSpecialField(WordReaderBase reader, FieldDescriptor fld, string fieldCode)
        {
            switch (m_currField.FieldType)
            {
                case FieldType.FieldLink:
                case FieldType.FieldEmbed:
                    ReadOleObject(reader, m_currField.FieldType);
                    ProcessChunk(reader);
                    break;
                case FieldType.FieldTOC:
                    ReadTOC(reader);
                    ProcessChunk(reader);
                    break;
                case FieldType.FieldMergeField:
                case FieldType.FieldNext:
                    ReadMergeField(reader);
                    break;
                case FieldType.FieldFormula:
                case FieldType.FieldExpression:
                    if (fld != null)
                        ReadExpressionField(fieldCode, fld);
                    break;
            }
        }
        /// <summary>
        /// Inserts the start field.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void InsertStartField(WordReaderBase reader)
        {
            WField field = m_currField;
            WParagraph paragraph = CurrentParagraph;
            m_fieldStack.Push(field);
            AddItem(CurrentField, paragraph);
            CharacterPropertiesConverter.CHPToFormat(reader, m_currField.CharacterFormat);
        }
        /// <summary>
        /// Reads expression or formula field
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        /// <param name="fld">The FLD.</param>
        private void ReadExpressionField(string fieldCode, FieldDescriptor fld)
        {
            if (fld.Type == FieldType.FieldExpression || fld.Type == FieldType.FieldFormula)
            {
                Debug.WriteLine("Field type is Expression or Formula. Type defined by FieldDescriptor");
                m_currField.FieldType = fld.Type;
            }
            if (fld.Type == FieldType.FieldExpression)
            {
                m_currField.m_fieldValue = fieldCode;
                m_currField.m_formattingString = "";
                m_currField.FieldType = FieldType.FieldExpression;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ReadMergeField(WordReaderBase reader)
        {
            WField field = m_currField;
            WParagraph paragraph = CurrentParagraph;
            m_fieldStack.Push(field);
            AddItem(CurrentField, paragraph);

            string fieldValue = string.Empty;
            //Updates the character formattings of the merge field.
            if (m_characterProperties != null
                && reader.ChunkType == WordChunkType.FieldEndMark)
                CharacterPropertiesConverter.CHPToFormat(m_characterProperties, m_currField.CharacterFormat);
            while (reader.ChunkType != WordChunkType.FieldEndMark &&
                   reader.ReadChunk() != WordChunkType.FieldEndMark)
            {
                if (fieldValue == string.Empty || fieldValue == "�")
                    CharacterPropertiesConverter.CHPToFormat(reader, m_currField.CharacterFormat);
                fieldValue += reader.TextChunk;                

                if (reader.ChunkType == WordChunkType.Text && m_currField is WMergeField)
                {
                    WTextRange text = new WTextRange(DocumentEx);
                    text.Text = reader.TextChunk;
                    CharacterPropertiesConverter.CHPToFormat(reader, text.CharacterFormat );
                    (m_currField as WMergeField).TextItems.Add(text);
                }
            }

            m_currField.Text = fieldValue;
            m_fieldStack.Pop();
        }
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="reader"></param>
        private void ReadOleObject(WordReaderBase reader, FieldType type)
        {
            WField field = m_currField;
            WParagraph paragraph = CurrentParagraph;
            m_fieldStack.Push(field);
            WOleObject oleObject = new WOleObject(DocumentEx);
            oleObject.OleStorageName = reader.CharacterProperties.PicLocation.ToString();
            CurrentField.SetOwner(oleObject);
            oleObject.Field = m_currField;

            if (type == FieldType.FieldEmbed)
            {
                oleObject.SetLinkType(OleLinkType.Embed);
            }
            else
            {
                oleObject.SetLinkType(OleLinkType.Link);
            }
            if (reader.m_streamsManager.ObjectPoolStream != null && reader.m_streamsManager.ObjectPoolStream.Length != 0)
                oleObject.ParseObjectPool(reader.m_streamsManager.ObjectPoolStream);
            AddItem(oleObject, paragraph);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadTOC(WordReaderBase reader)
        {
            if (DocumentEx.TOC == null)
            {
                m_fieldStack.Push(m_currField);
                TableOfContent toc = new TableOfContent(DocumentEx, m_currField.m_formattingString);
                DocumentEx.TOC = toc;
                toc.FormattingString = m_currField.m_formattingString;
                CharacterPropertiesConverter.CHPToFormat(reader, toc.TOCField.CharacterFormat);
                AddItem(toc, CurrentParagraph);
            }
            else
            {
                InsertStartField(reader);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldCode"></param>
        private WField ParseFieldCode(string fieldCode)
        {
            WField field = null;
            string sourceFieldCode = fieldCode;
            fieldCode = fieldCode.Trim();
            FieldType fieldType = FieldTypeDefiner.GetFieldType(fieldCode);

            switch (fieldType)
            {
                case FieldType.FieldMergeField:
                    field = new WMergeField(DocumentEx);
                    break;
                case FieldType.FieldSequence:
                    field = new WSeqField(DocumentEx);
                    break;
                //case FieldType.FieldEmbed:
                //  field = new WEmbedField( DocumentEx );
                //  break;
                case FieldType.FieldFormCheckBox:
                    field = new WCheckBox(DocumentEx);
                    break;
                case FieldType.FieldFormTextInput:
                    field = new WTextFormField(DocumentEx);
                    break;
                case FieldType.FieldFormDropDown:
                    field = new WDropDownFormField(DocumentEx);
                    break;
                case FieldType.FieldIf:
                    field = new WIfField(DocumentEx);
                    break;
                case FieldType.FieldOCX:
                    field = new WControlField(DocumentEx);
                    break;
                default:
                    field = new WField(DocumentEx);
                    break;
            }
            field.ParseFieldCode(fieldCode);
            field.FieldType = fieldType;
            field.FieldCode = sourceFieldCode;

            return field;
        }

        #endregion

        #region Implementation / formats
        /// <summary>
        /// Reads list format.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="listFormat"></param>
        protected void ReadListFormat(WordReaderBase reader, WListFormat listFormat)
        {
            if (reader.HasList())
            {
                ListInfo listInfo = reader.ListInfo;
                ListPropertiesConverter.Export(listFormat, reader);
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// </summary>
        /// <param name="charFormat"></param>
        protected void AddUsedFonts(WCharacterFormat charFormat)
        {
            string fontName = charFormat.GetFontName(WCharacterFormat.FontNameAsciiKey);

            FontStyle fontStyle = new FontStyle();
            if (charFormat.HasValue(WCharacterFormat.BoldKey) && charFormat.Bold)
            {
                fontStyle |= FontStyle.Bold;
            }
            if (charFormat.HasValue(WCharacterFormat.ItalicKey) && charFormat.Italic)
            {
                fontStyle |= FontStyle.Italic;
            }
            if (charFormat.HasValue(WCharacterFormat.UnderlineKey) && charFormat.UnderlineStyle != UnderlineStyle.None)
            {
                fontStyle |= FontStyle.Underline;
            }
            if (charFormat.HasValue(WCharacterFormat.StrikeKey) && charFormat.Strikeout)
            {
                fontStyle |= FontStyle.Strikeout;
            }
            Font font = null;
            try
            {
                font = new Font(fontName, 11, fontStyle);
            }
            catch (Exception ex)
            {
                FontFamily fontFamily = new FontFamily(fontName);
                if (fontFamily.IsStyleAvailable(FontStyle.Bold))
                    fontStyle |= FontStyle.Bold;
                if (fontFamily.IsStyleAvailable(FontStyle.Italic))
                    fontStyle |= FontStyle.Italic;
                if (fontFamily.IsStyleAvailable(FontStyle.Underline))
                    fontStyle |= FontStyle.Underline;
                if (fontFamily.IsStyleAvailable(FontStyle.Strikeout))
                    fontStyle |= FontStyle.Strikeout;
                font = new Font(fontName, 11, fontStyle);
            }
            if (!DocumentEx.UsedFontNames.Contains(font))
                DocumentEx.UsedFontNames.Add(font);
        }
#endif
        /// <summary>
        /// Reads the character format.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="charFormat">The character format.</param>
        protected void ReadCharacterFormat(WordReaderBase reader, WCharacterFormat charFormat)
        {
            CharacterPropertiesConverter.CHPToFormat(reader, charFormat);

#if !SILVERLIGHT && !WP
            AddUsedFonts(charFormat);
#endif
        }
        /// <summary>
        /// Reads the paragraph format.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="paragraph">The paragraph.</param>
        protected void ReadParagraphFormat(WordReaderBase reader, IWParagraph paragraph)
        {
            ParagraphPropertiesConverter.Export(reader.ParagraphProperties, paragraph.ParagraphFormat);
            UpdateParagraphStyle(paragraph, reader);
        }
        /// <summary>
        /// Updates the paragraph style.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="reader">The reader.</param>
        protected void UpdateParagraphStyle(IWParagraph paragraph, WordReaderBase reader)
        {
            int styleIndex = reader.CurrentStyleIndex;
            WordStyle style = reader.StyleSheet.GetStyleByIndex(styleIndex);
            //Checks SPRM for style ID
            if(paragraph.ParagraphFormat.Sprms.Contain(WordSprmOptions.sprmPIstd))
            {
                SinglePropertyModifierRecord sprm = paragraph.ParagraphFormat.Sprms[WordSprmOptions.sprmPIstd];
                if (reader.StyleSheet.GetStyleByIndex(sprm.ShortValue) != null)
                {
                    style = reader.StyleSheet.GetStyleByIndex(sprm.ShortValue);
                    styleIndex = sprm.ShortValue;
                }
            }
            IWParagraphStyle dlsStyle = null;
            if (style.IsCharacterStyle)
            {
                dlsStyle = DocumentEx.Styles.FindByName("Normal") as IWParagraphStyle;
            }
            else
            {
                string styleName;
                if(reader.StyleSheet.StyeNames.ContainsKey(styleIndex))
                    styleName = reader.StyleSheet.StyeNames[styleIndex];
                else
                    styleName = style.Name;
                dlsStyle = DocumentEx.Styles.FindByName(styleName, StyleType.ParagraphStyle) as IWParagraphStyle;
            }

            //      if( dlsStyle == null )
            //      {
            //        //throw new InvalidOperationException();   
            //        dlsStyle = DocumentEx.AddStyle( StyleType.ParagraphStyle, styleName ) as IWParagraphStyle;
            //        ParagraphPropertiesConverter.Export( style.ParagraphProperties, ( dlsStyle as IWParagraphStyle).ParagraphFormat );
            //        CharacterPropertiesConverter.CHPToFormat( style.CharacterProperties, ( dlsStyle as IWParagraphStyle ).CharacterFormat );
            //      }
            //if style id not present in the  style sheet then set style id to sprmPIstd to preserve word behaviour
            if (dlsStyle != null)
                (paragraph as WParagraph).ApplyStyle(dlsStyle);
            else
                paragraph.ParagraphFormat.Sprms.SetValue(WordSprmOptions.sprmPIstd, (short)styleIndex);
        }
        /// <summary>
        /// Reads the table row format.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="table">The table.</param>
        private void ReadTableRowFormat(WordReaderBase reader, WTable table)
        {
            TablePropertiesConverter.Export(reader.ParagraphProperties, table.LastRow.RowFormat);
            WTableRow tempRow = table.LastRow;
            ReadCharacterFormat(reader, tempRow.CharacterFormat);
        }
        #endregion

        #region Implementation / helper methods
        /// <summary>
        /// Processes the commented text.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="para">The paragraph.</param>
        protected virtual void ProcessCommText(WordReaderBase reader, WParagraph para)
        { }
        #endregion

        #region Internal declaration
        /// <summary>
        /// 
        /// </summary>
        internal enum PrepareTableState
        {
            NoChange,
            EnterTable,
            LeaveTable
        }
        /// <summary>
        /// 
        /// </summary>
        internal struct PrepareTableInfo
        {
            internal bool InTable;
            internal int Level;
            internal int PrevLevel;
            internal PrepareTableState State;
            /// <summary>
            /// Updates the specified reader.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="prevLevel">The prev level.</param>
            internal PrepareTableInfo(WordReaderBase reader, int prevLevel)
            {
                InTable = reader.HasTableBody;
                PrevLevel = prevLevel;
                Level = InTable ? reader.ParagraphProperties.TablesNestingLevel : 0;

                if (Level > PrevLevel)
                {
                    State = PrepareTableState.EnterTable;
                }
                else if (Level < PrevLevel)
                {
                    State = PrepareTableState.LeaveTable;
                }
                else
                {
                    State = PrepareTableState.NoChange;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal struct ComplexTable
        {
            internal WTable Table;
            internal WTable OneRowTable;
            internal ComplexTable(WTable oneRowTable)
            {
                Table = null;
                OneRowTable = oneRowTable;
            }
            /// <summary>
            /// Sets the null.
            /// </summary>
            internal void SetNull()
            {
                Table = null;
                OneRowTable = null;
            }
            /// <summary>
            /// Appends the one row to table.
            /// </summary>
            internal void AppendOneRowToTable()
            {
                Table = OneRowTable;
                OneRowTable = null;
            }
            /// <summary>
            /// 
            /// </summary>
            internal void CreateOneRowTable(WordDocument docEx)
            {
                OneRowTable = new WTable(docEx);
                OneRowTable.ResetCells(1, 1);
                OneRowTable.TableFormat.Borders.BorderType = BorderStyle.None;
            }
        }
        #endregion
    }
    internal class DocReaderAdapter : DocReaderAdapterBase
    {
        #region Fields
        private AnnotationAdapter m_annAdapter = null;
        private FootnoteAdapter m_ftnAdapter = null;
        private EndnoteAdapter m_endNoteAdapter = null;
        private TextboxAdapter m_txbxAdapter = null;
        private HeaderFooterAdapter m_hfAdapter = null;
        #endregion

        #region Public methods
        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="wordDoc">The word doc.</param>
        public void Read(WordReader reader, WordDocument wordDoc)
        {
            AdapterListIDHolder.Instance.LfoStyleIDtoName.Clear();
            AdapterListIDHolder.Instance.ListStyleIDtoName.Clear();

            Init(wordDoc);
            ReadPassword(reader);
            reader.ReadDocumentHeader(wordDoc);

            //Make sure if document is Write Protected.
            wordDoc.WriteProtected = reader.TablesData.FIBData.fReadOnlyRecommended;
            wordDoc.HasPicture = reader.TablesData.FIBData.fHasPic;
            //Update word version.
            wordDoc.WordVersion = reader.TablesData.FIBData.nFibNew > 0 ? reader.TablesData.FIBData.nFibNew : reader.TablesData.FIBData.fibVersion;
            ReadDOP(reader);
            wordDoc.IsOpening = true;
            ReadStyleSheet(reader);
            wordDoc.FontSubstitutionTable = reader.StyleSheet.FontSubstitutionTable;
            ReadEscher(reader);
            ReadBackground();

            ReadSubDocument(reader, WordSubdocument.Footnote);
            ReadSubDocument(reader, WordSubdocument.Annotation);
            ReadSubDocument(reader, WordSubdocument.Endnote);
            ReadSubDocument(reader, WordSubdocument.TextBox);

            do
            {
                IWSection sec = wordDoc.AddSection();
                ReadTextBody(reader, (WTextBody)sec.Body);
                ReadSectionFormat(reader, sec);
            }
            while (reader.ChunkType != WordChunkType.DocumentEnd);

            ReadSubDocument(reader, WordSubdocument.HeaderFooter);
            ReadDocumentProperties(reader);
            CheckWatermark( wordDoc.Sections[ 0 ] as WSection );

            if (DocumentEx.HasListStyle())
                ParseListPicture();

            reader.ReadDocumentEnd();
            wordDoc.FFNStringTable = reader.m_docInfo.TablesData.FFNStringTable;
            wordDoc.IsOpening = false;
        }
        #endregion

        #region Implementation

        private void ReadPassword(WordReader reader)
        {
            if (DocumentEx.Password != null)
            {
                reader.NeedPassword +=
                  new NeedPasswordEventHandler(DocumentEx.GetPassword);
            }
        }
        /// <summary>
        /// Reads the style sheet.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadStyleSheet(WordReader reader)
        {
            WordStyleSheet styleSheet = reader.StyleSheet;
            int count = styleSheet.StylesCount;
            Dictionary<string, int> styles = new Dictionary<string, int>();
            for (int i = 0; i < count; i++)
            {
                WordStyle wordStyle = styleSheet.GetStyleByIndex(i);
                //case 13: Reserved style at the fixed index 13 in the stylesheet
                if (i == 13)
                {
                    styleSheet.IsFixedIndex13HasStyle = true;
                    styleSheet.FixedIndex13StyleName = wordStyle.Name;
                }
                //case 14: Reserved style at the fixed index 14 in the stylesheet
                if (i == 14)
                {
                    styleSheet.IsFixedIndex14HasStyle = true;
                    styleSheet.FixedIndex14StyleName = wordStyle.Name;
                }

                if (wordStyle.Name != null)
                {
                    if (wordStyle.ID == 0 && wordStyle.Name != "Normal")
                    {
                        wordStyle.Name = "Normal";
                    }
                    string styleName = wordStyle.Name;
                    if (styles.ContainsKey(wordStyle.Name))
                    {
                        string name = wordStyle.Name;
                        styleName = wordStyle.Name + "_" + styles[wordStyle.Name].ToString();
                        styles[name] += 1;
                    }
                    else
                        styles.Add(wordStyle.Name, 0);
                    styleSheet.StyeNames.Add(i, styleName); 
                    if (!wordStyle.IsCharacterStyle)
                    {
                        IStyle dlsStyle = DocumentEx.AddStyle(StyleType.ParagraphStyle, styleName);
                        WParagraphStyle paragraphStyle = dlsStyle as WParagraphStyle;
                        paragraphStyle.StyleId = wordStyle.ID;
                        paragraphStyle.IsPrimaryStyle = wordStyle.IsPrimary;
                        paragraphStyle.IsSemiHidden = wordStyle.IsSemiHidden;
                        paragraphStyle.UnhideWhenUsed = wordStyle.UnhideWhenUsed;
                        (dlsStyle as Style).TypeCode = wordStyle.TypeCode;
                        if (wordStyle.TypeCode == WordStyleType.TableStyle && wordStyle.TableStyleData != null)
                        {
                            Style style = dlsStyle as Style;
                            style.TableStyleData = new byte[wordStyle.TableStyleData.Length];
                            Buffer.BlockCopy(wordStyle.TableStyleData, 0, style.TableStyleData, 0, wordStyle.TableStyleData.Length);
                        }
                        ParagraphPropertiesConverter.Export(wordStyle.ParagraphProperties, paragraphStyle.ParagraphFormat);
                        CharacterPropertiesConverter.CHPToFormat(wordStyle.CharacterProperties, paragraphStyle.CharacterFormat);

#if !SILVERLIGHT && !WP
                        AddUsedFonts(paragraphStyle.CharacterFormat);
#endif
                        if (reader.HasList())
                        {
                            int formatIndex = wordStyle.ParagraphProperties.ListFormatIndex;
                            int levelIndex = wordStyle.ParagraphProperties.ListLevelIndex;
                            ListPropertiesConverter.Export(formatIndex, levelIndex, paragraphStyle.ListFormat, reader);
                            if (paragraphStyle.ListFormat.CurrentListLevel != null)
                                paragraphStyle.ListFormat.CurrentListLevel.ParaStyleName = paragraphStyle.Name.Replace(" ", string.Empty);
                        }
                    }
                    else
                    {
                        CharacterStyle characterStyle = (CharacterStyle)DocumentEx.AddStyle(StyleType.CharacterStyle, styleName);
                        characterStyle.StyleId = wordStyle.ID;
                        characterStyle.IsPrimaryStyle = wordStyle.IsPrimary;
                        characterStyle.IsSemiHidden = wordStyle.IsSemiHidden;
                        characterStyle.UnhideWhenUsed = wordStyle.UnhideWhenUsed;
                        (characterStyle as Style).TypeCode = wordStyle.TypeCode;
                        if (wordStyle.TypeCode == WordStyleType.TableStyle && wordStyle.TableStyleData != null)
                        {
                            Style style = characterStyle as Style;
                            style.TableStyleData = new byte[wordStyle.TableStyleData.Length];
                            Buffer.BlockCopy(wordStyle.TableStyleData, 0, style.TableStyleData, 0, wordStyle.TableStyleData.Length);
                        }
                        CharacterPropertiesConverter.CHPToFormat(wordStyle.CharacterProperties, characterStyle.CharacterFormat);

#if !SILVERLIGHT && !WP
                        AddUsedFonts(characterStyle.CharacterFormat);
#endif
                    }
                }
            }

            for (int index = 0, cnt = DocumentEx.Styles.Count; index < cnt; index++)
            {
                Style style = DocumentEx.Styles[index] as Style;

                if (string.IsNullOrEmpty(style.Name))
                    continue;

                int styleIndex = styleSheet.StyleNameToIndex(style.Name,
                  style.StyleType == StyleType.CharacterStyle);

                // Apply base styles
                int baseStyleIndex = styleSheet.GetStyleByIndex(styleIndex).BaseStyleIndex;
                if (baseStyleIndex != 4095)
                {
                    if (styleSheet.StyeNames.ContainsKey(baseStyleIndex))
                    {
                        string baseStyle = styleSheet.StyeNames[baseStyleIndex];
                        if (baseStyle != null) style.ApplyBaseStyle(baseStyle);
                    }
                }
                else if (style.BaseStyle != null && style is WParagraphStyle)
                    style.RemoveBaseStyle();

                // Set next style name 
                int nextStyleIndex = styleSheet.GetStyleByIndex(styleIndex).NextStyleIndex;
                if (nextStyleIndex != 4095)
                {
                    WordStyle nextStyle = styleSheet.GetStyleByIndex(nextStyleIndex);
                    if (nextStyle != null) style.NextStyle = nextStyle.Name;
                }

                // Set link style name
                int linkStyleIndex = styleSheet.GetStyleByIndex(styleIndex).LinkStyleIndex;
                if (linkStyleIndex != 4095 && linkStyleIndex != 0)
                {
                    WordStyle linkStyle = styleSheet.GetStyleByIndex(linkStyleIndex);
                    if (linkStyle != null) style.LinkStyle = linkStyle.Name;
                }
            }
            //initialize the reserved style details in the styles collection.
            DocumentEx.Styles.FixedIndex13HasStyle = styleSheet.IsFixedIndex13HasStyle;
            DocumentEx.Styles.FixedIndex14HasStyle = styleSheet.IsFixedIndex14HasStyle;
            DocumentEx.Styles.FixedIndex13StyleName = styleSheet.FixedIndex13StyleName;
            DocumentEx.Styles.FixedIndex14StyleName = styleSheet.FixedIndex14StyleName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadEscher(WordReader reader)
        {
            if (reader.Escher != null)
            {
                DocumentEx.Escher = reader.Escher;
            }
        }
        /// <summary>
        /// Reads the background.
        /// </summary>
        private void ReadBackground()
        {
            DocumentEx.ReadBackground();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="wsType"></param>
        private void ReadSubDocument(WordReader reader, WordSubdocument wsType)
        {
            if (!SubDocumentExists(reader, wsType))
                return;

            SubDocumentAdapter adapter = null;

            switch (wsType)
            {
                case WordSubdocument.HeaderFooter:
                    adapter = m_hfAdapter = new HeaderFooterAdapter();
                    break;
                case WordSubdocument.Footnote:
                    adapter = m_ftnAdapter = new FootnoteAdapter();
                    break;
                case WordSubdocument.Endnote:
                    adapter = m_endNoteAdapter = new EndnoteAdapter();
                    break;
                case WordSubdocument.Annotation:
                    adapter = m_annAdapter = new AnnotationAdapter();
                    break;
                case WordSubdocument.TextBox:
                    adapter = m_txbxAdapter = new TextboxAdapter();
                    break;
                default:
                    break;
            }

            if (adapter != null)
            {
                adapter.ReadSubDocBody(reader, DocumentEx);
            }
        }
        /// <summary>
        /// Reads the section format.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="sec">The sec.</param>
        private void ReadSectionFormat(WordReader reader, IWSection sec)
        {
            if (reader.ChunkType != WordChunkType.DocumentEnd)
            {
                ReadListFormat(reader, CurrentParagraph.ListFormat);
                ReadCharacterFormat(reader, CurrentParagraph.BreakCharacterFormat);
                ReadParagraphFormat(reader, CurrentParagraph);
                UpdateParagraphStyle(CurrentParagraph, reader);
            }

            SectionPropertiesConverter.Export(reader.SectionProperties, sec as WSection, true);
            m_currParagraph = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadDocumentProperties(WordReader reader)
        {
            ReadBuiltInDocumentProperties(reader);
            DocumentEx.m_customProp = reader.CustomDocumentProperties;

            if (reader.MacrosStream != null)
            {
#if WINRT
                DocumentEx.MacrosData = reader.MacrosStream.ToArray();
#else
                DocumentEx.MacrosData = reader.MacrosStream.GetBuffer();
#endif
            }
            if (reader.Variables != null)
            {
                DocumentEx.Variables.UpdateVariables(reader.Variables);
            }
            if (reader.MacroCommands != null)
            {
                DocumentEx.MacroCommands = reader.MacroCommands;
            }
            if (reader.AssociatedStrings != null)
            {
                DocumentEx.AssociatedStrings.Parse(reader.AssociatedStrings);
            }
            if (reader.GrammarSpellingData != null)
            {
                DocumentEx.GrammarSpellingData = reader.GrammarSpellingData;
            }
            if (reader.DOP != null)
            {
                DocumentEx.DifferentOddAndEvenPages = reader.DOP.OddAndEvenPagesHeaderFooter;
                DocumentEx.DefaultTabWidth = (float)reader.DOP.DefaultTabWidth / DLSConstants.TwipsInOnePoint;
            }

            // Check document view
            if (reader.DOP.ViewType != 1)
            {
                DocumentEx.ViewSetup.DocumentViewType = (DocumentViewType)reader.DOP.ViewType;
            }
            if (reader.DOP.ZoomType != 0)
            {
                DocumentEx.ViewSetup.ZoomType = (ZoomType)reader.DOP.ZoomType;
            }
            if (reader.DOP.ZoomPercent != 0 && reader.DOP.ZoomPercent != ViewSetup.DEF_ZOOMING)
            {
                DocumentEx.ViewSetup.SetZoomPercent(reader.DOP.ZoomPercent);
            }

            DocumentEx.StandardAsciiFont = reader.StandardAsciiFont;
            DocumentEx.StandardFarEastFont = reader.StandardFarEastFont;
            DocumentEx.StandardNonFarEastFont = reader.StandardNonFarEastFont;
            DocumentEx.StandardBidiFont = reader.StandardBidiFont;
            DocumentEx.Properties.SetVersion(reader.Version);
        }
        /// <summary>
        /// Reads Built in Document Properties
        /// </summary>
        /// <param name="reader">reader</param>
        private void ReadBuiltInDocumentProperties(WordReader reader)
        {
            //Application name
            if (reader.BuiltinDocumentProperties.ApplicationName != null)
                DocumentEx.m_builtinProp.ApplicationName = reader.BuiltinDocumentProperties.ApplicationName.ToString();
            //Author
            if (reader.BuiltinDocumentProperties.Author != null)
                DocumentEx.m_builtinProp.Author = reader.BuiltinDocumentProperties.Author.ToString();
            //Category
            if (reader.BuiltinDocumentProperties.Category != null)
                DocumentEx.m_builtinProp.Category = reader.BuiltinDocumentProperties.Category.ToString();
            //Character count
            if (reader.BuiltinDocumentProperties.CharCount != null)
                DocumentEx.m_builtinProp.CharCount = reader.BuiltinDocumentProperties.CharCount;
            //Comments
            if (reader.BuiltinDocumentProperties.Comments != null)
                DocumentEx.m_builtinProp.Comments = reader.BuiltinDocumentProperties.Comments.ToString();
            //Company   
            if (reader.BuiltinDocumentProperties.Company != null)
                DocumentEx.m_builtinProp.Company = reader.BuiltinDocumentProperties.Company.ToString();
            //Created date
            if (reader.BuiltinDocumentProperties.CreateDate != null && (reader.BuiltinDocumentProperties.CreateDate.CompareTo(new DateTime(1900, 12, 31)) > 0))
                DocumentEx.m_builtinProp.CreateDate = (DateTime)reader.BuiltinDocumentProperties.CreateDate;
            //Last printed date
            if (reader.BuiltinDocumentProperties.LastPrinted != null && (reader.BuiltinDocumentProperties.LastPrinted.CompareTo(new DateTime(1900, 12, 31)) > 0))
                DocumentEx.m_builtinProp.LastPrinted = (DateTime)reader.BuiltinDocumentProperties.LastPrinted;
            //Last saved date
            if (reader.BuiltinDocumentProperties.LastSaveDate != null && (reader.BuiltinDocumentProperties.LastSaveDate.CompareTo(new DateTime(1900, 12, 31)) > 0))
                DocumentEx.m_builtinProp.LastSaveDate = (DateTime)reader.BuiltinDocumentProperties.LastSaveDate;
            //Document security
            if (reader.BuiltinDocumentProperties.DocSecurity != null)
                DocumentEx.m_builtinProp.DocSecurity = reader.BuiltinDocumentProperties.DocSecurity;
            //Keywords
            if (reader.BuiltinDocumentProperties.Keywords != null)
                DocumentEx.m_builtinProp.Keywords = reader.BuiltinDocumentProperties.Keywords.ToString();
            //Last Author
            if (reader.BuiltinDocumentProperties.LastAuthor != null)
                DocumentEx.m_builtinProp.LastAuthor = reader.BuiltinDocumentProperties.LastAuthor.ToString();
            //Manager
            if (reader.BuiltinDocumentProperties.Manager != null)
                DocumentEx.m_builtinProp.Manager = reader.BuiltinDocumentProperties.Manager.ToString();
            //Paragraph count
            if (reader.BuiltinDocumentProperties.ParagraphCount != null)
                DocumentEx.m_builtinProp.ParagraphCount = reader.BuiltinDocumentProperties.ParagraphCount;
            //Revision number
            if (reader.BuiltinDocumentProperties.RevisionNumber != null)
                DocumentEx.m_builtinProp.RevisionNumber = reader.BuiltinDocumentProperties.RevisionNumber.ToString();
            //Subject
            if (reader.BuiltinDocumentProperties.Subject != null)
                DocumentEx.m_builtinProp.Subject = reader.BuiltinDocumentProperties.Subject.ToString();
            //Template
            if (reader.BuiltinDocumentProperties.Template != null)
                DocumentEx.m_builtinProp.Template = reader.BuiltinDocumentProperties.Template.ToString();
            //Thumbnail
            if (reader.BuiltinDocumentProperties.Thumbnail != null)
                DocumentEx.m_builtinProp.Thumbnail = reader.BuiltinDocumentProperties.Thumbnail;
            //Title
            if (reader.BuiltinDocumentProperties.Title != null)
                DocumentEx.m_builtinProp.Title = reader.BuiltinDocumentProperties.Title.ToString();
            //Total editing time
            if (reader.BuiltinDocumentProperties.TotalEditingTime != null)
                DocumentEx.m_builtinProp.TotalEditingTime = (TimeSpan)reader.BuiltinDocumentProperties.TotalEditingTime;
            //Word count
            if (reader.BuiltinDocumentProperties.WordCount != null)
                DocumentEx.m_builtinProp.WordCount = reader.BuiltinDocumentProperties.WordCount;
            //Bytes count
            if (reader.BuiltinDocumentProperties.BytesCount != null)
                DocumentEx.m_builtinProp.BytesCount = reader.BuiltinDocumentProperties.BytesCount;
            //Hidden count
            if (reader.BuiltinDocumentProperties.HiddenCount != null)
                DocumentEx.m_builtinProp.HiddenCount = reader.BuiltinDocumentProperties.HiddenCount;
            //Lines count
            if (reader.BuiltinDocumentProperties.LinesCount != null)
                DocumentEx.m_builtinProp.LinesCount = reader.BuiltinDocumentProperties.LinesCount;
            //Note count
            if (reader.BuiltinDocumentProperties.NoteCount != null)
                DocumentEx.m_builtinProp.NoteCount = reader.BuiltinDocumentProperties.NoteCount;
            //Page count
            if (reader.BuiltinDocumentProperties.PageCount != null)
                DocumentEx.m_builtinProp.PageCount = reader.BuiltinDocumentProperties.PageCount;
            //Slide count
            if (reader.BuiltinDocumentProperties.SlideCount != null)
                DocumentEx.m_builtinProp.SlideCount = reader.BuiltinDocumentProperties.SlideCount;
        }
        /// <summary>
        /// Reads DOPDescriptor data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadDOP(WordReader reader)
        {
            if (reader.DOP != null)
            {
                DocumentEx.DOP = reader.DOP;
            }
        }
        /// <summary>
        /// Defines if specified subdocument exists in the documnt.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="wsType">Type of the ws.</param>
        /// <returns></returns>
        private bool SubDocumentExists(WordReader reader, WordSubdocument wsType)
        {
            return reader.TablesData.HasSubdocument(wsType);
        }
        #endregion

        #region Implementation / overrides
        /// <summary>
        /// Ends the of text body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="chunkType">Type of the chunk.</param>
        /// <returns></returns>
        protected override bool EndOfTextBody(WordReaderBase reader, WordChunkType chunkType)
        {
            return (chunkType == WordChunkType.SectionEnd) || (chunkType == WordChunkType.DocumentEnd);
        }
        /// <summary>
        /// Reads the annotation.
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadAnnotation(WordReaderBase reader)
        {
            if (m_annAdapter != null)
            {
                WComment comm = m_annAdapter.GetNextComment();

                if (comm != null)
                {
                    if (comm.Format.TagBkmk == -1)
                        comm.Format.UpdateTagBkmk();

                    UpdateCommentMarks(comm);
                    CurrentParagraph.Items.Add(comm);
                }
            }
        }
        /// <summary>
        /// Reads the footnote.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void ReadFootnote(WordReaderBase reader)
        {
            WordReader wReader = reader as WordReader;
            bool isMultipleFootnoteText = true;
            string footNoteMarker = string.Empty;
            while (wReader != null && isMultipleFootnoteText)
            {
                WFootnote footnote = null;
                footNoteMarker = reader.TextChunk;
                if (wReader.IsFootnote)
                {
                    footnote = m_ftnAdapter.GetNextFootEndNote();
                }
                else if (wReader.IsEndnote)
                {
                    footnote = m_endNoteAdapter.GetNextFootEndNote();
                }
                //Checks for Muliple footnote / endnote marker
                isMultipleFootnoteText = IsMultipleFootNoteEndNoteMarker(ref footNoteMarker, wReader, footnote);
                if (footnote != null)
                {
                    CurrentParagraph.Items.Add(footnote);

                    if (reader.ChunkType != WordChunkType.Footnote)
                    {
                        footnote.CustomMarker = footNoteMarker;
                        footnote.IsAutoNumbered = false;
                    }
                    if (reader.ChunkType == WordChunkType.Symbol)
                    {
                        SymbolDescriptor symbolDescriptor = reader.CharacterProperties.Symbol;
                        footnote.SymbolCode = symbolDescriptor.CharCode;
                        footnote.SymbolFontName = (string)reader.StyleSheet.FontNamesList[symbolDescriptor.FontCode];
                    }

                    ReadCharacterFormat(reader, footnote.MarkerCharacterFormat);
                    ReadParagraphFormat(reader, CurrentParagraph);
                }
            }
        }
        /// <summary>
        /// Determines whether the footnote / endnote marker refers multiple footnotes / end notes
        /// </summary>
        /// <param name="footNoteMarker"></param>
        /// <param name="reader"></param>
        /// <param name="footnote"></param>
        /// <returns></returns>
        private bool IsMultipleFootNoteEndNoteMarker(ref string footNoteMarker, WordReader wReader, WFootnote footnote)
        {
            string textChunk = wReader.TextChunk;
            if (footnote == null)
                return false;
            if (footnote.TextBody.Paragraphs.Count > 0 && !footnote.TextBody.Paragraphs[0].Text.StartsWith(textChunk))
            {
                int i = 0;
                footNoteMarker = textChunk[0].ToString();
                while (++i < textChunk.Length &&
                    footnote.TextBody.Paragraphs[0].Text.StartsWith(footNoteMarker + textChunk[i].ToString()))
                    footNoteMarker += textChunk[i].ToString();

                textChunk = textChunk.Replace(footNoteMarker, string.Empty);
                WParagraph para = null;

                if (wReader.IsFootnote && m_ftnAdapter.m_currFootEndnoteIndex < m_ftnAdapter.m_footEndNotes.Count)
                    para = m_ftnAdapter.m_footEndNotes[m_ftnAdapter.m_currFootEndnoteIndex].TextBody.Paragraphs[0];
                else if (wReader.IsEndnote && m_endNoteAdapter.m_currFootEndnoteIndex < m_endNoteAdapter.m_footEndNotes.Count)
                    para = m_endNoteAdapter.m_footEndNotes[m_endNoteAdapter.m_currFootEndnoteIndex].TextBody.Paragraphs[0];

                if (para !=null && para.Text.StartsWith(textChunk))
                {
                    wReader.TextChunk = textChunk;
                    return true;
                }
                else
                    footNoteMarker = wReader.TextChunk;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadDocumentEnd(WordReaderBase reader)
        {
        }
        /// <summary>
        /// Reads shape objects
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fspa">The File Shape Address.</param>
        protected override void ReadTextBox(WordReaderBase reader, FileShapeAddress fspa)
        {
            bool skipPositionOrigins = (reader.TablesData.FIBData.nFibNew > 0xC1);
            WTextBox textbox = m_txbxAdapter.ReadTextBoxShape(fspa, skipPositionOrigins);
            if (textbox != null)
            {
                CharacterPropertiesConverter.CHPToFormat(reader, textbox.CharacterFormat);
                CurrentParagraph.Items.Add(textbox);
            }
        }
        /// <summary>
        /// Checks the text embed.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        protected override void CheckTextEmbed(ShapeBase shape, WPicture picture)
        {
            if (m_txbxAdapter == null)
                return;

            WTextBox textBox = m_txbxAdapter.GetAutoShapeTextBox(shape.ShapeProps.Spid);

            if (textBox == null)
                return;
            picture.EmbedBody = textBox.TextBoxBody;
        }
        /// <summary>
        /// Reads text body for autoshape.
        /// </summary>
        /// <param name="shapeId"></param>
        /// <param name="shapeObj"></param>
        protected override void ReadAutoShapeTextBox(int shapeId, ShapeObject shapeObj)
        {
            if (m_txbxAdapter == null)
                return;

            WTextBox autoShapeTextBox = m_txbxAdapter.GetAutoShapeTextBox(shapeId);
            if (autoShapeTextBox != null)
            {
                shapeObj.IsHeaderAutoShape = false;
                shapeObj.AutoShapeTextCollection.Add(autoShapeTextBox);
            }
        }
        /// <summary>
        /// Processes the commented text.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="para">The paragraph.</param>
        protected override void ProcessCommText(WordReaderBase reader, WParagraph para)
        {
            if (!(reader is WordReader) || m_annAdapter == null || para == null)
                return;

            if (para.Items.Count == 0)
                return;

            if (m_annAdapter.Comments == null || m_annAdapter.Comments.Count == 0)
                return;

            // ToDo: implement support for embedded comments
            WComment comment = m_annAdapter.CurrentComment;
            if (comment != null)
            {
                if (reader.StartTextPos >= comment.Format.StartTextPos &&
                  reader.EndTextPos <= comment.Format.Position)
                {
                    // Example: |S|..aaabbb..|E|
                    comment.CommentedItems.Add(para.LastItem);
                }
                else if (para.LastItem is WTextRange && comment.Format.StartTextPos <= reader.EndTextPos)
                {
                    if (reader.StartTextPos < comment.Format.StartTextPos &&
                      reader.EndTextPos <= comment.Format.Position)
                    {
                        // Example: a|S|aabbb
                        SplitCommText(para, reader.StartTextPos, comment.Format.StartTextPos);
                        comment.CommentedItems.Add(para.LastItem);
                    }
                    else if (reader.StartTextPos > comment.Format.StartTextPos &&
                      reader.EndTextPos > comment.Format.Position &&
                      reader.StartTextPos < comment.Format.Position)
                    {
                        // Example: aaabb|E|b
                        SplitCommText(para, reader.StartTextPos, comment.Format.Position);
                        comment.CommentedItems.Add(para.LastItem.PreviousSibling as ParagraphItem);
                    }
                    else if (reader.StartTextPos < comment.Format.StartTextPos &&
                      reader.EndTextPos > comment.Format.Position)
                    {
                        // Example: a|S|aabb|E|b
                        SplitCommText(para, reader.StartTextPos, comment.Format.StartTextPos);
                        SplitCommText(para, reader.StartTextPos, comment.Format.Position);
                        comment.CommentedItems.Add(para.LastItem.PreviousSibling as ParagraphItem);
                    }
                }
            }
        }
        #endregion

        #region Implementation / helper methods
        /// <summary>
        /// Splits the comm text.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="startTextPos">The start text pos.</param>
        /// <param name="splitPos">The split pos.</param>
        private void SplitCommText(WParagraph para, int startTextPos, int splitPos)
        {
            if (splitPos > startTextPos)
            {
                WTextRange textRange = para.LastItem as WTextRange;
                string text = textRange.Text;
                int textRangeLen = splitPos - startTextPos;
                if (textRangeLen > text.Length)
                    return;
                textRange.Text = text.Substring(0, textRangeLen);
                string newText = text.Substring(textRangeLen, text.Length - textRangeLen);
                IWTextRange newRange = para.AppendText(newText);
                newRange.ApplyCharacterFormat(textRange.CharacterFormat);
            }
        }
        /// <summary>
        /// Updates the comment marks.
        /// </summary>
        /// <param name="comment">The comment.</param>
        private void UpdateCommentMarks(WComment comment)
        {
            int commItemsCnt = comment.CommentedItems.Count;
            if (comment.CommentedItems.Count == 0)
                return;

            WCommentMark startMark = new WCommentMark(DocumentEx, comment.Format.TagBkmk);
            WCommentMark endMark = new WCommentMark(DocumentEx, comment.Format.TagBkmk, CommentMarkType.CommentEnd);

            // Insert start mark
            ParagraphItem firstItem = comment.CommentedItems[0];
            if (firstItem.PreviousSibling == null)
            {
                firstItem.OwnerParagraph.Items.Insert(0, startMark);
            }
            else
            {
                int firstItemIndex = firstItem.GetIndexInOwnerCollection();
                firstItem.OwnerParagraph.Items.Insert(firstItemIndex, startMark);
            }
            // Insert end mark.      
            ParagraphItem lastItem = comment.CommentedItems[commItemsCnt - 1];
            if (lastItem.NextSibling == null)
            {
                lastItem.OwnerParagraph.Items.Add(endMark);
            }
            else
            {
                int lastItemIndex = lastItem.GetIndexInOwnerCollection();
                firstItem.OwnerParagraph.Items.Insert(lastItemIndex + 1, endMark);
            }

        }
        /// <summary>
        /// Parses the list picture.
        /// </summary>
        private void ParseListPicture()
        {
            foreach (ListStyle style in DocumentEx.ListStyles)
            {
                for (int i = 0, count = style.Levels.Count; i < count; i++)
                {
                    WListLevel listLevel = style.Levels[i];
                    int index = listLevel.PicIndex;
                    if (index >= 0 && index != int.MaxValue)
                    {
                        if (index <= ListPictures.Count - 1)
                        {
                            WPicture pic = ListPictures[index];
                            listLevel.PicBullet = pic;
                        }
#if !SILVERLIGHT && !WP
                        else
                        {
                            WPicture pic = new WPicture(DocumentEx);
                            Bitmap bmp = new Bitmap(3, 3);
                            pic.LoadImage(bmp as Image);
                            listLevel.PicBullet = pic;
                            listLevel.IsEmptyPicture = true;
                        }
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Checks the watermark.
        /// </summary>
        /// <param name="section">The section.</param>
        private void CheckWatermark(WSection section)
        {
            if (section.Document.Watermark.Type != WatermarkType.NoWatermark)
            {
                if (!section.HeadersFooters.OddHeader.WriteWatermark &&
                    !section.HeadersFooters.FirstPageHeader.WriteWatermark)
                {
                    section.Document.InsertWatermark(WatermarkType.NoWatermark);
                }
            }
        }
        #endregion

        #region Internal declaration
        /// <summary>
        /// 
        /// </summary>
        internal abstract class SubDocumentAdapter : DocReaderAdapterBase
        {
            /// <summary>
            /// Reads subdocument.
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="documentEx"></param>
            internal void ReadSubDocBody(WordReader reader, WordDocument documentEx)
            {
                Init(documentEx);
                Read(reader);
                reader.UnfreezeStreamPos();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            internal abstract void Read(WordReader reader);
        }
        /// <summary>
        /// 
        /// </summary>
        internal class HeaderFooterAdapter : SubDocumentAdapter
        {
            #region Fields
            /// <summary>
            /// Current type of header/footer.
            /// </summary>
            private int m_currentHFType = 0;
            /// <summary>
            /// 
            /// </summary>
            private bool m_itemEnd;
            /// <summary>
            /// Header/footer textbox adapter
            /// </summary>
            private HFTextboxAdapter m_hfTxbxAdapter = null;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the text box adapter.
            /// </summary>
            /// <value>The text box adapter.</value>
            internal HFTextboxAdapter TextBoxAdapter
            {
                get
                {
                    return m_hfTxbxAdapter;
                }
            }
            #endregion

            #region Internal methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            internal override void Read(WordReader reader)
            {
                WordHeaderFooterReader hfReader = reader.GetSubdocumentReader(WordSubdocument.HeaderFooter) as WordHeaderFooterReader;
                hfReader.Bookmarks = reader.Bookmarks;

                if (reader.TablesData.HasSubdocument(WordSubdocument.HeaderTextBox))
                {
                    this.ReadSubdocument(reader, WordSubdocument.HeaderTextBox);
                }

                int count = DocumentEx.Sections.Count;
                m_finalize = false;
                WTextBody curTextBody = null;
                hfReader.MoveToSection(1);
                //Reads the footnote and endnote separators.
                for (int j = 0; j < 6; j++)
                {
                    curTextBody = new WTextBody(DocumentEx, null);
                    ReadTextBody(hfReader, curTextBody);
                    RemoveLastParagraph(curTextBody);
                    if (curTextBody.ChildEntities.Count > 0)
                        SetSeparatorBody(curTextBody, j);
                }
                for (int i = 0; i < count; i++)
                {
                    hfReader.MoveToSection(i + 1);
                    WSection curSection = DocumentEx.Sections[i];
                    //Reads the header footer contents.
                    m_itemEnd = false;
                    hfReader.MoveToItem(6);
                    hfReader.HeaderType = (HeaderType)0;
                    m_currentHFType = 0;
                    while (!m_itemEnd)
                    {
                        curTextBody = curSection.HeadersFooters[m_currentHFType] as WTextBody;
                        ReadTextBody(hfReader, curTextBody);
                    }

                    m_itemEnd = false;
                    m_currentHFType = 0;
                    RemoveHFLastParagraphs(curSection);
                }
            }
            /// <summary>
            /// Sets the separator body.
            /// </summary>
            /// <param name="textBody">The text body.</param>
            /// <param name="index">The index.</param>
            /// <returns>WTextBody.</returns>
            private void SetSeparatorBody(WTextBody textBody, int index)
            {
                switch (index)
                {
                    case 0:
                        DocumentEx.Footnotes.Separator = textBody;
                        break;
                    case 1:
                        DocumentEx.Footnotes.ContinuationSeparator = textBody;
                        break;
                    case 2:
                        DocumentEx.Footnotes.ContinuationNotice = textBody;
                        break;
                    case 3:
                        DocumentEx.Endnotes.Separator = textBody;
                        break;
                    case 4:
                        DocumentEx.Endnotes.ContinuationSeparator = textBody;
                        break;
                    case 5:
                        DocumentEx.Endnotes.ContinuationNotice = textBody;
                        break;
                }
            }
            #endregion

            #region Implementation / overrides
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="chunkType"></param>
            /// <returns></returns>
            protected override bool EndOfTextBody(WordReaderBase reader, WordChunkType chunkType)
            {
                m_currentHFType += (chunkType == WordChunkType.EndOfSubdocText && m_currentHFType < 5) ? 1 : 0;
                m_itemEnd = (chunkType == WordChunkType.DocumentEnd) ? true : false;
                return chunkType == WordChunkType.DocumentEnd || chunkType == WordChunkType.EndOfSubdocText;
            }
            /// <summary>
            /// Read textbox.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="fspa">The file shape address structure</param>
            protected override void ReadTextBox(WordReaderBase reader, FileShapeAddress fspa)
            {
                bool skipPositionOrigins = (reader.TablesData.FIBData.nFibNew > 0xC1);
                WTextBox textbox = m_hfTxbxAdapter.ReadTextBoxShape(fspa, skipPositionOrigins);
                if (textbox != null)
                {
                    CharacterPropertiesConverter.CHPToFormat(reader, textbox.CharacterFormat);
                    CurrentParagraph.Items.Add(textbox);
                }
            }
            /// <summary>
            /// Read
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            protected override bool ReadWatermark(WordReaderBase reader)
            {
                return reader.ReadWatermark(DocumentEx);
            }
            /// <summary>
            /// Reads text body for autoshape.
            /// </summary>
            /// <param name="shapeId"></param>
            /// <param name="shapeObj"></param>
            protected override void ReadAutoShapeTextBox(int shapeId, ShapeObject shapeObj)
            {
                if (m_hfTxbxAdapter == null)
                    return;

                WTextBox autoShapeTextBox = m_hfTxbxAdapter.GetAutoShapeTextBox(shapeId);
                if (autoShapeTextBox != null)
                {
                    shapeObj.IsHeaderAutoShape = true;
                    shapeObj.AutoShapeTextCollection.Add(autoShapeTextBox);
                }
            }
            /// <summary>
            /// Checks the text embed.
            /// </summary>
            /// <param name="shape">The shape.</param>
            /// <returns></returns>
            protected override void CheckTextEmbed(ShapeBase shape, WPicture picture)
            {
                if (m_hfTxbxAdapter == null)
                    return;

                WTextBox textBox = m_hfTxbxAdapter.GetAutoShapeTextBox(shape.ShapeProps.Spid);

                if (textBox == null)
                    return;
                picture.EmbedBody = textBox.TextBoxBody;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="wsType"></param>
            private void ReadSubdocument(WordReader reader, WordSubdocument wsType)
            {
                SubDocumentAdapter adapter = null;

                switch (wsType)
                {
                    case WordSubdocument.HeaderTextBox:
                        adapter = m_hfTxbxAdapter = new HFTextboxAdapter();
                        break;
                    default:
                        break;
                }

                if (adapter != null)
                {
                    adapter.ReadSubDocBody(reader, DocumentEx);
                }
            }
            /// <summary>
            /// Removes the last paragraphs in headers / footers.
            /// </summary>
            /// <param name="section">The section.</param>
            private void RemoveHFLastParagraphs(IWSection section)
            {
                for (int i = 0; i < 6; i++)
                {
                    BodyItemCollection bodyItems = section.HeadersFooters[i].Items;
                    IWParagraph lastPara = bodyItems.LastItem as IWParagraph;

                    if (lastPara != null && lastPara.Items.Count == 0)
                    {
                        bodyItems.Remove(lastPara);
                    }
                }
            }
            /// <summary>
            /// Removes Last paragraph in separator stories
            /// </summary>
            /// <param name="textBody"></param>
            private void RemoveLastParagraph(WTextBody textBody)
            {
                IWParagraph lastPara = textBody.Items.LastItem as IWParagraph;
                if (lastPara != null && lastPara.Items.Count == 0)
                {
                    textBody.Items.Remove(lastPara);
                }
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class AnnotationAdapter : SubDocumentAdapter
        {
            #region Fields
            private List<WComment> m_comments = new List<WComment>();
            private WComment m_currComment = null;
            private int m_currCommentIndex = 0;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the current comment.
            /// </summary>
            /// <value>The current comment.</value>
            internal WComment CurrentComment
            {
                get
                {
                    if (m_currCommentIndex < m_comments.Count)
                    {
                        return m_comments[m_currCommentIndex];
                    }

                    return null;
                }
            }
            /// <summary>
            /// Gets the comments.
            /// </summary>
            /// <value>The comments.</value>
            internal List<WComment> Comments
            {
                get
                {
                    return m_comments;
                }
            }
            #endregion

            #region Internal methods
            /// <summary>
            /// Reads the specified reader.
            /// </summary>
            /// <param name="reader">The reader.</param>
            internal override void Read(WordReader reader)
            {
                WordAnnotationReader wsReader = reader.GetSubdocumentReader(WordSubdocument.Annotation) as WordAnnotationReader;
                wsReader.Bookmarks = reader.Bookmarks;

                do
                {
                    AddComment(wsReader);
                    ReadTextBody(wsReader, m_currComment.TextBody);
                    m_currCommentIndex++;
                }
                while (wsReader.ChunkType != WordChunkType.DocumentEnd);
                m_currCommentIndex = 0;
            }
            /// <summary>
            /// Gets the next comment.
            /// </summary>
            /// <returns></returns>
            internal WComment GetNextComment()
            {
                if (m_currCommentIndex < m_comments.Count)
                {
                    return m_comments[m_currCommentIndex++];
                }

                return null;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Adds the comment.
            /// </summary>
            /// <param name="reader">The reader.</param>
            private void AddComment(WordAnnotationReader reader)
            {
                m_currComment = new WComment(DocumentEx);
                ReadCommentFormat(reader, m_currComment.Format);
                m_comments.Add(m_currComment);
            }
            #endregion

            #region Implementation / overrides
            /// <summary>
            /// Defines end of text body for annotations.
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="chunkType"></param>
            /// <returns></returns>
            protected override bool EndOfTextBody(WordReaderBase reader, WordChunkType chunkType)
            {
                return (reader as WordAnnotationReader).ItemNumber != m_currCommentIndex || chunkType == WordChunkType.DocumentEnd;
            }
            #endregion

            #region Implementation / formats
            /// <summary>
            /// Reads the comment format.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="format">The comment format.</param>
            private void ReadCommentFormat(WordAnnotationReader reader, WCommentFormat format)
            {
                AnnotationDescriptor desc = reader.Descriptor;

                if (desc != null)
                {
                    format.UserInitials = desc.UserInitials;
                    format.User = reader.User;
                    format.BookmarkStartOffset = reader.BookmarkStartOffset;
                    format.BookmarkEndOffset = reader.BookmarkEndOffset;
                    format.Position = reader.Position;
                    format.TagBkmk = desc.TagBkmk;
                }
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class FootnoteAdapter : SubDocumentAdapter
        {
            #region Fields
            /// <summary>
            /// Array of footnotes
            /// </summary>
            internal List<WFootnote> m_footEndNotes = new List<WFootnote>();
            /// <summary>
            /// Current footnote
            /// </summary>
            protected WFootnote m_currFootEndNote = null;
            /// <summary>
            /// Index of current footnote/endnote in collection of footnotes.
            /// </summary>
            internal int m_currFootEndnoteIndex = 0;
            /// <summary>
            /// Number of footnotes/endnotes in document
            /// </summary>
            protected int m_footEndNotesCount;
            #endregion

            #region Internal methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            internal override void Read(WordReader reader)
            {
                WordSubdocumentReader wsReader = Init(reader);
                wsReader.Bookmarks = reader.Bookmarks;

                for (int i = 0; i < m_footEndNotesCount; i++)
                {
                    AddFootEndNote(wsReader);
                    wsReader.MoveToItem( m_currFootEndnoteIndex );
                    ReadTextBody(wsReader, m_currFootEndNote.TextBody);
                    m_currFootEndnoteIndex++;
                }
                m_currFootEndnoteIndex = 0;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal WFootnote GetNextFootEndNote()
            {
                if (m_currFootEndnoteIndex < m_footEndNotes.Count)
                {
                    return m_footEndNotes[m_currFootEndnoteIndex++];
                }

                return null;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Initialize FootnoteAdapter
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            protected virtual WordSubdocumentReader Init(WordReader reader)
            {
                m_footEndNotesCount = reader.TablesData.Footnotes.Count - 1;
                return reader.GetSubdocumentReader(WordSubdocument.Footnote) as WordFootnoteReader;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            protected virtual void AddFootEndNote(IWordSubdocumentReader reader)
            {
                m_currFootEndNote = new WFootnote(DocumentEx);
                m_currFootEndNote.FootnoteType = FootnoteType.Footnote;
                m_footEndNotes.Add(m_currFootEndNote);
            }
            #endregion

            #region Implementation / overrides
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="chunkType"></param>
            /// <returns></returns>
            protected override bool EndOfTextBody(WordReaderBase reader, WordChunkType chunkType)
            {
                return (reader as WordFootnoteReader).ItemNumber != m_currFootEndnoteIndex || chunkType == WordChunkType.DocumentEnd;
                //return chunkType == WordChunkType.DocumentEnd;
            }
            #endregion

            #region Implementation / formats
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            private void ReadFootnoteFormat(IWordSubdocumentReader reader)
            {
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class EndnoteAdapter : FootnoteAdapter
        {
            #region Class overrides
            /// <summary>
            /// Initialize EndnoteAdapter.
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            protected override WordSubdocumentReader Init(WordReader reader)
            {
                m_footEndNotesCount = reader.TablesData.Endnotes.Count - 1;
                return reader.GetSubdocumentReader(WordSubdocument.Endnote) as WordEndnoteReader;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            protected override void AddFootEndNote(IWordSubdocumentReader reader)
            {
                m_currFootEndNote = new WFootnote(DocumentEx);
                m_currFootEndNote.FootnoteType = FootnoteType.Endnote;
                m_footEndNotes.Add(m_currFootEndNote);
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class TextboxAdapter : SubDocumentAdapter
        {
            #region Fields
            /// <summary>
            /// Currently used textbox.
            /// </summary>
            protected WTextBox m_currTextBox;
            /// <summary>
            /// Number of textboxes in document.
            /// </summary>
            protected int m_txbxCount;
            /// <summary>
            /// Type of textbox
            /// </summary>
            protected WordSubdocument m_textBoxType;
            /// <summary>
            /// Index of textbox in sequence of textboxes.
            /// </summary>
            protected int m_currentTxbxIndex;
            /// <summary>
            /// Collection of textboxes
            /// </summary>
            protected ShapeObjectTextCollection m_textBoxCollection = new ShapeObjectTextCollection();
            #endregion

            #region Internal methods
            /// <summary>
            /// Reads textboxes
            /// </summary>
            /// <param name="reader"></param>
            internal override void Read(WordReader reader)
            {
                WordSubdocumentReader wsReader = Init(reader);
                wsReader.Bookmarks = reader.Bookmarks;
                m_finalize = false;
                for (int i = 0; i < m_txbxCount - 1; i++)
                {
                    if (CreateAndAddTextBox(reader))
                    {
                        wsReader.MoveToItem(m_currentTxbxIndex);
                        ReadTextBody(wsReader, m_currTextBox.TextBoxBody);
                        m_nestedTextBodies.Clear();
                    }
                    m_currentTxbxIndex++;
                }
            }
            #endregion

            #region Implementation / overrides
            /// <summary>
            /// Defines end of textbox
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="chunkType"></param>
            /// <returns></returns>
            protected override bool EndOfTextBody(WordReaderBase reader, WordChunkType chunkType)
            {
                return chunkType == WordChunkType.DocumentEnd || m_currentTxbxIndex != (reader as WordTextBoxReader).ItemNumber;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            protected virtual WordSubdocumentReader Init(WordReader reader)
            {
                if (reader.TablesData.ArtObj.MainDocTxBxs != null)
                {
                    m_txbxCount = reader.TablesData.ArtObj.MainDocTxBxs.Count;
                }
                m_textBoxType = WordSubdocument.TextBox;

                return reader.GetSubdocumentReader(WordSubdocument.TextBox) as WordTextBoxReader;
            }
            /// <summary>
            /// Add textbox to internal textbox collection.
            /// </summary>
            private bool CreateAndAddTextBox(WordReaderBase baseReader)
            {
                bool retValue = false;
                int txBxId = baseReader.TablesData.ArtObj.GetShapeObjectId(m_textBoxType, m_currentTxbxIndex);

                if (txBxId != 0)
                {
                    m_currTextBox = new WTextBox(DocumentEx);
                    m_textBoxCollection.AddTextBox(txBxId, m_currTextBox);
                    retValue = true;
                }

                return retValue;
            }
            /// <summary>
            /// Reads the text box shape.
            /// </summary>
            /// <param name="fspa">The File Shape Address.</param>
            /// <param name="skipPositionOrigins">The skip position origins.</param>
            /// <returns></returns>
            internal WTextBox ReadTextBoxShape(FileShapeAddress fspa, bool skipPositionOrigins)
            {
                WTextBox textbox = m_textBoxCollection.GetTextBox(fspa.Spid);

                if (textbox == null)
                {
                    return null;
                }
                MsofbtSpContainer spContainer = null;
                if (DocumentEx.Escher.Containers.ContainsKey(fspa.Spid))
                {
                    spContainer = DocumentEx.Escher.Containers[fspa.Spid] as MsofbtSpContainer;
                }
                TextBoxPropertiesConverter.Export(spContainer, fspa, textbox.TextBoxFormat, skipPositionOrigins);
                return textbox;
            }
            /// <summary>
            /// Gets textbox for autoshape by spid.
            /// </summary>
            /// <param name="shapeId"></param>
            /// <returns></returns>
            internal WTextBox GetAutoShapeTextBox(int shapeId)
            {
                return m_textBoxCollection.GetTextBox(shapeId);
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class HFTextboxAdapter : TextboxAdapter
        {
            #region TextboxAdapter overrides
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            protected override WordSubdocumentReader Init(WordReader reader)
            {
                if (reader.TablesData.ArtObj.HfDocTxBxs != null)
                {
                    m_txbxCount = reader.TablesData.ArtObj.HfDocTxBxs.Count;
                }
                m_textBoxType = WordSubdocument.HeaderTextBox;

                return reader.GetSubdocumentReader(WordSubdocument.HeaderTextBox) as WordHFTextBoxReader;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="chunkType"></param>
            /// <returns></returns>
            protected override bool EndOfTextBody(WordReaderBase reader, WordChunkType chunkType)
            {
                return chunkType == WordChunkType.DocumentEnd || m_currentTxbxIndex != (reader as WordHFTextBoxReader).ItemNumber;
            }
            #endregion
        }
        #endregion
    }
}
