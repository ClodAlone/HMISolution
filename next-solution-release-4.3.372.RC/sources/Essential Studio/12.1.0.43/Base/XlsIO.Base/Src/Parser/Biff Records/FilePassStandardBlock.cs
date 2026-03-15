#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for FilePassStandardBlock.
	/// </summary>
	public class FilePassStandardBlock
	{
    #region Class members
    /// <summary>
    /// Unique document identifier used to initialize the encryption algorithm.
    /// </summary>
    private byte[] m_arrDocumentID = new byte[ 16 ];
    /// <summary>
    /// Encrypted document identifier used to verify the entered password.
    /// </summary>
    private byte[] m_arrEncyptedDocumentID = new byte[ 16 ];
    /// <summary>
    /// Digest used to verify the entered password.
    /// </summary>
    private byte[] m_arrDigest = new byte[ 16 ];
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public FilePassStandardBlock()
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Unique document identifier used to initialize the encryption algorithm. Read-only.
    /// </summary>
    public byte[] DocumentID
    {
      get
      {
        return m_arrDocumentID;
      }
    }
    /// <summary>
    /// Encrypted document identifier used to verify the entered password. Read-only.
    /// </summary>
    public byte[] EncyptedDocumentID
    {
      get
      {
        return m_arrEncyptedDocumentID;
      }
    }
    /// <summary>
    /// Digest used to verify the entered password. Read-only.
    /// </summary>
    public byte[] Digest
    {
      get
      {
        return m_arrDigest;
      }
    }
    /// <summary>
    /// Correct record size.
    /// </summary>
    public const int StoreSize = 16 + 16 + 16;
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    public void ParseStructure( DataProvider provider, int iOffset, int iLength )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      iOffset = provider.ReadArray( iOffset, m_arrDocumentID );
      iOffset = provider.ReadArray( iOffset, m_arrEncyptedDocumentID );
      iOffset = provider.ReadArray( iOffset, m_arrDigest );
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="iLength">Buffer length.</param>
    public void InfillInternalData( DataProvider provider, int iOffset, int iLength )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      int iArrayLength = m_arrDocumentID.Length;
      provider.WriteBytes( iOffset, m_arrDocumentID, 0, iArrayLength );
      iOffset += iArrayLength;

      iArrayLength = m_arrEncyptedDocumentID.Length;
      provider.WriteBytes( iOffset, m_arrEncyptedDocumentID, 0, iArrayLength );
      iOffset += iArrayLength;

      iArrayLength = m_arrDigest.Length;
      provider.WriteBytes( iOffset, m_arrDigest, 0, iArrayLength );
      iOffset += iArrayLength;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static int GetStoreSize( ExcelVersion version )
    {
      return StoreSize;
    }
    #endregion
  }
}
