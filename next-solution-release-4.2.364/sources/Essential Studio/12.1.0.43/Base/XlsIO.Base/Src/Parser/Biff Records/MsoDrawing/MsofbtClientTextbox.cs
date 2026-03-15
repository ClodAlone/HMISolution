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
using System.Collections;

using Syncfusion.XlsIO.Implementation;
using System.IO;
using System.Collections.Generic;
//using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsofbtSpgr.
  /// </summary>
  [MsoDrawing( MsoRecords.msofbtClientTextbox )]
  [Syncfusion.Documentation.DocumentationExclude()]
  [CLSCompliant( false )]
  public class MsofbtClientTextBox : MsoBase
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private List<BiffRecordRaw> m_arrAdditionalData = new List<BiffRecordRaw>( 3 );
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public TextObjectRecord TextObject
    {
      get
      {
        return ( m_arrAdditionalData.Count > 0 )
          ? m_arrAdditionalData[ 0 ] as TextObjectRecord
          : null;
      }
      set
      {
        //m_ObjRecord = value;
        //m_arrAdditionalData[ 0 ] = value;
        m_arrAdditionalData.Insert( 0, value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string Text
    {
      get
      {
        TextObjectRecord textObject = TextObject;
        int iTextSize = ( textObject != null ) ? ( int )textObject.TextLen : 0;

        int iCount = m_arrAdditionalData.Count;

        if( iTextSize == 0 || iCount <= 1 )
          return null;

        int iPreparedTextSize = 0;
        int iRecordIndex = 1;

        while( iPreparedTextSize < iTextSize )
        {
          ContinueRecord continueRecord = m_arrAdditionalData[ iRecordIndex ] as ContinueRecord;
          byte[] arrData = continueRecord.Data;
          bool bUnicode = arrData[ 0 ] != 0;
          int iRecordLen = continueRecord.Length - 1;

          iPreparedTextSize += bUnicode ?
            iRecordLen / 2 :
            iRecordLen;

          iRecordIndex++;
        }

        return CombineAndExtractText( 1, iRecordIndex );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="afterEndIndex"></param>
    /// <returns></returns>
    private string CombineAndExtractText( int startIndex, int afterEndIndex )
    {
      string result = string.Empty;

      for( int i = startIndex; i < afterEndIndex; i++ )
      {
        ContinueRecord record = ( ContinueRecord )m_arrAdditionalData[ i ];
        byte[] arrData = record.Data;
        int iLength = record.Length;
        bool bUnicode = arrData[ 0 ] != 0;
        int iTextLength = bUnicode ? ( iLength - 1 ) / 2 : iLength - 1;
        result += record.GetString( 0, iTextLength );
      }

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    public byte[] FormattingRuns
    {
      get
      {
        TextObjectRecord textObject = TextObject;
        byte[] arrRuns = null;

        if( textObject != null )
        {
          int iTotal = TextObject.FormattingRunsLen;
          int iCurrent = 0;
          int iCount = m_arrAdditionalData.Count;
          int iStartIndex = iCount - 1;

          for( ; iCurrent < iTotal; iStartIndex-- )
          {
            BiffRecordRaw record = ( BiffRecordRaw )m_arrAdditionalData[ iStartIndex ];
            iCurrent += record.Length;
          }

          arrRuns = ( iTotal > 0 ) ? new byte[ iTotal ] : null;
          for( int i = iStartIndex + 1, iOffset = 0; i < iCount; i++ )
          {
            BiffRecordRaw record = ( BiffRecordRaw )m_arrAdditionalData[ i ];
            int iLength = record.Length;
            Buffer.BlockCopy( record.Data, 0, arrRuns, iOffset, iLength );
            iOffset += iLength;
          }
        }

        return arrRuns;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public BiffRecordRaw[] AdditionalData
    {
      get
      {
        return ( m_arrAdditionalData != null ) ?
          m_arrAdditionalData.ToArray() :
          null;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtClientTextBox( MsoBase parent )
      : base( parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtClientTextBox( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    /// <param name="dataGetter">Data getter.</param>
    public MsofbtClientTextBox( MsoBase parent, byte[] data, int iOffset
      , GetNextMsoDrawingData dataGetter )
      : base( parent, data, iOffset, dataGetter )
    {
      BiffRecordRaw[] arrAdditionalData = dataGetter();

      if( arrAdditionalData == null )//|| arrAdditionalData.Length != 1 )
        throw new ArgumentException( "Additional data can't be null"
          //+ " and must contains only one element" 
          );

      m_arrAdditionalData.Clear();
      m_arrAdditionalData.AddRange( arrAdditionalData );
      //m_ObjRecord = ( OBJRecord ) arrAdditionalData[ 0 ];
    }

    #endregion

    #region Class overrides
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      m_iLength = 0;

      if( arrBreaks != null && arrRecords != null )
      {
        arrBreaks.Add( m_iLength + iOffset );
        arrRecords.Add( m_arrAdditionalData );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns cloned instance.</returns>
    protected override object InternalClone()
    {
      MsofbtClientTextBox instance = ( MsofbtClientTextBox )base.InternalClone();
      instance.m_arrAdditionalData = CloneUtils.CloneCloneable( m_arrAdditionalData );

      return instance;
    }
    /// <summary>
    /// Updates NextMsoDrawingData.
    /// </summary>
    public override void UpdateNextMsoDrawingData()
    {
      BiffRecordRaw[] arrAdditionalData = DataGetter();

      if( arrAdditionalData == null )//|| arrAdditionalData.Length != 1 )
        throw new ArgumentException( "Additional data can't be null"
          //+ " and must contains only one element" 
          );

      m_arrAdditionalData.Clear();
      m_arrAdditionalData.AddRange( arrAdditionalData );
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="record"></param>
    public void AddRecord( BiffRecordRaw record )
    {
      m_arrAdditionalData.Add( record );
    }
    #endregion

  }
}
