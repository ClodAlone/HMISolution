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
    internal class MessageImprint : Asn1Sequence
    {
        #region Constructor
        public MessageImprint(string oid, byte[] hash)
            : base()
        {
            Asn1ObjectIdentifier identifier = new Asn1ObjectIdentifier(oid);
            base.Objects.Add(new AlgorithmIdentifier(identifier, new Asn1Null()));
            base.Objects.Add(new Asn1OctetString(hash));
        }
        #endregion
    }
}
