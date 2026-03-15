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
using System.Drawing;

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a Table Cell.
  /// </summary>
  public class TableCell 
  : TextBody,
    IWidget
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private CellFormat m_cellFormat;
    private float m_cellWidth = -1;
    private TableRow m_ownerRow;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets owner row of the cell.
    /// </summary>
    public TableRow OwnerRow
    {
      get
      {
        return m_ownerRow;
      }
    }
    /// <summary>
    /// Gets / sets cell width.
    /// </summary>
    /// <remarks>Not supported by Essential DPF</remarks>
    public float Width
    {
      get
      {
        if( !IsFixedWith )
        {
          return m_ownerRow.CalculateDinamicCellWidth();
        }
        
        return m_cellWidth;
      }
      set
      {
        if( m_cellWidth != value )
        {
          m_cellWidth = value;
          m_ownerRow.OwnerTable.CorrectTableWidth( m_ownerRow );
        }
      }
    }
    /// <summary>
    /// Gets cell format.
    /// </summary>
    /// <remarks>Not supported by Essential DPF</remarks>
    public CellFormat CellFormat
    {
      get
      {
        return m_cellFormat;
      }
    }
    /// <summary>
    /// Gets if size of cell is fixed.
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal bool IsFixedWith
    {
      get
      {
        return ( m_cellWidth > -1 );
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
        return m_cellWidth;
      }
    }
#if DEBUG_LAYOUTING
    /// <summary>
    /// 
    /// </summary>
    private int DBG_CellIndex
    {
      get
      {
        return GetCellIndex();
      }
    }
