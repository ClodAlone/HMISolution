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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for FieldDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class FieldDescriptor : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private byte m_ch;
        /// <summary>
        /// 
        /// </summary>
        private byte m_reserved;
        /// <summary>
        /// 
        /// </summary>
        private byte m_fieldType;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal bool HasSeparator
        {
            get
            {
                return ((m_fieldType & 0x80) != 0);
            }
            set
            {
                m_fieldType = (byte)SetBit(m_fieldType, 7, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsResultDirty
        {
            get
            {
                return ((m_fieldType & 4) != 0);
            }
            set
            {
                m_fieldType = (byte)SetBit(m_fieldType, 2, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsResultEdited
        {
            get
            {
                return ((m_fieldType & 8) != 0);
            }
            set
            {
                m_fieldType = (byte)SetBit(m_fieldType, 3, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsNested
        {
            get
            {
                return ((m_fieldType & 0x40) != 0);
            }
            set
            {
                m_fieldType = (byte)SetBit(m_fieldType, 6, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal FieldType Type
        {
            get
            {
                return (FieldType)m_fieldType;
            }
            set
            {
                m_fieldType = (byte)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte FieldBoundary
        {
            get
            {
                return m_ch;
            }
            set
            {
                m_ch = value;
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal FieldDescriptor(BinaryReader reader)
        {
            Read(reader);
        }
        /// <summary>
        /// 
        /// </summary>
        internal FieldDescriptor()
        { }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sh"></param>
        internal void Parse(short sh)
        {
            byte[] buf = BitConverter.GetBytes(sh);
            m_ch = (byte)(buf[0] & 0x1f);
            m_reserved = (byte)(buf[0] & 0xe0);
            m_fieldType = buf[1];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal short Save()
        {
            byte[] buf = new byte[2];
            buf[0] = (byte)(m_ch | m_reserved);
            buf[1] = (m_fieldType);
            return BitConverter.ToInt16(buf, 0);

        }
        /// <summary>
        /// Clone field
        /// </summary>
        /// <returns></returns>
        internal FieldDescriptor Clone()
        {
            FieldDescriptor fld = new FieldDescriptor();
            fld.HasSeparator = HasSeparator;
            fld.IsNested = IsNested;
            fld.IsResultDirty = IsResultDirty;
            fld.IsResultEdited = IsResultEdited;
            fld.FieldBoundary = FieldBoundary;
            fld.Type = Type;

            return fld;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal void Read(BinaryReader reader)
        {
            byte[] arr = reader.ReadBytes(2);
            m_ch = (byte)(arr[0] & 0x1f);
            m_reserved = (byte)(arr[0] & 0xe0);
            m_fieldType = arr[1];
        }
        /// <summary>
        /// Write FieldDescriptor to stream
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            stream.WriteByte((byte)((m_ch | m_reserved)));
            stream.WriteByte(m_fieldType);
        }
        #endregion
    }

}
