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

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for UniversalPropertyException.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class UniversalPropertyException : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Internal data.
        /// </summary>
        private byte[] m_arrData;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal UniversalPropertyException()
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="iCount">Number of bytes for the new record.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal UniversalPropertyException(byte[] arrData, int iOffset, int iCount)
            : base(arrData, iOffset, iCount)
        {
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Returns internal data array. Read-only.
        /// </summary>
        internal byte[] Data
        {
            get
            {
                return m_arrData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return m_arrData.Length;
            }
        }

        #endregion

        #region Class Overrides
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal override void Parse(byte[] arrData, int iOffset, int iCount)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset", "Value can not be less 0 and greater arrData.Length");

            if (iCount < 0 || iCount + iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iCount");

            if (m_arrData == null || m_arrData.Length != iCount)
            {
                m_arrData = new byte[iCount];
            }

            Array.Copy(arrData, iOffset, m_arrData, 0, iCount);
        }

        #endregion
    }
}
