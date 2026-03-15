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

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.Documentation;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for WordFKPData.
    /// </summary>
    [CLSCompliant(false)]
    [DocumentationExclude()]
    internal class WordFKPData
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_COUNTRUN_SIZE = 1;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private WPFIBData m_fib;

        private WPTablesData m_tables;

        /// <summary>
        /// Fills objects by write operations.
        /// </summary>
        private List<UInt32> m_papxPositions = new List<UInt32>();

        private List<ParagraphExceptionInDiskPage> m_papxProps = new List<ParagraphExceptionInDiskPage>();
        private List<UInt32> m_chpxPositions = new List<UInt32>();
        private List<CharacterPropertyException> m_chpxProps = new List<CharacterPropertyException>();
        private List<Int32> m_sepxPositions = new List<Int32>();
        private List<SectionPropertyException> m_sepxProps = new List<SectionPropertyException>();

        /// <summary>
        /// Fills objects by read operation. 
        /// </summary>
        private FKPStructure[] m_chpxFKPs = new FKPStructure[0];

        private FKPStructure[] m_papxFKPs = new FKPStructure[0];
        private ParagraphPropertiesPage[] m_papxPages;
        private CharacterPropertiesPage[] m_chpxPages;
        private SectionPropertyException[] m_secProperties;
        private long m_lastSepxPosition = 0;
        ////    private MemoryStream m_hugePapxStream = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Member initializing constructor
        /// </summary>
        internal WordFKPData(WPFIBData fib, WPTablesData tables)
        {
            m_fib = fib;
            m_tables = tables;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets FIB of word file
        /// </summary>
        internal WPFIBData FIBData
        {
            get
            {
                return m_fib;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal long EndOfSepx
        {
            get
            {
                return m_lastSepxPosition;
            }
        }

        /*/// <summary>
        /// Gets current page with paragraph properties.
        /// </summary>
        internal ParagraphPropertiesPage CurrentPapxPage
        {
          get
          {
            return GetPapxPage( m_iCurrentPapxFKPIndex );
          }
        }
        /// <summary>
        /// Gets current page with character properties.
        /// </summary>
        internal CharacterPropertiesPage CurrentChpxPage
        {
          get
          {
            return GetChpxPage( m_iCurrentChpxFKPIndex );
          }
        }
        /// <summary>
        ///  Gets current SectionPropertyException
        /// </summary>
        internal SectionPropertyException CurrentSepx
        {
          get
          {
            return m_secProperties[ m_iCurrentSepxIndex ];
          }
        }
        /// <summary>
        ///  Gets current CharacterPropertyException
        /// </summary>
        internal CharacterPropertyException CurrentChpx
        {
          get
          {
            return CurrentChpxPage.CharacterProperties[ m_iCurrentChpxIndex ];
          }
        }
        /// <summary>
        ///  Gets current ParagraphPropertyException
        /// </summary>
        internal ParagraphPropertyException CurrentPapx
        {
          get
          {
            return CurrentPapxPage.ParagraphProperties[ m_iCurrentPapxIndex ];
          }
        }*/

        /// <summary>
        /// Gets number of added SectionPropertyExceptions
        /// </summary>
        internal int SepxAddedCount
        {
            get
            {
                return m_sepxProps.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal WPTablesData Tables
        {
            get
            {
                return m_tables;
            }
        }

        ////    /// <summary>
        ////    ///
        ////    /// </summary>
        ////    internal MemoryStream HugePapxStream
        ////    {
        ////      get
        ////      {
        ////        return m_hugePapxStream;
        ////      }
        ////    }
        #endregion

        #region Class internal methods
        /// <summary>
        ///  Gets SectionPropertyException
        /// </summary>
        internal SectionPropertyException GetSepx(int index)
        {
            return m_secProperties[index];
        }

        /// <summary>
        /// Add character properties exception
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="chpx"></param>
        internal void AddChpxProperties(uint pos, CharacterPropertyException chpx)
        {
            if (pos < 0)
                throw new ArgumentOutOfRangeException("pos");

            if (chpx == null)
                chpx = new CharacterPropertyException();

            m_chpxPositions.Add(pos);
            m_chpxProps.Add(chpx);
        }

        /// <summary>
        /// Add paragraph properties exception
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="papx"></param>
        /// <param name="dataStream"></param>
        internal void AddPapxProperties(uint pos, ParagraphExceptionInDiskPage papx, MemoryStream dataStream)
        {
            if (pos < 0)
                throw new ArgumentOutOfRangeException("pos");

            if (papx == null)
                papx = new ParagraphExceptionInDiskPage();

            m_papxPositions.Add(pos);

            if (papx.Length < 485)
            {
                m_papxProps.Add(papx);
            }
            else
            {
                int m_dataStreamPos = (int)dataStream.Position;
                short length = (short)papx.PropertyModifiers.Length;
                byte[] bytesSize = BitConverter.GetBytes(length);
                dataStream.Write(bytesSize, 0, bytesSize.Length);

                papx.SaveHugePapx(dataStream);

                papx = new ParagraphExceptionInDiskPage();
                papx.PropertyModifiers.SetValue(WordSprmOptions.sprmPHugePapx2, m_dataStreamPos);
                m_papxProps.Add(papx);
            }
        }

        /// <summary>
        /// Add section properties exception 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="sepx"></param>
        internal void AddSepxProperties(int pos, SectionPropertyException sepx)
        {
            m_sepxPositions.Add(pos);
            m_sepxProps.Add(sepx);
        }

        /// <summary>
        /// Reads character/paragraph/section properties from stream
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(MemoryStream stream)
        {
            m_chpxFKPs = ReadFKPs(stream, m_tables.CHPXBinaryTable);
            m_chpxPages = new CharacterPropertiesPage[m_chpxFKPs.Length];

            m_papxFKPs = ReadFKPs(stream, m_tables.PAPXBinaryTable);
            m_papxPages = new ParagraphPropertiesPage[m_papxFKPs.Length];

            SectionDescriptor[] arrDescriptors = m_tables.SectionsTable.Descriptors;
            m_secProperties = new SectionPropertyException[arrDescriptors.Length];

            for (int i = 0, end = arrDescriptors.Length; i < end; i++)
            {
                uint iPos = arrDescriptors[i].SepxPosition;
                if (iPos != 0xFFFFFFFF)
                {
                    stream.Position = iPos;
                    m_secProperties[i] = new SectionPropertyException(stream);
                }
                else
                    m_secProperties[i] = new SectionPropertyException(true);
            }

            m_lastSepxPosition = stream.Position;
        }

        /// <summary>
        /// Writes character/paragraph/section properties exceptions to stream
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            WriteChpx(stream);
            WritePapx(stream);
            WriteSepx(stream);
        }

        /// <summary>
        /// Gets i-th page page with paragraph properties exception.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        internal ParagraphPropertiesPage GetPapxPage(int i)
        {
            if (m_papxPages[i] == null)
            {
                m_papxPages[i] = new ParagraphPropertiesPage(m_papxFKPs[i]);
            }

            return m_papxPages[i];
        }

        /// <summary>
        /// Gets i-th page page with character properties exception.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        internal CharacterPropertiesPage GetChpxPage(int i)
        {
            if (m_chpxPages[i] == null)
            {
                m_chpxPages[i] = new CharacterPropertiesPage(m_chpxFKPs[i]);
            }

            return m_chpxPages[i];
        }

        /// <summary>
        /// Rewrite last Papx to the document
        /// </summary>
        internal void CloneAndAddLastPapx(uint pos)
        {
            int index = m_papxPositions.IndexOf(pos);
            ParagraphExceptionInDiskPage prop = m_papxProps[index];
            //// Rewrite last papx
            m_papxProps[m_papxProps.Count - 1] = prop;
        }

        /// <summary>
        /// Rewrite last chpx to the document.
        /// </summary>
        /// <param name="pos"></param>
        internal void CloneAndAddLastChpx(uint pos)
        {
            int index = m_chpxPositions.IndexOf(pos);
            CharacterPropertyException prop = m_chpxProps[index];
            //// Rewrite last chpx
            m_chpxProps[m_chpxProps.Count - 1] = prop;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Reads array of FKPs from stream according to BinaryTable
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        internal FKPStructure[] ReadFKPs(MemoryStream stream, BinaryTable table)
        {
            int iEntrCount = table.Entries.Length;
            FKPStructure[] retFKPs = new FKPStructure[iEntrCount];

            for (int i = 0; i < iEntrCount; i++)
            {
                int iCurrentPage = table.Entries[i].Value;
                stream.Position = iCurrentPage * Constants.DiskPageSize;
                retFKPs[i] = new FKPStructure(stream);
            }

            return retFKPs;
        }

        /// <summary>
        /// Write all papxs to stream 
        /// </summary>
        /// <param name="stream"></param>
        private void WritePapx(Stream stream)
        {
            int papxCount = m_papxPositions.Count;
            int papxIndex = 0;
            uint pagePos = (uint)m_fib.fcMin;
            BinaryWriter writer = new BinaryWriter(stream);

            // SaveHugePapx all papx
            while (papxIndex < papxCount)
            {
                ParagraphPropertiesPage page = new ParagraphPropertiesPage();
                papxIndex = FillPapxPage(page, pagePos, papxIndex);

                int iPapxPage = AlignByDiskPage(stream);
                m_tables.AddPapxRecord(pagePos, iPapxPage);
                ////page.Save( stream, m_memConverter );
                page.SaveToStream(writer, stream);

                pagePos = m_papxPositions[papxIndex - 1];
            }
        }

        /// <summary>
        /// Fill papx page with sprms
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagePos"></param>
        /// <param name="papxIndex"></param>
        /// <returns></returns>
        private int FillPapxPage(ParagraphPropertiesPage page, uint pagePos, int papxIndex)
        {
            page.RunsCount = GetPapxCountPerPage(papxIndex);
            if (page.RunsCount == 0)
            {
                throw new Exception(string.Empty);
            }

            page.FileCharPos[0] = pagePos;

            for (int i = 0, len = page.RunsCount; i < len; i++)
            {
                page.ParagraphProperties[i] = m_papxProps[papxIndex];

                page.FileCharPos[i + 1] = m_papxPositions[papxIndex];
                papxIndex++;
            }

            return papxIndex;
        }

        /// <summary>
        /// Gets count of sprms per one page
        /// </summary>
        /// <param name="papxIndex"></param>
        /// <returns></returns>
        private int GetPapxCountPerPage(int papxIndex)
        {
            int i, sumPapxSize = 0;
            int papxCount = m_papxPositions.Count;

            for (i = papxIndex; i < papxCount; i++)
            {
                ParagraphExceptionInDiskPage papx = m_papxProps[i];

                if ((papx.Length + BXStructure.DEF_RECORD_SIZE +
                  Constants.FileCharPosSize * 2 + sumPapxSize) >
                  Constants.DiskPageSize - DEF_COUNTRUN_SIZE)
                {
                    break;
                }

                sumPapxSize += papx.Length + BXStructure.DEF_RECORD_SIZE + Constants.FileCharPosSize;
            }

            return i - papxIndex;
        }

        /// <summary>
        /// Write all chpxs to stream
        /// </summary>
        /// <param name="stream"></param>
        private void WriteChpx(Stream stream)
        {
            int chpxCount = m_chpxPositions.Count;
            int chpxIndex = 0;
            uint pagePos = (uint)m_fib.fcMin;
            BinaryWriter writer = new BinaryWriter(stream);

            while (chpxIndex < chpxCount)
            {
                ////Trace.Write( ">>>>>>>>>>>>> Fill chpx page : " );
                CharacterPropertiesPage page = new CharacterPropertiesPage();
                chpxIndex = FillChpxPage(page, pagePos, chpxIndex);

                int iChpxPage = AlignByDiskPage(stream);
                m_tables.AddChpxRecord(pagePos, iChpxPage);

                ////Trace.Write( ">>>>>>>>>>>>> Save chpx page : " );

                page.SaveToStream(writer, stream);
                pagePos = m_chpxPositions[chpxIndex - 1];
            }
        }

        /// <summary>
        /// Fill chpx page with sprms
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagePos"></param>
        /// <param name="chpxIndex"></param>
        /// <returns></returns>
        private int FillChpxPage(CharacterPropertiesPage page, uint pagePos, int chpxIndex)
        {
            int i, len;
            page.RunsCount = GetChpxCountPerPage(chpxIndex);
            page.FileCharPos[0] = pagePos;

            for (i = 0, len = page.RunsCount; i < len; i++)
            {
                page.CharacterProperties[i] = m_chpxProps[chpxIndex];

                page.FileCharPos[i + 1] = m_chpxPositions[chpxIndex];
                chpxIndex++;
            }

            return chpxIndex;
        }

        /// <summary>
        /// Gets count of sprms per one page
        /// </summary>
        /// <param name="chpxIndex"></param>
        /// <returns></returns>
        private int GetChpxCountPerPage(int chpxIndex)
        {
            int i, sum = 0, Length;
            int chpxCount = m_chpxPositions.Count;
            for (i = chpxIndex; i < chpxCount; i++)
            {
                CharacterPropertyException chpx = m_chpxProps[i];
                Length = (chpx.Length % 2 != 0) ? chpx.Length + 1 : chpx.Length;
                if (IsChpxRepeats(chpxIndex,i))
                {
                    if ((sum + Constants.FileCharPosSize * 2 + 1) >=
                    Constants.DiskPageSize - DEF_COUNTRUN_SIZE)
                        break;
                    sum += Constants.FileCharPosSize + 1;
                }
                else
                {
                    if ((sum + Length + Constants.FileCharPosSize * 2 + 1) >=
                      Constants.DiskPageSize - DEF_COUNTRUN_SIZE)
                        break;
                    sum += Length + Constants.FileCharPosSize + 1;
                }
                 // 1 - length of offset record
            }

            ////Trace.WriteLine(sum - (i - chpxIndex) * (Constants.FileCharPosSize + 1), "Chpx size in page");
            return i - chpxIndex;
        }
        /// <summary>
        /// Check whether Chpx repeats within the specified ranges (chpxIndex to CurrentIndex)
        /// </summary>
        /// <param name="chpxIndex">Chpx Index</param>
        /// <param name="CurrentIndex">Current Index</param>
        /// <returns></returns>
        internal bool IsChpxRepeats(int chpxIndex,int CurrentIndex)
        {
            CharacterPropertyException currentChpx = m_chpxProps[CurrentIndex];
            CharacterPropertyException prevChpx;
            bool ischpxRepeats = false;
            for (int i = chpxIndex; i < CurrentIndex; i++)
            {
                prevChpx = m_chpxProps[i];
                if (currentChpx.Length == prevChpx.Length && currentChpx.ModifiersCount == prevChpx.ModifiersCount)
                {
                    if (ischpxRepeats = currentChpx.Equals(prevChpx))
                        break;
                }
            }
            return ischpxRepeats;
        }
        /// <summary>
        /// Write all sepxs to stream
        /// </summary>
        /// <param name="stream"></param>
        private void WriteSepx(Stream stream)
        {
            m_secProperties = new SectionPropertyException[m_sepxPositions.Count];

            for (int i = 0, len = m_sepxPositions.Count; i < len; i++)
            {
                int pos = m_sepxPositions[i];
                SectionPropertyException sx = m_sepxProps[i];
                int sepxPos = AlignByDiskPage(stream);

                m_tables.AddSectionRecord(pos, sepxPos);
                m_secProperties[i] = sx;
                sx.Save(stream);
            }
        }

        /// <summary>
        /// Aligns position in stream to the end of the page
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private int AlignByDiskPage(Stream stream)
        {
            int pos = (int)stream.Position;
            int pageNum = pos / Constants.DiskPageSize;

            // Calculates page number
            if (pos % Constants.DiskPageSize != 0)
            {
                pageNum++;
            }

            // Calculates new position in stream
            pos = Constants.DiskPageSize * pageNum;

            // Moves stream positions and fills zero data
            while (stream.Position < pos)
            {
                ////stream.WriteByte( 0 );  
                byte[] tmp = new byte[pos - stream.Position];
                stream.Write(tmp, 0, (int)(pos - stream.Position));
            }

            return pageNum;
        }
        #endregion
    }
}