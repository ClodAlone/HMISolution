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
  /// This interface should be implemented by all token classes that
  /// needs some additional data to be read after all formula tokens.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public interface IAdditionalData
  {
    /// <summary>
    /// Reads additional token data that is placed after all formula tokens.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the additional token data.</param>
    /// <returns>Final offset in the data array.</returns>
    int ReadArray( DataProvider provider, int offset );
    
    /// <summary>
    /// Returns size of the additional data. Read-only.
    /// </summary>
    int AdditionalDataSize { get; }
  }
}
