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

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;

using DOPER = Syncfusion.XlsIO.Parser.Biff_Records.AutoFilterRecord.DOPER;
using DOPERDataType = Syncfusion.XlsIO.Parser.Biff_Records.AutoFilterRecord.DOPER.DOPERDataType;
using Syncfusion.XlsIO.Implementation.Collections;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Summary description for AutoFilterConditionImpl.
  /// </summary>
  public class AutoFilterConditionImpl
    : /*CommonObject
    ,*/ IAutoFilterCondition
  {
    #region Class members
    /// <summary>
    /// Data type.
    /// </summary>
    private ExcelFilterDataType m_dataType;
    /// <summary>
    /// Comparison operator.
    /// </summary>
    private ExcelFilterCondition m_conditionOperator;
    /// <summary>
    /// String value.
    /// </summary>
    private string m_strValue;
    /// <summary>
    /// Boolean value.
    /// </summary>
    private bool m_bValue;
    /// <summary>
    /// Error code.
    /// </summary>
    private byte m_btErrorCode;
    /// <summary>
    /// Floating-point value.
    /// </summary>
    private double m_dValue;
    private AutoFiltersCollection m_autofilters;
    private AutoFilterImpl m_autoFilter;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance.
    /// </summary>
    public AutoFilterConditionImpl(AutoFiltersCollection autofilters)
    {
      //
      // TODO: Add constructor logic here
      //
      m_autofilters = autofilters;
	}

    #endregion

    #region IAutoFilterCondition Members
    /// <summary>
    /// Data type.
    /// </summary>
    public ExcelFilterDataType DataType
    {
      get
      {
        return m_dataType;
      }
      set
      {
        m_dataType = value;
        for ( int currentFilter = 0, iCount = m_autofilters.Count; currentFilter < iCount; currentFilter++ )
        {
            if ( m_autofilters[currentFilter].FirstCondition.DataType == ExcelFilterDataType.MatchAllBlanks 
                || m_autofilters[currentFilter].SecondCondition.DataType == ExcelFilterDataType.MatchAllBlanks )
            {
                m_autoFilter = (AutoFilterImpl)m_autofilters[currentFilter];
                m_autoFilter.SelectRangesToFilter();
                m_autoFilter.SetCondition( ExcelFilterCondition.Equal, m_dataType, String.Empty,currentFilter );
            }
            if ( m_autofilters[currentFilter].FirstCondition.DataType == ExcelFilterDataType.MatchAllNonBlanks
               || m_autofilters[currentFilter].SecondCondition.DataType == ExcelFilterDataType.MatchAllNonBlanks)
            {
                m_autoFilter = (AutoFilterImpl)m_autofilters[currentFilter];
                m_autoFilter.SelectRangesToFilter();
                m_autoFilter.SetCondition( ExcelFilterCondition.NotEqual, m_dataType, String.Empty,currentFilter);
            }
        }
      }
    }

    /// <summary>
    /// Comparison operator.
    /// </summary>
    public ExcelFilterCondition ConditionOperator
    {
      get
      {
        return m_conditionOperator;
      }
      set
      {
        m_conditionOperator = value;
      }
    }

    /// <summary>
    /// String value.
    /// </summary>
    public string String
    {
      get
      {
        return m_strValue;
      }
      set
      {
        m_strValue = value;
        for (int currentFilter = 0, iCount = m_autofilters.Count; currentFilter < iCount; currentFilter++)
        {
            object conditionValue = m_strValue;
            if (m_autofilters[currentFilter].FirstCondition.String == m_strValue && m_autofilters[currentFilter].FirstCondition.DataType == m_dataType )
            {
                m_autoFilter = (AutoFilterImpl)m_autofilters[currentFilter];
                m_autoFilter.SelectRangesToFilter();
                m_autoFilter.SetCondition(ExcelFilterCondition.Equal, m_autofilters[currentFilter].FirstCondition.DataType, conditionValue,currentFilter);
            }
            if (m_autofilters[currentFilter].SecondCondition.String == m_strValue && m_autofilters[currentFilter].SecondCondition.DataType == m_dataType )
            {
                m_autoFilter = (AutoFilterImpl)m_autofilters[currentFilter];
                m_autoFilter.SelectRangesToFilter();
                m_autoFilter.SetCondition(ExcelFilterCondition.Equal, m_autofilters[currentFilter].FirstCondition.DataType, conditionValue,currentFilter);
            }
        }       
      }
    }

    /// <summary>
    /// Boolean value.
    /// </summary>
    public bool Boolean
    {
      get
      {
        return m_bValue;
      }
      set
      {
        m_bValue = value;
      }
    }

    /// <summary>
    /// Error code.
    /// </summary>
    public byte ErrorCode
    {
      get
      {
        return m_btErrorCode;
      }
      set
      {
        m_btErrorCode = value;
      }
    }

    /// <summary>
    /// Floating-point value.
    /// </summary>
    public double Double
    {
      get
      {
        return m_dValue;
      }
      set
      {
        m_dValue = value;
        object conditionValue = m_dValue;
        for( int currentFilter = 0, iCount = m_autofilters.Count ; currentFilter < iCount; currentFilter++ )
        {
            if ( m_autofilters[currentFilter].FirstCondition.Double == m_dValue &&
                 m_autofilters[currentFilter].FirstCondition.DataType == m_dataType &&
                 m_autofilters[currentFilter].FirstCondition.ConditionOperator == m_conditionOperator )
            {
                m_autoFilter = (AutoFilterImpl)m_autofilters[currentFilter];
                m_autoFilter.SelectRangesToFilter();
                m_autoFilter.SetCondition(m_autofilters[currentFilter].FirstCondition.ConditionOperator, m_autofilters[currentFilter].FirstCondition.DataType, conditionValue,currentFilter);
            }
            if ( m_autofilters[currentFilter].SecondCondition.Double == m_dValue &&
                      m_autofilters[currentFilter].SecondCondition.DataType == m_dataType &&
                      m_autofilters[currentFilter].SecondCondition.ConditionOperator == m_conditionOperator )
            {
                m_autoFilter = (AutoFilterImpl)m_autofilters[currentFilter];
                m_autoFilter.SelectRangesToFilter();
                m_autoFilter.SetCondition(m_autofilters[currentFilter].SecondCondition.ConditionOperator,m_autofilters[currentFilter].SecondCondition.DataType, conditionValue,currentFilter);                
            }
        }        
      }
    }

    #endregion

    #region Class implementation methods
    /// <summary>
    /// Parse current object.
    /// </summary>
    /// <param name="condition">Current condition.</param>
    [ CLSCompliant( false ) ]
    public void Parse( DOPER condition )
    {
      switch( condition.DataType )
      {
        case DOPERDataType.FilterNotUsed:
          m_dataType = ExcelFilterDataType.NotUsed;
          break;

        case DOPERDataType.BoolOrError:
          if( condition.IsBool )
          {
            m_dataType = ExcelFilterDataType.Boolean;
            m_bValue = condition.Boolean;
          }
          else
          {
            m_dataType = ExcelFilterDataType.ErrorCode;
            m_btErrorCode = condition.ErrorCode;
          }
          break;

        case DOPERDataType.Number:
          m_dataType = ExcelFilterDataType.FloatingPoint;
          m_dValue = condition.Number;
          break;

        case DOPERDataType.RKNumber:
          m_dataType = ExcelFilterDataType.FloatingPoint;
          m_dValue = RKRecord.ConvertToDouble( condition.RKNumber );
          break;

        case DOPERDataType.String:
          m_dataType = ExcelFilterDataType.String;
          m_strValue = condition.StringValue;
          break;

        case DOPERDataType.MatchBlanks:
          m_dataType = ExcelFilterDataType.MatchAllBlanks;
          break;

        case DOPERDataType.MatchNonBlanks:
          m_dataType = ExcelFilterDataType.MatchAllNonBlanks;
          break;

        default:
          throw new ArgumentOutOfRangeException( "condition.DataType" );
      }

      m_conditionOperator = ( ExcelFilterCondition )condition.ComparisonSign;
    }
    /// <summary>
    /// Serialize method.
    /// </summary>
    /// <param name="condition">Current condition.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( DOPER condition )
    {
      switch( m_dataType )
      {
        case ExcelFilterDataType.NotUsed:
          condition.DataType = DOPERDataType.FilterNotUsed;
          break;

        case ExcelFilterDataType.Boolean:
          condition.DataType = DOPERDataType.BoolOrError;
          condition.Boolean = m_bValue;
          break;

        case ExcelFilterDataType.ErrorCode:
          condition.DataType = DOPERDataType.BoolOrError;
          condition.ErrorCode = m_btErrorCode;
          break;

        case ExcelFilterDataType.FloatingPoint:
          condition.DataType = DOPERDataType.Number;
          condition.Number = m_dValue;
          break;

        case ExcelFilterDataType.String:
          condition.DataType = DOPERDataType.String;
          condition.StringValue = m_strValue;
          break;

        case ExcelFilterDataType.MatchAllBlanks:
          condition.DataType = DOPERDataType.MatchBlanks;
          break;

        case ExcelFilterDataType.MatchAllNonBlanks:
          condition.DataType = DOPERDataType.MatchNonBlanks;
          break;

        default:
          throw new ArgumentOutOfRangeException( "m_dataType" );
      }

      condition.ComparisonSign = ( DOPER.DOPERComparisonSign )m_conditionOperator;
    }
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns just cloned object.</returns>
    public AutoFilterConditionImpl Clone( object parent )
    {
      AutoFilterConditionImpl result = ( AutoFilterConditionImpl )MemberwiseClone();
      //result.SetParent( parent );

      return result;
    }
    #endregion
  }
}
