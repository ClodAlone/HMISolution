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

#region File using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
using Syncfusion.DocIO.Utilities;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for FormField.
    /// </summary>
    internal class FormField
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal const int DEF_VALUE = 25;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private FieldType m_fieldType;
        private PICF m_picf;
        private ushort m_params;
        private ushort m_maxLength;
        private ushort m_checkBoxSize;
        private string m_title;
        private string m_defaultTextInputValue;
        private bool m_defaultCheckBoxValue;
        private ushort m_defaultDropDownValue;
        private string m_textFormat;
        private string m_help;
        private string m_tooltip;
        private string m_macroOnStart;
        private string m_macroOnEnd;
        private List<String> m_dropDownItems;
        private bool m_isUnicode;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal FormFieldType FormFieldType
        {
            get
            {
                return
                  (((FormFieldType)m_params) & (FormFieldType.DropDown | FormFieldType.CheckBox));
            }
        }
        /// <summary>
        /// Gets field type.
        /// </summary>
        internal FieldType FieldType
        {
            get
            {
                return m_fieldType;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short Params
        {
            get
            {
                return (short)m_params;
            }
            set
            {
                m_params = (ushort)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int MaxLength
        {
            get
            {
                return m_maxLength;
            }
            set
            {
                m_maxLength = (ushort)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int CheckBoxSize
        {
            get
            {
                return m_checkBoxSize;
            }
            set
            {
                m_checkBoxSize = (ushort)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool Checked
        {
            get
            {
                switch (Value)
                {
                    case 0:
                        {
                            return false;
                        }
                    case 1:
                        {
                            return true;
                        }
                    case 25:
                        {
                            return m_defaultCheckBoxValue;
                        }
                }
                throw new ArgumentException("Unsupported checkbox field value found.");
            }
            set
            {
                Value = (value ? 1 : 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool DefaultCheckBoxValue
        {
            get
            {
                return m_defaultCheckBoxValue;
            }
            set
            {
                m_defaultCheckBoxValue = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int DefaultDropDownValue
        {
            get
            {
                return m_defaultDropDownValue;
            }
            set
            {
                m_defaultDropDownValue = (ushort)value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string DefaultTextInputValue
        {
            get
            {
                return m_defaultTextInputValue;
            }
            set
            {
                m_defaultTextInputValue = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string Format
        {
            get
            {
                return m_textFormat;
            }
            set
            {
                m_textFormat = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int Value
        {
            get
            {
                return ((m_params & 0x7c) >> 2);
            }
            set
            {
                m_params = (ushort)((m_params & -125) | (value << 2));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string Help
        {
            get
            {
                return m_help;
            }
            set
            {
                m_help = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string Tooltip
        {
            get
            {
                return m_tooltip;
            }
            set
            {
                m_tooltip = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string MacroOnStart
        {
            get
            {
                return m_macroOnStart;
            }
            set
            {
                m_macroOnStart = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string MacroOnEnd
        {
            get
            {
                return m_macroOnEnd;
            }
            set
            {
                m_macroOnEnd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int DropDownIndex
        {
            get
            {
                return (Value == DEF_VALUE) ? m_defaultDropDownValue : Value;
                // 	      if ( Value == DEF_VALUE )
                //	      {
                //	        return m_defaultDropDownValue;
                //	      }
                //	      return Value;
            }
            set
            {
                Value = (value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<String> DropDownItems
        {
            get
            {
                return m_dropDownItems;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string DropDownValue
        {
            get
            {
                return m_dropDownItems[DropDownIndex];
            }
            set
            {
                for (int i = 0; i < m_dropDownItems.Count; i++)
                {
#if SILVERLIGHT || WP
          if( string.Compare( m_dropDownItems[ i ], value ) == 0 )
#else
                    if (string.Compare(m_dropDownItems[i], value, true) == 0)
#endif
                    {
                        DropDownIndex = i;
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsCalculateOnExit
        {
            get
            {
                return ((m_params & 0x4000) != 0);
            }
            set
            {
                BaseWordRecord.SetBitsByMask(m_params, 0x4000, 14, value ? 1 : 0);
                //	      m_params = Convert.ToUInt16(Convert((long) m_params, (long) 0x4000, value));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsCheckBoxExplicitSize
        {
            get
            {
                return ((m_params & 0x400) != 0);
            }
            set
            {
                BaseWordRecord.SetBitsByMask(m_params, 0x400, 11, value ? 1 : 0);
                //        m_params = Convert.ToUInt16(Convert((long) m_params, (long) 0x400, value));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsDisabled
        {
            get
            {
                return ((m_params & 0x200) != 0);
            }
            set
            {
                BaseWordRecord.SetBitsByMask(m_params, 0x200, 10, value ? 1 : 0);
                //        m_params = Convert.ToUInt16(Convert((long) m_params, (long) 0x200, value));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TextFormFieldType TextFormFieldType
        {
            get
            {
                return (TextFormFieldType)((m_params & 0x3800) >> 11);
            }
            set
            {
                m_params = (ushort)((m_params & -14337) | (((int)value) << 11));
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldType"></param>
        internal FormField(FieldType fieldType)
        {
            m_fieldType = fieldType;
            switch (fieldType)
            {
                case FieldType.FieldFormTextInput:
                    {
                        m_params = 0;
                        break;
                    }
                case FieldType.FieldFormCheckBox:
                    {
                        m_params = 1;
                        break;
                    }
                case FieldType.FieldFormDropDown:
                    {
                        m_params = 0x8002;
                        m_dropDownItems = new List<String>();
                        break;
                    }
                default:
                    {
                        throw new Exception("Unknown field type.");
                    }
            }
            m_picf = new PICF();
            Value = DEF_VALUE;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="reader"></param>
        internal FormField(FieldType fieldType, BinaryReader reader)
        {
            byte headerByte;
            m_fieldType = fieldType;
            long startPos = reader.BaseStream.Position;
            if (reader.BaseStream.Length > reader.BaseStream.Position)
                m_picf = new PICF(reader);
            else
                m_picf = new PICF();

            if (m_picf.lcb > 68)
            {
                headerByte = reader.ReadByte();

                if (headerByte == 255)
                {
                    reader.BaseStream.Position += 3;
                    headerByte = reader.ReadByte();
                    m_isUnicode = true;
                }
                //      reader.ReadUInt32();
                //      m_params = reader.ReadUInt16();
                //      m_isUnicode = true;

                m_params = reader.ReadByte();

                if (m_isUnicode)
                {
                    m_params = (ushort)((m_params << 8) + headerByte);
                }
                else if (headerByte != 255)
                {
                    m_params += headerByte;
                }

                m_maxLength = reader.ReadUInt16();
                m_checkBoxSize = reader.ReadUInt16();

                if (!m_isUnicode)
                {
                    reader.BaseStream.Position += 2;
                }
                //m_isUnicode = type;
                m_title = ReadString(reader, true);

                switch (fieldType)
                {
                    case FieldType.FieldFormTextInput:
                        {
                            m_defaultTextInputValue = ReadString(reader, true);
                            break;
                        }
                    case FieldType.FieldFormCheckBox:
                        {
                            m_defaultCheckBoxValue = reader.ReadUInt16() != 0;
                            break;
                        }
                    case FieldType.FieldFormDropDown:
                        {
                            m_defaultDropDownValue = reader.ReadUInt16();
                            break;
                        }
                    case FieldType.FieldLink:
                        throw new NotImplementedException("Link fields are not yet supported");
                }

                m_textFormat = ReadString(reader, true);
                m_help = ReadString(reader, true);
                m_tooltip = ReadString(reader, true);
                m_macroOnStart = ReadString(reader, true);
                m_macroOnEnd = ReadString(reader, true);

                if (fieldType == FieldType.FieldFormDropDown)
                {
                    reader.ReadUInt16();
                    int count = reader.ReadInt32();
                    m_dropDownItems = new List<String>();
                    for (int i = 0; i < count; i++)
                    {
                        m_dropDownItems.Add(ReadString(reader, false));
                    }
                }
                long length = reader.BaseStream.Position - startPos;

                if (m_picf.lcb > length)
                {
                    reader.BaseStream.Position = startPos + m_picf.lcb;
                    return;
                }
                if (m_picf.lcb != length)
                {
                    throw new ArgumentException("Unrecognized format of the form field.");
                }
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            long startPos = writer.BaseStream.Position;
            m_picf.Write(stream);
            writer.Write(uint.MaxValue);
            writer.Write(m_params);
            writer.Write(m_maxLength);
            writer.Write(m_checkBoxSize);
            WriteString(m_title, writer, true);
            switch (m_fieldType)
            {
                case FieldType.FieldFormTextInput:
                    {
                        WriteString(m_defaultTextInputValue, writer, true);
                        break;
                    }
                case FieldType.FieldFormCheckBox:
                    {
                        writer.Write(m_defaultCheckBoxValue ? ((ushort)1) : ((ushort)0));
                        break;
                    }
                case FieldType.FieldFormDropDown:
                    {
                        writer.Write(m_defaultDropDownValue);
                        break;
                    }
                default:
                    {
                        throw new Exception("Unsupported form field type.");
                    }
            }
            WriteString(m_textFormat, writer, true);
            WriteString(m_help, writer, true);
            WriteString(m_tooltip, writer, true);
            WriteString(m_macroOnStart, writer, true);
            WriteString(m_macroOnEnd, writer, true);
            if (m_fieldType == FieldType.FieldFormDropDown)
            {
                writer.Write((ushort)0xffff);
                writer.Write((uint)m_dropDownItems.Count);
                foreach (string text in m_dropDownItems)
                {
                    WriteString(text, writer, false);
                }
            }
            long endPos = writer.BaseStream.Position;
            m_picf.lcb = (int)(endPos - startPos);
            writer.BaseStream.Position = startPos;
            m_picf.Write(stream);
            writer.BaseStream.Position = endPos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="readZero"></param>
        /// <returns></returns>
        internal static string ReadUnicodeString(BinaryReader binaryReader, bool readZero)
        {
            int length = binaryReader.ReadInt16();
            byte[] buf = binaryReader.ReadBytes(length * 2);
#if SILVERLIGHT || WP
      string text = Encoding.Unicode.GetString( buf, 0, buf.Length );
#else
            string text = Encoding.Unicode.GetString(buf);
#endif
            if (readZero)
            {
                binaryReader.ReadInt16();
            }
            return text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="readZero"></param>
        /// <returns></returns>
        internal string ReadString(BinaryReader binaryReader, bool readZero)
        {
            int length = m_isUnicode ? binaryReader.ReadInt16() * 2 : binaryReader.ReadByte();
            byte[] buf = binaryReader.ReadBytes(length);
            string text = string.Empty;
            if (m_isUnicode)
            {
#if SILVERLIGHT || WP
                text = Encoding.Unicode.GetString( buf, 0, buf.Length );
#else
                text = Encoding.Unicode.GetString(buf);
#endif
            }
            else
            {
                text = DocIOEncoding.GetString(buf);
            }
            //Encoding enc = m_isUnicode ? Encoding.Unicode : Encoding.ASCII;
            //string text = enc.GetString(buf);
            if (readZero)
            {
                int sep = m_isUnicode ? binaryReader.ReadInt16() : binaryReader.ReadByte();
            }
            return text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="writer"></param>
        /// <param name="writeSeparator"></param>
        internal static void WriteString(string text, BinaryWriter writer, bool writeSeparator)
        {
            string value = (text != null) ? text : "";
            writer.Write((short)value.Length);
            writer.Write(Encoding.Unicode.GetBytes(value));
            if (writeSeparator)
            {
                writer.Write((short)0);
            }
        }
        #endregion
    }
}
