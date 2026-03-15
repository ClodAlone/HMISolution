#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation.Security
{
	/// <summary>
	/// Summary description for IEncryptor.
	/// </summary>
	public interface IDecryptor
	{
//    /// <summary>
//    /// Decrypts stream. Puts result into MemoryStream.
//    /// </summary>
//    /// <param name="stream">Stream to decrypt.</param>
//    /// <returns>Decrypted stream.</returns>
//    MemoryStream Decrypt( Stream stream );
    /// <summary>
    /// Decrypts DataProvider and writes result back into it.
    /// </summary>
    /// <param name="provider">Provider to decrypt.</param>
    /// <param name="offset">Offset to the first byte to decrypt.</param>
    /// <param name="length">Number of bytes to decrypt.</param>
    /// <param name="streamPosition">Position of the record in the stream.</param>
    void Decrypt( DataProvider provider, int offset, int length, long streamPosition );
    /// <summary>
    /// Decrypts byte array and writes result back into it.
    /// </summary>
    /// <param name="buffer">Array to decrypt.</param>
    /// <param name="offset">Offset to the first byte to decrypt.</param>
    /// <param name="length">Number of bytes to decrypt.</param>
    void Decrypt( byte[] buffer, int offset, int length );
    /// <summary>
    /// Sets information required to decrypt.
    /// </summary>
    /// <param name="docId">Unique document id.</param>
    /// <param name="encryptedDocId">Encrypted document id.</param>
    /// <param name="digest">Digest used to verify the entered password.</param>
    /// <param name="password">Password to use for verification and decryption.</param>
    /// <returns>True if password was verified and method succeeded.</returns>
    bool SetDecryptionInfo( byte[] docId, byte[] encryptedDocId, byte[] digest, string password );
  }
}
