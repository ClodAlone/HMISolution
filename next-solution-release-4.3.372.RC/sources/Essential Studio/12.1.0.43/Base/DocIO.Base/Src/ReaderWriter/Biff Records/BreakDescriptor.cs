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
//using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
//using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
//using Syncfusion.DocIO.IO.Stream.Win32;

using TableEntry = Syncfusion.DocIO.ReaderWriter.Biff_Records.BreakDescriptorRecord;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for BreakDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class BreakDescriptor : BaseWordRecord
    {
        #region Class constants
        internal const int DEF_BKD_SIZE = 6;
        #endregion

        #region Class members
        /// <summary>
        /// except in textbox BKD, index to PGD in plfpgd that describes the page this break is on.
        /// </summary>
        internal short m_ipgd;
        /// <summary>
        /// number of cp's considered for this break; note that the CP's described by cpDepend in this break reside in the next BKD
        /// </summary>
        internal short m_dcpDepend;
        /// <summary>
        /// 
        /// </summary>
        internal byte m_iCol;
        /// <summary>
        /// Option flags.
        /// </summary>
        internal byte m_options;
        #endregion

        #region Class properties
        /// <summary>
        /// Index to PGD in plfpgd.
        /// </summary>
        internal short Ipgd
        {
            get
            {
                return m_ipgd;
            }
            set
            {
                m_ipgd = value;
            }
        }
        /// <summary>
        /// Number of cp's considered for this break.
        /// </summary>
        internal short DcpDepend
        {
            get
            {
                return m_dcpDepend;
            }
            set
            {
                m_dcpDepend = value;
            }
        }
        /// <summary>
        /// Options
        /// </summary>
        internal byte Options
        {
            get
            {
                return m_options;
            }
            set
            {
                m_options = value;
            }
        }
        /// <summary>
        /// Get/set table break option.
        /// </summary>
        internal bool TableBreak
        {
            get
            {
                return (m_options & 0x01) != 0;
            }
            set
            {
                if (value)
                {
                    m_options = (byte)(m_options | 0x01);
                }
                else
                {
                    m_options = (byte)(m_options & (~0x01));
                }
            }
        }
        /// <summary>
        /// Get/set column brake option.
        /// </summary>
        internal bool ColumnBreak
        {
            get
            {
                return ((m_options & 0x02) >> 1) != 0;
            }
            set
            {
                if (value)
                {
                    m_options = (byte)(m_options | 0x02);
                }
                else
                {
                    m_options = (byte)(m_options & (~0x02));
                }
            }
        }
        /// <summary>
        /// Get/set Marked option.
        /// </summary>
        internal bool Marked
        {
            get
            {
                return ((m_options & 0x04) >> 2) != 0;
            }
            set
            {
                if (value)
                {
                    m_options = (byte)(m_options | 0x04);
                }
                else
                {
                    m_options = (byte)(m_options & (~0x04));
                }
            }
        }
        /// <summary>
        /// Get/set fUnk option
        /// in textbox BKD, when == 1 indicates cpLim of this textbox is not valid.
        /// </summary>
        internal bool Unk
        {
            get
            {
                return ((m_options & 0x08) >> 3) != 0;
            }
            set
            {
                if (value)
                {
                    m_options = (byte)(m_options | 0x08);
                }
                else
                {
                    m_options = (byte)(m_options & (~0x08));
                }
            }
        }
        /// <summary>
        /// Get/set text overflows the end of this textbox option
        /// </summary>
        internal bool TextOverflow
        {
            get
            {
                return ((m_options & 0x10) >> 4) != 0;
            }
            set
            {
                if (value)
                {
                    m_options = (byte)(m_options | 0x10);
                }
                else
                {
                    m_options = (byte)(m_options & (~0x10));
                }
            }
        }

        #endregion

        #region Class Initialize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        internal BreakDescriptor()
        { }
        /// <summary>
        /// Constructor with table stream parametr
        /// </summary>
        /// <param name="stream">table stream</param>
        internal BreakDescriptor(Stream stream)
        {
            Read(stream);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Fill class fields
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            m_ipgd = ReadInt16(stream);
            m_dcpDepend = ReadInt16(stream);
            m_iCol = (byte)stream.ReadByte();
            m_options = (byte)stream.ReadByte();
        }
        /// <summary>
        /// Write break descriptor structure to stream
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            WriteInt16(stream, m_ipgd);
            WriteInt16(stream, m_dcpDepend);
            stream.WriteByte(m_iCol);
            stream.WriteByte(m_options);
        }
        #endregion
    }
}
