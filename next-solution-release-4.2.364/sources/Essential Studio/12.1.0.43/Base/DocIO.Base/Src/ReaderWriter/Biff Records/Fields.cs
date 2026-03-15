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

#region File using directives
using System;
using System.Collections;
using System.IO;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using System.Collections.Specialized;
using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for Fields.
    /// </summary>
    [CLSCompliant(false)]
    internal class Fields : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal int DEF_FLD_SIZE = 2;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private Stream m_stream;
        private BinaryReader m_reader;
        private DocIOSortedList<int, FieldDescriptor> m_curList;
        private Dictionary<WordSubdocument, DocIOSortedList<int, FieldDescriptor>> m_fieldsList
          = new Dictionary<WordSubdocument, DocIOSortedList<int, FieldDescriptor>>();
        private int m_endPos;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FieldDescriptor> MainFields
        {
            get
            {
              if(m_fieldsList.ContainsKey(WordSubdocument.Main))
                return m_fieldsList[WordSubdocument.Main];
              return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FieldDescriptor> HFFields
        {
            get
            {
              if(m_fieldsList.ContainsKey(WordSubdocument.HeaderFooter))
                return m_fieldsList[WordSubdocument.HeaderFooter];
              return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FieldDescriptor> FtnFields
        {
            get
            {
              if(m_fieldsList.ContainsKey(WordSubdocument.Footnote))
                return m_fieldsList[WordSubdocument.Footnote];
              return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FieldDescriptor> AtnFields
        {
            get
            {
              if(m_fieldsList.ContainsKey(WordSubdocument.Annotation))
                return m_fieldsList[WordSubdocument.Annotation];
              return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FieldDescriptor> EdnFields
        {
            get
            {
              if(m_fieldsList.ContainsKey(WordSubdocument.Endnote))
                return m_fieldsList[WordSubdocument.Endnote];
              return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FieldDescriptor> TxbxFields
        {
            get
            {
              if(m_fieldsList.ContainsKey(WordSubdocument.TextBox))
                return m_fieldsList[WordSubdocument.TextBox];
              return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocIOSortedList<int, FieldDescriptor> HdrTxbxFields
        {
            get
            {
              if(m_fieldsList.ContainsKey(WordSubdocument.HeaderTextBox))
                return m_fieldsList[WordSubdocument.HeaderTextBox];
              return null;
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="reader"></param>
        internal Fields(WPFIBData fib, BinaryReader reader)
        {
            m_fieldsList = new Dictionary<WordSubdocument, DocIOSortedList<int, FieldDescriptor>>();
            m_reader = reader;
            ReadFieldsForSubDoc(WordSubdocument.Main, fib.fcPlcfFldMom, fib.lcbPlcfFldMom);
            ReadFieldsForSubDoc(WordSubdocument.HeaderFooter, fib.fcPlcfFldHdr, fib.lcbPlcfFldHdr);
            ReadFieldsForSubDoc(WordSubdocument.Footnote, fib.fcPlcffldFtn, fib.lcbPlcffldFtn);
            ReadFieldsForSubDoc(WordSubdocument.Annotation, fib.fcPlcffldAtn, fib.lcbPlcffldAtn);
            ReadFieldsForSubDoc(WordSubdocument.Endnote, fib.fcPlcffldEdn, fib.lcbPlcffldEdn);
            ReadFieldsForSubDoc(WordSubdocument.TextBox, fib.fcPlcffldTxbx, fib.lcbPlcffldTxbx);
            ReadFieldsForSubDoc(WordSubdocument.HeaderTextBox, fib.fcPlcffldHdrTxbx, fib.lcbPlcffldHdrTxbx);
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        internal Fields()
        {
        }
        #endregion

        #region Class internal methods
        /// <summary>
        ///  
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="fld"></param>
        /// <param name="pos"></param>
        internal void AddField(WordSubdocument docType, FieldDescriptor fld, int pos)
        {
            if (!m_fieldsList.ContainsKey(docType))
            {
                DocIOSortedList<int, FieldDescriptor> hashTable = new DocIOSortedList<int, FieldDescriptor>();
                m_fieldsList.Add(docType, hashTable);
            }
            m_fieldsList[docType].Add(pos, fld);
        }
        /// <summary>
        /// Gets fields for subdocument.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal DocIOSortedList<int, FieldDescriptor> GetFieldsForSubDoc( WordSubdocument type )
        {
            return m_fieldsList[type];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        /// <param name="endCharacter"></param>
        internal void Write(Stream stream, WPFIBData fib, int endCharacter)
        {
            m_stream = stream;
            m_endPos = endCharacter;
            if (m_fieldsList.Count > 0)
            {
                if (MainFields != null)
                {
                    fib.fcPlcfFldMom = (int)m_stream.Position;
                    WriteFieldsForSubDoc(MainFields);
                    fib.lcbPlcfFldMom = (int)(m_stream.Position - fib.fcPlcfFldMom);
                }
                if (HFFields != null)
                {
                    fib.fcPlcfFldHdr = (int)m_stream.Position;
                    WriteFieldsForSubDoc(HFFields);
                    fib.lcbPlcfFldHdr = (int)(m_stream.Position - fib.fcPlcfFldHdr);
                }
                if (FtnFields != null)
                {
                    fib.fcPlcffldFtn = (int)m_stream.Position;
                    WriteFieldsForSubDoc(FtnFields);
                    fib.lcbPlcffldFtn = (int)(m_stream.Position - fib.fcPlcffldFtn);
                }
                if (AtnFields != null)
                {
                    fib.fcPlcffldAtn = (int)m_stream.Position;
                    WriteFieldsForSubDoc(AtnFields);
                    fib.lcbPlcffldAtn = (int)(m_stream.Position - fib.fcPlcffldAtn);
                }
                if (EdnFields != null)
                {
                    fib.fcPlcffldEdn = (int)m_stream.Position;
                    WriteFieldsForSubDoc(EdnFields);
                    fib.lcbPlcffldEdn = (int)(m_stream.Position - fib.fcPlcffldEdn);
                }
                if (TxbxFields != null)
                {
                    fib.fcPlcffldTxbx = (int)m_stream.Position;
                    WriteFieldsForSubDoc(TxbxFields);
                    fib.lcbPlcffldTxbx = (int)(m_stream.Position - fib.fcPlcffldTxbx);
                }
                if (HdrTxbxFields != null)
                {
                    fib.fcPlcffldHdrTxbx = (int)m_stream.Position;
                    WriteFieldsForSubDoc(HdrTxbxFields);
                    fib.lcbPlcffldHdrTxbx = (int)(m_stream.Position - fib.fcPlcffldHdrTxbx);
                }
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        internal FieldDescriptor FindFld(WordSubdocument docType, int pos)
        {
            DocIOSortedList<int, FieldDescriptor> list = m_fieldsList[ docType ];
            return list[pos];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pos"></param>
        /// <param name="posNext"></param>
        private void ReadFieldDescriptor(BinaryReader reader, int pos, int posNext)
        {
            FieldDescriptor fldDesc = new FieldDescriptor(reader);
            m_curList[pos] = fldDesc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="pos"></param>
        /// <param name="length"></param>
        private void ReadFieldsForSubDoc(WordSubdocument docType, int pos, int length)
        {
            m_curList = new DocIOSortedList<int, FieldDescriptor>();
            m_fieldsList[docType] = m_curList;
            m_reader.BaseStream.Position = pos;
            PosStructReader.Read(m_reader, (int)length, DEF_FLD_SIZE, new PosStructReaderDelegate(ReadFieldDescriptor));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stList"></param>
        private void WriteFieldsForSubDoc( DocIOSortedList<int, FieldDescriptor> stList )
        {
            //Write positions to stream
            //      foreach( DictionaryEntry entry in stList )
            for (int i = 0, cnt = stList.Count; i < cnt; i++)
            {
                WriteInt32(m_stream, (int)stList.GetKey(i));
            }
            WriteInt32(m_stream, m_endPos);
            //      foreach( DictionaryEntry entry in stList )
            foreach (int key in stList.Keys)
            {
                stList[key].Write(m_stream);
            }
        }


        #endregion
    }
}
