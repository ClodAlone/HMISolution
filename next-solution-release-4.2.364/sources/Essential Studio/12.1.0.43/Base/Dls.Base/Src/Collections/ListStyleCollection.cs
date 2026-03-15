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

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Represents list style collection
  /// </summary>
  public class ListStyleCollection
    : StyleCollection
  {
    #region Class properties
    /// <summary>
    /// Get Style by index
    /// </summary>
    new public ListStyle this[ int index ]
    {
      get
      {
        return ( ListStyle )List[ index ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    protected internal ListStyleCollection( IDocument doc )
      : base( doc )
    {}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="doc"></param>
    protected internal ListStyleCollection( ListStyleCollection collection, IDocument doc )
      : base ( doc )
    {
      foreach( ListStyle listStyle in this )
      {
        collection.Add( listStyle.Clone( doc ));
      }
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds Style to collection 
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    public int Add( ListStyle style )
    {
      return List.Add( style );
    }
    /// <summary>
    /// Finds Style by name 
    /// </summary>
    /// <param name="name"></param>
    new public ListStyle FindByName( string name )
    {
      ListStyle style = base.FindByName( name ) as ListStyle;
      return style;
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object CreateItem( IXDLSContentReader reader )
    {
      return DocumentEx.CreateListStyleImpl();
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.StyleItemTag;
      }
    }
    #endregion
  }
}