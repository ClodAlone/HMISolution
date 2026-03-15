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
  [ MsoDrawing( MsoRecords.msofbtAnchor ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtAnchor : MsoBase
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 16;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iLeft;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iTop;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iRight;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private int m_iBottom;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int Left
    {
      get
      {
        return m_iLeft;
      }
      set
      {
        m_iLeft = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Top
    {
      get
      {
        return m_iTop;
      }
      set
      {
        m_iTop = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Right
    {
      get
      {
        return m_iRight;
      }
      set
      {
        m_iRight = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Bottom
    {
      get
      {
        return m_iBottom;
      }
      set
      {
        m_iBottom = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    public MsofbtAnchor( MsoBase parent )
      : base( parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtAnchor( MsoBase parent, byte[] data, int iOffset )
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
    /// <param name="arrRecords">List with records.</param>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      m_iLength = DEF_RECORD_SIZE;
      WriteInt32( stream, m_iLeft );
      WriteInt32( stream, m_iTop );
      WriteInt32( stream, m_iRight );
      WriteInt32( stream, m_iBottom );
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      m_iLeft = ReadInt32( stream );
      m_iTop = ReadInt32( stream );
      m_iRight = ReadInt32( stream );
      m_iBottom = ReadInt32( stream );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion
  }
}