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
  /// Summary description for MsofbtDg.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtDg ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtDg : MsoBase
  {
    #region Class constants
    /// <summary>
    /// Default instance.
    /// </summary>
    private const int DEF_INSTANCE = 1;
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 8;
    #endregion

    #region Class members
    /// <summary>
    /// The number of shapes in this drawing.
    /// </summary>
    [ BiffRecordPos( 0, 4 ) ]
    private uint m_uiShapesNumber;
    /// <summary>
    /// The last MSOSPID given to an SP in this Drawing Group.
    /// </summary>
    [ BiffRecordPos( 4, 4 ) ]
    private int m_iLastId;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtDg( MsoBase parent )
      : base( parent )
    {
      Instance = DEF_INSTANCE;
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtDg( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// The number of shapes in this drawing.
    /// </summary>
    public uint ShapesNumber
    {
      get
      {
        return m_uiShapesNumber;
      }
      set
      {
        m_uiShapesNumber = value;
      }
    }
    /// <summary>
    /// The last MSOSPID given to an SP in this Drawing Group.
    /// </summary>
    public int LastId
    {
      get
      {
        return m_iLastId;
      }
      set
      {
        m_iLastId = value;
      }
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
      WriteUInt32( stream, m_uiShapesNumber );
      WriteInt32( stream, m_iLastId );
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      m_uiShapesNumber = ReadUInt32( stream );
      m_iLastId = ReadInt32( stream );
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
