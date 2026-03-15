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
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// the text in the textbox, in a host-defined format.
    /// </summary>
    internal class MsofbtClientTextbox : BaseEscherRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_txId;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int Txid
        {
            get
            {
                return m_txId;
            }
            set
            {
                m_txId = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        public MsofbtClientTextbox(WordDocument doc) : base(doc)
        {
            Header.Type = MSOFBT.msofbtClientTextbox;
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_txId = ReadInt32(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            WriteInt32(stream, m_txId);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtClientTextbox clientTxBx = (MsofbtClientTextbox)this.MemberwiseClone();
            clientTxBx.m_doc = m_doc;
            return clientTxBx;
        }

        #endregion
    }
}
