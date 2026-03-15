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
	/// Represents Table Column collection.
	/// </summary>
	public class TableColumnCollection : EntityCollectionBase
	{
	  #region Class properties
	  /// <summary>
	  /// Gets TableColumn by index
	  /// </summary>
	  public TableColumn this[ int index ]
	  {
	    get
	    {
	      return ( TableColumn )List[ index ];
	    }
	  }
	  #endregion

	  #region Class initialize / finalize methods
	  /// <summary>
	  /// Initializing constructor
	  /// </summary>
	  /// <param name="table"></param>
	  protected internal TableColumnCollection( Table table )
	    : base( table)
	  {
	  }
	  #endregion
	  
    #region Class helper methods
    /// <summary>
    /// Gets TableColumn by index
    /// </summary>
    protected internal int Add( TableColumn column )
    {
      return List.Add( column );
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
	      return XDLSConstants.ColumnItemTag;
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
	    return DocumentEx.CreateTableColumnImpl( Owner as Table );
	  }
	  #endregion
	}
}