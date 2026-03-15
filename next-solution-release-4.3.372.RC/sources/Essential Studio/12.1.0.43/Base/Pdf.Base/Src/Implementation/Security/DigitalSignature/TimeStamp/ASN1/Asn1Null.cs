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
    internal class Asn1Null : AsnObject
    {
        #region Constructor
        /// <summary>
        /// Creates a new instance of the Asn1Null object
        /// </summary>
        public Asn1Null()
            : base(ASN1Tags.Null)
        {

        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns null bytes
        /// </summary>
        /// <returns>null bytes</returns>
        private byte[] ToArray()
        {
            byte[] value = new byte[0];
            return value;
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
