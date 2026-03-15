#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

using Syncfusion.XlsIO.Implementation.Security;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for IBiffStorage.
	/// </summary>
  public interface IBiffStorage
  {
    /// <summary>
    /// Returns type code of the biff storage. Read-only.
    /// </summary>
    TBIFFRecord TypeCode { get; }
    /// <summary>
    /// Returns code of the biff storage. Read-only.
    /// </summary>
    int RecordCode { get; }
    /// <summary>
    /// Indicates whether data array is required by this record.
    /// </summary>
    bool NeedDataArray { get; }
    /// <summary>
    /// Indicates record position in stream. This is a utility member of class and
    /// is used only in the serialization process. Does not influence the data.
    /// </summary>
    long StreamPos { get; set; }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    int GetStoreSize( ExcelVersion version );
    /// <summary>
    /// Save record data to stream.
    /// </summary>
    /// <param name="writer">Writer that will receive record data.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns>Size of the record.</returns>
    /// <exception cref="System.ArgumentNullException">If writer is NULL.</exception>
    /// <exception cref="System.ApplicationException">
    ///   If m_iLength of internal record data array is less than zero.
    /// </exception>
    int FillStream( BinaryWriter writer, DataProvider provider, IEncryptor encryptor, int streamPosition );
  }
}
