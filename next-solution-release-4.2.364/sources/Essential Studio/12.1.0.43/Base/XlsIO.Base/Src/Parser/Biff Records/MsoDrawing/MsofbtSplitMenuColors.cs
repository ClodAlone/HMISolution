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
  /// Summary description for MsofbtSplitMenuColors.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtSplitMenuColors ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtSplitMenuColors : MsoBase
  {
    #region Constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int RecordSize = 16;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iFillColor;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iLineColor;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iShadowColor;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private int m_i3DColor;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int FillColor
    {
      get
      {
        return m_iFillColor;
      }
      set
      {
        m_iFillColor = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int LineColor
    {
      get
      {
        return m_iLineColor;
      }
      set
      {
        m_iLineColor = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int ShadowColor
    {
      get
      {
        return m_iShadowColor;
      }
      set
      {
        m_iShadowColor = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Color3D
    {
      get
      {
        return m_i3DColor;
      }
      set
      {
        m_i3DColor = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtSplitMenuColors( MsoBase parent )
      : base( parent )
    {
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtSplitMenuColors( MsoBase parent, byte[] data, int iOffset )
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
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks,
      List<List<BiffRecordRaw>> arrRecords )
    {
      WriteInt32( stream, m_iFillColor );
      WriteInt32( stream, m_iLineColor );
      WriteInt32( stream, m_iShadowColor );
      WriteInt32( stream, m_i3DColor );
      m_iLength = RecordSize;
    }
    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      //AutoExtractFields();
      m_iFillColor = ReadInt32( stream );
      m_iLineColor = ReadInt32( stream );
      m_iShadowColor = ReadInt32( stream );
      m_i3DColor = ReadInt32( stream );
    }
    #endregion
  }
}
