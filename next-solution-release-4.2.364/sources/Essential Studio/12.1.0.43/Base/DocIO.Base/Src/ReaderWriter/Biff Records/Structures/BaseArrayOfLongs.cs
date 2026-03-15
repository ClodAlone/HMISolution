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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for ArrayOfLongsBase.
    /// </summary>
    internal class BaseArrayOfLongs
    {
        #region Class constants
        /// <summary>
        /// Size in bytes of each member in this array.
        /// </summary>
        private const int DEF_MEMBER_SIZE = 4;
        #endregion

        #region Class members
        /// <summary>
        /// Array of ints.
        /// </summary>
        protected int[] m_arrLongs = new int[0];
        #endregion

        #region Class methods
        /// <summary>
        /// Resizes array.
        /// </summary>
        /// <param name="iNewSize">New size of the array.</param>
        internal void Resize(int iNewSize)
        {
            if (iNewSize < 0)
                throw new ArgumentOutOfRangeException("iNewSize can't be less than zero.");

            if (iNewSize != m_arrLongs.Length)
            {
                m_arrLongs = new int[iNewSize];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrBuffer"></param>
        internal virtual void SetBuffer(byte[] arrBuffer)
        {
            if (arrBuffer == null)
                throw new ArgumentNullException("arrBuffer");

            Resize(arrBuffer.Length / DEF_MEMBER_SIZE);
            Buffer.BlockCopy(arrBuffer, 0, m_arrLongs, 0, arrBuffer.Length);
            //API.CopyMemory( m_arrLongs, arrBuffer, arrBuffer.Length );
        }

        /// <summary>
        /// Copies array of longs into array of bytes.
        /// </summary>
        /// <param name="arrData">Destination array of bytes.</param>
        /// <param name="iOffset">Offset to the start byte in the destination array to copy into.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        /// <returns>Size in bytes of the copied data.</returns>
        internal virtual int Save(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            int iDataSize = m_arrLongs.Length * Constants.BytesInInt;

            if (iOffset < 0
              || iOffset > arrData.Length
              || iOffset + iDataSize > arrData.Length)
            {
                throw new ArgumentOutOfRangeException("iOffset");
            }

            if (iDataSize == 0) return 0;

            //      API.CopyMemory( ref arrData[ iOffset ], m_arrLongs, iDataSize );
            Buffer.BlockCopy(m_arrLongs, 0, arrData, iOffset, iDataSize);

            return iDataSize;
        }
        #endregion
    }
}
