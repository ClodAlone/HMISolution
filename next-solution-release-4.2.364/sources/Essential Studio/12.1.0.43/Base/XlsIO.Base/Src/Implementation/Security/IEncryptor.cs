#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation.Security
{
	/// <summary>
	/// Summary description for IEncryptor.
	/// </summary>
	public interface IEncryptor
	{
    /// <summary>
    /// Sets information required to encrypt the document.
    /// </summary>
    /// <param name="docId">Unique document id.</param>
    /// <param name="password">Encryption password.</param>
    void SetEncryptionInfo( byte[] docId, string password );
    /// <summary>
    /// Encrypts DataProvider and writes result back into it.
    /// </summary>
    /// <param name="provider">Provider to encrypt.</param>
    /// <param name="offset">Offset to the first byte to encrypt.</param>
    /// <param name="length">Number of bytes to encrypt.</param>
    /// <param name="streamPosition">Position of the data in the stream.</param>
    void Encrypt( DataProvider provider, int offset, int length, long streamPosition );
    /// <summary>
    /// Encrypts DataProvider and writes result back into it.
    /// </summary>
    /// <param name="data">Data to encrypt.</param>
    /// <param name="offset">Offset to the first byte to encrypt.</param>
    /// <param name="length">Number of bytes to encrypt.</param>
    /// <param name="streamPosition">Position of the data in the stream.</param>
    void Encrypt( byte[] data, int offset, int length, long streamPosition );
    /// <summary>
    /// Creates FilPassRecord that corresponds to this document.
    /// This method can only be called after encryption info was set.
    /// </summary>
    /// <returns>Created record.</returns>
    FilePassRecord GetFilePassRecord();
  }
}
