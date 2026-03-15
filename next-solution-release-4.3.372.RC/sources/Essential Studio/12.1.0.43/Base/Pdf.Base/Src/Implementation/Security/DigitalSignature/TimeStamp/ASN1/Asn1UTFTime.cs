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
    internal class Asn1UTFTime : AsnObject
    {
        
        #region Fields
        /// <summary>
        /// Represents the UTF time in bytes
        /// </summary>
        private byte[] m_value;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the UTFTime
        /// </summary>
        /// <param name="value">UTFTime in bytes</param>
        public Asn1UTFTime(byte[] value)
            : base(ASN1Tags.UTFTime)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            m_value = value;
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
