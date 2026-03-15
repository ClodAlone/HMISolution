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

using System;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a collection of custom Add In functions.
  /// </summary>
  public interface IAddInFunctions : IParentApplication
  {
    /// <summary>
    /// 
    /// </summary>
    IAddInFunction this[ int index ] { get; }
    /// <summary>
    /// Returns number of elements in the collection.
    /// </summary>
    int Count { get; }
#if !(WINRT )
    /// <summary>
    /// Adds new function to the collection.
    /// </summary>
    /// <param name="strFileName">Name of the file that contains add-in function.</param>
    /// <param name="strFunctionName">Function to add.</param>
    /// <returns>Index of the added function.</returns>
    int Add( string strFileName, string strFunctionName );
#endif
    /// <summary>
    /// Adds new local function to the collection.
    /// </summary>
    /// <param name="strFunctionName">Function to add.</param>
    /// <returns>Index of the added function.</returns>
    int Add( string strFunctionName );
  }
}
