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
  /// Summary description for MsoUnknown.
  /// </summary>
  [ MsoDrawing( MsoRecords.msoUnknown ) ]
    [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsoUnknown : MsoBase
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsoUnknown( MsoBase parent )
      : base( parent )
    {
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsoUnknown( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure( Stream stream )
    {
      if( m_iLength > 0 )
      {
        m_data = new byte[ m_iLength ];
        stream.Read( m_data, 0, m_iLength );
      }
    }

    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">List with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">List with records.</param>
    public override void InfillInternalData( Stream stream, int iOffset, List<int> arrBreaks, List<List<BiffRecordRaw>> arrRecords )
    {
      if( m_iLength > 0 )
        stream.Write( m_data, 0, m_iLength );
    }
    /// <summary>
    /// Indicates whether record needs internal data array or if it can be cleaned.
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    #endregion
  }
}
