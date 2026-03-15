#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Text;
using System.IO;

namespace Syncfusion.DocIO.DLS.Convertors
{
   public class RtfReader
    {
        #region constant
        private const byte b_endTag = (byte)'}';
        #endregion

        #region Fields
        private byte[] m_rtfData;
        private Encoding m_encoding;
        private int m_position = 0;
        private long m_length;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Rtf data
        /// </summary>
        public byte[] RtfData
        {
            get
            {
                return m_rtfData;
            }

        }
        /// <summary>
        /// Gets the encoding.
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return m_encoding;
            }
        }
        /// <summary>
        /// Gets and Sets the current position in data buffer.
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
        /// Gets and Sets the data buffer length
        /// </summary>
        public long Length
        {
            get
            {
                return m_length;
            }
            set
            {
                m_length = value;
            }
        }

        #endregion

        #region Constructor
        public RtfReader(Stream stream)
        {
            m_rtfData=new byte[stream.Length];
            stream.Read(m_rtfData,0 ,(int)stream.Length);
            m_length = m_rtfData.Length;
#if !(SILVERLIGHT || WP) || WINRT
            m_encoding = Encoding.GetEncoding(DLSConstants.WindowsCodePage);
#else
            m_encoding = new Windows1252Encoding();
#endif
        }
        #endregion

        #region Methods
        /// <summary>
        /// Read a single character
        /// </summary>
        /// <returns></returns>
        public char ReadChar()
        {
            char ch = (char)m_rtfData[m_position];
            m_position++;
            return ch;           
         }
       /// <summary>
       /// Reads the image bytes from the stream
       /// </summary>
       /// <returns></returns>
        public string ReadImageBytes()
        {
            int index = System.Array.IndexOf(m_rtfData, b_endTag,m_position);
            string imageString = Encoding.GetString(m_rtfData, m_position, index-m_position);
            m_position = index;
            imageString = imageString.Replace("\r\n", "");
            return imageString;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            m_rtfData = null;
            m_encoding = null;
        }
        #endregion
    }
}
