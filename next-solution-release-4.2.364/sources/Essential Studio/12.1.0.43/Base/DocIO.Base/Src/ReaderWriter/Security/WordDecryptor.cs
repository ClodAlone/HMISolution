#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region file using directives
using System;
using System.IO;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Security
{
    /// <summary>
    /// Represents an WordDecryptor.
    /// </summary>
    [CLSCompliant(false)]
    internal class WordDecryptor
    {
        #region Class constants
        private const int DEF_READ_LENGTH = 16;
        private const int DEF_PAS_LEN = 64;
        private const int DEF_BLOCK_SIZE = 512;
        private const int DEF_START_POS = 0;
        private const int DEF_INC_BYTE_MAXVAL = 256;
        private const uint DEF_PASSWORD_CONST = 0xCE4B;
        private static readonly ushort[] initCodeArr = new ushort[15]{
        0xE1F0, 0x1D0F, 0xCC9C, 0x84C0, 0x110C,
        0x0E10, 0xF1CE, 0x313E, 0x1872, 0xE139,
        0xD40F, 0x84F9, 0x280C, 0xA96A, 0x4EC3
      };
        // 2-dimentional matrix [ bit index, char index]
        private static readonly ushort[,] encryptMatrix = new ushort[7, 15] {
        {
          // Char indexes for "Bit0"
          0xAEFC, 0x7B61, 0x4563, 0x0375, 0xD849,
          0x6F45, 0xEB23, 0x47D3, 0xB861, 0x45A0,
          0xAA51, 0x76B4, 0x3730, 0x3331, 0x1021
        },
        {
          // Char indexes for "Bit1"
          0x4DD9, 0xF6C2, 0x8AC6, 0x06EA, 0xA0B3, 
          0xDE8A, 0xC667, 0x8FA6, 0x60E3, 0x8B40, 
          0x4483, 0xED68, 0x6E60, 0x6662, 0x2042
        },
        {
          // Char indexes for "Bit2"
          0x9BB2, 0xFDA5, 0x05AD, 0x0DD4, 0x5147, 
          0xAD35, 0x9CEF, 0x0F6D, 0xC1C6, 0x06A1,
          0x8906, 0xCAF1, 0xDCC0, 0xCCC4, 0x4084
        },
        {
          // Char indexes for "Bit3"
          0x2745, 0xEB6B, 0x0B5A, 0x1BA8, 0xA28E, 
          0x4A4B, 0x29FF, 0x1EDA, 0x93AD, 0x0D42,
          0x022D, 0x85C3, 0xA9A1, 0x89A9, 0x8108
        },
        {
          // Char indexes for "Bit4"
          0x4E8A, 0xC6F7, 0x16B4, 0x3750, 0x553D,
          0x9496, 0x53FE, 0x3DB4, 0x377B, 0x1A84,
          0x045A, 0x1BA7, 0x4363, 0x0373, 0x1231
        },
        {
          // Char indexes for "Bit5"
          0x9D14, 0x9DCF, 0x2D68, 0x6EA0, 0xAA7A,
          0x390D, 0xA7FC, 0x7B68, 0x6EF6, 0x3508,
          0x08B4, 0x374E, 0x86C6, 0x06E6, 0x2462
        },
        {
          // Char indexes for "Bit6"
          0x2A09, 0x2BBF, 0x5AD0, 0xDD40, 0x44D5,
          0x721A, 0x5FD9, 0xF6D0, 0xDDEC, 0x6A10,
          0x1168, 0x6E9C, 0x1DAD, 0x0DCC, 0x48C4
        }
    };
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_baDocumentID = new byte[DEF_READ_LENGTH];
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_baPoint = new byte[DEF_PAS_LEN];
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_baHash = new byte[DEF_READ_LENGTH];
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_baPassword = new byte[DEF_PAS_LEN];
        /// <summary>
        /// 
        /// </summary>
        private MD5Context m_valContext = new MD5Context();
        /// <summary>
        /// 
        /// </summary>
        private WPFIBData m_fibData = null;
        /// <summary>
        /// 
        /// </summary>
        private MemoryStream m_tableStream = null;
        /// <summary>
        /// 
        /// </summary>
        private MemoryStream m_dataStream = null;
        /// <summary>
        /// 
        /// </summary>
        private MemoryStream m_mainStream = null;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsComplexFile = false;
        /// <summary>
        /// 
        /// </summary>
        private WordKey m_key;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the table stream.
        /// </summary>
        /// <value>The table stream.</value>
        internal MemoryStream TableStream
        {
            get
            {
                return m_tableStream;
            }
        }
        /// <summary>
        /// Gets the main stream.
        /// </summary>
        /// <value>The main stream.</value>
        internal MemoryStream MainStream
        {
            get
            {
                return m_mainStream;
            }
        }
        /// <summary>
        /// Gets the data stream
        /// </summary>
        internal MemoryStream DataStream
        {
            get
            {
                return m_dataStream;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordDecryptor"/> [ERROR: invalid expression DeclaringTypeKind].
        /// </summary>
        internal WordDecryptor()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WordDecryptor"/> class.
        /// </summary>
        /// <param name="tableStream">The table stream.</param>
        /// <param name="mainStream">The main stream.</param>
        /// <param name="fib">The fib.</param>
        internal WordDecryptor(MemoryStream tableStream, MemoryStream mainStream, MemoryStream dataStream, WPFIBData fib)
        {
            m_tableStream = tableStream;
            m_mainStream = mainStream;
            m_dataStream = dataStream;
            m_fibData = fib;
            m_bIsComplexFile = fib.IsComplexFile;
        }
        #endregion

        #region Class internal Methods
        /// <summary>
        /// Tests the encrypt.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void TestEncrypt(ref MemoryStream stream, string password, ref byte[] docID, ref byte[] point, ref byte[] hash)
        {
            m_baDocumentID = docID;
            ConvertPassword(password);
            PrepareValContext();

            Buffer.BlockCopy(m_baDocumentID, 0, m_baPoint, 0, DEF_READ_LENGTH);

            m_baPoint[16] = 0x80;
            //SetByte( m_baPoint, 17, 47, 0 );
            Array.Clear(m_baPoint, 17, 47);
            m_baPoint[56] = 0x80;

            MD5Context mdContext2 = new MD5Context();
            mdContext2.Update(m_baPoint, DEF_PAS_LEN);
            mdContext2.StoreDigest();

            Buffer.BlockCopy(mdContext2.Digest, 0, m_baHash, 0, DEF_READ_LENGTH);

            //WordKey key = new WordKey();
            //MakeKey( key, 0, m_valContext );
            MakeKey(0);

            DecryptBuffer(m_baPoint, DEF_READ_LENGTH);
            DecryptBuffer(m_baHash, DEF_READ_LENGTH);

            //ByteArrayDataProvider provider = new ByteArrayDataProvider( m_baPoint );
            //DecryptBuffer( provider, 0, DEF_READ_LENGTH, key );

            //provider.SetBuffer( m_baHash );
            //DecryptBuffer( provider, 0, DEF_READ_LENGTH, key );
            stream = DecryptStream(stream);

            docID = m_baDocumentID;
            point = m_baPoint;
            hash = m_baHash;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="password"></param>
        /// <param name="docid"></param>
        /// <param name="point"></param>
        /// <param name="hash"></param>
        public void TestDecrypt(ref MemoryStream stream, string password, ref byte[] docid, ref byte[] point, ref byte[] hash)
        {
            Buffer.BlockCopy(docid, 0, m_baDocumentID, 0, DEF_READ_LENGTH);
            Buffer.BlockCopy(point, 0, m_baPoint, 0, DEF_READ_LENGTH);
            Buffer.BlockCopy(hash, 0, m_baHash, 0, DEF_READ_LENGTH);
            ConvertPassword(password);
            VerifyPassword();

            stream = DecryptStream(stream);
        }
        /// <summary>
        /// Checks the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        internal bool CheckPassword(string password)
        {
            if (m_tableStream == null)
            {
                throw new ArgumentNullException("Table Stream is null referenced.");
            }

            m_tableStream.Position = Constants.BytesInInt;
            m_tableStream.Read(m_baDocumentID, 0, DEF_READ_LENGTH);
            m_tableStream.Read(m_baPoint, 0, DEF_READ_LENGTH);
            m_tableStream.Read(m_baHash, 0, DEF_READ_LENGTH);
            ConvertPassword(password);

            return VerifyPassword();
        }
        /// <summary>
        /// Decrypts our streams.
        /// </summary>
        /// <returns></returns>
        internal void Decrypt()
        {
            m_tableStream = DecryptStream(m_tableStream);
            m_mainStream = DecryptStream(m_mainStream);
            if (m_dataStream != null)
                m_dataStream = DecryptStream(m_dataStream);
            ReadFib();
        }
        /// <summary>
        /// Decrypts our streams.
        /// </summary>
        /// <returns></returns>
        internal void Encrypt(string password)
        {
            m_baDocumentID = Guid.NewGuid().ToByteArray();
            ConvertPassword(password);
            PrepareValContext();

            Buffer.BlockCopy(m_baDocumentID, 0, m_baPoint, 0, DEF_READ_LENGTH);

            m_baPoint[16] = 0x80;
            Array.Clear(m_baPoint, 17, 47);
            m_baPoint[56] = 0x80;

            MD5Context mdContext2 = new MD5Context();
            mdContext2.Update(m_baPoint, DEF_PAS_LEN);
            mdContext2.StoreDigest();

            Buffer.BlockCopy(mdContext2.Digest, 0, m_baHash, 0, DEF_READ_LENGTH);

            MakeKey(0);

            DecryptBuffer(m_baPoint, DEF_READ_LENGTH);
            DecryptBuffer(m_baHash, DEF_READ_LENGTH);

            m_tableStream = DecryptStream(m_tableStream);

            // Write encryption type information
            m_tableStream.Position = 0;
            m_tableStream.WriteByte(1);
            m_tableStream.WriteByte(0);
            m_tableStream.WriteByte(1);
            m_tableStream.WriteByte(0);

            // Write password hash.
            m_tableStream.Write(m_baDocumentID, 0, DEF_READ_LENGTH);
            m_tableStream.Write(m_baPoint, 0, DEF_READ_LENGTH);
            m_tableStream.Write(m_baHash, 0, DEF_READ_LENGTH);

            m_mainStream = DecryptStream(m_mainStream);
            if (m_dataStream != null)
                m_dataStream = DecryptStream(m_dataStream);
            WriteFib();
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Reads the fib.
        /// </summary>
        private void ReadFib()
        {
            m_mainStream.Position = DEF_START_POS;
            m_fibData.ReadDecrypted(m_mainStream);
            m_fibData.fEncrypted = false;
            m_fibData.fComplex = m_bIsComplexFile;
        }
        /// <summary>
        /// Reads the fib.
        /// </summary>
        private void WriteFib()
        {
            m_mainStream.Position = DEF_START_POS;
            m_fibData.WriteDecrypted(m_mainStream);
        }

        /// <summary>
        /// Decrypts the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        private MemoryStream DecryptStream(MemoryStream stream)
        {
            byte[] temp = new byte[DEF_READ_LENGTH];
            MemoryStream retStream = new MemoryStream();
            long end = stream.Length;
            int j = DEF_START_POS;
            stream.Position = j;
            uint blk = DEF_START_POS;
            MakeKey(blk);

            while (j < end)
            {
                int count = stream.Read(temp, 0, DEF_READ_LENGTH);

                for (int i = count; i < DEF_READ_LENGTH; i++)
                    temp[i] = 1;

                DecryptBuffer(temp, DEF_READ_LENGTH);
                retStream.Write(temp, 0, DEF_READ_LENGTH);
                j += DEF_READ_LENGTH;

                if ((j % DEF_BLOCK_SIZE) == 0)
                {
                    blk++;
                    MakeKey(blk);
                }

            }

            retStream.Position = DEF_START_POS;
            return retStream;
        }
        /// <summary>
        /// Swaps a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        private void Swap(ref byte a, ref byte b)
        {
            byte temp = a;
            a = b;
            b = temp;
        }
        /// <summary>
        /// Converts the password.
        /// </summary>
        private void ConvertPassword(string password)
        {
            int i;

            for (i = 0; i < password.Length; i++)
            {
                m_baPassword[2 * i] = (byte)(password[i] & Byte.MaxValue);
                m_baPassword[2 * i + 1] = (byte)((password[i] >> 8) & Byte.MaxValue);
            }

            m_baPassword[2 * i] = 0x80;
            m_baPassword[56] = (byte)(i << 4);
        }

        /// <summary>
        /// Prepares the key.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private void PrepareKey(byte[] data)
        {
            m_key = new WordKey();
            byte index1 = 0;
            byte index2 = 0;

            for (int i = 0; i < DEF_INC_BYTE_MAXVAL; i++)
                m_key.status[i] = (byte)i;

            for (int i = 0; i < DEF_INC_BYTE_MAXVAL; i++)
            {
                index2 = (byte)((data[index1] + m_key.status[i] + index2) % DEF_INC_BYTE_MAXVAL);
                Swap(ref m_key.status[i], ref m_key.status[index2]);
                index1 = (byte)((index1 + 1) % DEF_READ_LENGTH);
            }
        }

        /// <summary>
        /// Makes the key.
        /// </summary>
        /// <returns></returns>
        private void MakeKey(uint block)
        {
            MD5Context mdContext = new MD5Context();
            byte[] pwarray = new byte[DEF_PAS_LEN];

            Buffer.BlockCopy(m_valContext.Digest, 0, pwarray, 0, 5);
            //API.CopyMemory( pwarray, m_valContext.Digest, 5 );

            pwarray[5] = (byte)(block & Byte.MaxValue);
            pwarray[6] = (byte)((block >> 8) & Byte.MaxValue);
            pwarray[7] = (byte)((block >> 16) & Byte.MaxValue);
            pwarray[8] = (byte)((block >> 24) & Byte.MaxValue);
            pwarray[9] = 0x80;
            pwarray[56] = 0x48;
            mdContext.Update(pwarray, DEF_PAS_LEN);
            mdContext.StoreDigest();
            PrepareKey(mdContext.Digest);
        }

        /// <summary>
        /// Compares memory blocks
        /// </summary>
        /// <param name="block1">The block1.</param>
        /// <param name="block2">The block2.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private bool MemoryCompare(byte[] block1, byte[] block2, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (block1[i] != block2[i])
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Verifies the password.
        /// </summary>
        /// <returns></returns>
        private bool VerifyPassword()
        {
            //      MD5Context	mdContext1 = new MD5Context();
            //      mdContext1.Update( m_baPassword, DEF_PAS_LEN	);
            //      mdContext1.StoreDigest();
            //      m_valContext	=	new	MD5Context();
            //      uint offset	=	0, keyoffset = 0, tocopy	=	5;

            //      while( offset != DEF_READ_LENGTH )
            //      {
            //        if( DEF_PAS_LEN - offset < 5 )
            //        {
            //          tocopy = DEF_PAS_LEN - offset;
            //        }

            ////				API.CopyMemory(	ref	m_baPassword[offset], ref mdContext1.Digest[ keyoffset ], (int)tocopy );
            //        Buffer.BlockCopy( mdContext1.Digest, ( int )keyoffset, m_baPassword, ( int )offset, ( int )tocopy );
            //        offset += tocopy;

            //        if( offset == DEF_PAS_LEN )
            //        {
            //          m_valContext.Update( m_baPassword, DEF_PAS_LEN );
            //          keyoffset = tocopy;
            //          tocopy = 5 - tocopy;
            //          offset = 0;
            //          continue;
            //        }

            //        keyoffset = 0;
            //        tocopy = 5;
            ////        API.CopyMemory( ref m_baPassword[ offset ], m_baDocumentID, DEF_READ_LENGTH );
            //        Buffer.BlockCopy( m_baDocumentID, 0, m_baPassword, ( int )offset, DEF_READ_LENGTH );
            //        offset += DEF_READ_LENGTH;
            //      }

            //      m_baPassword[ 16 ] = 0x80;
            //      Array.Clear( m_baPassword,17, 47 );
            //      m_baPassword[ 56 ] = 0x80;
            //      m_baPassword[ 57 ] = 0x0A;
            //      m_valContext.Update( m_baPassword, DEF_PAS_LEN );
            //      m_valContext.StoreDigest();      
            //      MakeKey( 0 );
            //      DecryptBuffer( m_baPoint, DEF_READ_LENGTH );
            //      DecryptBuffer( m_baHash, DEF_READ_LENGTH );
            //      m_baPoint[16] = 0x80;           
            //      Array.Clear( m_baPoint, 17, 47 );
            //      m_baPoint[ 56 ] = 0x80;
            //      MD5Context mdContext2 = new MD5Context();
            //      mdContext2.Update( m_baPoint, DEF_PAS_LEN );
            //      mdContext2.StoreDigest();

            //      return MemoryCompare( mdContext2.Digest, m_baHash, DEF_READ_LENGTH );
            PrepareValContext();

            MakeKey(0);

            DecryptBuffer(m_baPoint, DEF_READ_LENGTH);
            DecryptBuffer(m_baHash, DEF_READ_LENGTH);

            m_baPoint[16] = 0x80;
            Array.Clear(m_baPoint, 17, 47);
            m_baPoint[56] = 0x80;
            MD5Context mdContext2 = new MD5Context();
            mdContext2.Update(m_baPoint, DEF_PAS_LEN);
            mdContext2.StoreDigest();

            return MemoryCompare(mdContext2.Digest, m_baHash, DEF_READ_LENGTH);
        }
        /// <summary>
        /// 
        /// </summary>
        private void PrepareValContext()
        {
            MD5Context mdContext = new MD5Context();
            mdContext.Update(m_baPassword, DEF_PAS_LEN);
            mdContext.StoreDigest();

            m_valContext = new MD5Context();
            int iOffset = 0;
            int iKeyOffset = 0;
            int iCopySize = 5;

            while (iOffset != DEF_READ_LENGTH)
            {
                if (DEF_PAS_LEN - iOffset < 5)
                {
                    iCopySize = DEF_PAS_LEN - iOffset;
                }

                Buffer.BlockCopy(mdContext.Digest, iKeyOffset, m_baPassword, iOffset, iCopySize);
                iOffset += iCopySize;

                if (iOffset == DEF_PAS_LEN)
                {
                    m_valContext.Update(m_baPassword, DEF_PAS_LEN);
                    iKeyOffset = iCopySize;
                    iCopySize = 5 - iCopySize;
                    iOffset = 0;
                    continue;
                }

                iKeyOffset = 0;
                iCopySize = 5;
                Buffer.BlockCopy(m_baDocumentID, 0, m_baPassword, iOffset, DEF_READ_LENGTH);
                iOffset += DEF_READ_LENGTH;
            }

            m_baPassword[16] = 0x80;
            Array.Clear(m_baPassword, 17, 47);
            m_baPassword[56] = 0x80;
            m_baPassword[57] = 0x0A;
            m_valContext.Update(m_baPassword, DEF_PAS_LEN);
            m_valContext.StoreDigest();
        }
        /// <summary>
        /// Decripts buffer
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="length">The length.</param>
        private void DecryptBuffer(byte[] data, int length)
        {
            byte xorIndex;

            for (int i = 0; i < length; i++)
            {
                m_key.x = (byte)((m_key.x + 1) % DEF_INC_BYTE_MAXVAL);
                m_key.y = (byte)((m_key.status[m_key.x] + m_key.y) % DEF_INC_BYTE_MAXVAL);
                Swap(ref m_key.status[m_key.x], ref m_key.status[m_key.y]);
                xorIndex = (byte)((m_key.status[m_key.x] + m_key.status[m_key.y]) % DEF_INC_BYTE_MAXVAL);
                data[i] ^= m_key.status[xorIndex];
            }
        }

        #endregion

        #region Implementation / Password hash
        /// <summary>
        /// Gets the password hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        internal static uint GetPasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0;

            // Truncate the password to 15 symbols
            if (password.Length > 15)
                password = password.Substring(0, 15);

            ushort lowerOrder = GetLowOrderHash(password);
            ushort hightOrder = GetHighOrderHash(password);
            uint result = hightOrder;
            result <<= 16;
            result |= lowerOrder;

            //result = RevertBytes( result );

            return result;
        }
        /// <summary>
        /// Reverts the bytes.
        /// </summary>
        /// <param name="changeVal">The change val.</param>
        /// <returns></returns>
        private static uint RevertBytes(uint changeVal)
        {
            uint result = 0;
            for (int i = 0; i < 4; i++)
            {
                result |= (changeVal & 0x000000FF);
                if (i < 3)
                {
                    result <<= 8;
                    changeVal >>= 8;
                }
            }

            return result;
        }
        /// <summary>
        /// Gets the high order hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        private static ushort GetHighOrderHash(string password)
        {
            char curChar;
            ushort result = initCodeArr[password.Length - 1];
            int shift = 15 - password.Length;

            for (int charIndex = 0, cnt = password.Length; charIndex < cnt; charIndex++)
            {
                curChar = password[charIndex];
                bool[] bitsArr = GetCharBits7(curChar);
                for (int bitIndex = 0; bitIndex < 7; bitIndex++)
                {
                    if (bitsArr[bitIndex])
                        result ^= encryptMatrix[bitIndex, shift + charIndex];
                }
            }
            return result;
        }
        /// <summary>
        /// Gets the low order password hash.
        /// </summary>
        /// <param name="password">Password to hash.</param>
        /// <returns>Hash value for the password string.</returns>
        private static ushort GetLowOrderHash(string password)
        {
            if (password == null)
                return 0;//throw new ArgumentNullException( "password" );

            ushort usHash = 0;

            for (int iCharIndex = 0, len = password.Length; iCharIndex < len; iCharIndex++)
            {
                bool[] bits = GetCharBits15(password[iCharIndex]);
                bits = RotateBits(bits, iCharIndex + 1);
                ushort curNumber = GetUInt16FromBits(bits);
                usHash ^= curNumber;
            }

            return (ushort)(usHash ^ password.Length ^ DEF_PASSWORD_CONST);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="charToConvert"></param>
        /// <returns></returns>
        private static bool[] GetCharBits7(char charToConvert)
        {
            ushort curBit = 1;
            bool[] arrResult = new bool[7];
            ushort charVal = Convert.ToUInt16(charToConvert);
            bool takeHigh = ((charVal & 0x00ff) == 0) ? true : false;
            if (takeHigh)
                charVal >>= 8;

            for (int i = 0; i < 7; i++)
            {
                arrResult[i] = ((charVal & curBit) == curBit);
                curBit <<= 1;
            }

            return arrResult;
        }
        /// <summary>
        /// Converts character to 15 bits sequence
        /// </summary>
        /// <param name="charToConvert">Character to convert.</param>
        private static bool[] GetCharBits15(char charToConvert)
        {
            bool[] arrResult = new bool[15];
            ushort usSource = Convert.ToUInt16(charToConvert);
            ushort curBit = 1;

            for (int i = 0; i < 15; i++)
            {
                arrResult[i] = ((usSource & curBit) == curBit);
                curBit <<= 1;
            }

            return arrResult;
        }
        /// <summary>
        /// Converts bits array to UInt16 value.
        /// </summary>
        /// <param name="bits">Array to convert.</param>
        /// <returns>Converted UInt16 value.</returns>
        private static ushort GetUInt16FromBits(bool[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException("bits");

            if (bits.Length > 16)
                throw new ArgumentOutOfRangeException("There can't be more than 16 bits");

            //      bool[] arrResult = new bool[ 15 ];
            ushort usResult = 0;
            ushort curBit = 1;

            for (int i = 0, len = bits.Length; i < len; i++)
            {
                if (bits[i]) usResult += curBit;
                curBit <<= 1;
            }

            return usResult;
        }
        /// <summary>
        /// Rotates (cyclic shift) bits in the array specified number of times
        /// </summary>
        /// <param name="bits">Array to rotate</param>
        /// <param name="count">Number of times to rotate</param>
        /// <returns>Rotated array.</returns>
        private static bool[] RotateBits(bool[] bits, int count)
        {
            if (bits == null)
                throw new ArgumentNullException("bits");

            if (bits.Length == 0)
                return bits;

            if (count < 0)
                throw new ArgumentOutOfRangeException("count can't be less than zero");

            bool[] arrResult = new bool[bits.Length];

            for (int i = 0, len = bits.Length; i < len; i++)
            {
                int newPos = (i + count) % len;

                arrResult[newPos] = bits[i];
            }

            return arrResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static int Round(int value, int degree)
        {
            if (degree == 0)
                throw new ArgumentOutOfRangeException("degree can't be 0");

            int mod = value % degree;

            return value - mod + degree;
        }
        #endregion
    }
}
