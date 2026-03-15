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
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Implementation
{
  class FormulaEvaluator
  {
    public object TryGetValue( Ptg[] formula, IWorksheet sheet )
    {
      if( formula == null || formula.Length == 0 )
        return null;

      // 1. Check whether there is just one constant item
      if( formula.Length == 1 )
      {
        Ptg token = formula[ 0 ];
        return GetSingleTokenResult( token, sheet );
      }
      else
      {
        return null;
        //throw new Exception( "The method or operation is not implemented." );
      }
    }

    private object GetSingleTokenResult( Ptg token, IWorksheet sheet )
    {
      object result = null;

      switch( token.TokenCode )
      {
        case FormulaToken.tBoolean:
          result = ( token as BooleanPtg ).Value;
          break;

        case FormulaToken.tNumber:
          result = ( token as DoublePtg ).Value;
          break;

        case FormulaToken.tInteger:
          result = ( double )( ( token as IntegerPtg ).Value );
          break;

        case FormulaToken.tStringConstant:
          result = ( token as StringConstantPtg ).Value;
          break;

        case FormulaToken.tRef1:
        case FormulaToken.tRef2:
        case FormulaToken.tRef3:
          IRange range = (token as RefPtg).GetRange(sheet.Workbook, sheet);
          if ((range as RangeImpl).IsSingleCell  || range.HasFormula)
          {
              result = range.Value2;
          }
          break;
      }

      return result;
    }
  }
}
