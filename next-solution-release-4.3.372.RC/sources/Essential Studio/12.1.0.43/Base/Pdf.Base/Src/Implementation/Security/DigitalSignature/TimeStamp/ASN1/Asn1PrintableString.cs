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
    internal class Asn1PrintableString : AsnObject
    {
        #region Fields
        /// <summary>
        /// Represents the Printable string in bytes
        /// </summary>
        private byte[] m_value;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the Asn1PrintableString
        /// </summary>
        /// <param name="bytes"></param>
        public Asn1PrintableString(byte[] bytes)
            : base(ASN1Tags.PrintableString)
        {
            m_value = bytes;
            string str = System.Text.Encoding.ASCII.GetString(bytes);

            if (str == null)
                throw new ArgumentNullException("printable string cannot be null");
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Encodes as Asn1Object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            return base.AsnEncode(m_value);
        }
        #endregion

    }
}
