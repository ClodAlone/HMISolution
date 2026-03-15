#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.IO;
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Security;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Primitives
{
    /// <summary>
    /// Implements PDF string object.
    /// </summary>
#if NETFX_CORE || WP
    public class PdfString:
#else
    internal class PdfString : 
#endif
        IPdfPrimitive, IPdfDecryptable
    {
        #region Constants
        /// <summary>
        /// General markers for string.
        /// </summary>
        public const string StringMark = "()";
        /// <summary>
        /// Hex markers for string.
        /// </summary>
        public const string HexStringMark = "<>";
        /// <summary>
        /// Format of password data.
        /// </summary>
        private const string HexFormatPattern = "{0:X2}";
        #endregion

        #region Fields
        /// <summary>
        /// Value indicating whether the string was converted to hex.
        /// </summary>
        private bool m_bHex = false;
        /// <summary>
        /// Value of the object.
        /// </summary>
        private string m_value;
        /// <summary>
        /// The byte data of the string.
        /// </summary>
        private byte[] m_data;
        /// <summary>
        /// Indicates whether to check if the value has unicode characters.
        /// </summary>
        private bool m_bConverted;
        /// <summary>
        /// Indicates whether we should convert data to Unicode.
        /// </summary>
        private ForceEncoding m_bForceEncoding;
        /// <summary>
        /// Shows if the data of the stream was decrypted.
        /// </summary>
        private bool m_bDecrypted = false;
        ///// <summary>
        ///// Shows the type of data source;
        ///// </summary>
        //private SourceType m_sourceType = SourceType.StringValue;
        /// <summary>
        /// Shows the type of object status whether it is object registered or other status;
        /// </summary>
        private ObjectStatus m_status;
        /// <summary>
        /// Indicates if the object is currently in saving state or not.
        /// </summary>
        private bool m_isSaving;
        /// <summary>
        /// Holds the index number of the object.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Internal variable to store the position.
        /// </summary>
        private int m_position = -1;
        /// <summary>
        /// Shows if the data of the stream was decrypted.
        /// </summary>
        private bool m_isParentDecrypted = false;
        /// <summary>
        /// Internal variable to hold PdfCrossTable reference.
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Internal variable to hold cloned object.
        /// </summary>
        private PdfString m_clonedObject = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets string value of the object.
        /// </summary>
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (value != m_value)
                {
                    m_value = value;
                    m_data = null;
                    //m_sourceType = SourceType.StringValue;
                    Encode = ForceEncoding.None;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether string is in hex.
        /// </summary>
        internal bool Hex
        {
            get
            {
                return m_bHex;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check if the value
        ///  has unicode characters.
        /// </summary>
        internal bool Converted
        {
            get
            {
                return m_bConverted;
            }
            set
            {
                if (!m_bHex)
                {
                    m_bConverted = value;
                }

            }
        }

        /// <summary>
        /// Gets or sets value indicating whether we should convert data to Unicode.
        /// </summary>
        internal ForceEncoding Encode
        {
            get
            {
                return m_bForceEncoding;
            }
            set
            {
                m_bForceEncoding = value;
            }
        }

        /// <summary>
        /// Gets a flag that shows if the object has been decrypted already.
        /// </summary>
        public bool Decrypted
        {
            get
            {
                return m_bDecrypted;
            }
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        internal byte[] Bytes
        {
            get
            {
                if (m_data == null)
                {
                    m_data = GetBytes();
                }

                return m_data;
            }
        }

        /// <summary>
        /// Gets or sets the Status of the specified object.
        /// </summary>
        public ObjectStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this document is saving or not.
        /// </summary>
        public bool IsSaving
        {
            get
            {
                return m_isSaving;
            }
            set
            {
                m_isSaving = value;
            }
        }

        /// <summary>
        /// Gets or sets the integer value of the specified object.
        /// </summary>
        public int ObjectCollectionIndex
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        public int Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        /// <summary>
        /// Gets or set the flag indicating whether the string in the stream is decrypted or not.
        /// </summary>
        internal bool IsParentDecrypted
        {
            get
            {
                return m_isParentDecrypted;
            }
            set
            {
                m_isParentDecrypted = value;
            }
        }

        /// <summary>
        /// Returns PdfCrossTable associated with the object.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
        }

        /// <summary>
        /// Returns cloned object.
        /// </summary>
        public IPdfPrimitive ClonedObject
        {
            get
            {
                return m_clonedObject;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfString"/> class.
        /// </summary>
        public PdfString()
        {
        }

        /// <summary>
        /// Creates new PDF string object.
        /// </summary>
        /// <param name="value">Value of the object.</param>
        public PdfString(string value)
            : base()
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length > 0 && value[0] == 0xfeff)
            {
                m_value = value.Substring(1);
            }
            else
            {
                m_value = value;
                m_data = GetAsciiBytes(value);
            }
        }

        /// <summary>
        /// Initialize a string from a hex string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="encrypted">if set to <c>true</c> the string has been encrypted.</param>
        public PdfString(string value, bool encrypted)
            : base()
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!encrypted && !(value == string.Empty))
            {
                m_data = HexToBytes(value);
                if (m_data.Length != 0)
                {
                    if (m_data[0] == 0xfe && m_data[1] == 0xff) // UTF-16 BE string.
                    {
                        m_value = Encoding.BigEndianUnicode.GetString(m_data, 2, m_data.Length - 2);
                    }
                    else
                    {
                        m_value = ByteToString(m_data);
                    }
                }
                else
                {
                    m_value = value;
                }
            }
            else
            {
                m_value = value;
            }

            m_bHex = true;
        }

        /// <summary>
        /// Creates new PDF string object.
        /// </summary>
        /// <param name="value">Value of the object.</param>
        public PdfString(byte[] value)
            : base()
        {
            if (value == null)
                throw new ArgumentNullException("value");

            m_data = value;

            if (m_data[0] == 0xfe && m_data[1] == 0xff) // UTF-16 BE string.
            {
                m_value = Encoding.BigEndianUnicode.GetString(m_data, 2, m_data.Length - 2);
            }
            else
            {
                m_value = ByteToString(m_data);
            }

            m_bHex = true;
            //m_sourceType = SourceType.ByteBuffer;
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Converts byte data to string.
        /// </summary>
        /// <param name="data">Bytes to be converted.</param>
        /// <returns>Destination string.</returns>
        public static string ByteToString(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("stream");

            return ByteToString(data, data.Length);
        }

        /// <summary>
        /// Converts byte data to string.
        /// </summary>
        /// <param name="data">Bytes to be converted.</param>
        /// <param name="length">The actual length of the buffer.</param>
        /// <returns>Destination string.</returns>
        internal static string ByteToString(byte[] data, int length)
        {
            if (data == null)
                throw new ArgumentNullException("stream");

            if (length > data.Length)
                throw new ArgumentOutOfRangeException(
                    "length", "The length can't be more then the array lenght.");

            char[] buf = new char[length];

            for (int i = 0, size = length; i < size; ++i)
            {
                buf[i] = (char)data[i];
            }

            return new string(buf);
        }

        /// <summary>
        /// Determines if the string is a unicode one.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>True if string is in Unicode format; otherwise False.</returns>
        public static bool IsUnicode(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return Encoding.UTF8.GetByteCount(value) != value.Length;
        }

        /// <summary>
        /// Converts string to array of unicode symbols.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <param name="bAddPrefix">Indicates whether we should add Unicode prefix to output data.</param>
        /// <returns>Array of data in unicode format.</returns>
        public static byte[] ToUnicodeArray(string value, bool bAddPrefix)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            int size = Encoding.BigEndianUnicode.GetByteCount(value);
            byte[] preamble = null;
            int startIndex = 0;

            if (bAddPrefix)
            {
                preamble = Encoding.BigEndianUnicode.GetPreamble();
                startIndex = preamble.Length;
                size += startIndex;
            }

            byte[] output = new byte[size];

            if (bAddPrefix)
            {
                preamble.CopyTo(output, 0);
            }

            Encoding.BigEndianUnicode.GetBytes(value, 0, value.Length,
                output, startIndex);

            return output;
        }

        /// <summary>
        /// FromDate in PDF suitable form.
        /// </summary>
        /// <param name="dateTime">The datetime.</param>
        /// <returns></returns>
        public static string FromDate(DateTime dateTime)
        {
            return dateTime.ToString("D:yyyyMMddHHmmss");
        }

        /// <summary>
        /// Compare two PDF strings by their bytes.
        /// </summary>
        /// <param name="str1">The first string.</param>
        /// <param name="str2">The second string.</param>
        /// <returns>If the first string is greater then the second one it returns a value greater then 0,
        /// if the second string is greater then the first one, it returns a value lower then 0,
        /// if both are equal the 0 is returned.</returns>
        internal static int ByteCompare(PdfString str1, PdfString str2)
        {
            byte[] data1 = str1.Bytes;
            byte[] data2 = str2.Bytes;

            int commonSize = Math.Min(data1.Length, data2.Length);
            int result = 0;

            for (int i = 0; i < commonSize; ++i)
            {
                byte byte1 = data1[i];
                byte byte2 = data2[i];

                result = byte1 - byte2;

                if (result != 0)
                    break;
            }

            if (result == 0)
            {
                result = data1.Length - data2.Length;
            }

            return result;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Explicit operator. Converts system string into PdfString.
        /// </summary>
        /// <param name="str">The system string.</param>
        /// <returns>Properly initialized PdfString.</returns>
        public static explicit operator PdfString(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            PdfString s = new PdfString(str);

            return s;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Converts PDFString to string.
        /// </summary>
        /// <returns>Byte array containing PDF representation of this object.</returns>
        internal byte[] PdfEncode(PdfDocumentBase document)
        {
            byte[] data = null;

            if (!Hex)
            {
                data = GetBytes();
#if SILVERLIGHT || NETFX_CORE || WP
                data = EscapeSymbols(data);
#else
                PdfSecurity security = (document == null) ? null : document.Security as PdfSecurity;

                if (security == null || !security.Enabled)
                {
                    StringBuilder colorCheck = new StringBuilder();
                    if (data.Length > 10)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            colorCheck.Append(Convert.ToChar(data[i]));
                        }
                    }
                    if (colorCheck.ToString() == "ColorFound")
                    {
                        byte[] tempData = new byte[data.Length - 10];
                        for (int i = 0; i < data.Length -10; i++)
                        {
                            tempData[i] = data[i + 10];
                        }
                        data = new byte[data.Length - 10];
                        data = tempData;
                    }
                    else
                    data = EscapeSymbols(data);
                }
#endif

            }
            else
            {
                if (m_data == null)
                {
                    data = GetAsciiBytes(Value);
                }
                else
                {
                    data = GetAsciiBytes(BytesToHex(m_data));
                }
            }

            MemoryStream ms = new MemoryStream(data.Length + 2);
            string markers = (Hex) ? HexStringMark : StringMark;

            bool hex = false;
            data = EncryptIfNeeded(data, document);
            for (int i = 0; i < data.Length; i++)
            {
                if ((data[i] >= 48 && data[i] <= 57) || ((data[i] >= 65 && data[i] <= 70) || (data[i] >= 97 && data[i] <= 102)))
                {
                    hex = true;
                }
                else
                {
                    hex = false;
                }
            }
            if (Hex)
            {
                if (!hex)
                {
                    data = GetAsciiBytes(BytesToHex(data));
                }
            }
            ms.WriteByte((byte)markers[0]);

            ms.Write(data, 0, data.Length);
            ms.WriteByte((byte)markers[1]);

            byte[] buff = ms.ToArray();
            ms.Dispose();

            return buff;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Gets ascii bytes for specified string value.
        /// </summary>
        /// <param name="value">String for which to get bytes.</param>
        /// <returns>Bytes retrieved from specified text.</returns>
        private static byte[] GetAsciiBytes(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            byte[] bytes = new byte[value.Length];

            for (int i = 0, len = value.Length; i < len; ++i)
            {
                bytes[i] = (byte)value[i];
            }

            return bytes;
        }

        /// <summary>
        /// Converts bytes to string using hex format for representing string.
        /// </summary>
        /// <param name="bytes">Bytes to be converted.</param>
        /// <returns>String int hex format.</returns>
        internal static string BytesToHex(byte[] bytes)
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            for (int i = 0, len = bytes.Length; i < len; ++i)
            {
                builder.AppendFormat(HexFormatPattern, bytes[i]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// If needed encryption of data, encrypts data and returns new content.
        /// </summary>
        /// <param name="data">Bytes to be encrypted.</param>
        /// <param name="document">A PDF document.</param>
        /// <returns>Encrypted data.</returns>
        private byte[] EncryptIfNeeded(byte[] data, PdfDocumentBase document)
        {
            if (data == null)
                throw new ArgumentNullException("data");
#if SILVERLIGHT || NETFX_CORE || WP
            return data;
#else
            PdfSecurity security = (document == null) ? null : document.Security as PdfSecurity;

            if (security == null || !(security.Enabled))
            {
                return data;
            }
            else
            {
                long key = document.CurrentSavingObj.ObjNum;
                data = security.Encryptor.EncryptData(key, data,true);
            }
#endif
            return EscapeSymbols(data);
        }

        /// <summary>
        /// Escapes special symbols.
        /// </summary>
        /// <param name="data">Data from which must be escaped special symbols.</param>
        /// <returns>Data from which are escaped special symbols.</returns>
        internal static byte[] EscapeSymbols(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            MemoryStream ms = new MemoryStream();

            byte bt;

            for (int i = 0, len = data.Length; i < len; i++)
            {
                bt = data[i];

                switch (bt)
                {
                    case 40:
                    case 41:
                        ms.WriteByte(92);
                        ms.WriteByte(bt);
                        break;

                    case 13:
                        ms.WriteByte(92);
                        ms.WriteByte(114);
                        break;

                    case 0x3e:
                    case 92:
                        ms.WriteByte(92);
                        ms.WriteByte(bt);
                        break;

                    default:
                        ms.WriteByte(bt);
                        break;
                }
            }

            byte[] result = PdfStream.StreamToBytes(ms);
            ms.Dispose();

            return result;
        }

        /// <summary>
        /// Converts hexadecimal digits into a byte array.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>The byte array.</returns>
        public byte[] HexToBytes(string value)
        {
            List<byte> hexNumbers = new List<byte>(value.Length);

            foreach (char c in value)
            {
                if (Char.IsLetterOrDigit(c))
                {
                    byte digit = ParseHex(c);
                    hexNumbers.Add(digit);
                }
            }

            byte[] result = HexDigitsToNumbers(hexNumbers);

            return result;
        }

        /// <summary>
        /// Parses the hex.
        /// </summary>
        /// <param name="c">The character representing a hex digit.</param>
        /// <returns>A byte value.</returns>
        private byte ParseHex(char c)
        {
            byte value = 0;

            if (c >= '0' && c <= '9')
            {
                value = (byte)(c - '0');
            }
            else if (c >= 'A' && c <= 'F')
            {
                value = (byte)(c - 'A' + 10);
            }
            else if (c >= 'a' && c <= 'f')
            {
                value = (byte)(c - 'a' + 10);
            }

            return value;
        }

        /// <summary>
        /// Converts hex digits into byte numbers.
        /// </summary>
        /// <param name="hexNumbers">The hex numbers.</param>
        /// <returns>The byte array.</returns>
        private byte[] HexDigitsToNumbers(List<byte> hexNumbers)
        {
            byte value = 0;
            bool start = true;
            List<byte> list = new List<byte>(hexNumbers.Count / 2 + 1);

            foreach (byte digit in hexNumbers)
            {
                if (start)
                {
                    value = (byte)(digit << 4);
                    start = false;
                }
                else
                {
                    value += digit;
                    list.Add(value);
                    start = true;
                }
            }

            if (!start)
            {
                list.Add(value);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Converts the Value to array of bytes.
        /// </summary>
        /// <returns>An array of bytes.</returns>
        private byte[] GetBytes()
        {
            bool unicode = (!Converted && IsUnicode(m_value));

            if (Encode == ForceEncoding.ASCII)
            {
                unicode = false;
            }
            else if (Encode == ForceEncoding.Unicode)
            {
                unicode = true;
            }

            return GetBytes(unicode);
        }

        /// <summary>
        /// Returns a byta array based on the value.
        /// </summary>
        /// <param name="unicode">Shows if should be unicode encoding.</param>
        /// <returns>The byte array.</returns>
        private byte[] GetBytes(bool unicode)
        {
            byte[] data = (unicode) ?
                ToUnicodeArray(m_value, !Converted) : GetAsciiBytes(m_value);

            return data;
        }
        #endregion

        #region IPdfSavable Members
        /// <summary>
        /// Saves the object using the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Save(IPdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(PdfEncode(writer.Document));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Forces PdfString to the hex mode.
        /// </summary>
        internal void ToHex()
        {
            if (!Hex)
            {
                byte[] buf = GetBytes();

                Value = BytesToHex(buf);
                m_bHex = true;
            }
        }

        /// <summary>
        /// Creates a copy of PdfString.
        /// </summary>
        public IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            if (m_clonedObject != null && m_clonedObject.CrossTable == crossTable)
                return m_clonedObject;
            else
                m_clonedObject = null;

            // Else clone the object.
            PdfString newString = new PdfString(m_value);
            newString.Encode = m_bForceEncoding;
            newString.Converted = m_bConverted;
            newString.m_bHex = m_bHex;
            newString.m_crossTable = crossTable;

            m_clonedObject = newString;

            return newString;
        }

        /// <summary>
        /// Converts string value to a byte array.
        /// </summary>
        /// <param name="data">The string data.</param>
        /// <returns>The resulting byte array.</returns>
        internal static byte[] StringToByte(string data)
        {
            return GetAsciiBytes(data);
        }

        /// <summary>
        /// Processes the unicode string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        private void ProcessUnicodeWithPreamble(ref string text, Encoding encoding)
        {
            byte[] data = PdfString.StringToByte(text.Substring(2));
            for (int i = 0; i < data.Length - 1; i++)
            {
                if (data[i] == 92 && (data[i + 1] == 40 || data[i + 1] == 41 || data[i + 1] == 13 || data[i + 1] == 0x3e || data[i + 1] == 92))
                {
                    for (int j = i; j < data.Length - 1; j++)
                        data[j] = data[j + 1];
                    byte[] tempSub = new byte[data.Length - 1];
                    System.Buffer.BlockCopy(data, 0, tempSub, 0, data.Length - 1);
                    data = tempSub;
                    i--;
                }
            }
            text = encoding.GetString(data, 0, data.Length);
        }
        #endregion

        #region Internals
        /// <summary>
        /// Shows what encoding must be used.
        /// </summary>
        internal enum ForceEncoding
        {
            None,
            ASCII,
            Unicode
        }

        /// <summary>
        /// The types of the PDFString data source.
        /// </summary>
        private enum SourceType
        {
            StringValue,
            ByteBuffer,
        }
        #endregion

        #region IPdfDecryptable Members

        /// <summary>
        /// Gets a value indicating whether the object was encrypted.
        /// </summary>
        bool IPdfDecryptable.WasEncrypted
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Decrypts the specified encryptor.
        /// </summary>
        /// <param name="encryptor">The encryptor.</param>
        /// <param name="currObjNumber">The current object number.</param>
        public void Decrypt(PdfEncryptor encryptor, long currObjNumber)
        {
            if (encryptor != null && !m_bDecrypted && !IsParentDecrypted)
            {
                //if (encryptor.Changed == true)
                {
                    m_bDecrypted = true;

                    Value = ByteToString(Bytes);
                    Value = ByteToString(encryptor.EncryptData(currObjNumber, Bytes,false));

                    byte[] unicodePreamble = Encoding.Unicode.GetPreamble();
                    string unicodePreambleString = PdfString.ByteToString(unicodePreamble);
                    byte[] bigEndianPreamble = Encoding.BigEndianUnicode.GetPreamble();
                    string bigEndianPreambleString = PdfString.ByteToString(bigEndianPreamble);
                    if (Value.Length > 1)
                    {
                        if (Value.Substring(0, 2).Equals(unicodePreambleString))
                        {
                            ProcessUnicodeWithPreamble(ref m_value, Encoding.Unicode);
                        }
                        else if (Value.Substring(0, 2).Equals(bigEndianPreambleString))
                        {
                            ProcessUnicodeWithPreamble(ref m_value, Encoding.BigEndianUnicode);
                        }
                    }
                }
            }
        }
#endif
        #endregion
    }
}
