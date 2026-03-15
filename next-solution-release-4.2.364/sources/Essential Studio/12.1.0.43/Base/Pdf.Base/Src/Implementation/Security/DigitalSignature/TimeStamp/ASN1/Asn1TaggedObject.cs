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
    internal class Asn1TaggedObject : AsnObject
    {
        #region Fields
        /// <summary>
        /// Denotes if the tag is explicit
        /// </summary>
        private bool m_isExplicit;
        /// <summary>
        /// Represents the tag object
        /// </summary>
        private AsnObject m_obj;
        /// <summary>
        /// Represents the tag number
        /// </summary>
        private int m_tagNo;

        # endregion

        #region Properties
        /// <summary>
        /// Returns the tag objects
        /// </summary>
        internal AsnObject Objects
        {
            get
            {
                return m_obj;
            }
        }

        /// <summary>
        /// Returns whether it is explicit
        /// </summary>
        internal bool IsExplicit
        {
            get
            {
                return m_isExplicit;
            }
        }

        /// <summary>
        /// Returns the tag number
        /// </summary>
        internal int TagNumber
        {
            get
            {
                return m_tagNo;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the tag object
        /// </summary>
        /// <param name="isExplicit">If explicit</param>
        /// <param name="obj">Tag object</param>
        /// <param name="tagNo">Tag number</param>
        internal Asn1TaggedObject(bool isExplicit, AsnObject obj, int tagNo)
            : base(ASN1Tags.Tagged)
        {
            this.m_isExplicit = isExplicit;
            this.m_obj = obj;
            this.m_tagNo = tagNo;
        }
        #endregion



    }
}
