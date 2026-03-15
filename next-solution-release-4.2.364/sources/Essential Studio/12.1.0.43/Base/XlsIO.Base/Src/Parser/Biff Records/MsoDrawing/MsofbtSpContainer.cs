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
using Syncfusion.XlsIO.Implementation;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsofbtSpgrContainer.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtSpContainer ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtSpContainer : MsoContainerBase
  {
    #region Class constants
    /// <summary>
    /// Default version of container.
    /// </summary>
    private const int DEF_VERSION = 15;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    public MsofbtSpContainer( MsoBase parent )
      : base( parent )
    {
      //
      // TODO: Add constructor logic here
      //
      Version = DEF_VERSION;
    }
    /// <summary>
    /// Creates new object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Base data.</param>
    /// <param name="iOffset">Offset index.</param>
    public MsofbtSpContainer( MsoBase parent, byte[] data, int iOffset )
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
    public MsofbtSpContainer( MsoBase parent, byte[] data, int iOffset
      , GetNextMsoDrawingData dataGetter )
      : base( parent, data, iOffset, dataGetter )
  {
  }
    #endregion
  }
}
