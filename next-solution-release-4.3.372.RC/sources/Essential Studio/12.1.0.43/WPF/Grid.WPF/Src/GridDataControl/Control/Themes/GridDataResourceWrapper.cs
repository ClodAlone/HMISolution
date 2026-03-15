#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Reflection;

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Windows.GridCommon;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Globalization;
    using Syncfusion.Windows.Controls.Grid.Resources;

    public static class GridDataResourceWrapper
    {
        public static void SetResources(Assembly assembly)
        {
            SR.SetResources(assembly);
        }

        public static Brush TwilightBlueNormal
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var brush = RdBrushes["TwilightBlueNormal"] as Brush;
                return brush;
            }
        }

        public static Brush TwilightBlueHover
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var brush = RdBrushes["TwilightBlueHover"] as Brush;
                return brush;
            }
        }

        public static Style FilterStyle
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var filterStyle = RdBrushes["FilterButtonStyle"] as Style;
                return filterStyle;
            }
        }

        public static Geometry BorderPathDesc
        {
            get
            {
                var geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 101,104.333L 104.333,100.999L 107.667,97.6667L 101,97.6667L 94.3333,97.6667L 97.6667,100.999L 101,104.333 Z ");
                return geometry;
            }
        }

        public static Geometry BorderPathAsc
        {
            get
            {
                var geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 146.529,135.153L 156.997,145.62L 136.061,145.62L 146.529,135.153 Z ");
                return geometry;
            }
        }

        public static Brush BlackAsterisk
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var brush = RdBrushes["BlackAsterisk"] as DrawingBrush;
                brush.AlignmentX = AlignmentX.Center;
                brush.AlignmentY = AlignmentY.Center;
                brush.Stretch = Stretch.Uniform;
                return brush;
            }
        }

        public static Brush WhiteAsterisk
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var brush = RdBrushes["WhiteAsterisk"] as DrawingBrush;
                brush.AlignmentX = AlignmentX.Center;
                brush.AlignmentY = AlignmentY.Center;
                brush.Stretch = Stretch.Uniform;
                return brush;
            }
        }
        //ResouceDictionary was defined frequently for small changes. So, to avoid this Resource Dictionary was defined as an private method and used whenever required.
        private static ResourceDictionary rdBrushes;
        private static ResourceDictionary RdBrushes
        {
            get
            {
                if (rdBrushes == null)
                {
                    rdBrushes = new ResourceDictionary()
                    {
                        Source = new Uri("/Syncfusion.Grid.Wpf;component/GridDataControl/CellRenderers/Themes/Generic.brushes.xaml", UriKind.RelativeOrAbsolute)
                    };
                }
                return rdBrushes;
            }
        }

        private static ResourceDictionary rdThemes;
        private static ResourceDictionary RdThemes
        {
            get
            {
                if (rdThemes == null)
                {
                    rdThemes = new ResourceDictionary()
                    {
                        Source = new Uri("/Syncfusion.Grid.Wpf;component/GridDataControl/CellRenderers/Themes/Generic.themes.xaml", UriKind.RelativeOrAbsolute)
                    };
                }
                return rdThemes;
            }
        }

        private static ResourceDictionary GetCellRendererDictionary()
        {
            ResourceDictionary rd = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Grid.Wpf;component/GridDataControl/CellRenderers/Themes/Generic.brushes.xaml", UriKind.RelativeOrAbsolute)
            };
            return rd;
        }

        private static ResourceDictionary GetThemeDictionary()
        {
            ResourceDictionary rd = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Grid.Wpf;component/GridDataControl/CellRenderers/Themes/Generic.themes.xaml", UriKind.RelativeOrAbsolute)
            };
            //rd.Source = GridUtil.GetXamlConvertedValue<Uri>("Syncfusion.Grid.Wpf;component/GridDataControl/CellRenderers/Themes/Generic.themes.xaml");
            return rd;
        }

        public static System.Windows.Controls.Grid PlusGrid
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var grid = RdBrushes["PlusGrid"] as System.Windows.Controls.Grid;
                return grid;
            }
        }

        public static System.Windows.Controls.Grid MinusGrid
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var grid = RdBrushes["MinusGrid"] as System.Windows.Controls.Grid;
                return grid;
            }
        }

        public static System.Windows.Controls.Grid LegacyPlusGrid
        {
            get
            {
                var rd = GetCellRendererDictionary();
                var grid = rd["LegacyStylePlusGrid"] as System.Windows.Controls.Grid;
                return grid;
            }
        }

        public static System.Windows.Controls.Grid LegacyMinusGrid
        {
            get
            {
                //var rd = GetCellRendererDictionary();          
                //RdBrushes.FindName["LegacyStyleMinusGrid"] as new System.Windows.Controls.Grid();
                //var newgrid = rd["LegacyStyleMinusGrid"] as System.Windows.Controls.Grid;
                //return newgrid;
                return (System.Windows.Controls.Grid)RdBrushes["LegacyStyleMinusGrid"];// as System.Windows.Controls.Grid;
            }
        }

        public static System.Windows.Controls.Grid LegacyPlusOff14
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var grid = RdBrushes["LegacyPlusOff14"] as System.Windows.Controls.Grid;
                return grid;
            }
        }

        public static System.Windows.Controls.Grid LegacyMinusOff14
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var grid = RdBrushes["LegacyMinusOff14"] as System.Windows.Controls.Grid;
                return grid;
            }
        }

        public static System.Windows.Controls.Grid PlusOff14
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var grid = RdBrushes["PlusOff14"] as System.Windows.Controls.Grid;
                return grid;
            }
        }

        public static System.Windows.Controls.Grid MinusOff14
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var grid = RdBrushes["MinusOff14"] as System.Windows.Controls.Grid;
                return grid;
            }
        }

        public static Brush Plus
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var brush = RdBrushes["Plus"] as Brush;
                return brush;
            }
        }

        public static Brush Minus
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var brush = RdBrushes["Minus"] as Brush;
                return brush;
            }
        }

        public static Brush StaticPencilEditing
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var brush = RdBrushes["StaticPencilEditing"] as Brush;
                return brush;
            }
        }

        public static Brush StaticPencil
        {
            get
            {
                //var rd = GetCellRendererDictionary();
                var brush = RdBrushes["StaticPencil"] as Brush;
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

        public static string Clear
        {
            get
            {                
                return SR.GetString(CultureInfo.CurrentUICulture, "Clear");
            }

        }

        public static string ClearFilter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ClearFilter");
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

        public static string ShowColumnOptionsFilter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ShowColumnOptionsFilter");
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

        public static string Search
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Search");
            }
        }

        public static string SelectAll
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "SelectAll");
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

        public static string DynamicOptions
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DynamicOptions");
            }

        }

        public static string HeaderText
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "HeaderText");
            }

        }
        static string allowFilter;
        public static string AllowFilter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AllowFilter");
            }
            set { allowFilter = value; }
        }

        public static string AllowSort
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AllowSort");
            }

        }

        public static string AllowDrag
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AllowDrag");
            }

        }

       
        public static string SetFrozenColumn
        {
            get
            {
                
                return SR.GetString(CultureInfo.CurrentUICulture, "SetFrozenColumn");
            }

        }
        

        public static string AllowResize
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AllowResize");
            }

        }

        public static string IsReadOnly
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "IsReadOnly");
            }

        }

        public static string AllowGroup
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AllowGroup");
            }

        }

        public static string Vertical
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Vertical");
            }

        }

        public static string VTop
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "VTop");
            }

        }

        public static string VCenter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "VCenter");
            }

        }

        public static string VBottom
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "VBottom");
            }

        }

        public static string VStretch
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "VStretch");
            }

        }

        public static string Horizontal
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Horizontal");
            }

        }

        public static string HLeft
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "HLeft");
            }

        }

        public static string HCenter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "HCenter");
            }

        }

        public static string HRight
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "HRight");
            }

        }

        public static string HStretch
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "HStretch");
            }

        }

        public static string ColumnFormat
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ColumnFormat");
            }

        }

        public static string WidthOptions
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "WidthOptions");
            }

        }

        public static string AutoFit
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AutoFit");
            }

        }

        public static string ApplyWidthSettings
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ApplyWidthSettings");
            }

        }

        public static string Width
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Width");
            }

        }

        public static string TextAlignment
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "TextAlignment");
            }

        }

        public static string None
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "None");
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

        //  public static string NotEquals1
        //  {
        //      get
        //      {
        //          var rd = GetCultureInfoDictionary();
        //          return SR.GetString(CultureInfo.CurrentUICulture, " rd["NotEquals1"] as string;
        //      }
        //  }

        //public static string Equals1
        //  {
        //      get
        //      {
        //          var rd = GetCultureInfoDictionary();
        //          return SR.GetString(CultureInfo.CurrentUICulture, " rd["Equals1"] as string;
        //      }
        //  }


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

        //Temp
        public static string CurrentPagePrefix_TotalPageCountKnown
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "CurrentPagePrefix_TotalPageCountKnown");
            }

        }

        public static string AutoEllipsisString
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AutoEllipsisString");
            }

        }

        public static string AutomationPeerName_TotalPageCountKnown
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AutomationPeerName_TotalPageCountKnown");
            }

        }

        public static string AutomationPeerName_TotalPageCountUnknown
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AutomationPeerName_TotalPageCountUnknown");
            }

        }


        public static string CurrentPageText
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "CurrentPageText");
            }
        }

        public static string CurrentPagePrefix_TotalPageCountUnknown
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "CurrentPagePrefix_TotalPageCountUnknown");
            }

        }

        public static string CurrentPageSuffix_TotalPageCountKnown
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "CurrentPageSuffix_TotalPageCountKnown");
            }

        }

        public static string CurrentPageSuffix_TotalPageCountUnknown
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "CurrentPageSuffix_TotalPageCountUnknown");
            }

        }

        public static string InvalidButtonPanelContent
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "InvalidButtonPanelContent");
            }

        }

        public static string InvalidTimeSpan
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "InvalidTimeSpan");
            }

        }

        public static string PageIndexMustBeNegativeOne
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "PageIndexMustBeNegativeOne");
            }

        }

        public static string UnderlyingPropertyIsReadOnly
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "UnderlyingPropertyIsReadOnly");
            }

        }

        public static string ValueMustBeGreaterThanOrEqualTo
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ValueMustBeGreaterThanOrEqualTo");
            }

        }
        //Till this

        //For ExcelLike Filtering

        public static string EqualsSmall
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "EqualsSmall");
            }

        }

        public static string DoesNotEquals
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DoesNotEquals");
            }

        }

        public static string IsGreaterThan
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "IsGreaterThan");
            }

        }

        public static string IsGreaterThanOrEqualto
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "IsGreaterThanOrEqualto");
            }

        }

        public static string IsLessThan
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "IsLessThan");
            }

        }

        public static string IsLessThanorEqualto
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "IsLessThanorEqualto");
            }

        }

        public static string BeginsWith
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "BeginsWith");
            }

        }

        public static string DoesNotBeginsWith
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DoesNotBeginsWith");
            }

        }

        public static string ContainsSmall
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ContainsSmall");
            }

        }

        public static string EndsWithSmall
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "EndsWithSmall");
            }

        }

        public static string DoesNotEndWith
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DoesNotEndWith");
            }

        }

        public static string DoesNotContain
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DoesNotContain");
            }

        }

        public static string OK
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "OK");
            }
        }

        public static string Cancel
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Cancel");
            }

        }

        public static string ShowRowsWhere
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ShowRowsWhere");
            }

        }

        public static string UseSingleCharacter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "UseSingleCharacter");
            }

        }

        public static string UseSeriesCharacter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "UseSeriesCharacter");
            }

        }

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

        public static string DoesNotEqual
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DoesNotEqual");
            }

        }

        public static string BeginsWithCaps
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "BeginsWithCaps");
            }

        }

        public static string DoesNotContainCaps
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DoesNotContainCaps");
            }

        }

        public static string EndsWithCaps
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "EndsWithCaps");
            }

        }

        public static string Top10
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Top10");
            }

        }

        public static string AboveAverage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AboveAverage");
            }

        }

        public static string BelowAverage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "BelowAverage");
            }

        }
        

        public static string GreaterThanOrEqualTo
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "GreaterThanOrEqualTo");
            }

        }

        public static string LessThanWithSpace
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "LessThanWithSpace");
            }

        }

        public static string LessThanOrEqualTo
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "LessThanOrEqualTo");
            }

        }

        public static string Before
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Before");
            }

        }

        public static string After
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "After");
            }

        }

        public static string Tomorrow
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Tomorrow");
            }

        }

        public static string Today  
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Today");
            }

        }

        public static string Yesterday
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Yesterday");
            }

        }

        public static string NextWeek
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NextWeek");
            }

        }

        public static string ThisWeek
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ThisWeek");
            }

        }

        public static string LastWeek
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "LastWeek");
            }

        }

        public static string NextMonth
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NextMonth");
            }

        }

        public static string ThisMonth
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ThisMonth");
            }

        }

        public static string LastMonth
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "LastMonth");
            }

        }

        public static string NextYear
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NextYear");
            }

        }

        public static string ThisYear
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ThisYear");
            }

        }

        public static string LastYear
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "LastYear");
            }

        }

        public static string isbefore
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "isbefore");
            }

        }

        public static string isafter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "isafter");
            }

        }

        public static string isafterorequalto
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "isafterorequalto");
            }

        }

        public static string isbeforeorequalto
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "isbeforeorequalto");
            }

        }

        public static string NoItemMatch
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "NoItemMatch");
            }

        }

        public static string CustomAutoFilter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "CustomAutoFilter");
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

        public static string DateTimeDescending
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "DateTimeDescending");
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

        

        public static string BlankFilterText
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "BlankFilterText");
            }

        }          
        
        
        //Till THis

    }
}
