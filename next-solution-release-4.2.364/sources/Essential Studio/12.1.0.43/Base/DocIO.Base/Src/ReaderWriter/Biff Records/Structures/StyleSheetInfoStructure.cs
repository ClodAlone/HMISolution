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

using FTC = System.UInt16;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// STSHI: STyleSHeet Information, as stored in a file
    /// Note that new fields can be added to the STSHI without invalidating
    /// the file format, because it is stored preceded by it's length.
    /// When reading a STSHI from an older version, new fields will be zero.
    /// </summary>
    [CLSCompliant(false)]
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    internal class StyleSheetInfoStructure : DataStructure
    {
        #region Class constants
        /// <summary>
        /// Bit index for StdStyleNamesWritten property.
        /// </summary>
        private const int DEF_BIT_STYLE_NAMES_WRITTEN = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int DEF_RECORD_SIZE = 20;
        #endregion

        #region Class members
        /// <summary>
        /// Count of styles in stylesheet (cstd).
        /// </summary>
        //    [ FieldOffset( 0 ) ]
        private ushort m_usStylesCount;

        /// <summary>
        /// Length of STD Base as stored in a file.
        /// </summary>
        //    [ FieldOffset( 2 ) ]
        private ushort m_usSTDBaseLength;//cbSTDBaseInFile;

        /// <summary>
        /// Option flags.
        /// </summary>
        //    [ FieldOffset( 4 ) ]
        private ushort m_usOptions = 1;

        /// <summary>
        /// Max sti known when this file was written (StiMaxWhenSaved).
        /// This indicates the last built-in style known to the version of Word
        /// that saved this file.
        /// </summary>
        //    [ FieldOffset( 6 ) ]
        private ushort m_usStiMaxWhenSaved;

        /// <summary>
        /// How many fixed-index istds are there?
        /// </summary>
        //    [ FieldOffset( 8 ) ]
        private ushort m_usISTDMaxFixedWhenSaved;

        /// <summary>
        /// Current version of built-in stylenames (nVerBuiltInNamesWhenSaved).
        /// </summary>
        //    [ FieldOffset( 10 ) ]
        private ushort m_usBuiltInNamesVersion = 4;

        /// <summary>
        /// ftc used by StandardChpStsh for this document (rgftcStandardChpStsh).
        /// </summary>
        //    [ FieldOffset( 12 ) ]
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
#endif
        private FTC[] m_arrStandardChpStsh = new FTC[3];
        /// <summary>
        ///  Represents the sprmCFtcBi for default document formatting
        /// </summary>
        private ushort m_ftcBi;
        #endregion

        #region Class Properties
        /// <summary>
        /// Count of styles in stylesheet (cstd).
        /// </summary>
        internal ushort StylesCount
        {
            get
            {
                return m_usStylesCount;
            }
            set
            {
                m_usStylesCount = value;
            }
        }

        /// <summary>
        /// Length of STD Base as stored in a file (cbSTDBaseInFile).
        /// </summary>
        internal ushort STDBaseLength
        {
            get
            {
                return m_usSTDBaseLength;
            }
            set
            {
                m_usSTDBaseLength = value;
            }
        }

        /// <summary>
        /// Indicates whether built-in stylenames are stored.
        /// </summary>
        internal bool IsStdStyleNamesWritten
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions, DEF_BIT_STYLE_NAMES_WRITTEN);
            }
            set
            {
                m_usOptions = (ushort)BaseWordRecord.SetBit(m_usOptions,
                  DEF_BIT_STYLE_NAMES_WRITTEN, value);
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
                return m_usOptions;
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
                return m_usStiMaxWhenSaved;
            }
            set
            {
                m_usStiMaxWhenSaved = value;
            }
        }

        /// <summary>
        /// How many fixed-index istds are there?
        /// </summary>
        internal ushort ISTDMaxFixedWhenSaved
        {
            get
            {
                return m_usISTDMaxFixedWhenSaved;
            }
            set
            {
                m_usISTDMaxFixedWhenSaved = value;
            }
        }

        /// <summary>
        /// Current version of built-in stylenames (nVerBuiltInNamesWhenSaved).
        /// </summary>
        internal ushort BuiltInNamesVersion
        {
            get
            {
                return m_usBuiltInNamesVersion;
            }
            set
            {
                m_usBuiltInNamesVersion = value;
            }
        }

        /// <summary>
        /// ftc used by StandardChpStsh for this document. Read-only.
        /// </summary>
        internal FTC[] StandardChpStsh
        {
            get
            {
                return m_arrStandardChpStsh;
            }
            set
            {
                if (value == null || value.Length != 3)
                {
                    throw new ArgumentException("Trying to set wrong StandardChpStsh");
                }
                m_arrStandardChpStsh = value;
            }
        }

        /// <summary>
        /// Gets and sets the default Bidi font
        /// </summary>
        internal ushort FtcBi
        {
            get
            {
                return m_ftcBi;
            }
            set
            {
                m_ftcBi = value;
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

        #region Implementation / overrides
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_usStylesCount = ReadUInt16(arrData, ref iOffset);
            m_usSTDBaseLength = ReadUInt16(arrData, ref iOffset);
            m_usOptions = ReadUInt16(arrData, ref iOffset);
            m_usStiMaxWhenSaved = ReadUInt16(arrData, ref iOffset);
            m_usISTDMaxFixedWhenSaved = ReadUInt16(arrData, ref iOffset);
            m_usBuiltInNamesVersion = ReadUInt16(arrData, ref iOffset);

            for (int i = 0; i < 3; i++)
            {
                m_arrStandardChpStsh[i] = ReadUInt16(arrData, ref iOffset);
            }
            //Handled specifically for SILVERLIGHT/WINRT source, to skip parsing of sprmCFtcBi for default document formatting if not defined.
            if (arrData.Length > iOffset + 1)
                m_ftcBi = ReadUInt16(arrData, ref iOffset);
        }

        /// <summary>
        /// Saves the specifies structure.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        /// <param name="iOffset">The offset.</param>
        internal override int Save(byte[] arrData, int iOffset)
        {
            WriteUInt16(arrData, ref iOffset, m_usStylesCount);
            WriteUInt16(arrData, ref iOffset, m_usSTDBaseLength);
            WriteUInt16(arrData, ref iOffset, m_usOptions);
            WriteUInt16(arrData, ref iOffset, m_usStiMaxWhenSaved);
            WriteUInt16(arrData, ref iOffset, m_usISTDMaxFixedWhenSaved);
            WriteUInt16(arrData, ref iOffset, m_usBuiltInNamesVersion);

            for (int i = 0; i < 3; i++)
            {
                WriteUInt16(arrData, ref iOffset, m_arrStandardChpStsh[i]);
            }
            WriteUInt16(arrData, ref iOffset, m_ftcBi);
            return DEF_RECORD_SIZE;
        }
        #endregion
    }
}
