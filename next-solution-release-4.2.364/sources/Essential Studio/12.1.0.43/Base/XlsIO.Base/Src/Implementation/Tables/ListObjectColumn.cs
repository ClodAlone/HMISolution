#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.Tables
{
  class ListObjectColumn : IListObjectColumn
  {
    #region Constants
    private const string SubTotalFormat = "=SUBTOTAL({0}{1}[{2}])";
    #endregion

    #region Members
    private string m_strName;
    private int m_iIndex;
    private ExcelTotalsCalculation m_totals;
    private string m_strTotalsLabel;
    private ListObject m_parentTable;
    private int m_iId;
    private string m_strCalculatedFormula;
    private int m_queryTableFieldId;
    #endregion

    #region IListObjectColumn Members
    /// <summary>
    /// Gets or sets name of the column.
    /// </summary>
    public string Name
    {
      get
      {
         IRange tableRange = m_parentTable.Location;
         int iRow = tableRange.Row;
         int iColumn = GetColumnIndex(tableRange);
         m_strName = m_parentTable.Worksheet[iRow,iColumn].DisplayText;
         return m_strName;
      }
      set
      {
          IRange tableRange = m_parentTable.Location;
          int iRow = tableRange.Row;
          int iColumn = GetColumnIndex(tableRange);
          m_parentTable.Worksheet[iRow,iColumn].Text = value;
      }
    }
    /// <summary>
    /// Gets column index.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
    }
    /// <summary>
    /// Gets or sets function used for totals calculation.
    /// </summary>
    public ExcelTotalsCalculation TotalsCalculation
    {
      get
      {
        return m_totals;
      }
      set
      {
        m_totals = value;

        IRange totalCell = TotalCell;
        WorkbookImpl book = totalCell.Worksheet.Workbook as WorkbookImpl;

        if( !book.Loading )
        {
          string strComma = book.ArgumentsSeparator;

          if( value != ExcelTotalsCalculation.None )
          {
            string formulaString = m_strName;

            bool bThrowException = book.ThrowOnUnknownNames;
            book.ThrowOnUnknownNames = false;

            formulaString = formulaString.Replace("'", "''");
            formulaString = formulaString.Replace("[", "'[");
            formulaString = formulaString.Replace("]", "']");

            totalCell.Formula = string.Format(SubTotalFormat, (int)value, strComma, formulaString);
            book.ThrowOnUnknownNames = bThrowException;
          }
          else
          {
            totalCell.Value = string.Empty;
          }
        }
      }
    }

    public string TotalsRowLabel
    {
      get
      {
        return m_strTotalsLabel;
      }
      set
      {
        m_strTotalsLabel = value;

        if( !Workbook.Loading )
          TotalCell.Value = value;
      }
    }
    private IRange TotalCell
    {
      get
      {
        IRange location = m_parentTable.Location;
        return location.Worksheet[ location.LastRow, location.Column + m_iIndex - 1 ];
      }
    }
    /// <summary>
    /// Gets or sets ccolumn id.
    /// </summary>
    public int Id
    {
      get
      {
        return m_iId;
      }
      set
      {
        m_iId = value;
      }
    }
    /// <summary>
    /// Gets calculated formula value.
    /// </summary>
    public string CalculatedFormula
    {
      get
      {
        return m_strCalculatedFormula;
      }
      set
      {
        m_strCalculatedFormula = value;
      }
    }
    /// <summary>
    /// Gets parent workbook.
    /// </summary>
    private WorkbookImpl Workbook
    {
      get
      {
        return m_parentTable.Worksheet.Workbook as WorkbookImpl;
      }
    }
    public int QueryTableFieldId
    {
        get
        {
            return m_queryTableFieldId;
        }
        set
        {
            m_queryTableFieldId = value;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="name">Column name.</param>
    /// <param name="index">Column index.</param>
    /// <param name="parentTable">Parent table object.</param>
    public ListObjectColumn( string name, int index, ListObject parentTable, int id )
    {
      if( parentTable == null )
        throw new ArgumentNullException( "parentTable" );

      m_strName = name;
      m_iIndex = index;
      m_parentTable = parentTable;
      m_iId = id;
    }
    /// <summary>
    /// Find Column Index.
    /// </summary>
    /// <param name="range">Listobject Range.</param>
    private int GetColumnIndex(IRange range)
    {
        int iColumn = range.Column;
        return (iColumn + m_iIndex - 1);
    }
    /// <summary>
    /// Creates a copy of the current column.
    /// </summary>
    /// <param name="parentTable">New parent table for the column</param>
    /// <returns></returns>
    public ListObjectColumn Clone( ListObject parentTable )
    {
      if( parentTable == null )
        throw new ArgumentNullException( "parentTable" );

      ListObjectColumn result = ( ListObjectColumn )MemberwiseClone();
      result.m_parentTable = parentTable;
      return result;
    }
    #endregion
  }
}
