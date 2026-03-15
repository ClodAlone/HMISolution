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
using System.IO;

namespace Syncfusion.Pdf.Security
{

    internal class TimeStampResponse
    {

        #region Fields
        /// <summary>
        /// AsnObject that represents the encoded time stamp response
        /// </summary>
        private AsnObject m_encodedObject;
        /// <summary>
        /// Represents the Public Key Infrastructure status info
        /// </summary>
        private Asn1Integer m_pkiStatusInfo;
        /// <summary>
        /// Time stamp token of the obtained time stamp response 
        /// </summary>
        private AsnObject m_timeStampToken;
        /// <summary>
        /// Oid type of the time stamp response
        /// </summary>
        private Asn1ObjectIdentifier m_contentType;      

        #endregion

        #region Properties
        /// <summary>
        /// Returns the final encoded AsnSequence 
        /// </summary>
        internal AsnObject Object
        {
            get
            {
                return m_encodedObject;
            }

        }
        #endregion

        #region constructor
        /// <summary>
        /// Intializes a new instance and reads the timestamp response
        /// </summary>
        /// <param name="bytes"> Array containing the time stamp request bytes</param>
        internal TimeStampResponse(byte[] bytes)
        {
            Asn1InputStream inputStream = new Asn1InputStream(bytes);

            ReadTimeStampResponse(inputStream);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Encodes the timestamp response from the Asn1Sequence
        /// </summary>
        /// <param name="encodedObject"> Asn1Sequence containing the AsnObjects</param>
        /// <returns>Encoded bytes</returns>
        internal byte[] GetEncoded(AsnObject encodedObject)
        {
           return ReadTimeStampToken(encodedObject);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Reads the Asn objects in the time stamp response stream
        /// </summary>
        /// <param name="stream"> Input response stream</param>
        private void ReadTimeStampResponse(Asn1InputStream stream)
        {
            this.m_encodedObject = stream.ReadObject();
        }

        /// <summary>
        /// Parses the time stamp response and encodes the content
        /// </summary>
        /// <param name="encodedObject">Asn1Sequence containing the AsnObjects</param>
        /// <returns>Encoded bytes</returns>
        private byte[] ReadTimeStampToken(AsnObject encodedObject)
        {
            if (encodedObject is Asn1Sequence)
            {
                this.m_pkiStatusInfo = ((((encodedObject as Asn1Sequence)[0])as Asn1Sequence)[0]) as Asn1Integer ;
                this.m_timeStampToken = (encodedObject as Asn1Sequence)[1];
            }

            return ReadContentInfo();
        }

        /// <summary>
        /// Reads the content type of the response and encodes it
        /// </summary>
        /// <returns>Encoded bytes</returns>
        private byte[] ReadContentInfo()
        {
            this.m_contentType = (m_timeStampToken as Asn1Sequence)[0] as Asn1ObjectIdentifier;
            return ReadTimeStampContent();
        }

        /// <summary>
        /// Encodes the time stamp content info
        /// </summary>
        /// <returns>Encoded bytes</returns>
        private byte[] ReadTimeStampContent()
        {                   
            Asn1OutputStream asnStream = new Asn1OutputStream();
            return asnStream.ParseTimeStamp(m_timeStampToken);           
        }
        #endregion
    }
}
