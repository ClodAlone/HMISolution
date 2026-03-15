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
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for MsofbtUnknown.
    /// </summary>
    internal class MsofbtGeneral : BaseEscherRecord
    {
        #region Class members
        private byte[] m_data;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal byte[] Data
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MsofbtGeneral"/> class.
        /// </summary>
        /// <param name="doc"></param>
        internal MsofbtGeneral(WordDocument doc)
            : base(doc)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_data = new byte[Header.Length];
            stream.Read(m_data, 0, Header.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            stream.Write(m_data, 0, m_data.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtGeneral genData = new MsofbtGeneral(m_doc);

            genData.m_data = new byte[m_data.Length];
            m_data.CopyTo(genData.m_data, 0);
            genData.Header = Header.Clone();
            genData.m_doc = m_doc;
            return genData;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();
            m_data = null;
        }
        #endregion
    }
}
