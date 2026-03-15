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
    internal class Asn1ObjectIdentifier : AsnObject
    {
        #region Constants
        /// <summary>
        /// Seperator in the Oid 
        /// </summary>
        private char c_tokenSeparator = unchecked('.');
        #endregion

        #region Fields
        /// <summary>
        /// Represents the object identifier string
        /// </summary>
        private string m_oid;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the Asn1ObjectIdentifier
        /// </summary>
        /// <param name="oid">Oid String</param>
        public Asn1ObjectIdentifier(string oid)
            : base(ASN1Tags.ObjectIdentifier)
        {
            if (string.IsNullOrEmpty(oid))
                throw new ArgumentNullException("Oid");

            m_oid = oid;
        }

        /// <summary>
        /// Creates a new instance of the Object Indentifier
        /// </summary>
        /// <param name="bytes">Oid in bytes</param>
        public Asn1ObjectIdentifier(byte[] bytes)
            : base(ASN1Tags.ObjectIdentifier)
        {
            this.m_oid = CreateOidString(bytes);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Parses the Oid string and converts it to bytes
        /// </summary>
        /// <returns>Oid in bytes</returns>
        private byte[] ToArray()
        {
            string[] parts = m_oid.Split(new char[] { c_tokenSeparator });
            string part;

            int firstPart = int.Parse(parts[0]);
            int secondPart = int.Parse(parts[1]);

            MemoryStream stream = new MemoryStream();
            AppendField(firstPart * 40 + secondPart, stream);

            for (int i = 2; i < parts.Length; i++)
            {
                part = parts[i];

                if (part.Length < 18)
                    AppendField(int.Parse(part), stream);
                else
                    AppendField(new BigInteger(part), stream);
            }

            byte[] res = stream.ToArray();
            stream.Dispose();

            return res;
        }

        private void AppendField(long value, Stream outStream)
        {
            if (value >= (1L << 7))
            {
                if (value >= (1L << 14))
                {
                    if (value >= (1L << 21))
                    {
                        if (value >= (1L << 28))
                        {
                            if (value >= (1L << 35))
                            {
                                if (value >= (1L << 42))
                                {
                                    if (value >= (1L << 49))
                                    {
                                        if (value >= (1L << 56))
                                        {
                                            outStream.WriteByte((byte)((value >> 56) | 0x80));
                                        }
                                        outStream.WriteByte((byte)((value >> 49) | 0x80));
                                    }
                                    outStream.WriteByte((byte)((value >> 42) | 0x80));
                                }
                                outStream.WriteByte((byte)((value >> 35) | 0x80));
                            }
                            outStream.WriteByte((byte)((value >> 28) | 0x80));
                        }
                        outStream.WriteByte((byte)((value >> 21) | 0x80));
                    }
                    outStream.WriteByte((byte)((value >> 14) | 0x80));
                }
                outStream.WriteByte((byte)((value >> 7) | 0x80));
            }
            outStream.WriteByte((byte)(value & 0x7f));
        }

        private void AppendField(BigInteger value, Stream outStream)
        {
            int byteCount = (value.bitLength() + 6) / 7;
            if (byteCount == 0)
            {
                outStream.WriteByte(0);
            }
            else
            {
                BigInteger tmpValue = value;
                byte[] tmp = new byte[byteCount];
                for (int i = byteCount - 1; i >= 0; i--)
                {
                    tmp[i] = (byte)((tmpValue.intValue() & 0x7f) | 0x80);
                    tmpValue = tmpValue.shiftRight(7);
                }
                tmp[byteCount - 1] &= 0x7f;
                outStream.Write(tmp, 0, tmp.Length);
            }
        }

        /// <summary>
        /// Encodes as Asn1Object
        /// </summary>
        /// <returns>Encoded bytes</returns>
        public byte[] AsnEncode()
        {
            return base.AsnEncode(ToArray());
        }

        /// <summary>
        /// Creates the Oid String from the input bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>Oid String</returns>
        private string CreateOidString(byte[] bytes)
        {

            StringBuilder objId = new StringBuilder();
            long value = 0;
            bool first = true;

            for (int i = 0; i != bytes.Length; i++)
            {
                int b = bytes[i];

                if (value < 0x80000000000000L)
                {
                    value = value * 128 + (b & 0x7f);
                    if ((b & 0x80) == 0)
                    {
                        if (first)
                        {
                            switch ((int)value / 40)
                            {
                                case 0:
                                    objId.Append('0');
                                    break;
                                case 1:
                                    objId.Append('1');
                                    value -= 40;
                                    break;
                                default:
                                    objId.Append('2');
                                    value -= 80;
                                    break;
                            }
                            first = false;
                        }

                        objId.Append('.');
                        objId.Append(value);
                        value = 0;
                    }
                }
            }

            return objId.ToString();
        }
        #endregion
    }
}
