#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Grid.Converter
{
    public static class GridFormatConversionHelper
    {

#if !WinRT
        private static Dictionary<GridColumn, NumberFormatInfo> NumberFormatInfoDict = new Dictionary<GridColumn, NumberFormatInfo>();
      
        internal static string ConvertNumberFormatToExcel(NumberFormatInfo numberFormat, GridColumn gridColumn)
        {
            String format = "General";
            string posPattern, negPattern = string.Empty;
            if (gridColumn is GridNumericColumn)
            {
                format = "0";
                if (numberFormat.NumberDecimalDigits > 0)
                {
                    format += ".";
                    for (int i = 0; i < numberFormat.NumberDecimalDigits; i++)
                    {
                        format += "0";
                    }
                }
                switch (numberFormat.NumberNegativePattern)
                {
                    case 0:
                        negPattern = "(" + format + ")";
                        break;
                    case 1:
                        negPattern = "-" + format;
                        break;
                    case 2:
                        negPattern = "- " + format;
                        break;
                    case 3:
                        negPattern = format + "-";
                        break;
                    case 4:
                        negPattern = format + " -";
                        break;
                    default:
                        negPattern = "(" + format + ")";
                        break;
                }
                format = format + "_);" + negPattern;
            }
            else if (gridColumn is GridCurrencyColumn)
            {
                format = "#,##0";
                string currencySymbol = "[$" + numberFormat.CurrencySymbol + "]";
                if (numberFormat.CurrencyDecimalDigits > 0)
                {
                    format += ".";
                    for (int i = 0; i < numberFormat.CurrencyDecimalDigits; i++)
                    {
                        format += "0";
                    }
                }
                switch (numberFormat.CurrencyPositivePattern)
                {
                    case 0:
                        posPattern = currencySymbol + format;
                        break;
                    case 1:
                        posPattern = format + currencySymbol;
                        break;
                    case 2:
                        posPattern = currencySymbol + " " + format;
                        break;
                    case 3:
                        posPattern = format + " " + currencySymbol;
                        break;
                    default:
                        posPattern = currencySymbol + format;
                        break;
                }
                switch (numberFormat.CurrencyNegativePattern)
                {
                    case 0:
                        negPattern = "(" + currencySymbol + format + ")";
                        break;
                    case 1:
                        negPattern = currencySymbol + format;
                        break;
                    case 2:
                        negPattern = currencySymbol + "-" + format;
                        break;
                    case 3:
                        negPattern = currencySymbol + format + "-";
                        break;
                    case 4:
                        negPattern = "(" + format + currencySymbol + ")";
                        break;
                    case 5:
                        negPattern = "-" + format + currencySymbol;
                        break;
                    case 6:
                        negPattern = format + "-" + currencySymbol;
                        break;
                    case 7:
                        negPattern = format + currencySymbol + "-";
                        break;
                    case 8:
                        negPattern = "-" + format + " " + currencySymbol;
                        break;
                    case 9:
                        negPattern = "-" + currencySymbol + " " + format;
                        break;
                    case 10:
                        negPattern = format + " " + currencySymbol + "-";
                        break;
                    case 11:
                        negPattern = currencySymbol + " " + format + "-";
                        break;
                    case 12:
                        negPattern = currencySymbol + " " + "-" + format;
                        break;
                    case 13:
                        negPattern = format + "- " + currencySymbol;
                        break;
                    case 14:
                        negPattern = "(" + currencySymbol + " " + format + ")";
                        break;
                    case 15:
                        negPattern = "(" + format + " " + currencySymbol + ")";
                        break;
                    default:
                        negPattern = "(" + currencySymbol + format + ")";
                        break;
                }
                format = posPattern + "_);" + negPattern;
            }
            else if (gridColumn is GridPercentColumn)
            {
                format = "0";
                string percentSymbol = "[$" + numberFormat.PercentSymbol + "]";
                if (numberFormat.PercentDecimalDigits > 0)
                {
                    format += ".";
                    for (int i = 0; i < numberFormat.PercentDecimalDigits; i++)
                    {
                        format += "0";
                    }
                }
                switch (numberFormat.PercentPositivePattern)
                {
                    case 0:
                        posPattern = format + " " + percentSymbol;
                        break;
                    case 1:
                        posPattern = format + percentSymbol;
                        break;
                    case 2:
                        posPattern = percentSymbol + format;
                        break;
                    case 3:
                        posPattern = percentSymbol + " " + format;
                        break;
                    default:
                        posPattern = format + " " + percentSymbol;
                        break;
                }
                switch (numberFormat.PercentNegativePattern)
                {
                    case 0:
                        negPattern = "- " + format + percentSymbol;
                        break;
                    case 1:
                        negPattern = "-" + format + percentSymbol;
                        break;
                    case 2:
                        negPattern = "-" + percentSymbol + format;
                        break;
                    case 3:
                        negPattern = percentSymbol + "-" + format;
                        break;
                    case 4:
                        negPattern = percentSymbol + format + "-";
                        break;
                    case 5:
                        negPattern = format + "-" + percentSymbol;
                        break;
                    case 6:
                        negPattern = format + percentSymbol + "-";
                        break;
                    case 7:
                        negPattern = "-" + percentSymbol + " " + format;
                        break;
                    case 8:
                        negPattern = format + " " + percentSymbol + "-";
                        break;
                    case 9:
                        negPattern = percentSymbol + " " + format + "-";
                        break;
                    case 10:
                        negPattern = percentSymbol + " " + "-" + format;
                        break;
                    case 11:
                        negPattern = format + "- " + percentSymbol;
                        break;
                    default:
                        negPattern = "- " + format + percentSymbol;
                        break;
                }
                format = posPattern + "_);" + negPattern;
            }
            return format;
        }

        internal static NumberFormatInfo GetNumberFormatInfo(GridColumn gridColumn)
        {
            if (gridColumn is GridCurrencyColumn)
            {
                GridCurrencyColumn column = gridColumn as GridCurrencyColumn;

                if (!NumberFormatInfoDict.ContainsKey(gridColumn))
                {
                    var currencyNumberFormatInfo = new NumberFormatInfo
                    {
                        CurrencyDecimalDigits = column.CurrencyDecimalDigits,
                        CurrencyDecimalSeparator = column.CurrencyDecimalSeparator,
                        CurrencyGroupSeparator = column.CurrencyGroupSeparator,
                        CurrencyNegativePattern = column.CurrencyNegativePattern,
                        CurrencyPositivePattern = column.CurrencyPositivePattern,
                        CurrencySymbol = column.CurrencySymbol
                    };
                    NumberFormatInfoDict.Add(gridColumn, currencyNumberFormatInfo);
                    return currencyNumberFormatInfo;
                }
                else
                    return NumberFormatInfoDict[gridColumn];
            }
            else if (gridColumn is GridPercentColumn)
            {
                GridPercentColumn column = gridColumn as GridPercentColumn;

                if (!NumberFormatInfoDict.ContainsKey(gridColumn))
                {
                    var percentFormatInfo = new NumberFormatInfo
                    {
                        PercentDecimalDigits = column.PercentDecimalDigits,
                        PercentDecimalSeparator = column.PercentDecimalSeparator,
                        PercentGroupSeparator = column.PercentGroupSeparator,
                        PercentNegativePattern = column.PercentNegativePattern,
                        PercentPositivePattern = column.PercentPositivePattern,
                        PercentSymbol = column.PercentSymbol
                    };

                    NumberFormatInfoDict.Add(gridColumn, percentFormatInfo);
                    return percentFormatInfo;
                }
                else
                    return NumberFormatInfoDict[gridColumn];
            }
            else if (gridColumn is GridNumericColumn)
            {
                GridNumericColumn column = gridColumn as GridNumericColumn;

                if (!NumberFormatInfoDict.ContainsKey(gridColumn))
                {
                    var numericNumberFormatInfo = new NumberFormatInfo
                    {
                        NumberDecimalDigits = column.NumberDecimalDigits,
                        NumberDecimalSeparator = column.NumberDecimalSeparator,
                        NumberGroupSeparator = column.NumberGroupSeparator,
                        NumberNegativePattern = column.NumberNegativePattern
                    };
                    NumberFormatInfoDict.Add(gridColumn, numericNumberFormatInfo);
                    return numericNumberFormatInfo;
                }
                else
                    return NumberFormatInfoDict[gridColumn];
            }
            else
                return new NumberFormatInfo();
        }
#else
        internal static string GetExcelNumberFormat(GridColumn column, string formatString)
        {
            string format = "General";
            if (column is GridNumericColumn)
            {
                var numberFormat = CultureInfo.CurrentCulture.NumberFormat;
                formatString = formatString.ToUpper();
                switch (formatString)
                {
                    case "C":
                    case "C2":
                        format = "[$" + numberFormat.CurrencySymbol + "]" + "0.00";
                        break;
                    case "C0":
                        format = "[$" + numberFormat.CurrencySymbol + "]" + "0";
                        break;
                    case "C1":
                        format = "[$" + numberFormat.CurrencySymbol + "]" + "0.0";
                        break;
                    case "C3":
                        format = "[$" + numberFormat.CurrencySymbol + "]" + "0.000";
                        break;
                    case "C4":
                        format = "[$" + numberFormat.CurrencySymbol + "]" + "0.0000";
                        break;
                    case "E":
                    case "E4":
                        format = "0.0000E+00";
                        break;
                    case "E0":
                        format = "0E+00";
                        break;
                    case "E1":
                        format = "0.0E+00";
                        break;
                    case "E2":
                        format = "0.00E+00";
                        break;
                    case "E3":
                        format = "0.000E+00";
                        break;
                    case "F":
                    case "F2":
                        format = "0.00";
                        break;
                    case "F0":
                        format = "0";
                        break;
                    case "F1":
                        format = "0.0";
                        break;
                    case "F3":
                        format = "0.000";
                        break;
                    case "F4":
                        format = "0.0000";
                        break;
                    case "N":
                    case "N2":
                        format = "#,#0.00";
                        break;
                    case "N0":
                        format = "#,#0";
                        break;
                    case "N1":
                        format = "#,#0.0";
                        break;
                    case "N3":
                        format = "#,#0.000";
                        break;
                    case "N4":
                        format = "#,#0.0000";
                        break;
                    case "P":
                    case "P2":
                        format = "0.00" + "[$" + numberFormat.PercentSymbol + "]";
                        break;
                    case "P0":
                        format = "0" + "[$" + numberFormat.PercentSymbol + "]";
                        break;
                    case "P1":
                        format = "0.0" + "[$" + numberFormat.PercentSymbol + "]";
                        break;
                    case "P3":
                        format = "0.000" + "[$" + numberFormat.PercentSymbol + "]";
                        break;
                    case "P4":
                        format = "0.0000" + "[$" + numberFormat.PercentSymbol + "]";
                        break;
                    default:
                        format = "0.0";
                        break;
                }

            }
            if (column is GridDateTimeColumn)
            {
                var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
                switch (formatString)
                {
                    case "d":
                        format = dateTimeFormat.ShortDatePattern;
                        break;
                    case "D":
                        format = dateTimeFormat.LongDatePattern;
                        break;
                    case "f":
                        format = "[$-409]dd mmmm yyyy h:mm;@";
                        break;
                    case "F":
                        format = dateTimeFormat.FullDateTimePattern;
                        break;
                    case "g":
                        format = "dd-mm-yyyy h:mm";
                        break;
                    case "o":
                        format = dateTimeFormat.SortableDateTimePattern;
                        break;
                    case "G":
                        format = "dd-mm-yyyy h:mm:ss";
                        break;
                    case "M":
                        format = dateTimeFormat.MonthDayPattern;
                        break;
                    case "R":
                        format = "[$-409]ddd, dd mmmm yyyy h:mm:ss;@";
                        break;
                    case "t":
                        format = dateTimeFormat.ShortTimePattern;
                        break;
                    case "T":
                        format = dateTimeFormat.LongTimePattern;
                        break;
                    case "u":
                        format = dateTimeFormat.UniversalSortableDateTimePattern;
                        break;
                    case "Y":
                        format = dateTimeFormat.YearMonthPattern;
                        break;
                    default:
                        format = dateTimeFormat.FullDateTimePattern;
                        break;
                }
            }
            return format;
        }
#endif
       
    }
}
