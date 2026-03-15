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
using System.Collections;

namespace Syncfusion.Pdf.Security
{
    internal class Asn1Sequence : AsnObject, IEnumerable
    {
        #region Fields
        /// <summary>
        /// Sequence Containing the AsnObjects 
        /// </summary>
        private List<AsnObject> m_objects;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the AsnObjects in the sequence
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
        /// Creates a new instance of the Asn1Sequence
        /// </summary>
        public Asn1Sequence()
            : base(ASN1Tags.Sequence | ASN1Tags.Constructed)
        {
            m_objects = new List<AsnObject>();
        }

        /// <summary>
        /// Creates a new instance of the Asn1Sequence
        /// </summary>
        /// <param name="sequence">List containing the AsnObjects</param>
        public Asn1Sequence(List<AsnObject> sequence) : base (ASN1Tags.Sequence | ASN1Tags.Constructed)
        {
            m_objects = new List<AsnObject>();
            foreach (AsnObject ao in sequence)
            {
                m_objects.Add(ao);
            }

        }
        #endregion

        #region Implementation
        /// <summary>
        /// Reads the objects in the sequence
        /// </summary>
        /// <returns>Encoded bytes</returns>
        private byte[] ToArray()
        {
            MemoryStream stream = new MemoryStream();

            foreach (AsnObject obj in m_objects)
            {
                byte[] buffer = null;

                if (obj is Asn1Integer)
                {
                    buffer = (obj as Asn1Integer).AsnEncode();
                }
                else if (obj is Asn1Boolean)
                {
                    buffer = (obj as Asn1Boolean).AsnEncode();
                }
                else if (obj is Asn1Null)
                {
                    buffer = (obj as Asn1Null).AsnEncode();
                }
                else if (obj is Asn1ObjectIdentifier)
                {
                    buffer = (obj as Asn1ObjectIdentifier).AsnEncode();
                }
                else if (obj is Asn1OctetString)
                {
                    buffer = (obj as Asn1OctetString).AsnEncode();
                }
                else if (obj is Asn1Sequence)
                {
                    buffer = (obj as Asn1Sequence).AsnEncode();
                }
                else if (obj is AlgorithmIdentifier)
                {
                    buffer = (obj as AlgorithmIdentifier).AsnEncode();
                }

                stream.Write(buffer, 0, buffer.Length);
            }

            return stream.ToArray();
        }       
       
        /// <summary>
        /// Encodes as Asn1Object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            byte[] buff = ToArray();
            return base.AsnEncode(buff);
        }

        /// <summary>
        /// Enumerates the Sequence
        /// </summary>
        /// <returns>AsnObject</returns>
        public IEnumerator GetEnumerator()
        {
            return m_objects.GetEnumerator();
        }

        /// <summary>
        /// Return the asnobject with respect to index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Asnobject in the index</returns>
        public AsnObject this[int index]
        {
            get { return (AsnObject)m_objects[index]; }
        }
        #endregion
    }
}
