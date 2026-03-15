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

    internal class Asn1InputStream
    {

        #region Fields
        /// <summary>
        /// Represents the final output stream
        /// </summary>
        private MemoryStream m_stream;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Instance of the input response stream
        /// </summary>
        /// <param name="bytes">Time stamp response in bytes</param>
        public Asn1InputStream(byte[] bytes)
        {
            m_stream = new MemoryStream(bytes,false);
        }
        #endregion

        #region implementation
        /// <summary>
        /// Reads the Asnobject in the response stream with the tags
        /// </summary>
        /// <returns></returns>
        internal AsnObject ReadObject()
        {
            int tag = m_stream.ReadByte();
            if (tag == -1)
            {
                return null;
            }

            int tagNo = 0;

            if ((tag & (int)ASN1Tags.Tagged) != 0 || (tag & (int)ASN1Tags.Application) != 0)
            {
                tagNo = ReadTagNumber(tag);
            }

            int length = ReadLength();

            if (tag == 0 && length == 0)
            {
                return null;
            }

            byte[] bytes = new byte[length];

            ReadFullStream(bytes);

            return BuildObject(tag, tagNo, bytes);
        }

        /// <summary>
        /// Builds the Asnobjects using the tag in the stream
        /// </summary>
        /// <param name="tag">Tag of the Object</param>
        /// <param name="tagNo">Tag number of the AsnObject</param>
        /// <param name="bytes">object in bytes</param>
        /// <returns></returns>
        internal AsnObject BuildObject(int tag, int tagNo, byte[] bytes)
        {

            switch (tag)
            {
                case (int)ASN1Tags.Null:
                    return new Asn1Null();
                case (int)ASN1Tags.Sequence | (int)ASN1Tags.Constructed:
                    {
                        List<AsnObject> objectList = BuildSequence(bytes);
                        return new Asn1Sequence(objectList);
                    }
                case (int)ASN1Tags.Set | (int)ASN1Tags.Constructed:
                    {
                        List<AsnObject> objectList = BuildSequence(bytes);
                        return new Asn1Set(objectList);
                    }
                case (int)ASN1Tags.Boolean:
                    return new Asn1Boolean(bytes);
                case (int)ASN1Tags.Integer:
                    return new Asn1Integer(bytes[0]);
                case (int)ASN1Tags.ObjectIdentifier:
                    return new Asn1ObjectIdentifier(bytes);
                case (int)ASN1Tags.BitString:
                    {
                        int padBits = bytes[0];
                        byte[] data = new byte[bytes.Length - 1];
                        Array.Copy(bytes, 1, data, 0, bytes.Length - 1);
                        return new Asn1BitString(data, padBits);
                    }
                case (int)ASN1Tags.PrintableString:
                    return new Asn1PrintableString(bytes);
                case (int)ASN1Tags.IA5String:
                    return new Asn1IA5String(bytes);
                case (int)ASN1Tags.OctetString:
                    return new Asn1OctetString(bytes);
                case (int)ASN1Tags.UTFTime:
                    return new Asn1UTFTime(bytes);
                case (int)ASN1Tags.GeneralizedTime:
                    return new Asn1GeneralizedTime(bytes);
                default:
                    {
                        if ((tag & (int)ASN1Tags.Tagged) != 0)
                        {
                            bool isImplicit = ((tag & (int)ASN1Tags.Constructed) == 0);

                            if (bytes.Length == 0)
                            {
                                AsnObject asn1Object;

                                if (isImplicit)
                                    asn1Object = new Asn1Null();
                                else
                                    asn1Object = new Asn1Sequence();

                                return new Asn1TaggedObject(false, asn1Object, tagNo);
                            }

                            if (isImplicit)
                            {
                                return new Asn1TaggedObject(false, new Asn1OctetString(bytes), tagNo);
                            }

                            Asn1InputStream inputStream = new Asn1InputStream(bytes);
                            AsnObject asnObject = inputStream.ReadObject();


                            if (inputStream.m_stream.Position == bytes.Length)
                            {
                                return new Asn1TaggedObject(true, asnObject, tagNo);
                            }

                            List<AsnObject> objectList = new List<AsnObject>();

                            while (asnObject != null)
                            {
                                objectList.Add(asnObject);
                                asnObject = inputStream.ReadObject();
                            }

                            return new Asn1TaggedObject(false, new Asn1Sequence(objectList), tagNo);

                        }

                        return new Asn1Boolean(false);

                    }
                    m_stream.Close();
            }
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Reads the lengtth of the stream
        /// </summary>
        /// <returns>length</returns>
        private int ReadLength()
        {
            int length = m_stream.ReadByte();
            if (length < 0)
            {
                throw new IOException("EOF found when length expected");
            }

            if (length == 0x80)
            {
                return -1;
            }

            if (length > 127)
            {
                int size = length & 0x7f;

                if (size > 4)
                {
                    throw new IOException("DER length more than 4 bytes");
                }

                length = 0;
                for (int i = 0; i < size; i++)
                {
                    int next = m_stream.ReadByte();

                    if (next < 0)
                    {
                        throw new IOException("EOF found reading length");
                    }

                    length = (length << 8) + next;
                }

                if (length < 0)
                {
                    throw new IOException("corrupted steam - negative length found");
                }

            }

            return length;

        }

        /// <summary>
        /// Reads the bytes of the individual asn1objects
        /// </summary>
        /// <param name="bytes"></param>
        private void ReadFullStream(byte[] bytes)
        {

            int left = bytes.Length;
            if (left == 0)
            {
                return;
            }

            int length;
            while ((length = m_stream.Read(bytes, bytes.Length - left, left)) > 0)
            {
                if ((left -= length) == 0)
                {
                    return;
                }
            }

            if (left != 0)
            {
                throw new EndOfStreamException("EOF encountered in middle of object");
            }
        }


        /// <summary>
        /// Reads the tag number for a given tag
        /// </summary>
        /// <param name="tag">ASN1tag</param>
        /// <returns>Tag number</returns>
        private int ReadTagNumber(int tag)
        {
            int tagNo = tag & 0x1f;

            if (tagNo == 0x1f)
            {
                int b = m_stream.ReadByte();

                tagNo = 0;

                while ((b >= 0) && ((b & 0x80) != 0))
                {
                    tagNo |= (b & 0x7f);
                    tagNo <<= 7;
                    b = m_stream.ReadByte();
                }

                if (b < 0)
                {
                    return 0;
                    throw new EndOfStreamException("EOF found inside tag value.");
                }

                tagNo |= (b & 0x7f);
            }

            return tagNo;
        }

        /// <summary>
        /// Builds the AsnSequence(or AsnSet) with the given bytes
        /// </summary>
        /// <param name="bytes">input bytes</param>
        /// <returns>List of AsnObjects</returns>
        private List<AsnObject> BuildSequence(byte[] bytes)
        {
            Asn1InputStream inputStream = new Asn1InputStream(bytes);
            return inputStream.BuildEncodableVector();
        }

        /// <summary>
        /// Builds the AsnSequence(or AsnSet) with the given bytes
        /// </summary>        
        /// <returns>List of AsnObjects</returns>
        private List<AsnObject> BuildEncodableVector()
        {

            List<AsnObject> objectList = new List<AsnObject>();
            AsnObject asn1object;

            while ((asn1object = ReadObject()) != null)
            {
                objectList.Add(asn1object);
            }

            return objectList;
        }
        #endregion
    }
}
