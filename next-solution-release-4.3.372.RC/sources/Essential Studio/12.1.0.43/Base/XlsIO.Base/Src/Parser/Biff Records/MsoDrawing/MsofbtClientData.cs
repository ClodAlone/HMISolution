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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsofbtSpgr.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtClientData ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtClientData : MsoBase
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private List<BiffRecordRaw> m_arrAdditionalData = new List<BiffRecordRaw>();
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public OBJRecord ObjectRecord
    {
      get
      {
        if( m_arrAdditionalData.Count > 0 && m_arrAdditionalData[ 0 ] is OBJRecord )
          return ( OBJRecord )m_arrAdditionalData[ 0 ];

        return null;
      }
      set
      {
        throw new NotImplementedException();
        //m_ObjRecord = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public BiffRecordRaw[] AdditionalData
    {
      get
      {
        return ( m_arrAdditionalData != null )
          ? m_arrAdditionalData.ToArray()
          : null;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtClientData( MsoBase parent )
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
    public MsofbtClientData( MsoBase parent, byte[] data, int iOffset )
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
    public MsofbtClientData( MsoBase parent, byte[] data, int iOffset
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

      for( int i = m_arrAdditionalData.Count - 1; i >= 0; i-- )
      {
        if( m_arrAdditionalData[ i ] is NoteRecord )
        {
          m_arrAdditionalData.RemoveAt( i );
        }
        else
        {
          break;
        }
      }
      //m_arrAdditionalData.AddRange( arrAdditionalData );
      //m_arrAdditionalData.Add( arrAdditionalData[ 0 ] );
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
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks,
      List<List<BiffRecordRaw>> arrRecords )
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
      MsofbtClientData instance = ( MsofbtClientData )base.InternalClone();
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

      for( int i = m_arrAdditionalData.Count - 1; i >= 0; i-- )
      {
        if( m_arrAdditionalData[ i ] is NoteRecord )
        {
          m_arrAdditionalData.RemoveAt( i );
        }
        else
        {
          break;
        }
      }
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Adds single record to the AdditionalData array.
    /// </summary>
    /// <param name="record">Record to add.</param>
    public void AddRecord( BiffRecordRaw record )
    {
      m_arrAdditionalData.Add( record );
    }

    /// <summary>
    /// Adds range of records to the AdditionalData array.
    /// </summary>
    /// <param name="records">Collection to add.</param>
    public void AddRecordRange( ICollection<BiffRecordRaw> records )
    {
      m_arrAdditionalData.AddRange( records );
    }
    /// <summary>
    /// Adds range of records to the AdditionalData array.
    /// </summary>
    /// <param name="records">Collection to add.</param>
    public void AddRecordRange( IList records )
    {
      for( int i = 0, len = records.Count; i < len; i++ )
      {
        m_arrAdditionalData.Add( records[ i ] as BiffRecordRaw );
      }
    }
    #endregion

  }
}
