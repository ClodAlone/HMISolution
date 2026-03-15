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
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS.Collections
{
  /// <summary>
  /// Represents a column collection.
  /// </summary>
  public class ColumnCollection : EntityCollectionBase
  {
    #region Class properties
    /// <summary>
    /// Gets column by index.
    /// </summary>
    public Column this[ int index ]
    {
      get
      {
        return ( Column )List[ index ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates collection with specified section owner.
    /// </summary>
    protected internal ColumnCollection( ISection section )
      : base( section )
    {
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds Column object to the collection
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    public int Add( Column column )
    {
      return List.Add( column );
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
      return DocumentEx.CreateColumnImpl();
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.ColumnTag;
      }
    }
    #endregion
  }
}