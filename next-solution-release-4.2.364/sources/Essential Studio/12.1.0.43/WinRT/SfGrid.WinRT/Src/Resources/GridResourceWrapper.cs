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
using System.IO;
using System.Linq;
using System.Text;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public static class GridResourceWrapper
    {
        public static string SelectAll
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "SelectAll"); }
        }

        public static string AND
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "AND"); }
        }

        public static string OR
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "OR"); }
        }

        public static string AdvancedFiltersButtonText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "AdvancedFiltersButtonText"); }
        }

        public static string ShowRowsWhere
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ShowRowsWhere"); }
        }

        public static string Blanks
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Blanks"); }
        }

        public static string Cancel
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Cancel"); }
        }

        public static string Done
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Done"); }
        }

        public static string ClearFilter
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ClearFilter"); }
        }

        public static string NoMatches
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "NoMatches"); }
        }

        public static string OK
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "OK"); }
        }

        public static string Search
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Search"); }
        }

        public static string SortNumberAscending
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "SortNumberAscending"); }
        }

        public static string SortNumberDescending
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "SortNumberDescending"); }
        }
        public static string SortDateAscending
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "SortDateAscending"); }
        }

        public static string SortDateDescending
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "SortDateDescending"); }
        }
        public static string SortStringAscending
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "SortStringAscending"); }
        }

        public static string SortStringDescending
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "SortStringDescending"); }
        }

        public static string NoItems
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "NoItems"); }
        }

        public static string RowErrorMessage
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "RowErrorMessage"); }
        }

        public static string ColumnChooserTitle
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "ColumnChooserTitle"); }
        }

        public static string ColumnChooserWaterMark
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "ColumnChooserWaterMark"); }
        }

        public static string AddNewRowText
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "AddNewRowText"); }
        }

        public static string GroupDropAreaText
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "GroupDropAreaText"); }
        }

        public static string PrintPreview
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "PrintPreview"); }
        }

        public static string Print
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "Print"); }
        }

        public static string Equalss
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "Equalss"); }
        }

        public static string NotEquals
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "NotEquals"); }
        }

        public static string BeginsWith
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "BeginsWith"); }
        }

        public static string EndsWith
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "EndsWith"); }
        }

        public static string Contains
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "Contains"); }
        }

        public static string NotContains
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "NotContains"); }
        }

        public static string Empty
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "Empty"); }
        }

        public static string NotEmpty
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "NotEmpty"); }
        }

        public static string Null
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "Null"); }
        }

        public static string NotNull
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "NotNull"); }
        }

        public static string LessThanorEqual
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "LessThanorEqual"); }
        }

        public static string LessThan
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "LessThan"); }
        }
        public static string GreaterThan
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "GreaterThan"); }
        }

        public static string GreaterThanorEqual
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "GreaterThanorEqual"); }
        }
        public static string Before
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "Before"); }
        }
        public static string BeforeOrEqual
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "BeforeOrEqual"); }
        }

        public static string After
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "After"); }
        }

        public static string AfterOrEqual
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "AfterOrEqual"); }
        }

        public static string EnterValidFilterValue
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "EnterValidFilterValue"); }
        }
        public static string TextFilters
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "TextFilters"); }
        }
        public static string NumberFilters
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "NumberFilters"); }
        }
        public static string DateFilters
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "DateFilters"); }
        }
#if !SILVERLIGHT

        public static string QuickPrint
        {
            get { return SR.GetString(CultureInfo.CurrentCulture, "QuickPrint"); }
        }

#endif
    }

    public class ResourceHelper
    {
        public string SelectAll { get { return GridResourceWrapper.SelectAll; } }
        public string AdvancedFilters { get { return GridResourceWrapper.AdvancedFiltersButtonText; } }
        public string AND { get { return GridResourceWrapper.AND; } }
        public string OR { get { return GridResourceWrapper.OR; } }
        public string ShowRowsWhere { get { return GridResourceWrapper.ShowRowsWhere; } }
        public string Blanks { get { return GridResourceWrapper.Blanks; } }
        public string Cancel { get { return GridResourceWrapper.Cancel; } }
        public string ClearFilter { get { return GridResourceWrapper.ClearFilter; } }
        public string NoMatches { get { return GridResourceWrapper.NoMatches; } }
        public string OK { get { return GridResourceWrapper.OK; } }
        public string RowErrorMessage { get { return GridResourceWrapper.RowErrorMessage; } }
        public string Search { get { return GridResourceWrapper.Search; } }
        public string SortNumberAscending { get { return GridResourceWrapper.SortNumberAscending; } }
        public string SortNumberDescending { get { return GridResourceWrapper.SortNumberDescending; } }
        public string SortStringAscending { get { return GridResourceWrapper.SortStringAscending; } }
        public string SortStringDescending { get { return GridResourceWrapper.SortStringDescending; } }
        public string NoItems { get { return GridResourceWrapper.NoItems; } }
        public string ColumnChooserTitle { get { return GridResourceWrapper.ColumnChooserTitle; } }
        public string ColumnChooserWaterMark { get { return GridResourceWrapper.ColumnChooserWaterMark; } }
        public string AddNewRowText { get { return GridResourceWrapper.AddNewRowText; } }
        public string GroupDropAreaText { get { return GridResourceWrapper.GroupDropAreaText; } }
        public string Print { get { return GridResourceWrapper.Print; } }
        public string PrintPreview { get { return GridResourceWrapper.PrintPreview; } }
        public string Equalss { get { return GridResourceWrapper.Equalss; } }
        public string NotEquals { get { return GridResourceWrapper.NotEquals; } }
        public string BeginsWith { get { return GridResourceWrapper.BeginsWith; } }
        public string EndsWith { get { return GridResourceWrapper.EndsWith; } }
        public string Contains { get { return GridResourceWrapper.Contains; } }
        public string NotContains { get { return GridResourceWrapper.NotContains; } }
        public string Empty { get { return GridResourceWrapper.Empty; } }
        public string NotEmpty { get { return GridResourceWrapper.NotEmpty; } }
        public string Null { get { return GridResourceWrapper.Null; } }
        public string NotNull { get { return GridResourceWrapper.NotNull; } }
        public string LessThan { get { return GridResourceWrapper.LessThan; } }
        public string LessThanorEqual { get { return GridResourceWrapper.LessThanorEqual; } }
        public string GreaterThan { get { return GridResourceWrapper.GreaterThan; } }
        public string GreaterThanorEqual { get { return GridResourceWrapper.GreaterThanorEqual; } }
    }
}
