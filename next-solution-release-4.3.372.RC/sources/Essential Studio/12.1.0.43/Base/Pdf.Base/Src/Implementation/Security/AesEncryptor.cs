#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WP
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// Implements both the 128 bit and 256 bit AES encryption.
    /// </summary>
    class AesEncryptor
    {

        #region Constants
        /// <summary>
        /// Block size for 128 bit encryption
        /// </summary>
        private int c_blockSize = 16;
        #endregion

        #region Fields
        /// <summary>
        /// Aes encryptor
        /// </summary>
        private Aes m_aes;

        /// <summary>
        /// Cypher Blocking Chain vector
        /// </summary>
        private byte[] m_cbcV = new byte[16];

        /// <summary>
        /// vector that represents next block of the Chain 
        /// </summary>
        private byte[] m_nextBlockV = new Byte[16];

        /// <summary>
        /// Offset of the initialization vector.
        /// </summary>
        private int m_ivOff = 0;

        /// <summary>
        /// Buffer containing the initialization vector
        /// </summary>
        private byte[] m_buf = new byte[16];

        /// <summary>
        /// Reprents the current process either encryption/decryption
        /// </summary>
        private bool m_isEncryption;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AesEncryptor class.
        /// </summary>
        internal AesEncryptor(byte[] key,byte[] iv, bool isEncryption)
        {
            if (key.Length == c_blockSize)
                m_aes = new Aes(Aes.KeySize.Bits128, key);
            else
                m_aes = new Aes(Aes.KeySize.Bits256, key);

            Array.Copy(iv, 0, m_buf, 0, iv.Length);
            Array.Copy(iv, 0, m_cbcV, 0, iv.Length);            

            if(isEncryption)
                m_ivOff = m_buf.Length;

            m_isEncryption = isEncryption;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Does the initial AES encryption process in CBC mode.
        /// </summary>
        /// <param name="input">input data stream.</param>
        /// <param name="inOff">Offset of input stream.</param>
        /// <param name="length">length of the input stream.</param>
        /// <param name="output">output encrypted stream.</param>
        /// <param name="input">input data stream.</param> 
        internal void ProcessBytes(byte[] input,int inOff,int length,byte[] output,int outOff)
        {
            if (length < 0)
            {
                throw new ArgumentException("input data length cannot be negative");
            }            
          
            int resultLen = 0;
            int bytesLeft = m_buf.Length - m_ivOff;

            if (length > bytesLeft)
            {
                Array.Copy(input, inOff, m_buf, m_ivOff, bytesLeft);

                resultLen += ProcessBlock(m_buf, 0, output, outOff);

                m_ivOff = 0;
                length -= bytesLeft;
                inOff += bytesLeft;

                while (length > m_buf.Length)
                {
                    resultLen += ProcessBlock(input, inOff, output, outOff + resultLen);

                    length -= c_blockSize;
                    inOff += c_blockSize;
                    
                }
            }

            Array.Copy(input, inOff, m_buf, m_ivOff, length);

            m_ivOff += length;

            
        }

        
        /// <summary>
        /// Does the final encryption after padding (PKCS7 standards).
        /// </summary>          
        /// <param name="outBytes">output encrypted stream.</param>
        /// <param name="outOff">current offset of the output stream.</param> 
        internal int Finalize(byte[] output)
        {
            
            int resultLen = 0;
            int outOff = 0;

            if (m_isEncryption)
            {
                if (m_ivOff == c_blockSize)
                {                   
                    resultLen = ProcessBlock(m_buf, 0, output, outOff);
                    m_ivOff = 0;
                }

                //Add padding using PKCS7 padding scheme
                AddPadding(m_buf, m_ivOff);

                resultLen += ProcessBlock(m_buf, 0, output, outOff + resultLen);
            }
            else
            {

                if (m_ivOff == c_blockSize)
                {
                    resultLen = ProcessBlock(m_buf, 0, output, 0);
                    m_ivOff = 0;
                }              

                //check the padding scheme
                resultLen -= CheckPadding(output);
               
            }

                return resultLen;
        }

        /// <summary>
        /// Calculates the correct block size for the given input data length
        /// </summary>          
        /// <param name="length">length of the data.</param> 
        internal int GetBlockSize(int length)
        {
            int total = length + m_ivOff;

            int leftOver = total % m_buf.Length;

            if (leftOver == 0)
            {
                return total - m_buf.Length;
            }

            return total - leftOver;
        }

        /// <summary>
        /// Calculates the length for padding
        /// </summary>          
        /// <param name="length">length of the data.</param>         
        internal int CalculateOutputSize()
        {
            int total = m_ivOff;

            int leftOver = total % m_buf.Length;

            if (leftOver == 0)
            {
                if (m_isEncryption)
                {
                    return total + m_buf.Length;
                }
                return total;
            }

            return total - leftOver + m_buf.Length;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Does Cypher Blocking chain operation and calls for encryption.
        /// </summary>
        /// <param name="input">input data stream.</param>
        /// <param name="inOff">Offset of input stream.</param>       
        /// <param name="outBytes">output encrypted stream.</param>
        /// <param name="outOff">current offset of the output stream.</param> 
        private int ProcessBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
        {
            int length = 0;

            if ((inOff + c_blockSize) > input.Length)
            {
                throw new ArgumentException("input buffer length is too short");
            }

            if (m_isEncryption)
            {
                //Step1 : XOR the cipher blocking chain vector with the input data
                for (int i = 0; i < c_blockSize; i++)
                {
                    m_cbcV[i] ^= input[inOff + i];
                }

                //Step2 : Encrypt the data and replace the CBC vector
                length = m_aes.Cipher(m_cbcV, outBytes, outOff);

                Array.Copy(outBytes, outOff, m_cbcV, 0, m_cbcV.Length);
            }
            else
            {
                Array.Copy(input, inOff, m_nextBlockV, 0, c_blockSize);

                length = m_aes.InvCipher(m_nextBlockV,outBytes, outOff);


                // XOR the cipher blocking chain vector with the output data
                
                for (int i = 0; i < c_blockSize; i++)
                {
                    outBytes[outOff + i] ^= m_cbcV[i];
                }

               //swap and move the buffer to the next position
                
                byte[] tmp;

                tmp = m_cbcV;
                m_cbcV = m_nextBlockV;
                m_nextBlockV = tmp;

            }

            return length;


        }
        #endregion

        #region Helper Methods   

        /// <summary>
        /// Add padding in the PKCS7 standards.
        /// </summary>          
        /// <param name="input">input data.</param>
        /// <param name="inOff">offset where the padding has to be done.</param> 
        private static int AddPadding(byte[] input,int inOff)
        {
            byte data = (byte)(input.Length - inOff);

            while (inOff < input.Length)
            {
                input[inOff] = data;
                inOff++;
            }

            return data;
        }

        /// <summary>
        /// Checks for the padding added in the encrypted cypher.
        /// </summary>          
        /// <param name="input">input data.</param>       
        private static int CheckPadding(byte[] input)
        {

            int count = input[input.Length - 1] & 0xff;

            for (int i = 1; i <= count; i++)
            {
                if (input[input.Length - i] != count)
                    throw new ArgumentException("Error while decrypting padding block");
            }

            return count;
        }
        #endregion

    }
}
#endif