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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.GridCommon;

    public interface IGridDataVisualStyle
    {
        /// <summary>
        /// Gets the row header background.
        /// </summary>
        /// <value>The row header background.</value>
        Brush RowHeaderBackground { get; }

        /// <summary>
        /// Gets the background brush for the Header cell.
        /// </summary>
        /// <value>The header background brush.</value>
        Brush HeaderBackgroundBrush { get; }

        /// <summary>
        /// Gets the foreground brush for the Header cell.
        /// </summary>
        /// <value>The header foreground brush.</value>
        Brush HeaderForegroundBrush { get; }

        /// <summary>
        /// Gets the hover background brush for the Header cell.
        /// </summary>
        /// <value>The header hover background brush.</value>
        Brush HeaderHoverBackgroundBrush { get; }

        /// <summary>
        /// Gets the Cell border information for the Header cell area
        /// </summary>
        /// <value>The header cell borders.</value>
        CellBordersInfo HeaderCellBorders { get; }

        /// <summary>
        /// Gets the header control's border which says as inner border's brush for all directions.
        /// </summary>
        /// <value>The header inner border.</value>
        Brush HeaderInnerBorder { get; }

        /// <summary>
        /// Gets the filter popup background brush.
        /// </summary>
        /// <value>The filter popup background brush.</value>
        Brush FilterPopupBackgroundBrush { get; }

        /// <summary>
        /// Gets the filter popup foreground brush.
        /// </summary>
        /// <value>The filter popup foreground brush.</value>
        Brush FilterPopupForegroundBrush { get; }

        /// <summary>
        /// Gets the header control's border which says as inner border's thickness for all directions.
        /// </summary>
        /// <value>The header inner border thickness.</value>
        Thickness HeaderInnerBorderThickness { get; }

        /// <summary>
        /// Gets the font information of the Header cell.
        /// </summary>
        /// <value>The header font.</value>
        GridFontInfo HeaderFont { get; }

        /// <summary>
        /// Gets the text’s margin of a value cell.
        /// </summary>
        /// <value>The header text margins.</value>
        CellMarginsInfo HeaderTextMargins { get; }

        /// <summary>
        /// Gets the Cell border information of a value cell.
        /// </summary>
        /// <value>The value cell borders.</value>
        CellBordersInfo ValueCellBorders { get; }

        /// <summary>
        /// Gets the foreground brush of a value cell.
        /// </summary>
        /// <value>The value foreground brush.</value>
        Brush ValueForegroundBrush { get; }

        /// <summary>
        /// Gets the background brush of a value cell.
        /// </summary>
        /// <value>The value background brush.</value>
        Brush ValueBackgroundBrush { get; }

        /// <summary>
        /// Gets the value text margins.
        /// </summary>
        /// <value>The value text margins.</value>
        CellMarginsInfo ValueTextMargins { get; }

        /// <summary>
        /// Gets the font information of a value cell.
        /// </summary>
        /// <value>The value font.</value>
        GridFontInfo ValueFont { get; }

        /// <summary>
        /// Gets the brush for the sort icon.
        /// </summary>
        /// <value>The sort widget brush.</value>
        Brush SortWidgetBrush { get; }

        /// <summary>
        /// Gets the grid border brush.
        /// </summary>
        /// <value>The grid border brush.</value>
        Brush GridBorderBrush { get; }

        /// <summary>
        /// Gets the grid border thickness.
        /// </summary>
        /// <value>The grid border thickness.</value>
        Thickness GridBorderThickness { get; }

        /// <summary>
        /// Gets the background brush for the GroupDropArea.
        /// </summary>
        /// <value>The group area background brush.</value>
        Brush GroupAreaBackgroundBrush { get; }

        /// <summary>
        /// Gets the foreground brush for the GroupDropArea’s text.
        /// </summary>
        /// <value>The group area foreground brush.</value>
        Brush GroupAreaForegroundBrush { get; }

        /// <summary>
        /// Gets the Font information for the grouped header cell in the GroupDropArea.
        /// </summary>
        /// <value>The group header font.</value>
        GridFontInfo GroupHeaderFont { get; }

        /// <summary>
        /// Gets the CellBorder information for the grouped header cell in the GroupDropArea. 
        /// </summary>
        /// <value>The group cell borders.</value>
        CellBordersInfo GroupCellBorders { get; }

        /// <summary>
        /// Gets the CellMargin information for the grouped header cell in the GroupDropArea.
        /// </summary>
        /// <value>The group cell border margins.</value>
        CellMarginsInfo GroupCellBorderMargins { get; }

        /// <summary>
        /// Gets the Background for the DragDropIndicator.
        /// </summary>
        /// <value>The drag drop indicator brush.</value>
        Brush DragDropIndicatorBrush { get; }

        /// <summary>
        /// Gets the Outer Border Brush for the DragDropIndicator.
        /// </summary>
        /// <value>The drag drop indicator outer brush.</value>
        Brush DragDropIndicatorOuterBrush { get; }

        /// <summary>
        /// Gets the background brush for the summary caption row.
        /// </summary>
        /// <value>The summary caption background.</value>
        Brush SummaryCaptionBackground { get; }

        /// <summary>
        /// Gets the foreground brush for the summary caption row.
        /// </summary>
        /// <value>The summary caption foreground.</value>
        Brush SummaryCaptionForeground { get; }

        /// <summary>
        /// Gets the font information that to be displayed on the summary caption row.
        /// </summary>
        /// <value>The summary caption font.</value>
        GridFontInfo SummaryCaptionFont { get; }

        /// <summary>
        /// Gets the background brush for the summary row.
        /// </summary>
        /// <value>The summary row background.</value>
        Brush SummaryRowBackground { get; }

        /// <summary>
        /// Gets the foreground brush for the summary row.
        /// </summary>
        /// <value>The summary row foreground.</value>
        Brush SummaryRowForeground { get; }

        /// <summary>
        /// Gets the font information that to be displayed on the summary row.
        /// </summary>
        /// <value>The summary row font.</value>
        GridFontInfo SummaryRowFont { get; }

        /// <summary>
        /// Gets the background brush for the selected row.
        /// </summary>
        /// <value>The row header selection background.</value>
        Brush RowHeaderSelectionBackground { get; }

        /// <summary>
        /// Gets the foreground brush for the selected row.
        /// </summary>
        /// <value>The row header foreground.</value>
        Brush RowHeaderForeground { get; }

        /// <summary>
        /// Gets the highlight brush.
        /// </summary>
        /// <value>The highlight brush.</value>
        Brush HighlightBrush { get; }

        /// <summary>
        /// Gets the border brush of a selected current cell.
        /// </summary>
        /// <value>The current cell border brush.</value>
        Brush CurrentCellBorderBrush { get; }

        /// <summary>
        /// Gets the border thickness of a selected current cell.
        /// </summary>
        /// <value>The width of the current cell border.</value>
        double CurrentCellBorderWidth { get; }

        /// <summary>
        /// Gets the highlight selection background.
        /// </summary>
        /// <value>The highlight selection background.</value>
        Brush HighlightSelectionBackground { get; }

        /// <summary>
        /// Gets the highlight selection foreground.
        /// </summary>
        /// <value>The highlight selection foreground.</value>
        Brush HighlightSelectionForeground { get; }

        /// <summary>
        /// Gets the background brush for the column options popup.
        /// </summary>
        /// <value>The column options popup background.</value>
        Brush ColumnOptionsPopupBackground { get; }

        /// <summary>
        /// Gets the foreground brush for the column options popup.
        /// </summary>
        /// <value>The column options popup foreground.</value>
        Brush ColumnOptionsPopupForeground { get; }

        /// <summary>
        /// Gets the background brush for the column options icon.
        /// </summary>
        /// <value>The column options button background.</value>
        Brush ColumnOptionsButtonBackground { get; }

        /// <summary>
        /// Gets the column options close button brush.
        /// </summary>
        /// <value>The column options close button brush.</value>
        Brush ColumnOptionsCloseButtonBrush { get; }

        /// <summary>
        /// Gets the background brush for the Filter icon.
        /// </summary>
        /// <value>The filter button inner brush.</value>
        Brush FilterButtonInnerBrush { get; }

        /// <summary>
        /// Gets the border brush for the Filter icon.
        /// </summary>
        /// <value>The filter button outer brush.</value>
        Brush FilterButtonOuterBrush { get; }

        /// <summary>
        /// Gets the hover background brush for the Filter icon.
        /// </summary>
        /// <value>The filter button hover inner brush.</value>
        Brush FilterButtonHoverInnerBrush { get; }

        /// <summary>
        /// Gets the hover border brush for the Filter icon.
        /// </summary>
        /// <value>The filter button hover outer brush.</value>
        Brush FilterButtonHoverOuterBrush { get; }

        /// <summary>
        /// Gets the background brush of the filter icon when the filter is applied.
        /// </summary>
        /// <value>The filter button applied brush.</value>
        Brush FilterButtonAppliedBrush { get; }

        /// <summary>
        /// Gets the background brush for the default Expander icon(i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus button background.</value>
        Brush PlusMinusButtonBackground { get; }

        /// <summary>
        /// Gets the border brush for the default Expander icon (i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus button border brush.</value>
        Brush PlusMinusButtonBorderBrush { get; }

        /// <summary>
        /// Gets the foreground brush for the default Expander icon (i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus button foreground.</value>
        Brush PlusMinusButtonForeground { get; }

        /// <summary>
        /// Gets the row background.
        /// </summary>
        /// <value>The row background.</value>
        Brush RowBackground { get; }

        /// <summary>
        /// Gets the alternate row background.
        /// </summary>
        /// <value>The alternate row background.</value>
        Brush AlternateRowBackground { get; }

        /// <summary>
        /// Gets the background brush for the row where the mouse is pointed.
        /// </summary>
        /// <value>The hovering record cell background.</value>
        Brush HoveringRecordCellBackground { get; }

        /// <summary>
        /// Gets the background brush for the selected group caption row.
        /// </summary>
        /// <value>The group caption selection background.</value>
        Brush GroupCaptionSelectionBackground { get; }

        /// <summary>
        /// Gets the foreground brush for the row where the mouse is pointed.
        /// </summary>
        /// <value>The group caption selection foreground.</value>
        Brush GroupCaptionSelectionForeground { get; }

        /// <summary>
        /// Gets the foreground brush for a selected cell’s background in a row.
        /// </summary>
        /// <value>The current cell selection background.</value>
        Brush CurrentCellSelectionBackground { get; }

        /// <summary>
        /// Gets the foreground brush for a selected cell’s foreground in a row.
        /// </summary>
        /// <value>The current cell selection foreground.</value>
        Brush CurrentCellSelectionForeground { get; }

        /// <summary>
        /// Gets the hover background brush for the options of header cell 
        /// control such as sort icon, filter icon and column options icon.
        /// </summary>
        /// <value>The header options hover background.</value>
        Brush HeaderOptionsHoverBackground { get; }

        /// <summary>
        /// Gets the background brush for the options of header cell control such as sort icon, 
        /// filter icon and column options icon when it is checked.
        /// </summary>
        /// <value>The header options checked background.</value>
        Brush HeaderOptionsCheckedBackground { get; }

        /// <summary>
        /// Gets the border brush for the options of header cell 
        /// control such as sort icon, filter icon and column options icon.
        /// </summary>
        /// <value>The header options border brush.</value>
        Brush HeaderOptionsBorderBrush { get; }

        /// <summary>
        /// Gets or background brush for the default expanded Expander icon
        /// </summary>
        /// <value>The plus minus expanded button background.</value>
        Brush PlusMinusExpandedButtonBackground { get; }

        /// <summary>
        /// Gets the border brush for the default expanded Expander icon
        /// </summary>
        /// <value>The plus minus expanded button border brush.</value>
        Brush PlusMinusExpandedButtonBorderBrush { get; }

        /// <summary>
        /// Gets the foreground brush for the default expanded Expander icon
        /// </summary>
        /// <value>The plus minus expanded button foreground.</value>
        Brush PlusMinusExpandedButtonForeground { get; }

        /// <summary>
        /// Gets the Hover background brush for the default Expander icon(i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus hover button background.</value>
        Brush PlusMinusHoverButtonBackground { get; }

        /// <summary>
        /// Gets the Hover border brush for the default Expander icon (i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus hover button border brush.</value>
        Brush PlusMinusHoverButtonBorderBrush { get; }

        /// <summary>
        /// Gets the Hover foreground brush for the default Expander icon (i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus hover button foreground.</value>
        Brush PlusMinusHoverButtonForeground { get; }

        /// <summary>
        /// Gets the plus minus caption selected button background.
        /// </summary>
        /// <value>The plus minus caption selected button background.</value>
        Brush PlusMinusCaptionSelectedButtonBackground { get; }

        /// <summary>
        /// Gets the plus minus caption selected button border brush.
        /// </summary>
        /// <value>The plus minus caption selected button border brush.</value>
        Brush PlusMinusCaptionSelectedButtonBorderBrush { get; }

        /// <summary>
        /// Gets the plus minus caption selected button foreground.
        /// </summary>
        /// <value>The plus minus caption selected button foreground.</value>
        Brush PlusMinusCaptionSelectedButtonForeground { get; }

        /// <summary>
        /// Gets the hover foreground brush for the Header cell.
        /// </summary>
        /// <value>The header hover foreground brush.</value>
        Brush HeaderHoverForegroundBrush { get; }

       

    }

    public interface IGridDataNestedVisualStyle
    {
        /// <summary>
        /// Gets the Cell borders information for the top left header cell of the Nested grid.
        /// </summary>
        /// <value>The top left cell header cell border.</value>
        CellBordersInfo TopLeftCellHeaderCellBorder { get; }
        /// <summary>
        /// Gets the Cell borders information for the header cells not but the top left header cell of the Nested grid.
        /// </summary>
        /// <value>The nested header cell border.</value>
        CellBordersInfo NestedHeaderCellBorder { get; }
        /// <summary>
        /// Gets the Cell borders information for the left value cells (i.e. top left column) of the Nested grid.
        /// </summary>
        /// <value>The first header column border.</value>
        CellBordersInfo FirstHeaderColumnBorder { get; }
        /// <summary>
        /// Gets the Cell borders information for the value cells not but the first left column value cells of the Nested grid.
        /// </summary>
        /// <value>The last header column border.</value>
        CellBordersInfo LastHeaderColumnBorder { get; }
    }

    #region LegacyStyles

    public class GridDataLegacyDefaultGridVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF2860D"),Offset = 0.08d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD9600F"),Offset = 0.97d},                                             
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF2860D"),Offset = 0.08d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD9600F"),Offset = 0.97d},                                             
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1F65A9"),Offset=0.009d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF144691"),Offset=0.991d},                                             
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1F62A7"), Offset=0.051d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF154792"), Offset=0.987},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF628CB6"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF2860D"), Offset=0.08},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD9600F"), Offset=0.97},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF19A17"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFEFF7FD"), Offset=0.056d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFDEEEF9"), Offset=0.944d},      
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFFFFF"), Offset=1d},               
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }


        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(Brushes.Black, 0.2d),
                    Right = new Pen(Brushes.Black, 0.2d),
                    Bottom = new Pen(Brushes.Black, 0.2d),
                    Left = new Pen(Brushes.Black, 0.2d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(Brushes.Black, 0.2d),
                    Right = new Pen(Brushes.Black, 0.2d),
                    Bottom = new Pen(Brushes.Black, 0.2d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(Brushes.Black, 0.2d),
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(Brushes.Black, 0.2d),
                };
            }
        }

        #region IGridDataVisualStyle Members

        public Brush HeaderBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(SystemColors.ControlColor);
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(SystemColors.ControlColor);
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color=Color.FromArgb(255,160,160,186),Offset=0d},                   
                    new GradientStop(){ Color=Color.FromArgb(255,244,246,247),Offset=1d}
                }, 0.00);
                brush.StartPoint = new Point(0.503593, 0.966534);
                brush.EndPoint = new Point(0.503593, 0.11463);
                return brush;

            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(Colors.Black), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(Colors.Black), 0.2d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return GridFontInfo.Default;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 6d };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(Colors.Black), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(Colors.Black), 0.2d),
                    Top = new Pen(new SolidColorBrush(Colors.Black), 0.2d),
                    Left = new Pen(new SolidColorBrush(Colors.Black), 0.2d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(Colors.White);
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
                return GridFontInfo.Default;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                Color c = SystemColors.HighlightColor;
                return new SolidColorBrush(Color.FromArgb(96, c.R, c.G, c.B));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(Colors.Black), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(Colors.Black), 0.2d),
                    Top = new Pen(new SolidColorBrush(Colors.Black), 0.2d),
                    Left = new Pen(new SolidColorBrush(Colors.Black), 0.2d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public GridFontInfo SummaryRowFont
        {
            get
            {
                return this.ValueFont;
            }
        }
        #endregion

        public Brush RowHeaderSelectionBackground
        {
            get { return new SolidColorBrush() { Color = Colors.Black }; }
        }

        public Brush RowHeaderForeground
        {
            get { return new SolidColorBrush() { Color = Colors.Gray }; }
        }

        public Brush HeaderInnerBorder
        {
            get { return new SolidColorBrush() { Color = Colors.Transparent }; }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get { return new Thickness(0.2d); }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush() { Color = Colors.Black };
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush() { Color = SystemColors.HighlightColor };
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush() { Color = SystemColors.HighlightTextColor };
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {                   
                    new GradientStop(){ Color=Color.FromArgb(255,240,240,240),Offset=0d},                   
                    new GradientStop(){ Color=Color.FromArgb(255,240,240,240),Offset=1d}
                }, 0.00);

                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;

            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 69, 70, 71));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 215, 215, 215));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 0, 0, 25));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return new SolidColorBrush(SystemColors.ControlDarkColor);
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 137, 137, 137));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 0, 0, 25));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color=Color.FromArgb(255,200,200,200),Offset=0d},                   
                    new GradientStop(){ Color=Color.FromArgb(255,50,50,50),Offset=1d}
                }, 0.00);
                brush.StartPoint = new Point(0.569377, 3.01722e-005);
                brush.EndPoint = new Point(0.569377, 1.00003);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return Brushes.Gray; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }


        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }


    }

    public class GridDataLegacyOffice2007BlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }


        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF7FAFF"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF6FBFE"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FE"), Offset=1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7FBFD"), Offset=0.996d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FD"), Offset=0},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.823, 0.155);
                brush.EndPoint = new Point(0.823, 1.328);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }



        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#006593CF"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d)
                };
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = Color.FromArgb(255, 197, 222, 255), Offset = 1d},
                    new GradientStop(){ Color = Color.FromArgb(255, 249, 252, 255) , Offset = 0d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = Color.FromArgb(255, 197, 222, 255), Offset = 1d},
                    new GradientStop(){ Color = Color.FromArgb(255, 249, 252, 255) , Offset = 0d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFFFEE4"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFFE9A8"), Offset = 0.518d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFFD767"), Offset = 0.522d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFFE69F"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9AC6FF"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#7FC0D8F1")); }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF154A93"));
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 14d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.Bold,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF")), 0.5d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF")), 0.5d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 6d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFE7A2"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3764B0"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 13d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9AC6FF"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFFF8FF"), Offset = 0d},
                    new GradientStop(){ Color =  ColorExtensions.StringToColor("#FFC0C8F0"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.570556, 2.58505e-005);
                brush.EndPoint = new Point(0.570556, 0.99999);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF304860"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF90A8C0"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0d},
                    new GradientStop() { Color = ColorExtensions.StringToColor("#FFC8DFFF"), Offset = 1d}
                }, 0.00);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9AC6FF"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9AC6FF"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9AC6FF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyOffice2007SilverVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDCDDE1"), Offset = 0.888d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF9FAFC"), Offset = 0.056d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDCDDE1"), Offset=0},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF9FAFC"), Offset=1},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.659, 0.045);
                brush.EndPoint = new Point(0.659, 0.988);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF707173"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF5F6F8"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#006F7074"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                //<LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0">
                //   <GradientStop Color="#FFE9ECF8" Offset="0"/>
                //   <GradientStop Color="#FFD4D8E2" Offset="1"/>
                //   <GradientStop Color="#FFD6DAE4" Offset="0.518"/>
                //   <GradientStop Color="#FFC5C7D1" Offset="0.522"/>
                //</LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = Color.FromArgb(255, 200, 201, 202), Offset = 1d},
                    new GradientStop(){Color = Color.FromArgb(255, 241, 243, 243), Offset = 0d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                //<LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0">
                //   <GradientStop Color="#FFE9ECF8" Offset="0"/>
                //   <GradientStop Color="#FFD4D8E2" Offset="1"/>
                //   <GradientStop Color="#FFD6DAE4" Offset="0.518"/>
                //   <GradientStop Color="#FFC5C7D1" Offset="0.522"/>
                //</LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = Color.FromArgb(255, 200, 201, 202), Offset = 1d},
                    new GradientStop(){Color = Color.FromArgb(255, 241, 243, 243), Offset = 0d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFEE4"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE9A8"), Offset = 0.518},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE69F"), Offset =  1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 14d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.Bold,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 6d };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF")), 0.5d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF")), 0.5d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF383838"));
            }
        }

        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo ValueTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 4d };
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6E6D8F"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#7FC0D8F1")); }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF0F1F2"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFE7A2"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF706F91"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 13d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6E6D8F"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFF8FF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC0C8F0"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.570556, 2.58505e-005);
                brush.EndPoint = new Point(0.570556, 0.99999);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF304860"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF90A8C0"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF9F9FA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDBDDE1"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6E6D8F"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6E6D8F"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return new SolidColorBrush(SystemColors.ControlDarkColor);
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6E6D8F"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074")), 0.2d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyOffice2007BlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.046d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0.904d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.975},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.659, 0.045);
                brush.EndPoint = new Point(0.659, 0.988);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD6D6E0"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000802"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF5F6F8"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#004C535C"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = Color.FromArgb(255, 223, 223, 223), Offset = 1d},
                    new GradientStop(){Color = Color.FromArgb(255, 248, 248, 248), Offset = 0d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = Color.FromArgb(255, 223, 223, 223), Offset = 1d},
                    new GradientStop(){Color = Color.FromArgb(255, 248, 248, 248), Offset = 0d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFEE4"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE9A8"), Offset = 0.518},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE69F"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9199A4"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#7FC0D8F1")); }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF0F1F2"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF464646"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 14d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.Bold,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF")), 0.5d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF")), 0.5d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF383838"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 6d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFE7A2"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF616A76"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 13d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9199A4"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFF8FF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC0C8F0"), Offset =  1d}
                }, 0d);
                brush.StartPoint = new Point(0.570556, 2.58505e-005);
                brush.EndPoint = new Point(0.570556, 0.99999);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF304860"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF90A8C0"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF9F9FA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD6D9DE"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9199A4"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9199A4"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9199A4"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
       public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C")), 0.2d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyBlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {

        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.046d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0.904d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.975},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.659, 0.045);
                brush.EndPoint = new Point(0.659, 0.988);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD6D6E0"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }





        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF55371F"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.0280268" EndPoint="0.5,0.951414">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF2A2A2A" Offset="0"/>
                //        <GradientStop Color="#FF010101" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2A2A2A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF010101"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }


        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.0280268" EndPoint="0.5,0.951414">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF2A2A2A" Offset="0"/>
                //        <GradientStop Color="#FF010101" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2A2A2A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF010101"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF343434"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF010101"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.238967);
                brush.EndPoint = new Point(0.5, -0.0186828);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontWeight = FontWeights.Normal
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,-0.0606109" EndPoint="0.5,1.03035">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FFCCCCCC" Offset="0"/>
                //        <GradientStop Color="#FFFCFCFC" Offset="0.510989"/>
                //        <GradientStop Color="#FFCCCCCC" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#78CCCCCC"), Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#78FCFCFC"), Offset= 0.5d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#78CCCCCC"), Offset= 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0606109);
                brush.EndPoint = new Point(0.5, 1.03035);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCACAD8"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontWeight = FontWeights.SemiBold,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA39D99"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA39D99"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1A1A1A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1A1A1A"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 202, 202, 216));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 160, 87, 29));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 160, 87, 29));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA39D99"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA39D99"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2A2A2A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF010101"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.500001, 0.0597219);
                brush.EndPoint = new Point(0.500001, 0.911631);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF454545"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacySyncBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF6FBFE"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FE"), Offset=1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7FBFD"), Offset=0.996d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FD"), Offset=0},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.823, 0.155);
                brush.EndPoint = new Point(0.823, 1.328);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }



        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,-0.0430693" EndPoint="0.5,0.928826">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF578FD4" Offset="0"/>
                //        <GradientStop Color="#FF538ACF" Offset="0.318681"/>
                //        <GradientStop Color="#FF3360A2" Offset="0.604396"/>
                //        <GradientStop Color="#FF305C9E" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF578FD4"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF538ACF"), Offset = 0.318681d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3360A2"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF305C9E"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,-0.0430693" EndPoint="0.5,0.928826">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF578FD4" Offset="0"/>
                //        <GradientStop Color="#FF538ACF" Offset="0.318681"/>
                //        <GradientStop Color="#FF3360A2" Offset="0.604396"/>
                //        <GradientStop Color="#FF305C9E" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF578FD4"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF538ACF"), Offset = 0.318681d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3360A2"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF305C9E"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF84EDF7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF42B3DE"), Offset = 0.357143d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF429CC2"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2B8FBA"), Offset = 0.511014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0085AA"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF49B9E4"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.184751" EndPoint="0.5,0.689042">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FFE1F3FC" Offset="0"/>
                //        <GradientStop Color="#FFFFFFFF" Offset="0.549451"/>
                //        <GradientStop Color="#FFE6F5FC" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78E1F3FC"), Offset=  0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78FFFFFF"), Offset = 0.549451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78E6F5FC"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4A88C6"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF5F5F5"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6BC2E7"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2F5BB7"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2F5C9E"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF548DD1"), Offset=  1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 208, 219, 229));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 225, 243, 252));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 225, 243, 252));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF84EDF7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF42B3DE"), Offset = 0.357143d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF429CC2"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2B8FBA"), Offset = 0.511014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0085AA"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF49B9E4"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.500003, 0.032804);
                brush.EndPoint = new Point(0.500003, 0.854236);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF305CB7"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d),
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFF")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyGlassyGreenVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF629110")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0.229d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEAF390"), Offset=0.974d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.101000003516674, 0.0879999995231628);
                brush.EndPoint = new Point(0.791999995708466, 0.860000014305115);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0.229d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF898F4A"), Offset=0.974d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.101000003516674, 0.0879999995231628);
                brush.EndPoint = new Point(0.791999995708466, 0.860000014305115);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF263007"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF263007"));

            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA1C240"), Offset = 0.16d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF6B963A"), Offset=0.991d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF6E973A"), Offset=0.013d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA0C140"), Offset=0.782d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.823000013828278, 0.155000001192093);
                brush.EndPoint = new Point(0.823000013828278, 1.32799994945526);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF629110"));
            }
        }


        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEDECEC"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8C8C8C"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6D993A"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBFBFB"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF586458"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF657E66"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B543B"), Offset = 0.518681d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF243726"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF19281C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF657E66"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B543B"), Offset = 0.518681d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF243726"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF19281C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF657E66"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B543B"), Offset = 0.357143d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF29432A"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0D560E"), Offset = 0.511014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF095F0D"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF268D2A"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB08551"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78BD9C79"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78BD9C79"), Offset = 0.249451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78AB834C"), Offset = 0.549451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78A3773B"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF627D63"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }

        }
        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1E2D20")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF242D27")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1E2D20")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1E2D20")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF242D27")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1E2D20")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1E2D20")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF036F05"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFEB74A"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF795737"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF657E66"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF19281C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB08551"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB08551"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB08551"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF657E66"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B543B"), Offset = 0.357143d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF29432A"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0D560E"), Offset = 0.511014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF095F0D"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF268D2A"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF141F17"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1C9DF")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.1d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1C9DF")), 0.1d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.1d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.1d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacySunBlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF635552"), Offset = 0.14d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF49382D"), Offset=0.5d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF3A2115"), Offset = 0.5d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAF4802"), Offset=0.986d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAF4802"), Offset = 1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF24201F"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF635552"), Offset = 0.14d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF49382D"), Offset=0.5d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF3A2115"), Offset = 0.5d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAF4802"), Offset=0.986d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAF4802"), Offset = 1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF645753"), Offset=0.03d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF49372C"), Offset=0.5d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF362117"), Offset=0.5d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC0672A"), Offset=1d},                                       
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF61544F"), Offset=0.014d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4A382D"), Offset=0.5d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF372217"), Offset=0.5d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4D2D17"), Offset=0.999d},                      
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918"));
            }
        }


        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA092"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2B2929"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2C2A2A"), Offset=0.027},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2E2C2C"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF24201F"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8E4C32"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("Transparent"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF413E3B"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF494643"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC96928"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC96928"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBF4D00"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBF4D00"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC96928"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC96928"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBF4D00"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBF4D00"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC65800"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD77400"), Offset = 0.518},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE99400"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF8AC00"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#90FEB547"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#90F8A528"), Offset = 0.249451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#90F9A82D"), Offset = 0.549451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#90F3970B"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB0AFAF"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF626160")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF626160")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF575555"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7D532B"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF8A528"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFEB74A"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Yellow;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC96928"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBF4D00"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6A0100"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC96928"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC96928"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBF4D00"), Offset=  0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBF4D00"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return this.HeaderInnerBorder;
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyShinyRedVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDD9493"), Offset = 0.18d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFB83C36"), Offset=0.469d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA2241E"), Offset = 0.513d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBC5B53"), Offset=0.783d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF4D4C7"), Offset = 1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4D4C7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE3A0A0"), Offset=0.03d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFB73832"), Offset=0.496d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA2221C"), Offset=0.501d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE9DB"), Offset=1d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBD5C54"), Offset=0.769d},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE3A0A0"), Offset=0.03d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFB73832"), Offset=0.498d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA2221C"), Offset=0.506d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF701A19"), Offset=0.997d},                      
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9A9A9A"), Offset = 0.038d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A6A6A"), Offset = 0.614},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF777777"), Offset = 0.633},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB6B6B6"), Offset = 1d},                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAECEB"), Offset = 0.023d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBCBDBF"), Offset = 0.402},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3A7AA"), Offset = 0.41},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3A7AA"), Offset = 0.448d}, 
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF979CA0"), Offset = 0.456},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF74787B"), Offset = 0.938d}, 
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEDB7B7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFCF6967"), Offset = 0.4},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB83B37"), Offset = 0.45},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFAE231E"), Offset = 0.5d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF8F8F8"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF681817"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFEAB6B8"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFD98583"), Offset = 0.357143d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFAD2728"), Offset = 0.510964d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF8E1E1A"), Offset = 0.511014d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF7D1B1C"), Offset = 0.604396d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF651B1C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFEAB6B8"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFD98583"), Offset = 0.357143d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFAD2728"), Offset = 0.510964d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF8E1E1A"), Offset = 0.511014d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF7D1B1C"), Offset = 0.604396d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF651B1C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFE69EA1"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFDE847B"), Offset = 0.357143d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFC8554E"), Offset = 0.510964d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFA52023"), Offset = 0.511014d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFBC6A55"), Offset = 0.804396d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFF8D2BB"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFAF5F1"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#80EFEFEF"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#80FBFBFB"), Offset = 0.249451d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#80F5F5F5"), Offset = 0.549451d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#80ABABAB"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCBCFD2"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBECACA")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF5D5A51")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB6ABA9")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFCBCFD2")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFCBCFD2")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBECACA")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF5D5A51")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB6ABA9")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB6ABA9")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFBABFC5"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6A6A6A"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD98F72"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFDFA"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Maroon;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFEAB6B8"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF651B1C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6A0100"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFEAB6B8"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFD98583"), Offset = 0.357143d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFAD2728"), Offset = 0.510964d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF8E1E1A"), Offset = 0.511014d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF7D1B1C"), Offset = 0.604396d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF651B1C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
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
                return this.HeaderInnerBorder;
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBECACA")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB6ABA9")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBECACA")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB6ABA9")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB6ABA9")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyShinyBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {

        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF9BCEF6"), Offset = 0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4092DE"), Offset=0.366d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1175D2"), Offset = 0.406d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF0168C6"), Offset=0.503d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF08477D"), Offset = 0.987d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF203F5A"), Offset = 1d},    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4D4C7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA4D5F9"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4193E0"), Offset=0.343d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1276D4"), Offset=0.392d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF0168C8"), Offset=0.401d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF035DB0"), Offset=0.608d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF559ACB"), Offset=0.804d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF93D5FA"), Offset=0.951d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC5E9FE"), Offset=1d},                                               
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA4D5F9"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4193E0"), Offset=0.384d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1276D4"), Offset=0.412d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF0168C8"), Offset=0.506d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF08477D"), Offset=0.973d} 
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9A9A9A"), Offset = 0.038d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A6A6A"), Offset = 0.614},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF777777"), Offset = 0.633},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB6B6B6"), Offset = 1d},                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAECEB"), Offset = 0.023d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBCBDBF"), Offset = 0.402},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3A7AA"), Offset = 0.41},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3A7AA"), Offset = 0.448d}, 
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF979CA0"), Offset = 0.456},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF74787B"), Offset = 0.938d}, 
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9BCEF6"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4092DE"), Offset = 0.366},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1175D2"), Offset = 0.406},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0168C6"), Offset = 0.503},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF08477D"), Offset = 0.987},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF203F5A"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF8F8F8"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5AA4E6"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFA1D5FC"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF61A5E4"), Offset = 0.357143d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF2180DA"), Offset = 0.510964d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF0260B8"), Offset = 0.511014d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF064B86"), Offset = 0.604396d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF143E68"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFA1D5FC"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF61A5E4"), Offset = 0.357143d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF2180DA"), Offset = 0.510964d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF0260B8"), Offset = 0.511014d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF064B86"), Offset = 0.604396d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF143E68"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF94CBF5"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF5AA4E6"), Offset = 0.357143d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF016ACD"), Offset = 0.510964d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF035FB3"), Offset = 0.511014d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF599ECE"), Offset = 0.804396d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF8CCDF4"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFDFA"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#509F9F9F"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#506B6B6B"), Offset = 0.249451d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#50A5A5A5"), Offset = 0.549451d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#50ABABAB"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCBCFD2"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA9AEB1")), 0.1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF5D5A51")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA8AB")), 0.1d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFCBCFD2")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFCBCFD2")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA9AEB1")), 0.1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF5D5A51")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA8AB")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA8AB")), 0.1d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6A6A6A"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA1D5FC"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9F9F9F"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF143E68"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFA1D5FC"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1564AF"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1564AF"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
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
                return this.HeaderInnerBorder;
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA9AEB1")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA8AB")), 0.1d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA9AEB1")), 0.1d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA8AB")), 0.1d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA8AB")), 0.1d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyBureauBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEBA50"), Offset = 0.015d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFE9A05"), Offset=0.029d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFE6800"), Offset = 0.985d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD85800"), Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF90DCF7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7F7F7"), Offset=0.23d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE8E8E8"), Offset=0.991d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBCBCBC"), Offset=1d},                                                      
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBEBEBE"), Offset = 0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDEDEDE"), Offset = 0.249d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD5D5D5"), Offset = 0.956d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC6C6C6"), Offset = 0.961d},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEDECEC"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8C8C8C"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6D993A"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFAFAFA"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD3E1EF"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBFD1E8"), Offset = 1d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF83A7D5"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF99B5DB"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC0D2EA"), Offset = 0d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBFD1E8"), Offset = 1d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF83A7D5"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF99B5DB"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC0D2EA"), Offset = 0d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFEFAE7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFDEFA2"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFECA24"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFEE14F"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3E6398"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78F6CB72"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78F89937"), Offset = 0.549451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78F18634"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F9DDA"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1C9DF")), 0.1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF91B5DB")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.1d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF79A8CE")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF79A8CE")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.AliceBlue;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1C9DF")), 0.1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF91B5DB")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.1d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5A80B4"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF78A9F9"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB6D2F6"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.MidnightBlue;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF6AC65"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBFD1E8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC0D2EA"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.AliceBlue;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3E6398"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F9DDA"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3E6398"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
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
                return this.HeaderInnerBorder;
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1C9DF")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF91B5DB")), 0.1d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1C9DF")), 0.1d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.1d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.1d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyBureauBlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEBA50"), Offset = 0.015d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFE9A05"), Offset=0.029d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFE6800"), Offset = 0.985d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD85800"), Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF90DCF7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7F7F7"), Offset=0.23d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE8E8E8"), Offset=0.991d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBCBCBC"), Offset=1d},                                                      
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBEBEBE"), Offset = 0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDEDEDE"), Offset = 0.249d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD5D5D5"), Offset = 0.956d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC6C6C6"), Offset = 0.961d},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF2860D"), Offset=0.08},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD9600F"), Offset=0.97},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF19A17"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFEFF7FD"), Offset=0.056d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFDEEEF9"), Offset=0.944d},      
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFFFFF"), Offset=1d},               
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }


        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDCDFE5"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFC9CED5"), Offset = 1d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF9FA8B8"), Offset = 0.522d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFBDC7D5"), Offset = 0.518d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFE1E7EE"), Offset = 0d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }


        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFC9CED5"), Offset = 1d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FF9FA8B8"), Offset = 0.522d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFBDC7D5"), Offset = 0.518d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFE1E7EE"), Offset = 0d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFEFAE7"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFDEFA2"), Offset = 0.518d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFECA24"), Offset = 0.522d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFFEE14F"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF475476"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#78F6CB72"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#78F89937"), Offset = 0.549451d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#78F18634"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF858584"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF808080")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D")), 0.1d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF808080")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D")), 0.1d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3939"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCBCFD5"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9E9E9"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.Black;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF6AC65"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFE1E7EE"), Offset = 0d},
                    new GradientStop(){ Color = ColorExtensions.StringToColor("#FFC9CED5"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3E6398"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF858584"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFACB2BD"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
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
                return this.HeaderInnerBorder;
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF808080")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D")), 0.1d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyBlendVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF676767"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF676767"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF676767"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF525252"));
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2B2B2B"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF535352"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF272828"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF535352"));
            }
        }


        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAAAAAA"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE8E8E8"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF424242"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0732399);
                brush.EndPoint = new Point(0.5, 1.02406);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF424242"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0732399);
                brush.EndPoint = new Point(0.5, 1.02406);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD7D7D7"));
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF282828"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 11.75d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#78D7D7D7"));
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF282828"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF424242")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12.5d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.2d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF424242"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAAAAAA"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD7D7D7"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF282828"));
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD7D7D7"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD7D7D7"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3A3A3A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3A3A3A"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 215, 215, 215));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD7D7D7"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF282828"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF22282F"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE2E2E2"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF424242"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.059718);
                brush.EndPoint = new Point(0.5, 0.911629);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE8E8E8"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF424242")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF424242")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF424242")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF424242")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF424242")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {

        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.ControlLightLightColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF6FBFE"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FE"), Offset=1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7FBFD"), Offset=0.996d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FD"), Offset=0},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.823, 0.155);
                brush.EndPoint = new Point(0.823, 1.328);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7380A3"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.0597032" EndPoint="0.5,0.911615">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF395286" Offset="0"/>
                //        <GradientStop Color="#FF111E4D" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF395286"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF111E4D"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0597032);
                brush.EndPoint = new Point(0.5, 0.911615);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.0597032" EndPoint="0.5,0.911615">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF395286" Offset="0"/>
                //        <GradientStop Color="#FF111E4D" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF395286"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF111E4D"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0597032);
                brush.EndPoint = new Point(0.5, 0.911615);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4877B3"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF173D8C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.499998, 0.0596964);
                brush.EndPoint = new Point(0.499998, 0.911608);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78DCEFFF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78FFFFFF"), Offset = 0.549451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78E0F1FF"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7687A5"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB3C3D6")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB3C3D6")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d),
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFAAF40"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7DA4"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF69A2E8"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF3F6F9"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF385186"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF101E4C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 208, 219, 229));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 102, 146, 201));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 102, 146, 201));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC2CFDE"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7DA4"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB3C3D6"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A7D99"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.570556, 2.58505e-005);
                brush.EndPoint = new Point(0.570556, 0.99999);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacySilverVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDCDDE1"), Offset = 0.888d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF9FAFC"), Offset = 0.056d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDCDDE1"), Offset=0},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF9FAFC"), Offset=1},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.659, 0.045);
                brush.EndPoint = new Point(0.659, 0.988);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF707173"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE8E9EA"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.940303" EndPoint="0.5,0.0883908">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FFBDC1C8" Offset="0"/>
                //        <GradientStop Color="#FFEDEFF0" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBDC1C8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEDEFF0"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.940303);
                brush.EndPoint = new Point(0.5, 0.0883908);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.940303" EndPoint="0.5,0.0883908">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FFBDC1C8" Offset="0"/>
                //        <GradientStop Color="#FFEDEFF0" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBDC1C8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEDEFF0"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.940303);
                brush.EndPoint = new Point(0.5, 0.0883908);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA0A0BA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF4F6F7"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.503593, 0.966534);
                brush.EndPoint = new Point(0.503593, 0.11463);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78D6D7D9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78FFFFFF"), Offset = 0.5d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78CCCCCC"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0384275);
                brush.EndPoint = new Point(0.5, 0.795361);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7687A5"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE0E0E0")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE0E0E0")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE0E0E0")), 0.3d),
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFECEEEF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4D0AE"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF898989"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5E5E5E"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF3F6F9"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF766C6C"), Offset = 0.375d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC9BFBF"), Offset = 0.375d},
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.028);
                brush.EndPoint = new Point(0.5, 0.985);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 69, 70, 71));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 0, 0, 25));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF898989"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC8C8C8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF323232"),  Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.569377, 3.01722e-005);
                brush.EndPoint = new Point(0.569377, 1.00003);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d),
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyOffice14BlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFF861A"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEEF4FA"),Offset=0},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EBF9"), Offset=1d},                                                                                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD8E6F7"), Offset=0.049d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEEF4FA"),Offset=0.951d},   
                }, 0.0d);
                brush.StartPoint = new Point(0.821, 0.964);
                brush.EndPoint = new Point(0.814, 0.101);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0")); }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B")); }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF9F9F9"), Offset=0.054d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFC6DDF1"), Offset=0.406d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD5E5F4"),Offset=0.975d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF5FDFE"), Offset=0.987d},         
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }



        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF758EAB"), Offset = 0.014},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF8AA3C2"), Offset = 0.019}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF8AA3C2"), Offset=0.042},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF95AECD"), Offset=0.047},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF9FB7D6"), Offset=0.948},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF96AECD"), Offset=0.977}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF87A0BC"), Offset=0.981}, 
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1ECFA"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 1d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 1d)
                };
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(1d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF5FB"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1ECFA"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF5FB"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1ECFA"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE4F1FC"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFCCDFF3"), Offset = 0.375d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDBE9F6"), Offset = 0.764d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5FDFF"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#7FA7CDF0")); }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCFDDEE"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 14d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.Bold,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8DAED9")), 0.275d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8DAED9")), 0.275d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 1d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 1d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 6d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5F9FF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD7E3F3"), Offset = 0.357143d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFCAD8EA"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBDCDE0"), Offset = 0.641014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB8C8DB"), Offset = 0.804396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB3C4D8"), Offset = 1d}
                }, 0d);

                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 13d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.570556, 2.58505e-005);
                brush.EndPoint = new Point(0.570556, 0.99999);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF262626"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1ECFA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF5FB"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4FCFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyOffice14BlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {

        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFF861A"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE9E9E9"),Offset=0.134d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE0E0E0"),Offset=0.768d},                                                                                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDEDEDE"), Offset=0.096d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE9E9E9"), Offset=0.892d},   
                }, 0.0d);
                brush.StartPoint = new Point(0.814, 0.101);
                brush.EndPoint = new Point(0.794, 0.813);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFA9C1DE")); }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF21272D")); }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF3D3D3D"), Offset=0},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF434343"), Offset=0.026}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF424242"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF7D7D7D"), Offset=0.04d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF4F4F4F"), Offset=0.598d},  
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF555555"), Offset=0.879d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF6B6B6B"), Offset=0.984d},            
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB4B4B4"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d)
                };
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(1d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAEAEA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE9E9E9"), Offset = 0.446732d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE2E2E2"), Offset = 0.765472d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDFDFDF"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAEAEA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE9E9E9"), Offset = 0.446732d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE2E2E2"), Offset = 0.765472d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDFDFDF"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   new GradientStop(){Color = ColorExtensions.StringToColor("#FF7B7B7B"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A6A6A"), Offset = 0.223d},
                    new GradientStop(){Color = Colors.Gray, Offset = 0.955d}
                }, 0d);

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#7FA7CDF0")); }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB2B2B2"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 14d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.Bold,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB8B8B8")), 0.275d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB8B8B8")), 0.275d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 6d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF494949"), Offset = 0.357143d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF434343"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3D3D3D"), Offset = 0.641014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF373737"), Offset = 0.804396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1A1E23"), Offset = 1d}
                }, 0d);

                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF363636"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 13d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.White;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.570556, 2.58505e-005);
                brush.EndPoint = new Point(0.570556, 0.99999);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF262626"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAEAEA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDFDFDF"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB2B2B2"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
       public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 1d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyOffice14SilverVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFF861A"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }
        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEFF2F6"),Offset=0.002d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EBF9"),Offset=1d},                                                                                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EAF8"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEEF1F5"), Offset=1d},   
                }, 0.0d);
                brush.StartPoint = new Point(0.814, 0.101);
                brush.EndPoint = new Point(0.794, 0.813);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0")); }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B")); }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA6ACB4"), Offset=0.02},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB0B8C0"), Offset=0.025}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFBCC3CB"), Offset=0.204},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFBCC3CB"), Offset=0.975},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB4BAC2"), Offset=0.98}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }


        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD7DCE2"), Offset=0.213d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD4DAE1"), Offset=0.622d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFE6ECF1"), Offset=0.975d},      
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEFF2F6"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d)
                };
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(1d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5F7F9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF1F4F7"), Offset = 0.446732d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F3F6"), Offset = 0.765472d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF2F6"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }


        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5F7F9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF1F4F7"), Offset = 0.446732d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F3F6"), Offset = 0.765472d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF2F6"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF1F4F8"), Offset = 0.043d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE7EBEF"), Offset = 0.435d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF3F4F6"), Offset = 0.87d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 11.5d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#7FA7CDF0")); }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9EDF1"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 12.5d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.Bold,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB5B9BE")), 0.275d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB5B9BE")), 0.275d),
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB5B9BE")), 0.275d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB5B9BE")), 0.275d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 6d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF7FAFC"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6EAEE"), Offset = 0.245672d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD8DCE1"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFCACED3"), Offset = 0.741014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB7BCC1"), Offset = 1d}
                }, 0d);

                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Calibri"),
                    FontSize = 13d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF595959"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.570556, 2.58505e-005);
                brush.EndPoint = new Point(0.570556, 0.99999);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF262626"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5F7F9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF2F6"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
                //return new SolidColorBrush(SystemColors.ControlDarkColor);
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4FCFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d)
                };
            }
        }

        #endregion
    }

    public class GridDataLegacyVS2010VisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAEB3B8"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF0EEE7"),Offset=0.01d},           
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEFAEF"),Offset=0.042d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFF4D0"),Offset=0.422d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.484d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.958d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF293955"),Offset=0.99d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF47566E"),Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4C365"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAEB3B8"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF0EEE7"),Offset=0.01d},           
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEFAEF"),Offset=0.042d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFF4D0"),Offset=0.422d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.484d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.958d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF293955"),Offset=0.99d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF47566E"),Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1A283D"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1A283D"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1A283D"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAEB3B8"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF0EEE7"),Offset=0.01d},           
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEFAEF"),Offset=0.042d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFF4D0"),Offset=0.422d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.484d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.958d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF293955"),Offset=0.99d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF47566E"),Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"), Offset = 0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"), Offset = 1d},   
                }, 0.0d);
                brush.StartPoint = new Point(0.814, 0.101);
                brush.EndPoint = new Point(0.794, 0.813);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE5C365"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEFAEF"), Offset=0},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEF3CF"), Offset=0.416}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEE7A5"), Offset=0.584},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get { return Brushes.Black; }
        }

        public Brush CurrentCellSelectionBackground
        {
            get { return Brushes.White; }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return Brushes.Black; }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFFBEF"), Offset=0.064d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFF3CF"), Offset=0.485d},      
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFECB5"), Offset=0.491d},               
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }


        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7380A3"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     new GradientStop(){Color = ColorExtensions.StringToColor("#FF6F82A4"), Offset = 0.042d},
                      new GradientStop(){Color = ColorExtensions.StringToColor("#FF4D5F82"), Offset = 1d},                          
                }, 0d);
                brush.StartPoint = new Point(0.483497, -0.481132);
                brush.EndPoint = new Point(0.483497, 0.518871);

                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     new GradientStop(){Color = ColorExtensions.StringToColor("#FF6F82A4"), Offset = 0.042d},
                      new GradientStop(){Color = ColorExtensions.StringToColor("#FF4D5F82"), Offset = 1d},                          
                }, 0d);
                brush.StartPoint = new Point(0.483497, -0.481132);
                brush.EndPoint = new Point(0.483497, 0.518871);

                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     new GradientStop(){Color = ColorExtensions.StringToColor("#FF879DC6"), Offset = 0.061d},
                      new GradientStop(){Color = ColorExtensions.StringToColor("#FF4B5F89"), Offset = 0.974d},                                         
                }, 0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCED4DD"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                return Brushes.Red;
                //return new SolidColorBrush(ColorExtensions.StringToColor("#FFCED4DD"));
                //LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                //{
                //    new GradientStop(){Color = ColorExtensions.StringToColor("#78DCEFFF"), Offset = 0d},
                //    new GradientStop(){Color = ColorExtensions.StringToColor("#78FFFFFF"), Offset = 0.549451d},
                //    new GradientStop(){Color = ColorExtensions.StringToColor("#78E0F1FF"), Offset = 1d}
                //}, 0d);
                //brush.StartPoint = new Point(0.5, -0.00285119);
                //brush.EndPoint = new Point(0.5, 0.825156);
                //return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB6BDCE"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {

                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB3C3D6")), 0.3d),
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB3C3D6")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB3C3D6")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d),
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFBCC7D8"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD8DDE5"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B4E99"));
                //return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
                //return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCED4DD"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFBCC7D8"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF243652"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF385186"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF101E4C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 208, 219, 229));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 102, 146, 201));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 102, 146, 201));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCED4DD"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4B5975"));
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF323C4F"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF323C4F"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
                //LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                //{
                //    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB3C3D6"), Offset = 0d},
                //    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A7D99"), Offset = 1d}
                //}, 0d);
                //brush.StartPoint = new Point(0.570556, 2.58505e-005);
                //brush.EndPoint = new Point(0.570556, 0.99999);
                //return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return Brushes.Maroon; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return Brushes.Red; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFD6DFF2")); }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        #endregion
    }


    #endregion

    public class GridDataDefaultGridVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {

        private Brush _gridBorderBrush;
        public Brush GridBorderBrush
        {
            get
            {
                if (_gridBorderBrush == null)
                {
                    _gridBorderBrush = new SolidColorBrush(ColorExtensions.StringToColor("#FFE2E2E2"));
                }
                return _gridBorderBrush;
            }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4C365"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEEF4FA"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EBF9"),Offset=1d},                                             
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBAC9DB"), Offset = 0.028d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC9D4E4"), Offset = 0.03d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EBF5"), Offset = 0.136d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFCFDBEC"), Offset = 0.523d}, 
                }, 0.0d);
                brush.StartPoint = new Point(0.814, 0.041);
                brush.EndPoint = new Point(0.821, 0.904);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF6F7F7"), Offset=0.072},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFE5E5E5"), Offset=0.928},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFEAF3FE"), Offset=0.07d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD0E5FE"), Offset=0.98d},                                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFAFBFD"), Offset=0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFEDF4FD"), Offset=0.798d},      
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF0F7FE"), Offset=1d},               
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }


        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(Brushes.Black, 0.25d),
                    Right = new Pen(Brushes.Black, 0.25d),
                    Bottom = new Pen(Brushes.Black, 0.25d),
                    Left = new Pen(Brushes.Black, 0.25d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(Brushes.Black, 0.2d),
                    Right = new Pen(Brushes.Black, 0.2d),
                    Bottom = new Pen(Brushes.Black, 0.2d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(Brushes.Black, 0.2d),
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(Brushes.Black, 0.2d),
                };
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5FAFF"), Offset = 0.099d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6F0FA"), Offset = 0.515d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDCE6F4"), Offset = 0.52d}
                }, 0.0d);
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5FAFF"), Offset = 0.099d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6F0FA"), Offset = 0.515d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDCE6F4"), Offset = 0.52d}
                }, 0.0d);
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5FAFF"), Offset = 0.099d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6F0FA"), Offset = 0.515d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDCE6F4"), Offset = 0.52d}
                }, 0.0d);
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF839CBC")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF839CBC")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF839CBC")), 0.2d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return GridFontInfo.Default;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 3d, Right = 3d };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F"));
            }
        }

        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
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
                return GridFontInfo.Default;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFAFBFD"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEDF4FD"), Offset=0.798d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F7FE"), Offset=1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4F4F5"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF829BBB")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF829BBB")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF829BBB")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF829BBB")), 0.2d)
                };
            }
        }

        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF1F1F6"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF1F1F6"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get { return Brushes.Black; }
        }

        public Brush RowHeaderForeground
        {
            get { return Brushes.Gray; }
        }

        public Brush HeaderInnerBorder
        {
            get { return Brushes.Transparent; }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get { return new Thickness(0.2d); }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA1BFE3"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAF3FE"), Offset = 0.07d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD0E5FE"), Offset = 0.9d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F0F0"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F0F0"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 69, 70, 71));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 215, 215, 215));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 141, 154, 183));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF898989"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return new SolidColorBrush(SystemColors.GrayTextColor);
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA8A8A8"));
            }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataOffice2007BlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF6FBFE"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FE"), Offset=1d},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7FBFD"), Offset=0.996d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FD"), Offset=0},                  
                },0.0d);
                brush.StartPoint = new Point(0.823, 0.155);
                brush.EndPoint = new Point(0.823, 1.328);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBEE"), Offset=0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF3D58F"), Offset=0.413},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF1B634"), Offset=0.514},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF7D14A"), Offset=0.887},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFBD88B"), Offset=0.976},                    
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFDD88"), Offset=1},   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF7FAFF"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.2d)
                };
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC4D9FF"), Offset=0.97},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFBFFFF"), Offset=0.03},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC4D9FF"), Offset=0.97},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFBFFFF"), Offset=0.03},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE4FBFD"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC3D8FE"), Offset=1},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBB83D"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1d}
                },0.0d);
                brush.StartPoint = new Point(0.5,0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBEE"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE7B1"), Offset = 0.413},                  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFDCF4C"), Offset = 0.524},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE496"), Offset = 0.929},  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFEDAF"), Offset = 0.976},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFDD88"), Offset = 1},    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB0D7FF"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                        
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.25d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF")), 0.2d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBE292"));
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000B05"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
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
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF"));
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9AC6FF"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush( ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return this.HeaderInnerBorder; 
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC8DFFF"), Offset=1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9AC6FF"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC4C7CF"), Offset=1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.168, 0.081);
                brush.EndPoint = new Point(0.603, 0.712);
                return brush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF8F93AC"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF464B5C"), Offset=1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.388, 0.196);
                brush.EndPoint = new Point(0.394, 0.762);
                return brush;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9AC6FF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataOffice2007SilverVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }


        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDCDDE1"), Offset = 0.888d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF9FAFC"), Offset = 0.056d},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDCDDE1"), Offset=0},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF9FAFC"), Offset=1},                  
                },0.0d);
                brush.StartPoint = new Point(0.659, 0.045);
                brush.EndPoint = new Point(0.659, 0.988);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF707173"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBEE"), Offset=0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF3D58F"), Offset=0.413},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF1B634"), Offset=0.514},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF7D14A"), Offset=0.887},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFBD88B"), Offset=0.976},                    
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFDD88"), Offset=1},   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF5F6F8"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#006F7074"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDDDEE2"), Offset=0.874d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFAFBFD"), Offset=0.082},                 
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDDDEE2"), Offset=0.874d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFAFBFD"), Offset=0.082},                 
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF9FAFC"), Offset=0.943d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDCDDE1"), Offset=0.097},                  
                },0.0d);
                brush.StartPoint = new Point(0.823000013828278, 0.155000001192093);
                brush.EndPoint = new Point(0.823000013828278, 1.32799994945526);
                return brush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")), 0.2d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                        
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 4d, Right = 4d };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6")), 0.25d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo ValueTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 4d };
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBB83D"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset = 0.022d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset = 0.499},                  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset = 0.507d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset = 1},           
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBEE"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE7B1"), Offset = 0.413},                  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFDCF4C"), Offset = 0.524},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE496"), Offset = 0.929},  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFEDAF"), Offset = 0.976},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFDD88"), Offset = 1},    
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCFD2DB"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")), 0.2d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEEEFF3"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBE292"));
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000B05"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6E6D8F"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF304860"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return this.HeaderInnerBorder;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF9F9FA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDBDDE1"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6E6D8F"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC4C7CF"), Offset=1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.168, 0.081);
                brush.EndPoint = new Point(0.603, 0.712);
                return brush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF8F93AC"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF464B5C"), Offset=1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.388, 0.196);
                brush.EndPoint = new Point(0.394, 0.762);
                return brush;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6E6D8F"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7074"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return new SolidColorBrush(SystemColors.GrayTextColor);
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
       public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataOffice2007BlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.046d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0.904d},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.975},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0},                  
                },0.0d);
                brush.StartPoint = new Point(0.659, 0.045);
                brush.EndPoint = new Point(0.659, 0.988);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000802"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBEE"), Offset=0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF3D58F"), Offset=0.413},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF1B634"), Offset=0.514},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF7D14A"), Offset=0.887},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFBD88B"), Offset=0.976},                    
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFDD88"), Offset=1},   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF5F6F8"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#004C535C"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   new GradientStop(){Color = ColorExtensions.StringToColor("#FFFCFDFE"), Offset = 0.061d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD7D7E1"), Offset = 0.896},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF00080E"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   new GradientStop(){Color = ColorExtensions.StringToColor("#FFFCFDFE"), Offset = 0.061d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD7D7E1"), Offset = 0.896},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF00080E"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.046d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0.904d},                  
                },0.0d);
                brush.StartPoint = new Point(0.823000013828278, 0.155000001192093);
                brush.EndPoint = new Point(0.823000013828278, 1.32799994945526);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF400C0C"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBB83D"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get 
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset = 0.022d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset = 0.499},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset = 0.507d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset = 1d}
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBEE"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE7B1"), Offset = 0.413},                  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFDCF4C"), Offset = 0.524},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE496"), Offset = 0.929},  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFEDAF"), Offset = 0.976},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFDD88"), Offset = 1},    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF535353"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")), 0.2d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                   
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBDBDBD")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBDBDBD")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBDBDBD")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBDBDBD")),0.25d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")), 0.2d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE6E7E9"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBE292"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000802"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000B05"));
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9199A4"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF304860"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return this.HeaderInnerBorder;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF9F9FA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD6D9DE"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9199A4"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD6D6D6"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.168, 0.081);
                brush.EndPoint = new Point(0.603, 0.712);
                return brush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF7E7E7E"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B3B3B"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.388, 0.196);
                brush.EndPoint = new Point(0.394, 0.762);
                return brush;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9199A4"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C535C"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
       public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }
        
        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF6FBFE"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FE"), Offset=1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7FBFD"), Offset=0.996d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FD"), Offset=0},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.823, 0.155);
                brush.EndPoint = new Point(0.823, 1.328);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7380A3"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.0597032" EndPoint="0.5,0.911615">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF395286" Offset="0"/>
                //        <GradientStop Color="#FF111E4D" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF395286"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF111E4D"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0597032);
                brush.EndPoint = new Point(0.5, 0.911615);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.0597032" EndPoint="0.5,0.911615">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF395286" Offset="0"/>
                //        <GradientStop Color="#FF111E4D" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF395286"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF111E4D"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0597032);
                brush.EndPoint = new Point(0.5, 0.911615);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4877B3"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF173D8C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.499998, 0.0596964);
                brush.EndPoint = new Point(0.499998, 0.911608);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6593CF"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78DCEFFF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78FFFFFF"), Offset = 0.549451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78E0F1FF"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7687A5"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                   
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB3C3D6")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB3C3D6")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d),
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFAAF40"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7DA4"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF69A2E8"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF3F6F9"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF385186"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF101E4C"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 208, 219, 229));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 102, 146, 201));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 102, 146, 201));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC2CFDE"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6F7DA4"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB3C3D6"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A7D99"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.570556, 2.58505e-005);
                brush.EndPoint = new Point(0.570556, 0.99999);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
       public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF000019")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataSilverVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {

        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDCDDE1"), Offset = 0.888d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF9FAFC"), Offset = 0.056d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDCDDE1"), Offset=0},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF9FAFC"), Offset=1},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.659, 0.045);
                brush.EndPoint = new Point(0.659, 0.988);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF707173"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE8E9EA"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.940303" EndPoint="0.5,0.0883908">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FFBDC1C8" Offset="0"/>
                //        <GradientStop Color="#FFEDEFF0" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBDC1C8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEDEFF0"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.940303);
                brush.EndPoint = new Point(0.5, 0.0883908);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.940303" EndPoint="0.5,0.0883908">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FFBDC1C8" Offset="0"/>
                //        <GradientStop Color="#FFEDEFF0" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBDC1C8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEDEFF0"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.940303);
                brush.EndPoint = new Point(0.5, 0.0883908);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA0A0BA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF4F6F7"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.503593, 0.966534);
                brush.EndPoint = new Point(0.503593, 0.11463);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78D6D7D9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78FFFFFF"), Offset = 0.5d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78CCCCCC"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0384275);
                brush.EndPoint = new Point(0.5, 0.795361);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7687A5"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                    
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE0E0E0")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE0E0E0")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE0E0E0")), 0.3d),
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFECEEEF"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4D0AE"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF898989"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5E5E5E"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF3F6F9"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF766C6C"), Offset = 0.375d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC9BFBF"), Offset = 0.375d},
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.028);
                brush.EndPoint = new Point(0.5, 0.985);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 69, 70, 71));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 0, 0, 25));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF898989"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC8C8C8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF323232"),  Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.569377, 3.01722e-005);
                brush.EndPoint = new Point(0.569377, 1.00003);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d),
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF686868")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataBlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {

        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.046d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0.904d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFBFCFD"), Offset = 0.975},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD6D6E0"), Offset = 0},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.659, 0.045);
                brush.EndPoint = new Point(0.659, 0.988);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD6D6E0"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF55371F"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.0280268" EndPoint="0.5,0.951414">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF2A2A2A" Offset="0"/>
                //        <GradientStop Color="#FF010101" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2A2A2A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF010101"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,0.0280268" EndPoint="0.5,0.951414">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FF2A2A2A" Offset="0"/>
                //        <GradientStop Color="#FF010101" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2A2A2A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF010101"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.0280268);
                brush.EndPoint = new Point(0.5, 0.951414);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF343434"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF010101"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.238967);
                brush.EndPoint = new Point(0.5, -0.0186828);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontWeight = FontWeights.Normal
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                // <LinearGradientBrush StartPoint="0.5,-0.0606109" EndPoint="0.5,1.03035">
                //    <LinearGradientBrush.GradientStops>
                //        <GradientStop Color="#FFCCCCCC" Offset="0"/>
                //        <GradientStop Color="#FFFCFCFC" Offset="0.510989"/>
                //        <GradientStop Color="#FFCCCCCC" Offset="1"/>
                //    </LinearGradientBrush.GradientStops>
                // </LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#78CCCCCC"), Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#78FCFCFC"), Offset= 0.5d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#78CCCCCC"), Offset= 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0606109);
                brush.EndPoint = new Point(0.5, 1.03035);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCACAD8"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontWeight = FontWeights.Normal,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA39D99"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA39D99"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1A1A1A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1A1A1A"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 202, 202, 216));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 160, 87, 29));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 160, 87, 29));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA39D99"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA39D99"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2A2A2A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF010101"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.500001, 0.0597219);
                brush.EndPoint = new Point(0.500001, 0.911631);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA0571D"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF454545"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF423E38")), 0.3d)
                };
            }
        }

        #endregion
    }

    public class GridDataSyncBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {

        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFBDA"),Offset=0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset=0.499d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD767"),Offset=0.507d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=0.996d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0.012d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=0.304d},     
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset=0.31d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset=1d}                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFD4A1"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA48D62"),Offset=1d},                                        
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF6FBFE"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FE"), Offset=1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7FBFD"), Offset=0.996d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC3D8FD"), Offset=0},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.823, 0.155);
                brush.EndPoint = new Point(0.823, 1.328);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBDA"), Offset=0.022},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A8"), Offset=0.499},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFD767"), Offset=0.507},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE59C"), Offset=1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFBD69"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8A567"), Offset=0.006},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFA83D"), Offset=0.298},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFF8D00"), Offset=0.304},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFC551"), Offset=0.963},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFBC35"), Offset=1},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {               
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF578FD4"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF538ACF"), Offset = 0.318681d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3360A2"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF305C9E"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }


        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF578FD4"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF538ACF"), Offset = 0.318681d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3360A2"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF305C9E"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF84EDF7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF42B3DE"), Offset = 0.357143d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF429CC2"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2B8FBA"), Offset = 0.511014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0085AA"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF49B9E4"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {               
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78E1F3FC"), Offset=  0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78FFFFFF"), Offset = 0.549451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78E6F5FC"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4A88C6"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                   
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD0DBE5")), 0.3d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF5F5F5"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6BC2E7"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2F5BB7"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2F5C9E"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF548DD1"), Offset=  1d}
                }, 0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 225, 243, 252));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 225, 243, 252));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF84EDF7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF42B3DE"), Offset = 0.357143d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF429CC2"), Offset = 0.510964d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2B8FBA"), Offset = 0.511014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0085AA"), Offset = 0.604396d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF49B9E4"), Offset = 1d}
                }, 0d);
                brush.StartPoint = new Point(0.500003, 0.032804);
                brush.EndPoint = new Point(0.500003, 0.854236);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF305CB7"));
            }
        }

        public Brush GroupingIndicatorInnerBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }

        public Brush GroupingIndicatorOuterBrush
        {
            get { return this.FilterButtonOuterBrush; }
        }

        public Brush GroupingIndicatorHoverInnerBrush
        {
            get { return this.FilterButtonHoverInnerBrush; }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d),
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.3d)
                };
            }
        }

        #endregion
    }   

    public class GridDataBlendVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF676767"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF676767"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF525252"));
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2B2B2B"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF535352"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6C6C"));
            }
        }


        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAAAAAA"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }



        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2B2B2B"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF525252"));
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                    
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF535352"));
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF444444"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2B2B2B")), 0.3d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                       
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2B2B2B")), 0.23d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 3d, Right = 3d };               
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.White;
            }
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
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF535352"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3A3A3A"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3A3A3A"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Colors.White);
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 215, 215, 215));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
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
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF22282F"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE2E2E2"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return Brushes.Transparent;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }    

    public class GridDataGlassyGreenVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF629110")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0.229d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEAF390"), Offset=0.974d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.101000003516674, 0.0879999995231628);
                brush.EndPoint = new Point(0.791999995708466, 0.860000014305115);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0.229d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF898F4A"), Offset=0.974d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.101000003516674, 0.0879999995231628);
                brush.EndPoint = new Point(0.791999995708466, 0.860000014305115);
                return brush;

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF263007"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF263007"));

            }
        }



        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA1C240"), Offset = 0.16d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF6B963A"), Offset=0.991d},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF6E973A"), Offset=0.013d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA0C140"), Offset=0.782d},                  
                },0.0d);
                brush.StartPoint = new Point(0.823000013828278, 0.155000001192093);
                brush.EndPoint = new Point(0.823000013828278, 1.32799994945526);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF629110"));
            }
        }


        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("White"), Offset=0.229d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD9DFA3"), Offset=0.974d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.101,0.088);
                brush.EndPoint = new Point(0.792,0.86);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8C8C8C"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6D993A"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBFBFB"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF586458"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA2C340"), Offset = 0.177},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF67943A"), Offset = 1},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA2C340"), Offset = 0.177},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF67943A"), Offset = 1},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6ECDF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF87A167"), Offset = 0.024d},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6D9240"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9FC140"), Offset = 0.741d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA1C240"), Offset = 0.953d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF87A73D"), Offset = 0.322d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }



        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                   
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF92B80D"), Offset = 0.229},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB0C8F0"), Offset = 0.974}, 
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }       

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB2CD52"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return Brushes.White;
            }

        }
        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.23d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,                              
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.23d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.23d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF376E35"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB7C064")), 0.25d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA3B26B"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF376E35"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.White;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return Brushes.Transparent;
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.25d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF92B80D"), Offset = 0.965},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB0CC50"), Offset = 0.335}, 
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF376E35"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3"));
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB08551"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
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
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0.229d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB0C8F0"), Offset = 0.974d},                   
                },0.0d);
                brush.StartPoint = new Point(0.101,0.088);
                brush.EndPoint = new Point(0.792,0.86);
                return brush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
       public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataSunBlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF635552"), Offset = 0.14d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF49382D"), Offset=0.5d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF3A2115"), Offset = 0.5d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAF4802"), Offset=0.986d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAF4802"), Offset = 1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5,0);
                brush.EndPoint = new Point(0.5,1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF24201F"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF635552"), Offset = 0.14d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF49382D"), Offset=0.5d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF3A2115"), Offset = 0.5d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAF4802"), Offset=0.986d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAF4802"), Offset = 1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF645753"), Offset=0.03d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF49372C"), Offset=0.5d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF362117"), Offset=0.5d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC0672A"), Offset=1d},                                       
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF61544F"), Offset=0.014d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4A382D"), Offset=0.5d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF372217"), Offset=0.5d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4D2D17"), Offset=0.999d},                      
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918"));
            }
        }


        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA092"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2B2929"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2C2A2A"), Offset=0.027},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF2E2C2C"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF24201F"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8E4C32"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("Transparent"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF413E3B"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF494643"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF655855"), Offset = 0.02},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF49372C"), Offset = 0.5d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF352016"), Offset = 0.5d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB64A01"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEBEBEB"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF655855"), Offset = 0.02},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF49372C"), Offset = 0.5d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF352016"), Offset = 0.5d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB64A01"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEBEBEB"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF61544F"), Offset = 0.014d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4A382D"), Offset = 0.5},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF372217"), Offset = 0.5},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4D2D17"), Offset = 0.999},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                  
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF5D5350"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFCC6431"), Offset = 1d},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF81807E"), Offset = 0.016d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF575451"), Offset = 0.988d},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")), 0.25d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,   
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")),0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")),0.25d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD0D0D0"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF413E3B"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF24201F"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF707070"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCD7244"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.White;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFEB74A"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF24201F"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC96928"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBF4D00"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
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
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6A0100"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return PlusMinusButtonBackground;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataShinyRedVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDD9493"), Offset = 0.18d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFB83C36"), Offset=0.469d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA2241E"), Offset = 0.513d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBC5B53"), Offset=0.783d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF4D4C7"), Offset = 1d},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4D4C7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }



        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE3A0A0"), Offset=0.03d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFB73832"), Offset=0.496d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA2221C"), Offset=0.501d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE9DB"), Offset=1d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBD5C54"), Offset=0.769d},                     
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE3A0A0"), Offset=0.03d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFB73832"), Offset=0.498d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA2221C"), Offset=0.506d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF701A19"), Offset=0.997d},                      
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9A9A9A"), Offset = 0.038d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A6A6A"), Offset = 0.614},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF777777"), Offset = 0.633},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB6B6B6"), Offset = 1d},                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAECEB"), Offset = 0.023d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBCBDBF"), Offset = 0.402},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3A7AA"), Offset = 0.41},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3A7AA"), Offset = 0.448d}, 
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF979CA0"), Offset = 0.456},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF74787B"), Offset = 0.938d}, 
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEDB7B7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFCF6967"), Offset = 0.4},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB83B37"), Offset = 0.45},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFAE231E"), Offset = 0.5d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF8F8F8"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6A6A7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB83731"), Offset = 0.5},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3221C"), Offset = 0.5},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6D1919"), Offset = 1d},                    
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6A6A7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB83731"), Offset = 0.5},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3221C"), Offset = 0.5},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6D1919"), Offset = 1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE3A0A0"), Offset = 0.003},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB73832"), Offset = 0.496},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA2221C"), Offset = 0.501},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE9DB"), Offset = 1},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBD5C54"), Offset = 0.769d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9A9A9A"), Offset = 0.038d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A6A6A"), Offset = 0.614d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF777777"), Offset = 0.633d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB6B6B6"), Offset = 1d}
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD1D1D1"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBECACA")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF5D5A51")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB6ABA9")), 0.25d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                   
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.20d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.20d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")),0.20d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")),0.20d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEAEAEA"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF611716"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.White;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFDFA"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF353332"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBCBCBC"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFAAAAAA"), Offset = 0.023},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF787875"), Offset = 0.421},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9FA1A0"), Offset = 0.966},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC8C8C6"), Offset = 1},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAB6B8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF651B1C"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
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
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6A0100"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5F1514"));
            }
        }

        public Brush PlusMinusButtonForeground
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
                return PlusMinusButtonBackground;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataShinyBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF9BCEF6"), Offset = 0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4092DE"), Offset=0.366d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1175D2"), Offset = 0.406d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF0168C6"), Offset=0.503d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF08477D"), Offset = 0.987d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF203F5A"), Offset = 1d},    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4D4C7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA4D5F9"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4193E0"), Offset=0.343d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1276D4"), Offset=0.392d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF0168C8"), Offset=0.401d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF035DB0"), Offset=0.608d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF559ACB"), Offset=0.804d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF93D5FA"), Offset=0.951d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC5E9FE"), Offset=1d},                                               
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA4D5F9"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF4193E0"), Offset=0.384d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1276D4"), Offset=0.412d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF0168C8"), Offset=0.506d}, 
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF08477D"), Offset=0.973d} 
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9A9A9A"), Offset = 0.038d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A6A6A"), Offset = 0.614},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF777777"), Offset = 0.633},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB6B6B6"), Offset = 1d},                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAECEB"), Offset = 0.023d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBCBDBF"), Offset = 0.402},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3A7AA"), Offset = 0.41},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA3A7AA"), Offset = 0.448d}, 
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF979CA0"), Offset = 0.456},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF74787B"), Offset = 0.938d}, 
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9BCEF6"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4092DE"), Offset = 0.366},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1175D2"), Offset = 0.406},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0168C6"), Offset = 0.503},     
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF08477D"), Offset = 0.987},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF203F5A"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF8F8F8"));
            }
        }



        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5AA4E6"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA4D5F9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4193E0"), Offset = 0.384},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1276D4"), Offset = 0.412},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0168C8"), Offset = 0.506},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF08477D"), Offset = 0.973},                   
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA4D5F9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4193E0"), Offset = 0.384},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1276D4"), Offset = 0.412},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0168C8"), Offset = 0.506},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF08477D"), Offset = 0.973},                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA4D5F9"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4193E0"), Offset = 0.343},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF1276D4"), Offset = 0.392},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0168C8"), Offset = 0.441},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF035DB0"), Offset = 0.608},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF559ACB"), Offset = 0.804},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF93D5FA"), Offset = 0.951},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC5E9FE"), Offset = 1d},                    
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9A9A9A"), Offset = 0.038},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6A6A6A"), Offset = 0.614d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF777777"), Offset = 0.633d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB6B6B6"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD1D1D1"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF611716"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")), 0.25d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")),0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")),0.25d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF9ACEF6")), 0.09d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF4092DF")), 0.376d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF1074D2")), 0.416d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF08477D")), 0.991d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEAEAEA"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return this.ValueForegroundBrush;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.White;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF353332"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBCBCBC"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFAAAAAA"), Offset = 0.023},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF787875"), Offset = 0.421},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9FA1A0"), Offset = 0.966},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC8C8C6"), Offset = 1},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
			// The Color is changed from (ColorExtensions.StringToColor("#FFFFFFFF") to Black due to cell value is not visible while Grouped the nested grid and selected the row . 
            return Brushes.Black;  

            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF143E68"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA1D5FC"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1564AF"));
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1564AF"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
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
                return PlusMinusButtonBackground;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataBureauBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEBA50"), Offset = 0.015d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFE9A05"), Offset=0.029d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFE6800"), Offset = 0.985d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD85800"), Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF90DCF7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7F7F7"), Offset=0.23d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE8E8E8"), Offset=0.991d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBCBCBC"), Offset=1d},                                                      
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBEBEBE"), Offset = 0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDEDEDE"), Offset = 0.249d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD5D5D5"), Offset = 0.956d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC6C6C6"), Offset = 0.961d},                     
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEDECEC"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFD9605"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFD6D01"), Offset = 0.918d},                                                
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFEBA50"), Offset = 0.015d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFE9A05"), Offset = 0.029d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFE6800"), Offset = 0.985d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD85800"), Offset = 1d},                                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFAFAFA"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD3E1EF"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F0F0"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD9D9D9"), Offset = 1d},                    
                },0.0d);
                brush.StartPoint = new Point(0.5,0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F0F0"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD9D9D9"), Offset = 1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }


        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF8F8F8"), Offset = 0.202},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFECECEC"), Offset = 0.986},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB8B8B8"), Offset = 0.993},                    
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF323232"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFEFEFE"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF7F7F7"), Offset = 0.207},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF4F4F4"), Offset = 1},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC6DFFF"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.25d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                   
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")),0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")),0.25d)

                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF48525B"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1C9DF")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF91B5DB")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF98BEE5")), 0.25d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE6EEF7"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFEECA6"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.MidnightBlue;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFD7401"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF96DDF7"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF0BB2F0"), Offset = 1},                   
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBFD1E8"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC0D2EA"), Offset = 1d}
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.AliceBlue;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD6D6D6"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.168, 0.081);
                brush.EndPoint = new Point(0.603, 0.712);
                return brush; ;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFADADAD"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF838383"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.388, 0.196);
                brush.EndPoint = new Point(0.394, 0.762);
                return brush;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3E6398"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3E6398"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5B5B5B")); 
            }
        }

        public Brush PlusMinusButtonForeground
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
                return PlusMinusButtonBackground;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataBureauBlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(SystemColors.GrayTextColor); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEBA50"), Offset = 0.015d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFE9A05"), Offset=0.029d},      
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFE6800"), Offset = 0.985d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD85800"), Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF90DCF7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF7F7F7"), Offset=0.23d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE8E8E8"), Offset=0.991d},         
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBCBCBC"), Offset=1d},                                                      
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBEBEBE"), Offset = 0.022d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDEDEDE"), Offset = 0.249d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD5D5D5"), Offset = 0.956d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC6C6C6"), Offset = 0.961d},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF2860D"), Offset=0.08},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD9600F"), Offset=0.97},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF19A17"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFEFF7FD"), Offset=0.056d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFDEEEF9"), Offset=0.944d},      
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFFFFF"), Offset=1d},               
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }


        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF414141"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDCDFE5"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0.5d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC9CED5"), Offset = 1d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9FA8B8"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBDC7D5"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1E7EE"), Offset = 0d},
                },0.0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC9CED5"), Offset = 1d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9FA8B8"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBDC7D5"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1E7EE"), Offset = 0d},
                }, 0.0d);
                brush.StartPoint = new Point(0.5, -0.0430693);
                brush.EndPoint = new Point(0.5, 0.928826);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFEFAE7"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFDEFA2"), Offset = 0.518d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFECA24"), Offset = 0.522d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFEE14F"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, -0.0511562);
                brush.EndPoint = new Point(0.5, 1.00518);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF475476"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78F6CB72"), Offset =  0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78F89937"), Offset = 0.549451d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#78F18634"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, -0.00285119);
                brush.EndPoint = new Point(0.5, 0.825156);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF858584"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF808080")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D")), 0.1d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 10d,                   
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E")), 0.15d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2E2E2E")), 0.15d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF808080")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D")), 0.1d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3939"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCBCFD5"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9E9E9"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.Black;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF6AC65"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return this.ValueBackgroundBrush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1E7EE"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC9CED5"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3E6398"));
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF858584"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFACB2BD"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
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
                return this.HeaderInnerBorder;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF808080")), 0.3d)
                };
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d)
                };
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2")), 0.1d)
                };
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return new CellBordersInfo()
                {
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D")), 0.1d)
                };
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataTwilightBlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFA2E2F9"), Offset = 0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF00AFF0"), Offset=0.998d},                                
                }, 0.0d);
                brush.StartPoint = new Point(0.832, 0.472);
                brush.EndPoint = new Point(0.168, 0.528);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF9CE0F8"), Offset=0.992d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF05B0E2"), Offset=0d},                                                                                   
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF99DDF7"), Offset=0.08d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF09B0EE"), Offset=1d},   
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7"));
            }
        }


        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return GroupCaptionSelectionBackground;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFBBD878"), Offset = 0.01d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF8ACB22"), Offset = 0.874d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF92CA11"), Offset = 0.906d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF8DC61E"), Offset = 0.979d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF83CA0E"), Offset = 1d},                   
                }, 0.0d);
                brush.StartPoint = new Point(0, 0.5);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7FD7F7"));
            }
        }





        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFAFAFA"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA2E2F9"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF00AFF0"), Offset = 0.999},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFA2E2F9"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF00AFF0"), Offset = 0.999},                  
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC6F0FF"), Offset = 0},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF07B0EF"), Offset = 0.999},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7FD7F7"));
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE5F7FD"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF49535C"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1CFDA")), 0.25d),
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1CFDA")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1CFDA")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1CFDA")), 0.25d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                   
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1CFDA")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1CFDA")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1CFDA")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFB1CFDA")), 0.25d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF49535C"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFEFEFE"));
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE5F7FD"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCBEB85"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF49535C"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF44AEEC"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF44AEEC"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC1DCEF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF89B9D9"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.SortWidgetBrush;
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
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
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF858584"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFACB2BD"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5B5B5B"));
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataOffice14BlueVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        private Brush _gridBorderBrush;
        public Brush GridBorderBrush
        {
            get
            {
                if (_gridBorderBrush == null)
                    _gridBorderBrush = new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
                return _gridBorderBrush;
            }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFF861A"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEEF4FA"),Offset=0},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EBF9"), Offset=1d},                                                                                   
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD8E6F7"), Offset=0.049d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEEF4FA"),Offset=0.951d},   
                },0.0d);
                brush.StartPoint = new Point(0.821, 0.964);
                brush.EndPoint = new Point(0.814, 0.101);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0")); }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B")); }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF9F9F9"), Offset=0.054d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFC6DDF1"), Offset=0.406d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD5E5F4"),Offset=0.975d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF5FDFE"), Offset=0.987d},         
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF758EAB"), Offset = 0.014},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF8AA3C2"), Offset = 0.019}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF8AA3C2"), Offset=0.042},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF95AECD"), Offset=0.047},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF9FB7D6"), Offset=0.948},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF96AECD"), Offset=0.977}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF87A0BC"), Offset=0.981}, 
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1ECFA"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d)
                };
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF5FB"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1ECFA"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF5FB"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1ECFA"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD9E7F8"), Offset = 0.031d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF5FB"), Offset = 0.963d},                    
                },0.0d);
                brush.StartPoint = new Point(0.781, 0.014);
                brush.EndPoint = new Point(0.779, 0.987);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF768CA7"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF9F9F9"), Offset = 0.054d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC6DDF1"), Offset = 0.406d},   
                     new GradientStop(){Color = ColorExtensions.StringToColor("#FFD5E5F4"), Offset = 0.975d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5FDFE"), Offset = 0.987d},         
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFC8DBEF"), Offset = 0.058d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB3CBE5"), Offset = 0.598d},   
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFCFE2F2"), Offset = 0.984d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F")); 
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCFDDEE"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                   
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.23d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")),0.23d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F")); 
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCCDBEC"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCCDBEC"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDBE3ED"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA8A8A8"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA8A8A8"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE1ECFA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF5FB"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD6D6D6"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.168, 0.081);
                brush.EndPoint = new Point(0.603, 0.712);
                return brush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF7E7E7E"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B3B3B"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.388, 0.196);
                brush.EndPoint = new Point(0.394, 0.762);
                return brush;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4FCFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataOffice14BlackVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFF861A"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE9E9E9"),Offset=0.134d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE0E0E0"),Offset=0.768d},                                                                                   
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFDEDEDE"), Offset=0.096d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE9E9E9"), Offset=0.892d},   
                },0.0d);
                brush.StartPoint = new Point(0.814, 0.101);
                brush.EndPoint = new Point(0.794, 0.813);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFA9C1DE")); }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF21272D")); }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF3D3D3D"), Offset=0},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF434343"), Offset=0.026}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF424242"), Offset=1},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF7D7D7D"), Offset=0.04d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF4F4F4F"), Offset=0.598d},  
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF555555"), Offset=0.879d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF6B6B6B"), Offset=0.984d},            
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB4B4B4"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.23d)
                };
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAEAEA"), Offset = 0.08d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDFDFDF"), Offset = 0.936d},                   
                },0.0d);
                brush.StartPoint = new Point(0.032, 0.066);
                brush.EndPoint = new Point(0.032, 0.957);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF363636"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAEAEA"), Offset = 0.08d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDFDFDF"), Offset = 0.936d},                   
                }, 0.0d);
                brush.StartPoint = new Point(0.032, 0.066);
                brush.EndPoint = new Point(0.032, 0.957);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF363636"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   new GradientStop(){Color = ColorExtensions.StringToColor("#FFDFDFDF"), Offset = 0.069d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAEAEA"), Offset = 0.931d},
                },0.0d);

                brush.StartPoint = new Point(0.789, 0.014);
                brush.EndPoint = new Point(0.779, 0.987);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF313131"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF7D7D7D"), Offset = 0.04d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4F4F4F"), Offset = 0.598d},  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF555555"), Offset = 0.879d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6B6B6B"), Offset = 0.984d},            
                },0.0d);
                brush.StartPoint = new Point(0.5,0);
                brush.EndPoint = new Point(0.5,1);
                return brush; 
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF5B5B5B"), Offset = 0.043d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF484848"), Offset = 0.399d},  
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF505050"), Offset = 0.745d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF6B6B6B"), Offset = 0.973d},            
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.052);
                brush.EndPoint = new Point(0.5, 1);
                return brush; 
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                   
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.23d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")),0.23d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.25d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return Brushes.Gray;
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return Brushes.White;
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4E4E4"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA8A8A8"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAEAEA"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDFDFDF"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD6D6D6"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.168, 0.081);
                brush.EndPoint = new Point(0.603, 0.712);
                return brush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF7E7E7E"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B3B3B"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.388, 0.196);
                brush.EndPoint = new Point(0.394, 0.762);
                return brush;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB2B2B2"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataOffice14SilverVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFF861A"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEFF2F6"),Offset=0.002d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EBF9"),Offset=1d},                                                                                   
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EAF8"), Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEEF1F5"), Offset=1d},   
                },0.0d);
                brush.StartPoint = new Point(0.814, 0.101);
                brush.EndPoint = new Point(0.794, 0.813);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFA7CDF0")); }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B")); }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA6ACB4"), Offset=0.02},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB0B8C0"), Offset=0.025}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFBCC3CB"), Offset=0.204},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFBCC3CB"), Offset=0.975},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB4BAC2"), Offset=0.98}
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }


        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD7DCE2"), Offset=0.213d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD4DAE1"), Offset=0.622d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFE6ECF1"), Offset=0.975d},      
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.23d)
                };
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5F7F9"), Offset = 0d}, 
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF2F6"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.0320000015199184, 0.111000001430511);
                brush.EndPoint = new Point(0.0320000015199184, 0.912000000476837);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5F7F9"), Offset = 0d}, 
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF2F6"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.0320000015199184, 0.111000001430511);
                brush.EndPoint = new Point(0.0320000015199184, 0.912000000476837);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFBAC1C9"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = new FontWeight(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get 
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE3E8ED"), Offset = 0.4d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF4F6F9"), Offset = 0.032d},                   
                },0.0d);
                brush.StartPoint = new Point(0.5,0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD7DCE2"), Offset = 0.213d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD4DAE1"), Offset = 0.622d},   
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6ECF1"), Offset = 0.975d},      
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F"));
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4E8ED"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                   
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")),0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.23d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")),0.23d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 1d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4E8ED"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4E8ED"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDADADA"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return this.HeaderInnerBorder;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5F7F9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEFF2F6"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD6D6D6"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.168, 0.081);
                brush.EndPoint = new Point(0.603, 0.712);
                return brush;
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF7E7E7E"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B3B3B"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.388, 0.196);
                brush.EndPoint = new Point(0.394, 0.762);
                return brush;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4FCFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return new SolidColorBrush(SystemColors.ControlColor);
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataVS2010VisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAEB3B8"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF0EEE7"),Offset=0.01d},           
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEFAEF"),Offset=0.042d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFF4D0"),Offset=0.422d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.484d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.958d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF293955"),Offset=0.99d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF47566E"),Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4C365"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFAEB3B8"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF0EEE7"),Offset=0.01d},           
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFEFAEF"),Offset=0.042d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFF4D0"),Offset=0.422d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.484d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.958d},        
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF293955"),Offset=0.99d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF47566E"),Offset=1d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1A283D"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1A283D"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1A283D"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB3B8BF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE5E5E1"), Offset = 0.01d},       
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBF2"), Offset = 0.02d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFF4D0"), Offset = 0.459d},         
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A6"), Offset = 0.551d},                       
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF75849F"), Offset = 0.775d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3E5378"), Offset = 0.115d},                        
                }, 0.0d);
                brush.StartPoint = new Point(0.508, 0.034);
                brush.EndPoint = new Point(0.509, 1.147);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return Brushes.Transparent;
            }
        }


        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEFAEF"), Offset=0},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEF3CF"), Offset=0.416}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEE7A5"), Offset=0.584},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get { return Brushes.Black; }
        }

        public Brush CurrentCellSelectionBackground
        {
            get { return Brushes.White; }
        }

        public Brush CurrentCellSelectionForeground
        {
            get { return Brushes.Black; }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFFBEF"), Offset=0.064d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFF3CF"), Offset=0.485d},      
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFECB5"), Offset=0.491d},               
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }


        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }


        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2"));              
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4A5D80"), Offset = 0.225d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3E5378"), Offset = 0.885d},                    
                },0.0d);
                brush.StartPoint = new Point(0.5,0);
                brush.EndPoint = new Point(0.5, 1);

                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF4A5D80"), Offset = 0.225d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3E5378"), Offset = 0.885d},                    
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);

                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFB3B8BF"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE5E5E1"), Offset = 0.01d},       
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBF2"), Offset = 0.02d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFF4D0"), Offset = 0.459d},         
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFE8A6"), Offset = 0.551d},                       
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.Regular,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBEF"), Offset = 0.064d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFF3CF"), Offset = 0.485d},      
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFECB5"), Offset = 0.491d},               
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF9CAAC1"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8495A9")), 0.2d),
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8495A9")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8495A9")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8495A9")), 0.2d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                         
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")), 0.2d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")),0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")),0.2d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1B293E"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d

                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFBCC7D8"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFEECA6"));              
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1B293E"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1B293E"));
            }
        }

        public GridFontInfo SummaryRowFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),                    
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush RowHeaderSelectionBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFCED4DD"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9C256"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF8EDCC"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFFFBED"), Offset = 0.023d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFBEBBF"), Offset = 0.438d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF9DD8A"), Offset = 0.539d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF7D572"), Offset = 0.977d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFBD972"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE9ECEE"), Offset = 0.012d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFD0D7E2"), Offset = 0.998d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF9BA7B7"), Offset = 1d}
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                return Brushes.Black;
                //return this.HeaderForegroundBrush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return this.FilterButtonInnerBrush;
                //return new SolidColorBrush(ColorExtensions.StringToColor("#FF739AC1");
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                //return Brushes.Red;
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF7E7E7E"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FF3B3B3B"), Offset = 1d},
                }, 0.0d);
                brush.StartPoint = new Point(0.388, 0.196);
                brush.EndPoint = new Point(0.394, 0.762);
                return brush;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1A283D"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return PlusMinusButtonBackground;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {

            get
            { return new SolidColorBrush(ColorExtensions.StringToColor("#FF323C4F")); }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF323C4F")); }
        }

        public Brush GroupAreaForegroundBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000")); }
        }

        public Brush FilterButtonAppliedBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF4B5975")); }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFD6DFF2")); }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataWindows7VisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFE2E2E2")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4C365"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            }
        }


        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFEEF4FA"),Offset=0d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EBF9"),Offset=1d},                                             
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFBAC9DB"), Offset = 0.028d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFC9D4E4"), Offset = 0.03d},   
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE1EBF5"), Offset = 0.136d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFCFDBEC"), Offset = 0.523d}, 
                },0.0d);
                brush.StartPoint = new Point(0.814, 0.041);
                brush.EndPoint = new Point(0.821, 0.904);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF6F7F7"), Offset=0.072},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFE5E5E5"), Offset=0.928},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get 
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFE8F1F9"), Offset=0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFABCFFD"), Offset=1d},                                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFAFBFD"), Offset=0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFEDF4FD"), Offset=0.798d},      
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF0F7FE"), Offset=1d},               
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }


        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5FAFF"), Offset = 0.099d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6F0FA"), Offset = 0.515d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDCE6F4"), Offset = 0.52d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }


        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5FAFF"), Offset = 0.099d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6F0FA"), Offset = 0.515d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDCE6F4"), Offset = 0.52d}
                }, 0.0d);
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF5FAFF"), Offset = 0.099d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE6F0FA"), Offset = 0.515d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFDCE6F4"), Offset = 0.52d}
                }, 0.0d);
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC")), 0.2d),
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC")), 0.2d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC")), 0.2d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC")), 0.2d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return GridFontInfo.Default;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 3d, Right = 3d };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC")), 0.25d),
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC")), 0.25d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F"));
            }
        }

        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
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
                return GridFontInfo.Default;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFFAFBFD"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEDF4FD"), Offset=0.798d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F7FE"), Offset=1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4F4F5"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF829BBB")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF829BBB")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF829BBB")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF829BBB")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF1F1F6"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFEAEAEA"), Offset=0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFCCD9EA"), Offset=0.007d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8EEF7"), Offset=0.046d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
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
            get 
            {
                return Brushes.Transparent;
            }
        }

        public Brush RowHeaderForeground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));
            }
        }

        public Brush HeaderInnerBorder
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC8BCBC"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get { return new Thickness(0.1d); }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7DA2CE"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFE8F1F9"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFABCFFD"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return Brushes.Black;
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F0F0"), Offset = 0d},
                    new GradientStop(){Color = ColorExtensions.StringToColor("#FFF0F0F0"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.016);
                brush.EndPoint = new Point(0.5, 0.999);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 69, 70, 71));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 215, 215, 215));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 141, 154, 183));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6B7987"));
                //return Brushes.Red;
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF898989"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000019"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return new SolidColorBrush(SystemColors.GrayTextColor);
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF827E7E"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return this.HeaderInnerBorder;
            }
        }

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }

    }

    public class GridDataSyncfusionVisualStyle : IGridDataNestedVisualStyle, IGridDataVisualStyle
    {
        public Brush GridBorderBrush
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")); }
        }

        public Thickness GridBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush RowHeaderBackground
        {
            get { return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return this.PlusMinusButtonBackground;
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return this.PlusMinusButtonBorderBrush;
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return this.PlusMinusButtonForeground;
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF2860D"),Offset = 0.08d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD9600F"),Offset = 0.97d},                                             
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6"));

            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFF2860D"),Offset = 0.08d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFD9600F"),Offset = 0.97d},                                             
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1F65A9"),Offset = 0.009d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF144691"),Offset = 0.991d},                                             
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF1F62A7"), Offset = 0.051d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FF154792"), Offset = 0.987d},                     
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF628CB6"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF2860D"), Offset=0.08},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD9600F"), Offset=0.97},                     
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1F62A6"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFEFF7FD"), Offset=0.056d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFDEEEF9"), Offset=0.944d},      
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFFFFF"), Offset=1d},               
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush RowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }



        public Brush HeaderInnerBorder
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get
            {
                return new Thickness(0d);
            }
        }

        public Brush HeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color = ColorExtensions.StringToColor("#FF2066AB"), Offset = 0d},
                    new GradientStop{Color = ColorExtensions.StringToColor("#FF144591"), Offset = 1d},                   
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }


        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color = ColorExtensions.StringToColor("#FF2066AB"), Offset = 0d},
                    new GradientStop{Color = ColorExtensions.StringToColor("#FF144591"), Offset = 1d},                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color = ColorExtensions.StringToColor("#FF1F65A9"), Offset = 0.009d},
                    new GradientStop{Color = ColorExtensions.StringToColor("#FF144691"), Offset = 0.991d},                  
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush SortWidgetBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public GridFontInfo ValueFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,                    
                };
            }
        }

        public Brush HighlightBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color = ColorExtensions.StringToColor("#FFEFF7FD"), Offset = 0.056d},
                    new GradientStop{Color = ColorExtensions.StringToColor("#FFDEEEF9"), Offset = 0.944d},
                    new GradientStop{Color = ColorExtensions.StringToColor("#FFFFFFFF"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEEF6FD"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2C367E"));
            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF628CB6")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF628CB6")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF628CB6")), 0.23d)
                };
            }
        }

        public GridFontInfo HeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,                    
                };
            }
        }

        public CellBordersInfo ValueCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.23d), // newly added left and right
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.23d)
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2C367E"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get
            {
                return this.HeaderFont;
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.3d)
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush ValueBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public CellMarginsInfo HeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                    Right = 4d
                };
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

        public Brush SummaryCaptionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDFEEF9"));
            }
        }

        public Brush SummaryRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFEECA6"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2C367E"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get
            {
                return this.ValueFont;
            }
        }

        public Brush SummaryRowForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2C367E"));
            }
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
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush RowHeaderForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE1F3FC"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF3870D"));
            }
        }

        public double CurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color = ColorExtensions.StringToColor("#FFF3870D"), Offset = 0.018d},
                    new GradientStop{Color = ColorExtensions.StringToColor("#FFD05311"), Offset = 0.996d},
                    new GradientStop{Color = ColorExtensions.StringToColor("#FFFF865D"), Offset = 1d}
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush HighlightSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color = ColorExtensions.StringToColor("#FF2F5C9E"), Offset = 0d},
                    new GradientStop{Color = ColorExtensions.StringToColor("#FF548DD1"), Offset = 1d},
                },0.0d);
                brush.StartPoint = new Point(0.5, 0.984);
                brush.EndPoint = new Point(0.5, 0.001);
                return brush;
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 208, 219, 229));
                return brush;
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 225, 243, 252));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 225, 243, 252));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
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
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return this.HeaderInnerBorder;
            }
        }

        #region IGridDataNestedVisualStyle Members

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        #endregion

        public Brush DragDropIndicatorBrush
        {
            get { return this.FilterButtonInnerBrush; }
        }
        public Brush DragDropIndicatorOuterBrush
        {
            get { return Brushes.Gray; }
        }
    }

    public class GridDataMetroVisualStyle : IGridDataVisualStyle, IGridDataNestedVisualStyle
    {
        public Brush GridBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD1D1D1"));
            }
        }

        public Thickness GridBorderThickness
        {
            get
            {
                return new Thickness(1d);
            }
        }

        public Brush RowHeaderBackground
        {
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9E9E9")); } //return this.HeaderBackgroundBrush; }
        }

        public Brush HeaderHoverForegroundBrush
        {
            get
            {
                return this.HeaderForegroundBrush;
            }
        }
        public Brush HeaderBackgroundBrush
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF119EDA"));
            }
        }

        public Brush HeaderForegroundBrush
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterPopupBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF119EDA"));
            }
        }

        public Brush FilterPopupForegroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush HeaderHoverBackgroundBrush
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5EC8F5"));

            }
        }

        public CellBordersInfo HeaderCellBorders
        {
            get 
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.23d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.23d)
                };
            }
        }

        public Brush HeaderInnerBorder
        {
            get 
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Thickness HeaderInnerBorderThickness
        {
            get 
            {
                return new Thickness(0);
            }

        }

        public GridFontInfo HeaderFont
        {
            get 
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe WP"),
                    FontSize = 14d,
                    FontWeight = FontWeights.Normal,
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
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.23d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.23d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.23d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.23d),
                };
            }
        }

        public Brush ValueForegroundBrush
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));
            }
        }

        public Brush ValueBackgroundBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public CellMarginsInfo ValueTextMargins
        {
            get 
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                };
            }
        }

        public GridFontInfo ValueFont
        {
            get 
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe WP"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    FontWeight = FontWeights.Normal,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public Brush SortWidgetBrush
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush GroupAreaBackgroundBrush
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF4F4F4"));
            }
        }

        public Brush GroupAreaForegroundBrush
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));
            }
        }

        public GridFontInfo GroupHeaderFont
        {
            get 
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe WP"),
                    FontSize = 11d,
                };
            }
        }

        public CellBordersInfo GroupCellBorders
        {
            get 
            {
                return new CellBordersInfo()
                {
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")), 0.25d),
                };
            }
        }
        public CellMarginsInfo GroupCellBorderMargins
        {
            get
            {
                return new CellMarginsInfo(new Thickness(1));
            }
        }
        public Brush SummaryCaptionBackground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDEDCDC"));
            }
        }

        public Brush SummaryCaptionForeground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));
            }
        }

        public GridFontInfo SummaryCaptionFont
        {
            get 
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe WP"),
                    FontSize = 13d,
                };
            }

        }

        public Brush SummaryRowBackground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD0D0D0"));
            }
        }

        public Brush SummaryRowForeground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));
            }
        }

        public GridFontInfo SummaryRowFont
        {
            get 
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe WP"),
                    FontSize = 13d,
                };
            }
        }

        public Brush RowHeaderSelectionBackground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9E9E9")); 
            }
        }

        public Brush RowHeaderForeground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));
            }
        }

        public Brush HighlightBrush
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDADADA"));
            }
        }

        public Brush CurrentCellBorderBrush
        {
            get 
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public double CurrentCellBorderWidth
        {
            get 
            {
                return 0.5d;
            }
        }

        public Brush HighlightSelectionBackground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2ABFF1"));
            }
        }

        public Brush HighlightSelectionForeground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush ColumnOptionsPopupBackground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush ColumnOptionsPopupForeground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF666666")); 
            }
        }

        public Brush ColumnOptionsButtonBackground
        {
            get 
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#00FFFFFF"));
            }
        }

        public Brush ColumnOptionsCloseButtonBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2ABFF1"));
            }
        }

        public Brush FilterButtonInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush FilterButtonOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#00FFFFFF"));
            }
        }

        public Brush FilterButtonHoverInnerBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#00FFFFFF"));
            }
        }

        public Brush FilterButtonHoverOuterBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#00FFFFFF"));
            }
        }

        public Brush FilterButtonAppliedBrush
        {
            get
            {
                return Brushes.WhiteSmoke;
            }
        }

        public Brush PlusMinusButtonBackground
        {
            get
            {
                return this.HeaderBackgroundBrush;
            }
        }

        public Brush PlusMinusButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB8BBBC"));
            }
        }

        public Brush PlusMinusButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));
            }
        }

        public Brush DragDropIndicatorBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6"));
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
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush AlternateRowBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF6F6F6"));
            }
        }

        public Brush HoveringRecordCellBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDADADA"));
            }
        }

        public Brush GroupCaptionSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC4C4C4"));
            }
        }

        public Brush GroupCaptionSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush CurrentCellSelectionBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2AABCF"));
            }
        }

        public Brush CurrentCellSelectionForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush HeaderOptionsHoverBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF67D9FF"));
            }
        }

        public Brush HeaderOptionsCheckedBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2AABCF"));
            }
        }

        public Brush HeaderOptionsBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#00FFFFFF"));
            }
        }

        public Brush PlusMinusExpandedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));
            }
        }

        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));
            }
        }

        public Brush PlusMinusExpandedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));
            }
        }

        public Brush PlusMinusHoverButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF41B1E1"));
            }
        }

        public Brush PlusMinusHoverButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFB8BBBC"));
            }
        }

        public Brush PlusMinusHoverButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
            }
        }

        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo NestedHeaderCellBorder
        {
            get
            {
                return this.HeaderCellBorders;
            }
        }

        public CellBordersInfo FirstHeaderColumnBorder
        {
            get
            {
                return this.ValueCellBorders;
            }
        }

        public CellBordersInfo LastHeaderColumnBorder
        {
            get 
            {
                return this.ValueCellBorders;
            }
        }
    }

    internal static class ColorExtensions
    {
        public static Color StringToColor(string hexaColor)
        {
            var color = Color.FromArgb(Convert.ToByte(hexaColor.Substring(1, 2), 16), Convert.ToByte(hexaColor.Substring(3, 2), 16), Convert.ToByte(hexaColor.Substring(5, 2), 16), Convert.ToByte(hexaColor.Substring(7, 2), 16));
            return color;
        }
    }

    internal static class Brushes
    {
        public static SolidColorBrush Black
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public static SolidColorBrush White
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public static SolidColorBrush WhiteSmoke
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF5F5F5"));
            }
        }

        public static SolidColorBrush Yellow
        {
            get
            {
                return new SolidColorBrush(Colors.Yellow);
            }
        }

        public static SolidColorBrush Maroon
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF800000"));
            }
        }

        public static SolidColorBrush MidnightBlue
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF191970"));
            }
        }

        public static SolidColorBrush AliceBlue
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF0F8FF"));
            }
        }

        public static SolidColorBrush Gray
        {
            get
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public static SolidColorBrush Transparent
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public static SolidColorBrush Red
        {
            get
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

    }
}