#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace System.Security.Cryptography
{
    class Cryptography
    {
    }
    public class HMACSHA1
    {
        private byte[] m_key;
        public byte[] ComputeHash()
        {
            //out 
            IBuffer buffMsg;
            CryptographicKey hmacKey;
            IBuffer buffHMAC;
            // Create a MacAlgorithmProvider object for the specified algorithm.
            MacAlgorithmProvider objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);

            // Create a CryptographicHash object. This object can be reused to continually
            // hash new messages.
            buffMsg = CryptographicBuffer.GenerateRandom(20);
            // Create a key to be signed with the message.
            IBuffer buffKeyMaterial = CryptographicBuffer.CreateFromByteArray(Key);
            hmacKey = objMacProv.CreateKey(buffKeyMaterial);

            // Sign the key and message together.
            buffHMAC = CryptographicEngine.Sign(hmacKey, buffMsg);

            // Verify that the HMAC length is correct for the selected algorithm
            if (buffHMAC.Length != objMacProv.MacLength)
            {
                throw new Exception("Error computing digest");
            }

            byte[] hash = new byte[buffHMAC.Length];
            CryptographicBuffer.CopyToByteArray(buffHMAC, out hash);
            return hash;
        }
        //
        // Summary:
        //     Computes the hash value for the specified byte array.
        //
        // Parameters:
        //   buffer:
        //     The input to compute the hash code for.
        //
        // Returns:
        //     The computed hash code.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.ObjectDisposedException:
        //     The object has already been disposed.
        public byte[] ComputeHash(byte[] buffer)
        {
            //out 
            IBuffer buffMsg;
            CryptographicKey hmacKey;
            IBuffer buffHMAC;
            // Create a MacAlgorithmProvider object for the specified algorithm.
            MacAlgorithmProvider objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);

            // Create a CryptographicHash object. This object can be reused to continually
            // hash new messages.
            buffMsg = CryptographicBuffer.CreateFromByteArray(buffer);
            // Create a key to be signed with the message.
            IBuffer buffKeyMaterial = CryptographicBuffer.CreateFromByteArray(Key);
            hmacKey = objMacProv.CreateKey(buffKeyMaterial);

            // Sign the key and message together.
            buffHMAC = CryptographicEngine.Sign(hmacKey, buffMsg);

            // Verify that the HMAC length is correct for the selected algorithm
            if (buffHMAC.Length != objMacProv.MacLength)
            {
                throw new Exception("Error computing digest");
            }

            byte[] hash = new byte[buffHMAC.Length];
            CryptographicBuffer.CopyToByteArray(buffHMAC, out hash);
            return hash;
        }
        //
        // Summary:
        //     Computes the hash value for the specified System.IO.Stream object.
        //
        // Parameters:
        //   inputStream:
        //     The input to compute the hash code for.
        //
        // Returns:
        //     The computed hash code.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The object has already been disposed.
        public byte[] ComputeHash(Stream inputStream)
        {
            //out 
            IBuffer buffMsg;
            CryptographicKey hmacKey;
            IBuffer buffHMAC;
            byte[] buffer = new byte[inputStream.Length];
            inputStream.Read(buffer, 0, buffer.Length);
            // Create a MacAlgorithmProvider object for the specified algorithm.
            MacAlgorithmProvider objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);

            // Create a CryptographicHash object. This object can be reused to continually
            // hash new messages.
            buffMsg = CryptographicBuffer.CreateFromByteArray(buffer);
            // Create a key to be signed with the message.
            IBuffer buffKeyMaterial = CryptographicBuffer.CreateFromByteArray(Key);
            hmacKey = objMacProv.CreateKey(buffKeyMaterial);

            // Sign the key and message together.
            buffHMAC = CryptographicEngine.Sign(hmacKey, buffMsg);

            // Verify that the HMAC length is correct for the selected algorithm
            if (buffHMAC.Length != objMacProv.MacLength)
            {
                throw new Exception("Error computing digest");
            }

            byte[] hash = new byte[buffHMAC.Length];
            CryptographicBuffer.CopyToByteArray(buffHMAC, out hash);
            return hash;
        }
        //
        // Summary:
        //     Computes the hash value for the specified region of the specified byte array.
        //
        // Parameters:
        //   buffer:
        //     The input to compute the hash code for.
        //
        //   offset:
        //     The offset into the byte array from which to begin using data.
        //
        //   count:
        //     The number of bytes in the array to use as data.
        //
        // Returns:
        //     The computed hash code.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     count is an invalid value.-or-buffer length is invalid.
        //
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     offset is out of range. This parameter requires a non-negative number.
        //
        //   System.ObjectDisposedException:
        //     The object has already been disposed.
        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
    //out 
            IBuffer buffMsg;
            CryptographicKey hmacKey;
            IBuffer buffHMAC;

            byte[] inputBuffer = new byte[20];
            Array.Copy(buffer, offset, inputBuffer, 0, count);
            // Create a MacAlgorithmProvider object for the specified algorithm.
            MacAlgorithmProvider objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);

            // Create a CryptographicHash object. This object can be reused to continually
            // hash new messages.
            buffMsg = CryptographicBuffer.CreateFromByteArray(inputBuffer);
            // Create a key to be signed with the message.
            IBuffer buffKeyMaterial = CryptographicBuffer.CreateFromByteArray(Key);
            hmacKey = objMacProv.CreateKey(buffKeyMaterial);

            // Sign the key and message together.
            buffHMAC = CryptographicEngine.Sign(hmacKey, buffMsg);

            // Verify that the HMAC length is correct for the selected algorithm
            if (buffHMAC.Length != objMacProv.MacLength)
            {
                throw new Exception("Error computing digest");
            }

            byte[] hash = new byte[buffHMAC.Length];
            CryptographicBuffer.CopyToByteArray(buffHMAC, out hash);
            return hash;
        }

        public byte[] Key 
        { 
            get 
            {
                return m_key;
            }
            set
            {
                m_key = value;
            }
        }
    }
    public class SHA1Managed:SHA1
    {
    }
    public class SHA1
    {
        //
        // Summary:
        //     Computes the hash value for the specified byte array.
        //
        // Parameters:
        //   buffer:
        //     The input to compute the hash code for.
        //
        // Returns:
        //     The computed hash code.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.ObjectDisposedException:
        //     The object has already been disposed.
        public byte[] ComputeHash(byte[] buffer)
        {
            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);

            // Create a CryptographicHash object. This object can be reused to continually
            // hash new messages.
            CryptographicHash objHash = objAlgProv.CreateHash();
            IBuffer buffMsg1 = CryptographicBuffer.CreateFromByteArray(buffer);
            objHash.Append(buffMsg1);
            IBuffer buffHash1 = objHash.GetValueAndReset();
            byte[] hash = new byte[20];
            CryptographicBuffer.CopyToByteArray(buffHash1, out hash);
            return hash;
        }
        //
        // Summary:
        //     Computes the hash value for the specified System.IO.Stream object.
        //
        // Parameters:
        //   inputStream:
        //     The input to compute the hash code for.
        //
        // Returns:
        //     The computed hash code.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The object has already been disposed.
        public byte[] ComputeHash(Stream inputStream)
        {
            byte[] buffer = new byte[inputStream.Length];
            inputStream.Read(buffer, 0, buffer.Length);
            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);

            // Create a CryptographicHash object. This object can be reused to continually
            // hash new messages.
            CryptographicHash objHash = objAlgProv.CreateHash();
            IBuffer buffMsg1 = CryptographicBuffer.CreateFromByteArray(buffer);
            objHash.Append(buffMsg1);
            IBuffer buffHash1 = objHash.GetValueAndReset();
            byte[] hash = new byte[20];
            CryptographicBuffer.CopyToByteArray(buffHash1, out hash);
            return hash;
        }
        //
        // Summary:
        //     Computes the hash value for the specified region of the specified byte array.
        //
        // Parameters:
        //   buffer:
        //     The input to compute the hash code for.
        //
        //   offset:
        //     The offset into the byte array from which to begin using data.
        //
        //   count:
        //     The number of bytes in the array to use as data.
        //
        // Returns:
        //     The computed hash code.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     count is an invalid value.-or-buffer length is invalid.
        //
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     offset is out of range. This parameter requires a non-negative number.
        //
        //   System.ObjectDisposedException:
        //     The object has already been disposed.
        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {

            byte[] inputBuffer = new byte[20];
            Array.Copy(buffer, offset, inputBuffer, 0, count);
            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);

            // Create a CryptographicHash object. This object can be reused to continually
            // hash new messages.
            CryptographicHash objHash = objAlgProv.CreateHash();
            IBuffer buffMsg1 = CryptographicBuffer.CreateFromByteArray(inputBuffer);
            objHash.Append(buffMsg1);
            IBuffer buffHash1 = objHash.GetValueAndReset();
            byte[] hash = new byte[20];
            CryptographicBuffer.CopyToByteArray(buffHash1, out hash);
            return hash;
        }
    }
}
