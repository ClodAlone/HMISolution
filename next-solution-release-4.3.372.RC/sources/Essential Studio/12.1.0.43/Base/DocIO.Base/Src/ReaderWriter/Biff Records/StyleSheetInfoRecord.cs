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
using System.Diagnostics;
using System.IO;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using FTC = System.UInt16;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// STSHI: STyleSHeet Information, as stored in a file
    /// Note that new fields can be added to the STSHI without invalidating
    /// the file format, because it is stored preceded by it's length.
    /// When reading a STSHI from an older version, new fields will be zero.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class StyleSheetInfoRecord : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// Bit index for StdStyleNamesWritten property.
        /// </summary>
        private const int DEF_BIT_STYLE_NAMES_WRITTEN = 0;
        #endregion

        #region Class members
        /// <summary>
        /// Underlying structure.
        /// </summary>
        private StyleSheetInfoStructure m_structure = new StyleSheetInfoStructure();
        #endregion

        #region Class Properties
        /// <summary>
        /// Count of styles in stylesheet (cstd).
        /// </summary>
        internal ushort StylesCount
        {
            get
            {
                return m_structure.StylesCount;
            }
            set
            {
                m_structure.StylesCount = value;
            }
        }
        /// <summary>
        /// Length of STD Base as stored in a file (cbSTDBaseInFile).
        /// </summary>
        internal ushort STDBaseLength
        {
            get
            {
                return m_structure.STDBaseLength;
            }
            set
            {
                m_structure.STDBaseLength = value;
            }
        }
        /// <summary>
        /// Indicates whether built-in stylenames are stored.
        /// </summary>
        internal bool IsStdStyleNamesWritten
        {
            get
            {
                return m_structure.IsStdStyleNamesWritten;
            }
            set
            {
                m_structure.IsStdStyleNamesWritten = value;
            }
        }
#if DEBUG
        /// <summary>
        /// Returns option flags. Read-only.
        /// </summary>
        internal ushort Options
        {
            get
            {
                return m_structure.Options;
            }
        }
#endif
        /// <summary>
        /// Max sti known when this file was written.
        /// </summary>
        internal ushort StiMaxWhenSaved
        {
            get
            {
                return m_structure.StiMaxWhenSaved;
            }
            set
            {
                m_structure.StiMaxWhenSaved = value;
            }
        }
        /// <summary>
        /// How many fixed-index istds are there?
        /// </summary>
        internal ushort ISTDMaxFixedWhenSaved
        {
            get
            {
                return m_structure.ISTDMaxFixedWhenSaved;
            }
            set
            {
                m_structure.ISTDMaxFixedWhenSaved = value;
            }
        }
        /// <summary>
        /// Current version of built-in stylenames (nVerBuiltInNamesWhenSaved).
        /// </summary>
        internal ushort BuiltInNamesVersion
        {
            get
            {
                return m_structure.BuiltInNamesVersion;
            }
            set
            {
                m_structure.BuiltInNamesVersion = value;
            }
        }
        /// <summary>
        /// ftc used by StandardChpStsh for this document. Read-only.
        /// </summary>
        internal FTC[] StandardChpStsh
        {
            get
            {
                return m_structure.StandardChpStsh;
            }
            set
            {
                m_structure.StandardChpStsh = value;
            }
        }

        /// <summary>
        /// Underlying structure.
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
                return m_structure.Length;
            }
        }
        /// <summary>
        /// Gets and sets the default bidi font name
        /// </summary>
        internal ushort FtcBi
        {
            get
            {
                return m_structure.FtcBi;
            }
            set
            {
                m_structure.FtcBi = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleSheetInfoRecord"/> class.
        /// </summary>
        /// <param name="iSTDBaseLength">Length of the i STD base.</param>
        internal StyleSheetInfoRecord(ushort iSTDBaseLength)
        {
            STDBaseLength = iSTDBaseLength;
            StiMaxWhenSaved = 91;
            ISTDMaxFixedWhenSaved = 15;
            BuiltInNamesVersion = 0;
            IsStdStyleNamesWritten = true;
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        internal StyleSheetInfoRecord(byte[] arrData)
            : base(arrData)
        { }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        internal StyleSheetInfoRecord(byte[] arrData, int iOffset)
            : base(arrData, iOffset)
        { }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="iCount">Number of bytes for the new record.</param>
        internal StyleSheetInfoRecord(byte[] arrData, int iOffset, int iCount)
            : base(arrData, iOffset, iCount)
        { }

        /// <summary>
        /// Creates new record from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal StyleSheetInfoRecord(Stream stream, int iCount)
            : base(stream, iCount)
        { }
        #endregion

        #region Implementation / Overrides
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal override void Parse(byte[] arrData, int iOffset, int iCount)
        {
            base.Parse(arrData, iOffset, iCount);
        }
        #endregion
    }
}