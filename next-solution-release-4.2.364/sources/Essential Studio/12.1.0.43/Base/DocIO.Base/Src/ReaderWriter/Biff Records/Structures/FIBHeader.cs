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
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for FIBHeader.
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Explicit)]
#endif
    [CLSCompliant(false)]
    internal class FIBHeader : DataStructure
    {
        //    /// <summary>
        //    /// Beginning of the FIB header
        //    /// </summary>
        //    //[ FieldOffset( 0 ) ]
        //    internal FIBH fibh;
        /// <summary>
        /// magic number
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(0)]
#endif
        internal ushort wIdent;
        /// <summary>
        /// FIB version written. This will be >= 101 for all Word 6.0 for Windows and after documents.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(2)]
#endif
        internal ushort nFib;
        /// <summary>
        /// product version written by
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(4)]
#endif
        internal ushort nProduct;
        /// <summary>
        /// language stamp -- localized version
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(6)]
#endif
        internal ushort lid;
        /// <summary>
        /// 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(8)]
#endif
        internal short pnNext;

        #region BitField 10
        /// <summary>
        /// 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(10)]
#endif
        internal ushort m_usOptions;
        ////[ FieldOffset( 10 ) ]
        ///// <summary>
        ///// Set if this document is a template
        ///// </summary>
        //0x0001//internal ushort fDot // 0x0001;
        ///// <summary>
        ///// Set if this document is a glossary
        ///// </summary>
        //0x0002//internal ushort fGlsy // 0x0002;
        ///// <summary>
        ///// when 1, file is in complex, fast-saved format.
        ///// </summary>
        //0x0004//internal ushort fComplex // 0x0004;
        ///// <summary>
        ///// set if file contains 1 or more pictures
        ///// </summary>
        //0x0008//internal ushort fHasPic // 0x0008;
        ///// <summary>
        ///// count of times file was quicksaved
        ///// </summary>
        //0x00F0//internal ushort cQuickSaves // 0x00F0;
        ///// <summary>
        ///// Set if file is encrypted
        ///// </summary>
        //0x0100//internal ushort fEncrypted // 0x0100;
        ///// <summary>
        ///// When 0, this fib refers to the table stream named "0Table", when 1, this fib refers to the table stream named "1Table". Normally, a file will have only one table stream, but under unusual circumstances a file may have table streams with both names. In that case, this flag must be used to decide which table stream is valid.
        ///// </summary>
        //0x0200//internal ushort fWhichTblStm // 0x0200;
        ///// <summary>
        ///// Set when user has recommended that file be read read-only
        ///// </summary>
        //0x0400//internal ushort fReadOnlyRecommended // 0x0400;
        ///// <summary>
        ///// Set when file owner has made the file write reserved
        ///// </summary>
        //0x0800//internal ushort fWriteReservation // 0x0800;
        ///// <summary>
        ///// Set when using extended character set in file
        ///// </summary>
        //0x1000//internal ushort fExtChar // 0x1000;
        ///// <summary>
        ///// REVIEW
        ///// </summary>
        //0x2000//internal ushort fLoadOverride // 0x2000;
        ///// <summary>
        ///// REVIEW
        ///// </summary>
        //0x4000//internal ushort fFarEast // 0x4000;
        ///// <summary>
        ///// REVIEW
        ///// </summary>
        //0x8000//internal ushort fCrypto // 0x8000;
        #endregion

        /// <summary>
        /// This file format it compatible with readers that understand nFib at or above this value.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(12)]
#endif
        internal ushort nFibBack;
        /// <summary>
        /// File encrypted key, only valid if fEncrypted.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(14)]
#endif
        internal int lKey;
        /// <summary>
        /// environment in which file was created
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(18)]
#endif
        internal byte envr;

        #region BitField 19
        /// <summary>
        /// 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(19)]
#endif
        internal byte m_btOptions2;
        ////[ FieldOffset( 19 ) ]
        ///// <summary>
        ///// when 1, this file was last saved in the Mac environment
        ///// </summary>
        //0x01//internal byte fMac // 0x01;
        ///// <summary>
        ///// 
        ///// </summary>
        //0x02//internal byte fEmptySpecial // 0x02;
        ///// <summary>
        ///// 
        ///// </summary>
        //0x04//internal byte fLoadOverridePage // 0x04;
        ///// <summary>
        ///// 
        ///// </summary>
        //0x08//internal byte fFutureSavedUndo // 0x08;
        ///// <summary>
        ///// 
        ///// </summary>
        //0x10//internal byte fWord97Saved // 0x10;
        ///// <summary>
        ///// 
        ///// </summary>
        //0xFE//internal byte fSpare0 // 0xFE;
        #endregion

        /// <summary>
        /// Default extended character set id for text in document stream. (overridden by chp.chse)
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(20)]
#endif
        internal ushort chs;
        /// <summary>
        /// Default extended character set id for text in internal data structures
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(22)]
#endif
        internal ushort chsTables;
        /// <summary>
        /// file offset of first character of text. In non-complex files a CP can be transformed into an FC by the following transformation:
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(24)]
#endif
        internal uint fcMin;
        /// <summary>
        /// file offset of last character of text in document text stream + 1
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(28)]
#endif
        internal uint fcMac;
        /// <summary>
        /// Count of fields in the array of "shorts"
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(32)]
#endif
        internal ushort csw;

        /// <summary>
        /// 
        /// </summary>
        internal const int DEF_HEADER_SIZE = 34;

        #region IBaseStructure Members
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return DEF_HEADER_SIZE;
            }
        }
        #endregion

        #region Implementation / override
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            wIdent = ReadUInt16(arrData, ref iOffset);
            nFib = ReadUInt16(arrData, ref iOffset);
            nProduct = ReadUInt16(arrData, ref iOffset);
            lid = ReadUInt16(arrData, ref iOffset);
            pnNext = ReadInt16(arrData, ref iOffset);
            m_usOptions = ReadUInt16(arrData, ref iOffset);
            nFibBack = ReadUInt16(arrData, ref iOffset);
            lKey = ReadInt32(arrData, ref iOffset);

            envr = arrData[iOffset];
            iOffset += 1;
            m_btOptions2 = arrData[iOffset];
            iOffset += 1;

            chs = ReadUInt16(arrData, ref iOffset);
            chsTables = ReadUInt16(arrData, ref iOffset);
            fcMin = ReadUInt32(arrData, ref iOffset);
            fcMac = ReadUInt32(arrData, ref iOffset);
            csw = ReadUInt16(arrData, ref iOffset);
        }

        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        internal override int Save(byte[] arrData, int iOffset)
        {
            WriteUInt16(arrData, ref iOffset, wIdent);
            WriteUInt16(arrData, ref iOffset, nFib);
            WriteUInt16(arrData, ref iOffset, nProduct);
            WriteUInt16(arrData, ref iOffset, lid);
            WriteInt16(arrData, ref iOffset, pnNext);
            WriteUInt16(arrData, ref iOffset, m_usOptions);
            WriteUInt16(arrData, ref iOffset, nFibBack);
            WriteInt32(arrData, ref iOffset, lKey);

            arrData[iOffset] = envr;
            iOffset += 1;
            arrData[iOffset] = m_btOptions2;
            iOffset += 1;

            WriteUInt16(arrData, ref iOffset, chs);
            WriteUInt16(arrData, ref iOffset, chsTables);
            WriteUInt32(arrData, ref iOffset, fcMin);
            WriteUInt32(arrData, ref iOffset, fcMac);
            WriteUInt16(arrData, ref iOffset, csw);

            return DEF_HEADER_SIZE;
        }
        #endregion
    }
}
