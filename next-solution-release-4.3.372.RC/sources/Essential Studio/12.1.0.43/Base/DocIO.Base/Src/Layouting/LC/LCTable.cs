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

#region Class members
using System;
using System.Drawing;
#endregion

namespace Syncfusion.Layouting
{                                                  
  /// <summary>
  /// Summary description for LCTable.
  /// </summary>
  public class LCTable : LayoutContext
  {
    #region Members 
    //private double m_skipOffset = 0f;
    //private bool m_bIsHorizontalNotFitted = false;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bHeaderRepeat = false;
    /// <summary>
    /// 
    /// </summary>
    private int m_currHeaderRowIndex = -1;
    /// <summary>
    /// 
    /// </summary>
    private int m_currRowIndex = -1;
    /// <summary>
    /// 
    /// </summary>
    private int m_currColIndex = -1;
    /// <summary>
    /// 
    /// </summary>
    private double[] m_columnsWidth;
    /// <summary>
    /// 
    /// </summary>
    private double[] m_columnsPosX;
    /// <summary>
    /// 
    /// </summary>
    private LayoutedWidget m_currRowLW;
    /// <summary>
    /// 
    /// </summary>
    private LayoutedWidget m_currCellLW;
    /// <summary>
    /// 
    /// </summary>
    private double[] m_rowsHeight;
    /// <summary>
    /// 
    /// </summary>
    protected bool m_bAtLastOneCellFitted = false;
    /// <summary>
    /// 
    /// </summary>
    private SplitWidgetContainer[] m_splitedCells;
    /// <summary>
    /// 
    /// </summary>
    private LayoutState m_blastRowState = LayoutState.Unknown;
    /// <summary>
    /// 
    /// </summary>
    private SplitTableWidget m_spitTableWidget = null;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the table layout info.
    /// </summary>
    /// <value>The table layout info.</value>
    public ITableLayoutInfo TableLayoutInfo
    {
      get
      {
        return TableWidget.TableLayoutInfo;
      }
    }
    /// <summary>
    /// Gets the table widget.
    /// </summary>
    /// <value>The table widget.</value>
    public ITableWidget TableWidget
    {
      get
      {
        return m_widget as ITableWidget;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected int CurrRowIndex
    {
      get
      {
        if( m_bHeaderRepeat )
        {
          return m_currHeaderRowIndex;
        }
        return m_currRowIndex;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="LCTable"/> class.
    /// </summary>
    /// <param name="splitWidget">The split widget.</param>
    /// <param name="lcOperator">The lc operator.</param>
    public LCTable( SplitTableWidget splitWidget, ILCOperator lcOperator )
      : base( splitWidget.TableWidget, lcOperator )
    {
      m_bHeaderRepeat = true;
      m_currRowIndex = splitWidget.StartRowNumber - 1;
      m_spitTableWidget = splitWidget;

      //m_currColIndex = splitWidget.StartColumnNumber - 1;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="LCTable"/> class.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="lcOperator">The lc operator.</param>
    public LCTable( ITableWidget table, ILCOperator lcOperator )
      : base( table, lcOperator )
    {
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Layouts the specified rect.
    /// </summary>
    /// <param name="rect">The rect.</param>
    /// <returns></returns>
    public override LayoutedWidget Layout( RectangleF rect )
    {
      CreateTableClientArea( rect );
      CalculateColumnsWidth();
      CreateLayoutedWidget( rect.Location );
      m_rowsHeight = new double[TableWidget.ColumnsCount];

      do
      {
        if( !CreateRowLayoutedWidget() )
        {
          if( m_bAtLastOneCellFitted )
          {
            m_ltState = LayoutState.Fitted;
          }
          break;
        }
        
        DoLayoutRow();
        CommitRow();
      }
      while( m_ltState == LayoutState.Unknown );

      DeleteContinuousCells();

#if DEBUG_LAYOUTING
      /* Debug code */ DBG_CommitChildContext( this );
#endif
      return m_ltWidget;
    }
    #endregion

    #region Implementation ( row )
    /// <summary>
    /// 
    /// </summary>
    private void CalculateColumnsWidth()
    {
      int columnsCount = TableWidget.ColumnsCount;
      m_columnsWidth = new double[ columnsCount ];
      m_columnsPosX = new double[ columnsCount ];
      
      for( int i = 0, len = m_columnsWidth.Length; i < len; i++ )
      {
        m_columnsWidth[ i ] = TableLayoutInfo.CellsWidth[ i ];
        m_columnsPosX[ i ] = ( i == 0 )
          ? 0
          : m_columnsPosX[ i - 1 ] + TableLayoutInfo.CellsWidth[ i - 1 ];
      }            
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool CreateRowLayoutedWidget()
    {
      if( CurrRowIndex + 1 < TableWidget.RowsCount )
      {
        m_currColIndex = -1;
        NextRowIndex();
        m_currRowLW = new LayoutedWidget( 
          TableWidget.GetRowWidget( CurrRowIndex ) 
          );
        
        m_currRowLW.Bounds = new RectangleF( m_layoutArea.ClientActiveArea.Location, new SizeF() );
        return true;
      }
      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    private void DoLayoutRow()
    {
#if DEBUG_LAYOUTING
      DBG_WriteSpec( m_currRowIndex.ToString(), "NEW ROW" );
#endif
      m_splitedCells = new SplitWidgetContainer[ TableWidget.ColumnsCount ];
      LayoutTableInfo tableInfo;
      
      do
      {
        // Creates next child context 
        LayoutContext childContext = CreateNextCellContext();
        
        if( childContext == null )
        {
          break;
        }

        tableInfo = childContext.Widget.LayoutInfo as LayoutTableInfo;
                
        if( tableInfo.IsColumnMergeContinue )
        {
          m_currRowLW.ChildWidgets.Add( new LayoutedWidget( childContext.Widget ) );
          continue;
        }

        LayoutArea cellArea = GetCellClientArea( ( tableInfo != null ) ? tableInfo.IsColumnMergeStart : false );

        if( cellArea.Height > m_layoutArea.ClientActiveArea.Height )
        {
          m_ltState = LayoutState.NotFitted;
        }
        else
        {
#if DEBUG_LAYOUTING
          DBG_WriteSpec( m_currRowIndex.ToString() + " / " + m_currColIndex.ToString(), "ROW / CELL" );
#endif
          DoLayoutCell( childContext, cellArea.ClientArea ); //??
          CommitCellContext( childContext );
        }
      }
      while( State == LayoutState.Unknown );
    }
    /// <summary>
    /// 
    /// </summary>
    private void CommitRow()
    {
      if( m_ltState == LayoutState.Unknown && m_bAtLastOneCellFitted )
      {
        UpdateRowSize();
        
        //for( int i = 0; i < m_currRowLW.ChildWidgets.Count; i++ )
        for( int i = 0; i < TableWidget.ColumnsCount; i++ )        
        {
          UpdateCellSize( i );
        }

        // Adds row to table
        m_ltWidget.ChildWidgets.Add( m_currRowLW );
        UpdateLWBounds();
        UpdateClientArea();
       
        if( m_blastRowState == LayoutState.Splitted && m_ltWidget.ChildWidgets.Count > 0 )
        {
          m_sptWidget = new SplitTableWidget( TableWidget, m_currRowIndex + 1, m_splitedCells );
          m_ltState = LayoutState.Splitted;
        }
      }
      else if( m_ltWidget.ChildWidgets.Count > 0)
      {
        m_sptWidget = new SplitTableWidget( TableWidget, m_currRowIndex + 1 );
        m_ltState = LayoutState.Splitted;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateRowLWBounds( LayoutContext childContext )
    {
      RectangleF bounds = m_currRowLW.Bounds;
      RectangleF childBounds = m_currCellLW.Bounds;

      double rightPad = ( m_bSkipAreaSpacing ) ? 
        0f : childContext.BoundsPaddingRight;
      double bottomPad = ( m_bSkipAreaSpacing ) ? 
        0f : childContext.BoundsPaddingBottom;
      
      //ChangeChildsAlignment();
      
      double right = Math.Max( childBounds.Right + rightPad, bounds.Right );
      double bottom = Math.Max( childBounds.Bottom + bottomPad, bounds.Bottom );
      SizeF size = new SizeF( (float)(right - bounds.Left), (float)(bottom - bounds.Top) );
      m_currRowLW.Bounds = new RectangleF( bounds.Location, size );
    }
    /// <summary>
    /// 
    /// </summary>
    private void NextRowIndex()
    {
      if( m_bHeaderRepeat )
      {
        //Trace.WriteLine( "Header Row:" + m_currHeaderRowIndex );
        m_currHeaderRowIndex++;
        
        if( TableLayoutInfo.HeadersRowCount < m_currHeaderRowIndex + 1 )
        {
          m_bHeaderRepeat = false;
        }
      }
      else
      {
        //Trace.WriteLine( "Row:" + m_currRowIndex );
        m_currRowIndex++;
      }
    }
    #endregion
    
    #region Implementation
    /// <summary>
    /// Updates the height of the row.
    /// </summary>
    private void UpdateRowSize()
    {
      RectangleF rowBounds = m_currRowLW.Bounds;
      double maxPadding = 0;
      
      for( int i = 0; i < m_currRowLW.ChildWidgets.Count; i++ )
      {
        LayoutedWidget cellLtWidget = m_currRowLW.ChildWidgets[ i ];

        LayoutTableInfo cellTableInfo = cellLtWidget.Widget.LayoutInfo as LayoutTableInfo;
        
        bool currCellContinue = cellTableInfo.IsRowMergeContinue;
        bool nextCellContinue = false;
        
        maxPadding =
          Math.Max( maxPadding,
                    cellLtWidget.Widget.LayoutInfo.Paddings.Bottom +
                    cellLtWidget.Widget.LayoutInfo.Paddings.Top );

        // Checks if next cell not "IsRowMergeContinue"
        if( CurrRowIndex != TableWidget.RowsCount - 1 )
        {
          IWidgetContainer wc = TableWidget.GetCellWidget( CurrRowIndex + 1, i );
          LayoutTableInfo wcTableInfo = wc.LayoutInfo as LayoutTableInfo;
          nextCellContinue = wcTableInfo.IsRowMergeContinue;
        }

        if( currCellContinue && !nextCellContinue )
        {
          rowBounds.Height = (float)Math.Max( m_rowsHeight[ i ], rowBounds.Height );
        }
        
      }
      
      LayoutTableInfo rowTableInfo = m_currRowLW.Widget.LayoutInfo as LayoutTableInfo;
      
      rowTableInfo.RowHeight += (float)maxPadding;
      //rowBounds.Width = m_clientArea.Width;
      rowBounds.Width = m_layoutArea.ClientActiveArea.Width;
      // expansion row height to "fixed row height"
      if( rowTableInfo.IsExactlyRowHeight && rowTableInfo.RowHeight > 0 )
      {
        rowBounds.Height = ( float )rowTableInfo.RowHeight;
      }
      else if( !m_bHeaderRepeat && !rowTableInfo.IsRowSplitted )
      {
        rowBounds.Height = ( float )Math.Max( rowBounds.Height, rowTableInfo.RowHeight );
      }
      
      m_currRowLW.Bounds = rowBounds;
    }
    /// <summary>
    /// Updates the size of the cell.
    /// </summary>
    /// <param name="column">The column.</param>
    private void UpdateCellSize( int column )
    {
      LayoutTableInfo cellLI = TableWidget.GetCellWidget( m_currRowIndex, column ).LayoutInfo as LayoutTableInfo;
      
      if( !cellLI.IsColumnMergeContinue )
      {
        UpdateCellWidth( column );
        UpdateCellHeight( column );
      }
    }
    /// <summary>
    /// Updates the width of the cell.
    /// </summary>
    /// <param name="column">The column.</param>
    private void UpdateCellWidth( int column )
    {
      LayoutedWidget cellLtWidget = m_currRowLW.ChildWidgets[ column ];
      RectangleF bounds = cellLtWidget.Bounds;
      m_currColIndex = column;
      
      bounds.Width = (float)(m_columnsWidth[ column ] + GetCellMergedWidth());
//      if( column + 1 < m_currRowLW.ChildWidgets.Count )
//      {
//        LayoutedWidget next_cellLtWidget = m_currRowLW.ChildWidgets[ column + 1 ];
//        bounds.Width = next_cellLtWidget.Bounds.Left - bounds.Left;  
//        //NOTE: !!!MAYBE subtract next cell margins
//      }
//      else
//      {
//        bounds.Width = (float)(m_currRowLW.Bounds.Right - bounds.Left + cellLtWidget.Widget.LayoutInfo.Margins.Left);
//      }
      // Correct cell width mergins
      bounds.Width -= (float)(cellLtWidget.Widget.LayoutInfo.Margins.Left +
                              cellLtWidget.Widget.LayoutInfo.Margins.Right);
      cellLtWidget.Bounds = bounds;
    }
    /// <summary>
    /// Updates the height of the cell.
    /// </summary>
    /// <param name="column">The column.</param>
    private void UpdateCellHeight( int column )
    {
      LayoutedWidget cellLtWidget = m_currRowLW.ChildWidgets[ column ];
      
      RectangleF bounds = cellLtWidget.Bounds;
      RectangleF rowBounds = m_currRowLW.Bounds;

      bool currCellMergeContinue =
        ( cellLtWidget.Widget.LayoutInfo as LayoutTableInfo ).IsRowMergeContinue;
      bool nextCellMergeContinue = false;

      if( CurrRowIndex != TableWidget.RowsCount - 1 )
      {
        IWidgetContainer widget = TableWidget.GetCellWidget( CurrRowIndex + 1, column );
        nextCellMergeContinue = ( widget.LayoutInfo as LayoutTableInfo ).IsRowMergeContinue;
      }
      
      if( currCellMergeContinue && !nextCellMergeContinue )
      {
        UpdateContinueCellHeight( column );
      }

      // Correct cell height margins
      bounds.Height = rowBounds.Height - 
        (float)(cellLtWidget.Widget.LayoutInfo.Margins.Top + 
        cellLtWidget.Widget.LayoutInfo.Margins.Bottom);
      cellLtWidget.Bounds = bounds;
      
      double rowHeight = m_rowsHeight[ column ] - rowBounds.Height;
      m_rowsHeight[ column ] = ( rowHeight > 0 ) ? rowHeight : 0;
    }
    /// <summary>
    /// Updates the height of the continue cell.
    /// </summary>
    /// <param name="column">The column.</param>
    private void UpdateContinueCellHeight( int column )
    {
      int rowIndex = CurrRowIndex - 1;
      RectangleF rowBounds = m_currRowLW.Bounds;

      while( rowIndex > -1 )
      {
        LayoutTableInfo last_widgetTableInfo = TableWidget.GetCellWidget( rowIndex, column ).LayoutInfo as LayoutTableInfo;

        if( last_widgetTableInfo.IsRowMergeStart && m_ltWidget.ChildWidgets.Count > rowIndex )
        {
          if( m_ltWidget.ChildWidgets.Count == 0 )
          {
            break;
          }
          LayoutedWidget last_cellLtWidget = m_ltWidget.ChildWidgets[ rowIndex ].ChildWidgets[ column ];
          
          RectangleF cell_bounds = last_cellLtWidget.Bounds;
          cell_bounds.Height = rowBounds.Bottom - last_cellLtWidget.Bounds.Top;
          // Correct cell mergins          
          cell_bounds.Height -= (float)last_cellLtWidget.Widget.LayoutInfo.Margins.Bottom;
          last_cellLtWidget.Bounds = cell_bounds;
          break;
        }
        rowIndex--;
      }
    }
    /// <summary>
    /// Deletes the continuous cells.
    /// </summary>
    private void DeleteContinuousCells()
    {
      for( int i = 0, cnt = m_ltWidget.ChildWidgets.Count; i < cnt; i++ )
      {
        LayoutedWidget lw = m_ltWidget.ChildWidgets[ i ];
        
        for( int j = lw.ChildWidgets.Count - 1; j > -1; j-- )
        {
          LayoutedWidget cellLW = lw.ChildWidgets[ j ];
          
          LayoutTableInfo tableInfo = cellLW.Widget.LayoutInfo as LayoutTableInfo;
          bool colMergeCont = tableInfo.IsColumnMergeContinue;
          
          if( tableInfo.IsRowMergeContinue || colMergeCont )
          {
            if( SearchedMergeStart( i, j ) || colMergeCont )
            {
              m_ltWidget.ChildWidgets[ i ].ChildWidgets.RemoveAt( j );
            }
          }
          else if( cellLW.ChildWidgets.Count > 0 )
          {
            // Update cell vertical alignment
            RectangleF bounds = cellLW.Bounds;
            LayoutedWidget child = cellLW.ChildWidgets[ 0 ];
            float displacement = 0;
      
            switch( ( child.Widget.LayoutInfo as LayoutTableInfo ).VerticalAlignment )
            {
              case 1:          
                displacement = ( bounds.Height - child.Bounds.Height ) / 2;
                break;
              case 2:
                displacement = bounds.Height - child.Bounds.Height;
                break;
            }
      
            child.ShiftLocation( 0, displacement );
            child.Bounds = RectangleF.Empty;
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    private void CreateTableClientArea( RectangleF rect )
    {
      ITableLayoutInfo info = TableLayoutInfo;
#if FIXED_TABLE_WIDTH      
      float fullWidth = (float)(info.Width
                        + LayoutInfo.Paddings.Left
                        + LayoutInfo.Paddings.Right
                        + LayoutInfo.Margins.Left
                        + LayoutInfo.Margins.Right);  
      if( fullWidth > 0 && fullWidth < rect.Width )
         rect.Width = fullWidth;
#endif

#if !FIXED_TABLE_WIDTH
      CorrectTableClientArea( ref rect );      
#endif

      if( TableLayoutInfo.Height > 0 && info.Height < rect.Height )
        rect.Height = info.Height;

      CreateLayoutArea( rect );
    }
    /// <summary>
    /// Does the layout cell.
    /// </summary>
    /// <param name="childContext">The child context.</param>
    /// <param name="cellArea">The cell area.</param>
    private void DoLayoutCell( LayoutContext childContext, RectangleF cellArea )
    {
      ( childContext.LayoutInfo as LayoutTableInfo ).IsExactlyRowHeight =
        ( m_currRowLW.Widget.LayoutInfo as LayoutTableInfo ).IsExactlyRowHeight;
      LayoutedWidget child = childContext.Layout( cellArea );
      LayoutedWidget lw = new LayoutedWidget( childContext.Widget, child.Bounds.Location );
      lw.ChildWidgets.Add( child );
      lw.Bounds = child.Bounds;
      m_currCellLW = lw;
    }
    /// <summary>
    /// Gets the width of the merged cell.
    /// </summary>
    /// <returns></returns>
    private double GetCellMergedWidth()
    {
      double cellWidth = 0;
      int colIndex = m_currColIndex + 1;

      while( colIndex < TableWidget.ColumnsCount )
      {
        IWidgetContainer widget = TableWidget.GetCellWidget( CurrRowIndex, colIndex );

        if( ( widget.LayoutInfo as LayoutTableInfo ).IsColumnMergeContinue )
        {
          cellWidth += m_columnsWidth[ colIndex ];
        }
        else
        {
          break;
        }

        colIndex++;
      }

      return cellWidth;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private LayoutContext CreateNextCellContext()
    {
      if( m_currColIndex + 1 < TableWidget.ColumnsCount )
      {
        m_currColIndex++;
        IWidgetContainer widget = null;

        if( m_spitTableWidget != null &&
          m_spitTableWidget.SplittedCells != null &&
          CurrRowIndex == m_spitTableWidget.StartRowNumber - 1 )
        {
          widget = m_spitTableWidget.SplittedCells[ m_currColIndex ];
        }
        
        if( widget == null )
        {
          widget = TableWidget.GetCellWidget( CurrRowIndex, m_currColIndex );
        }
        
        return LayoutContext.Create( widget, m_lcOperator );
      }

      return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="childContext"></param>
    private void CommitCellContext( LayoutContext childContext )
    {
      switch( childContext.State )
      {
        case LayoutState.Unknown:
          //m_ltState = LayoutState.Fitting;
          m_currRowLW.ChildWidgets.Add( m_currCellLW );
          m_bAtLastOneCellFitted = true;
          break;
        case LayoutState.NotFitted:
          CommitForNotFitted( childContext );
          break;
        case LayoutState.Splitted:
          CommitForSplited( childContext );
          break;
          //case LayoutState.Fitting:
        case LayoutState.Fitted:
          CommitForFitted( childContext );
          break;
      }
    }
    /// <summary>
    /// Commits for splited.
    /// </summary>
    /// <param name="childContext">The child context.</param>
    private void CommitForSplited( LayoutContext childContext )
    {
      LayoutTableInfo tableInfo = m_currRowLW.Widget.LayoutInfo as LayoutTableInfo;
        
      if( tableInfo.IsExactlyRowHeight )
      {
        CommitForFitted( childContext );
        m_ltState = LayoutState.Unknown;
      }
      else
      {
        ( m_currCellLW.Widget.LayoutInfo as LayoutTableInfo ).IsRowSplitted = true;
        tableInfo.IsRowSplitted = true;
        CommitForFitted( childContext );
        
        if( m_blastRowState == LayoutState.Unknown )
        {
          m_blastRowState = LayoutState.Splitted;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    private void CommitForNotFitted( LayoutContext childContext )
    {
      if( childContext.IsVerticalNotFitted )
      {
        LayoutTableInfo tableInfo = m_currRowLW.Widget.LayoutInfo as LayoutTableInfo;
        
        if( tableInfo.IsExactlyRowHeight )
        {
          if( m_currCellLW.Bounds.Height == 0 )
          {
            RectangleF bounds = m_currCellLW.Bounds;
            bounds.Height = ( float )Math.Abs( tableInfo.RowHeight );
            m_currCellLW.Bounds = bounds;
          }
          
          CommitForFitted( childContext );
          //m_ltState = LayoutState.Fitted;
        }
        else
        {
          m_ltState = LayoutState.NotFitted;
        }
      }
      else
      {
        if( m_currRowIndex < ( TableWidget.RowsCount - 1 ) )
        {
          //CommitForFitted( childContext );
          CommitForSplited( childContext );
          m_ltState = LayoutState.NotFitted;
        }
        else
        {
          //m_bIsHorizontalNotFitted = true;
          m_ltState = LayoutState.NotFitted;
          //Trace.WriteLine( "last row" );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    private void CommitForFitted( LayoutContext childContext )
    {
      if( ! (m_currRowLW.Widget.LayoutInfo as LayoutTableInfo ).IsExactlyRowHeight )
      {
        m_splitedCells[ m_currColIndex ] = ( ( childContext.SplittedWidget as SplitWidgetContainer ) != null )
                                             ? childContext.SplittedWidget as SplitWidgetContainer
                                             : new SplitWidgetContainer( childContext.Widget as IWidgetContainer );
        ( m_splitedCells[ m_currColIndex ].LayoutInfo as LayoutTableInfo ).IsRowSplitted = true;
      }
      
      m_currRowLW.ChildWidgets.Add( m_currCellLW );
      LayoutTableInfo tableInfo = m_currCellLW.Widget.LayoutInfo as LayoutTableInfo;
      
      if( tableInfo.IsRowMergeStart )
      {
        m_rowsHeight[ m_currColIndex ] = m_currCellLW.Bounds.Height
                                         + tableInfo.Margins.Bottom
                                         + tableInfo.Paddings.Bottom
                                         + tableInfo.Paddings.Top;
      }
      else if( !tableInfo.IsRowMergeContinue )
      {
        UpdateRowLWBounds( childContext );
      }

      //m_ltState = LayoutState.Fitting;
      m_bAtLastOneCellFitted = true;
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateClientArea()
    {
      m_layoutArea.CutFromTop( m_currRowLW.Bounds.Bottom );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private LayoutArea GetCellClientArea( bool horMergeStart )
    {
      RectangleF rect = m_layoutArea.ClientActiveArea;//m_clientArea.OuterArea;
      double cellX = rect.X + m_columnsPosX[ m_currColIndex ]; //- m_skipOffset;
      double cellY = rect.Y;
      double cellWidth = m_columnsWidth[ m_currColIndex ];
      double cellHeight = rect.Height;

      if( horMergeStart )
      {
        cellWidth += GetCellMergedWidth();
      }
      
      LayoutTableInfo rowTableInfo = m_currRowLW.Widget.LayoutInfo as LayoutTableInfo;
      
      bool isExactlyRowHeight = rowTableInfo.IsExactlyRowHeight;
      
      if( isExactlyRowHeight )
      {
        // TODO: add cell spacing
        cellHeight = rowTableInfo.RowHeight;
      }
      else      
      {
        if( cellHeight < rowTableInfo.RowHeight )
        {
          cellHeight = 0;
        } 
      }

      RectangleF cellRect =
        new RectangleF( ( float )cellX, ( float )cellY, ( float )cellWidth, ( float )cellHeight );
      
      return new LayoutArea( cellRect );
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateLWBounds()
    {
      RectangleF bounds = m_ltWidget.Bounds;
      bounds.Width = m_currRowLW.Bounds.Width +
        ( float )( TableWidget.LayoutInfo.Paddings.Left +
        TableWidget.LayoutInfo.Paddings.Right );
      bounds.Height = m_currRowLW.Bounds.Bottom - bounds.Top
                      + ( float )TableWidget.LayoutInfo.Paddings.Bottom;
      m_ltWidget.Bounds = bounds;
    }
    /// <summary>
    /// Searcheds the merge start.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="coll">The collection.</param>
    /// <returns></returns>
    private bool SearchedMergeStart( int row, int coll )
    {
      bool next = row > 0;
      
      while( next )
      {
        LayoutedWidget rowLW = m_ltWidget.ChildWidgets[ row - 1 ];

        if( rowLW.ChildWidgets.Count - 1 < coll
            || ( rowLW.ChildWidgets[ coll ].Widget.LayoutInfo as LayoutTableInfo ).IsRowMergeStart )
        {
          return true;
        }

        row--;
        next = row > 0;
      }
      
      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    private void CorrectTableClientArea( ref RectangleF rect )
    {
      ITableLayoutInfo info = TableLayoutInfo;

      bool isFixed = false;
      float fixedSize = 0;
      int fixedCount = 0;

      for( int i = 0, count = info.IsDefaultCells.Length; i < count; i++ )
      {
        if( info.IsDefaultCells[ i ] )
        {
          isFixed |= true;
          fixedSize += info.CellsWidth[ i ];
          fixedCount++;
        }
      }

      if( !isFixed && info.Width > rect.Width )
      {
        info.Width = rect.Width - ( float )info.CellSpasings - ( float )info.CellPaddings;
      }
      else
      {
        if( fixedCount == TableWidget.ColumnsCount )
        {
          info.Width = 0;

          for( int i = 0; i < fixedCount; i++ )
          {
            info.Width += info.CellsWidth[i];
          }
        }

      }

      rect.Width = info.Width;

      float cellWidth = ( info.Width - fixedSize ) / ( info.CellsWidth.Length - fixedCount );

      for( int i = 0, count = info.IsDefaultCells.Length; i < count; i++ )
      {
        if( !info.IsDefaultCells[ i ] )
        {
          info.CellsWidth[ i ] = cellWidth;
        }
      }
    }
    #endregion
  }
}