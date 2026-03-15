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
  /// Represents list levels collection.
  /// </summary>
  public class ListLevelCollection
    : EntityCollectionBase
  {
    #region Class properties
    /// <summary>
    /// Gets List level by index.
    /// </summary>
    public ListLevel this[ int index ]
    {
      get
      {
        return ( ListLevel )List[ index ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    internal ListLevelCollection( ListStyle owner  )
      : base( owner )
    {}
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds List level to collection. 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    internal int Add( ListLevel level )
    { 
      return List.Add( level );
    }
    /// <summary>
    /// Clone ListLevelCollection
    /// </summary>
    /// <returns></returns>
    internal ListLevelCollection Clone( ListStyle owner )
    {
      ListLevelCollection cloneLevelCollection = new ListLevelCollection( owner );
      foreach( ListLevel listLevel in this )
      {
        cloneLevelCollection.Add( listLevel.Clone( owner ));        
      }
      return cloneLevelCollection;
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
      return DocumentEx.CreateListLevelImpl( Owner as ListStyle );
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.ListLevelItemTag;
      }
    }
    #endregion
  }
}