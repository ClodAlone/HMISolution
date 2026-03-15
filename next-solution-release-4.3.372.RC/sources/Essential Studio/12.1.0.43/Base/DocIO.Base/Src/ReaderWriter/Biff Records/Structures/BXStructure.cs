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
    /// Summary description for BXStructure.
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    [CLSCompliant(false)]
    internal class BXStructure : DataStructure
    {
        #region Class constants
        /// <summary>
        /// Size of the record.
        /// </summary>
        internal const int DEF_RECORD_SIZE = 13;
        #endregion

        #region Class members
        /// <summary>
        /// Word offset of the PAPX (PAragraph Property eXception ) recorded
        /// for the paragraph corresponding to the BX.
        /// </summary>
        private byte m_btOffset;

        /// <summary>
        /// PHE structure which stores the current paragraph height for
        /// the paragraph corresponding to the BX.
        /// </summary>
        private ParagraphHeight m_height = new ParagraphHeight();
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal BXStructure()
        {
        }
        #endregion

        #region Class Properties

        /// <summary>
        /// Word offset of the PAPX (PAragraph Property eXception ) recorded
        /// for the paragraph corresponding to the BX.
        /// </summary>
        internal byte Offset
        {
            get
            {
                return m_btOffset;
            }
            set
            {
                m_btOffset = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ParagraphHeight Height
        {
            get
            {
                return m_height;
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

        #region internal methods
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_btOffset = arrData[iOffset];
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
            arrData[iOffset] = m_btOffset;
            iOffset += 1;

            return m_height.Save(arrData, iOffset) + 1;
        }

        /// <summary>
        /// Saves the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Save(BinaryWriter writer)
        {
            writer.Write(m_btOffset);
            ParagraphHeightStructure phs = m_height.Structure;
            writer.Write(phs.Options);
            writer.Write(phs.Width);
            writer.Write(phs.Height);
        }
        #endregion
    }
}
