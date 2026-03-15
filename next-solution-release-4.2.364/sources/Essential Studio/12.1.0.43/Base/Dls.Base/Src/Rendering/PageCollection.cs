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

using System.Collections;

namespace Syncfusion.DLS.Rendering
{
  /// <summary>
  /// Summary description for PageCollection.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class PageCollection : CollectionBase
  {
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public Page this[ int index ]
    {
      get
      {
        return List[ index ] as Page;
      }
    }
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public PageCollection()
    {
    }
    #endregion
    
    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public int Add( Page page )
    {
      return List.Add( page );
    }
    #endregion
  }
}