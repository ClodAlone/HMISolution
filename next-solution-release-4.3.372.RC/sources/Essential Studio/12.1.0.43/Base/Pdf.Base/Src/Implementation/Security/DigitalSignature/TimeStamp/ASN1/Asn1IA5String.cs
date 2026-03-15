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
    internal class Asn1IA5String : AsnObject
    {
        #region Fields
        /// <summary>
        /// Represents the Asn1IA5String time 
        /// </summary>
        public byte[] m_value;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the As1IA5Time 
        /// </summary>
        /// <param name="bytes">Bytes containing the time</param>
        public Asn1IA5String(byte[] bytes)
            : base(ASN1Tags.IA5String)
        {
            m_value = bytes;
            string m_string = Encoding.ASCII.GetString(bytes);

            if (m_string == null)
                throw new ArgumentNullException("IA5 string cannot be null");
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Encodes as Asn1object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            return base.AsnEncode(m_value);
        }
        #endregion
    }
}
