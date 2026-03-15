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
using System.Collections;

using Syncfusion.DLS.XML;

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Represents style collection
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class StyleCollection
    : EntityCollectionBase,
      IStyleCollection
  {
    #region Class properties
    /// <summary>
    /// Get Style by index
    /// </summary>
    public IStyle this[ int index ]
    {
      get
      {
        return ( IStyle )List[ index ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    protected internal StyleCollection( IDocument doc )
      : base( doc )
    {}
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds Style to collection 
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    public int Add( IStyle style )
    {
      return List.Add( style );
    }
    /// <summary>
    /// Finds Style by name 
    /// </summary>
    /// <param name="name"></param>
    public IStyle FindByName( string name )
    {
      IStyle style = null;

      for( int i = 0; i < Count; i++ )
      {
        if( this[ i ].Name == name )
        {
          style = this[ i ];
          break;
        }
      }
      
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
      StyleType styleType = ( StyleType )GetItemType( reader, typeof( StyleType ), 
                                                StyleType.ParagraphStyle );

      return DocumentEx.CreateStyleImpl( styleType );
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