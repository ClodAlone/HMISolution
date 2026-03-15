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

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// This record specifies linked series data or text.
  /// </summary>
  [ Biff( TBIFFRecord.ChartAI ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartAIRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Link index options.
    /// </summary>
    public enum LinkIndex
    {
      /// <summary>
      /// Represents the LinkToTitleOrText link index option.
      /// </summary>
      LinkToTitleOrText = 0,
      /// <summary>
      /// Represents the LinkToValues link index option.
      /// </summary>
      LinkToValues      = 1,
      /// <summary>
      /// Represents the LinkToCategories link index option.
      /// </summary>
      LinkToCategories  = 2,
      /// <summary>
      /// Represents the LinkToBubbles link index option.
      /// </summary>
      LinkToBubbles     = 3,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ReferenceType
    {
      /// <summary>
      /// Use default categories.
      /// </summary>
      DefaultCategories = 0,
      /// <summary>
      /// Text or value entered directly into the formula bar.
      /// </summary>
      EnteredDirectly = 1,
      /// <summary>
      /// Linked to worksheet.
      /// </summary>
      Worksheet = 2,
      /// <summary>
      /// Not used.
      /// </summary>
      NotUsed = 3,
      /// <summary>
      /// Error reported.
      /// </summary>
      ErrorReported = 4,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Link index identifier.
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte    m_id;
    /// <summary>
    /// Reference type.
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte    m_ReferenceType;
    /// <summary>
    /// Option flags holder.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort  m_usOptions;
    /// <summary>
    /// Index to number format record.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort  m_usNumIndex;
    /// <summary>
    /// Size of parsed formula of link array.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort  m_usFormulaSize;
    /// <summary>
    /// True if this object has a custom number format; False if number format 
    /// is linked to data source.
    /// </summary>
    [ BiffRecordPos( 2, 1, TFieldType.Bit ) ]
    private bool    m_bCustomNumberFormat;
    /// <summary>
    /// 
    /// </summary>
    private Ptg[]   m_arrExpression;
    #endregion

    #region Class properties
    /// <summary>
    /// Link index identifier.
    /// </summary>
    public LinkIndex IndexIdentifier
    {
      get
      {
        return ( LinkIndex )m_id;
      }
      set
      {
        m_id = ( byte )value;
      }
    }

    /// <summary>
    /// Reference type.
    /// </summary>
    public ReferenceType Reference
    {
      get
      {
        return ( ReferenceType )m_ReferenceType;
      }
      set
      {
        m_ReferenceType = ( byte )value;
      }
    }
    /// <summary>
    /// Option flags holder.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Index to number format record.
    /// </summary>
    public ushort NumberFormatIndex
    {
      get
      {
        return m_usNumIndex;
      }
      set
      {
        if( value != m_usNumIndex )
        {
          m_usNumIndex = value;
        }
      }
    }
    /// <summary>
    /// Size of parsed formula of link array.
    /// </summary>
    public ushort FormulaSize
    {
      get
      {
        return m_usFormulaSize;
      }
      set
      {
        if( value != m_usFormulaSize )
        {
          m_usFormulaSize = value;
        }
      }
    }
    /// <summary>
    /// True if this object has a custom number format; False if number format 
    /// is linked to data source.
    /// </summary>
    public bool IsCustomNumberFormat
    {
      get
      {
        return m_bCustomNumberFormat;
      }
      set
      {
        m_bCustomNumberFormat = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Ptg[] ParsedExpression
    {
      get
      {
        return m_arrExpression;
      }
      set
      {
        m_arrExpression = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartAIRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  ChartAIRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartAIRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = 8;

      if( m_arrExpression != null )
      {
        for( int i = 0, len = m_arrExpression.Length; i < len; i++ )
        {
          Ptg token = m_arrExpression[ i ];
          iResult += token.GetSize( version );

          IAdditionalData addData = token as IAdditionalData;

          if( addData != null )
          {
            iResult += addData.AdditionalDataSize;
          }
        }
      }

      return iResult;
    }
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
      // TODO: check correctness of data

      //AutoExtractFields();
      m_id = provider.ReadByte( iOffset );
      iOffset++;

      m_ReferenceType = provider.ReadByte( iOffset );
      iOffset++;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bCustomNumberFormat = provider.ReadBit( iOffset, 1 );
      iOffset += 2;

      m_usNumIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usFormulaSize = provider.ReadUInt16( iOffset );
      iOffset += 2;

      if( m_usFormulaSize > 0 )
      {
//        byte[] arrFormula = GetBytes( 8, m_usFormulaSize );
//        ByteArrayDataProvider provider = new ByteArrayDataProvider( arrFormula );
        int iFinalOffset;
        m_arrExpression = FormulaUtil.ParseExpression( provider, iOffset,
          m_usFormulaSize, out iFinalOffset, version );
      }
      else
      {
        m_arrExpression = null;
      }
    }
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_usOptions &= 0x01;
      m_usFormulaSize = 0;
      byte[] arrFormula = null;

      if( m_arrExpression != null && m_arrExpression.Length > 0 )
      {
        arrFormula = FormulaUtil.PtgArrayToByteArray( m_arrExpression, version );
        m_usFormulaSize = ( ushort )arrFormula.Length;
      }
      
      provider.WriteByte( iOffset, m_id );
      iOffset++;

      provider.WriteByte( iOffset, m_ReferenceType );
      iOffset++;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bCustomNumberFormat, 1 );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usNumIndex );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usFormulaSize );
      iOffset += 2;

      m_iLength = 8;
      if( arrFormula != null )
      {
        provider.WriteBytes( iOffset, arrFormula, 0, m_usFormulaSize );
        m_iLength += m_usFormulaSize;
      }
    }
    #endregion

    #region ICloneable Method
    /// <summary>
    /// Clones current record.
    /// </summary>
    /// <returns>Returns cloned object.</returns>
    public override object Clone()
    {
      ChartAIRecord result = ( ChartAIRecord )base.Clone();

      if( m_arrExpression == null )
        return result;

      int iLen = m_arrExpression.Length;

      result.m_arrExpression = new Ptg[ iLen ];

      for( int i = 0; i < iLen; i++ )
      {
        result.m_arrExpression[ i ] = ( Ptg )CloneUtils.CloneCloneable( m_arrExpression[ i ] );
      }

      return result;
    }
    #endregion
  }
}