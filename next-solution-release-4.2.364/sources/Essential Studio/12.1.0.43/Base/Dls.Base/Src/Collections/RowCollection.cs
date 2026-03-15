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
  /// Represents row collection
  /// </summary>
  public class RowCollection : EntityCollectionBase
  {
    #region Class properties
    /// <summary>
    /// Gets TableRow by index
    /// </summary>
    public TableRow this[ int index ]
    {
      get
      {
        return ( TableRow )List[ index ];
      }
    }
    #endregion

    #region Class initialize / finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="table"></param>
    protected internal RowCollection( Table table )
      : base( table )
    {
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds table row to collection
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public int Add( TableRow row )
    {
      return List.Add( row );
    }
    /// <summary>
    /// Inserts an row to row collection
    /// </summary>
    /// <param name="index"></param>
    /// <param name="row"></param>
    public void Insert( int index, TableRow row )
    {
      List.Insert( index, row );
    }
    /// <summary>
    /// Returns index in collection of a specified row
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public int IndexOf( TableRow row )
    {
      return List.IndexOf( row );
    }
    #endregion

    #region XML serializable overrides
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.RowItemTag;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object CreateItem( IXDLSContentReader reader )
    {
      return DocumentEx.CreateTableRowImpl( Owner as Table );
    }
    #endregion
  }
}