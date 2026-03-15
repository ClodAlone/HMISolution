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
  /// Summary description for MsofbtSp.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtSp ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtSp : MsoBase
  {
    #region Class constants
    /// <summary>
    /// Default record version.
    /// </summary>
    private const int DEF_VERSION = 2;
    /// <summary>
    /// Record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 8;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iShapeId;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 4, 4 ) ]
    private uint m_uiOptions;
    /// <summary>
    /// This shape is a group shape.
    /// </summary>
    [ BiffRecordPos( 4, 0, TFieldType.Bit ) ]
    private bool m_bGroup;
    /// <summary>
    /// Not a top-level shape.
    /// </summary>
    [ BiffRecordPos( 4, 1, TFieldType.Bit ) ]
    private bool m_bChild;
    /// <summary>
    /// This is the topmost group shape. Exactly one of these per drawing.
    /// </summary>
    [ BiffRecordPos( 4, 2, TFieldType.Bit ) ]
    private bool m_bPatriarch;
    /// <summary>
    /// The shape has been deleted.
    /// </summary>
    [ BiffRecordPos( 4, 3, TFieldType.Bit ) ]
    private bool m_bDeleted;
    /// <summary>
    /// The shape is an OLE object.
    /// </summary>
    [ BiffRecordPos( 4, 4, TFieldType.Bit ) ]
    private bool m_bOleShape;
    /// <summary>
    /// Shape has a hspMaster property.
    /// </summary>
    [ BiffRecordPos( 4, 5, TFieldType.Bit ) ]
    private bool m_bHaveMaster;
    /// <summary>
    /// Shape is flipped horizontally.
    /// </summary>
    [ BiffRecordPos( 4, 6, TFieldType.Bit ) ]
    private bool m_bFlipH;
    /// <summary>
    /// Shape is flipped vertically.
    /// </summary>
    [ BiffRecordPos( 4, 7, TFieldType.Bit ) ]
    private bool m_bFlipV;
    /// <summary>
    /// Connector type of shape.
    /// </summary>
    [ BiffRecordPos( 5, 0, TFieldType.Bit ) ]
    private bool m_bConnector;
    /// <summary>
    /// Shape has an anchor of some kind.
    /// </summary>
    [ BiffRecordPos( 5, 1, TFieldType.Bit ) ]
    private bool m_bHaveAnchor;
    /// <summary>
    /// Background shape.
    /// </summary>
    [ BiffRecordPos( 5, 2, TFieldType.Bit ) ]
    private bool m_bBackground;
    /// <summary>
    /// Shape has a shape type property.
    /// </summary>
    [ BiffRecordPos( 5, 3, TFieldType.Bit ) ]
    private bool m_bHaveSpt;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int ShapeId
    {
      get
      {
        return m_iShapeId;
      }
      set
      {
        m_iShapeId = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public uint Options
    {
      get
      {
        return m_uiOptions;
      }
#if DEBUG
      set
      {
        m_uiOptions = value;
      }
#endif
    }
    /// <summary>
    /// This shape is a group shape.
    /// </summary>
    public bool IsGroup
    {
      get
      {
        return m_bGroup;
      }
      set
      {
        m_bGroup = value;
      }
    }
    /// <summary>
    /// Not a top-level shape.
    /// </summary>
    public bool IsChild
    {
      get
      {
        return m_bChild;
      }
      set
      {
        m_bChild = value;
      }
    }
    /// <summary>
    /// This is the topmost group shape. Exactly one of these per drawing.
    /// </summary>
    public bool IsPatriarch
    {
      get
      {
        return m_bPatriarch;
      }
      set
      {
        m_bPatriarch = value;
      }
    }
    /// <summary>
    /// The shape has been deleted.
    /// </summary>
    public bool IsDeleted
    {
      get
      {
        return m_bDeleted;
      }
      set
      {
        m_bDeleted = value;
      }
    }
    /// <summary>
    /// The shape is an OLE object.
    /// </summary>
    public bool IsOleShape
    {
      get
      {
        return m_bOleShape;
      }
      set
      {
        m_bOleShape = value;
      }
    }
    /// <summary>
    /// Shape has a hspMaster property.
    /// </summary>
    public bool IsHaveMaster
    {
      get
      {
        return m_bHaveMaster;
      }
      set
      {
        m_bHaveMaster = value;
      }
    }
    /// <summary>
    /// Shape is flipped horizontally.
    /// </summary>
    public bool IsFlipH
    {
      get
      {
        return m_bFlipH;
      }
      set
      {
        m_bFlipH = value;
      }
    }
    /// <summary>
    /// Shape is flipped vertically.
    /// </summary>
    public bool IsFlipV
    {
      get
      {
        return m_bFlipV;
      }
      set
      {
        m_bFlipV = value;
      }
    }
    /// <summary>
    /// Connector type of shape.
    /// </summary>
    public bool IsConnector
    {
      get
      {
        return m_bConnector;
      }
      set
      {
        m_bConnector = value;
      }
    }
    /// <summary>
    /// Shape has an anchor of some kind.
    /// </summary>
    public bool IsHaveAnchor
    {
      get
      {
        return m_bHaveAnchor;
      }
      set
      {
        m_bHaveAnchor = value;
      }
    }
    /// <summary>
    /// Background shape.
    /// </summary>
    public bool IsBackground
    {
      get
      {
        return m_bBackground;
      }
      set
      {
        m_bBackground = value;
      }
    }
    /// <summary>
    /// Shape has a shape type property.
    /// </summary>
    public bool IsHaveSpt
    {
      get
      {
        return m_bHaveSpt;
      }
      set
      {
        m_bHaveSpt = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtSp( MsoBase parent )
      : base( parent )
    {
      Version = DEF_VERSION;
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtSp( MsoBase parent, byte[] data, int iOffset )
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
      WriteInt32( stream, m_iShapeId );

      SetBitInVar( ref m_uiOptions, m_bGroup, 0 );
      SetBitInVar( ref m_uiOptions, m_bChild, 1 );
      SetBitInVar( ref m_uiOptions, m_bPatriarch, 2 );
      SetBitInVar( ref m_uiOptions, m_bDeleted, 3 );
      SetBitInVar( ref m_uiOptions, m_bOleShape, 4 );
      SetBitInVar( ref m_uiOptions, m_bHaveMaster, 5 );
      SetBitInVar( ref m_uiOptions, m_bFlipH, 6 );
      SetBitInVar( ref m_uiOptions, m_bFlipV, 7 );
      SetBitInVar( ref m_uiOptions, m_bConnector, 8 );
      SetBitInVar( ref m_uiOptions, m_bHaveAnchor, 9 );
      SetBitInVar( ref m_uiOptions, m_bBackground, 10 );
      SetBitInVar( ref m_uiOptions, m_bHaveSpt, 11 );

      WriteUInt32( stream, m_uiOptions );

      m_iLength = DEF_RECORD_SIZE;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      m_iShapeId = ReadInt32( stream );

      m_uiOptions = ReadUInt32( stream );
      m_bGroup = GetBitFromVar( m_uiOptions, 0 );
      m_bChild = GetBitFromVar( m_uiOptions, 1 );
      m_bPatriarch = GetBitFromVar( m_uiOptions, 2 );
      m_bDeleted = GetBitFromVar( m_uiOptions, 3 );
      m_bOleShape = GetBitFromVar( m_uiOptions, 4 );
      m_bHaveMaster = GetBitFromVar( m_uiOptions, 5 );
      m_bFlipH = GetBitFromVar( m_uiOptions, 6 );
      m_bFlipV = GetBitFromVar( m_uiOptions, 7 );

      m_bConnector = GetBitFromVar( m_uiOptions, 8 );
      m_bHaveAnchor = GetBitFromVar( m_uiOptions, 9 );
      m_bBackground = GetBitFromVar( m_uiOptions, 10 );
      m_bHaveSpt = GetBitFromVar( m_uiOptions, 11 );
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
