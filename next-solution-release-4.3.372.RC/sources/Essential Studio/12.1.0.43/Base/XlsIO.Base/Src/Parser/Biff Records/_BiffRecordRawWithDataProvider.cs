#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for BiffRecordRawWithDataProvider.
	/// </summary>
	[ CLSCompliant( false ) ]
	public abstract class BiffRecordRawWithDataProvider
#if DEBUG
    : BiffRecordRaw
#else
    : BiffRecordWithStreamPos
#endif
    , IDisposable
	{
    #region Class members
    /// <summary>
    /// Object that gives access to the record's data.
    /// </summary>
    protected DataProvider m_provider;
    #endregion

    #region Class initialize/finilize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
		public BiffRecordRawWithDataProvider()
		{
		}
    /// <summary>
    /// 
    /// </summary>
    ~BiffRecordRawWithDataProvider()
    {
      Dispose();
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public abstract void ParseStructure();
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_provider = Syncfusion.XlsIO.Implementation.ApplicationImpl.CreateDataProvider();
      m_provider.EnsureCapacity( iLength );
      m_iLength = iLength;
      provider.CopyTo( iOffset, m_provider, 0, iLength );
      ParseStructure();

      if( !NeedDataArray )
      {
        //m_data = new byte[ 0 ];
        m_provider.Clear();
        AutoGrowData = true;
      }
    }
    /// <summary>
    /// In this method, the class must pack all of its properties into
    /// an internal Data array: m_data. This method is called by
    /// FillStream, when the record must be serialized into stream.
    /// </summary>
    public abstract void InfillInternalData( ExcelVersion version );
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <returns>Size of the record data.</returns>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      InfillInternalData( version );

      if( m_iLength > 0 )
      {
        //Buffer.BlockCopy( m_data, 0, arrBuffer, iOffset, m_iLength );
        //provider.WriteBytes( iOffset, m_data, 0, m_iLength );
        m_provider.CopyTo( 0, provider, iOffset, m_iLength );
      }
      //throw new NotImplementedException( TypeCode.ToString() );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override object Clone()
    {
      BiffRecordRawWithDataProvider result = ( BiffRecordRawWithDataProvider )base.Clone ();
      IntPtr heap = IntPtr.Zero;

#if !SILVERLIGHT && !WINRT && !WP
      if( m_provider is IntPtrDataProvider )
      {
        heap = ( m_provider as IntPtrDataProvider ).HeapHandle;
      }
#endif

      result.m_provider = ( heap != IntPtr.Zero ) ?
        Syncfusion.XlsIO.Implementation.ApplicationImpl.CreateDataProvider( heap ) :
        Syncfusion.XlsIO.Implementation.ApplicationImpl.CreateDataProvider();

      return result;
    }
    #endregion

    #region Class Get methods
    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <returns>Retrieved string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// if Offset + iStrLen is out of data array
    /// </exception>
    protected internal string GetString( int offset, int iStrLen )
    {
      int Bytes;
      return GetString( offset, iStrLen, out Bytes );
    }

    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <param name="iBytesInString">Gets bytes count that this string occupies in the data array.</param>
    /// <returns>Retrieved string.</returns>
    protected internal string GetString( int offset, int iStrLen, out int iBytesInString )
    {
      return GetString( offset, iStrLen, out iBytesInString, false );
    }
    /// <summary>
    /// Gets string from internal record data using GetBytes.
    /// </summary>
    /// <param name="offset">Offset of starting byte.</param>
    /// <param name="iStrLen">Length of the string.</param>
    /// <param name="iBytesInString">Gets bytes count that this string occupies in the data array.</param>
    /// <param name="isByteCounted">Flag for is bytes count available.</param>
    /// <returns>Retrieved string.</returns>
    protected internal string GetString( int offset, int iStrLen, out int iBytesInString, bool isByteCounted )
    {
      return m_provider.ReadString( offset, iStrLen, out iBytesInString, isByteCounted );
    }
    /// <summary>
    /// Detect type of string and extracts it.
    /// </summary>
    /// <param name="offset">Record data offset.</param>
    /// <param name="continuePos">Contain next position.</param>
    /// <param name="continueCount">Number of elements in the continuePos collection.</param>
    /// <param name="iBreakIndex">Current index in the continuePos array.</param>
    /// <param name="length">Length of string record.</param>
    /// <param name="rich">Array of rich formatting values.</param>
    /// <param name="extended">Array of unknown FarEast data.</param>
    /// <returns>Extracted string.</returns>
    protected string GetUnkTypeString( int offset, IList<int> continuePos, int continueCount,
      ref int iBreakIndex, out int length, out byte[] rich, out byte[] extended )
    {
      string  retValue = null;//string.Empty;
      int     retLength = 3;

      rich = null;
      extended = null;

      int iCurPos = offset;

      ushort iLen = m_provider.ReadUInt16( iCurPos );
      byte btFlags = m_provider.ReadByte( iCurPos + 2 );

      bool bIsUnicode    = ( btFlags & 0x1 ) == 1;
      bool bIsUniFarEast = ( ( btFlags & 4 ) != 0 );
      bool bIsUniRich    = ( ( btFlags & 8 ) != 0 );

      int iStrOffset = 3;
      short sRichRuns = 0;
      int iFarEastSize = 0;

      if( bIsUniRich )
      {
        sRichRuns = m_provider.ReadInt16( iCurPos + iStrOffset );
        iStrOffset += 2;
        retLength += 2;
      }
      
      if( bIsUniFarEast )
      {
        iFarEastSize = m_provider.ReadInt32( iCurPos + iStrOffset );
        iStrOffset += 4;
        retLength += 4;
      }

      // TODO: continue to fix this method.
      int iStringStart = iCurPos + iStrOffset;
      int iCurChar = 0;

      Encoding encoding = bIsUnicode
        ? Encoding.Unicode
        : BiffRecordRaw.LatinEncoding;

      while( iCurChar < iLen )
      {
        int iDesiredLen = bIsUnicode ? ( iLen - iCurChar ) * 2 : iLen - iCurChar;
        
        // Get length of string or part of it in continue record.
        int iBreakPos = FindNextBreak( continuePos, continueCount, iStringStart, ref iBreakIndex );
        int iBytesLeft  = iBreakPos - iStringStart;

        if( iDesiredLen <= iBytesLeft )
        {
          string strPart = m_provider.ReadString( iStringStart, iDesiredLen, encoding, bIsUnicode );
          retValue = ( retValue == null )
            ? strPart
            : retValue + strPart;

          retLength += iDesiredLen;
          break;
        }
        else
        {
          // Get part of string.
          string strPart = m_provider.ReadString( iStringStart, iBytesLeft, encoding, bIsUnicode );
          retValue = ( retValue == null )
            ? strPart
            : retValue + strPart;

          //          retValue += encoding.GetString( m_data, iStringStart, iBytesLeft );
          iCurChar += bIsUnicode ? iBytesLeft / 2 : iBytesLeft;
          byte btOption = m_provider.ReadByte( iStringStart + iBytesLeft );

          if( btOption == 0 || btOption == 1 )
          {
            bIsUnicode = ( btOption == 1 );

            encoding = bIsUnicode
              ? Encoding.Unicode
              : BiffRecordRaw.LatinEncoding;

            iStringStart++;
            retLength++;
          }

          iStringStart += iBytesLeft;
          retLength += iBytesLeft;
        }
      }

      if( bIsUniRich )
      {
        int iSize = sRichRuns * 4;
        rich = new byte[ iSize ];
        m_provider.ReadArray( offset + retLength, rich, iSize );
        retLength += iSize;
      }

      if( bIsUniFarEast )
      {
        extended = new byte[ iFarEastSize ];
        m_provider.ReadArray( offset + retLength, extended, iFarEastSize );
        retLength += iFarEastSize;
      }

      length = retLength;

      return ( retValue != null )
        ? retValue
        : string.Empty;
    }
    #endregion

    #region Class Set methods
    /// <summary>
    /// Sets byte in internal record data array values.
    /// </summary>
    /// <param name="offset">Offset in internal record data array to start from.</param>
    /// <param name="value">Byte value to set.</param>
    internal protected void SetByte( int offset, byte value )
    {
      m_provider.WriteByte( offset, value );
    }

    /// <summary>
    /// Sets ushort in internal record data array values.
    /// </summary>
    /// <param name="offset">Offset in internal record data array to start from.</param>
    /// <param name="value">Value to set.</param>
    internal protected void SetUInt16( int offset, ushort value )
    {
      m_provider.WriteUInt16( offset, value );
    }

    /// <summary>
    /// Sets bytes in internal record data array values.
    /// </summary>
    /// <param name="offset">Offset in internal record data array to start from.</param>
    /// <param name="value">Array of bytes to set.</param>
    /// <param name="pos">Position in value array to the data that will be set.</param>
    /// <param name="length">Length of the data.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If value array is NULL.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If pos or length would be less than zero  or their sum would be more
    ///   than size of value array.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If internal record data array is too small for receiving value array
    ///   and AutoGrowData is False.
    /// </exception>
    internal protected void SetBytes( int offset, byte[] value, int pos, int length )
    {
      m_provider.WriteBytes( offset, value, pos, length );
    }

    /// <summary>
    /// Sets bytes in internal record data array values.
    /// </summary>
    /// <param name="offset">Offset in internal record data array to start from.</param>
    /// <param name="value">Array of bytes to set.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If value array is NULL.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If pos or length would be less than zero  or their sum would be more
    ///   than size of value array.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   If internal record data array is too small for receiving value array
    ///   and AutoGrowData is False.
    /// </exception>
    internal protected void SetBytes( int offset, byte[] value )
    {
      SetBytes( offset, value, 0, value.Length );
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method
    /// without string length.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <returns>Size of the string in bytes.</returns>
    internal protected int SetStringNoLen( int offset, string value )
    {
      return SetStringNoLen( offset, value, false );
    }

    /// <summary>
    /// Sets string in internal record data array using SetBytes method
    /// without string length.
    /// </summary>
    /// <param name="offset">Offset to the string.</param>
    /// <param name="value">Value of the string.</param>
    /// <param name="bEmptyCompressed">Indicates whether write compressed attribute for empty strings.</param>
    /// <returns>Size of the string in bytes.</returns>
    internal protected int SetStringNoLen( int offset, string value, bool bEmptyCompressed )
    {
      if( value == null || value.Length == 0 )
      {
        if( bEmptyCompressed )
        {
          if( AutoGrowData )
          {
            m_provider.EnsureCapacity( offset );
          }

          m_provider.WriteByte( offset, 0 );
          return 1;
        }

        return 0;
      }

      byte[] tmpData = Encoding.Unicode.GetBytes( value );

      if( AutoGrowData )
      {
        m_provider.EnsureCapacity( offset + tmpData.Length );
      }

      // we always save strings in Unicode
      m_provider.WriteByte( offset, 1 );

      // save string
      m_provider.WriteBytes( offset + 1, tmpData, 0, tmpData.Length );

      return tmpData.Length + 1;
    }

    #endregion

    #region IDisposable Members
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if( m_provider != null )
      {
        m_provider.Dispose();
        m_provider = null;
      }

      GC.SuppressFinalize( this );
    }

    #endregion
  }
}
