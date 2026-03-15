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
    internal class Asn1GeneralizedTime : AsnObject
    {
        #region Fields
        /// <summary>
        /// Represents the generalized time 
        /// </summary>
        private byte[] m_value;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the As1GeneralizedTime 
        /// </summary>
        /// <param name="bytes">Bytes containg the time</param>
        internal Asn1GeneralizedTime(byte[] bytes)
            : base(ASN1Tags.GeneralizedTime)
        {
            m_value = bytes;
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
