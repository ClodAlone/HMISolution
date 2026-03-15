#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// This interface should be implemented by those tokens which contains 3D references.
  /// </summary>
  [ CLSCompliant( false ) ]
  public interface IReference
  {
    /// <summary>
    /// Index to ExternSheetRecord.
    /// </summary>
    ushort RefIndex { get; set; }
  }
}
