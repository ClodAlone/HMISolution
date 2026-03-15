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
using System.IO;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsofbtSpgr.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtRegroupItems ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtRegroupItems : MsoBase
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private byte[] m_arrData;
    #endregion

    #region Class properties
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtRegroupItems( MsoBase parent )
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
    public MsofbtRegroupItems( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</paramList<List<BiffRecordRaw>>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks,
      List<List<BiffRecordRaw>> arrRecords )
    {
      m_iLength = ( m_arrData != null ) ?
        m_arrData.Length :
        0;

      if( m_iLength > 0 )
        stream.Write( m_arrData, 0, m_iLength );
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      if( m_iLength > 0 )
      {
        m_arrData = new byte[ m_iLength ];
        stream.Read( m_arrData, 0, m_iLength );
      }
    }
    #endregion
  }
}
