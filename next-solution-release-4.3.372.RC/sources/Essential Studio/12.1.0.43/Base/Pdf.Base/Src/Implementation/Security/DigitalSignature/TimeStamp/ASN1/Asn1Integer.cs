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
    internal class Asn1Integer : AsnObject
    {
        #region Fields
        /// <summary>
        /// Represents the Asn1Integer 
        /// </summary>
        private long m_value;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the As1Integer
        /// </summary>
        /// <param name="value">Asn1Integer value</param>
        public Asn1Integer(long  value)
            : base(ASN1Tags.Integer)
        {
            m_value = value;
        }
        #endregion

        #region Implemenation
        /// <summary>
        /// Returns the integer
        /// </summary>
        /// <returns>Encoded Bytes</returns>
        private byte[] ToArray()
        {
            return Syncfusion.Licensing.math.BigInteger.valueOf(m_value).toByteArray();
        }

        /// <summary>
        /// Encodes as Asn1Object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            return base.AsnEncode(ToArray());
        }
        #endregion
    }
}
