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
#endregion

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// Supports parsing, which parses internal records and creates necessary objects.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  interface IParseable
  {
    /// <summary>
    /// Parses internal records.
    /// </summary>
    void Parse();
  }
}
