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
using System.IO;

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for BaseContainer.
    /// </summary>
    internal abstract class BaseEscherRecord : BaseWordRecord
    {
        #region Class members
        private _MSOFBH m_msofbh;
        internal WordDocument m_doc;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal _MSOFBH Header
        {
            get
            {
                return m_msofbh;
            }
            set
            {
                m_msofbh = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal BaseEscherRecord(WordDocument doc)
        {
            m_doc = doc;
            m_msofbh = new _MSOFBH(doc);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="version"></param>
        internal BaseEscherRecord(MSOFBT type, int version, WordDocument doc)
            : this(doc)
        {
            m_msofbh.Type = type;
            m_msofbh.Version = version;
        }
        #endregion

        #region Class abstract methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected abstract void ReadRecordData(Stream stream);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected abstract void WriteRecordData(Stream stream);
        /// <summary>
        /// Clone current record.
        /// </summary>
        internal abstract BaseEscherRecord Clone();
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msofbh"></param>
        /// <param name="stream"></param>
        internal bool ReadRecord(_MSOFBH msofbh, Stream stream)
        {
            m_msofbh = msofbh;
            int pos = (int)stream.Position;
            ReadRecordData(stream);
            int length = ((int)stream.Position) - pos;
            if (length != m_msofbh.Length)
            {
                //throw new ArgumentOutOfRangeException( "Incorrect number of bytes read from record." );
                return false;
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void ReadMsofbhWithRecord(Stream stream)
        {
            ReadRecord(new _MSOFBH(stream, m_doc), stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal int WriteMsofbhWithRecord(Stream stream)
        {
            int headerPos = Convert.ToInt32(stream.Position);
            Header.Write(stream);
            int recorPos = Convert.ToInt32(stream.Position);
            WriteRecordData(stream);
            int endPos = Convert.ToInt32(stream.Position);
            Header.Length = (endPos - recorPos);
            stream.Position = headerPos;
            Header.Write(stream);
            stream.Position = endPos;
            return (endPos - headerPos);
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal virtual void Close()
        { }
        #endregion
    }
}
