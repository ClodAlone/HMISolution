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
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
#endregion

namespace Syncfusion.ExcelRW.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsofbtColorMRURecord.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtColorMRU ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtColorMRU : MsoBase
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    public MsofbtColorMRU( MsoBase parent )
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
    public MsofbtColorMRU( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }
    #endregion

    #region Class properties
    #endregion

    #region Class overrides
    /// <summary>
    /// Infills internal data array.
    /// </summary>
    /// <param name="iOffset">Offset.</param>
    /// <param name="arrBreaks">ArrayList with breaks indexes in arrRecords.</param>
    /// <param name="arrRecords">ArrayList with records.</param>
    public override void InfillInternalData( int iOffset, ArrayList arrBreaks, ArrayList arrRecords )
    {
      //m_iLength = AutoInfillFromFields();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure()
    {
      //AutoExtractFields();
    }
    #endregion
  }
}
