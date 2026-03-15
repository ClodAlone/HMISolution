#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
  /// <summary>
  /// End of OBJ record.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [CLSCompliant( false )]
  public class ftEnd : ObjSubRecord
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 4;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ftEnd()
      : base( TObjSubRecordType.ftEnd )
    {
    }

    /// <summary>
    /// Creates new instance of the subrecord.
    /// </summary>
    /// <param name="type">Type of the subrecord.</param>
    /// <param name="length">Length of the subrecord's data.</param>
    /// <param name="buffer">Array that contains subrecord's data.</param>
    public ftEnd( TObjSubRecordType type, ushort length, byte[] buffer )
      : base( type, length, buffer )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Parses array of bytes.
    /// </summary>
    /// <param name="buffer">Array to parse.</param>
    protected override void Parse( byte[] buffer )
    {
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
