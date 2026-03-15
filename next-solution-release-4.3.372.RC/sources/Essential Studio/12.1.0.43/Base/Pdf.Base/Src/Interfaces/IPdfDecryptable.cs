#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// Interface of the objects that support Decryptable of their internals.
    /// </summary>
    internal interface IPdfDecryptable
    {
        /// <summary>
        /// Gets a value indicating whether [was encrypted].
        /// </summary>
        /// <value><c>true</c> if [was encrypted]; otherwise, <c>false</c>.</value>
        bool WasEncrypted { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IPdfDecryptable"/> is decrypted.
        /// </summary>
        /// <value><c>true</c> if decrypted; otherwise, <c>false</c>.</value>
        bool Decrypted { get; }

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Decrypts the specified encryptor.
        /// </summary>
        /// <param name="encryptor">The encryptor.</param>
        /// <param name="currObjNumber">The curr obj number.</param>
        void Decrypt(PdfEncryptor encryptor, long currObjNumber);
#endif
    }

}