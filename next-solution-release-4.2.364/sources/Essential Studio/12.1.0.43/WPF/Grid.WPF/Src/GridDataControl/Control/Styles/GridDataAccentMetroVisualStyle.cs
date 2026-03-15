#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Grid
{

    public class GridDataAccentMetroVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush MetroBorderBrush { get; set; }
        public Brush MetroBrush { get; set; }
        public Brush MetroFocusedBorderBrush { get; set; }
        public FontFamily MetroFontFamily { get; set; }
        public Brush MetroForegroundBrush { get; set; }
        public Brush MetroHighlightedForegroundBrush { get; set; }
        public Brush MetroHoverBrush { get; set; }
        public Brush MetroPanelBackgroundBrush { get; set; }

        public void UpdateVisualStyle()
        {

        }

        ResourceDictionary rd;
        public GridDataAccentMetroVisualStyle()
        {
            rd = new ResourceDictionary();
            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/MetroStyle.xaml",
                                UriKind.RelativeOrAbsolute);

            MetroBorderBrush = rd["MetroBorderBrush"] as Brush;
            MetroBrush = rd["MetroBrush"] as Brush;
            MetroFocusedBorderBrush = rd["MetroFocusedBorderBrush"] as Brush;
            MetroFontFamily = rd["MetroFontFamily"] as FontFamily;
            MetroForegroundBrush = rd["MetroForegroundBrush"] as Brush;
            MetroHighlightedForegroundBrush = rd["MetroHighlightedForegroundBrush"] as Brush;
            MetroHoverBrush = rd["MetroHoverBrush"] as Brush;
            MetroPanelBackgroundBrush = rd["MetroPanelBackgroundBrush"] as Brush;

        }

        public Brush RowHeaderBackground
        {
            get { return MetroBrush; }
        }

        public Brush HoveringRecordCellForeground
        {
            get { return MetroHighlightedForegroundBrush; }
        }

        public Brush HeaderBackgroundBrush
        {
            get { return MetroBrush; }
        }

        public Brush HeaderForegroundBrush
        {
            get { return MetroHighlightedForegroundBrush; }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get { return MetroPanelBackgroundBrush; }
        }

        public Brush FilterPopupForegroundBrush
        {
            get { return ValueForegroundBrush; }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                return GridUtil.GetHoverColor(MetroBrush, 0.40); 
            }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get { return this.HeaderForegroundBrush; }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(MetroBorderBrush, 0.18d),
                    Right = new Pen(MetroBorderBrush, 0.18d)
                };
            }
        }

        public Brush HeaderInnerBorder
        {
            get { return MetroBorderBrush; }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get { return new Thickness(0.5, 0, 0.5, 1); }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = MetroFontFamily,
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal
                };
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(MetroBorderBrush, 0.22d),
                    Bottom = new Pen(MetroBorderBrush, 0.22d), // newly added left and right
                    Left = new Pen(MetroBorderBrush, 0.22d),
                    Right = new Pen(MetroBorderBrush, 0.22d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF333333"));
            }

        }

        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));
            }
        }

        public CellMarginsInfo ValueTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = MetroFontFamily,
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        //change
        public Brush SortWidgetBrush
        {
            get { return GridUtil.GetHoverColor(MetroForegroundBrush, 0.40); }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get { return MetroPanelBackgroundBrush; }
        }

        public Brush GroupAreaForegroundBrush
        {
            get { return MetroForegroundBrush; }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = MetroFontFamily,
                    FontSize = 13d,
                };
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(MetroBorderBrush, 0.25d),
                    Bottom = new Pen(MetroBorderBrush, 0.25d), // newly added left and right
                    Left = new Pen(MetroBorderBrush, 0.25d),
                    Right = new Pen(MetroBorderBrush, 0.25d)
                };
            }
        }

        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1, 1, 2, 2));
            }
        }

        public Brush SummaryCaptionBackground
        {
            get
            {
                return GridUtil.GetHoverColor(MetroBrush, 0.75);
                //return MetroBrush;
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = MetroFontFamily,
                    FontSize = 13d,
                };
            }
        }

        public Brush SummaryRowBackground
        {
            get { return this.SummaryCaptionBackground; }
        }

        public Brush SummaryRowForeground
        {
            get { return this.HeaderForegroundBrush; }
        }

        public GridFontInfo SummaryRowFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush RowHeaderSelectionBackground
        {
            get { return this.HeaderHoverBackgroundBrush; }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return GridUtil.GetHoverColor(MetroForegroundBrush, 0.40);
            }
        }

        public Brush HighlightBrush
        {
            get { return MetroBrush; }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return MetroBorderBrush;
            }
        }

        public double CurrentCellBorderWidth
        {
            get { return 0.5d; }
        }

        public Brush HighlightSelectionBackground
        {
            get { return MetroBrush; }
        }

        public Brush HighlightSelectionForeground
        {
            get { return MetroHighlightedForegroundBrush; }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get { return MetroPanelBackgroundBrush; }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get { return ValueForegroundBrush; }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get { return this.HeaderForegroundBrush; }
        }

        public Brush ColumnOptionsButtonBorderBrush
        {
            get { return Brushes.Transparent; } // MetroBorderBrush; }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get { return MetroForegroundBrush; }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                // return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));
                return this.HeaderForegroundBrush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return Brushes.Transparent;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                // return GridUtil.GetXamlConvertedValue<Brush>("#F4FCFF");
                return HeaderHoverForegroundBrush;
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Brush>("#000000");
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get { return MetroForegroundBrush; }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.RowHeaderForeground;
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return MetroBorderBrush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return this.RowHeaderForeground;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.RowHeaderForeground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return MetroBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                // return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));
                return this.RowHeaderForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {
                // return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF41B1E1"));
                return this.HeaderHoverBackgroundBrush;
            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                // return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFB8BBBC"));
                return MetroBorderBrush;
            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                //return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));
                return RowHeaderForeground;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                //return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));
                return MetroBrush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                //return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));
                return RowHeaderForeground;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));
            }
        }

        public Brush DragDropIndicatorBrush
        {
            get
            {
                // return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC6C6C6"));
                return MetroBrush;
            }
        }

        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));
            }
        }

        private Brush alternateRowBackground = null;

        public Brush AlternateRowBackground
        {
            get
            {
                if (alternateRowBackground == null)
                {
                    alternateRowBackground = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFF6F6F6"));
                }
                return alternateRowBackground;
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFDADADA"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return GridUtil.GetHoverColor(MetroBrush, 0.40);
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return MetroForegroundBrush;
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return GridUtil.GetHoverColor(MetroBrush, 0.40);
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return MetroHighlightedForegroundBrush; }
        }

        //change
        public Brush HeaderOptionsHoverBackground
        {
            get { 
                return GridUtil.GetHoverColor(MetroBrush, 0.45); //new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF67D9FF"));
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get { return HeaderHoverBackgroundBrush; }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get { return MetroBorderBrush; }
        }

        public Brush GridBorderBrush
        {
            get
            {
                // return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#D1D1D1"));
                return MetroBorderBrush;
            }
        }

        public Thickness GridBorderThickness
        {
            get
            {
                return new Thickness(1d);
            }
        }

        public Brush HoveringGroupCaptionCellBackground
        {
            get
            {
                // return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFDADADA"));
                return GridUtil.GetHoverColor(MetroHoverBrush, 0.1);
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(MetroBorderBrush, 1d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(MetroBorderBrush, 1d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(MetroBorderBrush, 1d)
                };
            }
        }

        #endregion
    }

}
