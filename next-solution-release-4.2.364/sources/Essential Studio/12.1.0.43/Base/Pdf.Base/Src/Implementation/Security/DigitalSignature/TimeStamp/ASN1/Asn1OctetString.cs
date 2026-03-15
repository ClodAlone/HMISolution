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
    internal class Asn1OctetString : AsnObject
    {
        #region Fields
        /// <summary>
        /// Represents the Octet String in bytes
        /// </summary>
        private byte[] m_value;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the Asn1OctetString
        /// </summary>
        /// <param name="value"></param>
        public Asn1OctetString(byte[] value)
            : base(ASN1Tags.OctetString)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            m_value = value;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Encode as Asn1Object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            return base.AsnEncode(m_value);
        }
        #endregion
    }
}
