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

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Shapes;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class represents single autofilter item.
  /// </summary>
  public class AutoFilterImpl
    : /*CommonObject
    ,*/ IAutoFilter
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// First condition of autofilter.
    /// </summary>
    private AutoFilterConditionImpl m_firstCondition;
    /// <summary>
    /// Second condition of autofilter.
    /// </summary>
    private AutoFilterConditionImpl m_secondCondition;
    /// <summary>
    /// Auto filter record.
    /// </summary>
    private AutoFilterRecord m_record;
    /// <summary>
    /// Form control shape implementation.
    /// </summary>
    private FormControlShapeImpl m_shape;
    /// <summary>
    /// Parent collection.
    /// </summary>
    private AutoFiltersCollection m_autofilters;
   
    internal int m_colIndex;
    internal double m_filterValue;
    private Dictionary<IRange, double> m_rangeDict = new Dictionary<IRange, double>();
    private List<KeyValuePair<IRange, double>> m_rangeList = new List<KeyValuePair<IRange, double>>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="parent">Parent filters collection.</param>
    public AutoFilterImpl( AutoFiltersCollection parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      InitializeConditions(parent);
      m_record = ( AutoFilterRecord )BiffRecordFactory.GetRecord( TBIFFRecord.AutoFilter );
      //SetParents();
      m_autofilters = parent;
    }
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="parent">Parent filters collection.</param>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="iRowIndex">Row index.</param>
    public AutoFilterImpl( AutoFiltersCollection parent,
      int iColumnIndex, int iLastColumn, int iRowIndex )
      : this( parent )
    {
      m_shape = WorksheetShapes.AddFormControlShape();
      //m_shape.UpdatePositions = false;

      m_shape.LeftColumn = iColumnIndex;
      m_shape.RightColumn = iLastColumn + 1;
      m_shape.TopRow = iRowIndex;
      m_shape.BottomRow = iRowIndex + 1;

      //m_shape.EvaluateTopLeftPosition();

      //if( m_shape.IsSizeWithCell )
      //{
      //  m_shape.UpdateHeight();
      //  m_shape.UpdateWidth();
      //}

      //m_shape.UpdatePositions = true;
    }
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="parent">Parent filters collection.</param>
    /// <param name="record">Base record.</param>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="iTopRow">Row index.</param>
    [ CLSCompliant( false ) ]
    public AutoFilterImpl( AutoFiltersCollection parent,
      AutoFilterRecord record, int iColumnIndex, int iTopRow )
      //: base( application, parent )
    {
      InitializeConditions(parent);
      Parse( record, iColumnIndex, iTopRow );
      m_autofilters = parent;
    }
    /// <summary>
    /// Creates new instances of condition variables.
    /// </summary>
    private void InitializeConditions(AutoFiltersCollection parent)
    {
        m_firstCondition = new AutoFilterConditionImpl(parent);
        m_secondCondition = new AutoFilterConditionImpl(parent);
    }
    #endregion

    #region IAutoFilter Members
    /// <summary>
    /// First condition of autofilter. Read-only.
    /// </summary>
    public IAutoFilterCondition FirstCondition
    {
      get
      {
        return m_firstCondition;
      }
    }

    /// <summary>
    /// Second condition of autofilter. Read-only.
    /// </summary>
    public IAutoFilterCondition SecondCondition
    {
      get
      {
        return m_secondCondition;
      }
    }

    /// <summary>
    /// False indicates that this autofilter was not used; otherwise True.
    /// </summary>
    public bool IsFiltered
    {
      get
      {
        if( IsTop10 && Top10Number > 0 || IsSimple1 || IsSimple2)
        {
          return true;
        }

        if( FirstCondition.DataType == ExcelFilterDataType.NotUsed
          && SecondCondition.DataType == ExcelFilterDataType.NotUsed )
        {
          return false;
        }

        return true;
      }
    }
    /// <summary>
    /// True means to use AND operation between conditions,
    /// False to use OR.
    /// </summary>
    public bool IsAnd
    {
      get
      {
        return m_record.IsAnd;
      }
      set
      {
        m_record.IsAnd = value;
      }
    }
    /// <summary>
    /// True if the Top 10 AutoFilter shows percentage;
    /// False if it shows items.
    /// </summary>
    public bool IsPercent
    {
      get
      {
        return m_record.IsPercent;
      }
      set
      {
        m_record.IsPercent = value;
      }
    }

    /// <summary>
    /// True if the first condition is a simple equality.
    /// </summary>
    public bool IsSimple1
    {
      get
      {
        return m_record.IsSimple1;
      }
      set
      {
        m_record.IsSimple1 = value;
      }
    }
    /// <summary>
    /// True if the second condition is a simple equality.
    /// </summary>
    public bool IsSimple2
    {
      get
      {
        return m_record.IsSimple2;
      }
      set
      {
        m_record.IsSimple2 = value;
      }
    }
    /// <summary>
    /// True if the Top 10 AutoFilter shows the top items;
    /// False if it shows the bottom items.
    /// </summary>
    public bool IsTop
    {
      get
      {
        return m_record.IsTop;
      }
      set
      {
        m_record.IsTop = value;
      }
    }
    /// <summary>
    /// True if the condition is a Top 10 AutoFilter.
    /// </summary>
    public bool IsTop10
    {
      get
      {
        return m_record.IsTop10;
      }
      set
      {
        m_record.IsTop10 = value;
      }
    }
    /// <summary>
    /// Number of elements to show in Top10 mode.
    /// </summary>
    public int Top10Number
    {
      get
      {
        return m_record.Top10Number;
      }
      set
      {
        m_record.Top10Number = value;
        SelectRangesToFilter();
        SetTop10();
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns parent worksheet. Read-only.
    /// </summary>
    public WorksheetImpl Worksheet
    {
      get
      {
        return m_autofilters.Worksheet;
      }
    }
    /// <summary>
    /// Returns shapes collection of the parent worksheet. Read-only.
    /// </summary>
    public ShapesCollection WorksheetShapes
    {
      get
      {
        return ( ShapesCollection )Worksheet.Shapes;
      }
    }
    /// <summary>
    /// Number of AutoFilter drop-down arrows on the sheet. ( One based ) 
    /// </summary>
    public int Index
    {
      get
      {
        return m_record.Index + 1;
      }
      set
      {
        m_record.Index = ( ushort )value;
      }
    }
    /// <summary>
    /// If true - than contain non default first condition; otherwise false. Read-only.
    /// </summary>
    public bool IsFirstCondition
    {
      get
      {
        return (!IsTop10 && m_firstCondition.DataType != ExcelFilterDataType.NotUsed
          && m_firstCondition.DataType != ExcelFilterDataType.MatchAllBlanks);
      }
    }
    /// <summary>
    /// If true - than contain non default second condition; otherwise false. Read-only.
    /// </summary>
    public bool IsSecondCondition
    {
      get
      {
        return !IsTop10 && m_secondCondition.DataType != ExcelFilterDataType.NotUsed;
      }
    }
    /// <summary>
    /// If filtered to blanks - true. Read-only.
    /// </summary>
    public bool IsBlanks
    {
      get
      {
        return m_record.IsBlank;
      }
    }
    /// <summary>
    /// If filtered to nonblanks - true. Read-only.
    /// </summary>
    public bool IsNonBlanks
    {
      get
      {
        return m_record.IsNonBlank;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Clears content of the autofilter (shape corresponding to autofilter).
    /// </summary>
    public void Clear()
    {
      if( m_shape != null )
      {
        m_shape.Remove();
        m_shape = null;
      }
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns just created object.</returns>
    public object Clone( object parent )
    {
      AutoFiltersCollection collection = ( AutoFiltersCollection )CommonObject.FindParent(
        parent, typeof( AutoFiltersCollection ) );
      AutoFilterImpl result = new AutoFilterImpl( collection );

      if( m_firstCondition != null )
      {
        result.m_firstCondition = m_firstCondition.Clone( result );
      }

      if( m_secondCondition != null )
      {
        result.m_secondCondition = m_secondCondition.Clone( result );
      }

      if( m_record != null )
      {
        result.m_record = ( AutoFilterRecord )m_record.Clone();
      }

      if( m_shape != null )
      {
        ShapesCollection destShapes = result.WorksheetShapes;
        bool bFound = false;

        for( int i = 0, len = destShapes.Count; i < len; i++ )
        {
          FormControlShapeImpl shape = destShapes[ i ] as FormControlShapeImpl;

          if( shape != null )
          {
            if( shape.TopRow == m_shape.TopRow && shape.BottomRow == m_shape.BottomRow &&
              shape.LeftColumn == m_shape.LeftColumn && shape.RightColumn == m_shape.RightColumn &&
              shape.ShapeType == m_shape.ShapeType )
            {
              bFound = true;
              result.m_shape = shape;
              break;
            }
          }
        }

        if( !bFound )
          result.m_shape = ( FormControlShapeImpl )m_shape.Clone( destShapes, null, null, true );
      }
      
      return result;
    }
    /// <summary>
    /// To select the range in which the autofilter has to be applied
    /// </summary>
    internal void SelectRangesToFilter()
    {
        IRange filterRange = Worksheet.AutoFilters.FilterRange;

        IRange colFilterRange = Worksheet.Range[filterRange.Row + 1, m_colIndex, filterRange.LastRow, m_colIndex];

        foreach (IRange range in colFilterRange)
        {
            m_rangeDict.Add(range, range.Number);
        }

        foreach (KeyValuePair<IRange, double> pair in m_rangeDict)
        {
            m_rangeList.Add(pair);
        }
        
        m_rangeList.Sort(delegate(KeyValuePair<IRange, double> x, KeyValuePair<IRange, double> y) { return x.Value.CompareTo(y.Value); });
    }
    /// <summary>
    /// To set the top10 filter for filtering values based on IsTop and Top10Number
    /// </summary>
    internal void SetTop10()
    {
        int count, filtervalueindex, startRange, endRange;      
        
        count = m_rangeList.Count;      
        
        if (IsTop)
        {
            filtervalueindex = count - Top10Number ;
            m_filterValue = m_rangeList[filtervalueindex].Value;
            startRange = 0;
            endRange = filtervalueindex + 1;
        }
        else 
        {
            filtervalueindex = Top10Number - 1;
            m_filterValue = m_rangeList[filtervalueindex].Value;
            startRange = filtervalueindex + 1;
            endRange = count;
        }

        for (int i = startRange; i < endRange; i++)
        {
            CellRecordCollection cells = Worksheet.CellRecords;
            if (cells.Table.Rows[(m_rangeList[i].Key.Row) - 1] != null)
            {
                if (m_rangeList[i].Value != m_filterValue && !(((RowStorage)cells.Table.Rows[(m_rangeList[i].Key.Row) - 1]).IsHidden))
                    m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
            }
            
        }
    }
    /// <summary>
    /// To filter values based upon the conditions according to condition operator, datatype and condition value
    /// </summary>
    /// <param name="conditionOperator">Comparison operator</param>
    /// <param name="datatype">Data Type</param>
    /// <param name="conditionValue">Value may be double or string</param>
    /// <param name="currentAutoFilter" ></param>
    internal void SetCondition(ExcelFilterCondition conditionOperator, ExcelFilterDataType datatype, object conditionValue,int currentAutoFilter)
    {
        ExcelFilterCondition secondConditionOperator = m_autofilters[currentAutoFilter].SecondCondition.ConditionOperator;
        m_autofilters.Worksheet.EnableSheetCalculations();
        switch (conditionOperator)
        {
            case ExcelFilterCondition.GreaterOrEqual:
                
                for (int i = 0; i < m_rangeList.Count; i++)
                {
                    if (secondConditionOperator  != 0 && m_autofilters[currentAutoFilter].IsAnd == false)
                    {
                        if (m_rangeList[i].Key.HasFormula)
                        {
                            if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) >= Convert.ToDouble(conditionValue))
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);
                        }
                        else if (m_rangeList[i].Value >= Convert.ToDouble(conditionValue))
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);

                    }
                    else
                    {
                        if (m_rangeList[i].Key.HasFormula)
                        {
                            if (Convert .ToDouble (m_rangeList[i].Key.CalculatedValue) < Convert.ToDouble(conditionValue))
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                        }
                        else if (m_rangeList[i].Value < Convert.ToDouble(conditionValue) || m_rangeList[i].Key.Value == "")
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                    }
                }
                break;

            case ExcelFilterCondition.Greater:
                for (int i = 0; i < m_rangeList.Count; i++)
                {
                    if (secondConditionOperator!= 0 && m_autofilters[currentAutoFilter].IsAnd == false)
                    {
                        if (m_rangeList[i].Key.HasFormula)
                        {
                            if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) > Convert.ToDouble(conditionValue))
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);
                        }
                        else if (m_rangeList[i].Value > Convert.ToDouble(conditionValue))
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);

                    }
                    else
                    {
                        if (m_rangeList[i].Key.HasFormula)
                        {
                            if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) <= Convert.ToDouble(conditionValue)) 
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                        }
                        else if (m_rangeList[i].Value <= Convert.ToDouble(conditionValue) || m_rangeList[i].Key.Value == "")
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                    }
                                       
                }
                break;

            case ExcelFilterCondition.LessOrEqual:
                for (int i = 0; i < m_rangeList.Count; i++)
                {
                    if (secondConditionOperator != 0 && m_autofilters[currentAutoFilter].IsAnd == false)
                    {
                        if (m_rangeList[i].Key.HasFormula)
                        {
                            if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) <= Convert.ToDouble(conditionValue))
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);
                        }
                        else if (m_rangeList[i].Value <= Convert.ToDouble(conditionValue))
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);

                    }
                    else
                    {
                        if (m_rangeList[i].Key.HasFormula)
                        {
                            if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) > Convert.ToDouble(conditionValue)) 
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                        }
                            else if (m_rangeList[i].Value > Convert.ToDouble(conditionValue) || m_rangeList[i].Key.Value == string.Empty)
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                    }
                }                        
                break;

            case ExcelFilterCondition.Less:
                for (int i = 0; i < m_rangeList.Count; i++)
                {
                    if (secondConditionOperator!= 0 && m_autofilters[currentAutoFilter].IsAnd == false)
                    {
                        if (m_rangeList[i].Key.HasFormula)
                        {
                            if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) < Convert.ToDouble(conditionValue))
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);
                        }
                        else if (m_rangeList[i].Value < Convert.ToDouble(conditionValue))
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);

                    }
                    else
                    {
                        if (m_rangeList[i].Key.HasFormula)
                        {
                            if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) >= Convert.ToDouble(conditionValue))
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                        }
                        else if (m_rangeList[i].Value >= Convert.ToDouble(conditionValue) || m_rangeList[i].Key.Value == "")
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                    }
                }
                break;
            case ExcelFilterCondition.Equal:
                if (datatype == ExcelFilterDataType.MatchAllBlanks )
                {
                    IsSimple1 = true;
                    for (int i = 0; i < m_rangeList.Count; i++)
                    {
                        if (secondConditionOperator != 0 && m_autofilters[currentAutoFilter].IsAnd == false)
                        {
                            if (m_rangeList[i].Key.Value == string.Empty)
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);

                        }
                        else if (m_rangeList[i].Key.Value != string.Empty)
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                    }
                }
                else
                {
                    if (datatype == ExcelFilterDataType.String )
                        IsSimple1 = true;

                    for (int i = 0; i < m_rangeList.Count; i++)
                    {
                        if (secondConditionOperator != 0 && m_autofilters[currentAutoFilter].IsAnd == false)
                        {
                            if (m_rangeList[i].Key.HasFormula)
                            {
                                if (Convert.ToDouble(m_rangeList[i].Key.DisplayText) == Convert.ToDouble(conditionValue))
                                    m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);
                            }
                            else if (m_rangeList[i].Key.Value == Convert.ToString(conditionValue))
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);

                        }
                        else
                        {
                            if (m_rangeList[i].Key.DisplayText != Convert.ToString(conditionValue) || m_rangeList[i].Key.DisplayText == "")
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                            else
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);
                        }
                    }
                }
                break;

            case ExcelFilterCondition.NotEqual:
                if (datatype == ExcelFilterDataType.MatchAllNonBlanks )
                {                    
                    for (int i = 0; i < m_rangeList.Count; i++)
                    {
                        if (secondConditionOperator != 0 && m_autofilters[currentAutoFilter].IsAnd == false)
                        {
                            if (m_rangeList[i].Key.Value != string.Empty)
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);

                        }
                        else if (m_rangeList[i].Key.Value == string.Empty)
                            m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                    }
                }
                else
                {
                    for (int i = 0; i < m_rangeList.Count; i++)
                    {
                        if (secondConditionOperator != 0 && m_autofilters[currentAutoFilter].IsAnd == false)
                        {
                            if (m_rangeList[i].Key.HasFormula)
                            {
                                if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) != Convert.ToDouble(conditionValue))
                                    m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);
                            }
                            else if (m_rangeList[i].Value != Convert.ToDouble(conditionValue))
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, true);

                        }
                        else
                        {
                            if (m_rangeList[i].Key.HasFormula)
                            {
                                if (Convert.ToDouble(m_rangeList[i].Key.CalculatedValue) == Convert.ToDouble(conditionValue))
                                    m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                            }
                            else if (m_rangeList[i].Value == Convert.ToDouble(conditionValue) || m_rangeList[i].Key.Value == "")
                                m_rangeList[i].Key.Worksheet.ShowRow(m_rangeList[i].Key.Row, false);
                        }
                    }
                }
                break;  
        }
        m_autofilters.Worksheet.DisableSheetCalculations();
    }
    
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Parses specified record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="iRowIndex">Row index.</param>
    [ CLSCompliant( false ) ]
    public void Parse( AutoFilterRecord record, int iColumnIndex, int iRowIndex )
    {
      m_record = ( AutoFilterRecord )record.Clone();
      m_firstCondition.Parse( record.FirstCondition );
      m_secondCondition.Parse( record.SecondCondition );

      ShapesCollection shapes = WorksheetShapes;

      for( int i = 0, len = shapes.Count; i < len; i++ )
      {
        IShape shape = shapes[ i ];

        if( shape is FormControlShapeImpl )
        {
          FormControlShapeImpl combo = shape as FormControlShapeImpl;

          if( combo.LeftColumn == iColumnIndex && combo.Top == iRowIndex )
          {
            m_shape = combo;
            return;
          }
        }
      }

      m_shape = WorksheetShapes.AddFormControlShape();
      m_shape.LeftColumn = iColumnIndex;
      m_shape.TopRow = iRowIndex;
    }

    /// <summary>
    /// Saves autofilter as biff records.
    /// </summary>
    /// <param name="records">Records to be serialized.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      m_firstCondition.Serialize( m_record.FirstCondition );
      m_secondCondition.Serialize( m_record.SecondCondition );

      records.Add( m_record );
    }
    #endregion
  }
}
