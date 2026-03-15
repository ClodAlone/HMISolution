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
using System.IO;
using System.Text;

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.Documentation;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for WPTablesData.
    /// </summary>
    [CLSCompliant(false)]
    [DocumentationExclude()]
    internal class WPTablesData
    {
        #region Class constants
        /// <summary>
        /// Default name for table stream.
        /// </summary>
        internal const string DEF_TABLESTREAM_NAME = "1Table";

        /// <summary>
        /// 
        /// </summary>
        private const string EXC_NOTREAD_TABLE_MESS =
          "You can not get \"table\" object without calling the Read() method!";
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        internal EscherClass m_escher = null;

        /// <summary>
        /// 
        /// </summary>
        private WPFIBData m_wpFIBData = null;

        /// <summary>
        /// 
        /// </summary>
        private SectionExceptionsTable m_sectionTable;

        /// <summary>
        /// 
        /// </summary>
        internal PieceTable m_pieceTable;

        /// <summary>
        /// 
        /// </summary>
        private FontFamilyNameStringTable m_ffnStringTable;

        /// <summary>
        /// 
        /// </summary>
        private BookmarkNameStringTable m_bkmkStringTable;

        /// <summary>
        /// 
        /// </summary>
        private BookmarkDescriptor m_bkmkDescriptor;

        /// <summary>
        /// 
        /// </summary>
        private BinaryTable m_binTableCHPX;

        /// <summary>
        /// 
        /// </summary>
        private BinaryTable m_binTablePAPX;

        /// <summary>
        /// 
        /// </summary>
        private ListInfo m_listInfo;

        /// <summary>
        /// 
        /// </summary>
        private CharPosTableRecord m_charPosTableHF;

        /// <summary>
        /// 
        /// </summary>
        private StyleSheetInfoRecord m_styleSheetInfo = new StyleSheetInfoRecord(18);

        /// <summary>
        /// 
        /// </summary>
        private StyleDefinitionRecord[] m_arrStyleDefinitions = null;

        /// <summary>
        /// 
        /// </summary>
        private List<UInt32> m_pieceTablePositions = new List<UInt32>();

        /// <summary>
        /// 
        /// </summary>
        internal List<Encoding> m_pieceTableEncodings = new List<Encoding>();

        /// <summary>
        /// 
        /// </summary>
        DOPDescriptor m_dopDescriptor = new DOPDescriptor();

        /// <summary>
        /// 
        /// </summary>
        private ArtObjectsRW m_artObjects;

        /// <summary>
        /// 
        /// </summary>
        private AnnotationsRW m_anotations;

        /// <summary>
        /// 
        /// </summary>
        private FootnotesRW m_footnotes;

        /// <summary>
        /// 
        /// </summary>
        private EndnotesRW m_endnotes;

        /// <summary>
        /// 
        /// </summary>
        private Fields m_fields;

        private byte[] m_macroCommands = null;
        private byte[] m_variables = null;
        private byte[] m_assocStrings;

        /// <summary>
        /// 
        /// </summary>
        private SinglePropertyModifierArray m_clxModifiers;

        private List<Int32> m_secPositions = new List<Int32>();
        private List<Int32> m_sepxPositions = new List<Int32>();
        private List<UInt32> m_papPositions = new List<UInt32>();
        private List<Int32> m_papxPositions = new List<Int32>();
        private List<UInt32> m_chpPositions = new List<UInt32>();
        private List<Int32> m_chpxPositions = new List<Int32>();
        private int[] m_headerPositions;

        /// <summary>
        /// 
        /// </summary>
        private string m_standardAsciiFont;

        private string m_standardFarEastFont;
        private string m_standardNonFarEastFont;
        /// <summary>
        /// Represents the default CS/Bidi font
        /// </summary>
        private string m_standardBidiFont;
        /// <summary>
        /// 
        /// </summary>
        private GrammarSpelling m_grammarSpellingTablesData;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WPTablesData"/> class.
        /// </summary>
        /// <param name="fib">The fib.</param>
        internal WPTablesData(WPFIBData fib)
        {
            m_wpFIBData = fib;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Get artobject's object
        /// </summary>
        internal ArtObjectsRW ArtObj
        {
            get
            {
                if (m_artObjects == null)
                {
                    m_artObjects = new ArtObjectsRW();
                }

                return m_artObjects;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal AnnotationsRW Annotations
        {
            get
            {
                if (m_anotations == null)
                {
                    m_anotations = new AnnotationsRW();
                }

                return m_anotations;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal FootnotesRW Footnotes
        {
            get
            {
                if (m_footnotes == null)
                {
                    m_footnotes = new FootnotesRW();
                }

                return m_footnotes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal EndnotesRW Endnotes
        {
            get
            {
                if (m_endnotes == null)
                {
                    m_endnotes = new EndnotesRW();
                }

                return m_endnotes;
            }
        }

        /// <summary>
        /// Gets sections table.
        /// </summary>
        internal SectionExceptionsTable SectionsTable
        {
            get
            {
                if (m_sectionTable == null)
                {
                    throw new InvalidOperationException(EXC_NOTREAD_TABLE_MESS);
                }

                return m_sectionTable;
            }
        }

        /// <summary>
        /// Gets/sets FontFamilyNameStringTable 
        /// </summary>
        internal FontFamilyNameStringTable FFNStringTable
        {
            get
            {
                if (m_ffnStringTable == null)
                {
                    throw new InvalidOperationException(EXC_NOTREAD_TABLE_MESS);
                }

                return m_ffnStringTable;
            }

            set
            {
                m_ffnStringTable = value;
            }
        }

        /// <summary>
        /// Gets/sets stylesheet information
        /// </summary>
        internal StyleSheetInfoRecord StyleSheetInfo
        {
            get
            {
                return m_styleSheetInfo;
            }

            set
            {
                m_styleSheetInfo = value;
            }
        }

        /// <summary>
        /// Gets/sets array of style definition records.
        /// </summary>
        internal StyleDefinitionRecord[] StyleDefinitions
        {
            get
            {
                return m_arrStyleDefinitions;
            }

            set
            {
                m_arrStyleDefinitions = value;
            }
        }

        /// <summary>
        /// Gets header footer character position table.
        /// </summary>
        internal CharPosTableRecord HeaderFooterCharPosTable
        {
            get
            {
                return m_charPosTableHF;
            }
        }

        /// <summary>
        /// Gets chpx BinaryTable
        /// </summary>
        internal BinaryTable CHPXBinaryTable
        {
            get
            {
                return m_binTableCHPX;
            }
        }

        /// <summary>
        /// Gets papx BinaryTable
        /// </summary>
        internal BinaryTable PAPXBinaryTable
        {
            get
            {
                return m_binTablePAPX;
            }
        }

        /// <summary>
        /// Gets/sets header positions
        /// </summary>
        internal int[] HeaderPositions
        {
            get
            {
                return m_headerPositions;
            }

            set
            {
                m_headerPositions = value;
            }
        }

        /// <summary>
        /// Gets number of sections
        /// </summary>
        internal int SectionCount
        {
            get
            {
                return m_secPositions.Count + 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal BookmarkNameStringTable BookmarkStrings
        {
            get
            {
                if (m_bkmkStringTable == null)
                {
                    m_bkmkStringTable = new BookmarkNameStringTable();
                }

                return m_bkmkStringTable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal BookmarkDescriptor BookmarkDescriptor
        {
            get
            {
                if (m_bkmkDescriptor == null)
                {
                    m_bkmkDescriptor = new BookmarkDescriptor();
                }

                return m_bkmkDescriptor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal DOPDescriptor DOP
        {
            get
            {
                return m_dopDescriptor;
            }

            set
            {
                m_dopDescriptor = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ListInfo ListInfo
        {
            get
            {
                if (m_listInfo == null)
                {
                    m_listInfo = new ListInfo();
                }

                return m_listInfo;
            }

            set
            {
                m_listInfo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal List<UInt32> PieceTablePositions
        {
            get
            {
                return m_pieceTablePositions;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ArtObjectsRW FileArtObjects
        {
            get
            {
                return m_artObjects;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal EscherClass Escher
        {
            get
            {
                if (m_escher == null)
                {
                    m_escher = new EscherClass(null);
                }

                return m_escher;
            }

            set
            {
                m_escher = value;
            }
        }

        /// <summary>
        /// Get/set fields property
        /// </summary>
        internal Fields Fields
        {
            get
            {
                if (m_fields == null)
                {
                    m_fields = new Fields();
                }

                return m_fields;
            }

            set
            {
                m_fields = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal byte[] MacroCommands
        {
            get
            {
                return m_macroCommands;
            }

            set
            {
                m_macroCommands = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal WPFIBData FIBData
        {
            get
            {
                return m_wpFIBData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string StandardAsciiFont
        {
            get
            {
                return m_standardAsciiFont;
            }

            set
            {
                m_standardAsciiFont = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string StandardFarEastFont
        {
            get
            {
                return m_standardFarEastFont;
            }

            set
            {
                m_standardFarEastFont = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string StandardNonFarEastFont
        {
            get
            {
                return m_standardNonFarEastFont;
            }

            set
            {
                m_standardNonFarEastFont = value;
            }
        }
        /// <summary>
        /// Gets and sets the standard/default bidi font
        /// </summary>
        internal string StandardBidiFont
        {
            get
            {
                return m_standardBidiFont;
            }
            set
            {
                m_standardBidiFont = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal GrammarSpelling GrammarSpellingData
        {
            get
            {
                return m_grammarSpellingTablesData;
            }

            set
            {
                m_grammarSpellingTablesData = value;
            }
        }

        /// <summary>
        /// Gets or sets the byte array of document variables.
        /// </summary>
        /// <value>The variables.</value>
        internal byte[] Variables
        {
            get
            {
                return m_variables;
            }

            set
            {
                m_variables = value;
            }
        }

        /// <summary>
        /// Gets or sets the asociated strings.
        /// </summary>
        /// <value>The asociated strings.</value>
        internal byte[] AsociatedStrings
        {
            get
            {
                return m_assocStrings;
            }

            set
            {
                m_assocStrings = value;
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Read structures from stream
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            WPFIBData fib = m_wpFIBData;

            // Create section table
            //Trace.WriteLine("Section table reading...");
            stream.Position = fib.fcPlcfsed;
            m_sectionTable = new SectionExceptionsTable(stream, fib.lcbPlcfsed);

            // Create FFN table
            //Trace.WriteLine("FFN table reading...");
            ReadFFNTable(stream, fib);

            ReadBookmarks(fib, stream);
            stream.Position = fib.fcPlcftxbxTxt;
            // Create field table
            ReadFields(fib, stream);

            // Create CHPX table
            //Trace.WriteLine("CHPx table reading...");
            stream.Position = fib.fcPlcfbteChpx;
            m_binTableCHPX = new BinaryTable(stream, fib.lcbPlcfbteChpx);

            // Create PAPX table
            //Trace.WriteLine("PAPx table reading...");
            stream.Position = fib.fcPlcfbtePapx;
            m_binTablePAPX = new BinaryTable(stream, fib.lcbPlcfbtePapx);

            //Trace.WriteLine("StyleSheet table reading...");
            ReadStyleSheet(stream);

            //Trace.WriteLine("Header table reading...");

            if (fib.lcbPlcfhdd > 0)
            {
                stream.Position = fib.fcPlcfhdd;
                m_charPosTableHF = new CharPosTableRecord(stream, fib.lcbPlcfhdd);
            }

            ReadLists(fib, stream);

            ReadDocumentProperties(fib, stream);

            //ExtractListTables         (node, stream, fib);
            //ParseGlossaryFiles        (node, stream, fib);
            //ParseComplexPart          (node, stream, fib);
            ReadComplexPart(stream);

            ParsePieceTableEncodings();

            // read footnote tables
            ReadFootnotes(fib, stream);
            // read annotation tables
            ReadAnnotations(fib, stream);
            // read endnote tables
            ReadEndnotes(fib, stream);
            // read artobject tables
            ReadArtObjects(fib, stream);

            ReadMacroCommands(fib, stream);

            ReadGrammarSpellingData(fib, stream);
            // Read document variables.
            ReadVariables(fib, stream);
            // Read table of associated strings
            ReadAssocStrings(fib, stream);
        }

        /// <summary>
        /// Write member structures to stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="hasSubDocument"></param>
        internal void Write(Stream stream, bool hasSubDocument)
        {
            WPFIBData fib = m_wpFIBData;
            GenerateTables(hasSubDocument);

            // FFN Table
            fib.fcSttbfffn = (int)stream.Position;
            //stream.Write(BitConverter.GetBytes((ushort)478), 0, 2);
            WriteFFNTable(stream);

            fib.lcbSttbfffn = (int)(stream.Position - fib.fcSttbfffn);
            WriteStyleSheet(stream);
            fib.lcbStshfOrig = fib.lcbStshf = (int)(stream.Position - fib.fcStshf);

            // Sections
            fib.fcPlcfsed = (int)stream.Position;
            fib.lcbPlcfsed = m_sectionTable.Save(stream);

            // CHPx
            fib.fcPlcfbteChpx = (int)stream.Position;
            m_binTableCHPX.Save(stream);
            fib.lcbPlcfbteChpx = m_binTableCHPX.Length;

            // PAPx
            stream.Position = fib.fcPlcfbteChpx + fib.lcbPlcfbteChpx;
            fib.fcPlcfbtePapx = (int)stream.Position;

            m_binTablePAPX.Save(stream);
            fib.lcbPlcfbtePapx = m_binTablePAPX.Length;

            if (hasSubDocument)
            {
                // Header/Footer 
                fib.fcPlcfhdd = (int)stream.Position;
                fib.lcbPlcfhdd = 0;

                if (m_charPosTableHF != null && m_charPosTableHF.Positions != null)
                {
                    fib.lcbPlcfhdd = m_charPosTableHF.Save(stream);
                }
            }

            // Clx (Piece Table)
            fib.fcClx = (int)stream.Position;
            m_pieceTable.Save(stream);
            fib.lcbClx = (m_pieceTable.Length);

            WriteLists(stream);

            WriteFields(stream, fib);

            WriteBookmarks(stream, fib);

            // Writing Document Properties
            WriteDocumentProperties(stream, fib);

            if (hasSubDocument)
            {
                // Write footnote tables.
                WriteFootnotes(fib, stream);
                // Write annotation tables.
                WriteAnnotations(fib, stream);
                // Write endonote tables.
                WriteEndnotes(fib, stream);
            }

            WriteArtObjects(stream, fib);
            WriteMacroCommands(fib, stream);
            WriteGrammarSpellingData(fib, stream);
            WriteVariables(fib, stream);
            WriteAssocStrings(fib, stream);
        }

        /// <summary>
        /// Converts position in text to position in file
        /// </summary>
        /// <param name="charPos"></param>
        /// <returns></returns>
        internal UInt32 ConvertCharPosToFileCharPos(UInt32 charPos)
        {
            UInt32 resFileCharPos = UInt32.MaxValue;
            PieceTable pieceTable = m_pieceTable;

            for (int i = 0, end = pieceTable.EntriesCount; i < end; i++)
            {
                // If specified pos contains in pieceTable positions
                // get it.
                if (charPos >= pieceTable.FileCharacterPos[i] && charPos < pieceTable.FileCharacterPos[i + 1])
                {
                    bool bIsUnicode;
                    resFileCharPos = NormalizeFileCharPos(pieceTable.Entries[i].FileOffset, out bIsUnicode);

                    // Calculate FileCharPos value
                    if (!bIsUnicode)
                    {
                        resFileCharPos += (charPos - pieceTable.FileCharacterPos[i]);
#if SILVERLIGHT || WP
                        m_wpFIBData.Encoding = Encoding.UTF8;
#else
                        m_wpFIBData.Encoding = Encoding.ASCII;
#endif
                    }
                    else
                    {
                        resFileCharPos += ((charPos - pieceTable.FileCharacterPos[i]) * 2);
                        m_wpFIBData.Encoding = Encoding.Unicode;
                    }

                    break;
                }
            }

            // If not found range in pieceTable, get last position
            if (resFileCharPos == UInt32.MaxValue)
            {
                int lastEntryIndex = pieceTable.EntriesCount - 1;
                bool bIsUnicode;
                resFileCharPos =
                  NormalizeFileCharPos(pieceTable.Entries[lastEntryIndex].FileOffset, out bIsUnicode);

                // Calculate FileCharPos value
                if (!bIsUnicode)
                {
                    resFileCharPos += (charPos - pieceTable.FileCharacterPos[lastEntryIndex]);
#if SILVERLIGHT || WP
                        m_wpFIBData.Encoding = Encoding.UTF8;
#else
                        m_wpFIBData.Encoding = Encoding.ASCII;
#endif
                }
                else
                {
                    resFileCharPos += ((charPos - pieceTable.FileCharacterPos[lastEntryIndex]) * 2);
                    m_wpFIBData.Encoding = Encoding.Unicode;
                }
            }

            return resFileCharPos;
        }

        /// <summary>
        /// Gets array of bookmarks from the document
        /// </summary>
        /// <returns></returns>
        internal BookmarkInfo[] GetBookmarks()
        {
            if (m_bkmkStringTable == null || m_bkmkStringTable.BookmarkCount == 0)
            {
                return null;
            }

            int count = m_bkmkStringTable.BookmarkCount;
            BookmarkInfo[] bookmarkInfoArr = new BookmarkInfo[count];
            for (int i = 0; i < count; i++)
            {
                bookmarkInfoArr[i] =
                  new BookmarkInfo(
                  m_bkmkStringTable[i], m_bkmkDescriptor.GetBeginPos(i), m_bkmkDescriptor.GetEndPos(i),
                  m_bkmkDescriptor.IsCellGroup(i), m_bkmkDescriptor.GetStartCellIndex(i),
                  m_bkmkDescriptor.GetEndCellIndex(i));
                bookmarkInfoArr[i].Index = i;
            }

            return bookmarkInfoArr;
        }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="ilfo"></param>
        //    /// <param name="ilvl"></param>
        //    /// <returns></returns>
        //    internal ParagraphPropertyException GetListPapx(int ilfo, int ilvl)
        //    {
        //      ParagraphPropertyException papx = new ParagraphPropertyException();
        //      ListDataRecord record = new ListDataRecord();
        //      int listId = ((ListFormatOverrideRecord)m_listOverride[ilfo - 1]).ListId;
        //      for (int i = 0; i < m_listRecord.Count; i++)
        //      {
        //        if (listId == ((ListDataRecord)m_listRecord[i]).ListId)
        //        {
        //          record = (ListDataRecord)m_listRecord[i];
        //        }
        //      }
        //      papx.PropertyModifiers = record.ListLevels[ilvl].Papx;
        //      return papx;
        //    }
        //
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="ilfo"></param>
        //    /// <param name="ilvl"></param>
        //    /// <returns></returns>
        //    internal CharacterPropertyException GetListChpx(int ilfo, int ilvl)
        //    {
        //      CharacterPropertyException chpx = new CharacterPropertyException();
        //      ListDataRecord record = new ListDataRecord();
        //      int listId = ((ListFormatOverrideRecord)m_listOverride[ilfo - 1]).ListId;
        //      for (int i = 0; i < m_listRecord.Count; i++)
        //      {
        //        if (listId == ((ListDataRecord)m_listRecord[i]).ListId)
        //        {
        //          record = (ListDataRecord)m_listRecord[i];
        //        }
        //      }
        //      chpx.PropertyModifiers = record.ListLevels[ilvl].Chpx;
        //      return chpx;
        //    }
        //
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="ilfo"></param>
        //    /// <returns></returns>
        //    internal ListDataRecord GetListRecord(int ilfo)
        //    {
        //      if (m_listRecord.Count < ilfo)
        //      {
        //        InitListRecord(ilfo);
        //      }
        //      int listId = ((ListFormatOverrideRecord)m_listOverride[ilfo - 1]).ListId;
        //      //
        //
        //      ListDataRecord record = new ListDataRecord();
        //      for (int i = 0; i < m_listRecord.Count; i++)
        //      {
        //        if (listId == ((ListDataRecord)m_listRecord[i]).ListId)
        //        {
        //          record = (ListDataRecord)m_listRecord[i];
        //          break;
        //        }
        //      }
        //      return record;
        //    }
        //
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="ilfo"></param>
        //    /// <returns></returns>
        //    internal ListLevelRecord GetListLevel(int ilfo, int ilvl)
        //    {
        //      if (m_listOverride.Count < ilfo)
        //      {
        //        InitListRecord(ilfo);
        //      }
        //      int listId = ((ListFormatOverrideRecord)m_listOverride[ilfo - 1]).ListId;
        //      for (int i = 0; i < m_listRecord.Count; i++)
        //      {
        //        if (listId == ((ListDataRecord)m_listRecord[i]).ListId)
        //        {
        //          return ((ListDataRecord)m_listRecord[i]).ListLevels[ilvl];
        //        }
        //      }
        //      return null;
        //    }
        //
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="index"></param>
        //    /// <returns></returns>
        //    internal ListFormatOverrideRecord GetListFormatOverride(int index)
        //    {
        //      return (ListFormatOverrideRecord)m_listOverride[index - 1];
        //    }

        /// <summary>
        /// Add section record
        /// </summary>
        /// <param name="charPos"></param>
        /// <param name="sepxPos"></param>
        internal void AddSectionRecord(int charPos, int sepxPos)
        {
            m_secPositions.Add(charPos);
            m_sepxPositions.Add(sepxPos);
        }

        /// <summary>
        /// Add papx record
        /// </summary>
        /// <param name="charPos"></param>
        /// <param name="papxPos"></param>
        internal void AddPapxRecord(uint charPos, int papxPos)
        {
            m_papPositions.Add(charPos);
            m_papxPositions.Add(papxPos);
        }

        /// <summary>
        /// Add chpx record
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="chpxPos"></param>
        internal void AddChpxRecord(uint pos, int chpxPos)
        {
            m_chpPositions.Add(pos);
            m_chpxPositions.Add(chpxPos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="styleSheet"></param>
        internal void AddStyleSheetTable(WordStyleSheet styleSheet)
        {
            int stCount = styleSheet.StylesCount;

            StyleSheetInfo.StylesCount = (ushort)stCount;
            StyleDefinitionRecord[] records = new StyleDefinitionRecord[stCount];
            //ushort styleUID = 4095;
            ushort styleUID = 4094;

            for (int i = 0; i < stCount; i++)
            {
                // Gets style object by index
                WordStyle style = styleSheet.GetStyleByIndex(i);

                // Make STD record by style info
                StyleDefinitionRecord record = null;
                if (style != WordStyle.Empty)
                {
                    //ushort styleID = (ushort)(style.ID > -1 ? style.ID : --styleUID);
                    ushort styleID = (ushort)(style.ID > -1 ? style.ID : styleUID);
                    record = new StyleDefinitionRecord(style.Name, styleID, StyleSheetInfo);

                    if (styleID > 0 && styleID < 10)
                    {
                        record.IsQFormat = true;
                        record.UnhideWhenUsed = true;
                    }
                    else if (style.IsPrimary)
                        record.IsQFormat = true;

                    //record.BaseStyle = 0;
                    record.BaseStyle = (ushort)style.BaseStyleIndex;
                    record.NextStyleId = (ushort)style.NextStyleIndex;
                    record.HasUpe = style.HasUpe;
                    record.IsSemiHidden = style.IsSemiHidden;
                    record.UnhideWhenUsed = style.UnhideWhenUsed;
                    record.TypeCode = style.TypeCode;
                    // Init style link identifier
                    if (style.LinkStyleIndex >= 0 && style.LinkStyleIndex < stCount)
                        record.LinkStyleId = (ushort)style.LinkStyleIndex;

                    if (style.IsCharacterStyle)
                    {
                        record.TypeCode = WordStyleType.CharacterStyle;
                        //            record.CharacterProperty = style.CharacterProperties.CloneChpx();
                        record.CharacterProperty = style.CharacterProperties.CharacterPropertyException;
                        record.ParagraphProperty = null;
                    }
                    else
                    {
                        if (record.TypeCode == WordStyleType.TableStyle && style.TableStyleData != null)
                        {
                            record.UpxNumber = 3;
                            record.Tapx = new byte[style.TableStyleData.Length];
                            Buffer.BlockCopy(style.TableStyleData, 0, record.Tapx, 0, style.TableStyleData.Length);
                        }
                        else
                        {
                            record.TypeCode = WordStyleType.ParagraphStyle;
                            //            record.ParagraphProperty = style.ParagraphProperties.ClonePapx();
                            //            record.CharacterProperty = style.CharacterProperties.CloneChpx();
                            record.ParagraphProperty = style.ParagraphProperties.ParagraphPropertyException;
                            record.CharacterProperty = style.CharacterProperties.CharacterPropertyException;
                        }
                    }
                }

                // Set item of STD array
                records[i] = record;
            }

            StyleDefinitions = records;

            ushort[] standardChpStsh = new ushort[3];
            if (m_standardAsciiFont != null)
            {
                standardChpStsh[0] = (ushort)styleSheet.FontNameToIndex(m_standardAsciiFont);
            }

            if (m_standardFarEastFont != null)
            {
                standardChpStsh[1] = (ushort)styleSheet.FontNameToIndex(m_standardFarEastFont);
            }
            if (m_standardNonFarEastFont != null)
            {
                standardChpStsh[2] = (ushort)styleSheet.FontNameToIndex(m_standardNonFarEastFont);
            }
            if (m_standardBidiFont != null)
            {
                StyleSheetInfo.FtcBi = (ushort)styleSheet.FontNameToIndex(m_standardBidiFont);
            }
            StyleSheetInfo.StandardChpStsh = standardChpStsh;

            AddFFNTable(styleSheet);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            m_wpFIBData = null;
            m_sectionTable = null;
            m_pieceTable = null;
            m_ffnStringTable = null;
            m_bkmkStringTable = null;
            m_binTableCHPX = null;
            m_binTablePAPX = null;
            m_listInfo = null;
            m_charPosTableHF = null;
            m_styleSheetInfo = null;

            if (m_arrStyleDefinitions != null && m_arrStyleDefinitions.Length > 0)
            {
                StyleDefinitionRecord record = null;
                int cnt = m_arrStyleDefinitions.Length;
                for (int i = 0; i < cnt; i++)
                {
                    record = m_arrStyleDefinitions[i];
                    record = null;
                }
            }

            m_pieceTablePositions = null;
            m_pieceTableEncodings = null;
            m_dopDescriptor = null;
            m_escher = null;
            m_artObjects = null;
            m_anotations = null;
            m_footnotes = null;
            m_endnotes = null;
            m_fields = null;
            m_macroCommands = null;
            m_variables = null;
            m_clxModifiers = null;
            m_secPositions = null;
            m_sepxPositions = null;
            m_papPositions = null;
            m_papxPositions = null;
            m_chpPositions = null;
            m_chpxPositions = null;
            m_grammarSpellingTablesData = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        private void ReadFFNTable(Stream stream, WPFIBData fib)
        {
            m_ffnStringTable = new FontFamilyNameStringTable();
            stream.Position = fib.fcSttbfffn;
            m_ffnStringTable.Parse(stream, fib.lcbSttbfffn);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal Encoding GetEncodingByFC(long position)
        {
            //for (int i = 0; i < m_pieceTablePositions.Count; i++)
            for (int i = 0; i < m_pieceTable.EntriesCount; i++)
            {
                if (m_pieceTablePositions[i] <= position && position <= m_pieceTablePositions[i + 1])
                {
                    return m_pieceTableEncodings[i];
                }
            }

            return m_wpFIBData.Encoding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        internal uint ConvertFCToCP(uint fc)
        {
            uint cp = 0;
            for (int i = 0, count = m_pieceTablePositions.Count - 1; i < count; i++)
            {
                if (fc >= m_pieceTablePositions[i] && fc <= m_pieceTablePositions[i + 1])
                {
                    uint charSize = (uint)((m_pieceTableEncodings[i] == Encoding.Unicode) ? 2 : 1);

                    if (fc == m_pieceTablePositions[i + 1])
                    {
                        i += 1;
                    }

                    uint dif = (fc - m_pieceTablePositions[i]) / charSize;
                    cp = m_pieceTable.FileCharacterPos[i] + dif;
                    break;
                }
            }

            return cp;
        }

        /// <summary>
        /// Determines whether the document has the specified type of subdocument.
        /// </summary>
        /// <param name="wsType">Type of the subdocument.</param>
        /// <returns>
        /// <c>true</c> if the specified ws type has subdocument; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasSubdocument(WordSubdocument wsType)
        {
            switch (wsType)
            {
                case WordSubdocument.Annotation:
                    return (m_anotations != null && m_anotations.Count > 0);
                case WordSubdocument.Endnote:
                    return (m_endnotes != null && m_endnotes.Count > 0);
                case WordSubdocument.Footnote:
                    return (m_footnotes != null && m_footnotes.Count > 0);
                case WordSubdocument.TextBox:
                    return (m_artObjects != null && m_artObjects.MainDocTxBxs != null && m_artObjects.MainDocTxBxs.Count > 0);
                case WordSubdocument.HeaderTextBox:
                    return (m_artObjects != null && m_artObjects.HfDocTxBxs != null && m_artObjects.HfDocTxBxs.Count > 0);
            }

            return true;
        }

        /// <summary>
        /// Determines whether this instance has list.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance has list; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasList()
        {
            return (m_listInfo == null) ? false : true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddFFNTable(WordStyleSheet styleSheet)
        {
            FontFamilyNameStringTable ffnStringTable = new FontFamilyNameStringTable();

            ffnStringTable.RecordsCount = styleSheet.FontNamesList.Count;

            for (int i = 0, len = ffnStringTable.FontFamilyNameRecords.Length; i < len; i++)
            {
                bool flag = true;
                if (m_ffnStringTable != null)
                {
                    for (int j = 0; j < FFNStringTable.RecordsCount; j++)
                    {
                        if (FFNStringTable.FontFamilyNameRecords[j].FontName == styleSheet.FontNamesList[i])
                        {
                            ffnStringTable.FontFamilyNameRecords[i] = FFNStringTable.FontFamilyNameRecords[j];
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    FontFamilyNameRecord record = new FontFamilyNameRecord();
                    if (styleSheet.FontSubstitutionTable.ContainsKey(styleSheet.FontNamesList[i]))
                        record.AlternativeFontName = styleSheet.FontSubstitutionTable[styleSheet.FontNamesList[i]];
                    record.FontName = styleSheet.FontNamesList[i];
                    record.PitchRequest = 2;
                    record.TrueType = true;
                    record.FontFamilyID = 1;
                    record.Weight = 400;

                    ffnStringTable.FontFamilyNameRecords[i] = record;
                }
            }

            FFNStringTable = ffnStringTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void WriteFFNTable(Stream stream)
        {
            m_ffnStringTable.Save(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void WriteStyleSheet(Stream stream)
        {
            // Stylesheets
            m_wpFIBData.fcStshfOrig = m_wpFIBData.fcStshf = (int)stream.Position;

            ushort stdCount = (ushort)(m_arrStyleDefinitions.Length);
            ushort stshLength = (ushort)m_styleSheetInfo.Length;

            WriteShort(stream, stshLength);

            m_styleSheetInfo.StylesCount = stdCount;
            m_styleSheetInfo.Save(stream);
            for (int i = 0, len = m_arrStyleDefinitions.Length; i < len; i++)
            {
                StyleDefinitionRecord record = m_arrStyleDefinitions[i];

                if (record == null || record.StyleName == null)
                {
                    stream.Write(new byte[2] { 0, 0 }, 0, 2);
                }
                else
                {
                    //record.TypeCode = WordStyleType.CharacterStyle;
                    //record.BaseStyle = 4095;

                    if (record.StyleId == 0 || record.StyleId == 65)
                    {
                        record.BaseStyle = 4095;
                    }

                    WriteShort(stream, (ushort)record.Length);
                    int iCount = record.Save(stream);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private int WriteShort(Stream stream, ushort val)
        {
            byte[] usValArr = BitConverter.GetBytes(val);
            stream.Write(usValArr, 0, usValArr.Length);

            return usValArr.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void ReadStyleSheet(Stream stream)
        {
            long lNextBlockStart = ReadStyleSheetTable(stream);
            ReadStylesDefinitions(stream, lNextBlockStart);

            if (StyleSheetInfo.StandardChpStsh[0] < m_ffnStringTable.FontFamilyNameRecords.Length)
                m_standardAsciiFont = m_ffnStringTable.FontFamilyNameRecords[StyleSheetInfo.StandardChpStsh[0]].FontName;

            if (StyleSheetInfo.StandardChpStsh[1] < m_ffnStringTable.FontFamilyNameRecords.Length)
                m_standardFarEastFont = m_ffnStringTable.FontFamilyNameRecords[StyleSheetInfo.StandardChpStsh[1]].FontName;

            if (StyleSheetInfo.StandardChpStsh[2] < m_ffnStringTable.FontFamilyNameRecords.Length)
                m_standardNonFarEastFont = m_ffnStringTable.FontFamilyNameRecords[StyleSheetInfo.StandardChpStsh[2]].FontName;

            if (StyleSheetInfo.FtcBi < m_ffnStringTable.FontFamilyNameRecords.Length)
                m_standardBidiFont = m_ffnStringTable.FontFamilyNameRecords[StyleSheetInfo.FtcBi].FontName;

            if (string.IsNullOrEmpty(m_standardAsciiFont))
                m_standardAsciiFont = WordStyleSheet.DEF_FONT_NAME;
            if (string.IsNullOrEmpty(m_standardFarEastFont))
                m_standardFarEastFont = WordStyleSheet.DEF_FONT_NAME;
            if (string.IsNullOrEmpty(m_standardNonFarEastFont))
                m_standardNonFarEastFont = WordStyleSheet.DEF_FONT_NAME;
            if (string.IsNullOrEmpty(m_standardBidiFont))
                m_standardBidiFont = WordStyleSheet.DEF_FONT_NAME;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private long ReadStyleSheetTable(Stream stream)
        {
            int iPos = m_wpFIBData.fcStshf;
            int iLength = m_wpFIBData.lcbStshf;

            if (iLength <= 0)
            {
                throw new Exception("Length of StyleSheetInfo record can not be less 0!");
            }

            stream.Position = iPos;
            ushort usCount = BaseWordRecord.ReadUInt16(stream);
            m_styleSheetInfo = new StyleSheetInfoRecord(stream, usCount);

            return iPos + iLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lNextBlockStart"></param>
        private void ReadStylesDefinitions(Stream stream, long lNextBlockStart)
        {
            int iCount = m_styleSheetInfo.StylesCount;
            ushort usCount;
            StyleDefinitionRecord[] records = new StyleDefinitionRecord[iCount];

            for (int i = 0; i < iCount; i++)
            {
                usCount = BaseWordRecord.ReadUInt16(stream);
                StyleDefinitionRecord record =
                  new StyleDefinitionRecord(stream, usCount, m_styleSheetInfo);

                records[i] = record;

                if (stream.Position > lNextBlockStart)
                {
                    throw new Exception("Stream position is too big.");
                }
            }

            m_arrStyleDefinitions = records;
        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateTables(bool hasSubDocument)
        {
            // Style sheet table
            //m_styleSheetInfo = new StyleSheetInfoRecord();

            // Section table
            m_sectionTable = new SectionExceptionsTable();
            m_sectionTable.EntriesCount = m_secPositions.Count;

            for (int i = 0, len = m_secPositions.Count; i < len; i++)
            {
                m_sectionTable.Positions[i] = m_secPositions[i];
                m_sectionTable.Descriptors[i].MacPrintOffset = -1;
                m_sectionTable.Descriptors[i].SepxPosition =
                  (uint)(Constants.DiskPageSize * m_sepxPositions[i]);

            }

            int position = m_wpFIBData.ccpText
              + m_wpFIBData.ccpFtn
              + m_wpFIBData.ccpHdr
              + m_wpFIBData.ccpAtn
              + m_wpFIBData.ccpEdn
              + m_wpFIBData.ccpTxbx
              + m_wpFIBData.ccpHdrTxbx;
            //if (hasSubDocument)
            //  position++;
            m_sectionTable.Positions[m_secPositions.Count] = position;

            // CHPX table
            /*
            BinTableEntry chEntry = new BinTableEntry();
            chEntry.Value = m_wpFKPData.ChpxPage;//*/

            m_binTableCHPX = new BinaryTable();
            m_binTableCHPX.EntriesCount = m_chpPositions.Count;

            for (int i = 0, len = m_chpPositions.Count; i < len; i++)
            {
                m_binTableCHPX.FileCharacterPos[i] = m_chpPositions[i];
                BinTableEntry pEntry = new BinTableEntry();
                pEntry.Value = m_chpxPositions[i];
                m_binTableCHPX.Entries[i] = pEntry;
            }
            //m_binTableCHPX.FileCharacterPos[0] = (uint)m_wpFIBData.fcMin;
            //m_binTableCHPX.FileCharacterPos[1] = (uint)m_wpFIBData.fcMac;//2049;
            //m_binTableCHPX.Entries[0] = chEntry;

            m_binTableCHPX.FileCharacterPos[m_binTableCHPX.EntriesCount] = (uint)m_wpFIBData.fcMac; //2049;
            // PAPX table
            //BinTableEntry pEntry = new BinTableEntry();
            //pEntry.Value = m_wpFKPData.PapxPage;

            m_binTablePAPX = new BinaryTable();
            m_binTablePAPX.EntriesCount = m_papPositions.Count;

            for (int i = 0, len = m_papPositions.Count; i < len; i++)
            {
                m_binTablePAPX.FileCharacterPos[i] = m_papPositions[i];
                BinTableEntry pEntry = new BinTableEntry();
                pEntry.Value = m_papxPositions[i];
                m_binTablePAPX.Entries[i] = pEntry;
            }

            m_binTablePAPX.FileCharacterPos[m_binTablePAPX.EntriesCount] = (uint)m_wpFIBData.fcMac; //2049;
            /*
            m_binTablePAPX.FileCharacterPos[0] = (uint)m_wpFIBData.fcMin;//1536;
            m_binTablePAPX.FileCharacterPos[1] = (uint)m_wpFIBData.fcMac;//2049;
            m_binTablePAPX.Entries[0] = pEntry; //*/

            // Piece table
            m_pieceTable = new PieceTable();
            m_pieceTable.EntriesCount = 1;
            m_pieceTable.FileCharacterPos[0] = 0;
            m_pieceTable.FileCharacterPos[1] = (uint)position;
            //m_pieceTable.FileCharacterPos[1] = (uint)(m_wpFIBData.ccpText 
            //  + m_wpFIBData.ccpFtn
            //  + m_wpFIBData.ccpEdn
            //  + m_wpFIBData.ccpHdr 
            //  + m_wpFIBData.ccpAtn 
            //  + m_wpFIBData.ccpTxbx
            //  + m_wpFIBData.ccpHdrTxbx);

            m_pieceTable.Entries[0].FileOffset =
              (uint)(m_wpFIBData.Encoding == Encoding.Unicode ? 2048 : 0x40001000);

            m_pieceTable.Entries[0].fCopied = true;

            if (hasSubDocument)
            {
                // Header tables 
                m_charPosTableHF = new CharPosTableRecord();
                m_charPosTableHF.Positions = m_headerPositions;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileCharPos"></param>
        /// <param name="bIsUnicode"></param>
        /// <returns></returns>
        private UInt32 NormalizeFileCharPos(UInt32 fileCharPos, out bool bIsUnicode)
        {
            bIsUnicode = true;

            if ((fileCharPos & 0x40000000) != 0)
            {
                fileCharPos = fileCharPos & 0xbfffffff;
                fileCharPos = fileCharPos / 2;

                bIsUnicode = false;
            }

            return fileCharPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void ReadComplexPart(Stream stream)
        {
            int iStartPos = m_wpFIBData.fcClx;
            int uiLength = m_wpFIBData.lcbClx;
            long lLastByte = iStartPos + uiLength;

            stream.Position = iStartPos;

            while (stream.Position < lLastByte)
            {
                WordComplexBlockType blockType = (WordComplexBlockType)stream.ReadByte();
                byte[] arrBuffer;
                int iCount;

                switch (blockType)
                {
                    case WordComplexBlockType.Sprms:
                        iCount = ReadUInt16(stream);

                        arrBuffer = new byte[iCount];
                        int iReadCount = stream.Read(arrBuffer, 0, iCount);

                        if (iReadCount != iCount)
                        {
                            throw new Exception("Was unable to read specified number of bytes");
                        }

                        m_clxModifiers = new SinglePropertyModifierArray();
                        m_clxModifiers.Parse(arrBuffer, 0, iCount);
                        break;

                    case WordComplexBlockType.PieceTable:
                        arrBuffer = new byte[Constants.BytesInInt];
                        iCount = stream.Read(arrBuffer, 0, Constants.BytesInInt);

                        if (iCount != Constants.BytesInInt)
                        {
                            throw new Exception("Was unable to read bytes from the stream");
                        }

                        iCount = BitConverter.ToInt32(arrBuffer, 0);

                        arrBuffer = new byte[iCount];

                        if (iCount != stream.Read(arrBuffer, 0, iCount))
                        {
                            throw new Exception("Was unable to read bytes from the stream");
                        }

                        m_pieceTable = new PieceTable(arrBuffer);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Unknown block type.");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private ushort ReadUInt16(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            byte[] arrShortBuffer = new byte[Constants.BytesInWord];
            int iReadCount = stream.Read(arrShortBuffer, 0, Constants.BytesInWord);

            if (iReadCount != Constants.BytesInWord)
            {
                throw new Exception("Unable to read enough data from the stream");
            }

            return BitConverter.ToUInt16(arrShortBuffer, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private uint ReadUInt32(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            byte[] arrShortBuffer = new byte[Constants.BytesInInt];
            int iReadCount = stream.Read(arrShortBuffer, 0, Constants.BytesInInt);

            if (iReadCount != Constants.BytesInInt)
            {
                throw new Exception("Unable to read enough data from the stream");
            }

            return BitConverter.ToUInt32(arrShortBuffer, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void WriteLists(Stream stream)
        {
            if (m_listInfo == null)
                return;

            m_wpFIBData.fcPlcfLst = (int)stream.Position;
            m_wpFIBData.lcbPlcfLst = m_listInfo.WriteLst(stream);

            m_wpFIBData.fcPlfLfo = (int)stream.Position;
            m_wpFIBData.lcbPlfLfo = m_listInfo.WriteLfo(stream);

            m_wpFIBData.fcSttbListNames = (int)stream.Position;
            m_wpFIBData.lcbSttbListNames += (Constants.BytesInWord +
              m_listInfo.WriteStringTable(stream));
        }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="ilfo"></param>
        //    private void InitListRecord(int ilfo)
        //    {
        //      int i = m_listRecord.Add(new ListDataRecord());
        //
        //      if (i < 0)
        //      {
        //        throw new ArgumentOutOfRangeException(" ilfo index ");
        //      }
        //
        //      ((ListDataRecord)m_listRecord[ilfo - 1]).ListLevels = new ListLevelRecord[9];
        //
        //      for (int j = 0; j < 9; j++)
        //      {
        //        ((ListDataRecord)m_listRecord[ilfo - 1]).ListLevels[j] = new ListLevelRecord();
        //        ((ListDataRecord)m_listRecord[ilfo - 1]).ListLevels[j].Papx =
        //          new SinglePropertyModifierArray();
        //        ((ListDataRecord)m_listRecord[ilfo - 1]).ListLevels[j].Chpx =
        //          new SinglePropertyModifierArray();
        //      }
        //      m_listOverride.Add(new ListFormatOverrideRecord());
        //      ((ListDataRecord)m_listRecord[ilfo - 1]).ListId = ilfo;
        //      ((ListFormatOverrideRecord)m_listOverride[ilfo - 1]).ListId = ilfo;
        //    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadLists(WPFIBData fib, Stream stream)
        {
            if (fib.lcbPlcfLst != 0 && fib.lcbPlfLfo != 0)
            {
                m_listInfo = new ListInfo(fib, stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        private void WriteDocumentProperties(Stream stream, WPFIBData fib)
        {
            fib.fcDop = (int)stream.Position;
            fib.lcbDop = m_dopDescriptor.Write(stream);

            //      FileStream file = new FileStream("D:\\dop.dat", FileMode.Open);
            //      byte[] arr = new byte[file.Length];
            //      file.Read(arr, 0, arr.Length);
            //      file.Close();
            //
            //      stream.Write(arr, 0, arr.Length);
            //      fib.lcbDop = arr.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadDocumentProperties(WPFIBData fib, Stream stream)
        {
            stream.Position = fib.fcDop;
            int length = fib.lcbDop;
            m_dopDescriptor = new DOPDescriptor(stream, fib.fcDop, length, fib.IsDocumentTemplate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        private void WriteFields(Stream stream, WPFIBData fib)
        {
            if (m_fields == null)
                return;

            int count = m_pieceTable.EntriesCount;
            int endCharacter = (byte)(m_pieceTable.FileCharacterPos[count] + 1);
            m_fields.Write(stream, fib, endCharacter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadFields(WPFIBData fib, Stream stream)
        {
            m_fields = new Fields(fib, new BinaryReader(stream));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        private void WriteBookmarks(Stream stream, WPFIBData fib)
        {
            if (m_bkmkStringTable == null)
                return;

            if (m_bkmkStringTable.BookmarkNamesLength == 0)
                return;

            // Writing bookmark names
            m_bkmkStringTable.Save(stream, fib);

            // Selecting end position of text document
            int count = m_pieceTable.EntriesCount;
            int endCharacter = (int)(m_pieceTable.FileCharacterPos[count] + 2);

            // Writing bookmarks positions
            m_bkmkDescriptor.Save(stream, fib, endCharacter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadBookmarks(WPFIBData fib, Stream stream)
        {
            // Create bookmark table
            //Trace.WriteLine("sttbBkmk table reading...");
            stream.Position = fib.fcSttbfbkmk;
            int length = fib.lcbSttbfbkmk;
            int bkfPos = fib.fcPlcfbkf;
            int bkfLength = fib.lcbPlcfbkf;

            int bklPos = fib.fcPlcfbkl;
            int bklLength = fib.lcbPlcfbkl;

            if (length > 0 && bkfLength > 0 && bklLength > 0)
            {
                m_bkmkStringTable = new BookmarkNameStringTable(stream, length);

                m_bkmkDescriptor =
                  new BookmarkDescriptor(
                  stream, m_bkmkStringTable.BookmarkCount, bkfPos, bkfLength, bklPos, bklLength);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ParsePieceTableEncodings()
        {
            m_pieceTablePositions.Clear();
            m_pieceTableEncodings.Clear();

            //m_pieceTablePositions.Add((uint)ConvertCharPosToFileCharPos(0));

            for (int i = 0, end = m_pieceTable.EntriesCount; i < end; i++)
            {
                // If specified pos contains in pieceTable positions
                // get it.
                bool bIsUnicode;
                uint resFileCharPos = NormalizeFileCharPos(m_pieceTable.Entries[i].FileOffset, out bIsUnicode);

                m_pieceTablePositions.Add(resFileCharPos);
#if !SILVERLIGHT && !WP
                m_pieceTableEncodings.Add(bIsUnicode ? Encoding.Unicode : Encoding.GetEncoding(0x4e4));
#else
                m_pieceTableEncodings.Add( bIsUnicode ? Encoding.Unicode : Encoding.UTF8 );
#endif
            }

            m_pieceTablePositions.Add(
              ConvertCharPosToFileCharPos(m_pieceTable.FileCharacterPos[m_pieceTable.EntriesCount]));
            //m_pieceTablePositions.Add((uint)0);
            //      for (int i = 0, end = m_pieceTable.FileCharacterPos.Length; i < end; i++)
            //      {
            //        // If specified pos contains in pieceTable positions
            //        // get it.
            //        uint resFileCharPos = ConvertCharPosToFileCharPos(m_pieceTable.FileCharacterPos[i]);
            //          
            //        m_pieceTablePositions.Add(resFileCharPos);
            //        m_pieceTableEncodings.Add(m_wpFIBData.Encoding);
            //      }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadArtObjects(WPFIBData fib, Stream stream)
        {
            if (ContainShapes(fib))
            {
                m_artObjects = new ArtObjectsRW(fib, stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadAnnotations(WPFIBData fib, Stream stream)
        {
            if (fib.lcbPlcfandTxt > 0 || fib.lcbPlcfandRef > 0)
            {
                m_anotations = new AnnotationsRW(stream, fib);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadFootnotes(WPFIBData fib, Stream stream)
        {
            if (fib.lcbPlcffndTxt > 0 || fib.lcbPlcffndRef > 0)
            {
                m_footnotes = new FootnotesRW(stream, fib);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadEndnotes(WPFIBData fib, Stream stream)
        {
            if (fib.lcbPlcfendTxt > 0 || fib.lcbPlcfendRef > 0)
            {
                m_endnotes = new EndnotesRW(stream, fib);
            }
        }

        /// <summary>
        /// Writes the art objects.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fib">The fib.</param>
        private void WriteArtObjects(Stream stream, WPFIBData fib)
        {
            MsofbtDgContainer mainDocContainer = m_escher.FindDgContainerForSubDocType(ShapeDocType.Main);
            MsofbtDgContainer hfDocContainer = m_escher.FindDgContainerForSubDocType(ShapeDocType.HeaderFooter);

            if (mainDocContainer == null && hfDocContainer == null)
            {
                return;
            }

            if (m_artObjects != null)
            {
                if (m_artObjects.MainDocFSPAs != null)
                {
                    //IDictionaryEnumerator enumerator = m_artObjects.MainDocFSPAs.GetEnumerator();
                    ContainerCollection collection = mainDocContainer.PatriarchGroupContainer.Children;
                    //          SynchronizeSpids(collection, enumerator);
                }

                if (m_artObjects.HfDocFSPAs != null)
                {
                    //IDictionaryEnumerator enumerator = m_artObjects.HfDocFSPAs.GetEnumerator();
                    ContainerCollection collection = hfDocContainer.PatriarchGroupContainer.Children;
                    //          SynchronizeSpids(collection, enumerator);
                }

                int count = m_pieceTable.EntriesCount;
                int endHeader = fib.ccpHdr + 2;
                int endCharacter = (int)(m_pieceTable.FileCharacterPos[count]);

                m_artObjects.Write(stream, fib, endCharacter, endHeader);
            }

            fib.fcDggInfo = (int)stream.Position;
            fib.lcbDggInfo = (int)m_escher.WriteContainers(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void WriteAnnotations(WPFIBData fib, Stream stream)
        {
            if (m_anotations == null)
                return;

            m_anotations.Write(stream, fib);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void WriteFootnotes(WPFIBData fib, Stream stream)
        {
            if (m_footnotes == null)
                return;

            m_footnotes.InitialDescriptorNumber = DOP.InitialFootnoteNumber;
            m_footnotes.Write(stream, fib);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void WriteEndnotes(WPFIBData fib, Stream stream)
        {
            if (m_endnotes == null)
                return;

            m_endnotes.InitialDescriptorNumber = DOP.InitialEndnoteNumber;
            m_endnotes.Write(stream, fib);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="mainEnumerator"></param>
        private static void SynchronizeSpids(ContainerCollection collection, IDictionaryEnumerator mainEnumerator)
        {
            foreach (BaseEscherRecord container in collection)
            {
                MsofbtSpContainer spContainer = container as MsofbtSpContainer;
                if (spContainer != null && spContainer.Shape.ShapeType == EscherShapeType.msosptPictureFrame)
                {
                    //            (m_fspas[i] as FileShapeAddress).m_spid = spContainer.Shape.ShapeId;
                    //            i++;
                    mainEnumerator.MoveNext();
                    (mainEnumerator.Value as FileShapeAddress).Spid = spContainer.Shape.ShapeId;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadMacroCommands(WPFIBData fib, Stream stream)
        {
            stream.Position = fib.fcCmds;
            m_macroCommands = new byte[fib.lcbCmds];
            stream.Read(m_macroCommands, 0, m_macroCommands.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void WriteMacroCommands(WPFIBData fib, Stream stream)
        {
            if (m_macroCommands != null)
            {
                fib.fcCmds = (int)stream.Position;
                stream.Write(m_macroCommands, 0, m_macroCommands.Length);
                fib.lcbCmds = m_macroCommands.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void ReadGrammarSpellingData(WPFIBData fib, Stream stream)
        {
            m_grammarSpellingTablesData = new GrammarSpelling(fib, stream, m_charPosTableHF);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void WriteGrammarSpellingData(WPFIBData fib, Stream stream)
        {
            if (m_grammarSpellingTablesData != null)
            {
                m_grammarSpellingTablesData.Write(fib, stream);
            }
        }

        /// <summary>
        /// Contains the shapes.
        /// </summary>
        /// <param name="fib">The fib.</param>
        /// <returns></returns>
        private bool ContainShapes(WPFIBData fib)
        {
            bool containShapes = false;

            if (fib.lcbPlcspaMom > 0 || fib.lcbPlcspaHdr > 0 ||
              fib.lcbPlcftxbxTxt > 0 || fib.lcbPlcfHdrtxbxTxt > 0 ||
              fib.lcbPlcftxbxBkd > 0 || fib.lcbPlcfHdrtxbxBkd > 0)
            {
                containShapes = true;
            }

            return containShapes;
        }

        /// <summary>
        /// Reads the variables.
        /// </summary>
        /// <param name="fib">The fib.</param>
        /// <param name="stream">The stream.</param>
        private void ReadVariables(WPFIBData fib, Stream stream)
        {
            if (fib.lcbStwUser > 0)
            {
                stream.Position = fib.fcStwUser;
                m_variables = new byte[fib.lcbStwUser];
                stream.Read(m_variables, 0, m_variables.Length);
            }
        }

        /// <summary>
        /// Writes the variables.
        /// </summary>
        /// <param name="fib">The fib.</param>
        /// <param name="stream">The stream.</param>
        private void WriteVariables(WPFIBData fib, Stream stream)
        {
            if (m_variables != null)
            {
                fib.fcStwUser = (int)stream.Position;
                stream.Write(m_variables, 0, m_variables.Length);
                fib.lcbStwUser = m_variables.Length;
            }
        }

        /// <summary>
        /// Reads the table of associated strings.
        /// </summary>
        /// <param name="fib">The fib.</param>
        /// <param name="stream">The stream.</param>
        private void ReadAssocStrings(WPFIBData fib, Stream stream)
        {
            if (fib.lcbSttbfAssoc > 0)
            {
                stream.Position = fib.fcSttbfAssoc;
                m_assocStrings = new byte[fib.lcbSttbfAssoc];
                stream.Read(m_assocStrings, 0, m_assocStrings.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        private void WriteAssocStrings(WPFIBData fib, Stream stream)
        {
            if (m_assocStrings != null)
            {
                fib.fcSttbfAssoc = (int)stream.Position;
                stream.Write(m_assocStrings, 0, m_assocStrings.Length);
                fib.lcbSttbfAssoc = m_assocStrings.Length;
            }
        }
        #endregion
    }
}
