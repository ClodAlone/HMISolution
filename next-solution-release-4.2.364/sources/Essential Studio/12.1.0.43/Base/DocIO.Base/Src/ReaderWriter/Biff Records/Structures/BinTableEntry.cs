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
    /// Summary description for BinTableEntry.
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    internal class BinTableEntry
    {
        #region Class constants
        /// <summary>
        /// Record size in bytes.
        /// </summary>
        public const int RECORD_SIZE = 4;
        #endregion

        #region Class members
        /// <summary>
        /// Binary table entry value.
        /// </summary>
        private int m_iValue;
        #endregion

        #region Class Properties
        /// <summary>
        /// Binary table entry value.
        /// </summary>
        internal int Value
        {
            get
            {
                return m_iValue;
            }
            set
            {
                m_iValue = value;
            }
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Parses the specified data array.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The i offset.</param>
        /// <returns></returns>
        internal int Parse(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset", "Value can not be less 0 and greater arrData.Length");

            m_iValue = BitConverter.ToInt32(arrData, iOffset);
            return iOffset + Constants.BytesInInt;
        }

        /// <summary>
        /// Saves the structure to data array.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The i offset.</param>
        internal void Save(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            if (iOffset + RECORD_SIZE > arrData.Length)
                throw new ArgumentOutOfRangeException("arrData.Length");

            byte[] arrResult = BitConverter.GetBytes(m_iValue);
            arrResult.CopyTo(arrData, iOffset);
        }
        #endregion
    }
}
