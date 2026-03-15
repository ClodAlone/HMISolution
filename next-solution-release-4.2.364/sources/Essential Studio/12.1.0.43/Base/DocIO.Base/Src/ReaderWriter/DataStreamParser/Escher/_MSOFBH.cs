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
    /// Common Header
    /// The common record header is an 8-byte structure defined in msodr.h as follows:
    /// typedef struct MSOFBH
    ///         {
    ///           struct
    ///           {
    ///             ULONG ver : 4;
    ///             ULONG inst: 12;
    ///             ULONG fbt : 16;
    ///           };
    ///             ULONG cbLength;
    ///         } MSOFBH;
    /// </summary>
    internal class _MSOFBH : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal const int DEF_ISCONTAINER = 15;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_version;
        private int m_instance;
        private int m_type;
        private int m_length;
        internal WordDocument m_doc;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int Instance
        {
            get
            {
                return m_instance;
            }
            set
            {
                m_instance = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsContainer
        {
            get
            {
                return (m_version == DEF_ISCONTAINER);
            }
            set
            {
                if (value)
                {
                    m_version = 15;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        new internal int Length
        {
            get
            {
                return m_length;
            }
            set
            {
                m_length = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal MSOFBT Type
        {
            get
            {
                return (MSOFBT)m_type;
            }
            set
            {
                m_type = (int)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int Version
        {
            get
            {
                return m_version;
            }
            set
            {
                m_version = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal _MSOFBH(WordDocument doc)
        {
            m_doc = doc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal _MSOFBH(Stream stream, WordDocument doc)
        {
            m_doc = doc;
            Read(stream);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            int num1 = ReadInt32(stream);
            m_version = num1 & 15;
            m_instance = (num1 & 0xfff0) >> 4;
            m_type = (int)((num1 & 0xffff0000) >> 0x10);
            m_length = ReadInt32(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            int num1 = 0;
            num1 |= m_version;
            num1 |= (m_instance << 4);
            num1 |= (m_type << 0x10);
            WriteInt32(stream, num1);
            WriteInt32(stream, m_length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal BaseEscherRecord CreateRecordFromHeader()
        {
            switch (Type)
            {
                case MSOFBT.msofbtDggContainer:
                    {
                        return new MsofbtDggContainer(m_doc);
                    }
                case MSOFBT.msofbtBstoreContainer:
                    {
                        return new MsofbtBstoreContainer(m_doc);
                    }
                case MSOFBT.msofbtDgContainer:
                    {
                        return new MsofbtDgContainer(m_doc);
                    }
                case MSOFBT.msofbtSpgrContainer:
                    {
                        return new MsofbtSpgrContainer(m_doc);
                    }
                case MSOFBT.msofbtSpContainer:
                    {
                        return new MsofbtSpContainer(m_doc);
                    }
                case MSOFBT.msofbtSolverContainer:
                    {
                        return new MsofbtSolverContainer(m_doc);
                    }
                case MSOFBT.msofbtDgg:
                    {
                        return new MsofbtDgg(m_doc);
                    }
                case MSOFBT.msofbtBSE:
                    {
                        return new MsofbtBSE(m_doc);
                    }
                case MSOFBT.msofbtDg:
                    {
                        return new MsofbtDg(m_doc);
                    }
                case MSOFBT.msofbtSpgr:
                    {
                        return new MsofbtSpgr(m_doc);
                    }
                case MSOFBT.msofbtSp:
                    {
                        return new MsofbtSp(m_doc);
                    }
                case MSOFBT.msofbtOPT:
                    {
                        return new MsofbtOPT(m_doc);
                    }
                case MSOFBT.msofbtClientTextbox:
                    {
                        return new MsofbtClientTextbox(m_doc);
                    }
                case MSOFBT.msofbtClientAnchor:
                    {
                        return new MsofbtClientAnchor(m_doc);
                    }
                case MSOFBT.msofbtClientData:
                    {
                        return new MsofbtClientData(m_doc);
                    }
                case MSOFBT.msofbtBlipEMF:
                case MSOFBT.msofbtBlipWMF:
                    {
                        return new MsofbtMetaFile(m_doc);
                    }
                case MSOFBT.msofbtBlipJPEG:
                case MSOFBT.msofbtBlipPNG:
                case MSOFBT.msofbtBlipDIB:
                    {
                        return new MsofbtImage(m_doc);
                    }
                case MSOFBT.msofbtREGROUPItems:
                    {
                        return new MsofbtGeneral(m_doc);
                    }
                case MSOFBT.msofbtSecondaryFOPT:
                    {
                        return new MsofbtSecondaryFOPT(m_doc);
                    }
                case MSOFBT.msofbtTertiaryFOPT:
                    {
                        return new MsofbtTertiaryFOPT(m_doc);
                    }
            }
            if (IsContainer)
            {
                return new BaseContainer(m_doc);
            }
            return new MsofbtGeneral(m_doc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal _MSOFBH Clone()
        {
            //      _MSOFBH fbh = new _MSOFBH();
            //      fbh.m_instance = m_instance;
            //      fbh.m_length = m_length;
            //      fbh.m_type = m_type;
            //      fbh.m_version = m_version;
            _MSOFBH fbh = (_MSOFBH)this.MemberwiseClone();
            fbh.m_doc = m_doc;
            return fbh;
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static BaseEscherRecord ReadHeaderWithRecord(Stream stream, WordDocument doc)
        {
            _MSOFBH msofbh = new _MSOFBH(stream, doc);
            BaseEscherRecord record = msofbh.CreateRecordFromHeader();
            bool recordCheck = record.ReadRecord(msofbh, stream);
            return (recordCheck) ? record : null;
        }
        #endregion

    }
}
