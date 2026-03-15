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
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Syncfusion.Pdf.Cryptography
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
    public class SHA1Managed : SHA1
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
    public class SHA256Managed : SHA256
    {
    }
    public class SHA256
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
        [SecuritySafeCritical]
        public byte[] ComputeHash(byte[] buffer)
        {
            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);

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
    public class MD5CryptoServiceProvider : MD5
    {
    }
    public class MD5
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
        [SecuritySafeCritical]
        public byte[] ComputeHash(byte[] buffer)
        {
            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);

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
    public class Rijndael
    {
        private Rijndael()
        {
        }

        public static Rijndael Create()
        {
            return new Rijndael();
        }

        public int KeySize
        {
            get;
            set;
        }

        public byte[] Key
        {
            get;
            set;
        }

        public byte[] IV
        {
            get;
            set;
        }

        public PaddingMode Padding
        {
            get;
            set;
        }

        public CipherMode Mode
        {
            get;
            set;
        }

        [SecuritySafeCritical]
        public byte[] Encrypt(byte[] data)
        {
           // data = new byte[] { 42, 18, 209, 249, 164, 29, 165, 241, 195, 33, 123, 55, 81, 61, 79, 137, 183, 235, 14, 246, 204, 219, 91, 119, 177, 119, 134, 94, 189, 216, 158, 129 };
            byte[] result = null;
            UInt32 keySize = (UInt32)KeySize;

            IBuffer encrypted;

            IBuffer buffer;

            IBuffer iv = null;
            SymmetricKeyAlgorithmProvider algorithm;

            if (Padding == PaddingMode.None)
            {
                if (Mode == CipherMode.CBC)
                    algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");
                else
                {
                    algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_ECB");
                   // byte[] temp = new byte[32];
                   // Array.Copy(data, temp, data.Length);
                   // data = temp;
                    IV = null;
                }

            }
            else
            {
                if (Mode == CipherMode.CBC)
                    algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
                else
                    algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_ECB_PKCS7");
            }

            //Key = new byte[] { 141, 65, 167, 115, 125, 8, 77, 231, 242, 240, 211, 8, 27, 51, 179, 25, 87, 87, 71, 58, 225, 80, 14, 118, 76, 79, 5, 38, 103, 247, 246, 227 };
            //Key = new byte[] { 143, 249, 243, 70, 51, 150, 185, 6, 221, 199, 76, 143, 138, 168, 56, 171, 198, 98, 102, 137, 246, 214, 6, 90, 55, 195, 26, 80, 93, 85, 45, 58 };
            IBuffer keymaterial = CryptographicBuffer.CreateFromByteArray(Key);

            CryptographicKey key = algorithm.CreateSymmetricKey(keymaterial);

            iv = CryptographicBuffer.CreateFromByteArray(IV);

            buffer = CryptographicBuffer.CreateFromByteArray(data);

            encrypted = CryptographicEngine.Encrypt(key, buffer, iv);
            CryptographicBuffer.CopyToByteArray(encrypted,out result);

            return result;
        }

        public byte[] Decrypt(byte[] data)
        {
            byte[] result = null;
            UInt32 keySize = (UInt32)KeySize;

            IBuffer encrypted;

            IBuffer buffer;

            IBuffer iv = null;
            SymmetricKeyAlgorithmProvider algorithm;

            if (Padding == PaddingMode.None)
            {
                if (Mode == CipherMode.CBC)
                    algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");
                else
                    algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_ECB");

            }
            else
            {
                if (Mode == CipherMode.CBC)
                    algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
                else
                    algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_ECB_PKCS7");
            }

            IBuffer keymaterial = CryptographicBuffer.CreateFromByteArray(Key);

            CryptographicKey key = algorithm.CreateSymmetricKey(keymaterial);

            iv = CryptographicBuffer.CreateFromByteArray(IV);

            buffer = CryptographicBuffer.CreateFromByteArray(data);

            encrypted = CryptographicEngine.Decrypt(key, buffer, iv);
            CryptographicBuffer.CopyToByteArray(encrypted, out result);

            return result;
        }       

    }
    public enum PaddingMode
    {
        None,
        PKCS7
    }
    public enum CipherMode
    {
        CBC,
        ECB,
    }

}
