#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsoBase.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public abstract class MsoBase
    : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const ushort DEF_VERSION_MASK = 0x000F;
    /// <summary>
    /// 
    /// </summary>
    private const ushort DEF_INST_MASK = 0xFFF0;
    /// <summary>
    /// 
    /// </summary>
    private const ushort DEF_INST_START_BIT = 4;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_MAXIMUM_RECORD_SIZE = int.MaxValue;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected ushort m_usVersionAndInst;
    /// <summary>
    /// 
    /// </summary>
    private ushort m_usRecordType;
    /// <summary>
    /// 
    /// </summary>
    private GetNextMsoDrawingData m_dataGetter;
    /// <summary>
    /// 
    /// </summary>
    private MsoBase m_parent;
    /// <summary>
    /// Dictionary key - Type of the MsoBase class, Value - its code.
    /// </summary>
    private static Dictionary<Type, int> s_dicTypeToCode = new Dictionary<Type, int>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Static constructor.
    /// </summary>
    static MsoBase()
    {
      s_dicTypeToCode.Add( typeof( MsofbtClientTextBox ), ( int )MsoRecords.msofbtClientTextbox);
      s_dicTypeToCode.Add( typeof( MsofbtSp ),            ( int )MsoRecords.msofbtSp );
      s_dicTypeToCode.Add( typeof( MsofbtSpgrContainer ), ( int )MsoRecords.msofbtSpgrContainer );
      s_dicTypeToCode.Add( typeof( MsofbtAnchor ),        ( int )MsoRecords.msofbtAnchor );
      s_dicTypeToCode.Add( typeof( MsofbtClientAnchor ),  ( int )MsoRecords.msofbtClientAnchor );
      s_dicTypeToCode.Add( typeof( MsofbtDgContainer ),   ( int )MsoRecords.msofbtDgContainer );
      s_dicTypeToCode.Add( typeof( MsofbtRegroupItems ),  ( int )MsoRecords.msofbtRegroupItems );
      s_dicTypeToCode.Add( typeof( MsofbtDg ),            ( int )MsoRecords.msofbtDg );
      s_dicTypeToCode.Add( typeof( MsofbtDggContainer ),  ( int )MsoRecords.msofbtDggContainer );
      s_dicTypeToCode.Add( typeof( MsofbtOPT ),           ( int )MsoRecords.msofbtOPT );
      s_dicTypeToCode.Add( typeof( MsofbtSpContainer ),   ( int )MsoRecords.msofbtSpContainer );
      s_dicTypeToCode.Add( typeof( MsofbtSplitMenuColors ), ( int )MsoRecords.msofbtSplitMenuColors );
      s_dicTypeToCode.Add( typeof( MsofbtDgg ),           ( int )MsoRecords.msofbtDgg );
      s_dicTypeToCode.Add( typeof( MsofbtBSE ),           ( int )MsoRecords.msofbtBSE );
      s_dicTypeToCode.Add( typeof( MsofbtSpgr ),          ( int )MsoRecords.msofbtSpgr );
      s_dicTypeToCode.Add( typeof( MsofbtBstoreContainer ), ( int )MsoRecords.msofbtBstoreContainer );
      s_dicTypeToCode.Add( typeof( MsofbtClientData ),    ( int )MsoRecords.msofbtClientData );
      s_dicTypeToCode.Add( typeof( MsoUnknown ),          ( int )MsoRecords.msoUnknown );
      s_dicTypeToCode.Add( typeof( MsofbtChildAnchor ),   ( int )MsoRecords.msofbtChildAnchor );
      s_dicTypeToCode.Add( typeof( MsoMetafilePicture ), 0 );
      s_dicTypeToCode.Add( typeof( MsoBitmapPicture ), 0 );
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    public MsoBase()
    {
      Type type = this.GetType();

      s_dicTypeToCode.TryGetValue( type, out m_iCode );
      m_usRecordType = ( ushort )m_iCode;
    }
    /// <summary>
    /// 
    /// </summary>
    public MsoBase( MsoBase parent ) : this()
    {
      m_parent = parent;
    }
    /// <summary>
    /// 
    /// </summary>
    public MsoBase( MsoBase parent, byte[] data, int offset )
      : this( parent, data, offset, null )
    {
      //      m_usVersionAndInst  = BitConverter.ToUInt16( data, offset );
      //      m_usRecordType      = BitConverter.ToUInt16( data, offset + 2 );
      //      m_iLength           = BitConverter.ToInt32( data, offset + 4 );
      //      
      //      m_data = new byte[ m_iLength ];
      //      Array.Copy( data, offset, m_data, 0, m_iLength );
      //
      //      ParseData();
    }
    /// <summary>
    /// 
    /// </summary>
    public MsoBase( MsoBase parent, byte[] data, int offset, GetNextMsoDrawingData dataGetter )
      : this( parent )
    {
      m_dataGetter = dataGetter;
      FillRecord( data, offset );
    }
    /// <summary>
    /// 
    /// </summary>
    public MsoBase( MsoBase parent, Stream stream, GetNextMsoDrawingData dataGetter )
      : this( parent )
    {
      m_dataGetter = dataGetter;
      FillRecord( stream );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int Version
    {
      get
      {
        return BiffRecordRaw.GetUInt16BitsByMask( m_usVersionAndInst, DEF_VERSION_MASK );
      }
      set
      {
        BiffRecordRaw.SetUInt16BitsByMask( ref m_usVersionAndInst, DEF_VERSION_MASK
          , ( ushort ) value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Instance
    {
      get
      {
        return BiffRecordRaw.GetUInt16BitsByMask( m_usVersionAndInst, DEF_INST_MASK )
          >> DEF_INST_START_BIT;
      }
      set
      {
        BiffRecordRaw.SetUInt16BitsByMask( ref m_usVersionAndInst, DEF_INST_MASK,
          ( ushort )( value << DEF_INST_START_BIT ) );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public MsoRecords MsoRecordType
    {
      get
      {
        return ( MsoRecords )m_usRecordType;
      }
      set
      {
        m_usRecordType = ( ushort )value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public GetNextMsoDrawingData DataGetter
    {
      get
      {
        return m_dataGetter;
      }
      set
      {
        m_dataGetter = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public MsoBase Parent
    {
      get
      {
        return m_parent;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Fills record from the data array.
    /// </summary>
    /// <param name="data">Array that contains record data.</param>
    /// <param name="iOffset">Offset to the record data.</param>
    /// <returns>Size of the extracted data.</returns>
    public virtual int FillRecord( byte[] data, int iOffset )
    {
      if( data == null )
        throw new ArgumentNullException( "reader" );

      int iStartOffset = iOffset;

      try
      {
        if( data.Length - iOffset - 8 < 0 )
          throw new ApplicationException( "Unexpected end of record - reached end of the array." );

        m_usVersionAndInst = BitConverter.ToUInt16( data, iOffset );
        iOffset += 2;

        m_usRecordType = BitConverter.ToUInt16( data, iOffset );
        iOffset += 2;

        if( m_usRecordType == 0 )
          throw new ApplicationException( "Mso Record identification code is wrong (zero)." );

        m_iLength = BitConverter.ToInt32( data, iOffset );
        iOffset += 4;

        if( m_iLength < MinimumRecordSize )
          throw new SmallBiffRecordDataException( "Code :" + m_iCode.ToString()
            + "\n Real size: " + m_iLength + ". Expected size: " + MaximumRecordSize.ToString() );

        if( m_iLength > MaximumRecordSize )
          throw new LargeBiffRecordDataException( "Code :" + ((MsoRecords)m_iCode).ToString() + m_iCode
            + "\n Real size: " + m_iLength + ". Expected size: " + MaximumRecordSize.ToString() );

        if( data.Length - iOffset - m_iLength < 0 )
          throw new ApplicationException( "Unexpected end of records stream. Record data cannot " +
            "be read - reached end of stream." );

        m_data = new byte[ m_iLength ];
        Array.Copy( data, iOffset, m_data, 0, m_iLength );

        ParseStructure();

        return (int)( m_iLength + 8 );
      }
      catch( ApplicationException ex )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "Hidden Exception" );

        if( ex.InnerException != null )
        {
          Debug.WriteLine( ex.InnerException.Message
            + Environment.NewLine + ex.InnerException.StackTrace, "Inner exception" );
        }

        // recover Stream Position on exception
        iOffset = iStartOffset;
        throw;
      }
    }
    /// <summary>
    /// Fills internal data array.
    /// </summary>
    public virtual void FillArray( Stream stream )
    {
      FillArray( stream, 0, null, null );
    }
    /// <summary>
    /// Fills internal data array.
    /// </summary>
    /// <param name="stream">Stream to write record data into.</param>
    /// <param name="iOffset">Offset index.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    /// <returns>Returns array of bytes.</returns>
    public virtual void FillArray( Stream stream, int iOffset, List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      byte[] result = new byte[ 8 ];//new byte[ m_iLength + 8 ];

      long lPosition = stream.Position;
      stream.Position += 8;
      InfillInternalData( stream, iOffset, arrBreaks, arrRecords );

      long lLastPosition = stream.Position;
      stream.Position = lPosition;

      int Offset = 0;
      BitConverter.GetBytes( m_usVersionAndInst ).CopyTo( result, Offset );
      Offset += 2;

      BitConverter.GetBytes( m_usRecordType ).CopyTo( result, Offset );
      Offset += 2;

      BitConverter.GetBytes( m_iLength ).CopyTo( result, Offset );
      Offset += 4;

      stream.Write( result, 0, Offset );
      stream.Position = lLastPosition;
    }
    /// <summary>
    /// Returns maximum record size. Read-only.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_MAXIMUM_RECORD_SIZE;
      }
    }
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      MemoryStream stream = new MemoryStream();
      InfillInternalData( stream, 0, null, null );
    }
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    public abstract void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks,
      List<List<BiffRecordRaw>> arrRecords );
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <param name="parent">Parent object to create instance.</param>
    /// <returns>Returns cloned instance.</returns>
    public MsoBase Clone( MsoBase parent )
    {
      MsoBase result = ( MsoBase )InternalClone();
      result.m_parent = parent;
      return result;
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    protected virtual object InternalClone()
    {
      MsoBase result = ( MsoBase )base.Clone();

      if( m_data != null )
      {
        result.m_data = CloneUtils.CloneByteArray( m_data );
      }

      return result;
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
      return InternalClone();
      //throw new NotSupportedException( "Use MsoBase Clone(MsoBase parent) instead of this method" );
    }
    /// <summary>
    /// Updates NextMsoDrawingData.
    /// </summary>
    public virtual void UpdateNextMsoDrawingData()
    {}
    /// <summary>
    /// Extracts record from the stream.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public abstract void ParseStructure( Stream stream );
    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure()
    {
      throw new NotSupportedException( "The method or operation is not supported for MsoRecords." );
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Converts from fixed point integer value into double.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted double value.</returns>
    public static double ConvertFromInt32( int value )
    {
      byte[] buffer = BitConverter.GetBytes( value );

      double result = BitConverter.ToInt16( buffer, 0 );
      ushort fraction = BitConverter.ToUInt16( buffer, 2);
      double curFraction = 0.5;
      ushort curBit = 1 << 15;

      for( int i = 0; i < 16; i++ )
      {
        if( ( curBit & fraction ) != 0 )
        {
          result += curFraction;
        }

        curFraction /= 2;
        curBit >>= 1;
      }

      return result;
    }
    /// <summary>
    /// Writes Int32 value int stream.
    /// </summary>
    /// <param name="stream">Stream to write value into.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteInt32( Stream stream, int value )
    {
      byte[] arrData = BitConverter.GetBytes( value );
      stream.Write( arrData, 0, arrData.Length );
    }
    /// <summary>
    /// Writes UInt32 value int stream.
    /// </summary>
    /// <param name="stream">Stream to write value into.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteUInt32( Stream stream, uint value )
    {
      byte[] arrData = BitConverter.GetBytes( value );
      stream.Write( arrData, 0, arrData.Length );
    }
    /// <summary>
    /// Writes Int16 value int stream.
    /// </summary>
    /// <param name="stream">Stream to write value into.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteInt16( Stream stream, short value )
    {
      byte[] arrData = BitConverter.GetBytes( value );
      stream.Write( arrData, 0, arrData.Length );
    }
    /// <summary>
    /// Writes UInt16 value int stream.
    /// </summary>
    /// <param name="stream">Stream to write value into.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteUInt16( Stream stream, ushort value )
    {
      byte[] arrData = BitConverter.GetBytes( value );
      stream.Write( arrData, 0, arrData.Length );
    }
    /// <summary>
    /// Reads Int32 value from stream.
    /// </summary>
    /// <param name="stream">Stream to read value from.</param>
    /// <returns>Value extracted from the stream.</returns>
    public static int ReadInt32( Stream stream )
    {
      byte[] arrData = new byte[ ExcelConstants.IntSize ];
      stream.Read( arrData, 0, ExcelConstants.IntSize );
      return BitConverter.ToInt32( arrData, 0 );
    }
    /// <summary>
    /// Reads UInt32 value from stream.
    /// </summary>
    /// <param name="stream">Stream to read value from.</param>
    /// <returns>Value extracted from the stream.</returns>
    public static uint ReadUInt32( Stream stream )
    {
      byte[] arrData = new byte[ ExcelConstants.IntSize ];
      stream.Read( arrData, 0, ExcelConstants.IntSize );
      return BitConverter.ToUInt32( arrData, 0 );
    }
    /// <summary>
    /// Reads Int16 value from stream.
    /// </summary>
    /// <param name="stream">Stream to read value from.</param>
    /// <returns>Value extracted from the stream.</returns>
    public static short ReadInt16( Stream stream )
    {
      byte[] arrData = new byte[ ExcelConstants.ShortSize ];
      stream.Read( arrData, 0, ExcelConstants.ShortSize );
      return BitConverter.ToInt16( arrData, 0 );
    }
    /// <summary>
    /// Reads UInt16 value from stream.
    /// </summary>
    /// <param name="stream">Stream to read value from.</param>
    /// <returns>Value extracted from the stream.</returns>
    public static ushort ReadUInt16( Stream stream )
    {
      byte[] arrData = new byte[ ExcelConstants.ShortSize ];
      stream.Read( arrData, 0, ExcelConstants.ShortSize );
      return BitConverter.ToUInt16( arrData, 0 );
    }
    #endregion

    internal void FillRecord( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      try
      {
        if( stream.Length - stream.Position - 8 < 0 )
          throw new ApplicationException( "Unexpected end of record - reached end of the array." );

        m_usVersionAndInst = ReadUInt16( stream );
        m_usRecordType = ReadUInt16( stream );

        //Debug.WriteLine( ( MsoRecords )m_usRecordType );

        if( m_usRecordType == 0 )
          throw new ApplicationException( "Mso Record identification code is wrong (zero)." );

        m_iLength = ReadInt32( stream );

        if( m_iLength < MinimumRecordSize )
          throw new SmallBiffRecordDataException( "Code :" + m_iCode.ToString()
            + "\n Real size: " + m_iLength + ". Expected size: " + MaximumRecordSize.ToString() );

        if( m_iLength > MaximumRecordSize )
          throw new LargeBiffRecordDataException( "Code :" + ( ( MsoRecords )m_iCode ).ToString() + m_iCode
            + "\n Real size: " + m_iLength + ". Expected size: " + MaximumRecordSize.ToString() );

        if( stream.Length - stream.Position - m_iLength < 0 )
          throw new ApplicationException( "Unexpected end of records stream. Record data cannot " +
            "be read - reached end of stream." );

        ParseStructure( stream );

        //return ( int )( m_iLength + 8 );
      }
      catch( ApplicationException ex )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "Hidden Exception" );

        if( ex.InnerException != null )
        {
          Debug.WriteLine( ex.InnerException.Message
            + Environment.NewLine + ex.InnerException.StackTrace, "Inner exception" );
        }

        // recover Stream Position on exception
        throw;
      }
    }
  }
}
