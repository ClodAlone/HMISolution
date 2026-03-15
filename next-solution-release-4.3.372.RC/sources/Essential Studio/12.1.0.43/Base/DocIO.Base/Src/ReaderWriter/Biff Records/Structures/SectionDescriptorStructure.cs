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
    /// Summary description for SectionDescriptorStructure.
    /// </summary>
    [CLSCompliant(false)]
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Explicit)]
#endif
    internal class SectionDescriptorStructure : DataStructure
    {
        #region Constants
        private const int DEF_RECORD_SIZE = 12;
        #endregion

        #region Class members
        /// <summary>
        /// used internally by Word (fn).
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(0)]
#endif
        private short m_sInternal;
        /// <summary>
        /// file offset in main stream to beginning of SEPX stored for section.
        /// If sed.fcSepx == 0xFFFFFFFF, the section properties for the section are equal
        /// to the standard SEP (see SEP definition).
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(2)]
#endif
        private uint m_fcSepx;
        /// <summary>
        /// used internally by Word (fnMpr).
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(6)]
#endif
        private short m_sInternal2;
        /// <summary>
        /// Points to offset in FC space of main stream where the Macintosh Print Record
        /// for a document created on a Mac will be stored.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [FieldOffset(8)]
#endif
        private int m_fcMpr;
        #endregion

        #region Class Properties
        /// <summary>
        /// used internally by Word (fn).
        /// </summary>
        internal short Internal1
        {
            get
            {
                return m_sInternal;
            }
            set
            {
                m_sInternal = value;
            }
        }

        /// <summary>
        /// used internally by Word (fnMpr).
        /// </summary>
        internal short Internal2
        {
            get
            {
                return m_sInternal2;
            }
            set
            {
                m_sInternal2 = value;
            }
        }

        /// <summary>
        /// file offset in main stream to beginning of SEPX stored for section.
        /// If sed.fcSepx == 0xFFFFFFFF, the section properties for the section are equal
        /// to the standard SEP (see SEP definition).
        /// </summary>
        internal uint SepxPosition
        {
            get
            {
                return m_fcSepx;
            }
            set
            {
                m_fcSepx = value;
            }
        }

        /// <summary>
        /// Points to offset in FC space of main stream where the Macintosh Print Record
        /// for a document created on a Mac will be stored.
        /// </summary>
        internal int MacPrintOffset
        {
            get
            {
                return m_fcMpr;
            }
            set
            {
                m_fcMpr = value;
            }
        }

        /// <summary>
        /// Gets the size of the structure.
        /// </summary>
        /// <value>The length.</value>
        internal override int Length
        {
            get
            {
                return DEF_RECORD_SIZE;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_sInternal = ReadInt16(arrData, ref iOffset);
            m_fcSepx = ReadUInt32(arrData, ref iOffset);
            m_sInternal2 = ReadInt16(arrData, ref iOffset);
            m_fcMpr = ReadInt32(arrData, ref iOffset);
        }

        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        internal override int Save(byte[] arrData, int iOffset)
        {
            WriteInt16(arrData, ref iOffset, m_sInternal);
            WriteUInt32(arrData, ref iOffset, m_fcSepx);
            WriteInt16(arrData, ref iOffset, m_sInternal2);
            WriteInt32(arrData, ref iOffset, m_fcMpr);

            return DEF_RECORD_SIZE;
        }
        #endregion
    }
}
