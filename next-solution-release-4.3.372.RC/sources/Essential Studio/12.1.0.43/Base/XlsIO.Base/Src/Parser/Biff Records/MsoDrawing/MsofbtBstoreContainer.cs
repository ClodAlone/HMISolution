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
  /// Summary description for MsofbtBstoreContainer.
  /// </summary>
  [ MsoDrawing( MsoRecords.msofbtBstoreContainer ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MsofbtBstoreContainer : MsoContainerBase
  {
    #region Class constants
    /// <summary>
    /// Default version of container.
    /// </summary>
    private const int DEF_VERSION = 15;
    /// <summary>
    /// Default instance of container.
    /// </summary>
    private const int DEF_INSTANCE = 1;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    public MsofbtBstoreContainer( MsoBase parent )
      : base( parent )
    {
      Version = DEF_VERSION;
      Instance = DEF_INSTANCE;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="data"></param>
    /// <param name="iOffset"></param>
    public MsofbtBstoreContainer( MsoBase parent, byte[] data, int iOffset )
      : base( parent, data, iOffset )
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="data"></param>
    /// <param name="iOffset"></param>
    /// <param name="dataGetter"></param>
    public MsofbtBstoreContainer( MsoBase parent, byte[] data, int iOffset
      , GetNextMsoDrawingData dataGetter )
      : base( parent, data, iOffset, dataGetter )
  {
  }
    #endregion

    #region Class overrides

    protected override void OnDispose()
    {
        if ((this as MsoContainerBase).Items.Length > 0)
        {
            int loopCount = (this as MsoContainerBase).Items.Length;
            for (int i = 0; i < loopCount; i++)
            {
                (this as MsoContainerBase).Items[i].Dispose();
            }
        }
        base.OnDispose();
    }
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
      Instance = Items.Length;
      base.InfillInternalData( stream, iOffset, arrBreaks, arrRecords );
    }

    #endregion
  }
}
