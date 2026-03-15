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

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a Table Row.
  /// </summary>
  public class TableRow : WidgetBase, IWidget
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private CellCollection m_cells;
    /// <summary>
    /// 
    /// </summary>
    private float m_rowHeight;
    /// <summary>
    /// 
    /// </summary> 
    private TableRowHeightType m_heightType;
//    /// <summary>
//    /// 
//    /// </summary>
//    private float m_rowHeightExactly;
//    /// <summary>
//    /// 
//    /// </summary>
    //private Table m_ownerTable;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bHeader = false;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets/sets whether the row is a table header.
    /// </summary>
    public bool IsHeader
    {
      get
      {
        return m_bHeader;
      }
      set
      {
        m_bHeader = value;
      }
    }
    /// <summary>
    /// Gets/sets cell collection.
    /// </summary>
    public CellCollection Cells
    {
      get
      {
        return m_cells;
      }
      set
      {
        m_cells = value;
      }
    }
    /// <summary>
    /// Gets/sets height of the row.
    /// </summary>
    public float Height
    {
      get
      {
        return m_rowHeight;
      }
      set
      {
//        if( value <= 0 )
//        {
//          throw new ArgumentException( "Height cannot be less than or equal to Zero " );
//        }
        m_rowHeight = value;
      }
    }
    /// <summary>
    /// Get/set table row height type
    /// </summary>
    public TableRowHeightType HeightType
    {
      get
      {
        return m_heightType;
      }
      set
      {
         m_heightType = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Table OwnerTable
    {
      get
      {
        return Owner as Table;
      }
    }
#if DEBUG_LAYOUTING
    /// <summary>
    /// 
    /// </summary>
    private int DBG_RowIndex
    {
      get
      {
        return GetRowIndex();
      }
    }
#endif
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates TableRow object for specified table.
    /// </summary>
    public TableRow( Table owner )
      : base( owner.Document )
    {
      //m_ownerTable = owner;
      SetOwner( owner );
      m_cells = DocumentEx.CreateTableCellCollectionImpl( this );
    }
    /// <summary>
    /// 
    /// </summary>
    protected internal TableRow( TableRow row, Table owner )
      : this( owner )
    {
      foreach( TableCell cell in row.Cells )
      {
        Cells.Add( cell.Clone( this ) );
      }

      IsHeader = row.IsHeader;
      Height = row.Height;
      HeightType = row.HeightType;
      
//      if( row.Height > 0 )
//      {
//        HeightType = TableRowHeightType.AtLeast
//        Height = row.Height;
//      }
//      if( row.HeightExactly > 0 )
//      {
//        HeightExactly = row.HeightExactly;
//      }
      
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Reset all cells to default state.
    /// </summary>
    protected internal void ResetCells()
    {
      m_cells.Clear();

      for( int i = 0; i < OwnerTable.Columns.Count; i++ )
      {
        TableColumn column = OwnerTable.Columns[ i ];
        TableCell cell = DocumentEx.CreateTableCellImpl( this );
        cell.UpdateWidth( column.InternalWidth );
        m_cells.Add( cell );
      }
    }
    /// <summary>
    /// Clones itself and sets new owner document.
    /// </summary>
    /// <returns></returns>
    public TableRow Clone( Table table )
    {
      return CloneImpl( table );
    }
    /// <summary>
    /// Returns index of the row in owner table.
    /// </summary>
    /// <returns></returns>
    public int GetRowIndex()
    {
      return OwnerTable.Rows.IndexOf( this );
    }
    #endregion

    #region XDLS serializable overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      //XDLSHolder.AddElement( XDLSConstants.TableFormatTag, TableFormat );
      XDLSHolder.AddElement( XDLSConstants.CellsItemTag, Cells );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      //base.WriteXmlAttributes (writer);
      if( Height > 0 )
      {
        writer.WriteValue( XDLSConstants.TableRowHeigthAttr, Height );
      }
      if( IsHeader )
      {
        writer.WriteValue( XDLSConstants.TableRowHeaderAttr, IsHeader );
      }
      writer.WriteValue( XDLSConstants.TableRowHeighTypeAttr, HeightType );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      //base.ReadXmlAttributes (reader);
      if( reader.HasAttribute( XDLSConstants.TableRowHeigthAttr ) )
      {
        Height = reader.ReadFloat( XDLSConstants.TableRowHeigthAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TableRowHeaderAttr ) )
      {
        IsHeader = reader.ReadBoolean( XDLSConstants.TableRowHeaderAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TableRowHeighTypeAttr ) )
      {
        HeightType = ( TableRowHeightType )reader.ReadEnum( XDLSConstants.TableRowHeighTypeAttr, typeof( TableRowHeightType ) );
      }
    }
    #endregion

    #region WidgetBase overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutTableInfo( m_heightType == TableRowHeightType.Exactly, m_rowHeight );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      ( cg as DLSGraphics ).DrawTableRow( this, ltWidget );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal float CalculateDinamicCellWidth()
    {
      float fixedWidth = 0;
      int countFickleCells = 0;

      foreach( TableCell cell in Cells )
      {
        if( cell.IsFixedWith )
        {
          fixedWidth += cell.Width;
        }
        else
        {
          ++countFickleCells;
        }
      }

      float fickleWidth = OwnerTable.Width - fixedWidth;
      return ( fickleWidth < 0 ) ? -1f : fickleWidth / countFickleCells;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected virtual TableRow CloneImpl( Table table )
    {
      return new TableRow( this, table );
    }
    #endregion
  }
}