#endif
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Creates TableCell object for specified row.
    /// </summary>
    public TableCell( TableRow row )
      : base( row.Document )
    {
      m_ownerRow = row;
      m_paragraphs = DocumentEx.CreateParagraphCollectionImpl( this );
      m_cellFormat = DocumentEx.CreateCellFormatImpl();
      TableFormat tableFormat = OwnerRow.OwnerTable.TableFormat;
     
//      m_cellFormat.Borders.ImportContainer( tableFormat.Borders );
      
//      m_cellFormat.Borders.Left.Color = tableFormat.Borders.Left.Color;
//      m_cellFormat.Borders.Right.Color = tableFormat.Borders.Right.Color;
//      m_cellFormat.Borders.Top.Color = tableFormat.Borders.Top.Color;
//      m_cellFormat.Borders.Bottom.Color = tableFormat.Borders.Bottom.Color;
      Borders cellBorders = m_cellFormat.Borders;
      Borders tableBorders = tableFormat.Borders;
      
//      CopyBorderFormatting( cellBorders.Left, tableBorders.Left );
//      CopyBorderFormatting( cellBorders.Bottom, tableBorders.Bottom );
//      CopyBorderFormatting( cellBorders.Right, tableBorders.Right );
//      CopyBorderFormatting( cellBorders.Top, tableBorders.Top );
      cellBorders.Left.CopyBorderFormatting( tableBorders.Left );
      cellBorders.Right.CopyBorderFormatting( tableBorders.Right );
      cellBorders.Bottom.CopyBorderFormatting( tableBorders.Bottom );
      cellBorders.Top.CopyBorderFormatting( tableBorders.Top );
      
      m_cellFormat.ApplyBase( tableFormat );
    }
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="row"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal TableCell( TableCell cell, TableRow row )
      : this( row )
    {
      m_cellWidth = cell.InternalWidth;
      m_cellFormat.ImportContainer( cell.CellFormat );
//      m_cellFormat.ApplyBase( row.OwnerTable.TableFormat );
      
      foreach( Paragraph p in cell.Paragraphs )
      {
        Paragraphs.Add( p.Clone( row.Document ) );
      }
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Clones itself for specified row.
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public TableCell Clone( TableRow row )
    {
      return CloneImpl( row );
    }
    /// <summary>
    /// Get cell index in the table row.
    /// </summary>
    /// <returns></returns>
    public int GetCellIndex()
    {
      return OwnerRow.Cells.IndexOf( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected virtual TableCell CloneImpl( TableRow row )
    {
      return new TableCell( this, row );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal void UpdateWidth( float width )
    {
      m_cellWidth = width;
    }
    protected internal void UpdateWidth( float width, bool saveFixed )
    {
      bool isfixed = IsFixedWith;
      
      m_cellWidth = width;
      m_ownerRow.OwnerTable.CorrectTableWidth( m_ownerRow );
      
      if( !isfixed && saveFixed )
      {
        m_cellWidth = -1;
      }
    }
    #endregion
    
    #region XDLSSerializableBase overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( XDLSConstants.ParagraphsTag, Paragraphs );
      XDLSHolder.AddElement( XDLSConstants.CellFormatTag, CellFormat );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      
      if( IsFixedWith )
      {
        writer.WriteValue( XDLSConstants.TableCellWidthAttr, Width );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      
      if( reader.HasAttribute( XDLSConstants.TableCellWidthAttr ) )
      {
        UpdateWidth(  reader.ReadFloat( XDLSConstants.TableCellWidthAttr ));
      }
    }
    #endregion

    #region WidgetContainer overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void CreateLayoutInfo()
    {
      if( Paragraphs.Count == 0 )
      {
        // NOTE: if paragraphs count = 0 ???
        AddParagraph();
      }
      
      m_layoutInfo = new LayoutCellInfo( this );
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override ICollectionBase WidgetCollection
    {
      get
      {
        return m_paragraphs;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      (cg as DLSGraphics).DrawTableCell( this, ltWidget );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="destBorder"></param>
    /// <param name="sourceBorder"></param>
    private void CopyBorderFormatting( Border destBorder, Border sourceBorder )
    {
      destBorder.BorderType = sourceBorder.BorderType;
      destBorder.LineWidth = sourceBorder.LineWidth;
      destBorder.Color = sourceBorder.Color;
      destBorder.Shadow = sourceBorder.Shadow;
    }
    #endregion
    
    #region Class Internal declarations
    /// <summary>
    /// 
    /// </summary>
    internal class LayoutCellInfo 
      : LayoutTableInfo
    {
      #region Class Members
      /// <summary>
      /// 
      /// </summary>
      TableCell m_cell;
      #endregion

      #region Class initialize/finalize methods
      /// <summary>
      /// Initializes a new instance of the <see cref="LayoutCellInfo"/> class.
      /// </summary>
      /// <param name="cell">The cell.</param>
      internal LayoutCellInfo( TableCell cell )
        : base( ChildrenLayoutDirection.Vertical )
      {
        m_cell = cell;
        InitSpacings();
        InitMerges();
        CellFormat cellFormat = m_cell.CellFormat;
        m_verticalAlignment = ( byte )cellFormat.VerticalAlignment;
        m_textWrap = cellFormat.TextWrap;
      }
      #endregion
      
      #region Class helper methods
      /// <summary>
      /// Corrects the mergins.
      /// </summary>
      private void CorrectMergins()
      {
        int cellIndex = m_cell.GetCellIndex();
        int rowIndex = m_cell.OwnerRow.GetRowIndex();
        
        if( cellIndex != 0 )
        {
          Margins.Left = 0;
        }
        if( cellIndex != m_cell.OwnerRow.Cells.Count - 1 )
        {
          Margins.Right = 0;
        }
        if( rowIndex != 0 )
        {
          Margins.Top = 0;
        }
        if( rowIndex != m_cell.OwnerRow.OwnerTable.Rows.Count - 1 )
        {
          Margins.Bottom = 0;
        }
      }
      /// <summary>
      /// Determines the formats.
      /// </summary>
      private void InitMerges()
      {
        CellFormat cellFormat = m_cell.CellFormat;
        m_isColumnMergeStart = ( cellFormat.HorizontalMerge == CellMerge.Start );
        m_isColumnMergeContinue = ( cellFormat.HorizontalMerge == CellMerge.Continue );
        m_isRowMergeStart = ( cellFormat.VerticalMerge == CellMerge.Start );
        m_isRowMergeContinue = ( cellFormat.VerticalMerge == CellMerge.Continue );
      }
      /// <summary>
      /// Determines the cell spacing.
      /// </summary>
      private void InitSpacings()
      {
        Borders borders = m_cell.CellFormat.Borders;
        Paddings paddings = m_cell.CellFormat.Paddings;

        if( borders.NoBorder ) 
          borders = m_cell.OwnerRow.OwnerTable.TableFormat.Borders;
                                 
        float leftHalfWidth = borders.Left.LineWidth / 2;
        float topHalfWidth = borders.Top.LineWidth / 2;
        float rightHalfWidth = borders.Right.LineWidth / 2;
        float bottomHalfWidth = borders.Bottom.LineWidth / 2;
        float cellSpacing = m_cell.OwnerRow.OwnerTable.TableFormat.CellSpacing / 2;

        Paddings.Left = leftHalfWidth + paddings.Left;
        Paddings.Top = topHalfWidth + ( ( paddings.Top > 0 ) ? paddings.Top : m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.Top );        
        Paddings.Right = rightHalfWidth + paddings.Right;
        Paddings.Bottom = bottomHalfWidth + ( ( paddings.Bottom > 0 ) ? paddings.Bottom : m_cell.OwnerRow.OwnerTable.TableFormat.Paddings.Bottom );

        Margins.Left = cellSpacing + leftHalfWidth;
        Margins.Top = cellSpacing + topHalfWidth;
        Margins.Right = cellSpacing + rightHalfWidth;
        Margins.Bottom = cellSpacing + bottomHalfWidth;
        
        if( cellSpacing < 0 )
        {
          CorrectMergins();
        }
      }
      #endregion
    }
    #endregion
  }
}
