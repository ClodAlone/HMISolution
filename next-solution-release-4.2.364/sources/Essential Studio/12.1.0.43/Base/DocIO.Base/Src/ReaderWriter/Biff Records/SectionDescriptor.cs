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

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for SectionDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    //  [ StructLayout( LayoutKind.Sequential ) ]
    //  [ StructLayout( LayoutKind.Explicit ) ]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SectionDescriptor : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Underlying structure.
        /// </summary>
        private SectionDescriptorStructure m_structure = new SectionDescriptorStructure();
        #endregion

        #region Class constants
        /// <summary>
        /// Size of the record.
        /// </summary>
        internal const int DEF_RECORD_SIZE = 12;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal SectionDescriptor()
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal SectionDescriptor(byte[] arrData)
            : base(arrData)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="iOffset"></param>
        /// <param name="converter"></param>
        internal SectionDescriptor(byte[] arrData, int iOffset)
            : base(arrData, iOffset)
        {
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// used internally by Word (fn).
        /// </summary>
        internal short Internal1
        {
            get
            {
                return m_structure.Internal1;
            }
            set
            {
                m_structure.Internal1 = value;
            }
        }
        /// <summary>
        /// used internally by Word (fnMpr).
        /// </summary>
        internal short Internal2
        {
            get
            {
                return m_structure.Internal2;
            }
            set
            {
                m_structure.Internal2 = value;
            }
        }
        /// <summary>
        /// File offset in main stream to beginning of SEPX stored for section.
        /// If sed.fcSepx == 0xFFFFFFFF, the section properties for the section are equal
        /// to the standard SEP (see SEP definition).
        /// </summary>
        internal uint SepxPosition
        {
            get
            {
                return m_structure.SepxPosition;
            }
            set
            {
                m_structure.SepxPosition = value;
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
                return m_structure.MacPrintOffset;
            }
            set
            {
                m_structure.MacPrintOffset = value;
            }
        }
        /// <summary>
        /// Returns underlying class or structure.
        /// </summary>
        protected override DataStructure UnderlyingStructure
        {
            get
            {
                return m_structure;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return DEF_RECORD_SIZE;
            }
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal override void Parse(byte[] arrData, int iOffset, int iCount)
        {
            m_structure.Parse(arrData, iOffset);
        }
        #endregion
    }
}
