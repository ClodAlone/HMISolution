#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Windows.GridCommon;
    using System.Windows.Controls;
    using System.IO;
    using System.Windows.Markup;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Controls.Grid.Resources;
    using System.Globalization;

    public static class GridDataResourceWrapper
    {
        
        //public static Style BorderPlusStyle
        //{
        //    get
        //    {
        //        var rd = GetCellRendererDictionary();
        //        return rd["BorderStylePlus"] as Style;
        //    }
        //}

        //public static Style BorderMinusStyle
        //{
        //    get
        //    {
        //        var rd = GetCellRendererDictionary();
        //        return rd["BorderStyleMinus"] as Style;
        //    }
        //}

        public static Brush TwilightBlueHover
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["TwilightBlueHover"] as Brush;
                return brush;
            }
        }

        public static System.Windows.Shapes.Path BorderPathAsc
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var bdStyle = rd["BorderStyleAsc"] as System.Windows.Shapes.Path;
                return bdStyle;
            }
        }

        public static System.Windows.Shapes.Path BorderPathDesc
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var bdStyle = rd["BorderStyleDesc"] as System.Windows.Shapes.Path;
                return bdStyle;
            }
        }

        public static Brush BlackAsterisk
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["BlackAsterisk"] as Brush;
                //brush.AlignmentX = AlignmentX.Center;
                //brush.AlignmentY = AlignmentY.Center;
                //brush.Stretch = Stretch.Uniform;
                return brush;
            }
        }

        public static Brush WhiteAsterisk
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["WhiteAsterisk"] as Brush;
                //brush.AlignmentX = AlignmentX.Center;
                //brush.AlignmentY = AlignmentY.Center;
                //brush.Stretch = Stretch.Uniform;
                return brush;
            }
        }

        private static ResourceDictionary GetCellRendererDictionary()
        {

            var stream = Application.GetResourceStream(new Uri("Syncfusion.Grid.Silverlight;component/GridDataControl/CellRenderers/Themes/Generic.brushes.xaml", UriKind.Relative)).Stream;
            var streamreader = new StreamReader(stream);
            var rd = XamlReader.Load(streamreader.ReadToEnd()) as ResourceDictionary;
            streamreader.Close();
            return rd;
        }

        private static ResourceDictionary GetThemeDictionary()
        {
            var stream = Application.GetResourceStream(new Uri("Syncfusion.Grid.Silverlight;component/GridDataControl/CellRenderers/Themes/Generic.themes.xaml", UriKind.Relative)).Stream;
            var streamreader = new StreamReader(stream);
            var rd = XamlReader.Load(streamreader.ReadToEnd()) as ResourceDictionary;
            streamreader.Close();
            return rd;
        }

        public static Canvas Plus
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["Plus"] as Canvas;
                return brush;
            }
        }

        public static Canvas Minus
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["Minus"] as Canvas;
                return brush;
            }
        }

        public static Canvas PlusOff14
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["PlusOff14"] as Canvas;
                return brush;
            }
        }

        public static Canvas MinusOff14
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["MinusOff14"] as Canvas;
                return brush;
            }
        }

        public static Brush StaticPencilEditing
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["StaticPencilEditing"] as Brush;
                return brush;
            }
        }

        public static Brush StaticPencil
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var brush = rd["StaticPencil"] as Brush;
                return brush;
            }
        }

        public static string NoRecordsfound
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "NoRecordsfound"); }

        }

        public static string InvalidDataTime
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "InvalidDataTime"); }

        }


        public static string InvalidDataToFilter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "InvalidDataToFilter");
            }

        }

        public static string DeleteMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DeleteMessage");
            }

        }

        public static string NoMoreItemRemoveMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NoMoreItemRemoveMessage");
            }

        }

        public static string NotSupportDeletingItemMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NotSupportDeletingItemMessage");
            }

        }


        public static string ConfirmDeleteMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ConfirmDeleteMessage");
            }

        }


        public static string CanntPerformSortMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "CanntPerformSortMessage");
            }

        }

        public static string NotEnoughSpaceMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NotEnoughSpaceMessage");
            }

        }


        public static string ClipboardCopyPaste
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ClipboardCopyPaste");
            }

        }

        public static string AllFilter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AllFilter");
            }
        }

        public static string SelectAllFilter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "SelectAllFilter");
            }

        }

        public static string EnterFilterValue
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "EnterFilterValue");
            }

        }

        public static string SlideToFilter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "SlideToFilter");
            }

        }

        public static string SelectFilterDate
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "SelectFilterDate");
            }

        }

        public static string FilterDateTo
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "FilterDateTo");
            }

        }

        public static string Clear
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Clear");
            }

        }

        public static string GreaterThan
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "GreaterThan");
            }

        }

        public static string GreaterThanOrEqual
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "GreaterThanOrEqual");
            }

        }

        public static string LessThan
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "LessThan");
            }

        }

        public static string LessThanOrEqual
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "LessThanOrEqual");
            }

        }

        public static string EqualsString
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "EqualsString");
            }

        }

        public static string NotEquals
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NotEquals");
            }
        }

        public static string AND
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AND");
            }

        }

        public static string OR
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "OR");
            }

        }

        public static string MatchCase
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "MatchCase");
            }

        }

        public static string Contains
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Contains");
            }

        }

        public static string EndsWith
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "EndsWith");
            }

        }

        public static string Between
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Between");
            }

        }
        
        public static string StartsWith
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "StartsWith");
            }

        }

        public static string AdvanceFilteringEqualsString
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AdvanceFilteringEqualsString");
            }

        }

        public static string AdvanceFilteringGreaterThanString
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AdvanceFilteringGreaterThanString");
            }

        }

        public static string AdvnaceFilteringBetweenString
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AdvnaceFilteringBetweenString");
            }

        }

        public static string AdvanceFilteringContainsString
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AdvanceFilteringContainsString");
            }

        }


        //******************
        // Important Note: The following properties are declared but blocked 
        // because these features are not implemented yet for Grid Silverlight. 
        // Once ColumnOptions feature is implemented make use of these properties
        //******************

        //public static string DynamicOptions
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "DynamicOptions");
        //    }

        //}

        //public static string HeaderText
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "HeaderText");
        //    }

        //}

        //public static string AllowFilter
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "AllowFilter");
        //    }

        //}

        //public static string AllowSort
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "AllowSort");
        //    }

        //}

        //public static string AllowDrag
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "AllowDrag");
        //    }

        //}

        //public static string AllowResize
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "AllowResize");
        //    }

        //}

        //public static string IsReadOnly
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "IsReadOnly");
        //    }

        //}

        //public static string AllowGroup
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "AllowGroup");
        //    }

        //}

        //public static string Vertical
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "Vertical");
        //    }

        //}

        //public static string VTop
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "VTop");
        //    }

        //}

        //public static string VCenter
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "VCenter");
        //    }

        //}

        //public static string VBottom
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "VBottom");
        //    }

        //}

        //public static string VStretch
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "VStretch");
        //    }

        //}

        //public static string Horizontal
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "Horizontal");
        //    }

        //}

        //public static string HLeft
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "HLeft");
        //    }

        //}

        //public static string HCenter
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "HCenter");
        //    }

        //}

        //public static string HRight
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "HRight");
        //    }

        //}

        //public static string HStretch
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "HStretch");
        //    }

        //}

        //public static string ColumnFormat
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "ColumnFormat");
        //    }

        //}

        //public static string WidthOptions
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "WidthOptions");
        //    }

        //}

        //public static string AutoFit
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "AutoFit");
        //    }

        //}

        //public static string ApplyWidthSettings
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "ApplyWidthSettings");
        //    }

        //}

        //public static string Width
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "Width");
        //    }

        //}

        //public static string TextAlignment
        //{
        //    get
        //    {
        //        return SR.GetString(CultureInfo.CurrentUICulture, "TextAlignment");
        //    }

        //}

        //******************
        // Important Note: The above properties are declared but blocked 
        // because these features are not implemented yet for Grid Silverlight. 
        // Once ColumnOptions feature is implemented make use of these properties
        //******************

        public static string None
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "None");
            }

        }

        public static string DragDropText
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DragDropText");
            }

        }

        public static string InvalidColumn
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "InvalidColumn");
            }

        }

        //For ExcelLike Filtering

        public static string NumberFilters
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NumberFilters");
            }

        }

        public static string TextFilters
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "TextFilters");
            }

        }

        public static string DateFilters
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DateFilters");
            }

        }

        public static string NoItemMatch
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NoItemMatch");
            }

        }


        public static string NumberAscending
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NumberAscending");
            }

        }

        public static string NumberDescending
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NumberDescending");
            }

        }

        public static string StringAscending
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "StringAscending");
            }

        }

        public static string StringDescending
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "StringDescending");
            }

        }

        public static string DateTimeAscending
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DateTimeAscending");
            }

        }

        public static string BetweenAdvance
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "BetweenAdvance");
            }

        }         

        public static string DateTimeDescending
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DateTimeDescending");
            }

        }      

        public static string BlankFilterString
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "BlankFilterString");
            }

        }  
    }

    public class ResourceHelper
    {
        public string EnterFilterValue { get { return GridDataResourceWrapper.EnterFilterValue; } }
        public string Clear { get { return GridDataResourceWrapper.Clear; } }
        public string None { get { return GridDataResourceWrapper.None; } }
        public string GreaterThan { get { return GridDataResourceWrapper.GreaterThan; } }
        public string GreaterThanOrEqual { get { return GridDataResourceWrapper.GreaterThanOrEqual; } }
        public string LessThan { get { return GridDataResourceWrapper.LessThan; } }
        public string LessThanOrEqual { get { return GridDataResourceWrapper.LessThanOrEqual; } }
        public string EqualsString { get { return GridDataResourceWrapper.EqualsString; } }
        public string NotEquals { get { return GridDataResourceWrapper.NotEquals; } }
        public string SlideToFilter { get { return GridDataResourceWrapper.SlideToFilter; } }
        public string FilterDateTo { get { return GridDataResourceWrapper.FilterDateTo; } }
        public string SelectFilterDate { get { return GridDataResourceWrapper.SelectFilterDate; } }
        public string NoItemMatch { get { return GridDataResourceWrapper.NoItemMatch; } }
        public string SelectAllFilter { get { return GridDataResourceWrapper.SelectAllFilter; } }
        public string OR { get { return GridDataResourceWrapper.OR; } }
        public string AND { get { return GridDataResourceWrapper.AND; } }
        public string MatchCase { get { return GridDataResourceWrapper.MatchCase; } }
    }
}
