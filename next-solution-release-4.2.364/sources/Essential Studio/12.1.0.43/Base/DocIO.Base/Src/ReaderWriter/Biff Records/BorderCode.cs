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

using System;
using System.IO;
using System.Runtime.InteropServices;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#if !WINRT && !WP
using System.Drawing;
#endif

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for BRC.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class BorderCode : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private int DEF_NEW_BRC_LENGTH = 8;
        #endregion

        #region Class members
        /// <summary>
        /// width of a single line in 1/8 pt, max of 32 pt.
        /// </summary>
        private byte m_dptLineWidth = 0;
        /// <summary>
        /// border type code:
        /// </summary>
        private byte m_brcType = 0;
        /// <summary>
        /// color code
        /// </summary>
        private byte m_colorId = 0;
        /// <summary>
        /// 
        /// </summary>
        private Color m_colorExt = Color.Empty;
        /// <summary>
        /// :5 1F00 width of space to maintain between border and text within border. 
        ///         Must be 0 when BRC is a substructure of TC. Stored in points. 
        /// :1 2000 when 1, border is drawn with shadow. Must be 0 when BRC is a substructure of the TC 
        /// </summary>
        private byte m_props = 0;
        private bool m_default = true;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="iOffset"></param>
        internal BorderCode(byte[] arr, int iOffset)
        {
            Parse(arr, iOffset);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Parse byte array to brc structure
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="iOffset"></param>
        internal void Parse(byte[] arr, int iOffset)
        {
            m_dptLineWidth = arr[iOffset + 0];
            m_brcType = arr[iOffset + 1];
            m_colorId = arr[iOffset + 2];
            m_props = arr[iOffset + 3];
            m_default = false;
        }
        /// <summary>
        /// Parse byte array to brc structure
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="iOffset"></param>
        internal void ParseNewBrc(byte[] arr, int iOffset)
        {
            if (arr.Length - iOffset >= DEF_NEW_BRC_LENGTH)
            {
                m_colorExt = WordColor.ConvertRGBToColor(BitConverter.ToUInt32(arr, iOffset));
                //        m_colorExt = Color.FromArgb( ( int )BitConverter.ToUInt32( arr, iOffset ) );
                m_dptLineWidth = arr[iOffset + 4];
                m_brcType = arr[iOffset + 5];
                m_props = arr[iOffset + 6];
                int unused = arr[iOffset + 7];
                m_default = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="iOffset"></param>
        internal void Save(byte[] arr, int iOffset)
        {
            arr[iOffset + 0] = m_dptLineWidth;
            arr[iOffset + 1] = m_brcType;
            arr[iOffset + 2] = m_colorId;
            arr[iOffset + 3] = m_props;
            m_default = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="iOffset"></param>
        internal void SaveNewBrc(byte[] arr, int iOffset)
        {
            byte[] bytes = BitConverter.GetBytes(WordColor.ConvertColorToRGB(m_colorExt));
            //      byte[] bytes = BitConverter.GetBytes( m_colorExt.ToArgb() );
            bytes.CopyTo(arr, iOffset);
            arr[iOffset + 4] = m_dptLineWidth;
            arr[iOffset + 5] = m_brcType;
            arr[iOffset + 6] = m_props;
            arr[iOffset + 7] = 0;
            m_default = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal void Read(BinaryReader reader)
        {
            m_dptLineWidth = (byte)reader.ReadByte();
            if (m_dptLineWidth != 0xff)
            {
                m_brcType = (byte)reader.ReadByte();
                m_colorId = (byte)reader.ReadByte();
                m_props = (byte)reader.ReadByte();
                m_default = false;
            }
            else
            {
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            if (!IsClear)
            {
                stream.WriteByte(m_dptLineWidth);
                stream.WriteByte(m_brcType);
                stream.WriteByte(m_colorId);
                stream.WriteByte(m_props);
            }
            else
            {
                WriteUInt32(stream, uint.MaxValue);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal BorderCode Clone()
        {
            return base.MemberwiseClone() as BorderCode;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Width of a Line 
        /// </summary>
        internal byte LineWidth
        {
            get
            {
                return m_dptLineWidth;
            }
            set
            {
                m_dptLineWidth = value;
                m_default = false;
            }
        }
        /// <summary>
        /// Border type code
        /// </summary>
        internal byte BorderType
        {
            get
            {
                return m_brcType;
            }
            set
            {
                m_brcType = value;
                m_default = false;
            }
        }
        /// <summary>
        /// Width of space to maintain between border and text within border
        /// </summary>
        internal byte Space
        {
            get
            {
                return (byte)(m_props & 0x1F);
            }
            set
            {
                byte res = value;
                m_props = (byte)(m_props & 0xE0);
                m_props += res;
                m_default = false;
            }
        }
        /// <summary>
        /// when true, border is drawn with shadow
        /// </summary>
        internal bool Shadow
        {
            get
            {
                byte val = (byte)(m_props & 0x20);
                val = (byte)(val >> 5);
                return (val == 1);
            }
            set
            {
                byte res = (value == true) ? (byte)1 : (byte)0;
                m_props = (byte)(m_props & 0xDF);
                res = (byte)(res << 5);
                m_props += res;
                m_default = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte LineColor
        {
            get
            {
                return m_colorId;
            }
            set
            {
                m_colorId = value;
                m_default = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Color LineColorExt
        {
            get
            {
                if (m_colorExt == Color.Empty)
                {
                    return WordColor.ConvertIdToColor(m_colorId);
                }
                return m_colorExt;
            }
            set
            {
                m_colorExt = value;
                m_default = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsDefault
        {
            get
            {
                return m_default;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsClear
        {
            get
            {
                return m_dptLineWidth == 255;
            }
        }
        #endregion
    }
}
