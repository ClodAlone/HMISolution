#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
    /// <summary>
    /// this class parse and serialize check box Linked cell
    /// </summary>
  class ftCblsFmla :
    ObjSubRecord,
    IFormulaRecord
  {

    #region Members
    /// <summary>
    /// Formula tokens.
    /// </summary>
    private Ptg[] m_formula;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ftCblsFmla()
        : base(TObjSubRecordType.ftCblsFmla)
    {
    }
    /// <summary>
    /// Initialize new instance.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Array that contains subrecord's data.</param>
    public ftCblsFmla(TObjSubRecordType type, ushort length, byte[] buffer)
      : base( type, length, buffer )
    {
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets/sets parsed formula tokens.
    /// </summary>
    public Ptg[] Formula
    {
      get
      {
        return m_formula;
      }
      set
      {
        m_formula = value;
      }
    }
    #endregion

    #region Methods
    protected override void Parse( byte[] buffer )
    {
      int iOffset = 0;
      int iTokensSize = BitConverter.ToInt16( buffer, iOffset );
      iOffset += ExcelConstants.ShortSize;

      int iReserved = BitConverter.ToInt32( buffer, iOffset );
      iOffset += ExcelConstants.IntSize;

      ByteArrayDataProvider provider = new ByteArrayDataProvider( buffer );
      int iFinatOffset;
      m_formula = FormulaUtil.ParseExpression( provider, iOffset, iTokensSize,
        out iFinatOffset, ExcelVersion.Excel97to2003 );
    }

    protected override void Serialize( DataProvider provider, int iOffset )
    {
      byte[] arrFormulaData = FormulaUtil.PtgArrayToByteArray( m_formula, ExcelVersion.Excel97to2003 );
      int iFormulaSize = arrFormulaData.Length;
      provider.WriteInt16( iOffset, ( short )iFormulaSize );
      iOffset += ExcelConstants.ShortSize;

      provider.WriteInt32( iOffset, 0 ); // reserved value
      iOffset += ExcelConstants.IntSize;

      provider.WriteBytes( iOffset, arrFormulaData );
      iOffset += iFormulaSize;

      provider.WriteByte( iOffset, 0 );
    }
    public override int GetStoreSize( ExcelVersion version )
    {
      return DVRecord.GetFormulaSize( m_formula, version, true ) +
        ExcelConstants.IntSize + ExcelConstants.ShortSize + HeaderSize + 1;
    }
    #endregion
  }
}
