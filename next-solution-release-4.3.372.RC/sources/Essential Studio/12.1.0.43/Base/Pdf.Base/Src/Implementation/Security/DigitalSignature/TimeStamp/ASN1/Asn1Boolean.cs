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
    internal class Asn1Boolean : AsnObject
    {
        #region Fields
        /// <summary>
        /// Represents the Asn1Boolean value
        /// </summary>
        private bool m_value;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the As1Boolean object
        /// </summary>
        /// <param name="value">boolean value</param>
        public Asn1Boolean(bool value)
            : base(ASN1Tags.Boolean)
        {
            m_value = value;
        }

        /// <summary>
        /// Creates a new instance of the As1Boolean object
        /// </summary>
        /// <param name="value">Bytes containg the boolean value</param>
        public Asn1Boolean(byte[] bytes) 
            : base(ASN1Tags.Boolean)
        {
            m_value = (bytes[0] == (byte)0xff);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts the boolean value to bytes 
        /// </summary>
        /// <returns>Asn1Boolean object in bytes</returns>
        private byte[] ToArray()
        {
            byte[] res = new byte[1];
            res[0] = m_value ? (byte)0xff : (byte)0;
            return res;
        }

        /// <summary>
        /// Encodes as Asn1object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            return base.AsnEncode(ToArray());
        }
        #endregion
    }
}
