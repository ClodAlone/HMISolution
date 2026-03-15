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
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class is used to apply conditional formatting to the cells.
  /// </summary>
  internal class CFApplier
  {
    #region Members
    /// <summary>
    /// This object is used to evaluate formula.
    /// </summary>
    private FormulaEvaluator m_evaluator = new FormulaEvaluator();
    /// <summary>
    /// Object that is used to compare two string objects.
    /// </summary>
    private StringComparer m_comparer =
#if !SILVERLIGHT && !WINRT && !WP
        new StringComparer();
#else
        StringComparer.CurrentCulture;
#endif
    private const string m_trueString="TRUE";
    #endregion

    #region Methods
    /// <summary>
    /// Applies cf to the specified cell and format if necessary.
    /// </summary>
    /// <param name="cell">Cell to check condition for.</param>
    /// <param name="xf">ExtendedFormat to apply to.</param>
    /// <returns>Modified extended format or null if no modifications required.</returns>
    public ExtendedFormatImpl ApplyCF( IRange cell, ExtendedFormatImpl xf )
    {
      ExtendedFormatImpl result = null;

      if( ( cell as RangeImpl ).HasConditionFormats )
      {
        IConditionalFormats formats = cell.ConditionalFormats;

        for( int i = 0, len = formats.Count; i < len && result == null; i++ )
        {
          IConditionalFormat format = formats[ i ];
          result = CheckAndApplyCondition( format, cell, xf );
        }
      }

      return ( result != null ) ?
        result :
        xf;
    }
    /// <summary>
    /// Check and applies condition if necessary.
    /// </summary>
    /// <param name="format">Format to check and apply.</param>
    /// <param name="cell">Cell to check condition for.</param>
    /// <param name="xf">ExtendedFormat to apply to.</param>
    /// <returns>Modified extended format or null if no modifications required.</returns>
    private ExtendedFormatImpl CheckAndApplyCondition( IConditionalFormat format,
      IRange cell,
      ExtendedFormatImpl xf )
    {
      ExtendedFormatImpl result;

      switch( format.FormatType )
      {
        case ExcelCFType.CellValue:
          result = CheckAndApplyConditionValue( format, cell, xf );
          break;

        case ExcelCFType.Formula:
          result = CheckAndApplyConditionFormula( format, cell, xf );
          break;

        default:
          result = null;
          break;
      }

      return result;
    }
    /// <summary>
    /// Checks and applies condition if condition type is Value.
    /// </summary>
    /// <param name="format">Format to check and apply.</param>
    /// <param name="cell">Cell to check condition for.</param>
    /// <param name="xf">ExtendedFormat to apply to.</param>
    /// <returns>Modified extended format or null if no modifications required.</returns>
    private ExtendedFormatImpl CheckAndApplyConditionValue( IConditionalFormat format,
      IRange cell,
      ExtendedFormatImpl xf )
    {
      IInternalConditionalFormat internalFormat = format as IInternalConditionalFormat;
      IWorksheet sheet = cell.Worksheet;
      object value1 = m_evaluator.TryGetValue( internalFormat.FirstFormulaPtgs, sheet );
      object value2 = m_evaluator.TryGetValue( internalFormat.SecondFormulaPtgs, sheet );
      bool bSuccess = false;

      switch( format.Operator )
      {
        case ExcelComparisonOperator.Between:
          bSuccess = CheckBetween( cell, value1, value2 );
          break;

        case ExcelComparisonOperator.Equal:
          bSuccess = CheckEqual( cell, value1 );
          break;

        case ExcelComparisonOperator.Greater:
          bSuccess = CheckGreater( cell, value1 );
          break;

        case ExcelComparisonOperator.GreaterOrEqual:
          bSuccess = CheckGreaterOrEqual( cell, value1 );
          break;

        case ExcelComparisonOperator.Less:
          bSuccess = CheckLess( cell, value1 );
          break;

        case ExcelComparisonOperator.LessOrEqual:
          bSuccess = CheckLessOrEqual( cell, value1 );
          break;

        case ExcelComparisonOperator.NotBetween:
          bSuccess = CheckNotBetween( cell, value1, value2 );
          break;

        case ExcelComparisonOperator.NotEqual:
          bSuccess = CheckNotEqual( cell, value1 );
          break;

        case ExcelComparisonOperator.None:
          break;
      }

      ExtendedFormatImpl result = null;

      if( bSuccess )
        result = ApplyCondition( format, xf );

      return result;
    }
    /// <summary>
    /// Checks whether cell value is between value1 and value2.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value1">The first value to compare.</param>
    /// <param name="value2">The second value to compare.</param>
    /// <returns>True if cell value is between value1 and value2.</returns>
    private bool CheckBetween( IRange cell, object value1, object value2 )
    {
      if( value1 == null || value2 == null )
        return false;

      return CheckGreaterOrEqual( cell, value1 ) && CheckLessOrEqual( cell, value2 );
    }
    /// <summary>
    /// Checks whether cell value is equal to value.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value">Value to check.</param>
    /// <returns>True if cell value is equal to value.</returns>
    private bool CheckEqual( IRange cell, object value )
    {
      if( value == null )
        return false;

      return Compare( cell, value ) == 0;
    }
    /// <summary>
    /// Checks whether cell value is greater than value.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value">Value to check.</param>
    /// <returns>True if cell value is greater than value.</returns>
    private bool CheckGreater( IRange cell, object value )
    {
      if( value == null )
        return false;

      return Compare( cell, value ) > 0;
    }
    /// <summary>
    /// Checks whether cell value is greater than or equal to value.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value">Value to check.</param>
    /// <returns>True if cell value is greater than or equal to value.</returns>
    private bool CheckGreaterOrEqual( IRange cell, object value )
    {
      if( value == null )
        return false;

      return Compare( cell, value ) >= 0;
    }
    /// <summary>
    /// Checks whether cell value is less than value.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value">Value to check.</param>
    /// <returns>True if cell value is less than value.</returns>
    private bool CheckLess( IRange cell, object value )
    {
      if( value == null )
        return false;

      int iValue = Compare( cell, value );
      return ( iValue != int.MinValue && iValue < 0 );
    }
    /// <summary>
    /// Checks whether cell value is less than or equal to value.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value">Value to check.</param>
    /// <returns>True if cell value is greater than or equal to value.</returns>
    private bool CheckLessOrEqual( IRange cell, object value )
    {
      if( value == null )
        return false;

      int iValue = Compare( cell, value );
      return ( iValue != int.MinValue && iValue <= 0 );
    }
    /// <summary>
    /// Checks whether cell value is not between value1 and value2.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value1">The first value to compare.</param>
    /// <param name="value2">The second value to compare.</param>
    /// <returns>True if cell value is not between value1 and value2.</returns>
    private bool CheckNotBetween( IRange cell, object value1, object value2 )
    {
      if( value1 == null || value2 == null )
        return false;

      return CheckLess( cell, value1 ) || CheckGreater( cell, value2 );
    }
    /// <summary>
    /// Checks whether cell value is not equal to value.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value">Value to check.</param>
    /// <returns>True if cell value is not equal to value.</returns>
    private bool CheckNotEqual( IRange cell, object value )
    {
      if( value == null )
        return false;

      int iValue = Compare( cell, value );
      return ( iValue != int.MinValue && iValue != 0 );

      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Compares cell value and specified value.
    /// </summary>
    /// <param name="cell">Cell to check.</param>
    /// <param name="value">Value to check.</param>
    /// <returns>Int.MinValue if was unable to compare, 0 if equal, 1 if greater, -1 if less.</returns>
    private int Compare( IRange cell, object value )
    {
      if( value == null )
        return int.MinValue;

      int iResult = int.MinValue;

      if( cell.HasNumber )
      {
        double dNumber = cell.Number;
        iResult = CompareDouble( dNumber, value );
      }
      else if( cell.HasString )
      {
        string text = cell.Text;
        CompareString( text, value );
      }
      else if( cell.HasBoolean )
      {
        bool boolean = cell.Boolean;
        iResult = CompareBoolean( boolean, value );
      }
      else if( cell.HasFormula )
      {
        if( cell.HasFormulaBoolValue )
        {
          iResult = CompareBoolean( cell.FormulaBoolValue, value );
        }
        else if( cell.FormulaStringValue != null )
        {
          iResult = CompareString( cell.FormulaStringValue, value );
        }
        else if( !double.IsNaN( cell.FormulaNumberValue ) )
        {
          iResult = CompareDouble( cell.FormulaNumberValue, value );
        }
      }

      return iResult;
    }

    private int CompareDouble( double number, object value )
    {
      return ( value is double ) ?
        number.CompareTo( ( double )value ) :
        int.MinValue;
    }

    private int CompareString( string text, object value )
    {
      string strValue = value as string;
      return ( strValue != null ) ?
        m_comparer.Compare( text, strValue ) :
        int.MinValue;
    }
    private int CompareBoolean( bool boolean, object value )
    {
      return ( value is bool ) ?
        boolean.CompareTo( ( bool )value ) :
        int.MinValue;
    }
    /// <summary>
    /// Checks and applies condition if condition type is Formula.
    /// </summary>
    /// <param name="format">Format to check and apply.</param>
    /// <param name="cell">Cell to check condition for.</param>
    /// <param name="xf">ExtendedFormat to apply to.</param>
    /// <returns>Modified extended format or null if no modifications required.</returns>
    private ExtendedFormatImpl CheckAndApplyConditionFormula( IConditionalFormat format,
      IRange cell,
      ExtendedFormatImpl xf )
    {
      ExtendedFormatImpl result = null;

      if( cell.Formula == format.FirstFormula )
      {
        result = ApplyCondition( format, xf );
      }
      else if (cell.ConditionalFormats.Count != 0 && cell.DisplayText == "  ")
      {
          if (format.FormatType == ExcelCFType.Formula)
          {
              cell.Worksheet.EnableSheetCalculations();
              string value = cell.Worksheet.CalcEngine.ParseAndComputeFormula(format.FirstFormula);

              if (value == m_trueString)
              {
                  result = ApplyCondition(format, xf);
              }
              cell.Worksheet.DisableSheetCalculations();
          }

      }
      return result;
    }
    /// <summary>
    /// Applies condition settings to specified Extended format.
    /// </summary>
    /// <param name="condition">Condition to apply.</param>
    /// <param name="xf">ExtendedFormat to apply settings to.</param>
    /// <returns></returns>
    private ExtendedFormatImpl ApplyCondition( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      ExtendedFormatImpl result = new ExtendedFormatStandAlone( xf );//( ExtendedFormatImpl )xf.Clone();

      UpdateFill( condition, result );
      UpdateFont( condition, result );
      UpdateBorders( condition, result );

      return result;
    }
    /// <summary>
    /// Updates font settings.
    /// </summary>
    /// <param name="condition">Conditional format to get settings from.</param>
    /// <param name="xf">Extended format to update.</param>
    private void UpdateFont( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      xf.Font.BeginUpdate();
      UpdateFontFormat( condition, xf );
      UpdateFontColor( condition, xf );
      xf.Font.EndUpdate();
    }
    /// <summary>
    /// Update fill settings.
    /// </summary>
    /// <param name="condition">Conditional format to get settings from.</param>
    /// <param name="xf">Extended format to update.</param>
    private void UpdateFill( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      UpdatePatternFormat( condition, xf );
      UpdatePatternColor( condition, xf );
      UpdateBackgroundColor( condition, xf );
    }
    /// <summary>
    /// Updates pattern color if necessary.
    /// </summary>
    /// <param name="condition">Conditional format to get settings from.</param>
    /// <param name="xf">Extended format to update.</param>
    private void UpdatePatternColor( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      if( condition.IsPatternColorPresent )
      {
        xf.PatternColor = condition.ColorRGB;
      }
    }
    /// <summary>
    /// Updates background color if necessary.
    /// </summary>
    /// <param name="condition">Conditional format to get settings from.</param>
    /// <param name="xf">Extended format to update.</param>
    private void UpdateBackgroundColor( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      if( condition.IsBackgroundColorPresent )
      {
        xf.Color = condition.BackColorRGB;
      }
    }
    /// <summary>
    /// Updates font color if necessary.
    /// </summary>
    /// <param name="condition">Conditional format to get settings from.</param>
    /// <param name="xf">Extended format to update.</param>
    private void UpdateFontColor( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      if( condition.IsFontColorPresent )
      {
        xf.Font.RGBColor = condition.FontColorRGB;
      }
    }
    /// <summary>
    /// Updates pattern format if necessary.
    /// </summary>
    /// <param name="condition">Conditional format to get settings from.</param>
    /// <param name="xf">Extended format to update.</param>
    private void UpdatePatternFormat( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      if( condition.IsPatternFormatPresent )
      {
        xf.FillPattern = condition.FillPattern;
      }
    }
    /// <summary>
    /// Updates font format if necessary.
    /// </summary>
    /// <param name="condition">Conditional format to get settings from.</param>
    /// <param name="xf">Extended format to update.</param>
    private void UpdateFontFormat( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      if( condition.IsFontFormatPresent )
      {
        IFont font = xf.Font;

        font.Bold = condition.IsBold;
        font.Italic = condition.IsItalic;
        font.Strikethrough = condition.IsStrikeThrough;
        font.Subscript = condition.IsSubScript;
        font.Superscript = condition.IsSuperScript;
        font.Underline = condition.Underline;
      }
    }
    /// <summary>
    /// Updates border format if necessary.
    /// </summary>
    /// <param name="condition">Conditional format to get settings from.</param>
    /// <param name="xf">Extended format to update.</param>
    private void UpdateBorders( IConditionalFormat condition, ExtendedFormatImpl xf )
    {
      if( condition.IsBorderFormatPresent )
      {
        if( condition.IsLeftBorderModified )
        {
          xf.LeftBorderColor.SetRGB( condition.LeftBorderColorRGB );
          xf.LeftBorderLineStyle = condition.LeftBorderStyle;
        }

        if( condition.IsRightBorderModified )
        {
          xf.RightBorderColor.SetRGB( condition.RightBorderColorRGB );
          xf.RightBorderLineStyle = condition.RightBorderStyle;
        }

        if( condition.IsTopBorderModified )
        {
          xf.TopBorderColor.SetRGB( condition.TopBorderColorRGB );
          xf.TopBorderLineStyle = condition.TopBorderStyle;
        }

        if( condition.IsBottomBorderModified )
        {
          xf.BottomBorderColor.SetRGB( condition.BottomBorderColorRGB );
          xf.BottomBorderLineStyle = condition.BottomBorderStyle;
        }
      }
    }
    #endregion
  }
}
