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

namespace Syncfusion.Pdf.Security
{
    internal class Asn1BitString : AsnObject
    {

        #region Fields
        /// <summary>
        /// Represents the bit string in bytes 
        /// </summary>
        private readonly byte[] m_value;
        /// <summary>
        /// Represents the padding bits used
        /// </summary>
        private int m_padBits;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the Asn1BitString
        /// </summary>
        /// <param name="bytes">Asn1BitString in bytes</param>
        /// <param name="padBit">padding bits</param>
        public Asn1BitString(byte[] bytes, int padBit)
            : base(ASN1Tags.BitString)
        {
            this.m_value = bytes;
            this.m_padBits = padBit;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Encodes as Asn1Object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            byte[] value = new byte[m_value.Length + 1];
            value[0] = (byte)m_padBits;
            Array.Copy(m_value, 0, value, 1, value.Length - 1);
            return base.AsnEncode(value);
        }
        #endregion

    }
}
