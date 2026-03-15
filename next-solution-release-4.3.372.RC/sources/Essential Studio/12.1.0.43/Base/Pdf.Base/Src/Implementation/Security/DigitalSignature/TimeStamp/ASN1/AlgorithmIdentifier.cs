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
using Syncfusion.Licensing.math;

namespace Syncfusion.Pdf.Security
{
    internal class AlgorithmIdentifier : AsnObject
    {
        #region Fields
        /// <summary>
        /// AsnSequence which represents the Algorithm
        /// </summary>
        private Asn1Sequence m_seq;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the AlgorithmIdentifier class
        /// </summary>
        /// <param name="oid">Object Identifeier</param>
        /// <param name="param">parameters</param>
        public AlgorithmIdentifier(Asn1ObjectIdentifier oid, AsnObject param)
        {
            m_seq = new Asn1Sequence();
            m_seq.Objects.Add(oid);
            m_seq.Objects.Add(param);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Encodes as Asn1Object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            return m_seq.AsnEncode();
        }
        #endregion
    }
}
