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
#endregion

using Syncfusion.DLS.XML;

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a Table Column.
  /// </summary>
  public class TableColumn : XDLSSerializableBase
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private ITable m_ownerTable;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets index of column in owner table.
    /// </summary>
    /// <returns></returns>
    protected int ColumnIndex
    {
      get
      {
        return OwnerTable.Columns.IndexOf( this );
      }
    }
    /// <summary>
    /// Gets owner table.
    /// </summary>
    public ITable OwnerTable
    {
      get
      {
        return m_ownerTable;
      }
    }
    /// <summary>
    /// Get/sets the width of the column.
    /// </summary>
    public float Width
    {
      get
      {
        if( OwnerTable.Rows.Count > 0 )
        {
          return OwnerTable.Rows[ 0 ].Cells[ ColumnIndex ].Width;
        }
        
        return -1;
      }
      set
      {
        if( Width != value )
        {
          int columnIndex = ColumnIndex;
          
          foreach( TableRow row in OwnerTable.Rows )
          {
            row.Cells[ columnIndex ].UpdateWidth( value );
          }
          
          if( OwnerTable.Rows.Count > 0 )
          {
            (OwnerTable as Table).CorrectTableWidth( OwnerTable.Rows[ 0 ] );
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal float InternalWidth
    {
      get
      {
        if( OwnerTable.Rows.Count > 0 )
        {
          return OwnerTable.Rows[ 0 ].Cells[ ColumnIndex ].InternalWidth;
        }
        
        return -1;
      }
    }
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// Creates TableColumn object for specified table
    /// </summary>
    public TableColumn( ITable table ) 
      : base( table.Document )
    {
      m_ownerTable = table;
    }
    #endregion
  }
}