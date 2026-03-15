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

using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for Shapes.
    /// </summary>
    [CLSCompliant(false)]
    internal class ArtObjectsRW : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Table stream
        /// </summary>
        private Stream m_stream;
        /// <summary>
        /// Sorted list with FileShapeAddress structures
        /// </summary>
        private DocIOSortedList<WordSubdocument, DocIOSortedList<int, FileShapeAddress>> m_fspas;
        /// <summary>
        /// Sorted list with TexBoxStory structures
        /// </summary>
        private DocIOSortedList<WordSubdocument, DocIOSortedList<int, TextBoxStoryDescriptor>> m_txbxs;
        /// <summary>
        /// Sorted list with textbox BreakDescriptors
        /// </summary>
        private DocIOSortedList<WordSubdocument, DocIOSortedList<int, BreakDescriptor>> m_txbxBkds;
        /// <summary>
        /// Last text position for textbox in Main
        /// </summary>
        private int m_txBxMainEndPos;
        /// <summary>
        /// Last text position for textbox in Header
        /// </summary>
        private int m_txBxHeaderEndPos;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FileShapeAddress> MainDocFSPAs
        {
            get
            {
                if (m_fspas.ContainsKey(WordSubdocument.Main))
                {
                    return m_fspas[WordSubdocument.Main];
                }

                return null;
            }
        }
        /// <summary>
        /// Sorted list of  textboxes
        /// </summary>
        internal DocIOSortedList<int, TextBoxStoryDescriptor> MainDocTxBxs
        {
            get
            {
                if (m_txbxs.ContainsKey(WordSubdocument.Main))
                {
                    return m_txbxs[WordSubdocument.Main];
                }
                return null;
            }
        }
        /// <summary>
        /// Sorted list of textbox descriptors
        /// </summary>
        internal DocIOSortedList<int, BreakDescriptor> MainDocTxBxBKDs
        {
            get
            {
                if (m_txbxBkds.ContainsKey(WordSubdocument.Main))
                {
                    return m_txbxBkds[WordSubdocument.Main];
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FileShapeAddress> HfDocFSPAs
        {
            get
            {
                if (m_fspas.ContainsKey(WordSubdocument.HeaderFooter))
                {
                    return m_fspas[WordSubdocument.HeaderFooter];
                }

                return null;
            }
        }
        /// <summary>
        /// Sorted list of header/footer's textboxes
        /// </summary>
        internal DocIOSortedList<int, TextBoxStoryDescriptor> HfDocTxBxs
        {
            get
            {
                if (m_txbxs.ContainsKey(WordSubdocument.HeaderFooter))
                {
                    return m_txbxs[WordSubdocument.HeaderFooter];
                }
                return null;
            }
        }
        /// <summary>
        /// Sorted list of header/footer's textboxe descriptors
        /// </summary>
        internal DocIOSortedList<int, BreakDescriptor> HfDocTxBxBKDs
        {
            get
            {
                if (m_txbxBkds.ContainsKey(WordSubdocument.HeaderFooter))
                {
                    return m_txbxBkds[WordSubdocument.HeaderFooter];
                }
                return null;
            }
        }
        /// <summary>
        /// Get count of the structures
        /// </summary>
        internal int StructsCount
        {
            get
            {
                int count = (MainDocFSPAs == null) ? 0 : MainDocFSPAs.Count;
                count += (HfDocFSPAs == null) ? 0 : HfDocFSPAs.Count;
                count += (HfDocTxBxs == null) ? 0 : HfDocTxBxs.Count;
                count += (MainDocTxBxs == null) ? 0 : MainDocTxBxs.Count;

                return count;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        internal ArtObjectsRW(WPFIBData fib, Stream stream)
            : this()
        {
            Read(stream, fib);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtObjectsRW"/> class.
        /// </summary>
        internal ArtObjectsRW()
        {
            m_fspas = new DocIOSortedList<WordSubdocument, DocIOSortedList<int, FileShapeAddress>>();
            m_txbxBkds = new DocIOSortedList<WordSubdocument, DocIOSortedList<int, BreakDescriptor>>();
            m_txbxs = new DocIOSortedList<WordSubdocument, DocIOSortedList<int, TextBoxStoryDescriptor>>();
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fspa"></param>
        /// <param name="docType"></param>
        /// <param name="pos"></param>
        internal void AddFSPA(FileShapeAddress fspa, WordSubdocument docType, int pos)
        {
            if (!m_fspas.ContainsKey(docType))
            {
                DocIOSortedList<int, FileShapeAddress> hashTable = new DocIOSortedList<int, FileShapeAddress>();
                m_fspas.Add(docType, hashTable);
            }
            m_fspas[docType].Add(pos, fspa);
        }
        /// <summary>
        /// Initialize MainDocTxBxs and MainDocTxBxBKDs, or HfDocTxBxBKDs and HfDocTxBxs
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="txbxStoryDesc"></param>
        /// <param name="txbxBKDesc"></param>
        /// <param name="pos"></param>
        internal void AddTxbx(WordSubdocument docType, TextBoxStoryDescriptor txbxStoryDesc, BreakDescriptor txbxBKDesc, int pos)
        {
            //Add TextBoxStoryDescriptor
            if (!m_txbxs.ContainsKey(docType))
            {
                DocIOSortedList<int, TextBoxStoryDescriptor> hashTable = new DocIOSortedList<int, TextBoxStoryDescriptor>();
                m_txbxs.Add(docType, hashTable);
            }
            m_txbxs[docType].Add(pos, txbxStoryDesc);
            //Add textbox Breakdescriptor
            if (!m_txbxBkds.ContainsKey(docType))
            {
                DocIOSortedList<int, BreakDescriptor> hashTable = new DocIOSortedList<int, BreakDescriptor>();
                m_txbxBkds.Add(docType, hashTable);
            }
            m_txbxBkds[docType].Add(pos, txbxBKDesc);
            if (docType == WordSubdocument.Main) m_txBxMainEndPos = pos + 3;
            if (docType == WordSubdocument.HeaderFooter) m_txBxHeaderEndPos = pos + 3;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        internal void Read(Stream stream, WPFIBData fib)
        {
            m_stream = stream;

            ReadShapeFSPA(WordSubdocument.Main, fib.fcPlcspaMom, (int)fib.lcbPlcspaMom);
            ReadShapeFSPA(WordSubdocument.HeaderFooter, fib.fcPlcspaHdr, (int)fib.lcbPlcspaHdr);
            ReadTxbx(WordSubdocument.Main, fib.fcPlcftxbxTxt, (int)fib.lcbPlcftxbxTxt);
            ReadTxbx(WordSubdocument.HeaderFooter, fib.fcPlcfHdrtxbxTxt, (int)fib.lcbPlcfHdrtxbxTxt);
            ReadTxbxBkd(WordSubdocument.Main, fib.fcPlcftxbxBkd, (int)fib.lcbPlcftxbxBkd);
            ReadTxbxBkd(WordSubdocument.HeaderFooter, fib.fcPlcfHdrtxbxBkd, (int)fib.lcbPlcfHdrtxbxBkd);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        /// <param name="endMain"></param>
        /// <param name="endHeader"></param>
        internal void Write(Stream stream, WPFIBData fib, int endMain, int endHeader)
        {
            m_stream = stream;
            WriteFSPAs(fib, endMain, endHeader);
            WriteTxBxs(fib, endMain);
            WriteTxBxBKDs(fib);
        }
        /// <summary>
        /// Get textbox text positions (needed in Reader)
        /// </summary>
        /// <param name="isHdrTxbx"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal int GetTxbxPosition(bool isHdrTxbx, int index)
        {
            if (isHdrTxbx && HfDocTxBxs != null)
            {
                return (index == HfDocTxBxs.Count) ? (GetKey(HfDocTxBxs,index - 1)) + 3 :
                                                       GetKey(HfDocTxBxs,index);
            }
            else
            {
                if (MainDocTxBxs != null)
                {
                    return (index == MainDocTxBxs.Count) ? (GetKey(MainDocTxBxs,index - 1)) + 3 :
                                                             GetKey(MainDocTxBxs,index);
                }
            }
            return 0;
        }
        /// <summary>
        /// Get ShapeObject's identificator from TextboxStoryDescriptor collection.  
        /// </summary>
        /// <param name="subDocType">Subdocument type.</param>
        /// <param name="txbxIndex">ShapeObject entry.</param>
        /// <returns></returns>
        internal int GetShapeObjectId(WordSubdocument subDocType, int txbxIndex)
        {
            TextBoxStoryDescriptor txbxStory;
            if (subDocType == WordSubdocument.TextBox)
            {
                txbxStory = GetByIndex(MainDocTxBxs, txbxIndex);
            }
            else
            {
                txbxStory = GetByIndex(HfDocTxBxs, txbxIndex);
            }

            return txbxStory.ShapeIdent;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="CP"></param>
        /// <returns></returns>
        internal FileShapeAddress FindFileShape(WordSubdocument docType, int CP)
        {
            DocIOSortedList<int, FileShapeAddress> list = m_fspas[docType];
            FileShapeAddress fspa = list != null ? list[CP] : null;

            //#if DEBUG
            //      if( fspa == null )
            //      {
            //        throw new ArgumentException( "Cannot find file shape address for the specified CP." );
            //      }
            //#endif

            return fspa;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="pos"></param>
        /// <param name="length"></param>
        private void ReadShapeFSPA(WordSubdocument docType, int pos, int length)
        {
            m_stream.Position = pos;
            DocIOSortedList<int, FileShapeAddress> tempList = new DocIOSortedList<int, FileShapeAddress>();
            m_fspas[docType] = tempList;

            if (length != 0)
            {
                int[] posArray = GetPositions(FileShapeAddress.DEF_FSPA_LENGTH, length);
                for (int k = 0, count = posArray.Length - 1; k < count; k++)
                {
                    FileShapeAddress fspa = new FileShapeAddress(m_stream);
                    bool istempListHasSameSPID = false;
                    int[] fspaKeyArray = new int[tempList.Count];
                    tempList.Keys.CopyTo(fspaKeyArray, 0);
                    for (int i = 0; i < fspaKeyArray.Length; i++)
                        if (tempList[fspaKeyArray[i]].Spid == fspa.Spid)
                            istempListHasSameSPID = true;
                    if(!istempListHasSameSPID)
                        tempList.Add(posArray[k], fspa);
                }
            }
        }
        /// <summary>
        /// Read TextBoxes
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="pos">table stream position</param>
        /// <param name="length"></param>
        private void ReadTxbx(WordSubdocument docType, int pos, int length)
        {
            if (length > 0)
            {
                DocIOSortedList<int, TextBoxStoryDescriptor> tempList = new DocIOSortedList<int, TextBoxStoryDescriptor>();
                m_txbxs[docType] = tempList;
                m_stream.Position = pos;

                int[] txbxTextPositions = GetPositions(TextBoxStoryDescriptor.DEF_TXBX_LENGTH, length);

                for (int i = 0; i < txbxTextPositions.Length - 1; i++)
                {
                    TextBoxStoryDescriptor txbxStory = new TextBoxStoryDescriptor(m_stream);
                    tempList.Add(txbxTextPositions[i], txbxStory);
                }
            }
        }
        /// <summary>
        /// Read TextBox breakdescriptors
        /// </summary>
        private void ReadTxbxBkd(WordSubdocument docType, int pos, int length)
        {
            if (length > 0)
            {
                DocIOSortedList<int, BreakDescriptor> tempList = new DocIOSortedList<int, BreakDescriptor>();
                m_txbxBkds[docType] = tempList;
                m_stream.Position = pos;
                int[] txbxBKDPositions = GetPositions(BreakDescriptor.DEF_BKD_SIZE, length);
                for (int i = 0; i < txbxBKDPositions.Length - 1; i++)
                {
                    BreakDescriptor txbxBKD = new BreakDescriptor(m_stream);
                    tempList.Add(txbxBKDPositions[i], txbxBKD);
                }
            }
        }
        /// <summary>
        /// Get positions for each type of artobjects
        /// </summary>
        /// <param name="structSize"> structure type </param>
        /// <param name="length"> structure length </param>
        /// <returns></returns>
        private int[] GetPositions(int structSize, int length)
        {
            int posCount = (length - Constants.BytesInInt)
              / (structSize + Constants.BytesInInt) + 1;
            int[] positions = new int[posCount];
            for (int i = 0; i < posCount; i++)
            {
                positions[i] = (int)ReadUInt32(m_stream);
            }
            return positions;
        }
        /// <summary>
        /// Write FSPA
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="endMain"></param>
        /// <param name="endHeader"></param>
        private void WriteFSPAs(WPFIBData fib, int endMain, int endHeader)
        {
            if (MainDocFSPAs != null || HfDocFSPAs != null)
            {
                if (MainDocFSPAs != null)
                {
                    fib.fcPlcspaMom = (int)m_stream.Position;
                    WriteArtObjectsFSPAs(MainDocFSPAs, endMain);
                    fib.lcbPlcspaMom = (int)(m_stream.Position - fib.fcPlcspaMom);
                }
                if (HfDocFSPAs != null)
                {
                    fib.fcPlcspaHdr = (int)m_stream.Position;
                    WriteArtObjectsFSPAs(HfDocFSPAs, endHeader);
                    fib.lcbPlcspaHdr = (int)(m_stream.Position - fib.fcPlcspaHdr);
                }
            }
        }
        /// <summary>
        /// Write textbox
        /// </summary>
        /// <param name="fib">fib data</param>
        /// <param name="endCharacter"></param>
        private void WriteTxBxs(WPFIBData fib, int endCharacter)
        {
            if (MainDocTxBxs != null || HfDocTxBxs != null)
            {
                //Write document data to stream
                if (MainDocTxBxs != null)
                {
                    //Set textbox offset subdocument
                    fib.fcPlcftxbxTxt = (int)m_stream.Position;
                    WriteArtObjectsTxBxs(MainDocTxBxs, endCharacter);
                    fib.lcbPlcftxbxTxt = (int)(m_stream.Position - fib.fcPlcftxbxTxt);
                }
                //Write header/footer data to stream
                if (HfDocTxBxs != null)
                {
                    //Set header textbox offset subdocument
                    fib.fcPlcfHdrtxbxTxt = (int)m_stream.Position;
                    WriteArtObjectsTxBxs(HfDocTxBxs, endCharacter);
                    fib.lcbPlcfHdrtxbxTxt = (int)(m_stream.Position - fib.fcPlcfHdrtxbxTxt);
                }
            }
        }
        /// <summary>
        /// Write break descriptors
        /// </summary>
        /// <param name="fib"></param>
        private void WriteTxBxBKDs(WPFIBData fib)
        {
            if (MainDocTxBxBKDs != null || HfDocTxBxBKDs != null)
            {
                //Write document data to stream
                if (MainDocTxBxBKDs != null)
                {
                    //Set textbox offset subdocument
                    fib.fcPlcftxbxBkd = (int)m_stream.Position;
                    WriteArtObjectsTxBxBKDs(MainDocTxBxBKDs, m_txBxMainEndPos);
                    fib.lcbPlcftxbxBkd = (int)(m_stream.Position - fib.fcPlcftxbxBkd);
                }

                //Write header/footer data to stream
                if (HfDocTxBxBKDs != null)
                {
                    //Set header textbox offset subdocument
                    fib.fcPlcfHdrtxbxBkd = (int)m_stream.Position;
                    WriteArtObjectsTxBxBKDs(HfDocTxBxBKDs, m_txBxHeaderEndPos);
                    fib.lcbPlcfHdrtxbxBkd = (int)(m_stream.Position - fib.fcPlcfHdrtxbxBkd);
                }
            }
        }
        /// <summary>
        /// Write data to stream
        /// </summary>
        /// <param name="stList"></param>
        /// <param name="endPos">End position for FSPA </param>
        private void WriteArtObjectsTxBxBKDs(DocIOSortedList<int, BreakDescriptor> stList, int endPos)
        {
            //Write positions to stream

            foreach (int key in stList.Keys)
            {
                WriteInt32(m_stream, key);
            }
            WriteInt32(m_stream, endPos);
            //Write structures
            foreach (BreakDescriptor value in stList.Values)
            {
                value.Write(m_stream);
            }
        }
        /// <summary>
        /// Write data to stream
        /// </summary>
        /// <param name="stList"></param>
        /// <param name="endPos">End position for FSPA </param>
        private void WriteArtObjectsTxBxs(DocIOSortedList<int, TextBoxStoryDescriptor> stList, int endPos)
        {
            //Write positions to stream

            foreach (int key in stList.Keys)
            {
                WriteInt32(m_stream, key);
            }
            WriteInt32(m_stream, endPos);
            //Write structures
            foreach (TextBoxStoryDescriptor value in stList.Values)
            {
                value.Write(m_stream);
            }
        }
        private void WriteArtObjectsFSPAs( DocIOSortedList<int, FileShapeAddress> stList, int endPos )
        {
            //Write positions to stream

            foreach (int key in stList.Keys)
            {
                WriteInt32(m_stream, key);
            }
            WriteInt32(m_stream, endPos);
            //Write structures
            foreach (FileShapeAddress value in stList.Values)
            {
                value.Write(m_stream);
            }
        }
        /// <summary>
        /// Gets the index of the by.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private TextBoxStoryDescriptor GetByIndex( DocIOSortedList<int, TextBoxStoryDescriptor> col, int index )
        {
            if (index > col.Count - 1 || index < 0)
                return null;

            return col.Values[ index ];
            //int curIndex = 0;
            //foreach (TextBoxStoryDescriptor value in col.Values)
            //{
            //    if (curIndex == index)
            //        return value;
            //    curIndex++;
            //}
            //return null;
        }
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private int GetKey( DocIOSortedList<int, TextBoxStoryDescriptor> col, int index )
        {
            if (index > col.Count - 1 || index < 0)
                return -1;
            //int curIndex = 0;

            //foreach (int key in col.Keys)
            //{
            //    if (curIndex == index)
            //        return key;
            //    curIndex++;
            //}
            return col.Keys[ index ];
        }
        #endregion

        #region Commented
        ///// <summary>
        ///// Write data to stream
        ///// </summary>
        ///// <param name="stList"></param>
        ///// <param name="endPos">End position for FSPA </param>
        //private void WriteArtObjects(SortedList stList, int endPos)
        //{
        //    //Write positions to stream

        //    foreach (DictionaryEntry entry in stList)
        //    {
        //        WriteInt32(m_stream, (int)entry.Key);
        //    }
        //    WriteInt32(m_stream, endPos);
        //    //Write structures
        //    foreach (DictionaryEntry entry in stList)
        //    {
        //        if (entry.Value is FileShapeAddress)
        //            ((FileShapeAddress)entry.Value).Write(m_stream);
        //        else if (entry.Value is TextBoxStoryDescriptor)
        //        {
        //            ((TextBoxStoryDescriptor)entry.Value).Write(m_stream);
        //        }
        //        else if (entry.Value is BreakDescriptor)
        //        {
        //            //If options is set before write then...
        //            ((BreakDescriptor)entry.Value).Write(m_stream);
        //        }
        //    }
        //}
        #endregion
    }
}
