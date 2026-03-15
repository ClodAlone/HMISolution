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
    class TimeStampRequest : AsnObject
    {
        #region Constants
        private const string c_IdSHA = "1.3.14.3.2.26";
        private const string c_IdTimeStampToken = "1.2.840.113549.1.9.16.2.14";
        #endregion

        #region Fields
        private Asn1Integer m_version;
        private MessageImprint m_messageImprint;
        private Asn1Boolean m_certReq;
        #endregion

        public TimeStampRequest(bool certReq)
            : base(ASN1Tags.Sequence | ASN1Tags.Constructed)
        {
            m_certReq = new Asn1Boolean(certReq);
            m_version = new Asn1Integer(1l);
        }

        #region Implementation
        public byte[] GetAsnEncodedTimestampRequest(byte[] hash)
        {
            m_messageImprint = new MessageImprint(c_IdSHA, hash);

            byte[] version = m_version.AsnEncode();
            byte[] imprint = m_messageImprint.AsnEncode();
            byte[] certReq = m_certReq.AsnEncode();

            byte[] result = new byte[version.Length + imprint.Length + certReq.Length];

            Array.Copy(version, result, version.Length);
            Array.Copy(imprint, 0, result, version.Length, imprint.Length);
            Array.Copy(certReq, 0, result, version.Length + imprint.Length, certReq.Length);

            result = base.AsnEncode(result);

            return result;
        }
        #endregion
    }
}
