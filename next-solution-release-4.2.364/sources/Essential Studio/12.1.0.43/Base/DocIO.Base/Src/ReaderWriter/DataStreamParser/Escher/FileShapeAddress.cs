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
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for FileShapeAddress.
    /// </summary>
    [CLSCompliant(false)]
    internal class FileShapeAddress : BaseWordRecord
    {
        #region Class constants
        internal const int DEF_FSPA_LENGTH = 26;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_spid;
        private int m_xaLeft;
        private int m_yaTop;
        private int m_xaRight;
        private int m_yaBottom;
        private bool m_fHdr;
        private int m_relHrzPos;
        private int m_relVrtPos;
        private int m_wrapStyle;
        private int m_wrapType;
        private bool m_fRcaSimple;
        private bool m_fBelowText;
        private bool m_fAnchorLock;
        private int m_cTxbx;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int Spid
        {
            get
            {
                return m_spid;
            }
            set
            {
                m_spid = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int XaLeft
        {
            get
            {
                return m_xaLeft;
            }
            set
            {
                m_xaLeft = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int YaTop
        {
            get
            {
                return m_yaTop;
            }
            set
            {
                m_yaTop = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int XaRight
        {
            get
            {
                return m_xaRight;
            }
            set
            {
                m_xaRight = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int YaBottom
        {
            get
            {
                return m_yaBottom;
            }
            set
            {
                m_yaBottom = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsHeaderShape
        {
            get
            {
                return m_fHdr;
            }
            set
            {
                m_fHdr = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal HorizontalOrigin RelHrzPos
        {
            get
            {
                return (HorizontalOrigin)m_relHrzPos;
            }
            set
            {
                m_relHrzPos = (int)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal VerticalOrigin RelVrtPos
        {
            get
            {
                return (VerticalOrigin)m_relVrtPos;
            }
            set
            {
                m_relVrtPos = (int)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TextWrappingStyle TextWrappingStyle
        {
            get
            {
                return (TextWrappingStyle)m_wrapStyle;
            }
            set
            {
                m_wrapStyle = (int)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TextWrappingType TextWrappingType
        {
            get
            {
                return (TextWrappingType)m_wrapType;
            }
            set
            {
                m_wrapType = (int)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsRcaSimple
        {
            get
            {
                return m_fRcaSimple;
            }
            set
            {
                m_fRcaSimple = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsBelowText
        {
            get
            {
                return m_fBelowText;
            }
            set
            {
                m_fBelowText = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsAnchorLock
        {
            get
            {
                return m_fAnchorLock;
            }
            set
            {
                m_fAnchorLock = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int TxbxCount
        {
            get
            {
                return m_cTxbx;
            }
            set
            {
                m_cTxbx = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int Height
        {
            get
            {
                return (m_yaBottom - m_yaTop);
            }
            set
            {
                m_yaBottom = m_yaTop + value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int Width
        {
            get
            {
                return (m_xaRight - m_xaLeft);
            }
            set
            {
                m_xaRight = m_xaLeft + value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal FileShapeAddress(Stream stream)
        {
            Read(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        internal FileShapeAddress()
        { }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            m_spid = ReadInt32(stream);
            m_xaLeft = ReadInt32(stream);
            m_yaTop = ReadInt32(stream);
            m_xaRight = ReadInt32(stream);
            m_yaBottom = ReadInt32(stream);

            int tmp = ReadInt16(stream);
            m_fHdr = (tmp & 1) != 0;
            m_relHrzPos = (tmp & 6) >> 1;
            m_relVrtPos = (tmp & 0x18) >> 3;
            m_wrapStyle = (tmp & 480) >> 5;
            m_wrapType = (tmp & 0x1e00) >> 9;
            m_fRcaSimple = (tmp & 0x2000) != 0;
            m_fBelowText = (tmp & 0x4000) != 0;
            m_fAnchorLock = (tmp & 0x8000) != 0;
            if (m_fBelowText && TextWrappingStyle == TextWrappingStyle.InFrontOfText)
                TextWrappingStyle = TextWrappingStyle.Behind;
            m_cTxbx = ReadInt32(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            WriteInt32(stream, m_spid);
            WriteInt32(stream, m_xaLeft);
            WriteInt32(stream, m_yaTop);
            WriteInt32(stream, m_xaRight);
            WriteInt32(stream, m_yaBottom);
            if (TextWrappingStyle == TextWrappingStyle.Behind && m_fBelowText)
                TextWrappingStyle = TextWrappingStyle.InFrontOfText;

            int tmp = 0;
            tmp |= (m_fHdr ? 1 : 0);
            tmp |= (m_relHrzPos << 1);
            tmp |= (m_relVrtPos << 3);
            tmp |= (m_wrapStyle << 5);
            tmp |= (m_wrapType << 9);
            tmp |= (m_fRcaSimple ? 0x2000 : 0);
            tmp |= (m_fBelowText ? 0x4000 : 0);
            tmp |= (m_fAnchorLock ? 0x8000 : 0);
            WriteInt16(stream, (short)tmp);

            WriteInt32(stream, m_cTxbx);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal FileShapeAddress Clone()
        {
            FileShapeAddress fspa = new FileShapeAddress();
            fspa.m_cTxbx = m_cTxbx;
            fspa.m_fAnchorLock = m_fAnchorLock;
            fspa.m_fBelowText = m_fBelowText;
            fspa.m_fHdr = m_fHdr;
            fspa.m_fRcaSimple = m_fRcaSimple;
            fspa.m_relHrzPos = m_relHrzPos;
            fspa.m_relVrtPos = m_relVrtPos;
            fspa.m_spid = m_spid;
            fspa.m_wrapStyle = m_wrapStyle;
            fspa.m_wrapType = m_wrapType;
            fspa.m_xaLeft = m_xaLeft;
            fspa.m_xaRight = m_xaRight;
            fspa.m_yaBottom = m_yaBottom;
            fspa.m_yaTop = m_yaTop;

            return fspa;
        }
        #endregion

    }
}
