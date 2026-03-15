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
using System.Collections;

namespace Syncfusion.Pdf.Security
{
    internal class Asn1Set : AsnObject, IEnumerable
    {
        #region Fields
        /// <summary>
        /// Set of AsnObjects 
        /// </summary>
        private List<AsnObject> m_objects;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the AsnObjects in the set
        /// </summary>
        public List<AsnObject> Objects
        {
            get
            {
                return m_objects;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the Asn1Set
        /// </summary>
        public Asn1Set() 
        {
            m_objects = new List<AsnObject>();
        }

       /// <summary>
        /// Creates a new instance of the Asn1Set
       /// </summary>
       /// <param name="sequence">List containg the asnobjects</param>
        public Asn1Set(List<AsnObject> sequence) 
        {
            m_objects = new List<AsnObject>();
            foreach (AsnObject ao in sequence)
            {
                m_objects.Add(ao);
            }

        }
        #endregion

        #region implementation
        /// <summary>
        /// Enumerates the Asn1Set
        /// </summary>
        /// <returns>Asnobject</returns>
        public IEnumerator GetEnumerator()
        {
            return m_objects.GetEnumerator();
        }

        /// <summary>
        /// return AsnObject in the specific index of the Asn1Set
        /// </summary>
        /// <param name="index"></param>
        /// <returns>AsnObject</returns>
        public AsnObject this[int index]
        {
            get { return (AsnObject)m_objects[index]; }
        }
        #endregion

    }

}
