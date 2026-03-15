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
using System.Text.RegularExpressions;
using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
using System.Collections;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a Table.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class Table
    : ParagraphItem,
      ITable,
      ITableWidget
  {
    #region Class members
    private TableColumnCollection m_columns = null;
    /// <summary>
    /// 
    /// </summary>
    private RowCollection m_rows = null;
    private ITableLayoutInfo m_tableInfo;
    //private float m_fHeight = -1;
    private float m_fWidth = -1;
    private TableFormat m_tableFormat;
    /// <summary>
    /// 
    /// </summary>
    private bool m_columnsDeserializationChecked = true;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets table width
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public float Width
    {
      get
      {
        return m_fWidth;
      }
      set
      {
        m_fWidth = value;
        //CorrectTableCellWidths();
      }
    }
    /// <summary>
    /// Gets table format
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public TableFormat TableFormat
    {
      get
      {
        return m_tableFormat;
      }
    }
    /// <summary>
    /// Get table rows
    /// </summary>
    public RowCollection Rows
    {
      get
      {
        return m_rows;
      }
    }
    /// <summary>
    /// Get last cell of the table
    /// </summary>
    public TableCell LastCell
    {
      get
      {
        if( Rows.Count > 0 )
        {
          CellCollection cells = Rows[ Rows.Count - 1 ].Cells;

          if( cells.Count > 0 )
          {
            return cells[ cells.Count - 1 ];
          }
        }
        return null;
      }
    }
    /// <summary>
    /// Get last row of the table
    /// </summary>
    public TableRow LastRow
    {
      get
      {
        if( Rows.Count > 0 )
        {
          return Rows[ Rows.Count - 1 ];
        }

        return null;
      }
    }
    /// <summary>
    /// Gets table columns
    /// </summary>
    public TableColumnCollection Columns
    {
      get
      {
        bool ensure = false;

        if( Rows.Count > 0 )
         ensure = ( Rows[ 0 ].Cells.Count != m_columns.Count );

        if( !m_columnsDeserializationChecked || ensure )
        {
          if( ensure )
            m_columns.Clear();

          EnsureColumns();
        }
               
        return m_columns;
      }
    }
    /// <summary>
    /// Get tavble cell by row and column index
    /// </summary>
    public TableCell this[ int row, int column ]
    {
      get
      {
        return Rows[ row ].Cells[ column ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public Table( IDocument doc )
      : base( doc )
    {
      m_rows = DocumentEx.CreateTableRowCollectionImpl( this );
      m_tableFormat = DocumentEx.CreateTableFormatImpl();
      m_tableFormat.Borders.BorderType = BorderStyle.Single;
      m_tableFormat.Borders.Color = Color.Black;
      m_tableFormat.Borders.LineWidth = 1f;
      m_fWidth = doc.Sections[ doc.Sections.Count - 1 ].PageSetup.ClientWidth;
      m_columns = DocumentEx.CreateTableColumnCollectionImpl( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <param name="paragraph"></param>
    protected internal Table( ITable table, IParagraph paragraph )
      : base( (paragraph as Paragraph).Document )
    {
      m_rows = DocumentEx.CreateTableRowCollectionImpl( this );
      m_tableFormat = DocumentEx.CreateTableFormatImpl();
      m_columns = DocumentEx.CreateTableColumnCollectionImpl( this );
      
      SetOwnerParagraph( paragraph as Paragraph, (table as ParagraphItem).StartIndex );
      
      m_tableFormat.ImportContainer( table.TableFormat );
      m_fWidth = table.Width;
      //m_fHeight = table.Height;

      
      for( int i =0, len = table.Rows.Count/* - 4*/; i < len; i++ )
      {
//        m_rows.Add( table.Rows[ i ].Clone( ( Table )table ) );
        m_rows.Add( table.Rows[ i ].Clone( this ) );
      }

      for( int i = 0; i < table.Columns.Count; i++ )
      {
        TableColumn column = DocumentEx.CreateTableColumnImpl( this );
        m_columns.Add( column );
      }
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Resets all cells to default state
    /// </summary>
    /// <param name="rowsNum"></param>
    /// <param name="columnsNum"></param>
    public void ResetCells( int rowsNum, int columnsNum )
    {
      if( columnsNum < 1 )
      {
        throw new ArgumentOutOfRangeException( "columnsNum", columnsNum, "Value can not be less 1" );
      }
      if( rowsNum < 0 )
      {
        throw new ArgumentOutOfRangeException( "rowsNum", columnsNum, "Value can not be less 0" );
      }

      m_rows.Clear();
      m_columns.Clear();
      AddColumn( columnsNum );

      while( rowsNum > 0 )
      {
        AddRow();
        --rowsNum;
      }
    }
    /// <summary>
    /// Adds columkn to table
    /// </summary>
    public void AddColumn()
    {
      m_columns.Add( DocumentEx.CreateTableColumnImpl( this ) );
      
      for( int i = 0; i < Rows.Count; i++ )
      {
        TableRow row = Rows[ i ];
        row.Cells.Add( DocumentEx.CreateTableCellImpl( row ) );
      }
      
    }
    /// <summary>
    /// Adds several columns to table
    /// </summary>
    public void AddColumn( int columnsCount )
    {
      while( columnsCount > 0 )
      {
        AddColumn();
        --columnsCount;
      }
    }
    /// <summary>
    /// Adds row to table
    /// </summary>
    /// <returns></returns>
    public TableRow AddRow()
    {
      TableRow row = DocumentEx.CreateTableRowImpl( this );
      row.ResetCells();
      m_rows.Add( row );
      return row;
    }
    /// <summary>
    /// Adds row to table with copy format option
    /// </summary>
    /// <param name="isCopyFormat">Indicates whether copy format from previous row or not</param>
    /// <returns></returns>
    public TableRow AddRow( bool isCopyFormat )
    {
      TableRow lastRow = ( Rows.Count > 0 ) ? Rows[ Rows.Count - 1 ] : null;
      TableRow row = AddRow();
      
      if( isCopyFormat && lastRow != null )
      {
        for( int i=0, cnt = lastRow.Cells.Count; i < cnt; i++ )
        {
          row.Cells[ i ].CellFormat.ImportContainer( lastRow.Cells[ i ].CellFormat );
          row.Cells[ i ].UpdateWidth( lastRow.Cells[ i ].InternalWidth );
        }
        
        if( lastRow.Height != 0 )
        {
          row.Height = lastRow.Height;
        }
      }
      
      return row;
    }
    /// <summary>
    /// Clones itself
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public override IParagraphItem Clone( IParagraph paragraph )
    {
      return new Table( this, paragraph );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="replace"></param>
    /// <returns></returns>
    internal int Replace( Regex pattern, string replace )
    {
      int changesMade = 0;
      
      foreach( TableRow row in m_rows )
      {
        foreach( TableCell cell in row.Cells )
        {
          foreach( Paragraph paragraph in cell.Paragraphs )
          {
            changesMade += paragraph.Replace( pattern, replace );
          }
        }
      }
      
      return changesMade;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    internal TextRangesHolder Find( Regex pattern )
    {
      TextRangesHolder holder = new TextRangesHolder();
      
      foreach( TableRow row in m_rows )
      {
        foreach( TableCell cell in row.Cells )
        {
          foreach( Paragraph paragraph in cell.Paragraphs )
          {
            holder = paragraph.Find( pattern );

            if( holder != null && holder.Count > 0 )
            {
              return holder;
            }
          }
        }
      }
      
      return holder;
    }
    internal ArrayList FindAll( Regex pattern )
    {
      ArrayList holderList = new ArrayList();

      foreach( TableRow row in m_rows )
      {
        foreach( TableCell cell in row.Cells )
        {
          foreach( Paragraph paragraph in cell.Paragraphs )
          {
            ArrayList holders = paragraph.FindAll( pattern );

            if( holders != null && holders.Count > 0 )
            {
              holderList.AddRange( holders );
            }
          }
        }
      }

      return holderList;
    }
    #endregion

    #region XDLSSerializable overrides
    /// <summary>
    /// 
    /// </summary>
    protected override void InitXDLSHolder()
    {
      //XDLSHolder.AddElement( XDLSConstants.ColumnsItemTag, Columns );
      XDLSHolder.AddElement( XDLSConstants.RowsItemTag, Rows );
      XDLSHolder.AddElement( XDLSConstants.TableFormatTag, TableFormat );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.TypeTag, ParagraphItemType.Table );
      writer.WriteValue( XDLSConstants.TableWidthAttr, Width );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      m_columnsDeserializationChecked = false;
      if( reader.HasAttribute( XDLSConstants.TableWidthAttr ) )
      {
        Width = reader.ReadFloat(XDLSConstants.TableWidthAttr);
      }
    }
    #endregion

    #region ITableWidget implement
    /// <summary>
    /// 
    /// </summary>
    protected override void CreateLayoutInfo()
    {
      m_layoutInfo = new LayoutInfo( ChildrenLayoutDirection.Horizontal );
      DetermineTableFormat();
    }
    /// <summary>
    /// Determines the table format.
    /// </summary>
    private void DetermineTableFormat()
    {
      float cellSpacing = TableFormat.CellSpacing / 4;
      float leftIndent = TableFormat.LeftIndent;

      if( /*cellSpacing*/TableFormat.CellSpacing > -1 )
      {
        Borders borders = TableFormat.Borders;
        float leftHalfWidth = borders.Left.LineWidth / 2;
        float topHalfWidth = borders.Top.LineWidth / 2;
        float rightHalfWidth = borders.Right.LineWidth / 2;
        float bottomHalfWidth = borders.Bottom.LineWidth / 2;
      
        Spacings paddings = m_layoutInfo.Paddings;
        paddings.Left = leftHalfWidth + cellSpacing;
        paddings.Top = topHalfWidth + cellSpacing;
        paddings.Right = rightHalfWidth + cellSpacing;
        paddings.Bottom = bottomHalfWidth + cellSpacing;
        
        Spacings margins = m_layoutInfo.Margins;
        margins.Left = leftHalfWidth;
        margins.Top = topHalfWidth;
        margins.Right = rightHalfWidth;
        margins.Bottom = bottomHalfWidth;
      }
      
      m_layoutInfo.Margins.Left += leftIndent;
      m_layoutInfo.IsLineBreak = true;
    }
    /// <summary>
    /// 
    /// </summary>
    ITableLayoutInfo ITableWidget.TableLayoutInfo
    {
      get
      {
        if( m_tableInfo == null )
        {
          m_tableInfo = new TableLayoutInfo( this );
        }
        
        return m_tableInfo;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    int ITableWidget.RowsCount
    {
      get
      {
        return m_rows.Count;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    int ITableWidget.ColumnsCount
    {
      get
      {
        return Columns.Count;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    IWidgetContainer ITableWidget.GetCellWidget( int row, int column )
    {
      return m_rows[ row ].Cells[ column ];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    IWidget ITableWidget.GetRowWidget( int row )
    {
      return m_rows[ row ];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      ( cg as DLSGraphics ).DrawTable( this, ltWidget );
    }
    #endregion
    
    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    protected internal void CorrectTableCellWidths()
    {
      if( Rows.Count > 0 )
      {
        TableRow row = Rows[ 0 ];
        
        if( Rows.Count > 1 )
        {
          SynhranizeOtherCellWidths( row );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected internal void CorrectTableWidth( TableRow row )
    {
      float minTableWidth = 0;
      
      foreach( TableCell cell in row.Cells )
      {
        if( cell.IsFixedWith )
          minTableWidth += cell.Width;
      }
      
      if( minTableWidth > 0 && minTableWidth > m_fWidth )
      {
        m_fWidth = minTableWidth;
      }

      SynhranizeOtherCellWidths( row );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    private void SynhranizeOtherCellWidths( TableRow row )
    {
      int rowIndex = row.GetRowIndex();

      bool synhronize = true;
      int prev = Rows[0].Cells.Count;

      for( int i = 0; i < Rows.Count; i++ )
      {
        if( Rows[ i ].Cells.Count != prev )
        {
          synhronize = false;
          break;
        }
      }

      if( synhronize )
      {
        for( int i = 0; i < Rows.Count; i++ )
        {
          if( i != rowIndex )
          {
            TableRow iRow = Rows[ i ];

            for( int j = 0; j < iRow.Cells.Count; j++ )
            {
              TableCell cell = iRow.Cells[ j ];
              if( cell.CellFormat.VerticalMerge == row.Cells[ j ].CellFormat.VerticalMerge )
                cell.UpdateWidth( row.Cells[ j ].InternalWidth );
            }
          }
        }
      }         
    }
    /// <summary>
    /// 
    /// </summary>
    private void EnsureColumns()
    {
      if( Rows.Count > 0 )
      {
        CellCollection cells = Rows[0].Cells;
        if( m_columns.Count != cells.Count )
        {
          for (int i = 0; i < cells.Count; i++)
          {
            m_columns.Add(DocumentEx.CreateTableColumnImpl(this));
          }
        }
      }
      m_columnsDeserializationChecked = true;
    }
    #endregion
    
    #region Class internal declarations
    /// <summary>
    /// 
    /// </summary>
    protected class TableLayoutInfo 
      : ITableLayoutInfo
    {
      #region Class members
      /// <summary>
      /// 
      /// </summary>
      private Table m_table;
      private float[] m_cellsWidth;
      private int m_headersRowCount = 0;
      private bool[] m_isDefaultCells;
      #endregion

      #region Class properties
      /// <summary>
      /// 
      /// </summary>
      public float Width
      {
        get
        {
          return m_table.Width;
        }
        set
        {
          m_table.Width = value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public float Height
      {
        get
        {
          return 0f;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public float[] CellsWidth
      {
        get
        {
          return m_cellsWidth;
        }
        set
        {
          m_cellsWidth = value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public int HeadersRowCount
      {
        get
        {
          return m_headersRowCount;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool[] IsDefaultCells
      {
        get
        {
          return m_isDefaultCells;
        }
      }
      #endregion
      
      #region Class initialize/finalize methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="table"></param>
      public TableLayoutInfo( Table table )
      {
        m_table = table;        
        m_cellsWidth = new float[ m_table.Columns.Count ];
        m_isDefaultCells = new bool[ m_table.Columns.Count ];

        for( int i = 0, cnt = m_table.Columns.Count; i < cnt; i++ )
        {
          TableCell cell = m_table.LastRow.Cells[ i ];
          m_cellsWidth[ i ] = cell.Width;
          m_isDefaultCells[ i ] = cell.IsFixedWith;
        }
        m_headersRowCount = GetHeadersRowCount();
      }
      #endregion
      
      #region Class helper methods
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private int GetHeadersRowCount()
      {
        int hrCount = 0;
        for( int i = 0; i < m_table.Rows.Count; i++ )
        {
          TableRow row = m_table.Rows[ i ];
        
          if( !row.IsHeader )
            break; 
        
          hrCount++;
        }
      
        return hrCount;
      }
      #endregion

      #region ITableLayoutInfo Members

      /// <summary>
      /// Gets owner table cell spasings.
      /// </summary>
      public double CellSpasings
      {
        get
        {
          if( ( m_table.Owner.Owner as TableCell ) != null )
          {
            double spasings = ( m_table.Owner.Owner as TableCell ).OwnerRow.OwnerTable.TableFormat.CellSpacing * 2;
            return Math.Max( 0, spasings );
          }

          return 0;
        }
      }
      /// <summary>
      /// Gets owner cell paddings.
      /// </summary>
      public double CellPaddings
      {
        get
        {
          if( ( m_table.Owner.Owner as TableCell ) != null )
          {
            return ( m_table.Owner.Owner as TableCell ).CellFormat.Paddings.Left + ( m_table.Owner.Owner as TableCell ).CellFormat.Paddings.Right;
          }

          return 0;
        }
      }

      #endregion      
    }
    #endregion
  }
}