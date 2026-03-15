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
    internal class Asn1OutputStream
    {

        #region Fields
        /// <summary>
        /// Represents the final output stream
        /// </summary>
        private MemoryStream m_stream;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the Asn1OutputStream
        /// </summary>
        internal Asn1OutputStream()
        {
            m_stream = new MemoryStream();
        }
        #endregion

        #region implementation

        /// <summary>
        /// Parses the given time stamp token
        /// </summary>
        /// <param name="encodedObject">AsnObject, either a sequence or set</param>
        /// <returns>Encoded bytes</returns>
        internal byte[] ParseTimeStamp(AsnObject encodedObject)
        {
            Asn1OutputStream finalStream = new Asn1OutputStream();
            byte[] buffer;

            m_stream.WriteByte((byte)(ASN1Tags.Sequence | ASN1Tags.Constructed));
            m_stream.WriteByte(0x80);


            if ((encodedObject as Asn1Sequence)[0] is Asn1ObjectIdentifier)
            {
                // BER object identifier (first object of sequence)
                buffer = ((encodedObject as Asn1Sequence)[0] as Asn1ObjectIdentifier).AsnEncode();
                m_stream.Write(buffer, 0, buffer.Length);
            }

            if ((encodedObject as Asn1Sequence)[1] is Asn1TaggedObject)
            {
                //BER tagged object (second object of sequence)
                AsnObject asnObject = (encodedObject as Asn1Sequence)[1];

                m_stream.WriteByte((byte)((int)ASN1Tags.Constructed | (int)ASN1Tags.Tagged | (asnObject as Asn1TaggedObject).TagNumber));
                m_stream.WriteByte(0x80);

                List<AsnObject> objectList = new List<AsnObject>();
                objectList.Add((asnObject as Asn1TaggedObject).Objects);
                Asn1Sequence sequence = new Asn1Sequence(objectList);

                buffer = finalStream.ParseTimeStampToken(sequence);
                m_stream.Write(buffer, 0, buffer.Length);
            }

            //final bytes
            m_stream.WriteByte(0x00);
            m_stream.WriteByte(0x00);
            m_stream.WriteByte(0x00);
            m_stream.WriteByte(0x00);

            m_stream.Close();
            return m_stream.ToArray();
        }

        /// <summary>
        /// Parses the time stamp token with the given sequence
        /// </summary>
        /// <param name="encodedObject">Asnobject, either a sequence or set</param>
        /// <returns>Encoded bytes</returns>
        internal byte[] ParseTimeStampToken(AsnObject encodedObject)
        {
            Asn1Sequence sequence = null;
            if (encodedObject is Asn1Sequence)
                sequence = encodedObject as Asn1Sequence;
            else if (encodedObject is Asn1Set)
            {
                sequence = new Asn1Sequence();
                foreach (AsnObject asn in (encodedObject as Asn1Set).Objects)
                {
                    sequence.Objects.Add(asn);
                }
            }

            foreach (AsnObject asnObject in sequence)
            {
                byte[] buffer = null;

                if (asnObject is Asn1Integer)
                {
                    buffer = (asnObject as Asn1Integer).AsnEncode();
                }
                else if (asnObject is Asn1Boolean)
                {
                    buffer = (asnObject as Asn1Boolean).AsnEncode();
                }
                else if (asnObject is Asn1Null)
                {
                    buffer = (asnObject as Asn1Null).AsnEncode();
                }
                else if (asnObject is Asn1ObjectIdentifier)
                {
                    buffer = (asnObject as Asn1ObjectIdentifier).AsnEncode();
                }
                else if (asnObject is Asn1TaggedObject)
                {
                    if ((asnObject as Asn1TaggedObject).Objects is Asn1Sequence)
                    {
                        List<AsnObject> objectList = new List<AsnObject>();
                        objectList.Add((asnObject as Asn1TaggedObject).Objects);
                        Asn1Sequence newSequence = new Asn1Sequence(objectList);
                        Asn1OutputStream outStream = new Asn1OutputStream();
                        buffer = outStream.ParseTimeStampToken(newSequence);
                    }
                    else
                    {
                        if ((asnObject as Asn1TaggedObject).Objects is Asn1TaggedObject)
                        {
                            Asn1OutputStream outStream = new Asn1OutputStream();
                            buffer = outStream.ParseTimeStampToken((asnObject as Asn1TaggedObject).Objects);
                        }

                        else if ((asnObject as Asn1TaggedObject).Objects is Asn1OctetString)
                        {
                            Asn1OutputStream outStream = new Asn1OutputStream();
                            buffer = ((asnObject as Asn1TaggedObject).Objects as Asn1OctetString).AsnEncode();
                        }
                        else if ((asnObject as Asn1TaggedObject).Objects is Asn1Integer)
                        {
                            Asn1OutputStream outStream = new Asn1OutputStream();
                            buffer = ((asnObject as Asn1TaggedObject).Objects as Asn1Integer).AsnEncode();
                        }
                    }

                    if ((asnObject as Asn1TaggedObject).IsExplicit)
                    {
                        m_stream.WriteByte((byte)((int)ASN1Tags.Constructed | (int)ASN1Tags.Tagged | (asnObject as Asn1TaggedObject).TagNumber));
                        WriteCorrrectLength(buffer);
                    }
                    else
                    {
                        buffer[0] &= (byte)ASN1Tags.Constructed;
                        buffer[0] |= (byte)((int)ASN1Tags.Tagged | (asnObject as Asn1TaggedObject).TagNumber);
                    }

                }
                else if (asnObject is Asn1Set)
                {
                    Asn1OutputStream outStream = new Asn1OutputStream();
                    buffer = outStream.ParseTimeStampToken(asnObject);
                    m_stream.WriteByte((byte)(ASN1Tags.Set | ASN1Tags.Constructed));
                    WriteCorrrectLength(buffer);
                }
                else if (asnObject is Asn1Sequence)
                {
                    Asn1OutputStream outStream = new Asn1OutputStream();
                    outStream.ParseTimeStampToken(asnObject);
                    buffer = outStream.m_stream.ToArray();
                    m_stream.WriteByte((byte)(ASN1Tags.Sequence | ASN1Tags.Constructed));
                    WriteCorrrectLength(buffer);

                }

                else if (asnObject is Asn1OctetString)
                {
                    buffer = (asnObject as Asn1OctetString).AsnEncode();
                }

                else if (asnObject is AlgorithmIdentifier)
                {
                    buffer = (asnObject as AlgorithmIdentifier).AsnEncode();
                }
                else if (asnObject is Asn1UTFTime)
                {
                    buffer = (asnObject as Asn1UTFTime).AsnEncode();
                }

                else if (asnObject is Asn1BitString)
                {
                    buffer = (asnObject as Asn1BitString).AsnEncode();
                }

                else if (asnObject is Asn1PrintableString)
                {
                    buffer = (asnObject as Asn1PrintableString).AsnEncode();
                }

                else if (asnObject is Asn1IA5String)
                {
                    buffer = (asnObject as Asn1IA5String).AsnEncode();
                }

                else
                {

                }
                m_stream.Write(buffer, 0, buffer.Length);
            }
            m_stream.Close();
            return m_stream.ToArray();
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Calculates the correct output length for the given bytes
        /// </summary>
        /// <param name="buffer">input bytes</param>
        private void WriteCorrrectLength(byte[] buffer)
        {
            if (buffer.Length > 127)
            {
                int size = 1;
                uint value = (uint)buffer.Length;

                while ((value >>= 8) != 0)
                {
                    size++;
                }

                m_stream.WriteByte((byte)(size | 0x80));

                for (int i = (size - 1) * 8; i >= 0; i -= 8)
                {
                    m_stream.WriteByte((byte)(buffer.Length >> i));
                }
            }
            else
            {
                m_stream.WriteByte((byte)buffer.Length);
            }
        }
        #endregion
    }
}
