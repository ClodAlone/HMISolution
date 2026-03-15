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
using CharacterProperties = Syncfusion.DLS.CharacterFormat;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Interface publishes base style functionality
  /// </summary>
  public interface IStyle
  {
    /// <summary>
    /// Gets style name.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Clones itself.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    IStyle Clone( IDocument document );
  }
}