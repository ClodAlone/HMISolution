#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
  public class ftMacro : ObjSubRecord
  {
    #region Members
    /// <summary>
    /// Formula tokens that identifies associated macro.
    /// </summary>
    private Ptg[] m_arrTokens;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets formula tokens with associated macro.
    /// </summary>
    public Ptg[] Tokens
    {
      get
      {
        return m_arrTokens;
      }
      set
      {
        m_arrTokens = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ftMacro()
      : base( TObjSubRecordType.ftMacro )
    {
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ftMacro( ushort length, byte[] buffer )
      : base( TObjSubRecordType.ftMacro, length, buffer )
    {
    }

    protected override void Parse( byte[] buffer )
    {
      int iOffset = 0;
      int iFinalOffset;
      int iRecordDataSize = BitConverter.ToUInt16( buffer, iOffset );
      iOffset += ExcelConstants.ShortSize;

      iOffset += ExcelConstants.IntSize; // reserved value;

      ByteArrayDataProvider provider = new ByteArrayDataProvider( buffer );
      m_arrTokens = FormulaUtil.ParseExpression( provider, iOffset, iRecordDataSize,
        out iFinalOffset, ExcelVersion.Excel97to2003 );
    }

    protected override void Serialize( DataProvider provider, int iOffset )
    {
      byte[] tokens = FormulaUtil.PtgArrayToByteArray( m_arrTokens, ExcelVersion.Excel97to2003 );
      int iFormulaSize = tokens.Length;

      provider.WriteUInt16( iOffset, ( ushort )iFormulaSize );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt32( iOffset, 0 );// reserved value
      iOffset += ExcelConstants.IntSize;

      provider.WriteBytes( iOffset, tokens );
      //base.Serialize( provider, iOffset );
    }

    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = DVRecord.GetFormulaSize( m_arrTokens, version, true ) + HeaderSize +
        ExcelConstants.ShortSize + ExcelConstants.IntSize;

      if( iResult % 2 != 0 )
        iResult++;

      return iResult;
    }
    /// <summary>
    /// Clones current objects.
    /// </summary>
    /// <returns>Returns instance of cloned object.</returns>
    public override object Clone()
    {
      ftMacro result = ( ftMacro )base.Clone();
      result.m_arrTokens = CloneUtils.ClonePtgArray( m_arrTokens );
      return result;
    }
    #endregion
  }
}
