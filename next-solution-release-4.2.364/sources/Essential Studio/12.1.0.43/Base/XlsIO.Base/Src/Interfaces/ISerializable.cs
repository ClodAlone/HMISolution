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

using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// Represents objects that can be saved into list of biff records.
  /// </summary>
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public interface ISerializable
  {
    /// <summary>
    /// Saves object into list of biff records.
    /// </summary>
    /// <param name="records">List of biff records to save object into.</param>
    void Serialize( IList<IBiffStorage> records );
  }
}
