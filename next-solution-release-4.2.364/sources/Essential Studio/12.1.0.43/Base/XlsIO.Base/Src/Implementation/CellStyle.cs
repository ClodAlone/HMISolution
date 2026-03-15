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

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Collections;

#endregion

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Summary description for CellStyle.
	/// </summary>
	public class CellStyle
    : ExtendedFormatWrapper
    , IStyle
	{
    #region Class members
    /// <summary>
    /// Parent range.
    /// </summary>
    private RangeImpl m_range;
    /// <summary>
    /// Represents whether to get adjacent or not
    /// </summary>
    private bool m_bAskAdjacent = true;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of cell style.
    /// </summary>
    /// <param name="range">Parent range.</param>
    public CellStyle( RangeImpl range )
      : base( range.Workbook )
    {
      m_range = range;
    }

    /// <summary>
    /// Creates new instance of cell style.
    /// </summary>
    /// <param name="range">Parent range.</param>
    /// <param name="iXFIndex">Index of extended format to wrap.</param>
    public CellStyle( RangeImpl range, int iXFIndex )
      : base( range.Workbook, iXFIndex )
    {
      m_range = range;
    }

    #endregion

    #region Class overrides
    /// <summary>
    /// This method is called before changes in extended format.
    /// </summary>
    public override void BeginUpdate()
    {
      if( BeginCallsCount == 0 )
      {
        BeforeRead();
        m_xFormat = m_book.CreateExtFormatWithoutRegister( m_xFormat );
      }

      base.BeginUpdate();
    }
    /// <summary>
    /// This method is called after changes in extended format.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
         m_xFormat=m_book.AddExtendedProperties(m_xFormat);

        if (!(m_xFormat.Index > m_book.DefaultXFIndex && m_book.m_xfCellCount.ContainsKey(m_xFormat.Index)))
        {                
            m_xFormat = m_book.RegisterExtFormat(m_xFormat);
            m_range.ExtendedFormatIndex = (ushort)m_xFormat.Index;
        }
        else
        {
            int CellsCount;
            m_book.m_xfCellCount.TryGetValue(m_xFormat.Index, out CellsCount);

            if (CellsCount == 1)
            {                
                m_book.InnerExtFormats[m_xFormat.Index].UpdateFromCurrentExtendedFormat(m_xFormat);
                m_range.ExtendedFormatIndex = (ushort)m_xFormat.Index;
            }
            else
            {
                m_book.m_xfCellCount[m_xFormat.Index] = CellsCount - 1;
                    
                m_xFormat = m_book.RegisterExtFormat(m_xFormat);
                m_range.ExtendedFormatIndex = (ushort)m_xFormat.Index;
            }
        }
      }
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    protected override void SetParents( object parent )
    {
      m_range = CommonObject.FindParent( parent, typeof( RangeImpl ) ) as RangeImpl;

      if( m_range == null )
        throw new ArgumentNullException( "parent", "Can't find parent range." );

      m_book = m_range.Workbook;
    }

    /// <summary>
    /// This method is called before reading any value. Can be used
    /// to update wrapped object before read operation.
    /// </summary>
    protected override void BeforeRead()
    {
      if( BeginCallsCount == 0 )
      {
        base.BeforeRead();
        SetFormatIndex( m_range.ExtendedFormatIndex );
      }
    }
    /// <summary>
    /// Get/set LeftBorder color.
    /// </summary>
    public override ColorObject LeftBorderColor
    {
      get
      {
        return GetLeftBorderColor( AskAdjacent );
      }
    }
    /// <summary>
    /// Get/set RightBorder color.
    /// </summary>
    public override ColorObject RightBorderColor
    {
      get
      {
        return GetRightBorderColor( AskAdjacent );
      }
    }
    /// <summary>
    /// Get/set TopBorder color.
    /// </summary>
    public override ColorObject TopBorderColor
    {
        get
        {
            return GetTopBorderColor(AskAdjacent);
        }
    }
    /// <summary>
    /// Get/set BottomBorder color.
    /// </summary>
    public override ColorObject BottomBorderColor
    {
        get
        {
            return GetBottomBorderColor(AskAdjacent);
        }
    }
    /// <summary>
    /// Gets / sets line style of the left border.
    /// </summary>
    public override ExcelLineStyle LeftBorderLineStyle
    {
      get
      {
        return GetLeftLineStyle( AskAdjacent );
      }
      set
      {
        if (GetLeftLineStyle(false) != value)
           base.LeftBorderLineStyle = value;
      }
    }
    /// <summary>
    /// Gets / sets line style of the right border.
    /// </summary>
    public override ExcelLineStyle RightBorderLineStyle
    {
      get
      {
        return GetRightLineStyle( AskAdjacent );
      }
      set
      {
        if (GetRightLineStyle(false) != value)
            base.RightBorderLineStyle = value;
      }
    }
    /// <summary>
    /// Gets / sets line style of the top border.
    /// </summary>
    public override ExcelLineStyle TopBorderLineStyle
    {
      get
      {
        return GetTopLineStyle( AskAdjacent );
      }
      set
      {
        if (GetTopLineStyle(false) != value)
            base.TopBorderLineStyle = value;
      }
    }
    /// <summary>
    /// Gets / sets line style of the top border.
    /// </summary>
    public override ExcelLineStyle BottomBorderLineStyle
    {
      get
      {
        return GetBottomLineStyle( AskAdjacent );
      }
      set
      {
        if (GetBottomLineStyle(false) != value)
            base.BottomBorderLineStyle = value;
      }
    }
    /// <summary>
    /// Gets or sets a boolean value 
    /// whether to get adjacent or not
    /// </summary>
    internal bool AskAdjacent
    {
        get
        {
            return m_bAskAdjacent;
        }
        set
        {
            m_bAskAdjacent = value;
        }
    }
    /// <summary>
    /// Gets line style of the left border.
    /// </summary>
    /// <param name="askAdjecent"></param>
    /// <returns></returns>
    protected ExcelLineStyle GetLeftLineStyle( bool askAdjecent )
    {
      RangeImpl actualRange = null;

      if (m_range.IsMerged && askAdjecent)
      {
          actualRange = m_range;
          IRange[] mergedCells = this.m_range.MergeArea.Cells;
          m_range = (mergedCells[0] as RangeImpl);
      }

      ExcelLineStyle result = base.LeftBorderLineStyle;

      if( result == ExcelLineStyle.None && askAdjecent )
      {
        IRange leftCell = GetLeftCell();

        if (leftCell != null && leftCell.Columns[0].ColumnWidth != 0)
          result = ( leftCell.CellStyle as CellStyle ).GetRightLineStyle( false );
      }

      if (actualRange != null)
          m_range = actualRange;

      return result;
    }
    /// <summary>
    /// Gets line style of the right border.
    /// </summary>
    /// <param name="askAdjecent"></param>
    /// <returns></returns>
    protected ExcelLineStyle GetRightLineStyle( bool askAdjecent )
    {
      RangeImpl actualRange = null;

      if (m_range.IsMerged && askAdjecent)
      {
          actualRange = m_range;
          IRange[] mergedCells = this.m_range.MergeArea.Cells;
          m_range = (mergedCells[mergedCells.Length - 1] as RangeImpl);
      }

      ExcelLineStyle result = base.RightBorderLineStyle;

      if( result == ExcelLineStyle.None && askAdjecent )
      {
        IRange cell = GetRightCell();

        if (cell != null && cell.Columns[0].ColumnWidth != 0)
          result = ( cell.CellStyle as CellStyle ).GetLeftLineStyle( false );
      }

      if (actualRange != null)
          m_range = actualRange;

      return result;
    }
    /// <summary>
    /// Gets line style of the top border.
    /// </summary>
    /// <param name="askAdjecent"></param>
    /// <returns></returns>
    protected ExcelLineStyle GetTopLineStyle( bool askAdjecent )
    {
      RangeImpl actualRange = null;

      if (m_range.IsMerged && askAdjecent)
      {
          actualRange = m_range;
          IRange[] mergedCells = this.m_range.MergeArea.Cells;
          m_range = (mergedCells[0] as RangeImpl);
      }

      ExcelLineStyle result = base.TopBorderLineStyle;

      if( result == ExcelLineStyle.None && askAdjecent )
      {
        IRange cell = GetTopCell();

        if (cell != null && cell.Rows[0].RowHeight != 0)
          result = ( cell.CellStyle as CellStyle ).GetBottomLineStyle( false );
      }

      if (actualRange != null)
          m_range = actualRange;

      return result;
    }
    /// <summary>
    /// Get line style of the bottom border.
    /// </summary>
    /// <param name="askAdjecent"></param>
    /// <returns></returns>
    protected ExcelLineStyle GetBottomLineStyle( bool askAdjecent )
    {
      RangeImpl actualRange = null;

      if (m_range.IsMerged && askAdjecent)
      {
          actualRange = m_range;
          IRange[] mergedCells = this.m_range.MergeArea.Cells;
          m_range = (mergedCells[mergedCells.Length - 1] as RangeImpl);
      }

      ExcelLineStyle result = base.BottomBorderLineStyle;

      if( result == ExcelLineStyle.None && askAdjecent )
      {
        IRange cell = GetBottomCell();

        if( cell != null && cell.Rows[0].RowHeight != 0)
          result = ( cell.CellStyle as CellStyle ).GetTopLineStyle( false );
      }

      if (actualRange != null)
          m_range = actualRange;

      return result;
    }
    /// <summary>
    /// Gets line color of the left border.
    /// </summary>
    /// <param name="askAdjecent"></param>
    /// <returns></returns>
    protected ColorObject GetLeftBorderColor(bool askAdjacent)
    {
        ColorObject result = base.LeftBorderColor;
        ExcelLineStyle lineStyle = base.LeftBorderLineStyle;

        if (lineStyle == ExcelLineStyle.None && askAdjacent)
        {
            IRange leftCell = GetLeftCell();

            if (leftCell != null)
                result = (leftCell.CellStyle as CellStyle).GetRightBorderColor(false);
        }

        return result;
    }
    /// <summary>
    /// Gets line color of the right border.
    /// </summary>
    /// <param name="askAdjecent"></param>
    /// <returns></returns>
    protected ColorObject GetRightBorderColor(bool askAdjacent)
    {
        ColorObject result = base.RightBorderColor;
        ExcelLineStyle lineStyle = base.RightBorderLineStyle;

        if (lineStyle == ExcelLineStyle.None && askAdjacent)
        {
            IRange cell = GetRightCell();

            if (cell != null)
                result = (cell.CellStyle as CellStyle).GetLeftBorderColor(false);
        }

        return result;
    }
    /// <summary>
    /// Gets line color of the top border.
    /// </summary>
    /// <param name="askAdjecent"></param>
    /// <returns></returns>
    protected ColorObject GetTopBorderColor(bool askAdjecent)
    {
        ColorObject result = base.TopBorderColor;
        ExcelLineStyle lineStyle = base.TopBorderLineStyle;

        if (lineStyle == ExcelLineStyle.None && askAdjecent)
        {
            IRange cell = GetTopCell();

            if (cell != null)
                result = (cell.CellStyle as CellStyle).GetBottomBorderColor(false);
        }

        return result;
    }
    /// <summary>
    /// Get line color of the bottom border.
    /// </summary>
    /// <param name="askAdjecent"></param>
    /// <returns></returns>
    protected ColorObject GetBottomBorderColor(bool askAdjecent)
    {
        ColorObject result = base.BottomBorderColor;
        ExcelLineStyle lineStyle = base.BottomBorderLineStyle;

        if (lineStyle == ExcelLineStyle.None && askAdjecent)
        {
            IRange cell = GetBottomCell();

            if (cell != null)
                result = (cell.CellStyle as CellStyle).GetTopBorderColor(false);
        }

        return result;
    }
    /// <summary>
    /// Gets left adjecent cell.
    /// </summary>
    /// <returns></returns>
    private IRange GetLeftCell()
    {
      return GetCell( 0, -1 );
    }
    /// <summary>
    /// Gets rigth adjecent cell.
    /// </summary>
    /// <returns></returns>
    private IRange GetRightCell()
    {
      return GetCell( 0, 1 );
    }
    /// <summary>
    /// Gets top adjecent cell.
    /// </summary>
    /// <returns></returns>
    private IRange GetTopCell()
    {
      return GetCell( -1, 0 );
    }
    /// <summary>
    /// Gets bottom adjecent cell.
    /// </summary>
    /// <returns></returns>
    private IRange GetBottomCell()
    {
      return GetCell( 1, 0 );
    }
    /// <summary>
    /// Gets relative cell.
    /// </summary>
    /// <param name="rowDelta"></param>
    /// <param name="colDelta"></param>
    /// <returns></returns>
    private IRange GetCell( int rowDelta, int colDelta )
    {
      int iRow = m_range.Row + rowDelta;
      int iColumn = m_range.Column + colDelta;

      IRange result = null;

      if( iRow > 0 && iRow <= m_book.MaxRowCount && iColumn > 0 && iColumn <= m_book.MaxColumnCount )
        result = m_range[ iRow, iColumn ];

      return result;
    }
    #endregion
  }
}
