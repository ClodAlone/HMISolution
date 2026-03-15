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
  /// Represents cell collection
  /// </summary>
  public class CellCollection : EntityCollectionBase
  {
    #region Class properties
    /// <summary>
    /// Gets table cell by index.
    /// </summary>
    public TableCell this[ int index ]
    {
      get
      {
        return List[ index ] as TableCell;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates collection with specified row owner.
    /// </summary>
    /// <param name="row"></param>
    public CellCollection( TableRow row )
      : base( row )
    {
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Adds cell to table
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    internal protected int Add( TableCell cell )
    {
      return List.Add( cell );
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.CellItemTag;
      }
    }
    /// <summary>
    /// Creates a new cell
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    protected override object CreateItem( IXDLSContentReader reader )
    {
      return DocumentEx.CreateTableCellImpl( Owner as TableRow );
    }
    #endregion
  }
}