#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Syncfusion.Pdf.Security
{
    internal class AsnObject
    {
        #region Fields
        /// <summary>
        /// Represents the tag of the AsnObject
        /// </summary>
        ASN1Tags m_tag;
        /// <summary>
        /// Stream used for encoding
        /// </summary>
        MemoryStream m_outStream;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the Asnobject
        /// </summary>
        public AsnObject()
        {
        }

        /// <summary>
        /// Creates a new instance of the Asnobject
        /// </summary>
        /// <param name="tag"></param>
        public AsnObject(ASN1Tags tag)
        {
            m_tag = tag;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Encodes the AsnObject
        /// </summary>
        /// <param name="value">Input bytes</param>
        /// <returns>Encoded bytes</returns>
        internal byte[] AsnEncode(byte[] value)
        {
            m_outStream = new MemoryStream();

            m_outStream.WriteByte((byte)m_tag);
            WriteCorrectLength(value.Length);
            m_outStream.Write(value, 0, value.Length);

            m_outStream.Close();
            return m_outStream.ToArray(); ;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Calculates the correct length of the encoded bytes 
        /// </summary>
        /// <param name="length"></param>
        private void WriteCorrectLength(int length)
        {
            if (length > 127)
            {
                int size = 1;
                uint value = (uint)length;

                while ((value >>= 8) != 0)
                {
                    size++;
                }

                m_outStream.WriteByte((byte)(size | 0x80));

                for (int i = (size - 1) * 8; i >= 0; i -= 8)
                {
                    m_outStream.WriteByte((byte)(length >> i));
                }
            }
            else
            {
                m_outStream.WriteByte((byte)length);
            }           
        }
        #endregion
    }
}
