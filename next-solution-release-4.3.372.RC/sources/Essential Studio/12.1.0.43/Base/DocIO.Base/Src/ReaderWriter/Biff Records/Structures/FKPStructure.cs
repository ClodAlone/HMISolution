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
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Formatted disK Page - 512 bytes
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    internal class FKPStructure : DataStructure
    {
        #region Class constants
        /// <summary>
        /// Size of the record.
        /// </summary>
        internal const int DEF_RECORD_SIZE = Constants.DiskPageSize;
        #endregion

        #region Class members
        /// <summary>
        /// Page data.
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DEF_RECORD_SIZE - 1)]
#endif
        private byte[] m_arrPageData = new byte[DEF_RECORD_SIZE - 1];

        /// <summary>
        /// Count of elements in the array.
        /// </summary>
        //[ FieldOffset( 511 ) ]
        private byte m_btLength;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal FKPStructure()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal FKPStructure(Stream stream)
        {
            stream.Read(m_arrPageData, 0, Constants.DiskPageSize - 1);
            m_btLength = (byte)stream.ReadByte();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Page data.
        /// </summary>
        internal byte[] PageData
        {
            get
            {
                return m_arrPageData;
            }
        }

        /// <summary>
        /// Count of elements.
        /// </summary>
        internal byte Count
        {
            get
            {
                return m_btLength;
            }
            set
            {
                if (m_btLength != value)
                {
                    m_btLength = value;
                }
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

        #region Class methods
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_arrPageData = ReadBytes(arrData, m_arrPageData.Length, ref iOffset);
            m_btLength = arrData[iOffset];
            iOffset += 1;
        }

        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns>Length</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset + DEF_RECORD_SIZE > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            m_arrPageData.CopyTo(arrData, iOffset);
            iOffset += m_arrPageData.Length;
            arrData[iOffset] = m_btLength;

            return ++iOffset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal int Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            //      if( iOffset < 0 || iOffset + DEF_RECORD_SIZE > arrData.Length )
            //        throw new ArgumentOutOfRangeException( "iOffset" );

            stream.Write(m_arrPageData, 0, m_arrPageData.Length);
            //      m_arrPageData.CopyTo( arrData, iOffset );
            //      iOffset += m_arrPageData.Length;
            //      arrData[ iOffset ] = m_btLength;
            stream.WriteByte(m_btLength);

            return (int)stream.Position;
        }
        #endregion
    }
}
